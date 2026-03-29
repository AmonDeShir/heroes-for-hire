using System;
using UnityEngine;

namespace Heroes.Content.Abstractions
{
    public enum TargetType
    {
        Self,
        Ally,
        Enemy,
        Area
    }
    
    public interface IStatusSkillData : ISkillData
    {
        int Duration { get; }
        float Range { get; }
        IEntityStats Stats { get; }
        GameObject EffectPrefab { get; }
        TargetType TargetType { get; }
    }
}
