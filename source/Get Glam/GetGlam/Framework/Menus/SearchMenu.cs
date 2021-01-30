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
using System;
using System.Collections.Generic;

namespace GetGlam.Framework.Menus
{
    public class SearchMenu : IClickableMenu
    {
        // Instance of ModEntry
        private ModEntry Entry;

        // Instance of GlamMenu
        private GlamMenu Menu;

        // The glam menu tab to change back to the glam menu
        private ClickableTextureComponent GlamMenuTab;

        // The favorite tab for show
        private ClickableTextureComponent FavoriteMenuTab;

        // Tab Button for Search Menu
        private ClickableTextureComponent SearchMenuTab;

        // List that has all the star buttons
        private List<ClickableTextureComponent> StarComponents = new List<ClickableTextureComponent>();

        // Dictionary used for setting hovertext
        private Dictionary<ClickableTextureComponent, string> StarHoverTextDict = new Dictionary<ClickableTextureComponent, string>();

        // X padding between stars
        private int StarPaddingX = 48;

        // Y padding between stars
        private int StarPaddingY = 96;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="menu"></param>
        public SearchMenu(ModEntry entry, GlamMenu menu) :
            base((int)Utility.getTopLeftPositionForCenteringOnScreen(712, 712, 0, 0).X,
                 (int)Utility.getTopLeftPositionForCenteringOnScreen(712, 712, 0, 0).Y - borderWidth,
                 712,
                 712,
                 false)
        {
            Entry = entry;
            Menu = menu;

            SetUpMenu();
        }

        /// <summary>
        /// Sets up the Menu.
        /// </summary>
        private void SetUpMenu()
        {
            CreateTabs();
            AddStarButtons();
        }

        /// <summary>
        /// Creates the tabs for switching between menus.
        /// </summary>
        private void CreateTabs()
        {
            GlamMenuTab = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen - IClickableMenu.borderWidth - 8, this.yPositionOnScreen + 96, 64, 64), Game1.mouseCursors, new Rectangle(672, 80, 16, 16), 4f, false);
            FavoriteMenuTab = new ClickableTextureComponent("FavoriteTab", new Rectangle(this.xPositionOnScreen - IClickableMenu.borderWidth - 8, this.yPositionOnScreen + 160, 64, 64), null, "FavoriteTab", Game1.mouseCursors, new Rectangle(656, 80, 16, 16), 4f, false);
            SearchMenuTab = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen - IClickableMenu.borderWidth + 1, this.yPositionOnScreen + 400, 64, 64), Game1.mouseCursors2, new Rectangle(96, 48, 16, 16), 4f, false);
        }

        /// <summary>
        /// Adds star buttons to the menu.
        /// </summary>
        private void AddStarButtons()
        {
            int index = 0;
            foreach (string hairPack in Menu.PackHelper.HairStyleSearch.Keys)
            {
                if (index % 10 == 0 && index != 0)
                {
                    StarPaddingX = 48;
                    StarPaddingY += 48;
                }

                ClickableTextureComponent starComponent = new ClickableTextureComponent(
                    new Rectangle(this.xPositionOnScreen + StarPaddingX, this.yPositionOnScreen + this.height / 4 + StarPaddingY, 24, 24),
                    Game1.mouseCursors, new Rectangle(346, 392, 8, 8),
                    4f);
                StarHoverTextDict.Add(starComponent, hairPack);
                StarComponents.Add(starComponent);
                StarPaddingX += 64;

                index++;
            }
        }

        /// <summary>
        /// Perform Hover Action Override.
        /// </summary>
        /// <param name="x">X position of the mouse</param>
        /// <param name="y">Y position of the mouse</param>
        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);

            foreach (ClickableTextureComponent star in StarComponents)
            {
                ChangeComponentScalePerformHover(star, x, y);

                if (star.containsPoint(x, y))
                    star.hoverText = StarHoverTextDict[star];
                else
                    star.hoverText = "";
            }
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
        /// Recieve Left Click Override.
        /// </summary>
        /// <param name="x">X position of the mouse</param>
        /// <param name="y">Y position of the mouse</param>
        /// <param name="playSound">Whether to play a sound</param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);

            OnClickStarButtons(x, y);
            OnClickTabs(x, y);
        }

        /// <summary>
        /// On Click For Star Buttons.
        /// </summary>
        /// <param name="x">X position of the mouse</param>
        /// <param name="y">Y position of the mouse</param>
        private void OnClickStarButtons(int x, int y)
        {
            foreach (ClickableTextureComponent star in StarComponents)
            {
                if (star.containsPoint(x, y))
                {
                    Menu.PlayerLoader.SetPlayerHairStyle(Menu.PackHelper.HairStyleSearch[star.hoverText]);
                    Game1.activeClickableMenu = Menu;
                }
            }
        }

        /// <summary>
        /// On Click For Tabs.
        /// </summary>
        /// <param name="x">X position of the mouse</param>
        /// <param name="y">Y position of the mouse</param>
        private void OnClickTabs(int x, int y)
        {
            if (GlamMenuTab.containsPoint(x, y))
                Game1.activeClickableMenu = Menu;

            if (FavoriteMenuTab.containsPoint(x, y))
                Game1.activeClickableMenu = new FavoriteMenu(Entry, Menu.PlayerLoader, Menu);
        }

        /// <summary>
        /// Draw Override.
        /// </summary>
        /// <param name="b">Games SpriteBatch</param>
        public override void draw(SpriteBatch b)
        {
            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);

            DrawTabs(b);
            DrawText(b);
            DrawButtons(b);
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
            SearchMenuTab.draw(b);
        }

        /// <summary>
        /// Draws The Buttons.
        /// </summary>
        /// <param name="b">Game's SpriteBatch</param>
        private void DrawButtons(SpriteBatch b)
        {
            foreach (ClickableTextureComponent star in StarComponents)
                star.draw(b);

            // Loop through to draw hover text on top
            foreach (ClickableTextureComponent star in StarComponents)
                DrawHoverText(b, star);          
        }

        /// <summary>
        /// Draws Static Text.
        /// </summary>
        /// <param name="b">Games SpriteBatch</param>
        private void DrawText(SpriteBatch b)
        {
            Utility.drawTextWithShadow(b, "Jump to a hair pack.", Game1.smallFont, new Vector2(this.xPositionOnScreen + 48, this.yPositionOnScreen + 104), Game1.textColor, 2f);
            Utility.drawTextWithShadow(b, "Click a star to jump to a installed hair pack.", Game1.smallFont, new Vector2(this.xPositionOnScreen + 48, this.yPositionOnScreen + 160), Game1.textColor, 1f);
            Utility.drawTextWithShadow(b, "Hover over a star to view which pack.", Game1.smallFont, new Vector2(this.xPositionOnScreen + 48, this.yPositionOnScreen + 192), Game1.textColor, 1f);
        }

        /// <summary>
        /// Draws Hover Text For Star Buttons.
        /// </summary>
        /// <param name="b">Games SpriteBatch</param>
        /// <param name="star">Current Star Button</param>
        public void DrawHoverText(SpriteBatch b, ClickableTextureComponent star)
        {
            if (!star.hoverText.Equals(""))
                IClickableMenu.drawHoverText(b, star.hoverText, Game1.smallFont, 0, 0, -1, null, -1, null, null, 0, -1, -1, -1, -1, 1f, null, null);
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
