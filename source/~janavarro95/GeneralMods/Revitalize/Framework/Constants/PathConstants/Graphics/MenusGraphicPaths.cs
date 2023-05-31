/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omegasis.Revitalize.Framework.Constants.PathConstants.Graphics
{
    /// <summary>
    /// Relative paths for all menu/hud content for the mod's assets.
    /// </summary>
    public class MenusGraphicPaths
    {
        public static string Menus = Path.Combine(RelativePaths.Graphics_Folder, "Menus");

        public static string CraftingMenu = Path.Combine(Menus, "CraftingMenu");
        public static string EnergyMenu = Path.Combine(Menus, "EnergyMenu");
        public static string InventoryMenu = Path.Combine(Menus, "InventoryMenu");
        public static string DimensionalStorageMenu = Path.Combine(Menus, "DimensionalStorageMenu");

        public static string Misc = Path.Combine(Menus, "Misc");

    }
}
