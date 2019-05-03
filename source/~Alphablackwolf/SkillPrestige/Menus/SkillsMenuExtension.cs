using System.Collections.Generic;
using System.Linq;
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
    /// Extends the skills menu in Stardew Valley to add prestige buttons next to the skills.
    /// </summary>
    internal static class SkillsMenuExtension
    {
        private static bool _skillsMenuInitialized;
        private static readonly IList<TextureButton> PrestigeButtons = new List<TextureButton>();

        // ReSharper disable once SuggestBaseTypeForParameter - we specifically want the skills page here, even if our usage could work against IClickableMenu.
        private static void IntializeSkillsMenu(SkillsPage skillsPage)
        {
            Logger.LogVerbose("Initializing Skills Menu...");
            _skillsMenuInitialized = true;
            PrestigeButtons.Clear();
            foreach (var skill in Skill.AllSkills)
            {
                var width = 8 * Game1.pixelZoom;
                var height = 8 * Game1.pixelZoom;
                var yOffset = (Game1.tileSize/2.5).Floor();
                var yPadding = (Game1.tileSize * 1.05).Floor();
                var xOffset = skillsPage.width + Game1.tileSize;
                var bounds = new Rectangle(skillsPage.xPositionOnScreen + xOffset, skillsPage.yPositionOnScreen + yPadding + yOffset * skill.SkillScreenPosition + skill.SkillScreenPosition * height, width, height);
                var prestigeButton = new TextureButton(bounds, SkillPrestigeMod.PrestigeIconTexture, new Rectangle(0, 0, 32, 32), () => OpenPrestigeMenu(skill), "Click to open the Prestige menu.");
                PrestigeButtons.Add(prestigeButton);
                Logger.LogVerbose($"{skill.Type.Name} skill prestige button initiated at {bounds.X}, {bounds.Y}. Width: {bounds.Width}, Height: {bounds.Height}");
                Mouse.MouseMoved += prestigeButton.CheckForMouseHover;
                Mouse.MouseClicked += prestigeButton.CheckForMouseClick;
            }
            Logger.LogVerbose("Skills Menu initialized.");
        }

        private static void UnloadSkillsPageAdditions()
        {
            Logger.LogVerbose("Unloading Skills Menu.");
            _skillsMenuInitialized = false;
            foreach (var button in PrestigeButtons)
            {
                Mouse.MouseMoved -= button.CheckForMouseHover;
                Mouse.MouseClicked -= button.CheckForMouseClick;
            }
            PrestigeButtons.Clear();
            Logger.LogVerbose("Skills Menu unloaded.");
        }

        public static void AddPrestigeButtonsToMenu()
        {
            var activeClickableMenu = Game1.activeClickableMenu as GameMenu;
            if (activeClickableMenu == null || activeClickableMenu.currentTab != 1)
            {
                if (_skillsMenuInitialized) UnloadSkillsPageAdditions();
            }
            else
            {
                var skillsPage = (SkillsPage)((List<IClickableMenu>)activeClickableMenu.GetInstanceField("pages"))[1];
                if (!_skillsMenuInitialized) IntializeSkillsMenu(skillsPage);
                var spriteBatch = Game1.spriteBatch;
                DrawPrestigeButtons(spriteBatch);
                DrawProfessionHoverText(spriteBatch, skillsPage);
                DrawPrestigeButtonsHoverText(spriteBatch);
                Mouse.DrawCursor(spriteBatch);
            }
        }

        private static void DrawPrestigeButtons(SpriteBatch spriteBatch)
        {
            foreach (var prestigeButton in PrestigeButtons)
            {
                prestigeButton.Draw(spriteBatch);
            }
        }

        private static void DrawPrestigeButtonsHoverText(SpriteBatch spriteBatch)
        {
            foreach (var prestigeButton in PrestigeButtons)
            {
                prestigeButton.DrawHoverText(spriteBatch);
            }
        }

        private static void DrawProfessionHoverText(SpriteBatch spriteBatch, SkillsPage skillsPage)
        {
            var hoverText = (string) skillsPage.GetInstanceField("hoverText");
            if (hoverText.Length <= 0) return;
            var hoverTitle = (string) skillsPage.GetInstanceField("hoverTitle");
            IClickableMenu.drawHoverText(spriteBatch, hoverText, Game1.smallFont, 0, 0, -1, hoverTitle.Length > 0 ? hoverTitle : null);
        }

        private static void OpenPrestigeMenu(Skill skill)
        {
            Logger.LogVerbose("Skills Menu - Setting up Prestige Menu...");
            var menuWidth = Game1.tileSize * 18;
            var menuHeight = Game1.tileSize * 10;

            var menuXCenter = (menuWidth + IClickableMenu.borderWidth * 2) / 2;
            var menuYCenter = (menuHeight + IClickableMenu.borderWidth * 2) / 2;
            var viewport = Game1.graphics.GraphicsDevice.Viewport;
            var screenXCenter = (int)(viewport.Width * (1.0 / Game1.options.zoomLevel)) / 2;
            var screenYCenter = (int)(viewport.Height * (1.0 / Game1.options.zoomLevel)) / 2;
            var bounds = new Rectangle(screenXCenter - menuXCenter, screenYCenter - menuYCenter, menuWidth + IClickableMenu.borderWidth*2, menuHeight + IClickableMenu.borderWidth*2);
            Game1.playSound("bigSelect");
            Logger.LogVerbose("Getting currently loaded prestige data...");
            var prestige = PrestigeSaveData.CurrentlyLoadedPrestigeSet.Prestiges.Single(x => x.SkillType == skill.Type);
            Game1.activeClickableMenu = new PrestigeMenu(bounds, skill, prestige);
            Logger.LogVerbose("Skills Menu - Loaded Prestige Menu.");
        }
    }
}
