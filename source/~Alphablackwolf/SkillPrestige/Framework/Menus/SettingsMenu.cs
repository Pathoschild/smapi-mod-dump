/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

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
        /*********
        ** Fields
        *********/
        private int DebounceTimer = 10;
        private bool InputInitiated;

        private Checkbox ResetRecipesCheckbox;
        private Checkbox UseExperienceMultiplierCheckbox;
        private Checkbox PainlessPrestigeModeCheckbox;
        private IntegerEditor TierOneCostEditor;
        private IntegerEditor TierTwoCostEditor;
        private IntegerEditor PointsPerPrestigeEditor;
        private IntegerEditor ExperiencePerPainlessPrestigeEditor;


        /*********
        ** Public methods
        *********/
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


        /*********
        ** Private methods
        *********/
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
