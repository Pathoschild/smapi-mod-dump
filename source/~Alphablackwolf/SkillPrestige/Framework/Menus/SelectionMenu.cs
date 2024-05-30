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
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.Framework.InputHandling;
using SkillPrestige.Framework.Menus.Elements;
using SkillPrestige.Framework.Menus.Elements.Buttons;
using SkillPrestige.Logging;
using SkillPrestige.Mods;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using Vector2 = Microsoft.Xna.Framework.Vector2;

// ReSharper disable PossibleLossOfFraction

namespace SkillPrestige.Framework.Menus
{
    /// <summary>Represents a menu where players can choose to prestige a skill and select prestige awards.</summary>
    internal class SelectionMenu : IClickableMenu, IInputHandler
    {
        private int DebounceTimer = 10;
        private List<PrestigableSkillButtonSet> PrestigableSkillButtonSets;
        private int ScrollOffset;
        private const int MaximumDisplayedEntries = 7;
        private const int IconSize = 16 * Game1.pixelZoom;
        private const int DefaultPadding = 16;
        private TextureButton SettingsButton;

        private VerticalScrollBar ScrollBar;
        public SelectionMenu(Rectangle bounds)
            : base(bounds.X, bounds.Y, bounds.Width, bounds.Height, true)
        {
            Logger.LogVerbose("New Selection Menu created.");
            this.behaviorBeforeCleanup = _ => Mouse.ForceUseGamepadSelectionMouse = false;
            this.LoadSettingsButton();
            this.LoadSkillPrestigeButtonSets();
            this.LoadScrollBar();
        }

        private void LoadSettingsButton()
        {
            Logger.LogVerbose("Selection menu - Initiating settings button...");
            const int buttonWidth = 16 * Game1.pixelZoom;
            const int buttonHeight = 16 * Game1.pixelZoom;
            int rightEdgeOfDialog = this.xPositionOnScreen + this.width;
            var bounds = new Rectangle(rightEdgeOfDialog - buttonWidth - Game1.tileSize, this.yPositionOnScreen, buttonWidth, buttonHeight);
            this.SettingsButton = new TextureButton(bounds, Game1.mouseCursors, new Rectangle(96, 368, 16, 16), this.OpenSettingsMenu, "Open Settings Menu");

            Logger.LogVerbose("Selection menu - Settings button initiated.");
        }

        private void LoadSkillPrestigeButtonSets()
        {
            this.PrestigableSkillButtonSets = Skill.AllSkills.Select((skill, index) => new PrestigableSkillButtonSet(skill, index, this.width - spaceToClearSideBorder * 2 - 64, this)).ToList();
            this.UpdateSkillPrestigeButtonSetDisplay();
        }

        private void LoadScrollBar()
        {
            Logger.LogVerbose("Selection Menu: Loading scrollbar...");
            int xPosition = this.xPositionOnScreen + this.width + spaceToClearSideBorder * 2;
            int yPosition = this.yPositionOnScreen + spaceToClearTopBorder;
            const int scrollBarWidth = 32;
            var scrollBarBounds = new Rectangle(xPosition, yPosition, scrollBarWidth,
                this.height - spaceToClearTopBorder - spaceToClearSideBorder * 2);
            this.ScrollBar = new VerticalScrollBar(scrollBarBounds, this.PrestigableSkillButtonSets.Count,
                MaximumDisplayedEntries, (scrollIndex) =>
                {
                    this.ScrollOffset = scrollIndex;
                    this.UpdateSkillPrestigeButtonSetDisplay();
                });
            Logger.LogVerbose("Scrollbar loaded.");
        }

        private void UpdateSkillPrestigeButtonSetDisplay()
        {
            var displayableButtonSets = this.PrestigableSkillButtonSets.Where(buttonSet =>
                buttonSet.Position < MaximumDisplayedEntries + this.ScrollOffset
                && buttonSet.Position >= this.ScrollOffset);

            var buttonSetsToHide = this.PrestigableSkillButtonSets.Where(buttonSet =>
                buttonSet.Position >= MaximumDisplayedEntries + this.ScrollOffset
                || buttonSet.Position < this.ScrollOffset);
            foreach (var buttonSet in buttonSetsToHide)
                buttonSet.ShouldBeDrawn = false;

            const int leftPadding = 32;
            const int topPadding = 16;
            const int yOffsetPerButton = IconSize + DefaultPadding;
            int xStart = this.xPositionOnScreen + spaceToClearSideBorder + leftPadding;
            int yStart = this.yPositionOnScreen + spaceToClearTopBorder + topPadding;
            foreach (var prestigableSkillButtonSet in displayableButtonSets
                         .Select((item, index) => new {Item = item, Index = index}))
            {
                int yPosition = yStart + prestigableSkillButtonSet.Index * yOffsetPerButton;
                prestigableSkillButtonSet.Item.DrawLocation = new Vector2(xStart, yPosition);
                prestigableSkillButtonSet.Item.ShouldBeDrawn = true;
            }

            this.allClickableComponents = this.GetAllClickableComponents();
        }
        public override void applyMovementKey(int direction)
        {
            switch (direction)
            {
                //direction of 2 is down
                case 2 when this.currentlySnappedComponent?.downNeighborID == 999:
                    this.ScrollBar.ScrollDown();
                    break;
                //direction of 0 is up
                case 0 when this.currentlySnappedComponent?.upNeighborID == -999:
                    this.ScrollBar.ScrollUp();
                    break;
                default:
                    this.moveCursorInDirection(direction);
                    break;
            }
        }
        private List<ClickableComponent> GetAllClickableComponents()
        {
            const int settingsButtonId = 5;
            this.upperRightCloseButton.downNeighborID = settingsButtonId;
            var settingsButtonComponent = this.SettingsButton.ClickableTextureComponent;
            settingsButtonComponent.myID = settingsButtonId;
            int nextIndexToUse = 20;
            settingsButtonComponent.upNeighborID = this.upperRightCloseButton.myID;
            settingsButtonComponent.downNeighborID = nextIndexToUse;
            var list = new List<ClickableComponent> { this.upperRightCloseButton, settingsButtonComponent };
            foreach (int index in Enumerable.Range(0, this.PrestigableSkillButtonSets.Count(x => x.ShouldBeDrawn)))
            {
                var skillButtonSet = this.PrestigableSkillButtonSets.Where(x => x.ShouldBeDrawn).ElementAt(index);
                var component = skillButtonSet.ClickableComponent;
                var previousButtonSet = this.PrestigableSkillButtonSets.Where(x => x.ShouldBeDrawn).ElementAtOrDefault(index - 1);
                var nextButtonSet = this.PrestigableSkillButtonSets.Where(x => x.ShouldBeDrawn).ElementAtOrDefault(index + 1);
                component.myID = nextIndexToUse;
                if (previousButtonSet == null)
                {
                    component.upNeighborID = this.PrestigableSkillButtonSets.First().Skill.Type == skillButtonSet.Skill.Type
                        ? settingsButtonId
                        : -999;
                    component.rightNeighborID = settingsButtonId;
                }
                if (previousButtonSet != null) component.upNeighborID = nextIndexToUse - 1;
                if(nextButtonSet == null && skillButtonSet.Skill.Type != this.PrestigableSkillButtonSets.Last().Skill.Type) component.downNeighborID = 999;
                if (nextButtonSet != null) component.downNeighborID = nextIndexToUse + 1;
                list.Add(component);
                nextIndexToUse++;
            }
            return list;
        }

        private void OpenSettingsMenu()
        {
            Logger.LogVerbose("Selection Menu - Initiating Settings Menu...");
            const int menuWidth = Game1.tileSize * 12;
            const int menuHeight = Game1.tileSize * 10;

            int menuXCenter = (menuWidth + borderWidth * 2) / 2;
            int menuYCenter = (menuHeight + borderWidth * 2) / 2;
            int screenXCenter = Game1.uiViewport.Width / 2;
            int screenYCenter = Game1.uiViewport.Height / 2;
            var bounds = new Rectangle(screenXCenter - menuXCenter, screenYCenter - menuYCenter, menuWidth + IClickableMenu.borderWidth * 2, menuHeight + IClickableMenu.borderWidth * 2);
            Game1.playSound("bigSelect");
            this.exitThisMenu(false);
            Game1.activeClickableMenu = new SettingsMenu(bounds);
            Game1.nextClickableMenu.Add(this);
            Logger.LogVerbose("Selection Menu - Loaded Settings Menu.");
        }


        public void OnCursorMoved(CursorMovedEventArgs e)
        {
            if (this.DebounceTimer > 0)
                return;
            this.ScrollBar.OnCursorMoved(e);
            this.SettingsButton.OnCursorMoved(e);
            foreach (var prestigableSkillButtonSet in this.PrestigableSkillButtonSets)
                prestigableSkillButtonSet.OnCursorMoved();
            Mouse.ForceUseGamepadSelectionMouse = this.PrestigableSkillButtonSets.Any(x => x.IsHovered);
        }

        public void OnButtonPressed(ButtonPressedEventArgs e, bool isClick)
        {
            if (this.DebounceTimer > 0)
                return;
            this.ScrollBar.OnButtonPressed(e, isClick);
            this.SettingsButton.OnButtonPressed(e, isClick);
            foreach(var skillButtonSet in this.PrestigableSkillButtonSets)
                skillButtonSet.OnButtonPressed(e, isClick);
        }

        public override void leftClickHeld(int x, int y)
        {
            base.leftClickHeld(x, y);
            this.ScrollBar.leftClickHeld(x, y);
        }

        public override void releaseLeftClick(int x, int y)
        {
            base.releaseLeftClick(x, y);
            this.ScrollBar.releaseLeftClick(x, y);
        }

        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            this.ScrollBar.ReceiveScrollWheelAction(direction);
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
            this.DrawSettingsButton(spriteBatch);
            if (this.PrestigableSkillButtonSets.Count > MaximumDisplayedEntries)
                this.ScrollBar.DrawScrollBar(spriteBatch);
            foreach (var buttonSet in this.PrestigableSkillButtonSets)
                buttonSet.Draw(spriteBatch);
            foreach (var buttonSet in this.PrestigableSkillButtonSets)
                buttonSet.DrawHover(spriteBatch);
            this.SettingsButton.DrawHoverText(spriteBatch);
            Mouse.DrawCursor(spriteBatch);
        }

        private void DrawSettingsButton(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Game1.mouseCursors, new Vector2(this.SettingsButton.Bounds.X, this.SettingsButton.Bounds.Y), this.SettingsButton.SourceRectangle, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.0001f);
        }
    }
}
