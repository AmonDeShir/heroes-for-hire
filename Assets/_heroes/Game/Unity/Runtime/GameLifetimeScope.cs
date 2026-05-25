using Heroes.Content.Buildings;
using Heroes.Content.Heroes;
using Heroes.Game.Buildings;
using Heroes.Game.Core;
using Heroes.Game.AI;
using Heroes.Game.Heroes;
using Heroes.Game.Core.Events;
using Heroes.Presentation.UI.BuildingPanel;
using Heroes.Presentation.UI.HeroesPanel;
using Heroes.Presentation.UI.QuestPanel;
using Heroes.Presentation.UI.Input;
using Heroes.Presentation.UI.GameEnd;
using Heroes.Presentation.UI.ResourcesPanel;
using Heroes.Presentation.UI.SelectionPanel;
using Heroes.Game.Quests;
using Heroes.Game.Monsters;
using EventBus;
using Registry;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Heroes.Game.Runtime
{
    public class GameLifetimeScope : LifetimeScope
    {
        [Header("Content")]
        [SerializeField] private BuildingCatalog buildingCatalog;
        [SerializeField] private ItemCatalog itemCatalog;

        [Header("Runtime")]
        [SerializeField] private GameWorldStateManager gameWorldStateManager;
        [SerializeField] private BuildingIncomeService buildingIncomeService;

        [Header("Controllers")]
        [SerializeField] private BuildingPlacementController placementController;
        [SerializeField] private SelectionController selectionController;
        [SerializeField] private QuestController questController;
        [SerializeField] private BuildingCursor buildingCursor;
        [SerializeField] private CheatsController cheatsController;

        [Header("UI")]
        [SerializeField] private BuildingPanelPresenter buildingPanelPresenter;
        [SerializeField] private KingdomResourcesPresenter kingdomResourcesPresenter;
        [SerializeField] private SelectionPanelPresenter selectionPanelPresenter;
        [SerializeField] private HeroesPanelPresenter heroesPanelPresenter;
        [SerializeField] private QuestPanelPresenter questPanelPresenter;
        [SerializeField] private UiInputGatePresenter uiInputGatePresenter;
        [SerializeField] private GameEndPresenter gameEndPresenter;

        [Header("Gameplay")]
        [SerializeField] private CombatBootstrap combatBootstrap;
        [SerializeField] private MonsterLairBootstrap monsterLairBootstrap;
        [SerializeField] private HeroReviveService heroReviveService;
        [SerializeField] private global::Heroes.Game.Quests.QuestCompletionListener questCompletionListener;

        [SerializeField] private int startGold = 1000;

        [Header("GOAP")]
        [SerializeField] private GoapBuildingReferences goapBuildings;

        [Header("Bootstrap")]
        [SerializeField] private bool spawnCastleIfMissing = true;
        [SerializeField] private Vector3 castleSpawnPosition = Vector3.zero;

        [SerializeField] private BuildingDefinition Castle;

        protected override void Configure(IContainerBuilder builder)
        {
            GoapRuntimeConfig.Set(goapBuildings);
            global::Heroes.GOAP.Core.PlanningDebugSettings.Enabled = false;
            global::Heroes.GOAP.Core.PlanningDebugSettings.LogToFile = false;
            global::Heroes.GOAP.Core.PlanningDebugSettings.MaxPlansToLog = 0;
            global::Heroes.GOAP.Core.PlanningDebugSettings.FlushIntervalSeconds = 1f;
            global::Heroes.GOAP.Core.PlanningDebugSettings.MaxBufferedLines = 0;

            buildingCatalog.Initialize();
            if (itemCatalog != null)
            {
                itemCatalog.Initialize();
            }

            builder.RegisterInstance(buildingCatalog);
            builder.RegisterInstance(itemCatalog != null ? itemCatalog : ScriptableObject.CreateInstance<ItemCatalog>());
            var kingdomModel = new KingdomModel(startGold);
            builder.RegisterInstance(kingdomModel);

            builder.Register<KingdomService>(Lifetime.Singleton);
            builder.Register<BuildingPlacementSelectionService>(Lifetime.Singleton);
            builder.Register<BuildingPlacementService>(Lifetime.Singleton);
            builder.Register<HeroRosterService>(Lifetime.Singleton);
            builder.Register<HeroSpawnService>(Lifetime.Singleton);
            builder.Register<BuildingUpgradeService>(Lifetime.Singleton);
            builder.Register<ShopService>(Lifetime.Singleton);
            builder.Register<BuildingPopulationService>(Lifetime.Singleton);
            builder.Register<SelectionService>(Lifetime.Singleton);
            builder.Register<QuestService>(Lifetime.Singleton);

            builder.RegisterComponent(buildingIncomeService);

            builder.RegisterComponent(gameWorldStateManager);
            builder.RegisterComponent(placementController);
            builder.RegisterComponent(selectionController);
            builder.RegisterComponent(questController);
            builder.RegisterComponent(buildingPanelPresenter);
            builder.RegisterComponent(kingdomResourcesPresenter);
            builder.RegisterComponent(selectionPanelPresenter);
            builder.RegisterComponent(heroesPanelPresenter);
            builder.RegisterComponent(questPanelPresenter);
            builder.RegisterComponent(uiInputGatePresenter);
            builder.RegisterComponent(gameEndPresenter);
            builder.RegisterComponent(buildingCursor);

            builder.RegisterComponent(cheatsController);
            builder.RegisterComponent(combatBootstrap);
            builder.RegisterComponent(heroReviveService);
            builder.RegisterComponent(questCompletionListener);
            builder.RegisterComponent(monsterLairBootstrap);
        }

        private void Start()
        {
            
            _ = Container.Resolve<BuildingPopulationService>();

            buildingIncomeService.Initialize(Container.Resolve<KingdomService>());

            if (!spawnCastleIfMissing || buildingCatalog == null)
            {
                return;
            }

            foreach (var b in UnityEngine.Object.FindObjectsByType<BuildingFacade>(FindObjectsSortMode.None))
            {
                if (b != null && b.Definition != null && b.Definition.Id == Castle.Id)
                {
                    return;
                }
            }

            var def = buildingCatalog.GetById(Castle.Id);
            
            if (def == null || def.Prefab == null)
            {
                return;
            }

            var castle = UnityEngine.Object.Instantiate(def.Prefab, castleSpawnPosition, Quaternion.identity);
            var instanceId = System.Guid.NewGuid().ToString();
            
            castle.Initialize(def, instanceId);
            Registry<BuildingFacade>.TryAdd(castle);

            EventBus<BuildingPlacedEvent>.Invoke(new BuildingPlacedEvent
            {
                InstanceId = instanceId,
                DefinitionId = def.Id,
                Position = castleSpawnPosition
            });
        }
    }
}


