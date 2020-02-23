using System;
using System.Collections.Generic;
using System.Linq;
using JoysOfEfficiency.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using static System.String;
using static StardewValley.Game1;
using Object = StardewValley.Object;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace JoysOfEfficiency.Utils
{
    using Player = Farmer;
    using SVObject = Object;
    internal class Util
    {
        private static ITranslationHelper Translation => InstanceHolder.Translation;
        private static Config Config => InstanceHolder.Config;


        private static int _lastItemIndex;

        public static string GetItemName(int parentSheetIndex)
        {
            return new SVObject(parentSheetIndex, 1).DisplayName;
        }

        public static T FindToolFromInventory<T>(bool fromEntireInventory) where T : Tool
        {
            Player player = Game1.player;
            if (player.CurrentTool is T t)
            {
                return t;
            }
            if (!fromEntireInventory)
                return null;

            foreach (Item item in player.Items)
            {
                if (item is T t2)
                {
                    return t2;
                }
            }
            return null;
        }

        public static float Cap(float f, float min, float max)
        {
            return f < min ? min : (f > max ? max : f);
        }

        

        public static void ShowHudMessage(string message, int duration = 3500)
        {
            HUDMessage hudMessage = new HUDMessage(message, 3)
            {
                noIcon = true,
                timeLeft = duration
            };
            addHUDMessage(hudMessage);
        }

        public static IEnumerable<FarmAnimal> GetAnimalsList(Character player)
        {
            List<FarmAnimal> list = new List<FarmAnimal>();
            switch (player.currentLocation)
            {
                case Farm farm:
                {
                    list.AddRange(farm.animals.Values);
                    break;
                }

                case AnimalHouse house:
                {
                    list.AddRange(house.animals.Values);
                    break;
                }
            }
            return list;
        }

        public static Rectangle Expand(Rectangle rect, int radius)
        {
            return new Rectangle(rect.Left - radius, rect.Top - radius, 2 * radius, 2 * radius);
        }

        public static void DrawSimpleTextbox(SpriteBatch batch, string text, int x, int y, SpriteFont font, object ctx, Item item = null)
        {
            Vector2 stringSize = text == null ? Vector2.Zero : font.MeasureString(text);
            if (x < 0)
            {
                x = 0;
            }
            if (y < 0)
            {
                y = 0;
            }

            if (ctx is OptionsElement)
            {
                y -= 64;
            }
            int rightX = (int)stringSize.X + tileSize / 2 + 8;
            if (item != null)
            {
                rightX += tileSize;
            }
            if (x + rightX > viewport.Width)
            {
                x = viewport.Width - rightX;
            }
            int bottomY = (int)stringSize.Y + 32;
            if (item != null)
            {
                bottomY = (int)(tileSize * 1.7);
            }
            if (bottomY + y > viewport.Height)
            {
                y = viewport.Height - bottomY;
            }
            DrawWindow(x, y, rightX, bottomY);
            if (!IsNullOrEmpty(text))
            {
                Vector2 vector2 = new Vector2(x + tileSize / 4, y + (bottomY - stringSize.Y) / 2 + 8f);
                Utility.drawTextWithShadow(batch, text, font, vector2, Color.Black);
            }
            item?.drawInMenu(batch, new Vector2(x + (int)stringSize.X + 24, y + 16), 1.0f, 1.0f, 0.9f, StackDrawType.Draw_OneInclusive);
        }

        public static void DrawSimpleTextbox(SpriteBatch batch, string text, SpriteFont font, object context, bool isIcon = false, Item item = null, bool isTips = false)
        {
            DrawSimpleTextbox(batch, text, getMouseX() + tileSize / 2, getMouseY() + (isTips ? 0 : (isIcon ? 24 : tileSize) ) + 24, font, context, item);
        }

        public static bool IsThereAnyWaterNear(GameLocation location, Vector2 tileLocation)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Vector2 toCheck = tileLocation + new Vector2(i, j);
                    int x = (int)toCheck.X, y = (int)toCheck.Y;
                    if (location.doesTileHaveProperty(x, y, "Water", "Back") != null || location.doesTileHaveProperty(x, y, "WaterSource", "Back") != null || location is BuildableGameLocation loc2 && loc2.buildings.Where(b => b.occupiesTile(toCheck)).Any(building => building.buildingType.Value == "Well"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static T FindToolFromInventory<T>(Player player, bool findFromInventory) where T : Tool
        {
            if (player.CurrentTool is T t)
            {
                return t;
            }
            return findFromInventory ? player.Items.OfType<T>().FirstOrDefault() : null;
        }

        public static List<T> GetObjectsWithin<T>(int radius, bool ignoreBalancedMode = false) where T : SVObject
        {
            if (!Context.IsWorldReady || currentLocation?.Objects == null)
            {
                return new List<T>();
            }
            if (InstanceHolder.Config.BalancedMode && !ignoreBalancedMode)
            {
                radius = 1;
            }

            GameLocation location = player.currentLocation;
            Vector2 ov = player.getTileLocation();
            List<T> list = new List<T>();
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    Vector2 loc = ov + new Vector2(dx, dy);
                    if (location.Objects.ContainsKey(loc) && location.Objects[loc] is T t)
                    {
                        list.Add(t);
                    }
                }
            }
            return list;
        }

        public static bool IsPlayerIdle()
        {
            if (paused || !shouldTimePass())
            {
                //When game is paused or time is stopped already. it's not idle.
                return false;
            }

            if (player.CurrentToolIndex != _lastItemIndex)
            {
                //When tool index changed, it's not idle.
                _lastItemIndex = player.CurrentToolIndex;
                return false;
            }

            if (player.isMoving() || player.UsingTool)
            {
                //When player is moving or is using tools, it's not idle of cause.
                return false;
            }

            return true;
        }

        /// <summary>
        /// Adds an item into the player inventory.
        /// </summary>-0
        /// <param name="item">The item to push</param>
        /// <returns>Remaining stack number that couldn't be added</returns>
        public static int AddItemIntoInventory(Item item)
        {
            int oldStack = item.Stack;
            int remaining = oldStack;
            for (int i = 0; i < player.MaxItems; i++)
            {
                if (player.Items[i] == null || i >= player.Items.Count)
                {
                    remaining = 0;
                    break;
                }

                Item stack = player.Items[i];

                if (!stack.canStackWith(item))
                {
                    continue;
                }

                int toPut = Math.Min(remaining, stack.maximumStackSize() - stack.Stack);
                if (toPut > 0)
                {
                    remaining -= toPut;
                }

                if (remaining == 0)
                {
                    break;
                }
            }

            player.addItemToInventoryBool(item);
            if (activeClickableMenu is ItemGrabMenu && oldStack - remaining > 0)
            {
                // Draw item pickup hud because addItemToInventoryBool doesn't if ItemGrabMenu is opened.
                Item toShow = item.getOne();
                toShow.Stack = oldStack - remaining;
                DrawItemPickupHud(toShow);
            }

            return remaining;
        }

        public static void DrawCursor()
        {
            if (!options.hardwareCursor)
                spriteBatch.Draw(mouseCursors, new Vector2(getOldMouseX(), getOldMouseY()),
                    getSourceRectForStandardTileSheet(mouseCursors, options.gamepadControls ? 44 : 0, 16, 16),
                    Color.White, 0f, Vector2.Zero, pixelZoom + dialogueButtonScale / 150f, SpriteEffects.None, 1f);
        }

        public static int GetMaxCan(WateringCan can)
        {
            if (can == null)
                return -1;
            switch (can.UpgradeLevel)
            {
                case 0:
                    can.waterCanMax = 40;
                    break;
                case 1:
                    can.waterCanMax = 55;
                    break;
                case 2:
                    can.waterCanMax = 70;
                    break;
                case 3:
                    can.waterCanMax = 85;
                    break;
                case 4:
                    can.waterCanMax = 100;
                    break;
                default:
                    return -1;
            }

            return can.waterCanMax;

        }

        public static void DrawColoredBox(SpriteBatch batch, int x, int y, int width, int height, Color color)
        {
            batch.Draw(fadeToBlackRect, new Rectangle(x, y, width, height), color);
        }

        public static void DrawWindow(int x, int y, int width, int height)
        {
            IClickableMenu.drawTextureBox(spriteBatch, x, y, width, height, Color.White);
        }

        public static Dictionary<Vector2, T> GetFeaturesWithin<T>(int radius) where T : TerrainFeature
        {
            if (!Context.IsWorldReady)
            {
                return new Dictionary<Vector2, T>();
            }

            if (Config.BalancedMode)
            {
                radius = 1;
            }
            GameLocation location = player.currentLocation;
            Vector2 ov = player.getTileLocation();
            Dictionary<Vector2, T> list = new Dictionary<Vector2, T>();

            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    Vector2 loc = ov + new Vector2(dx, dy);
                    if (location.terrainFeatures.ContainsKey(loc) && location.terrainFeatures[loc] is T t)
                    {
                        list.Add(loc, t);
                    }
                }
            }
            return list;
        }

        public static Vector2 GetLocationOf(GameLocation location, SVObject obj)
        {
            return location.Objects.Pairs.Any(kv => kv.Value == obj) ? location.Objects.Pairs.First(kv => kv.Value == obj).Key : new Vector2(-1, -1);
        }

        

        

        
        

        private static void DrawItemPickupHud(Item item)
        {
            Color color = Color.WhiteSmoke;
            string text = item.DisplayName;

            if (item is Object obj2)
            {
                switch (obj2.Type)
                {
                    case "Arch":
                        color = Color.Tan;
                        text += content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1954");
                        break;
                    case "Fish":
                        color = Color.SkyBlue;
                        break;
                    case "Mineral":
                        color = Color.PaleVioletRed;
                        break;
                    case "Vegetable":
                        color = Color.PaleGreen;
                        break;
                    case "Fruit":
                        color = Color.Pink;
                        break;
                }
            }

            addHUDMessage(new HUDMessage(text, Math.Max(1, item.Stack), true, color, item));
        }

        public static int GetTruePrice(Item item)
        {
            return item is SVObject obj ? obj.sellToStorePrice() * 2 : item.salePrice();
        }

        public static void DrawString(SpriteBatch batch, SpriteFont font, ref Vector2 location, string text, Color color, float scale, bool next = false)
        {
            Vector2 stringSize = font.MeasureString(text) * scale;
            batch.DrawString(font, text, location, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            if (next)
            {
                location += new Vector2(stringSize.X, 0);
            }
            else
            {
                location += new Vector2(0, stringSize.Y + 4);
            }
        }

        public static string TryFormat(string str, params object[] args)
        {
            try
            {
                string ret = Format(str, args);
                return ret;
            }
            catch
            {
                // ignored
            }

            return "";
        }
    }
}
