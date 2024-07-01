/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jolly-Alpaca/PrismaticDinosaur
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using PrismaticDinosaur.Patches;

namespace PrismaticDinosaur
{
    internal sealed class ModEntry : Mod
    {
        internal static IMonitor ModMonitor { get; private set; } = null!;

        internal static IModHelper ModHelper { get; private set; } = null!;
        internal static Harmony Harmony { get; private set; } = null!;
        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;
            ModHelper = helper;

            var harmony = new Harmony(this.ModManifest.UniqueID);
            MonsterPatcher.Apply(ModMonitor, harmony);
            DinoMonsterPatcher.Apply(ModMonitor, harmony);
        }
    }
}
