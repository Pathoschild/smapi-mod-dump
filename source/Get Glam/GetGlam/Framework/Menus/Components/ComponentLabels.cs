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
    public class ComponentLabels
    { 
        // Instance of Glam Menu
        private GlamMenu Menu;

        // Instance of GlamMenuComponents
        private GlamMenuComponents MenuComponents;

        // List of new labels added to the menu
        public List<ClickableComponent> NewLabels = new List<ClickableComponent>();

        // Label for the Hat Hair Fix
        public ClickableComponent HatFixLabel;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="menu">Instance of GlamMenu</param>
        /// <param name="components">Instance of GlamMenuComponents</param>
        public ComponentLabels(GlamMenu menu, GlamMenuComponents components)
        {
            Menu = menu;
            MenuComponents = components;
        }

        /// <summary>
        /// Adds labels to the list.
        /// </summary>
        /// <param name="leftButtons"></param>
        public void AddLabels(ComponentLeftButtons leftButtons, ClickableComponent hairButton)
        {
            NewLabels.Add(new ClickableComponent(new Rectangle(leftButtons.GetIndex(0).bounds.X + 70, leftButtons.GetIndex(0).bounds.Y + 16, 1, 1), "Base", "Base"));
            NewLabels.Add(new ClickableComponent(new Rectangle(leftButtons.GetIndex(1).bounds.X + 70, leftButtons.GetIndex(1).bounds.Y + 16, 1, 1), "Skin", "Skin"));
            NewLabels.Add(new ClickableComponent(new Rectangle(leftButtons.GetIndex(2).bounds.X + 70, leftButtons.GetIndex(2).bounds.Y + 16, 1, 1), "Hair", "Hair"));
            NewLabels.Add(new ClickableComponent(new Rectangle(leftButtons.GetIndex(3).bounds.X + 70, leftButtons.GetIndex(3).bounds.Y + 16, 1, 1), "Acc.", "Acc."));
            NewLabels.Add(new ClickableComponent(new Rectangle(leftButtons.GetIndex(4).bounds.X + 70, leftButtons.GetIndex(4).bounds.Y + 16, 1, 1), "Face", "Face"));
            NewLabels.Add(new ClickableComponent(new Rectangle(leftButtons.GetIndex(5).bounds.X + 70, leftButtons.GetIndex(5).bounds.Y + 16, 1, 1), "Nose", "Nose"));
            NewLabels.Add(new ClickableComponent(new Rectangle(leftButtons.GetIndex(6).bounds.X + 70, leftButtons.GetIndex(6).bounds.Y + 16, 1, 1), "Shoe", "Shoe"));
            NewLabels.Add(new ClickableComponent(new Rectangle(leftButtons.GetIndex(7).bounds.X + 70, leftButtons.GetIndex(7).bounds.Y + 16, 1, 1), "Dresser", "Dresser"));
            NewLabels.Add(new ClickableComponent(new Rectangle(Menu.xPositionOnScreen + Menu.width - 212, Menu.yPositionOnScreen + 272, 1, 1), "Eye Color:", "Eye Color:"));
            NewLabels.Add(new ClickableComponent(new Rectangle(Menu.xPositionOnScreen + Menu.width - 212, Menu.yPositionOnScreen + 360, 1, 1), "Hair Color:", "Hair Color:"));
            HatFixLabel = new ClickableComponent(new Rectangle(hairButton.bounds.X + 48, hairButton.bounds.Y, 1, 1), "Hat Ignores Hair", "Hat Ignores Hair");
        }

        /// <summary>
        /// Clears the Labels List.
        /// </summary>
        public void ClearList()
        {
            NewLabels.Clear();
        }

        /// <summary>
        /// Gets the label at a specific index.
        /// </summary>
        /// <param name="index">The index to get in the list</param>
        /// <returns>The component at that index</returns>
        public ClickableComponent GetIndex(int index)
        {
            return NewLabels[index];
        }

        /// <summary>
        /// Draws the Labels.
        /// </summary>
        /// <param name="b">The games spritebatch</param>
        public void Draw(SpriteBatch b)
        {
            foreach (ClickableComponent component in NewLabels)
            {
                if (component.name.Equals("Base") && MenuComponents.ShouldDrawBaseButtons)
                {
                    Utility.drawTextWithShadow(b, component.name, Game1.smallFont, new Vector2(component.bounds.X, component.bounds.Y), Game1.textColor);
                    Utility.drawTextWithShadow(b, Menu.BaseIndex.ToString(), Game1.smallFont, new Vector2(component.bounds.X + 16, component.bounds.Y + 32), Game1.textColor);
                }
                else if (component.name.Equals("Face") && MenuComponents.ShouldDrawNosesAndFaceButtons)
                {
                    Utility.drawTextWithShadow(b, component.name, Game1.smallFont, new Vector2(component.bounds.X, component.bounds.Y), Game1.textColor);
                    Utility.drawTextWithShadow(b, Menu.FaceIndex.ToString(), Game1.smallFont, new Vector2(component.bounds.X + 16, component.bounds.Y + 32), Game1.textColor);
                }
                else if (component.name.Equals("Nose") && MenuComponents.ShouldDrawNosesAndFaceButtons)
                {
                    Utility.drawTextWithShadow(b, component.name, Game1.smallFont, new Vector2(component.bounds.X, component.bounds.Y), Game1.textColor);
                    Utility.drawTextWithShadow(b, Menu.NoseIndex.ToString(), Game1.smallFont, new Vector2(component.bounds.X + 16, component.bounds.Y + 32), Game1.textColor);
                }
                else if (component.name.Equals("Shoe") && MenuComponents.ShouldDrawShoeButtons)
                {
                    Utility.drawTextWithShadow(b, component.name, Game1.smallFont, new Vector2(component.bounds.X, component.bounds.Y), Game1.textColor);
                    Utility.drawTextWithShadow(b, Menu.ShoeIndex.ToString(), Game1.smallFont, new Vector2(component.bounds.X + 16, component.bounds.Y + 32), Game1.textColor);
                }
                else if (component.name.Equals("Hair"))
                {
                    Utility.drawTextWithShadow(b, component.name, Game1.smallFont, new Vector2(component.bounds.X, component.bounds.Y), Game1.textColor);
                    Utility.drawTextWithShadow(b, Game1.player.hair.Value.ToString(), Game1.smallFont, new Vector2(component.bounds.X + 16, component.bounds.Y + 32), Game1.textColor);
                }
                else if (component.name.Equals("Skin"))
                {
                    Utility.drawTextWithShadow(b, component.name, Game1.smallFont, new Vector2(component.bounds.X, component.bounds.Y), Game1.textColor);
                    Utility.drawTextWithShadow(b, Game1.player.skin.Value.ToString(), Game1.smallFont, new Vector2(component.bounds.X + 16, component.bounds.Y + 32), Game1.textColor);
                }
                else if (component.name.Equals("Acc."))
                {
                    Utility.drawTextWithShadow(b, component.name, Game1.smallFont, new Vector2(component.bounds.X, component.bounds.Y), Game1.textColor);
                    Utility.drawTextWithShadow(b, Game1.player.accessory.Value == -1 ? "na" : Game1.player.accessory.Value.ToString(), Game1.smallFont, new Vector2(component.bounds.X + 16, component.bounds.Y + 32), Game1.textColor);
                }
                else if (component.name.Equals("Eye Color:") || component.name.Equals("Hair Color:"))
                {
                    Utility.drawTextWithShadow(b, component.name, Game1.smallFont, new Vector2(component.bounds.X, component.bounds.Y), Game1.textColor);
                }
                else if (component.name.Equals("Dresser") && MenuComponents.ShouldDrawDresserButtons)
                {
                    Utility.drawTextWithShadow(b, component.name, Game1.smallFont, new Vector2(component.bounds.X, component.bounds.Y), Game1.textColor);
                }
            }
        }
    }
}
