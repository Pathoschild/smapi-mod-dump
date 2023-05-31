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
using Microsoft.Xna.Framework;

namespace RainbowCursor.Models {
    public class PaletteConfig {

        public string? Id { get; }
        public string? Name { get; }
        public List<Color> Colors { get; }

        public PaletteConfig(string? id, string? name, List<Color>? colors) {
            Id = id;
            Name = name;
            Colors = colors ?? new();
        }
    }
}

