using Heroes.Content.Buildings;
using Heroes.Game.Buildings;
using Heroes.Game.Core;
using Heroes.Game.AI;
using Heroes.Game.Heroes;
using Heroes.Presentation.UI.BuildingPanel;
using Heroes.Presentation.UI.ResourcesPanel;
using Heroes.Presentation.UI.SelectionPanel;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Heroes.Game.Runtime
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private BuildingCatalog buildingCatalog;
        [SerializeField] private BuildingPlacementController placementController;
        [SerializeField] private SelectionController selectionController;
        [SerializeField] private BuildingCursor buildingCursor;
        [SerializeField] private GameWorldStateManager gameWorldStateManager;
        [SerializeField] private BuildingPanelPresenter buildingPanelPresenter;
        [SerializeField] private KingdomResourcesPresenter kingdomResourcesPresenter;
        [SerializeField] private SelectionPanelPresenter selectionPanelPresenter;

        [SerializeField] private int startGold = 1000;

        protected override void Configure(IContainerBuilder builder)
        {
            buildingCatalog.Initialize();

            builder.RegisterInstance(buildingCatalog);
            builder.RegisterInstance(new KingdomModel(startGold));

            builder.Register<KingdomService>(Lifetime.Singleton);
            builder.Register<BuildingPlacementSelectionService>(Lifetime.Singleton);
            builder.Register<BuildingPlacementService>(Lifetime.Singleton);
            builder.Register<HeroRosterService>(Lifetime.Singleton);
            builder.Register<HeroSpawnService>(Lifetime.Singleton);
            builder.Register<BuildingUpgradeService>(Lifetime.Singleton);
            builder.Register<SelectionService>(Lifetime.Singleton);

            builder.RegisterComponent(gameWorldStateManager);
            builder.RegisterComponent(placementController);
            builder.RegisterComponent(selectionController);
            builder.RegisterComponent(buildingPanelPresenter);
            builder.RegisterComponent(kingdomResourcesPresenter);
            builder.RegisterComponent(selectionPanelPresenter);
            builder.RegisterComponent(buildingCursor);
        }
    }
}
