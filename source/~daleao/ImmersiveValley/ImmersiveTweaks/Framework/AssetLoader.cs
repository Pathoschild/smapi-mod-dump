/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Tweaks.Framework;

#region using directives

using System;
using System.IO;
using StardewModdingAPI;

#endregion using directives

/// <summary>Loads custom mod textures.</summary>
public class AssetLoader : IAssetLoader
{
    /// <inheritdoc />
    public bool CanLoad<T>(IAssetInfo asset)
    {
        return asset.AssetName.Contains(ModEntry.Manifest.UniqueID) && asset.AssetName.Contains("BetterHoneyMeadIcons");
    }

    /// <inheritdoc />
    public T Load<T>(IAssetInfo asset)
    {
        var assetName = asset.AssetName.Split('/')[1];
        if (assetName == "BetterHoneyMeadIcons")
            return ModEntry.ModHelper.Content.Load<T>(Path.Combine("assets",
                "mead-" + ModEntry.Config.HoneyMeadStyle.ToLower() + ".png"));
        
        throw new InvalidOperationException($"Unexpected asset '{assetName}'.");
    }
}