/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlyingTNT/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using ResourceStorage.BetterCrafting;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Inventories;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using Object = StardewValley.Object;

namespace ResourceStorage
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry : Mod
    {

        public static IMonitor SMonitor;
        public static IModHelper SHelper;
        public static ModConfig Config;

        public static ModEntry context;
        public static string dictKey = "aedenthorn.ResourceStorage/dictionary"; // Not updating to FlyingTNT.ResourceStorage for backwards compatibility
        public static Dictionary<long, Dictionary<string, long>> resourceDict = new();

        public static PerScreen<GameMenu> gameMenu = new PerScreen<GameMenu>();
        public static PerScreen<ClickableTextureComponent> resourceButton = new PerScreen<ClickableTextureComponent>();
        private Harmony harmony;


        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            context = this;

            SMonitor = Monitor;
            SHelper = helper;

            Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            Helper.Events.GameLoop.ReturnedToTitle += GameLoop_ReturnedToTitle;
            Helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            Helper.Events.GameLoop.Saving += GameLoop_Saving;

            harmony = new Harmony(ModManifest.UniqueID);

            #region INVENTORY_PATCHES
            harmony.Patch(
                original: AccessTools.Method(typeof(Inventory), nameof(Inventory.ReduceId)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(Inventory_ReduceId_Prefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Inventory), nameof(Inventory.CountId)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(Inventory_CountId_Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Inventory), nameof(Inventory.ContainsId), new Type[] { typeof(string) }),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(Inventory_ContainsId_Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Inventory), nameof(Inventory.ContainsId), new Type[] { typeof(string), typeof(int) }),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(Inventory_ContainsId2_Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Inventory), nameof(Inventory.GetById)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(Inventory_GetById_Postfix))
            );
            #endregion

            #region FARMER_PATCHES
            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.addItemToInventory), new Type[] { typeof(Item), typeof(List<Item>) }),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(Farmer_addItemToInventory_Prefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.getItemCount)),
                transpiler: new HarmonyMethod(typeof(ModEntry), nameof(Farmer_getItemCount_Transpiler))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.couldInventoryAcceptThisItem), new Type[] { typeof(Item) }),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(Farmer_couldInventoryAcceptThisItem_Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.couldInventoryAcceptThisItem), new Type[] { typeof(string), typeof(int), typeof(int) }),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(Farmer_couldInventoryAcceptThisItem2_Postfix))
            );
            #endregion

            #region OBJECT_PATCHES
            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.ConsumeInventoryItem), new Type[] { typeof(Farmer), typeof(Item), typeof(int) }),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(Object_ConsumeInventoryItem_Prefix))
            );
            #endregion

            #region CRAFTING_RECIPE_PATCHES
            harmony.Patch(
                original: AccessTools.Method(typeof(CraftingRecipe), nameof(CraftingRecipe.ConsumeAdditionalIngredients)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(CraftingRecipe_ConsumeAdditionalIngredientsPrefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(CraftingRecipe), nameof(CraftingRecipe.getCraftableCount), new Type[] { typeof(IList<Item>) }),
                transpiler: new HarmonyMethod(typeof(ModEntry), nameof(CraftingRecipe_getCraftableCount_Transpiler))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(CraftingRecipe), nameof(CraftingRecipe.consumeIngredients)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(CraftingRecipe_consumeIngredients_Prefix)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(CraftingRecipe_consumeIngredients_Postfix))
            );
            #endregion

            #region GAME_MENU_PATCHES
            harmony.Patch(
                original: AccessTools.Constructor(typeof(GameMenu), new Type[] { typeof(bool) }),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(GameMenu_Constructor_Postfix))
            );
            #endregion

            #region INVENTORY_PAGE_PATCHES
            harmony.Patch(
                original: AccessTools.Constructor(typeof(InventoryPage), new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) }),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(InventoryPage_Constructor_Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(InventoryPage), nameof(InventoryPage.draw), new Type[] {typeof(SpriteBatch)}),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(InventoryPage_draw_Prefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(InventoryPage), nameof(InventoryPage.performHoverAction)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(InventoryPage_performHoverAction_Prefix))
            );
            
            harmony.Patch(
                original: AccessTools.Method(typeof(InventoryPage), nameof(InventoryPage.receiveKeyPress)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(InventoryPage_receiveKeyPressPrefix))
            );
            
            harmony.Patch(
                original: AccessTools.Method(typeof(InventoryPage), nameof(InventoryPage.receiveGamePadButton)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(InventoryPage_receiveGamePadButton_Prefix))
            );
            
            harmony.Patch(
                original: AccessTools.Method(typeof(InventoryPage), nameof(InventoryPage.receiveLeftClick)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(InventoryPage_receiveLeftClick_Prefix))
            );
            #endregion

            #region ICLICKABLE_MENU_PATCHES
            harmony.Patch(
                original: AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.populateClickableComponentList)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(IClickableMenu_populateClickableComponentList_Postfix))
            );
            #endregion
        }

        public void GameLoop_Saving(object sender, SavingEventArgs e)
        {
            SaveResourceDictionary(Game1.player);
        }

        public void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            SMonitor.Log("Removing this player's dictionary.");
            resourceDict.Remove(Game1.player.UniqueMultiplayerID);
        }

        public void GameLoop_ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            SMonitor.Log("Removing this player's dictionary.");
            resourceDict.Remove(Game1.player.UniqueMultiplayerID);
        }

        public void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            BetterCraftingIntegration.Initialize(SMonitor, SHelper, Config);

            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = SHelper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is not null)
            {
                // register mod
                configMenu.Register(
                    mod: ModManifest,
                    reset: () => Config = new ModConfig(),
                    save: () => SHelper.WriteConfig(Config)
                );

                configMenu.AddBoolOption(
                    mod: ModManifest,
                    name: () => SHelper.Translation.Get("GMCM_Option_ModEnabled_Name"),
                    getValue: () => Config.ModEnabled,
                    setValue: value => Config.ModEnabled = value
                );

                configMenu.AddBoolOption(
                    mod: ModManifest,
                    name: () => SHelper.Translation.Get("GMCM_Option_AutoUse_Name"),
                    getValue: () => Config.AutoUse,
                    setValue: value => Config.AutoUse = value
                );

                configMenu.AddBoolOption(
                    mod: ModManifest,
                    name: () => SHelper.Translation.Get("GMCM_Option_ShowMessage_Name"),
                    getValue: () => Config.ShowMessage,
                    setValue: value => Config.ShowMessage = value
                );
                
                configMenu.AddKeybind(
                    mod: ModManifest,
                    name: () => SHelper.Translation.Get("GMCM_Option_ResourcesKey_Name"),
                    getValue: () => Config.ResourcesKey,
                    setValue: value => Config.ResourcesKey = value
                );
                
                configMenu.AddKeybind(
                    mod: ModManifest,
                    name: () => SHelper.Translation.Get("GMCM_Option_ModKey1_Name"),
                    getValue: () => Config.ModKey1,
                    setValue: value => Config.ModKey1 = value
                );
                
                configMenu.AddNumberOption(
                    mod: ModManifest,
                    name: () => SHelper.Translation.Get("GMCM_Option_ModKey1Amount_Name"),
                    getValue: () => Config.ModKey1Amount,
                    setValue: value => Config.ModKey1Amount = value
                );
                
                configMenu.AddKeybind(
                    mod: ModManifest,
                    name: () => SHelper.Translation.Get("GMCM_Option_ModKey2_Name"),
                    getValue: () => Config.ModKey2,
                    setValue: value => Config.ModKey2 = value
                );
                configMenu.AddNumberOption(
                    mod: ModManifest,
                    name: () => SHelper.Translation.Get("GMCM_Option_ModKey2Amount_Name"),
                    getValue: () => Config.ModKey2Amount,
                    setValue: value => Config.ModKey2Amount = value
                );


                configMenu.AddKeybind(
                    mod: ModManifest,
                    name: () => SHelper.Translation.Get("GMCM_Option_ModKey3_Name"),
                    getValue: () => Config.ModKey3,
                    setValue: value => Config.ModKey3 = value
                );
                configMenu.AddNumberOption(
                    mod: ModManifest,
                    name: () => SHelper.Translation.Get("GMCM_Option_ModKey3Amount_Name"),
                    getValue: () => Config.ModKey3Amount,
                    setValue: value => Config.ModKey3Amount = value
                );

                configMenu.AddNumberOption(
                    mod: ModManifest,
                    name: () => SHelper.Translation.Get("GMCM_Option_IconOffsetX_Name"),
                    getValue: () => Config.IconOffsetX,
                    setValue: value => Config.IconOffsetX = value
                );
                
                configMenu.AddNumberOption(
                    mod: ModManifest,
                    name: () => SHelper.Translation.Get("GMCM_Option_IconOffsetY_Name"),
                    getValue: () => Config.IconOffsetY,
                    setValue: value => Config.IconOffsetY = value
                );

            }
        }
    }
}