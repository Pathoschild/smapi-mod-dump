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
using GMCMOptions.Framework;
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
            public bool showParaText = true;
        }

        /// <summary>
        /// The current configuration value.
        /// </summary>
        internal Config config;

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
            var configMenuExtObsolete = Helper.ModRegistry.GetApi<IObsoleteApiMethods>("jltaylor-us.GMCMOptions");
            if (configMenu is null || configMenuExt is null || configMenuExtObsolete is null) {
                return;
            }
            Config uncommittedConfig = new Config();

            // register the mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => {
                    config = new Config();
                    uncommittedConfig = new Config();
                },
                save: SaveConfig);
            configMenu.OnFieldChanged(
                mod: ModManifest,
                onChange: (string str, object obj) => {
                    // All of the config options that you want to use for dynamic updates need to be included here.
                    switch (str) {
                        case "showPara":
                            uncommittedConfig.showParaText = (bool)obj;
                            break;
                        default:
                            break;
                    }
                });
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
                label: (v) => testImageData[v].label(),
                imageTooltipText: (idx) => $"Image tooltip test text for image {idx}");
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
#pragma warning disable CS0618 // Type or member is obsolete
            configMenuExtObsolete.AddImageOption(
                mod: ModManifest,
                getValue: () => config.i3,
                setValue: (v) => config.i3 = v,
                name: () => "Simplified image picker",
                tooltip: () => "With default layout",
                choices: () => testImageData);
#pragma warning restore CS0618 // Type or member is obsolete
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

                    } else if (v == 2) {
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
                imageTooltipText: (idx) => $"Image tooltip test text for image {idx}",
                arrowLocation: (int)IGMCMOptionsAPI.ImageOptionArrowLocation.Sides,
                labelLocation: (int)IGMCMOptionsAPI.ImageOptionLabelLocation.Bottom);

            int paraCounter = 0;
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => config.showParaText,
                setValue: (bool v) => config.showParaText = v,
                name: () => "show paragraph text",
                fieldId: "showPara");
            configMenuExt.AddDynamicParagraph(
                mod: ModManifest,
                logName: "counter",
                text: () => uncommittedConfig.showParaText ? $"Paragraph text - called {paraCounter++ / 100} x100 times.  More text to show that it does in fact eventually word wrap.  (And that word wrapping happens at somewhere at least vaguely close to the correct width.)" : "",
                isStyledText: false
                );
            configMenu.AddParagraph(ModManifest, () => "the next paragraph (so you can tell the height of the previous item)");
            string styledText =
                "A Dynamic Paragraph that passes <b><color name=\"blue\">true</color></b> for the <b>isStyledText</b>"
                + " argument when it is registered can use simple html-like formatting directives in the text.  Text"
                + " must be valid xml (if it were to be wrapped in a single xml tag).  Currently supported format"
                + " options are <b>bold</b> and <color name=\"blue\">color</color>.";
            configMenuExt.AddDynamicParagraph(
                mod: ModManifest,
                logName: "styled text",
                text: () => styledText,
                isStyledText: true
                );
            configMenu.AddParagraph(ModManifest, () => "Raw text of above paragraph: " + styledText);
            configMenuExt.AddDynamicParagraph(
                mod: ModManifest,
                logName: "dynamic styled",
                text: () => {
                    Color c = Utility.GetPrismaticColor(speedMultiplier: 2);
                    return $"Styled text can also be <color r=\"{c.R}\" g=\"{c.G}\" b=\"{c.B}\" >dynamic</color>.";
                },
                isStyledText: true
                );
            configMenuExt.AddDynamicParagraph(
                mod: ModManifest,
                logName: "error test",
                text: () => "Error testing:  <qqq atr=\"xyz\">unknown element</qqq>.  <color>no color specified</color>."
                            + "<color r=\"abc\">bad color attr</color> </b>invalid xml",
                isStyledText: true
                );
        }

        public void RemoveFromGMCM() {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            configMenu?.Unregister(ModManifest);
        }
    }
    public interface IGenericModConfigMenuApi {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);
        void Unregister(IManifest mod);
        void OnFieldChanged(IManifest mod, Action<string, object> onChange);
        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string>? tooltip = null, string? fieldId = null);
        void AddParagraph(IManifest mod, Func<string> text);
    }
}
