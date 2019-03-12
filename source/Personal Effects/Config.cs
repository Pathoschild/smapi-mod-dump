using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Modworks = bwdyworks.Modworks;

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

        public string GetPronoun(int index)
        {
            switch (index) {
                case 2:
                    if (IsFemale) return "her";
                    else return "his";
                case 1:
                    if (IsFemale) return "her";
                    else return "him";
                default:
                    if (IsFemale) return "she";
                    else return "he";
            } 
        }

        public bool HasMaleItems()
        {
            return IsFemale == CrossDress;
        }

        public string Abbreviate()
        {
            string abbr = new string(new string(InternalName.ToLower().Where(c => !"aeiouy".Contains(c)).ToArray()).ToCharArray().Distinct().ToArray());
            return abbr;
        }
    }

    class Config
    {
        public static Dictionary<string, ConfigNPC> Data;
        private static ConfigNPC NoData;
        public static bool ready = false;
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
                    ready = true;
                }
                catch (Exception e)
                {
                    Modworks.Log.Error("Failed to read config file: " + e.Message);
                }
                //set internal names
                foreach(var kvp in Data)
                {
                    kvp.Value.InternalName = kvp.Key;

                    //child safety - if any configured NPCs are children, we'll disable them here
                    var n = StardewValley.Game1.getCharacterFromName(kvp.Value.InternalName);
                    if (n == null) continue; //if we can't read it, we'll let it pass.
                    if (Modworks.NPCs.IsChild(n)) kvp.Value.Enabled = false;
                }

                foreach(ConfigNPC cnpc in Data.Values)
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
            foreach(var npc in Data)
            {
                if (npc.Value.DisplayName.Equals(displayName))
                {
                    return npc.Key;
                }
            }
            return null;
        }
    }
}
