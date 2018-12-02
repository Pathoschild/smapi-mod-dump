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
        private HarmonyInstance _harmony;
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

            MenuEvents.MenuChanged += MenuChanged_Event;
            InputEvents.ButtonPressed += Button_Pressed_Event;
            GameEvents.FirstUpdateTick += Game_FirstTick;
            GraphicsEvents.OnPostRenderGuiEvent += Draw;

            SaveEvents.AfterLoad += AfterLoad;
            SaveEvents.BeforeSave += BeforeSave;
            SaveEvents.AfterSave += AfterSave;
            GameEvents.UpdateTick += Game_Update;
        }

        private void Game_FirstTick(object sender, EventArgs e)
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
            if (cookingSkillLoaded) return;
            _harmony = HarmonyInstance.Create("productions.EternalSoap.RemoteFridgeStorage");
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        private void Game_Update(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady) return;

            _handler.Game_Update();
        }

        private void AfterSave(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady) return;
            _handler.AfterSave();
        }

        private void BeforeSave(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady) return;
            _handler.BeforeSave();
        }

        private void AfterLoad(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady) return;
            _handler.AfterLoad();
        }


        private void Draw(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady) return;
            _handler.DrawFridge();
        }

        private void Button_Pressed_Event(object sender, EventArgsInput e)
        {
            if (!Context.IsWorldReady) return;

            if (e.Button == SButton.MouseLeft)
            {
                _handler.HandleClick(e);
            }
        }

        /// <summary>
        /// If the opened menu was a crafting menu, call the handler to load the menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuChanged_Event(object sender, EventArgsClickableMenuChanged e)
        {
            if (!Context.IsWorldReady) return;
            //Replace menu if the new menu has the attribute cooking set to true and the new menu is not my crafting page.
            if (e.NewMenu != null &&
                Helper.Reflection.GetField<bool>(e.NewMenu, "cooking", false) != null &&
                Helper.Reflection.GetField<bool>(e.NewMenu, "cooking").GetValue() &&
                !(e.NewMenu is RemoteFridgeCraftingPage))
            {
                _handler.LoadMenu(e);
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