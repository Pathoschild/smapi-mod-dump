/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JhonnieRandler/TVBrasileira
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using TVBrasileira.frameworks;

namespace TVBrasileira.channels
{
    public abstract class Channel
    {
        protected readonly IModHelper Helper;
        protected readonly IMonitor Monitor;

        protected List<string> TargetDialogueAssets;
        protected List<string> TargetImageAssets;

        protected Channel(IModHelper helper, IMonitor monitor)
        {
            Helper = helper;
            Monitor = monitor;
        }

        /// <summary>
        /// Determines whether the current channel is enabled based on its toggle property stored in the configuration file.
        /// The toggle property is dynamically retrieved based on the name of the channel class, with the pattern "{ClassName}Toggle".
        /// If the toggle property is found, its value is returned, otherwise the method returns true.
        /// </summary>
        /// <returns>Boolean indicating if the channel is enabled.</returns>
        protected bool IsChannelEnabled()
        {
            var toggleName = $"{GetType().Name}Toggle";
            var modConfig = Helper.ReadConfig<ModConfig>();

            var toggleProperty = modConfig.GetType().GetProperty(toggleName);
            if (toggleProperty == null)
            {
                Monitor.Log("The ModConfig " + toggleName + " property returned null when trying to read mod options, probably due to bad orthography." + 
                            "\nKeeping broken channels enabled." +
                            "\nIf you're seeing this, please report at nexusmods.com/stardewvalley/mods/10843?tab=posts " +
                            "or github.com/JhonnieRandler/TVBrasileira/issues.", LogLevel.Error);
                return true;
            }

            var result = toggleProperty.GetValue(modConfig);
            if (result is bool toggleValue) return toggleValue;
            
            Monitor.Log("The ModConfig " + toggleName + " property is not boolean." +
                        "\nKeeping broken channels enabled." +
                        "\nIf you're seeing this the devs really f* it up this time /j, please report at nexusmods.com/stardewvalley/mods/10843?tab=posts " +
                        "or github.com/JhonnieRandler/TVBrasileira/issues.", LogLevel.Error);
            return true;
        }

        /// <summary>
        /// Check if the requested asset is equivalent to a target dialogue asset.
        /// If equivalent, edit the asset to set custom dialogues.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments that contain information about the requested asset.</param>
        protected void CheckTargetDialogues(object sender, AssetRequestedEventArgs e)
        {
            var requestedAssetName = e.NameWithoutLocale;
            foreach (var targetAsset in TargetDialogueAssets)
            {
                if (!requestedAssetName.IsEquivalentTo(targetAsset)) continue;
                e.Edit(asset =>
                {
                    var editor = asset.AsDictionary<string, string>();
                    SetCustomDialogues(editor, requestedAssetName);
                });
                break;
            }
        }

        /// <summary>
        /// Sets custom dialogues for the specified asset.
        /// </summary>
        /// <param name="editor">The editor for the asset data as a dictionary.</param>
        /// <param name="assetName">The name of the asset.</param>
        protected abstract void SetCustomDialogues(IAssetDataForDictionary<string, string> editor, IAssetName assetName);
        
        /// <summary>
        /// Check if the requested asset is equivalent to a target image asset.
        /// If equivalent, edit the asset to set custom images.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments that contain information about the requested asset.</param>
        protected void CheckTargetImages(object sender, AssetRequestedEventArgs e)
        {
            var requestedAssetName = e.NameWithoutLocale;
            foreach (var targetAsset in TargetImageAssets)
            {
                if (!requestedAssetName.IsEquivalentTo(targetAsset)) continue;
                e.Edit(asset =>
                {
                    var editor = asset.AsImage();
                    SetCustomImages(editor, requestedAssetName);
                });
                break;
            }
        }
        
        /// <summary>
        /// Sets custom images for the specified asset.
        /// </summary>
        /// <param name="editor">The editor for the asset data as an image.</param>
        /// <param name="assetName">The name of the asset.</param>
        protected abstract void SetCustomImages(IAssetDataForImage editor, IAssetName assetName);
        
        /// <summary>
        /// Invalidates the cache for the target assets in the `TargetDialogueAssets` list.
        /// </summary>
        protected void InvalidateDialogues()
        {
            foreach (var targetAsset in TargetDialogueAssets)
            {
                string currentLocale =
                    Helper.GameContent.CurrentLocale != "" ? "." + Helper.GameContent.CurrentLocale : "";

                Helper.GameContent.InvalidateCache(targetAsset);
                Helper.GameContent.InvalidateCache(targetAsset + currentLocale);
            }
            Monitor.Log("Assets being invalidated twice is expected and part of how the mod works." +
                        "\nThere's also a chance you'll see an asset being invalidated and a very similarly named asset" +
                        "\n failing to invalidate right after." +
                        "\nIf you've found this through a log or other means, " +
                        "no need to point this as an error at first sight.", LogLevel.Debug);
        }
    }
}