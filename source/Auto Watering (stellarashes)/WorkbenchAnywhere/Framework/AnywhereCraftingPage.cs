/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stellarashes/SDVMods
**
*************************************************/

using StardewValley.Menus;
using StardewValley.Objects;
using System.Collections.Generic;

namespace WorkbenchAnywhere.Framework
{
    public class AnywhereCraftingPage : CraftingPage
    {
        public AnywhereCraftingPage(int x, int y, int width, int height, bool cooking = false, bool standalone_menu = false, List<Chest> material_containers = null):
            base(x, y, width, height, cooking, standalone_menu, material_containers)
        {
        }
    }
}
