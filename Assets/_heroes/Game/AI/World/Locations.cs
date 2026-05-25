using System;
using System.Collections.Generic;
using UnityEngine;

namespace Heroes.Game.AI
{
    public struct Location
    {
        public string ID;
        public Vector2 Position;
        public string DefinitionId;
        public float Radius;
    }

    public class Locations
    {
        public readonly Dictionary<string, Location[]> Values = new();

        public Locations Clone()
        {
            var clone = new Locations();

            foreach (var pair in Values)
            {
                var copy = new Location[pair.Value.Length];
                Array.Copy(pair.Value, copy, pair.Value.Length);
                clone.Values[pair.Key] = copy;
            }

            return clone;
        }

        public bool TryGetClosest(string definitionId, Vector2 from, out Vector2 position)
        {
            return TryGetClosest(definitionId, from, _ => true, out position);
        }

        public bool TryGetClosest(string definitionId, Vector2 from, Func<Location, bool> predicate, out Vector2 position)
        {
            position = default;

            if (!Values.TryGetValue(definitionId, out var locations))
            {
                return false;
            }

            var closestDistance = float.MaxValue;
            var found = false;

            foreach (var location in locations)
            {
                if (!predicate(location))
                {
                    continue;
                }

                var distance = (location.Position - from).sqrMagnitude;

                if (distance >= closestDistance)
                {
                    continue;
                }

                closestDistance = distance;
                position = location.Position;
                found = true;
            }

            return found;
        }

        public bool TryGetClosestLocation(string definitionId, Vector2 from, out Location location)
        {
            location = default;

            if (!Values.TryGetValue(definitionId, out var locations))
            {
                return false;
            }

            var closestDistance = float.MaxValue;
            var found = false;

            foreach (var loc in locations)
            {
                var distance = (loc.Position - from).sqrMagnitude;
                if (distance >= closestDistance)
                {
                    continue;
                }

                closestDistance = distance;
                location = loc;
                found = true;
            }

            return found;
        }

        public bool TryGetPositionByInstanceId(string id, out Vector2 position)
        {
            position = default;

            foreach (var locations in Values.Values)
            {
                foreach (var location in locations)
                {
                    if (location.ID != id)
                    {
                        continue;
                    }

                    position = location.Position;
                    return true;
                }
            }

            return false;
        }

        public bool HasAny(string definitionId)
        {
            return Values.TryGetValue(definitionId, out var locations) && locations.Length > 0;
        }

        public bool HasAny(string definitionId, Func<Location, bool> predicate)
        {
            if (!Values.TryGetValue(definitionId, out var locations))
            {
                return false;
            }

            foreach (var location in locations)
            {
                if (predicate(location))
                {
                    return true;
                }
            }

            return false;
        }

        public void RegisterLocation(Location location)
        {
            var definitionId = location.DefinitionId;

            if (!Values.TryGetValue(definitionId, out var locations))
            {
                Values[definitionId] = new[] { location };
                return;
            }

            var newLocations = new Location[locations.Length + 1];
            Array.Copy(locations, newLocations, locations.Length);
            newLocations[^1] = location;

            Values[definitionId] = newLocations;
        }

        public bool RemoveLocation(string definitionId, string id)
        {
            if (!Values.TryGetValue(definitionId, out var locations))
            {
                return false;
            }

            var index = Array.FindIndex(locations, x => x.ID == id);

            if (index < 0)
            {
                return false;
            }

            if (locations.Length == 1)
            {
                Values.Remove(definitionId);
                return true;
            }

            var newLocations = new Location[locations.Length - 1];

            if (index > 0)
            {
                Array.Copy(locations, 0, newLocations, 0, index);
            }

            if (index < locations.Length - 1)
            {
                Array.Copy(locations, index + 1, newLocations, index, locations.Length - index - 1);
            }

            Values[definitionId] = newLocations;
            return true;
        }
    }
}


