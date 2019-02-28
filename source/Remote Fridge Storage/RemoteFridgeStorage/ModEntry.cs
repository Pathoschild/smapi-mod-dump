using System;
using System.Collections.Generic;
using System.Reflection;
using Harmony;
using Microsoft.Xna.Framework.Graphics;
using RemoteFridgeStorage.apis;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace RemoteFridgeStorage
{
    /// <summary>The mod entry point.</summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ModEntry : Mod
    {
        private FridgeHandler _handler;
        public static ModEntry Instance;
        private bool cookingSkillLoaded;
        public ICookingSkillApi CookinSkillApi { get; private set; }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Harmony();

            var fridgeSelected = helper.Content.Load<Texture2D>("assets/fridge.png");
            var fridgeDeselected = helper.Content.Load<Texture2D>("assets/fridge2.png");

            var categorizeChestsLoaded = helper.ModRegistry.IsLoaded("CategorizeChests") ||
                                         helper.ModRegistry.IsLoaded("aEnigma.ConvenientChests");
            cookingSkillLoaded = helper.ModRegistry.IsLoaded("spacechase0.CookingSkill");
            if (cookingSkillLoaded) Monitor.Log("Cooking skill is loaded on game start try to hook into the api");
            _handler = new FridgeHandler(fridgeSelected, fridgeDeselected, categorizeChestsLoaded, cookingSkillLoaded);

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
            if (cookingSkillLoaded)
            {
                CookinSkillApi = Helper.ModRegistry.GetApi<ICookingSkillApi>("spacechase0.CookingSkill");

                if (CookinSkillApi == null)
                {
                    Monitor.Log(
                        "Could not load Cookingskill API, mods might not work correctly, are you using the patched version of cooking skills https://github.com/SoapStuff/CookingSkill/releases?",
                        LogLevel.Warn);
                }
                else
                {
                    CookinSkillApi.setFridgeFunction(Fridge);
                    Monitor.Log("Succesfully hooked into the cooking skill API!", LogLevel.Info);
                }
            }
        }

        private void Harmony()
        {
            if (cookingSkillLoaded)
                return;

            var harmony = HarmonyInstance.Create("productions.EternalSoap.RemoteFridgeStorage");

            harmony.Patch(
                original: AccessTools.Method(typeof(CraftingRecipe), nameof(CraftingRecipe.consumeIngredients)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(HarmonyRecipePatchConsumeIngredients), nameof(HarmonyRecipePatchConsumeIngredients.Prefix)))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(CraftingRecipe), nameof(CraftingRecipe.drawRecipeDescription)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(HarmonyRecipePatchDraw), nameof(HarmonyRecipePatchDraw.Prefix)))
            );
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
                !(e.NewMenu is RemoteFridgeCraftingPage))
            {
                _handler.LoadMenu(e.NewMenu);
            }
        }

        /// <summary>
        /// Return the list used for the fridge items.
        /// </summary>
        /// <returns></returns>
        protected virtual IList<Item> FridgeImpl()
        {
            return _handler.FridgeList;
        }

        /// <summary>
        /// Calls the FridgeImpl method on the ModEntry instance.
        /// </summary>
        /// <returns></returns>
        public static IList<Item> Fridge()
        {
            return Instance.FridgeImpl();
        }
    }
}