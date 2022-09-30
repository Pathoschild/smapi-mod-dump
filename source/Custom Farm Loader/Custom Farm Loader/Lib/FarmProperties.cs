/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-custom-farm-loader
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Custom_Farm_Loader.Lib.Enums;
using Microsoft.Xna.Framework;
using StardewValley;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;

namespace Custom_Farm_Loader.Lib
{
    public class FarmProperties
    {
        private static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public bool FishSplashing = false;
        public bool SpawnMonstersAtNight = false;

        public static void Initialize(Mod mod)
        {
            Mod = mod;
            Monitor = mod.Monitor;
            Helper = mod.Helper;
        }

        public static FarmProperties parseJObject(JProperty jObj)
        {
            FarmProperties farmProperties = new FarmProperties();

            foreach (JProperty jProperty in jObj.First()) {
                if (jProperty.Value.Type == JTokenType.Null)
                    continue;

                string name = jProperty.Name;
                string value = jProperty.Value.ToString();

                switch (name.ToLower()) {
                    case "fishsplashing":
                        farmProperties.FishSplashing = bool.Parse(value); break;
                    case "spawnmonstersatnight":
                        farmProperties.SpawnMonstersAtNight = bool.Parse(value); break;
                    default:
                        Monitor.Log("Unknown Properties Attribute", LogLevel.Error);
                        throw new ArgumentException($"Unknown Properties Attribute", name);
                }

            }

            return farmProperties;
        }
    }
}
