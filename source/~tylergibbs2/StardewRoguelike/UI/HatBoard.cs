/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewRoguelike.HatQuests;
using StardewRoguelike.VirtualProperties;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace StardewRoguelike.UI
{
    public class HatBoard : IClickableMenu
    {
        private readonly Texture2D BoardTexture;

        private List<HatQuest> Quests { get; } = HatQuest.PickThree();

        private bool AcceptedQuest { get; set; } = false;

        private readonly ClickableComponent acceptLeftQuestButton;
        private readonly ClickableComponent acceptMiddleQuestButton;
        private readonly ClickableComponent acceptRightQuestButton;

        private Vector2 acceptTextSize = Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest"));

        public HatBoard() : base(0, 0, 0, 0, showUpperRightCloseButton: true)
        {
            BoardTexture = Game1.temporaryContent.Load<Texture2D>("TileSheets\\HatBoard");

            width = 1352;
            height = 792;

            acceptLeftQuestButton = new(Rectangle.Empty, "")
            {
                myID = 100,
                rightNeighborID = 101,
            };
            acceptMiddleQuestButton = new(Rectangle.Empty, "")
            {
                myID = 101,
                leftNeighborID = 100,
                rightNeighborID = 102,
            };
            acceptRightQuestButton = new(Rectangle.Empty, "")
            {
                myID = 102,
                leftNeighborID = 101,
            };

            CalculatePosition();

            allClickableComponents ??= new();

            allClickableComponents.Add(acceptLeftQuestButton);
            allClickableComponents.Add(acceptMiddleQuestButton);
            allClickableComponents.Add(acceptRightQuestButton);

            if (Game1.options.SnappyMenus)
                snapToDefaultClickableComponent();
        }

        private void CalculatePosition()
        {
            Vector2 center = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
            xPositionOnScreen = (int)center.X;
            yPositionOnScreen = (int)center.Y;
            initialize(xPositionOnScreen, yPositionOnScreen, width, height, showUpperRightCloseButton: true);

            acceptLeftQuestButton.bounds = new(xPositionOnScreen + 120, yPositionOnScreen + height - 128, (int)acceptTextSize.X + 24, (int)acceptTextSize.Y + 24);
            acceptMiddleQuestButton.bounds = new(xPositionOnScreen + width / 2 - 135, yPositionOnScreen + height - 128, (int)acceptTextSize.X + 24, (int)acceptTextSize.Y + 24);
            acceptRightQuestButton.bounds = new(xPositionOnScreen + width - 360, yPositionOnScreen + height - 128, (int)acceptTextSize.X + 24, (int)acceptTextSize.Y + 24);
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            CalculatePosition();
        }

        public override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = acceptMiddleQuestButton;
            snapCursorToCurrentSnappedComponent();
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            Game1.playSound("bigDeSelect");
            exitThisMenu();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);

            if (AcceptedQuest)
                return;

            if (acceptLeftQuestButton.containsPoint(x, y))
            {
                Game1.playSound("newArtifact");
                Game1.player.set_FarmerActiveHatQuest(Quests[0]);
                AcceptedQuest = true;

                acceptMiddleQuestButton.visible = false;
                acceptRightQuestButton.visible = false;
            }
            else if (acceptMiddleQuestButton.containsPoint(x, y))
            {
                Game1.playSound("newArtifact");
                Game1.player.set_FarmerActiveHatQuest(Quests[1]);
                AcceptedQuest = true;

                acceptLeftQuestButton.visible = false;
                acceptRightQuestButton.visible = false;
            }
            else if (acceptRightQuestButton.containsPoint(x, y))
            {
                Game1.playSound("newArtifact");
                Game1.player.set_FarmerActiveHatQuest(Quests[2]);
                AcceptedQuest = true;

                acceptLeftQuestButton.visible = false;
                acceptMiddleQuestButton.visible = false;
            }
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);

            if (AcceptedQuest)
                return;

            float oldScale = acceptLeftQuestButton.scale;
            acceptLeftQuestButton.scale = acceptLeftQuestButton.containsPoint(x, y) ? 1.5f : 1f;
            if (acceptLeftQuestButton.scale > oldScale)
                Game1.playSound("Cowboy_gunshot");
            oldScale = acceptMiddleQuestButton.scale;
            acceptMiddleQuestButton.scale = acceptMiddleQuestButton.containsPoint(x, y) ? 1.5f : 1f;
            if (acceptMiddleQuestButton.scale > oldScale)
                Game1.playSound("Cowboy_gunshot");
            oldScale = acceptRightQuestButton.scale;
            acceptRightQuestButton.scale = acceptRightQuestButton.containsPoint(x, y) ? 1.5f : 1f;
            if (acceptRightQuestButton.scale > oldScale)
                Game1.playSound("Cowboy_gunshot");
        }


        private void DrawHat(SpriteBatch b, HatQuest quest, int x, int y, float alpha)
        {
            b.Draw(FarmerRenderer.hatsTexture, new Vector2(x, y), quest.GetHatSourceRect(), Color.White * alpha, 0f, new Vector2(10f, 10f), 8f, SpriteEffects.None, 0.1f);
        }

        private void DrawQuestDetails(SpriteBatch b, HatQuest quest, int x, float alpha)
        {
            Vector2 hatNameSize = Game1.dialogueFont.MeasureString(quest.GetHatName());

            int smallHatOffset = quest.IsBigHat() ? 0 : 32;
            DrawHat(b, quest, x + 100, yPositionOnScreen + 220 + smallHatOffset, alpha);
            b.DrawString(Game1.dialogueFont, quest.GetHatName(), new Vector2(x + 100 - (int)hatNameSize.X / 2, yPositionOnScreen + 300), Game1.textColor * alpha, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);

            string parsed = Game1.parseText($"{quest.GetHatBuffDescription()}\n\n\n{quest.GetHatQuestDetails()}", Game1.smallFont, 320);
            Vector2 parsedSize = Game1.smallFont.MeasureString(parsed);
            b.DrawString(Game1.smallFont, parsed, new Vector2(x + 100 - (int)parsedSize.X / 2, yPositionOnScreen + 350), Game1.textColor * alpha, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            b.Draw(BoardTexture, new Vector2(xPositionOnScreen, yPositionOnScreen), new Rectangle(0, 0, 338, 198), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

            DrawQuestDetails(b, Quests[0], xPositionOnScreen + 150, acceptLeftQuestButton.visible ? 1f : 0.5f);
            DrawQuestDetails(b, Quests[1], xPositionOnScreen + 582, acceptMiddleQuestButton.visible ? 1f : 0.5f);
            DrawQuestDetails(b, Quests[2], xPositionOnScreen + 1015, acceptRightQuestButton.visible ? 1f : 0.5f);

            if (!AcceptedQuest)
            {
                drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 373, 9, 9), acceptLeftQuestButton.bounds.X, acceptLeftQuestButton.bounds.Y, acceptLeftQuestButton.bounds.Width, acceptLeftQuestButton.bounds.Height, (acceptLeftQuestButton.scale > 1f) ? Color.LightPink : Color.White, 4f * acceptLeftQuestButton.scale);
                Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:AcceptQuest"), Game1.dialogueFont, new Vector2(acceptLeftQuestButton.bounds.X + 12, acceptLeftQuestButton.bounds.Y + (LocalizedContentManager.CurrentLanguageLatin ? 16 : 12)), Game1.textColor);

                drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 373, 9, 9), acceptMiddleQuestButton.bounds.X, acceptMiddleQuestButton.bounds.Y, acceptMiddleQuestButton.bounds.Width, acceptMiddleQuestButton.bounds.Height, (acceptMiddleQuestButton.scale > 1f) ? Color.LightPink : Color.White, 4f * acceptMiddleQuestButton.scale);
                Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:AcceptQuest"), Game1.dialogueFont, new Vector2(acceptMiddleQuestButton.bounds.X + 12, acceptMiddleQuestButton.bounds.Y + (LocalizedContentManager.CurrentLanguageLatin ? 16 : 12)), Game1.textColor);

                drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 373, 9, 9), acceptRightQuestButton.bounds.X, acceptRightQuestButton.bounds.Y, acceptRightQuestButton.bounds.Width, acceptRightQuestButton.bounds.Height, (acceptRightQuestButton.scale > 1f) ? Color.LightPink : Color.White, 4f * acceptRightQuestButton.scale);
                Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:AcceptQuest"), Game1.dialogueFont, new Vector2(acceptRightQuestButton.bounds.X + 12, acceptRightQuestButton.bounds.Y + (LocalizedContentManager.CurrentLanguageLatin ? 16 : 12)), Game1.textColor);
            }

            base.draw(b);
            drawMouse(b);
        }
    }
}
