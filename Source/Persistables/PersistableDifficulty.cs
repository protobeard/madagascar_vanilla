using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
// ReSharper disable InconsistentNaming
// ReSharper disable RedundantArgumentDefaultValue

namespace MadagascarVanilla.Persistables
{
    public class PersistableDifficulty : IExposable
    {
        // TODO: remove defaults, no need for them.
        public float threatScale = 1f;
        public bool allowBigThreats = true;
        public bool allowIntroThreats = true;
        public bool allowCaveHives = true;
        public bool peacefulTemples;
        public bool allowViolentQuests = true;
        public bool babiesAreHealthy;
        public bool noBabiesOrChildren;
        public bool childRaidersAllowed;
        public bool childShamblersAllowed = true;
        public bool predatorsHuntHumanlikes = true;
        public float scariaRotChance;
        public float colonistMoodOffset;
        public float tradePriceFactorLoss;
        public float cropYieldFactor = 1f;
        public float mineYieldFactor = 1f;
        public float butcherYieldFactor = 1f;
        public float fishingYieldFactor = 1f;
        public float researchSpeedFactor = 1f;
        public float diseaseIntervalFactor = 1f;
        public float enemyReproductionRateFactor = 1f;
        public float playerPawnInfectionChanceFactor = 1f;
        public float manhunterChanceOnDamageFactor = 1f;
        public float deepDrillInfestationChanceFactor = 1f;
        public float wastepackInfestationChanceFactor = 1f;
        public float foodPoisonChanceFactor = 1f;
        public float maintenanceCostFactor = 1f;
        public float enemyDeathOnDownedChanceFactor = 1f;
        public float adaptationGrowthRateFactorOverZero = 1f;
        public float adaptationEffectFactor = 1f;
        public float questRewardValueFactor = 1f;
        public float raidLootPointsFactor = 1f;
        public bool allowTraps = true;
        public bool allowTurrets = true;
        public bool allowMortars = true;
        public bool classicMortars;
        public bool allowExtremeWeatherIncidents = true;
        public bool fixedWealthMode;
        public float fixedWealthTimeFactor = 1f;
        public float friendlyFireChanceFactor = 0.4f;
        public float allowInstantKillChance = 1f;
        public float nomadicMineableResourcesFactor = 1f;
        public float lowPopConversionBoost = 3f;
        public float minThreatPointsRangeCeiling;
        public float childAgingRate = 4f;
        public float adultAgingRate = 1f;
        public bool unwaveringPrisoners = true;
        public float anomalyThreatsInactiveFraction = 0.08f;
        public float anomalyThreatsActiveFraction = 0.3f;
        public float? overrideAnomalyThreatsFraction;
        public float studyEfficiencyFactor = 1f;
        
        // AnomalyPlaystyleDef
        public string anomalyPlaystyleDefName;

        public PersistableDifficulty() { }

        public PersistableDifficulty(Difficulty difficulty)
        {
            threatScale = difficulty.threatScale;
            allowBigThreats = difficulty.allowBigThreats;
            allowIntroThreats = difficulty.allowIntroThreats;
            allowCaveHives = difficulty.allowCaveHives;
            peacefulTemples = difficulty.peacefulTemples;
            allowViolentQuests = difficulty.allowViolentQuests;
            predatorsHuntHumanlikes = difficulty.predatorsHuntHumanlikes;
            scariaRotChance = difficulty.scariaRotChance;
            colonistMoodOffset = difficulty.colonistMoodOffset;
            tradePriceFactorLoss = difficulty.tradePriceFactorLoss;
            cropYieldFactor = difficulty.cropYieldFactor;
            mineYieldFactor = difficulty.mineYieldFactor;
            butcherYieldFactor = difficulty.butcherYieldFactor;
            fishingYieldFactor = difficulty.fishingYieldFactor;
            researchSpeedFactor = difficulty.researchSpeedFactor;
            diseaseIntervalFactor = difficulty.diseaseIntervalFactor;
            enemyReproductionRateFactor = difficulty.enemyReproductionRateFactor;
            playerPawnInfectionChanceFactor = difficulty.playerPawnInfectionChanceFactor;
            manhunterChanceOnDamageFactor = difficulty.manhunterChanceOnDamageFactor;
            deepDrillInfestationChanceFactor = difficulty.deepDrillInfestationChanceFactor;
            wastepackInfestationChanceFactor = difficulty.wastepackInfestationChanceFactor;
            nomadicMineableResourcesFactor = difficulty.nomadicMineableResourcesFactor;
            foodPoisonChanceFactor = difficulty.foodPoisonChanceFactor;
            maintenanceCostFactor = difficulty.maintenanceCostFactor;
            enemyDeathOnDownedChanceFactor = difficulty.enemyDeathOnDownedChanceFactor;
            adaptationGrowthRateFactorOverZero = difficulty.adaptationGrowthRateFactorOverZero;
            adaptationEffectFactor = difficulty.adaptationEffectFactor;
            questRewardValueFactor = difficulty.questRewardValueFactor;
            raidLootPointsFactor = difficulty.raidLootPointsFactor;
            allowTraps = difficulty.allowTraps;
            allowTurrets = difficulty.allowTurrets;
            allowMortars = difficulty.allowMortars;
            classicMortars = difficulty.classicMortars;
            allowExtremeWeatherIncidents = difficulty.allowExtremeWeatherIncidents;
            fixedWealthMode = difficulty.fixedWealthMode;
            fixedWealthTimeFactor = difficulty.fixedWealthTimeFactor;
            friendlyFireChanceFactor = difficulty.friendlyFireChanceFactor;
            allowInstantKillChance = difficulty.allowInstantKillChance;
            lowPopConversionBoost = difficulty.lowPopConversionBoost;
            minThreatPointsRangeCeiling = difficulty.minThreatPointsRangeCeiling;
            babiesAreHealthy = difficulty.babiesAreHealthy;
            noBabiesOrChildren = difficulty.noBabiesOrChildren;
            childAgingRate = difficulty.childAgingRate;
            adultAgingRate = difficulty.adultAgingRate;
            unwaveringPrisoners = difficulty.unwaveringPrisoners;
            childRaidersAllowed = difficulty.childRaidersAllowed;
            anomalyThreatsInactiveFraction = difficulty.anomalyThreatsInactiveFraction;
            anomalyThreatsActiveFraction = difficulty.anomalyThreatsActiveFraction;
            studyEfficiencyFactor = difficulty.studyEfficiencyFactor;
            
            anomalyPlaystyleDefName = difficulty.AnomalyPlaystyleDef.defName;
            
            SetMinThreatPointsCurve();
        }
        
        public Difficulty DifficultyFromSelf()
        {
            if (MadagascarVanillaMod.Verbose()) Log.Message($"Making Difficulty");
            Difficulty difficulty = new Difficulty();
            
            difficulty.threatScale = threatScale;
            difficulty.allowBigThreats = allowBigThreats;
            difficulty.allowIntroThreats = allowIntroThreats;
            difficulty.allowCaveHives = allowCaveHives;
            difficulty.peacefulTemples = peacefulTemples;
            difficulty.allowViolentQuests = allowViolentQuests;
            difficulty.predatorsHuntHumanlikes = predatorsHuntHumanlikes;
            difficulty.scariaRotChance = scariaRotChance;
            difficulty.colonistMoodOffset = colonistMoodOffset;
            difficulty.tradePriceFactorLoss = tradePriceFactorLoss;
            difficulty.cropYieldFactor = cropYieldFactor;
            difficulty.mineYieldFactor = mineYieldFactor;
            difficulty.butcherYieldFactor = butcherYieldFactor;
            difficulty.fishingYieldFactor = fishingYieldFactor;
            difficulty.researchSpeedFactor = researchSpeedFactor;
            difficulty.diseaseIntervalFactor = diseaseIntervalFactor;
            difficulty.enemyReproductionRateFactor = enemyReproductionRateFactor;
            difficulty.playerPawnInfectionChanceFactor = playerPawnInfectionChanceFactor;
            difficulty.manhunterChanceOnDamageFactor = manhunterChanceOnDamageFactor;
            difficulty.deepDrillInfestationChanceFactor = deepDrillInfestationChanceFactor;
            difficulty.wastepackInfestationChanceFactor = wastepackInfestationChanceFactor;
            difficulty.nomadicMineableResourcesFactor = nomadicMineableResourcesFactor;
            difficulty.foodPoisonChanceFactor = foodPoisonChanceFactor;
            difficulty.maintenanceCostFactor = maintenanceCostFactor;
            difficulty.enemyDeathOnDownedChanceFactor = enemyDeathOnDownedChanceFactor;
            difficulty.adaptationGrowthRateFactorOverZero = adaptationGrowthRateFactorOverZero;
            difficulty.adaptationEffectFactor = adaptationEffectFactor;
            difficulty.questRewardValueFactor = questRewardValueFactor;
            difficulty.raidLootPointsFactor = raidLootPointsFactor;
            difficulty.allowTraps = allowTraps;
            difficulty.allowTurrets = allowTurrets;
            difficulty.allowMortars = allowMortars;
            difficulty.classicMortars = classicMortars;
            difficulty.allowExtremeWeatherIncidents = allowExtremeWeatherIncidents;
            difficulty.fixedWealthMode = fixedWealthMode;
            difficulty.fixedWealthTimeFactor = fixedWealthTimeFactor;
            difficulty.friendlyFireChanceFactor = friendlyFireChanceFactor;
            difficulty.allowInstantKillChance = allowInstantKillChance;
            difficulty.lowPopConversionBoost = lowPopConversionBoost;
            difficulty.minThreatPointsRangeCeiling = minThreatPointsRangeCeiling;
            difficulty.babiesAreHealthy = babiesAreHealthy;
            difficulty.noBabiesOrChildren = noBabiesOrChildren;
            difficulty.childAgingRate = childAgingRate;
            difficulty.adultAgingRate = adultAgingRate;
            difficulty.unwaveringPrisoners = unwaveringPrisoners;
            difficulty.childRaidersAllowed = childRaidersAllowed;
            difficulty.anomalyThreatsInactiveFraction = anomalyThreatsInactiveFraction;
            difficulty.anomalyThreatsActiveFraction = anomalyThreatsActiveFraction;
            difficulty.studyEfficiencyFactor = studyEfficiencyFactor;
            
            if (anomalyPlaystyleDefName != null)
                difficulty.AnomalyPlaystyleDef = DefDatabase<AnomalyPlaystyleDef>.GetNamed(anomalyPlaystyleDefName);
            
            MethodInfo setMinThreatPointsCurveMethod = AccessTools.Method(typeof(Difficulty),"SetMinThreatPointsCurve");
            setMinThreatPointsCurveMethod.Invoke(difficulty, null);
            
            return difficulty;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref threatScale, "threatScale", 0f);
            Scribe_Values.Look(ref allowBigThreats, "allowBigThreats", defaultValue: false);
            Scribe_Values.Look(ref allowIntroThreats, "allowIntroThreats", defaultValue: false);
            Scribe_Values.Look(ref allowCaveHives, "allowCaveHives", defaultValue: false);
            Scribe_Values.Look(ref peacefulTemples, "peacefulTemples", defaultValue: false);
            Scribe_Values.Look(ref allowViolentQuests, "allowViolentQuests", defaultValue: false);
            Scribe_Values.Look(ref babiesAreHealthy, "babiesAreHealthy", defaultValue: false);
            Scribe_Values.Look(ref noBabiesOrChildren, "noBabiesOrChildren", defaultValue: false);
            Scribe_Values.Look(ref predatorsHuntHumanlikes, "predatorsHuntHumanlikes", defaultValue: false);
            Scribe_Values.Look(ref scariaRotChance, "scariaRotChance", 0f);
            Scribe_Values.Look(ref colonistMoodOffset, "colonistMoodOffset", 0f);
            Scribe_Values.Look(ref tradePriceFactorLoss, "tradePriceFactorLoss", 0f);
            Scribe_Values.Look(ref cropYieldFactor, "cropYieldFactor", 0f);
            Scribe_Values.Look(ref mineYieldFactor, "mineYieldFactor", 0f);
            Scribe_Values.Look(ref butcherYieldFactor, "butcherYieldFactor", 0f);
            Scribe_Values.Look(ref fishingYieldFactor, "fishingYieldFactor", 1f);
            Scribe_Values.Look(ref researchSpeedFactor, "researchSpeedFactor", 0f);
            Scribe_Values.Look(ref diseaseIntervalFactor, "diseaseIntervalFactor", 0f);
            Scribe_Values.Look(ref enemyReproductionRateFactor, "enemyReproductionRateFactor", 0f);
            Scribe_Values.Look(ref playerPawnInfectionChanceFactor, "playerPawnInfectionChanceFactor", 0f);
            Scribe_Values.Look(ref manhunterChanceOnDamageFactor, "manhunterChanceOnDamageFactor", 0f);
            Scribe_Values.Look(ref deepDrillInfestationChanceFactor, "deepDrillInfestationChanceFactor", 0f);
            Scribe_Values.Look(ref wastepackInfestationChanceFactor, "wastepackInfestationChanceFactor", 0f);
            Scribe_Values.Look(ref nomadicMineableResourcesFactor, "nomadicMineableResourcesFactor", 1f);
            Scribe_Values.Look(ref foodPoisonChanceFactor, "foodPoisonChanceFactor", 0f);
            Scribe_Values.Look(ref maintenanceCostFactor, "maintenanceCostFactor", 0f);
            Scribe_Values.Look(ref enemyDeathOnDownedChanceFactor, "enemyDeathOnDownedChanceFactor", 0f);
            Scribe_Values.Look(ref adaptationGrowthRateFactorOverZero, "adaptationGrowthRateFactorOverZero", 0f);
            Scribe_Values.Look(ref adaptationEffectFactor, "adaptationEffectFactor", 0f);
            Scribe_Values.Look(ref questRewardValueFactor, "questRewardValueFactor", 0f);
            Scribe_Values.Look(ref raidLootPointsFactor, "raidLootPointsFactor", 1f);
            Scribe_Values.Look(ref allowTraps, "allowTraps", defaultValue: true);
            Scribe_Values.Look(ref allowTurrets, "allowTurrets", defaultValue: true);
            Scribe_Values.Look(ref allowMortars, "allowMortars", defaultValue: true);
            Scribe_Values.Look(ref classicMortars, "classicMortars", defaultValue: true);
            Scribe_Values.Look(ref allowExtremeWeatherIncidents, "allowExtremeWeatherIncidents", defaultValue: true);
            Scribe_Values.Look(ref fixedWealthMode, "fixedWealthMode", defaultValue: false);
            Scribe_Values.Look(ref fixedWealthTimeFactor, "fixedWealthTimeFactor", 1f);
            Scribe_Values.Look(ref friendlyFireChanceFactor, "friendlyFireChanceFactor", 0.4f);
            Scribe_Values.Look(ref allowInstantKillChance, "allowInstantKillChance", 1f);
            Scribe_Values.Look(ref lowPopConversionBoost, "lowPopConversionBoost", 3f);
            Scribe_Values.Look(ref minThreatPointsRangeCeiling, "minThreatPointsRangeCeiling", 70f);
            Scribe_Values.Look(ref adultAgingRate, "adultAgingRate", 1f);
            Scribe_Values.Look(ref childAgingRate, "childAgingRate", 4f);
            Scribe_Values.Look(ref unwaveringPrisoners, "unwaveringPrisoners", defaultValue: false);
            Scribe_Values.Look(ref childRaidersAllowed, "childRaidersAllowed", defaultValue: true);
            Scribe_Values.Look(ref anomalyThreatsInactiveFraction, "anomalyThreatsInactiveFraction", 0.08f);
            Scribe_Values.Look(ref anomalyThreatsActiveFraction, "anomalyThreatsActiveFraction", 0.3f);
            Scribe_Values.Look(ref overrideAnomalyThreatsFraction, "overrideAnomalyThreatsFraction");
            Scribe_Values.Look(ref studyEfficiencyFactor, "studyEfficiencyFactor", 1f);
            
            //Scribe_Defs.Look(ref anomalyPlaystyleDef, "anomalyPlaystyleDef");
            Scribe_Values.Look(ref anomalyPlaystyleDefName, "anomalyPlaystyleDefName");
            
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                maintenanceCostFactor = Mathf.Max(0.01f, maintenanceCostFactor);
                SetMinThreatPointsCurve();
            }
            // if (Scribe.mode == LoadSaveMode.PostLoadInit && ModsConfig.AnomalyActive && anomalyPlaystyleDef == null)
            // {
            //     anomalyPlaystyleDef = AnomalyPlaystyleDefOf.Standard;
            // }
        }

        // copied over from Difficulty
        private void SetMinThreatPointsCurve()
        {
            if (minThreatPointsRangeCeiling < 35f)
            {
                Log.Warning($"Min threat points range ceiling is below {35f}, resetting to {70f}");
                minThreatPointsRangeCeiling = 70f;
            }
            // minThreatPointsRangeCeilingCurveCached = new SimpleCurve
            // {
            //     new CurvePoint(12f, 35f),
            //     new CurvePoint(35f, minThreatPointsRangeCeiling)
            // };
        }
    }
}