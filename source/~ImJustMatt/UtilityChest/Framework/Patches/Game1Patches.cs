/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using Harmony;
using ImJustMatt.Common.Patches;
using StardewModdingAPI;
using StardewValley;

namespace ImJustMatt.UtilityChest.Framework.Patches
{
    internal class Game1Patches : BasePatch<UtilityChest>
    {
        private static IInputHelper InputHelper;

        public Game1Patches(IMod mod, HarmonyInstance harmony) : base(mod, harmony)
        {
            InputHelper = Mod.Helper.Input;

            harmony.Patch(
                AccessTools.Method(typeof(Game1), nameof(Game1.pressSwitchToolButton)),
                new HarmonyMethod(GetType(), nameof(PressSwitchToolButtonPrefix))
            );
        }

        private static bool PressSwitchToolButtonPrefix()
        {
            return !InputHelper.IsDown(SButton.LeftShift) && !InputHelper.IsDown(SButton.RightShift);
        }
    }
}