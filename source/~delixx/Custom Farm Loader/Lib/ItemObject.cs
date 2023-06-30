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
using StardewModdingAPI;

namespace Custom_Farm_Loader.Lib
{
    public class ItemObject
    {
        private static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        private static Dictionary<int, string> CachedItemData;

        public string Id;
        public int Amount;
        public int Quality;

        public static void Initialize(Mod mod)
        {
            Mod = mod;
            Monitor = mod.Monitor;
            Helper = mod.Helper;

            CachedItemData = Helper.GameContent.Load<Dictionary<int, string>>("Data\\ObjectInformation");
        }

        public static List<string> MapNameToParentsheetindex(List<string> names)
        {
            List<string> ret = new List<string>();

            names.ForEach(name => ret.Add(MapNameToParentsheetindex(name)));
            return ret;
        }

        public static string MapNameToParentsheetindex(string name)
        {
            var comparableName = name.ToLower().Replace("_", " ").Replace("'", "");
            var match = CachedItemData.FirstOrDefault(fur => fur.Value.ToLower().Replace("'", "").StartsWith(comparableName + "/"));

            if (match.Value != null)
                return match.Key.ToString();

            return name;
        }

        public static string GetItemData(string name, int index)
        {

            if (int.TryParse(name, out int id)) {
                var item = CachedItemData[id];

                if (item != null)
                    return item.Split('/')[index];
            }

            return "";
        }
    }
}
