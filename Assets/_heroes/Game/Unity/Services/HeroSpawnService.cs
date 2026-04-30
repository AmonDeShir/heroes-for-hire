using System;
using Heroes.Game.Buildings;
using Heroes.Game.AI;
using Heroes.Content.Heroes;
using Registry;
using UnityEngine;
using UnityEngine.AI;

namespace Heroes.Game.Heroes
{
    public sealed class HeroSpawnService
    {
        private readonly GameWorldStateManager _worldStateManager;

        public HeroSpawnService(GameWorldStateManager worldStateManager)
        {
            _worldStateManager = worldStateManager;
        }

        public HeroFacade Spawn(HeroDefinition definition, BuildingFacade home)
        {
            if (definition == null || definition.Prefab == null || home == null)
            {
                return null;
            }

            var spawnPosition = ResolveSpawnPosition(home.transform.position);
            var hero = UnityEngine.Object.Instantiate(definition.Prefab, spawnPosition, Quaternion.identity);
            hero.Initialize(definition, Guid.NewGuid().ToString(), home.Id, _worldStateManager);
            Registry<HeroFacade>.TryAdd(hero);
            return hero;
        }

        private static Vector3 ResolveSpawnPosition(Vector3 origin)
        {
            var candidate = origin + new Vector3(2f, 0f, 2f);
            return NavMesh.SamplePosition(candidate, out var hit, 4f, NavMesh.AllAreas) ? hit.position : origin;
        }
    }
}
