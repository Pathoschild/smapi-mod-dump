using BetterFarmAnimalVariety.Editors;
using BetterFarmAnimalVariety.Models;
using BetterFarmAnimalVariety.Patches;
using Harmony;
using Microsoft.Xna.Framework.Graphics;
using Paritee.StardewValleyAPI.Buildings.AnimalShop;
using Paritee.StardewValleyAPI.Buildings.AnimalShop.FarmAnimals;
using Paritee.StardewValleyAPI.FarmAnimals.Variations;
using Paritee.StardewValleyAPI.Menus;
using Paritee.StardewValleyAPI.Players;
using Paritee.StardewValleyAPI.Players.Actions;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace BetterFarmAnimalVariety
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        public ModConfig Config;

        public Player Player;
        public BlueVariation BlueFarmAnimals;
        public VoidVariation VoidFarmAnimals;
        public AnimalShop AnimalShop;

        private bool ChangedPurchaseAnimalsMenuClickableComponents = false;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Config
            this.Config = this.LoadConfig();

            // Asset Editors
            this.Helper.Content.AssetEditors.Add(new AnimalBirthEditor(this));

            // Events
            this.Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            this.Helper.Events.Display.RenderingActiveMenu += this.OnRenderingActiveMenu;
            this.Helper.Events.Display.RenderedActiveMenu += this.OnRenderedActiveMenu;
            this.Helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            this.Helper.Events.Display.MenuChanged += this.OnMenuChanged;

            this.ApplyHarmonyPatches();
        }

        private void ApplyHarmonyPatches()
        {
            // Harmony
            var harmony = HarmonyInstance.Create("paritee.betterfarmanimalvariety");

            MethodInfo targetMethod;
            HarmonyMethod prefixMethod;

            // Remove the hardcoded check on the Vanilla stock categories
            targetMethod = AccessTools.Method(typeof(PurchaseAnimalsMenu), "getAnimalTitle");
            prefixMethod = new HarmonyMethod(typeof(PurchaseAnimalsMenuPatch).GetMethod("getAnimalTitlePrefix"));
            harmony.Patch(targetMethod, prefixMethod, null);

            // Remove the hardcoded check on the Vanilla stock categories
            targetMethod = AccessTools.Method(typeof(PurchaseAnimalsMenu), "getAnimalDescription");
            prefixMethod = new HarmonyMethod(typeof(PurchaseAnimalsMenuPatch).GetMethod("getAnimalDescriptionPrefix"));
            harmony.Patch(targetMethod, prefixMethod, null);
        }

        public override object GetApi()
        {
            return new ModApi(this.Config);
        }

        private ModConfig LoadConfig()
        {
            // Load up the config
            ModConfig config = this.Helper.ReadConfig<ModConfig>();

            config.InitializeFarmAnimals();

            return config;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            this.Player = new Player(Game1.player, this.Helper);

            // Set up everything else
            BlueConfig blueConfig = new BlueConfig(this.Player.HasSeenEvent(BlueVariation.EVENT_ID));
            this.BlueFarmAnimals = new BlueVariation(blueConfig);

            VoidConfig voidConfig = new VoidConfig(this.Config.VoidFarmAnimalsInShop, this.Player.HasCompletedQuest(VoidVariation.QUEST_ID));
            this.VoidFarmAnimals = new VoidVariation(voidConfig);

            List<FarmAnimalForPurchase> farmAnimalsForPurchase = this.Config.GetFarmAnimalsForPurchase(Game1.getFarm());
            StockConfig stockConfig = new StockConfig(farmAnimalsForPurchase, this.BlueFarmAnimals, this.VoidFarmAnimals);
            Stock stock = new Stock(stockConfig);

            this.AnimalShop = new AnimalShop(stock);
        }

        private void OnRenderingActiveMenu(object sender, RenderingActiveMenuEventArgs e)
        {
            // Ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady || Game1.activeClickableMenu == null)
            {
                return;
            }

            if (!(Game1.activeClickableMenu is StardewValley.Menus.NamingMenu))
            {
                return;
            }

            StardewValley.Menus.NamingMenu namingMenu = Game1.activeClickableMenu as StardewValley.Menus.NamingMenu;

            if (namingMenu.GetType() == typeof(StardewValley.Menus.NamingMenu))
            {
                List<string> loadedTypes = this.Config.GetFarmAnimalTypes();
                BreedFarmAnimalConfig breedFarmAnimalConfig = new BreedFarmAnimalConfig(loadedTypes, this.BlueFarmAnimals);
                BreedFarmAnimal breedFarmAnimal = new BreedFarmAnimal(this.Player, breedFarmAnimalConfig);

                NameFarmAnimalMenu nameFarmAnimalMenu = new NameFarmAnimalMenu(namingMenu, breedFarmAnimal);

                nameFarmAnimalMenu.HandleChange();
            }
        }

        private void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            // Ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady || Game1.activeClickableMenu == null)
            {
                return;
            }

            // Stop triggering the heavy redraws
            if (this.ChangedPurchaseAnimalsMenuClickableComponents)
            {
                return;
            }

            if (Game1.activeClickableMenu.GetType() == typeof(StardewValley.Menus.PurchaseAnimalsMenu))
            {
                if (!(Game1.activeClickableMenu is StardewValley.Menus.PurchaseAnimalsMenu))
                {
                    return;
                }

                StardewValley.Menus.PurchaseAnimalsMenu purchaseAnimalsMenu = Game1.activeClickableMenu as StardewValley.Menus.PurchaseAnimalsMenu;

                // We need to completely redo the animalsToPurchase to account for the custom sprites
                Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
                int iconHeight = 0;

                foreach (KeyValuePair<string, ConfigFarmAnimal> entry in this.Config.FarmAnimals)
                {
                    if (entry.Value.CanBePurchased())
                    {
                        Texture2D texture = this.Helper.Content.Load<Texture2D>(entry.Value.AnimalShop.Icon, ContentSource.ModFolder);

                        iconHeight = texture.Height;

                        textures.Add(entry.Value.Category, texture);
                    }
                }

                purchaseAnimalsMenu.animalsToPurchase = this.AnimalShop.FarmAnimalStock.DetermineClickableComponents(purchaseAnimalsMenu, textures);

                int rows = (int)Math.Ceiling((float)purchaseAnimalsMenu.animalsToPurchase.Count / 3); // Always at least one row

                // Adjust the size of the menud if there are more or less rows than it normally handles
                if (iconHeight > 0)
                {
                    purchaseAnimalsMenu.height = (int)(iconHeight * 2f) + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth / 2 + rows * 85;
                }

                this.ChangedPurchaseAnimalsMenuClickableComponents = true;

                return;
            }
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            this.ChangedPurchaseAnimalsMenuClickableComponents = false;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // Ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
            {
                return;
            }

            // We only care about left mouse clicks right now
            if (e.Button != SButton.MouseLeft)
            {
                return;
            }

            ActiveClickableMenu activeClickableMenu = new ActiveClickableMenu(Game1.activeClickableMenu);

            if (!activeClickableMenu.IsOpen())
            {
                return;
            }

            if (!(activeClickableMenu.GetMenu() is StardewValley.Menus.PurchaseAnimalsMenu))
            {
                return;
            }

            // Purchasing a new animal
            StardewValley.Menus.PurchaseAnimalsMenu purchaseAnimalsMenu = activeClickableMenu.GetMenu() as StardewValley.Menus.PurchaseAnimalsMenu;

            PurchaseFarmAnimal purchaseFarmAnimal = new PurchaseFarmAnimal(this.Player, this.AnimalShop);
            PurchaseFarmAnimalMenu purchaseFarmAnimalMenu = new PurchaseFarmAnimalMenu(purchaseAnimalsMenu, purchaseFarmAnimal);

            purchaseFarmAnimalMenu.HandleTap(e);
        }
    }
}
