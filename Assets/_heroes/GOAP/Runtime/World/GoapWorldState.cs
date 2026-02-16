using System.Collections.Generic;
using Heroes.Goap.Runtime.Values;
using UnityEngine;

namespace Heroes.Goap.Runtime.World
{
    public class GoapWorldState
    {
        readonly Dictionary<string, GoapValue> m_Values = new Dictionary<string, GoapValue>();
        static readonly Dictionary<LocationSO, List<GoapLocationMarker>> s_LocationMarkers = new Dictionary<LocationSO, List<GoapLocationMarker>>();

        public IReadOnlyDictionary<string, GoapValue> Values => m_Values;

        public void Set(string variableName, GoapValue value)
        {
            if (string.IsNullOrWhiteSpace(variableName))
                return;

            m_Values[variableName] = value;
        }

        public bool TryGet(string variableName, out GoapValue value)
        {
            return m_Values.TryGetValue(variableName, out value);
        }

        public static void RegisterLocation(GoapLocationMarker marker)
        {
            if (marker == null || marker.LocationType == null)
                return;

            if (!s_LocationMarkers.TryGetValue(marker.LocationType, out var list))
            {
                list = new List<GoapLocationMarker>();
                s_LocationMarkers[marker.LocationType] = list;
            }

            if (!list.Contains(marker))
                list.Add(marker);
        }

        public static void UnregisterLocation(GoapLocationMarker marker)
        {
            if (marker == null || marker.LocationType == null)
                return;

            if (!s_LocationMarkers.TryGetValue(marker.LocationType, out var list))
                return;

            list.Remove(marker);
            if (list.Count == 0)
                s_LocationMarkers.Remove(marker.LocationType);
        }

        public static bool TryGetClosestLocation(LocationSO locationType, Vector3 from, out GoapLocationMarker marker)
        {
            marker = null;
            if (locationType == null)
                return false;

            if (!s_LocationMarkers.TryGetValue(locationType, out var list) || list.Count == 0)
                return false;

            float bestDistance = float.MaxValue;
            for (int i = 0; i < list.Count; i++)
            {
                var candidate = list[i];
                if (candidate == null)
                    continue;

                var distance = (candidate.transform.position - from).sqrMagnitude;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    marker = candidate;
                }
            }

            return marker != null;
        }
    }
}
