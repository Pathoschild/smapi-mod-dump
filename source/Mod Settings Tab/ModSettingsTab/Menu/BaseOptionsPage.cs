using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ModSettingsTab.Framework;
using StardewValley;
using StardewValley.Menus;

// ReSharper disable MemberCanBePrivate.Global

namespace ModSettingsTab.Menu
{
    public abstract class BaseOptionsPage : IClickableMenu
    {
        protected string HoverText = "";
        protected readonly List<ClickableTextureComponent> SideTabs = new List<ClickableTextureComponent>();
        protected readonly List<ClickableTextureComponent> FavoriteSideTabs = new List<ClickableTextureComponent>();
        protected readonly List<IClickableMenu> PagesCollections = new List<IClickableMenu>();
        protected readonly List<IClickableMenu> FavoritePagesCollections = new List<IClickableMenu>();
        protected readonly ClickableTextureComponent ReloadIndicator;
        protected bool ShouldDrawCloseButton;


        protected const int TabHeight = 64;
        protected const int FavoriteTabSize = 48;
        protected const int WidthToMoveActiveTab = 16;
        protected const int RegionOriginalOptions = 7001;
        protected const int RegionOptionsMod = 7002;
        protected const int RegionFavoriteOptionsMod = 7003;
        protected const int DistanceFromMenuBottomBeforeNewPage = 128;
        protected int CurrentTab;
        protected static int SavedTab;

        protected BaseOptionsPage(int x, int y, int width, int height) : base(x, y, width, height, true)
        {
            ReloadIndicator = new ClickableTextureComponent(
                "",
                new Rectangle(xPositionOnScreen + width - ModData.Offset, yPositionOnScreen + height - 20, 44, 56),
                "",
                Helper.I18N.Get("OptionsPage.ReloadIndicator"),
                Game1.mouseCursors,
                new Rectangle(383, 493, 11, 14),
                4f);

            // -------- mod options tab ---------
            var modOptionsComponent = new ClickableTextureComponent("",
                new Rectangle(
                    xPositionOnScreen - 48,
                    yPositionOnScreen + DistanceFromMenuBottomBeforeNewPage + TabHeight,
                    64, 64),
                "",
                Helper.I18N.Get("OptionsPage.ModSettingsTab"), ModData.Texture,
                new Rectangle(0, 0, 64, 64), 1f)
            {
                myID = RegionOptionsMod,
                upNeighborID = RegionOriginalOptions,
                downNeighborID = RegionFavoriteOptionsMod,
                rightNeighborID = 0
            };
            SideTabs.Add(modOptionsComponent);
            PagesCollections.Add(new OptionsModPage(x, y, width, height));
            // -------- SMAPI tab ---------
            var smapiOptionsComponent = new ClickableTextureComponent("",
                new Rectangle(
                    xPositionOnScreen - 48,
                    yPositionOnScreen + height - DistanceFromMenuBottomBeforeNewPage,
                    64, 64), "",
                "SMAPI", ModData.Texture,
                new Rectangle(0, 64, 64, 64),
                1f)
            {
                myID = RegionFavoriteOptionsMod + FavoriteSideTabs.Count,
                downNeighborID = 0,
                rightNeighborID = 0
            };
            SideTabs.Add(smapiOptionsComponent);
            PagesCollections.Add(new SmapiOptionsPage(x, y, width, height));
        }

        protected void UpdateFavoriteTabs()
        {
            FavoriteSideTabs.Clear();
            FavoritePagesCollections.Clear();
            if (CurrentTab > 1) ResetTab(1);
            var fModCount = FavoriteData.ModList.Count;
            if (fModCount == 0) return;

            for (int i = fModCount, c = 0; i > 0; i--, c++)
            {
                var manifest = FavoriteData.ModList[i - 1].Manifest;
                var favoriteModComponent = new ClickableTextureComponent("",
                    new Rectangle(
                        xPositionOnScreen - 48,
                        yPositionOnScreen + DistanceFromMenuBottomBeforeNewPage + TabHeight * 2 + 16 +
                        FavoriteTabSize * c,
                        64, FavoriteTabSize), "",
                    manifest.Name, ModData.Texture,
                    FavoriteData.FavoriteTabSource[manifest.UniqueID], 2f)
                {
                    myID = RegionFavoriteOptionsMod + c,
                    upNeighborID = RegionFavoriteOptionsMod + c - 1,
                    downNeighborID = RegionFavoriteOptionsMod + c + 1,
                    rightNeighborID = 0
                };
                FavoriteSideTabs.Add(favoriteModComponent);
                FavoritePagesCollections.Add(new FavoriteOptionsModPage(xPositionOnScreen, yPositionOnScreen, width,
                    height, i - 1));
            }

            var stl = SideTabs.Last();
            stl.myID = FavoriteSideTabs.Count;
            stl.upNeighborID = FavoriteSideTabs.Last().myID;
        }

        protected override void customSnapBehavior(int direction, int oldRegion, int oldId)
        {
            base.customSnapBehavior(direction, oldRegion, oldId);
        }

        public override bool shouldDrawCloseButton() => false;

        public override void receiveScrollWheelAction(int direction)
        {
            if (GameMenu.forcePreventClose)
                return;
            base.receiveScrollWheelAction(direction);
            if (CurrentTab < SideTabs.Count)
                PagesCollections[CurrentTab].receiveScrollWheelAction(direction);
            else if (CurrentTab < SideTabs.Count + FavoriteSideTabs.Count)
                FavoritePagesCollections[CurrentTab - SideTabs.Count].receiveScrollWheelAction(direction);
        }


        public override void snapToDefaultClickableComponent()
        {
            base.snapToDefaultClickableComponent();
            currentlySnappedComponent = getComponentWithID(0);
            snapCursorToCurrentSnappedComponent();
            if (CurrentTab < SideTabs.Count)
                PagesCollections[CurrentTab].snapToDefaultClickableComponent();
            else if (CurrentTab < SideTabs.Count + FavoriteSideTabs.Count) 
                FavoritePagesCollections[CurrentTab - SideTabs.Count].snapToDefaultClickableComponent();
        }

        public override void leftClickHeld(int x, int y)
        {
            base.leftClickHeld(x, y);
            if (CurrentTab < SideTabs.Count)
                PagesCollections[CurrentTab].leftClickHeld(x, y);
            else if (CurrentTab < SideTabs.Count + FavoriteSideTabs.Count)
                FavoritePagesCollections[CurrentTab - SideTabs.Count].leftClickHeld(x, y);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (GameMenu.forcePreventClose)
                return;
            base.receiveLeftClick(x, y, playSound);
            if (x > xPositionOnScreen + borderWidth)
            {
                if (CurrentTab < SideTabs.Count)
                    PagesCollections[CurrentTab].receiveLeftClick(x, y, playSound);
                else if (CurrentTab < SideTabs.Count + FavoriteSideTabs.Count)
                    FavoritePagesCollections[CurrentTab - SideTabs.Count].receiveLeftClick(x, y, playSound);
                return;
            }

            var tabsCount = SideTabs.Count + FavoriteSideTabs.Count;
            for (var index = 0; index < tabsCount; index++)
            {
                if (index < SideTabs.Count)
                {
                    if (!SideTabs[index].containsPoint(x, y) || CurrentTab == index) continue;
                    ResetTab(index);
                    return;
                }

                if (!FavoriteSideTabs[index - SideTabs.Count].containsPoint(x, y) || CurrentTab == index) continue;
                ResetTab(index);
                return;
            }
        }

        protected virtual void ResetTab(int index)
        {
            if (CurrentTab < SideTabs.Count + FavoriteSideTabs.Count)
            {
                if (CurrentTab < SideTabs.Count)
                    SideTabs[CurrentTab].bounds.X -= WidthToMoveActiveTab;
                else
                    FavoriteSideTabs[CurrentTab - SideTabs.Count].bounds.X -= WidthToMoveActiveTab;
            }

            if (index < SideTabs.Count)
            {
                SideTabs[index].bounds.X += WidthToMoveActiveTab;
            }
            else if (index < SideTabs.Count + FavoriteSideTabs.Count)
            {
                FavoriteSideTabs[index - SideTabs.Count].bounds.X += WidthToMoveActiveTab;
            }

            CurrentTab = SavedTab = index;
            Game1.playSound("smallSelect");
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (GameMenu.forcePreventClose)
                return;
            if (x < xPositionOnScreen - borderWidth) return;
            if (CurrentTab < SideTabs.Count)
                PagesCollections[CurrentTab].receiveRightClick(x, y, playSound);
            else if (CurrentTab < SideTabs.Count + FavoriteSideTabs.Count)
                FavoritePagesCollections[CurrentTab - SideTabs.Count].receiveRightClick(x, y, playSound);
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            HoverText = "";
            
            if (CurrentTab < SideTabs.Count)
                PagesCollections[CurrentTab].performHoverAction(x, y);
            else if (CurrentTab < SideTabs.Count + FavoriteSideTabs.Count)
                FavoritePagesCollections[CurrentTab - SideTabs.Count].performHoverAction(x, y);
            
            if (ModData.NeedReload && ReloadIndicator.containsPoint(x, y))
            {
                HoverText = ReloadIndicator.hoverText;
                return;
            }

            using (var enumerator = SideTabs.Where(sideTab => sideTab.containsPoint(x, y)).GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    HoverText = enumerator.Current?.hoverText;
                    return;
                }
            }

            using (var enumerator = FavoriteSideTabs.Where(sideTab => sideTab.containsPoint(x, y)).GetEnumerator())
            {
                if (!enumerator.MoveNext()) return;
                HoverText = enumerator.Current?.hoverText;
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            if (GameMenu.forcePreventClose)
                return;
            base.releaseLeftClick(x, y);
            if (CurrentTab < SideTabs.Count)
                PagesCollections[CurrentTab].releaseLeftClick(x, y);
            else if (CurrentTab < SideTabs.Count + FavoriteSideTabs.Count)
                FavoritePagesCollections[CurrentTab - SideTabs.Count].releaseLeftClick(x, y);
        }

        public override void snapCursorToCurrentSnappedComponent()
        {
            if (CurrentTab < SideTabs.Count)
                PagesCollections[CurrentTab].snapCursorToCurrentSnappedComponent();
            else if (CurrentTab < SideTabs.Count + FavoriteSideTabs.Count)
                FavoritePagesCollections[CurrentTab - SideTabs.Count].snapCursorToCurrentSnappedComponent();
        }

        public override void receiveKeyPress(Keys key)
        {
            if (CurrentTab < SideTabs.Count)
                PagesCollections[CurrentTab].receiveKeyPress(key);
            else  if (CurrentTab < SideTabs.Count + FavoriteSideTabs.Count)
                FavoritePagesCollections[CurrentTab - SideTabs.Count].receiveKeyPress(key);
            base.receiveKeyPress(key);
        }

        // ReSharper disable once UnusedMember.Global
        public void SetScrollBarToCurrentIndex()
        {
            if (CurrentTab < SideTabs.Count)
                ((BaseOptionsModPage) PagesCollections[CurrentTab]).SetScrollBarToCurrentIndex();
            else if (CurrentTab < SideTabs.Count + FavoriteSideTabs.Count)
                ((BaseOptionsModPage) FavoritePagesCollections[CurrentTab - SideTabs.Count])
                    .SetScrollBarToCurrentIndex();
        }

        public override void draw(SpriteBatch b)
        {
            if (ShouldDrawCloseButton) upperRightCloseButton.draw(b);
            foreach (var sideTab in SideTabs)
                sideTab.draw(b);
            foreach (var sideTab in FavoriteSideTabs)
                sideTab.draw(b);
            if (ModData.NeedReload && ModData.Config.ShowReloadIcon) ReloadIndicator.draw(b);
            if (CurrentTab < SideTabs.Count)
                PagesCollections[CurrentTab].draw(b);
            else if (CurrentTab < SideTabs.Count + FavoriteSideTabs.Count)
                FavoritePagesCollections[CurrentTab - SideTabs.Count].draw(b);
            if (HoverText.Equals(""))
                return;
            drawHoverText(b, HoverText, Game1.smallFont);
        }
    }
}