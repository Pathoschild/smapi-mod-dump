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
    /// All relative paths to all graphics content for the mods's assets.
    /// </summary>
    public class ObjectsGraphicsPaths
    {
        public static string Objects = Path.Combine(RelativePaths.Graphics_Folder, "Objects");

        public static string Crafting = Path.Combine(Objects, "Crafting");
        public static string Farming = Path.Combine(Objects, "Farming");
        public static string Furniture = Path.Combine(Objects, "Furniture");
        public static string Machines = Path.Combine(Objects, "Machines");
        public static string Misc = Path.Combine(Objects, "Misc");

        public static string Resources = Path.Combine(Objects, "Resources");
        public static string Storage = Path.Combine(Objects, "Storage");
        public static string Resources_Ore = Path.Combine(Resources, "Ore");
        public static string Resources_ResourcePlants = Path.Combine(Resources, "ResourcePlants");
    }
}
