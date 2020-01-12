using GetGlam.Framework.DataModels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace GetGlam.Framework
{
    /// <summary>Class that handles all the players saved favorites</summary>
    public class FavoriteMenu : IClickableMenu
    {
        //Instance of Entry
        private ModEntry Entry;

        //Instance of CharacterLoader
        private CharacterLoader PlayerLoader;

        //Instance of GlamMenu
        private GlamMenu Menu;

        //List that has all the star buttons
        private List<ClickableTextureComponent> StarComponents = new List<ClickableTextureComponent>();

        //The set button
        private ClickableTextureComponent SetButton;

        //The delete button
        private ClickableTextureComponent DeleteButton;

        //The glam menu tab to change back to the glam menu
        private ClickableTextureComponent GlamMenuTab;

        //The favorite tab for show
        private ClickableTextureComponent FavoriteMenuTab;

        //Left direction to change players facing direction
        private ClickableTextureComponent LeftDirectionButton;

        //Right direction to change players facing direction
        private ClickableTextureComponent RightDirectionButton;

        //X padding between stars
        private int StarPaddingX = 48;

        //Y padding between stars
        private int StarPaddingY = 96;

        //Current index of the star button selected
        private int CurrentIndex;

        //Was a favorite applied
        private bool WasFavoriteApplied = false;

        /// <summary>FavoriteMenu Constructor</summary>
        /// <param name="entry">The instance of ModEntry</param>
        /// <param name="playerLoader">The instance of CharacterLoader</param>
        /// <param name="menu">The instance of the GlamMenu</param>
        public FavoriteMenu(ModEntry entry, CharacterLoader playerLoader, GlamMenu menu)
            :base((int)Utility.getTopLeftPositionForCenteringOnScreen(712, 712, 0, 0).X, (int)Utility.getTopLeftPositionForCenteringOnScreen(712, 712, 0, 0).Y - IClickableMenu.borderWidth, 712, 712, false)
        {
            //Set the fields
            Entry = entry;
            PlayerLoader = playerLoader;
            Menu = menu;
          
            //Set up the button layouts
            SetUpButtons();
        }

        /// <summary>Creates the buttons for the menu and sets the positions</summary>
        private void SetUpButtons()
        {
            //Create the tabs
            GlamMenuTab = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen - IClickableMenu.borderWidth - 8, this.yPositionOnScreen + 96, 64, 64), Game1.mouseCursors, new Rectangle(672, 80, 16, 16), 4f, false);
            FavoriteMenuTab = new ClickableTextureComponent("FavoriteTab", new Rectangle(this.xPositionOnScreen - IClickableMenu.borderWidth + 1, this.yPositionOnScreen + 160, 64, 64), null, "FavoriteTab", Game1.mouseCursors, new Rectangle(656, 80, 15, 16), 4f, false);

            //Create the set and delete buttons
            SetButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 128, this.yPositionOnScreen + this.width / 2, 84, 44), Game1.mouseCursors, new Rectangle(294, 428, 21, 11), 4f);
            DeleteButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 256, this.yPositionOnScreen + this.width / 2, 45, 45), Game1.mouseCursors, new Rectangle(290, 344, 9, 9), 5f);

            //Create the left and right direction buttons
            LeftDirectionButton = new ClickableTextureComponent("Direction", new Rectangle(this.xPositionOnScreen + this.width - 200, this.yPositionOnScreen + 320, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false);
            RightDirectionButton = new ClickableTextureComponent("Direction", new Rectangle(this.xPositionOnScreen + this.width - 128, this.yPositionOnScreen + 320, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false);

            //Set the set and delete button to invisible
            SetButton.visible = false;
            DeleteButton.visible = false;

            //Create each of the star buttons for the menu and apply the padding
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

            //Update the star button sprites
            UpdateStarButtons();
        }

        /// <summary>Updates the star buttons sprites based on if it's a saved favorite</summary>
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

        /// <summary>Override to handle the mouse over a specific element</summary>
        /// <param name="x">The x position of the mouse</param>
        /// <param name="y">The y position of the mouse</param>
        public override void performHoverAction(int x, int y)
        {
            //Call the base
            base.performHoverAction(x, y);

            //Check the star buttons
            foreach (ClickableTextureComponent star in StarComponents)
            {
                if (star.containsPoint(x, y))
                    star.scale = Math.Min(star.scale + 0.02f, star.baseScale + 0.5f);
                else
                    star.scale = Math.Max(star.scale - 0.02f, star.baseScale);
            }

            //Check the Set Button
            if (SetButton.containsPoint(x, y))
                SetButton.scale = Math.Min(SetButton.scale + 0.02f, SetButton.baseScale + 0.5f);
            else
                SetButton.scale = Math.Max(SetButton.scale - 0.02f, SetButton.baseScale);

            //Check the Delete button
            if (DeleteButton.containsPoint(x, y))
                DeleteButton.scale = Math.Min(DeleteButton.scale + 0.02f, DeleteButton.baseScale + 0.5f);
            else
                DeleteButton.scale = Math.Max(DeleteButton.scale - 0.02f, DeleteButton.baseScale);

            //Check the left direction button
            if (LeftDirectionButton.containsPoint(x, y))
                LeftDirectionButton.scale = Math.Min(LeftDirectionButton.scale + 0.02f, LeftDirectionButton.baseScale + 0.1f);
            else
                LeftDirectionButton.scale = Math.Max(LeftDirectionButton.scale - 0.02f, LeftDirectionButton.baseScale);

            //Check the right direction button
            if (RightDirectionButton.containsPoint(x, y))
                RightDirectionButton.scale = Math.Min(RightDirectionButton.scale + 0.02f, RightDirectionButton.baseScale + 0.1f);
            else
                RightDirectionButton.scale = Math.Max(RightDirectionButton.scale - 0.02f, RightDirectionButton.baseScale);
        }

        /// <summary>Override to handle recieving a left click on a specific element</summary>
        /// <param name="x">The x position of the click</param>
        /// <param name="y">The y position of the click</param>
        /// <param name="playSound">Whether to play sound when clicked</param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            //Loop through to see if the star buttons were clicked
            for (int i = 0; i < StarComponents.Count; i++)
            {
                if (StarComponents[i].containsPoint(x, y))
                {
                    //Set the buttons to visible
                    SetButton.visible = true;
                    DeleteButton.visible = true;

                    //Set the index and load the favorite to the player
                    CurrentIndex = i;
                    PlayerLoader.LoadFavorite(CurrentIndex, Menu, false);

                    if (StarComponents[i].scale != 0f)
                    {
                        StarComponents[i].scale -= 0.25f;
                        StarComponents[i].scale = Math.Max(0.75f, StarComponents[i].scale);
                    }
                    Game1.playSound("coin");
                }
            }

            //Check if the glam menu tab was clicked
            if (GlamMenuTab.containsPoint(x, y))
            {
                //If there wasn't a favorite applied then restore the player
                if (!WasFavoriteApplied)
                    Menu.RestoreSnapshot();

                //Set the menu back to the glam menu
                Game1.activeClickableMenu = Menu;
            }

            //Check if the set button was clicked
            if (SetButton.containsPoint(x, y))
            {
                //A favorite was applied and change the player
                WasFavoriteApplied = true;
                PlayerLoader.LoadFavorite(CurrentIndex, Menu, true);
                if (SetButton.scale != 0f)
                {
                    SetButton.scale -= 0.25f;
                    SetButton.scale = Math.Max(0.75f, SetButton.scale);
                }
                Game1.playSound("coin");
            }

            //Check if the delete button was clicked
            if (DeleteButton.containsPoint(x, y))
            {
                //Set the slot to a default model and update the star buttons
                PlayerLoader.Favorites[CurrentIndex] = new FavoriteModel();
                UpdateStarButtons();
                if (DeleteButton.scale != 0f)
                {
                    DeleteButton.scale -= 0.25f;
                    DeleteButton.scale = Math.Max(0.75f, DeleteButton.scale);
                }
                Game1.playSound("coin");
            }

            //Check if the left direction button was clicked and change player direction
            if (LeftDirectionButton.containsPoint(x, y))
            {
                Game1.player.faceDirection((Game1.player.facingDirection + 5) % 4);
                Game1.player.FarmerSprite.StopAnimation();
                Game1.player.completelyStopAnimatingOrDoingAction();
                if (LeftDirectionButton.scale != 0f)
                {
                    LeftDirectionButton.scale -= 0.25f;
                    LeftDirectionButton.scale = Math.Max(0.75f, LeftDirectionButton.scale);
                }
                Game1.playSound("pickUpItem");
            }

            //Check if the right direction button was clicked and change player direction
            if (RightDirectionButton.containsPoint(x, y))
            {
                Game1.player.faceDirection((Game1.player.facingDirection - 1 + 4) % 4);
                Game1.player.FarmerSprite.StopAnimation();
                Game1.player.completelyStopAnimatingOrDoingAction();
                if (RightDirectionButton.scale != 0f)
                {
                    RightDirectionButton.scale -= 0.25f;
                    RightDirectionButton.scale = Math.Max(0.75f, LeftDirectionButton.scale);
                }
                Game1.playSound("pickUpItem");
            }
        }

        /// <summary>Override to handle drawing the different parts of the menu</summary>
        /// <param name="b">The games spritebatch</param>
        public override void draw(SpriteBatch b)
        {
            //Draw the dialouge box
            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);

            //Draw the tabs
            GlamMenuTab.draw(b);
            FavoriteMenuTab.draw(b);

            //Draw the portrait box
            b.Draw(Game1.daybg, new Vector2(this.xPositionOnScreen + this.width - 196, this.yPositionOnScreen + 128), Color.White);

            //Draw the farmer
            Game1.player.FarmerRenderer.draw(b, Game1.player.FarmerSprite.CurrentAnimationFrame, Game1.player.FarmerSprite.CurrentFrame, Game1.player.FarmerSprite.SourceRect, new Vector2(this.xPositionOnScreen + this.width - 164, this.yPositionOnScreen + 160), Vector2.Zero, 0.8f, Color.White, 0f, 1f, Game1.player);

            //Draw the set button if it's visible
            if (SetButton.visible)
                SetButton.draw(b);

            //Draw the delete button if it's visible
            if (DeleteButton.visible)
                DeleteButton.draw(b);

            //Draw the direction buttons
            LeftDirectionButton.draw(b);
            RightDirectionButton.draw(b);

            //Draw each of the star components
            foreach (ClickableTextureComponent star in StarComponents)
                star.draw(b);

            //Draw the favorite text and instructions
            Utility.drawTextWithShadow(b, "Favorites", Game1.smallFont, new Vector2(this.xPositionOnScreen + 48, this.yPositionOnScreen + 104), Game1.textColor, 2f);
            Utility.drawTextWithShadow(b, "Iriduim stars are saved favorites.", Game1.smallFont, new Vector2(this.xPositionOnScreen + 48, this.yPositionOnScreen + 160), Game1.textColor, 1f);
            Utility.drawTextWithShadow(b, "Gold stars are default.", Game1.smallFont, new Vector2(this.xPositionOnScreen + 48, this.yPositionOnScreen + 192), Game1.textColor, 1f);
            Utility.drawTextWithShadow(b, "Click on a favorite to view it.", Game1.smallFont, new Vector2(this.xPositionOnScreen + 48, this.yPositionOnScreen + 224), Game1.textColor, 1f);
            Utility.drawTextWithShadow(b, "Use the `Set` to apply a favorite.", Game1.smallFont, new Vector2(this.xPositionOnScreen + 48, this.yPositionOnScreen + 256), Game1.textColor, 1f);
            Utility.drawTextWithShadow(b, "Use the `X` to delete a favorite.", Game1.smallFont, new Vector2(this.xPositionOnScreen + 48, this.yPositionOnScreen + 288), Game1.textColor, 1f);

            //Draw the mouse if they're not using the hardware cursor
            if (Game1.activeClickableMenu == this && !Game1.options.hardwareCursor)
                base.drawMouse(b);
        }
    }
}
