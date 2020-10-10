/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using System.Collections.Generic;

namespace CustomCraftingStation
{
    public class ContentPack
    {
        public List<CraftingStation> CraftingStations;
    }

    public class CraftingStation
    {
        public string BigCraftable { get; set; } //A big craftable to interact with to open the menu
        public string TileData { get; set; } //Name of the tiledata used to interact with to open the menu
        public bool ExclusiveRecipes { get; set; } = true; //Removes the listed recipes from the vanilla crafting menus
        public List<string> CraftingRecipes { get; set; } = new List<string>(); //list of recipe names
        public List<string> CookingRecipes { get; set; } = new List<string>();//list of recipe names

    }
}
