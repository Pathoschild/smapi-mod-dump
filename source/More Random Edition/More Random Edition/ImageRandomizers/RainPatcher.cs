/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.IO;

namespace Randomizer
{
    /// <summary>
    /// Modifies the rain asset with custom graphics
    /// </summary>
    public class RainPatcher : ImagePatcher
    {
        public const string StardewAssetPath = "TileSheets/rain";

        public RainPatcher()
        {
            SubFolder = "CustomImages/TileSheets";
        }

        /// <summary>
        /// Called when the asset is requested
        /// Patches the image into the game
        /// </summary>
        /// <param name="asset">The equivalent asset from Stardew to modify</param>
        public override void OnAssetRequested(IAssetData asset)
        {
            // Make sure we don't try to do this when a farm is not loaded
            if (!Globals.Config.RandomizeRain || Game1.player == null) 
            {
                return; 
            }

            var editor = asset.AsImage();
            using Texture2D customRain = Globals.ModRef.Helper.ModContent
                .Load<Texture2D>(GetCustomAssetPath());

            editor.PatchImage(customRain);
            TryWriteRandomizedImage(customRain);
        }

        /// <summary>c
        /// Gets the rain image to use foro the replacement
        /// </summary>
        /// <returns>The full path of the image, starting at the root of the mod</returns>
        public string GetCustomAssetPath()
        {
            RNG rng = RNG.GetDailyRNG(nameof(RainPatcher));
            string randomRainAsset = rng.GetRandomValueFromList(GetAllFileNamesInFolder());
            return Path.Combine(PatcherImageFolder, randomRainAsset);
        }
    }
}
