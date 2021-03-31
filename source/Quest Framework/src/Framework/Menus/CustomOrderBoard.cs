/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace QuestFramework.Framework.Menus
{
    public class CustomOrderBoard : IClickableMenu
    {
        private readonly Texture2D _billboardTexture;
        private readonly Rectangle _billboardSourceRectange = new Rectangle(0, 0, 338, 198);
        private readonly Rectangle _acceptButtonSourceRectangle = new Rectangle(403, 373, 9, 9);

        public const int basewidth = 338;
        public const int baseheight = 198;

        public ClickableComponent acceptLeftQuestButton;
        public ClickableComponent acceptRightQuestButton;
        public string boardType = "";
        public SpecialOrder leftOrder;
        public SpecialOrder rightOrder;

        public static readonly string[] emojiIndices = new string[38]
        {
            "Abigail",
            "Penny",
            "Maru",
            "Leah",
            "Haley",
            "Emily",
            "Alex",
            "Shane",
            "Sebastian",
            "Sam",
            "Harvey",
            "Elliott",
            "Sandy",
            "Evelyn",
            "Marnie",
            "Caroline",
            "Robin",
            "Pierre",
            "Pam",
            "Jodi",
            "Lewis",
            "Linus",
            "Marlon",
            "Willy",
            "Wizard",
            "Morris",
            "Jas",
            "Vincent",
            "Krobus",
            "Dwarf",
            "Gus",
            "Gunther",
            "George",
            "Demetrius",
            "Clint",
            "Baby",
            "Baby",
            "Bear"
        };

        public CustomOrderBoard(string board_type = "", Texture2D customTexture = null)
            : base(0, 0, 0, 0, showUpperRightCloseButton: true)
        {
            SpecialOrder.UpdateAvailableSpecialOrders(force_refresh: false);
            this.boardType = board_type;

            if (customTexture != null)
            {
                this._billboardTexture = customTexture;
            }
            else
            {
                this._billboardTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\SpecialOrdersBoard");
            }

            base.width = 1352;
            base.height = 792;
            Vector2 center = Utility.getTopLeftPositionForCenteringOnScreen(base.width, base.height);
            base.xPositionOnScreen = (int)center.X;
            base.yPositionOnScreen = (int)center.Y;
            Rectangle leftButtonBounds = new Rectangle(
                x: base.xPositionOnScreen + (base.width / 4) - 128, y: base.yPositionOnScreen + base.height - 128,
                width: (int)Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest")).X + 24,
                height: (int)Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest")).Y + 24
            );
            this.acceptLeftQuestButton = new ClickableComponent(leftButtonBounds, "")
            {
                myID = 0,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                upNeighborID = -99998,
                downNeighborID = -99998
            };
            Rectangle rightButtonBOunds = new Rectangle(
                x: base.xPositionOnScreen + base.width * 3 / 4 - 128,
                y: base.yPositionOnScreen + base.height - 128,
                width: (int)Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest")).X + 24,
                height: (int)Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest")).Y + 24
            );
            this.acceptRightQuestButton = new ClickableComponent(rightButtonBOunds, "")
            {
                myID = 1,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                upNeighborID = -99998,
                downNeighborID = -99998
            };
            this.leftOrder = Game1.player.team.GetAvailableSpecialOrder(0, this.GetOrderType());
            this.rightOrder = Game1.player.team.GetAvailableSpecialOrder(1, this.GetOrderType());
            base.upperRightCloseButton = new ClickableTextureComponent(
                bounds: new Rectangle(base.xPositionOnScreen + base.width - 20, base.yPositionOnScreen, 48, 48),
                texture: Game1.mouseCursors,
                sourceRect: new Rectangle(337, 494, 12, 12),
                scale: 4f
            );

            Game1.playSound("bigSelect");
            this.UpdateButtons();

            if (Game1.options.SnappyMenus)
            {
                base.populateClickableComponentList();
                this.snapToDefaultClickableComponent();
            }
        }

        public virtual void UpdateButtons()
        {
            bool alreadyAccepted = Game1.player.team.acceptedSpecialOrderTypes.Contains(this.GetOrderType());

            this.acceptLeftQuestButton.visible = this.leftOrder != null && !alreadyAccepted;
            this.acceptRightQuestButton.visible = this.rightOrder != null && !alreadyAccepted;
        }

        public override void snapToDefaultClickableComponent()
        {
            base.currentlySnappedComponent = base.getComponentWithID(0);
            this.snapCursorToCurrentSnappedComponent();
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            Game1.playSound("bigDeSelect");
            base.exitThisMenu();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            if (this.acceptLeftQuestButton.visible && this.acceptLeftQuestButton.containsPoint(x, y))
            {
                Game1.playSound("newArtifact");
                if (this.leftOrder != null)
                {
                    Game1.player.team.acceptedSpecialOrderTypes.Add(this.GetOrderType());
                    SpecialOrder order2 = this.leftOrder;
                    Game1.player.team.specialOrders.Add(SpecialOrder.GetSpecialOrder(order2.questKey.Value, order2.generationSeed.Value));
                    QuestFrameworkMod.Multiplayer.globalChatInfoMessage("AcceptedSpecialOrder", Game1.player.Name, order2.GetName());
                    this.UpdateButtons();
                }
            }
            else if (this.acceptRightQuestButton.visible && this.acceptRightQuestButton.containsPoint(x, y))
            {
                Game1.playSound("newArtifact");
                if (this.rightOrder != null)
                {
                    Game1.player.team.acceptedSpecialOrderTypes.Add(this.GetOrderType());
                    SpecialOrder order = this.rightOrder;
                    Game1.player.team.specialOrders.Add(SpecialOrder.GetSpecialOrder(order.questKey.Value, order.generationSeed.Value));
                    QuestFrameworkMod.Multiplayer.globalChatInfoMessage("AcceptedSpecialOrder", Game1.player.Name, order.GetName());
                    this.UpdateButtons();
                }
            }
        }

        public string GetOrderType()
        {
            return this.boardType;
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            if (Game1.questOfTheDay != null && !Game1.questOfTheDay.accepted.Value)
            {
                float oldScale = this.acceptLeftQuestButton.scale;
                this.acceptLeftQuestButton.scale = (this.acceptLeftQuestButton.bounds.Contains(x, y) ? 1.5f : 1f);
                if (this.acceptLeftQuestButton.scale > oldScale)
                    Game1.playSound("Cowboy_gunshot");

                oldScale = this.acceptRightQuestButton.scale;
                this.acceptRightQuestButton.scale = (this.acceptRightQuestButton.bounds.Contains(x, y) ? 1.5f : 1f);
                if (this.acceptRightQuestButton.scale > oldScale)
                    Game1.playSound("Cowboy_gunshot");
            }
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            b.Draw(this._billboardTexture, new Vector2(base.xPositionOnScreen, base.yPositionOnScreen),
                   this._billboardSourceRectange, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

            if (!Game1.player.team.acceptedSpecialOrderTypes.Contains(this.GetOrderType()))
            {
                SpriteText.drawStringWithScrollCenteredAt(b, Game1.content.LoadString("Strings\\UI:ChooseOne"), base.xPositionOnScreen + base.width / 2, Math.Max(10, base.yPositionOnScreen - 70), SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\UI:ChooseOne")) + 1);
            }

            if (this.leftOrder != null)
                this.DrawQuestDetails(b, this.leftOrder, base.xPositionOnScreen + 64 + 32);
            if (this.rightOrder != null)
                this.DrawQuestDetails(b, this.rightOrder, base.xPositionOnScreen + 704 + 32);

            if (this.acceptLeftQuestButton.visible)
            {
                // Button base
                drawTextureBox(b, Game1.mouseCursors, this._acceptButtonSourceRectangle,
                    this.acceptLeftQuestButton.bounds.X, this.acceptLeftQuestButton.bounds.Y,
                    this.acceptLeftQuestButton.bounds.Width, this.acceptLeftQuestButton.bounds.Height,
                    (this.acceptLeftQuestButton.scale > 1f) ? Color.LightPink : Color.White,
                    4f * this.acceptLeftQuestButton.scale);
                // Button text
                Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:AcceptQuest"), Game1.dialogueFont, new Vector2(this.acceptLeftQuestButton.bounds.X + 12, this.acceptLeftQuestButton.bounds.Y + (LocalizedContentManager.CurrentLanguageLatin ? 16 : 12)), Game1.textColor);
            }
            if (this.acceptRightQuestButton.visible)
            {
                // Button base
                drawTextureBox(b, Game1.mouseCursors, this._acceptButtonSourceRectangle,
                    this.acceptRightQuestButton.bounds.X, this.acceptRightQuestButton.bounds.Y,
                    this.acceptRightQuestButton.bounds.Width, this.acceptRightQuestButton.bounds.Height,
                    (this.acceptRightQuestButton.scale > 1f) ? Color.LightPink : Color.White,
                    4f * this.acceptRightQuestButton.scale);
                // Button text
                Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:AcceptQuest"), Game1.dialogueFont, new Vector2(this.acceptRightQuestButton.bounds.X + 12, this.acceptRightQuestButton.bounds.Y + (LocalizedContentManager.CurrentLanguageLatin ? 16 : 12)), Game1.textColor);
            }

            base.draw(b);
            Game1.mouseCursorTransparency = 1f;

            if (!Game1.options.SnappyMenus || this.acceptLeftQuestButton.visible || this.acceptRightQuestButton.visible)
            {
                base.drawMouse(b);
            }
        }

        public KeyValuePair<Texture2D, Rectangle>? GetPortraitForRequester(string requester_name)
        {
            if (requester_name != null)
            {
                for (int i = 0; i < emojiIndices.Length; i++)
                {
                    if (emojiIndices[i] == requester_name)
                    {
                        return new KeyValuePair<Texture2D, Rectangle>(ChatBox.emojiTexture, new Rectangle(i % 14 * 9, 99 + i / 14 * 9, 9, 9));
                    }
                }
            }

            return null;
        }

        public void DrawQuestDetails(SpriteBatch b, SpecialOrder order, int x)
        {
            bool dehighlight = false;
            bool found_match = false;
            foreach (SpecialOrder active_order in Game1.player.team.specialOrders)
            {
                if (active_order.questState.Value != 0)
                {
                    continue;
                }
                foreach (SpecialOrder available_order in Game1.player.team.availableSpecialOrders)
                {
                    if (!(available_order.orderType.Value != this.GetOrderType()) && active_order.questKey.Value == available_order.questKey.Value)
                    {
                        if (order.questKey != active_order.questKey)
                        {
                            dehighlight = true;
                        }
                        found_match = true;
                        break;
                    }
                }
                if (found_match)
                {
                    break;
                }
            }
            if (!found_match && Game1.player.team.acceptedSpecialOrderTypes.Contains(this.GetOrderType()))
            {
                dehighlight = true;
            }
            SpriteFont font = (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko) ? Game1.smallFont : Game1.dialogueFont;
            Color font_color = Game1.textColor;
            float shadow_intensity = 0.5f;
            float graphic_alpha = 1f;
            if (dehighlight)
            {
                font_color = Game1.textColor * 0.25f;
                shadow_intensity = 0f;
                graphic_alpha = 0.25f;
            }
            int header_y = base.yPositionOnScreen + 128;
            string order_name = order.GetName();
            KeyValuePair<Texture2D, Rectangle>? portrait_icon = this.GetPortraitForRequester(order.requester.Value);

            if (portrait_icon.HasValue)
            {
                Utility.drawWithShadow(b, portrait_icon.Value.Key, new Vector2(x, header_y), portrait_icon.Value.Value, Color.White * graphic_alpha, 0f, Vector2.Zero, 4f, flipped: false, -1f, -1, -1, shadow_intensity * 0.6f);
            }

            Utility.drawTextWithShadow(b, order_name, font, new Vector2(x + 256 - font.MeasureString(order_name).X / 2f, header_y), font_color, 1f, -1f, -1, -1, shadow_intensity);
            
            string rawDescription = order.GetDescription();
            string description = Game1.parseText(rawDescription, font, 512);
            float height = font.MeasureString(description).Y;
            float scale = 1f;
            float maxHeight = 400f;
            while (height > maxHeight && !(scale <= 0.25f))
            {
                scale -= 0.05f;
                description = Game1.parseText(rawDescription, font, (int)(512f / scale));
                height = font.MeasureString(description).Y;
            }
            Utility.drawTextWithShadow(b, description, font, new Vector2(x, base.yPositionOnScreen + 192), font_color, scale, -1f, -1, -1, shadow_intensity);

            if (dehighlight)
                return;

            int daysLeft = order.GetDaysLeft();
            int dueDateYPos = base.yPositionOnScreen + 576;
            Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(x, dueDateYPos), new Rectangle(410, 501, 9, 9), Color.White, 0f, Vector2.Zero, 4f, flipped: false, 0.99f, -1, -1, shadow_intensity * 0.6f);
            Utility.drawTextWithShadow(b, Game1.parseText((daysLeft > 1) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11374", daysLeft) : Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11375", daysLeft), Game1.dialogueFont, base.width - 128), Game1.dialogueFont, new Vector2(x + 48, dueDateYPos), font_color, 1f, -1f, -1, -1, shadow_intensity);

            int reward = -1;
            foreach (OrderReward r in order.rewards)
            {
                if (r is GemsReward gems)
                {
                    reward = gems.amount.Value;
                    break;
                }
            }

            if (reward != -1)
            {
                Utility.drawWithShadow(b, Game1.objectSpriteSheet, new Vector2(x + 512f / scale - Game1.dialogueFont.MeasureString(string.Concat(reward)).X - 12f - 60f, dueDateYPos - 8), new Rectangle(288, 561, 15, 15), Color.White, 0f, Vector2.Zero, 4f, flipped: false, 0.99f, -1, -1, shadow_intensity * 0.6f);
                Utility.drawTextWithShadow(b, Game1.parseText(string.Concat(reward), Game1.dialogueFont, base.width - 128), Game1.dialogueFont, new Vector2(x + 512f / scale - Game1.dialogueFont.MeasureString(string.Concat(reward)).X - 4f, dueDateYPos), font_color, 1f, -1f, -1, -1, shadow_intensity);
                Utility.drawTextWithShadow(b, Game1.parseText(Utility.loadStringShort("StringsFromCSFiles", "QuestLog.cs.11376"), Game1.dialogueFont, base.width - 128), Game1.dialogueFont, new Vector2(x + 512f / scale - Game1.dialogueFont.MeasureString(Utility.loadStringShort("StringsFromCSFiles", "QuestLog.cs.11376")).X + 8f, dueDateYPos - 60), font_color * 0.6f, 1f, -1f, -1, -1, shadow_intensity);
            }
        }
    }
}
