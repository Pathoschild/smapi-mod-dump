/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://sourceforge.net/p/sdvmods-artifact-fix-redux/
**
*************************************************/

/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;

using Harmony;

using StardewModdingAPI;

namespace ArtifactSystemFixed_Redux
    {
    class ASFR_Mod : Mod
        {
        internal static Mod Instance;
        internal static IMonitor ModMonitor;
        internal static IManifest Manifest;

        internal static ASFRedux ASFRedux;

        public override void Entry(IModHelper helper) {
            ASFR_Mod.Instance = this;
            ASFR_Mod.ModMonitor = this.Monitor;
            ASFR_Mod.Manifest = this.ModManifest;

            Version myver = typeof(ASFR_Mod).Assembly.GetName().Version;
            Version harver = typeof(HarmonyInstance).Assembly.GetName().Version;
            string phrev = PatcherHelper.Meta.Revision;
            Log.Info($"Version {myver} using Harmony {harver} and PatcherHelper {phrev}");

            ASFRedux = new ASFRedux(this);
            }
        }
    }
