/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/StardewSandbox
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;

namespace HatShopRestoration.Framework.Patches
{
    internal abstract class PatchTemplate
    {
        internal static IMonitor _monitor;
        internal static IModHelper _helper;

        internal PatchTemplate(IMonitor modMonitor, IModHelper modHelper)
        {
            _monitor = modMonitor;
            _helper = modHelper;
        }

        internal abstract void Apply(Harmony harmony);
    }
}