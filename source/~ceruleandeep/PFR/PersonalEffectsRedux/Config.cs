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

    class Config
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


            string filepath = directory + "config.json";
            if (File.Exists(filepath))
            {
                try
                {
                    string filecontents = File.ReadAllText(filepath);
                    Data = JsonConvert.DeserializeObject<Dictionary<string, ConfigNPC>>(filecontents);
                    Ready = true;
                }
                catch (Exception e)
                {
                    Modworks.Log.Error("Failed to read config file: " + e.Message);
                }
                //set internal names
                foreach (var kvp in Data)
                {
                    kvp.Value.InternalName = kvp.Key;

                    //child safety - if any configured NPCs are children, we'll disable them here
                    var n = StardewValley.Game1.getCharacterFromName(kvp.Value.InternalName);
                    if (n == null) continue; //if we can't read it, we'll let it pass.
                    if (Modworks.NPCs.IsChild(n)) kvp.Value.Enabled = false;
                }

                foreach (ConfigNPC cnpc in Data.Values)
                {
                    if (cnpc.Enabled)
                    {
                        Modworks.Log.Trace("Personal Effects enabled for NPC " + cnpc.DisplayName);
                    }
                }
            }
        }

        public static ConfigNPC GetNPC(string npc)
        {
            if (Data.ContainsKey(npc)) return Data[npc];
            return NoData;
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

        public int PercentChance()
        {
            switch (Rarity)
            /*
            int very_rare = 1; //wrong house o.O - otherspots
            int rare = 2; //wrong part of the house - otherspots
            int normal = 4; //bedroom, usually - homespots
            */
            {
                case "very_rare":
                    return 1;
                case "rare":
                    return 2;
                case "normal":
                    return 4;
                case "always":
                    return 100;
                case "never":
                    return -1;
                default:
                    return 4;
            }
        }
    }

    class ConfigLocations
    {
        public static List<ConfigLocation> Data;
        public static bool Ready;
        public static void Load(string directory)
        {
            string filepath = Path.Combine(directory, "assets", "locations.json");
            if (! File.Exists(filepath))
            {
                Modworks.Log.Error($"Location file does not exist at {filepath}");

            }
            if (File.Exists(filepath))
            {
                try
                {
                    var filecontents = File.ReadAllText(filepath);
                    Data = JsonConvert.DeserializeObject<List<ConfigLocation>>(filecontents);
                    Ready = true;
                }
                catch (Exception e)
                {
                    Modworks.Log.Error("Failed to read location file: " + e.Message);
                }
            }
        }
    }
}