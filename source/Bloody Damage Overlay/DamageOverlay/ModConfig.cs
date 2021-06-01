/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://sourceforge.net/p/sdvmod-damage-overlay/
**
*************************************************/

/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using System.Collections.Generic;

namespace DamageOverlay
    {
    public class ModConfig
        {
        public string overlay = "bloody";
        public KeybindList temp_hide_hotkey = KeybindList.Parse("OemSemicolon");
        }

    public class ContentPackContent
        {
        public Dictionary<string, string> overlays;
        }

    public class OverlayDefinition
        {
        public Dictionary<string, string> images;
        public Dictionary<string, TextureDefinition> textures;
        public SortedDictionary<int, string> thresholds;
        }

    public class TextureDefinition
        {
        public string image;
        public float transparency = 0.0f;
        public float rotation = 0.0f;
        //public RGBA blend_color;
        public XYWH crop;

        public override string ToString() => string.Join("|",
            image,
            transparency.ToString(),
            crop?.ToString() ?? "full" //,
            //blend_color?.ToString() ?? "White"
            );
        }

    public class XYWH
        {
        public int X;
        public int Y;
        public int W;
        public int H;

        public override string ToString() => $"{{X:{X} Y:{Y} W:{W} H:{H}}}";
        public Rectangle ToRectangle() => new Rectangle(X, Y, W, H);
        }

    //public class RGBA
    //    {
    //    public float R;
    //    public float G;
    //    public float B;
    //    public float A = 100.0f;

    //    public override string ToString() => $"{{R:{R} G:{G} B:{B} A:{A}}}";
    //    public Color ToColor() => new Color(R, G, B, A);
    //    }
    }
