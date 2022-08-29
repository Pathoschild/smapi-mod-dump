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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

using WhereAreThey.Extensions;


namespace WhereAreThey
{
    // Represents one line in overlay
    class OverlayLine
    {
        public long FarmerId { get; private set; } = 0;
        public string Text { get; private set; } = "";
        public bool Bold { get; private set; } = false;
        public SpriteFont Font { get; private set; }
        public OverlayConfig Config { get; private set; }

        // Size of the box line is taking up (roughly, icon might go out a bit)
        public Vector2 Size => size;
        private Vector2 size;

        public OverlayLine(long farmerId, string text, bool bold, SpriteFont font, OverlayConfig config)
        {
            FarmerId = farmerId;
            Text = text;
            Bold = bold;

            Font = font;
            Config = config;

            size = font.MeasureString(text); // This is the best function ever
            size += Config.AfterIcon;  // Account for icon
        }

        // Draw icon and text onto given sprite batch in given position
        public void Draw(SpriteBatch b, Vector2 position)
        {
            var farmer = Game1.getFarmer(FarmerId);
            var iconPos = position + Config.IconOffset;
            var textPos = position;

            if (Config.IconPosition) // On the right
            {
                iconPos += new Vector2(size.X - Config.IconWidth, 0);
            }
            else // On the left
            {
                textPos += Config.AfterIcon;
            }

            // Display farmer icon  // TODO tooltip for icon displaying farmer's name
            if (farmer != null)
                farmer.FarmerRenderer.drawMiniPortrat(b, iconPos, 0, Config.IconBaseScale, 0, farmer);

            // Shadow (I know there's DrawTextWithShadow, but there's no shadow for bold soooo)
            b.DrawString(Font, Text, textPos + new Vector2(2), Config.BackgroundColor);

            // Text
            if (Bold)
                Utility.drawBoldText(b, Text, Font, textPos, Config.TextColor);
            else
                b.DrawString(Font, Text, textPos, Config.TextColor);
        }
    }

    class LocationOverlay : IDisposable
    {
        private readonly ModEntry theMod;

        private OverlayConfig Config => theMod.Config;
        private readonly SpriteFont font = Game1.smallFont;

        // List of current player locations
        private readonly List<(long, string)> locations = new(); 

        // Precomputed values / Can differ between players, therefore PerScreen
        private readonly PerScreen<Rectangle> overlayBounds = new();
        private readonly PerScreen<List<OverlayLine>> lines = new(() => new()); // C# moment

        // Basically screen dimensions
        private static Rectangle Screen => Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea;

        public LocationOverlay(ModEntry mod)
        {
            theMod = mod;

            theMod.Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            theMod.Helper.Events.Display.RenderedHud += OnRenderedHud;
        }

        private bool is15 = false;

        // Update data on game update
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (Context.IsSplitScreen && !Context.IsMainPlayer) return;

            // ~4 times a second
            if (is15 = e.IsMultipleOf(15))
            {
                locations.Clear();

                // Get all farmers locations
                foreach (Farmer farmer in Game1.getOnlineFarmers())
                {
                    var farmerId = farmer.UniqueMultiplayerID;
                    var location = farmer.currentLocation?.Name;

                    if (location != null)
                        locations.Add((farmerId, location));
                }
            }
        }

        // Precalculate size of the overlay and displayed lines
        private void PrepareOverlay()
        {
            lines.Value.Clear();

            // Start with padding (and correct for extra spacing down the line)
            float height = Config.VPadding * 2 - Config.Spacing;
            float width = 0; // Can't start with padding, because of max

            // To check if in the same location
            var currentLocation = Game1.player.currentLocation.Name;

            // Create lines
            foreach (var (farmerId, location) in locations)
            {
                // Check for display self
                if (farmerId == Game1.player.UniqueMultiplayerID && !Config.DisplaySelf)
                    continue;

                var text = GetLocationDisplayName(location);
                var line = new OverlayLine(farmerId, text, 
                    Config.HighlightSameLocation && location == currentLocation, font, Config);

                // Adjust box dimensions
                width = MathF.Max(width, line.Size.X);
                height += line.Size.Y + Config.Spacing;

                lines.Value.Add(line);
            }

            width += Config.HPadding * 2; // Add padding

            int originX = Config.HOffset;
            int originY = Config.VOffset;

            if (Config.Position == Anchor.TopRight || Config.Position == Anchor.BottomRight)
                originX = Screen.Right - (int)width - originX;

            if (Config.Position == Anchor.BottomLeft || Config.Position == Anchor.BottomRight) 
                originY = Screen.Bottom - (int)height - originY;
            
            // Area of the whole overlay
            overlayBounds.Value = new Rectangle(originX, originY, (int)width, (int)height);
        }

        // Check if overlay should be displayed or not
        private bool ShouldHideOverlay()
        {
            if (locations.Count <= 0) return true; // Nothing to show
            if (Config.HideInSingleplayer && !Game1.IsMultiplayer) return true;
            if (Game1.game1.takingMapScreenshot) return true; // Didn't think of it before

            // That's how ToDew does it, seems to be working
            if (Config.HideInCutscene && (Game1.eventUp || Game1.farmEvent != null)) return true;

            // If this ever stops working, try   Game1.whereIsTodaysFest.Equals(Game1.currentLocation.Name)
            if (Config.HideAtFestival && Game1.isFestival()) return true;
            
            return false;
        }

        // Render overlay in UI
        private void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            if (ShouldHideOverlay()) return;

            if (is15) PrepareOverlay();

            if (lines.Value.Count <= 0) return;

            // Semi-transparent background
            e.SpriteBatch.Draw(Game1.fadeToBlackRect, overlayBounds.Value, Config.BackgroundColor);

            var pos = overlayBounds.Value.Location.ToVector2() + Config.Padding;

            // Now display individual lines
            foreach (var line in lines.Value)
            {
                line.Draw(e.SpriteBatch, pos);

                // TODO adjust for flow?
                pos.Y += line.Size.Y + Config.Spacing;
            }
        }

        // Get location display name from i18n filed or form its internal name, if missing
        private string GetLocationDisplayName(string location)
        {
            string name;

            if (location.StartsWith("UndergroundMine"))
            {
                // Mine and Skull Cavern floors

                var key = "MineFloor";
                var floor = int.Parse(Regex.Match(location, @"(\d+)").Value);

                if (floor > 120)  // 1st floor of cavern is just UndergroundMines121
                {
                    key = "SkullCaveFloor";
                    floor -= 120;
                }

                name = theMod.Helper.Translation.Get(key, new { floor });
            }
            else
            {
                // Any other location
                name = theMod.Helper.Translation.Get(location).Default(location.HumanizeName());
            }

            // Name intentionally empty
            if (name == "")
                name = location.HumanizeName();

            return name;
        }

        ~LocationOverlay() => Dispose(false);
        
        protected bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Just in case
        }

        protected void Dispose(bool disposing)
        {
            if (_disposed) return;

            theMod.Helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
            theMod.Helper.Events.Display.RenderedHud -= OnRenderedHud;

            _disposed = true;
        }
    }
}
