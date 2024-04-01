/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tbonetomtom/StardewMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace MoneyManagementMod
{
    public class MoneyMenu : IClickableMenu
    {
        private readonly PublicMoney publicMoney;
        private Vector2 position;
        public int moneyShakeTimer;

        public MoneyMenu(PublicMoney publicMoney, int X, int Y)
        {
            position = new Vector2(X, Y);
            this.publicMoney = publicMoney;

        }

        public void DrawMoneyBox(SpriteBatch b, int overrideX = -1, int overrideY = -1)
        {
            this.publicMoney.Draw(b, ((overrideY != -1) ? new Vector2((overrideX == -1) ? this.position.X : ((float)overrideX), overrideY - 172) : this.position) + new Vector2(68 + ((this.moneyShakeTimer > 0) ? Game1.random.Next(-3, 4) : 0), 196 + ((this.moneyShakeTimer > 0) ? Game1.random.Next(-3, 4) : 0)), publicMoney.PublicBal);
            if (this.moneyShakeTimer > 0)
            {
                this.moneyShakeTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
            }
        }
        public override void draw(SpriteBatch b)
        {
            this.DrawMoneyBox(b, (int)position.X, (int)position.Y);
        }
    }
}
