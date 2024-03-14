/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using xTile.Dimensions;

using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace FarmAnimalVarietyRedux.Menus
{
    /// <summary>The menu for displaying individual animal information.</summary>
    public class CustomAnimalQueryMenu : IClickableMenu
    {
        /*********
        ** Constants
        *********/
        /// <summary>The default window width.</summary>
        public static readonly int DefaultWidth = 384;

        /// <summary>The default window height.</summary>
        public static readonly int DefaultHeight = 512;


        /*********
        ** Fields
        *********/
        /// <summary>The current stage of the menu.</summary>
        private AnimalQueryMenuStage CurrentStage = AnimalQueryMenuStage.Main;

        /// <summary>The current hover text.</summary>
        private string HoverText;

        /// <summary>The animal the menu is for.</summary>
        private FarmAnimal Animal;

        /// <summary>The name of the parent of the animal.</summary>
        private string ParentName;

        /// <summary>The love level of the animal towards the farmer.</summary>
        private float LoveLevel;

        /// <summary>The current valid building the player is panned to.</summary>
        /// <remarks>This is the index in <see cref="ValidBuildings"/> that the player is currently looking at when choosing a building to put the animal in.<br/>This is used in the <see cref="AnimalQueryMenuStage.Rehoming"/> stage of the menu.</remarks>
        private int CurrentValidBuildingIndex;

        /// <summary>A list of buildings that are valid for the animal to live in.</summary>
        /// <remarks>This is used for the camera panning feature when the player is choosing a building to put the animal in.<br/>This is used in the <see cref="AnimalQueryMenuStage.Rehoming"/> stage of the menu.</remarks>
        private List<Building> ValidBuildings = new List<Building>();

        /// <summary>The name text box.</summary>
        /// <remarks>This is used in the <see cref="AnimalQueryMenuStage.Main"/> stages of the menu.</remarks>
        public TextBox NameTextBox;

        /// <summary>The clickable component for the <see cref="NameTextBox"/>.</summary>
        /// <remarks>This is used in the <see cref="AnimalQueryMenuStage.Main"/> stage of the menu.</remarks>
        public ClickableComponent NameTextBoxComponent;

        /// <summary>The move home button.</summary>
        /// <remarks>This is used in the <see cref="AnimalQueryMenuStage.Main"/> stage of the menu.</remarks>
        public ClickableTextureComponent MoveHomeButton;

        /// <summary>The sell button.</summary>
        /// <remarks>This is used in the <see cref="AnimalQueryMenuStage.Main"/> stage of the menu.</remarks>
        public ClickableTextureComponent SellButton;

        /// <summary>The allow reproduction button.</summary>
        /// <remarks>This is used in the <see cref="AnimalQueryMenuStage.Main"/> stage of the menu.</remarks>
        public ClickableTextureComponent AllowReproductionButton;

        /// <summary>The ok button.</summary>
        /// <remarks>This is used in the <see cref="AnimalQueryMenuStage.Main"/> stages of the menu.</remarks>
        public ClickableTextureComponent OkButton;

        /// <summary>The back button.</summary>
        /// <remarks>This is used in the <see cref="AnimalQueryMenuStage.Rehoming"/> stage of the menu.</remarks>
        public ClickableTextureComponent BackButton;

        /// <summary>The confirm sell button.</summary>
        /// <remarks>This is used in the <see cref="AnimalQueryMenuStage.ConfirmingSell"/> stage of the menu.</remarks>
        public ClickableTextureComponent ConfirmSellButton;

        /// <summary>The deny sell button.</summary>
        /// <remarks>This is used in the <see cref="AnimalQueryMenuStage.ConfirmingSell"/> stage of the menu.</remarks>
        public ClickableTextureComponent DenySellButton;


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="animal">The animal the menu is for.</param>
        public CustomAnimalQueryMenu(FarmAnimal animal)
        {
            Animal = animal;
            Animal.makeSound();

            Game1.player.Halt();
            Game1.player.faceGeneralDirection(Animal.Position, 0, false, false);

            this.xPositionOnScreen = Game1.uiViewport.Width / 2 - AnimalQueryMenu.width / 2;
            this.yPositionOnScreen = Game1.uiViewport.Height / 2 - AnimalQueryMenu.height / 2;

            // parent name
            if (Animal.parentId != -1)
            {
                var parent = Utility.getAnimal(Animal.parentId);
                if (parent != null)
                    ParentName = parent.displayName;
            }

            // name textbox
            NameTextBox = new TextBox(null, null, Game1.dialogueFont, Game1.textColor)
            {
                X = Game1.uiViewport.Width / 2 - 128 - 12,
                Y = this.yPositionOnScreen - 4 + 128,
                Width = 256,
                Height = 192,
                Text = Animal.displayName
            };

            NameTextBoxComponent = new ClickableComponent(bounds: new Rectangle(NameTextBox.X, NameTextBox.Y, NameTextBox.Height, 64), "")
            {
                myID = 0,
                rightNeighborID = 1,
                downNeighborID = 1
            };

            // move home button
            MoveHomeButton = new ClickableTextureComponent(
                bounds: new Rectangle(
                    x: this.xPositionOnScreen + DefaultWidth + 4,
                    y: this.yPositionOnScreen + DefaultHeight - 256 - IClickableMenu.borderWidth,
                    width: 64,
                    height: 64),
                texture: Game1.mouseCursors,
                sourceRect: new Rectangle(16, 384, 16, 16),
                scale: 4
            )
            {
                myID = 1,
                upNeighborID = 0,
                leftNeighborID = 0,
                downNeighborID = 2
            };

            // sell button
            SellButton = new ClickableTextureComponent(
                bounds: new Rectangle(
                    x: this.xPositionOnScreen + DefaultWidth + 4,
                    y: this.yPositionOnScreen + DefaultHeight - 192 - IClickableMenu.borderWidth,
                    width: 64,
                    height: 64),
                texture: Game1.mouseCursors,
                sourceRect: new Rectangle(0, 384, 16, 16),
                scale: 4
            )
            {
                myID = 2,
                upNeighborID = 1,
                downNeighborID = Animal.CanHavePregnancy() ? 3 : 4
            };

            // allow reproduction button
            if (Animal.CanHavePregnancy())
                AllowReproductionButton = new ClickableTextureComponent(
                    bounds: new Rectangle(
                        x: this.xPositionOnScreen + DefaultWidth + 16,
                        y: this.yPositionOnScreen + DefaultHeight - 120 - IClickableMenu.borderWidth,
                        width: 36,
                        height: 36),
                    texture: Game1.mouseCursors,
                    sourceRect: new Rectangle(Animal.allowReproduction ? 128 : 137, 393, 9, 9),
                    scale: 4
                )
                {
                    myID = 3,
                    upNeighborID = 2,
                    downNeighborID = 4
                };

            // ok button
            OkButton = new ClickableTextureComponent(
                bounds: new Rectangle(
                    x: this.xPositionOnScreen + DefaultWidth + 4,
                    y: this.yPositionOnScreen + DefaultHeight - 64 - IClickableMenu.borderWidth,
                    width: 64,
                    height: 64),
                texture: Game1.mouseCursors,
                sourceRect: Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46),
                scale: 1
            )
            {
                myID = 4,
                upNeighborID = Animal.CanHavePregnancy() ? 3 : 2
            };

            // back button
            BackButton = new ClickableTextureComponent(
                bounds: new Rectangle(
                    x: Game1.uiViewport.Width - 128,
                    y: Game1.uiViewport.Height - 128,
                    width: 64,
                    height: 64),
                texture: Game1.mouseCursors,
                sourceRect: Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47),
                scale: 1
            );

            // confirm sell button
            ConfirmSellButton = new ClickableTextureComponent(
                bounds: new Rectangle(
                    x: Game1.uiViewport.Width / 2 - 64 - 4,
                    y: Game1.uiViewport.Height / 2 - 32,
                    width: 64,
                    height: 64),
                texture: Game1.mouseCursors,
                sourceRect: Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46),
                scale: 1
            )
            {
                myID = 10,
                rightNeighborID = 11
            };

            // deny sell button
            DenySellButton = new ClickableTextureComponent(
                bounds: new Rectangle(
                    x: Game1.uiViewport.Width / 2 + 4,
                    y: Game1.uiViewport.Height / 2 - 32,
                    width: 64,
                    height: 64),
                texture: Game1.mouseCursors,
                sourceRect: Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47),
                scale: 1
            )
            {
                myID = 11,
                leftNeighborID = 10
            };

            // fix animals if it's broken
            if (Animal.home == null || Animal.home.indoors.Value == null)
                Utility.fixAllAnimals();

            // get all the valid buildings (this is needed for the building panning feature)
            ValidBuildings = new List<Building>();
            var customAnimal = ModEntry.Instance.Api.GetAnimalByInternalSubtypeName(Animal.type);
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

            LoveLevel = Animal.friendshipTowardFarmer / 1000f;

            if (Game1.options.SnappyMenus)
            {
                this.populateClickableComponentList();
                this.snapToDefaultClickableComponent();
            }
        }

        /// <summary>Sets the <see cref="IClickableMenu.currentlySnappedComponent"/> and snaps the cursor to the <see cref="OkButton"/>.</summary>
        public override void snapToDefaultClickableComponent()
        {
            if (CurrentStage == AnimalQueryMenuStage.Main)
                this.currentlySnappedComponent = this.getComponentWithID(4);
            else if (CurrentStage == AnimalQueryMenuStage.ConfirmingSell)
                this.currentlySnappedComponent = this.getComponentWithID(11);

            this.snapCursorToCurrentSnappedComponent();
        }

        /// <summary>Determine whether it's safe to close the menu.</summary>
        /// <returns><see langword="true"/> if the menu can safely close; otherwise, <see langword="false"/>.</returns>
        public override bool readyToClose() => CurrentStage == AnimalQueryMenuStage.Main;

        /// <summary>Closes the menu safely even in an unsafe context.</summary>
        public override void emergencyShutDown()
        {
            if (readyToClose())
                this.exitThisMenu();

            SetupMainStage();
            this.exitThisMenu();
            Game1.player.forceCanMove();
        }

        /// <summary>Determines if the snappy menu movement should be disabled.</summary>
        /// <returns><see langword="true"/> if the snappy menu movement should be disabled; otherwise, <see langword="false"/>.</returns>
        public override bool overrideSnappyMenuCursorMovementBan() => CurrentStage == AnimalQueryMenuStage.Rehoming;

        /// <summary>Whether the game pad cursor should be clamped.</summary>
        /// <returns><see langword="true"/> if the game pad cursor should be clamped; otherwise, <see langword="false"/>.</returns>
        public override bool shouldClampGamePadCursor() => CurrentStage == AnimalQueryMenuStage.Rehoming;

        /// <summary>Invoked once per tick.</summary>
        /// <param name="time">The game time for the tick update.</param>
        public override void update(GameTime time)
        {
            base.update(time);

            // ensure player is looking for a home for an animal
            if (CurrentStage != AnimalQueryMenuStage.Rehoming)
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

        /// <summary>Invoked when the player presses keyboard key.</summary>
        /// <param name="key">The key that was pressed.</param>
        public override void receiveKeyPress(Keys key)
        {
            base.receiveKeyPress(key);

            // check if player is trying to exit the menu
            if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose())
            {
                if (CurrentStage == AnimalQueryMenuStage.Main)
                {
                    Game1.player.forceCanMove();
                    Game1.exitActiveMenu();
                    Game1.playSound("bigDeSelect");
                }
                else
                {
                    SetupMainStage();
                    Game1.playSound("smallSelect");
                }
            }

            // check if player is picking a home for an animal as the movement keys can be used to pan camera
            if (CurrentStage == AnimalQueryMenuStage.Rehoming)
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
        
        /// <summary>Invoked when the player presses a gamepad button.</summary>
        /// <param name="b">The button that was pressed.</param>
        public override void receiveGamePadButton(Buttons b)
        {
            if (CurrentStage != AnimalQueryMenuStage.Main)
                if (b == Buttons.B)
                {
                    SetupMainStage();
                    Game1.playSound("smallSelect");
                }

            if (CurrentStage == AnimalQueryMenuStage.Rehoming)
            {
                // handle building panning
                if (b == Buttons.LeftShoulder || b == Buttons.LeftTrigger)
                    PanCameraToPreviousBuilding();
                else if (b == Buttons.RightShoulder || b == Buttons.RightTrigger)
                    PanCameraToNextBuilding();
            }
        }

        /// <summary>Invoked when the manu should perform hover actions.</summary>
        /// <param name="x">The X position of the mouse.</param>
        /// <param name="y">The Y position of the mouse.</param>
        public override void performHoverAction(int x, int y)
        {
            HoverText = "";
            if (CurrentStage == AnimalQueryMenuStage.Main)
            {
                // update button scales
                OkButton.tryHover(x, y, .05f);
                SellButton.tryHover(x, y);
                MoveHomeButton.tryHover(x, y);
                if (AllowReproductionButton != null)
                    AllowReproductionButton.tryHover(x, y);

                // update hover text
                if (SellButton.containsPoint(x, y))
                    HoverText = Game1.content.LoadString("Strings\\UI:AnimalQuery_Sell", Animal.getSellPrice());
                if (MoveHomeButton.containsPoint(x, y))
                    HoverText = Game1.content.LoadString("Strings\\UI:AnimalQuery_Move");
                if (AllowReproductionButton != null && AllowReproductionButton.containsPoint(x, y))
                    HoverText = Game1.content.LoadString("Strings\\UI:AnimalQuery_AllowReproduction");
            }
            else if (CurrentStage == AnimalQueryMenuStage.Rehoming)
            {
                // back button
                BackButton.tryHover(x, y, .05f);

                // buildings
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
                    var customAnimal = ModEntry.Instance.Api.GetAnimalByInternalSubtypeName(Animal.type);
                    if (customAnimal != null)
                        foreach (var building in customAnimal.Buildings)
                            if (hoveredBuilding.buildingType.Value.ToLower() == building.ToLower() && !(hoveredBuilding.indoors.Value as AnimalHouse).isFull())
                                highLightColour = Color.LightGreen * .8f;

                    hoveredBuilding.color.Value = highLightColour;
                }
            }
            else if (CurrentStage == AnimalQueryMenuStage.ConfirmingSell)
            {
                ConfirmSellButton.tryHover(x, y, .05f);
                DenySellButton.tryHover(x, y, .05f);
            }
        }

        /// <summary>Invoked when the left mouse button is clicked.</summary>
        /// <param name="x">The X position of the mouse.</param>
        /// <param name="y">The Y position of the mouse.</param>
        /// <param name="playSound">Whether sound should be played.</param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (CurrentStage == AnimalQueryMenuStage.Main)
            {
                // move home button
                if (MoveHomeButton.containsPoint(x, y))
                {
                    CurrentStage = AnimalQueryMenuStage.Rehoming;
                    Game1.playSound("smallSelect");
                    Game1.globalFadeToBlack(SetupRehomingStage);
                }

                // sell button
                if (SellButton.containsPoint(x, y))
                {
                    CurrentStage = AnimalQueryMenuStage.ConfirmingSell;
                    Game1.playSound("smallSelect");
                    this.snapToDefaultClickableComponent();
                }

                // allow reproduction button
                if (AllowReproductionButton != null && AllowReproductionButton.containsPoint(x, y))
                {
                    Game1.playSound("drumkit6");
                    Animal.allowReproduction.Value = !Animal.allowReproduction;

                    if (Animal.allowReproduction)
                        AllowReproductionButton.sourceRect.X = 128;
                    else
                        AllowReproductionButton.sourceRect.X = 137;
                }

                // ok button
                if (OkButton.containsPoint(x, y))
                {
                    NameTextBox.Selected = false;
                    Game1.exitActiveMenu();
                    if (NameTextBox.Text.Length > 0 && !Utility.areThereAnyOtherAnimalsWithThisName(NameTextBox.Text))
                    {
                        Animal.displayName = NameTextBox.Text;
                        Animal.Name = NameTextBox.Text;
                    }
                    Game1.playSound("smallSelect");
                }

                // name text box
                NameTextBox.Update();
            }
            else if (CurrentStage == AnimalQueryMenuStage.Rehoming)
            {
                // back button
                if (BackButton.containsPoint(x, y))
                {
                    Game1.globalFadeToBlack(SetupMainStage);
                    Game1.playSound("smallSelect");
                    return;
                }

                // check for clicked building
                var clickTile = new Vector2(
                    x: (int)((Utility.ModifyCoordinateFromUIScale(x) + Game1.viewport.X) / 64f),
                    y: (int)((Utility.ModifyCoordinateFromUIScale(y) + Game1.viewport.Y) / 64f)
                );

                var selectedBuilding = Game1.getFarm().getBuildingAt(clickTile);

                // ensure a building was actually clicked
                if (selectedBuilding == null)
                    return;

                // ensure building is valid
                var isBuildingValid = false;
                var customAnimal = ModEntry.Instance.Api.GetAnimalByInternalSubtypeName(Animal.type);
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
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:AnimalQuery_Moving_BuildingFull"));
                        return;
                    }

                    // ensure building isn't it's current home
                    if (selectedBuilding == Animal.home)
                    {
                        // show 'That Is My Home' message
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:AnimalQuery_Moving_AlreadyHome"));
                        return;
                    }

                    // change animals home
                    (Animal.home.indoors.Value as AnimalHouse).animalsThatLiveHere.Remove(Animal.myID);
                    if ((Animal.home.indoors.Value as AnimalHouse).animals.ContainsKey(Animal.myID))
                    {
                        (Animal.home.indoors.Value as AnimalHouse).animals.Remove(Animal.myID);
                        (selectedBuilding.indoors.Value as AnimalHouse).animals.Add(Animal.myID, Animal);
                    }

                    Animal.home = selectedBuilding;
                    Animal.homeLocation.Value = new Vector2(selectedBuilding.tileX, selectedBuilding.tileY);
                    (selectedBuilding.indoors.Value as AnimalHouse).animalsThatLiveHere.Add(Animal.myID);
                    Animal.makeSound();
                    Game1.globalFadeToBlack(FinishRehomingStage);
                }
                else
                {
                    // show '{0}s Can't Live There.'
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:AnimalQuery_Moving_CantLiveThere", Animal.shortDisplayType()));
                }
            }
            else if (CurrentStage == AnimalQueryMenuStage.ConfirmingSell)
            {
                // confirm sell button
                if (ConfirmSellButton.containsPoint(x, y))
                {
                    // give player money
                    Game1.player.Money += Animal.getSellPrice();

                    // remove animal from home
                    (Animal.home.indoors.Value as AnimalHouse).animalsThatLiveHere.Remove(Animal.myID);
                    Animal.health.Value = -1;

                    // unreserve grass if it had reserved some
                    if (Animal.foundGrass != null && FarmAnimal.reservedGrass.Contains(Animal.foundGrass))
                        FarmAnimal.reservedGrass.Remove(Animal.foundGrass);

                    // create green smoke particles
                    var numberOfClouds = Animal.frontBackSourceRect.Width / 2;
                    for (int i = 0; i < numberOfClouds; i++)
                    {
                        var greenness = Game1.random.Next(25, 200); // this is be taken away from the red and blue channels to vary the greenness
                        var multiplayer = (Multiplayer)typeof(Game1).GetField("multiplayer", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
                        multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(
                            rowInAnimationTexture: 5,
                            position: Animal.Position + new Vector2(Game1.random.Next(-32, Animal.frontBackSourceRect.Width * 3), Game1.random.Next(-32, Animal.frontBackSourceRect.Height * 3)),
                            color: new Color(255 - greenness, 255, 255 - greenness),
                            animationInterval: (Game1.random.NextDouble() < 0.5) ? 50 : Game1.random.Next(30, 200),
                            sourceRectWidth: 64,
                            sourceRectHeight: 64,
                            delay: (Game1.random.NextDouble() < 0.5) ? Game1.random.Next(0, 600) : 0
                        )
                        {
                            scale = Game1.random.Next(2, 5) * .25f,
                            alpha = Game1.random.Next(2, 5) * .25f,
                            motion = new Vector2(0, (0 - (float)Game1.random.NextDouble()))
                        });
                    }

                    Game1.playSound("newRecipe");
                    Game1.playSound("money");
                    Game1.exitActiveMenu();
                }

                // deny sell button
                if (DenySellButton.containsPoint(x, y))
                {
                    CurrentStage = AnimalQueryMenuStage.Main;
                    Game1.playSound("smallSelect");
                    this.snapToDefaultClickableComponent();
                }
            }
        }

        /// <summary>Invoked when the right mouse button is clicked.</summary>
        /// <param name="x">The X position of the mouse.</param>
        /// <param name="y">The Y position of the mouse.</param>
        /// <param name="playSound">Whether sound should be played.</param>
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (CurrentStage == AnimalQueryMenuStage.Main)
            {
                NameTextBox.Selected = false;
                Game1.exitActiveMenu();
                if (NameTextBox.Text.Length > 0 && !Utility.areThereAnyOtherAnimalsWithThisName(NameTextBox.Text))
                {
                    Animal.displayName = NameTextBox.Text;
                    Animal.Name = NameTextBox.Text;
                }
                Game1.playSound("smallSelect");
            }
            else if (CurrentStage == AnimalQueryMenuStage.Rehoming)
            {
                Game1.globalFadeToBlack(SetupMainStage);
            }
        }

        /// <summary>Invoked when the menu should get drawn.</summary>
        /// <param name="b">The sprite batch to draw the menu to.</param>
        public override void draw(SpriteBatch b)
        {
            if (CurrentStage == AnimalQueryMenuStage.Main || CurrentStage == AnimalQueryMenuStage.ConfirmingSell) // when the animal is being sold, the main menu should be drawn underneath
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
                    y: this.yPositionOnScreen + 128,
                    width: DefaultWidth,
                    height: DefaultHeight - 128,
                    speaker: false,
                    drawOnlyBox: true
                );

                // name box
                NameTextBox.Draw(b);

                // age
                var ageInMonths = (Animal.GetDaysOwned() + 1) / 28 + 1;
                var ageText = (ageInMonths <= 1)
                    ? Game1.content.LoadString("Strings\\UI:AnimalQuery_Age1")
                    : Game1.content.LoadString("Strings\\UI:AnimalQuery_AgeN", ageInMonths);
                if (Animal.age < Animal.ageWhenMature)
                    ageText += Game1.content.LoadString("Strings\\UI:AnimalQuery_AgeBaby"); // append '(Baby)' on the end
                Utility.drawTextWithShadow(
                    b: b,
                    text: ageText,
                    font: Game1.smallFont,
                    position: new Vector2(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 32, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16 + 128),
                    color: Game1.textColor
                );

                // parent
                var yOffset = 0;
                if (ParentName != null)
                {
                    yOffset = 21;
                    Utility.drawTextWithShadow(
                        b: b,
                        text: Game1.content.LoadString("Strings\\UI:AnimalQuery_Parent", ParentName),
                        font: Game1.smallFont,
                        position: new Vector2(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 32, 32 + this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16 + 128),
                        color: Game1.textColor
                    );
                }

                // hearts
                var halfHeart = (LoveLevel * 1000 % 200 >= 100) // half heart refers to which heart should be displayed as a half heart (if any)
                    ? LoveLevel * 1000 / 200
                    : -100;
                for (int i = 0; i < 5; i++)
                {
                    var sourceRectangle = new Rectangle(211 + ((LoveLevel * 1000 <= ((i + 1) * 195)) ? 7 : 0), 428, 7, 6);
                    var layerDepth = .89f;
                    if (halfHeart == 1)
                    {
                        sourceRectangle = new Rectangle(211, 428, 4, 6);
                        layerDepth = .891f;
                    }

                    b.Draw(
                        texture: Game1.mouseCursors,
                        position: new Vector2(this.xPositionOnScreen + 96 + 32 * i, yOffset + this.yPositionOnScreen - 32 + 320),
                        sourceRectangle: sourceRectangle,
                        color: Color.White,
                        rotation: 0,
                        origin: Vector2.Zero,
                        scale: 4,
                        effects: SpriteEffects.None,
                        layerDepth: layerDepth
                    );
                }

                // mood message
                Utility.drawTextWithShadow(
                    b: b,
                    text: Game1.parseText(Animal.getMoodMessage(), Game1.smallFont, AnimalQueryMenu.width - IClickableMenu.spaceToClearSideBorder * 2 - 64),
                    font: Game1.smallFont,
                    position: new Vector2(base.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 32, yOffset + base.yPositionOnScreen + 384 - 64 + 4),
                    color: Game1.textColor
                );

                // buttons
                MoveHomeButton.draw(b);
                SellButton.draw(b);
                OkButton.draw(b);
                if (AllowReproductionButton != null) // this is null when the animal can't reproduce
                    AllowReproductionButton.draw(b);

                // hover text
                if (!string.IsNullOrEmpty(HoverText))
                    IClickableMenu.drawHoverText(b, HoverText, Game1.smallFont);

                // draw the overlayed sell menu if needed
                if (CurrentStage == AnimalQueryMenuStage.ConfirmingSell)
                {
                    // dark background
                    b.Draw(
                        texture: Game1.fadeToBlackRect,
                        destinationRectangle: Game1.graphics.GraphicsDevice.Viewport.Bounds,
                        color: Color.Black * .75f
                    );

                    // menu background
                    Game1.drawDialogueBox(
                        x: Game1.uiViewport.Width / 2 - 160,
                        y: Game1.uiViewport.Height / 2 - 192,
                        width: 320,
                        height: 256,
                        speaker: false,
                        drawOnlyBox: true
                    );

                    // 'Are you sure?' label
                    var confirmText = Game1.content.LoadString("Strings\\UI:AnimalQuery_ConfirmSell");
                    b.DrawString(
                        spriteFont: Game1.dialogueFont,
                        text: confirmText,
                        position: new Vector2(Game1.uiViewport.Width / 2 - Game1.dialogueFont.MeasureString(confirmText).X / 2, Game1.uiViewport.Height / 2 - 96 + 8),
                        color: Game1.textColor
                    );

                    // buttons
                    ConfirmSellButton.draw(b);
                    DenySellButton.draw(b);
                }
            }
            else if (CurrentStage == AnimalQueryMenuStage.Rehoming)
            {
                // back button
                BackButton.draw(b);

                // housing string
                var buildingsString = "";
                var animal = ModEntry.Instance.Api.GetAnimalByInternalSubtypeName(Animal.type);
                if (animal != null)
                    buildingsString = Utilities.ConstructBuildingString(animal.Buildings);
                var housingString = $"Choose a {buildingsString} for your {animal.Name}";

                SpriteText.drawStringWithScrollBackground(
                    b: b,
                    s: housingString,
                    x: Game1.uiViewport.Width / 2 - SpriteText.getWidthOfString(housingString) / 2,
                    y: 16
                );
            }

            this.drawMouse(b);
        }


        /*********
        ** Private Methods
        *********/
        /// <summary>Pans the camera to the previous valid building, when in the <see cref="PurchaseAnimalsMenuStage.HomingAnimal"/> stage.</summary>
        private void PanCameraToPreviousBuilding()
        {
            // validate
            if (CurrentStage != AnimalQueryMenuStage.Rehoming)
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
            if (CurrentStage != AnimalQueryMenuStage.Rehoming)
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

        /// <summary>Sets up the game state ready for the <see cref="AnimalQueryMenuStage.Rehoming"/> stage.</summary>
        private void SetupRehomingStage()
        {
            Game1.displayFarmer = false;
            Game1.currentLocation = Game1.getFarm();
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear();
            Game1.displayHUD = false;
            Game1.viewportFreeze = true;
            Game1.viewport.Location = new Location(3136, 320);
            Game1.panScreen(0, 0);
        }

        /// <summary>Sets up the game state ready for the <see cref="AnimalQueryMenuStage.Main"/> stage when transitionaing from a different stage (not initially loading up the menu).</summary>
        private void SetupMainStage()
        {
            Game1.currentLocation = Game1.player.currentLocation;
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear(() => CurrentStage = AnimalQueryMenuStage.Main);
            Game1.displayHUD = true;
            Game1.viewportFreeze = false;
            Game1.displayFarmer = true;
            this.snapToDefaultClickableComponent();
        }

        /// <summary>Sets the game state ready for exiting the menu from the <see cref="AnimalQueryMenuStage.Rehoming"/> stage.</summary>
        private void FinishRehomingStage()
        {
            Game1.exitActiveMenu();
            Game1.currentLocation = Game1.player.currentLocation;
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear();
            Game1.displayHUD = true;
            Game1.viewportFreeze = false;
            Game1.displayFarmer = true;
            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:AnimalQuery_Moving_HomeChanged"), Color.LimeGreen, 3500f));
        }
    }
}
