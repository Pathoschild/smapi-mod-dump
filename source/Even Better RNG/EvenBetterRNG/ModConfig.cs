/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://sourceforge.net/p/sdvmod-even-better-rng/
**
*************************************************/

/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using StardewModdingAPI.Utilities;

namespace EvenBetterRNG
{
    class ModConfig
    {
        /// <summary>PRNG Seed for XoShiRo128**; if set to "0" actually means "use lib's default method of getting seed"</summary>
        public int RNGSeed { get; set; } = 0;
        public KeybindList ReloadHotkey { get; set; } = KeybindList.Parse("RightShift + F5");
        public bool ReseedOnReload { get; set; } = false;
        public bool OverrideDailyLuck { get; set; } = true;
        public bool GaussianLuck { get; set; } = true;
        public float GaussianLuckStdDev { get; set; } = 30.3f;
        public KeybindList ExperimentalChangeTodaysLuckHotkey { get; set; } = KeybindList.Parse("RightControl + F5");
        public bool ExperimentalForcePerTick { get; set; } = false;
        }
    }
