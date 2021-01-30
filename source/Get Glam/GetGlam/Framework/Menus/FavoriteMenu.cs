/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MartyrPher/GetGlam
**
*************************************************/

using GetGlam.Framework.DataModels;
using GetGlam.Framework.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace GetGlam.Framework
{
    /// <summary>
    /// Class that handles all the players saved favorites.
    /// </summary>
    public class FavoriteMenu : IClickableMenu
    {
        // Instance of Entry
        private ModEntry Entry;

        // Instance of CharacterLoader
        private CharacterLoader PlayerLoader;

        // Instance of GlamMenu
        private GlamMenu Menu;

        // List that has all the star buttons
        private List<ClickableTextureComponent> StarComponents = new List<ClickableTextureComponent>();

        // The set button
        private ClickableTextureComponent SetButton;

        // The delete button
        private ClickableTextureComponent DeleteButton;

        // The glam menu tab to change back to the glam menu
        private ClickableTextureComponent GlamMenuTab;

        // The favorite tab for show
        private ClickableTextureComponent FavoriteMenuTab;

        // Left direction to change players facing direction
        private ClickableTextureComponent LeftDirectionButton;

        // Right direction to change players facing direction
        private ClickableTextureComponent RightDirectionButton;

        // X padding between stars
        private int StarPaddingX = 48;

        // Y padding between stars
        private int StarPaddingY = 96;

        // Current index of the star button selected
        private int CurrentIndex;

        // Was a favorite applied
        private bool WasFavoriteApplied = false;

        /// <summary>
        /// FavoriteMenu Constructor.
        /// </summary>
        /// <param name="entry">The instance of ModEntry</param>
        /// <param name="playerLoader">The instance of CharacterLoader</param>
        /// <param name="menu">The instance of the GlamMenu</param>
        public FavoriteMenu(ModEntry entry, CharacterLoader playerLoader, GlamMenu menu)
            :base((int)Utility.getTopLeftPositionForCenteringOnScreen(712, 712, 0, 0).X, (int)Utility.getTopLeftPositionForCenteringOnScreen(712, 712, 0, 0).Y - IClickableMenu.borderWidth, 712, 712, false)
        {
            Entry = entry;
            PlayerLoader = playerLoader;
            Menu = menu;
          
            SetUpMenu();
        }

        /// <summary>
        /// Creates the Menu.
        /// </summary>
        private void SetUpMenu()
        {
            CreateTabs();
            CreateButtons();
            CreateDirectionButtons();
            SetButtonsInvisible();

            AddStarButtons();
            UpdateStarButtons();
        }

        /// <summary>
        /// Creates the tabs for switching between menus.
        /// </summary>
        private void CreateTabs()
        {
            GlamMenuTab = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen - IClickableMenu.borderWidth - 8, this.yPositionOnScreen + 96, 64, 64), Game1.mouseCursors, new Rectangle(672, 80, 16, 16), 4f, false);
            FavoriteMenuTab = new ClickableTextureComponent("FavoriteTab", new Rectangle(this.xPositionOnScreen - IClickableMenu.borderWidth + 1, this.yPositionOnScreen + 160, 64, 64), null, "FavoriteTab", Game1.mouseCursors, new Rectangle(656, 80, 15, 16), 4f, false);
        }

        /// <summary>
        /// Creates the set and delete buttons.
        /// </summary>
        private void CreateButtons()
        {
            SetButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 128, this.yPositionOnScreen + this.width / 2, 84, 44), Game1.mouseCursors, new Rectangle(294, 428, 21, 11), 4f);
            DeleteButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 256, this.yPositionOnScreen + this.width / 2, 45, 45), Game1.mouseCursors, new Rectangle(290, 344, 9, 9), 5f);
        }

        /// <summary>
        /// Creates the left and right arrow buttons.
        /// </summary>
        private void CreateDirectionButtons()
        {
            LeftDirectionButton = new ClickableTextureComponent("Direction", new Rectangle(this.xPositionOnScreen + this.width - 200, this.yPositionOnScreen + 320, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false);
            RightDirectionButton = new ClickableTextureComponent("Direction", new Rectangle(this.xPositionOnScreen + this.width - 128, this.yPositionOnScreen + 320, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false);
        }

        /// <summary>
        /// Sets the buttons as invisible.
        /// </summary>
        private void SetButtonsInvisible()
        {
            SetButton.visible = false;
            DeleteButton.visible = false;
        }

        /// <summary>
        /// Adds star buttons to the menu.
        /// </summary>
        private void AddStarButtons()
        {
            for (int i = 0; i < 40; i++)
            {
                if (i % 10 == 0 && i != 0)
                {
                    StarPaddingX = 48;
                    StarPaddingY += 48;
                }

                StarComponents.Add(new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + StarPaddingX, this.yPositionOnScreen + this.height / 2 + StarPaddingY, 24, 24), Game1.mouseCursors, new Rectangle(346, 400, 8, 8), 4f));

                StarPaddingX += 64;
            }
        }

        /// <summary>
        /// Updates the star buttons sprites based on if it's a saved favorite.
        /// </summary>
        private void UpdateStarButtons()
        {
            for (int i = 0; i < 40; i++)
            {
                if (!PlayerLoader.Favorites[i].IsDefault)
                    StarComponents[i].sourceRect.Y = 392;
                else
                    StarComponents[i].sourceRect.Y = 400;
            }
        }

        /// <summary>
        /// Override to handle the mouse over a specific element.
        /// </summary>
        /// <param name="x">The x position of the mouse</param>
        /// <param name="y">The y position of the mouse</param>
        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);

            PerformHoverActionStarButtons(x, y);
            PerformHoverActionSetDeleteButtons(x, y);
            PerformHoverActionDirectionButtons(x, y);         
        }

        /// <summary>
        /// Performs hover action for star buttons.
        /// </summary>
        /// <param name="x">X position of the mouse</param>
        /// <param name="y">Y position of the mouse</param>
        private void PerformHoverActionStarButtons(int x, int y)
        {
            foreach (ClickableTextureComponent star in StarComponents)
                ChangeComponentScalePerformHover(star, x, y);
        }

        /// <summary>
        /// Performs hover action for set and delete buttons.
        /// </summary>
        /// <param name="x">X position of the mouse</param>
        /// <param name="y">Y position of the mouse</param>
        private void PerformHoverActionSetDeleteButtons(int x, int y)
        {
            ChangeComponentScalePerformHover(SetButton, x, y);
            ChangeComponentScalePerformHover(DeleteButton, x, y);
        }

        /// <summary>
        /// Performs hover action for the direction buttons.
        /// </summary>
        /// <param name="x">X position of the mouse</param>
        /// <param name="y">Y position of the mouse</param>
        private void PerformHoverActionDirectionButtons(int x, int y)
        {
            ChangeComponentScalePerformHover(LeftDirectionButton, x, y);
            ChangeComponentScalePerformHover(RightDirectionButton, x, y);
        }

        /// <summary>
        /// Changes a components scale on hover.
        /// </summary>
        /// <param name="component">The component</param>
        /// <param name="x">X position of the mouse</param>
        /// <param name="y">Y position of the mouse</param>
        private void ChangeComponentScalePerformHover(ClickableTextureComponent component, int x, int y)
        {
            if (component.containsPoint(x, y))
                component.scale = Math.Min(component.scale + 0.02f, component.baseScale + 0.5f);
            else
                component.scale = Math.Max(component.scale - 0.02f, component.baseScale);
        }

        /// <summary>
        /// Override to handle recieving a left click on a specific element.
        /// </summary>
        /// <param name="x">The x position of the click</param>
        /// <param name="y">The y position of the click</param>
        /// <param name="playSound">Whether to play sound when clicked</param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            OnClickStarButtons(x, y);
            OnClickGlamMenuTab(x, y);

            OnClickSetButton(x, y);
            OnClickDeleteButton(x, y);

            OnClickLeftDirectionButton(x, y);
            OnClickRightDirectionButton(x, y);
        }

        /// <summary>
        /// Star button On Click.
        /// </summary>
        /// <param name="x">X position of the mouse</param>
        /// <param name="y">Y position of the mouse</param>
        private void OnClickStarButtons(int x, int y)
        {
            for (int i = 0; i < StarComponents.Count; i++)
            {
                if (StarComponents[i].containsPoint(x, y))
                {
                    SetButton.visible = true;
                    DeleteButton.visible = true;

                    // Set the index and load the favorite to the player
                    CurrentIndex = i;
                    PlayerLoader.LoadFavorite(CurrentIndex, Menu, false);
                    ChangeComponentOnClickScale(StarComponents[i]);
                    Game1.playSound("coin");
                }
            }
        }

        /// <summary>
        /// Glam Menu On Click.
        /// </summary>
        /// <param name="x">X position of the mouse</param>
        /// <param name="y">Y position of the mouse</param>
        private void OnClickGlamMenuTab(int x, int y)
        {
            if (GlamMenuTab.containsPoint(x, y))
            {
                // If there wasn't a favorite applied then restore the player
                if (!WasFavoriteApplied)
                    Menu.RestoreSnapshot();

                // Set the menu back to the glam menu
                Game1.activeClickableMenu = Menu;
            }
        }

        /// <summary>
        /// Set Button On Click.
        /// </summary>
        /// <param name="x">X position of the mouse</param>
        /// <param name="y">Y position of the mouse</param>
        private void OnClickSetButton(int x, int y)
        {
            if (SetButton.containsPoint(x, y))
            {
                // A favorite was applied and change the player
                WasFavoriteApplied = true;
                PlayerLoader.LoadFavorite(CurrentIndex, Menu, true);
                ChangeComponentOnClickScale(SetButton);
                Game1.playSound("coin");
            }
        }

        /// <summary>
        /// Delete Button On Click.
        /// </summary>
        /// <param name="x">X position of the mouse</param>
        /// <param name="y">Y position of the mouse</param>
        private void OnClickDeleteButton(int x, int y)
        {
            if (DeleteButton.containsPoint(x, y))
            {
                // Set the slot to a default model and update the star buttons
                PlayerLoader.Favorites[CurrentIndex] = new FavoriteModel();
                UpdateStarButtons();
                ChangeComponentOnClickScale(DeleteButton);
                Game1.playSound("coin");
            }
        }

        /// <summary>
        /// Left Direction Button On Click.
        /// </summary>
        /// <param name="x">X position of the mouse</param>
        /// <param name="y">Y position of the mouse</param>
        private void OnClickLeftDirectionButton(int x, int y)
        {
            if (LeftDirectionButton.containsPoint(x, y))
            {
                Game1.player.faceDirection((Game1.player.facingDirection + 5) % 4);
                Game1.player.FarmerSprite.StopAnimation();
                Game1.player.completelyStopAnimatingOrDoingAction();
                ChangeComponentOnClickScale(LeftDirectionButton);
                Game1.playSound("pickUpItem");
            }
        }

        /// <summary>
        /// Right Direction Button On Click.
        /// </summary>
        /// <param name="x">X position of the mouse</param>
        /// <param name="y">Y position of the mouse</param>
        private void OnClickRightDirectionButton(int x, int y)
        {
            if (RightDirectionButton.containsPoint(x, y))
            {
                Game1.player.faceDirection((Game1.player.facingDirection - 1 + 4) % 4);
                Game1.player.FarmerSprite.StopAnimation();
                Game1.player.completelyStopAnimatingOrDoingAction();
                ChangeComponentOnClickScale(RightDirectionButton);
                Game1.playSound("pickUpItem");
            }
        }

        /// <summary>
        /// Changes a compnents scale when clicked.
        /// </summary>
        /// <param name="component">Current Component</param>
        private void ChangeComponentOnClickScale(ClickableTextureComponent component)
        {
            if (component.scale != 0f)
            {
                component.scale -= 0.25f;
                component.scale = Math.Max(0.75f, component.scale);
            }
        }

        /// <summary>
        /// Override to handle drawing the different parts of the menu.
        /// </summary>
        /// <param name="b">Game's SpriteBatch</param>
        public override void draw(SpriteBatch b)
        {
            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);

            DrawTabs(b);
            DrawPortraitBox(b);
            DrawFarmer(b);
            DrawButtons(b);

            DrawText(b);
            DrawCursor(b);
        }

        /// <summary>
        /// Draws Menu Tabs.
        /// </summary>
        /// <param name="b">Game's SpriteBatch</param>
        private void DrawTabs(SpriteBatch b)
        {
            GlamMenuTab.draw(b);
            FavoriteMenuTab.draw(b);
        }

        /// <summary>
        /// Draws Portrait Box.
        /// </summary>
        /// <param name="b">Game's SpriteBatch</param>
        private void DrawPortraitBox(SpriteBatch b)
        {
            b.Draw(Game1.daybg, new Vector2(this.xPositionOnScreen + this.width - 196, this.yPositionOnScreen + 128), Color.White);
        }

        /// <summary>
        /// Draws The Farmer.
        /// </summary>
        /// <param name="b">Game's SpriteBatch</param>
        private void DrawFarmer(SpriteBatch b)
        {
            Game1.player.FarmerRenderer.draw(b, Game1.player.FarmerSprite.CurrentAnimationFrame, Game1.player.FarmerSprite.CurrentFrame, Game1.player.FarmerSprite.SourceRect, new Vector2(this.xPositionOnScreen + this.width - 164, this.yPositionOnScreen + 160), Vector2.Zero, 0.8f, Color.White, 0f, 1f, Game1.player);
        }

        /// <summary>
        /// Draws The Buttons.
        /// </summary>
        /// <param name="b">Game's SpriteBatch</param>
        private void DrawButtons(SpriteBatch b)
        {
            if (SetButton.visible)
                SetButton.draw(b);

            if (DeleteButton.visible)
                DeleteButton.draw(b);

            LeftDirectionButton.draw(b);
            RightDirectionButton.draw(b);

            foreach (ClickableTextureComponent star in StarComponents)
                star.draw(b);
        }

        /// <summary>
        /// Draws Text.
        /// </summary>
        /// <param name="b">Game's SpriteBatch</param>
        private void DrawText(SpriteBatch b)
        {
            Utility.drawTextWithShadow(b, "Favorites", Game1.smallFont, new Vector2(this.xPositionOnScreen + 48, this.yPositionOnScreen + 104), Game1.textColor, 2f);
            Utility.drawTextWithShadow(b, "Iriduim stars are saved favorites.", Game1.smallFont, new Vector2(this.xPositionOnScreen + 48, this.yPositionOnScreen + 160), Game1.textColor, 1f);
            Utility.drawTextWithShadow(b, "Gold stars are default.", Game1.smallFont, new Vector2(this.xPositionOnScreen + 48, this.yPositionOnScreen + 192), Game1.textColor, 1f);
            Utility.drawTextWithShadow(b, "Click on a favorite to view it.", Game1.smallFont, new Vector2(this.xPositionOnScreen + 48, this.yPositionOnScreen + 224), Game1.textColor, 1f);
            Utility.drawTextWithShadow(b, "Use the `Set` to apply a favorite.", Game1.smallFont, new Vector2(this.xPositionOnScreen + 48, this.yPositionOnScreen + 256), Game1.textColor, 1f);
            Utility.drawTextWithShadow(b, "Use the `X` to delete a favorite.", Game1.smallFont, new Vector2(this.xPositionOnScreen + 48, this.yPositionOnScreen + 288), Game1.textColor, 1f);
        }

        /// <summary>
        /// Draws Cursor.
        /// </summary>
        /// <param name="b">Game's SpriteBatch</param>
        private void DrawCursor(SpriteBatch b)
        {
            if (Game1.activeClickableMenu == this && !Game1.options.hardwareCursor)
                base.drawMouse(b);
        }
    }
}
