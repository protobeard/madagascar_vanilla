using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using MadagascarVanilla.Persistables;
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
        [CanBeNull] private string _scenarioName;
        [CanBeNull] private Scenario _scenario;
        [CanBeNull]
        public Scenario SelectedScenario
        {
            get
            {
                if (_scenario != null)
                    return _scenario;

                if (_scenarioName != null)
                {
                    // foreach (Scenario scenario in ScenarioLister.AllScenarios())
                    // {
                    //     Log.Message($"name: {scenario.name}, hashcode: {scenario.GetHashCode()}");
                    // }
                    
                    _scenario = ScenarioLister.AllScenarios().FirstOrDefault(scenario => scenario.name == _scenarioName);
                }

                return _scenario;
            }
            set
            {
                _scenario = value;
                _scenarioName = value?.name;
            }
        }
        
        // New Game Setup (Storyteller)
        [CanBeNull] private string _storytellerName;
        [CanBeNull] private StorytellerDef _storyteller;
        [CanBeNull]
        public StorytellerDef StorytellerDef
        {
            get
            {
                if (_storyteller != null)
                    return _storyteller;

                if (_storytellerName != null)
                    _storyteller = DefDatabase<StorytellerDef>.GetNamed(_storytellerName, false);

                return _storyteller;
            }
            set
            {
                _storyteller = value;
                _storytellerName = value?.defName;
            }
        }

        [CanBeNull] private string _difficultyDefName;
        [CanBeNull] private DifficultyDef _difficultyDef;
        [CanBeNull]
        public DifficultyDef DifficultyDef
        {
            get
            {
                if (_difficultyDef != null)
                    return _difficultyDef;
                
                if (_difficultyDefName != null)
                    _difficultyDef = DefDatabase<DifficultyDef>.GetNamed(_difficultyDefName, false);

                return _difficultyDef;
            }
            set
            {
                _difficultyDef = value;
                _difficultyDefName = value?.defName;
            }
        }

        [CanBeNull] private PersistableDifficulty _difficultyForPersisting;
        [CanBeNull] private Difficulty _difficulty;

        public Difficulty Difficulty
        {
            get
            {
                if (_difficulty != null)
                    return _difficulty;

                if (_difficultyForPersisting == null)
                {
                    _difficulty ??= new Difficulty();
                    _difficultyForPersisting ??= new PersistableDifficulty();
                }
                else
                {
                    _difficulty ??= _difficultyForPersisting.DifficultyFromSelf();
                }

                return _difficulty;
            }
            set
            {
                _difficulty = value;
                _difficultyForPersisting = new PersistableDifficulty(value);
            }
        }

        public bool Permadeath;

        // New Game Setup (World)
        [CanBeNull] private List<string> _factionDefNames;
        [CanBeNull] private List<FactionDef> _factionDefs;
        [CanBeNull]
        public List<FactionDef> Factions
        {
            get
            {
                if (_factionDefs != null)
                    return _factionDefs;

                if (_factionDefNames != null)
                {
                    _factionDefs ??= new List<FactionDef>();
                    _factionDefs.Clear();
                    foreach (string factionDefName in _factionDefNames)
                    {
                        if (MadagascarVanillaMod.Verbose()) Log.Message($"Restoring faction: {factionDefName}");

                        FactionDef faction = DefDatabase<FactionDef>.GetNamed(factionDefName, false);
                        if (!FactionGenerator.ConfigurableFactions.Contains(faction))
                        {
                            if (MadagascarVanillaMod.Verbose()) Log.Message($"Madagascar Vanilla: Unknown faction ({faction.defName})), skipping.");
                            continue;
                        }

                        _factionDefs.Add(faction);
                    }
                }
                
                return _factionDefs;
            }
            set
            {
                _factionDefs = value;
                _factionDefNames = value?.Select(faction => faction?.defName).ToList();
            }
        }
        
        public float? PlanetCoverage;
        public OverallRainfall? Rainfall;
        public OverallTemperature? Temperature;
        public OverallPopulation? Population;
        public LandmarkDensity? LandmarkDensity;
        public float? Pollution;
        public int? MapSize;
        public Season? StartingSeason;

        // New Game Setup (Ideology)
        
        // expected to be null unless PresetSelection is Preset
        [CanBeNull] private string _ideoligionDefName;
        [CanBeNull] private IdeoPresetDef _ideoligionDef;
        [CanBeNull] public IdeoPresetDef Ideoligion
        {
            get
            {
                if (_ideoligionDef != null)
                    return _ideoligionDef;
                
                if (_ideoligionDefName != null)
                    _ideoligionDef = DefDatabase<IdeoPresetDef>.GetNamed(_ideoligionDefName, false);

                return _ideoligionDef;
            }
            set
            {
                _ideoligionDef = value;
                _ideoligionDefName = value?.defName;
            }
        }
        
        public PresetSelectionType? PresetSelection;

        [CanBeNull] private string _structureDefName;
        [CanBeNull] private MemeDef _structureDef; 
        [CanBeNull] public MemeDef Structure
        {
            get
            {
                if (_structureDef != null)
                    return _structureDef;
                
                if (_structureDefName != null)
                    _structureDef = DefDatabase<MemeDef>.GetNamed(_structureDefName, false);

                return _structureDef;
            }
            set
            {
                _structureDef = value;
                _structureDefName = value?.defName;
            }
        }

        // Max of 3 styles
        [CanBeNull] private List<string> _styleCategoryDefNames;
        [CanBeNull] private List<StyleCategoryDef> _styleCategoryDefs;
        [CanBeNull] public List<StyleCategoryDef> StyleCategories
        {
            get
            {
                if (_styleCategoryDefs != null)
                    return _styleCategoryDefs;
                
                if (_styleCategoryDefNames != null)
                    _styleCategoryDefs = _styleCategoryDefNames.Select(styleCategoryDefName => DefDatabase<StyleCategoryDef>.GetNamed(styleCategoryDefName, false)).ToList();

                return _styleCategoryDefs;
            }
            set
            {
                _styleCategoryDefs = value;
                _styleCategoryDefNames = value?.Select(styleCategory => styleCategory?.defName).ToList();
            }
        }

        // Copy of private enum PresetSelection in Page_ChooseIdeoPreset
        // FIXME: clean this up/turn into a constant?
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
        public bool EnablePersistingFoodPolicies;
        public bool EnablePersistingReadingPolicies;
        public bool EnablePersistingDrugPolicies;
        public bool EnablePersistingMedicalSettings;

        private Dictionary<int, PersistableThingFilter> _apparelFilters;
        private List<ApparelPolicy> _apparelPoliciesForPersisting;
        private List<ApparelPolicy> _apparelPolicies;
        public List<ApparelPolicy> ApparelPolicies
        {
            get
            {
                if  (_apparelPolicies != null)
                    return _apparelPolicies;
                
                // lazy load: Is it the first time we've ever accessed _apparelPolicies, or do we have stuff saved?
                if (_apparelPoliciesForPersisting == null || _apparelPoliciesForPersisting.Count == 0)
                {
                    _apparelPolicies ??= new List<ApparelPolicy>();
                    _apparelPoliciesForPersisting ??= new List<ApparelPolicy>();
                    _apparelFilters ??= new Dictionary<int, PersistableThingFilter>();
                }
                else
                {
                    _apparelPolicies ??= new List<ApparelPolicy>();
                    _apparelPolicies.Clear();
                    foreach (ApparelPolicy apparelPolicy in _apparelPoliciesForPersisting)
                    {
                        if (MadagascarVanillaMod.Verbose()) Log.Message($"Restoring filter to {apparelPolicy.label}, {apparelPolicy.id}");
                        PersistableThingFilter persistableThingFilter = _apparelFilters[apparelPolicy.id];
                        if (persistableThingFilter != null)
                        {
                            apparelPolicy.filter = persistableThingFilter.ThingFilterFromSelf();
                            _apparelPolicies.Add(apparelPolicy);
                        }
                        else
                        {
                            Log.Error($"persistableThingFilter is null for {apparelPolicy.id}");
                        }
                    }
                }
                return _apparelPolicies;
            }
        }
        
        private Dictionary<int, PersistableThingFilter> _foodFilters;
        private List<FoodPolicy> _foodPoliciesForPersisting;
        private List<FoodPolicy> _foodPolicies;
        public List<FoodPolicy> FoodPolicies
        {
            get
            {
                if  (_foodPolicies != null)
                    return _foodPolicies;
                
                // lazy load: Is it the first time we've ever accessed _foodPolicies, or do we have stuff saved?
                if (_foodPoliciesForPersisting == null || _foodPoliciesForPersisting.Count == 0)
                {
                    _foodPolicies ??= new List<FoodPolicy>();
                    _foodPoliciesForPersisting ??= new List<FoodPolicy>();
                    _foodFilters ??= new Dictionary<int, PersistableThingFilter>();
                }
                else
                {
                    _foodPolicies ??= new List<FoodPolicy>();
                    _foodPolicies.Clear();
                    foreach (FoodPolicy foodPolicy in _foodPoliciesForPersisting)
                    {
                        if (MadagascarVanillaMod.Verbose()) Log.Message($"Restoring filter to {foodPolicy.label}, {foodPolicy.id}");
                        PersistableThingFilter persistableThingFilter = _foodFilters[foodPolicy.id];
                        if (persistableThingFilter != null)
                        {
                            foodPolicy.filter = persistableThingFilter.ThingFilterFromSelf();
                            _foodPolicies.Add(foodPolicy);
                        }
                        else
                        {
                            Log.Error($"persistableThingFilter is null for {foodPolicy.id}");
                        }
                    }
                }
                return _foodPolicies;
            }
        }
        
        private Dictionary<int, PersistableThingFilter> _readingDefFilters;
        private Dictionary<int, PersistableThingFilter> _readingEffectFilters;
        private List<ReadingPolicy> _readingPoliciesForPersisting;
        private List<ReadingPolicy> _readingPolicies;
        public List<ReadingPolicy> ReadingPolicies
        {
            get
            {
                if  (_readingPolicies != null)
                    return _readingPolicies;
                
                // lazy load: Is it the first time we've ever accessed _readingPolicies, or do we have stuff saved?
                if (_readingPoliciesForPersisting == null || _readingPoliciesForPersisting.Count == 0)
                {
                    _readingPolicies ??= new List<ReadingPolicy>();
                    _readingPoliciesForPersisting ??= new List<ReadingPolicy>();
                    _readingDefFilters ??= new Dictionary<int, PersistableThingFilter>();
                    _readingEffectFilters ??= new Dictionary<int, PersistableThingFilter>();
                }
                else
                {
                    _readingPolicies ??= new List<ReadingPolicy>();
                    _readingPolicies.Clear();
                    foreach (ReadingPolicy readingPolicy in _readingPoliciesForPersisting)
                    {
                        //ReadingPolicy rp = new ReadingPolicy(readingPolicy.id, readingPolicy.label);
                        
                        if (MadagascarVanillaMod.Verbose()) Log.Message($"Restoring filters to {readingPolicy.label}, {readingPolicy.id}");
                        PersistableThingFilter persistableThingDefFilter = _readingDefFilters[readingPolicy.id];
                        PersistableThingFilter persistableThingEffectFilter = _readingEffectFilters[readingPolicy.id];
                        
                        if (persistableThingDefFilter != null)
                            readingPolicy.defFilter = persistableThingDefFilter.ThingFilterFromSelf();
                        else
                            Log.Error($"persistableThingDefFilter is null for {readingPolicy.id}");
                        
                        if (persistableThingEffectFilter != null)
                            readingPolicy.effectFilter = persistableThingEffectFilter.ThingFilterFromSelf();
                        else
                            Log.Error($"persistableThingEffectFilter is null for {readingPolicy.id}");
                        
                        _readingPolicies.Add(readingPolicy);
                    }
                }
                return _readingPolicies;
            }
        }

        // private readonly List<string> _drugPolicyFieldsToPersist = new List<string>() {"sourceDefName", "entriesInt"};
        // private List<PersistableDictionary> _persistableDrugPolicies;
        
        private List<PersistableDrugPolicy> _drugPoliciesForPersisting;
        private List<DrugPolicy> _drugPolicies;
        public List<DrugPolicy> DrugPolicies
        {
            get
            {
                if (_drugPolicies != null)
                    return _drugPolicies;

                // lazy load: Is it the first time we've ever accessed _drugPolicies, or do we have stuff saved?
                if (_drugPoliciesForPersisting == null || _drugPoliciesForPersisting.Count == 0)
                {
                    _drugPolicies ??= new List<DrugPolicy>();
                    _drugPoliciesForPersisting ??= new List<PersistableDrugPolicy>();
                }
                else
                {
                    _drugPolicies ??= new List<DrugPolicy>();
                    _drugPolicies.Clear();
                    foreach (PersistableDrugPolicy noDefDrugPolicy in _drugPoliciesForPersisting)
                    {
                        if (MadagascarVanillaMod.Verbose()) Log.Message($"Restoring {noDefDrugPolicy.label}, {noDefDrugPolicy.id} into _drugPolicies");
                        DrugPolicy drugPolicy = noDefDrugPolicy.DrugPolicyFromSelf();
                        _drugPolicies.Add(drugPolicy);
                    }
                }

                return _drugPolicies;
            }
        }

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

        private Dictionary<ScheduleType, List<string>> _defaultSchedulesForPersisting;
        private Dictionary<ScheduleType, List<TimeAssignmentDef>> _defaultSchedulesDictionary;
        public Dictionary<ScheduleType, List<TimeAssignmentDef>> DefaultSchedulesDictionary
        {
            get
            {
                // We've already populated stuff
                if (_defaultSchedulesDictionary != null)
                    return _defaultSchedulesDictionary;
                
                // lazy load: Is it the first time we've ever accessed _defaulSchedulesDict, or do we have stuff saved?
                if (_defaultSchedulesForPersisting is null || _defaultSchedulesForPersisting.Count == 0)
                {
                    _defaultSchedulesDictionary = new Dictionary<ScheduleType, List<TimeAssignmentDef>>()
                    {
                        { ScheduleType.DayShift, GeneratePawnTimeAssignments(ScheduleType.DayShift) },
                        { ScheduleType.NightShift, GeneratePawnTimeAssignments(ScheduleType.NightShift) },
                        { ScheduleType.Biphasic, GeneratePawnTimeAssignments(ScheduleType.Biphasic) },
                        { ScheduleType.NeverSleep, GeneratePawnTimeAssignments(ScheduleType.NeverSleep) }
                    };
                }
                else
                {
                    _defaultSchedulesDictionary = new Dictionary<ScheduleType, List<TimeAssignmentDef>>(); 
                    foreach (var (key, value) in _defaultSchedulesForPersisting)
                    {
                        // Soft check for errors on Def lookup - could be modded content from a mod that has been
                        // removed. Replace with "Anything" on failed lookup.
                        List<TimeAssignmentDef> scheduleByHour = value.Select(defName => DefDatabase<TimeAssignmentDef>.GetNamed(defName, false) ?? TimeAssignmentDefOf.Anything).ToList();
                        _defaultSchedulesDictionary[key] = scheduleByHour;
                    }
                }
                return _defaultSchedulesDictionary;
            }
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

        private string _billStoreModeDefName;
        private BillStoreModeDef _billStoreModeDef;
        public BillStoreModeDef BillStoreMode
        {
            get
            {
                if (_billStoreModeDef != null)
                    return _billStoreModeDef;
                
                if (_billStoreModeDefName != null)
                    _billStoreModeDef = DefDatabase<BillStoreModeDef>.GetNamed(_billStoreModeDefName, false);

                return _billStoreModeDef;
            }
            set
            {
                _billStoreModeDef = value;
                _billStoreModeDefName = value?.defName;
            }
        }

        private string _billRepeatModeDefName;
        private BillRepeatModeDef _billRepeatModeDef;
        public BillRepeatModeDef BillRepeatMode
        {
            get
            {
                if (_billRepeatModeDef != null)
                    return _billRepeatModeDef;
                
                if (_billRepeatModeDefName != null)
                    _billRepeatModeDef = DefDatabase<BillRepeatModeDef>.GetNamed(_billRepeatModeDefName, false);

                return _billRepeatModeDef;
            }
            set
            {
                _billRepeatModeDef = value;
                _billRepeatModeDefName = value?.defName;
            }
        }
        
        // Only used if BillRepeatMode = TargetCount
        public IntRange HitpointRangeToCount = new IntRange(1, 100);
        
        // FIXME: Use QualityRange
        //QualityRange QualityRange = QualityRange.All;
        
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
        
        // Odyssey 
        public bool EnableShowExpandingLandmarks;
        
        // AutoCut (Wind Turbines and Animal Pens)
        public bool EnableAutoCut;
        
        private static Vector2 _scrollPosition;

        private int SettingsHeight()
        {
            const int numToggles = 52;
            const int numRadios = 22;
            int medicalCategories = Enum.GetNames(typeof(MedicalCareCategory)).Count(); // 11
            int scheduleTypes = Enum.GetNames(typeof(ScheduleType)).Count(); // 4
            const int areaListHeight = 15; // one text box
            const int hitpointRangeCountHeight = 15;
            const int ingredientRadiusHeight = 15;

            // FIXME: magic numbers
            return (int)(1500 + numToggles * 20 + numRadios * 20 + medicalCategories * MedicalRowHeight +
                         scheduleTypes * ScheduleRowHeight + areaListHeight + hitpointRangeCountHeight + ingredientRadiusHeight);
        }

        public void DoSettingsWindowContents(Rect rect)
        {
            Rect scrollRect = new Rect(rect.x, rect.y, rect.width - 20f, SettingsHeight());
            Rect drawRect = new(rect.x, rect.y, scrollRect.width - 20f, SettingsHeight() - 20f);
            
            Widgets.BeginScrollView(rect.BottomPart(0.9f).TopPart(0.9f), ref _scrollPosition, scrollRect);
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(drawRect);
            
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
            
            // TODO: Reset settings button
            
            Widgets.EndScrollView();
            listingStandard.End();
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
            Scribe_Values.Look(ref _scenarioName, "scenarioName");

            // Persist New Game Setup (Storyteller)
            Scribe_Values.Look(ref _storytellerName, "storytellerName");
            Scribe_Values.Look(ref _difficultyDefName, "difficultyDefName");

            if (Scribe.mode == LoadSaveMode.Saving)
            {
                if (Difficulty != null)
                {
                    _difficultyForPersisting = new PersistableDifficulty(Difficulty);
                }
            }
            Scribe_Deep.Look(ref _difficultyForPersisting, "difficultyForPersisting");
            
            Scribe_Values.Look(ref Permadeath, "permadeath");

            // Persist New Game Setup (World)
            Scribe_Collections.Look(ref _factionDefNames, "factionDefNames", LookMode.Value);
            Scribe_Values.Look(ref PlanetCoverage, "planetCoverage");
            Scribe_Values.Look(ref Rainfall, "rainfall");
            Scribe_Values.Look(ref Temperature, "temperature");
            Scribe_Values.Look(ref Population, "population");
            Scribe_Values.Look(ref LandmarkDensity, "landmarkDensity");
            Scribe_Values.Look(ref Pollution, "pollution");
            Scribe_Values.Look(ref MapSize, "mapSize");
            Scribe_Values.Look(ref StartingSeason, "startingSeason");

            // Persist New Game Setup (Ideoligion)
            Scribe_Values.Look(ref _ideoligionDefName, "ideoligionDefName");
            Scribe_Values.Look(ref PresetSelection, "presetSelection");
            Scribe_Values.Look(ref _structureDefName, "structureDefName");
            Scribe_Collections.Look(ref _styleCategoryDefNames, "styles", LookMode.Value);

            // Persist new game Setup (Misc)
            Scribe_Values.Look(ref EnableWorkPriorities, "enableWorkPriorities");
            Scribe_Values.Look(ref DefaultHostilityResponse, "defaultHostilityResponse");
            Scribe_Values.Look(ref DisableGoodwillRewards, "disableGoodwillRewards");
            Scribe_Values.Look(ref DisableRoyalFavorRewards, "disableRoyalFavorRewards");
            
            // Persist Policies
            Scribe_Values.Look(ref EnablePersistingApparelPolicies, "enablePersistingApparelPolicies");
            Scribe_Values.Look(ref EnablePersistingFoodPolicies, "enablePersistingFoodPolicies");
            Scribe_Values.Look(ref EnablePersistingReadingPolicies, "enablePersistingReadingPolicies");
            Scribe_Values.Look(ref EnablePersistingDrugPolicies, "enablePersistingDrugPolicies");
            Scribe_Values.Look(ref EnablePersistingMedicalSettings, "enablePersistingMedicalSettings");
            
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                if (ApparelPolicies.Any())
                {
                    _apparelPoliciesForPersisting.Clear();
                    _apparelFilters.Clear();
                    foreach (ApparelPolicy apparelPolicy in ApparelPolicies)
                    {
                        if (MadagascarVanillaMod.Verbose())  Log.Message($"Persisting apparel policy {apparelPolicy.label}");
                        _apparelFilters[apparelPolicy.id] = new PersistableThingFilter(apparelPolicy.filter);
                        
                        ApparelPolicy ap = new ApparelPolicy
                        {
                            id = apparelPolicy.id,
                            label = apparelPolicy.label
                        };
                        _apparelPoliciesForPersisting.Add(ap);
                    }
                }
            }
            
            Scribe_Collections.Look(ref _apparelFilters, "apparelFilters", LookMode.Value, LookMode.Deep);
            Scribe_Collections.Look(ref _apparelPoliciesForPersisting, "apparelPoliciesForPersisting", LookMode.Deep);
            
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                if (FoodPolicies.Any())
                {
                    _foodPoliciesForPersisting.Clear();
                    _foodFilters.Clear();
                    foreach (FoodPolicy foodPolicy in FoodPolicies)
                    {
                        if (MadagascarVanillaMod.Verbose()) Log.Message($"Persisting food policy {foodPolicy.label}");
                        _foodFilters[foodPolicy.id] = new PersistableThingFilter(foodPolicy.filter);

                        FoodPolicy foodPolicyToPersist = new FoodPolicy
                        {
                            id = foodPolicy.id,
                            label = foodPolicy.label
                        };
                        _foodPoliciesForPersisting.Add(foodPolicyToPersist);
                    }
                }
            }
            
            Scribe_Collections.Look(ref _foodFilters, "foodFilters", LookMode.Value, LookMode.Deep);
            Scribe_Collections.Look(ref _foodPoliciesForPersisting, "foodPoliciesForPersisting", LookMode.Deep);
            
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                if (ReadingPolicies.Any())
                {
                    _readingPoliciesForPersisting.Clear();
                    _readingDefFilters.Clear();
                    _readingEffectFilters.Clear();
                    foreach (ReadingPolicy readingPolicy in ReadingPolicies)
                    {
                        if (MadagascarVanillaMod.Verbose()) Log.Message($"Persisting reading policy {readingPolicy.label}");
                        _readingDefFilters[readingPolicy.id] = new PersistableThingFilter(readingPolicy.defFilter);
                        _readingEffectFilters[readingPolicy.id] = new PersistableThingFilter(readingPolicy.effectFilter);

                        ReadingPolicy readingPolicyToPersist = new ReadingPolicy
                        {
                            id = readingPolicy.id,
                            label = readingPolicy.label
                        };
                        _readingPoliciesForPersisting.Add(readingPolicyToPersist);
                    }
                }
            }
            
            Scribe_Collections.Look(ref _readingDefFilters, "readingDefFilters", LookMode.Value, LookMode.Deep);
            Scribe_Collections.Look(ref _readingEffectFilters, "readingEffectFilters", LookMode.Value, LookMode.Deep);
            Scribe_Collections.Look(ref _readingPoliciesForPersisting, "readingPoliciesForPersisting", LookMode.Deep);
            
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                if (DrugPolicies.Any())
                {
                    _drugPoliciesForPersisting.Clear();
                    foreach (DrugPolicy drugPolicy in DrugPolicies)
                    {
                        if (MadagascarVanillaMod.Verbose()) Log.Message($"Persisting drug policy {drugPolicy.label} from DrugPolicies");
                        PersistableDrugPolicy drugPolicyToPersist = new PersistableDrugPolicy(drugPolicy);
                        _drugPoliciesForPersisting.Add(drugPolicyToPersist);
                    }
                }
            }
            
            Scribe_Collections.Look(ref _drugPoliciesForPersisting, "drugPoliciesForPersisting", LookMode.Deep);

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
            
            if (Scribe.mode == LoadSaveMode.Saving)
            {            
                _defaultSchedulesForPersisting ??= new Dictionary<ScheduleType, List<string>>();
                foreach (var (key, value) in DefaultSchedulesDictionary)
                {
                    List<string> scheduleByHour = value.Select(timeAssignment => timeAssignment.defName).ToList();
                    _defaultSchedulesForPersisting[key] = scheduleByHour;
                }
            }
            
            Scribe_Collections.Look(ref _defaultSchedulesForPersisting, "defaultSchedulesDictionary", LookMode.Value, LookMode.Value);
            
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
            
            // Should be okay to store these as defs because I'm not supporting all modes,
            // only the chosen ones.
            Scribe_Values.Look(ref _billStoreModeDefName, "billStoreModeName");
            Scribe_Values.Look(ref _billRepeatModeDefName, "billRepeatModeName");
            
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
            
            // Persist Odyssey
            Scribe_Values.Look(ref EnableShowExpandingLandmarks, "enableShowExpandingLandmarks");
            
            // Persist AutoCut
            Scribe_Values.Look(ref EnableAutoCut, "enableAutoCut");
        }
    }
}