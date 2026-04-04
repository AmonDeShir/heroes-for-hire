using System;
using Heroes.Game.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Heroes.Game.Authoring
{
    public class BuildingDataAuthoring : MonoBehaviour
    {
        public GameObject complete;
        public Transform buildingRoot;
        public Transform destructionRoot;

        public class Baker : Baker<BuildingDataAuthoring>
        {
            public override void Bake(BuildingDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                LoadToBuffer(entity, authoring.buildingRoot, e => new BuildingPhaseElement { Value = e });
                LoadToBuffer(entity, authoring.destructionRoot, e => new DestroyPhaseElement { Value = e });
                
                AddComponent(entity, new BuildingVisuals
                {
                    Value = GetEntity(authoring.complete, TransformUsageFlags.Dynamic)
                });
            }

            private void LoadToBuffer<T>(Entity entity, Transform parent, Func<Entity, T> create) where T : unmanaged, IBufferElementData
            {
                var buffer = AddBuffer<T>(entity);
                
                for (var i = 0; i < parent.childCount; i++)
                {
                    var child = parent.GetChild(i);

                    buffer.Add(create.Invoke(GetEntity(child, TransformUsageFlags.Dynamic)));
                }
            }
        }
    }
}