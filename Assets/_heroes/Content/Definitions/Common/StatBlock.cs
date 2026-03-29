using System;
using Heroes.Content.Abstractions;
using UnityEngine;

namespace Heroes.Content.Definitions.Common
{
    [Serializable]
    public struct StatBlock : IStatBlock
    {
        [SerializeField] private float strength;
        [SerializeField] private float agility;
        [SerializeField] private float intelligence;
        [SerializeField] private float endurance;
        [SerializeField] private float luck;
        [SerializeField] private float wisdom;

        public float Strength => strength;
        public float Agility => agility;
        public float Intelligence => intelligence;
        public float Endurance => endurance;
        public float Luck => luck;
        public float Wisdom => wisdom;
    }
}
