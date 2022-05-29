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
using static GMCMOptions.IGMCMOptionsAPI;

namespace GMCMOptions.Framework {
    /// <summary>
    /// Implementation of the <c cref="IGMCMOptionsAPI">IGMCMOptionsAPI</c>.
    /// </summary>
    public class API : IGMCMOptionsAPI {
        private readonly IModRegistry modRegistry;
        private bool fixedHeight;
        public API(IModRegistry modRegistry) {
            this.modRegistry = modRegistry;
            IModInfo gmcm = modRegistry.Get("spacechase0.GenericModConfigMenu");
            if (gmcm is null) {
                this.fixedHeight = false;
            } else {
                this.fixedHeight = gmcm.Manifest.Version.IsOlderThan(new SemanticVersion(1, 8, 2));
            }
        }

        /// <inheritdoc/>
        public void AddColorOption(IManifest mod, Func<Color> getValue, Action<Color> setValue, Func<string> name,
            Func<string> tooltip = null, bool showAlpha = true,
            uint colorPickerStyle = 0, string fieldId = null) {
            var gmcm = modRegistry.GetApi<GMCMAPI>("spacechase0.GenericModConfigMenu");
            if (gmcm == null) return;
            ColorPickerOption option = new ColorPickerOption(fixedHeight, getValue, setValue, showAlpha, (ColorPickerStyle)colorPickerStyle);
            gmcm.AddComplexOption(
                mod: mod,
                name: name,
                tooltip: tooltip,
                draw: option.Draw,
                height: option.Height,
                beforeMenuOpened: option.Reset,
                beforeSave: option.SaveChanges,
                afterReset: option.Reset,
                fieldId: fieldId);
        }

        /// <inheritdoc/>
        public void AddImageOption(IManifest mod,
                    Func<uint> getValue,
                    Action<uint> setValue,
                    Func<string> name,
                    Func<uint> getMaxValue,
                    Func<int> maxImageHeight,
                    Func<int> maxImageWidth,
                    Action<uint, SpriteBatch, Vector2> drawImage,
                    Func<string> tooltip = null,
                    Func<uint, String> label = null,
                    int arrowLocation = (int)ImageOptionArrowLocation.Top,
                    int labelLocation = (int)ImageOptionLabelLocation.Top,
                    string fieldId = null) {
            var gmcm = modRegistry.GetApi<GMCMAPI>("spacechase0.GenericModConfigMenu");
            if (gmcm == null) return;
            ImagePickerOption option = new ImagePickerOption(getValue, setValue, getMaxValue,
                maxImageHeight, maxImageWidth, drawImage, label,
                (ImagePickerOption.ArrowLocation)arrowLocation, (ImagePickerOption.LabelLocation)labelLocation);
            gmcm.AddComplexOption(
                mod: mod,
                name: name,
                tooltip: tooltip,
                draw: option.Draw,
                height: option.Height,
                beforeMenuOpened: option.Reset,
                beforeSave: option.SaveChanges,
                afterReset: option.Reset,
                fieldId: fieldId);
        }

        /// <inheritdoc/>
        public void AddImageOption(IManifest mod,
                                   Func<uint> getValue,
                                   Action<uint> setValue,
                                   Func<string> name,
                                   Func<(Func<String> label, Texture2D sheet, Rectangle? sourceRect)[]> choices,
                                   Func<string> tooltip = null,
                                   int arrowLocation = (int)ImageOptionArrowLocation.Top,
                                   int labelLocation = (int)ImageOptionLabelLocation.Top,
                                   string fieldId = null) {
            var gmcm = modRegistry.GetApi<GMCMAPI>("spacechase0.GenericModConfigMenu");
            if (gmcm == null) return;
            ImagePickerOption option = new ImagePickerOption(
                getValue: getValue,
                setValue: setValue,
                getMaxValue: () => (uint)choices().Length - 1,
                maxImageHeight: () => choices().Select(choice => choice.sourceRect?.Height ?? choice.sheet.Height).Max(),
                maxImageWidth: () => choices().Select(choice => choice.sourceRect?.Width ?? choice.sheet.Width).Max(),
                drawImage: (v, b, pos) => {
                    var allChoices = choices();
                    int maxHeight = allChoices.Select(choice => choice.sourceRect?.Height ?? choice.sheet.Height).Max();
                    int maxWidth = allChoices.Select(choice => choice.sourceRect?.Width ?? choice.sheet.Width).Max();
                    var choice = allChoices[v];
                    int height = choice.sourceRect?.Height ?? choice.sheet.Height;
                    int width = choice.sourceRect?.Width ?? choice.sheet.Width;
                    Vector2 centeredPos = new Vector2(pos.X + (maxWidth - width) / 2, pos.Y + (maxHeight - height) / 2);
                    b.Draw(choice.sheet, centeredPos, choice.sourceRect, Color.White);
                },
                label: (v) => choices()[v].label?.Invoke(),
                (ImagePickerOption.ArrowLocation)arrowLocation, (ImagePickerOption.LabelLocation)labelLocation);
            gmcm.AddComplexOption(
                mod: mod,
                name: name,
                tooltip: tooltip,
                draw: option.Draw,
                height: option.Height,
                beforeMenuOpened: option.Reset,
                beforeSave: option.SaveChanges,
                afterReset: option.Reset,
                fieldId: fieldId);
        }


    }
    /// <summary>
    /// The portion of the GMCM API that we need
    /// </summary>
    public interface GMCMAPI {
        // see https://github.com/spacechase0/StardewValleyMods/blob/develop/GenericModConfigMenu/IGenericModConfigMenuApi.cs
        void AddComplexOption(IManifest mod, Func<string> name, Action<SpriteBatch, Vector2> draw, Func<string> tooltip = null, Action beforeMenuOpened = null, Action beforeSave = null, Action afterSave = null, Action beforeReset = null, Action afterReset = null, Action beforeMenuClosed = null, Func<int> height = null, string fieldId = null);

    }

}
