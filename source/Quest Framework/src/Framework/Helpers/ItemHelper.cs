/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Framework.Helpers
{
    internal static class ItemHelper
    {
        public static int GetWeaponId(string swordIdName)
        {
            Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\weapons");

            if (int.TryParse(swordIdName, out int id))
                return dictionary.Keys.Contains(id) ? id : -1;

            var swordData = from s in dictionary
                            where s.Value.StartsWith($"{swordIdName}/")
                            select s.Key;

            if (swordData.Any())
                return swordData.First();

            return -1;
        }

        public static int GetObjectId(string objIdName)
        {
            var dictionary = Game1.objectInformation;

            if (int.TryParse(objIdName, out int id))
                return dictionary.Keys.Contains(id) ? id : -1;

            var objectData = from s in dictionary
                            where s.Value.StartsWith($"{objIdName}/")
                            select s.Key;

            if (objectData.Any())
                return objectData.First();

            return -1;
        }
    }
}
