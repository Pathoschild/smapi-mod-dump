/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SoapStuff/Remote-Fridge-Storage
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using RemoteFridgeStorage.API;
using RemoteFridgeStorage.controller;
using RemoteFridgeStorage.controller.saving;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace RemoteFridgeStorage
{
    /// <inheritdoc />
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /// <summary>The mod configuration from the player.</summary>

        public FridgeController FridgeController;

        public ChestController ChestController;

        public SaveManager SaveManager;

        private CompatibilityInfo _compatibilityInfo;

        public static ModEntry Instance { get; private set; }

        /// <inheritdoc />
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;

            var textures = LoadAssets(helper.ModContent);
            _compatibilityInfo = GetCompatibilityInfo(helper);

            ChestController = new ChestController(textures, _compatibilityInfo);
            FridgeController = new FridgeController(ChestController);
            SaveManager = new SaveManager(ChestController);

            Helper.Events.Display.MenuChanged += OnMenuChanged;
            Helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
            Helper.Events.Input.ButtonPressed += OnButtonPressed;
            Helper.Events.GameLoop.SaveLoaded += SaveManager.SaveLoaded;
            Helper.Events.GameLoop.Saving += SaveManager.Saving;
        }

        /// <summary>
        /// Check for loaded mods for compatibility reasons
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        private static CompatibilityInfo GetCompatibilityInfo(IModHelper helper)
        {
            // Compatibility checks
            var cookingSkillLoaded = helper.ModRegistry.IsLoaded("spacechase0.CookingSkill");
            var categorizeChestsLoaded = helper.ModRegistry.IsLoaded("CategorizeChests");
            var convenientChestsLoaded = helper.ModRegistry.IsLoaded("aEnigma.ConvenientChests");
            var megaStorageLoaded = helper.ModRegistry.IsLoaded("Alek.MegaStorage");
            var chestAnywhereLoaded = helper.ModRegistry.IsLoaded("Pathoschild.ChestsAnywhere");

            var compatibilityInfo = new CompatibilityInfo
            {
                CategorizeChestLoaded = categorizeChestsLoaded,
                ConvenientChestLoaded = convenientChestsLoaded,
                CookingSkillLoaded = cookingSkillLoaded,
                MegaStorageLoaded = megaStorageLoaded,
                ChestAnywhereLoaded = chestAnywhereLoaded
            };
            return compatibilityInfo;
        }

        /// <summary>
        /// Load the textures
        /// </summary>
        /// <param name="contentHelper"></param>
        /// <returns></returns>
        private static Textures LoadAssets(IModContentHelper contentHelper)
        {
            // Assets
            var fridgeSelected = contentHelper.Load<Texture2D>("assets/fridge.png");
            var fridgeDeselected = contentHelper.Load<Texture2D>("assets/fridge2.png");

            var textures = new Textures
            {
                FridgeSelected = fridgeSelected,
                FridgeDeselected = fridgeDeselected,
            };
            return textures;
        }

        /// <summary>
        /// Draw the icon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            ChestController.DrawFridgeIcon(e);
        }

        /// <summary>
        /// Add chests to fridge
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            ChestController.UpdateChest();
            if (e.NewMenu == e.OldMenu || e.NewMenu == null)
                return;

            if (e.NewMenu.GetType().ToString() == "LoveOfCooking.Objects.CookingMenu")
            {
                return;
            }
            // If The (Cooking) Crafting page is opened
            if (e.NewMenu is StardewValley.Menus.CraftingPage &&
                Helper.Reflection.GetField<bool>(e.NewMenu, "cooking", false) != null &&
                Helper.Reflection.GetField<bool>(e.NewMenu, "cooking").GetValue())
            {
                FridgeController.InjectItems();
                return;
            }

            // If the Cooking Skill Page is opened.
            if (_compatibilityInfo.CookingSkillLoaded &&
                e.NewMenu.GetType().ToString() == "CookingSkill.NewCraftingPage")
            {
                FridgeController.InjectItems();
                return;
            }
            
            if (Helper.Reflection.GetField<bool>(e.NewMenu, "cooking", false) != null &&
                Helper.Reflection.GetField<bool>(e.NewMenu, "cooking").GetValue())
            {
                Monitor.Log("Menu changed to " + e.NewMenu.GetType() + " which is a unrecognized type. Is it from an incompatible mod?",LogLevel.Warn);    
            }
            
        }

        /// <summary>
        /// Handle clicking of the fridge icons.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (e.Button == SButton.MouseLeft)
            {
                ChestController.HandleClick(e.Cursor);
            }
        }


        /// <summary>
        /// Container for textures
        /// </summary>
        public struct Textures
        {
            public Texture2D FridgeSelected;
            public Texture2D FridgeSelectedAlt;
            public Texture2D FridgeDeselected;
            public Texture2D FridgeDeselectedAlt;
        }

        /// <summary>
        /// Container for modInfo
        /// </summary>
        public struct CompatibilityInfo
        {
            public bool CookingSkillLoaded;
            public bool CategorizeChestLoaded;
            public bool ConvenientChestLoaded;
            public bool MegaStorageLoaded;
            public bool ChestAnywhereLoaded;
        }

        public void Log(string s)
        {
            Monitor.Log(s);
        }
    }
}