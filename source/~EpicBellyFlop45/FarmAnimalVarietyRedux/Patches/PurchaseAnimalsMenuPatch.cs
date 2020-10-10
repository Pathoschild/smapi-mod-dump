/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EpicBellyFlop45/StardewMods
**
*************************************************/

using FarmAnimalVarietyRedux.Models;
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

using SObject = StardewValley.Object;

namespace FarmAnimalVarietyRedux.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="PurchaseAnimalsMenu"/> class.</summary>
    internal class PurchaseAnimalsMenuPatch
    {
        /*********
        ** Fields
        *********/
        /// <summary>The number of animal clickable components can fit on a row on a 720p -> 1080p resolution viewport.</summary>
        private const int NumberOfIconsPerRow1280 = 5;

        /// <summary>The number of animal clickable components can fit on a row on a 1080p+ resolution viewport.</summary>
        private const int NumberOfIconsPerRow1920 = 9;

        /// <summary>The width of an animal clickable component.</summary>
        private const int IconWidth = 32;

        /// <summary>A list of buildings that are valid for the animal to live in.</summary>
        /// <remarks>This is used for the camera panning feature.</remarks>
        private static List<Building> ValidBuildings = new List<Building>();

        /// <summary>The current valid building the player is panned to.</summary>
        private static int CurrentValidBuildingIndex;

        /// <summary>The number of rows that can be shown on the screen at a time.</summary>
        private static int NumberOfVisibleRows;

        /// <summary>The total number of rows.</summary>
        private static int NumberOfTotalRows;

        /// <summary>The number of icons in each row at the current resolution.</summary>
        private static int NumberOfIconsPerRow;

        /// <summary>The scroll bar up arrow component.</summary>
        private static ClickableTextureComponent UpArrow;

        /// <summary>The scroll bar down arrow component.</summary>
        private static ClickableTextureComponent DownArrow;

        /// <summary>The scroll bar component.</summary>
        private static ClickableTextureComponent ScrollBar;

        /// <summary>The scroll bar handle bounds.</summary>
        private static Rectangle ScrollBarHandle;

        /// <summary>The row that is at the top menu.</summary>
        /// <remarks>Used from the scroll bar.</remarks>
        private static int CurrentRowIndex;

        /// <summary>A blank black texture used for setting the animal icon when non exists (in the case an animal was passed that didn't have an icon).</summary>
        private static Texture2D BlankTexture;


        /*********
        ** Internal Methods
        *********/
        /// <summary>The prefix for the constructor.</summary>
        /// <param name="stock">The menu stock.</param>
        /// <param name="__instance">The current <see cref="PurchaseAnimalsMenu"/> instance being patched.</param>
        internal static bool ConstructorPrefix(List<StardewValley.Object> stock, PurchaseAnimalsMenu __instance)
        {
            // generate and cache blank texture
            if (BlankTexture == null)
            {
                var blankTexture = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
                blankTexture.SetData(new Color[] { Color.White });
                BlankTexture = blankTexture;
            }

            CurrentRowIndex = 0;

            // determine the number of icons per row to use, this is to fill the screen as best as possible
            if (Game1.graphics.GraphicsDevice.Viewport.Width >= 1920)
                NumberOfIconsPerRow = NumberOfIconsPerRow1920;
            else
                NumberOfIconsPerRow = NumberOfIconsPerRow1280;

            // calculate current and max number of rows to display
            // visible rows should take up no more than 60% of the screen
            NumberOfVisibleRows = (int)Math.Floor((double)((Game1.graphics.GraphicsDevice.Viewport.Height / 100f * 60f) / 64)); // 64 is the pixel height of rows
            NumberOfTotalRows = (int)Math.Ceiling(stock.Count / (double)NumberOfIconsPerRow);

            // calculate menu dimensions
            __instance.width = IconWidth * Math.Min(NumberOfIconsPerRow, stock.Count) * 4 + (IClickableMenu.borderWidth * 2); // 4 is sprite scale
            __instance.height = Math.Min(NumberOfVisibleRows, NumberOfTotalRows) * 85 + 64 + (IClickableMenu.borderWidth * 2);

            // get the top left position for the background asset
            Vector2 backgroundTopLeftPosition = Utility.getTopLeftPositionForCenteringOnScreen(__instance.width, __instance.height);
            __instance.xPositionOnScreen = (int)backgroundTopLeftPosition.X;
            __instance.yPositionOnScreen = (int)backgroundTopLeftPosition.Y - 32;

            __instance.animalsToPurchase = new List<ClickableTextureComponent>();
            // add a clickable texture for each animal
            for (int i = 0; i < stock.Count; i++)
            {
                var animal = ModEntry.Instance.Api.GetAnimalByName(stock[i].Name);
                var shopIconTexture = animal.Data.AnimalShopInfo.ShopIcon ?? BlankTexture;

                // create animal button
                var animalComponent = new ClickableTextureComponent(
                    name: stock[i].salePrice().ToString(),
                    bounds: new Rectangle(
                        x: __instance.xPositionOnScreen + IClickableMenu.borderWidth + i % NumberOfIconsPerRow * IconWidth * 4, // 4 is the sprite scale
                        y: __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth / 2 + i / NumberOfIconsPerRow * 85,
                        width: IconWidth * 4,
                        height: 16 * 4),
                    label: null,
                    hoverText: stock[i].Name,
                    texture: shopIconTexture,
                    sourceRect: new Rectangle(0, 0, 32, 16),
                    scale: 4,
                    drawShadow: stock[i].Type == null
                );
                animalComponent.item = stock[i];
                animalComponent.myID = i;
                animalComponent.upNeighborID = i - NumberOfIconsPerRow;
                animalComponent.leftNeighborID = i % NumberOfIconsPerRow == 0 ? -1 : i - 1;
                animalComponent.rightNeighborID = i % NumberOfIconsPerRow == NumberOfIconsPerRow - 1 ? -1 : i + 1;
                animalComponent.downNeighborID = i + NumberOfIconsPerRow;

                __instance.animalsToPurchase.Add(animalComponent);
            }

            // scroll bar buttons
            // TODO: controller support for buttons
            // TODO: only show buttons if they're actually needed
            UpArrow = new ClickableTextureComponent(
                bounds: new Rectangle(
                    x: __instance.xPositionOnScreen + __instance.width + 16,
                    y: __instance.yPositionOnScreen + 16 + 64,
                    width: 44,
                    height: 48),
                texture: Game1.mouseCursors,
                sourceRect: new Rectangle(421, 459, 11, 12),
                scale: 4
            );

            DownArrow = new ClickableTextureComponent(
                bounds: new Rectangle(
                    x: __instance.xPositionOnScreen + __instance.width + 16,
                    y: __instance.yPositionOnScreen + __instance.height - 64,
                    width: 44,
                    height: 48),
                texture: Game1.mouseCursors,
                sourceRect: new Rectangle(421, 472, 11, 12),
                scale: 4
            );

            ScrollBar = new ClickableTextureComponent(
                bounds: new Rectangle(
                    x: UpArrow.bounds.X + 12,
                    y: UpArrow.bounds.Y + UpArrow.bounds.Height + 4,
                    width: 24,
                    height: 40),
                texture: Game1.mouseCursors,
                sourceRect: new Rectangle(435, 463, 6, 2),
                scale: 4
            );

            ScrollBarHandle = new Rectangle(
                x: ScrollBar.bounds.X,
                y: UpArrow.bounds.Y + UpArrow.bounds.Height + 4,
                width: ScrollBar.bounds.Width,
                height: __instance.height - 64 - UpArrow.bounds.Height - 28
            );

            // ok button
            __instance.okButton = new ClickableTextureComponent(
                bounds: new Rectangle(
                    x: __instance.xPositionOnScreen + __instance.width + 4,
                    y: __instance.yPositionOnScreen + __instance.height - 64 - IClickableMenu.borderWidth,
                    width: 64,
                    height: 64),
                texture: Game1.mouseCursors,
                sourceRect: Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47),
                scale: 1
            );

            // name input text box
            var textBox = new TextBox(
                textBoxTexture: null,
                caretTexture: null,
                font: Game1.dialogueFont,
                textColor: Game1.textColor
            );
            textBox.X = Game1.viewport.Width / 2 - 192;
            textBox.Y = Game1.viewport.Height / 2;
            textBox.Width = 256;
            textBox.Height = 192;

            typeof(PurchaseAnimalsMenu).GetField("textBox", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, textBox);
            typeof(PurchaseAnimalsMenu).GetField("e", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, new TextBoxEvent(__instance.textBoxEnter));

            __instance.textBoxCC = new ClickableComponent(
                bounds: new Rectangle(
                    x: textBox.X,
                    y: textBox.Y,
                    width: 192,
                    height: 48),
                name: ""
            );
            __instance.textBoxCC.myID = 104;
            __instance.textBoxCC.rightNeighborID = 102;
            __instance.textBoxCC.downNeighborID = 101;

            // random button
            __instance.randomButton = new ClickableTextureComponent(
                bounds: new Rectangle(
                    x: textBox.X + textBox.Width + 64 + 48 - 8,
                    y: Game1.viewport.Height / 2 + 4,
                    width: 64,
                    height: 64),
                texture: Game1.mouseCursors,
                sourceRect: new Rectangle(
                    x: 381,
                    y: 361,
                    width: 10,
                    height: 10),
                scale: 4
            );
            __instance.randomButton.myID = 103;
            __instance.randomButton.leftNeighborID = 102;
            __instance.randomButton.downNeighborID = 101;
            __instance.randomButton.rightNeighborID = 101;

            // done name button
            __instance.doneNamingButton = new ClickableTextureComponent(
                bounds: new Rectangle(
                    x: textBox.X + textBox.Width + 32 + 4,
                    y: Game1.viewport.Height / 2 - 8,
                    width: 64,
                    height: 64),
                texture: Game1.mouseCursors,
                sourceRect: Game1.getSourceRectForStandardTileSheet(
                    tileSheet: Game1.mouseCursors,
                    tilePosition: 46),
                scale: 1
            );
            __instance.doneNamingButton.myID = 102;
            __instance.doneNamingButton.rightNeighborID = 103;
            __instance.doneNamingButton.leftNeighborID = 104;
            __instance.doneNamingButton.downNeighborID = 101;

            if (Game1.options.SnappyMenus)
            {
                __instance.populateClickableComponentList();
                __instance.snapToDefaultClickableComponent();
            }

            // exit button
            __instance.upperRightCloseButton = new ClickableTextureComponent(
                bounds: new Rectangle(
                    x: __instance.xPositionOnScreen + __instance.width - 36,
                    y: __instance.yPositionOnScreen + 56,
                    width: 48,
                    height: 48),
                texture: Game1.mouseCursors,
                sourceRect: new Rectangle(337, 494, 12, 12),
                scale: 4
            );

            return false;
        }

        /// <summary>The post fix for the GetAnimalDescription method.</summary>
        /// <param name="name">The name of the animal to get the description for.</param>
        /// <param name="__result">The return value of the method.</param>
        internal static void GetAnimalDescriptionPostFix(string name, ref string __result)
        {
            if (__result != "")
                return;

            var animal = ModEntry.Instance.Api.GetAnimalByName(name);
            if (animal == null)
                return;

            __result = animal.Data.AnimalShopInfo.Description;
        }

        /// <summary>The prefix for the ReceiveScrollWheelAction method.</summary>
        /// <param name="direction">The direction being scrolling in.</param>
        /// <returns>True meaning the original method will get ran.</returns>
        internal static bool ReceiveScrollWheelActionPrefix(int direction, IClickableMenu __instance)
        {
            if (!(__instance is PurchaseAnimalsMenu menu))
                return true;

            if (direction > 0)
            {
                PressUpArrow(menu);
                Game1.playSound("shiny4");
            }
            else if (direction < 0)
            {
                PressDownArrow(menu);
                Game1.playSound("shiny4");
            }

            return true;
        }

        /// <summary>The prefix for the PerformHoverAction method.</summary>
        /// <param name="x">The X position of the cursor.</param>
        /// <param name="y">The Y position of the cursor.</param>
        /// <param name="__instance">The <see cref="PurchaseAnimalsMenu"/> instance being patched.</param>
        /// <returns>False meaning the original method won't get ran.</returns>
        internal static bool PerformHoverActionPrefix(int x, int y, PurchaseAnimalsMenu __instance)
        {
            // get private members
            var freeze = (bool)typeof(PurchaseAnimalsMenu).GetField("freeze", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var onFarm = (bool)typeof(PurchaseAnimalsMenu).GetField("onFarm", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var namingAnimal = (bool)typeof(PurchaseAnimalsMenu).GetField("namingAnimal", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var animalBeingPurchased = (FarmAnimal)typeof(PurchaseAnimalsMenu).GetField("animalBeingPurchased", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

            __instance.hovered = (ClickableTextureComponent)null;
            if (Game1.globalFade || freeze)
                return false;

            if (__instance.okButton != null)
            {
                if (__instance.okButton.containsPoint(x, y))
                    __instance.okButton.scale = Math.Min(1.1f, __instance.okButton.scale + 0.05f);
                else
                    __instance.okButton.scale = Math.Max(1f, __instance.okButton.scale - 0.05f);
            }

            if (onFarm) // player is picking a house for the animal or naming the animal
            {
                if (!namingAnimal) // picking a house for the animal
                {
                    Vector2 tile = new Vector2(
                        (x + Game1.viewport.X) / 64,
                        (y + Game1.viewport.Y) / 64
                    );

                    // get farm location to highlight buildings
                    Farm farm = Game1.getLocationFromName("Farm") as Farm;

                    // color all buildings white
                    foreach (Building building in farm.buildings)
                        building.color.Value = Color.White;

                    // if the player is hovering over a building highlight it in red or green depending if it's a valid house
                    Building buildingAt = farm.getBuildingAt(tile);
                    if (buildingAt != null)
                    {
                        var highLightColor = Color.Red * .8f; ;

                        // get animal data
                        var animal = ModEntry.Instance.Api.GetAnimalBySubTypeName(animalBeingPurchased.type);
                        if (animal != null)
                        {
                            foreach (var building in animal.Data.Buildings)
                            {
                                if (buildingAt.buildingType.Value.ToLower() == building.ToLower() && !(buildingAt.indoors.Value as AnimalHouse).isFull())
                                    highLightColor = Color.LightGreen * .8f;
                            }
                        }

                        buildingAt.color.Value = highLightColor;
                    }
                }

                if (__instance.doneNamingButton != null)
                {
                    if (__instance.doneNamingButton.containsPoint(x, y))
                        __instance.doneNamingButton.scale = Math.Min(1.1f, __instance.doneNamingButton.scale + 0.05f);
                    else
                        __instance.doneNamingButton.scale = Math.Max(1f, __instance.doneNamingButton.scale - 0.05f);
                }

                __instance.randomButton.tryHover(x, y, 0.5f);
            }
            else
            {
                // increase scale of currently hovered scroll bar arrow
                if (UpArrow.containsPoint(x, y))
                    UpArrow.scale = Math.Min(4.4f, UpArrow.scale + 0.05f);
                else
                    UpArrow.scale = Math.Max(4, UpArrow.scale - 0.05f);

                if (DownArrow.containsPoint(x, y))
                    DownArrow.scale = Math.Min(4.4f, DownArrow.scale + 0.05f);
                else
                    DownArrow.scale = Math.Max(4, DownArrow.scale - 0.05f);

                // increase scale of currently hovered animal component
                var initialComponentIndex = CurrentRowIndex * NumberOfIconsPerRow;
                for (int i = initialComponentIndex; i < initialComponentIndex + NumberOfVisibleRows * NumberOfIconsPerRow; i++)
                {
                    // don't try to draw a component that doesn't exist (if the row isn't complete)
                    if (i == __instance.animalsToPurchase.Count)
                        break;

                    var animalComponent = __instance.animalsToPurchase[i];
                    if (animalComponent.containsPoint(x, y))
                    {
                        animalComponent.scale = Math.Min(animalComponent.scale + 0.05f, 4.1f);
                        __instance.hovered = animalComponent;
                    }
                    else
                        animalComponent.scale = Math.Max(4f, animalComponent.scale - 0.025f);
                }
            }

            // exit menu button
            __instance.upperRightCloseButton.tryHover(x, y);

            return false;
        }

        /// <summary>The prefix for the ReceiveKeyPress method.</summary>
        /// <param name="key">The key that has been pressed.</param>
        /// <returns>True meaning the original method will get ran.</returns>
        internal static bool ReceiveKeyPressPrefix(Keys key)
        {
            if (key != Keys.Left && key != Keys.Right ||
                (key == Keys.Left && Game1.oldKBState.IsKeyDown(Keys.Left)) ||
                (key == Keys.Right && Game1.oldKBState.IsKeyDown(Keys.Right))) // ensure either the left or right arrow has been pressed and that it's not being held
                return true;

            // ensure there are valid buildings
            if (ValidBuildings == null || ValidBuildings.Count == 0)
                return true;

            // get the new CurrentValidBuildingIndex
            if (key == Keys.Left)
            {
                // go to preveious valid buuilding in list
                if (CurrentValidBuildingIndex - 1 < 0) // if we are at the beginnning of the list, loop back to the end
                    CurrentValidBuildingIndex = ValidBuildings.Count - 1;
                else
                    CurrentValidBuildingIndex--;
            }
            else if (key == Keys.Right)
            {
                // go to next valid building in list
                if (CurrentValidBuildingIndex + 1 >= ValidBuildings.Count)
                    CurrentValidBuildingIndex = 0;
                else
                    CurrentValidBuildingIndex++;
            }

            // pan the screen to the new building
            PanCameraToBuilding(ValidBuildings[CurrentValidBuildingIndex]);
            return true;
        }

        /// <summary>The prefix for the GamePadButtonHeld method.</summary>
        /// <param name="b">The button being held.</param>
        /// <param name="__instance">The current <see cref="IClickableMenu"/> being patched.</param>
        /// <returns>True if the original method should get ran (the patch isn't being done of the PurchaseAnimalsMenu); otherwise, false.</returns>
        /// <remarks>Patch is done with <see cref="IClickableMenu"/> because <see cref="PurchaseAnimalsMenu"/> doesn't override the method.</remarks>
        internal static bool GamePadButtonHeldPrefix(Buttons b, IClickableMenu __instance)
        {
            if (!(__instance is PurchaseAnimalsMenu))
                return true;

            __instance.receiveGamePadButton(b);
            return false;
        }

        /// <summary>The prefix for the ReceiveGamePadButton method.</summary>
        /// <param name="b">The button that has been pressed.</param>
        /// <returns>False meaning the original method won't get ran.</returns>
        internal static bool ReceiveGamePadButtonPrefix(Buttons b, PurchaseAnimalsMenu __instance)
        {
            var onFarm = (bool)typeof(PurchaseAnimalsMenu).GetField("onFarm", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

            // handle selecting up and down scroll bar arrows with controller
            if (!onFarm)
            {
                // snapp cursor to default item if non is currently hovered
                if (__instance.hovered == null && __instance.currentlySnappedComponent == null)
                {
                    // press the up arrow enough to ensure the components get moved to the top
                    for (int i = 0; i < NumberOfTotalRows; i++)
                        PressUpArrow(__instance);

                    __instance.snapToDefaultClickableComponent();
                }

                // calculte the current index of the currently selected item
                var currentIndex = __instance.animalsToPurchase.IndexOf(__instance.hovered);

                // handle moving onto the arrow buttons from the animal components
                if (b == Buttons.DPadRight || b == Buttons.LeftThumbstickRight)
                {
                    // handle moving onto the up arrow button
                    if ((currentIndex + 1) % NumberOfIconsPerRow == 0)
                        __instance.currentlySnappedComponent = UpArrow;
                }

                // handle moving off of the up arrow button
                if (__instance.currentlySnappedComponent == UpArrow)
                {
                    // handle moving back onto the animal components
                    if (b == Buttons.DPadLeft || b == Buttons.LeftThumbstickLeft)
                        __instance.currentlySnappedComponent = __instance.animalsToPurchase[Math.Min(__instance.animalsToPurchase.Count - 1, (CurrentRowIndex * NumberOfIconsPerRow) + (NumberOfIconsPerRow - 1))];

                    // handle moving down to the down arrow button
                    else if (b == Buttons.DPadDown || b == Buttons.LeftThumbstickDown)
                        __instance.currentlySnappedComponent = DownArrow;
                }

                // handle moving off of the down arrow button
                else if (__instance.currentlySnappedComponent == DownArrow)
                {
                    // handle moving back onto the animal components
                    if (b == Buttons.DPadLeft || b == Buttons.LeftThumbstickLeft)
                        __instance.currentlySnappedComponent = __instance.animalsToPurchase[Math.Min(__instance.animalsToPurchase.Count - 1, (CurrentRowIndex + NumberOfVisibleRows) * NumberOfIconsPerRow - 1)];

                    // handle moving up to the up arrow
                    else if (b == Buttons.DPadUp || b == Buttons.LeftThumbstickUp)
                        __instance.currentlySnappedComponent = UpArrow;
                }

                // handle scrolling up if they are at the top and going up
                if (b == Buttons.DPadUp || b == Buttons.LeftThumbstickUp)
                {
                    // get the row of the selected component
                    var selectedItemRow = (int)(currentIndex / NumberOfIconsPerRow);

                    // ensure the selected row is the top visible row
                    if (CurrentRowIndex == selectedItemRow)
                        PressUpArrow(__instance);
                }

                // handle scrolling down if they are at the button and going down
                if (b == Buttons.DPadDown || b == Buttons.LeftThumbstickDown)
                {
                    // get the row of the selected component
                    var selectedItemRow = (int)(currentIndex / NumberOfIconsPerRow) + 1;

                    // ensure the selected row is the bottom visible row
                    if (CurrentRowIndex + NumberOfVisibleRows == selectedItemRow)
                        PressDownArrow(__instance);
                }
            }

            // handle building panning
            if (onFarm && (b == Buttons.LeftShoulder || b == Buttons.RightShoulder))
            {
                // ensure there are valid buildings
                if (ValidBuildings == null || ValidBuildings.Count == 0)
                    return true;

                // get the new CurrentValidBuildingIndex
                if (b == Buttons.LeftShoulder)
                {
                    // go to preveious valid buuilding in list
                    if (CurrentValidBuildingIndex - 1 < 0) // if we are at the beginnning of the list, loop back to the end
                        CurrentValidBuildingIndex = ValidBuildings.Count - 1;
                    else
                        CurrentValidBuildingIndex--;
                }
                else if (b == Buttons.RightShoulder)
                {
                    // go to next valid building in list
                    if (CurrentValidBuildingIndex + 1 >= ValidBuildings.Count)
                        CurrentValidBuildingIndex = 0;
                    else
                        CurrentValidBuildingIndex++;
                }

                // pan the screen to the new building
                PanCameraToBuilding(ValidBuildings[CurrentValidBuildingIndex]);
            }

            return false;
        }

        /// <summary>The prefix for the ReceiveLieftClick method.</summary>
        /// <param name="x">The X position of the cursor.</param>
        /// <param name="y">The Y position of the cursor.</param>
        /// <param name="__instance">The <see cref="PurchaseAnimalsMenu"/> instance being patched.</param>
        /// <returns>False meaning the original method won't get ran.</returns>
        internal static bool ReceiveLeftClickPrefix(int x, int y, PurchaseAnimalsMenu __instance)
        {
            // get private members
            var freeze = (bool)typeof(PurchaseAnimalsMenu).GetField("freeze", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var onFarm = (bool)typeof(PurchaseAnimalsMenu).GetField("onFarm", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var namingAnimal = (bool)typeof(PurchaseAnimalsMenu).GetField("namingAnimal", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var animalBeingPurchased = (FarmAnimal)typeof(PurchaseAnimalsMenu).GetField("animalBeingPurchased", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var e = (TextBoxEvent)typeof(PurchaseAnimalsMenu).GetField("e", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var textBox = (TextBox)typeof(PurchaseAnimalsMenu).GetField("textBox", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var newAnimalHome = (Building)typeof(PurchaseAnimalsMenu).GetField("newAnimalHome", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var priceOfAnimal = (int)typeof(PurchaseAnimalsMenu).GetField("priceOfAnimal", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var multiplayer = (Multiplayer)typeof(Game1).GetField("multiplayer", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

            if (Game1.globalFade || freeze)
                return false;

            // exit menu button
            if (__instance.upperRightCloseButton.containsPoint(x, y))
            {
                if (onFarm)
                {
                    Game1.playSound("smallSelect");
                    __instance.setUpForReturnToShopMenu();
                }
                else
                {
                    Game1.playSound("bigDeSelect");
                    __instance.exitThisMenu(true);
                }
            }

            if (onFarm) // player is picking a house for the animal or naming the animal
            {
                Building buildingAt = (Game1.getLocationFromName("Farm") as Farm).getBuildingAt(new Vector2(
                    (x + Game1.viewport.X) / 64,
                    (y + Game1.viewport.Y) / 64
                ));

                if (buildingAt != null && !namingAnimal) // picking a house for the animal and a building has been clicked
                {
                    var isBuildingValid = false;

                    // get animal data
                    var animal = ModEntry.Instance.Api.GetAnimalBySubTypeName(animalBeingPurchased.type);
                    if (animal != null)
                    {
                        foreach (var building in animal.Data.Buildings)
                        {
                            if (buildingAt.buildingType.Value.ToLower() == building.ToLower())
                                isBuildingValid = true;
                        }
                    }

                    if (isBuildingValid)
                    {
                        // ensure building has space for animal
                        if ((buildingAt.indoors.Value as AnimalHouse).isFull())
                        {
                            // show 'That Building Is Full' message
                            Game1.showRedMessage(Game1.content.LoadString(Path.Combine("Strings", "StringsFromCSFiles:PurchaseAnimalsMenu.cs.11321")));
                        }
                        else if (animalBeingPurchased.harvestType.Value != 2) // animal 'lays' items
                        {
                            namingAnimal = true;
                            newAnimalHome = buildingAt;

                            if (animalBeingPurchased.sound.Value != null && Game1.soundBank != null)
                            {
                                ICue cue = Game1.soundBank.GetCue(animalBeingPurchased.sound.Value);
                                cue.SetVariable("Pitch", 1200 + Game1.random.Next(-200, 201));
                                cue.Play();
                            }

                            textBox.OnEnterPressed += e;
                            textBox.Text = animalBeingPurchased.displayName;
                            Game1.keyboardDispatcher.Subscriber = textBox;

                            if (Game1.options.SnappyMenus)
                            {
                                __instance.currentlySnappedComponent = __instance.getComponentWithID(104);
                                __instance.snapCursorToCurrentSnappedComponent();
                            }
                        }
                        else if (Game1.player.Money >= priceOfAnimal)
                        {
                            newAnimalHome = buildingAt;
                            animalBeingPurchased.home = newAnimalHome;
                            animalBeingPurchased.homeLocation.Value = new Vector2(newAnimalHome.tileX, newAnimalHome.tileY);
                            animalBeingPurchased.setRandomPosition(animalBeingPurchased.home.indoors);
                            (newAnimalHome.indoors.Value as AnimalHouse).animals.Add(animalBeingPurchased.myID, animalBeingPurchased);
                            (newAnimalHome.indoors.Value as AnimalHouse).animalsThatLiveHere.Add(animalBeingPurchased.myID);
                            newAnimalHome = null;
                            namingAnimal = false;

                            if (animalBeingPurchased.sound.Value != null && Game1.soundBank != null)
                            {
                                ICue cue = Game1.soundBank.GetCue(animalBeingPurchased.sound.Value);
                                cue.SetVariable("Pitch", 1200 + Game1.random.Next(-200, 201));
                                cue.Play();
                            }

                            Game1.player.Money -= priceOfAnimal;
                            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString(Path.Combine("Strings", "StringsFromCSFiles:PurchaseAnimalsMenu.cs.11324"), animalBeingPurchased.type), Color.LimeGreen, 3500f));
                            animalBeingPurchased = new FarmAnimal(animalBeingPurchased.type, multiplayer.getNewID(), Game1.player.uniqueMultiplayerID);
                        }
                        else if (Game1.player.Money < priceOfAnimal)
                            Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
                    }
                    else
                    {
                        // show '{0}s Can't Live There.' 
                        Game1.showRedMessage(Game1.content.LoadString(Path.Combine("Strings", "StringsFromCSFiles:PurchaseAnimalsMenu.cs.11326"), animalBeingPurchased.type));
                    }
                }
                if (!namingAnimal)
                    return false;
                if (__instance.doneNamingButton.containsPoint(x, y))
                {
                    __instance.textBoxEnter(textBox);
                    Game1.playSound("smallSelect");
                }
                else if (namingAnimal && __instance.randomButton.containsPoint(x, y))
                {
                    animalBeingPurchased.Name = Dialogue.randomName();
                    animalBeingPurchased.displayName = animalBeingPurchased.Name;
                    textBox.Text = animalBeingPurchased.displayName;
                    __instance.randomButton.scale = __instance.randomButton.baseScale;
                    Game1.playSound("drumkit6");
                }
                textBox.Update();
            }
            else
            {
                // checked if scroll bar buttons were clicked
                if (UpArrow.containsPoint(x, y))
                {
                    PressUpArrow(__instance);
                    Game1.playSound("shwip");
                }

                if (DownArrow.containsPoint(x, y))
                {
                    PressDownArrow(__instance);
                    Game1.playSound("shwip");
                }

                // check if an animal component was clicked
                foreach (ClickableTextureComponent textureComponent in __instance.animalsToPurchase)
                {
                    if (textureComponent.containsPoint(x, y) && (textureComponent.item as StardewValley.Object).Type == null)
                    {
                        int num = textureComponent.item.salePrice();
                        if (Game1.player.Money >= num)
                        {
                            Game1.globalFadeToBlack(new Game1.afterFadeFunction(__instance.setUpForAnimalPlacement), 0.02f);
                            Game1.playSound("smallSelect");
                            onFarm = true;
                            animalBeingPurchased = new FarmAnimal(textureComponent.hoverText, multiplayer.getNewID(), Game1.player.UniqueMultiplayerID);
                            priceOfAnimal = num;

                            // calculate all the valid builds for the camera panning feature
                            ValidBuildings = new List<Building>();
                            foreach (var building in Game1.getFarm().buildings)
                            {
                                // ensure the animal can live in the building
                                var buildingValid = false;
                                // get animal data
                                var animal = ModEntry.Instance.Api.GetAnimalBySubTypeName(animalBeingPurchased.type);
                                if (animal != null)
                                {
                                    foreach (var animalBuilding in animal.Data.Buildings)
                                    {
                                        if (building.buildingType.Value.ToLower() == animalBuilding.ToLower())
                                            buildingValid = true;
                                    }

                                    break;
                                }

                                // ensure the animal can live in the building
                                if (!buildingValid)
                                    continue;

                                // ensure the building isn't full
                                var animalHouse = building.indoors.Value as AnimalHouse;
                                if (animalHouse == null || animalHouse.isFull())
                                    continue;

                                ValidBuildings.Add(building);
                            }
                        }
                        else
                            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString(Path.Combine("Strings", "StringsFromCSFiles:PurchaseAnimalsMenu.cs.11325")), Color.Red, 3500f));
                    }
                }
            }

            // reset potentially changed private members
            typeof(PurchaseAnimalsMenu).GetField("namingAnimal", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, namingAnimal);
            typeof(PurchaseAnimalsMenu).GetField("newAnimalHome", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, newAnimalHome);
            typeof(PurchaseAnimalsMenu).GetField("textBox", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, textBox);
            typeof(PurchaseAnimalsMenu).GetField("animalBeingPurchased", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, animalBeingPurchased);
            typeof(PurchaseAnimalsMenu).GetField("onFarm", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, onFarm);
            typeof(PurchaseAnimalsMenu).GetField("priceOfAnimal", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, priceOfAnimal);

            return false;
        }

        /// <summary>The prefix for the SetUpForAnimalPlacement method.</summary>
        /// <param name="__instance">The <see cref="PurchaseAnimalsMenu"/> instance being patched.</param>
        /// <returns>True meaning the original method will get ran.</returns>
        internal static bool SetUpForAnimalPlacementPrefix(PurchaseAnimalsMenu __instance)
        {
            __instance.upperRightCloseButton.bounds.X = Game1.viewport.Width - 128;
            __instance.upperRightCloseButton.bounds.Y = 64;

            return true;
        }

        /// <summary>The prefix for the SetUpForReturnToShopMenu method.</summary>
        /// <param name="__instance">The <see cref="PurchaseAnimalsMenu"/> instance being patched.</param>
        /// <returns>True meaning the original method will get ran.</returns>
        internal static bool SetUpForReturnToShopMenuPrefix(PurchaseAnimalsMenu __instance)
        {
            __instance.upperRightCloseButton.bounds.X = __instance.xPositionOnScreen + __instance.width - 36;
            __instance.upperRightCloseButton.bounds.Y = __instance.yPositionOnScreen + 56;

            return true;
        }

        /// <summary>The prefix for the Draw method.</summary>
        /// <param name="b">The spritebatch to draw the menu to.</param>
        /// <param name="__instance">The <see cref="PurchaseAnimalsMenu"/> instance being patched.</param>
        /// <returns>False meaning the original method won't get ran.</returns>
        internal static bool DrawPrefix(SpriteBatch b, PurchaseAnimalsMenu __instance)
        {
            // get required private members
            bool onFarm = (bool)typeof(PurchaseAnimalsMenu).GetField("onFarm", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            bool namingAnimal = (bool)typeof(PurchaseAnimalsMenu).GetField("namingAnimal", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            FarmAnimal animalBeingPurchased = (FarmAnimal)typeof(PurchaseAnimalsMenu).GetField("animalBeingPurchased", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            TextBox textBox = (TextBox)typeof(PurchaseAnimalsMenu).GetField("textBox", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

            // draw menu to pick which animal to buy
            if (!onFarm && !Game1.dialogueUp && !Game1.globalFade)
            {
                // dark background
                b.Draw(
                    texture: Game1.fadeToBlackRect,
                    destinationRectangle: Game1.graphics.GraphicsDevice.Viewport.Bounds,
                    color: Color.Black * 0.75f
                );

                // menu background
                Game1.drawDialogueBox(
                    x: __instance.xPositionOnScreen,
                    y: __instance.yPositionOnScreen,
                    width: __instance.width,
                    height: __instance.height,
                    speaker: false,
                    drawOnlyBox: true
                );

                // redraw the money box over the dark overlay
                Game1.dayTimeMoneyBox.drawMoneyBox(b, -1, -1);

                // animal icons
                var initialComponentIndex = CurrentRowIndex * NumberOfIconsPerRow;
                for (int i = initialComponentIndex; i < initialComponentIndex + NumberOfVisibleRows * NumberOfIconsPerRow; i++)
                {
                    // don't try to draw a component that doesn't exist (if the row isn't complete)
                    if (i == __instance.animalsToPurchase.Count)
                        break;

                    var animalComponent = __instance.animalsToPurchase[i];
                    animalComponent.draw(b, (animalComponent.item as StardewValley.Object).Type != null ? Color.Black * 0.4f : Color.White, 0.87f, 0);
                }

                // ensure scroll bar is actually needed before drawing it
                if (NumberOfTotalRows > NumberOfVisibleRows)
                {
                    UpArrow.draw(b);
                    DownArrow.draw(b);
                    ScrollBar.draw(b);
                    IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), ScrollBarHandle.X, ScrollBarHandle.Y, ScrollBarHandle.Width, ScrollBarHandle.Height, Color.White, 4);
                }
            }
            else if (!Game1.globalFade && onFarm) // the player is currently picking a house for the animal
            {
                // construct housing string
                var animal = ModEntry.Instance.Api.GetAnimalBySubTypeName(animalBeingPurchased.type);
                var buildingsString = "";

                for (var i = 0; i < animal.Data.Buildings.Count; i++)
                {
                    var building = animal.Data.Buildings[i];
                    buildingsString += building;
                    if (i != animal.Data.Buildings.Count - 1)
                        buildingsString += ", ";
                }

                var housingAnimalString = $"Choose a: {buildingsString} for your new {animalBeingPurchased.type}";

                // draw housing animal string;
                SpriteText.drawStringWithScrollBackground(
                    b: b,
                    s: housingAnimalString,
                    x: Game1.viewport.Width / 2 - SpriteText.getWidthOfString(housingAnimalString, 999999) / 2,
                    y: 16
                );

                // draw naming text box
                if (namingAnimal)
                {
                    // dark background
                    b.Draw(
                        texture: Game1.fadeToBlackRect,
                        destinationRectangle: Game1.graphics.GraphicsDevice.Viewport.Bounds,
                        color: Color.Black * 0.75f
                    );

                    // background
                    Game1.drawDialogueBox(
                        x: Game1.viewport.Width / 2 - 256,
                        y: Game1.viewport.Height / 2 - 192 - 32,
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
                        position: new Vector2((float)(Game1.viewport.Width / 2 - 256 + 32 + 8), (float)(Game1.viewport.Height / 2 - 128 + 8)),
                        color: Game1.textColor
                    );

                    // draw textbox
                    textBox.Draw(b, true);

                    // draw buttons
                    __instance.doneNamingButton.draw(b);
                    __instance.randomButton.draw(b);
                }
            }

            // check if hovered animal is buyable (to show hover text)
            if (__instance.hovered != null && (__instance.hovered.item as SObject).Type != null)
            {
                // display not available for purchase message
                IClickableMenu.drawHoverText(
                    b: b,
                    text: Game1.parseText((__instance.hovered.item as StardewValley.Object).Type, Game1.dialogueFont, 320),
                    font: Game1.dialogueFont
                );
            }

            // draw left info panel
            DrawInfoPanel(b, __instance);

            __instance.upperRightCloseButton.draw(b);
            __instance.drawMouse(b);

            return false;
        }


        /*********
        ** Private Methods
        *********/
        /// <summary>Pan the camera to the passed building.</summary>
        /// <param name="building">The building to pan the camera to.</param>
        private static void PanCameraToBuilding(Building building)
        {
            var panAmount = new Point(
                x: building.tileX * 64 - Game1.viewport.X - Game1.viewport.Width / 2 + building.tilesWide * 64 / 2,
                y: building.tileY * 64 - Game1.viewport.Y - Game1.viewport.Height / 2 + building.tilesHigh * 64 / 2
            ); // *64 is the tile width (16) * the game scale (4)

            Game1.panScreen(panAmount.X, panAmount.Y);
        }

        /// <summary>Performs the <see cref="UpArrow"/> click event.</summary>
        /// <param name="__instance">The <see cref="PurchaseAnimalsMenu"/> instance being patched.</param>
        private static void PressUpArrow(PurchaseAnimalsMenu __instance)
        {
            if (CurrentRowIndex < NumberOfTotalRows - NumberOfVisibleRows)
                return;

            // alter positions of each animal component
            foreach (var animalComponent in __instance.animalsToPurchase)
                animalComponent.bounds.Y += 85;

            CurrentRowIndex--;
        }

        /// <summary>Performs the <see cref="DownArrow"/> click event.</summary>
        /// <param name="__instance">The <see cref="PurchaseAnimalsMenu"/> instance being patched.</param>
        private static void PressDownArrow(PurchaseAnimalsMenu __instance)
        {
            if (CurrentRowIndex > 0)
                return;

            // alter positions of each animal component
            foreach (var animalComponent in __instance.animalsToPurchase)
                animalComponent.bounds.Y -= 85;

            CurrentRowIndex++;
        }

        /// <summary>Gets A list of <see cref="StardewValley.Object"/> of objects an animal can drop.</summary>
        /// <returns>A list of <see cref="StardewValley.Object"/> of objects an animal can drop.</returns>
        private static List<SObject> GetAllAnimalProducts(Animal animal)
        {
            var products = new List<SObject>();

            // each sub type
            foreach (var subType in animal.Data.Types)
            {
                // each produce season
                foreach (var produceSeasonInfo in subType.Produce.GetType().GetProperties())
                {
                    var produceSeason = (AnimalProduceSeason)produceSeasonInfo.GetValue(subType.Produce);

                    if (produceSeason?.Products != null)
                        foreach (var product in produceSeason.Products)
                            if (!products.Where(p => p.ParentSheetIndex == Convert.ToInt32(product.Id)).Any())
                                products.Add(new SObject(Convert.ToInt32(product.Id), 1));

                    if (produceSeason?.DeluxeProducts != null)
                        foreach (var product in produceSeason.DeluxeProducts)
                            if (!products.Where(p => p.ParentSheetIndex == Convert.ToInt32(product.Id)).Any())
                                products.Add(new SObject(Convert.ToInt32(product.Id), 1));
                }
            }

            return products;
        }

        /// <summary>Draws the info panel.</summary>
        /// <param name="spriteBatch">The <see cref="SpriteBatch"/> to draw the info panel to.</param>
        /// <param name="__instance">The <see cref="PurchaseAnimalsMenu"/> instance being patched.</param>
        private static void DrawInfoPanel(SpriteBatch spriteBatch, PurchaseAnimalsMenu __instance)
        {
            if (__instance.hovered == null || (__instance.hovered.item as SObject).Type != null) // draw Buildings info panel
            {
                var buildings = new List<Building>();
                foreach (var building in Game1.getFarm().buildings)
                    if (building.indoors.Value is AnimalHouse)
                        buildings.Add(building);

                // determine info manu height
                var buildingsStringHeight = buildings.Count * 45;

                var infoPanelHeight = 210 + buildingsStringHeight;

                // info panel background position
                var infoPanelPosition = new Rectangle(
                    x: __instance.xPositionOnScreen - 430,
                    y: (int)((Game1.graphics.GraphicsDevice.Viewport.Height / 2) - (infoPanelHeight / 2) - 32),
                    width: 450,
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

                // "Buildings" label
                SpriteText.drawString(
                    b: spriteBatch,
                    s: "Buildings",
                    x: infoPanelPosition.X + 65,
                    y: infoPanelPosition.Y + 115
                );

                // draw each building
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
                    spriteBatch.DrawString(
                        spriteFont: Game1.dialogueFont,
                        text: $"{indoors.Animals.Keys.Count()}/{indoors.animalLimit.Value}",
                        position: new Vector2(infoPanelPosition.X + 305, infoPanelPosition.Y + 165 + i * 45),
                        color: Color.Black
                    );
                }
            }
            else // Draw animal info panel
            {
                var animalData = ModEntry.Instance.Api.GetAnimalByName(__instance.hovered.hoverText);

                // determine info menu height
                // building string height
                var buildingsString = string.Join(", ", animalData.Data.Buildings);
                var parsedBuildingsString = Game1.parseText($"Buildings: {buildingsString}", Game1.smallFont, 325);
                var buildingStringsHeight = Game1.smallFont.MeasureString(parsedBuildingsString).Y;

                // description height
                var animalDescription = PurchaseAnimalsMenu.getAnimalDescription(__instance.hovered.hoverText);
                var descriptionString = Game1.parseText($"Description: {animalDescription}", Game1.smallFont, 325);

                // products height
                var products = GetAllAnimalProducts(animalData).Distinct().ToList();
                var productRows = (int)Math.Ceiling(products.Count / 5f);

                // panel height
                var descriptionHeight = Game1.smallFont.MeasureString(descriptionString).Y;
                var productsHeight = productRows * 64;
                var infoPanelHeight = 410 + buildingStringsHeight + descriptionHeight + productsHeight;

                // info panel background position
                var infoPanelPosition = new Rectangle(
                    x: __instance.xPositionOnScreen - 430,
                    y: (int)((Game1.graphics.GraphicsDevice.Viewport.Height / 2) - (infoPanelHeight / 2) - 32),
                    width: 450,
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
                    s: __instance.hovered.hoverText,
                    x: infoPanelPosition.X + 65,
                    y: infoPanelPosition.Y + 115
                );

                // draw cost
                SpriteText.drawString(
                    b: spriteBatch,
                    s: "$" + Game1.content.LoadString(Path.Combine("Strings", "StringsFromCSFiles:LoadGameMenu.cs.11020"), __instance.hovered.item.salePrice()),
                    x: infoPanelPosition.X + 65,
                    y: infoPanelPosition.Y + 170
                );

                // number of varients
                spriteBatch.DrawString(
                    spriteFont: Game1.smallFont,
                    text: $"Available in {animalData.Data.Types.Count} {(animalData.Data.Types.Count == 1 ? "variety" : "varieties")}",
                    position: new Vector2(infoPanelPosition.X + 65, infoPanelPosition.Y + 235),
                    color: Color.Black
                );

                // mature age
                spriteBatch.DrawString(
                    spriteFont: Game1.smallFont,
                    text: $"Matures in {animalData?.Data?.DaysTillMature ?? -1} days",
                    position: new Vector2(infoPanelPosition.X + 65, infoPanelPosition.Y + 285),
                    color: Color.Black
                );

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
        }
    }
}
