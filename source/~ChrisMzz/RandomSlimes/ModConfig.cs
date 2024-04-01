/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChrisMzz/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace RandomSlimes
{
    public class ModConfig
    {
        public bool InWoods { get; set; } = false;
        public bool InSlimeArea { get; set; } = true;
        public bool InQuarryArea { get; set; } = true;
        public bool InSkullCaverns { get; set; } = true;
        public bool InRegularMines { get; set; } = true;

        public bool Anywhere { get; set; } = false;


    }
}