/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace ContentPatcherAnimations.Framework
{
    // TODO: Optimize this
    internal class WatchForUpdatesAssetEditor : IAssetEditor
    {
        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (Mod.Instance.ScreenState == null)
                return false;

            foreach (var patchEntry in Mod.Instance.ScreenState.AnimatedPatches)
            {
                object patch = patchEntry.Value.PatchObj;
                string target = Mod.Instance.Helper.Reflection.GetProperty<string>(patch, "TargetAsset").GetValue();
                if (!string.IsNullOrWhiteSpace(target) && asset.AssetNameEquals(target))
                    return true;
            }
            return false;
        }

        public void Edit<T>(IAssetData asset)
        {
            if (Mod.Instance.ScreenState == null)
                return;

            foreach (var patchEntry in Mod.Instance.ScreenState.AnimatedPatches)
            {
                object patch = patchEntry.Value.PatchObj;
                string target = Mod.Instance.Helper.Reflection.GetProperty<string>(patch, "TargetAsset").GetValue();
                if (!string.IsNullOrWhiteSpace(target) && asset.AssetNameEquals(target))
                {
                    Mod.Instance.ScreenState.FindTargetsQueue.Enqueue(patchEntry.Key);
                }
            }
        }
    }
}
