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
                var entity = GetEntity(TransformUsageFlags.None);
                
                LoadToBuffer(entity, authoring.buildingRoot, e => new BuildingPhaseElement { value = e });
                LoadToBuffer(entity, authoring.destructionRoot, e => new DestroyPhaseElement { value = e });
                
                AddComponent(entity, new BuildingVisuals
                {
                    Visual = GetEntity(authoring.complete, TransformUsageFlags.Dynamic)
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