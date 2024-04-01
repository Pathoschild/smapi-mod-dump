/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley.Menus;
using StardewValley.Objects;
using System.Collections.Generic;

namespace AllChestsMenu
{
    public class StorageData
    {
        public string location;
        public string name;
        public string label;
        public int index;
        public InventoryMenu menu;
        public List<ClickableTextureComponent> inventoryButtons = new List<ClickableTextureComponent>();
        public bool collapsed;
        public Chest chest;
        public Vector2 tile;
    }
}