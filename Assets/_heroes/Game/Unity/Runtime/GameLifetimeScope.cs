using Heroes.Content.Buildings;
using Heroes.Game.Buildings;
using Heroes.Game.Core;
using Heroes.Presentation.UI.BuildingPanel;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Heroes.Game.Runtime
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private BuildingCatalog buildingCatalog;
        [SerializeField] private BuildingPlacementController placementController;
        [SerializeField] private BuildingPanelPresenter buildingPanelPresenter;
        [SerializeField] private int startGold = 1000;

        protected override void Configure(IContainerBuilder builder)
        {
            buildingCatalog.Initialize();

            builder.RegisterInstance(buildingCatalog);
            builder.RegisterInstance(new KingdomModel(startGold));

            builder.Register<BuildingPlacementSelectionService>(Lifetime.Singleton);
            builder.Register<BuildingPlacementService>(Lifetime.Singleton);

            builder.RegisterComponent(placementController);
            builder.RegisterComponent(buildingPanelPresenter);
        }
    }
}
