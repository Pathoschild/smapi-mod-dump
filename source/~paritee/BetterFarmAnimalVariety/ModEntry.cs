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
using System.Linq;
using System.Reflection;
using static StardewValley.Menus.LoadGameMenu;

namespace BetterFarmAnimalVariety
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        public ModConfig Config;
        public ModCommand Command;

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
            try
            {
                this.Config = this.LoadConfig();
            }
            catch(FormatException)
            {
                this.Monitor.Log($"Your config.json format is invalid and BFAV has shut down. You are running BFAV v{this.ModManifest.Version.ToString()} which requires a config.json format of {this.ModManifest.Version.MajorVersion.ToString()}.", LogLevel.Alert);
                return;
            }

            if (!this.Config.IsEnabled)
            {
                this.Monitor.Log($"BFAV is disabled. To enable, set IsEnabled to true in config.json.", LogLevel.Debug);
                return;
            }

            // Harmony
            this.ApplyHarmonyPatches();

            // Commands
            this.ApplyConsoleCommands();

            // Asset Editors
            this.Helper.Content.AssetEditors.Add(new AnimalBirthEditor(this));

            // Events
            this.Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            this.Helper.Events.Display.RenderingActiveMenu += this.OnRenderingActiveMenu;
            this.Helper.Events.Display.RenderedActiveMenu += this.OnRenderedActiveMenu;
            this.Helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            this.Helper.Events.Display.MenuChanged += this.OnMenuChanged;
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

        private void ApplyConsoleCommands()
        {
            this.Command = new ModCommand(this.Config, this.Helper, this.Monitor);

            this.Command.SetUp();
        }

        public override object GetApi()
        {
            return new ModApi(this.Config, this.ModManifest.Version);
        }

        private ModConfig LoadConfig()
        {
            // Load the config
            ModConfig config = this.Helper.ReadConfig<ModConfig>();

            string targetFormat = this.ModManifest.Version.MajorVersion.ToString();

            // Do this outside of the constructor so that we can use the ModManifest helper
            if (config.Format == null)
            {
                config.Format = targetFormat;

                this.Helper.WriteConfig<ModConfig>(config);
            }
            else if (!config.IsValidFormat(targetFormat))
            {
                throw new FormatException();
            }

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
                Dictionary<string, List<string>> farmAnimals = this.Config.GetFarmAnimalTypes();
                BreedFarmAnimalConfig breedFarmAnimalConfig = new BreedFarmAnimalConfig(farmAnimals, this.BlueFarmAnimals, this.Config.RandomizeNewbornFromCategory, this.Config.RandomizeHatchlingFromCategory, this.Config.IgnoreParentProduceCheck);
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

        private void AttemptToCleanSaves(ButtonPressedEventArgs e)
        {
            // Always attempt to clean up the animal types to prevent on save load crashes
            // if the patch mod had been removed without the animals being sold/deleted
            if (Game1.activeClickableMenu is TitleMenu titleMenu && TitleMenu.subMenu is LoadGameMenu loadGameMenu)
            {
                if (loadGameMenu.slotButtons == null)
                {
                    return;
                }

                List<MenuSlot> menuSlots = this.Helper.Reflection.GetField<List<MenuSlot>>(loadGameMenu, "menuSlots").GetValue();

                if (menuSlots == null || !menuSlots.Any())
                {
                    return;
                }

                int x = (int)e.Cursor.ScreenPixels.X;
                int y = (int)e.Cursor.ScreenPixels.Y;

                int currentItemIndex = this.Helper.Reflection.GetField<int>(loadGameMenu, "currentItemIndex").GetValue();

                for (int index = 0; index < loadGameMenu.slotButtons.Count; index++)
                {
                    if (currentItemIndex + index < menuSlots.Count && loadGameMenu.slotButtons[index].containsPoint(x, y))
                    {
                        SaveFileSlot saveFileSlot = menuSlots[currentItemIndex + index] as SaveFileSlot;

                        this.Helper.ConsoleCommands.Trigger("bfav_fa_fix", new string[] { saveFileSlot.Farmer.slotName });

                        break;
                    }
                }
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            this.AttemptToCleanSaves(e);

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
