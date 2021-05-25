/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://sourceforge.net/p/sdvmod-24h-harmony/
**
*************************************************/

/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Harmony;
using StardewModdingAPI;

namespace _24H_Harmony
    {
    public class ModEntry : Mod
        {
        public override void Entry(IModHelper helper) {
            Version myver = typeof(ModEntry).Assembly.GetName().Version;
            Version harver = typeof(HarmonyInstance).Assembly.GetName().Version;
            string phrev = PatcherHelper.Meta.Revision;
            Monitor.Log($"Version {myver} using Harmony {harver} and PatcherHelper {phrev}", LogLevel.Info);

            Patcher.Initialize(ModManifest.UniqueID, Monitor);
            try {
                Patcher.Execute();
                Monitor.Log("Patching complete. Enjoy your 24-hour clock!", LogLevel.Info);
                }
            catch (Exception ex) {
                Monitor.Log($"Failure in patching! Technical details:\n{ex}", LogLevel.Error);
                Monitor.Log("24-Hour Clock might not work properly!", LogLevel.Warn);
                }

            }

        }
    }
