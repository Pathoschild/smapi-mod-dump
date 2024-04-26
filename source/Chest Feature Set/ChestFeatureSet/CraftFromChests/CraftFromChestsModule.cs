/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zack20136/ChestFeatureSet
**
*************************************************/

using ChestFeatureSet.Framework;
using ChestFeatureSet.Framework.CFSChest;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace ChestFeatureSet.CraftFromChests
{
    public class CraftFromChestsModule : Module
    {
        public CraftFromChestsModule(ModEntry modEntry) : base(modEntry) { }

        public CFSChestController CFSChestController { get; private set; }

        private bool IsCraftingTab { get; set; }

        public override void Activate()
        {
            this.IsActive = true;

            Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
            textures.Add("SelectedComponent", this.ModEntry.Helper.ModContent.Load<Texture2D>("assets/unableToCraft.png"));
            textures.Add("DeselectedComponent", this.ModEntry.Helper.ModContent.Load<Texture2D>("assets/ableToCraft.png"));

            var sourceRect = new Rectangle(0, 0, textures["SelectedComponent"].Width, textures["SelectedComponent"].Height);

            if (this.Config.StashToChests)
                this.CFSChestController = new CFSChestController(this.ModEntry, "CraftFromChests.json", textures, sourceRect, 2);
            else
                this.CFSChestController = new CFSChestController(this.ModEntry, "CraftFromChests.json", textures, sourceRect, 1);

            this.Events.Input.ButtonPressed += this.OnButtonPressed;
            this.Events.Display.MenuChanged += this.OnMenuChanged;
            this.Events.Display.RenderedActiveMenu += this.OnRenderedActiveMenu;
        }

        public override void Deactivate()
        {
            this.IsActive = false;

            this.CFSChestController.Deactivate();
            this.CFSChestController = null;

            this.Events.Input.ButtonPressed -= this.OnButtonPressed;
            this.Events.Display.MenuChanged -= this.OnMenuChanged;
            this.Events.Display.RenderedActiveMenu -= this.OnRenderedActiveMenu;
        }

        /// <summary>
        /// Open the crafting page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == this.Config.OpenCraftingPageKey)
            {
                try
                {
                    var width = 800 + (IClickableMenu.borderWidth * 2);
                    var height = 600 + (IClickableMenu.borderWidth * 2);
                    var (x, y) = Utility.getTopLeftPositionForCenteringOnScreen(width, height).ToPoint();
                    Game1.activeClickableMenu = new CraftingPage(x, y, width, height, false, true, new List<StardewValley.Inventories.IInventory>());
                }
                catch (Exception error)
                {
                    this.Monitor.Log(error.ToString(), LogLevel.Error);
                }
            }
        }

        /// <summary>
        /// Add chests to craftingPage (Crafting, Cooking)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!Context.IsWorldReady || e.NewMenu == e.OldMenu || e.NewMenu == null)
                return;

            this.CFSChestController.UpdateOpenedChest();

            // If The Crafting (Cooking) page is opened
            if (e.NewMenu is CraftingPage)
            {
                this.InjectItems();
                return;
            }
        }

        /// <summary>
        /// Add chests to GameMenu craftingPage.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            if (Game1.activeClickableMenu is StardewValley.Menus.GameMenu menu && menu.GetCurrentPage() is CraftingPage)
            {
                if (!this.IsCraftingTab)
                    this.InjectItems();

                this.IsCraftingTab = true;
            }
            else
                this.IsCraftingTab = false;
        }

        /// <summary>
        /// Inject the items on the page.
        /// </summary>
        private void InjectItems()
        {
            var page = Game1.activeClickableMenu;

            if (page == null)
                return;

            CraftingPage? nowPage = null;
            if (page is CraftingPage craftingPage)
            {
                nowPage = craftingPage;
            }
            else if (page is GameMenu gameMenu && gameMenu.GetCurrentPage() is CraftingPage craftingTab)
            {
                nowPage = craftingTab;
            }

            if (nowPage != null)
            {
                IEnumerable<Chest>? chests = null;
                if (this.Config.CraftLocationSetting is "Anywhere")
                    chests = ChestExtension.GetAllChests().Select(chestPair => chestPair.Chest).Where(chest => !this.CFSChestController.GetChests().Contains(chest));
                else if (this.Config.CraftLocationSetting is "FarmArea" && LocationExtension.GetFarmAndCustomArea().Contains(Game1.player.currentLocation.Name))
                    chests = ChestExtension.GetAreaChests(LocationExtension.GetFarmAndCustomArea()).Select(chestPair => chestPair.Chest).Where(chest => !this.CFSChestController.GetChests().Contains(chest));
                else
                    chests = ChestExtension.GetNearbyChests(Game1.player, this.Config.CraftRadius).Where(chest => !this.CFSChestController.GetChests().Contains(chest));

                if (!chests.Any())
                    return;

                var nearbyInventory = chests.Select((chest, i) => chest.Items).ToList();

                if (nowPage._materialContainers == null)
                    nowPage._materialContainers = new List<StardewValley.Inventories.IInventory>();

                nowPage._materialContainers.AddRange(nearbyInventory);

                return;
            }

            this.Monitor.Log($"Failed to inject items into: {page.GetType()}.", LogLevel.Warn);
        }
    }
}
