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
    public class ComponentRightButtons
    {
        // Instance of Glam Menu
        private GlamMenu Menu;

        // Instance of GlamMenuComponents
        private GlamMenuComponents MenuComponents;

        // List of new right buttons added to the menu
        private List<ClickableTextureComponent> NewRightButtonsList = new List<ClickableTextureComponent>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="components"></param>
        public ComponentRightButtons(GlamMenu menu, GlamMenuComponents components)
        {
            Menu = menu;
            MenuComponents = components;
        }

        /// <summary>
        /// Adds Right Buttons to the Button List.
        /// </summary>
        public void AddButtons()
        {
            NewRightButtonsList.Add(new ClickableTextureComponent("RightBase", new Rectangle(Menu.xPositionOnScreen + 170, Menu.yPositionOnScreen + 128, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false));
            NewRightButtonsList.Add(new ClickableTextureComponent("RightChangeSkin", new Rectangle(Menu.xPositionOnScreen + 170, Menu.yPositionOnScreen + 192 + MenuComponents.PaddingY, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false));
            NewRightButtonsList.Add(new ClickableTextureComponent("RightChangeHair", new Rectangle(Menu.xPositionOnScreen + 170, Menu.yPositionOnScreen + 256 + MenuComponents.PaddingY, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false));
            NewRightButtonsList.Add(new ClickableTextureComponent("RightChangeAcc", new Rectangle(Menu.xPositionOnScreen + 170, Menu.yPositionOnScreen + 320 + MenuComponents.PaddingY, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false));
            NewRightButtonsList.Add(new ClickableTextureComponent("RightFace", new Rectangle(Menu.xPositionOnScreen + 170, Menu.yPositionOnScreen + 384 + MenuComponents.PaddingY, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false));
            NewRightButtonsList.Add(new ClickableTextureComponent("RightNose", new Rectangle(Menu.xPositionOnScreen + 170, Menu.yPositionOnScreen + 448 + MenuComponents.PaddingY, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false));
            NewRightButtonsList.Add(new ClickableTextureComponent("RightShoe", new Rectangle(Menu.xPositionOnScreen + 170, Menu.yPositionOnScreen + 512 + MenuComponents.PaddingY, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false));
            NewRightButtonsList.Add(new ClickableTextureComponent("RightDresser", new Rectangle(Menu.xPositionOnScreen + Menu.width / 2 + 48, Menu.yPositionOnScreen + 200, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false));
            NewRightButtonsList.Add(new ClickableTextureComponent("RightDirection", new Rectangle(Menu.xPositionOnScreen + Menu.width / 2 + 48, Menu.yPositionOnScreen + 288, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false));
        }

        /// <summary>
        /// Clears the Right Button List.
        /// </summary>
        public void ClearList()
        {
            NewRightButtonsList.Clear();
        }

        /// <summary>
        /// Sets a component in the list visible or invisible.
        /// </summary>
        /// <param name="visible"></param>
        /// <param name="listIndex"></param>
        public void SetComponentVisible(bool visible, int listIndex)
        {
            NewRightButtonsList[listIndex].visible = visible;
        }

        /// <summary>
        /// Gets the right button at a specific index.
        /// </summary>
        /// <param name="index">The index to get in the list</param>
        /// <returns>The component at that index</returns>
        public ClickableTextureComponent GetIndex(int index)
        {
            return NewRightButtonsList[index];
        }

        /// <summary>
        /// Right button on hover.
        /// </summary>
        /// <param name="x">X position of the mouse</param>
        /// <param name="y">Y position of the mouse</param>
        public void OnHover(int x, int y)
        {
            foreach (ClickableTextureComponent leftButton in NewRightButtonsList)
                MenuComponents.ChangeHoverActionScale(leftButton, x, y, 0.01f, 0.1f);
        }

        /// <summary>
        /// Right button Left Click.
        /// </summary>
        /// <param name="x">X position of the mouse</param>
        /// <param name="y">Y position of the mouse</param>
        public void LeftClick(int x, int y)
        {
            foreach (ClickableTextureComponent component in NewRightButtonsList)
            {
                if (component.bounds.Contains(x, y) && component.visible)
                {
                    SelectionClick(component.name, 1);
                    MenuComponents.ChangeScaleLeftClick(component);

                    MenuComponents.CheckIfBald(component);
                    Game1.playSound("grassyStep");
                }
            }
        }

        /// <summary>
        /// Changes made when a right button is clicked.
        /// </summary>
        /// <param name="buttonName">The buttons name</param>
        /// <param name="direction">The direction of the arrow</param>
        private void SelectionClick(string buttonName, int direction)
        {
            switch (buttonName)
            {
                case "RightBase":
                    if (!NewRightButtonsList[0].visible)
                        return;

                    ChangeRightButtonBase(direction);
                    break;

                case "RightNose":
                    ChangeRightButtonNose(direction);
                    break;

                case "RightFace":
                    ChangeRightButtonFace(direction);
                    break;

                case "RightShoe":
                    ChangeRightButtonShoe(direction);
                    break;

                case "RightDresser":
                    if (!NewRightButtonsList[7].visible)
                        return;

                    ChangeRightButtonDresser(direction);
                    break;

                case "RightChangeHair":
                    ChangeRightButtonHair(direction);
                    break;

                case "RightChangeSkin":
                    Game1.player.changeSkinColor(Game1.player.skin.Value + direction);
                    break;

                case "RightChangeAcc":
                    Game1.player.changeAccessory(Game1.player.accessory.Value + direction);
                    break;

                case "RightDirection":
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
        private void ChangeRightButtonBase(int direction)
        {
            if (Menu.BaseIndex + direction > (Game1.player.isMale ? Menu.PackHelper.MaleBaseTextureList.Count : Menu.PackHelper.FemaleBaseTextureList.Count))
                Menu.BaseIndex = 0;
            else
                Menu.BaseIndex += direction;

            Menu.FaceIndex = 0;
            Menu.NoseIndex = 0;
            Menu.PlayerChanger.ChangePlayerBase(Game1.player.isMale, Menu.BaseIndex, Menu.FaceIndex, Menu.NoseIndex, Menu.ShoeIndex, Menu.IsBald);
        }

        /// <summary>
        /// Changes the Nose.
        /// </summary>
        /// <param name="direction">Direction of change</param>
        private void ChangeRightButtonNose(int direction)
        {
            if (Menu.NoseIndex + direction > Menu.PackHelper.GetNumberOfFacesAndNoses(Game1.player.isMale, Menu.BaseIndex, false))
                Menu.NoseIndex = 0;
            else
                Menu.NoseIndex += direction;

            Menu.PlayerChanger.ChangePlayerBase(Game1.player.isMale, Menu.BaseIndex, Menu.FaceIndex, Menu.NoseIndex, Menu.ShoeIndex, Menu.IsBald);
        }

        /// <summary>
        /// Changes the Face.
        /// </summary>
        /// <param name="direction">Direction of change</param>
        private void ChangeRightButtonFace(int direction)
        {
            if (Menu.FaceIndex + direction > Menu.PackHelper.GetNumberOfFacesAndNoses(Game1.player.isMale, Menu.BaseIndex, true))
                Menu.FaceIndex = 0;
            else
                Menu.FaceIndex += direction;

            Menu.PlayerChanger.ChangePlayerBase(Game1.player.isMale, Menu.BaseIndex, Menu.FaceIndex, Menu.NoseIndex, Menu.ShoeIndex, Menu.IsBald);
        }

        /// <summary>
        /// Changes the Shoes.
        /// </summary>
        /// <param name="direction">Direction of change</param>
        private void ChangeRightButtonShoe(int direction)
        {
            if (Menu.ShoeIndex + direction > (Game1.player.isMale ? Menu.PackHelper.MaleShoeTextureList.Count : Menu.PackHelper.FemaleShoeTextureList.Count))
                Menu.ShoeIndex = 0;
            else
                Menu.ShoeIndex += direction;

            Menu.PlayerChanger.ChangePlayerBase(Game1.player.isMale, Menu.BaseIndex, Menu.FaceIndex, Menu.NoseIndex, Menu.ShoeIndex, Menu.IsBald);
        }

        /// <summary>
        /// Change the Dresser.
        /// </summary>
        /// <param name="direction">Direction of change</param>
        private void ChangeRightButtonDresser(int direction)
        {
            if (Menu.DresserIndex + direction <= Menu.Dresser.GetNumberOfDressers())
                Menu.DresserIndex += direction;
            else
                Menu.DresserIndex = 1;

            // Change the Dressers source rect and update the dresser in the farmhouse
            Menu.Dresser.TextureSourceRect.Y = Menu.DresserIndex.Equals(1) ? 0 : Menu.DresserIndex * 32 - 32;
            Menu.Dresser.SetDresserTileSheetPoint(Menu.DresserIndex);
            Menu.Dresser.UpdateDresserInFarmHouse();
        }

        /// <summary>
        /// Changes the hair
        /// </summary>
        /// <param name="direction">Direction of change</param>
        private void ChangeRightButtonHair(int direction)
        {
            if (Menu.PackHelper.NumberOfHairstlyesAdded == 74 && Game1.player.hair.Value + 1 > FarmerRenderer.hairStylesTexture.Height / 96 * 8 - 1)
                Game1.player.hair.Set(0);
            else if (Menu.PackHelper.NumberOfHairstlyesAdded != 74 && Game1.player.hair.Value.Equals(Menu.PackHelper.NumberOfHairstlyesAdded - 1))
                Game1.player.hair.Set(0);
            else
                Game1.player.hair.Set(Game1.player.hair.Value + direction);
        }

        /// <summary>
        /// Draws the right buttons.
        /// </summary>
        /// <param name="b">The games spritebatch</param>
        public void Draw(SpriteBatch b)
        {
            foreach (ClickableTextureComponent component in NewRightButtonsList)
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
