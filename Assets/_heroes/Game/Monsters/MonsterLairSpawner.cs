using System.Collections.Generic;
using Heroes.Content.Monsters;
using Heroes.Game.Buildings;
using UnityEngine;
using UnityEngine.AI;

namespace Heroes.Game.Monsters
{
    public sealed class MonsterLairSpawner : MonoBehaviour
    {
        [System.Serializable]
        public struct WaveMilestone
        {
            public float Minutes;
            public int Count;
        }

        [Header("Data")]
        [SerializeField] private MonsterLairDefinition lair;

        [Header("Monster")]
        [SerializeField] private Transform spawnPoint;

        [Header("Waves")]
        

        private readonly List<MonsterFacade> _alive = new();
        private float _startAt;
        private float _nextWaveAt;
        private BuildingFacade _building;

        private void Awake()
        {
            _building = GetComponent<BuildingFacade>();
        }

        private void Start()
        {
            _startAt = Time.unscaledTime;
            _nextWaveAt = Time.unscaledTime + 1f;
        }

        public void Configure(MonsterLairDefinition lairDefinition, Transform spawnPointOverride)
        {
            lair = lairDefinition;
            if (spawnPointOverride != null)
            {
                spawnPoint = spawnPointOverride;
            }

            _startAt = Time.unscaledTime;
            _nextWaveAt = Time.unscaledTime + 1f;
        }

        private void Update()
        {
            if (_building != null && !_building.IsAlive)
            {
                return;
            }

            var def = lair != null ? lair.Monster : null;
            if (def == null || def.Prefab == null)
            {
                return;
            }

            if (lair == null)
            {
                return;
            }

            var interval = lair.SpawnIntervalSeconds;
            var max = lair.MaxAlive;
            var raidTh = lair.RaidThreshold;
            var ms = lair.Milestones;

            PruneDead();

            var raid = _alive.Count > raidTh;
            if (raid)
            {
                for (var i = 0; i < _alive.Count; i++)
                {
                    var m = _alive[i];
                    if (m != null)
                    {
                        m.SetRaidMode(true);
                    }
                }
            }

            if (Time.unscaledTime < _nextWaveAt)
            {
                return;
            }

            _nextWaveAt = Time.unscaledTime + Mathf.Max(1f, interval);
            SpawnWave(def, max, ms, raid);
        }

        private void SpawnWave(MonsterDefinition def, int max, WaveMilestone[] ms, bool raid)
        {
            if (_alive.Count >= max)
            {
                return;
            }

            var count = GetCurrentWaveCount(ms);
            if (count <= 0)
            {
                return;
            }

            var available = Mathf.Max(0, max - _alive.Count);
            count = Mathf.Min(count, available);

            var pos = spawnPoint != null ? spawnPoint.position : transform.position;
            for (var i = 0; i < count; i++)
            {
                var p = pos + new Vector3(Random.Range(-1.5f, 1.5f), 0f, Random.Range(-1.5f, 1.5f));
                if (NavMesh.SamplePosition(p, out var hit, 6f, NavMesh.AllAreas))
                {
                    p = hit.position;
                }

                var inst = Instantiate(def.Prefab, p, Quaternion.identity);
                inst.Initialize(def, pos);
                inst.SetRaidMode(raid);
                _alive.Add(inst);
            }
        }

        private int GetCurrentWaveCount(WaveMilestone[] ms)
        {
            var minutes = (Time.unscaledTime - _startAt) / 60f;
            var best = 1;
            if (ms != null)
            {
                for (var i = 0; i < ms.Length; i++)
                {
                    if (minutes >= ms[i].Minutes)
                    {
                        best = Mathf.Max(1, ms[i].Count);
                    }
                }
            }
            return best;
        }

        private void PruneDead()
        {
            for (var i = _alive.Count - 1; i >= 0; i--)
            {
                var m = _alive[i];
                if (m == null || !m.IsAlive)
                {
                    _alive.RemoveAt(i);
                }
            }
        }
    }
}
