using System;
using System.Collections.Generic;
using Heroes.Goap.Runtime.Values;
using UnityEngine;

namespace Heroes.Goap.Runtime.Core
{
    [CreateAssetMenu(menuName = "Heroes/GOAP/Archetype", fileName = "GoapArchetype")]
    public class GoapArchetypeAsset : ScriptableObject
    {
        public GoapGraphAsset Graph;
        public GoapArchetypeAsset Parent;
    }
}
