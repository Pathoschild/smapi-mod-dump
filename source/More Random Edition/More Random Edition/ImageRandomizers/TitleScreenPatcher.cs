/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System.IO;

namespace Randomizer
{
    /// <summary>
    /// Modifies the title screen with our randomizer graphics
    /// </summary>
    public class TitleScreenPatcher : ImagePatcher
    {
        public const string StardewAssetPath = "Minigames/TitleButtons";

        public TitleScreenPatcher()
        {
            SubFolder = "Minigames";
        }

        /// <summary>
        /// Called when the asset is requested
        /// Patches the image into the game
        /// </summary>
        /// <param name="asset">The equivalent asset from Stardew to modify</param>
        public override void OnAssetRequested(IAssetData asset)
        {
            var editor = asset.AsImage();
            ApplyOverlay(editor, "SplashScreenOverlay", x: 173, y: 359);
            ApplyOverlay(editor, "TitleScreenOverlay", x: 0, y: 132);
        }

        /// <summary>
        /// Applies an overlay to the title screen asset
        /// </summary>
        /// <param name="editor">The editor used to path the image</param>
        /// <param name="overlayFileName">The file name of the overlay</param>
        /// <param name="x">The x coordinate to overlay onto</param>
        /// <param name="y">The y coordinate to overlay onto</param>
        private void ApplyOverlay(IAssetDataForImage editor, string overlayFileName, int x, int y)
        {
            string path = Path.Combine(PatcherImageFolder, $"{overlayFileName}.png");
            IRawTextureData overlay =
                Globals.ModRef.Helper.ModContent.Load<IRawTextureData>(path);

            editor.PatchImage(
                overlay,
                targetArea: new Rectangle(x, y, overlay.Width, overlay.Height),
                patchMode: PatchMode.Overlay);
        }
    }
}
