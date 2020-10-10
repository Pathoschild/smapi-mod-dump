/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/millerscout/StardewMillerMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomyMod.Helpers;
using EconomyMod.Interface.PageContent;
using EconomyMod.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace EconomyMod.Interface.Submenu
{
    public class EconomyPage : Page
    {
        private TaxationService taxation;

        private ClickableComponent payButton;
        public EconomyPage(UIFramework ui, Texture2D texture, string Hovertext, TaxationService taxation) : base(ui, texture, Hovertext)
        {
            this.taxation = taxation;
            //Elements.Add(new OptionsElement(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11233")));
            //Elements.Add(new ContentElementText(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11234")));
            Elements.Add(new ContentElementHeaderText(Util.Helper.Translation.Get("BalanceReportText")));
            Elements.Add(new ContentElementText(() => $"{Util.Helper.Translation.Get("CurrentLotValueText")}: {taxation.LotValue.Sum}g"));
            Elements.Add(new ContentElementText(() => $"{Util.Helper.Translation.Get("CurrentTaxBalance")}: {taxation.State?.PendingTaxAmount}g"));

            TaxSchedule scheduledTask = null;
            CustomWorldDate date = null;
            load();
            void load()
            {
                scheduledTask = taxation.State?.ScheduledTax.OrderBy(c => c.DayCount).FirstOrDefault(c => !c.Paid);
                date = scheduledTask?.DayCount.ToWorldDate();
            };

            Elements.Add(new ContentElementText(() =>
            {
                load();

                return scheduledTask != null ? Util.Helper.Translation.Get("NextScheduledTaxText") : Util.Helper.Translation.Get("NoBillsForYouText");
            }));
            Elements.Add(new ContentElementText(() =>
            {
                return scheduledTask != null ? $"{Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5678", date.DayOfMonth, (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es) ? date.Season.GetLocalizedSeason().ToLower() : date.Season.GetLocalizedSeason(), date.Year)} - {scheduledTask.Sum}g" : "";

            }));


            payButton = new ClickableComponent(new Rectangle(xPositionOnScreen + 64, Game1.activeClickableMenu.height + 50, (int)Game1.dialogueFont.MeasureString("_____________").X, 96), "", "_____________");

            for (int i = 0; i < Elements.Count; ++i)
                Slots.Add(new ClickableComponent(
                    new Rectangle(
                        xPositionOnScreen + Game1.tileSize / 4,
                        yPositionOnScreen + Game1.tileSize * 5 / 4 + Game1.pixelZoom + i * (height - Game1.tileSize * 2) / 7,
                        width - Game1.tileSize / 2,
                        (height - Game1.tileSize * 2) / 7 + Game1.pixelZoom),
                    i.ToString()));

            this.Draw = DrawContent;
            this.DrawHover = DrawHoverContent;
            this.LeftClickAction += Leftclick;


        }

        private void Leftclick(object sender, Coordinate coord)
        {
            if (payButton.containsPoint(coord.X, coord.Y) && taxation.State.PendingTaxAmount != 0)
            {
                taxation.PayTaxes();
            }
        }
        private void DrawHoverContent(int x, int y)
        {

            if (payButton.containsPoint(x, y) && taxation.State.PendingTaxAmount != 0)
            {
                if (payButton.scale == 0f)
                {
                    Game1.playSound("Cowboy_gunshot");
                }
                payButton.scale = 1f;
            }
            else
            {
                payButton.scale = 0f;
            }


        }

        private void DrawContent()
        {
            //if (economyPage.currentTab == Convert.ToInt32(sideTabButton.name))
            //{

            int currentItemIndex = 0;
            for (int i = 0; i < Slots.Count; ++i)
            {
                if (currentItemIndex >= 0 &&
                    currentItemIndex + i < Elements.Count)
                {
                    Elements[currentItemIndex + i].draw(Game1.spriteBatch, Slots[i].bounds.X, Slots[i].bounds.Y);
                }
            }
            //}
            InterfaceHelper.Draw(InterfaceHelper.GetSideTabSizeForPage(this, 7));


            //var SidetabRect = InterfaceHelper.GetSideTabSizeForPage(economyPage, economyPage.sideTabs.Count);
            //sideTabButton = new ClickableTextureComponent(string.Concat(0), SidetabRect, "", "Tax payment and report", Util.Helper.Content.Load<Texture2D>($"assets/Interface/sidebarButtonReport.png"), new Rectangle(0, 0, 16, 16), 4f);

            //payButton = new ClickableComponent(new Rectangle(economyPage.xPositionOnScreen + 64, Game1.activeClickableMenu.height + 50, (int)Game1.dialogueFont.MeasureString("_____________").X, 96), "", "_____________");

            //economyPage.OnDraw += (object _, SpriteBatch batch) => Draw(batch);
            //economyPage.OnHover += (object _, Tuple<int, int> coord) => PerformHover(coord.Item1, coord.Item2);
            //economyPage.OnLeftClick += (object _, Tuple<int, int> coord) => ReceiveLeftClick(coord.Item1, coord.Item2);


            DrawPayButton();
        }

        private void DrawPayButton()
        {
            if (taxation.State != null && taxation.State.PendingTaxAmount != 0)
            {
                var qtd = taxation.State.AllTaxScheduled.Count();
                IClickableMenu.drawTextureBox(Game1.spriteBatch, Game1.mouseCursors, new Rectangle(432, 439, 9, 9), payButton.bounds.X, payButton.bounds.Y, payButton.bounds.Width, payButton.bounds.Height, (payButton.scale > 0f) ? Color.Wheat : Color.White, 4f);
                Utility.drawTextWithShadow(Game1.spriteBatch, Util.Helper.Translation.Get("PayText"), Game1.dialogueFont, new Vector2(payButton.bounds.Center.X, payButton.bounds.Center.Y + 4) - Game1.dialogueFont.MeasureString(Util.Helper.Translation.Get("PayText")) / 2f, Game1.textColor, 1f, -1f, -1, -1, 0f);
            }
        }

    }

}
