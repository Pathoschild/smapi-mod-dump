/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EpicBellyFlop45/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using System;
using System.IO;
using System.Reflection;

namespace FarmAnimalVarietyRedux.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="AnimalQueryMenu"/> class.</summary>
    internal class AnimalQueryMenuPatch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The post fix for the constructor.</summary>
        /// <param name="animal">The animal whose query menu is being patched.</param>
        internal static void ConstructorPostFix(FarmAnimal animal)
        {
            animal.makeSound();
        }

        /// <summary>The prefix for the PerformHoverAction method.</summary>
        /// <param name="x">The X position of the cursor.</param>
        /// <param name="y">The Y position of the cursor.</param>
        /// <param name="__instance">The current instance being patched.</param>
        /// <returns>False meaning the original method won't get ran.</returns>
        internal static bool PerformHoverActionPrefix(int x, int y, AnimalQueryMenu __instance)
        {
            // get private members
            var movingAnimal = (bool)typeof(AnimalQueryMenu).GetField("movingAnimal", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var animal = (FarmAnimal)typeof(AnimalQueryMenu).GetField("animal", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

            var hoverText = "";
            if (movingAnimal) // the player is choosing a new home for the animal
            {
                var tile = new Vector2((x + Game1.viewport.X) / 64, (y + Game1.viewport.Y) / 64);
                var farm = Game1.getLocationFromName("Farm") as Farm;

                // highlight all buildings white
                foreach (Building building in farm.buildings)
                    building.color.Value = Color.White;

                // check the building the player is currently hovering on
                Building hoveredBuilding = farm.getBuildingAt(tile);
                if (hoveredBuilding != null)
                {
                    // set the highlight color of the currently hovered building based on if the animal can live in the building
                    var highLightColor = Color.Red * .8f; ;

                    // get animal data
                    var customAnimal = ModEntry.Instance.Api.GetAnimalBySubTypeName(animal.type);
                    if (customAnimal != null)
                    {
                        foreach (var building in customAnimal.Data.Buildings)
                        {
                            if (hoveredBuilding.buildingType.Value.ToLower() == building.ToLower() && !(hoveredBuilding.indoors.Value as AnimalHouse).isFull())
                                highLightColor = Color.LightGreen * .8f;
                        }
                    }

                    hoveredBuilding.color.Value = highLightColor;
                }
            }

            // change scale ('zoom in') of buttons when player hover on them
            // ok button
            if (__instance.okButton != null)
            {
                if (__instance.okButton.containsPoint(x, y))
                    __instance.okButton.scale = Math.Min(1.1f, __instance.okButton.scale + 0.05f);
                else
                    __instance.okButton.scale = Math.Max(1f, __instance.okButton.scale - 0.05f);
            }

            // sell button
            if (__instance.sellButton != null)
            {
                if (__instance.sellButton.containsPoint(x, y))
                {
                    __instance.sellButton.scale = Math.Min(4.1f, __instance.sellButton.scale + 0.05f);
                    hoverText = Game1.content.LoadString(Path.Combine("Strings", "UI:AnimalQuery_Sell"), animal.getSellPrice());
                }
                else
                    __instance.sellButton.scale = Math.Max(4f, __instance.sellButton.scale - 0.05f);
            }

            // move home button
            if (__instance.moveHomeButton != null)
            {
                if (__instance.moveHomeButton.containsPoint(x, y))
                {
                    __instance.moveHomeButton.scale = Math.Min(4.1f, __instance.moveHomeButton.scale + 0.05f);
                    hoverText = Game1.content.LoadString(Path.Combine("Strings", "UI:AnimalQuery_Move"));
                }
                else
                    __instance.moveHomeButton.scale = Math.Max(4f, __instance.moveHomeButton.scale - 0.05f);
            }

            // allow reproduction button
            if (__instance.allowReproductionButton != null)
            {
                if (__instance.allowReproductionButton.containsPoint(x, y))
                {
                    __instance.allowReproductionButton.scale = Math.Min(4.1f, __instance.allowReproductionButton.scale + 0.05f);
                    hoverText = Game1.content.LoadString(Path.Combine("Strings", "UI:AnimalQuery_AllowReproduction"));
                }
                else
                    __instance.allowReproductionButton.scale = Math.Max(4f, __instance.allowReproductionButton.scale - 0.05f);
            }

            // yes button
            if (__instance.yesButton != null)
            {
                if (__instance.yesButton.containsPoint(x, y))
                    __instance.yesButton.scale = Math.Min(1.1f, __instance.yesButton.scale + 0.05f);
                else
                    __instance.yesButton.scale = Math.Max(1f, __instance.yesButton.scale - 0.05f);
            }

            // no button
            if (__instance.noButton != null)
            {
                if (__instance.noButton.containsPoint(x, y))
                    __instance.noButton.scale = Math.Min(1.1f, __instance.noButton.scale + 0.05f);
                else
                    __instance.noButton.scale = Math.Max(1f, __instance.noButton.scale - 0.05f);
            }

            // set private members
            typeof(AnimalQueryMenu).GetField("hoverText", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, hoverText);
            return false;
        }

        /// <summary>The prefix for the ReceiveLeftClick method.</summary>
        /// <param name="x">The X position of the cursor.</param>
        /// <param name="y">The Y position of the cursor.</param>
        /// <param name="__instance">The current instance being patched.</param>
        /// <returns>False meaning the original method won't get ran.</returns>
        internal static bool ReceiveLeftClickPrefix(int x, int y, AnimalQueryMenu __instance)
        {
            if (Game1.globalFade)
                return false;

            // get private members
            var movingAnimal = (bool)typeof(AnimalQueryMenu).GetField("movingAnimal", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var animal = (FarmAnimal)typeof(AnimalQueryMenu).GetField("animal", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var confirmingSell = (bool)typeof(AnimalQueryMenu).GetField("confirmingSell", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var textBox = (TextBox)typeof(AnimalQueryMenu).GetField("textBox", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var multiplayer = (Multiplayer)typeof(Game1).GetField("multiplayer", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

            if (movingAnimal) // if the player is currently choosing a new home for the animal
            {
                if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                {
                    Game1.globalFadeToBlack(new Game1.afterFadeFunction(__instance.prepareForReturnFromPlacement), 0.02f);
                    Game1.playSound("smallSelect");
                }

                // the building currently under the cursor
                var tile = new Vector2((x + Game1.viewport.X) / 64, (y + Game1.viewport.Y) / 64);
                var farm = Game1.getLocationFromName("Farm") as Farm;
                Building buildingAt = farm.getBuildingAt(tile);
                if (buildingAt == null)
                    return false;

                var isBuildingValid = true;
                var customAnimal = ModEntry.Instance.Api.GetAnimalBySubTypeName(animal.type);
                if (customAnimal != null)
                {
                    // keep track of whether the animal can live in the building to determine whether to display the 'I Can't Live Here' message
                    var canAnimalLiveHere = false;
                    foreach (var building in customAnimal.Data.Buildings)
                    {
                        if (buildingAt.buildingType.Value.ToLower() == building.ToLower())
                        {
                            canAnimalLiveHere = true;
                            if ((buildingAt.indoors.Value as AnimalHouse).isFull())
                            {
                                Game1.showRedMessage(Game1.content.LoadString(Path.Combine("Strings", "UI:AnimalQuery_Moving_BuildingFull")));
                                isBuildingValid = false;
                            }
                            else if (buildingAt.Equals(animal.home))
                            {
                                Game1.showRedMessage(Game1.content.LoadString(Path.Combine("Strings", "UI:AnimalQuery_Moving_AlreadyHome")));
                                isBuildingValid = false;
                            }
                        }
                    }

                    if (!canAnimalLiveHere)
                    {
                        Game1.showRedMessage(Game1.content.LoadString(Path.Combine("Strings", "UI:AnimalQuery_Moving_CantLiveThere"), animal.shortDisplayType()));
                        isBuildingValid = false;
                    }
                }

                // if the building is valid, move the animal to it
                if (isBuildingValid)
                {
                    (animal.home.indoors.Value as AnimalHouse).animalsThatLiveHere.Remove(animal.myID);

                    if ((animal.home.indoors.Value as AnimalHouse).animals.ContainsKey(animal.myID))
                    {
                        (buildingAt.indoors.Value as AnimalHouse).animals.Add(animal.myID, animal);
                        (animal.home.indoors.Value as AnimalHouse).animals.Remove(animal.myID);
                    }

                    animal.home = buildingAt;
                    animal.homeLocation.Value = new Vector2(buildingAt.tileX, buildingAt.tileY);
                    (buildingAt.indoors.Value as AnimalHouse).animalsThatLiveHere.Add(animal.myID);
                    animal.makeSound();
                    Game1.globalFadeToBlack(new Game1.afterFadeFunction(__instance.finishedPlacingAnimal), 0.02f);
                }
            }
            else if (confirmingSell) // the player is selling the animal
            {
                if (__instance.yesButton.containsPoint(x, y))
                {
                    Game1.player.Money += animal.getSellPrice();
                    (animal.home.indoors.Value as AnimalHouse).animalsThatLiveHere.Remove(animal.myID);
                    animal.health.Value = -1;

                    // draw the green puffs of smoke
                    int numberOfSmokeAnimations = animal.frontBackSourceRect.Width / 2;
                    for (int i = 0; i < numberOfSmokeAnimations; i++)
                    {
                        int colourVariationAmount = Game1.random.Next(25, 200);
                        multiplayer.broadcastSprites(
                            location: Game1.currentLocation,
                            sprites: new TemporaryAnimatedSprite(
                                rowInAnimationTexture: 5,
                                position: animal.Position + new Vector2(
                                    x: Game1.random.Next(-32, animal.frontBackSourceRect.Width * 3),
                                    y: Game1.random.Next(-32, animal.frontBackSourceRect.Height * 3)),
                                color: new Color(
                                    r: byte.MaxValue - colourVariationAmount,
                                    g: byte.MaxValue,
                                    b: byte.MaxValue - colourVariationAmount),
                                animationInterval: Game1.random.NextDouble() < 0.5 ? 50f : Game1.random.Next(30, 200),
                                sourceRectWidth: 64,
                                sourceRectHeight: 64,
                                delay: Game1.random.NextDouble() < 0.5 ? 0 : Game1.random.Next(0, 600)
                        )
                            {
                                scale = Game1.random.Next(2, 5) * 0.25f,
                                alpha = Game1.random.Next(2, 5) * 0.25f,
                                motion = new Vector2(0.0f, (float)-Game1.random.NextDouble())
                            });
                    }

                    Game1.playSound("newRecipe");
                    Game1.playSound("money");
                    Game1.exitActiveMenu();
                }
                else if (__instance.noButton.containsPoint(x, y))
                {
                    confirmingSell = false;
                    Game1.playSound("smallSelect");

                    if (Game1.options.SnappyMenus)
                    {
                        __instance.currentlySnappedComponent = __instance.getComponentWithID(103);
                        __instance.snapCursorToCurrentSnappedComponent();
                    }
                }
            }
            else // player is in the main query menu
            {
                // close menu
                if (__instance.okButton != null && __instance.okButton.containsPoint(x, y) && __instance.readyToClose())
                {
                    Game1.exitActiveMenu();

                    if (textBox.Text.Length > 0 && !Utility.areThereAnyOtherAnimalsWithThisName(textBox.Text))
                    {
                        animal.displayName = textBox.Text;
                        animal.Name = textBox.Text;
                    }

                    Game1.playSound("smallSelect");
                }

                // sell animal - brings up animal confirmation dialogue
                if (__instance.sellButton.containsPoint(x, y))
                {
                    confirmingSell = true;

                    // create yes button
                    var yesButton = new ClickableTextureComponent(
                        bounds: new Rectangle(
                            x: Game1.viewport.Width / 2 - 64 - 4,
                            y: Game1.viewport.Height / 2 - 32,
                            width: 64,
                            height: 64),
                        texture: Game1.mouseCursors,
                        sourceRect: Game1.getSourceRectForStandardTileSheet(
                            tileSheet: Game1.mouseCursors,
                            tilePosition: 46),
                        scale: 1
                    );
                    yesButton.myID = 111;
                    yesButton.rightNeighborID = 105;
                    __instance.yesButton = yesButton;

                    // create no button
                    var noButton = new ClickableTextureComponent(
                        bounds: new Rectangle(
                            x: Game1.viewport.Width / 2 + 4,
                            y: Game1.viewport.Height / 2 - 32,
                            width: 64,
                            height: 64),
                        texture: Game1.mouseCursors,
                        sourceRect: Game1.getSourceRectForStandardTileSheet(
                            tileSheet: Game1.mouseCursors,
                            tilePosition: 47),
                        scale: 1
                    );
                    noButton.myID = 105;
                    noButton.leftNeighborID = 111;
                    __instance.noButton = noButton;

                    Game1.playSound("smallSelect");

                    if (Game1.options.SnappyMenus)
                    {
                        __instance.populateClickableComponentList();
                        __instance.currentlySnappedComponent = __instance.noButton;
                        __instance.snapCursorToCurrentSnappedComponent();
                    }
                }
                else if (__instance.moveHomeButton.containsPoint(x, y)) // player is abount to rehouse animal
                {
                    Game1.playSound("smallSelect");
                    Game1.globalFadeToBlack(new Game1.afterFadeFunction(__instance.prepareForAnimalPlacement), 0.02f);
                }
                else if (__instance.allowReproductionButton != null && __instance.allowReproductionButton.containsPoint(x, y)) // player is toggling whether the animal can reproduce
                {
                    Game1.playSound("drumkit6");
                    animal.allowReproduction.Value = !animal.allowReproduction;
                    __instance.allowReproductionButton.sourceRect.X = !animal.allowReproduction ? 137 : 128;
                }
                else
                {
                    textBox.Update();
                }
            }

            // set private members
            typeof(AnimalQueryMenu).GetField("animal", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, animal);
            typeof(AnimalQueryMenu).GetField("confirmingSell", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, confirmingSell);

            return false;
        }

        /// <summary>The prefix for the Draw method.</summary>
        /// <param name="b">The <see cref="SpriteBatch"/> to draw the menu to..</param>
        /// <param name="__instance">The <see cref="AnimatedSprite"/> instance being patched.</param>
        /// <returns>False meaning the original method won't get ran.</returns>
        internal static bool DrawPrefix(SpriteBatch b, AnimalQueryMenu __instance)
        {
            // get private members
            var movingAnimal = (bool)typeof(AnimalQueryMenu).GetField("movingAnimal", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var animal = (FarmAnimal)typeof(AnimalQueryMenu).GetField("animal", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var textBox = (TextBox)typeof(AnimalQueryMenu).GetField("textBox", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var parentName = (string)typeof(AnimalQueryMenu).GetField("parentName", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var loveLevel = (double)typeof(AnimalQueryMenu).GetField("loveLevel", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var confirmingSell = (bool)typeof(AnimalQueryMenu).GetField("confirmingSell", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var hoverText = (string)typeof(AnimalQueryMenu).GetField("hoverText", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

            // player is viewing main animal query menu
            if (!movingAnimal && !Game1.globalFade)
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
                    y: __instance.yPositionOnScreen + 128,
                    width: AnimalQueryMenu.width,
                    height: AnimalQueryMenu.height - 128,
                    speaker: false,
                    drawOnlyBox: true
                );

                if (animal.harvestType != (byte)2)
                    textBox.Draw(b);

                // construct age string
                int monthsOld = (animal.age + 1) / 28 + 1;
                string ageString = monthsOld <= 1
                    ? Game1.content.LoadString(Path.Combine("Strings", "UI:AnimalQuery_Age1"))
                    : Game1.content.LoadString(Path.Combine("Strings", "UI:AnimalQuery_AgeN"), monthsOld);

                // if the animal is a baby, add ' (Baby)' onto the age string
                if (animal.age < animal.ageWhenMature)
                    ageString += Game1.content.LoadString(Path.Combine("Strings", "UI:AnimalQuery_AgeBaby"));

                // draw age string
                Utility.drawTextWithShadow(
                    b: b,
                    text: ageString,
                    font: Game1.smallFont,
                    position: new Vector2(
                        x: __instance.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 32,
                        y: __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16 + 128),
                    color: Game1.textColor
                );

                // draw the parent of the animal
                int parentWidthOffset = 0;
                if (parentName != null)
                {
                    parentWidthOffset = 21;
                    Utility.drawTextWithShadow(
                        b: b,
                        text: Game1.content.LoadString(Path.Combine("Strings", "UI:AnimalQuery_Parent"), parentName),
                        font: Game1.smallFont,
                        position: new Vector2(
                            x: __instance.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 32,
                            y: __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 32 + 16 + 128),
                        color: Game1.textColor
                    );
                }

                // get whether a partial heart should be drawn (and in what heart it should be drawn)
                int partialHeartLocation = loveLevel * 1000f % 200f >= 100.0
                    ? (int)(loveLevel * 1000f / 200f)
                    : -100;

                // draw the 5 animal friendship hearts
                for (int i = 0; i < 5; i++)
                {
                    // draw heart background
                    b.Draw(
                        texture: Game1.mouseCursors,
                        position: new Vector2(
                            x: (float)(__instance.xPositionOnScreen + 96 + 32 * i),
                            y: (float)(parentWidthOffset + __instance.yPositionOnScreen - 32 + 320)),
                        sourceRectangle: new Rectangle(
                            x: 211 + (loveLevel * 1000.0 <= (double)((i + 1) * 195) ? 7 : 0),
                            y: 428,
                            width: 7,
                            height: 6),
                        color: Color.White,
                        rotation: 0,
                        origin: Vector2.Zero,
                        scale: 4,
                        effects: SpriteEffects.None,
                        layerDepth: 0.89f
                    );

                    if (partialHeartLocation == i)
                    {
                        // draw partially filled heart
                        b.Draw(
                            texture: Game1.mouseCursors,
                            position: new Vector2(
                                x: (float)(__instance.xPositionOnScreen + 96 + 32 * i),
                                y: (float)(parentWidthOffset + __instance.yPositionOnScreen - 32 + 320)),
                            sourceRectangle: new Rectangle(
                                x: 211,
                                y: 428,
                                width: 4,
                                height: 6),
                            color: Color.White,
                            rotation: 0,
                            origin: Vector2.Zero,
                            scale: 4,
                            effects: SpriteEffects.None,
                            layerDepth: 0.891f
                        );
                    }
                }

                // draw mood message
                Utility.drawTextWithShadow(
                    b: b,
                    text: Game1.parseText(animal.getMoodMessage(), Game1.smallFont, AnimalQueryMenu.width - IClickableMenu.spaceToClearSideBorder * 2 - 64),
                    font: Game1.smallFont,
                    position: new Vector2(
                        x: __instance.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 32,
                        y: parentWidthOffset + __instance.yPositionOnScreen + 384 - 64 + 4),
                    color: Game1.textColor
                );

                __instance.okButton.draw(b);
                __instance.sellButton.draw(b);
                __instance.moveHomeButton.draw(b);

                if (__instance.allowReproductionButton != null)
                    __instance.allowReproductionButton.draw(b);

                if (confirmingSell)
                {
                    // draw dark background
                    b.Draw(
                        texture: Game1.fadeToBlackRect,
                        destinationRectangle: Game1.graphics.GraphicsDevice.Viewport.Bounds,
                        color: Color.Black * 0.75f
                    );

                    // draw sell confirmation background
                    Game1.drawDialogueBox(
                        x: Game1.viewport.Width / 2 - 160,
                        y: Game1.viewport.Height / 2 - 192,
                        width: 320,
                        height: 256,
                        speaker: false,
                        drawOnlyBox: true
                    );

                    // draw 'Are you sure?' message
                    var message = "Are you sure?";
                    b.DrawString(
                        spriteFont: Game1.dialogueFont,
                        text: message,
                        position: new Vector2(
                            x: Game1.viewport.Width / 2f - Game1.dialogueFont.MeasureString(message).X / 2f,
                            y: Game1.viewport.Height / 2f - 96 + 8),
                        color: Game1.textColor
                    );

                    // draw yes/no buttons
                    __instance.yesButton.draw(b);
                    __instance.noButton.draw(b);
                }
                else if (hoverText != null && hoverText.Length > 0)
                {
                    // draw hover text
                    IClickableMenu.drawHoverText(
                        b: b,
                        text: hoverText,
                        font: Game1.smallFont
                    );
                }
            }
            else if (!Game1.globalFade) // player is choosing a new home for the animal
            {
                // construct housing string
                var customAnimal = ModEntry.Instance.Api.GetAnimalBySubTypeName(animal.type);
                var buildingsString = "";

                for (var i = 0; i < customAnimal.Data.Buildings.Count; i++)
                {
                    var building = customAnimal.Data.Buildings[i];
                    buildingsString += building;
                    if (i != customAnimal.Data.Buildings.Count - 1)
                        buildingsString += ", ";
                }

                var housingString = $"Please choose a: {buildingsString} for your {animal.type}";

                // housing string background
                Game1.drawDialogueBox(
                    x: 32,
                    y: -64,
                    width: (int)Game1.dialogueFont.MeasureString(housingString).X + IClickableMenu.borderWidth * 2 + 16,
                    height: 128 + IClickableMenu.borderWidth * 2,
                    speaker: false,
                    drawOnlyBox: true
                );

                // draw housing string
                b.DrawString(
                    spriteFont: Game1.dialogueFont,
                    text: housingString,
                    position: new Vector2(
                        x: 32 + IClickableMenu.spaceToClearSideBorder * 2 + 8,
                        y: 44f),
                    color: Game1.textColor
                );

                // draw ok button
                __instance.okButton.draw(b);
            }

            // draw cursor
            __instance.drawMouse(b);

            return false;
        }
    }
}
