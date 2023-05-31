/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/


using HarmonyLib;
using StardewValley;
using StardewValley.Menus;
using System;

namespace MultiplayerMod.Framework.Patch.Mobile
{
    internal class MobileCustomizerPatch : IPatch
    {
        public Type PATCH_TYPE { get; }

        public MobileCustomizerPatch()
        {
            PATCH_TYPE = typeof(IClickableMenu).Assembly.GetType("StardewValley.Menus.MobileCustomizer");
        }

        public void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(PATCH_TYPE, "receiveLeftClick", new Type[] { typeof(int), typeof(int), typeof(bool) }), postfix: new HarmonyMethod(this.GetType(), nameof(postfix_receiveLeftClick)));
            harmony.Patch(AccessTools.Method(PATCH_TYPE, "optionButtonClick", new Type[] { typeof(string) }), postfix: new HarmonyMethod(this.GetType(), nameof(postfix_optionButtonClick)));
            harmony.Patch(AccessTools.Constructor(PATCH_TYPE), postfix: new HarmonyMethod(this.GetType(), nameof(postfix_ctor)));
        }

        public static bool skipIntro { get; internal set; } = false;

        private static void postfix_receiveLeftClick(object __instance, int x, int y, bool playSound = true)
        {
            skipIntro = ModUtilities.Helper.Reflection.GetField<bool>(__instance, "skipIntro").GetValue();
        }
        private static void postfix_ctor(object __instance)
        {
            skipIntro = false;
        }
        private static void postfix_optionButtonClick(string name)
        {
            if (name == "OK")
            {
                if (Game1.client != null)
                {
                    Game1.player.isCustomized.Value = true;
                }
            }
        }
    }
}
