using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace AnimalHusbandryMod.common
{
    public class EventsOverrides
    {
        private static String spring_outdoorsTileSheetName = "maps/spring_outdoorsTileSheet";
        private static String summer_outdoorsTileSheetName = "maps/summer_outdoorsTileSheet";
        private static String fall_outdoorsTileSheetName = "maps/fall_outdoorsTileSheet";
        private static String winter_outdoorsTileSheetName = "maps/winter_outdoorsTileSheet";

        public static void addSpecificTemporarySprite(ref string key, ref GameLocation location)
        {
            if (key == "animalCompetition")
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
                    (float)(59 * Game1.tileSize) / 10000f, 0.0f, Color.White,
                    (float) Game1.pixelZoom, 0.0f, 0.0f, 0.0f, false));

                //Outdoors
                Rectangle singleFeed = new Rectangle(304, 144, 16, 32);
                Rectangle doubleFeed = new Rectangle(320,128, 32, 32);
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
                addTemporarySprite(location, outdoorsTextureName, water,32,62);
                addTemporarySprite(location, outdoorsTextureName, singleFeed, 34, 62);
                addTemporarySprite(location, outdoorsTextureName, create, 23, 62);

                addTemporarySprite(location, DataLoader.LooseSpritesName, TopLeft, 26, 64);
                addTemporarySprite(location, DataLoader.LooseSpritesName, TopCenter, 27, 64);
                addTemporarySprite(location, DataLoader.LooseSpritesName, TopCenter, 28, 64);
                addTemporarySprite(location, DataLoader.LooseSpritesName, TopCenter, 29, 64);
                addTemporarySprite(location, DataLoader.LooseSpritesName, TopCenter, 30, 64);
                addTemporarySprite(location, DataLoader.LooseSpritesName, TopRight, 31, 64);
                                                                    
                addTemporarySprite(location, DataLoader.LooseSpritesName, TopLeft, 25, 65);
                addTemporarySprite(location, DataLoader.LooseSpritesName, LeftUp, 26, 65);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 27, 65);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 28, 65);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 29, 65);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 30, 65);
                addTemporarySprite(location, DataLoader.LooseSpritesName, RightUp, 31, 65);
                addTemporarySprite(location, DataLoader.LooseSpritesName, TopRight, 32, 65);
                                                                    
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterLeft, 25, 66);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 26, 66);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 27, 66);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 28, 66);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 29, 66);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 30, 66);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 31, 66);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterRight, 32, 66);
                                                                    
                addTemporarySprite(location, DataLoader.LooseSpritesName, BottonLeft, 25, 67);
                addTemporarySprite(location, DataLoader.LooseSpritesName, LeftDown, 26, 67);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 27, 67);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 28, 67);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 29, 67);
                addTemporarySprite(location, DataLoader.LooseSpritesName, CenterCenter, 30, 67);
                addTemporarySprite(location, DataLoader.LooseSpritesName, RightDown, 31, 67);
                addTemporarySprite(location, DataLoader.LooseSpritesName, BottonRight, 32, 67);
                                                                    
                addTemporarySprite(location, DataLoader.LooseSpritesName, BottonLeft, 26, 68);
                addTemporarySprite(location, DataLoader.LooseSpritesName, BottonCenter, 27, 68);
                addTemporarySprite(location, DataLoader.LooseSpritesName, BottonCenter, 28, 68);
                addTemporarySprite(location, DataLoader.LooseSpritesName, BottonCenter, 29, 68);
                addTemporarySprite(location, DataLoader.LooseSpritesName, BottonCenter, 30, 68);
                addTemporarySprite(location, DataLoader.LooseSpritesName, BottonRight, 31, 68);
            }
        }

        private static void addTemporarySprite(GameLocation location, String textureName, Rectangle sourceRectangle , float x, float y)
        {
            location.TemporarySprites.Add(new TemporaryAnimatedSprite(textureName,
                sourceRectangle, 9999f, 1, 999
                , new Vector2(x, y) * (float)Game1.tileSize, false, false,
                (float)(y * Game1.tileSize) / 10000f, 0.0f, Color.White,
                (float)Game1.pixelZoom, 0.0f, 0.0f, 0.0f, false));
        }
    }
}
