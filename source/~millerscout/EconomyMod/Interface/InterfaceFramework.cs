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
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace EconomyMod.Interface
{
    public class UIFramework
    {
        public event EventHandler<Coordinate> OnDrawHover;
        public event EventHandler<Coordinate> OnLeftClick;
        public event EventHandler<SpriteBatch> OnDraw;
        public event EventHandler<int> OnBeginPageSideChanged;
        public event EventHandler<int> OnEndPageSideChanged;

        public int ActivePage { get; set; }
        public List<Func<Page>> PageFactory { get; }
        private Dictionary<int, Page> Pages { get; }
        private Dictionary<int, List<ClickableTextureComponent>> sideTabs = new Dictionary<int, List<ClickableTextureComponent>>();
        private string hoverText;

        /// <summary>
        /// DO NOT SET THIS VALUE USE setCurrentSideTab Instead.
        /// </summary>
        public int currentSideTab { get; private set; }
        public IEnumerable<TaxSchedule> ListOfScheduledTax { get; private set; }
        public Dictionary<int, bool> CalendarBool { get; private set; }

        public void setCurrentSideTab(int idSidetab)
        {
            OnBeginPageSideChanged?.Invoke(this, idSidetab);
            currentSideTab = idSidetab;
            OnEndPageSideChanged?.Invoke(this, idSidetab);
        }

        public UIFramework()
        {
            PageFactory = new List<Func<Page>>();
            Pages = new Dictionary<int, Page>();

            Util.Helper.Events.Display.MenuChanged += MenuChanged;
            Util.Helper.Events.Input.ButtonPressed += OnButtonPressed;
            OnBeginPageSideChanged += UIFramework_OnBeginPageSideChanged;
            OnEndPageSideChanged += UIFramework_OnEndPageSideChanged;

        }

        private void UIFramework_OnEndPageSideChanged(object sender, int e)
        {
            var tab = sideTabs[ActivePage].FirstOrDefault((Func<ClickableTextureComponent, bool>)(c => Convert.ToInt32(c.name) == currentSideTab));
            if (tab != null)
            {
                tab.bounds.X += Constants.sideTab_widthToMoveActiveTab;
            }

        }

        private void UIFramework_OnBeginPageSideChanged(object sender, int e)
        {
            var tab = sideTabs[ActivePage].FirstOrDefault((Func<ClickableTextureComponent, bool>)(c => Convert.ToInt32(c.name) == currentSideTab));
            if (tab != null)
            {
                tab.bounds.X -= Constants.sideTab_widthToMoveActiveTab;
            }
        }
        public virtual void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            int x = (int)e.Cursor.ScreenPixels.X;
            int y = (int)e.Cursor.ScreenPixels.Y;

            if (Game1.activeClickableMenu is GameMenu gameMenu)
            {
                if (e.Button == SButton.MouseLeft || e.Button == SButton.ControllerA)
                {
                    foreach (var page in Pages)
                    {


                        if (page.Value.PageButton != null && page.Value.PageButton.isWithinBounds(x, y))
                        {
                            if (Game1.activeClickableMenu is GameMenu menu)
                            {
                                menu.currentTab = page.Value.pageNumber;
                                setCurrentSideTab(0);
                                page.Value.SetAsActive();
                                Game1.playSound("smallSelect");
                            }
                            page.Value.PageButton.LeftClickAction?.Invoke();
                        }
                    }

                    foreach (var v in sideTabs)
                    {
                        var page = Pages.FirstOrDefault(c => c.Value.active);
                        if (gameMenu.currentTab >= 8 && !page.Value.active || gameMenu.currentTab < 8 && page.Value.pageNumber != gameMenu.currentTab) continue;
                        var tab = v.Value.FirstOrDefault(c => c.containsPoint(x, y));
                        if (tab != null)
                        {
                            Game1.playSound("smallSelect");
                            this.setCurrentSideTab(Convert.ToInt32(tab.name));
                            Pages.FirstOrDefault(c => c.Value.sideTabid == this.currentSideTab).Value.SetAsActive();
                        }
                    }
                }

            }

            OnLeftClick?.Invoke(this, new Coordinate(x, y));
        }

        public virtual void MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.OldMenu != null)
            {
                Util.Helper.Events.Display.RenderedActiveMenu -= DrawTabButton;
                Util.Helper.Events.Display.RenderedActiveMenu -= DrawContent;
                Util.Helper.Events.Display.RenderedActiveMenu -= DrawCalendar;
                this.CalendarBool = null;


                if (e.OldMenu is GameMenu gameMenu)
                {
                    List<IClickableMenu> tabPages = gameMenu.pages;
                }
            }

            if (e.NewMenu is GameMenu newMenu)
            {
                for (int i = 0; i < PageFactory.Count; i++)
                {
                    var page = PageFactory[i];

                    var newPage = page();

                    newPage.pageIndex = i;
                    newPage.pageNumber = newMenu.pages.Count + i;
                    Pages.Add(newMenu.pages.Count + i, newPage);

                }
                PageFactory.Clear();

                foreach (var page in Pages)
                {
                    if (page.Value.PageButton == null)
                    {
                        page.Value.AddPageButton();
                    }

                    List<IClickableMenu> tabPages = newMenu.pages;


                    tabPages.Add(page.Value);

                    page.Value.xPositionOnScreen = Game1.activeClickableMenu.xPositionOnScreen;
                    page.Value.yPositionOnScreen = Game1.activeClickableMenu.yPositionOnScreen + 10;
                    page.Value.height = Game1.activeClickableMenu.height;

                    //for (int i = 0; i < Slots.Count; ++i)
                    //{
                    //    var next = Slots[i];
                    //    next.bounds.X = xPositionOnScreen + Game1.tileSize / 4;
                    //    next.bounds.Y = yPositionOnScreen + Game1.tileSize * 5 / 4 + Game1.pixelZoom + i * (height - Game1.tileSize * 2) / 7;
                    //    next.bounds.Width = width - Game1.tileSize / 2;
                    //    next.bounds.Height = (height - Game1.tileSize * 2) / 7 + Game1.pixelZoom;
                    //}
                }

                Util.Helper.Events.Display.RenderedActiveMenu += DrawTabButton;
                Util.Helper.Events.Display.RenderedActiveMenu += DrawContent;
                //Util.Helper.Events.Display.RenderedActiveMenu += DrawCalendar;

            }
            else if (e.NewMenu is Billboard bill)
            {
                Util.Helper.Events.Display.RenderedActiveMenu += DrawCalendar;
            }

        }

        private void DrawCalendar(object sender, RenderedActiveMenuEventArgs e)
        {
            if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is Billboard bill)
            {

                if (CalendarBool == null)
                    CalendarBool = Game1.stats.DaysPlayed.ToWorldDate().GenerateCalendarTaxBool(ListOfScheduledTax);

                for (int i = 0; i < bill.calendarDays.Count; i++)
                {
                    var item = bill.calendarDays.ElementAt(i);



                    var halfWidth = item.bounds.Width / 2;
                    var halfHeight = item.bounds.Height / 2;
                    var NewBound = new Rectangle(item.bounds.X + halfWidth + halfWidth / 2, item.bounds.Y, halfWidth, halfHeight - 12); ;
                    InterfaceHelper.Draw(NewBound, InterfaceHelper.InterfaceHelperType.Red);

                    //e.SpriteBatch.End();
                    //e.SpriteBatch.Begin(SpriteSortMode.Deferred, )
                    if (CalendarBool.ElementAt(i).Value)
                        e.SpriteBatch.Draw(Util.Helper.Content.Load<Texture2D>($"assets/Interface/tabIcon.png"), new Vector2(NewBound.X, NewBound.Y), new Rectangle(0, 0, 16, 16), Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
                }
                //if (e. is GameMenu newMenu)
                //{

                //}
            }

        }

        public virtual void DrawTabButton(object sender, RenderedActiveMenuEventArgs e)
        {
            if (Game1.activeClickableMenu is GameMenu gameMenu && gameMenu.currentTab != 3) //don't render when the map is showing
            {
                foreach (var page in Pages)
                {
                    if (page.Value.PageButton == null) continue;

                    if (gameMenu.currentTab == page.Key)
                    {
                        page.Value.PageButton.yPositionOnScreen = Game1.activeClickableMenu.yPositionOnScreen + 24;

                    }
                    else
                    {
                        page.Value.PageButton.yPositionOnScreen = Game1.activeClickableMenu.yPositionOnScreen + 16;
                    }

                    page.Value.PageButton.draw(Game1.spriteBatch);
                    page.Value.PageButton.performHoverAction(Game1.getMouseX(), Game1.getMouseY());
                }


            }
        }
        public virtual void DrawContent(object sender, RenderedActiveMenuEventArgs e)
        {
            if (Game1.activeClickableMenu is GameMenu gameMenu)
            {

                foreach (var page in Pages.Values)
                {
                    if (gameMenu.currentTab >= 8 && !page.active || gameMenu.currentTab < 8 && page.pageNumber != gameMenu.currentTab) continue;
                    if (!GameMenu.forcePreventClose)
                    {
                        hoverText = "";
                    }

                    if (sideTabs.ContainsKey(page.pageGroup))
                    {
                        foreach (var tab in sideTabs[page.pageGroup])
                        {
                            tab.draw(Game1.spriteBatch);


                            if (tab.containsPoint(Game1.getMouseX(), Game1.getMouseY()))
                            {
                                hoverText = tab.hoverText;
                            }

                        }

                    }
                    if (page.active)
                    {
                        page.Draw?.Invoke();
                        page.DrawHover?.Invoke(Game1.getMouseX(), Game1.getMouseY());
                    }
                    IClickableMenu.drawHoverText(Game1.spriteBatch, hoverText, Game1.smallFont);

                }

                if (!Game1.options.hardwareCursor)
                {
                    Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
                }

            }


        }
        public virtual void AddNewPage(Func<Page> page, int pageGroup, SidetabData sidetabData = null)
        {
            PageFactory.Add((Func<Page>)(() =>
            {
                var p = page();
                p.pageGroup = pageGroup;
                p.OnBeginPageActiveChanged += Page_OnBeginPageActiveChanged;
                p.OnEndPageActiveChanged += Page_OnEndPageActiveChanged;
                if (sidetabData != null)
                {
                    int sidetabCount = sideTabs.ContainsKey(sidetabData.PageGroup) ? sideTabs[sidetabData.PageGroup].Count : 0;
                    var SidetabRect = InterfaceHelper.GetSideTabSizeForPage(p, sidetabCount);

                    int id = sideTabs.ContainsKey(sidetabData.PageGroup) ? sideTabs[sidetabData.PageGroup].Count : 0;
                    var sidetab = new ClickableTextureComponent(string.Concat(id), SidetabRect, "", sidetabData.HoverText, sidetabData.Texture, new Rectangle(0, 0, 16, 16), 4f);
                    p.sideTabid = id;
                    if (sideTabs.ContainsKey(sidetabData.PageGroup))
                    {
                        sideTabs[sidetabData.PageGroup].Add(sidetab);
                    }
                    else
                    {
                        sideTabs.Add(sidetabData.PageGroup, new List<ClickableTextureComponent> { sidetab });
                    }
                    p.LeftClickAction += (object _, Coordinate coord) =>
                    {



                    };


                }
                return p;
            }));
        }

        public virtual void Page_OnEndPageActiveChanged(object sender, int e)
        {

        }

        public virtual void Page_OnBeginPageActiveChanged(object sender, int pageNumber)
        {
            foreach (var page in Pages.Where(c => c.Key != pageNumber))
            {
                page.Value.SetAsInactive();
            }
        }
        public virtual void UpdateListDataList(IEnumerable<TaxSchedule> schedule)
        {
            this.ListOfScheduledTax = schedule;
            this.CalendarBool = null;
        }
    }
    public abstract class Page : IClickableMenu
    {
        public UIFramework ui;

        //public List<OptionsElement> Elements = new List<OptionsElement>();
        public List<IContentElement> Elements = new List<IContentElement>();
        public List<ClickableComponent> Slots = new List<ClickableComponent>();
        public Page(UIFramework ui, Texture2D Icon = null, string hoverText = null) : base(Game1.activeClickableMenu.xPositionOnScreen, Game1.activeClickableMenu.yPositionOnScreen + 10, Constants.MenuWidth, Game1.activeClickableMenu.height)
        {

            this.ui = ui;
            this.Icon = Icon;
            this.HoverText = hoverText;
        }

        public Action Draw { get; set; }
        public Action<int, int> DrawHover { get; set; }
        public event EventHandler<Coordinate> LeftClickAction;
        public event EventHandler<int> OnBeginPageActiveChanged;
        public event EventHandler<int> OnEndPageActiveChanged;
        public PageButton PageButton;

        internal int pageNumber;
        Texture2D Icon;
        string HoverText;
        /// <summary>
        /// This is the reference to know which is the positions of the button.
        /// </summary>
        internal int pageIndex;
        internal string hoverText;
        internal int sideTabid;
        internal int pageGroup;
        public bool active { get; private set; }


        public virtual void SetAsActive()
        {
            OnBeginPageActiveChanged?.Invoke(this, pageNumber);
            active = true;
            OnEndPageActiveChanged?.Invoke(this, pageNumber);
        }
        public virtual void SetAsInactive()
        {
            active = false;
        }
        public virtual void AddPageButton()
        {
            ///Shouldn't add a button.
            if (Icon == null || HoverText == null) return;
            PageButton = new PageButton(Icon, HoverText, pageIndex);


        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (!this.active) return;
            LeftClickAction?.Invoke(this, new Coordinate(x, y));
            base.receiveLeftClick(x, y, playSound);


        }
    }

    public class PageButton : IClickableMenu
    {
        public PageButton(Texture2D texture, string hoverText, int indexPosition)
        {
            width = 64;
            height = 64;
            GameMenu activeClickableMenu = Game1.activeClickableMenu as GameMenu;
            xPositionOnScreen = activeClickableMenu.xPositionOnScreen + activeClickableMenu.width - 304 + 64 * indexPosition;
            yPositionOnScreen = activeClickableMenu.yPositionOnScreen + 16;
            Bounds = new Rectangle(xPositionOnScreen, yPositionOnScreen, width, height);
            this.texture = texture;
            this.hoverText = hoverText;

        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (isWithinBounds(x, y))
            {
                base.receiveLeftClick(x, y, playSound);
            }
        }
        public override void draw(SpriteBatch b)
        {
            base.draw(b);

            Game1.spriteBatch.Draw(Game1.mouseCursors,
                new Vector2(xPositionOnScreen, yPositionOnScreen),
                new Rectangle(16, 368, 16, 16),
                Color.White,
                0.0f,
                Vector2.Zero,
                Game1.pixelZoom,
                SpriteEffects.None,
                1f);

            b.Draw(texture, new Vector2(xPositionOnScreen + 8, yPositionOnScreen + 14), new Rectangle(0, 0, 16, 16), Color.White, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);

            if (isWithinBounds(Game1.getMouseX(), Game1.getMouseY()))
            {
                IClickableMenu.drawHoverText(Game1.spriteBatch, hoverText, Game1.smallFont);
            }

        }

        public Rectangle Bounds { get; }
        public Action LeftClickAction { get; set; }
        private Texture2D texture;
        private string hoverText;
    }

    public class SidetabData
    {
        public SidetabData(Texture2D texture2D, string hoverText, int PageGroup)
        {
            this.Texture = texture2D;
            this.HoverText = hoverText;
            this.PageGroup = PageGroup;
        }

        public Texture2D Texture { get; set; }
        public string HoverText { get; set; }

        public int PageGroup;
    }
    public class Coordinate
    {
        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }
}
