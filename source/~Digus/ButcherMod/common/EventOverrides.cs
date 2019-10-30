using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnimalHusbandryMod.animals;
using AnimalHusbandryMod.animals.data;
using AnimalHusbandryMod.animals.events;
using AnimalHusbandryMod.common;
using AnimalHusbandryMod.farmer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Monsters;

namespace AnimalHusbandryMod.common
{
    public class EventsOverrides
    {
        private const String spring_outdoorsTileSheetName = "maps/spring_outdoorsTileSheet";
        private const String summer_outdoorsTileSheetName = "maps/summer_outdoorsTileSheet";
        private const String fall_outdoorsTileSheetName = "maps/fall_outdoorsTileSheet";
        private const String winter_outdoorsTileSheetName = "maps/winter_outdoorsTileSheet";
        private const String spring_towntName = "maps/spring_town";
        private const String summer_towntName = "maps/winter_town";
        private const String fall_towntName = "maps/fall_town";
        private const String winter_towntName = "maps/winter_town";
        private const String looseSprites_cursorsName = "LooseSprites/Cursors";
        private const String tileSheets_critters = "TileSheets/critters";
        private const String townInterior = "maps/townInterior";
        private const String looseSprites_temporary_sprites_1 = "LooseSprites/temporary_sprites_1";

        private static readonly Vector2 BirdOffset = new Vector2(-9f, -20f);
        private static readonly Vector2 FrogOffset = new Vector2(-3f, -4f);
        private static readonly Vector2 SquirrelOffset = new Vector2(-6f, -20f);
        private static readonly Vector2 CrabOffset = new Vector2(0f, -6f);

        public static void addSpecificTemporarySprite(Event __instance, ref string key, GameLocation location, string[] split)
        {
            if (!key.StartsWith("animalContest"))
            {
                return;
            }
            if (key == "animalContest")
            {
                String outdoorsTextureName = null;
                switch (SDate.Now().Season)
                {
                    case "spring":
                        outdoorsTextureName = spring_outdoorsTileSheetName;
                        break;
                    case "summer":
                        outdoorsTextureName = summer_outdoorsTileSheetName;
                        break;
                    case "fall":
                        outdoorsTextureName = fall_outdoorsTileSheetName;
                        break;
                    case "winter":
                        outdoorsTextureName = winter_outdoorsTileSheetName;
                        break;
                }

                location.TemporarySprites.Add(new TemporaryAnimatedSprite(DataLoader.LooseSpritesName,
                    new Rectangle(84, 0, 98, 79), 9999f, 1, 999,
                    new Vector2(26f, 59f) * (float) Game1.tileSize, false, false,
                    (float) (59 * Game1.tileSize) / 10000f, 0.0f, Color.White,
                    (float) Game1.pixelZoom, 0.0f, 0.0f, 0.0f, false));

                //Outdoors
                Rectangle singleFeed = new Rectangle(304, 144, 16, 32);
                Rectangle doubleFeed = new Rectangle(320, 128, 32, 32);
                Rectangle water = new Rectangle(288, 112, 32, 32);
                Rectangle create = new Rectangle(288, 144, 16, 32);
                //LooseSprites
                Rectangle TopLeft = new Rectangle(0, 44, 16, 16);
                Rectangle TopCenter = new Rectangle(16, 44, 16, 16);
                Rectangle TopRight = new Rectangle(32, 44, 16, 16);
                Rectangle CenterLeft = new Rectangle(0, 60, 16, 16);
                Rectangle CenterCenter = new Rectangle(16, 60, 16, 16);
                Rectangle CenterRight = new Rectangle(32, 60, 16, 16);
                Rectangle BottonLeft = new Rectangle(0, 76, 16, 16);
                Rectangle BottonCenter = new Rectangle(16, 76, 16, 16);
                Rectangle BottonRight = new Rectangle(32, 76, 16, 16);

                Rectangle LeftUp = new Rectangle(48, 44, 16, 16);
                Rectangle RightUp = new Rectangle(64, 44, 16, 16);
                Rectangle LeftDown = new Rectangle(48, 60, 16, 16);
                Rectangle RightDown = new Rectangle(64, 60, 16, 16);

                addTemporarySprite(location, outdoorsTextureName, doubleFeed, 24, 62);
                addTemporarySprite(location, outdoorsTextureName, water, 32, 62);
                addTemporarySprite(location, outdoorsTextureName, singleFeed, 34, 62);
                addTemporarySprite(location, outdoorsTextureName, create, 23, 62);

                addTemporarySprite(location, DataLoader.LooseSpritesName, TopLeft, 22, 64);
                addTemporarySprite(location, DataLoader.LooseSpritesName, TopRight, 23, 64);
                addTemporarySprite(location, DataLoader.LooseSpritesName, TopLeft, 24, 64);
                addTemporarySprite(location, DataLoader.LooseSpritesName, TopCenter, 25, 64);
                addTemporarySprite(location, DataLoader.LooseSpritesName, TopCenter, 26, 64);
                addTemporarySprite(location, DataLoader.LooseSpritesName, TopCenter, 27, 64);
                addTemporarySprite(location, DataLoader.LooseSpritesName, TopCenter, 28, 64);
                addTemporarySprite(location, DataLoader.LooseSpritesName, TopCenter, 29, 64);
                addTemporarySprite(location, DataLoader.LooseSpritesName, TopCenter, 30, 64);
                addTemporarySprite(location, DataLoader.LooseSpritesName, TopCenter, 31, 64);
                addTemporarySprite(location, DataLoader.LooseSpritesName, TopRight, 32, 64);
                addTemporarySprite(location, DataLoader.LooseSpritesName, TopLeft, 33, 64);
                addTemporarySprite(location, DataLoader.LooseSpritesName, TopCenter, 34, 64);
                addTemporarySprite(location, DataLoader.LooseSpritesName, TopRight, 35, 64);

                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterLeft, 22, 65);
                addTemporarySprite(location, DataLoader.LooseSpritesName, RightUp, 23, 65);
                addTemporarySprite(location, DataLoader.LooseSpritesName, LeftUp, 24, 65);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 25, 65);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 26, 65);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 27, 65);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 28, 65);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 29, 65);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 30, 65);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 31, 65);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterRight, 32, 65);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterLeft, 33, 65);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 34, 65);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterRight, 35, 65);

                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterLeft, 22, 66);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 23, 66);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 24, 66);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 25, 66);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 26, 66);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 27, 66);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 28, 66);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 29, 66);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 30, 66);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 31, 66);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterRight, 32, 66);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterLeft, 33, 66);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 34, 66);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterRight, 35, 66);

                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterLeft, 22, 67);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 23, 67);
                addTemporarySprite(location, DataLoader.LooseSpritesName, RightDown, 24, 67);
                addTemporarySprite(location, DataLoader.LooseSpritesName, LeftDown, 25, 67);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 26, 67);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 27, 67);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 28, 67);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 29, 67);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 30, 67);
                addTemporarySprite(location, DataLoader.LooseSpritesName, RightDown, 31, 67);
                addTemporarySprite(location, DataLoader.LooseSpritesName, BottonRight, 32, 67);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterLeft, 33, 67);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 34, 67);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterRight, 35, 67);

                addTemporarySprite(location, DataLoader.LooseSpritesName, BottonLeft, 22, 68);
                addTemporarySprite(location, DataLoader.LooseSpritesName, BottonCenter, 23, 68);
                addTemporarySprite(location, DataLoader.LooseSpritesName, BottonRight, 24, 68);
                addTemporarySprite(location, DataLoader.LooseSpritesName, TopLeft, 24, 68);
                addTemporarySprite(location, DataLoader.LooseSpritesName, BottonLeft, 25, 68);
                addTemporarySprite(location, DataLoader.LooseSpritesName, TopRight, 25, 68);
                addTemporarySprite(location, DataLoader.LooseSpritesName, BottonCenter, 26, 68);
                addTemporarySprite(location, DataLoader.LooseSpritesName, BottonCenter, 27, 68);
                addTemporarySprite(location, DataLoader.LooseSpritesName, BottonCenter, 28, 68);
                addTemporarySprite(location, DataLoader.LooseSpritesName, BottonCenter, 29, 68);
                addTemporarySprite(location, DataLoader.LooseSpritesName, BottonCenter, 30, 68);
                addTemporarySprite(location, DataLoader.LooseSpritesName, BottonRight, 31, 68);
                addTemporarySprite(location, DataLoader.LooseSpritesName, BottonLeft, 33, 68);
                addTemporarySprite(location, DataLoader.LooseSpritesName, BottonCenter, 34, 68);
                addTemporarySprite(location, DataLoader.LooseSpritesName, BottonRight, 35, 68);

                addTemporarySprite(location, DataLoader.LooseSpritesName, BottonLeft, 24, 69);
                addTemporarySprite(location, DataLoader.LooseSpritesName, BottonRight, 25, 69);
            }
            else if (key == "animalContestJoshDogSteak")
            {
                location.removeTemporarySpritesWithID(10);
                location.TemporarySprites.Add(new TemporaryAnimatedSprite(looseSprites_cursorsName,
                    new Microsoft.Xna.Framework.Rectangle(324, 1936, 12, 20), 80f, 4, 99999,
                    new Vector2(31f, 65f) * (float)Game1.tileSize + new Vector2(3f, 3f) * 4f, false, false,
                    (float)((66 * Game1.tileSize) / 10000f) + 0.00002f, 0.0f, Color.White, (float)Game1.pixelZoom, 0.0f, 0.0f, 0.0f, false)
                {
                    id = 11f
                });
                location.TemporarySprites.Add(new TemporaryAnimatedSprite(looseSprites_cursorsName,
                    new Microsoft.Xna.Framework.Rectangle(497, 1918, 11, 11), 999f, 1, 9999,
                    new Vector2(30f, 66f) * (float)Game1.tileSize + new Vector2(32f, -8f), false, false,
                    1f, 0.0f, Color.White, (float)Game1.pixelZoom, 0.0f, 0.0f, 0.0f, false)
                {
                    id = 12f
                });

            }
            else if (key == "animalContestJoshDogOut")
            {
                location.removeTemporarySpritesWithID(1);
                location.removeTemporarySpritesWithID(12);
                location.removeTemporarySpritesWithID(11);
                location.TemporarySprites.Add(new TemporaryAnimatedSprite(looseSprites_cursorsName,
                    new Microsoft.Xna.Framework.Rectangle(324, 1916, 12, 20), 500f, 6, 9999,
                    new Vector2(31f, 65f) * (float)Game1.tileSize + new Vector2(3f, 3f) * 4f, false, false,
                    (float)((66 * Game1.tileSize) / 10000f) + 0.00002f, 0.0f, Color.White, (float)Game1.pixelZoom, 0.0f, 0.0f, 0.0f, false)
                {
                    id = 10f
                });
            }
            else if (key == "animalContestJoshDog")
            {
                String townTextureName = null;
                switch (SDate.Now().Season)
                {
                    case "spring":
                        townTextureName = spring_towntName;
                        break;
                    case "summer":
                        townTextureName = summer_towntName;
                        break;
                    case "fall":
                        townTextureName = fall_towntName;
                        break;
                    case "winter":
                        townTextureName = winter_towntName;
                        break;
                }

                location.TemporarySprites.Add(new TemporaryAnimatedSprite(townTextureName,
                    new Microsoft.Xna.Framework.Rectangle(208, 0, 16, 32), 9999f, 1, 999
                    , new Vector2(31, 64) * (float)Game1.tileSize, false, false,
                    (float)(65 * Game1.tileSize) / 10000f, 0.0f, Color.White,
                    (float)Game1.pixelZoom, 0.0f, 0.0f, 0.0f, false));

                Action<int> addDogEyes = null;
                void AddDogEyesHidden(int x)
                {
                    location.TemporarySprites.Add(new TemporaryAnimatedSprite(townTextureName,
                        new Microsoft.Xna.Framework.Rectangle(192, 0, 16, 16), 1000f, 1, 8,
                        new Vector2(31f, 65f) * (float)Game1.tileSize, false, false,
                        (float)((66 * Game1.tileSize) / 10000f) - 0.01f, 0.0f, Color.White,
                        (float)Game1.pixelZoom, 0.0f, 0.0f, 0.0f, false)
                    {
                        id = 1,
                        endFunction = new TemporaryAnimatedSprite.endBehavior(addDogEyes)
                    });
                }
                addDogEyes = (int x) =>
                {
                    location.TemporarySprites.Add(new TemporaryAnimatedSprite(townTextureName,
                        new Microsoft.Xna.Framework.Rectangle(192, 0, 16, 16), 1000f, 1, 1,
                        new Vector2(31f, 65f) * (float)Game1.tileSize, false, false,
                        (float)((66 * Game1.tileSize) / 10000f) + 0.00002f, 0.0f, Color.White,
                        (float)Game1.pixelZoom, 0.0f, 0.0f, 0.0f, false)
                    {
                        id = 1,
                        endFunction = new TemporaryAnimatedSprite.endBehavior((Action<int>)AddDogEyesHidden)

                    });
                };
                AddDogEyesHidden(0);
            }
            else if (key == "animalContestFrogShow")
            {
                location.TemporarySprites.Add(new TemporaryAnimatedSprite(tileSheets_critters,
                    new Microsoft.Xna.Framework.Rectangle(0, 240, 16, 16), 500f, 1, 9999,
                    new Vector2(29f, 64f) * (float)Game1.tileSize + FrogOffset * 4f, false, false,
                    (float)((64 * Game1.tileSize) / 10000f) + 0.00002f, 0.0f, Color.White, (float)Game1.pixelZoom, 0.0f, 0.0f, 0.0f, false)
                {
                    id = 2
                });
            }
            else if (key == "animalContestFrogCroak")
            {
                location.TemporarySprites.Add(new TemporaryAnimatedSprite(tileSheets_critters,
                    new Microsoft.Xna.Framework.Rectangle(64, 240, 16, 16), 100f, 4, 1,
                    new Vector2(29f, 64f) * (float)Game1.tileSize + FrogOffset * 4f, false, false,
                    (float)((64 * Game1.tileSize) / 10000f) + 0.00003f, 0.0f, Color.White, (float)Game1.pixelZoom, 0.0f, 0.0f, 0.0f, false));
            }
            else if (key == "animalContestFrogRun")
            {
                location.removeTemporarySpritesWithID(2);
                location.TemporarySprites.Add(new TemporaryAnimatedSprite(tileSheets_critters,
                    new Microsoft.Xna.Framework.Rectangle(0, 240, 16, 16), 100f, 4, 5,
                    new Vector2(29f, 64f) * (float)Game1.tileSize + FrogOffset * 4f, false, false,
                    (float)((64 * Game1.tileSize) / 10000f) + 0.00002f, 0.0f, Color.White, (float)Game1.pixelZoom, 0.0f, 0.0f, 0.0f, false)
                {
                    motion = new Vector2(5f,0f)
                });
            }
            else if (key == "animalContestSquirrelShow")
            {
                Action<int> addStillSquirrel = null;
                void AddNimbleSquirrel(int x)
                {
                    location.TemporarySprites.Add(new TemporaryAnimatedSprite(tileSheets_critters,
                        new Microsoft.Xna.Framework.Rectangle(0, 192, 32, 32), 200f, 2, 4,
                        new Vector2(29f, 64f) * (float) Game1.tileSize + SquirrelOffset * 4f, false, false,
                        (float) ((64 * Game1.tileSize) / 10000f) + 0.00002f, 0.0f, Color.White, (float) Game1.pixelZoom,
                        0.0f, 0.0f, 0.0f, false)
                    {
                        id = 3,
                        endFunction = new TemporaryAnimatedSprite.endBehavior(addStillSquirrel)
                    });
                }
                addStillSquirrel = (int x) =>
                {
                    location.TemporarySprites.Add(new TemporaryAnimatedSprite(tileSheets_critters,
                        new Microsoft.Xna.Framework.Rectangle(0, 192, 32, 32), 2500f, 1, 1,
                        new Vector2(29f, 64f) * (float) Game1.tileSize + SquirrelOffset * 4f, false, false,
                        (float) ((64 * Game1.tileSize) / 10000f) + 0.00002f, 0.0f, Color.White, (float) Game1.pixelZoom,
                        0.0f, 0.0f, 0.0f, false)
                    {
                        id = 3,
                        endFunction = new TemporaryAnimatedSprite.endBehavior((Action<int>) AddNimbleSquirrel)

                    });
                };
                AddNimbleSquirrel(0);
            }
            else if (key == "animalContestSquirrelRun")
            {
                location.removeTemporarySpritesWithID(3);
                location.TemporarySprites.Add(new TemporaryAnimatedSprite(tileSheets_critters,
                    new Microsoft.Xna.Framework.Rectangle(64, 192, 32, 32), 50f, 6, 8,
                    new Vector2(29f, 64f) * (float)Game1.tileSize + SquirrelOffset * 4f, false, false,
                    (float)((64 * Game1.tileSize) / 10000f) + 0.00002f, 0.0f, Color.White, (float)Game1.pixelZoom,
                    0.0f, 0.0f, 0.0f, false)
                {
                    motion = new Vector2(5f, 0f)
                });
            }
            else if (key == "animalContestBirdShow")
            {
                Action<int> addSleepBird = null;
                void AddStillBird(int x)
                {
                    location.TemporarySprites.Add(new TemporaryAnimatedSprite(tileSheets_critters,
                        new Microsoft.Xna.Framework.Rectangle(160, 64, 32, 32), 2500f, 1, 1,
                        new Vector2(29f, 64f) * (float)Game1.tileSize + BirdOffset * 4f, false, true,
                        (float)((64 * Game1.tileSize) / 10000f) + 0.00002f, 0.0f, Color.White, (float)Game1.pixelZoom,
                        0.0f, 0.0f, 0.0f, false)
                    {
                        id = 4,
                        endFunction = new TemporaryAnimatedSprite.endBehavior(addSleepBird)
                    });
                }
                addSleepBird = (int x) =>
                {
                    location.TemporarySprites.Add(new TemporaryAnimatedSprite(tileSheets_critters,
                        new Microsoft.Xna.Framework.Rectangle(0, 96, 32, 32), 1500f, 1, 1,
                        new Vector2(29f, 64f) * (float)Game1.tileSize + BirdOffset * 4f, false, true,
                        (float)((64 * Game1.tileSize) / 10000f) + 0.00002f, 0.0f, Color.White, (float)Game1.pixelZoom,
                        0.0f, 0.0f, 0.0f, false)
                    {
                        id = 4,
                        endFunction = new TemporaryAnimatedSprite.endBehavior((Action<int>)AddStillBird)

                    });
                };
                AddStillBird(0);
            }
            else if (key == "animalContestWildBird")
            {
                Action<int> addSleepBird = null;
                void AddStillBird(int x)
                {
                    location.TemporarySprites.Add(new TemporaryAnimatedSprite(tileSheets_critters,
                        new Microsoft.Xna.Framework.Rectangle(160, 128, 32, 32), 2500f, 1, 1,
                        new Vector2(34f, 66f) * (float)Game1.tileSize + BirdOffset * 4f, false, true,
                        (float)((64 * Game1.tileSize) / 10000f) + 0.00002f, 0.0f, Color.White, (float)Game1.pixelZoom,
                        0.0f, 0.0f, 0.0f, false)
                    {
                        id = 7,
                        endFunction = new TemporaryAnimatedSprite.endBehavior(addSleepBird)
                    });
                }
                addSleepBird = (int x) =>
                {
                    location.TemporarySprites.Add(new TemporaryAnimatedSprite(tileSheets_critters,
                        new Microsoft.Xna.Framework.Rectangle(0, 160, 32, 32), 1500f, 1, 1,
                        new Vector2(34f, 66f) * (float)Game1.tileSize + BirdOffset * 4f, false, true,
                        (float)((64 * Game1.tileSize) / 10000f) + 0.00002f, 0.0f, Color.White, (float)Game1.pixelZoom,
                        0.0f, 0.0f, 0.0f, false)
                    {
                        id = 7,
                        endFunction = new TemporaryAnimatedSprite.endBehavior((Action<int>)AddStillBird)

                    });
                };
                AddStillBird(0);
            }
            else if (key == "animalContestBirdFly2")
            {
                location.removeTemporarySpritesWithID(4);
                location.TemporarySprites.Add(new TemporaryAnimatedSprite(tileSheets_critters,
                    new Microsoft.Xna.Framework.Rectangle(32, 96, 32, 32), 60f, 3, 18,
                    new Vector2(29f, 64f) * (float)Game1.tileSize + BirdOffset * 4f, false, true,
                    (float)((64 * Game1.tileSize) / 10000f) + 0.00003f, 0.0f, Color.White, (float)Game1.pixelZoom, 0.0f, 0.0f, 0.0f, false)
                {
                    pingPong = true,
                    motion = new Vector2(6f, -1f)
                });
            }
            else if (key == "animalContestBirdFly")
            {
                location.removeTemporarySpritesWithID(4);
                int count = 18;
                Action<int> addBirdFlap = null;
                TemporaryAnimatedSprite lastSprite = null;
                void AddBirdFly(int x)
                {
                    if (count == 0)
                    {
                        return;
                    }
                    Game1.playSound("batFlap");
                    Vector2 position = lastSprite == null ? new Vector2(29f, 64f) * (float)Game1.tileSize + BirdOffset * 4f : lastSprite.Position;
                    lastSprite = new TemporaryAnimatedSprite(tileSheets_critters,
                        new Microsoft.Xna.Framework.Rectangle(32, 96, 32, 32), 60f, 3, 1,
                        position, false, true,
                        (float)((64 * Game1.tileSize) / 10000f) + 0.00003f, 0.0f, Color.White, (float)Game1.pixelZoom,
                        0.0f, 0.0f, 0.0f, false)
                    {
                        endFunction = new TemporaryAnimatedSprite.endBehavior(addBirdFlap),
                        motion = new Vector2(6f, -1.3f)
                    };
                    location.TemporarySprites.Add(lastSprite);
                    count--;
                }
                addBirdFlap = (int x) =>
                {
                    lastSprite = new TemporaryAnimatedSprite(tileSheets_critters,
                        new Microsoft.Xna.Framework.Rectangle(64, 96, 32, 32), 60f, 1, 1,
                        lastSprite.Position, false, true,
                        (float)((64 * Game1.tileSize) / 10000f) + 0.00003f, 0.0f, Color.White, (float)Game1.pixelZoom,
                        0.0f, 0.0f, 0.0f, false)
                    {
                        endFunction = new TemporaryAnimatedSprite.endBehavior((Action<int>)AddBirdFly),
                        motion = new Vector2(6f, -1.3f)
                    };
                    location.TemporarySprites.Add(lastSprite);
                };
                AddBirdFly(0);
            }
            else if (key == "animalContestRabbitShow")
            {
                bool flipped = split.Length > 4 && Convert.ToBoolean(split[4]);
                location.TemporarySprites.Add(new TemporaryAnimatedSprite(tileSheets_critters,
                    new Microsoft.Xna.Framework.Rectangle(256, 192, 32, 32), 9999f, 1, 999,
                    new Vector2(Convert.ToSingle(split[2]), Convert.ToSingle(split[3])) * (float)Game1.tileSize + new Vector2(flipped? -7f: -10f, -20f) * 4f, false, flipped,
                    (float)((64 * Game1.tileSize) / 10000f) + 0.00002f, 0.0f, Color.White, (float)Game1.pixelZoom, 0.0f, 0.0f, 0.0f, false)
                {
                    id = split.Length > 5 ? 5 : 0
                });
            }
            else if (key == "animalContestRabbitRun")
            {
                location.removeTemporarySpritesWithID(5);
                location.TemporarySprites.Add(new TemporaryAnimatedSprite(tileSheets_critters,
                    new Microsoft.Xna.Framework.Rectangle(128, 160, 32, 32), 45f, 6, 14,
                    new Vector2(29f, 64f) * (float)Game1.tileSize + new Vector2(-10f, -20f) * 4f, false, false,
                    (float)((64 * Game1.tileSize) / 10000f) + 0.00002f, 0.0f, Color.White, (float)Game1.pixelZoom, 0.0f, 0.0f, 0.0f, false)
                {
                    motion = new Vector2(6f, 0f)
                });
            }
            else if (key == "animalContestEmilyParrot")
            {
                location.TemporarySprites.Add(new TemporaryAnimatedSprite(townInterior,
                    new Microsoft.Xna.Framework.Rectangle(464, 1056, 16, 32), 9999f, 1, 999,
                    new Vector2(34f, 65f) * (float)Game1.tileSize, false, false,
                    0.000001f, 0.0f, Color.White,
                    (float)Game1.pixelZoom, 0.0f, 0.0f, 0.0f, false));
                location.TemporarySprites.Add(new EmilysParrot(new Vector2(34, 64) * (float)Game1.tileSize + new Vector2(4f, 8f) * 4f));
            }
            else if (key == "animalContestEmilyParrotAction")
            {
                if (location.getTemporarySpriteByID(5858585) is EmilysParrot emilysParrot)
                {
                    emilysParrot.doAction();
                }
            }
            else if (key == "animalContestEmilyBoomBox")
            {
                location.TemporarySprites.Add(new TemporaryAnimatedSprite(looseSprites_cursorsName,
                    new Microsoft.Xna.Framework.Rectangle(586, 1871, 24, 14), 9999f, 1, 999,
                    new Vector2(33f, 65f) * (float)Game1.tileSize, false, false,
                    0.0000009f, 0.0f, Color.White,
                    (float)Game1.pixelZoom, 0.0f, 0.0f, 0.0f, false)
                {
                    id = 6
                });
            }
            else if (key == "animalContestEmilyBoomBoxStart")
            {
                location.getTemporarySpriteByID(6).pulse = true;
                location.getTemporarySpriteByID(6).pulseTime = 420f;
            }
            else if (key == "animalContestEmilyBoomBoxStop")
            {
                location.getTemporarySpriteByID(6).pulse=false;
                location.getTemporarySpriteByID(6).scale = 4f;
            }
            else if (key == "animalContestEmilyParrotDance")
            {
                if (location.getTemporarySpriteByID(5858585) is EmilysParrot emilysParrot)
                {
                    Vector2 position = emilysParrot.initialPosition;
                    location.removeTemporarySpritesWithID(5858585);
                    location.TemporarySprites.Add(new EmilysParrotDancer(position));
                }
            }
            else if (key == "animalContestEmilyParrotStopDance")
            {
                if (location.getTemporarySpriteByID(5858586) is EmilysParrotDancer emilysParrotDancer)
                {
                    Vector2 position = emilysParrotDancer.initialPosition;
                    location.removeTemporarySpritesWithID(5858586);
                    location.TemporarySprites.Add(new EmilysParrot(position));
                }
            }
            else if (key == "animalContestMaruRobot")
            {
                location.TemporarySprites.Add(new TemporaryAnimatedSprite(townInterior,
                    new Microsoft.Xna.Framework.Rectangle(448, 576, 16, 16), 9999f, 1, 999,
                    new Vector2(31f, 66f) * (float)Game1.tileSize, false, false,
                    0.000001f, 0.0f, Color.White,
                    (float)Game1.pixelZoom, 0.0f, 0.0f, 0.0f, false));
            }
            else if (key == "animalContestAbigailSlime")
            {
                __instance.actors.Add(new EventGreenSlime(new Vector2(31f, 66f) * (float)Game1.tileSize, 5));
            }
            else if (key == "animalContestWillyCrab")
            {
                location.TemporarySprites.Add(new TemporaryAnimatedSprite(looseSprites_temporary_sprites_1,
                    new Microsoft.Xna.Framework.Rectangle(259, 146, 18, 18), 250f, 3, 99999,
                    new Vector2(31f, 66f) * (float)Game1.tileSize + CrabOffset * 4f, false, false,
                    0.000001f, 0.0f, Color.White,
                    (float)Game1.pixelZoom, 0.0f, 0.0f, 0.0f, false)
                {
                    pingPong = true
                });
            }
            else if (key == "animalContestMarnieWinning")
            {
                location.TemporarySprites.Add(new TemporaryAnimatedSprite(looseSprites_cursorsName, new Microsoft.Xna.Framework.Rectangle(558, 1425, 20, 26), 400f, 3, 99999, new Vector2(24f, 65f) * 64f, false, false, 0.416f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, false)
                {
                    pingPong = true
                });
            }
            else if (key == "animalContestEnding")
            {
                AnimalContestController.EndEvent(FarmerLoader.FarmerData.AnimalContestData.Last());
            }

        }

        public static void skipEvent(Event __instance)
        {
            AnimalContestItem lastAnimalContest = FarmerLoader.FarmerData.AnimalContestData.LastOrDefault();
            if (lastAnimalContest != null && lastAnimalContest.EventId == __instance.id)
            {
                AnimalContestController.EndEvent(lastAnimalContest);
            }
        }

        private static void addTemporarySprite(GameLocation location, String textureName, Rectangle sourceRectangle , float x, float y)
        {
            location.TemporarySprites.Add(new TemporaryAnimatedSprite(textureName,
                sourceRectangle, 9999f, 1, 999
                , new Vector2(x, y) * (float)Game1.tileSize, false, false,
                0.0000001f, 0.0f, Color.White,
                (float)Game1.pixelZoom, 0.0f, 0.0f, 0.0f, false));
        }
    }
}
