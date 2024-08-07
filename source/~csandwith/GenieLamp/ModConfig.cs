/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace GenieLamp
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public string LampItem { get; set; } = "Golden Mask";
        public string MenuSound { get; set; } = "cowboy_explosion";
        public string WishSound { get; set; } = "yoba";
        public int WishesPerItem { get; set; } = 3;
    }
}
