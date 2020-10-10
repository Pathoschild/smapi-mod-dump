/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using System.Reflection;

namespace CasksEverywhere
{
    public class CasksEverywhereMod : Mod
    {
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            HarmonyInstance instance = HarmonyInstance.Create(helper.ModRegistry.ModID);

            instance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
