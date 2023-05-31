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
    public class StringsPaths
    {
        /// <summary>
        /// Path constant to the error strings directory for the mod.
        /// </summary>
        public static string ErrorStrings = Path.Combine(RelativePaths.ModAssets_Strings_Folder, "ErrorStrings");

        public static string Buildings = Path.Combine(RelativePaths.ModAssets_Strings_Folder, "Buildings");
        public static string BuildingDisplayStrings = Path.Combine(Buildings, "DisplayStrings");

        public static string Mail = Path.Combine(RelativePaths.ModAssets_Strings_Folder, "Mail");
        public static string Objects = Path.Combine(RelativePaths.ModAssets_Strings_Folder, "Objects");
        public static string ShopDialogue = Path.Combine(RelativePaths.ModAssets_Strings_Folder, "ShopDialogue");
        public static string UI = Path.Combine(RelativePaths.ModAssets_Strings_Folder, "UI");
        public static string Menus = Path.Combine(UI, "Menus");
        public static string MenuComponents = Path.Combine(Menus, "MenuComponents");

        public static string ObjectDisplayStrings = Path.Combine(Objects, "DisplayStrings");


    }
}
