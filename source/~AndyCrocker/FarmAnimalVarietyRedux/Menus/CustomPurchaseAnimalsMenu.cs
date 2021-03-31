/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using FarmAnimalVarietyRedux.EqualityComparers;
using FarmAnimalVarietyRedux.Models.Converted;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using xTile.Dimensions;

using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace FarmAnimalVarietyRedux.Menus
{
    /// <summary>The menu for purchasing farm animals.</summary>
    public class CustomPurchaseAnimalsMenu : IClickableMenu
    {
        /*********
        ** Constants
        *********/
        /// <summary>The width of an animal clickable component.</summary>
        public static readonly int IconWidth = 32;


        /*********
        ** Fields
        *********/
        /// <summary>The current stage of the menu.</summary>
        private PurchaseAnimalsMenuStage CurrentStage = PurchaseAnimalsMenuStage.PurchasingAnimal;

        /// <summary>The number of rows that can be shown on the screen at a time.</summary>
        private int NumberOfVisibleRows;

        /// <summary>The total number of rows.</summary>
        private int NumberOfTotalRows;

        /// <summary>The number of icons in each row at the current resolution.</summary>
        private int NumberOfIconsPerRow;

        /// <summary>The row that is at the top menu.</summary>
        /// <remarks>Used from the scroll bar.</remarks>
        private int CurrentRowIndex;

        /// <summary>The clickable animal component that is currently being hovered.</summary>
        private ClickableTextureComponent HoveredAnimalComponent;

        /// <summary>The clickable components for the animals that can be bought.</summary>
        /// <remarks>This is used in the <see cref="PurchaseAnimalsMenuStage.PurchasingAnimal"/> stage of the menu.</remarks>
        public List<ClickableTextureComponent> AnimalComponents = new List<ClickableTextureComponent>();

        /// <summary>The internal name of the animal for all the <see cref="AnimalComponents"/>.</summary>
        /// <remarks>This is used in the <see cref="PurchaseAnimalsMenuStage.PurchasingAnimal"/> stage of the menu.</remarks>
        private Dictionary<ClickableTextureComponent, string> AnimalComponentsInternalName = new Dictionary<ClickableTextureComponent, string>();

        /// <summary>The <see cref="AnimalComponents"/> that should be drawn.</summary>
        private List<ClickableTextureComponent> AnimalComponentsToDraw = new List<ClickableTextureComponent>();

        /// <summary>The scroll bar up arrow component.</summary>
        /// <remarks>This is used in the <see cref="PurchaseAnimalsMenuStage.PurchasingAnimal"/> stage of the menu.</remarks>
        public ClickableTextureComponent UpArrow;

        /// <summary>The scroll bar down arrow component.</summary>
        /// <remarks>This is used in the <see cref="PurchaseAnimalsMenuStage.PurchasingAnimal"/> stage of the menu.</remarks>
        public ClickableTextureComponent DownArrow;

        /// <summary>The scroll bar bounds.</summary>
        /// <remarks>This is used in the <see cref="PurchaseAnimalsMenuStage.PurchasingAnimal"/> stage of the menu.</remarks>
        private Rectangle ScrollBar;

        /// <summary>The scroll bar handle component.</summary>
        /// <remarks>This is used in the <see cref="PurchaseAnimalsMenuStage.PurchasingAnimal"/> stage of the menu.</remarks>
        private ClickableTextureComponent ScrollBarHandle;

        /// <summary>Whether the player is current dragging the <see cref="ScrollBarHandle"/>.</summary>
        /// <remarks>This is used in the <see cref="PurchaseAnimalsMenuStage.PurchasingAnimal"/> stage of the menu.</remarks>
        private bool IsScrolling;

        /// <summary>The offset of the mouse when <see cref="IsScrolling"/>.</summary>
        /// <remarks>This is so starting to drag the <see cref="ScrollBarHandle"/> from the middle, we can keep that offset.</remarks>
        private int ScrollBarHandleOffset;

        /// <summary>The animal the is currently being purchased.</summary>
        /// <remarks>This is used in the <see cref="PurchaseAnimalsMenuStage.HomingAnimal"/> stage of the menu.</remarks>
        private FarmAnimal AnimalBeingPurchased;

        /// <summary>The current valid building the player is panned to.</summary>
        /// <remarks>This is the index in <see cref="ValidBuildings"/> that the player is currently looking at when choosing a building to put a newly bought animal in.<br/>This is used in the <see cref="PurchaseAnimalsMenuStage.HomingAnimal"/> stage of the menu.</remarks>
        private int CurrentValidBuildingIndex;

        /// <summary>A list of buildings that are valid for the animal to live in.</summary>
        /// <remarks>This is used for the camera panning feature when the player is choosing a building to house a newly bought animal in.<br/>This is used in the <see cref="PurchaseAnimalsMenuStage.HomingAnimal"/> stage of the menu.</remarks>
        private List<Building> ValidBuildings = new List<Building>();

        /// <summary>The animal name input.</summary>
        /// <remarks>This is used in the <see cref="PurchaseAnimalsMenuStage.NamingAnimal"/> stage of the menu.</remarks>
        public TextBox NameTextBox;

        /// <summary>The clickable component for the <see cref="NameTextBox"/>.</summary>
        /// <remarks>This is used in the <see cref="PurchaseAnimalsMenuStage.NamingAnimal"/> stage of the menu.</remarks>
        public ClickableComponent NameTextBoxComponent;

        /// <summary>The randomise animal name button.</summary>
        /// <remarks>This is used in the <see cref="PurchaseAnimalsMenuStage.NamingAnimal"/> stage of the menu.</remarks>
        public ClickableTextureComponent RandomButton;

        /// <summary>The price of <see cref="AnimalBeingPurchased"/>.</summary>
        /// <remarks>This is used in the <see cref="PurchaseAnimalsMenuStage.NamingAnimal"/> stage of the menu (to charge the player after they've named the animal).</remarks>
        private int PriceOfAnimal;

        /// <summary>The building the <see cref="AnimalBeingPurchased"/> will live in.</summary>
        /// <remarks>This is used in the <see cref="PurchaseAnimalsMenuStage.NamingAnimal"/> stage of the menu (to put the animal in it's house once named).</remarks>
        private Building NewAnimalHome;

        /// <summary>The ok button.</summary>
        /// <remarks>This is used in the <see cref="PurchaseAnimalsMenuStage.Naming"/> stage of the menu.</remarks>
        public ClickableTextureComponent OkButton;

        /// <summary>The back button.</summary>
        /// <remarks>This is used in all stages of the menu.</remarks>
        public ClickableTextureComponent BackButton;

        /// <summary>A blank black texture used for setting the animal icon when one doesn't exist (in the case an animal was passed that didn't have an icon).</summary>
        /// <remarks>This is used in the <see cref="PurchaseAnimalsMenuStage.PurchasingAnimal"/> stage of the menu.</remarks>
        private static Texture2D BlankTexture;


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="stock">The stock of the animal menu.</param>
        public CustomPurchaseAnimalsMenu(List<StardewValley.Object> stock)
        {
            // generate and cache a blank texture
            if (BlankTexture == null)
            {
                var blankTexture = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
                blankTexture.SetData(new[] { Color.Black });
                BlankTexture = blankTexture;
            }

            // determine the number of icons per row to use, this is to fill the screen as best as possible
            var fiftyPercentWidth = Game1.uiViewport.Width / 100f * 50;
            NumberOfIconsPerRow = (int)(fiftyPercentWidth / (IconWidth * 4));

            // calculate current and max number of rows to display
            var fiftyPercentHeight = Game1.uiViewport.Height / 100f * 50;
            NumberOfVisibleRows = (int)Math.Floor(fiftyPercentHeight / 64f); // 64 is the pixel height of rows
            NumberOfTotalRows = (int)Math.Ceiling(stock.Count / (float)NumberOfIconsPerRow);

            // calculate menu dimensions
            this.width = IconWidth * Math.Min(NumberOfIconsPerRow, stock.Count) * 4 + (IClickableMenu.borderWidth * 2); // 4 is sprite scale
            this.height = Math.Min(NumberOfVisibleRows, NumberOfTotalRows) * 85 + 64 + (IClickableMenu.borderWidth * 2);

            // get the top left position for the background asset
            var backgroundTopLeftPosition = Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height);
            this.xPositionOnScreen = (int)backgroundTopLeftPosition.X;
            this.yPositionOnScreen = (int)backgroundTopLeftPosition.Y - 32;

            // add a clickable texture for each animal
            for (int i = 0; i < stock.Count; i++)
            {
                Texture2D shopIconTexture = null;
                try { shopIconTexture = Game1.content.Load<Texture2D>($"favr{stock[i].Name},shopIcon"); } catch { }
                shopIconTexture ??= BlankTexture;

                // create animal button
                var animalComponent = new ClickableTextureComponent(
                    name: stock[i].salePrice().ToString(),
                    bounds: new Rectangle(
                        x: this.xPositionOnScreen + IClickableMenu.borderWidth + i % NumberOfIconsPerRow * IconWidth * 4, // 4 is the sprite scale
                        y: this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth / 2 + i / NumberOfIconsPerRow * 85,
                        width: IconWidth * 4,
                        height: 16 * 4),
                    label: null,
                    hoverText: stock[i].displayName,
                    texture: shopIconTexture,
                    sourceRect: ModEntry.Instance.AssetManager.GetShopIconSourceRectangle(stock[i].Name),
                    scale: 4,
                    drawShadow: stock[i].Type == null
                )
                {
                    item = stock[i],
                    myID = i,
                    upNeighborID = i - NumberOfIconsPerRow,
                    leftNeighborID = i % NumberOfIconsPerRow == 0 ? -1 : i - 1,
                    rightNeighborID = i % NumberOfIconsPerRow == NumberOfIconsPerRow - 1 ? -1 : i + 1,
                    downNeighborID = i + NumberOfIconsPerRow
                };

                AnimalComponents.Add(animalComponent);
                AnimalComponentsInternalName[animalComponent] = stock[i].Name;
            }

            // scroll bar buttons
            UpArrow = new ClickableTextureComponent(
                bounds: new Rectangle(
                    x: this.xPositionOnScreen + this.width + 16,
                    y: this.yPositionOnScreen + 16 + 64,
                    width: 44,
                    height: 48),
                texture: Game1.mouseCursors,
                sourceRect: new Rectangle(421, 459, 11, 12),
                scale: 4
            )
            {
                myID = 50000,
                downNeighborID = 50001
            };

            DownArrow = new ClickableTextureComponent(
                bounds: new Rectangle(
                    x: this.xPositionOnScreen + this.width + 16,
                    y: this.yPositionOnScreen + this.height - 64,
                    width: 44,
                    height: 48),
                texture: Game1.mouseCursors,
                sourceRect: new Rectangle(421, 472, 11, 12),
                scale: 4
            )
            {
                myID = 50001,
                upNeighborID = 50000
            };

            ScrollBarHandle = new ClickableTextureComponent(
                bounds: new Rectangle(
                    x: UpArrow.bounds.X + 12,
                    y: UpArrow.bounds.Bottom + 4,
                    width: 24,
                    height: 40),
                texture: Game1.mouseCursors,
                sourceRect: new Rectangle(435, 463, 6, 10),
                scale: 4
            );

            ScrollBar = new Rectangle(
                x: ScrollBarHandle.bounds.X,
                y: UpArrow.bounds.Bottom + 4,
                width: ScrollBarHandle.bounds.Width,
                height: DownArrow.bounds.Y - UpArrow.bounds.Bottom - 8
            );

            // back button
            BackButton = new ClickableTextureComponent(
                bounds: new Rectangle(
                    x: this.xPositionOnScreen + this.width,
                    y: this.yPositionOnScreen + this.height,
                    width: 48,
                    height: 48),
                texture: Game1.mouseCursors,
                sourceRect: Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47),
                scale: 1
            );

            // name input text box
            NameTextBox = new TextBox(
                textBoxTexture: null,
                caretTexture: null,
                font: Game1.dialogueFont,
                textColor: Game1.textColor
            )
            {
                X = Game1.uiViewport.Width / 2 - 192,
                Y = Game1.uiViewport.Height / 2,
                Width = 256,
                Height = 192
            };

            NameTextBoxComponent = new ClickableComponent(bounds: new Rectangle(NameTextBox.X, NameTextBox.Y, 192, 48), "")
            {
                myID = 60000,
                rightNeighborID = 60001
            };

            // ok button
            OkButton = new ClickableTextureComponent(
                bounds: new Rectangle(
                    x: NameTextBox.X + NameTextBox.Width + 32 + 4,
                    y: Game1.uiViewport.Height / 2 - 8,
                    width: 64,
                    height: 64),
                texture: Game1.mouseCursors,
                sourceRect: Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46),
                scale: 1
            )
            {
                myID = 60001,
                leftNeighborID = 60000,
                rightNeighborID = 60002
            };


            // random button
            RandomButton = new ClickableTextureComponent(
                bounds: new Rectangle(
                    x: NameTextBox.X + NameTextBox.Width + 64 + 48 - 8,
                    y: Game1.uiViewport.Height / 2 + 4,
                    width: 64,
                    height: 64),
                texture: Game1.mouseCursors,
                sourceRect: new Rectangle(
                    x: 381,
                    y: 361,
                    width: 10,
                    height: 10),
                scale: 4
            )
            {
                myID = 60002,
                leftNeighborID = 60001
            };

            if (Game1.options.SnappyMenus)
            {
                this.populateClickableComponentList();
                snapToDefaultClickableComponent();
            }

            UpdateIconsToDraw();
            UpdateNeighboursForScrollButtons();
        }

        /// <summary>Sets the <see cref="IClickableMenu.currentlySnappedComponent"/> and snaps the cursor to the top left animal component.</summary>
        public override void snapToDefaultClickableComponent()
        {
            if (CurrentStage == PurchaseAnimalsMenuStage.PurchasingAnimal)
                this.currentlySnappedComponent = this.getComponentWithID(CurrentRowIndex * NumberOfIconsPerRow);
            else if (CurrentStage == PurchaseAnimalsMenuStage.NamingAnimal)
                this.currentlySnappedComponent = this.getComponentWithID(60000);

            this.snapCursorToCurrentSnappedComponent();
        }

        /// <summary>Determine whether it's safe to close the menu.</summary>
        /// <returns><see langword="true"/> if the menu can safely close; otherwise, <see langword="false"/>.</returns>
        public override bool readyToClose() => CurrentStage == PurchaseAnimalsMenuStage.PurchasingAnimal;

        /// <summary>Closes the menu safely even in an unsafe context.</summary>
        public override void emergencyShutDown()
        {
            if (readyToClose())
                this.exitThisMenu();

            SetupPurchaseStage(() =>
            {
                this.exitThisMenu();
                Game1.player.forceCanMove();
            });
        }

        /// <summary>Determines if the snappy menu movement should be disabled.</summary>
        /// <returns><see langword="true"/> if the snappy menu movement should be disabled; otherwise, <see langword="false"/>.</returns>
        public override bool overrideSnappyMenuCursorMovementBan() => CurrentStage == PurchaseAnimalsMenuStage.HomingAnimal;

        /// <summary>Whether the game pad cursor should be clamped.</summary>
        /// <returns><see langword="true"/> if the game pad cursor should be clamped; otherwise, <see langword="false"/>.</returns>
        public override bool shouldClampGamePadCursor() => CurrentStage == PurchaseAnimalsMenuStage.HomingAnimal;

        /// <summary>Snaps the cursor to <see cref="IClickableMenu.currentlySnappedComponent"/>.</summary>
        public override void snapCursorToCurrentSnappedComponent()
        {
            if (this.currentlySnappedComponent == null)
                return;

            if (CurrentStage == PurchaseAnimalsMenuStage.HomingAnimal)
                return;

            var disallowedComponents = AnimalComponents.Where(animalComponent => !AnimalComponentsToDraw.Contains(animalComponent));
            if (disallowedComponents.Contains(this.currentlySnappedComponent))
                return;

            base.snapCursorToCurrentSnappedComponent();
        }

        /// <summary>Invoked once per tick.</summary>
        /// <param name="time">The game time for the tick update.</param>
        public override void update(GameTime time)
        {
            base.update(time);

            // ensure player is looking for a home for an animal
            if (CurrentStage != PurchaseAnimalsMenuStage.HomingAnimal)
                return;

            // pan screen
            var mouseX = Game1.getOldMouseX(ui_scale: false) + Game1.viewport.X;
            var mouseY = Game1.getOldMouseY(ui_scale: false) + Game1.viewport.Y;

            if (mouseX - Game1.viewport.X < 64)
                Game1.panScreen(-8, 0);
            else if (mouseX - (Game1.viewport.X + Game1.viewport.Width) >= -64)
                Game1.panScreen(8, 0);

            if (mouseY - Game1.viewport.Y < 64)
                Game1.panScreen(0, -8);
            else if (mouseY - (Game1.viewport.Y + Game1.viewport.Height) >= -64)
                Game1.panScreen(0, 8);

            // run pressed keys twice as often, this is to smooth out camera panning when using the movement keys
            var pressedKeys = Game1.oldKBState.GetPressedKeys();
            foreach (var key in pressedKeys)
                this.receiveKeyPress(key);
        }

        /// <summary>Invoked when the player scrolls the mouse wheel.</summary>
        /// <param name="direction">The direction being scrolled in.</param>
        public override void receiveScrollWheelAction(int direction)
        {
            if (direction > 0)
            {
                PressUpArrow();
                Game1.playSound("shiny4");

                if (this.currentlySnappedComponent != null)
                {
                    var animalComponent = AnimalComponents.FirstOrDefault(animalComponent => animalComponent.myID == this.currentlySnappedComponent.upNeighborID);
                    if (animalComponent != null)
                        this.currentlySnappedComponent = animalComponent;
                }
            }
            else if (direction < 0)
            {
                PressDownArrow();
                Game1.playSound("shiny4");

                if (this.currentlySnappedComponent != null)
                {
                    var animalComponent = AnimalComponents.FirstOrDefault(animalComponent => animalComponent.myID == this.currentlySnappedComponent.downNeighborID);
                    if (animalComponent != null)
                        this.currentlySnappedComponent = animalComponent;
                }
            }

            if (Game1.options.SnappyMenus)
                this.snapCursorToCurrentSnappedComponent();
        }

        /// <summary>Invoked when the player presses keyboard key.</summary>
        /// <param name="key">The key that was pressed.</param>
        public override void receiveKeyPress(Keys key)
        {
            base.receiveKeyPress(key);

            // check if player is trying to exit the menu
            if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose())
            {
                Game1.player.forceCanMove();
                Game1.exitActiveMenu();
                Game1.playSound("bigDeSelect");
            }

            // check if player is picking a home for an animal as the movement keys can be used to pan camera
            if (CurrentStage == PurchaseAnimalsMenuStage.HomingAnimal)
            {
                // handle building panning
                if (key == Keys.Left && Game1.oldKBState.IsKeyUp(Keys.Left))
                    PanCameraToPreviousBuilding();
                else if (key == Keys.Right && Game1.oldKBState.IsKeyUp(Keys.Right))
                    PanCameraToNextBuilding();

                // handle camera panning
                if (!Game1.options.SnappyMenus)
                {
                    if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
                        Game1.panScreen(0, 4);
                    else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
                        Game1.panScreen(4, 0);
                    else if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
                        Game1.panScreen(0, -4);
                    else if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
                        Game1.panScreen(-4, 0);
                }
            }
        }

        /// <summary>Invoked when the player presses a gamepad button.</summary>
        /// <param name="b">The button that was pressed.</param>
        public override void receiveGamePadButton(Buttons b)
        {
            if (CurrentStage != PurchaseAnimalsMenuStage.PurchasingAnimal)
                if (b == Buttons.B)
                {
                    SetupPurchaseStage();
                    Game1.playSound("smallSelect");
                }

            if (CurrentStage == PurchaseAnimalsMenuStage.HomingAnimal)
            {
                // handle building panning
                if (b == Buttons.LeftShoulder || b == Buttons.LeftTrigger)
                    PanCameraToPreviousBuilding();
                else if (b == Buttons.RightShoulder || b == Buttons.RightTrigger)
                    PanCameraToNextBuilding();
            }
        }

        /// <summary>Invoked when the player holds a gamepad button.</summary>
        /// <param name="b">The button that's being held.</param>
        public override void gamePadButtonHeld(Buttons b)
        {
            if (CurrentStage != PurchaseAnimalsMenuStage.PurchasingAnimal)
                return;

            var isUpDirection = b == Buttons.DPadUp || b == Buttons.LeftThumbstickUp;
            var isDownDirection = b == Buttons.DPadDown || b == Buttons.LeftThumbstickDown;
            if (isUpDirection || isDownDirection)
                HandleScrollingWithDirectionalButton(isUpDirection);
        }

        /// <summary>Invoked when the manu should perform hover actions.</summary>
        /// <param name="x">The X position of the mouse.</param>
        /// <param name="y">The Y position of the mouse.</param>
        public override void performHoverAction(int x, int y)
        {
            // back button
            BackButton.tryHover(x, y, .05f);

            if (CurrentStage == PurchaseAnimalsMenuStage.PurchasingAnimal)
            {
                // resize purchase animal state components
                UpArrow.tryHover(x, y, .05f);
                DownArrow.tryHover(x, y, .05f);

                // set and increase the scale of the hovered animal component
                HoveredAnimalComponent = null;
                var initialComponentIndex = CurrentRowIndex * NumberOfIconsPerRow;
                for (int i = initialComponentIndex; i < initialComponentIndex + NumberOfVisibleRows * NumberOfIconsPerRow; i++)
                {
                    // don't try to scale a component that doesn't exist (if the row isn't complete)
                    if (i >= AnimalComponents.Count)
                        break;

                    var animalComponent = AnimalComponents[i];
                    if (animalComponent.containsPoint(x, y))
                    {
                        animalComponent.scale = Math.Min(animalComponent.scale + 0.05f, 4.1f);
                        HoveredAnimalComponent = animalComponent;
                    }
                    else
                        animalComponent.scale = Math.Max(4f, animalComponent.scale - 0.025f);
                }
            }
            else if (CurrentStage == PurchaseAnimalsMenuStage.HomingAnimal)
            {
                var hoverTile = new Vector2(
                    x: (int)((Utility.ModifyCoordinateFromUIScale(x) + Game1.viewport.X) / 64f),
                    y: (int)((Utility.ModifyCoordinateFromUIScale(y) + Game1.viewport.Y) / 64f)
                );

                var farm = Game1.getFarm();

                // colour all buildings white
                foreach (var building in farm.buildings)
                    building.color.Value = Color.White;

                // if the player is hovering over a building highlight it in red or green depending if it's a valid house
                var hoveredBuilding = farm.getBuildingAt(hoverTile);
                if (hoveredBuilding != null)
                {
                    var highLightColour = Color.Red * .8f;

                    // determine if building can be lived in by the animal
                    var customAnimal = ModEntry.Instance.Api.GetAnimalByInternalSubtypeName(AnimalBeingPurchased.type);
                    if (customAnimal != null)
                        foreach (var building in customAnimal.Buildings)
                            if (hoveredBuilding.buildingType.Value.ToLower() == building.ToLower() && !(hoveredBuilding.indoors.Value as AnimalHouse).isFull())
                                highLightColour = Color.LightGreen * .8f;

                    hoveredBuilding.color.Value = highLightColour;
                }
            }
            else if (CurrentStage == PurchaseAnimalsMenuStage.NamingAnimal)
            {
                OkButton.tryHover(x, y, .05f);
                RandomButton.tryHover(x, y, .5f);
            }
        }

        /// <summary>Invoked when the left mouse button is clicked.</summary>
        /// <param name="x">The X position of the mouse.</param>
        /// <param name="y">The Y position of the mouse.</param>
        /// <param name="playSound">Whether sound should be played.</param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            // exit menu button
            if (BackButton.containsPoint(x, y))
            {
                if (CurrentStage == PurchaseAnimalsMenuStage.PurchasingAnimal)
                    this.exitThisMenu(true);
                else
                    SetupPurchaseStage();
            }

            if (CurrentStage == PurchaseAnimalsMenuStage.PurchasingAnimal)
            {
                // check if scroll buttons were clicked if they were visible
                if (NumberOfTotalRows > NumberOfVisibleRows)
                {
                    if (UpArrow.containsPoint(x, y))
                    {
                        PressUpArrow();
                        Game1.playSound("shwip");
                    }

                    if (DownArrow.containsPoint(x, y))
                    {
                        PressDownArrow();
                        Game1.playSound("shwip");
                    }
                }

                if (ScrollBarHandle.containsPoint(x, y))
                {
                    IsScrolling = true;
                    ScrollBarHandleOffset = ScrollBarHandle.bounds.Y - y;
                }

                // check if an animal component was clicked
                foreach (var animalComponent in AnimalComponents)
                {
                    var internalName = AnimalComponentsInternalName[animalComponent];

                    // ensure animal component was clicked and can be bought
                    if (!animalComponent.containsPoint(x, y) || (animalComponent.item as StardewValley.Object).Type != null)
                        continue;

                    // ensure player has enough money
                    if (Game1.player.Money < animalComponent.item.salePrice())
                    {
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString(Path.Combine("Strings", "StringsFromCSFiles:PurchaseAnimalsMenu.cs.11325")), Color.Red, 3500f));
                        continue;
                    }

                    // set up state transition
                    var multiplayer = (Multiplayer)typeof(Game1).GetField("multiplayer", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

                    BackButton.bounds.X = Game1.uiViewport.Width - 128;
                    BackButton.bounds.Y = Game1.uiViewport.Height - 128;
                    Game1.globalFadeToBlack(new Game1.afterFadeFunction(SetupHomingStage));
                    CurrentStage = PurchaseAnimalsMenuStage.HomingAnimal;
                    AnimalBeingPurchased = new FarmAnimal(internalName, multiplayer.getNewID(), Game1.player.UniqueMultiplayerID);
                    Game1.playSound("smallSelect");
                    PriceOfAnimal = animalComponent.item.salePrice();

                    // get all the valid buildings (this is needed for the building panning feature)
                    ValidBuildings = new List<Building>();
                    var customAnimal = ModEntry.Instance.Api.GetAnimalByInternalName(internalName);
                    if (customAnimal != null)
                        foreach (var building in Game1.getFarm().buildings)
                        {
                            var buildingValid = false;
                            foreach (var animalBuilding in customAnimal.Buildings)
                                if (building.buildingType.Value.ToLower() == animalBuilding.ToLower())
                                    buildingValid = true;

                            if (!buildingValid) // ensure animal can live in building
                                continue;

                            // ensure building isn't full
                            var animalHouse = building.indoors.Value as AnimalHouse;
                            if (animalHouse == null || animalHouse.isFull())
                                continue;

                            ValidBuildings.Add(building);
                        }
                    ValidBuildings = ValidBuildings.OrderBy(building => building.tileX.Value).ToList();
                }
            }
            else if (CurrentStage == PurchaseAnimalsMenuStage.HomingAnimal)
            {
                var clickTile = new Vector2(
                    x: (int)((Utility.ModifyCoordinateFromUIScale(x) + Game1.viewport.X) / 64f),
                    y: (int)((Utility.ModifyCoordinateFromUIScale(y) + Game1.viewport.Y) / 64f)
                );

                var selectedBuilding = Game1.getFarm().getBuildingAt(clickTile);

                // ensure a building was actually clicked
                if (selectedBuilding != null)
                {
                    // ensure building is valid
                    var isBuildingValid = false;
                    var customAnimal = ModEntry.Instance.Api.GetAnimalByInternalSubtypeName(AnimalBeingPurchased.type);
                    if (customAnimal != null)
                        foreach (var building in customAnimal.Buildings)
                            if (selectedBuilding.buildingType.Value.ToLower() == building.ToLower())
                                isBuildingValid = true;

                    // get whether building is valid
                    if (isBuildingValid)
                    {
                        // ensure building has space for animal
                        if ((selectedBuilding.indoors.Value as AnimalHouse).isFull())
                        {
                            // show 'That Building Is Full' message
                            Game1.showRedMessage(Game1.content.LoadString(Path.Combine("Strings", "StringsFromCSFiles:PurchaseAnimalsMenu.cs.11321")));
                        }
                        else if (Game1.player.Money >= PriceOfAnimal) // transition to the naming animal state
                        {
                            // play animal sound
                            if (customAnimal.CustomSound != null)
                                customAnimal.CustomSound.Play();
                            else if (!string.IsNullOrEmpty(AnimalBeingPurchased.sound.Value) && AnimalBeingPurchased.sound.Value.ToLower() != "none" && Game1.soundBank != null)
                            {
                                var cue = Game1.soundBank.GetCue(AnimalBeingPurchased.sound.Value);
                                cue.SetVariable("Pitch", 1200 + Game1.random.Next(-200, 201));
                                cue.Play();
                            }

                            NameTextBox.OnEnterPressed += OnTextBoxEnterPressed;
                            NameTextBox.Text = AnimalBeingPurchased.displayName;
                            Game1.keyboardDispatcher.Subscriber = NameTextBox;

                            CurrentStage = PurchaseAnimalsMenuStage.NamingAnimal;
                            NewAnimalHome = selectedBuilding;

                            if (Game1.options.SnappyMenus)
                                this.snapToDefaultClickableComponent();
                        }
                        else
                            Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
                    }
                    else
                    {
                        // show '{0}s Can't Live There.'
                        Game1.showRedMessage(Game1.content.LoadString(Path.Combine("Strings", "StringsFromCSFiles:PurchaseAnimalsMenu.cs.11326"), AnimalBeingPurchased.displayType));
                    }
                }
            }
            else if (CurrentStage == PurchaseAnimalsMenuStage.NamingAnimal)
            {
                if (OkButton.containsPoint(x, y))
                {
                    OnTextBoxEnterPressed(NameTextBox); // just emulate the textbox event to handle animal state
                    Game1.playSound("smallSelect");
                }
                else if (RandomButton.containsPoint(x, y))
                {
                    AnimalBeingPurchased.Name = Dialogue.randomName();
                    AnimalBeingPurchased.displayName = AnimalBeingPurchased.Name;
                    NameTextBox.Text = AnimalBeingPurchased.displayName;
                    RandomButton.scale = RandomButton.baseScale;
                    Game1.playSound("drumkit6");
                }

                NameTextBox.Update();
            }
        }

        /// <summary>Invoked when the left mouse button is held.</summary>
        /// <param name="x">The X position of the mouse.</param>
        /// <param name="y">The Y position of the mouse.</param>
        public override void leftClickHeld(int x, int y)
        {
            if (!IsScrolling)
                return;

            ScrollBarHandle.bounds.Y = (int)MathHelper.Clamp(y - ScrollBarHandleOffset, ScrollBar.Y - ScrollBarHandleOffset, ScrollBar.Bottom - ScrollBarHandle.bounds.Height - ScrollBarHandleOffset) + ScrollBarHandleOffset;

            var amountScrolled = (ScrollBarHandle.bounds.Y - ScrollBar.Y) / (float)(ScrollBar.Height - ScrollBarHandle.bounds.Height);
            var newCurrentRowIndex = MathHelper.Clamp((int)((NumberOfTotalRows - NumberOfVisibleRows) * amountScrolled), 0, NumberOfTotalRows - NumberOfVisibleRows);
            var currentRowIndexDelta = newCurrentRowIndex - CurrentRowIndex;
            while (currentRowIndexDelta != 0)
            {
                if (currentRowIndexDelta < 0)
                    PressUpArrow();
                else
                    PressDownArrow();

                currentRowIndexDelta = newCurrentRowIndex - CurrentRowIndex;
            }

            // set the scroll bar so it sticks to the 'increments'
            SetScrollBarToCurrentIndex();
        }

        /// <summary>Invoked when the left mouse button is released.</summary>
        /// <param name="x">The X position of the mouse.</param>
        /// <param name="y">The Y position of the mouse.</param>
        public override void releaseLeftClick(int x, int y) => IsScrolling = false;

        /// <summary>Invoked when the menu should get drawn.</summary>
        /// <param name="b">The sprite batch to draw the menu to.</param>
        public override void draw(SpriteBatch b)
        {
            if (CurrentStage == PurchaseAnimalsMenuStage.PurchasingAnimal)
            {
                // dark background
                b.Draw(
                    texture: Game1.fadeToBlackRect,
                    destinationRectangle: Game1.graphics.GraphicsDevice.Viewport.Bounds,
                    color: Color.Black * .75f
                );

                // menu background
                Game1.drawDialogueBox(
                    x: this.xPositionOnScreen,
                    y: this.yPositionOnScreen,
                    width: this.width,
                    height: this.height,
                    speaker: false,
                    drawOnlyBox: true
                );

                // redraw the money box over the dark overlay
                Game1.dayTimeMoneyBox.drawMoneyBox(b, -1, -1);

                // animal icons
                foreach (var animalComponent in AnimalComponentsToDraw)
                    animalComponent.draw(b, (animalComponent.item as StardewValley.Object).Type != null ? Color.Black * 0.4f : Color.White, layerDepth: .87f);

                // draw scroll bar if it's needed
                if (NumberOfTotalRows > NumberOfVisibleRows)
                {
                    IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), ScrollBar.X, ScrollBar.Y, ScrollBar.Width, ScrollBar.Height, Color.White, 4);
                    ScrollBarHandle.draw(b);
                    UpArrow.draw(b);
                    DownArrow.draw(b);
                }

                // check if hovered animal is buyable (to show hover text)
                if (HoveredAnimalComponent != null && (HoveredAnimalComponent.item as StardewValley.Object).Type != null)
                {
                    // display not available for purchase message
                    IClickableMenu.drawHoverText(
                        b: b,
                        text: Game1.parseText((HoveredAnimalComponent.item as StardewValley.Object).Type, Game1.dialogueFont, 320),
                        font: Game1.dialogueFont
                    );
                }
            }
            else if (CurrentStage == PurchaseAnimalsMenuStage.HomingAnimal)
            {
                // housing string
                var buildings = "";
                var animal = ModEntry.Instance.Api.GetAnimalByInternalSubtypeName(AnimalBeingPurchased.type);
                if (animal != null)
                    buildings = Utilities.ConstructBuildingString(animal.Buildings);
                var housingString = $"Choose a {buildings} for your new {animal.Name}";

                SpriteText.drawStringWithScrollBackground(
                    b: b,
                    s: housingString,
                    x: Game1.uiViewport.Width / 2 - SpriteText.getWidthOfString(housingString, 999999) / 2,
                    y: 16
                );
            }
            else if (CurrentStage == PurchaseAnimalsMenuStage.NamingAnimal)
            {
                // dark background
                b.Draw(
                    texture: Game1.fadeToBlackRect,
                    destinationRectangle: Game1.graphics.GraphicsDevice.Viewport.Bounds,
                    color: Color.Black * .75f
                );

                // background
                Game1.drawDialogueBox(
                    x: Game1.uiViewport.Width / 2 - 256,
                    y: Game1.uiViewport.Height / 2 - 192 - 32,
                    width: 512,
                    height: 192,
                    speaker: false,
                    drawOnlyBox: true
                );

                // 'Name your new animal' label
                Utility.drawTextWithShadow(
                    b: b,
                    text: Game1.content.LoadString(Path.Combine("Strings", "StringsFromCSFiles:PurchaseAnimalsMenu.cs.11357")),
                    font: Game1.dialogueFont,
                    position: new Vector2((Game1.uiViewport.Width / 2 - 256 + 32 + 8), (Game1.uiViewport.Height / 2 - 128 + 8)),
                    color: Game1.textColor
                );

                // draw textbox
                NameTextBox.Draw(b, true);

                // draw buttons
                OkButton.draw(b);
                RandomButton.draw(b);
            }

            // draw the left info panel
            DrawInfoPanel(b);

            // draw back button and mouse
            BackButton.draw(b);

            if (CurrentStage == PurchaseAnimalsMenuStage.PurchasingAnimal)
                if (Game1.options.SnappyMenus)
                    this.snapCursorToCurrentSnappedComponent();
            this.drawMouse(b);
        }


        /*********
        ** Private Methods
        *********/
        /// <summary>Draws the left information panel.</summary>
        /// <param name="spriteBatch">The <see cref="SpriteBatch"/> to draw the info panel to.</param>
        private void DrawInfoPanel(SpriteBatch spriteBatch)
        {
            if (CurrentStage != PurchaseAnimalsMenuStage.PurchasingAnimal)
                return;

            var isHoveringValidAnimal = false;
            var animal = (CustomAnimal)null;
            if (HoveredAnimalComponent != null && (HoveredAnimalComponent.item as StardewValley.Object).Type == null)
            {
                animal = ModEntry.Instance.Api.GetAnimalByInternalName(AnimalComponentsInternalName[HoveredAnimalComponent]);
                isHoveringValidAnimal = animal != null;
            }

            var infoPanelWidth = (int)(Game1.uiViewport.Width / 100f * 27);

            if (isHoveringValidAnimal) // draw animal info if an animal is being hovered on
            {
                // determine info menu height
                var buildingsString = Utilities.ConstructBuildingString(animal.Buildings);
                var parsedBuildingsString = Game1.parseText($"Buildings: {buildingsString}", Game1.smallFont, infoPanelWidth - 125);
                var buildingStringsHeight = Game1.smallFont.MeasureString(parsedBuildingsString).Y;

                // description height
                var descriptionString = Game1.parseText($"Description: {animal.AnimalShopInfo.Description}", Game1.smallFont, infoPanelWidth - 125);
                var descriptionHeight = Game1.smallFont.MeasureString(descriptionString).Y;

                // products height
                var products = GetAllAnimalProducts(animal).Where(@object => @object.ParentSheetIndex != -1).Distinct(new ObjectParentSheetIndexEqualityComparer()).ToList();
                var productRows = (int)Math.Ceiling(products.Count / 5f);

                // panel height
                var productsHeight = productRows * 64;
                var infoPanelHeight = 410 + buildingStringsHeight + descriptionHeight + productsHeight;

                // info panel background position
                var infoPanelPosition = new Rectangle(
                    x: this.xPositionOnScreen - infoPanelWidth + 24,
                    y: (int)((Game1.uiViewport.Height / 2) - (infoPanelHeight / 2) - 32),
                    width: infoPanelWidth,
                    height: (int)infoPanelHeight
                );

                // draw info panel background
                Game1.drawDialogueBox(
                    x: infoPanelPosition.X,
                    y: infoPanelPosition.Y,
                    width: infoPanelPosition.Width,
                    height: infoPanelPosition.Height,
                    speaker: false,
                    drawOnlyBox: true
                );

                // draw animal name
                SpriteText.drawString(
                    b: spriteBatch,
                    s: HoveredAnimalComponent.hoverText,
                    x: infoPanelPosition.X + 65,
                    y: infoPanelPosition.Y + 115
                );

                // draw cost
                SpriteText.drawString(
                    b: spriteBatch,
                    s: "$" + Game1.content.LoadString(Path.Combine("Strings", "StringsFromCSFiles:LoadGameMenu.cs.11020"), HoveredAnimalComponent.item.salePrice()),
                    x: infoPanelPosition.X + 65,
                    y: infoPanelPosition.Y + 170
                );

                var subtypes = animal.Subtypes.Where(subtype => subtype.IsBuyable);

                // number of varients
                spriteBatch.DrawString(
                    spriteFont: Game1.smallFont,
                    text: $"Available in {subtypes.Count()} {(subtypes.Count() == 1 ? "variety" : "varieties")}",
                    position: new Vector2(infoPanelPosition.X + 65, infoPanelPosition.Y + 235),
                    color: Color.Black
                );

                // mature age
                {
                    // get the range
                    var lowestDaysTillMature = int.MaxValue;
                    var highestDaysTillMature = int.MinValue;
                    foreach (var subtype in subtypes)
                    {
                        if (subtype.DaysTillMature < lowestDaysTillMature)
                            lowestDaysTillMature = subtype.DaysTillMature;
                        if (subtype.DaysTillMature > highestDaysTillMature)
                            highestDaysTillMature = subtype.DaysTillMature;
                    }

                    // construct string
                    var daysTillMatureString = (lowestDaysTillMature == highestDaysTillMature)
                        ? lowestDaysTillMature.ToString()
                        : $"{lowestDaysTillMature} to {highestDaysTillMature}";

                    spriteBatch.DrawString(
                        spriteFont: Game1.smallFont,
                        text: $"Matures in {daysTillMatureString} days",
                        position: new Vector2(infoPanelPosition.X + 65, infoPanelPosition.Y + 285),
                        color: Color.Black
                    );
                }

                // buildings
                spriteBatch.DrawString(
                    spriteFont: Game1.smallFont,
                    text: parsedBuildingsString,
                    position: new Vector2(infoPanelPosition.X + 65, infoPanelPosition.Y + 335),
                    color: Color.Black
                );

                // description
                spriteBatch.DrawString(
                    spriteFont: Game1.smallFont,
                    text: descriptionString,
                    position: new Vector2(infoPanelPosition.X + 65, infoPanelPosition.Y + 350 + buildingStringsHeight),
                    color: Color.Black
                );

                // all product items (greyed out unless shipped)
                for (int i = 0; i < products.Count; i++)
                {
                    var product = products[i];
                    product.drawInMenu(
                        spriteBatch: spriteBatch,
                        location: new Vector2((i % 5) * 64 + infoPanelPosition.X + 65, (i / 5) * 64 + infoPanelPosition.Y + 360 + buildingStringsHeight + descriptionHeight),
                        scaleSize: 1,
                        transparency: 1,
                        layerDepth: 1,
                        drawStackNumber: StackDrawType.Draw,
                        color: Game1.player.basicShipped.ContainsKey(product.ParentSheetIndex) ? Color.White : Color.Black * 0.2f,
                        drawShadow: false
                    );
                }
            }
            else // draw buildings info panel if no valid animal component is being hovered
            {
                // get all farm buildingss
                var buildings = new List<Building>();
                foreach (var building in Game1.getFarm().buildings)
                    if (building.indoors.Value is AnimalHouse)
                        buildings.Add(building);

                // determine the info panel height
                var buildingsStringHeight = buildings.Count * 45;
                var infoPanelHeight = 210 + buildingsStringHeight;

                // info panel background position
                var infoPanelPosition = new Rectangle(
                    x: this.xPositionOnScreen - infoPanelWidth + 24,
                    y: (Game1.uiViewport.Height / 2) - (infoPanelHeight / 2) - 32,
                    width: infoPanelWidth,
                    height: infoPanelHeight
                );

                // draw info panel background
                Game1.drawDialogueBox(
                    x: infoPanelPosition.X,
                    y: infoPanelPosition.Y,
                    width: infoPanelPosition.Width,
                    height: infoPanelPosition.Height,
                    speaker: false,
                    drawOnlyBox: true
                );

                // "Buildings" label
                SpriteText.drawString(
                    b: spriteBatch,
                    s: "Buildings",
                    x: infoPanelPosition.X + 65,
                    y: infoPanelPosition.Y + 115
                );

                // draw each building label
                for (int i = 0; i < buildings.Count; i++)
                {
                    var building = buildings[i];

                    // building type
                    spriteBatch.DrawString(
                        spriteFont: Game1.dialogueFont,
                        text: building.buildingType.Value,
                        position: new Vector2(infoPanelPosition.X + 65, infoPanelPosition.Y + 165 + i * 45),
                        color: Color.Black
                    );

                    // space available
                    var indoors = building.indoors.Value as AnimalHouse;
                    var spaceAvailableText = $"{indoors.Animals.Keys.Count()}/{indoors.animalLimit.Value}";
                    var spaceAvailableTextWidth = Game1.dialogueFont.MeasureString(spaceAvailableText).X;
                    spriteBatch.DrawString(
                        spriteFont: Game1.dialogueFont,
                        text: spaceAvailableText,
                        position: new Vector2(infoPanelPosition.Right - spaceAvailableTextWidth - 65, infoPanelPosition.Y + 165 + i * 45),
                        color: Color.Black
                    );
                }
            }
        }

        /// <summary>Handles scrolling the menu when pressing a directional button.</summary>
        /// <param name="isUp">Whether the direction button is up.</param>
        private void HandleScrollingWithDirectionalButton(bool isUp)
        {
            if (currentlySnappedComponent == null)
                return;

            var currentIndex = AnimalComponents.FindIndex(component => component.myID == this.currentlySnappedComponent.myID);
            var selectedItemRow = currentIndex / NumberOfIconsPerRow;

            if (isUp)
            {
                // ensure the selected row is the top visible row
                if (CurrentRowIndex == selectedItemRow + 1)
                    PressUpArrow();
            }
            else
            {
                // ensure the selected row is the bottom visible row
                if (CurrentRowIndex + NumberOfVisibleRows == selectedItemRow)
                    PressDownArrow();
            }
        }

        /// <summary>Updates the <see cref="AnimalComponentsToDraw"/>.</summary>
        private void UpdateIconsToDraw()
        {
            AnimalComponentsToDraw.Clear();
            var initialComponentIndex = CurrentRowIndex * NumberOfIconsPerRow; // initial index of the current top left component (if it's scrolled down)
            for (int i = initialComponentIndex; i < initialComponentIndex + NumberOfVisibleRows * NumberOfIconsPerRow; i++)
            {
                // don't try to draw a component that doesn't exist (if the row isn't complete)
                if (i >= AnimalComponents.Count)
                    break;

                AnimalComponentsToDraw.Add(AnimalComponents[i]);
            }
        }

        /// <summary>Updates the neighbours of the right-most components to be able to go onto the scroll buttons and for the scroll buttons to be able to go onto the correct component.</summary>
        private void UpdateNeighboursForScrollButtons()
        {
            // ensure scroll buttons should even be calculated
            if (NumberOfTotalRows <= NumberOfVisibleRows)
                return;

            for (int i = 0; i < NumberOfVisibleRows; i++)
            {
                // get the right-most component from this row index
                var rowIndex = CurrentRowIndex + i;
                var componentIndex = rowIndex * NumberOfIconsPerRow + NumberOfIconsPerRow - 1;
                if (componentIndex >= AnimalComponents.Count - 1) // ensure the component exists (if the row isn't complete)
                    continue;

                // go to the up or down arrow depending on which is closer
                int rightNeighbourId;
                if (i + 1 <= NumberOfIconsPerRow / 2f)
                    rightNeighbourId = 50000;
                else
                    rightNeighbourId = 50001;

                var component = AnimalComponents[componentIndex];
                component.rightNeighborID = rightNeighbourId;
            }

            var topRightComponent = AnimalComponents[CurrentRowIndex * NumberOfIconsPerRow + NumberOfIconsPerRow - 1];
            var bottomRightComponentIndex = (CurrentRowIndex + Math.Min(NumberOfVisibleRows, NumberOfTotalRows) - 1) * NumberOfIconsPerRow + NumberOfIconsPerRow - 1;
            if (bottomRightComponentIndex >= AnimalComponents.Count) // if the bottom right icon of the bottom row doesn't exist, use the right-most component of the row below
                bottomRightComponentIndex -= NumberOfIconsPerRow;
            var bottomRightComponent = AnimalComponents[bottomRightComponentIndex];

            UpArrow.leftNeighborID = topRightComponent.myID;
            DownArrow.leftNeighborID = bottomRightComponent.myID;
        }

        /// <summary>Gets all the products a custom animal can drop.</summary>
        /// <param name="customAnimal">The animal to get the products of.</param>
        /// <returns>The products the specified animal can drop.</returns>
        private IEnumerable<StardewValley.Object> GetAllAnimalProducts(CustomAnimal customAnimal)
        {
            // validate
            if (customAnimal == null)
                yield break;

            // look in each subtype
            foreach (var subtype in customAnimal.Subtypes.Where(subtype => subtype.IsBuyable))
                if (subtype.Produce != null)
                    foreach (var product in subtype.Produce)
                    {
                        if (product.DefaultProductId != -1)
                            yield return new StardewValley.Object(product.DefaultProductId, 1);
                        if (product.UpgradedProductId != -1)
                            yield return new StardewValley.Object(product.UpgradedProductId, 1);
                    }
        }

        /// <summary>Performs the <see cref="UpArrow"/> click event.</summary>
        private void PressUpArrow()
        {
            if (CurrentRowIndex <= 0)
                return;

            foreach (var animalComponent in AnimalComponents)
                animalComponent.bounds.Y += 85;

            CurrentRowIndex--;

            SetScrollBarToCurrentIndex();
            UpdateIconsToDraw();
            UpdateNeighboursForScrollButtons();
        }

        /// <summary>Performs the <see cref="DownArrow"/> click event.</summary>
        private void PressDownArrow()
        {
            if (CurrentRowIndex >= NumberOfTotalRows - NumberOfVisibleRows)
                return;

            foreach (var animalComponent in AnimalComponents)
                animalComponent.bounds.Y -= 85;

            CurrentRowIndex++;

            SetScrollBarToCurrentIndex();
            UpdateIconsToDraw();
            UpdateNeighboursForScrollButtons();
        }

        /// <summary>Sets the scroll bar to the current row index.</summary>
        private void SetScrollBarToCurrentIndex()
        {
            if (NumberOfTotalRows <= NumberOfVisibleRows)
                return;

            ScrollBarHandle.bounds.Y = (int)((ScrollBar.Height - ScrollBarHandle.bounds.Height) / (float)(NumberOfTotalRows - NumberOfVisibleRows) * CurrentRowIndex + UpArrow.bounds.Bottom + 4);
        }

        /// <summary>Pans the camera to the previous valid building, when in the <see cref="PurchaseAnimalsMenuStage.HomingAnimal"/> stage.</summary>
        private void PanCameraToPreviousBuilding()
        {
            // validate
            if (CurrentStage != PurchaseAnimalsMenuStage.HomingAnimal)
                return;

            if (ValidBuildings == null || ValidBuildings.Count == 0)
                return;

            // select new index
            if (CurrentValidBuildingIndex - 1 < 0)
                CurrentValidBuildingIndex = ValidBuildings.Count - 1;
            else
                CurrentValidBuildingIndex--;

            // pan screen
            PanCameraToCurrentBuilding();
        }

        /// <summary>Pans the camera to the next valid building, when in the <see cref="PurchaseAnimalsMenuStage.HomingAnimal"/> stage.</summary>
        private void PanCameraToNextBuilding()
        {
            // validate
            if (CurrentStage != PurchaseAnimalsMenuStage.HomingAnimal)
                return;

            if (ValidBuildings == null || ValidBuildings.Count == 0)
                return;

            // select new index
            if (CurrentValidBuildingIndex + 1 >= ValidBuildings.Count)
                CurrentValidBuildingIndex = 0;
            else
                CurrentValidBuildingIndex++;

            // pan screen
            PanCameraToCurrentBuilding();
        }

        /// <summary>Pans the camera to the current building, when in the <see cref="PurchaseAnimalsMenuStage.HomingAnimal"/> stage.</summary>
        private void PanCameraToCurrentBuilding()
        {
            var building = ValidBuildings[CurrentValidBuildingIndex];

            var panAmount = new Point(
                x: building.tileX * 64 - Game1.viewport.X - Game1.viewport.Width / 2 + building.tilesWide * 64 / 2,
                y: building.tileY * 64 - Game1.viewport.Y - Game1.viewport.Height / 2 + building.tilesHigh * 64 / 2
            );

            Game1.panScreen(panAmount.X, panAmount.Y);
        }

        /// <summary>Sets up the game state ready for the <see cref="PurchaseAnimalsMenuStage.HomingAnimal"/> stage.</summary>
        private void SetupHomingStage()
        {
            Game1.displayFarmer = false;
            Game1.currentLocation = Game1.getFarm();
            Game1.player.viewingLocation.Value = "Farm";
            Game1.currentLocation.resetForPlayerEntry();
            Game1.currentLocation.cleanupBeforePlayerExit();
            Game1.globalFadeToClear();
            Game1.displayHUD = false;
            Game1.viewportFreeze = true;
            Game1.viewport.Location = new Location(3136, 320);
            Game1.panScreen(0, 0);
        }

        /// <summary>Sets up the game state ready for the <see cref="PurchaseAnimalsMenuStage.PurchasingAnimal"/> stage when transitionaing from a different stage (not initially loading up the menu).</summary>
        private void SetupPurchaseStage(System.Action callback = null)
        {
            var locationRequest = Game1.getLocationRequest("AnimalShop");
            locationRequest.OnWarp += delegate
            {
                CurrentStage = PurchaseAnimalsMenuStage.PurchasingAnimal;
                Game1.player.viewingLocation.Value = null;
                BackButton.bounds.X = this.xPositionOnScreen + this.width;
                BackButton.bounds.Y = this.yPositionOnScreen + this.height;
                Game1.displayHUD = true;
                Game1.displayFarmer = true;
                NameTextBox.OnEnterPressed -= OnTextBoxEnterPressed;
                NameTextBox.Selected = false;
                Game1.viewportFreeze = false;

                for (int i = 0; i < NumberOfTotalRows; i++)
                    PressUpArrow();

                if (Game1.options.SnappyMenus)
                    this.snapToDefaultClickableComponent();

                callback?.Invoke();
            };
            Game1.warpFarmer(locationRequest, Game1.player.getTileX(), Game1.player.getTileY(), Game1.player.facingDirection);
        }

        /// <summary>Invoked when the enter key is pressed and the <see cref="NameTextBox"/> is selected.</summary>
        /// <param name="textbox">The textbox that contains the name of the animal.</param>
        /// <remarks>This is used for setting up the animal state and returning back to Marnie's shop.</remarks>
        private void OnTextBoxEnterPressed(TextBox textbox)
        {
            if (CurrentStage != PurchaseAnimalsMenuStage.NamingAnimal)
                return;

            if (Game1.activeClickableMenu == null || !(Game1.activeClickableMenu is CustomPurchaseAnimalsMenu))
            {
                NameTextBox.OnEnterPressed -= OnTextBoxEnterPressed;
                typeof(TextBox).GetField("_selected", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(NameTextBox, false);
                return;
            }

            // ensure some thing was actually typed
            if (textbox.Text.Length <= 0)
                return;

            // ensure name isn't already used
            if (Utility.areThereAnyOtherAnimalsWithThisName(textbox.Text))
            {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11308"));
                return;
            }

            // warp player back to animal shop
            var locationRequest = Game1.getLocationRequest("AnimalShop");
            locationRequest.OnWarp += delegate
            {
                // fix game state
                CurrentStage = PurchaseAnimalsMenuStage.PurchasingAnimal;
                Game1.player.viewingLocation.Value = null;
                Game1.displayHUD = true;
                Game1.displayFarmer = true;
                NameTextBox.OnEnterPressed -= OnTextBoxEnterPressed;
                NameTextBox.Selected = false;
                Game1.viewportFreeze = false;

                // set up animal state
                AnimalBeingPurchased.Name = NameTextBox.Text;
                AnimalBeingPurchased.displayName = AnimalBeingPurchased.Name;
                AnimalBeingPurchased.home = NewAnimalHome;
                AnimalBeingPurchased.homeLocation.Value = new Vector2(NewAnimalHome.tileX, NewAnimalHome.tileY);
                AnimalBeingPurchased.setRandomPosition(AnimalBeingPurchased.home.indoors);
                (NewAnimalHome.indoors.Value as AnimalHouse).animals.Add(AnimalBeingPurchased.myID, AnimalBeingPurchased);
                (NewAnimalHome.indoors.Value as AnimalHouse).animalsThatLiveHere.Add(AnimalBeingPurchased.myID);
                Game1.player.Money -= PriceOfAnimal;

                // exit menu and show Marnie purchase animal dialogue
                this.exitThisMenu();
                Game1.player.forceCanMove();
                Game1.drawDialogue(
                    speaker: Game1.getCharacterFromName("Marnie"),
                    dialogue: AnimalBeingPurchased.isMale()
                        ? Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11311", AnimalBeingPurchased.displayName)
                        : Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11314", AnimalBeingPurchased.displayName)
                );
            };

            Game1.warpFarmer(locationRequest, Game1.player.getTileX(), Game1.player.getTileY(), Game1.player.facingDirection);
        }
    }
}
