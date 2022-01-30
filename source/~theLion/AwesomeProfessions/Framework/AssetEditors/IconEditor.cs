/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.AssetEditors;

#region using directives

using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Edits <c>LooseSprites/Cursors</c> and <c>TileSheets/BuffIcons</c> with new profession icons.</summary>
public class IconEditor : IAssetEditor
{
    private readonly Texture2D _tileSheet =
        ModEntry.ModHelper.Content.Load<Texture2D>(Path.Combine("assets", "sprites", "tilesheet.png"));

    /// <inheritdoc />
    public bool CanEdit<T>(IAssetInfo asset)
    {
        return asset.AssetNameEquals(PathUtilities.NormalizeAssetName("LooseSprites/Cursors")) ||
               asset.AssetNameEquals(PathUtilities.NormalizeAssetName("TileSheets/BuffsIcons"));
    }

    /// <inheritdoc />
    public void Edit<T>(IAssetData asset)
    {
        if (asset.AssetNameEquals(PathUtilities.NormalizeAssetName("LooseSprites/Cursors")))
        {
            // patch modded profession icons
            var editor = asset.AsImage();
            var srcArea = new Rectangle(0, 0, 96, 80);
            var targetArea = new Rectangle(0, 624, 96, 80);

            editor.PatchImage(_tileSheet, srcArea, targetArea);
        }
        else if (asset.AssetNameEquals(PathUtilities.NormalizeAssetName("TileSheets/BuffsIcons")))
        {
            // patch modded profession buff icons
            var editor = asset.AsImage();
            editor.ExtendImage(192, 80);
            var srcArea = new Rectangle(0, 80, 96, 32);
            var targetArea = new Rectangle(0, 48, 96, 32);

            editor.PatchImage(_tileSheet, srcArea, targetArea);
        }
        else
        {
            throw new InvalidOperationException($"Unexpected asset {asset.AssetName}.");
        }
    }
}