/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewGMCMOptions
**
*************************************************/

// // Copyright 2023 Jamie Taylor
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using static GMCMOptions.IGMCMOptionsAPI;

namespace GMCMOptions.Framework {
    public interface IObsoleteApiMethods {

        // Deprecated (because additional arguments with default values were added) in 1.5

        [Obsolete("This AddImageOption signature is deprecated; use one of the others instead")]
        void AddImageOption(IManifest mod,
                            Func<uint> getValue,
                            Action<uint> setValue,
                            Func<string> name,
                            Func<uint> getMaxValue,
                            Func<int> maxImageHeight,
                            Func<int> maxImageWidth,
                            Action<uint, SpriteBatch, Vector2> drawImage,
                            Func<string>? tooltip = null,
                            Func<uint, String?>? label = null,
                            int arrowLocation = (int)ImageOptionArrowLocation.Top,
                            int labelLocation = (int)ImageOptionLabelLocation.Top,
                            string? fieldId = null);
        [Obsolete("This AddImageOption signature is deprecated; use one of the others instead")]
        void AddImageOption(IManifest mod,
                            Func<uint> getValue,
                            Action<uint> setValue,
                            Func<string> name,
                            Func<(Func<String?> label, Texture2D sheet, Rectangle? sourceRect)[]> choices,
                            Func<string>? tooltip = null,
                            int arrowLocation = (int)ImageOptionArrowLocation.Top,
                            int labelLocation = (int)ImageOptionLabelLocation.Top,
                            string? fieldId = null);
    }
}

