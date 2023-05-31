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
using StardewModdingAPI;

namespace RainbowCursor {
    internal record class ColorPalette(
        string Id,
        IManifest ProvidedBy,
        Func<string> GetName,
        List<Color> Colors,
        Func<string?>? GetTitle = null,
        Func<string?>? GetDescription = null);
}

