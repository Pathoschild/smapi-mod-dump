/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stellarashes/SDVMods
**
*************************************************/

using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkbenchAnywhere.Utils
{
    public static class ChestExtensions
    {
        public static bool HasTag(this Chest chest, string tag)
        {
            return chest.Name?.Contains($"|{tag}|") ?? false;
        }

        /// <summary>
        /// Toggle tag and returns whether the chest has tag after the toggle
        /// </summary>
        /// <param name="chest"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool ToggleTag(this Chest chest, string tag)
        {
            string wrappedTag = $"|{tag}|";
            if (chest.HasTag(tag))
            {
                chest.Name = chest.Name.Replace(wrappedTag, "");
                return false;
            }
            else
            {
                chest.Name += wrappedTag;
                return true;
            }
        }
    }
}
