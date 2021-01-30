/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MartyrPher/GetGlam
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;

namespace GetGlam.Framework.Menus.Components
{
    public class ComponentLeftButtons
    {
        // Instance of Glam Menu
        private GlamMenu Menu;

        // Instance of GlamMenuComponents
        private GlamMenuComponents MenuComponents;

        // List of new left buttons added to the menu
        public List<ClickableTextureComponent> NewLeftButtonsList = new List<ClickableTextureComponent>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="menu">Instance of GlamMenu</param>
        /// <param name="components">Instance of GlamMenuComponents</param>
        public ComponentLeftButtons(GlamMenu menu, GlamMenuComponents components)
        {
            Menu = menu;
            MenuComponents = components;
        }

        /// <summary>
        /// Adds Left Arrow Buttons to a List.
        /// </summary>
        public void AddButtons()
        {
            NewLeftButtonsList.Add(new ClickableTextureComponent("LeftBase", new Rectangle(Menu.xPositionOnScreen + 44, Menu.yPositionOnScreen + 128, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false));
            NewLeftButtonsList.Add(new ClickableTextureComponent("LeftChangeSkin", new Rectangle(Menu.xPositionOnScreen + 44, Menu.yPositionOnScreen + 192 + MenuComponents.PaddingY, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false));
            NewLeftButtonsList.Add(new ClickableTextureComponent("LeftChangeHair", new Rectangle(Menu.xPositionOnScreen + 44, Menu.yPositionOnScreen + 256 + MenuComponents.PaddingY, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false));
            NewLeftButtonsList.Add(new ClickableTextureComponent("LeftChangeAcc", new Rectangle(Menu.xPositionOnScreen + 44, Menu.yPositionOnScreen + 320 + MenuComponents.PaddingY, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false));
            NewLeftButtonsList.Add(new ClickableTextureComponent("LeftFace", new Rectangle(Menu.xPositionOnScreen + 44, Menu.yPositionOnScreen + 384 + MenuComponents.PaddingY, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false));
            NewLeftButtonsList.Add(new ClickableTextureComponent("LeftNose", new Rectangle(Menu.xPositionOnScreen + 44, Menu.yPositionOnScreen + 448 + MenuComponents.PaddingY, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false));
            NewLeftButtonsList.Add(new ClickableTextureComponent("LeftShoe", new Rectangle(Menu.xPositionOnScreen + 44, Menu.yPositionOnScreen + 512 + MenuComponents.PaddingY, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false));
            NewLeftButtonsList.Add(new ClickableTextureComponent("LeftDresser", new Rectangle(Menu.xPositionOnScreen + Menu.width / 2 - 114, Menu.yPositionOnScreen + 200, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false));
            NewLeftButtonsList.Add(new ClickableTextureComponent("LeftDirection", new Rectangle(Menu.xPositionOnScreen + Menu.width / 2 - 114, Menu.yPositionOnScreen + 288, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false));
        }

        /// <summary>
        /// Clears the Left Button List.
        /// </summary>
        public void ClearList()
        {
            NewLeftButtonsList.Clear();
        }

        /// <summary>
        /// Sets a component in the list visible or invisible.
        /// </summary>
        /// <param name="visible"></param>
        /// <param name="listIndex"></param>
        public void SetComponentVisible(bool visible, int listIndex)
        {
            NewLeftButtonsList[listIndex].visible = visible;
        }

        /// <summary>
        /// Gets the left button at a specific index.
        /// </summary>
        /// <param name="index">The index to get in the list</param>
        /// <returns>The component at that index</returns>
        public ClickableTextureComponent GetIndex(int index)
        {
            return NewLeftButtonsList[index];
        }

        /// <summary>
        /// Left Button on hover.
        /// </summary>
        /// <param name="x">X position of the mouse</param>
        /// <param name="y">Y position of the mouse</param>
        public void OnHover(int x, int y)
        {
            foreach (ClickableTextureComponent leftButton in NewLeftButtonsList)
                MenuComponents.ChangeHoverActionScale(leftButton, x, y, 0.01f, 0.1f);
        }

        /// <summary>
        /// Left button Left Click.
        /// </summary>
        /// <param name="x">X position of the mouse</param>
        /// <param name="y">Y position of the mouse</param>
        public void LeftClick(int x, int y)
        {
            foreach (ClickableTextureComponent component in NewLeftButtonsList)
            {
                if (component.bounds.Contains(x, y) && component.visible)
                {
                    SelectionClick(component.name, -1);
                    MenuComponents.ChangeScaleLeftClick(component);

                    MenuComponents.CheckIfBald(component);
                    Game1.playSound("grassyStep");
                }
            }
        }

        /// <summary>
        /// Changes made when a left button is clicked.
        /// </summary>
        /// <param name="buttonName">The buttons name</param>
        /// <param name="direction">The direction of the arrow</param>
        private void SelectionClick(string buttonName, int direction)
        {
            switch (buttonName)
            {
                case "LeftBase":
                    if (!NewLeftButtonsList[0].visible)
                        return;

                    ChangeLeftButtonBase(direction);
                    break;

                case "LeftNose":
                    ChangeLeftButtonNose(direction);
                    break;

                case "LeftFace":
                    ChangeLeftButtonFace(direction);
                    break;

                case "LeftShoe":
                    ChangeLeftButtonShoe(direction);
                    break;

                case "LeftDresser":
                    if (!NewLeftButtonsList[7].visible)
                        return;

                    ChangeLeftButtonDresser(direction);
                    break;

                case "LeftChangeHair":
                    ChangeLeftButtonHair(direction);
                    break;

                case "LeftChangeSkin":
                    Game1.player.changeSkinColor(Game1.player.skin.Value + direction);
                    break;

                case "LeftChangeAcc":
                    Game1.player.changeAccessory(Game1.player.accessory.Value + direction);
                    break;

                case "LeftDirection":
                    Game1.player.faceDirection((Game1.player.FacingDirection - direction + 4) % 4);
                    Game1.player.FarmerSprite.StopAnimation();
                    Game1.player.completelyStopAnimatingOrDoingAction();
                    break;
            }
        }

        /// <summary>
        /// Changes the Base.
        /// </summary>
        /// <param name="direction">Direction of change</param>
        private void ChangeLeftButtonBase(int direction)
        {
            if (Menu.BaseIndex + direction > -1)
                Menu.BaseIndex += direction;
            else
                Menu.BaseIndex = Game1.player.isMale ? Menu.PackHelper.MaleBaseTextureList.Count : Menu.PackHelper.FemaleBaseTextureList.Count;

            Menu.FaceIndex = 0;
            Menu.NoseIndex = 0;
            Menu.PlayerChanger.ChangePlayerBase(Game1.player.isMale, Menu.BaseIndex, Menu.FaceIndex, Menu.NoseIndex, Menu.ShoeIndex, Menu.IsBald);
        }

        /// <summary>
        /// Changes the Nose.
        /// </summary>
        /// <param name="direction">Direction of change</param>
        private void ChangeLeftButtonNose(int direction)
        {
            if (Menu.NoseIndex + direction > -1)
                Menu.NoseIndex += direction;
            else
                Menu.NoseIndex = Menu.PackHelper.GetNumberOfFacesAndNoses(Game1.player.isMale, Menu.BaseIndex, false);

            Menu.PlayerChanger.ChangePlayerBase(Game1.player.isMale, Menu.BaseIndex, Menu.FaceIndex, Menu.NoseIndex, Menu.ShoeIndex, Menu.IsBald);
        }

        /// <summary>
        /// Changes the Face.
        /// </summary>
        /// <param name="direction">Direction of change</param>
        private void ChangeLeftButtonFace(int direction)
        {
            if (Menu.FaceIndex + direction > -1)
                Menu.FaceIndex += direction;
            else
                Menu.FaceIndex = Menu.PackHelper.GetNumberOfFacesAndNoses(Game1.player.isMale, Menu.BaseIndex, true);

            Menu.PlayerChanger.ChangePlayerBase(Game1.player.isMale, Menu.BaseIndex, Menu.FaceIndex, Menu.NoseIndex, Menu.ShoeIndex, Menu.IsBald);
        }

        /// <summary>
        /// Changes the Dresser.
        /// </summary>
        /// <param name="direction">Direction of change</param>
        private void ChangeLeftButtonDresser(int direction)
        {
            if (Menu.DresserIndex + direction > 0)
                Menu.DresserIndex += direction;
            else
                Menu.DresserIndex = Menu.Dresser.GetNumberOfDressers();

            // Change the Dressers source rect and update the dresser in the farmhouse
            Menu.Dresser.TextureSourceRect.Y = Menu.DresserIndex.Equals(1) ? 0 : Menu.DresserIndex * 32 - 32;
            Menu.Dresser.SetDresserTileSheetPoint(Menu.DresserIndex);
            Menu.Dresser.UpdateDresserInFarmHouse();
        }

        /// <summary>
        /// Changes the Shoe.
        /// </summary>
        /// <param name="direction">Direction of change</param>
        private void ChangeLeftButtonShoe(int direction)
        {
            if (Menu.ShoeIndex + direction > -1)
                Menu.ShoeIndex += direction;
            else
                Menu.ShoeIndex = Game1.player.isMale ? Menu.PackHelper.MaleShoeTextureList.Count : Menu.PackHelper.FemaleShoeTextureList.Count;

            Menu.PlayerChanger.ChangePlayerBase(Game1.player.isMale, Menu.BaseIndex, Menu.FaceIndex, Menu.NoseIndex, Menu.ShoeIndex, Menu.IsBald);
        }

        /// <summary>
        /// Changes the Hair.
        /// </summary>
        /// <param name="direction">Direction of change</param>
        private void ChangeLeftButtonHair(int direction)
        {
            if (Game1.player.hair.Get().Equals(0) && Menu.PackHelper.NumberOfHairstlyesAdded == 74)
                Game1.player.hair.Set(FarmerRenderer.hairStylesTexture.Height / 96 * 8 - 1);
            else if (Game1.player.hair.Get().Equals(0) && Menu.PackHelper.NumberOfHairstlyesAdded != 74)
                Game1.player.hair.Set(Menu.PackHelper.NumberOfHairstlyesAdded - 1);
            else
                Game1.player.hair.Set(Game1.player.hair.Value + direction);
        }

        /// <summary>
        /// Draws the Left Buttons.
        /// </summary>
        /// <param name="b">The games spritebatch</param>
        public void Draw(SpriteBatch b) 
        {
            foreach (ClickableTextureComponent component in NewLeftButtonsList)
            {
                if (component.name.Contains("Base") && MenuComponents.ShouldDrawBaseButtons)
                    component.draw(b);
                else if (component.name.Contains("Dresser") && MenuComponents.ShouldDrawDresserButtons)
                    component.draw(b);
                else if ((component.name.Contains("Nose") || component.name.Contains("Face") && MenuComponents.ShouldDrawNosesAndFaceButtons))
                    component.draw(b);
                else if (!component.name.Contains("Dresser") && !component.name.Contains("Base") && !component.name.Contains("Face") && !component.name.Contains("Nose"))
                    component.draw(b);
            }
        }
    }
}
