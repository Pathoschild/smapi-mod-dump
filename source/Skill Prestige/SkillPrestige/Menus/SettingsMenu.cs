using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.InputHandling;
using SkillPrestige.Logging;
using SkillPrestige.Menus.Elements.Buttons;
using StardewValley;
using StardewValley.Menus;

namespace SkillPrestige.Menus
{
    /// <summary>
    /// Represents a menu where players can change their per-save settings.
    /// </summary>
    internal class SettingsMenu : IClickableMenu
    {
        private static bool _buttonClickRegistered;
        private int _debouceWaitTime;
        private bool _inputInitiated;
        
        private Checkbox _resetRecipesCheckbox;
        private Checkbox _useExperienceMultiplierCheckbox;
        private Checkbox _painlessPrestigeModeCheckbox;
        private IntegerEditor _tierOneCostEditor;
        private IntegerEditor _tierTwoCostEditor;
        private IntegerEditor _pointsPerPrestigeEditor;
        private IntegerEditor _experiencePerPainlessPrestigeEditor;

        public SettingsMenu(Rectangle bounds) : base(bounds.X, bounds.Y, bounds.Width, bounds.Height, true)
        {
            Logger.LogVerbose("New Settings Menu created.");

            exitFunction = DeregisterMouseEvents;
        }


        private void RegisterMouseEvents()
        {
            if (_buttonClickRegistered) return;
            _buttonClickRegistered = true;
            Logger.LogVerbose("Settings menu - Registering mouse events...");
            Mouse.MouseMoved += _resetRecipesCheckbox.CheckForMouseHover;
            Mouse.MouseMoved += _useExperienceMultiplierCheckbox.CheckForMouseHover;
            Mouse.MouseClicked += _resetRecipesCheckbox.CheckForMouseClick;
            Mouse.MouseClicked += _useExperienceMultiplierCheckbox.CheckForMouseClick;
            Mouse.MouseClicked += _painlessPrestigeModeCheckbox.CheckForMouseClick;
            _tierOneCostEditor.RegisterMouseEvents();
            _tierTwoCostEditor.RegisterMouseEvents();
            _pointsPerPrestigeEditor.RegisterMouseEvents();
            _experiencePerPainlessPrestigeEditor.RegisterMouseEvents();
            Logger.LogVerbose("Settings menu - Mouse events registered.");
        }

        private void DeregisterMouseEvents()
        {
            Logger.LogVerbose("Settings menu - Deregistering mouse events...");
            if (!_buttonClickRegistered) return;
            Mouse.MouseMoved -= _resetRecipesCheckbox.CheckForMouseHover;
            Mouse.MouseMoved -= _useExperienceMultiplierCheckbox.CheckForMouseHover;
            Mouse.MouseClicked -= _resetRecipesCheckbox.CheckForMouseClick;
            Mouse.MouseClicked -= _useExperienceMultiplierCheckbox.CheckForMouseClick;
            Mouse.MouseClicked -= _painlessPrestigeModeCheckbox.CheckForMouseClick;
            _tierOneCostEditor.DeregisterMouseEvents();
            _tierTwoCostEditor.DeregisterMouseEvents();
            _pointsPerPrestigeEditor.DeregisterMouseEvents();
            _experiencePerPainlessPrestigeEditor.DeregisterMouseEvents();
            _buttonClickRegistered = false;
            Logger.LogVerbose("Settings menu - Mouse events deregistered.");
        }


        public override void receiveRightClick(int x, int y, bool playSound = true) { }

        private void InitiateInput()
        {
            if (_inputInitiated) return;
            _inputInitiated = true;
            Logger.LogVerbose("Settings menu - intiating input.");
            var resetRecipeCheckboxBounds = new Rectangle(xPositionOnScreen + spaceToClearSideBorder * 3, yPositionOnScreen + (Game1.tileSize * 3.5).Floor(), 9*Game1.pixelZoom, 9 * Game1.pixelZoom);
            _resetRecipesCheckbox = new Checkbox(PerSaveOptions.Instance.ResetRecipesOnPrestige, "Reset Recipes upon prestige.", resetRecipeCheckboxBounds, ChangeRecipeReset);
            var padding = 4*Game1.pixelZoom;
            var useExperienceMultiplierCheckboxBounds = resetRecipeCheckboxBounds;
            useExperienceMultiplierCheckboxBounds.Y += resetRecipeCheckboxBounds.Height + padding;
            _useExperienceMultiplierCheckbox = new Checkbox(PerSaveOptions.Instance.UseExperienceMultiplier, "Use prestige points experience multiplier.", useExperienceMultiplierCheckboxBounds, ChangeUseExperienceMultiplier);
            var tierOneEditorLocation = new Vector2(useExperienceMultiplierCheckboxBounds.X, useExperienceMultiplierCheckboxBounds.Y + useExperienceMultiplierCheckboxBounds.Height + padding);
            _tierOneCostEditor = new IntegerEditor("Cost of Tier 1 Prestige", PerSaveOptions.Instance.CostOfTierOnePrestige, 1, 100, tierOneEditorLocation, ChangeTierOneCost);
            var tierTwoEditorLocation = tierOneEditorLocation;
            tierTwoEditorLocation.X += _tierOneCostEditor.Bounds.Width + padding;
            _tierTwoCostEditor = new IntegerEditor("Cost of Tier 2 Prestige", PerSaveOptions.Instance.CostOfTierTwoPrestige, 1, 100, tierTwoEditorLocation, ChangeTierTwoCost);
            var pointsPerPrestigeEditorLocation = tierTwoEditorLocation;
            pointsPerPrestigeEditorLocation.Y += _tierTwoCostEditor.Bounds.Height + padding;
            pointsPerPrestigeEditorLocation.X = _tierOneCostEditor.Bounds.X;
            _pointsPerPrestigeEditor = new IntegerEditor("Points Per Prestige", PerSaveOptions.Instance.PointsPerPrestige, 1, 100, pointsPerPrestigeEditorLocation, ChangePointsPerPrestige);
            var painlessPrestigeModeCheckboxBounds = new Rectangle(_pointsPerPrestigeEditor.Bounds.X, _pointsPerPrestigeEditor.Bounds.Y + _pointsPerPrestigeEditor.Bounds.Height + padding, 9 * Game1.pixelZoom, 9 * Game1.pixelZoom);
            const string painlessPrestigeModeCheckboxText = "Painless Prestige Mode";
            _painlessPrestigeModeCheckbox = new Checkbox(PerSaveOptions.Instance.PainlessPrestigeMode, painlessPrestigeModeCheckboxText, painlessPrestigeModeCheckboxBounds, ChangePainlessPrestigeMode);
            var experiencePerPainlessPrestigeEditorLocation = new Vector2(painlessPrestigeModeCheckboxBounds.X, painlessPrestigeModeCheckboxBounds.Y);
            experiencePerPainlessPrestigeEditorLocation.X += painlessPrestigeModeCheckboxBounds.Width + Game1.dialogueFont.MeasureString(painlessPrestigeModeCheckboxText).X + padding;
            _experiencePerPainlessPrestigeEditor = new IntegerEditor("Extra Experience Cost", PerSaveOptions.Instance.ExperienceNeededPerPainlessPrestige, 1000, 100000, experiencePerPainlessPrestigeEditorLocation, ChangeExperiencePerPainlessPrestige, 1000);
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

        public override void draw(SpriteBatch spriteBatch)
        {
            if (_debouceWaitTime < 10)
            {
                _debouceWaitTime++;
            }
            else
            {
                RegisterMouseEvents();
            }

            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            upperRightCloseButton?.draw(spriteBatch);
            DrawHeader(spriteBatch);
            if (!_inputInitiated) InitiateInput();
            _resetRecipesCheckbox.Draw(spriteBatch);
            _useExperienceMultiplierCheckbox.Draw(spriteBatch);
            _tierOneCostEditor.Draw(spriteBatch);
            _tierTwoCostEditor.Draw(spriteBatch);
            _pointsPerPrestigeEditor.Draw(spriteBatch);
            _painlessPrestigeModeCheckbox.Draw(spriteBatch);
            _experiencePerPainlessPrestigeEditor.Draw(spriteBatch);
            Mouse.DrawCursor(spriteBatch);
        }

        private void DrawHeader(SpriteBatch spriteBatch)
        {
            const string title = "Skill Prestige Settings";
            spriteBatch.DrawString(Game1.dialogueFont, title, new Vector2(xPositionOnScreen + width / 2 - Game1.dialogueFont.MeasureString(title).X / 2f, yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize / 4), Game1.textColor);
            drawHorizontalPartition(spriteBatch, yPositionOnScreen + (Game1.tileSize * 2.5).Floor());
        }
    }
}
