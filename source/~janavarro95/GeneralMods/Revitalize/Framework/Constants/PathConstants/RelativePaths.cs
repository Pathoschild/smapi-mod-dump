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

namespace Omegasis.Revitalize.Framework.Constants.PathConstants
{
    public static class RelativePaths
    {
        public static string ModAssetsFolder = "ModAssets";
        public static string TemplatesFoldersName = "_Templates";


        public static string Graphics_Folder = Path.Combine(RelativePaths.ModAssetsFolder, "Graphics");

        public static string ModAssets_Strings_Folder = Path.Combine(ModAssetsFolder, "Strings");

        public static string ModAssets_Data_Folder = Path.Combine(ModAssetsFolder, "Data");

        public static string ModAssets_Maps_Folder = Path.Combine(ModAssetsFolder, "Maps");
    }
}
