using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lockpicks
{
    public class ConfigLockEnd
    {
        public string MapName;
        public int MapX;
        public int MapY;
        public ConfigLockEnd() { }
        public ConfigLockEnd(string mn, int x, int y)
        {
            MapName = mn;
            MapX = x;
            MapY = y;
        }

        public ConfigLockEnd(string str)
        {
            string[] ss = str.Split('.');
            MapName = ss[0];
            MapX = int.Parse(ss[1]);
            MapY = int.Parse(ss[2]);
        }

        public string str()
        {
            return MapName + "." + MapX + "." + MapY;
        }
    }

    class Config
    {
        public static Dictionary<string, string> Data;
        public static bool ready = false;
        public static void Load()
        {
            string filepath = Mod.instance.Helper.DirectoryPath + Path.DirectorySeparatorChar + "config.json";
            if (File.Exists(filepath))
            {
                try
                {
                    string filecontents = File.ReadAllText(filepath);
                    Dictionary<string, int> config = JsonConvert.DeserializeObject<Dictionary<string, int>>(filecontents);
                    Item.base_id = config["item_id"];
                }
                catch (Exception e)
                {
                    Item.base_id = 1919;
                }
            } else
            {
                Item.base_id = 1919;
            }
            filepath = Mod.instance.Helper.DirectoryPath + Path.DirectorySeparatorChar + "locks.json";
            if (File.Exists(filepath))
            {
                try
                {
                    string filecontents = File.ReadAllText(filepath);
                    Data = JsonConvert.DeserializeObject<Dictionary<string, string>>(filecontents);
                    ready = true;
                }
                catch (Exception e)
                {
                    Mod.instance.Monitor.Log("Failed to read config file: " + e.Message, StardewModdingAPI.LogLevel.Error);
                }
            }
        }

        public static ConfigLockEnd GetMatchingLockEnd(ConfigLockEnd cle)
        {
            string key = cle.str();
            if (Data.ContainsKey(key)) return new ConfigLockEnd(Data[key]);
            return null;
        }
    }
}
