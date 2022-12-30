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
namespace OrnithologistsGuild.Content
{
    public class ContentManager
    {
        public static Models.FeederDef[] Feeders;
        public static Models.FoodDef[] Foods;

        public static void Initialize()
        {
            Feeders = ModEntry.Instance.Helper.Data.ReadJsonFile<Models.FeederDef[]>("feeders.json");
            Foods = ModEntry.Instance.Helper.Data.ReadJsonFile<Models.FoodDef[]>("foods.json");
        }
    }
}

