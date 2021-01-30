/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MartyrPher/GetGlam
**
*************************************************/

using GetGlam.Framework.Menus.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;

namespace GetGlam.Framework.Menus
{
    public class GlamMenuComponents
    {
        // Instance of ModEntry
        private ModEntry Entry;

        // Instance of GlamMenu
        private GlamMenu Menu;

        // Instance of ContentPackHelper
        private ContentPackHelper PackHelper;

        // Left Buttons
        private ComponentLeftButtons LeftButtons;

        // Right Buttons
        private ComponentRightButtons RightButtons;

        // Labels
        private ComponentLabels Labels;

        // Buttons
        private ComponentButtons Buttons;

        // Eye Color Picker
        private ColorPicker EyeColorPicker;

        // Hair Color Picker
        private ColorPicker HairColorPicker;

        // Padding for each selection Y
        public int PaddingY = 8;

        // Whether to draw the base buttons
        public bool ShouldDrawBaseButtons = false;

        // Whether to draw the dresser button
        public bool ShouldDrawDresserButtons = false;

        // Whether to draw the face and nose buttons
        public bool ShouldDrawNosesAndFaceButtons = false;

        // Whether to draw the shoe buttons
        public bool ShouldDrawShoeButtons = false;

        // Whether the button is selected
        public bool IsHatFixSelected = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="menu"></param>
        /// <param name="packHelper"></param>
        public GlamMenuComponents(ModEntry entry, GlamMenu menu, ContentPackHelper packHelper)
        {
            Entry = entry;
            Menu = menu;
            PackHelper = packHelper;

            InitializeComponents();
        }

        /// <summary>
        /// Initialized Different Components.
        /// </summary>
        public void InitializeComponents()
        {
            LeftButtons = new ComponentLeftButtons(Menu, this);
            RightButtons = new ComponentRightButtons(Menu, this);
            Buttons = new ComponentButtons(Entry, Menu, this);
            Labels = new ComponentLabels(Menu, this);
        }

        /// <summary>
        /// Sets Up The Components.
        /// </summary>
        /// <remarks>This is also called on WindowChanged.</remarks>
        public void SetUpMenuComponents()
        {
            ClearLists();
            LeftButtons.AddButtons();
            RightButtons.AddButtons();
            Buttons.AddButtons();
            Labels.AddLabels(LeftButtons, Buttons.HatCoversHairButton);

            CreateColorPickers();
            SetButtonsToInvisible();

            EnableBaseButton();
            EnableDresserButtons();
            EnableFaceNoseButtons();
            EnableShoesButton();
        }

        /// <summary>
        /// Clears the lists incase of window change.
        /// </summary>
        private void ClearLists()
        {
            LeftButtons.ClearList();
            RightButtons.ClearList();
            Labels.ClearList();
            Buttons.ClearButtons();
        }

        /// <summary>
        /// Creates the Color Pickers.
        /// </summary>
        private void CreateColorPickers()
        {
            EyeColorPicker = new ColorPicker("Eyes", Labels.GetIndex(8).bounds.X, Labels.GetIndex(8).bounds.Y + 32);
            EyeColorPicker.setColor(Game1.player.newEyeColor.Value);

            HairColorPicker = new ColorPicker("Hair", Labels.GetIndex(9).bounds.X, Labels.GetIndex(9).bounds.Y + 32);
            HairColorPicker.setColor(Game1.player.hairstyleColor.Value);
        }

        /// <summary>
        /// Sets buttons to invisible.
        /// </summary>
        private void SetButtonsToInvisible()
        {
            // Base
            LeftButtons.SetComponentVisible(false, 0);
            RightButtons.SetComponentVisible(false, 0);

            // Dresser
            LeftButtons.SetComponentVisible(false, 7);
            RightButtons.SetComponentVisible(false, 7);

            // Shoes
            LeftButtons.SetComponentVisible(false, 6);
            RightButtons.SetComponentVisible(false, 6);
        }

        /// <summary>
        /// Enables Base Buttons.
        /// </summary>
        private void EnableBaseButton()
        {
            // Check if the menu should draw the base buttons if the base count is greater than 1
            if (Game1.player.isMale && PackHelper.MaleBaseTextureList.Count > 0)
            {
                ShouldDrawBaseButtons = true;
                LeftButtons.SetComponentVisible(true, 0);
                RightButtons.SetComponentVisible(true, 0);
            }
            else if (PackHelper.FemaleBaseTextureList.Count > 0)
            {
                ShouldDrawBaseButtons = true;
                LeftButtons.SetComponentVisible(true, 0);
                RightButtons.SetComponentVisible(true, 0);
            }
        }

        /// <summary>
        /// Enables Dresser buttons.
        /// </summary>
        private void EnableDresserButtons()
        {
            if (Menu.Dresser.GetNumberOfDressers() > 1)
            {
                ShouldDrawDresserButtons = true;
                LeftButtons.SetComponentVisible(true, 7);
                RightButtons.SetComponentVisible(true, 7);
            }
        }

        /// <summary>
        /// Enables Face and Nose Buttons.
        /// </summary>
        private void EnableFaceNoseButtons()
        {
            if (PackHelper.GetNumberOfFacesAndNoses(Game1.player.isMale, Menu.BaseIndex, true) > 0 && Entry.Config.PatchFaceNose)
            {
                ShouldDrawNosesAndFaceButtons = true;
                UpdateFaceAndNoseButtonsPositions(ShouldDrawNosesAndFaceButtons);
            }
            else
            {
                ShouldDrawNosesAndFaceButtons = false;
                UpdateFaceAndNoseButtonsPositions(ShouldDrawNosesAndFaceButtons);
            }
        }

        /// <summary>
        /// Enables Shoe Button.
        /// </summary>
        private void EnableShoesButton()
        {
            if (Entry.Config.PatchShoes)
            {
                LeftButtons.SetComponentVisible(true, 6);
                RightButtons.SetComponentVisible(true, 6);
                ShouldDrawShoeButtons = true;
            }
        }

        /// <summary>
        /// Update the buttons for changing the face and nose.
        /// </summary>
        /// <param name="isFaceAndNoseDrawing">Whether the face and nose buttons are drawing</param>
        private void UpdateFaceAndNoseButtonsPositions(bool isFaceAndNoseDrawing)
        {
            if (isFaceAndNoseDrawing && Entry.Config.PatchFaceNose)
            {
                LeftButtons.SetComponentVisible(true, 4);
                LeftButtons.SetComponentVisible(true, 5);
                RightButtons.SetComponentVisible(true, 4);
                RightButtons.SetComponentVisible(true, 5);

                LeftButtons.GetIndex(6).bounds.Y = Menu.yPositionOnScreen + 512 + PaddingY;
                RightButtons.GetIndex(6).bounds.Y = Menu.yPositionOnScreen + 512 + PaddingY;
                Labels.GetIndex(6).bounds.Y = LeftButtons.GetIndex(6).bounds.Y + 16;
            }
            else
            {
                LeftButtons.SetComponentVisible(false, 4);
                LeftButtons.SetComponentVisible(false, 5);
                RightButtons.SetComponentVisible(false, 4);
                RightButtons.SetComponentVisible(false, 5);

                LeftButtons.GetIndex(6).bounds.Y = Menu.yPositionOnScreen + 384 + PaddingY;
                RightButtons.GetIndex(6).bounds.Y = Menu.yPositionOnScreen + 384 + PaddingY;
                Labels.GetIndex(6).bounds.Y = LeftButtons.GetIndex(6).bounds.Y + 16;
            }
        }

        /// <summary>
        /// Handles Perform Hover Logic.
        /// </summary>
        /// <param name="x">X position of the mouse</param>
        /// <param name="y">Y position of the mouse</param>
        public void OnHover(int x, int y) 
        {
            LeftButtons.OnHover(x, y);
            RightButtons.OnHover(x, y);
            Buttons.OnHover(x, y);

            if (EyeColorPicker.containsPoint(x, y) || HairColorPicker.containsPoint(x, y))
                Game1.SetFreeCursorDrag();
        }

        /// <summary>
        /// Changes Scale When A Component Is Hovered.
        /// </summary>
        /// <param name="component">Current Component</param>
        /// <param name="x">X Position Of The Mouse</param>
        /// <param name="y">Y Position Of The Mouse</param>
        /// <param name="min">The Minimum Scale</param>
        /// <param name="max">The Maximum Scale</param>
        public void ChangeHoverActionScale(ClickableTextureComponent component, int x, int y, float min, float max)
        {
            if (component.containsPoint(x, y))
                component.scale = Math.Min(component.scale + min, component.baseScale + max);
            else
                component.scale = Math.Max(component.scale - min, component.baseScale);
        }

        /// <summary>
        /// Logic For Holding In LeftClick.
        /// </summary>
        /// <param name="x">X Position Of The Mouse</param>
        /// <param name="y">Y Position Of The Mouse</param>
        /// <remarks>Used For ColorPickers</remarks>
        public void LeftClickHeld(int x, int y)
        {
            if (EyeColorPicker.containsPoint(x, y))
            {
                EyeColorPicker.clickHeld(x, y);
                Game1.player.newEyeColor.Set(EyeColorPicker.getSelectedColor());
                Game1.player.changeEyeColor(Game1.player.newEyeColor);
            }

            if (HairColorPicker.containsPoint(x, y))
            {
                HairColorPicker.clickHeld(x, y);
                Game1.player.hairstyleColor.Set(HairColorPicker.getSelectedColor());
                Game1.player.changeHairColor(Game1.player.hairstyleColor);
            }
        }

        /// <summary>
        /// Logic For Releasing Left Click.
        /// </summary>
        /// <param name="x">X Position Of The Mouse</param>
        /// <param name="y">Y Position Of The Mouse</param>
        public void LeftClickReleased(int x, int y)
        {
            if (EyeColorPicker.containsPoint(x, y))
                EyeColorPicker.releaseClick();

            if (HairColorPicker.containsPoint(x, y))
                HairColorPicker.releaseClick();
        }

        /// <summary>
        /// Logic For Left Click.
        /// </summary>
        /// <param name="x">X Position Of The Mouse</param>
        /// <param name="y">Y Position Of The Mouse</param>
        public void LeftClick(int x, int y)
        {
            LeftButtons.LeftClick(x, y);
            RightButtons.LeftClick(x, y);
            Buttons.LeftClick(x, y);

            LeftClickColorPicker(x, y);
        } 

        /// <summary>
        /// ColorPicker Left Click.
        /// </summary>
        /// <param name="x">X Position Of The Mouse</param>
        /// <param name="y">Y Position Of The Mouse</param>
        public void LeftClickColorPicker(int x, int y)
        {
            // Eye and Color Picker clicks
            if (EyeColorPicker.containsPoint(x, y))
                Game1.player.changeEyeColor(EyeColorPicker.click(x, y));

            if (HairColorPicker.containsPoint(x, y))
                Game1.player.changeHairColor(HairColorPicker.click(x, y));
        }    

        /// <summary>
        /// Changes Scale Of A Component When Clicked.
        /// </summary>
        /// <param name="component">The Current Component</param>
        public void ChangeScaleLeftClick(ClickableTextureComponent component)
        {
            if (component.scale != 0f)
            {
                component.scale -= 0.25f;
                component.scale = Math.Max(0.75f, component.scale);
            }
        }

        /// <summary>
        /// Checks If The Player Is Bald.
        /// </summary>
        /// <param name="component">Current Component</param>
        public void CheckIfBald(ClickableTextureComponent component)
        {
            if (Game1.player.hair.Value - 49 <= 6 && !Menu.IsBald && component.name.Contains("ChangeHair"))
            {
                Menu.IsBald = true;
                Menu.PlayerChanger.ChangePlayerBase(Game1.player.isMale, Menu.BaseIndex, Menu.FaceIndex, Menu.NoseIndex, Menu.ShoeIndex, Menu.IsBald);
            }
            else if (Menu.IsBald && !(Game1.player.hair.Value - 49 <= 6) && component.name.Contains("ChangeHair"))
            {
                Menu.IsBald = false;
                Menu.PlayerChanger.ChangePlayerBase(Game1.player.isMale, Menu.BaseIndex, Menu.FaceIndex, Menu.NoseIndex, Menu.ShoeIndex, Menu.IsBald);
            }
        }

        /// <summary>
        /// Handles Drawing Components.
        /// </summary>
        /// <param name="b">Games SpriteBatch</param>
        public void Draw(SpriteBatch b)
        {
            DrawDresser(b);

            LeftButtons.Draw(b);
            RightButtons.Draw(b);
            Labels.Draw(b);
            Buttons.Draw(b, Labels.HatFixLabel);

            EyeColorPicker.draw(b);
            HairColorPicker.draw(b);
        }

        /// <summary>
        /// Draws The Dresser.
        /// </summary>
        /// <param name="b">Games SpriteBatch</param>
        private void DrawDresser(SpriteBatch b)
        {
            if (Menu.Config.DrawDresserInMenu)
                b.Draw(PackHelper.DresserTexture, new Vector2(Menu.xPositionOnScreen + Menu.width / 2 - 96, Menu.yPositionOnScreen + Menu.height / 2 - 80), Menu.Dresser.TextureSourceRect, Color.White, 0f, Vector2.Zero, 12f, SpriteEffects.None, 0.86f);
            else
            {
                b.Draw(Game1.daybg, new Vector2(Menu.xPositionOnScreen + Menu.width / 2 - 64, Menu.yPositionOnScreen + Menu.height / 2 - 64), Color.White);
                if (ShouldDrawDresserButtons)
                    b.Draw(PackHelper.DresserTexture, new Vector2(RightButtons.GetIndex(4).bounds.X + 64, RightButtons.GetIndex(4).bounds.Y), Menu.Dresser.TextureSourceRect, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.86f);
            }
        }
    }
}
