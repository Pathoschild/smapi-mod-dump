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
using PurrplingCore;
using QuestFramework.Extensions;
using QuestFramework.Quests;
using QuestFramework.Structures;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Quests;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestFramework.Framework.Menus
{
    internal class ManagedQuestLog : QuestLog
    {
        protected internal static IReflectionHelper Reflection => QuestFrameworkMod.Instance.Helper.Reflection;

        private int lastQuestPage = -1;
        private Item rewardItem;
        private List<CustomQuestObjective> _currentObjectives;

        protected List<List<IQuest>> Pages => Reflection.GetField<List<List<IQuest>>>(this, "pages").GetValue();
        protected int QuestPage => Reflection.GetField<int>(this, "questPage").GetValue();
        protected int CurrentPage => Reflection.GetField<int>(this, "currentPage").GetValue();

        public Quest CurrentQuest => this._shownQuest as Quest;

        public override void update(GameTime time)
        {
            if (this.lastQuestPage != this.QuestPage)
            {
                this.lastQuestPage = this.QuestPage;
                this.UpdateRewardBoxItem();
            }

            base.update(time);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.IsClickedOnRewardBox(x, y))
            {
                switch (this.CurrentQuest.AsManagedQuest().RewardType)
                {
                    case RewardType.Money:
                        Game1.player.Money += this.CurrentQuest.GetMoneyReward();
                        Game1.playSound("purchaseRepeat");
                        break;
                    case RewardType.Object:
                    case RewardType.Weapon:
                        Game1.playSound("coin");
                        Game1.player.addItemByMenuIfNecessary(this.rewardItem, null);
                        this.rewardItem = null;
                        break;
                }

                this.CurrentQuest.moneyReward.Value = 0;
                this.CurrentQuest.destroy.Value = true;

                return;
            }

            base.receiveLeftClick(x, y, playSound);
            this.LoadManagedQuestObjectives();
        }

        private void LoadManagedQuestObjectives()
        {
            if (this.QuestPage != -1 && this.QuestPage != this.lastQuestPage)
            {
                if (this._shownQuest is Quest quest && quest.IsManaged())
                {
                    var managedQuest = quest.AsManagedQuest();

                    managedQuest.UpdateCurrentObjectives();
                    this._currentObjectives = managedQuest.GetCurrentObjectives();
                }
            }
        }

        private bool IsClickedOnRewardBox(int x, int y)
        {
            return this.QuestPage != -1
                && this.CurrentQuest != null
                && this.CurrentQuest.IsManaged()
                && this.CurrentQuest.completed.Value 
                && this.CurrentQuest.moneyReward.Value > 0 
                && this.rewardBox.containsPoint(x, y);
        }

        private bool IsItemReward(RewardType rewardType)
        {
            return rewardType == RewardType.Object 
                || rewardType == RewardType.Weapon;
        }

        private void UpdateRewardBoxItem()
        {
            var managedQuest = this.CurrentQuest?.AsManagedQuest();

            this.rewardItem = null;

            switch (managedQuest?.RewardType)
            {
                case RewardType.Object:
                    this.rewardItem = new StardewValley.Object(
                        Vector2.Zero, managedQuest.Reward,
                        managedQuest.RewardAmount > 0 ? managedQuest.RewardAmount : 1);
                    break;
                case RewardType.Weapon:
                    this.rewardItem = new MeleeWeapon(managedQuest.Reward);
                    break;
            }
        }

        public override void draw(SpriteBatch b)
        {
            if (this.QuestPage != -1 && this.CurrentQuest != null && this.CurrentQuest.IsManaged())
            {
                this.DrawWindow(b);
                this.DrawManagedQuestDetails(b);
                this.DrawCursors(b);

                return;
            }

            base.draw(b);
        }

        private void DrawCursors(SpriteBatch b)
        {
            if (this.upperRightCloseButton != null || this.shouldDrawCloseButton())
            {
                this.upperRightCloseButton.draw(b);
            }

            if (this.QuestPage != -1)
            {
                this.backButton.draw(b);
            }

            Game1.mouseCursorTransparency = 1f;
            this.drawMouse(b);

            var hoverText = Reflection.GetField<string>(this, "hoverText").GetValue();
            if (hoverText.Length > 0)
            {
                drawHoverText(b, hoverText, Game1.dialogueFont);
            }
        }

        private void DrawWindow(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            SpriteText.drawStringWithScrollCenteredAt(
                b, Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11373"),
                this.xPositionOnScreen + (this.width / 2), this.yPositionOnScreen - 64, "", 1f, -1, 0, 0.88f, false);

            if (this.QuestPage != -1 && this.CurrentQuest.IsManaged() && this.CurrentQuest.AsManagedQuest().Texture != null)
            {
                b.Draw(
                    texture: this.CurrentQuest.AsManagedQuest().Texture,
                    destinationRectangle: new Rectangle(this.xPositionOnScreen,this.yPositionOnScreen, this.width, this.height),
                    color: Color.White);
            }
            else
            {
                drawTextureBox(
                    b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), this.xPositionOnScreen, this.yPositionOnScreen,
                    this.width, this.height, Color.White, 4f, true);
            }
        }

        private void DrawManagedQuestDetails(SpriteBatch b)
        {
            var managedQuest = this.CurrentQuest.AsManagedQuest();

            SpriteText.drawStringHorizontallyCenteredAt(
                b, this.CurrentQuest.questTitle,
                this.xPositionOnScreen
                + (this.width / 2)
                + (!this.CurrentQuest.dailyQuest.Value || this.CurrentQuest.daysLeft.Value <= 0 ? 0 : Math.Max(
                    32, SpriteText.getWidthOfString(this.CurrentQuest.questTitle, 999999) / 3) - 32),
                this.yPositionOnScreen + 32, 
                color: managedQuest.Colors?.TitleColor ?? -1);

            if (this.CurrentQuest.IsTimedQuest() && this.CurrentQuest.GetDaysLeft() > 0)
            {
                var left_text = Game1.parseText((this.CurrentQuest.GetDaysLeft() > 1)
                    ? Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11374", this.CurrentQuest.GetDaysLeft())
                    : (Game1.IsEnglish()
                        ? "Final Day"
                        : Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11375", this.CurrentQuest.GetDaysLeft())),
                    Game1.dialogueFont, base.width - 128);

                Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(this.xPositionOnScreen + 32, this.yPositionOnScreen + 48 - 8), new Rectangle(410, 501, 9, 9), Color.White, 0f, Vector2.Zero, 4f, flipped: false, 0.99f);

                if (managedQuest.Colors != null && managedQuest.Colors.ObjectiveColor != -1)
                {
                    b.DrawString(Game1.dialogueFont, left_text,
                        new Vector2(this.xPositionOnScreen + 80, this.yPositionOnScreen + 48 - 8),
                        SpriteText.getColorFromIndex(managedQuest.Colors.ObjectiveColor),
                        0.0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                }
                else
                {
                    Utility.drawTextWithShadow(
                    b, left_text, Game1.dialogueFont,
                    new Vector2(this.xPositionOnScreen + 80, this.yPositionOnScreen + 48 - 8),
                    Game1.textColor);
                }
            }

            string description = Game1.parseText(this.CurrentQuest.GetDescription(), Game1.dialogueFont, base.width - 128);
            Rectangle cached_scissor_rect = b.GraphicsDevice.ScissorRectangle;
            Vector2 description_size = Game1.dialogueFont.MeasureString(description);
            Rectangle scissor_rect = default(Rectangle);
            scissor_rect.X = this.xPositionOnScreen + 32;
            scissor_rect.Y = this.yPositionOnScreen + 96;
            scissor_rect.Height = this.yPositionOnScreen + this.height - 32 - scissor_rect.Y;
            scissor_rect.Width = base.width - 64;
            this._scissorRectHeight = scissor_rect.Height;
            scissor_rect = Utility.ConstrainScissorRectToScreen(scissor_rect);
            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, new RasterizerState
            {
                ScissorTestEnable = true
            });
            Game1.graphics.GraphicsDevice.ScissorRectangle = scissor_rect;
            Utility.drawTextWithShadow(
                    b, description, Game1.dialogueFont,
                    new Vector2(this.xPositionOnScreen + 64, (float)this.yPositionOnScreen - this.scrollAmount + 96f),
                    managedQuest.Colors != null && managedQuest.Colors.TextColor != -1
                        ? SpriteText.getColorFromIndex(managedQuest.Colors.TextColor)
                        : Game1.textColor, shadowIntensity: 0);

            float yPos = (float)(this.yPositionOnScreen + 96) + description_size.Y + 32f - this.scrollAmount;

            if (this.CurrentQuest.ShouldDisplayAsComplete())
            {
                b.End();
                b.GraphicsDevice.ScissorRectangle = cached_scissor_rect;
                b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                SpriteText.drawString(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11376"), this.xPositionOnScreen + 32 + 4, this.rewardBox.bounds.Y + 21 + 4, color: managedQuest.Colors?.TitleColor ?? -1);
                this.rewardBox.draw(b);

                if (this.CurrentQuest.moneyReward.Value <= 0)
                {
                    return;
                }

                if (this.IsItemReward(managedQuest.RewardType) && this.rewardItem != null)
                {
                    this.rewardItem.drawInMenu(
                        b, new Vector2(this.rewardBox.bounds.X + 18, this.rewardBox.bounds.Y + 18 - Game1.dialogueButtonScale / 6f), 0.9f + this.rewardBox.scale * 0.03f);
                    SpriteText.drawString(b,
                        Utils.CutText(this.rewardItem.DisplayName, 15),
                        this.xPositionOnScreen + 448, this.rewardBox.bounds.Y + 21 + 4,
                        color: managedQuest.Colors?.TitleColor ?? -1);
                }
                else if (managedQuest.RewardType == RewardType.Money)
                {
                    b.Draw(Game1.mouseCursors,
                           new Vector2(this.rewardBox.bounds.X + 16, this.rewardBox.bounds.Y + 16 - Game1.dialogueButtonScale / 2f),
                           new Rectangle?(new Rectangle(280, 410, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f,
                           SpriteEffects.None, 1f);
                    SpriteText.drawString(b,
                        Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", this.CurrentQuest.moneyReward.Value),
                        this.xPositionOnScreen + 448, this.rewardBox.bounds.Y + 21 + 4,
                        color: managedQuest.Colors?.TitleColor ?? -1);
                }
            }
            else
            {
                if (this._currentObjectives != null && this._currentObjectives.Any())
                {
                    foreach (var objective in this._currentObjectives)
                    {
                        Utility.drawWithShadow(
                            b, Game1.mouseCursors,
                            new Vector2((float)(this.xPositionOnScreen + 96) + 8f * Game1.dialogueButtonScale / 10f, yPos),
                            new Rectangle(412, 495, 5, 4), Color.White, (float)Math.PI / 2f, Vector2.Zero);

                        this.DrawObjective(b, objective, managedQuest.Colors, ref yPos);
                    }
                }
                this._contentHeight = yPos + this.scrollAmount - scissor_rect.Y;
                b.End();
                b.GraphicsDevice.ScissorRectangle = cached_scissor_rect;
                b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

                if (this.CurrentQuest.CanBeCancelled())
                {
                    this.cancelQuestButton.draw(b);
                }
                if (this.NeedsScroll())
                {
                    if (this.scrollAmount > 0f)
                    {
                        b.Draw(Game1.staminaRect, new Rectangle(scissor_rect.X, scissor_rect.Top, scissor_rect.Width, 4), Color.Black * 0.15f);
                    }
                    if (this.scrollAmount < this._contentHeight - this._scissorRectHeight)
                    {
                        b.Draw(Game1.staminaRect, new Rectangle(scissor_rect.X, scissor_rect.Bottom - 4, scissor_rect.Width, 4), Color.Black * 0.15f);
                    }
                }
            }

            if (NeedsScroll())
            {
                upArrow.draw(b);
                downArrow.draw(b);
                scrollBar.draw(b);
            }
        }

        private void DrawObjective(SpriteBatch b, CustomQuestObjective objective, QuestLogColors colors, ref float yPos)
        {
            var parsed_objective = Game1.parseText(objective.Text, Game1.dialogueFont, this.width - 128);

            if (colors != null && colors.ObjectiveColor != -1)
            {
                b.DrawString(Game1.dialogueFont,
                    parsed_objective,
                    new Vector2(this.xPositionOnScreen + 128, yPos - 8f),
                    objective.IsCompleted ? Game1.unselectedOptionColor : SpriteText.getColorFromIndex(colors.ObjectiveColor),
                    0.0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            }
            else
            {
                Utility.drawTextWithShadow(b,
                    parsed_objective,
                    Game1.dialogueFont, new Vector2(this.xPositionOnScreen + 128, yPos - 8f),
                    objective.IsCompleted ? Game1.unselectedOptionColor : Color.DarkBlue,
                    1f, -1f, -1, -1, 1f, 3);
            }

            yPos += Game1.dialogueFont.MeasureString(parsed_objective).Y;
        }
    }
}
