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
