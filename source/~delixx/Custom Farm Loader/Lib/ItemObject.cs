/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/custom-farm-loader
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;

namespace Custom_Farm_Loader.Lib
{
    public class ItemObject
    {
        private static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public string Id;
        public int Amount;
        public int Quality;

        public static void Initialize(Mod mod)
        {
            Mod = mod;
            Monitor = mod.Monitor;
            Helper = mod.Helper;
        }

        public static List<string> MapNameToItemId(List<string> names)
        {
            List<string> ret = new List<string>();

            names.ForEach(name => ret.Add(MapNameToItemId(name)));
            return ret;
        }

        public static string MapNameToItemId(string name)
        {
            var comparableName = name.ToLower().Replace("_", " ").Replace("'", "");
            var match = Game1.objectData.FirstOrDefault(fur => fur.Value.Name.ToLower().Replace("'", "").StartsWith(comparableName));

            if (match.Value != null)
                return match.Key;

            return name;
        }
    }
}
