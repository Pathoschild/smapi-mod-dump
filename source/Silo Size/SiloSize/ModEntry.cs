/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://sourceforge.net/p/sdvmod-silo-size/
**
*************************************************/

/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using StardewModdingAPI;
using System;
using Harmony;

namespace SiloSize
    {
    public class ModEntry : Mod
        {
        internal SiloSize siloSize;

        public override void Entry(IModHelper helper) {
            Version myver = typeof(ModEntry).Assembly.GetName().Version;
            Version harver = typeof(HarmonyInstance).Assembly.GetName().Version;
            string phrev = PatcherHelper.Meta.Revision;
            Monitor.Log($"Version {myver} using Harmony {harver} and PatcherHelper {phrev}", LogLevel.Info);

            siloSize = new SiloSize(this);

            Helper.Events.GameLoop.GameLaunched += siloSize.OnGameLaunched;
            Helper.Events.GameLoop.DayStarted += siloSize.OnDayStarted;
            }
        }
    }
