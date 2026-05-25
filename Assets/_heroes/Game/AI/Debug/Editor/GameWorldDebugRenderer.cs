using System;
using System.Linq;
using Heroes.GOAP.Editor;
using UnityEngine.UIElements;

namespace Heroes.Game.AI.Debug
{
    public sealed class GameWorldDebugRenderer : IGoapWorldDebugRenderer
    {
        public bool CanRender(object snapshot) => snapshot is GameWorldSnapshot;

        public void Render(object snapshot, VisualElement root)
        {
            root.Clear();

            if (snapshot is not GameWorldSnapshot world)
            {
                root.Add(new Label("No game world snapshot."));
                return;
            }

            if (world.Locations == null || world.Locations.Values == null || world.Locations.Values.Count == 0)
            {
                root.Add(new Label("Locations: 0"));
                return;
            }

            root.Add(new Label($"Locations: {world.Locations.Values.Count}"));

            foreach (var kvp in world.Locations.Values.OrderBy(k => k.Key, StringComparer.Ordinal))
            {
                var defId = kvp.Key;
                var count = kvp.Value != null ? kvp.Value.Length : 0;
                
                root.Add(new Label($"{FormatDefinitionId(defId, kvp.Value)}: {count}"));

                if (kvp.Value == null)
                {
                    continue;
                }
                
                var shown = 0;
                
                for (var i = 0; i < kvp.Value.Length && shown < 6; i++)
                {
                    var loc = kvp.Value[i];
                    root.Add(new Label($"  {loc.ID} @ {loc.Position}"));
                    shown++;
                }

                if (count > shown)
                {
                    root.Add(new Label($"  ... +{count - shown} more"));
                }
            }
        }

        private static string FormatDefinitionId(string defId, Location[] locations)
        {
            if (string.IsNullOrWhiteSpace(defId))
            {
                return "<empty>";
            }

            
            var refs = GoapRuntimeConfig.Buildings;
            if (refs != null)
            {
                if (refs.Guild != null && refs.Guild.Id == defId) return "Guild";
                if (refs.Market != null && refs.Market.Id == defId) return "Market";
                if (refs.Blacksmith != null && refs.Blacksmith.Id == defId) return "Blacksmith";
                if (refs.Farm != null && refs.Farm.Id == defId) return "Farm";
                if (refs.Castle != null && refs.Castle.Id == defId) return "Castle";
                if (refs.House != null && refs.House.Id == defId) return "House";
                if (refs.Tower != null && refs.Tower.Id == defId) return "Tower";
                if (refs.Alchemist != null && refs.Alchemist.Id == defId) return "Alchemist";
                if (refs.Chapel != null && refs.Chapel.Id == defId) return "Chapel";
            }

            return defId;
        }
    }
}


