/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BleakCodex/SpritesInDetail
**
*************************************************/

using System;
using System.Collections.Generic;

namespace SpritesInDetail
{
    public class Content
    {
        public List<Sprite> Sprites { get; set; }

    }

    public class Sprite
    {
        public string Target { get; set; }
        public string? FromFile { get; set; }
        public int? SpriteWidth { get; set; }
        public int? SpriteHeight { get; set; }
        public int? SpriteOriginX { get; set; }
        public int? SpriteOriginY { get; set; }

        public int? WidthScale { get; set; }
        public int? HeightScale { get; set; }

        public BreathType? BreathType { get; set; }
        public int? ChestSourceX { get; set; }
        public int? ChestSourceY { get; set; }
        public int? ChestSourceWidth { get; set; }
        public int? ChestSourceHeight { get; set; }
        public int? ChestAdjustX { get; set; }
        public int? ChestAdjustY { get; set; }

        public Dictionary<string, string>? When { get; set; }

        public List<PixelReplacement> PixelReplacements { get; set; } = new List<PixelReplacement>();

    }

    public class PixelReplacement
    {
        public int? TargetX { get; set; }
        public int? TargetY { get; set; }
        public string? FromFile { get; set; }
    }

    public enum BreathType
    {
        Male,
        Female,
        None
    }
}
