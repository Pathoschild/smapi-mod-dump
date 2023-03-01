/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewGMCMOptions
**
*************************************************/

// Copyright 2022 Jamie Taylor
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace GMCMOptions {
    /// <summary>
    /// An example showing usage of the complex options available in the GMCMOptions API.
    /// </summary>
    public class Example {
        /// <summary>
        /// An example configuration object.
        /// </summary>
        public class Config {
            public Color c1 = Color.BlueViolet;
            public Color c2 = Color.MediumAquamarine;
            public Color c3 = Color.SandyBrown;
            public Color c4 = Color.ForestGreen;
            public uint i1 = 0;
            public uint i2 = 1;
            public uint i3 = 2;
            public uint i4 = 0;
        }

        /// <summary>
        /// The current configuration value.
        /// </summary>
        private Config config;

        private IManifest ModManifest;
        private IModHelper Helper;
        public Example(IManifest manifest, IModHelper helper) {
            ModManifest = manifest;
            Helper = helper;
            // normally this would read the existing config: config = helper.ReadConfig<Config>();
            // but we don't actually have (or want) a config.json file
            config = new Config();
        }

        private void SaveConfig() {
            // normally this would save the config to a file: Helper.WriteConfig(config);
            // but we don't actually have (or want) a config.json file.
            // do nothing.
        }

        public void AddToGMCM() {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            var configMenuExt = Helper.ModRegistry.GetApi<IGMCMOptionsAPI>("jltaylor-us.GMCMOptions");
            if (configMenu is null || configMenuExt is null) {
                return;
            }
            // register the mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => config = new Config(),
                save: SaveConfig);
            // register some complex config options
            // Color options
            configMenuExt.AddColorOption(
                mod: ModManifest,
                getValue: () => config.c4,
                setValue: (c) => config.c4 = c,
                name: () => "Default options",
                tooltip: () => "This example shows the AddColorOption default options");
            configMenuExt.AddSimpleHorizontalSeparator(
                mod: ModManifest,
                alignment: (int)IGMCMOptionsAPI.HorizontalAlignment.Right,
                color: Color.ForestGreen);
            configMenuExt.AddColorOption(
                mod: ModManifest,
                getValue: () => config.c1,
                setValue: (c) => config.c1 = c,
                name: () => "Simple RGBA sliders",
                tooltip: () => "This example shows a single style of color picker (the RGB sliders), with alpha.",
                colorPickerStyle: (uint)IGMCMOptionsAPI.ColorPickerStyle.RGBSliders);
            configMenuExt.AddHorizontalSeparator(mod: ModManifest, alignment: (int)IGMCMOptionsAPI.HorizontalAlignment.Left);
            configMenuExt.AddColorOption(
                mod: ModManifest,
                getValue: () => config.c2,
                setValue: (c) => config.c2 = c,
                name: () => "Single Picker, no alpha",
                tooltip: () => "This example shows all different picker styles, but only one at a time, with no alpha slider.",
                showAlpha: false,
                colorPickerStyle: (uint)(IGMCMOptionsAPI.ColorPickerStyle.AllStyles | IGMCMOptionsAPI.ColorPickerStyle.RadioChooser));
            configMenuExt.AddColorOption(
                mod: ModManifest,
                getValue: () => config.c3,
                setValue: (c) => config.c3 = c,
                name: () => "All Pickers, with no alpha",
                tooltip: () => "This example shows all different picker styles, with multiple visible at a time, with no alpha slider.",
                showAlpha: false,
                colorPickerStyle: (uint)(IGMCMOptionsAPI.ColorPickerStyle.AllStyles | IGMCMOptionsAPI.ColorPickerStyle.ToggleChooser));
            // test the horizontal separator
            configMenuExt.AddHorizontalSeparator(
                mod: ModManifest,
                padAbove: 10,
                padBelow: 20,
                height: 5,
                getWidthFraction: () => 0.75 + 0.25 * Math.Sin(Game1.ticks / 100.0),
                getColor: () => Utility.GetPrismaticColor(speedMultiplier: 2),
                getShadowColor: () => Color.Transparent);
            // image options
            // test data for the images - labels are functions for easy i18n support
            (Func<String?> label, Texture2D texture, Rectangle? rect)[] testImageData = {
                (() => "Stardrop", Game1.mouseCursors, new Rectangle(346, 392, 8, 8)),
                (() => "Logo", Game1.content.Load<Texture2D>("LooseSprites\\logo"), null),
                (() => "Speech", Game1.mouseCursors, new Rectangle(66, 4, 14, 12)),
            };
            configMenuExt.AddImageOption(
                mod: ModManifest,
                getValue: () => config.i1,
                setValue: (v) => config.i1 = v,
                name: () => "Image picker option",
                getMaxValue: () => (uint)(testImageData.Length - 1),
                maxImageHeight: () => testImageData.Select(d => d.rect?.Height ?? d.texture.Height).Max(),
                maxImageWidth: () => testImageData.Select(d => d.rect?.Width ?? d.texture.Width).Max(),
                drawImage: (v, b, pos) => {
                    b.Draw(testImageData[v].texture, pos, testImageData[v].rect, Color.White);
                },
                label: (v) => testImageData[v].label());
            configMenuExt.AddImageOption(
                mod: ModManifest,
                getValue: () => config.i2,
                setValue: (v) => config.i2 = v,
                name: () => "Image picker option options",
                tooltip: () => "Showing different layout options",
                getMaxValue: () => (uint)(testImageData.Length - 1),
                maxImageHeight: () => testImageData.Select(d => d.rect?.Height ?? d.texture.Height).Max(),
                maxImageWidth: () => testImageData.Select(d => d.rect?.Width ?? d.texture.Width).Max(),
                drawImage: (v, b, pos) => {
                    b.Draw(testImageData[v].texture, pos, testImageData[v].rect, Color.White);
                },
                label: (v) => testImageData[v].label(),
                arrowLocation: (int)IGMCMOptionsAPI.ImageOptionArrowLocation.Sides,
                labelLocation: (int)IGMCMOptionsAPI.ImageOptionLabelLocation.Bottom);
            configMenuExt.AddImageOption(
                mod: ModManifest,
                getValue: () => config.i3,
                setValue: (v) => config.i3 = v,
                name: () => "Simplified image picker",
                tooltip: () => "With default layout",
                choices: () => testImageData) ;
            // Image option test with a complicated draw function
            configMenuExt.AddImageOption(
                mod: ModManifest,
                getValue: () => config.i4,
                setValue: (v) => config.i4 = v,
                name: () => "Image picker with custom drawing",
                getMaxValue: () => 2,
                maxImageHeight: () => 128,
                maxImageWidth: () => 64,
                drawImage: (v, b, pos) => {
                    FarmerRenderer.isDrawingForUI = true;
                    var farmer = Game1.player;
                    var oldPantsColor = farmer.pantsColor.Value;
                    var oldDir = farmer.facingDirection.Value;
                    farmer.faceDirection(Game1.down);
                    if (v == 1) {
                        farmer.changeShirt(10);
                        farmer.changePantStyle(0);
                        farmer.changePants(new Color(49, 49, 49));

                    } else if (v==2) {
                        farmer.changeShirt(265);
                        farmer.changePantStyle(2);
                        farmer.changePants(new Color(255, 255, 255));
                    }
                    farmer.FarmerRenderer.draw(b, farmer.FarmerSprite.CurrentAnimationFrame, farmer.FarmerSprite.CurrentFrame, farmer.FarmerSprite.SourceRect, pos, Vector2.Zero, 0.8f, Color.White, 0f, 1f, farmer);
                    farmer.changeShirt(-1);
                    farmer.changePants(oldPantsColor);
                    farmer.changePantStyle(-1);
                    farmer.faceDirection(oldDir);
                    FarmerRenderer.isDrawingForUI = false;
                },
                label: (v) => v == 0 ? null : v == 1 ? "tux" : "dress",
                arrowLocation: (int)IGMCMOptionsAPI.ImageOptionArrowLocation.Sides,
                labelLocation: (int)IGMCMOptionsAPI.ImageOptionLabelLocation.Bottom);

        }

        public void RemoveFromGMCM() {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            configMenu?.Unregister(ModManifest);
        }
    }
    public interface IGenericModConfigMenuApi {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);
        void Unregister(IManifest mod);
    }
}
