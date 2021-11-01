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
        public Config Config;

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
            Config = helper.ReadConfig<Config>();

            var textures = LoadAssets(helper);
            _compatibilityInfo = GetCompatibilityInfo(helper);

            ChestController = new ChestController(textures, _compatibilityInfo, Config);
            FridgeController = new FridgeController(ChestController);
            SaveManager = new SaveManager(ChestController,Config);

            Helper.Events.Display.MenuChanged += OnMenuChanged;
            Helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
            Helper.Events.Input.ButtonPressed += OnButtonPressed;
            Helper.Events.GameLoop.SaveLoaded += SaveManager.SaveLoaded;
            Helper.Events.GameLoop.Saving += SaveManager.Saving;
            Helper.Events.GameLoop.GameLaunched += OnLaunch;
            Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        /// <summary>
        /// Handle the mod menu api
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLaunch(object sender, GameLaunchedEventArgs e)
        {
            var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (api == null) return;

            Monitor.Log("Loaded Generic Mod Config Menu API", LogLevel.Info);

            api.RegisterModConfig(ModManifest,
                () => Config = new Config(),
                () => Helper.WriteConfig(Config));
            api.RegisterSimpleOption(ModManifest, "Flip Image", "Mirrors the image vertically", () => Config.FlipImage,
                (bool val) => Config.FlipImage = val);
            api.RegisterClampedOption(ModManifest, "Image Scale", "Scale of the image", () => (float) Config.ImageScale,
                (float val) => Config.ImageScale = val, 0.1f, 5.0f);
            api.RegisterSimpleOption(ModManifest, "Manual Placement",
                "Will use the positions defined below for placement instead of the default one",
                () => Config.OverrideOffset, OverrideSet);
            api.RegisterSimpleOption(ModManifest, "X Position", "The x position of the icon", () => Config.XOffset,
                (int val) => Config.XOffset = val);
            api.RegisterSimpleOption(ModManifest, "Y Position", "The y position of the icon", () => Config.YOffset,
                (int val) => Config.YOffset = val);
            api.RegisterSimpleOption(ModManifest, "Draggable",
                "Enable moving of the icon with the arrows", () => Config.Editable,
                EditableSet);
        }

        private void OverrideSet(bool val)
        {
            Config.OverrideOffset = val;
        }

        private void EditableSet(bool val)
        {
            Config.Editable = val;
        }

        /// <summary>
        /// Check for loaded mods for compatibility reasons
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        private static CompatibilityInfo GetCompatibilityInfo(IModHelper helper)
        {
            // Compatibility checks
            bool cookingSkillLoaded = helper.ModRegistry.IsLoaded("spacechase0.CookingSkill");
            bool categorizeChestsLoaded = helper.ModRegistry.IsLoaded("CategorizeChests");
            bool convenientChestsLoaded = helper.ModRegistry.IsLoaded("aEnigma.ConvenientChests");
            bool megaStorageLoaded = helper.ModRegistry.IsLoaded("Alek.MegaStorage");
            bool chestAnywhereLoaded = helper.ModRegistry.IsLoaded("Pathoschild.ChestsAnywhere");

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
        /// <param name="helper"></param>
        /// <returns></returns>
        private static Textures LoadAssets(IModHelper helper)
        {
            // Assets
            var fridgeSelected = helper.Content.Load<Texture2D>("assets/fridge.png");
            var fridgeSelectedAlt = helper.Content.Load<Texture2D>("assets/fridge-flipped.png");
            var fridgeDeselected = helper.Content.Load<Texture2D>("assets/fridge2.png");
            var fridgeDeselectedAlt = helper.Content.Load<Texture2D>("assets/fridge2-flipped.png");

            var textures = new Textures
            {
                FridgeSelected = fridgeSelected,
                FridgeDeselected = fridgeDeselected,
                FridgeSelectedAlt = fridgeSelectedAlt,
                FridgeDeselectedAlt = fridgeDeselectedAlt
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

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            ChestController.UpdateOffset();
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