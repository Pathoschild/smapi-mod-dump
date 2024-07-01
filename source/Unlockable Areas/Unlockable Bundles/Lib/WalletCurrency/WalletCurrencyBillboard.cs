/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Unlockable_Bundles.ModEntry;

namespace Unlockable_Bundles.Lib.WalletCurrency
{
    public class WalletCurrencyBillboard
    {
        public static bool IsActive = false;
        public static WalletCurrencyModel Currency;
        public static int Value;
        public static AnimatedTexture ItemTexture;
        public static float TimeToLive;
        public static bool ForceShow;
        public static float Height;

        private static MoneyDial MoneyDial;

        public static void Initialize()
        {
            Helper.Events.Display.Rendered += Draw; ;
            Helper.Events.Player.Warped += Player_Warped;
            Helper.Events.Display.MenuChanged += Display_MenuChanged;
        }

        private static void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is null)
                StopForceShow();
        }

        private static void Player_Warped(object sender, WarpedEventArgs e)
            => StopForceShow();

        public static void ShowCurrency(WalletCurrencyModel currency, long relevantPlayer, bool drawInFront = false)
        {
            var value = ModData.getWalletCurrency(currency.Id, relevantPlayer);
            Register(currency, value, value, false);
            ForceShow = true;
        }

        public static void StopForceShow()
        {
            ForceShow = false;
            TimeToLive = Math.Min(0.8f, TimeToLive);
        }

        public static void Register(WalletCurrencyModel currency, int oldValue, int newValue, bool playSound = true)
        {
            if (currency.PickupSound != "" && playSound)
                Game1.playSound(currency.PickupSound);

            TimeToLive = 5f;

            if (IsActive && Currency.Id == currency.Id) {
                Value = newValue;

            } else {
                IsActive = true;
                Currency = currency;
                Value = newValue;
                MoneyDial = new MoneyDial(currency.BillboardDigits, playSound: currency.PlayMoneyRollSound);
                MoneyDial.currentValue = oldValue;
                var texture = Helper.GameContent.Load<Texture2D>(currency.BillboardTexture);
                ItemTexture = new AnimatedTexture(texture, currency.BillboardAnimation, currency.BillboardTextureSize, currency.BillboardTextureSize);
            }
        }

        private static void Draw(object sender, RenderedEventArgs e)
        {
            if (!IsActive)
                return;

            Update(Game1.currentGameTime);
            Vector2 draw_position = new Vector2(16f, (int)Utility.Lerp(-26f, 0f, Height) * 4);

            ItemTexture.update(Game1.currentGameTime);
            DrawBillboard(e.SpriteBatch, draw_position);

            if (Height == 0 && TimeToLive <= 0f && !ForceShow)
                IsActive = false;
        }

        public static void DrawBillboard(SpriteBatch b, Vector2 drawPosition)
        {
            var itemTexture = ItemTexture.Texture;
            var sourceRectangle = ItemTexture.getOffsetRectangle();
            var itemTextureScale = 64f / sourceRectangle.Width;

            float xOffset = 132;
            b.Draw(Game1.mouseCursors2, drawPosition, new Rectangle(48, 176, 33, 26), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0f);
            for (int i = 2; i < Currency.BillboardDigits; i++) {
                b.Draw(Game1.mouseCursors2, drawPosition + new Vector2(xOffset, 0), new Rectangle(81, 176, 6, 26), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0f);
                xOffset += 24f;
            }
            b.Draw(Game1.mouseCursors2, drawPosition + new Vector2(xOffset, 0), new Rectangle(87, 176, 14, 26), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0f);

            MoneyDial?.draw(b, drawPosition + new Vector2(+108f, 40f), Value);
            b.Draw(itemTexture, drawPosition + new Vector2(4f, 6f) * 4f, sourceRectangle, Color.White, 0f, Vector2.Zero, itemTextureScale, SpriteEffects.None, 0f);
        }

        public static void Update(GameTime time)
        {
            if (TimeToLive > 0f)
                TimeToLive -= (float)time.ElapsedGameTime.TotalSeconds;

            if (TimeToLive > 0f || ForceShow)
                Height += (float)time.ElapsedGameTime.TotalSeconds / 0.5f;
            else
                Height -= (float)time.ElapsedGameTime.TotalSeconds / 0.5f;

            Height = Utility.Clamp(Height, 0f, 1f);
        }
    }
}
