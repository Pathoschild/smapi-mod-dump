/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

namespace CustomCraftingStations.Framework
{
    public class CustomCraftingStationsApi : ICustomCraftingStationsApi
    {
        public void SetCCSCraftingMenuOverride(bool menuOverride)
        {
            ModEntry.MenuOverride = menuOverride;
        }
    }
}
