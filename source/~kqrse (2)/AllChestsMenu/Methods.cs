/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using Object = StardewValley.Object;

namespace AllChestsMenu
{
    public partial class ModEntry
    {
        public static void OpenMenu()
        {
            if (Config.ModEnabled && Context.IsPlayerFree)
            {
                Game1.activeClickableMenu = new StorageMenu();
                Game1.playSound("bigSelect");
            }
        }
    }
}