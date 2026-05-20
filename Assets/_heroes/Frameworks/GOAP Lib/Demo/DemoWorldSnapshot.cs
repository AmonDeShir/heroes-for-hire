using System.Collections.Generic;
using System.Collections.ObjectModel;
using Heroes.GOAP.Core;
using UnityEngine;

namespace GOAP.Demo
{
    public readonly struct DemoWorldSnapshot : IReadOnlyWorldSnapshot
    {
        public int Version { get; }
        public bool IsValid { get; }
        public ReadOnlyDictionary<string, Vector2> Locations { get; }
        
        public DemoWorldSnapshot(int version, bool isValid, Dictionary<string, Vector2> locations)
        {
            Version = version;
            IsValid = isValid;
            Locations = new ReadOnlyDictionary<string, Vector2>(locations);
        }
    }
}


