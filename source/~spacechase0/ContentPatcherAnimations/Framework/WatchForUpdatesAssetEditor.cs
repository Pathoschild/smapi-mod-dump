/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace ContentPatcherAnimations.Framework
{
    // TODO: Optimize this
    /// <summary>An asset editor which detects when an animated texture changes.</summary>
    internal class WatchForUpdatesAssetEditor
    {
        /*********
        ** Fields
        *********/
        /// <summary>Get the patch and animation data for loaded patches.</summary>
        private readonly Func<IDictionary<Patch, PatchData>> GetAnimatedPatches;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="getAnimatedPatches">Get the patch and animation data for loaded patches.</param>
        public WatchForUpdatesAssetEditor(Func<IDictionary<Patch, PatchData>> getAnimatedPatches)
        {
            this.GetAnimatedPatches = getAnimatedPatches;
        }

#warning - ignoring locale for now to match CP.
        public void Ready(AssetReadyEventArgs e)
        {
            var animatedPatches = this.GetAnimatedPatches();

            foreach (PatchData patch in animatedPatches.Values)
            {
                if (patch.TargetName != null && e.NameWithoutLocale.IsEquivalentTo(patch.TargetName))
                    patch.ForceNextRefresh = true;
            }
        }

    }
}
