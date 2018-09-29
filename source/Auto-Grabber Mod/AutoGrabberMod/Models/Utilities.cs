using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using xTile.Tiles;

namespace AutoGrabberMod.Models
{
    public class Utilities
    {
        public static IMonitor Monitor { get; set; }
        public static IModHelper Helper { get; set; }
        public static Type[] FeatureTypes { get; set; }
        public static ModConfig Config;

        public static int TileUnderMouseX
        {
            get { return (Game1.getMouseX() + Game1.viewport.X) / Game1.tileSize; }
        }

        public static int TileUnderMouseY
        {
            get { return (Game1.getMouseY() + Game1.viewport.Y) / Game1.tileSize; }
        }

        public static bool IsGrabbableCoop(StardewValley.Object obj)
        {
            if (obj.bigCraftable.Value)
            {
                return obj.Name.Contains("Slime Ball");
            }

            return (obj.Name.Contains("Egg") || obj.Name.Contains("Wool") || obj.Name.Contains("Foot") || obj.Name.Contains("Feather"));
        }

        public static bool IsGrabbableWorld(StardewValley.Object obj)
        {
            if (obj.bigCraftable.Value)
            {
                return false;
            }
            switch (obj.ParentSheetIndex)
            {
                case 16: // Wild Horseradish
                case 18: // Daffodil
                case 20: // Leek
                case 22: // Dandelion
                case 430: // Truffle
                case 399: // Spring Onion
                case 257: // Morel
                case 404: // Common Mushroom
                case 296: // Salmonberry
                case 396: // Spice Berry
                case 398: // Grape
                case 402: // Sweet Pea
                case 420: // Red Mushroom
                case 259: // Fiddlehead Fern
                case 406: // Wild Plum
                case 408: // Hazelnut
                case 410: // Blackberry
                case 281: // Chanterelle
                case 412: // Winter Root
                case 414: // Crystal Fruit
                case 416: // Snow Yam
                case 418: // Crocus
                case 283: // Holly
                case 392: // Nautilus Shell
                case 393: // Coral
                case 397: // Sea Urchin
                case 394: // Rainbow Shell
                case 372: // Clam
                case 718: // Cockle
                case 719: // Mussel
                case 723: // Oyster
                case 78: // Cave Carrot
                case 90: // Cactus Fruit
                case 88: // Coconut
                    return true;
            }
            return false;
        }

        public static bool IsCropFlower(Crop crop)
        {
            switch (crop.indexOfHarvest.Value)
            {
                case 421:  // sunflower
                case 593: // summer spangle
                case 595:  // fairy rose
                case 591: // tulip
                case 597:  // blue jazz
                case 376:  // poppy
                case 0:
                    return true;
            }
            return false;
        }

        public static IEnumerable<Vector2> GetNearbyTiles(Vector2 tile, int range, int current = 1)
        {           
            for (int y = (int)tile.Y - current; y < tile.Y + current + 1; y++)
            {
                if(y == (tile.Y - current) || y == (tile.Y + current))
                {
                    for (int x = (int)tile.X - current; x < tile.X + current + 1; x++) yield return new Vector2(x, y);
                }
                else
                {
                    yield return new Vector2(tile.X - current, y);
                    yield return new Vector2(tile.X + current, y);
                }                
            }
            if (current < range) foreach (var vector in GetNearbyTiles(tile, range, current + 1)) yield return vector;
        }

        public static IEnumerable<KeyValuePair<Vector2, HoeDirt>> GetDirts(GameLocation location, Vector2[] tiles)
        {
            foreach (var tile in tiles)
            {
                if (location.terrainFeatures.TryGetValue(tile, out TerrainFeature dirt) && dirt is HoeDirt)
                {
                    yield return new KeyValuePair<Vector2, HoeDirt>(tile, dirt as HoeDirt);
                }
                else if (location.Objects.TryGetValue(tile, out StardewValley.Object pot) && pot is IndoorPot)
                {
                    yield return new KeyValuePair<Vector2, HoeDirt>(tile, (pot as IndoorPot).hoeDirt.Value);
                }
            }
        }        

        public static IEnumerable<Vector2> GetLocationObjectTiles(GameLocation location)
        {
            foreach (Vector2 tile in location.Objects.Keys)
            {
                yield return tile;
            }

            foreach (Vector2 tile in location.terrainFeatures.Keys)
            {
                yield return tile;
            }
        }

        public static bool IsFertilizer(StardewValley.Object obj)
        {
            switch (obj.ParentSheetIndex)
            {
                case HoeDirt.fertilizerLowQuality:
                case HoeDirt.fertilizerHighQuality:
                case HoeDirt.waterRetentionSoil:
                case HoeDirt.waterRetentionSoilQUality:
                case HoeDirt.speedGro:
                case HoeDirt.superSpeedGro:
                    return true;
            }
            return false;
        }

        public static bool IsFlower(int index)
        {
            switch (index)
            {
                case 421: // sunflower
                case 593: // summer spangle
                case 595: // fairy rose
                case 591: // tulip
                case 597: // blue jazz
                case 376: // poppy
                case 0: return true;
            }
            return false;
        }

        public static int GetSprinklerCapacity(StardewValley.Object obj)
        {
            int capacity = 0;
            switch (obj.ParentSheetIndex)
            {
                case 599: capacity = 4; break;
                case 621: capacity = 8; break;
                case 645: capacity = 24; break;
            }

            if (obj.Stack != 0) capacity *= obj.Stack;

            return capacity;
        }

        public static void GainExperience(int skill, int xp)
        {
            Game1.player.gainExperience(skill, xp);
        }

        public static IEnumerable<GameLocation> GetAllLocations()
        {
            foreach (GameLocation location in Game1.locations)
            {
                // current location
                yield return location;

                // buildings
                if (location is BuildableGameLocation buildableLocation)
                {
                    foreach (Building building in buildableLocation.buildings)
                    {
                        if (building.indoors != null)
                            yield return building.indoors;
                    }
                }
            }
        }

        public static void DrawTextBox(int x, int y, SpriteFont font, string message, int align = 0, float colorIntensity = 1F)
        {
            SpriteBatch spriteBatch = Game1.spriteBatch;

            Vector2 bounds = font.MeasureString(message);
            int width = (int)bounds.X + Game1.tileSize / 2;
            int height = (int)font.MeasureString(message).Y + Game1.tileSize / 3;
            switch (align)
            {
                case 0:
                    IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y, width, height + Game1.tileSize / 16, Color.White * colorIntensity);
                    Utility.drawTextWithShadow(spriteBatch, message, font, new Vector2(x + Game1.tileSize / 4, y + Game1.tileSize / 4), Game1.textColor);
                    break;
                case 1:
                    IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x - width / 2, y, width, height + Game1.tileSize / 16, Color.White * colorIntensity);
                    Utility.drawTextWithShadow(spriteBatch, message, font, new Vector2(x + Game1.tileSize / 4 - width / 2, y + Game1.tileSize / 4), Game1.textColor);
                    break;
                case 2:
                    IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x - width, y, width, height + Game1.tileSize / 16, Color.White * colorIntensity);
                    Utility.drawTextWithShadow(spriteBatch, message, font, new Vector2(x + Game1.tileSize / 4 - width, y + Game1.tileSize / 4), Game1.textColor);
                    break;
            }
        }


        public static IEnumerable<FarmAnimal> GetFarmAnimals(GameLocation location)
        {
            if (location is Farm farm)
            {
                foreach (var animal in farm.animals)
                {
                    foreach (var kv in animal) yield return kv.Value;
                }
            }
            else if (location is AnimalHouse house)
            {
                foreach (var animal in house.animals)
                {
                    foreach (var kv in animal) yield return kv.Value;
                }
            }
        }

        public static void drawTextureBox(SpriteBatch b, int x, int y, int width, int height, Color color)
        {
            IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y, width, height, color, 1f, true);
        }

        public static void drawTextureBox(SpriteBatch b, Texture2D texture, Rectangle sourceRect, int x, int y, int width, int height, Color color, float scale = 1f, bool drawShadow = true)
        {
            int num = sourceRect.Width / 3;
            if (drawShadow)
            {
                b.Draw(texture, new Vector2((float)(x + width - (int)((double)num * (double)scale) - 8), (float)(y + 8)), new Rectangle?(new Rectangle(sourceRect.X + num * 2, sourceRect.Y, num, num)), Color.Black * 0.4f, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.77f);
                b.Draw(texture, new Vector2((float)(x - 8), (float)(y + height - (int)((double)num * (double)scale) + 8)), new Rectangle?(new Rectangle(sourceRect.X, num * 2 + sourceRect.Y, num, num)), Color.Black * 0.4f, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.77f);
                b.Draw(texture, new Vector2((float)(x + width - (int)((double)num * (double)scale) - 8), (float)(y + height - (int)((double)num * (double)scale) + 8)), new Rectangle?(new Rectangle(sourceRect.X + num * 2, num * 2 + sourceRect.Y, num, num)), Color.Black * 0.4f, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.77f);
                b.Draw(texture, new Rectangle(x + (int)((double)num * (double)scale) - 8, y + 8, width - (int)((double)num * (double)scale) * 2, (int)((double)num * (double)scale)), new Rectangle?(new Rectangle(sourceRect.X + num, sourceRect.Y, num, num)), Color.Black * 0.4f, 0.0f, Vector2.Zero, SpriteEffects.None, 0.77f);
                b.Draw(texture, new Rectangle(x + (int)((double)num * (double)scale) - 8, y + height - (int)((double)num * (double)scale) + 8, width - (int)((double)num * (double)scale) * 2, (int)((double)num * (double)scale)), new Rectangle?(new Rectangle(sourceRect.X + num, num * 2 + sourceRect.Y, num, num)), Color.Black * 0.4f, 0.0f, Vector2.Zero, SpriteEffects.None, 0.77f);
                b.Draw(texture, new Rectangle(x - 8, y + (int)((double)num * (double)scale) + 8, (int)((double)num * (double)scale), height - (int)((double)num * (double)scale) * 2), new Rectangle?(new Rectangle(sourceRect.X, num + sourceRect.Y, num, num)), Color.Black * 0.4f, 0.0f, Vector2.Zero, SpriteEffects.None, 0.77f);
                b.Draw(texture, new Rectangle(x + width - (int)((double)num * (double)scale) - 8, y + (int)((double)num * (double)scale) + 8, (int)((double)num * (double)scale), height - (int)((double)num * (double)scale) * 2), new Rectangle?(new Rectangle(sourceRect.X + num * 2, num + sourceRect.Y, num, num)), Color.Black * 0.4f, 0.0f, Vector2.Zero, SpriteEffects.None, 0.77f);
                b.Draw(texture, new Rectangle((int)((double)num * (double)scale / 2.0) + x - 8, (int)((double)num * (double)scale / 2.0) + y + 8, width - (int)((double)num * (double)scale), height - (int)((double)num * (double)scale)), new Rectangle?(new Rectangle(num + sourceRect.X, num + sourceRect.Y, num, num)), Color.Black * 0.4f, 0.0f, Vector2.Zero, SpriteEffects.None, 0.77f);
            }
            var layerDepth = 0.9f;
            b.Draw(texture, new Rectangle((int)((double)num * (double)scale) + x, (int)((double)num * (double)scale) + y, width - (int)((double)num * (double)scale * 2.0), height - (int)((double)num * (double)scale * 2.0)), new Rectangle?(new Rectangle(num + sourceRect.X, num + sourceRect.Y, num, num)), color, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
            b.Draw(texture, new Vector2((float)x, (float)y), new Rectangle?(new Rectangle(sourceRect.X, sourceRect.Y, num, num)), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
            b.Draw(texture, new Vector2((float)(x + width - (int)((double)num * (double)scale)), (float)y), new Rectangle?(new Rectangle(sourceRect.X + num * 2, sourceRect.Y, num, num)), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
            b.Draw(texture, new Vector2((float)x, (float)(y + height - (int)((double)num * (double)scale))), new Rectangle?(new Rectangle(sourceRect.X, num * 2 + sourceRect.Y, num, num)), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
            b.Draw(texture, new Vector2((float)(x + width - (int)((double)num * (double)scale)), (float)(y + height - (int)((double)num * (double)scale))), new Rectangle?(new Rectangle(sourceRect.X + num * 2, num * 2 + sourceRect.Y, num, num)), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
            b.Draw(texture, new Rectangle(x + (int)((double)num * (double)scale), y, width - (int)((double)num * (double)scale) * 2, (int)((double)num * (double)scale)), new Rectangle?(new Rectangle(sourceRect.X + num, sourceRect.Y, num, num)), color, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
            b.Draw(texture, new Rectangle(x + (int)((double)num * (double)scale), y + height - (int)((double)num * (double)scale), width - (int)((double)num * (double)scale) * 2, (int)((double)num * (double)scale)), new Rectangle?(new Rectangle(sourceRect.X + num, num * 2 + sourceRect.Y, num, num)), color, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
            b.Draw(texture, new Rectangle(x, y + (int)((double)num * (double)scale), (int)((double)num * (double)scale), height - (int)((double)num * (double)scale) * 2), new Rectangle?(new Rectangle(sourceRect.X, num + sourceRect.Y, num, num)), color, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
            b.Draw(texture, new Rectangle(x + width - (int)((double)num * (double)scale), y + (int)((double)num * (double)scale), (int)((double)num * (double)scale), height - (int)((double)num * (double)scale) * 2), new Rectangle?(new Rectangle(sourceRect.X + num * 2, num + sourceRect.Y, num, num)), color, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
        }

        public static bool IsTilePassable(xTile.Dimensions.Location tileLocation, xTile.Dimensions.Rectangle viewport, GameLocation location)
        {
            xTile.ObjectModel.PropertyValue propertyValue = (xTile.ObjectModel.PropertyValue)null;
            Tile tile1 = location.map.GetLayer("Back").PickTile(new xTile.Dimensions.Location(tileLocation.X * 64, tileLocation.Y * 64), viewport.Size);
            tile1?.TileIndexProperties.TryGetValue("Passable", out propertyValue);
            Tile tile2 = location.map.GetLayer("Buildings").PickTile(new xTile.Dimensions.Location(tileLocation.X * 64, tileLocation.Y * 64), viewport.Size);
            if (propertyValue == null && tile2 == null)
                return tile1 != null;
            return false;
        }

        public static bool MakeHoeDirt(int x, int y, GameLocation location)
        {
            var tile = new Vector2(x, y);
            if (location.isTileOnMap(tile)
                && (location.doesTileHaveProperty(x, y, "Diggable", "Back") != null && !location.isTileOccupied(tile) && IsTilePassable(new xTile.Dimensions.Location(x, y), Game1.viewport, location)))
            {
                location.terrainFeatures.Add(tile, (TerrainFeature)new HoeDirt(!Game1.isRaining || !(bool)((NetFieldBase<bool, NetBool>)location.isOutdoors) ? 0 : 1));
                return true;
            }
            return false;
        }

        public static void DigUpArtifactSpot(int xLocation, int yLocation, GameLocation Location)
        {
            Farmer who = Game1.player;
            Random random = new Random(xLocation * 2000 + yLocation + (int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed);
            int objectIndex = -1;
            foreach (KeyValuePair<int, string> keyValuePair in (IEnumerable<KeyValuePair<int, string>>)Game1.objectInformation)
            {
                string[] strArray1 = keyValuePair.Value.Split('/');
                if (strArray1[3].Contains("Arch"))
                {
                    string[] strArray2 = strArray1[6].Split(' ');
                    int index = 0;
                    while (index < strArray2.Length)
                    {
                        if (strArray2[index].Equals((string)((NetFieldBase<string, NetString>)Location.name)) && random.NextDouble() < Convert.ToDouble(strArray2[index + 1], (IFormatProvider)CultureInfo.InvariantCulture))
                        {
                            objectIndex = keyValuePair.Key;
                            break;
                        }
                        index += 2;
                    }
                }
                if (objectIndex != -1)
                    break;
            }
            if (random.NextDouble() < 0.2 && !(Location is Farm))
                objectIndex = 102;
            if (objectIndex == 102 && who.archaeologyFound.ContainsKey(102) && who.archaeologyFound[102][0] >= 21)
                objectIndex = 770;
            if (objectIndex != -1)
            {
                Game1.createObjectDebris(objectIndex, xLocation, yLocation, who.UniqueMultiplayerID, Location);
                who.gainExperience(5, 25);
            }
            else if (Game1.currentSeason.Equals("winter") && random.NextDouble() < 0.5 && !(Location is Desert))
            {
                if (random.NextDouble() < 0.4)
                    Game1.createObjectDebris(416, xLocation, yLocation, who.UniqueMultiplayerID, Location);
                else
                    Game1.createObjectDebris(412, xLocation, yLocation, who.UniqueMultiplayerID, Location);
            }
            else
            {
                Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");
                if (!dictionary.ContainsKey((string)((NetFieldBase<string, NetString>)Location.name)))
                    return;
                string[] strArray = dictionary[(string)((NetFieldBase<string, NetString>)Location.name)].Split('/')[8].Split(' ');
                if (strArray.Length == 0 || strArray[0].Equals("-1"))
                    return;
                int index1 = 0;
                while (index1 < strArray.Length)
                {
                    if (random.NextDouble() <= Convert.ToDouble(strArray[index1 + 1]))
                    {
                        int index2 = Convert.ToInt32(strArray[index1]);
                        if (Game1.objectInformation.ContainsKey(index2))
                        {
                            if (Game1.objectInformation[index2].Split('/')[3].Contains("Arch") || index2 == 102)
                            {
                                if (index2 == 102 && who.archaeologyFound.ContainsKey(102) && who.archaeologyFound[102][0] >= 21)
                                    index2 = 770;
                                Game1.createObjectDebris(index2, xLocation, yLocation, who.UniqueMultiplayerID, Location);
                                break;
                            }
                        }
                        if (index2 == 330 && who.hasMagnifyingGlass && Game1.random.NextDouble() < 0.11)
                        {
                            StardewValley.Object unseenSecretNote = Location.tryToCreateUnseenSecretNote(who);
                            if (unseenSecretNote != null)
                            {
                                Game1.createItemDebris((Item)unseenSecretNote, new Vector2((float)xLocation + 0.5f, (float)yLocation + 0.5f) * 64f, -1, (GameLocation)null, -1);
                                break;
                            }
                        }
                        Game1.createMultipleObjectDebris(index2, xLocation, yLocation, random.Next(1, 4), who.UniqueMultiplayerID, Location);
                        break;
                    }
                    index1 += 2;
                }
            }
        }
    }
}
