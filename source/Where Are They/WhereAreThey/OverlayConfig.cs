/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/Capaldi12/wherearethey
**
*************************************************/

using System;
using Microsoft.Xna.Framework;


namespace WhereAreThey
{
    // Origin to position overlay around
    enum Anchor { TopLeft, TopRight, BottomLeft, BottomRight }

    // Configuration for the overlay
    class OverlayConfig
    {
        public Color BackgroundColor { get; set; } = Color.Black * 0.3f;
        public Color TextColor { get; set; } = Color.White;

        // Behaviour
        public bool DisplaySelf { get; set; } = true;
        public bool HideInSingleplayer { get; set; } = false;
        public bool HighlightSameLocation { get; set; } = true;
        public bool HideInCutscene { get; set; } = true;
        public bool HideAtFestival { get; set; } = true;

        // Appearance
        internal string position 
        { 
            get => Position.ToString(); 
            set => Position = Enum.Parse<Anchor>(value); 
        }
        public int VOffset { get; set; } = 310;
        public int HOffset { get; set; } = 10;
        public int VPadding { get; set; } = 5;
        public int HPadding { get; set; } = 10;
        public int Spacing { get; set; } = 5;
        public int IconSpacing { get; set; } = 10;

        internal string iconPosition
        {
            get => IconPosition ? "IconRight" : "IconLeft";
            set => IconPosition = value == "IconRight";
        }

        // Backing fields, that actually get saved
        public Anchor Position = Anchor.TopRight;

        public bool IconPosition = false;

        // Some magic numbers regarding icon
        internal int IconWidth { get; set; } = 42;
        internal int IconYOffset { get; set; } = -10;
        internal float IconBaseScale { get; set; } = 2.7f;

        // Convinience properties
        internal Vector2 Padding => new Vector2(HPadding, VPadding);
        internal Vector2 IconOffset => new Vector2(0, IconYOffset);
        internal Vector2 AfterIcon => new Vector2(IconWidth + IconSpacing, 0);
    }
}
