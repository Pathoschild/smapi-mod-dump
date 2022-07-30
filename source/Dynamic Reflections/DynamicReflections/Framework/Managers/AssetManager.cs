/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/DynamicReflections
**
*************************************************/

using DynamicReflections.Framework.Models;
using DynamicReflections.Framework.Models.Settings;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicReflections.Framework.Managers
{
    internal class AssetManager
    {
        internal Texture2D PuddlesTileSheetTexture { get; }
        internal Texture2D NightSkyTileSheetTexture { get; }
        internal string SkyEffectsTileSheetTexturePath { get; }

        public AssetManager(IModHelper helper)
        {
            // Get the asset folder path
            var assetFolderPath = helper.ModContent.GetInternalAssetName(Path.Combine("Framework", "Assets")).Name;

            // Load in the puddles tilesheet
            PuddlesTileSheetTexture = helper.ModContent.Load<Texture2D>(Path.Combine(assetFolderPath, "Textures", "puddles_sheet.png"));
            NightSkyTileSheetTexture = helper.ModContent.Load<Texture2D>(Path.Combine(assetFolderPath, "Textures", "night_sky_sheet.png"));
            SkyEffectsTileSheetTexturePath = helper.ModContent.GetInternalAssetName(Path.Combine(assetFolderPath, "Textures", "sky_effects_sheet.png")).Name;
        }
    }
}
