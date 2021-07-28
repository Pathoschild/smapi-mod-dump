/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bouhm/stardew-valley-mods
**
*************************************************/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using NPCMapLocations.Framework.Models;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;

namespace NPCMapLocations.Framework
{
    // Handles custom maps (recolors of the mod map), custom NPCs, custom sprites, custom names, etc.
    public class ModCustomizations
    {
        public Dictionary<string, MapVector[]> MapVectors { get; set; }
        public Dictionary<string, string> Names { get; set; }
        public HashSet<string> LocationExclusions { get; set; }
        public Dictionary<string, MapTooltip> Tooltips { get; set; }

        public readonly string MapsRootPath = "maps";
        public string MapsPath { get; set; }

        public ModCustomizations()
        {
            this.MapsPath = Path.Combine(this.MapsRootPath, "_default");

            this.Names = new Dictionary<string, string>();
            this.MapVectors = new Dictionary<string, MapVector[]>();
            this.Tooltips = new Dictionary<string, MapTooltip>();
            this.LocationExclusions = new HashSet<string>();
        }

        public void LoadCustomData(Dictionary<string, JObject> customNpcJson, Dictionary<string, JObject> customLocationJson)
        {
            this.LoadCustomLocations(customLocationJson);
            this.LoadCustomNpcs(customNpcJson);
        }

        private void LoadCustomLocations(Dictionary<string, JObject> customLocationJson)
        {
            foreach (var locationData in customLocationJson)
            {
                var location = locationData.Value;
                if (location.ContainsKey("MapVectors"))
                {
                    this.AddCustomMapLocation(locationData.Key, (JArray)location.GetValue("MapVectors"));
                }
                if (location.ContainsKey("MapTooltip"))
                {
                    this.AddTooltip(locationData.Key, (JObject)location.GetValue("MapTooltip"));
                }
                if (location.ContainsKey("Exclude"))
                {
                    if ((bool)location.GetValue("Exclude"))
                    {
                        this.LocationExclusions.Add(locationData.Key);
                    }
                }
            }
        }

        // Handles customizations for NPCs
        // Custom NPCs and custom names or sprites for existing NPCs
        private void LoadCustomNpcs(Dictionary<string, JObject> customNpcJson)
        {
            var npcMarkerOffsets = ModConstants.NpcMarkerOffsets;
            var npcExclusions = ModEntry.Globals.NpcExclusions;

            foreach (var npcData in customNpcJson)
            {
                var npc = npcData.Value;

                if (npc.ContainsKey("Exclude"))
                {
                    if ((bool)npc.GetValue("Exclude"))
                    {
                        npcExclusions.Add(npcData.Key);
                        continue;
                    }
                }

                if (npc.ContainsKey("MarkerCropOffset"))
                {
                    npcMarkerOffsets[npcData.Key] = (int)npc.GetValue("MarkerCropOffset");
                }
                else
                {
                    var gameNpc = Game1.getCharacterFromName(npcData.Key);
                    if (gameNpc != null)
                    {
                        // If custom crop offset is not specified, default to 0
                        if (!npcMarkerOffsets.ContainsKey(gameNpc.Name))
                            npcMarkerOffsets[gameNpc.Name] = 0;

                        // Children sprites are short so give them a booster seat
                        if (gameNpc is Child)
                        {
                            npcMarkerOffsets[gameNpc.Name] += 7;
                        }
                    }
                }
            }

            // Merge customizations into globals config
            ModEntry.Globals.NpcMarkerOffsets = this.MergeDictionaries(npcMarkerOffsets, ModEntry.Globals.NpcMarkerOffsets);
            ModEntry.Globals.NpcExclusions = npcExclusions;

            foreach (var character in Utility.getAllCharacters())
            {
                // Handle any modified NPC names 
                // Specifically mods that change names in dialogue files (displayName)
                if (!this.Names.ContainsKey(character.Name))
                    this.Names.Add(character.Name, character.displayName ?? character.Name);
            }

            // Handle duplicate displayName -- custom NPCs that replaces villagers
            Dictionary<string, string> dupes = this.Names
              .Where(n1 => this.Names.Any(n2 => n2.Key != n1.Key && n2.Value == n1.Value))
              .ToDictionary(n => n.Key, n => n.Value);

            // Properly replace the villager with custom NPC
            foreach (var dupe in dupes)
            {
                if (dupe.Key != dupe.Value)
                {
                    this.Names[dupe.Key] = dupe.Value;
                }
                else
                {
                    this.Names.Remove(dupe.Key);
                }
            }

            ModEntry.StaticHelper.Data.WriteJsonFile($"config/{Constants.SaveFolderName}.json", ModEntry.Config);
            ModEntry.StaticHelper.Data.WriteJsonFile("config/globals.json", ModEntry.Globals);
        }

        private void AddTooltip(string locationName, JObject tooltip)
        {
            this.Tooltips[locationName] = new MapTooltip(
              (int)tooltip.GetValue("X"),
              (int)tooltip.GetValue("Y"),
              (int)tooltip.GetValue("Width"),
              (int)tooltip.GetValue("Height"),
              (string)tooltip.GetValue("PrimaryText"),
              (string)tooltip.GetValue("SecondaryText")
            );

            if (tooltip.ContainsKey("SecondaryText"))
            {
                this.Tooltips[locationName].SecondaryText = (string)tooltip.GetValue("SecondaryText");
            }
        }

        // Any custom locations with given location on the map
        private void AddCustomMapLocation(string locationName, JArray mapLocations)
        {
            var mapVectors = mapLocations.ToObject<JObject[]>();
            var mapVectorArr = new MapVector[mapVectors.Length];
            for (int i = 0; i < mapVectors.Length; i++)
            {
                var mapVector = mapVectors[i];

                // Marker doesn't need to specify corresponding Tile position
                if (mapVector.GetValue("TileX") == null || mapVector.GetValue("TileY") == null)
                    mapVectorArr[i] = new MapVector(
                      (int)mapVector.GetValue("MapX"),
                      (int)mapVector.GetValue("MapY")
                    );
                // Region must specify corresponding Tile positions for
                // Calculations on movement within location
                else
                    mapVectorArr[i] = new MapVector(
                      (int)mapVector.GetValue("MapX"),
                      (int)mapVector.GetValue("MapY"),
                      (int)mapVector.GetValue("TileX"),
                      (int)mapVector.GetValue("TileY")
                    );
            }

            this.MapVectors.Add(locationName, mapVectorArr);
        }

        // Merge dictionaries, in case of key conflict d1 takes precedence
        private Dictionary<string, T> MergeDictionaries<T>(Dictionary<string, T> d1, Dictionary<string, T> d2)
        {
            var dictionaries = new[] { d1, d2 };
            return dictionaries.SelectMany(dict => dict)
                    .ToLookup(pair => pair.Key, pair => pair.Value)
                    .ToDictionary(group => group.Key, group => group.First());
        }
    }
}
