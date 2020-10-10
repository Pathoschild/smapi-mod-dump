using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomyMod.Helpers;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;

namespace EconomyMod.Interface.Submenu
{
    public class LoanPage : Page
    {
        public LoanPage(UIFramework ui, TaxationService taxation) : base(ui)
        {
            LoanButton = new ClickableComponent(InterfaceHelper.GetButtonSizeForPage(this), "", "_____________");

            for (int i = 0; i < 7; ++i)
                Slots.Add(new ClickableComponent(
                    new Rectangle(
                        xPositionOnScreen + Game1.tileSize / 4,
                        yPositionOnScreen + Game1.tileSize * 5 / 4 + Game1.pixelZoom + i * (height - Game1.tileSize * 2) / 7,
                        width - Game1.tileSize / 2,
                        (height - Game1.tileSize * 2) / 7 + Game1.pixelZoom),
                    i.ToString()));

            Draw = () =>
            {

                int currentItemIndex = 0;


                for (int i = 0; i < Slots.Count; ++i)
                {
                    InterfaceHelper.Draw(Slots[i].bounds);
                    if (currentItemIndex >= 0 &&
                        currentItemIndex + i < Elements.Count)
                    {
                        Elements[currentItemIndex + i].draw(Game1.spriteBatch, Slots[i].bounds.X, Slots[i].bounds.Y);
                    }
                }

                if (taxation.State.PendingTaxAmount != 0)
                {
                    IClickableMenu.drawTextureBox(Game1.spriteBatch, Game1.mouseCursors, new Rectangle(432, 439, 9, 9), LoanButton.bounds.X, LoanButton.bounds.Y, LoanButton.bounds.Width, LoanButton.bounds.Height, (LoanButton.scale > 0f) ? Color.Wheat : Color.White, 4f);
                    var btnPosition = new Vector2(LoanButton.bounds.Center.X, LoanButton.bounds.Center.Y + 4) - Game1.dialogueFont.MeasureString("Loan Funds - Pelican Town 10000g") / 2f;
                    Utility.drawTextWithShadow(Game1.spriteBatch, "Loan Funds - Pelican Town 10000g", Game1.dialogueFont, btnPosition, Game1.textColor, 1f, -1f, -1, -1, 0f);

                    InterfaceHelper.Draw(LoanButton.bounds, center: true);
                    InterfaceHelper.Draw(btnPosition, InterfaceHelper.InterfaceHelperType.Red);
                }
            };
            this.LeftClickAction += Leftclick;
        }

        private void Leftclick(object sender, Coordinate e)
        {
        }

        public ClickableComponent LoanButton { get; }
    }
}
