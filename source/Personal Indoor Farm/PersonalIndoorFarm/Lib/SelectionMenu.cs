/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/personal-indoor-farm
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalIndoorFarm.Lib
{
    internal class SelectionMenu : IClickableMenu
    {
        private static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        private Dictionary<string, PersonalFarmModel> Models;
        private int Index;
        private Texture2D Preview;
        private PersonalFarmModel Current { get => Models.ElementAt(Index).Value; }
        public bool Confirmed = false;
        public KeyValuePair<string, PersonalFarmModel> ConfirmedModel { get => Models.ElementAt(Index); }

        public ClickableTextureComponent LeftArrow;
        public ClickableTextureComponent RightArrow;
        public ClickableTextureComponent ConfirmButton;
        public ClickableTextureComponent CancelButton;

        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = Mod.Monitor;
            Helper = Mod.Helper;
        }

        public SelectionMenu() : base(Game1.uiViewport.Width / 2 - 320, Game1.uiViewport.Height - 64 - 192, 640, 192)
        {
            Models = Helper.GameContent.Load<Dictionary<string, PersonalFarmModel>>(AssetRequested.FarmsAsset);
            updatePreview();
            updatePosition();
            createClickTableTextures();
        }

        private void createClickTableTextures()
        {
            LeftArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + (width / 2) - 40 - 64 - 80, yPositionOnScreen + height - 100, 44, 48), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f) {
                myID = 8000,
                rightNeighborID = 8001,
                downNeighborID = 8003,
            };

            RightArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + (width / 2) + 40 - 64 + 80, yPositionOnScreen + height - 100, 44, 48), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f) {
                myID = 8001,
                leftNeighborID = 8000,
                downNeighborID = 8004
            };

            CancelButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + (width / 2) - 80, yPositionOnScreen + height - 60, 64, 64), Game1.mouseCursors, new Rectangle(192, 256, 64, 64), 1f) {
                myID = 8003,
                upNeighborID = 8000,
                rightNeighborID = 8004
            };

            ConfirmButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + (width / 2) + 12, yPositionOnScreen + height - 60, 64, 64), Game1.mouseCursors, new Rectangle(128, 256, 64, 64), 1f) {
                myID = 8004,
                upNeighborID = 8001,
                leftNeighborID = 8003
            };
        }

        private void updatePreview()
        => Preview = Helper.GameContent.Load<Texture2D>(Current.Preview);

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);
            var textureBoxRectangle = new Rectangle(xPositionOnScreen, yPositionOnScreen + 64, width, height - 200);
            drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), textureBoxRectangle.X, textureBoxRectangle.Y, textureBoxRectangle.Width, textureBoxRectangle.Height, Color.White, 4f);

            if (Preview is not null) {
                Rectangle previewRectangle = new Rectangle(textureBoxRectangle.X + 16, textureBoxRectangle.Y + 18, textureBoxRectangle.Width - 32, textureBoxRectangle.Height - 32);
                b.Draw(Preview, previewRectangle, Color.White);
            }

            if (canPageLeft())
                LeftArrow.draw(b);
            if (canPageRight())
                RightArrow.draw(b);
            CancelButton.draw(b);
            ConfirmButton.draw(b);

            if (Current.DisplayName is not null)
                SpriteText.drawStringWithScrollCenteredAt(b, Current.DisplayName, xPositionOnScreen + (width / 2), yPositionOnScreen + height - 180);

            drawMouse(b);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y);

            if (LeftArrow.containsPoint(x, y) && canPageLeft()) {
                Index--;
                Game1.playSound("shwip");
                updatePreview();

            } else if (RightArrow.containsPoint(x, y) && canPageRight()) {
                Index++;
                Game1.playSound("shwip");
                updatePreview();

            } else if (CancelButton.containsPoint(x, y))
                exitThisMenu(true);

            else if (ConfirmButton.containsPoint(x, y)) {
                Confirmed = true;
                Game1.playSound("coin");
                exitThisMenu(false);
            }
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            updatePosition();
            createClickTableTextures();
        }

        public void updatePosition()
        {
            width = 1000 + IClickableMenu.borderWidth * 2;
            height = 600 + IClickableMenu.borderWidth * 2;
            xPositionOnScreen = Game1.uiViewport.Width / 2 - (1000 + IClickableMenu.borderWidth * 2) / 2;
            yPositionOnScreen = Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2;
        }

        public override void performHoverAction(int x, int y)
        {
            LeftArrow.tryHover(x, y, 0.5f);
            RightArrow.tryHover(x, y, 0.5f);
            CancelButton.tryHover(x, y, 0.15f);
            ConfirmButton.tryHover(x, y, 0.15f);
        }

        private bool canPageLeft()
            => Index > 0;

        private bool canPageRight()
            => Index < (Models.Count - 1);
    }
}
