using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Igorious.StardewValley.ShowcaseMod.Constants;
using Igorious.StardewValley.ShowcaseMod.Data;

namespace Igorious.StardewValley.ShowcaseMod.ModConfig
{
    [DebuggerDisplay("[{Type}] {Rows}x{Columns}")]
    public sealed class LayoutConfig
    {
        public LayoutConfig() { }

        public LayoutConfig(ShowcaseLayoutKind type)
        {
            Type = type;
        }

        [DefaultValue(ShowcaseLayoutKind.Fixed)]
        public ShowcaseLayoutKind Type { get; set; } = ShowcaseLayoutKind.Fixed;

        [DefaultValue(1)]
        public float Scale { get; set; } = 1;

        [DefaultValue(1)]
        public int Rows { get; set; } = 1;

        [DefaultValue(1)]
        public int Columns { get; set; } = 1;

        public Bounds SpriteBounds { get; set; } = Bounds.Empty;
        public bool ShouldSerializeSpriteBounds() => SpriteBounds != Bounds.Empty;

        public Bounds AltSpriteBounds { get; set; } = Bounds.Empty;
        public bool ShouldSerializeAltSpriteBounds() => AltSpriteBounds != Bounds.Empty;

        public List<ItemPosition> Positions { get; set; } = new List<ItemPosition>();
        public bool ShouldSerializePositions() => Positions.Any();
    }
}