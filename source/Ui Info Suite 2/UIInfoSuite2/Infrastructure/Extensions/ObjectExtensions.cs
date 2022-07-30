/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Annosz/UiInfoSuite2
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;

namespace UIInfoSuite2.Infrastructure.Extensions
{
    public static class ObjectExtensions
    {
        #region Properties
        private static readonly Dictionary<string, int> NpcHeadShotSize = new()
        {
            { "Piere", 9 },
            { "Sebastian", 7 },
            { "Evelyn", 5 },
            { "Penny", 6 },
            { "Jas", 6 },
            { "Caroline", 5 },
            { "Dwarf", 5 },
            { "Sam", 9 },
            { "Maru", 6 },
            { "Wizard", 9 },
            { "Jodi", 7 },
            { "Krobus", 7 },
            { "Alex", 8 },
            { "Kent", 10 },
            { "Linus", 4 },
            { "Harvey", 9 },
            { "Shane", 8 },
            { "Haley", 6 },
            { "Robin", 7 },
            { "Marlon", 2 },
            { "Emily", 8 },
            { "Marnie", 5 },
            { "Abigail", 7 },
            { "Leah", 6 },
            { "George", 5 },
            { "Elliott", 9 },
            { "Gus", 7 },
            { "Lewis", 8 },
            { "Demetrius", 11 },
            { "Pam", 5 },
            { "Vincent", 6 },
            { "Sandy", 7 },
            { "Clint", 10 },
            { "Willy", 10 }
        };
        #endregion

        public static Rectangle GetHeadShot(this NPC npc)
        {
            int size;
            if (!NpcHeadShotSize.TryGetValue(npc.Name, out size))
                size = 4;

            Rectangle mugShotSourceRect = npc.getMugShotSourceRect();
            mugShotSourceRect.Height -= size / 2;
            mugShotSourceRect.Y -= size / 2;
            return mugShotSourceRect;
        }

        public static string SafeGetString(this IModHelper helper, string key)
        {
            var result = string.Empty;

            if (!string.IsNullOrEmpty(key) &&
                helper != null)
            {
                result = helper.Translation.Get(key);
            }

            return result;
        }
    }
}
