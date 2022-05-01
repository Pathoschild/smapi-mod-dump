/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StardewValley;

namespace PersonalEffects
{
    public class ConfigNPC
    {
        [JsonIgnore]
        internal string InternalName;
        public string DisplayName;
        public bool Enabled;
        public bool IsFemale;
        public bool CrossDress;
        public bool HomeSpots;
        public bool BathSpots;
        public bool OtherSpots;
        
        public bool HasMaleItems()
        {
            return IsFemale == CrossDress;
        }
    }

    internal static class NPCConfig
    {
        public static Dictionary<string, ConfigNPC> Data;
        private static ConfigNPC NoData;
        public static bool Ready;
        public static void Load(string directory)
        {
            NoData = new ConfigNPC
            {
                InternalName = "{Unknown NPC}",
                DisplayName = "{Unknown NPC}",
                IsFemale = false,
                Enabled = false,
                HomeSpots = false,
                BathSpots = false,
                OtherSpots = false
            };

            var filepath = Path.Combine(directory, "assets", "npcs.json");
            if (!File.Exists(filepath))
            {
                Modworks.Log.Error($"NPCConfig file not found at {filepath}");
                return;
            }
            try
            {
                var fileContents = File.ReadAllText(filepath);
                Data = JsonConvert.DeserializeObject<Dictionary<string, ConfigNPC>>(fileContents);
                Ready = true;
                Modworks.Log.Debug($"Loaded config from {filepath}, data has {Data.Count} NPCs");
            }
            catch (Exception e)
            {
                Modworks.Log.Error("Failed to read config file: " + e.Message);
            }
            //set internal names
            foreach (var (key, configNpc) in Data)
            {
                configNpc.InternalName = key;

                //child safety - if any configured NPCs are children, we'll disable them here
                var n = Game1.getCharacterFromName(configNpc.InternalName);
                if (n == null) continue; //if we can't read it, we'll let it pass.
                if (IsChild(n)) configNpc.Enabled = false;
            }

            foreach (var cnpc in Data.Values)
            {
                if (cnpc.Enabled)
                {
                    Modworks.Log.Trace("Personal Effects enabled for NPC " + cnpc.DisplayName);
                }
            }
        }

        private static bool IsChild(NPC npc)
        {

            if (npc is StardewValley.Characters.Child) return true; //should get vanilla player-children
            var dispositions = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
            if (dispositions.ContainsKey(npc.Name))
            {
                return dispositions[npc.Name].Split('/')[0] == "child";
            }
            //this npc doesn't exist in dispositions? perhaps a child, or other mod-added NPC (e.g. a Moongate)
            return npc.Age == 2; //should get any remaining NPC children
        }
        
        public static ConfigNPC GetNPC(string npc)
        {
            return Data != null && Data.ContainsKey(npc) ? Data[npc] : NoData;
        }

        public static string LookupNPC(string displayName)
        {
            foreach (var npc in Data)
            {
                if (npc.Value.DisplayName.Equals(displayName))
                {
                    return npc.Key;
                }
            }
            return null;
        }
    }

    public class ConfigLocation
    {
        public string NPC;
        public string Location;
        public string LocationType;
        public string LocationGender;
        public string Description;
        public int X;
        public int Y;
        public string Rarity;
        public string ForSVE;

        public int PercentChance()
        {
            return Rarity switch
            {
                "very_rare" => 1,
                "rare" => 2,
                "normal" => 4,
                "always" => 100,
                "never" => -1,
                _ => 4
            };
        }
    }

    class ConfigLocations
    {
        public static List<ConfigLocation> Data;
        public static bool Ready;
        public static void Load(string directory)
        {
            var filepath = Path.Combine(directory, "assets", "locations.json");
            if (! File.Exists(filepath))
            {
                Modworks.Log.Error($"Location file does not exist at {filepath}");

            }

            if (!File.Exists(filepath)) return;
            try
            {
                var fileContents = File.ReadAllText(filepath);
                Data = JsonConvert.DeserializeObject<List<ConfigLocation>>(fileContents);
                Ready = true;
            }
            catch (Exception e)
            {
                Modworks.Log.Error("Failed to read location file: " + e.Message);
            }
        }
    }
}