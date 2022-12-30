/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using AeroCore.Utils;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;

namespace AeroCore.Patches
{
    [ModInit]
    [HarmonyPatch(typeof(Game1))]
    internal class FadeHooks
    {
        internal static event Action<int> AfterFadeOut;
        internal static event Action<int> AfterFadeIn;
        private static IReflectedField<ScreenFade> fadeField;
        internal static readonly PerScreen<ScreenFade> gameFade = new(() => fadeField.GetValue());

        internal static void Init()
        {
            fadeField = ModEntry.helper.Reflection.GetField<ScreenFade>(typeof(Game1), "screenFade");
        }

        [HarmonyPatch("onFadeToBlackComplete")]
        [HarmonyPostfix]
        internal static void FadeOut()
        {
            AfterFadeOut?.Invoke(Context.ScreenId);
        }

        [HarmonyPatch("onFadedBackInComplete")]
        [HarmonyPostfix]
        internal static void FadeIn()
        {
            AfterFadeIn?.Invoke(Context.ScreenId);
        }
    }
}
