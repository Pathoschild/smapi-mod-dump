using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FarmAnimalVarietyRedux.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="PurchaseAnimalsMenu"/> class.</summary>
    internal class PurchaseAnimalsMenuPatch
    {
        /*********
        ** Fields
        *********/
        /// <summary>The number of animal clickable components can fit on a row on a 720p -> 1080p resolution viewport</summary>
        private const int NumberOfIconsPerRow1280 = 5;

        /// <summary>The number of animal clickable components can fit on a row on a 1080p+ resolution viewport.</summary>
        private const int NumberOfIconsPerRow1920 = 9;

        /// <summary>The width of an animal clickable component.</summary>
        private const int IconWidth = 32;


        /*********
        ** Internal Methods
        *********/
        /// <summary>The prefix for the constructor.</summary>
        /// <param name="stock">The menu stock.</param>
        /// <param name="__instance">The current <see cref="PurchaseAnimalsMenu"/> instance being patched.</param>
        internal static bool ConstructorPrefix(List<StardewValley.Object> stock, PurchaseAnimalsMenu __instance)
        {
            // determine the number of icons per row to use, this is to fill the screen as best as possible
            int numberOfIconsPerRow;
            if (Game1.graphics.GraphicsDevice.Viewport.Width >= 1920)
                numberOfIconsPerRow = NumberOfIconsPerRow1920;
            else
                numberOfIconsPerRow = NumberOfIconsPerRow1280;

            // calculate menu dimensions
            __instance.width = IconWidth * Math.Min(numberOfIconsPerRow, stock.Count) * 4 + (IClickableMenu.borderWidth * 2); // 4 is sprite scale
            int numberOfIconRows = (int)Math.Ceiling(stock.Count / (decimal)numberOfIconsPerRow);
            __instance.height = numberOfIconRows * 85 + 64 + (IClickableMenu.borderWidth * 2);

            // get the top left position for the background asset
            Vector2 backgroundTopLeftPosition = Utility.getTopLeftPositionForCenteringOnScreen(__instance.width, __instance.height);
            __instance.xPositionOnScreen = (int)backgroundTopLeftPosition.X;
            __instance.yPositionOnScreen = (int)backgroundTopLeftPosition.Y;

            __instance.animalsToPurchase = new List<ClickableTextureComponent>();
            // add a clickable texture for each animal
            for (int i = 0; i < stock.Count; i++)
            {
                Texture2D shopIconTexture = Game1.mouseCursors;
                Rectangle shopIconSourceRectangle = new Rectangle(i % 3 * 16 * 2, 448 + i / 3 * 16, 32, 16);

                // check if it's a custom animal
                foreach (var animal in ModEntry.Animals)
                {
                    if (animal.Name != stock[i].name)
                        continue;

                    shopIconTexture = animal.ShopIcon;
                    shopIconSourceRectangle = new Rectangle(
                        x: 0,
                        y: 0,
                        width: 32,
                        height: 16
                    );

                    break;
                }

                var animalComponent = new ClickableTextureComponent(
                    name: stock[i].salePrice().ToString(),
                    bounds: new Rectangle(
                        x: __instance.xPositionOnScreen + IClickableMenu.borderWidth + i % numberOfIconsPerRow * IconWidth * 4, // 4 is the sprite scale
                        y: __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth / 2 + i / numberOfIconsPerRow * 85,
                        width: IconWidth * 4,
                        height: 16 * 4),
                    label: null,
                    hoverText: stock[i].Name,
                    texture: shopIconTexture,
                    sourceRect: shopIconSourceRectangle,
                    scale: 4,
                    drawShadow: stock[i].Type == null
                );
                animalComponent.item = stock[i];
                animalComponent.myID = i;
                animalComponent.upNeighborID = i - numberOfIconsPerRow;
                animalComponent.leftNeighborID = i % numberOfIconsPerRow == 0 ? -1 : i - 1;
                animalComponent.rightNeighborID = i % numberOfIconsPerRow == numberOfIconsPerRow - 1 ? -1 : i + 1;
                animalComponent.downNeighborID = i + numberOfIconsPerRow;

                __instance.animalsToPurchase.Add(animalComponent);
            }

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

            return false;
        }

        /// <summary>The post fix for the GetAnimalDescription method.</summary>
        /// <param name="name">The name of the animal to get the description for.</param>
        /// <param name="__result">The return value of the method.</param>
        internal static void GetAnimalDescriptionPostFix(string name, ref string __result)
        {
            if (__result != "")
                return;

            var animalData = ModEntry.Animals.Where(animal => animal.Name == name).Select(animal => animal.Data).FirstOrDefault();
            if (animalData == null)
                return;

            __result = animalData.AnimalShopInfo.Description;
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
                    Farm locationFromName = Game1.getLocationFromName("Farm") as Farm;

                    // color all buildings white
                    foreach (Building building in locationFromName.buildings)
                        building.color.Value = Color.White;

                    // if the player is hovering over a building highlight it in red or green depending if it's a valid house
                    Building buildingAt = locationFromName.getBuildingAt(tile);
                    if (buildingAt != null)
                    {
                        var highLightColor = Color.Red * .8f; ;

                        // if 'buildingTypeILiveIn' is used, it's a default game animal
                        if (!string.IsNullOrEmpty(animalBeingPurchased.buildingTypeILiveIn.Value))
                        {
                            if (buildingAt.buildingType.Value.Contains(animalBeingPurchased.buildingTypeILiveIn.Value) && !(buildingAt.indoors.Value as AnimalHouse).isFull())
                                highLightColor = Color.LightGreen * .8f;
                        }
                        else // animal is a custom animal
                        {
                            // get animal data
                            foreach (var animal in ModEntry.Animals)
                            {
                                if (!animal.SubTypes.Where(subType => subType.Name == animalBeingPurchased.type).Any())
                                    continue;

                                foreach (var building in animal.Data.Buildings)
                                {
                                    if (buildingAt.buildingType.Value.ToLower() == building.ToLower() && !(buildingAt.indoors.Value as AnimalHouse).isFull())
                                        highLightColor = Color.LightGreen * .8f;
                                }

                                break;
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
                // increase scale of currently hovered animal component
                foreach (var animalComponent in __instance.animalsToPurchase)
                {
                    if (animalComponent.containsPoint(x, y))
                    {
                        animalComponent.scale = Math.Min(animalComponent.scale + 0.05f, 4.1f);
                        __instance.hovered = animalComponent;
                    }
                    else
                        animalComponent.scale = Math.Max(4f, animalComponent.scale - 0.025f);
                }
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

            if (__instance.okButton != null && __instance.okButton.containsPoint(x, y) && __instance.readyToClose())
            {
                if (onFarm)
                {
                    __instance.setUpForReturnToShopMenu();
                    Game1.playSound("smallSelect");
                }
                else
                {
                    Game1.exitActiveMenu();
                    Game1.playSound("bigDeSelect");
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
                    var buildingValid = false;

                    // determine is building is valid
                    if (!string.IsNullOrEmpty(animalBeingPurchased.buildingTypeILiveIn.Value)) // if 'buildingTypeILiveIn' is used, it's a default game animal
                    {
                        if (buildingAt.buildingType.Value.Contains(animalBeingPurchased.buildingTypeILiveIn.Value))
                            buildingValid = true;
                    }
                    else // animal is a custom animal
                    {
                        // get animal data
                        foreach (var animal in ModEntry.Animals)
                        {
                            if (!animal.SubTypes.Where(subType => subType.Name == animalBeingPurchased.type).Any())
                                continue;

                            foreach (var building in animal.Data.Buildings)
                            {
                                if (buildingAt.buildingType.Value.ToLower() == building.ToLower())
                                    buildingValid = true;
                            }

                            break;
                        }
                    }

                    // That Building Is Full

                    if (buildingValid)
                    {
                        if ((buildingAt.indoors.Value as AnimalHouse).isFull())
                        {
                            // show 'That Building Is Full' message
                            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11321"));
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
                            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11324", animalBeingPurchased.type), Color.LimeGreen, 3500f));
                            animalBeingPurchased = new FarmAnimal(animalBeingPurchased.type, multiplayer.getNewID(), Game1.player.uniqueMultiplayerID);
                        }
                        else if (Game1.player.Money < priceOfAnimal)
                            Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
                    }
                    else
                    {
                        // show '{0}s Can't Live There.' 
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11326", animalBeingPurchased.type));
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
                        }
                        else
                            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11325"), Color.Red, 3500f));
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

                // 'livestock' label
                SpriteText.drawStringWithScrollBackground(
                    b: b,
                    s: "Livestock:",
                    x: __instance.xPositionOnScreen + 96,
                    y: __instance.yPositionOnScreen
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
                foreach (ClickableTextureComponent textureComponent in __instance.animalsToPurchase)
                    textureComponent.draw(b, (textureComponent.item as StardewValley.Object).Type != null ? Color.Black * 0.4f : Color.White, 0.87f, 0);
            }
            else if (!Game1.globalFade && onFarm) // the player is currently picking a house for the animal
            {
                string housingAnimalString = Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11355", animalBeingPurchased.displayHouse, animalBeingPurchased.displayType);

                // if the animal is a custom animal construct a different string to accomodate more than 1 possible building
                foreach (var animal in ModEntry.Animals)
                {
                    if (animal.SubTypes.Where(subType => subType.Name == animalBeingPurchased.type).Any())
                    {
                        var buildingsString = "";

                        for (var i = 0; i < animal.Data.Buildings.Count; i++)
                        {
                            var building = animal.Data.Buildings[i];
                            buildingsString += building;
                            if (i != animal.Data.Buildings.Count - 1)
                                buildingsString += ", ";
                        }

                        housingAnimalString = $"Choose a: {buildingsString} for your new {animalBeingPurchased.type}";
                        break;
                    }
                }

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
                        text: Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11357"),
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

            if (!Game1.globalFade && __instance.okButton != null)
                __instance.okButton.draw(b);

            if (__instance.hovered != null)
            {
                // check if the hovered item is available for purchase
                if ((__instance.hovered.item as StardewValley.Object).Type != null)
                {
                    // display not available for purchase message
                    IClickableMenu.drawHoverText(
                        b,
                        Game1.parseText((__instance.hovered.item as StardewValley.Object).Type, Game1.dialogueFont, 320),
                        Game1.dialogueFont
                    );
                }
                else
                {
                    // draw the animal type label
                    SpriteText.drawStringWithScrollBackground(
                        b: b,
                        s: __instance.hovered.hoverText,
                        x: __instance.xPositionOnScreen + 436,
                        y: __instance.yPositionOnScreen,
                        placeHolderWidthText: "Truffle Pig"
                    );

                    // draw the animal price label
                    SpriteText.drawStringWithScrollBackground(
                        b: b,
                        s: "$" + Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object)__instance.hovered.item.salePrice()),
                        x: __instance.xPositionOnScreen + 796,
                        y: __instance.yPositionOnScreen,
                        placeHolderWidthText: "$99999999g",
                        alpha: Game1.player.Money >= __instance.hovered.item.salePrice() ? 1f : 0.5f
                    );

                    // draw the animal description
                    string animalDescription = PurchaseAnimalsMenu.getAnimalDescription(__instance.hovered.hoverText);
                    IClickableMenu.drawHoverText(
                        b,
                        Game1.parseText(animalDescription, Game1.smallFont, 320),
                        Game1.smallFont
                    );
                }
            }
            Game1.mouseCursorTransparency = 1f;
            __instance.drawMouse(b);

            return false;
        }
    }
}
