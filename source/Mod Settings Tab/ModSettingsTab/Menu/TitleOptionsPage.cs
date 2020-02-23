using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModSettingsTab.Framework;
using StardewValley;
using StardewValley.Menus;

namespace ModSettingsTab.Menu
{
    public sealed class TitleOptionsPage : BaseOptionsPage
    {
        private readonly List<ClickableTextureComponent> _topTabs = new List<ClickableTextureComponent>();
        private readonly List<IClickableMenu> _modManagerPagesCollections = new List<IClickableMenu>();
        private const int RegionModManagerTab = 5000;
        public TitleOptionsPage() : base(
            (int) ((Game1.viewport.Width * 1.1f - (800 + borderWidth * 2 + ModData.Offset)) / 2f),
            (int) ((Game1.viewport.Height * 1.1f - (600 + borderWidth * 2)) / 2f - 48),
            800 + borderWidth * 2 + ModData.Offset,
            600 + borderWidth * 2)
        {
            ReloadIndicator.bounds.X -= 320;
            var originalOptionsComponent = new ClickableTextureComponent("",
                new Rectangle(
                    xPositionOnScreen - 48 + WidthToMoveActiveTab,
                    yPositionOnScreen + DistanceFromMenuBottomBeforeNewPage,
                    64, 64), "",
                "Mod manager", Game1.mouseCursors,
                new Rectangle(672, 80, 16, 16),
                4f)
            {
                myID = RegionOriginalOptions,
                downNeighborID = RegionOptionsMod,
                rightNeighborID = 0
            };
            SideTabs.Insert(0, originalOptionsComponent);
            PagesCollections.Insert(0, new ModManagerPage(xPositionOnScreen, yPositionOnScreen, width, height));
            // -------- favorite mod tab ---------
            UpdateFavoriteTabs();
            UpdateModManagerTabs();
            ModManager.UpdateMod += UpdateModManagerTabs;
            FavoriteData.UpdateMod = UpdateFavoriteTabs;
            ResetTab(SavedTab);
        }

        protected override void ResetTab(int index)
        {
            var countTab = SideTabs.Count + FavoriteSideTabs.Count + _topTabs.Count;
            if (CurrentTab < countTab)
            {
                if (CurrentTab < SideTabs.Count)
                    SideTabs[CurrentTab].bounds.X -= WidthToMoveActiveTab;
                else if (CurrentTab < SideTabs.Count + FavoriteSideTabs.Count)
                    FavoriteSideTabs[CurrentTab - SideTabs.Count].bounds.X -= WidthToMoveActiveTab;
                else
                    _topTabs[CurrentTab - SideTabs.Count - FavoriteSideTabs.Count].bounds.Y -=
                        WidthToMoveActiveTab;
            }

            if (index < SideTabs.Count)
            {
                SideTabs[index].bounds.X += WidthToMoveActiveTab;
            }
            else if (index < SideTabs.Count + FavoriteSideTabs.Count)
            {
                FavoriteSideTabs[index - SideTabs.Count].bounds.X += WidthToMoveActiveTab;
            }
            else if (index < countTab)
            {
                _topTabs[index - SideTabs.Count - FavoriteSideTabs.Count].bounds.Y +=
                    WidthToMoveActiveTab;
            }

            CurrentTab = SavedTab = index;
            Game1.playSound("smallSelect");
        }

        private void UpdateModManagerTabs()
        {
            _topTabs.Clear();
            _modManagerPagesCollections.Clear();
            if (CurrentTab > 1) ResetTab(1);
            var i = 0;
            foreach (var p in ModManager.PackList)
            {
                var modPackComponent = new ClickableTextureComponent("",
                    new Rectangle(
                        xPositionOnScreen + 64 + i * 64,
                        yPositionOnScreen + 16,
                        64, 64), "",
                    p.Key, ModData.Texture,
                    new Rectangle(i * 16, 272, 16, 16), 4f)
                {
                    myID = RegionModManagerTab + i,
                    leftNeighborID = RegionModManagerTab + i - 1,
                    rightNeighborID = RegionModManagerTab + i + 1
                };
                i++;
                _topTabs.Add(modPackComponent);
                _modManagerPagesCollections.Add(new ModPackPage(xPositionOnScreen, yPositionOnScreen, width,
                    height,p.Key));
            }

            if (_topTabs.Count < 4)
            {
                var modPackComponent = new ClickableTextureComponent("",
                    new Rectangle(
                        xPositionOnScreen + 64 + i * 64,
                        yPositionOnScreen + 16,
                        64, 64), "",
                    Helper.I18N.Get("ModManager.AddModPack"), ModData.Texture,
                    new Rectangle(48, 176, 16, 16), 4f)
                {
                    myID = RegionModManagerTab + i,
                    leftNeighborID = RegionModManagerTab + i - 1
                };
                _topTabs.Add(modPackComponent);
                _modManagerPagesCollections.Add(new AddModPackPage(xPositionOnScreen, yPositionOnScreen, width,
                    height));
            }
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            TitleMenu.subMenu = null;
        }

        public override void releaseLeftClick(int x, int y)
        {
            x = (int) (x * 1.1f);
            y = (int) (y * 1.1f);
            if (CurrentTab < SideTabs.Count + FavoriteSideTabs.Count + _topTabs.Count
                && CurrentTab >= SideTabs.Count + FavoriteSideTabs.Count)
                _modManagerPagesCollections[CurrentTab - SideTabs.Count - FavoriteSideTabs.Count]
                    .releaseLeftClick(x, y);
            base.releaseLeftClick(x, y);
        }

        public override void leftClickHeld(int x, int y)
        {
            base.leftClickHeld((int) (x * 1.1f), (int) (y * 1.1f));
        }

        public override void receiveScrollWheelAction(int direction)
        {
            if (CurrentTab < SideTabs.Count + FavoriteSideTabs.Count + _topTabs.Count
                && CurrentTab >= SideTabs.Count + FavoriteSideTabs.Count)
                _modManagerPagesCollections[CurrentTab - SideTabs.Count - FavoriteSideTabs.Count]
                    .receiveScrollWheelAction(direction);
            base.receiveScrollWheelAction(direction);
        }
        
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (GameMenu.forcePreventClose)
                return;
            x = (int) (x * 1.1f);
            y = (int) (y * 1.1f);
            if (x > xPositionOnScreen + borderWidth)
            {
                if (CurrentTab < SideTabs.Count + FavoriteSideTabs.Count + _topTabs.Count
                    && CurrentTab >= SideTabs.Count + FavoriteSideTabs.Count)
                    _modManagerPagesCollections[CurrentTab - SideTabs.Count - FavoriteSideTabs.Count]
                        .receiveLeftClick(x, y);
            }

            for (var i = 0; i < _topTabs.Count; i++)
            {
                if (CurrentTab == i + SideTabs.Count + FavoriteSideTabs.Count ||
                    !_topTabs[i].containsPoint(x, y)) continue;
                ResetTab(i + SideTabs.Count + FavoriteSideTabs.Count);
            }

            base.receiveLeftClick(x, y, playSound);

        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            x = (int) (x * 1.1f);
            y = (int) (y * 1.1f);
            base.receiveRightClick(x, y, playSound);
        }

        public override void performHoverAction(int x, int y)
        {
            x = (int) (x * 1.1f);
            y = (int) (y * 1.1f);
            if (CurrentTab < SideTabs.Count + FavoriteSideTabs.Count + _topTabs.Count
                && CurrentTab >= SideTabs.Count + FavoriteSideTabs.Count)
            {
                _modManagerPagesCollections[CurrentTab - SideTabs.Count - FavoriteSideTabs.Count]
                    .performHoverAction(x, y);
            }

            using (var enumerator = _topTabs.Where(sideTab => sideTab.containsPoint(x, y)).GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    HoverText = enumerator.Current?.hoverText;
                    return;
                }
            }

            base.performHoverAction(x, y);
        }

        public override void draw(SpriteBatch b)
        {
            var viewport = Game1.graphics.GraphicsDevice.Viewport;
            b.Draw(Game1.fadeToBlackRect, viewport.Bounds, Color.Multiply(Color.Black, 0.6f));
            b.End();
            b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null,
                Matrix.CreateScale(0.9f));
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            foreach (var sideTab in _topTabs)
                sideTab.draw(b);
            foreach (var sideTab in SideTabs)
                sideTab.draw(b);
            foreach (var sideTab in FavoriteSideTabs)
                sideTab.draw(b);
            if (ModData.NeedReload && ModData.Config.ShowReloadIcon) ReloadIndicator.draw(b);
            if (CurrentTab < SideTabs.Count)
                PagesCollections[CurrentTab].draw(b);
            else if (CurrentTab < SideTabs.Count + FavoriteSideTabs.Count)
                FavoritePagesCollections[CurrentTab - SideTabs.Count].draw(b);
            else if (CurrentTab < SideTabs.Count + FavoriteSideTabs.Count + _topTabs.Count)
                _modManagerPagesCollections[CurrentTab - SideTabs.Count - FavoriteSideTabs.Count].draw(b);
            if (HoverText.Equals(""))
                return;
            drawHoverText(b, HoverText, Game1.smallFont);
        }
    }
}