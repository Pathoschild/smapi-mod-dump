/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mus-candidus/ImagEd
**
*************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;


namespace ImagEd.Framework {
    internal class RecolorToken {
        private readonly IModHelper helper_;
        private readonly IMonitor monitor_;

        // Required to implement UpdateContext() .
        private RecolorTokenArguments previousInputData_ = null;
        // Required to implement UpdateContext() .
        private bool mustUpdateContext_ = false;

        /// <summary>Indicates whether the token is enabled.</summary>
        internal bool Enabled { get; set; }

        public RecolorToken(IModHelper helper, IMonitor monitor) {
            helper_  = helper;
            monitor_ = monitor;
        }

        /// <summary>Get whether the values may change depending on the context.</summary>
        public bool IsMutable() => true;

        /// <summary>Get whether the token allows an input argument (e.g. an NPC name for a relationship token).</summary>
        public bool AllowsInput() => true;

        /// <summary>Whether the token requires an input argument to work, and does not provide values without it (see <see cref="AllowsInput"/>).</summary>
        public bool RequiresInput() => true;

        /// <summary>Whether the token may return multiple values for the given input.</summary>
        /// <param name="input">The input argument, if applicable.</param>
        public bool CanHaveMultipleValues(string input = null) => false;
        
        /// <summary>Get whether the token always chooses from a set of known values for the given input. Mutually exclusive with <see cref="HasBoundedRangeValues"/>.</summary>
        /// <param name="input">The input argument, if applicable.</param>
        /// <param name="allowedValues">The possible values for the input.</param>
        public bool HasBoundedValues(string input, out IEnumerable<string> allowedValues) {
            RecolorTokenArguments inputData = RecolorTokenArguments.Parse(input);
            string generatedFilePath = GenerateFilePath(inputData);

            allowedValues = new[] { generatedFilePath };

            return true;
        }

        /// <summary>Update the values when the context changes.</summary>
        /// <returns>Returns whether the value changed, which may trigger patch updates.</returns>
        public bool UpdateContext() {
            if (mustUpdateContext_) {
                mustUpdateContext_ = false;

                return true;
            }

            return false;
        }

        /// <summary>Get whether the token is available for use.</summary>
        public bool IsReady() => this.Enabled;

        /// <summary>Get the current values.</summary>
        /// <param name="input">The input argument, if applicable.</param>
        public IEnumerable<string> GetValues(string input) {
            RecolorTokenArguments inputData = RecolorTokenArguments.Parse(input);
            if (!(inputData.Equals(previousInputData_))) {
                mustUpdateContext_ = true;
            }
            previousInputData_ = inputData;

            IContentPack contentPack = Utility.GetContentPackFromModInfo(helper_.ModRegistry.Get(inputData.ContentPackName));
            monitor_.Log($"Content pack {contentPack.Manifest.UniqueID} requests recoloring of {inputData.AssetName}.");
            monitor_.Log($"Recolor with {inputData.MaskPath} and {Utility.ColorToHtml(inputData.BlendColor)}");

            // "gamecontent" means loading from game folder.
            Texture2D source = inputData.SourcePath.ToLowerInvariant() == "gamecontent"
                             ? helper_.Content.Load<Texture2D>(inputData.AssetName, ContentSource.GameContent)
                             : contentPack.LoadAsset<Texture2D>(inputData.SourcePath);

            Texture2D extracted = inputData.MaskPath.ToLowerInvariant() != "none"
                                ? ExtractSubImage(source,
                                                  contentPack.LoadAsset<Texture2D>(inputData.MaskPath),
                                                  inputData.DesaturationMode)
                                : source;

            Texture2D target = ColorBlend(extracted, inputData.BlendColor);

            // ATTENTION: In order to load files we just generated we need at least ContentPatcher 1.18.3 .
            string generatedFilePath = GenerateFilePath(inputData);
            string generatedFilePathAbsolute = Path.Combine(contentPack.DirectoryPath, generatedFilePath);
            Directory.CreateDirectory(Path.GetDirectoryName(generatedFilePathAbsolute));
            using (FileStream fs = new FileStream(generatedFilePathAbsolute, FileMode.Create)) {
                target.SaveAsPng(fs, target.Width, target.Height);
                fs.Close();
            }

            monitor_.Log($"Generated file {generatedFilePathAbsolute}, returning relative path {generatedFilePath}");

            helper_.Content.InvalidateCache(inputData.AssetName);

            yield return generatedFilePath;
        }

        /// <summary>Color blending (multiplication).</summary>
        private Texture2D ColorBlend(Texture2D source, Color blendColor) {
            Color[] sourcePixels = new Color[source.Width * source.Height];
            source.GetData(sourcePixels);
            // Renderer expects premultiplied alpha.
            for (int i = 0; i < sourcePixels.Length; i++) {
                sourcePixels[i]
                    = new Color((byte) (sourcePixels[i].R * blendColor.R / 255 * blendColor.A / 255),
                                (byte) (sourcePixels[i].G * blendColor.G / 255 * blendColor.A / 255),
                                (byte) (sourcePixels[i].B * blendColor.B / 255 * blendColor.A / 255),
                                (byte) (sourcePixels[i].A * blendColor.A / 255));
            }

            Texture2D blended = Utility.ArrayToTexture(sourcePixels, source.Width, source.Height);

            return blended;
        }

        /// <summary>Extracts a sub image using the given mask.</summary>
        private Texture2D ExtractSubImage(Texture2D source, Texture2D mask, Desaturation.Mode desaturationMode) {
            if (mask.Width != source.Width || mask.Height != source.Height) {
                throw new ArgumentException("Sizes of image and mask don't match");
            }

            Color[] sourcePixels = Utility.TextureToArray(source);
            Color[] maskPixels = Utility.TextureToArray(mask);
            Color[] extractedPixels = new Color[source.Width * source.Height];

            for (int i = 0; i < sourcePixels.Length; i++) {
                Color pixel = Desaturation.Desaturate(sourcePixels[i], desaturationMode);
                // Treat mask as grayscale (luma).
                byte maskValue = Desaturation.Desaturate(maskPixels[i], Desaturation.Mode.DesaturateLuma).R;
                // Multiplication is all we need: If maskValue is zero the resulting pixel is zero (TransparentBlack).
                extractedPixels[i] = pixel * (maskValue / 255.0f);
            }

            return Utility.ArrayToTexture(extractedPixels, source.Width, source.Height);
        }

        /// <summary>Generates a file path from token arguments.</summary>
        private string GenerateFilePath(RecolorTokenArguments inputData) {
            // "gamecontent" has no file extension, append ".png".
            string outputPath = inputData.SourcePath.ToLowerInvariant() == "gamecontent"
                              ? $"{inputData.AssetName}.png"
                              : inputData.SourcePath;

            // Encode configuration to avoid file name collisions.
            string suffix = $"recolored_{(int) inputData.DesaturationMode}_{inputData.BlendColor.PackedValue}";

            return Utility.AddFileNameSuffix(Path.Combine("generated", outputPath), suffix);
        }
    }
}