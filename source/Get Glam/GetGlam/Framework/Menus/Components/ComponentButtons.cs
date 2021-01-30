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
    public class ComponentButtons
    {
        // Instance of ModEntry
        private ModEntry Entry;

        // Instance of Glam Menu
        private GlamMenu Menu;

        // Instance of GlamMenuComponents
        private GlamMenuComponents MenuComponents;

        // Both gender buttons
        private List<ClickableTextureComponent> GenderButtons = new List<ClickableTextureComponent>();

        // Button for the Hat Hair Fix
        public ClickableTextureComponent HatCoversHairButton;

        // Button to save a layout to the favorites
        private ClickableTextureComponent AddToFavoritesButton;

        // Tab Button for the favorite menu
        private ClickableTextureComponent FavoriteMenuTab;

        // Tab Button for Glam Menu
        private ClickableTextureComponent GlamMenuTab;

        // Tab Button for Search Menu
        private ClickableTextureComponent SearchMenuTab;

        // Clothing tab
        private ClickableTextureComponent ClothingTab;

        // Okay Button
        private ClickableTextureComponent OkButton;

        // Cancel button
        private ClickableTextureComponent CancelButton;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="menu">Instance of GlamMenu</param>
        /// <param name="components">Instance of GlamMenuComponents</param>
        public ComponentButtons(ModEntry entry, GlamMenu menu, GlamMenuComponents components)
        {
            Entry = entry;
            Menu = menu;
            MenuComponents = components;
        }

        /// <summary>
        /// Add Buttons.
        /// </summary>
        public void AddButtons()
        {
            FavoriteMenuTab = new ClickableTextureComponent("FavoriteTab", new Rectangle(Menu.xPositionOnScreen - IClickableMenu.borderWidth - 8, Menu.yPositionOnScreen + 160, 64, 64), null, "FavoriteTab", Game1.mouseCursors, new Rectangle(656, 80, 16, 16), 4f, false);
            GlamMenuTab = new ClickableTextureComponent(new Rectangle(Menu.xPositionOnScreen - IClickableMenu.borderWidth + 1, Menu.yPositionOnScreen + 96, 64, 64), Game1.mouseCursors, new Rectangle(672, 80, 15, 16), 4f, false);
            SearchMenuTab = new ClickableTextureComponent(new Rectangle(Menu.xPositionOnScreen - IClickableMenu.borderWidth - 8, Menu.yPositionOnScreen + 400, 64, 64), Game1.mouseCursors2, new Rectangle(96, 48, 16, 16), 4f, false);

            HatCoversHairButton = new ClickableTextureComponent("HatFix", new Rectangle(Menu.xPositionOnScreen + Menu.width / 2 - 114, Menu.yPositionOnScreen + 128, 36, 36), null, "Hat Hair Fix", Game1.mouseCursors, new Rectangle(227, 425, 9, 9), 4f, false);
            AddToFavoritesButton = new ClickableTextureComponent("Favorite", new Rectangle(Menu.xPositionOnScreen + Menu.width - 96, Menu.yPositionOnScreen + 224, 48, 48), null, "Favorite", Game1.mouseCursors, new Rectangle(346, 392, 8, 8), 6f, false);

            AddOkayAndCancelButtons();
            AddGenderButtons();
            AddCustomizeAnywhereTab();

            HatCoversHairButton.visible = false;
        }

        /// <summary>
        /// Adds Okay and Cancel Buttons.
        /// </summary>
        private void AddOkayAndCancelButtons()
        {
            OkButton = new ClickableTextureComponent("Ok", new Rectangle(Menu.xPositionOnScreen + Menu.width - 108, Menu.yPositionOnScreen + Menu.height - 108, 64, 64), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46, -1, -1), 1f, false);
            CancelButton = new ClickableTextureComponent("Cancel", new Rectangle(OkButton.bounds.X - 74, this.OkButton.bounds.Y, 64, 64), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47, -1, -1), 1f, false);
        }

        /// <summary>
        /// Adds Gender Buttons.
        /// </summary>
        private void AddGenderButtons()
        {
            GenderButtons.Add(new ClickableTextureComponent(
                "Male",
                new Rectangle(Menu.xPositionOnScreen + Menu.width - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth - 128, Menu.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder, 64, 64),
                null,
                "Male",
                Game1.mouseCursors,
                new Rectangle(128, 192, 16, 16),
                4f,
                false)
            );

            GenderButtons.Add(new ClickableTextureComponent(
                "Female",
                new Rectangle(Menu.xPositionOnScreen + Menu.width - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth - 64, Menu.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder, 64, 64),
                null,
                "Female",
                Game1.mouseCursors,
                new Rectangle(144, 192, 16, 16),
                4f,
                false)
            );
        }

        /// <summary>
        /// Applies the clothing tab is Customize Anywhere is installed.
        /// </summary>
        private void AddCustomizeAnywhereTab()
        {
            if (Entry.IsCustomizeAnywhereInstalled)
                ClothingTab = new ClickableTextureComponent("ClothingTab", new Rectangle(Menu.xPositionOnScreen - IClickableMenu.borderWidth - 8, Menu.yPositionOnScreen + 224, 64, 64), null, "ClothingTab", Game1.mouseCursors2, new Rectangle(0, 48, 16, 16), 4f, false);
        }

        /// <summary>
        /// Clears the Gender Button List.
        /// </summary>
        public void ClearButtons()
        {
            GenderButtons.Clear();
        }

        /// <summary>
        /// Button On Hover.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void OnHover(int x, int y)
        {
            foreach (ClickableTextureComponent genderButton in GenderButtons)
                MenuComponents.ChangeHoverActionScale(genderButton, x, y, 0.05f, 0.5f);

            MenuComponents.ChangeHoverActionScale(AddToFavoritesButton, x, y, 0.05f, 0.5f);
            MenuComponents.ChangeHoverActionScale(OkButton, x, y, 0.05f, 0.1f);
            MenuComponents.ChangeHoverActionScale(CancelButton, x, y, 0.05f, 0.1f);
        }

        /// <summary>
        /// Button On Click.
        /// </summary>
        /// <param name="x">X position of the mouse</param>
        /// <param name="y">Y position of the mouse</param>
        public void LeftClick(int x, int y)
        {
            LeftClickHatCoverHairButton(x, y);
            LeftClickGenderButtons(x, y);
            LeftClickFavoritesButton(x, y);
            LeftClickFavoriteMenu(x, y);
            LeftClickSearchMenu(x, y);
            LeftClickOkayButton(x, y);
            LeftClickCancelButton(x, y);
            LeftClickCustomizeAnywhere(x, y);
        }

        /// <summary>
        /// Left click hat covers hair.
        /// </summary>
        /// <param name="x">X position of the mouse</param>
        /// <param name="y">Y position of the mouse</param>
        private void LeftClickHatCoverHairButton(int x, int y)
        {
            if (HatCoversHairButton.bounds.Contains(x, y) && !MenuComponents.IsHatFixSelected && HatCoversHairButton.visible)
            {
                MenuComponents.IsHatFixSelected = true;
                Game1.player.hat.Value.hairDrawType.Set(0);
                HatCoversHairButton.sourceRect.X = 236;
            }
            else if (HatCoversHairButton.bounds.Contains(x, y) && MenuComponents.IsHatFixSelected && HatCoversHairButton.visible)
            {
                MenuComponents.IsHatFixSelected = false;
                Game1.player.hat.Value.hairDrawType.Set(1);
                HatCoversHairButton.sourceRect.X = 227;
            }
        }

        /// <summary>
        /// Left Click Gender Buttons.
        /// </summary>
        /// <param name="x">X position of the mouse</param>
        /// <param name="y">Y position of the mouse</param>
        public void LeftClickGenderButtons(int x, int y)
        {
            foreach (ClickableTextureComponent genderButton in GenderButtons)
            {
                if (genderButton.containsPoint(x, y))
                {
                    SelectionClick(genderButton.name, 0);
                    Game1.playSound("yoba");
                }
            }
        }

        /// <summary>
        /// Left Click Favorites Button.
        /// </summary>
        /// <param name="x">X position of the mouse</param>
        /// <param name="y">Y position of the mouse</param>
        public void LeftClickFavoritesButton(int x, int y)
        {
            if (AddToFavoritesButton.containsPoint(x, y))
            {
                Menu.PlayerLoader.SaveFavoriteToList(Game1.player.isMale, Menu.BaseIndex, Game1.player.skin.Get(), Game1.player.hair.Get(), Menu.FaceIndex, Menu.NoseIndex, Menu.ShoeIndex, Game1.player.accessory.Get(), Menu.IsBald);
                MenuComponents.ChangeScaleLeftClick(AddToFavoritesButton);
                Game1.playSound("coin");
            }
        }

        /// <summary>
        /// Left Click Favorite Menu.
        /// </summary>
        /// <param name="x">X position of the mouse</param>
        /// <param name="y">Y position of the mouse</param>
        public void LeftClickFavoriteMenu(int x, int y)
        {
            if (FavoriteMenuTab.containsPoint(x, y))
            {
                Menu.TakeSnapshot();
                Game1.activeClickableMenu = new FavoriteMenu(Entry, Menu.PlayerLoader, Menu);
            }
        }

        /// <summary>
        /// Left Click Search Menu.
        /// </summary>
        /// <param name="x">X position of the mouse</param>
        /// <param name="y">Y position of the mouse</param>
        public void LeftClickSearchMenu(int x, int y)
        {
            if (SearchMenuTab.containsPoint(x, y))
                Game1.activeClickableMenu = new SearchMenu(Entry, Menu);
        }

        /// <summary>
        /// Left Click Okay Button.
        /// </summary>
        /// <param name="x">X position of the mouse</param>
        /// <param name="y">Y position of the mouse</param>
        public void LeftClickOkayButton(int x, int y)
        {
            if (OkButton.containsPoint(x, y))
            {
                Menu.PlayerLoader.SaveCharacterLayout(Menu.BaseIndex, Menu.FaceIndex, Menu.NoseIndex, Menu.ShoeIndex, Menu.DresserIndex, Menu.IsBald);
                Game1.exitActiveMenu();
                Game1.flashAlpha = 1f;
                Game1.playSound("yoba");
            }
        }

        /// <summary>
        /// Left Click Cancel Button.
        /// </summary>
        /// <param name="x">X position of the mouse</param>
        /// <param name="y">Y position of the mouse</param>
        public void LeftClickCancelButton(int x, int y)
        {
            if (CancelButton.containsPoint(x, y))
            {
                Menu.RestoreSnapshot();
                Game1.activeClickableMenu = null;
                MenuComponents.ChangeScaleLeftClick(CancelButton);
                Game1.playSound("bigDeSelect");
            }
        }

        /// <summary>
        /// Left Click Customize Anywhere.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void LeftClickCustomizeAnywhere(int x, int y)
        {
            if (Entry.IsCustomizeAnywhereInstalled)
            {
                if (ClothingTab.containsPoint(x, y))
                    Entry.HarmonyHelper.CustomizeAnywhereClothingMenu();
            }
        }

        /// <summary>
        /// Handles which index to move and by what direction.
        /// </summary>
        /// <param name="name">The name of the button</param>
        /// <param name="direction">Which direction to move the indexes</param>
        private void SelectionClick(string buttonName, int direction)
        {
            switch (buttonName)
            {
                case "Male":
                    Game1.player.changeGender(true);

                    // Reset the BaseIndex, ShoeIndex to prevent crashing
                    Menu.BaseIndex = 0;
                    Menu.ShoeIndex = 0;
                    Menu.PlayerChanger.ChangePlayerBase(Game1.player.isMale, Menu.BaseIndex, Menu.FaceIndex, Menu.NoseIndex, Menu.ShoeIndex, Menu.IsBald);
                    break;
                case "Female":
                    Game1.player.changeGender(false);

                    // Reset the BaseIndex, ShoeIndex to prevent crashing
                    Menu.BaseIndex = 0;
                    Menu.ShoeIndex = 0;
                    Menu.PlayerChanger.ChangePlayerBase(Game1.player.isMale, Menu.BaseIndex, Menu.FaceIndex, Menu.NoseIndex, Menu.ShoeIndex, Menu.IsBald);
                    break;
            }
        }

        /// <summary>
        /// Draw Buttons.
        /// </summary>
        /// <param name="b">The games SpriteBatch</param>
        public void Draw(SpriteBatch b, ClickableComponent hatFixLabel)
        {
            // Draw the tabs
            FavoriteMenuTab.draw(b);
            GlamMenuTab.draw(b);
            SearchMenuTab.draw(b);

            if (Entry.IsCustomizeAnywhereInstalled)
                ClothingTab.draw(b);

            // Draw the add to favorites button and the text
            AddToFavoritesButton.draw(b);
            Utility.drawTextWithShadow(b, "Add Fav:", Game1.smallFont, new Vector2(AddToFavoritesButton.bounds.X - 112, AddToFavoritesButton.bounds.Y + 8), Game1.textColor);

            // Check if the player is wearing a hat
            if (Game1.player.hat.Value != null)
            {
                HatCoversHairButton.visible = true;
                HatCoversHairButton.draw(b);
                Utility.drawTextWithShadow(b, hatFixLabel.name, Game1.smallFont, new Vector2(hatFixLabel.bounds.X, hatFixLabel.bounds.Y), Game1.textColor);

                if (MenuComponents.IsHatFixSelected)
                    HatCoversHairButton.sourceRect.X = 236;
            }

            // Draw the gender buttons
            foreach (ClickableTextureComponent component in GenderButtons)
            {
                component.draw(b);
                if (component.name.Equals("Male") && Game1.player.isMale || (component.name.Equals("Female") && !Game1.player.isMale))
                    b.Draw(Game1.mouseCursors, component.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 34, -1, -1)), Color.White);
            }

            // Draw the ok button and cancel button
            OkButton.draw(b);
            CancelButton.draw(b);
        }
    }
}
