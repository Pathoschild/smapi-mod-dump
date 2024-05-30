/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.Framework.InputHandling;
using SkillPrestige.Framework.Menus.Elements.Buttons;
using SkillPrestige.Logging;
using SkillPrestige.Professions;
using SpaceCore;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
// ReSharper disable PossibleLossOfFraction

namespace SkillPrestige.Framework.Menus
{
    /// <summary>Represents a menu where players can choose to prestige a skill and select prestige awards.</summary>
    internal class PrestigeMenu : IClickableMenu, IInputHandler
    {
        public readonly Skill Skill;
        private readonly Prestige Prestige;
        private PrestigeButton PrestigeButton;
        private Vector2 ProfessionButtonRowStartLocation;
        private Vector2 PrestigePointBonusLocation;
        private const int RowPadding = Game1.tileSize / 3;
        private int LeftProfessionStartingXLocation;
        private readonly IList<MinimalistProfessionButton> ProfessionButtons = new List<MinimalistProfessionButton>();
        private static int Offset => 4 * Game1.pixelZoom;

        private const string CostText = "Cost:";
        private int DebounceTimer = 10;
        private int XSpaceAvailableForProfessionButtons;

        public PrestigeMenu(Rectangle bounds, Skill skill, Prestige prestige)
            : base(bounds.X, bounds.Y, bounds.Width, bounds.Height, true)
        {
            Logger.LogVerbose($"New {skill.Type.Name} Prestige Menu created.");
            this.Skill = skill;
            this.Prestige = prestige;
            this.InitiatePrestigeButton();
            this.SetupInitialComponentIds();
        }

        private List<ClickableComponent> GetAllClickableComponents()
        {
            var list = new List<ClickableComponent>
            {
                this.upperRightCloseButton,
                this.PrestigeButton.ClickableTextureComponent,
            };
            list.AddRange(this.ProfessionButtons.Select(x => (ClickableComponent)x.ClickableTextureComponent));
            return list;
        }

        private void SetupInitialComponentIds()
        {
            this.PrestigeButton.ClickableTextureComponent.myID = 1;
            this.PrestigeButton.ClickableTextureComponent.upNeighborID = this.upperRightCloseButton.myID;
            this.PrestigeButton.ClickableTextureComponent.rightNeighborID = this.upperRightCloseButton.myID;

            this.upperRightCloseButton.downNeighborID = this.PrestigeButton.ClickableTextureComponent.myID;
            this.upperRightCloseButton.leftNeighborID = this.PrestigeButton.ClickableTextureComponent.myID;
        }

        /// <summary>Raised after the player moves the in-game cursor.</summary>
        /// <param name="e">The event data.</param>
        public void OnCursorMoved(CursorMovedEventArgs e)
        {
            if (this.DebounceTimer > 0)
                return;

            foreach (var button in this.ProfessionButtons)
                button.OnCursorMoved(e);
            this.PrestigeButton.OnCursorMoved(e);
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="e">The event data.</param>
        /// <param name="isClick">Whether the button press is a click.</param>
        public void OnButtonPressed(ButtonPressedEventArgs e, bool isClick)
        {
            if (this.DebounceTimer > 0)
                return;

            foreach (var button in this.ProfessionButtons)
                button.OnButtonPressed(e, isClick);
            this.PrestigeButton.OnButtonPressed(e, isClick);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.DebounceTimer > 0)
                return;
            base.receiveLeftClick(x, y, playSound);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            if (this.DebounceTimer > 0) this.DebounceTimer--;

            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);
            this.upperRightCloseButton?.draw(spriteBatch);
            this.DrawHeader(spriteBatch);
            this.DrawPrestigePoints(spriteBatch);
            this.DrawPrestigePointBonus(spriteBatch);
            this.PrestigeButton.Draw(spriteBatch);
            this.DrawLevelFiveProfessionCost(spriteBatch);
            if (!this.ProfessionButtons.Any()) this.InitiateProfessionButtons();
            this.UpdateProfessionButtonAvailability();
            this.DrawProfessionButtons(spriteBatch);
            this.DrawLevelTenProfessionCost(spriteBatch);
            this.DrawButtonHoverText(spriteBatch);
            Mouse.DrawCursor(spriteBatch);
        }

        private static int ProfessionButtonHeight(Profession profession)
        {
            int iconHeight = profession.IconSourceRectangle.Height * Game1.pixelZoom;
            int textHeight = Game1.dialogueFont.MeasureString(string.Join(Environment.NewLine, profession.DisplayName.Split(' '))).Y.Ceiling();
            return Offset * 3 + iconHeight + textHeight;
        }

        private int GetRowHeight<T>() where T : Profession
        {
            return this.Skill.Professions.Where(x => x is T).Select(ProfessionButtonHeight).Max();
        }

        private int CostTextYOffset<T>() where T : Profession
        {
            return ((double)this.GetRowHeight<T>() / 2 - Game1.dialogueFont.MeasureString(CostText).Y / 2).Floor();
        }

        private void InitiatePrestigeButton()
        {
            Logger.LogVerbose("Prestige menu - Initiating prestige button...");
            const int yOffset = 3;
            const int buttonWidth = 100 * Game1.pixelZoom;
            const int buttonHeight = 20 * Game1.pixelZoom;
            int rightEdgeOfDialog = this.xPositionOnScreen + this.width;
            var bounds = new Rectangle(rightEdgeOfDialog - spaceToClearSideBorder - buttonWidth, this.yPositionOnScreen + yOffset + (Game1.tileSize * 3.15).Floor(), buttonWidth, buttonHeight);

            bool prestigeButtonDisabled = true;
            if (PerSaveOptions.Instance.PainlessPrestigeMode)
            {
                if (this.Skill.GetSkillExperience() >=
                    15000 + PerSaveOptions.Instance.ExperienceNeededPerPainlessPrestige)
                {
                    prestigeButtonDisabled = false;
                }

            }
            else
            {
                if (this.Skill.GetSkillLevel() >= 10)
                {
                    if (!this.Skill.NewLevelForSkillExists())
                        prestigeButtonDisabled = false;
                }
            }

            this.PrestigeButton = new PrestigeButton(prestigeButtonDisabled, this.Skill)
            {
                Bounds = bounds,
            };
            Logger.LogVerbose("Prestige menu - Prestige button initiated.");
        }

        private void InitiateProfessionButtons()
        {
            Logger.LogVerbose("Prestige menu - Initiating profession buttons...");
            this.XSpaceAvailableForProfessionButtons = this.xPositionOnScreen + this.width - spaceToClearSideBorder * 2 - this.LeftProfessionStartingXLocation;
            this.InitiateLevelFiveProfessionButtons();
            this.InitiateLevelTenProfessionButtons();
            this.allClickableComponents = this.GetAllClickableComponents();
            Logger.LogVerbose("Prestige menu - Profession button initiated.");
        }

        private static int ProfessionButtonWidth(Profession profession)
        {
            return Game1.dialogueFont.MeasureString(string.Join(Environment.NewLine, profession.DisplayName.Split(' '))).X.Ceiling() + Offset * 2;
        }

        private void InitiateLevelFiveProfessionButtons()
        {
            Logger.LogVerbose("Prestige menu - Initiating level 5 profession buttons...");
            int leftProfessionButtonXCenter = this.LeftProfessionStartingXLocation + this.XSpaceAvailableForProfessionButtons / 4;
            int rightProfessionButtonXCenter = this.LeftProfessionStartingXLocation + (this.XSpaceAvailableForProfessionButtons * .75d).Floor();
            var firstProfession = this.Skill.Professions.First(x => x is TierOneProfession);

            var firstProfessionButton = new MinimalistProfessionButton
            {
                Bounds = new Rectangle(leftProfessionButtonXCenter - ProfessionButtonWidth(firstProfession) / 2,
                    (int)this.ProfessionButtonRowStartLocation.Y, ProfessionButtonWidth(firstProfession),
                    ProfessionButtonHeight(firstProfession)),
                CanBeAfforded = this.Prestige.PrestigePoints >= PerSaveOptions.Instance.CostOfTierOnePrestige,
                IsObtainable = true,
                Selected = this.Prestige.PrestigeProfessionsSelected.Contains(firstProfession.Id),
                Profession = firstProfession,
                ClickableTextureComponent =
                {
                    myID = 5,
                    upNeighborID = this.PrestigeButton.ClickableTextureComponent.myID
                }
            };

            this.PrestigeButton.ClickableTextureComponent.leftNeighborID = firstProfessionButton.ClickableTextureComponent.myID;



            var secondProfession = this.Skill.Professions.Where(x => x is TierOneProfession).Skip(1).First();

            var secondProfessionButton = new MinimalistProfessionButton
            {

                Bounds = new Rectangle(rightProfessionButtonXCenter - ProfessionButtonWidth(secondProfession) / 2,
                    (int)this.ProfessionButtonRowStartLocation.Y, ProfessionButtonWidth(secondProfession),
                    ProfessionButtonHeight(secondProfession)),
                CanBeAfforded = this.Prestige.PrestigePoints >= PerSaveOptions.Instance.CostOfTierOnePrestige,
                IsObtainable = true,
                Selected = this.Prestige.PrestigeProfessionsSelected.Contains(secondProfession.Id),
                Profession = secondProfession,
                ClickableTextureComponent =
                {
                    myID = 6,
                    leftNeighborID = firstProfessionButton.ClickableTextureComponent.myID,
                    upNeighborID = this.PrestigeButton.ClickableTextureComponent.myID
                }
            };

            firstProfessionButton.ClickableTextureComponent.rightNeighborID = secondProfessionButton.ClickableTextureComponent.myID;
            this.PrestigeButton.ClickableTextureComponent.downNeighborID = secondProfessionButton.ClickableTextureComponent.myID;
            this.ProfessionButtons.Add(firstProfessionButton);
            this.ProfessionButtons.Add(secondProfessionButton);
            Logger.LogVerbose("Prestige menu - Level 5 profession buttons initiated.");
        }

        private void InitiateLevelTenProfessionButtons()
        {
            Logger.LogVerbose("Prestige menu - Initiating level 10 profession buttons...");
            int nextId = 10;
            int buttonCenterIndex = 1;
            bool canBeAfforded = this.Prestige.PrestigePoints >= PerSaveOptions.Instance.CostOfTierTwoPrestige;
            foreach (var profession in this.Skill.Professions.Where(x => x is TierTwoProfession))
            {
                var tierTwoProfession = (TierTwoProfession)profession;
                var professionButton = new MinimalistProfessionButton
                {
                    Bounds = new Rectangle(
                        this.LeftProfessionStartingXLocation +
                        (this.XSpaceAvailableForProfessionButtons * (buttonCenterIndex / 8d)).Floor() -
                        ProfessionButtonWidth(profession) / 2,
                        (int)this.ProfessionButtonRowStartLocation.Y + this.GetRowHeight<TierOneProfession>() +
                        RowPadding, ProfessionButtonWidth(profession), ProfessionButtonHeight(profession)),
                    CanBeAfforded = canBeAfforded,
                    IsObtainable =
                        this.Prestige.PrestigeProfessionsSelected.Contains(tierTwoProfession.TierOneProfession.Id),
                    Selected = this.Prestige.PrestigeProfessionsSelected.Contains(tierTwoProfession.Id),
                    Profession = tierTwoProfession
                };
                    var firstProfessionButton = this.ProfessionButtons.First(x => x.Profession.LevelAvailableAt == 5);
                    var secondProfessionButton = this.ProfessionButtons.Last(x => x.Profession.LevelAvailableAt == 5);
                    professionButton.ClickableTextureComponent.myID = nextId;
                    switch (nextId)
                    {
                        case 10:
                            firstProfessionButton.ClickableTextureComponent.leftNeighborID = professionButton.ClickableTextureComponent.myID;
                            firstProfessionButton.ClickableTextureComponent.downNeighborID = professionButton.ClickableTextureComponent.myID;
                            professionButton.ClickableTextureComponent.upNeighborID = firstProfessionButton.ClickableTextureComponent.myID;
                            break;
                        case 11:
                            var firstTierTwoProfessionButton = this.ProfessionButtons.Last(x => x.Profession.LevelAvailableAt == 10);
                            professionButton.ClickableTextureComponent.leftNeighborID = firstTierTwoProfessionButton.ClickableTextureComponent.myID;
                            professionButton.ClickableTextureComponent.upNeighborID = firstProfessionButton.ClickableTextureComponent.myID;
                            firstTierTwoProfessionButton.ClickableTextureComponent.rightNeighborID = professionButton.ClickableTextureComponent.myID;
                            break;
                        case 12:
                            var secondTierTwoProfessionButton = this.ProfessionButtons.Last(x => x.Profession.LevelAvailableAt == 10);
                            professionButton.ClickableTextureComponent.leftNeighborID = secondTierTwoProfessionButton.ClickableTextureComponent.myID;
                            professionButton.ClickableTextureComponent.upNeighborID = secondProfessionButton.ClickableTextureComponent.myID;
                            secondProfessionButton.ClickableTextureComponent.downNeighborID = professionButton.ClickableTextureComponent.myID;
                            secondTierTwoProfessionButton.ClickableTextureComponent.rightNeighborID = professionButton.ClickableTextureComponent.myID;
                            break;
                        case 13:
                            var thirdTierTwoProfessionButton = this.ProfessionButtons.Last(x => x.Profession.LevelAvailableAt == 10);
                            professionButton.ClickableTextureComponent.leftNeighborID = thirdTierTwoProfessionButton.ClickableTextureComponent.myID;
                            professionButton.ClickableTextureComponent.upNeighborID = secondProfessionButton.ClickableTextureComponent.myID;
                            secondProfessionButton.ClickableTextureComponent.rightNeighborID = professionButton.ClickableTextureComponent.myID;
                            thirdTierTwoProfessionButton.ClickableTextureComponent.rightNeighborID = professionButton.ClickableTextureComponent.myID;
                            break;
                    }
                    nextId++;

                this.ProfessionButtons.Add(professionButton);
                buttonCenterIndex += 2;
            }
            Logger.LogVerbose("Prestige menu - Level 10 profession buttons initiated.");
        }

        private void UpdateProfessionButtonAvailability()
        {
            foreach (var button in this.ProfessionButtons)
            {
                button.CanBeAfforded = this.Prestige.PrestigePoints >= PerSaveOptions.Instance.CostOfTierTwoPrestige || button.Profession is TierOneProfession && this.Prestige.PrestigePoints >= PerSaveOptions.Instance.CostOfTierOnePrestige;
                button.IsObtainable = button.Profession is TierOneProfession || this.Prestige.PrestigeProfessionsSelected.Contains(((TierTwoProfession)button.Profession).TierOneProfession.Id);
            }
        }

        private void DrawHeader(SpriteBatch spriteBatch)
        {
            string title = $"{this.Skill.Type.Name} Prestige";
            this.DrawSkillIcon(spriteBatch, new Vector2(this.xPositionOnScreen + spaceToClearSideBorder + borderWidth, this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize / 4));
            spriteBatch.DrawString(Game1.dialogueFont, title, new Vector2(this.xPositionOnScreen + this.width / 2 - Game1.dialogueFont.MeasureString(title).X / 2f, this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize / 4), Game1.textColor);
            this.DrawSkillIcon(spriteBatch, new Vector2(this.xPositionOnScreen + this.width - spaceToClearSideBorder - borderWidth - Game1.tileSize, this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize / 4));
            this.drawHorizontalPartition(spriteBatch, this.yPositionOnScreen + (Game1.tileSize * 2.5).Floor());
        }

        private void DrawSkillIcon(SpriteBatch spriteBatch, Vector2 location)
        {
            Utility.drawWithShadow(spriteBatch, this.Skill.SkillIconTexture, location, this.Skill.SourceRectangleForSkillIcon, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, false, 0.88f);
        }

        private void DrawPrestigePoints(SpriteBatch spriteBatch)
        {
            const string pointText = "Prestige Points:";
            const int yOffset = 5 * Game1.pixelZoom;
            int yLocation = this.yPositionOnScreen + yOffset + (Game1.tileSize * 3.15).Floor();
            var textLocation = new Vector2(this.xPositionOnScreen + spaceToClearSideBorder / 2 + borderWidth, yLocation);
            spriteBatch.DrawString(Game1.dialogueFont, pointText, textLocation, Game1.textColor);
            int xPadding = (Game1.pixelZoom * 4.25).Floor();
            int prestigePointWidth = (NumberSprite.getWidth(PerSaveOptions.Instance.CostOfTierTwoPrestige) * 3.0).Ceiling();
            var pointNumberLocation = new Vector2(textLocation.X + Game1.dialogueFont.MeasureString(pointText).X + xPadding + prestigePointWidth, textLocation.Y + Game1.pixelZoom * 5);
            NumberSprite.draw(this.Prestige.PrestigePoints, spriteBatch, pointNumberLocation, Color.SandyBrown, 1f, .85f, 1f, 0);
            this.ProfessionButtonRowStartLocation = new Vector2(textLocation.X, textLocation.Y + Game1.dialogueFont.MeasureString(pointText).Y + RowPadding);
            this.PrestigePointBonusLocation = new Vector2(pointNumberLocation.X + prestigePointWidth + xPadding, yLocation);
        }

        private void DrawPrestigePointBonus(SpriteBatch spriteBatch)
        {
            if (PerSaveOptions.Instance.UseExperienceMultiplier)
                spriteBatch.DrawString(Game1.dialogueFont, $"({(this.Prestige.PrestigePoints * PerSaveOptions.Instance.ExperienceMultiplier * 100).Floor()}% XP bonus)", this.PrestigePointBonusLocation, Game1.textColor);
        }

        private void DrawLevelFiveProfessionCost(SpriteBatch spriteBatch)
        {
            var costTextLocation = this.ProfessionButtonRowStartLocation;
            costTextLocation.Y += this.CostTextYOffset<TierOneProfession>();
            spriteBatch.DrawString(Game1.dialogueFont, CostText, costTextLocation, Game1.textColor);
            var pointNumberLocation = new Vector2(costTextLocation.X + Game1.dialogueFont.MeasureString(CostText).X + (NumberSprite.getWidth(PerSaveOptions.Instance.CostOfTierOnePrestige) * 3.0).Ceiling(), costTextLocation.Y + Game1.pixelZoom * 5);
            NumberSprite.draw(PerSaveOptions.Instance.CostOfTierOnePrestige, spriteBatch, pointNumberLocation, Color.SandyBrown, 1f, .85f, 1f, 0);
            if (this.LeftProfessionStartingXLocation == 0)
                this.LeftProfessionStartingXLocation = pointNumberLocation.X.Ceiling() + NumberSprite.digitWidth;
        }

        private void DrawLevelTenProfessionCost(SpriteBatch spriteBatch)
        {
            float firstRowBottomLocation = this.ProfessionButtonRowStartLocation.Y + this.GetRowHeight<TierOneProfession>();
            var costTextLocation = new Vector2(this.ProfessionButtonRowStartLocation.X, firstRowBottomLocation + this.CostTextYOffset<TierTwoProfession>() + RowPadding);
            spriteBatch.DrawString(Game1.dialogueFont, CostText, costTextLocation, Game1.textColor);
            var pointNumberLocation = new Vector2(costTextLocation.X + Game1.dialogueFont.MeasureString(CostText).X + (NumberSprite.getWidth(PerSaveOptions.Instance.CostOfTierTwoPrestige) * 3.0).Ceiling(), costTextLocation.Y + Game1.pixelZoom * 5);
            NumberSprite.draw(PerSaveOptions.Instance.CostOfTierTwoPrestige, spriteBatch, pointNumberLocation, Color.SandyBrown, 1f, .85f, 1f, 0);
        }

        private void DrawProfessionButtons(SpriteBatch spriteBatch)
        {
            foreach (var button in this.ProfessionButtons)
            {
                button.Draw(spriteBatch);
            }
        }

        private void DrawButtonHoverText(SpriteBatch spriteBatch)
        {
            foreach (var button in this.ProfessionButtons)
            {
                button.DrawHoverText(spriteBatch);
            }
            this.PrestigeButton.DrawHoverText(spriteBatch);
        }
    }
}
