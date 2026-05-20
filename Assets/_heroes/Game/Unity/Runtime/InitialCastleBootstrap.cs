using System;
using EventBus;
using Heroes.Content.Buildings;
using Heroes.Game.Buildings;
using Heroes.Game.Core.Events;
using Registry;
using UnityEngine;
using VContainer;

namespace Heroes.Game.Runtime
{
    
    public sealed class InitialCastleBootstrap : MonoBehaviour
    {
        
        private const string CastleDefinitionId = "1c5b4b30-9e9d-4d9f-9e89-9c2a8fbf7a4c";

        [SerializeField] private Vector3 position = Vector3.zero;

        private BuildingCatalog _buildingCatalog;

        [Inject]
        public void Construct(BuildingCatalog buildingCatalog)
        {
            _buildingCatalog = buildingCatalog;
        }

        private void Start()
        {
            if (_buildingCatalog == null)
            {
                return;
            }

            
            foreach (var b in UnityEngine.Object.FindObjectsByType<BuildingFacade>(FindObjectsSortMode.None))
            {
                if (b != null && b.Definition != null && b.Definition.Id == CastleDefinitionId)
                {
                    return;
                }
            }

            var def = _buildingCatalog.GetById(CastleDefinitionId);
            if (def == null || def.Prefab == null)
            {
                return;
            }

            var castle = UnityEngine.Object.Instantiate(def.Prefab, position, Quaternion.identity);
            var instanceId = Guid.NewGuid().ToString();
            castle.Initialize(def, instanceId);
            Registry<BuildingFacade>.TryAdd(castle);

            EventBus<BuildingPlacedEvent>.Invoke(new BuildingPlacedEvent
            {
                InstanceId = instanceId,
                DefinitionId = def.Id,
                Position = position
            });
        }
    }
}


