using Heroes.Content.Definitions.Buildings;
using Heroes.Content.Abstractions;
using Heroes.Game.Abstractions;
using Heroes.Game.Core.Events;
using Heroes.Game.Core.Events.Bus;
using Heroes.Game.Domain.Resources;
using Heroes.Game.Systems.Buildings;
using Heroes.Presentation.UI.BuildingPanel;
using Heroes.Presentation.World;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Heroes.Game.Bootstrap
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private BuildingCatalogAsset buildingCatalog;
        [SerializeField] private int startingGold = 500;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IGameEventBus, GameEventBus>(Lifetime.Singleton);

            builder.RegisterInstance(new KingdomResources(startingGold))
                .As<IKingdomResources>()
                .AsSelf();

            if (buildingCatalog != null)
            {
                builder.RegisterInstance(buildingCatalog)
                    .As<IBuildingCatalog>();
            }

            builder.Register<IBuildingPlacementSelectionService, BuildingPlacementSelectionService>(Lifetime.Singleton);

            builder.Register<BuildingSystem>(Lifetime.Singleton)
                .As<IBuildingSystem>()
                .As<IBuildingPlacementService>();
            
            RegisterIfPresent(builder, Object.FindObjectOfType<BuildingPanelViewModel>(true));
            RegisterIfPresent(builder, Object.FindObjectOfType<BuildingPlacementInput>(true));
            RegisterIfPresent(builder, Object.FindObjectOfType<BuildingWorldPresenter>(true));
        }

        private static void RegisterIfPresent<T>(IContainerBuilder builder, T component)
            where T : Component
        {
            if (component != null)
            {
                builder.RegisterComponent(component);
            }
        }
    }
}
