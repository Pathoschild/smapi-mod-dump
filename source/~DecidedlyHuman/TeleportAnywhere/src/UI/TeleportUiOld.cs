/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using DecidedlyShared.Constants;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using TeleportAnywhere.Utilities;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace TeleportAnywhere.UI
{
    public class TeleportUiOld : IClickableMenu
    {
        //private List<(GameLocation location, string displayName)> locations;
        private int currentLocation;
        private bool eventLookupLoaded = false;
        private IModHelper helper;

        private readonly ClickableTextureComponent leftArrow;

        //private GameLocation[] locations;
        private readonly List<GameLocation> locations;
        private readonly LocationUtils locationUtils;
        private IMonitor monitor;
        private readonly ClickableTextureComponent rightArrow;
        private readonly TextBox searchBox;
        private readonly float startingZoomLevel;

        public TeleportUiOld(LocationUtils locationUtils, IMonitor monitor, IModHelper helper)
        {
            int startingWidth = 1000;
            int startingHeight = 120;
            int startingXPos = Game1.uiViewport.Width / 2 - startingWidth / 2;
            int startingYPos = 64;
            this.currentLocation = 0;
            this.initialize(startingXPos, startingYPos, startingWidth, startingHeight);
            this.locationUtils = locationUtils;
            this.monitor = monitor;
            this.locations = new List<GameLocation>();
            this.helper = helper;
            this.startingZoomLevel = Game1.options.desiredBaseZoomLevel;
            //eventLookupLoaded = helper.ModRegistry.IsLoaded("shekurika.EventLookup");

            //if (eventLookupLoaded)
            //{

            //}

            this.Enabled = false;

            this.rightArrow = new ClickableTextureComponent("Next Location",
                new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen + this.height / 4, 64, 64), "",
                "Next Location", Game1.mouseCursors, new Rectangle(0, 192, 64, 64), 1f, true);
            this.leftArrow = new ClickableTextureComponent("Previous Location",
                new Rectangle(this.xPositionOnScreen + this.width - 64, this.yPositionOnScreen + this.height / 4, 64,
                    64), "", "Previous Location", Game1.mouseCursors, new Rectangle(0, 256, 64, 64), 1f, true);
            this.searchBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont,
                Game1.textColor);
            this.searchBox.Width = 480;
            // searchBox.Height = 128;
            this.searchBox.X = Game1.uiViewport.Width / 2 - this.searchBox.Width / 2;
            this.searchBox.Y = this.yPositionOnScreen + this.height + 16;
            this.searchBox.Update();
            this.searchBox.OnEnterPressed += this.SearchBoxOnOnEnterPressed;

            foreach (var location in Game1.locations) this.locations.Add(location);

            var names = new List<string>();

            foreach (var location in Game1.locations)
                names.Add(location.NameOrUniqueName);

            foreach (var building in Game1.getFarm().buildings)
                if (building.indoors.Value != null)
                    this.locations.Add(building.indoors.Value);
            // locations.Append(Game1.locations.ToArray());

            var foundLocation = Utility.fuzzyLocationSearch(Game1.currentLocation.NameOrUniqueName);

            if (foundLocation != null) this.currentLocation = this.locations.IndexOf(foundLocation);

            locationUtils.ObserveLocation(this.locations[this.currentLocation]);
        }

        public bool TeleportDone { get; private set; }


        public bool Enabled { get; set; }

        public override void receiveScrollWheelAction(int direction)
        {
            float newZoomLevel =
                Math.Clamp(Game1.options.desiredBaseZoomLevel + direction * 0.001f * Game1.options.desiredBaseZoomLevel,
                    0.25f, 2f);
            Game1.options.desiredBaseZoomLevel = newZoomLevel;
        }

        private void SearchBoxOnOnEnterPressed(TextBox sender)
        {
            string search = sender.Text;
            var foundLocation = Utility.fuzzyLocationSearch(sender.Text);

            // foreach (GameLocation location in locations)
            // {
            //     if (location.Name.Contains(search))
            //         foundLocation = location;
            //     
            //     break;
            // }

            if (foundLocation != null)
            {
                this.currentLocation = this.locations.IndexOf(foundLocation);
                this.locationUtils.ObserveLocation(this.locations[this.currentLocation]);
            }
            // currentLocation = Array.IndexOf(locations, foundLocation);
        }

        public override void receiveKeyPress(Keys key)
        {
            if (key == Keys.Tab) this.searchBox.SelectMe();

            if (key == Keys.Escape)
                Game1.activeClickableMenu = null;
        }

        public override void update(GameTime time)
        {
            // width = Game1.uiViewport.Width / 3;
            this.xPositionOnScreen = Game1.uiViewport.Width / 2 - this.width / 2;
            this.rightArrow.bounds = new Rectangle(this.xPositionOnScreen + this.width - 64,
                this.yPositionOnScreen + this.height / 4, 64, 64);
            this.leftArrow.bounds =
                new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen + this.height / 4, 64, 64);
            this.searchBox.X = Game1.uiViewport.Width / 2 - this.searchBox.Width / 2 - 2;
            this.searchBox.Y = this.yPositionOnScreen + this.height + 60;
            this.searchBox.Update();

            int mouseX = Game1.getMouseX();
            int mouseY = Game1.getMouseY();
            int margin = 64;
            int panSpeed = 24;
            var moveLeft = Game1.options.moveLeftButton;
            var moveRight = Game1.options.moveRightButton;
            var moveUp = Game1.options.moveUpButton;
            var moveDown = Game1.options.moveDownButton;
            var keyboardState = Game1.GetKeyboardState();

            if (!this.searchBox.Selected)
            {
                if (mouseX < margin || Game1.isOneOfTheseKeysDown(keyboardState, moveLeft))
                    Game1.panScreen(-panSpeed, 0);
                if (mouseX > Game1.uiViewport.Width - margin || Game1.isOneOfTheseKeysDown(keyboardState, moveRight))
                    Game1.panScreen(panSpeed, 0);
                if (mouseY < margin || Game1.isOneOfTheseKeysDown(keyboardState, moveUp))
                    Game1.panScreen(0, -panSpeed);
                if (mouseY > Game1.uiViewport.Height - margin || Game1.isOneOfTheseKeysDown(keyboardState, moveDown))
                    Game1.panScreen(0, panSpeed);
            }

            this.UpdateMapViewport(this.locations[this.currentLocation]);

            // KeyboardState keys = Game1.input.GetKeyboardState();
            //
            // if (keys.IsKeyDown(Game1.options.moveLeftButton))
        }

        private void UpdateMapViewport(GameLocation l)
        {
            if (l.map.DisplayWidth < Game1.viewport.Width)
                Game1.viewport.Location = new Location(l.map.DisplayWidth / 2 - Game1.viewport.Width / 2,
                    Game1.viewport.Location.Y);
            if (l.map.DisplayHeight < Game1.viewport.Height)
                Game1.viewport.Location = new Location(Game1.viewport.Location.X,
                    l.map.DisplayHeight / 2 - Game1.viewport.Height / 2);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.leftArrow.bounds.Contains(x, y))
            {
                if (this.currentLocation - 1 < 0)
                    this.currentLocation = this.locations.Count - 1;
                else
                    this.currentLocation--;

                if (this.locations[this.currentLocation] != null)
                    this.locationUtils.ObserveLocation(this.locations[this.currentLocation]);
            }

            if (this.rightArrow.bounds.Contains(x, y))
            {
                if (this.currentLocation + 1 > this.locations.Count - 1)
                    this.currentLocation = 0;
                else
                    this.currentLocation++;

                if (this.locations[this.currentLocation] != null)
                    this.locationUtils.ObserveLocation(this.locations[this.currentLocation]);
            }

            // if (searchBox.)
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (Game1.whereIsTodaysFest != null)
                if (Game1.whereIsTodaysFest.Equals(this.locations[this.currentLocation].NameOrUniqueName))
                {
                    Game1.showRedMessage("Cannot warp into an event map.");

                    return;
                }

            this.locationUtils.ResetToStartingLocation(this.locations[this.currentLocation], Game1.currentCursorTile);
            this.TeleportDone = true;
            Game1.options.desiredBaseZoomLevel = this.startingZoomLevel;
            // Game1.warpFarmer(locations[currentLocation].Name, (int)Game1.currentCursorTile.X, (int)Game1.currentCursorTile.Y, 2);
        }

        private void OldLogic()
        {
            // drawTextureBox(
            //     b,
            //     Game1.menuTexture,
            //     new Rectangle(0, 256, 60, 60),
            //     xPositionOnScreen,
            //     yPositionOnScreen,
            //     width,
            //     height,
            //     Color.White,
            //     1f,
            //     true
            // );
            //
            // foreach (Layer layer in location.map.Layers)
            // {
            //     if (!layer.Id.Equals("Paths"))
            //     {
            //         // layer.Draw(Game1.mapDisplayDevice, new xTile.Dimensions.Rectangle(-xPositionOnScreen, -yPositionOnScreen, width, height), Location.Origin, false, 1);
            //         // layer.Draw(Game1.mapDisplayDevice, new xTile.Dimensions.Rectangle(0, 0, layer.DisplayWidth, layer.DisplayHeight), new Location(-100, -100), false, 1);
            //         // layer.Draw(Game1.mapDisplayDevice, new xTile.Dimensions.Rectangle(-xPositionOnScreen - 4 * 5, -yPositionOnScreen - 4 * 5, width - 4 * 14, height - 4 * 14), Location.Origin, false, 1);
            //         // XnaDisplayDevice device = new XnaDisplayDevice(Game1.content, Game1.game1.GraphicsDevice);
            //         //device.
            //
            //         var oldRenderTargets = Game1.graphics.GraphicsDevice.GetRenderTargets();
            //         RenderTarget2D newTarget = new RenderTarget2D(Game1.graphics.GraphicsDevice, layer.DisplayWidth, layer.DisplayHeight);
            //         Game1.SetRenderTarget(newTarget);
            //         layer.Draw(Game1.mapDisplayDevice, new xTile.Dimensions.Rectangle(xPositionOnScreen, yPositionOnScreen, layer.DisplayWidth, layer.DisplayHeight), Location.Origin, false, 1);
            //         // Game1.graphics.GraphicsDevice.SetRenderTargets(oldRenderTargets);
            //         // Game1.SetRenderTarget((RenderTarget2D)oldRenderTargets[0].RenderTarget);
            //     }
            // }
            //
            // drawMouse(b);
            // base.draw(b);
        }

        public override void draw(SpriteBatch b)
        {
            // If the menu isn't enabled, just return.
            if (!this.Enabled)
                return;

            int searchBackgroundWidth = this.width / 2;

            // Draw the box for our search widget.
            drawTextureBox(
                b,
                Game1.menuTexture,
                new Rectangle(0, 256, 60, 60),
                Game1.uiViewport.Width / 2 - searchBackgroundWidth / 2, this.yPositionOnScreen, this.width / 2,
                this.height * 2,
                Color.White
            );

            // Draw the box for our bounds.
            drawTextureBox(
                b,
                Game1.menuTexture,
                new Rectangle(0, 256, 60, 60), this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height,
                Color.White
            );

            string locationName = "";

            if (this.locations[this.currentLocation].isStructure)
            {
                // It's a structure, so we want to get where it is, on which map it's ok.
                var structure = this.locations[this.currentLocation];

                if (structure.warps.Any())
                    locationName =
                        $"{structure.Name} at X: {structure.warps[0].TargetX}, Y: {structure.warps[0].TargetY} on {structure.warps[0].TargetName}";
            }
            else
                locationName =
                    Translations.GetMapDisplayName(this.locations[this.currentLocation], Game1.player.farmName.Value);

            var farmNameSizeVector = Game1.dialogueFont.MeasureString(locationName);
            var searchLabelSizeVector = Game1.dialogueFont.MeasureString("Map Search");

            int farmNameXPos = Game1.uiViewport.Width / 2 - (int)farmNameSizeVector.X / 2;
            int farmNameYPos = (int)(this.yPositionOnScreen + this.height - this.height / 2 - farmNameSizeVector.Y / 2);
            int searchLabelXPos = Game1.uiViewport.Width / 2 - (int)searchLabelSizeVector.X / 2;
            int searchLabelYPos = (int)(this.yPositionOnScreen + 157 - searchLabelSizeVector.Y / 2);

            // Draw the currently selected location.
            this.DrawStringWithShadow(b, Game1.dialogueFont, locationName, new Vector2(farmNameXPos, farmNameYPos),
                Color.Black, new Color(221, 148, 84));

            // And draw our search label.
            this.DrawStringWithShadow(b, Game1.dialogueFont, "Map Search",
                new Vector2(searchLabelXPos, searchLabelYPos), Color.Black, new Color(221, 148, 84));

            // // And render the location on-screen.
            // locations[currentLocation].map.Draw(Game1.mapDisplayDevice, new xTile.Dimensions.Rectangle(Game1.uiViewport.Width / 2, Game1.uiViewport.Height / 2, 500, 500));


            // Draw our arrows.
            this.leftArrow.draw(b);
            this.rightArrow.draw(b);
            this.searchBox.Draw(b);

            // And draw our highlight square.
            b.Draw(Game1.mouseCursors,
                new Vector2(
                    Game1.currentCursorTile.X * Game1.tileSize - Game1.viewport.X,
                    Game1.currentCursorTile.Y * Game1.tileSize - Game1.viewport.Y) * Game1.options.zoomLevel,
                new Rectangle(194, 388, 16, 16),
                Color.White,
                0f,
                Vector2.Zero,
                Game1.options.zoomLevel * 4f,
                SpriteEffects.None,
                1f);

            // // And our player, for fun!
            // b.Draw(Game1.player.FarmerSprite.spriteTexture, new Vector2((Game1.currentCursorTile.X * Game1.tileSize) - Game1.viewport.X, (Game1.currentCursorTile.Y * Game1.tileSize) - Game1.viewport.Y), new Rectangle(0, 0, 6, 32), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

            this.drawMouse(b);
        }

        /// <summary>
        ///     Draw a string using the specified SpriteFont twice. One first and offset by two pixels (the shadow), and the second
        ///     in precisely the specified position.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch doing the drawing.</param>
        /// <param name="font">The SpriteFont to use.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="position">The position for the base text (not the shadow).</param>
        /// <param name="textColour">The <see cref="Color" /> of the text.</param>
        /// <param name="shadowColour">The <see cref="Color" /> of the shadow.</param>
        public void DrawStringWithShadow(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position,
            Color textColour, Color shadowColour, int xOffset = 2, int yOffset = 2)
        {
            // Draw the shadow...
            spriteBatch.DrawString(
                font,
                text,
                position + new Vector2(xOffset, yOffset),
                shadowColour
            );

            // ...and draw the text itself.
            spriteBatch.DrawString(
                font,
                text,
                position,
                textColour
            );
        }
    }
}
