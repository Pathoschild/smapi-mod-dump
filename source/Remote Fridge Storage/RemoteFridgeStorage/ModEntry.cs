using System;
using System.Collections.Generic;
using Harmony;
using Microsoft.Xna.Framework.Graphics;
using RemoteFridgeStorage.API;
using RemoteFridgeStorage.CraftingPage;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace RemoteFridgeStorage
{
    /// <inheritdoc />
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private bool _cookingSkillLoaded;
        private FridgeHandler _handler;

        /// <summary>The mod configuration from the player.</summary>
        public Config Config;

        public static ModEntry Instance { get; private set; }

        /// <inheritdoc />
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Config = helper.ReadConfig<Config>();
            // Assets
            var fridgeSelected =
                helper.Content.Load<Texture2D>(Config.FlipImage ? "assets/fridge-flipped.png" : "assets/fridge.png");
            var fridgeDeselected =
                helper.Content.Load<Texture2D>(Config.FlipImage ? "assets/fridge2-flipped.png" : "assets/fridge2.png");
            // Compatibility checks
            _cookingSkillLoaded = helper.ModRegistry.IsLoaded("spacechase0.CookingSkill");
            var categorizeChestsLoaded = helper.ModRegistry.IsLoaded("CategorizeChests");
            var convenientChestsLoaded = helper.ModRegistry.IsLoaded("aEnigma.ConvenientChests");
            var megaStorageLoaded = helper.ModRegistry.IsLoaded("Alek.MegaStorage");

            if (categorizeChestsLoaded) Monitor.Log("Categorize chests detected, moving icon location.", LogLevel.Info);
            if (convenientChestsLoaded) Monitor.Log("Convenient chests detected, moving icon location.", LogLevel.Info);
            if (megaStorageLoaded) Monitor.Log("Mega Storage detected, moving icon location.", LogLevel.Info);

            var offsetIcon = categorizeChestsLoaded || convenientChestsLoaded || megaStorageLoaded;

            _handler = new FridgeHandler(fridgeSelected, fridgeDeselected, offsetIcon, Config);
            AddEvents(helper);
        }

        private void AddEvents(IModHelper helper)
        {
            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Input.ButtonPressed += OnButtonPressed;

            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.GameLoop.Saved += OnSaved;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        /// <summary>
        /// Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            if (!_cookingSkillLoaded) return;
            var cookingSkillApi = Helper.ModRegistry.GetApi<ICookingSkillApi>("spacechase0.CookingSkill");

            if (cookingSkillApi == null)
            {
                Monitor.Log("Could not load CookingSkill API, mods might not work correctly.", LogLevel.Warn);
            }
            else
            {
                cookingSkillApi.setFridgeFunction(Fridge);
                _handler.CookingSkillApi = cookingSkillApi;
                Monitor.Log("Successfully hooked into the cooking skill API!", LogLevel.Info);
            }
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            _handler.Game_Update();
        }

        /// <summary>Raised after the game finishes writing data to the save file (except the initial save creation).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaved(object sender, SavedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            _handler.AfterSave();
        }

        /// <summary>Raised before the game begins writes data to the save file (except the initial save creation).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaving(object sender, SavingEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            _handler.BeforeSave();
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            _handler.AfterLoad();
        }


        /// <summary>When a menu is open (<see cref="Game1.activeClickableMenu"/> isn't null), raised after that menu is drawn to the sprite batch but before it's rendered to the screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            _handler.DrawFridge();
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (e.Button == SButton.MouseLeft)
            {
                _handler.HandleClick(e.Cursor);
            }
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            // If the opened menu was a crafting menu, call the handler to load the menu.
            //Replace menu if the new menu has the attribute cooking set to true and the new menu is not my crafting page.
            if (e.NewMenu != null &&
                Helper.Reflection.GetField<bool>(e.NewMenu, "cooking", false) != null &&
                Helper.Reflection.GetField<bool>(e.NewMenu, "cooking").GetValue() &&
                !(e.NewMenu is RemoteFridgeCraftingPage) && 
                (e.NewMenu is StardewValley.Menus.CraftingPage page)
                )
            {
                _handler.LoadMenu(page);
            }
        }

        public override object GetApi()
        {
            return new RemoteFridgeApi(_handler);
        }

        public IList<Item> Fridge()
        {
            return _handler.FridgeList;
        }
    }
}