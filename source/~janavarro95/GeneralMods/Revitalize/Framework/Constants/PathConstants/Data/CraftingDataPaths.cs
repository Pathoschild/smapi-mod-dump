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

namespace Omegasis.Revitalize.Framework.Constants.PathConstants.Data
{
    public class CraftingDataPaths
    {
        public static string CraftingPath = Path.Combine(RelativePaths.ModAssets_Data_Folder, "Crafting");
        public static string CraftingStationsPath = Path.Combine(CraftingPath, "CraftingStations");
        public static string CraftingStationTemplatesPath = Path.Combine(CraftingStationsPath, RelativePaths.TemplatesFoldersName);

        public static string RevitalizeMachinesPath = Path.Combine(CraftingPath, "RevitalizeMachines");
        public static string StardewValleyMachinesPath = Path.Combine(CraftingPath, "StardewValleyMachinesMachines");

    }
}
