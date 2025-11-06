using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace MadagascarVanilla.Settings
{
    public partial class MadagascarVanillaPersistables : ModSettings
    {
        // General Mod Settings
        public bool Verbose;
        
        // Bug Fixes
        public bool DisableRemovingApparelWhileBleeding;
        public bool EnableCompMilkableDisplayProperItem;
        
        // UI
        public bool DisableLearningHelperButton;
        public bool EnableTraitsInOutFitAssignmentRow;
        
        // Alerts
        public bool EnableSleepingAloneAlert;
        
        // New Game Setup
        public bool EnablePersistingNewGameSetup;
        
        // New Game Setup (Scenario)
        [CanBeNull] public Scenario SelectedScenario;
        
        // New Game Setup (Storyteller)
        private StorytellerDef _storyteller;
        private DifficultyDef _difficultyDef;
        private Difficulty _difficulty;

        public StorytellerDef StorytellerDef
        {
            get => _storyteller;
            set => _storyteller = value;
        }

        public DifficultyDef DifficultyDef
        {
            get => _difficultyDef;
            set => _difficultyDef = value;
        }

        public Difficulty Difficulty
        {
            get => _difficulty ??= new Difficulty();
            set => _difficulty = value;
        }

        public bool Permadeath;

        // New Game Setup (World)
        [CanBeNull] public List<FactionDef> Factions;
        public float? PlanetCoverage;
        public OverallRainfall? Rainfall;
        public OverallTemperature? Temperature;
        public OverallPopulation? Population;
        public LandmarkDensity? LandmarkDensity;
        public float? Pollution;
        public int? MapSize;
        public Season? StartingSeason;

        // New Game Setup (Ideology)
        [CanBeNull] public IdeoPresetDef Ideoligion; // expected to be null unless PresetSelection is Preset
        public PresetSelectionType? PresetSelection;
        [CanBeNull] public MemeDef Structure;
        [CanBeNull] public List<StyleCategoryDef> StyleCategories; // Max of 3 styles

        // Copy of private enum PresetSelection in Page_ChooseIdeoPreset
        public enum PresetSelectionType
        {
            Classic,
            CustomFluid,
            CustomFixed,
            Load,
            Preset
        }
        
        // New Game Setup (Misc)
        public bool EnableWorkPriorities;
        public bool DisableGoodwillRewards;
        public bool DisableRoyalFavorRewards;
        public HostilityResponseMode DefaultHostilityResponse = HostilityResponseMode.Flee;
        
        // Policies
        public bool EnablePersistingApparelPolicies;
        public bool EnablePersistingDrugPolicies;
        public bool EnablePersistingFoodPolicies;
        public bool EnablePersistingReadingPolicies;
        public bool EnablePersistingMedicalSettings;
        
        private List<ApparelPolicy> _apparelPolicies;
        private List<DrugPolicy> _drugPolicies;
        private List<FoodPolicy> _foodPolicies;
        private List<ReadingPolicy> _readingPolicies;

        public List<ApparelPolicy> ApparelPolicies => _apparelPolicies ??= new List<ApparelPolicy>();
        public List<DrugPolicy> DrugPolicies => _drugPolicies ??= new List<DrugPolicy>();
        public List<FoodPolicy> FoodPolicies => _foodPolicies ??= new List<FoodPolicy>();
        public List<ReadingPolicy> ReadingPolicies => _readingPolicies ??= new List<ReadingPolicy>();

        public MedicalCareCategory ColonistMedicalDefault = MedicalCareCategory.Best;
        public MedicalCareCategory PrisonerMedicalDefault = MedicalCareCategory.HerbalOrWorse;
        public MedicalCareCategory SlaveMedicalDefault = MedicalCareCategory.HerbalOrWorse;
        public MedicalCareCategory GhoulMedicalDefault = MedicalCareCategory.NoMeds;
        public MedicalCareCategory TamedAnimalMedicalDefault = MedicalCareCategory.HerbalOrWorse;
        public MedicalCareCategory FriendlyMedicalDefault = MedicalCareCategory.HerbalOrWorse;
        public MedicalCareCategory NeutralMedicalDefault = MedicalCareCategory.HerbalOrWorse;
        public MedicalCareCategory HostileMedicalDefault = MedicalCareCategory.HerbalOrWorse;
        public MedicalCareCategory NoFactionMedicalDefault = MedicalCareCategory.HerbalOrWorse;
        public MedicalCareCategory WildlifeMedicalDefault = MedicalCareCategory.HerbalOrWorse;
        public MedicalCareCategory EntityMedicalDefault = MedicalCareCategory.NoMeds;
        
        // Schedules
        public bool EnableBodyMasterySchedule;
        public bool EnableNeverSleepGeneSchedule;
        public bool EnableNightOwlSchedule;
        public bool EnableUVSensitiveSchedule;
        public bool EnableSleepyGeneSchedule;
        public bool EnableInitialSchedule;
        
        public enum ScheduleType
        {
            DayShift,
            NightShift,
            Biphasic,
            NeverSleep
        }

        private Dictionary<ScheduleType, List<TimeAssignmentDef>> _defaultSchedulesDictionary;

        public Dictionary<ScheduleType, List<TimeAssignmentDef>> DefaultSchedulesDictionary
        {
            get => _defaultSchedulesDictionary ??= new Dictionary<ScheduleType, List<TimeAssignmentDef>>()
            {
                { ScheduleType.DayShift, GeneratePawnTimeAssignments(ScheduleType.DayShift) },
                { ScheduleType.NightShift, GeneratePawnTimeAssignments(ScheduleType.NightShift) },
                { ScheduleType.Biphasic, GeneratePawnTimeAssignments(ScheduleType.Biphasic) },
                { ScheduleType.NeverSleep, GeneratePawnTimeAssignments(ScheduleType.NeverSleep) }
            };
            set => _defaultSchedulesDictionary = value;
        }

        // this should only be run if there are no timetables in the settings file.
        private List<TimeAssignmentDef> GeneratePawnTimeAssignments(ScheduleType type)
        {
            Pawn_TimetableTracker timetable = new Pawn_TimetableTracker(null);
            SetDefaultSchedule(timetable, type);

            return timetable.times;
        }
        
        // Areas
        public bool DisableAutoHomeArea;
        public bool EnableAutoRebuildInHomeArea;
        public List<string> StartingAreasList = new List<string>();
        
        // Storage (Shelf-like)
        public bool EnableClearShelfStorage;
        public bool EnableClearBookcaseStorage;
        public bool EnableClearOutfitStandStorage;
        public bool EnableClearHopperStorage;

        // Storage Special (Shelf-like)
        public bool DisableRottenShelfStorage;
        public bool DisableDeadmansShelfStorage;
        public bool DisableBiocodedShelfStorage;
        
        // Storage Special (Stockpile)
        public bool DisableRottenStockpileStorage;
        public bool DisableDeadmansStockpileStorage;
        public bool DisableBiocodedStockpileStorage;
        
        // Storage Special (Dumping Stockpile)
        public bool DisableRottenDumpingStockpileStorage;
        
        // Production Bills (General)
        public bool EnableProductionSpecialistOnlyBillAssignment;
        public bool EnableInspiredOnlyBillAssignment;
        
        public BillStoreModeDef BillStoreMode;
        public BillRepeatModeDef BillRepeatMode;
        
        // Only used if BillRepeatMode = TargetCount
        public IntRange HitpointRangeToCount = new IntRange(1, 100);
        public QualityCategory MinQualityToCount = QualityCategory.Awful;
        public QualityCategory MaxQualityToCount = QualityCategory.Legendary;

        public int IngredientSearchRadius = 999;

        // Production Bills (Tailor)
        public bool DisableClothTextile;
        public bool DisableValuableTextiles;
        public bool DisableMoodImpactingTextiles;
        
        // Production Bills (Crematorium)
        public bool DisableColonistCremation;
        
        // Prisoners
        public bool EnableAutoStrip;
        public bool EnableAutoStripArrestedColonist;
        
        // Mechanitor
        public bool EnableMechRepair;
        public bool EnableNonflammableMechResourceChips; // FIXME: not strictly QOL
        
        // Odyssey // FIXME: not strictly QOL
        public bool EnableShowExpandingLandmarks;
        public int AllowXGravshipSubquests = 3;
        public int OdysseyQuestExtensionMultiplier = 1;
        public IntRange OdysseyQuestRangeExtender = new IntRange(1, 3);
        
        // Plants // FIXME: not QOL
        public bool EnableHydroponicDevilstrand;
        public bool EnableFalloutImmuneDevilstrand;
        public bool EnableFalloutImmuneToxipotatos;
        public int GauralenPruningSpeedMultiplier = 1;
        
        // AutoCut (Wind Turbines and Animal Pens)
        public bool EnableAutoCut;
        
        private static Vector2 settingsPosition = new();
        
        public void DoSettingsWindowContents(Rect rect)
        {
            // FIXME: this is a hack -- scrollView doesn't get a scrollbar
            Rect scrollRect = new Rect(0, 0, rect.width - 20f, 9999f);
            Rect drawRect = new(0f, 0f, scrollRect.width, 9999f);
            
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(drawRect);
            Widgets.BeginScrollView(rect.BottomPartPixels(rect.height - 40), ref settingsPosition, scrollRect);
            
            listingStandard.Label("MV_MadagascarVanillaRestartWarning".Translate());
            
            listingStandard.GapLine();
            listingStandard.Gap();
            
            listingStandard.CheckboxLabeled("MV_VerboseModeLabel".Translate(), ref Verbose, "MV_VerboseModeTooltip".Translate());
            
            listingStandard.GapLine();
            listingStandard.Gap();
            
            DoBugFixSettingsContent(rect, listingStandard);
            listingStandard.GapLine();
            listingStandard.Gap();
            
            DoUISettingsContent(rect, listingStandard);
            listingStandard.GapLine();
            listingStandard.Gap();
            
            DoNewGameConfigurationSettingsContent(rect, listingStandard);
            listingStandard.GapLine();
            listingStandard.Gap();
            
            DoPolicyPersistenceSettingsContent(rect, listingStandard);
            listingStandard.GapLine();
            listingStandard.Gap();
            
            DoScheduleSettingsContent(rect, listingStandard);
            listingStandard.GapLine();
            listingStandard.Gap();
            
            DoAreaSettingsContent(rect, listingStandard);
            listingStandard.GapLine();
            listingStandard.Gap();
            
            DoStorageDefaultSettingsContent(rect, listingStandard);
            listingStandard.GapLine();
            listingStandard.Gap();
            
            DoProductionBillSettingsContent(rect, listingStandard);
            listingStandard.GapLine();
            listingStandard.Gap();
            
            DoPrisonerSettingsContent(rect, listingStandard);
            listingStandard.GapLine();
            listingStandard.Gap();
            
            DoMechanitorSettingsContent(rect, listingStandard);
            listingStandard.GapLine();
            listingStandard.Gap();
            
            DoOdysseyQuestSettingsContent(rect, listingStandard);
            listingStandard.GapLine();
            listingStandard.Gap();
            
            DoPlantSettingsContent(rect, listingStandard);
            listingStandard.GapLine();
            listingStandard.Gap();
            
            // TODO: Reset settings button
            
            Widgets.EndScrollView();
            listingStandard.End();
            //this.Write();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            
            // General Mod Settings
            Scribe_Values.Look(ref Verbose, "verboseMode");
            
            // Bug Fixes
            Scribe_Values.Look(ref DisableRemovingApparelWhileBleeding, "disableRemovingApparelWhileBleeding");
            Scribe_Values.Look(ref EnableCompMilkableDisplayProperItem, "enableCompMilkableDisplayProperItem");

            // UI
            Scribe_Values.Look(ref DisableLearningHelperButton, "disableLearningHelperButton");
            Scribe_Values.Look(ref EnableTraitsInOutFitAssignmentRow, "enableTraitsInOutFitAssignmentRow");
            
            // Alerts
            Scribe_Values.Look(ref EnableSleepingAloneAlert, "enableSleepingAloneAlert");
            
            // New Game Setup
            Scribe_Values.Look(ref EnablePersistingNewGameSetup, "enablePersistingNewGameSetup");
            
            // Persist New Game Setup (Scenario)
            Scribe_Deep.Look(ref SelectedScenario, "scenario");

            // Persist New Game Setup (Storyteller)
            Scribe_Defs.Look(ref _storyteller, "storyteller");
            Scribe_Defs.Look(ref _difficultyDef, "difficultyDef");
            Scribe_Deep.Look(ref _difficulty, "difficulty");
            Scribe_Values.Look(ref Permadeath, "permadeath");

            // Persist New Game Setup (World)
            Scribe_Collections.Look(ref Factions, "factions", LookMode.Def);
            Scribe_Values.Look(ref PlanetCoverage, "planetCoverage");
            Scribe_Values.Look(ref Rainfall, "rainfall");
            Scribe_Values.Look(ref Temperature, "temperature");
            Scribe_Values.Look(ref Population, "population");
            Scribe_Values.Look(ref LandmarkDensity, "landmarkDensity");
            Scribe_Values.Look(ref Pollution, "pollution");
            Scribe_Values.Look(ref MapSize, "mapSize");
            Scribe_Values.Look(ref StartingSeason, "startingSeason");

            // Persist New Game Setup (Ideoligion)
            Scribe_Defs.Look(ref Ideoligion, "ideoligion");
            Scribe_Values.Look(ref PresetSelection, "presetSelection");
            Scribe_Defs.Look(ref Structure, "structure");
            Scribe_Collections.Look(ref StyleCategories, "styles", LookMode.Def);

            // Persist new game Setup (Misc)
            Scribe_Values.Look(ref EnableWorkPriorities, "enableWorkPriorities");
            Scribe_Values.Look(ref DefaultHostilityResponse, "defaultHostilityResponse");
            Scribe_Values.Look(ref DisableGoodwillRewards, "disableGoodwillRewards");
            Scribe_Values.Look(ref DisableRoyalFavorRewards, "disableRoyalFavorRewards");
            
            // Persist Policies
            Scribe_Values.Look(ref EnablePersistingApparelPolicies, "enablePersistingApparelPolicies");
            Scribe_Values.Look(ref EnablePersistingDrugPolicies, "enablePersistingDrugPolicies");
            Scribe_Values.Look(ref EnablePersistingFoodPolicies, "enablePersistingFoodPolicies");
            Scribe_Values.Look(ref EnablePersistingReadingPolicies, "enablePersistingReadingPolicies");
            Scribe_Values.Look(ref EnablePersistingMedicalSettings, "enablePersistingMedicalSettings");
            
            Scribe_Collections.Look(ref _apparelPolicies, "apparelPolicies", LookMode.Deep);
            Scribe_Collections.Look(ref _drugPolicies, "drugPolicies", LookMode.Deep);
            Scribe_Collections.Look(ref _foodPolicies, "foodPolicies", LookMode.Deep);
            Scribe_Collections.Look(ref _readingPolicies, "readingPolicies", LookMode.Deep);
            
            Scribe_Values.Look(ref ColonistMedicalDefault, "colonistMedicalDefault");
            Scribe_Values.Look(ref PrisonerMedicalDefault, "prisonerMedicalDefault");
            Scribe_Values.Look(ref SlaveMedicalDefault, "slaveMedicalDefault");
            Scribe_Values.Look(ref GhoulMedicalDefault, "ghoulMedicalDefault");
            Scribe_Values.Look(ref TamedAnimalMedicalDefault, "tamedAnimalMedicalDefault");
            Scribe_Values.Look(ref FriendlyMedicalDefault, "friendlyMedicalDefault");
            Scribe_Values.Look(ref NeutralMedicalDefault, "neutralMedicalDefault");
            Scribe_Values.Look(ref HostileMedicalDefault, "hostileMedicalDefault");
            Scribe_Values.Look(ref NoFactionMedicalDefault, "noFactionMedicalDefault");
            Scribe_Values.Look(ref WildlifeMedicalDefault, "wildlifeMedicalDefault");
            Scribe_Values.Look(ref EntityMedicalDefault, "entityMedicalDefault");

            // Persist Schedule Defaults
            Scribe_Values.Look(ref EnableBodyMasterySchedule, "enableBodyMasterySchedule");
            Scribe_Values.Look(ref EnableNeverSleepGeneSchedule, "enableNeverSleepGeneSchedule");
            Scribe_Values.Look(ref EnableNightOwlSchedule, "enableNightOwlSchedule");
            Scribe_Values.Look(ref EnableUVSensitiveSchedule, "enableUVSensitiveSchedule");
            Scribe_Values.Look(ref EnableSleepyGeneSchedule, "enableSleepyGeneSchedule");
            Scribe_Values.Look(ref EnableInitialSchedule, "enableInitialSchedule");
            
            Scribe_Collections.Look(ref _defaultSchedulesDictionary, "defaultSchedulesDictionary", LookMode.Value, LookMode.Def);
            
            // Persist Areas
            Scribe_Values.Look(ref DisableAutoHomeArea, "disableAutoHomeArea");
            Scribe_Values.Look(ref EnableAutoRebuildInHomeArea, "enableAutoRebuildInHomeArea");
            
            Scribe_Collections.Look(ref StartingAreasList, "startingAreasList");
            
            // Persist Storage (Shelf-like)
            Scribe_Values.Look(ref EnableClearShelfStorage, "enableClearShelfStorage");
            Scribe_Values.Look(ref EnableClearBookcaseStorage, "enableClearBookcaseStorage");
            Scribe_Values.Look(ref EnableClearOutfitStandStorage, "enableClearOutfitStandStorage");
            Scribe_Values.Look(ref EnableClearHopperStorage, "enableClearHopperStorage");
            
            // Persist Storage Special (Shelf-like)
            Scribe_Values.Look(ref DisableRottenShelfStorage, "disableRottenShelfStorage");
            Scribe_Values.Look(ref DisableDeadmansShelfStorage, "disableDeadmansShelfStorage");
            Scribe_Values.Look(ref DisableBiocodedShelfStorage, "disableBiocodedShelfStorage");
            
            // Persist Storage Special (Stockpile)
            Scribe_Values.Look(ref DisableRottenStockpileStorage, "disableRottenStockpileStorage");
            Scribe_Values.Look(ref DisableDeadmansStockpileStorage, "disableDeadmansStockpileStorage");
            Scribe_Values.Look(ref DisableBiocodedStockpileStorage, "disableBiocodedStockpileStorage");
            
            // Persist Storage Special (Dumping Stockpile)
            Scribe_Values.Look(ref DisableRottenDumpingStockpileStorage, "disableRottenDumpingStockpileStorage");
            
            // Persist Production Bills (General)
            Scribe_Values.Look(ref EnableProductionSpecialistOnlyBillAssignment, "enableProductionSpecialistOnlyBillAssignment");
            Scribe_Values.Look(ref EnableInspiredOnlyBillAssignment, "enableInspiredOnlyBillAssignment");
            
            Scribe_Defs.Look(ref BillStoreMode, "billStoreMode");
            Scribe_Defs.Look(ref BillRepeatMode, "billRepeatMode");
            
            Scribe_Values.Look(ref HitpointRangeToCount, "hitpointRangeToCount");
            Scribe_Values.Look(ref MinQualityToCount, "minQualityToCount");
            Scribe_Values.Look(ref MaxQualityToCount, "maxQualityToCount");
            
            Scribe_Values.Look(ref IngredientSearchRadius, "ingredientSearchRadius");
            
            // Persist Production Bills (Tailor)
            Scribe_Values.Look(ref DisableClothTextile, "disableClothTextile");
            Scribe_Values.Look(ref DisableValuableTextiles, "disableValuableTextiles");
            Scribe_Values.Look(ref DisableMoodImpactingTextiles, "disableMoodImpactingTextiles");
            
            // Persist Production Bills (Crematorium)
            Scribe_Values.Look(ref DisableColonistCremation, "disableColonistCremation");
            
            // Persist Prisoners
            Scribe_Values.Look(ref EnableAutoStrip, "enableAutoStrip");
            Scribe_Values.Look(ref EnableAutoStripArrestedColonist, "enableAutoStripArrestedColonist");
            
            // Persist Mechanitor
            Scribe_Values.Look(ref EnableMechRepair, "enableMechRepair");
            Scribe_Values.Look(ref EnableNonflammableMechResourceChips, "enableNonflammableMechResourceChips");
            
            // Persist Odyssey
            Scribe_Values.Look(ref EnableShowExpandingLandmarks, "enableShowExpandingLandmarks");
            Scribe_Values.Look(ref AllowXGravshipSubquests, "allowXGravshipSubquests");
            Scribe_Values.Look(ref OdysseyQuestExtensionMultiplier, "odysseyQuestExtensionMultiplier");
            Scribe_Values.Look(ref OdysseyQuestRangeExtender, "odysseyQuestRangeExtender");
            
            // Persist Plants
            Scribe_Values.Look(ref EnableHydroponicDevilstrand, "enableHydroponicDevilstrand");
            Scribe_Values.Look(ref EnableFalloutImmuneDevilstrand, "enableFalloutImmuneDevilstrand");
            Scribe_Values.Look(ref EnableFalloutImmuneToxipotatos, "enableFalloutImmuneToxipotatos");
            Scribe_Values.Look(ref GauralenPruningSpeedMultiplier, "gauralenPruningSpeedMultiplier");
            
            // Persist AutoCut
            Scribe_Values.Look(ref EnableAutoCut, "enableAutoCut");
        }
    }
}