using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModSettingsTab.Framework;
using StardewValley;
using StardewValley.Menus;

namespace ModSettingsTab.Menu
{
    public sealed class GameOptionsPage : BaseOptionsPage
    {
        public GameOptionsPage(int x, int y, int width, int height) : base(x, y, width, height)
        {
            ShouldDrawCloseButton = true;
            upperRightCloseButton.bounds.X -= 42;
            // -------- standard options tab ---------
            var originalOptionsComponent = new ClickableTextureComponent("",
                new Rectangle(
                    xPositionOnScreen - 48 +  WidthToMoveActiveTab,
                    yPositionOnScreen + DistanceFromMenuBottomBeforeNewPage,
                    64, 64), "",
                Game1.content.LoadString("Strings\\UI:GameMenu_Options"), ModData.Texture,
                new Rectangle(32, 176, 16, 16),
                4f)
            {
                myID = RegionOriginalOptions,
                downNeighborID = RegionOptionsMod,
                rightNeighborID = 0
            };
            SideTabs.Insert(0, originalOptionsComponent);
            PagesCollections.Insert(0, new OptionsPage(x, y, width, height));
            // -------- favorite mod tab ---------
            UpdateFavoriteTabs();
            FavoriteData.UpdateMod = UpdateFavoriteTabs;
            ResetTab(SavedTab);
        }

        public override void draw(SpriteBatch b)
        {
            b.End();
            b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
            base.draw(b);
        }
    }
}