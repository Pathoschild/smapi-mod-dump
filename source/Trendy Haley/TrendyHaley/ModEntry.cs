/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mus-candidus/TrendyHaley
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

using TrendyHaley.Framework;


namespace TrendyHaley {
    public class ModEntry : Mod, IAssetEditor {
        private bool hasColdWeatherHaley_;
        private bool hasRandomFlowerQueen_;
        private ModConfig config_;
        private Color actualHairColor_;

        private Color spouseHairColor_;

        public override void Entry(IModHelper helper) {
            this.Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            this.Helper.Events.GameLoop.SaveLoaded   += OnSaveLoaded;
            this.Helper.Events.GameLoop.DayStarted   += OnDayStarted;
            this.Helper.Events.GameLoop.Saving       += OnSaving;
        }

        /// <summary>Implements <see cref="IAssetEditor.CanEdit"/>.</summary>
        public bool CanEdit<T>(IAssetInfo asset) {
            return asset.AssetNameEquals("Characters/Haley") ||
                   asset.AssetNameEquals("Portraits/Haley_Beach") ||
                   asset.AssetNameEquals("Characters/Haley_Beach") ||
                   asset.AssetNameEquals("Portraits/Haley") ||
                   asset.AssetNameEquals("LooseSprites/cowPhotos") ||
                   asset.AssetNameEquals("LooseSprites/cowPhotosWinter");
        }

        /// <summary>Implements <see cref="IAssetEditor.Edit"/>.</summary>
        public void Edit<T>(IAssetData asset) {
            if (asset.AssetNameEquals("Characters/Haley") || asset.AssetNameEquals("Portraits/Haley")) {
                this.Monitor.Log($"Edit asset {asset.AssetName}");

                IAssetDataForImage baseImage = asset.AsImage();
                Texture2D overlay;
                // Support for Cold Weather Haley.
                if (hasColdWeatherHaley_ && Game1.IsWinter) {
                    overlay = this.Helper.Content.Load<Texture2D>($"assets/{asset.AssetName}_winter_overlay_hair_gray.png");
                    // Workaround for the missing sleeping sprite of Cold Weather Haley.
                    if (asset.AssetNameEquals("Characters/Haley")) {
                        Texture2D sleepingHaley = this.Helper.Content.Load<Texture2D>($"assets/{asset.AssetName}_sleeping.png");
                        baseImage.PatchImage(sleepingHaley, patchMode: PatchMode.Overlay);
                    }

                }
                // Support for RandomFlowerQueen.
                else if (hasRandomFlowerQueen_ && asset.AssetNameEquals("Characters/Haley")) {
                    // We must replace the flowerqueen part of the base image since it contains unwanted pixels.
                    Texture2D haleyNoFlowercrown = this.Helper.Content.Load<Texture2D>($"assets/{asset.AssetName}_no_flowercrown.png");
                    baseImage.PatchImage(haleyNoFlowercrown, targetArea: new Rectangle(0, 320, 64, 64), patchMode: PatchMode.Replace);

                    overlay = this.Helper.Content.Load<Texture2D>($"assets/{asset.AssetName}_no_flowercrown_overlay_hair_gray.png");
                }
                else {
                    overlay = this.Helper.Content.Load<Texture2D>($"assets/{asset.AssetName}_overlay_hair_gray.png");
                }

                baseImage.PatchImage(ColorBlend(overlay, actualHairColor_), patchMode: PatchMode.Overlay);
            }
            else if (asset.AssetNameEquals("Characters/Haley_Beach") || asset.AssetNameEquals("Portraits/Haley_Beach")) {
                this.Monitor.Log($"Edit asset {asset.AssetName}");

                IAssetDataForImage baseImage = asset.AsImage();
                Texture2D overlay = this.Helper.Content.Load<Texture2D>($"assets/{asset.AssetName}_overlay_hair_gray.png");

                baseImage.PatchImage(ColorBlend(overlay, actualHairColor_), patchMode: PatchMode.Overlay);
            }
            else if (asset.AssetNameEquals("LooseSprites/cowPhotos") || asset.AssetNameEquals("LooseSprites/cowPhotosWinter")) {
                this.Monitor.Log($"Edit asset {asset.AssetName}");

                IAssetDataForImage baseImage = asset.AsImage();
                Texture2D overlay = this.Helper.Content.Load<Texture2D>("assets/Characters/Haley_cowPhotos_overlay_hair_gray.png");

                baseImage.PatchImage(ColorBlend(overlay, actualHairColor_), patchMode: PatchMode.Overlay);
            }
            else {
                throw new ArgumentException($"Invalid asset {asset.AssetName}");
            }
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
            // Check for ColdWeatherHaley CP mod.
            hasColdWeatherHaley_  = this.Helper.ModRegistry.IsLoaded("NanoGamer7.ColdWeatherHaley");
            // Check for RandomFlowerQueen CP mod.
            hasRandomFlowerQueen_ = this.Helper.ModRegistry.IsLoaded("Candidus42.RandomFlowerQueen");
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e) {
            // Read persisted config.
            config_ = this.Helper.ReadConfig<ModConfig>();
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e) {
            // Create a config entry for this save game if necessary.
            string saveGameName = $"{Game1.GetSaveGameName()}_{Game1.uniqueIDForThisGame}";
            if (!config_.SaveGame.ContainsKey(saveGameName)) {
                config_.SaveGame.Add(saveGameName, new ConfigEntry());
            }

            // Check relationship of farmer and Haley.
            bool isFarmerMarriedToHaley = Game1.player.isMarried() && Game1.player.getSpouse().Name.Equals("Haley");

            // First day of season or color unset.
            if (Game1.dayOfMonth == 1 || config_.SaveGame[saveGameName].HairColor == Color.Transparent) {
                // Get a new hair color for Haley.
                config_.SaveGame[saveGameName].HairColor = RandomColor();
                // Save config.
                this.Helper.WriteConfig(config_);

                SetHairColor(config_.SaveGame[saveGameName].HairColor);
                this.Monitor.Log($"Haley chose a new hair color for this season: {config_.SaveGame[saveGameName].HairColor}");

                if (config_.SaveGame[saveGameName].SpouseLookAlike && isFarmerMarriedToHaley) {
                    spouseHairColor_ = config_.SaveGame[saveGameName].HairColor;
                    Game1.player.changeHairColor(spouseHairColor_);
                    this.Monitor.Log($"{Game1.player.Name} has the same hair color as Haley");
                }

                return;
            }

            if (config_.SaveGame[saveGameName].ColorIsFading) {
                // The color gets brighter day by day so at season's end the color multiplier is white.
                Color baseColor = config_.SaveGame[saveGameName].HairColor;

                // The following calculations are simple enough to be done
                // even if we don't need their results, getting the conditions right
                // would make things unnecessarily complicated.

                // Needed for color blend and option SpouseLookAlike.
                Color colorFadedColor
                    = new Color((byte) (baseColor.R + (255 - baseColor.R) * (float) (Game1.dayOfMonth - 1) / 27.0f),
                                (byte) (baseColor.G + (255 - baseColor.G) * (float) (Game1.dayOfMonth - 1) / 27.0f),
                                (byte) (baseColor.B + (255 - baseColor.B) * (float) (Game1.dayOfMonth - 1) / 27.0f));

                // Needed for alpha blend: Base color with modified alpha channel.
                // Note that the renderer expects premultiplied alpha.
                Color alphaFadedColor
                    = Color.FromNonPremultiplied(baseColor.R,
                                                 baseColor.G,
                                                 baseColor.B,
                                                 (int) (255.0f * (float) (28 - Game1.dayOfMonth) / 27.0f));

                Color fadedColor = config_.SaveGame[saveGameName].AlphaBlend
                                 ? alphaFadedColor
                                 : colorFadedColor;

                SetHairColor(fadedColor);
                this.Monitor.Log($"Haley's hair color faded: {fadedColor}");

                if (config_.SaveGame[saveGameName].SpouseLookAlike && isFarmerMarriedToHaley) {
                    spouseHairColor_ = colorFadedColor;
                    Game1.player.changeHairColor(spouseHairColor_);
                    this.Monitor.Log($"{Game1.player.Name} has the same hair color as Haley");
                }
            }
            else {
                // We have to load the recolored hair overlay every day, otherwise it falls back to vanilla.
                Color hairColor = config_.SaveGame[saveGameName].HairColor;

                SetHairColor(hairColor);
            }
        }

        private void OnSaving(object sender, SavingEventArgs e) {
            string saveGameName = $"{Game1.GetSaveGameName()}_{Game1.uniqueIDForThisGame}";

            // Check relationship of farmer and Haley.
            bool isFarmerMarriedToHaley = Game1.player.isMarried() && Game1.player.getSpouse().Name.Equals("Haley");

            if (config_.SaveGame[saveGameName].SpouseLookAlike && isFarmerMarriedToHaley) {
                Color actualSpouseHairColor = Game1.player.hairstyleColor.Value;
                if (actualSpouseHairColor != spouseHairColor_)
                {
                    // Spouse chose a new hair color and so does Haley.
                    config_.SaveGame[saveGameName].HairColor = actualSpouseHairColor;
                    // Save config.
                    this.Helper.WriteConfig(config_);

                    this.Monitor.Log($"Haley has the same hair color as {Game1.player.Name}: {actualSpouseHairColor}");
                }
            }
            // Read persisted config.
            config_ = this.Helper.ReadConfig<ModConfig>();
        }

        /// <summary>Sets color and triggers sprite reload.</summary>
        private void SetHairColor(Color hairColor) {
            actualHairColor_ = hairColor;

            this.Helper.Content.InvalidateCache("Characters/Haley");
            this.Helper.Content.InvalidateCache("Portraits/Haley");
        }

        /// <summary>Random color (always full opaque).</summary>
        private static Color RandomColor() {
            int R = Game1.random.Next(0, 255);
            int G = Game1.random.Next(0, 255);
            int B = Game1.random.Next(0, 255);

            return new Color(R, G, B);
        }

        /// <summary>Color blending (multiplication).</summary>
        private Texture2D ColorBlend(Texture2D source, Color blendColor) {
            Color[] sourcePixels = new Color[source.Width * source.Height];
            source.GetData(sourcePixels);
            for (int i = 0; i < sourcePixels.Length; i++) {
                sourcePixels[i]
                    = new Color((byte) (sourcePixels[i].R * blendColor.R / 255),
                                (byte) (sourcePixels[i].G * blendColor.G / 255),
                                (byte) (sourcePixels[i].B * blendColor.B / 255),
                                (byte) (sourcePixels[i].A * blendColor.A / 255));
            }

            Texture2D blended = new Texture2D(Game1.graphics.GraphicsDevice, source.Width, source.Height);
            blended.SetData(sourcePixels);

            return blended;
        }
    }
}
