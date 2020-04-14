using MegaStorage.Framework;
using MegaStorage.Framework.Models;
using MegaStorage.Framework.Persistence;
using MegaStorage.Framework.UI;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.IO;

namespace MegaStorage
{
    public class MegaStorageMod : Mod
    {
        internal static MegaStorageMod Instance { get; private set; }
        internal static IMegaStorageApi API;
        internal static IModHelper ModHelper;
        internal static IMonitor ModMonitor;
        internal static IJsonAssetsApi JsonAssets;
        internal static IConvenientChestsApi ConvenientChests;
        internal static int LargeChestId { get; private set; } = -1;
        internal static int MagicChestId { get; private set; } = -1;
        internal static int SuperMagicChestId { get; private set; } = -1;
        internal static CustomItemGrabMenu ActiveItemGrabMenu { get; private set; }

        /*********
        ** Public methods
        *********/
        public override void Entry(IModHelper modHelper)
        {
            // Make Instance, ModHelper, and ModMonitor static for use in other classes
            Instance = this;
            ModHelper = modHelper ?? throw new ArgumentNullException(nameof(modHelper));
            ModMonitor = Monitor;
            API = new MegaStorageApi();

            ModMonitor.VerboseLog("Entry of MegaStorageMod");

            ModHelper.ReadConfig<ModConfig>();

            ModHelper.Events.GameLoop.GameLaunched += OnGameLaunched;
            ModHelper.Events.Display.MenuChanged += OnMenuChanged;
            ModHelper.Events.Display.WindowResized += OnWindowResized;
        }

        public override object GetApi() => API ??= new MegaStorageApi();

        /*********
        ** Private methods
        *********/
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            JsonAssets = ModHelper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
            if (JsonAssets is null)
            {
                Monitor.Log("JsonAssets is needed to load Mega Storage chests", LogLevel.Error);
                return;
            }

            ConvenientChests = ModHelper.ModRegistry.GetApi<IConvenientChestsApi>("aEnigma.ConvenientChests");
            if (!(ConvenientChests is null))
            {
                ModConfig.Instance.LargeChest.EnableCategories = false;
                ModConfig.Instance.MagicChest.EnableCategories = false;
                ModConfig.Instance.SuperMagicChest.EnableChest = false;
            }

            if (ModConfig.Instance.LargeChest.EnableChest)
                JsonAssets.LoadAssets(Path.Combine(ModHelper.DirectoryPath, "assets", "LargeChest"));
            if (ModConfig.Instance.MagicChest.EnableChest)
                JsonAssets.LoadAssets(Path.Combine(ModHelper.DirectoryPath, "assets", "MagicChest"));
            if (ModConfig.Instance.SuperMagicChest.EnableChest)
                JsonAssets.LoadAssets(Path.Combine(ModHelper.DirectoryPath, "assets", "SuperMagicChest"));

            JsonAssets.IdsAssigned += OnIdsAssigned;
            ItemPatcher.Start();
            SaveManager.Start();
            StateManager.Start();
        }

        private static void OnIdsAssigned(object sender, EventArgs e)
        {
            if (LargeChestId == -1 && ModConfig.Instance.LargeChest.EnableChest)
                LargeChestId = JsonAssets.GetBigCraftableId("Large Chest");
            if (MagicChestId == -1 && ModConfig.Instance.MagicChest.EnableChest)
                MagicChestId = JsonAssets.GetBigCraftableId("Magic Chest");
            if (SuperMagicChestId == -1 && ModConfig.Instance.SuperMagicChest.EnableChest)
                SuperMagicChestId = JsonAssets.GetBigCraftableId("Super Magic Chest");

            if ((LargeChestId <= -1 && ModConfig.Instance.LargeChest.EnableChest) ||
                (MagicChestId <= -1 && ModConfig.Instance.MagicChest.EnableChest) ||
                (SuperMagicChestId <= -1 && ModConfig.Instance.SuperMagicChest.EnableChest))
            {
                return;
            }

            ModMonitor.Log($"Large Chest Loaded with ID {LargeChestId}.");
            ModMonitor.Log($"Magic Chest Loaded with ID {MagicChestId}.");
            ModMonitor.Log($"Super Magic Chest Loaded with ID {SuperMagicChestId}.");
        }

        private static void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            ModMonitor.VerboseLog("New menu: " + e.NewMenu?.GetType());
            if (e.NewMenu is CustomItemGrabMenu customItemGrabMenu)
            {
                ActiveItemGrabMenu = customItemGrabMenu;
                return;
            }

            if (!(e.NewMenu is ItemGrabMenu itemGrabMenu) || !(itemGrabMenu.context is CustomChest customChest))
                return;

            ActiveItemGrabMenu = customChest.CreateItemGrabMenu();
            Game1.activeClickableMenu = ActiveItemGrabMenu;
        }

        private static void OnWindowResized(object sender, WindowResizedEventArgs e)
        {
            if (!(Game1.activeClickableMenu is CustomItemGrabMenu customItemGrabMenu))
                return;
            var oldBounds = new Rectangle(0, 0, e.OldSize.X, e.OldSize.Y);
            var newBounds = new Rectangle(0, 0, e.NewSize.X, e.NewSize.Y);
            customItemGrabMenu.gameWindowSizeChanged(oldBounds, newBounds);
        }
    }
}