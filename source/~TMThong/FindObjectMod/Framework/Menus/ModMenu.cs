/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley.Menus;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindObjectMod.Framework.Menus
{
    public class ModMenu : IClickableMenu
    {
        public IModHelper Helper { get; }

        public IMonitor Monitor { get; }

        public List<IClickableMenu> Pages { get; } = new List<IClickableMenu>();

        public ModMenu(IModHelper helper, IMonitor monitor)
        {
            this.Helper = helper;
            this.Monitor = monitor;
        }

        public virtual void Initialization()
        {
            bool isAndroid = Utilities.IsAndroid;
            if (isAndroid)
            {
                this.width = Game1.viewport.Width - 64;
                this.height = Game1.viewport.Height;
                this.xPositionOnScreen = 0;
                this.yPositionOnScreen = 0;
            }
            else
            {
                this.width = (int)((double)Game1.viewport.Width / 1.5);
                this.height = (int)((double)Game1.viewport.Height / 1.1);
                this.xPositionOnScreen = Game1.viewport.Width / 2 - this.width / 2;
                this.yPositionOnScreen = Game1.viewport.Height / 2 - this.height / 2;
            }
            base.initializeUpperRightCloseButton();
            this.Pages.Clear();
            OptionsPage optionsPage;
            try
            {
                optionsPage = new OptionsPage(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);
            }
            catch (Exception e)
            {
                optionsPage = typeof(OptionsPage).CreateInstance<OptionsPage>(new object[]
                {
                    this.xPositionOnScreen,
                    this.yPositionOnScreen,
                    this.width,
                    this.height,
                    1f,
                    1f
                });
            }
            optionsPage.options().Clear();
            ModEntry.Instance.InitOptionsElement();
            optionsPage.options().AddRange(Utilities.OptionsElements);
            optionsPage.InitializationScrollBoxAndroid(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);
            this.Pages.Add(optionsPage);
            this.Current = 0;
            this.currentPage_ = this.Pages[this.Current];
        }

        public IClickableMenu currentPage()
        {
            return this.currentPage_;
        }

        public void updateElements(List<OptionsElement> optionsElements)
        {
            OptionsPage optionsPage = this.currentPage() as OptionsPage;
            bool flag = optionsPage != null;
            if (flag)
            {
                optionsPage.options().Clear();
                optionsPage.options().AddRange(optionsElements);
                optionsPage.InitializationScrollBoxAndroid(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            bool flag = !GameMenu.forcePreventClose;
            if (flag)
            {
                bool flag2 = this.currentPage_ == null;
                if (flag2)
                {
                    this.Initialization();
                }
                else
                {
                    this.currentPage_.releaseLeftClick(x, y);
                }
                base.releaseLeftClick(x, y);
            }
        }

        public override void snapCursorToCurrentSnappedComponent()
        {
            bool flag = !GameMenu.forcePreventClose;
            if (flag)
            {
                bool flag2 = this.currentPage_ == null;
                if (flag2)
                {
                    this.Initialization();
                }
                else
                {
                    this.currentPage_.snapCursorToCurrentSnappedComponent();
                }
                base.snapCursorToCurrentSnappedComponent();
            }
        }

        public override void setCurrentlySnappedComponentTo(int id)
        {
            bool flag = !GameMenu.forcePreventClose;
            if (flag)
            {
                bool flag2 = this.currentPage_ == null;
                if (flag2)
                {
                    this.Initialization();
                }
                else
                {
                    this.currentPage_.setCurrentlySnappedComponentTo(id);
                }
                base.setCurrentlySnappedComponentTo(id);
            }
        }

        public override ClickableComponent getCurrentlySnappedComponent()
        {
            return this.currentPage_.getCurrentlySnappedComponent();
        }

        public override void update(GameTime time)
        {
            bool flag = !GameMenu.forcePreventClose;
            if (flag)
            {
                bool flag2 = this.currentPage_ == null;
                if (flag2)
                {
                    this.Initialization();
                }
                else
                {
                    this.currentPage_.update(time);
                }
                base.update(time);
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            bool flag = !GameMenu.forcePreventClose;
            if (flag)
            {
                bool flag2 = this.currentPage_ == null;
                if (flag2)
                {
                    this.Initialization();
                }
                else
                {
                    this.currentPage_.receiveLeftClick(x, y, true);
                }
                base.receiveLeftClick(x, y, true);
            }
        }

        public override void receiveScrollWheelAction(int direction)
        {
            bool flag = !GameMenu.forcePreventClose;
            if (flag)
            {
                bool flag2 = this.currentPage_ == null;
                if (flag2)
                {
                    this.Initialization();
                }
                else
                {
                    this.currentPage_.receiveScrollWheelAction(direction);
                }
                base.receiveScrollWheelAction(direction);
            }
        }

        public override void leftClickHeld(int x, int y)
        {
            bool flag = !GameMenu.forcePreventClose;
            if (flag)
            {
                bool flag2 = this.currentPage_ == null;
                if (flag2)
                {
                    this.Initialization();
                }
                else
                {
                    this.currentPage_.leftClickHeld(x, y);
                }
                base.leftClickHeld(x, y);
            }
        }

        public override void draw(SpriteBatch b)
        {
            bool flag = !GameMenu.forcePreventClose;
            if (flag)
            {
                bool flag2 = !Utilities.IsAndroid;
                if (flag2)
                {
                    Game1.DrawBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, null);
                }
                bool flag3 = this.currentPage_ == null;
                if (flag3)
                {
                    this.Initialization();
                }
                else
                {
                    this.currentPage_.draw(b);
                }
                base.draw(b);
                base.drawMouse(b);
            }
        }

        public override void performHoverAction(int x, int y)
        {
            bool flag = !GameMenu.forcePreventClose;
            if (flag)
            {
                bool flag2 = this.currentPage_ == null;
                if (flag2)
                {
                    this.Initialization();
                }
                else
                {
                    this.currentPage_.performHoverAction(x, y);
                }
                base.performHoverAction(x, y);
            }
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            this.Initialization();
        }

        public int Current = 0;

        [NonSerialized]
        internal IClickableMenu currentPage_;
    }
}
