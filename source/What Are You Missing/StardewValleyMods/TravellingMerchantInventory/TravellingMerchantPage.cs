using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Menus;
using StardewValley;
using Microsoft.Xna.Framework;

namespace TravellingMerchantInventory
{
    class TravellingMerchantPage : ShopMenu
    {
        private bool scrolling;
        private int currentItemIndex;
        private Rectangle scrollBarRunner;
        private List<Item> forSale = new List<Item>();
        private bool buyFromPage;
        
        public TravellingMerchantPage(Dictionary<Item, int[]> stock, bool buyFromPage = false, int currency = 0, string who = "Traveler")
      : base(stock, 0, "Traveler" )
        {
            this.buyFromPage = buyFromPage;
        }
        private void setScrollBarToCurrentIndex()
        {
            if (this.forSale.Count <= 0)
                return;
            this.scrollBar.bounds.Y = this.scrollBarRunner.Height / Math.Max(1, this.forSale.Count - 4 + 1) * this.currentItemIndex + this.upArrow.bounds.Bottom + 4;
            if (this.currentItemIndex != this.forSale.Count - 4)
                return;
            this.scrollBar.bounds.Y = this.downArrow.bounds.Y - this.scrollBar.bounds.Height - 4;
        }
        private void downArrowPressed()
        {
            this.downArrow.scale = this.downArrow.baseScale;
            ++this.currentItemIndex;
            this.setScrollBarToCurrentIndex();
        }

        private void upArrowPressed()
        {
            this.upArrow.scale = this.upArrow.baseScale;
            --this.currentItemIndex;
            this.setScrollBarToCurrentIndex();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (upperRightCloseButton.containsPoint(x, y))
            {
                if (playSound)
                    Game1.playSound("bigDeSelect");
                Game1.exitActiveMenu();
                return;
            }
            if (buyFromPage)
            {
                base.receiveLeftClick(x, y, playSound);
            }
            else
            {
                bool flag = false;
                if (this.upperRightCloseButton == null || !this.readyToClose() || !this.upperRightCloseButton.containsPoint(x, y))
                {
                    //originally returned so if we get here we don't want to do the next few lines
                    flag = true;
                }
                if (playSound && !flag)
                {
                    Game1.playSound("bigDeSelect");
                }
                
                if (Game1.activeClickableMenu == null)
                    return;
                Vector2 clickableComponent = this.inventory.snapToClickableComponent(x, y);
                if (this.downArrow.containsPoint(x, y) && this.currentItemIndex < Math.Max(0, this.forSale.Count - 4))
                {
                    this.downArrowPressed();
                    Game1.playSound("shwip");
                }
                else if (this.upArrow.containsPoint(x, y) && this.currentItemIndex > 0)
                {
                    this.upArrowPressed();
                    Game1.playSound("shwip");
                }
                else if (this.scrollBar.containsPoint(x, y))
                    this.scrolling = true;
                else if (!this.downArrow.containsPoint(x, y) && x > this.xPositionOnScreen + this.width && (x < this.xPositionOnScreen + this.width + 128 && y > this.yPositionOnScreen) && y < this.yPositionOnScreen + this.height)
                {
                    this.scrolling = true;
                    this.leftClickHeld(x, y);
                    this.releaseLeftClick(x, y);
                }
                this.currentItemIndex = Math.Max(0, Math.Min(this.forSale.Count - 4, this.currentItemIndex));

                if (!this.readyToClose() || x >= this.xPositionOnScreen - 64 && y >= this.yPositionOnScreen - 64 && (x <= this.xPositionOnScreen + this.width + 128 && y <= this.yPositionOnScreen + this.height + 64))
                    return;
                this.exitThisMenu(true);
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (buyFromPage)
            {
                base.receiveRightClick(x, y, playSound);
            }
        }
    }
}
