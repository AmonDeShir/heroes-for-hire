using Heroes.Content.Buildings;
using Heroes.Content.Heroes;
using Heroes.Game.Buildings;
using Heroes.Game.Core;
using Heroes.Game.AI;
using Heroes.Game.Heroes;
using Heroes.Game.Core.Events;
using Heroes.Presentation.UI.BuildingPanel;
using Heroes.Presentation.UI.ResourcesPanel;
using Heroes.Presentation.UI.SelectionPanel;
using EventBus;
using Registry;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Heroes.Game.Runtime
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private BuildingCatalog buildingCatalog;
        [SerializeField] private ItemCatalog itemCatalog;
        [SerializeField] private BuildingPlacementController placementController;
        [SerializeField] private SelectionController selectionController;
        [SerializeField] private BuildingCursor buildingCursor;
        [SerializeField] private GameWorldStateManager gameWorldStateManager;
        [SerializeField] private BuildingPanelPresenter buildingPanelPresenter;
        [SerializeField] private KingdomResourcesPresenter kingdomResourcesPresenter;
        [SerializeField] private SelectionPanelPresenter selectionPanelPresenter;

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
            global::Heroes.GOAP.Core.PlanningDebugSettings.Enabled = goapBuildings != null && goapBuildings.EnableGoapDebugLogs;
            global::Heroes.GOAP.Core.PlanningDebugSettings.LogToFile = true;
            global::Heroes.GOAP.Core.PlanningDebugSettings.MaxPlansToLog = 10;
            global::Heroes.GOAP.Core.PlanningDebugSettings.FlushIntervalSeconds = 0.5f;
            global::Heroes.GOAP.Core.PlanningDebugSettings.MaxBufferedLines = 2000;

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

            
            
            builder.RegisterComponent(new GameObject("BuildingIncomeService").AddComponent<BuildingIncomeService>());

            

            builder.RegisterComponent(gameWorldStateManager);
            builder.RegisterComponent(placementController);
            builder.RegisterComponent(selectionController);
            builder.RegisterComponent(buildingPanelPresenter);
            builder.RegisterComponent(kingdomResourcesPresenter);
            builder.RegisterComponent(selectionPanelPresenter);
            builder.RegisterComponent(buildingCursor);
        }

        private void Start()
        {
            
            _ = Container.Resolve<BuildingPopulationService>();

            var income = FindFirstObjectByType<BuildingIncomeService>();
            if (income != null)
            {
                income.Initialize(Container.Resolve<KingdomService>());
            }

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


