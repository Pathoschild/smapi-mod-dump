/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sarahvloos/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Extensions;

namespace AlwaysShowBarValues.UIElements
{
    internal abstract class StatBox
    {
        // Box Info
        /// <summary>The running instance of the mod this box displays stats for. Leave null for vanilla stats.</summary>
        internal Mod? ModInstance { get; set; }
        /// <summary>The box's title, as should be displayed in their settings page.</summary>
        internal string BoxName { get; set; }
        /// <summary>Whether this box is valid and should be drawn, added to settings, etc.</summary>
        internal bool IsValid { get; set; } = false;
        /// <summary>A disabled box will never be drawn.</summary>
        internal bool Enabled { get; set; } = true;
        /// <summary>Whether this box should be drawn, regardless of validity.</summary>
        internal bool ShouldDraw { get; set; } = true;
        /// <summary>Whether this box is semi-permanently hidden</summary>
        internal bool IsHidden { get; set; } = false;
        /// <summary>The keys pressed by the user to toggle this box's visibility.</summary>
        internal KeybindList ToggleKey { get; set; }


        // Box Style/Position
        /// <summary>The style for the box's background.</summary>
        internal string BoxStyle { get; set; } = "Round";
        /// <summary>Whether the box should be drawn above or below other HUD elements.</summary>
        internal bool Above { get; set; } = true;
        /// <summary>The player-selected position preset's name.</summary>
        internal string Position { get; set; } = "Top Left";
        /// <summary>The box's horizontal position, as chosen by the user. Ignored unless they chose the Custom preset.</summary>
        internal int X { get; set; } = 0;
        /// <summary>The box's vertical position, as chosen by the user. Ignored unless they chose the Custom preset.</summary>
        internal int Y { get; set; } = 0;
        /// <summary>The box's actual position, according to presets and custom positions.</summary>
        internal Vector2 BoxPosition
        {
            get { return GetPosition(); }
        }


        // Text properties

        /// <summary>The first value to be displayed on the box</summary>
        internal PlayerStat TopValue { get; set; } = new("Dummy", new Rectangle(), new Vector2());
        /// <summary>The second value to be displayed on the box</summary>
        internal PlayerStat BottomValue { get; set; } = new("Dummy", new Rectangle(), new Vector2());
        /// <summary>Whether icons should be to the left of the stat's text and numbers</summary>
        internal bool IconsLeftOfString { get; set; } = true;
        /// <summary>Whether text inside the box should have a shadow</summary>
        internal bool TextShadow { get; set; } = true;
        /// <summary>The size of the message (icon+text+numbers) inside the box</summary>
        internal Vector2 MessageSize
        {
            get
            {
                float x = 24f + Math.Max(TopValue.MaxStringSize.X, BottomValue.MaxStringSize.X);
                float y = 2 * TopValue.MaxStringSize.Y - 8f;
                return new Vector2(x, y);
            }
        }

        /// <summary>Initializes a stat box</summary>
        /// <param name="boxName">The box's name (and title in the configurations)</param>
        /// <param name="toggleKey">The key pressed by the user to toggle its visibility</param>
        /// <param name="modInstance">The running instance of the mod this box displays stats for. Leave blank for vanilla stats.</param>
        internal StatBox(KeybindList toggleKey, string boxName)
        {
            BoxName = boxName;
            ToggleKey = toggleKey;
        }

        /// <summary>Adds an instance to the box.</summary>
        /// <param name="modInstance"></param>
        public void AddModInstance(Mod? modInstance)
        {
            if (modInstance == null) return;
            ModInstance = modInstance;
            IsValid = true;
        }

        /// <summary>Checks whether its toggle key was pressed and toggles its drawing state if it was</summary>
        public void TryToggleShouldDraw()
        {
            if (ToggleKey.JustPressed()) ShouldDraw = !ShouldDraw;
        }

        /// <summary>Tries to update the stats tracked by the box</summary>
        /// <returns>Whether it was successfully able to access the stats it tracks</returns>
        public abstract bool UpdateCurrentStats();

        /// <summary>Given its position preset and X and Y overrides, calculates the box's starting position</summary>
        /// <returns>The box's starting position.</returns>
        private Vector2 GetPosition()
        {
            Rectangle tsarea = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();
            var res = Position switch
            {
                "Custom" => new Vector2(X, Y),
                "Bottom Left" => new Vector2(tsarea.Left + 16, tsarea.Bottom - 120),
                "Center Left" => new Vector2(tsarea.Left + 16, tsarea.Height / 2 - 66),
                "Top Left" => new Vector2(tsarea.Left + 16, tsarea.Top + 16),
                "Top Center" => new Vector2(tsarea.Width / 2 - 96, tsarea.Top + 116),
                "Center Right" => new Vector2(tsarea.Right - 2 * MessageSize.X, tsarea.Height / 2 - 56),
                "Bottom Right" => new Vector2(tsarea.Right - 2 * MessageSize.X - 72, tsarea.Bottom - 120),
                _ => new Vector2(tsarea.Left + 16, tsarea.Bottom - 120),
            };
            if (Game1.uiViewport.Width < 1400) res.Y -= 48f;
            return res;
        }
    }
}
