using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OmegasisCore.Menus;
using OmegasisCore.Menus.MenuComponentsAndResources;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegasisCore.Resources
{
    public class Lists
    {
        public static List<GameMenuComponentPair> ModdedGameMenuTabsAndPages;
        public static List<GameMenuComponentPair> VanillaGameMenuTabs;

        public static void initalizeLists()
        {
            ModdedGameMenuTabsAndPages = new List<GameMenuComponentPair>();
            VanillaGameMenuTabs = new List<GameMenuComponentPair>();
            initalizeVanillaGameMenuTabs();
        }

        public static void initalizeVanillaGameMenuTabs()
        {
            int xPositionOnScreen = Game1.viewport.Width / 2 - (800 + IClickableMenu.borderWidth * 2) / 2;
            int yPositionOnScreen = Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2;
            int width = 800 + IClickableMenu.borderWidth * 2;
            int height = 600 + IClickableMenu.borderWidth * 2;

            VanillaGameMenuTabs.Add(
                new Menus.MenuComponentsAndResources.GameMenuComponentPair(
                new ClickableTextureComponentExtended("inventory", new Rectangle(xPositionOnScreen + Game1.tileSize, yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + Game1.tileSize, Game1.tileSize, Game1.tileSize), "Inventory", "Inventory", ModCore.HELPER.Content.Load<Texture2D>(Path.Combine(ModCore.ModAssetFolder, "Menus", "GameMenu", "TabIcons", "InventoryTabIcon.xnb")), new Rectangle(0 * Game1.tileSize, 0 * Game1.tileSize, Game1.tileSize, Game1.tileSize), 1f, 0, false),
                new InventoryPage(xPositionOnScreen, yPositionOnScreen, width, height)
                ));

            VanillaGameMenuTabs.Add(
                new GameMenuComponentPair(
                new ClickableTextureComponentExtended("Skills", new Rectangle(xPositionOnScreen + Game1.tileSize * 2, yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + Game1.tileSize, Game1.tileSize, Game1.tileSize), "Skills", "Skills", ModCore.HELPER.Content.Load<Texture2D>(Path.Combine(ModCore.ModAssetFolder,"Menus", "GameMenu", "TabIcons", "BlankTabIcon.xnb")), new Rectangle(0 * Game1.tileSize, 0, Game1.tileSize, Game1.tileSize), 1f, 1, false),
                new SkillsPage(xPositionOnScreen, yPositionOnScreen, width, height)
                ));

            //For some reason I have to add in the Social Page after the game has been compiled. Probably something to do with retreiving the farmer and their list of friends.
            VanillaGameMenuTabs.Add(
                new GameMenuComponentPair(
                new ClickableTextureComponentExtended("Social", new Rectangle(xPositionOnScreen + Game1.tileSize * 3, yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + Game1.tileSize, Game1.tileSize, Game1.tileSize), "Social", "Social", ModCore.HELPER.Content.Load<Texture2D>(Path.Combine(ModCore.ModAssetFolder, "Menus", "GameMenu", "TabIcons", "SocialTabIcon")), new Rectangle(0 * Game1.tileSize, 0, Game1.tileSize, Game1.tileSize), 1f, 2, false),
                new SocialPage(xPositionOnScreen, yPositionOnScreen, width, height)
                ));
            
            VanillaGameMenuTabs.Add(
                new GameMenuComponentPair(
                new ClickableTextureComponentExtended("Map", new Rectangle(xPositionOnScreen + Game1.tileSize * 4, yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + Game1.tileSize, Game1.tileSize, Game1.tileSize), "Map", "Map", ModCore.HELPER.Content.Load<Texture2D>(Path.Combine(ModCore.ModAssetFolder, "Menus", "GameMenu", "TabIcons", "MapTabIcon")), new Rectangle(0 * Game1.tileSize, 0, Game1.tileSize, Game1.tileSize), 1f, 3, false),
                //This should be new MapPage(xPositionOnScreen, yPositionOnScreen, width, height), but it throws an error upon Mod.Entry.
                new ModdedMapPage(xPositionOnScreen, yPositionOnScreen, width, height)
                ));
            
            VanillaGameMenuTabs.Add(
                new GameMenuComponentPair(
                new ClickableTextureComponentExtended("Crafting", new Rectangle(xPositionOnScreen + Game1.tileSize * 5, yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + Game1.tileSize, Game1.tileSize, Game1.tileSize), "Crafting", "Crafting", ModCore.HELPER.Content.Load<Texture2D>(Path.Combine(ModCore.ModAssetFolder, "Menus", "GameMenu", "TabIcons", "CraftingTabIcon")), new Rectangle(0 * Game1.tileSize, 0, Game1.tileSize, Game1.tileSize), 1f, 4, false),
                new CraftingPage(xPositionOnScreen, yPositionOnScreen, width, height, false)
                ));

            VanillaGameMenuTabs.Add(
                new GameMenuComponentPair(
                new ClickableTextureComponentExtended("Collections", new Rectangle(xPositionOnScreen + Game1.tileSize * 6, yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + Game1.tileSize, Game1.tileSize, Game1.tileSize), "Collections", "Collections", ModCore.HELPER.Content.Load<Texture2D>(Path.Combine(ModCore.ModAssetFolder, "Menus", "GameMenu", "TabIcons", "CollectionsTabIcon")), new Rectangle(0 * Game1.tileSize, 0, Game1.tileSize, Game1.tileSize), 1f, 5, false),
                new CollectionsPage(xPositionOnScreen, yPositionOnScreen, width - Game1.tileSize - Game1.tileSize / 4, height)
                ));

            VanillaGameMenuTabs.Add(
                new GameMenuComponentPair(
                new ClickableTextureComponentExtended("Options", new Rectangle(xPositionOnScreen + Game1.tileSize * 7, yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + Game1.tileSize, Game1.tileSize, Game1.tileSize), "Options", "Options", ModCore.HELPER.Content.Load<Texture2D>(Path.Combine(ModCore.ModAssetFolder, "Menus", "GameMenu", "TabIcons", "OptionsTabIcon")), new Rectangle(0 * Game1.tileSize, 0, Game1.tileSize, Game1.tileSize), 1f, 6, false),
                new OptionsPage(xPositionOnScreen, yPositionOnScreen, width - Game1.tileSize - Game1.tileSize / 4, height)
                ));

            VanillaGameMenuTabs.Add(
                new GameMenuComponentPair(
                new ClickableTextureComponentExtended("Exit", new Rectangle(xPositionOnScreen + Game1.tileSize * 8, yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + Game1.tileSize, Game1.tileSize, Game1.tileSize), "Exit", "Exit", ModCore.HELPER.Content.Load<Texture2D>(Path.Combine(ModCore.ModAssetFolder, "Menus", "GameMenu", "TabIcons", "QuitTabIcon")), new Rectangle(0 * Game1.tileSize, 0, Game1.tileSize, Game1.tileSize), 1f, 7, false),
                new ExitPage(xPositionOnScreen, yPositionOnScreen, width - Game1.tileSize - Game1.tileSize / 4, height)
                ));
        }
    }
}
