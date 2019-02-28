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
        public string Name;
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
            string abbr = new string(new string(Name.ToLower().Where(c => !"aeiouy".Contains(c)).ToArray()).ToCharArray().Distinct().ToArray());
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
                Name = "{Unknown NPC}",
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
                foreach(ConfigNPC cnpc in Data.Values)
                {
                    if (cnpc.Enabled)
                    {
                        Modworks.Log.Trace("Enabled for NPC " + cnpc.Name);
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
                if (npc.Value.Name.Equals(displayName))
                {
                    return npc.Key;
                }
            }
            return null;
        }
    }
}
