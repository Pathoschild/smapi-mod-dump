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
using System.Linq;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using TrendyHaley.Framework;


namespace TrendyHaley {
    public class ModEntry : Mod {
        private bool hasRandomFlowerQueen_;
        private ModConfig config_;
        private Color actualHairColor_;

        private Color spouseHairColor_;

        public override void Entry(IModHelper helper) {
            this.Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            this.Helper.Events.GameLoop.SaveLoaded   += OnSaveLoaded;
            this.Helper.Events.GameLoop.DayStarted   += OnDayStarted;
            this.Helper.Events.GameLoop.Saving       += OnSaving;
            this.Helper.Events.Content.AssetRequested += (sender, e) => {
                if (CanEdit(e.NameWithoutLocale)) {
                    e.Edit(EditAsset);
                }
            };
        }

        private bool CanEdit(IAssetName asset) {
            return asset.IsEquivalentTo("Characters/Haley") ||
                   asset.IsEquivalentTo("Portraits/Haley") ||
                   asset.IsEquivalentTo("Characters/Haley_Beach") ||
                   asset.IsEquivalentTo("Portraits/Haley_Beach") ||
                   asset.IsEquivalentTo("Characters/Haley_Winter") ||
                   asset.IsEquivalentTo("Portraits/Haley_Winter") ||
                   asset.IsEquivalentTo("LooseSprites/cowPhotos") ||
                   asset.IsEquivalentTo("LooseSprites/cowPhotosWinter");
        }

        private void EditAsset(IAssetData asset) {
            if (config_ == null) {
                return;
            }

            if (asset.NameWithoutLocale.IsEquivalentTo("Characters/Haley") || asset.NameWithoutLocale.IsEquivalentTo("Portraits/Haley")) {
                this.Monitor.Log($"Edit asset {asset.NameWithoutLocale}");

                IAssetDataForImage baseImage = asset.AsImage();
                Texture2D overlay;
                // Support for RandomFlowerQueen.
                if (hasRandomFlowerQueen_ && asset.NameWithoutLocale.IsEquivalentTo("Characters/Haley")) {
                    // We must replace the flowerqueen part of the base image since it contains unwanted pixels.
                    Texture2D haleyNoFlowercrown = this.Helper.ModContent.Load<Texture2D>($"assets/{asset.NameWithoutLocale}_no_flowercrown.png");
                    baseImage.PatchImage(haleyNoFlowercrown, targetArea: new Rectangle(0, 320, 64, 64), patchMode: PatchMode.Replace);

                    overlay = this.Helper.ModContent.Load<Texture2D>($"assets/{asset.NameWithoutLocale}_no_flowercrown_overlay_hair_gray.png");
                }
                else {
                    overlay = this.Helper.ModContent.Load<Texture2D>($"assets/{asset.NameWithoutLocale}_overlay_hair_gray.png");
                }

                baseImage.PatchImage(ColorBlend(overlay, actualHairColor_), patchMode: PatchMode.Overlay);
            }
            else if (asset.NameWithoutLocale.IsEquivalentTo("Characters/Haley_Beach") ||
                     asset.NameWithoutLocale.IsEquivalentTo("Portraits/Haley_Beach") ||
                     asset.NameWithoutLocale.IsEquivalentTo("Characters/Haley_Winter") ||
                     asset.NameWithoutLocale.IsEquivalentTo("Portraits/Haley_Winter")) {
                this.Monitor.Log($"Edit asset {asset.NameWithoutLocale}");

                IAssetDataForImage baseImage = asset.AsImage();
                Texture2D overlay = this.Helper.ModContent.Load<Texture2D>($"assets/{asset.NameWithoutLocale}_overlay_hair_gray.png");

                baseImage.PatchImage(ColorBlend(overlay, actualHairColor_), patchMode: PatchMode.Overlay);
            }
            else if (asset.NameWithoutLocale.IsEquivalentTo("LooseSprites/cowPhotos") || asset.NameWithoutLocale.IsEquivalentTo("LooseSprites/cowPhotosWinter")) {
                this.Monitor.Log($"Edit asset {asset.NameWithoutLocale}");

                IAssetDataForImage baseImage = asset.AsImage();
                Texture2D overlay = this.Helper.ModContent.Load<Texture2D>("assets/Characters/Haley_cowPhotos_overlay_hair_gray.png");

                baseImage.PatchImage(ColorBlend(overlay, actualHairColor_), patchMode: PatchMode.Overlay);
            }
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
            // Check for RandomFlowerQueen CP mod.
            hasRandomFlowerQueen_ = this.Helper.ModRegistry.IsLoaded("Candidus42.RandomFlowerQueen");

            // GenericModConfigMenu support.
            var configMenu_ = this.Helper.ModRegistry.GetApi<GenericModConfigMenu.IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu_ is null) {
                return;
            }
            
            configMenu_.Register(this.ModManifest,
                                 () => config_ = new ModConfig(),
                                 () => this.Helper.WriteConfig(config_));

            configMenu_.AddNumberOption(this.ModManifest,
                                        () => Context.IsWorldReady ? config_.SaveGame[$"{Game1.GetSaveGameName()}_{Game1.uniqueIDForThisGame}"].HairColor.R : 0,
                                        (val) => { config_.SaveGame[$"{Game1.GetSaveGameName()}_{Game1.uniqueIDForThisGame}"].HairColor
                                                    = new Color(config_.SaveGame[$"{Game1.GetSaveGameName()}_{Game1.uniqueIDForThisGame}"].HairColor, 255) { R = (byte) val };
                                                   ComputeAndSetHairColor();
                                                 },
                                        () => "Hair color red channel",
                                        min: 0,
                                        max: 255);

            configMenu_.AddNumberOption(this.ModManifest,
                                        () => Context.IsWorldReady ? config_.SaveGame[$"{Game1.GetSaveGameName()}_{Game1.uniqueIDForThisGame}"].HairColor.G : 0,
                                        (val) => { config_.SaveGame[$"{Game1.GetSaveGameName()}_{Game1.uniqueIDForThisGame}"].HairColor
                                                    = new Color(config_.SaveGame[$"{Game1.GetSaveGameName()}_{Game1.uniqueIDForThisGame}"].HairColor, 255) { G = (byte) val };
                                                    ComputeAndSetHairColor();
                                                 },
                                        () => "Hair color green channel",
                                        min: 0,
                                        max: 255);

            configMenu_.AddNumberOption(this.ModManifest,
                                        () => Context.IsWorldReady ? config_.SaveGame[$"{Game1.GetSaveGameName()}_{Game1.uniqueIDForThisGame}"].HairColor.B : 0,
                                        (val) => { config_.SaveGame[$"{Game1.GetSaveGameName()}_{Game1.uniqueIDForThisGame}"].HairColor
                                                    = new Color(config_.SaveGame[$"{Game1.GetSaveGameName()}_{Game1.uniqueIDForThisGame}"].HairColor, 255) { B = (byte) val };
                                                    ComputeAndSetHairColor();
                                                 },
                                        () => "Hair color blue channel",
                                        min: 0,
                                        max: 255);

            configMenu_.AddTextOption(this.ModManifest,
                                      () => (Context.IsWorldReady ? config_.SaveGame[$"{Game1.GetSaveGameName()}_{Game1.uniqueIDForThisGame}"].HairDyeInterval : ConfigEntry.Interval.OncePerSeason).ToString(),
                                      (val) => { config_.SaveGame[$"{Game1.GetSaveGameName()}_{Game1.uniqueIDForThisGame}"].HairDyeInterval = Enum.Parse<ConfigEntry.Interval>(val);
                                                 ComputeAndSetHairColor(); },
                                      () => "Hair dye interval",
                                      null,
                                      Enum.GetNames<ConfigEntry.Interval>());

            configMenu_.AddBoolOption(this.ModManifest,
                                      () => Context.IsWorldReady ? config_.SaveGame[$"{Game1.GetSaveGameName()}_{Game1.uniqueIDForThisGame}"].ColorIsFading : false,
                                      (val) => { config_.SaveGame[$"{Game1.GetSaveGameName()}_{Game1.uniqueIDForThisGame}"].ColorIsFading = val;
                                                 ComputeAndSetHairColor();
                                               },
                                      () => "Color is fading");

            configMenu_.AddBoolOption(this.ModManifest,
                                      () => Context.IsWorldReady ? config_.SaveGame[$"{Game1.GetSaveGameName()}_{Game1.uniqueIDForThisGame}"].AlphaBlend : false,
                                      (val) => { config_.SaveGame[$"{Game1.GetSaveGameName()}_{Game1.uniqueIDForThisGame}"].AlphaBlend = val;
                                                 ComputeAndSetHairColor(); },
                                      () => "Alpha blend");

            configMenu_.AddBoolOption(this.ModManifest,
                                      () => Context.IsWorldReady ? config_.SaveGame[$"{Game1.GetSaveGameName()}_{Game1.uniqueIDForThisGame}"].SpouseLookAlike : false,
                                      (val) => { config_.SaveGame[$"{Game1.GetSaveGameName()}_{Game1.uniqueIDForThisGame}"].SpouseLookAlike = val;
                                                 ComputeAndSetHairColor(); },
                                      () => "Spouse look alike");
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

            ComputeAndSetHairColor();
        }

        private void ComputeAndSetHairColor() {
            string saveGameName = $"{Game1.GetSaveGameName()}_{Game1.uniqueIDForThisGame}";

            // Check relationship of farmer and Haley.
            bool isFarmerMarriedToHaley = Game1.player.isMarriedOrRoommates() && Game1.player.getSpouse().Name.Equals("Haley");

            // First day of season or color unset.
            var interval = config_.SaveGame[saveGameName].HairDyeInterval;
            int[] hairDyeDays = interval switch {
                ConfigEntry.Interval.OncePerSeason => new int[] { 1 },
                ConfigEntry.Interval.TwicePerSeason => new int[] { 1, 15 },
                ConfigEntry.Interval.OncePerWeek => new int[] { 1, 8, 15, 22 },
                _ => new int[] { 1 }
            };

            if (hairDyeDays.Contains(Game1.dayOfMonth) || config_.SaveGame[saveGameName].HairColor == Color.Transparent) {
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
                interval = config_.SaveGame[saveGameName].HairDyeInterval;

                // The following calculations are simple enough to be done
                // even if we don't need their results, getting the conditions right
                // would make things unnecessarily complicated.
                (int modulus, float divider) = interval switch {
                    ConfigEntry.Interval.OncePerSeason => (28, 27.0f),
                    ConfigEntry.Interval.TwicePerSeason => (14, 13.0f),
                    ConfigEntry.Interval.OncePerWeek => (7, 6.0f),
                    _ => (28, 27.0f)
                };

                // Needed for color blend and option SpouseLookAlike.
                Color colorFadedColor =  new Color((byte) (baseColor.R + (255 - baseColor.R) * (float) ((Game1.dayOfMonth - 1) % modulus) / divider),
                                                   (byte) (baseColor.G + (255 - baseColor.G) * (float) ((Game1.dayOfMonth - 1) % modulus) / divider),
                                                   (byte) (baseColor.B + (255 - baseColor.B) * (float) ((Game1.dayOfMonth - 1) % modulus) / divider));

                // Needed for alpha blend: Base color with modified alpha channel.
                // Note that the renderer expects premultiplied alpha.
                Color alphaFadedColor
                    = Color.FromNonPremultiplied(baseColor.R,
                                                 baseColor.G,
                                                 baseColor.B,
                                                 (int) (255.0f * (float) (modulus - (Game1.dayOfMonth - 1) % modulus) / divider));

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
            bool isFarmerMarriedToHaley = Game1.player.isMarriedOrRoommates() && Game1.player.getSpouse().Name.Equals("Haley");

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

            this.Helper.GameContent.InvalidateCache("Characters/Haley");
            this.Helper.GameContent.InvalidateCache("Portraits/Haley");
            this.Helper.GameContent.InvalidateCache("Characters/Haley_Beach");
            this.Helper.GameContent.InvalidateCache("Portraits/Haley_Beach");
            this.Helper.GameContent.InvalidateCache("Characters/Haley_Winter");
            this.Helper.GameContent.InvalidateCache("Portraits/Haley_Winter");
            this.Helper.GameContent.InvalidateCache("LooseSprites/cowPhotos");
            this.Helper.GameContent.InvalidateCache("LooseSprites/cowPhotosWinter");
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
