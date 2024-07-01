/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Microsoft.Xna.Framework;
using DeluxeJournal.Menus;

namespace DeluxeJournal.Framework.Data
{
    internal class OverlaySettings(Rectangle bounds, bool isVisible, bool isVisibilityLocked, bool isColorSelected, Color customColor)
    {
        public Rectangle Bounds { get; set; } = bounds;

        public bool IsVisible { get; set; } = isVisible;

        public bool IsVisibilityLocked { get; set; } = isVisibilityLocked;

        public bool IsColorSelected { get; set; } = isColorSelected;

        public Color CustomColor { get; set; } = customColor;

        /// <summary>Apply these settings to an <see cref="IOverlay"/>.</summary>
        public void Apply(IOverlay overlay)
        {
            overlay.Bounds = Bounds;
            overlay.IsVisible = IsVisible;
            overlay.IsVisibilityLocked = IsVisibilityLocked;
            overlay.IsColorSelected = IsColorSelected;
            overlay.CustomColor = CustomColor;
        }

        /// <summary>Update these settings using the active state of an <see cref="IOverlay"/>.</summary>
        public void Update(IOverlay overlay)
        {
            Bounds = overlay.Bounds;
            IsVisible = overlay.IsVisible;
            IsVisibilityLocked = overlay.IsVisibilityLocked;
            IsColorSelected = overlay.IsColorSelected;
            CustomColor = overlay.CustomColor;
        }

        /// <summary>Create a new <see cref="OverlaySettings"/> instance with default values.</summary>
        public static OverlaySettings NewDefault()
        {
            return new(new(0, 0, IOverlay.MinWidth, IOverlay.MinHeight), true, false, true, Color.White);
        }
    }
}
