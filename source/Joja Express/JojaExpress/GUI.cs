/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ofts-cqm/SDV_JojaExpress
**
*************************************************/

using StardewValley;
using StardewValley.GameData.Shops;
using StardewValley.Internal;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using Microsoft.Xna.Framework;

namespace JojaExpress
{
    public class GUI
    {
        public static Rectangle[] boxes = new Rectangle[] {
            new(48, 96, 24, 24),
            new(72, 96, 24, 24),
            new(96, 96, 24, 24),
        };
        public static int tick = 0, index = 0;
        public static PerScreen<int> absTick = new();
        public static Texture2D birdTexture;
        public static PerScreen<bool> showAnimation = new(), droped = new();
        public static PerScreen<Vector2> target = new(), current = new();
        public static PerScreen<GameLocation> targetLocation = new();

        public static void openMenu(string shopId, Func<ISalable, Farmer, int, bool> onPurchase, Func<ISalable, string> getPostFix)
        {
            if (!DataLoader.Shops(Game1.content).TryGetValue(shopId, out var value)) return;

            ShopOwnerData[] source = ShopBuilder.GetCurrentOwners(value).ToArray();
            ShopOwnerData? ownerData = source.FirstOrDefault((ShopOwnerData p) => p.Type == ShopOwnerType.AnyOrNone) ?? source.FirstOrDefault((ShopOwnerData p) => p.Type == ShopOwnerType.AnyOrNone);

            JojaShopMenu menu = new(shopId, value, ownerData, onPurchase, getPostFix);
            menu.searchBox.Selected = false;
            Game1.activeClickableMenu = menu;
        }

        public static string getPostFixForItem(ISalable item)
        {
            if (ModEntry.tobeReceived.Last().TryGetValue(item.QualifiedItemId, out int amt))
                return ModEntry.postfix.Tokens(new Dictionary<string, int>() { { "count", amt } }).ToString();
            else return "";
        }

        public static string getPostFixForLocalItem(ISalable item)
        {
            if (ModEntry.localReceived.TryGetValue(item.QualifiedItemId, out int amt))
                return ModEntry.postfix.Tokens(new Dictionary<string, int>() { { "count", amt } }).ToString();
            else return "";
        }

        public static void dropPackage(SpriteBatch b)
        {
            tick++;
            absTick.Value++;
            if (tick % 10 == 0) { index++; index %= 3; }
            if (tick == 60)
            {
                StardewValley.Object obj = new("ofts.jojaExp.item.package.local", 1);
                foreach (KeyValuePair<string, int> p in ModEntry.localReceived)
                {
                    obj.modData.Add(p.Key, p.Value.ToString());
                }
                targetLocation.Value.debris.Add(Game1.createItemDebris(obj, target.Value, 0));
            }
            if (tick == 120) droped.Value = true;

            if (Game1.currentLocation == targetLocation.Value)
                b.Draw(birdTexture,
                new Vector2(current.Value.X - Game1.viewport.X, current.Value.Y - Game1.viewport.Y
                + (float)(Math.Sin(absTick.Value / 10) * 16)),
                boxes[index], Color.White, 0, Vector2.Zero, 4f, SpriteEffects.None, 2.8f);
        }

        public static void drawBird(object? sender, RenderedWorldEventArgs e)
        {
            if (current.Value.X < target.Value.X && !droped.Value)
            {
                dropPackage(e.SpriteBatch);
                return;
            }

            if (!showAnimation.Value) return;
            tick++;
            absTick.Value++;
            if (tick >= 10) { tick = 0; index++; index %= 3; }

            if (Game1.currentLocation == targetLocation.Value)
                e.SpriteBatch.Draw(birdTexture,
                    new Vector2(current.Value.X - Game1.viewport.X, current.Value.Y - Game1.viewport.Y
                    + (float)(Math.Sin(absTick.Value / 10) * 16)),
                    boxes[index], Color.White, 0, Vector2.Zero, 4f, SpriteEffects.None, 2.8f);

            current.Value = new(current.Value.X - 6.4f, current.Value.Y);

            if (current.Value.X < target.Value.X - Game1.viewport.Width) showAnimation.Value = false;
        }

        public static void checkUI(object? sender, MenuChangedEventArgs e)
        {
            if (e.OldMenu is JojaShopMenu shop && shop.ShopId == "ofts.JojaExp.jojaLocal" && ModEntry.localReceived.Count > 0)
            {
                sendPackage(Game1.player);
            }
        }

        public static void sendPackage(Farmer who)
        {
            if (showAnimation.Value)
            {
                StardewValley.Object obj = new("ofts.jojaExp.item.package.local", 1);
                foreach (KeyValuePair<string, int> p in ModEntry.localReceived)
                {
                    obj.modData.Add(p.Key, p.Value.ToString());
                }
                targetLocation.Value.debris.Add(Game1.createItemDebris(obj, target.Value, 0));
            }

            targetLocation.Value = who.currentLocation;
            target.Value = new(who.Position.X, who.Position.Y - Game1.tileSize);
            current.Value = new(who.Position.X + Game1.viewport.Width, who.Position.Y - Game1.tileSize);
            showAnimation.Value = true;
            droped.Value = false;
            absTick.Value = 0;
        }
    }
}
