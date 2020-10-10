/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using System.Reflection;

namespace CustomFurnitureAnywhere
{
    public class CustomFurnitureAnywhereMod : Mod
    {
        public static IModHelper modhelper;
        public static IMonitor modmonitor;

        public override void Entry(IModHelper helper)
        {
            modhelper = helper;
            modmonitor = Monitor;
            harmonyFix();
        }

        public void harmonyFix()
        {
            var instance = HarmonyInstance.Create("Platonymous.CustomFurnitureAnywhere");
            instance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
