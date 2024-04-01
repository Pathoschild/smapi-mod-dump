/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using System.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework;
using ShopTileFramework.Framework;
using ShopTileFramework.Framework.Apis;
using ShopTileFramework.Framework.Patches;
using ShopTileFramework.Framework.Shop;
using ShopTileFramework.Framework.Utility;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using xTile.ObjectModel;

namespace ShopTileFramework
{
    /// <summary>
    /// Entry point of the Shop Tile Framework mod. This mod allows custom shops to be added to the game via data in
    /// json format, loaded via the Stardew Valley Modding API (SMAPI) as content packs
    /// </summary>
    public class ModEntry : Mod
    {
        //static copies of helper and monitor
        internal static IModHelper helper;
        internal static IMonitor monitor;

        //The following variables are to help revert hardcoded warps done by the carpenter and
        //animal shop menus
        private bool _changedMarnieStock;
        internal static GameLocation SourceLocation;
        private static Vector2 _playerPos = Vector2.Zero;

        public static bool VerboseLogging;

        public static bool JustOpenedVanilla = false;

        /// <summary>
        /// the Mod entry point called by SMAPI
        /// </summary>
        /// <param name="h">the helper provided by SMAPI</param>
        public override void Entry(IModHelper h)
        {
            //make helper and monitor static so they can be accessed in other classes
            helper = h;
            monitor = this.Monitor;

            //set verbose logging
            VerboseLogging = helper.ReadConfig<ModConfig>().VerboseLogging;

            if (VerboseLogging)
                monitor.Log("Verbose logging has been turned on. More information will be printed to the console.", LogLevel.Info);

            helper.Events.Input.ButtonPressed += this.Input_ButtonPressed;
            helper.Events.Display.MenuChanged += this.Display_MenuChanged;
            helper.Events.GameLoop.SaveLoaded += this.GameLoop_SaveLoaded;
            helper.Events.GameLoop.DayStarted += this.GameLoop_DayStarted;
            helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
            helper.Events.GameLoop.UpdateTicking += this.GameLoop_UpdateTicking;

            //add console commands
            new ConsoleCommands().Register(helper);

            //get all the info from content packs
            ShopManager.LoadContentPacks();

            Harmony harmony = new Harmony(this.ModManifest.UniqueID);
            VanillaShopStockPatches.Apply(harmony);
        }
        /// <summary>
        /// Checks for warps from the buildings/animals menu 
        /// and ensures the player is returned to their original location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameLoop_UpdateTicking(object sender, StardewModdingAPI.Events.UpdateTickingEventArgs e)
        {
            //Fixes the game warping the player to places we don't want them to warp
            //if buildings/animal purchase menus are brought up from a custom tile
            if (SourceLocation != null && (
                Game1.locationRequest?.Name == "AnimalShop" ||
                Game1.locationRequest?.Name == "WizardHouse" ||
                Game1.locationRequest?.Name == "ScienceHouse"))
            {
                Game1.locationRequest.Location = SourceLocation;
                Game1.locationRequest.IsStructure = SourceLocation.isStructure.Value;
            }
        }
        /// <summary>
        /// Stops Marnie's portrait from appearing in non-Marnie animal shops after animal purchasing
        /// And removes specified animals from Marnie's store
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Display_MenuChanged(object sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            //this block fixes marnie's portrait popping up after purchasing an animal
            if (e.OldMenu is PurchaseAnimalsMenu && e.NewMenu is DialogueBox && SourceLocation != null)
            {
                var animalPurchaseMessage = ((DialogueBox)e.NewMenu).getCurrentString();

                //go away marnie we don't want you
                Game1.exitActiveMenu();

                //display the animal purchase message without Marnie's face
                Game1.activeClickableMenu = new DialogueBox(animalPurchaseMessage);
            }

            //TODO: deprecate this once FAVR is out
            //this is the vanilla Marnie menu for us to exclude animals from
            if (e.NewMenu is PurchaseAnimalsMenu && SourceLocation == null &&
                !this._changedMarnieStock && AnimalShop.ExcludeFromMarnie.Count > 0)
            {
                //close the current menu to open our own	
                Game1.exitActiveMenu();
                var allAnimalsStock = Utility.getPurchaseAnimalStock(Game1.getFarm());
                this._changedMarnieStock = true;

                //removes all animals on the exclusion list
                var newAnimalStock = (from animal in allAnimalsStock
                                      where !AnimalShop.ExcludeFromMarnie.Contains(animal.Name)
                                      select animal).ToList();
                Game1.activeClickableMenu = new PurchaseAnimalsMenu(newAnimalStock);
            }

            //idk why some menus have a habit of warping the player a tile to the left ocassionally
            //so im just gonna warp them back to their original location eh
            if (e.NewMenu == null && _playerPos != Vector2.Zero)
            {
                Game1.player.position.Set(_playerPos);
            }
        }
        /// <summary>
        /// Returns an instance of this mod's api
        /// </summary>
        /// <returns></returns>
        public override object GetApi()
        {
            //TODO: Test this
            return new ShopTileFrameworkApi();
        }
        /// <summary>
        /// On a save loaded, store the language for translation purposes. Done on save loaded in
        /// case it's changed between saves
        /// 
        /// Also retrieve all object informations. This is done on save loaded because that's
        /// when JA adds custom items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameLoop_SaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            Translations.UpdateSelectedLanguage();
            ShopManager.UpdateTranslations();

            ItemsUtil.UpdateObjectInfoSource();
            ShopManager.InitializeItemStocks();

            ItemsUtil.RegisterItemsToRemove();
        }

        /// <summary>
        /// On game launched initialize all the shops and register all external APIs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            ShopManager.InitializeShops();

            ApiManager.RegisterJsonAssets();
            if (ApiManager.JsonAssets != null)
                ApiManager.JsonAssets.AddedItemsToShop += this.JsonAssets_AddedItemsToShop;

            ApiManager.RegisterExpandedPreconditionsUtility();
            ApiManager.RegisterBetterFarmAnimalVariety();
            ApiManager.RegisterFarmAnimalVarietyRedux();
        }

        private void JsonAssets_AddedItemsToShop(object sender, System.EventArgs e)
        {
            //make sure we only remove all objects if we camew from a vanilla store
            //this stops us from removing all packs from custom TMXL or STF stores
            if (!JustOpenedVanilla)
                return;

            if (Game1.activeClickableMenu is ShopMenu shop)
            {
                shop.setItemPriceAndStock(ItemsUtil.RemoveSpecifiedJAPacks(shop.itemPriceAndStock));
            }

            JustOpenedVanilla = false;
        }

        /// <summary>
        /// Refresh the stock of every store at the start of each day
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameLoop_DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            ShopManager.UpdateStock();
        }

        /// <summary>
        /// When input is received, check that the player is free and used an action button
        /// If so, attempt open the shop if it exists
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            //context and button check
            if (!Context.CanPlayerMove)
                return;

            //Resets the boolean I use to check if a menu used to move the player around came from my mod
            //and lets me return them to their original location
            SourceLocation = null;
            _playerPos = Vector2.Zero;
            //checks if i've changed marnie's stock already after opening her menu
            this._changedMarnieStock = false;

            if (Constants.TargetPlatform == GamePlatform.Android)
            {
                if (e.Button != SButton.MouseLeft)
                    return;
                if (e.Cursor.GrabTile != e.Cursor.Tile)
                    return;

                if (VerboseLogging)
                    monitor.Log("Input detected!");
            }
            else if (!e.Button.IsActionButton())
                return;

            Vector2 clickedTile = this.Helper.Input.GetCursorPosition().GrabTile;

            //check if there is a tile property on Buildings layer
            IPropertyCollection tileProperty = TileUtility.GetTileProperty(Game1.currentLocation, "Buildings", clickedTile);

            if (tileProperty == null)
                return;

            //if there is a tile property, attempt to open shop if it exists
            this.CheckForShopToOpen(tileProperty, e);
        }

        /// <summary>
        /// Checks the tile property for shops, and open them
        /// </summary>
        /// <param name="tileProperty"></param>
        /// <param name="e"></param>
        private void CheckForShopToOpen(IPropertyCollection tileProperty, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            //check if there is a Shop property on clicked tile
            tileProperty.TryGetValue("Shop", out PropertyValue shopProperty);
            if (VerboseLogging)
                monitor.Log($"Shop Property value is: {shopProperty}");
            if (shopProperty != null) //There was a `Shop` property so attempt to open shop
            {
                //check if the property is for a vanilla shop, and gets the shopmenu for that shop if it exists
                if (TileUtility.TryOpenVanillaShop(shopProperty, out bool warpingShop))
                {
                    if (warpingShop)
                    {
                        SourceLocation = Game1.currentLocation;
                        _playerPos = Game1.player.position.Get();
                    }

                    //stop the click action from going through after the menu has been opened
                    helper.Input.Suppress(e.Button);
                }
                else //no vanilla shop found
                {
                    //Extract the tile property value
                    string shopName = shopProperty.ToString();

                    if (ShopManager.ItemShops.ContainsKey(shopName))
                    {
                        //stop the click action from going through after the menu has been opened
                        helper.Input.Suppress(e.Button);
                        ShopManager.ItemShops[shopName].DisplayShop();
                    }
                    else
                    {
                        this.Monitor.Log($"A Shop tile was clicked, but a shop by the name \"{shopName}\" could not be opened.", LogLevel.Debug);
                    }
                }
            }
            else //no shop property found
            {
                tileProperty.TryGetValue("AnimalShop", out shopProperty); //see if there's an AnimalShop property
                if (shopProperty != null) //no animal shop found
                {
                    string shopName = shopProperty.ToString();
                    if (ShopManager.AnimalShops.ContainsKey(shopName))
                    {
                        //stop the click action from going through after the menu has been opened
                        helper.Input.Suppress(e.Button);
                        ShopManager.AnimalShops[shopName].DisplayShop();
                    }
                    else
                    {
                        this.Monitor.Log($"An Animal Shop tile was clicked, but a shop by the name \"{shopName}\" " +
                            $"was not found.", LogLevel.Debug);
                    }
                }
            } //end shopProperty null check
        }
    }
}
