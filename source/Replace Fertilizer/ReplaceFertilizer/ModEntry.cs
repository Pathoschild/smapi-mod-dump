/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jingshenSN2/ReplaceFertilizer
**
*************************************************/

using HarmonyLib;
using JingshenSN2.ReplaceFertilizer.Patches;
using StardewModdingAPI;

namespace JingshenSN2.ReplaceFertilizer
{
    internal sealed class ModEntry : Mod
    {
        private Harmony? _harmony;
        public static ModEntry? _instance;
        public override void Entry(IModHelper helper)
        {
            _instance = this;
            ApplyPatch();
        }

        public void Info(string message)
        {
            Monitor.Log(message, LogLevel.Info);
        }

        private void ApplyPatch()
        {
            _harmony = new Harmony(ModManifest.UniqueID);
            new HoeDirtPatch(Monitor).Apply(_harmony);
        }
    }
}