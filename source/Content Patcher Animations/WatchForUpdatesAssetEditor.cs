/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/ContentPatcherAnimations
**
*************************************************/

using StardewModdingAPI;
using System;

namespace ContentPatcherAnimations
{
    // TODO: Optimize this
    internal class WatchForUpdatesAssetEditor : IAssetEditor
    {
        public WatchForUpdatesAssetEditor()
        {
        }

        public bool CanEdit<T>( IAssetInfo asset )
        {
            if ( Mod.instance.ScreenState == null )
                return false;

            foreach ( var patchEntry in Mod.instance.ScreenState.animatedPatches )
            {
                var patch = patchEntry.Value.patchObj;
                var target = Mod.instance.Helper.Reflection.GetProperty<string>( patch, "TargetAsset" ).GetValue();
                if ( !string.IsNullOrWhiteSpace( target ) && asset.AssetNameEquals( target ) )
                    return true;
            }
            return false;
        }

        public void Edit<T>( IAssetData asset )
        {
            if ( Mod.instance.ScreenState == null )
                return;

            foreach ( var patchEntry in Mod.instance.ScreenState.animatedPatches )
            {
                var patch = patchEntry.Value.patchObj;
                var target = Mod.instance.Helper.Reflection.GetProperty<string>( patch, "TargetAsset" ).GetValue();
                if ( !string.IsNullOrWhiteSpace( target ) && asset.AssetNameEquals( target ) )
                {
                    Mod.instance.ScreenState.findTargetsQueue.Enqueue( patchEntry.Key );
                }
            }
        }
    }
}