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
using Prism99_Core.PatchingFramework;
using Prism99_Core.Utilities;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.Buildings;


namespace WhoLivesHere
{
    /// <summary>
    /// Version 1.6 logic
    /// </summary>
    internal class WhoLivesHereLogic
    {
        private  static bool Visible = false;
        private KeybindList toggleKey;
        private readonly static Dictionary<string, Tuple<Rectangle, Texture2D>> animalCache = new Dictionary<string, Tuple<Rectangle, Texture2D>>();
        public void Initialize(IModHelper helper,SDVLogger logger)
        {

            GamePatches Patches = new GamePatches();
            Patches.Initialize(helper.ModRegistry.ModID, logger);
            Patches.AddPatch(false, typeof(Building), "draw",
                new Type[] { typeof(SpriteBatch) }, typeof(WhoLivesHereLogic),
                nameof(BuildingDraw_Suffix), "Capture draw for drawing occupant images.",
                "Buildings");
            Patches.ApplyPatches("");
            //
            //  add button hook to toggle display
            //
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            //
            //  set toggle key
            //
            toggleKey = new KeybindList(new Keybind(new SButton[] { SButton.N, SButton.LeftControl }));

        }
        /// <summary>
        /// Check if toggle key was pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Input_ButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (toggleKey.JustPressed())
                Visible = !Visible;
        }
        /// <summary>
        /// Draw building inhabitants on buiding image
        /// </summary>
        /// <param name="__instance">Building to be drawn</param>
        /// <param name="b">SpriteBatch from the game</param>
        private static void BuildingDraw_Suffix(Building __instance, SpriteBatch b)
        {
            if (Visible)
            {
                //
                //  verify building has an indoors
                //
                if (__instance.indoors.Value != null)
                {
                    int iPointer = 0;
                    int columns = 6;
                    //
                    //  check for an AnimalHouse
                    //
                    if (__instance.indoors.Value is AnimalHouse house)
                    {
                        int topX = 70;
                        int topY = -120;
                        //
                        //  adjust display coordinates for a Coop
                        if (house.Name.Contains("Coop", StringComparison.CurrentCultureIgnoreCase))
                        {
                            topY = -170;
                        }
                        //
                        //  loop through the house animals and draw their
                        //  image above their home building
                        //
                        foreach (var animal in house.animalsThatLiveHere)
                        {
                            string animalType = null;
                            //
                            //  look for the animal in the building
                            if (house.animals.TryGetValue(animal, out var houseAnimal))
                                animalType = houseAnimal.type.Value;
                            //
                            //  look for the animal outdoors
                            else if (house.GetParentLocation().animals.TryGetValue(animal, out var farmAnimal))
                                animalType = farmAnimal.type.Value;

                            if (!string.IsNullOrEmpty(animalType))
                            {
                                if (TryGetGetAnimalImage(animalType, out Texture2D animalImage, out Rectangle sourceRectangle))
                                {
                                    int xOffset = (iPointer == 0 ? 0 : iPointer % columns) * 50;
                                    int yOffset = (iPointer == 0 ? 0 : iPointer / columns) * 60;

                                    if (animalImage == null)
                                    {
                                        b.DrawString(Game1.dialogueFont, animalType, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)__instance.tileX.Value * 64 + topX + xOffset, (int)__instance.tileY.Value * 64 + topY + yOffset)), Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                                    }
                                    else
                                    {
                                        int iScale = sourceRectangle.Height == 16 ? 3 : 2;
                                        b.Draw(animalImage, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)__instance.tileX.Value * 64 + topX + xOffset, (int)__instance.tileY.Value * 64 + topY + yOffset)), sourceRectangle, Color.White, 0, Vector2.Zero, iScale, SpriteEffects.None, 1);
                                    }
                                    iPointer++;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static bool TryGetAnimalPortraitDetails(string animalType, out Point portraitSize, out string textName)
        {
            portraitSize = Point.Zero;
            textName = null;
            if (Game1.farmAnimalData.TryGetValue(animalType, out var animalData))
            {
                portraitSize = new Point(animalData.SpriteWidth, animalData.SpriteHeight);
                if (string.IsNullOrEmpty(animalData.Texture))
                {
                    textName = "Animals/" + animalType;
                }
                else
                {
                    textName = animalData.Texture;
                }
                return true;
            }

            return false;
        }
        /// <summary>
        /// Get the animal spritesheet
        /// </summary>
        /// <param name="animalType">Requested type of animal</param>
        /// <param name="spriteSheet">Return SpriteSheet</param>
        /// <param name="sourceRectangle">Return Source Rectangle of the image</param>
        /// <returns>True if the animal spritessheet is found</returns>
        private static bool TryGetGetAnimalImage(string animalType, out Texture2D spriteSheet, out Rectangle sourceRectangle)
        {
            spriteSheet = null;
            sourceRectangle = Rectangle.Empty;

            if (string.IsNullOrEmpty(animalType)) return false;
            //
            // check for cached result
            //
            if (animalCache.TryGetValue(animalType, out Tuple<Rectangle, Texture2D> textureDetails))
            {
                spriteSheet = textureDetails.Item2;
                sourceRectangle = textureDetails.Item1;
                return true;
            }

            if (TryGetAnimalPortraitDetails(animalType, out Point imageDimensions, out string spriteSheetName))
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
