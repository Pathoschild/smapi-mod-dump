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
    /// Relative paths to all graphics folders for items for mod content.
    /// </summary>
    public class ItemsGraphicsPaths
    {
        public static string Items = Path.Combine(RelativePaths.Graphics_Folder, "Items");

        public static string Crafting = Path.Combine(Items, "Crafting");
        public static string Farming = Path.Combine(Items, "Farming");
        public static string Misc = Path.Combine(Items, "Misc");


        public static string Resources = Path.Combine(Items, "Resources");
        public static string Resources_Misc = Path.Combine(Resources, "Misc");
        public static string Resources_Ore = Path.Combine(Resources, "Ore");

        public static string Tools = Path.Combine(Items, "Tools");
    }
}
