/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace OrnithologistsGuild.Content
{
    public class ContentManager
    {
        public static Models.BathDef[] Baths;
        public static Models.FeederDef[] Feeders;

        public static Models.FoodDef[] Foods;

        public static string[] BathIds;
        public static string[] FeederIds;

        public static Dictionary<string, string[]> DefaultBiomes;

        public static void Initialize()
        {
            Baths = ModEntry.Instance.Helper.Data.ReadJsonFile<Models.BathDef[]>("baths.json");
            Feeders = ModEntry.Instance.Helper.Data.ReadJsonFile<Models.FeederDef[]>("feeders.json");

            Foods = ModEntry.Instance.Helper.Data.ReadJsonFile<Models.FoodDef[]>("foods.json");

            BathIds = Baths.Select(b => b.ID).ToArray();
            FeederIds = Feeders.Select(f => f.ID).ToArray();

            DefaultBiomes = ModEntry.Instance.Helper.Data.ReadJsonFile<Dictionary<string, string[]>>("default-biomes.json");
        }
    }
}

