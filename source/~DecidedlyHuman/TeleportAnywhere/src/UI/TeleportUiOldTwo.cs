/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

namespace TeleportAnywhere.UI
{
    // public class TeleportUi : IClickableMenu
    // {
    //     private readonly UiElement basePanel;
    //     private UiElement buttonFive;
    //     private UiElement buttonFour;
    //     private UiElement buttonOne;
    //     private UiElement buttonThree;
    //
    //     private UiElement buttonTwo;
    //
    //     //private List<(GameLocation location, string displayName)> locations;
    //     private int currentLocation;
    //     private bool eventLookupLoaded = false;
    //     private IModHelper helper;
    //
    //     private ClickableTextureComponent leftArrow;
    //
    //     //private GameLocation[] locations;
    //     private List<GameLocation> locations;
    //     private LocationUtils locationUtils;
    //     private readonly IMonitor monitor;
    //     private ClickableTextureComponent rightArrow;
    //     private TextBox searchBox;
    //     private float startingZoomLevel;
    //     private bool leftShiftHeld = true;
    //
    //     public TeleportUi(LocationUtils locationUtils, IMonitor monitor, IModHelper helper)
    //     {
    //         int startingWidth = 1000;
    //         int startingHeight = 120;
    //         int startingXPos = Game1.uiViewport.Width / 2 - startingWidth / 2;
    //         int startingYPos = 64;
    //         this.currentLocation = 0;
    //         this.monitor = monitor;
    //         InputEvents.InitInput(helper, (s, level) => { monitor.Log(s, level); });
    //         InputEvents.RegisterEvent(KeyPressType.Hold,
    //             Keys.LeftShift,
    //             () => { this.leftShiftHeld = true; },
    //             (s, level) => { monitor.Log(s, level); });
    //         InputEvents.RegisterEvent(KeyPressType.Released,
    //             Keys.LeftShift,
    //             () => { this.leftShiftHeld = false; },
    //             (s, level) => { monitor.Log(s, level); });
    //         this.leftShiftHeld = false;
    //
    //         this.basePanel = new UiElement("Base Panel", "", "", orientation: Orientation.Vertical,
    //             childAlignment: Alignment.Left, elementSpacing: 4, logger: new Logger(monitor));
    //         this.basePanel.MaximumElementsVisible = 10;
    //         // UiElement buttonOne = new UiElement("AddAThing", "Element 1", "Add a new thing.", 100, 32);
    //         // this.basePanel.AddElement(buttonOne);
    //         this.basePanel.ShowScrollBar = true;
    //         this.basePanel.bounds.Y = 134;
    //         this.basePanel.bounds.X = 148;
    //         this.basePanel.ShowSearchBox = true;
    //         this.basePanel.searchBox.OnEnterPressed += sender => { this.SearchBoxUpdated(sender.Text); };
    //         this.basePanel.searchBox.OnTabPressed += sender => { this.SearchBoxUpdated(sender.Text); };
    //
    //         foreach (var location in Game1.locations)
    //             this.basePanel.AddElement(
    //                 new UiElement(
    //                     Translations.GetMapDisplayName(location, Game1.player.farmName.Value),
    //                     Translations.GetMapDisplayName(location, Game1.player.farmName.Value),
    //                     "",
    //                     100,
    //                     32,
    //                     onClick: () => { locationUtils.ObserveLocation(location); }
    //                 )
    //             );
    //
    //         int i = 1;
    //
    //         // buttonOne.OnClick += () =>
    //         // {
    //         //     this.basePanel.AddElement(
    //         //         new UiElement(
    //         //             "NewThing", $"Element {i + 1}.", "", 100, 32
    //         //             )
    //         //         );
    //         //
    //         //     i++;
    //         // };
    //
    //         // buttonTwo = new UiElement("Button Two", $"And this is a {Environment.NewLine}multi-line string!", "", 64, 64);
    //         // buttonThree = new UiElement("Button Three", "", "", 512, 32);
    //         // buttonFour = new UiElement("Button Four", "", "", 100, 64);
    //         // buttonFive = new UiElement("Button Five", "", "", 64, 32);
    //         // UiElement buttonSix  = new UiElement("Button Five", "", "", 400, 32);
    //         //
    //         // basePanel.AddElement(buttonOne);
    //         // basePanel.AddElement(buttonTwo);
    //         // basePanel.AddElement(buttonThree);
    //         // basePanel.AddElement(buttonFour);
    //         // basePanel.AddElement(buttonFive);
    //         // basePanel.AddElement(buttonSix);
    //
    //         // foreach (var location in Game1.locations)
    //         //     this.basePanel.AddElement(
    //         //         new UiElement(
    //         //             Translations.GetMapDisplayName(location, Game1.player.farmName.Value),
    //         //             Translations.GetMapDisplayName(location, Game1.player.farmName.Value),
    //         //             "",
    //         //             100,
    //         //             32
    //         //         )
    //         //     );
    //
    //         this.basePanel.CalculateInitialSize();
    //         this.basePanel.UpdateElements();
    //
    //         this.initialize(this.basePanel.bounds.X, this.basePanel.bounds.Y, this.basePanel.bounds.Width,
    //             this.basePanel.bounds.Height);
    //         this.locationUtils = locationUtils;
    //     }
    //
    //     public bool TeleportDone { get; } = false;
    //
    //
    //     public bool Enabled { get; set; } = false;
    //
    //     private void SearchBoxUpdated(string text)
    //     {
    //         this.basePanel.SearchString = text;
    //         this.basePanel.UpdateElements();
    //     }
    //
    //     public override void performHoverAction(int x, int y)
    //     {
    //         this.basePanel.ReceiveCursorHover(x, y);
    //     }
    //
    //     public override void receiveScrollWheelAction(int direction)
    //     {
    //         // float newZoomLevel = Math.Clamp(Game1.options.desiredBaseZoomLevel + direction * 0.001f * Game1.options.desiredBaseZoomLevel, 0.25f, 2f);
    //         // Game1.options.desiredBaseZoomLevel = newZoomLevel;
    //
    //         direction = Math.Sign(direction);
    //
    //         this.monitor.Log($"Scroll: {Math.Sign(direction)}", LogLevel.Info);
    //
    //         if (direction < 0)
    //             this.basePanel.CurrentTopIndex += this.leftShiftHeld ? 12 : 1;
    //         else
    //             this.basePanel.CurrentTopIndex -= this.leftShiftHeld ? 12 : 1;
    //     }
    //
    //     public override void receiveKeyPress(Keys key)
    //     {
    //         if (key == Keys.Escape)
    //             Game1.activeClickableMenu = null;
    //     }
    //
    //     public override void update(GameTime time)
    //     {
    //         // // width = Game1.uiViewport.Width / 3;
    //         // xPositionOnScreen = Game1.uiViewport.Width / 2 - width / 2;
    //         // rightArrow.bounds = new Rectangle(xPositionOnScreen + width - 64, yPositionOnScreen + height / 4, 64, 64);
    //         // leftArrow.bounds = new Rectangle(xPositionOnScreen, yPositionOnScreen + height / 4, 64, 64);
    //         // searchBox.X = Game1.uiViewport.Width / 2 - searchBox.Width / 2 - 2;
    //         // searchBox.Y = this.yPositionOnScreen + height + 60;
    //         // searchBox.Update();
    //         // searchBox.SelectMe();
    //         //
    //         // int mouseX = Game1.getMouseX();
    //         // int mouseY = Game1.getMouseY();
    //         // int margin = 64;
    //         // int panSpeed = 24;
    //         // var moveLeft = Game1.options.moveLeftButton;
    //         // var moveRight = Game1.options.moveRightButton;
    //         // var moveUp = Game1.options.moveUpButton;
    //         // var moveDown = Game1.options.moveDownButton;
    //         // KeyboardState keyboardState = Game1.GetKeyboardState();
    //         //
    //         // if (!searchBox.Selected)
    //         // {
    //         //     if (mouseX < margin || Game1.isOneOfTheseKeysDown(keyboardState, moveLeft))
    //         //         Game1.panScreen(-panSpeed, 0);
    //         //     if (mouseX > Game1.uiViewport.Width - margin || Game1.isOneOfTheseKeysDown(keyboardState, moveRight))
    //         //         Game1.panScreen(panSpeed, 0);
    //         //     if (mouseY < margin || Game1.isOneOfTheseKeysDown(keyboardState, moveUp))
    //         //         Game1.panScreen(0, -panSpeed);
    //         //     if (mouseY > Game1.uiViewport.Height - margin || Game1.isOneOfTheseKeysDown(keyboardState, moveDown))
    //         //         Game1.panScreen(0, panSpeed);
    //         // }
    //         //
    //         // UpdateMapViewport(locations[currentLocation]);
    //         //
    //         // // KeyboardState keys = Game1.input.GetKeyboardState();
    //         // //
    //         // // if (keys.IsKeyDown(Game1.options.moveLeftButton))
    //     }
    //
    //     private void UpdateMapViewport(GameLocation l)
    //     {
    //         // if (l.map.DisplayWidth < Game1.viewport.Width)
    //         //     Game1.viewport.Location = new Location(l.map.DisplayWidth / 2 - Game1.viewport.Width / 2, Game1.viewport.Location.Y);
    //         // if (l.map.DisplayHeight < Game1.viewport.Height)
    //         //     Game1.viewport.Location = new Location(Game1.viewport.Location.X, l.map.DisplayHeight / 2 - Game1.viewport.Height / 2);
    //     }
    //
    //     public override void receiveLeftClick(int x, int y, bool playSound = true)
    //     {
    //         // if (leftArrow.bounds.Contains(x, y))
    //         // {
    //         //     if (currentLocation - 1 < 0)
    //         //     {
    //         //         currentLocation = locations.Count - 1;
    //         //     }
    //         //     else
    //         //     {
    //         //         currentLocation--;
    //         //     }
    //         //
    //         //     if (locations[currentLocation] != null)
    //         //     {
    //         //         locationUtils.ObserveLocation(locations[currentLocation]);
    //         //     }
    //         // }
    //         //
    //         // if (rightArrow.bounds.Contains(x, y))
    //         // {
    //         //     if (currentLocation + 1 > locations.Count - 1)
    //         //     {
    //         //         currentLocation = 0;
    //         //     }
    //         //     else
    //         //     {
    //         //         currentLocation++;
    //         //     }
    //         //
    //         //     if (locations[currentLocation] != null)
    //         //     {
    //         //         locationUtils.ObserveLocation(locations[currentLocation]);
    //         //     }
    //         // }
    //
    //         // if (searchBox.)
    //
    //         this.basePanel.LeftClick(x, y);
    //     }
    //
    //     public override void receiveRightClick(int x, int y, bool playSound = true)
    //     {
    //         // if (Game1.whereIsTodaysFest != null)
    //         // {
    //         //     if (Game1.whereIsTodaysFest.Equals(locations[currentLocation].NameOrUniqueName))
    //         //     {
    //         //         Game1.showRedMessage("Cannot warp into an event map.");
    //         //
    //         //         return;
    //         //     }
    //         // }
    //         //
    //         // locationUtils.ResetToStartingLocation(locations[currentLocation], Game1.currentCursorTile);
    //         // teleportDone = true;
    //         // Game1.options.desiredBaseZoomLevel = startingZoomLevel;
    //         // Game1.warpFarmer(locations[currentLocation].Name, (int)Game1.currentCursorTile.X, (int)Game1.currentCursorTile.Y, 2);
    //     }
    //
    //     public bool IsWithinBounds(int x, int y)
    //     {
    //         return this.basePanel.bounds.Contains(x, y);
    //     }
    //
    //     public override void draw(SpriteBatch b)
    //     {
    //         // If the menu isn't enabled, just return.
    //         if (!this.Enabled)
    //             return;
    //
    //         this.basePanel.Draw(b);
    //         this.drawMouse(b);
    //     }
    //
    //     public override void leftClickHeld(int x, int y)
    //     {
    //         this.basePanel.LeftClickHeld(x, y);
    //     }
    //
    //     public override void releaseLeftClick(int x, int y)
    //     {
    //         this.basePanel.LeftClickRelease(x, y);
    //     }
    //
    //     // public override void draw(SpriteBatch b)
    //     // {
    //     //     // If the menu isn't enabled, just return.
    //     //     if (!enabled)
    //     //         return;
    //     //
    //     //     int searchBackgroundWidth = width / 2;
    //     //
    //     //     // Draw the box for our search widget.
    //     //     drawTextureBox(
    //     //         b,
    //     //         Game1.menuTexture,
    //     //         new Rectangle(0, 256, 60, 60),
    //     //         Game1.uiViewport.Width / 2 - searchBackgroundWidth / 2,
    //     //         yPositionOnScreen,
    //     //         width / 2,
    //     //         height * 2,
    //     //         Color.White,
    //     //         1f,
    //     //         true
    //     //     );
    //     //
    //     //     // Draw the box for our bounds.
    //     //     drawTextureBox(
    //     //         b,
    //     //         Game1.menuTexture,
    //     //         new Rectangle(0, 256, 60, 60),
    //     //         xPositionOnScreen,
    //     //         yPositionOnScreen,
    //     //         width,
    //     //         height,
    //     //         Color.White,
    //     //         1f,
    //     //         true
    //     //     );
    //     //
    //     //     string locationName = "";
    //     //
    //     //     if (locations[currentLocation].isStructure)
    //     //     {
    //     //         // It's a structure, so we want to get where it is, on which map it's ok.
    //     //         GameLocation structure = locations[currentLocation];
    //     //
    //     //         if (structure.warps.Any())
    //     //             locationName = $"{structure.Name} at X: {structure.warps[0].TargetX}, Y: {structure.warps[0].TargetY} on {structure.warps[0].TargetName}";
    //     //     }
    //     //     else
    //     //         locationName = Translations.GetMapDisplayName(locations[currentLocation], Game1.player.farmName.Value);
    //     //
    //     //     Vector2 farmNameSizeVector = Game1.dialogueFont.MeasureString(locationName);
    //     //     Vector2 searchLabelSizeVector = Game1.dialogueFont.MeasureString("Map Search");
    //     //
    //     //     int farmNameXPos = Game1.uiViewport.Width / 2 - (int)farmNameSizeVector.X / 2;
    //     //     int farmNameYPos = (int)(yPositionOnScreen + height - height / 2 - farmNameSizeVector.Y / 2);
    //     //     int searchLabelXPos = Game1.uiViewport.Width / 2 - (int)searchLabelSizeVector.X / 2;
    //     //     int searchLabelYPos = (int)(yPositionOnScreen + 157 - searchLabelSizeVector.Y / 2);
    //     //
    //     //     // Draw the currently selected location.
    //     //     DrawStringWithShadow(b, Game1.dialogueFont, locationName, new Vector2(farmNameXPos, farmNameYPos), Color.Black, new Color(221, 148, 84));
    //     //
    //     //     // And draw our search label.
    //     //     DrawStringWithShadow(b, Game1.dialogueFont, "Map Search", new Vector2(searchLabelXPos, searchLabelYPos), Color.Black, new Color(221, 148, 84));
    //     //
    //     //     // // And render the location on-screen.
    //     //     // locations[currentLocation].map.Draw(Game1.mapDisplayDevice, new xTile.Dimensions.Rectangle(Game1.uiViewport.Width / 2, Game1.uiViewport.Height / 2, 500, 500));
    //     //
    //     //
    //     //     // Draw our arrows.
    //     //     leftArrow.draw(b);
    //     //     rightArrow.draw(b);
    //     //     searchBox.Draw(b);
    //     //
    //     //     // And draw our highlight square.
    //     //     b.Draw(Game1.mouseCursors,
    //     //         new Vector2(
    //     //             (Game1.currentCursorTile.X * Game1.tileSize) - Game1.viewport.X,
    //     //             (Game1.currentCursorTile.Y * Game1.tileSize) - Game1.viewport.Y) * Game1.options.zoomLevel,
    //     //         new Rectangle(194, 388, 16, 16),
    //     //         Color.White,
    //     //         0f,
    //     //         Vector2.Zero,
    //     //         Game1.options.zoomLevel * 4f,
    //     //         SpriteEffects.None,
    //     //         1f);
    //     //
    //     //     // // And our player, for fun!
    //     //     // b.Draw(Game1.player.FarmerSprite.spriteTexture, new Vector2((Game1.currentCursorTile.X * Game1.tileSize) - Game1.viewport.X, (Game1.currentCursorTile.Y * Game1.tileSize) - Game1.viewport.Y), new Rectangle(0, 0, 6, 32), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
    //     //
    //     //     drawMouse(b);
    //     // }
    // }
}
