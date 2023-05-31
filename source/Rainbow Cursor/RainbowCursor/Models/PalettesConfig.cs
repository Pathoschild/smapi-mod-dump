/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewRainbowCursor
**
*************************************************/

// Copyright 2023 Jamie Taylor
using System;
using System.Collections.Generic;
using StardewModdingAPI;

namespace RainbowCursor.Models {
    public class PalettesConfig {

        public ISemanticVersion? FormatVersion { get; }

        public List<PaletteConfig>? Palettes { get; }

        public PalettesConfig(ISemanticVersion? formatVersion, List<PaletteConfig>? palettes) {
            this.FormatVersion = formatVersion;
            this.Palettes = palettes;
        }

    }
}

