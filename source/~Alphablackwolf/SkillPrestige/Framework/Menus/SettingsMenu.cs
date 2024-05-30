/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.Framework.InputHandling;
using SkillPrestige.Framework.Menus.Elements.Buttons;
using SkillPrestige.Logging;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
// ReSharper disable PossibleLossOfFraction

namespace SkillPrestige.Framework.Menus
{
    /// <summary>Represents a menu where players can change their per-save settings.</summary>
    internal class SettingsMenu : IClickableMenu, IInputHandler
    {
        private int DebounceTimer = 10;
        private bool InputInitiated;

        private Checkbox ResetRecipesCheckbox;
        private Checkbox UseExperienceMultiplierCheckbox;
        private Checkbox PainlessPrestigeModeCheckbox;
        private IntegerEditor TierOneCostEditor;
        private IntegerEditor TierTwoCostEditor;
        private IntegerEditor PointsPerPrestigeEditor;
        private IntegerEditor ExperiencePerPainlessPrestigeEditor;

        public SettingsMenu(Rectangle bounds)
            : base(bounds.X, bounds.Y, bounds.Width, bounds.Height, true)
        {
            Logger.LogVerbose("New Settings Menu created.");
        }

        /// <summary>Raised after the player moves the in-game cursor.</summary>
        /// <param name="e">The event data.</param>
        public void OnCursorMoved(CursorMovedEventArgs e)
        {
            if (this.DebounceTimer > 0)
                return;

            this.ResetRecipesCheckbox.OnCursorMoved(e);
            this.UseExperienceMultiplierCheckbox.OnCursorMoved(e);
            this.PainlessPrestigeModeCheckbox.OnCursorMoved(e);

            this.TierOneCostEditor.OnCursorMoved(e);
            this.TierTwoCostEditor.OnCursorMoved(e);
            this.PointsPerPrestigeEditor.OnCursorMoved(e);
            this.ExperiencePerPainlessPrestigeEditor.OnCursorMoved(e);
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="e">The event data.</param>
        /// <param name="isClick">Whether the button press is a click.</param>
        public void OnButtonPressed(ButtonPressedEventArgs e, bool isClick)
        {
            if (this.DebounceTimer > 0)
                return;

            this.ResetRecipesCheckbox.OnButtonPressed(e, isClick);
            this.UseExperienceMultiplierCheckbox.OnButtonPressed(e, isClick);
            this.PainlessPrestigeModeCheckbox.OnButtonPressed(e, isClick);

            this.TierOneCostEditor.OnButtonPressed(e, isClick);
            this.TierTwoCostEditor.OnButtonPressed(e, isClick);
            this.PointsPerPrestigeEditor.OnButtonPressed(e, isClick);
            this.ExperiencePerPainlessPrestigeEditor.OnButtonPressed(e, isClick);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            if (this.DebounceTimer > 0)
                this.DebounceTimer--;

            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);
            this.upperRightCloseButton?.draw(spriteBatch);
            this.DrawHeader(spriteBatch);
            if (!this.InputInitiated) this.InitiateInput();
            this.ResetRecipesCheckbox.Draw(spriteBatch);
            this.UseExperienceMultiplierCheckbox.Draw(spriteBatch);
            this.TierOneCostEditor.Draw(spriteBatch);
            this.TierTwoCostEditor.Draw(spriteBatch);
            this.PointsPerPrestigeEditor.Draw(spriteBatch);
            this.PainlessPrestigeModeCheckbox.Draw(spriteBatch);
            this.ExperiencePerPainlessPrestigeEditor.Draw(spriteBatch);
            Mouse.DrawCursor(spriteBatch);
        }

        private void InitiateInput()
        {
            if (this.InputInitiated)
                return;
            this.InputInitiated = true;
            Logger.LogVerbose("Settings menu - initiating input.");
            var resetRecipeCheckboxBounds = new Rectangle(this.xPositionOnScreen + spaceToClearSideBorder * 3, this.yPositionOnScreen + (Game1.tileSize * 3.5).Floor(), 9 * Game1.pixelZoom, 9 * Game1.pixelZoom);
            this.ResetRecipesCheckbox = new Checkbox(PerSaveOptions.Instance.ResetRecipesOnPrestige, "Reset Recipes upon prestige.", resetRecipeCheckboxBounds, ChangeRecipeReset);

            const int padding = 4 * Game1.pixelZoom;
            var useExperienceMultiplierCheckboxBounds = resetRecipeCheckboxBounds;
            useExperienceMultiplierCheckboxBounds.Y += resetRecipeCheckboxBounds.Height + padding;
            this.UseExperienceMultiplierCheckbox = new Checkbox(PerSaveOptions.Instance.UseExperienceMultiplier, "Use prestige points experience multiplier.", useExperienceMultiplierCheckboxBounds, ChangeUseExperienceMultiplier);

            var tierOneEditorLocation = new Vector2(useExperienceMultiplierCheckboxBounds.X, useExperienceMultiplierCheckboxBounds.Y + useExperienceMultiplierCheckboxBounds.Height + padding);
            this.TierOneCostEditor = new IntegerEditor("Cost of Tier 1 Prestige", PerSaveOptions.Instance.CostOfTierOnePrestige, 1, 100, tierOneEditorLocation, ChangeTierOneCost);

            var tierTwoEditorLocation = tierOneEditorLocation;
            tierTwoEditorLocation.X += this.TierOneCostEditor.Bounds.Width + padding;
            this.TierTwoCostEditor = new IntegerEditor("Cost of Tier 2 Prestige", PerSaveOptions.Instance.CostOfTierTwoPrestige, 1, 100, tierTwoEditorLocation, ChangeTierTwoCost);

            var pointsPerPrestigeEditorLocation = tierTwoEditorLocation;
            pointsPerPrestigeEditorLocation.Y += this.TierTwoCostEditor.Bounds.Height + padding;
            pointsPerPrestigeEditorLocation.X = this.TierOneCostEditor.Bounds.X;
            this.PointsPerPrestigeEditor = new IntegerEditor("Points Per Prestige", PerSaveOptions.Instance.PointsPerPrestige, 1, 100, pointsPerPrestigeEditorLocation, ChangePointsPerPrestige);

            var painlessPrestigeModeCheckboxBounds = new Rectangle(this.PointsPerPrestigeEditor.Bounds.X, this.PointsPerPrestigeEditor.Bounds.Y + this.PointsPerPrestigeEditor.Bounds.Height + padding, 9 * Game1.pixelZoom, 9 * Game1.pixelZoom);
            const string painlessPrestigeModeCheckboxText = "Painless Prestige Mode";
            this.PainlessPrestigeModeCheckbox = new Checkbox(PerSaveOptions.Instance.PainlessPrestigeMode, painlessPrestigeModeCheckboxText, painlessPrestigeModeCheckboxBounds, ChangePainlessPrestigeMode);

            var experiencePerPainlessPrestigeEditorLocation = new Vector2(painlessPrestigeModeCheckboxBounds.X, painlessPrestigeModeCheckboxBounds.Y);
            experiencePerPainlessPrestigeEditorLocation.X += painlessPrestigeModeCheckboxBounds.Width + Game1.dialogueFont.MeasureString(painlessPrestigeModeCheckboxText).X + padding;
            this.ExperiencePerPainlessPrestigeEditor = new IntegerEditor("Extra Experience Cost", PerSaveOptions.Instance.ExperienceNeededPerPainlessPrestige, 1000, 100000, experiencePerPainlessPrestigeEditorLocation, ChangeExperiencePerPainlessPrestige, 1000);

            this.SetClickableTextureIds();
            this.allClickableComponents = this.GetAllClickableComponents();
        }

        private void SetClickableTextureIds()
        {
            // main ids, just did sequential numbers
            this.ResetRecipesCheckbox.ClickableTextureComponent.myID = 1;
            this.UseExperienceMultiplierCheckbox.ClickableTextureComponent.myID = 2;
            this.TierOneCostEditor.MinusButton.ClickableTextureComponent.myID = 3;
            this.TierOneCostEditor.PlusButton.ClickableTextureComponent.myID = 4;
            this.TierTwoCostEditor.MinusButton.ClickableTextureComponent.myID = 5;
            this.TierTwoCostEditor.PlusButton.ClickableTextureComponent.myID = 6;
            this.PointsPerPrestigeEditor.MinusButton.ClickableTextureComponent.myID = 7;
            this.PointsPerPrestigeEditor.PlusButton.ClickableTextureComponent.myID = 8;
            this.PainlessPrestigeModeCheckbox.ClickableTextureComponent.myID = 9;
            this.ExperiencePerPainlessPrestigeEditor.MinusButton.ClickableTextureComponent.myID = 10;
            this.ExperiencePerPainlessPrestigeEditor.PlusButton.ClickableTextureComponent.myID = 11;

            //neighbor ids
            this.upperRightCloseButton.downNeighborID = this.ResetRecipesCheckbox.ClickableTextureComponent.myID;

            this.ResetRecipesCheckbox.ClickableTextureComponent.upNeighborID = this.upperRightCloseButton.myID;
            this.ResetRecipesCheckbox.ClickableTextureComponent.downNeighborID = this.UseExperienceMultiplierCheckbox.ClickableTextureComponent.myID;

            this.UseExperienceMultiplierCheckbox.ClickableTextureComponent.upNeighborID = this.ResetRecipesCheckbox.ClickableTextureComponent.myID;
            this.UseExperienceMultiplierCheckbox.ClickableTextureComponent.downNeighborID = this.TierOneCostEditor.MinusButton.ClickableTextureComponent.myID;

            this.TierOneCostEditor.MinusButton.ClickableTextureComponent.upNeighborID = this.UseExperienceMultiplierCheckbox.ClickableTextureComponent.myID;
            this.TierOneCostEditor.MinusButton.ClickableTextureComponent.downNeighborID = this.PointsPerPrestigeEditor.MinusButton.ClickableTextureComponent.myID;
            this.TierOneCostEditor.MinusButton.ClickableTextureComponent.rightNeighborID = this.TierOneCostEditor.PlusButton.ClickableTextureComponent.myID;

            this.TierOneCostEditor.PlusButton.ClickableTextureComponent.upNeighborID = this.UseExperienceMultiplierCheckbox.ClickableTextureComponent.myID;
            this.TierOneCostEditor.PlusButton.ClickableTextureComponent.downNeighborID = this.PointsPerPrestigeEditor.PlusButton.ClickableTextureComponent.myID;
            this.TierOneCostEditor.PlusButton.ClickableTextureComponent.leftNeighborID = this.TierOneCostEditor.MinusButton.ClickableTextureComponent.myID;
            this.TierOneCostEditor.PlusButton.ClickableTextureComponent.rightNeighborID = this.TierTwoCostEditor.MinusButton.ClickableTextureComponent.myID;

            this.TierTwoCostEditor.MinusButton.ClickableTextureComponent.upNeighborID = this.UseExperienceMultiplierCheckbox.ClickableTextureComponent.myID;
            this.TierTwoCostEditor.MinusButton.ClickableTextureComponent.downNeighborID = this.PointsPerPrestigeEditor.PlusButton.ClickableTextureComponent.myID;
            this.TierTwoCostEditor.MinusButton.ClickableTextureComponent.leftNeighborID = this.TierOneCostEditor.PlusButton.ClickableTextureComponent.myID;
            this.TierTwoCostEditor.MinusButton.ClickableTextureComponent.rightNeighborID = this.TierTwoCostEditor.PlusButton.ClickableTextureComponent.myID;

            this.TierTwoCostEditor.PlusButton.ClickableTextureComponent.upNeighborID = this.UseExperienceMultiplierCheckbox.ClickableTextureComponent.myID;
            this.TierTwoCostEditor.PlusButton.ClickableTextureComponent.downNeighborID = this.PointsPerPrestigeEditor.PlusButton.ClickableTextureComponent.myID;
            this.TierTwoCostEditor.PlusButton.ClickableTextureComponent.leftNeighborID = this.TierTwoCostEditor.MinusButton.ClickableTextureComponent.myID;

            this.PointsPerPrestigeEditor.MinusButton.ClickableTextureComponent.upNeighborID = this.TierOneCostEditor.ClickableTextureComponent.myID;
            this.PointsPerPrestigeEditor.MinusButton.ClickableTextureComponent.downNeighborID = this.PainlessPrestigeModeCheckbox.ClickableTextureComponent.myID;
            this.PointsPerPrestigeEditor.MinusButton.ClickableTextureComponent.rightNeighborID = this.PointsPerPrestigeEditor.PlusButton.ClickableTextureComponent.myID;

            this.PointsPerPrestigeEditor.PlusButton.ClickableTextureComponent.upNeighborID = this.TierOneCostEditor.ClickableTextureComponent.myID;
            this.PointsPerPrestigeEditor.PlusButton.ClickableTextureComponent.downNeighborID = this.PainlessPrestigeModeCheckbox.ClickableTextureComponent.myID;
            this.PointsPerPrestigeEditor.PlusButton.ClickableTextureComponent.leftNeighborID = this.PointsPerPrestigeEditor.MinusButton.ClickableTextureComponent.myID;

            this.PainlessPrestigeModeCheckbox.ClickableTextureComponent.upNeighborID = this.PointsPerPrestigeEditor.MinusButton.ClickableTextureComponent.myID;
            this.PainlessPrestigeModeCheckbox.ClickableTextureComponent.downNeighborID = this.ExperiencePerPainlessPrestigeEditor.MinusButton.ClickableTextureComponent.myID;
            this.PainlessPrestigeModeCheckbox.ClickableTextureComponent.rightNeighborID = this.ExperiencePerPainlessPrestigeEditor.MinusButton.ClickableTextureComponent.myID;

            this.ExperiencePerPainlessPrestigeEditor.MinusButton.ClickableTextureComponent.upNeighborID = this.PainlessPrestigeModeCheckbox.ClickableTextureComponent.myID;
            this.ExperiencePerPainlessPrestigeEditor.MinusButton.ClickableTextureComponent.leftNeighborID = this.PainlessPrestigeModeCheckbox.ClickableTextureComponent.myID;
            this.ExperiencePerPainlessPrestigeEditor.MinusButton.ClickableTextureComponent.rightNeighborID = this.ExperiencePerPainlessPrestigeEditor.PlusButton.ClickableTextureComponent.myID;

            this.ExperiencePerPainlessPrestigeEditor.PlusButton.ClickableTextureComponent.upNeighborID = this.PainlessPrestigeModeCheckbox.ClickableTextureComponent.myID;
            this.ExperiencePerPainlessPrestigeEditor.PlusButton.ClickableTextureComponent.leftNeighborID = this.ExperiencePerPainlessPrestigeEditor.MinusButton.ClickableTextureComponent.myID;
        }

        private List<ClickableComponent> GetAllClickableComponents()
        {
            return new List<ClickableComponent>
            {
                this.upperRightCloseButton,
                this.ResetRecipesCheckbox.ClickableTextureComponent,
                this.UseExperienceMultiplierCheckbox.ClickableTextureComponent,
                this.TierOneCostEditor.MinusButton.ClickableTextureComponent,
                this.TierOneCostEditor.PlusButton.ClickableTextureComponent,
                this.TierTwoCostEditor.MinusButton.ClickableTextureComponent,
                this.TierTwoCostEditor.PlusButton.ClickableTextureComponent,
                this.PointsPerPrestigeEditor.MinusButton.ClickableTextureComponent,
                this.PointsPerPrestigeEditor.PlusButton.ClickableTextureComponent,
                this.PainlessPrestigeModeCheckbox.ClickableTextureComponent,
                this.ExperiencePerPainlessPrestigeEditor.MinusButton.ClickableTextureComponent,
                this.ExperiencePerPainlessPrestigeEditor.PlusButton.ClickableTextureComponent
            };
        }

        private static void ChangeRecipeReset(bool resetRecipes)
        {
            PerSaveOptions.Instance.ResetRecipesOnPrestige = resetRecipes;
            PerSaveOptions.Save();
        }

        private static void ChangeUseExperienceMultiplier(bool useExperienceMultiplier)
        {
            PerSaveOptions.Instance.UseExperienceMultiplier = useExperienceMultiplier;
            PerSaveOptions.Save();
        }

        private static void ChangeTierOneCost(int cost)
        {
            PerSaveOptions.Instance.CostOfTierOnePrestige = cost;
            PerSaveOptions.Save();
        }

        private static void ChangeTierTwoCost(int cost)
        {
            PerSaveOptions.Instance.CostOfTierTwoPrestige = cost;
            PerSaveOptions.Save();
        }

        private static void ChangePointsPerPrestige(int points)
        {
            PerSaveOptions.Instance.PointsPerPrestige = points;
            PerSaveOptions.Save();
        }

        private static void ChangePainlessPrestigeMode(bool usePainlessPrestigeMode)
        {
            PerSaveOptions.Instance.PainlessPrestigeMode = usePainlessPrestigeMode;
            PerSaveOptions.Save();
        }

        private static void ChangeExperiencePerPainlessPrestige(int experienceNeeded)
        {
            PerSaveOptions.Instance.ExperienceNeededPerPainlessPrestige = experienceNeeded;
            PerSaveOptions.Save();
        }

        private void DrawHeader(SpriteBatch spriteBatch)
        {
            const string title = "Skill Prestige Settings";
            spriteBatch.DrawString(Game1.dialogueFont, title, new Vector2(this.xPositionOnScreen + this.width / 2 - Game1.dialogueFont.MeasureString(title).X / 2f, this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize / 4), Game1.textColor);
            this.drawHorizontalPartition(spriteBatch, this.yPositionOnScreen + (Game1.tileSize * 2.5).Floor());
        }
    }
}
