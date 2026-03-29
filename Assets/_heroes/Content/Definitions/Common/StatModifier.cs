using System;
using Heroes.Content.Abstractions;
using UnityEngine;

namespace Heroes.Content.Definitions.Common
{
    [Serializable]
    public struct StatModifier : IStatModifier
    {
        [SerializeField] private StatType stat;
        [SerializeField] private float value;
        [SerializeField] private float durationSeconds;

        public StatType Stat => stat;
        public float Value => value;
        public float DurationSeconds => durationSeconds;
    }
}
