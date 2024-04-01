/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/WhoLivesHere
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using Prism99_Core.Utilities;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.Buildings;
using System;
using System.Linq;
using System.Collections.Generic;
using WhoLivesHereCore;
using WhoLivesHereCore.i18n;
#if !v16
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
#endif


namespace WhoLivesHere
{
    /// <summary>
    /// Entry module
    /// </summary>
    internal class WhoLivesHereLogic
    {
        private static bool Visible = false;
        private SDVLogger logger;
        private KeybindList toggleKey;
        private static Texture2D notHomeTexture;
        private static Texture2D notFedTexture;
        private static Texture2D unoccupiedTexture;
        private static Texture2D pageTabSquare;
        private static Texture2D pageTabSquareOutline;
        private static Texture2D animalCountBackground;
        private static WhoLivesHereConfig config;
        private static int pageSize = 18;
        private static int pageWidth = 6;
        private static bool autoToggledOn = false;
        private static bool autoToggledOff = false;
        private readonly static Dictionary<string, Tuple<Rectangle, Texture2D>> animalCache = new Dictionary<string, Tuple<Rectangle, Texture2D>>();
        public void Initialize(IModHelper helper, SDVLogger ologger, WhoLivesHereConfig oConfig)
        {
            logger = ologger;
            config = oConfig;
            //
            //  add translations
            //
            I18n.Init(helper.Translation);
            //
            //  add button hook to toggle display
            //
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            //
            //  add time change hook to check auto on/off
            //
            helper.Events.GameLoop.TimeChanged += GameLoop_TimeChanged;
            //
            //
            //
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.Display.RenderedHud += Display_RenderedHud;
            //
            //  apply required harmony patches
            //
            VersionLevel.ApplyPatches(helper.ModRegistry.ModID, ologger);
            //
            //  set toggle key
            //
            toggleKey = config.ToggleKey;
            //
            //  set assorted background colours
            //
            //  not at home
            Color[] colors = new Color[] { Color.SandyBrown };
            notHomeTexture = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            notHomeTexture.SetData<Color>(colors);
            //  not fed
            colors = new Color[] { Color.PaleVioletRed };
            notFedTexture = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            notFedTexture.SetData<Color>(colors);
            //  unoccupied
            colors = new Color[] { Color.MediumPurple };
            unoccupiedTexture = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            unoccupiedTexture.SetData<Color>(colors);
            // page tab inner square
            colors = new Color[] { Color.White };
            pageTabSquare = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            pageTabSquare.SetData<Color>(colors);
            // page tab outer square
            colors = new Color[] { Color.Black };
            pageTabSquareOutline = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            pageTabSquareOutline.SetData<Color>(colors);
            //  animal count background
            colors = new Color[] { Color.Black };
            animalCountBackground = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            animalCountBackground.SetData<Color>(colors);
        }


        /// <summary>
        /// Clear auto toggle flags at the start of the day
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameLoop_DayStarted(object? sender, DayStartedEventArgs e)
        {
            autoToggledOn = false;
            autoToggledOff = false;
        }

        /// <summary>
        /// Automatically toggle the display visibility, if enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameLoop_TimeChanged(object? sender, TimeChangedEventArgs e)
        {
            if (!autoToggledOn && !Visible && config.AutoOnTime > 0 && Game1.timeOfDay >= config.AutoOnTime && Game1.timeOfDay < config.AutoOffTime)
            {
                Visible = true;
                autoToggledOn = true;
            }
            if (!autoToggledOff && Visible && config.AutoOffTime > 0 && Game1.timeOfDay >= config.AutoOffTime)
            {
                Visible = false;
                autoToggledOff = true;
            }
        }

        /// <summary>
        /// Check if toggle key was pressed or left mouse click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Input_ButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            //
            //  check for visibilty toggle key
            //
            if (toggleKey.JustPressed())
            {
                Visible = !Visible;
                logger.Log($"WhoLivesHere toggled, visible:  {Visible}", LogLevel.Debug);
            }
            //
            //  check for left click to increment page tab
            //
            if (Visible && e.IsDown(SButton.MouseLeft) && Game1.currentLocation != null)
            {
                AnimalHouse? house = VersionLevel.GetHoverBuilding();
                if (house != null)
                {
                    IncrementPageTab(house);
                    house.modData["wholiveshere.lastupdate"] = GetTimeValue().ToString();
                }
            }
        }
        /// <summary>
        /// Increments the currently displayed page tab number
        /// </summary>
        /// <param name="house">The AnimalHouse to display</param>
        /// <returns>The current page number</returns>
        private static int IncrementPageTab(AnimalHouse house)
        {
            int pageNumber = 0;
            //
            //  check to see if multiple tabs are required
            //
            if (house.animalLimit.Value > pageSize)
            {
                if (house.modData.TryGetValue("wholiveshere.currentPage", out string currentPage))
                {
                    //  parse stored page number value
                    if (!int.TryParse(currentPage, out pageNumber))
                        pageNumber = 0;
                    //
                    //  validate the stored page number
                    if (pageNumber * pageSize > house.animalLimit.Value)
                        pageNumber = 0;

                    pageNumber++;
                    //
                    //  check for page rollover
                    //
                    if (pageNumber >= Math.Ceiling(house.animalLimit.Value / (double)pageSize))
                    {
                        pageNumber = 0;
                    }
                    //
                    //  check for hding empty tabs
                    //
                    if (config.HideEmptyTabs && pageNumber * pageSize > house.animalsThatLiveHere.Count)
                    {
                        pageNumber = 0;
                    }
                }
            }
            house.modData["wholiveshere.currentPage"] = pageNumber.ToString();

            return pageNumber;
        }
        /// <summary>
        /// Draw building inhabitants on buiding image
        /// </summary>
        /// <param name="__instance">Building to be drawn</param>
        /// <param name="b">SpriteBatch from the game</param>
        internal static void BuildingDraw_Suffix(Building __instance, SpriteBatch b)
        {
            if (Visible)
            {
                //
                //  verify building has an indoors
                //
                if (__instance.indoors.Value != null)
                {
                    //
                    //  check for an AnimalHouse
                    //
                    if (__instance.indoors.Value is AnimalHouse house)
                    {
                        DrawAnimalPage(__instance, b, house);
                    }
                }
            }
        }
        /// <summary>
        /// Get time value for tab page delay timing
        /// </summary>
        /// <returns>Game ticks</returns>
        private static int GetTimeValue()
        {
            return Game1.ticks;
        }
        /// <summary>
        /// Draw the information panel on screen
        /// </summary>
        /// <param name="__instance">Building housing the animals</param>
        /// <param name="b">SpriteBatch for drawing</param>
        /// <param name="house">Interior AnimalHouse</param>
        private static void DrawAnimalPage(Building __instance, SpriteBatch b, AnimalHouse house)
        {
            int pageNumber = 0;
            int iPointer = 0;
            int columns = pageWidth;
            int lastUpdate = 0;
            int topX = 60;
            int topY = -120;


            //
            //  adjust display coordinates for a Coop
            //
            if (house.Name.Contains("Coop", StringComparison.CurrentCultureIgnoreCase))
            {
                topX = 30;
                topY = -170;
            }
            //
            //  check for multi-page display
            //
            if (house.animalLimit.Value > pageSize)
            {
                //
                //  check for page data
                //
                if (house.modData.TryGetValue("wholiveshere.currentPage", out string currentPage))
                {
                    //  parse stored page number value
                    if (!int.TryParse(currentPage, out pageNumber))
                        pageNumber = 0;
                    //
                    //  validate the stored page number
                    if (pageNumber * pageSize > house.animalLimit.Value)
                        pageNumber = 0;
                }
                else
                {
                    // first loop, set currentPage to 0
                    house.modData["wholiveshere.currentPage"] = pageNumber.ToString();
                }
                if (house.modData.TryGetValue("wholiveshere.lastupdate", out string update))
                {
                    //  verify stored lastUpdate
                    if (int.TryParse(update, out lastUpdate))
                    {
                        // verify lastUpdate is current
                        if (lastUpdate > GetTimeValue())
                        {
                            lastUpdate = GetTimeValue();
                            house.modData["wholiveshere.lastupdate"] = GetTimeValue().ToString();
                        }
                    }
                    else
                    {
                        // value stored is invalid, set lastUpdate to now
                        lastUpdate = GetTimeValue();
                        house.modData["wholiveshere.lastupdate"] = GetTimeValue().ToString();
                    }
                }
                else
                {
                    // first loop, set lastUpdate to now
                    lastUpdate = GetTimeValue();
                    house.modData["wholiveshere.lastupdate"] = GetTimeValue().ToString();
                }

                if (config.PageDelay > 0 && GetTimeValue() - lastUpdate > config.PageDelay)
                {
                    //  rotate to the next page
                    pageNumber = IncrementPageTab(house);
                    house.modData["wholiveshere.lastupdate"] = GetTimeValue().ToString();
                }
            }
            //
            //  draw page tab boxes
            //
            int numPages = (int)Math.Ceiling(VersionLevel.MaxCapacity(__instance) / (double)pageSize);
            if (Math.Ceiling(VersionLevel.MaxCapacity(__instance) / (double)pageSize) > 1)
            {
                int pageTabSize = 24;
                int topOffSet = 200;
                int borderSize = 5;
                Vector2 rotationOrigin = new Vector2(pageTabSize / 2, pageTabSize / 2);
                int tabOffset = 200 - ((pageTabSize + 3) * numPages) / 2;
                for (int displayPage = 0; displayPage < numPages; displayPage++)
                {
                    int leftOffSet = tabOffset + displayPage * (pageTabSize + 3);

                    if (displayPage == pageNumber)
                    {
                        b.Draw(pageTabSquare, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)__instance.tileX.Value * 64 + topX + leftOffSet, (int)__instance.tileY.Value * 64 + topY + topOffSet)), new Rectangle(0, 0, pageTabSize, pageTabSize), Color.White, 0, rotationOrigin, 1, SpriteEffects.None, 0.98f);
                        b.Draw(pageTabSquareOutline, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)__instance.tileX.Value * 64 + topX + leftOffSet + borderSize, (int)__instance.tileY.Value * 64 + topY + topOffSet + borderSize)), new Rectangle(0, 0, pageTabSize - 2 * borderSize, pageTabSize - 2 * borderSize), Color.White, 0, rotationOrigin, 1, SpriteEffects.None, 0.99f);
                    }
                    else
                    {
                        b.Draw(pageTabSquare, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)__instance.tileX.Value * 64 + topX + leftOffSet, (int)__instance.tileY.Value * 64 + topY + topOffSet)), new Rectangle(0, 0, pageTabSize, pageTabSize), Color.White, 0, rotationOrigin, 1, SpriteEffects.None, 0.99f);
                    }
                }
            }
            int animalCount = 0;
            //
            //  loop through the house animals and draw their
            //  image above their home building
            //
            foreach (var animalId in house.animalsThatLiveHere.Skip(pageNumber * pageSize).OrderBy(p => p))
            {
                bool isHome = true;

                string animalType = string.Empty;
                FarmAnimal animal;
                //
                //  look for the animal in the building
                if (house.animals.TryGetValue(animalId, out animal))
                    animalType = animal.type.Value;
                //
                //  look for the animal outdoors
                else if (house.GetParentLocation()?.animals.TryGetValue(animalId, out animal) ?? false)
                {
                    animalType = animal.type.Value;
                    isHome = false;
                }

                if (!string.IsNullOrEmpty(animalType))
                {

                    if (TryGetGetAnimalImage(animalType, out Texture2D animalImage, out Rectangle sourceRectangle))
                    {
                        //
                        //  set offset from building location to top corner of
                        //  animal display
                        //
                        int xOffset = (iPointer == 0 ? 0 : iPointer % columns) * 70;
                        int yOffset = (iPointer == 0 ? 0 : iPointer / columns) * 70;

                        if (animalImage == null)
                        {
                            //
                            //  if we did not get an image, display text
                            //  of the missing animal type
                            //
                            b.DrawString(Game1.dialogueFont, animalType, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)__instance.tileX.Value * 64 + topX + xOffset, (int)__instance.tileY.Value * 64 + topY + yOffset)), Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                        }
                        else
                        {
                            int iScale = sourceRectangle.Height == 16 ? 3 : 2;
                            Vector2 rotationOrigin = new Vector2(sourceRectangle.Width / 2f, sourceRectangle.Height / 2f);
                            //
                            //  draw colour status boxes
                            //
                            //
                            //  calculate height of colour box(es)
                            //
                            int splits = 0;
                            if (!VersionLevel.WasFeed(animal))
                                splits++;
                            if (!isHome)
                                splits++;

                            int splitIndex = 0;
                            int splitHeight = splits == 0 ? sourceRectangle.Height : sourceRectangle.Height / splits;
                            //
                            //  based on # of data points to be display
                            //  set the colour box height
                            //
                            Rectangle backgroundBox;
                            if (splits > 1)
                            {
                                backgroundBox = new Rectangle(0, 0, sourceRectangle.Width, splitHeight);
                            }
                            else
                            {
                                backgroundBox = sourceRectangle;
                            }
                            if (!VersionLevel.WasFeed(animal))
                            {
                                b.Draw(notFedTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)__instance.tileX.Value * 64 + topX + xOffset, (int)__instance.tileY.Value * 64 + topY + yOffset)), backgroundBox, Color.White, 0, rotationOrigin, iScale, SpriteEffects.None, 0.99f);
                                splitIndex++;
                            }
                            if (!isHome)
                            {
                                //
                                //  calculate top position of the
                                //  colour box
                                //
                                int splitDrop = 0;
                                if (splitIndex > 0)
                                {
                                    splitDrop = splitHeight * (splitIndex + 1);
                                }
                                b.Draw(notHomeTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)__instance.tileX.Value * 64 + topX + xOffset, (int)__instance.tileY.Value * 64 + topY + yOffset + splitDrop)), backgroundBox, Color.White, 0, rotationOrigin, iScale, SpriteEffects.None, 0.99f);
                            }
                            //
                            //  draw animal portrait
                            //
                            b.Draw(animalImage, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)__instance.tileX.Value * 64 + topX + xOffset, (int)__instance.tileY.Value * 64 + topY + yOffset)), sourceRectangle, Color.White, 0, rotationOrigin, iScale, SpriteEffects.None, 1);
                        }
                        iPointer++;
                    }
                }
                animalCount++;
                if (animalCount >= pageSize)
                    break;
            }
            //
            //  add status boxes for empty house slots
            //
            if (!config.HideEmptyTabs && animalCount < pageSize && pageNumber * pageSize + animalCount < VersionLevel.MaxCapacity(__instance))
            {
                int placeHolderSize = 32;
                Vector2 rotationOrigin = new Vector2(placeHolderSize / 2, placeHolderSize / 2);
                for (int filler = pageNumber * pageSize + animalCount; filler < VersionLevel.MaxCapacity(__instance); filler++)
                {
                    int xOffset = (iPointer == 0 ? 0 : iPointer % columns) * 70;
                    int yOffset = (iPointer == 0 ? 0 : iPointer / columns) * 70;

                    b.Draw(unoccupiedTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)__instance.tileX.Value * 64 + topX + xOffset, (int)__instance.tileY.Value * 64 + topY + yOffset)), new Rectangle(0, 0, placeHolderSize, placeHolderSize), Color.White, 0, rotationOrigin, 2, SpriteEffects.None, 0.99f);

                    iPointer++;
                    animalCount++;
                    if (animalCount > pageSize - 1)
                        break;
                }
            }
            if (config.ShowAnimalCount)
            {
                int bgWidth = 60;
                int bgHeight = 40;
                Vector2 rotationOrg = new Vector2(bgWidth / 2, bgHeight / 2);
                b.Draw(animalCountBackground, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)__instance.tileX.Value * 64 + topX, (int)__instance.tileY.Value * 64 + topY + 200)), new Rectangle(0, 0, bgWidth, bgHeight), Color.White, 0, rotationOrg, 1, SpriteEffects.None, 0.99f);
                b.DrawString(Game1.smallFont, house.animalsThatLiveHere.Count.ToString(), new Vector2((int)__instance.tileX.Value * 64 + topX + 5, (int)__instance.tileY.Value * 64 + topY + 200 + 5), Color.White, 0f, rotationOrg, 1f, SpriteEffects.None, 1);
            }
        }
        private static void Display_RenderedHud(object? sender, RenderedHudEventArgs e)
        {
            if (config.ShowMissingHay)
                VersionLevel.HudRendered(e.SpriteBatch);
        }

        public static void DrawBubble(SpriteBatch b, Texture2D texture, Rectangle sourceRectangle, Vector2 destinationPosition)
        {
            Rectangle r = new Rectangle((int)(destinationPosition.X * Game1.options.zoomLevel), (int)(destinationPosition.Y * Game1.options.zoomLevel), Game1.tileSize * 3 / 4,
                Game1.tileSize * 3 / 4);
            b.Draw(Game1.mouseCursors, r, new Rectangle(141, 465, 20, 24), Color.White * 0.75f);
            r.Offset(r.Width / 4, r.Height / 6);
            r.Height /= 2;
            r.Width /= 2;
            b.Draw(texture, r, sourceRectangle, Color.White);
        }
        /// <summary>
        /// Get the animal spritesheet
        /// </summary>
        /// <param name="animalType">Requested type of animal</param>
        /// <param name="spriteSheet">Return SpriteSheet</param>
        /// <param name="sourceRectangle">Return Source Rectangle of the image</param>
        /// <returns>True if the animal spritessheet is found</returns>
        private static bool TryGetGetAnimalImage(string animalType, out Texture2D? spriteSheet, out Rectangle sourceRectangle)
        {
            spriteSheet = null;
            sourceRectangle = Rectangle.Empty;

            if (string.IsNullOrEmpty(animalType)) return false;
            //
            // check for cached result
            //
            if (animalCache.TryGetValue(animalType, out Tuple<Rectangle, Texture2D>? textureDetails))
            {
                spriteSheet = textureDetails.Item2;
                sourceRectangle = textureDetails.Item1;
                return true;
            }

            if (VersionLevel.TryGetAnimalPortraitDetails(animalType, out Point imageDimensions, out string spriteSheetName))
            {
                try
                {
                    Texture2D animalSpriteSheet = Game1.content.Load<Texture2D>(spriteSheetName);

                    if (animalSpriteSheet == null)
                    {
                        //StardewLogger.LogWarning("GetAnimalImage", $"Could not find image for a '{sAnimalType}'");
                    }
                    else
                    {
                        int height = 16;
                        int width = 16;
                        //
                        //  get image dimensions
                        //
                        if (imageDimensions != Point.Zero)
                        {
                            height = imageDimensions.Y;
                            width = imageDimensions.X;
                        }
                        //
                        //  add results to the cache
                        animalCache.Add(animalType, Tuple.Create(new Rectangle(0, 0, width, height), animalSpriteSheet));
                        //
                        //  set return values
                        //
                        spriteSheet = animalSpriteSheet;
                        sourceRectangle = new Rectangle(0, 0, width, height);
                        return true;
                    }
                }
                catch { }
            }

            return false;
        }
    }
}
