/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-bundles
**
*************************************************/

using System;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using HarmonyLib;
using System.Collections.Generic;
using Unlockable_Bundles.API;
using Unlockable_Bundles.Lib.ShopTypes;
using StardewValley.BellsAndWhistles;
using Unlockable_Bundles.Lib;
using StardewValley.Locations;
using System.Linq;
using Unlockable_Bundles.Lib.Enums;
using static StardewValley.BellsAndWhistles.ParrotUpgradePerch;
using Unlockable_Bundles.Lib.AdvancedPricing;

namespace Unlockable_Bundles
{

    public class ModEntry : Mod
    {

        public static Mod Mod;
        public static ModConfig Config;
        public static IMonitor _Monitor;
        public static IModHelper _Helper;
        public static UnlockableBundlesAPI _API;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            Mod = this;
            _Monitor = Monitor;
            _Helper = Helper;

            ContentPatcherHandling.Initialize();
            UnlockableBundlesAPI.Initialize();
            GenericModConfigMenuHandler.Initialize();
            Lib.Main.Initialize();

            helper.ConsoleCommands.Add("ub", "Dev Commands", this.commands);
        }

        //Please note that these commands are mainly used for development purposes.
        //There's no guarentee they wont be changed or removed unannounced (apart from those already documented)
        private void commands(string command, string[] args)
        {
            if (args.Length == 0) {
                printValidCommands();
                return;
            }

            switch (args[0].ToLower()) {
                case "help":
                    printHelp(); break;

                case "ok" or "purchase":
                    debugPurchase(); break;

                case "event":
                    if (args.Length == 1)
                        Monitor.Log("Missing Bundle Key", LogLevel.Error);
                    else
                        playEventScript(args[1]);
                    break;

                case "location":
                    printLocationData(); break;

                case "mailkey" or "mail":
                    if (args.Length == 1)
                        Monitor.Log("Missing Bundle Key", LogLevel.Error);
                    else
                        printMailKey(args.Skip(1).Join(null, " "));
                    break;

                case "discover":
                    if (args.Length == 1)
                        Monitor.Log("Missing Bundle Key", LogLevel.Error);
                    else
                        discoverBundles(args.Skip(1).Join(null, " "), true);
                    break;

                case "undiscover" or "forget":
                    if (args.Length == 1)
                        Monitor.Log("Missing Bundle Key", LogLevel.Error);
                    else
                        discoverBundles(args.Skip(1).Join(null, " "), false);
                    break;

                case "quality":
                    Game1.player.CurrentItem.Quality = int.Parse(args[1]); break;

                case "contexttags" or "tags":
                    printContextTags(); break;

                case "item" or "i":
                    addItem(args); break;

                case "showdebugnames":
                    Unlockable.ShowDebugNames = !Unlockable.ShowDebugNames; break;

                case "debug":
                    debug(); break;

                case "apitest":
                    apiTest(); break;

                case "eventtest":
                    eventTest(); break;

                default:
                    Monitor.Log("Unknown Command: " + args[0], LogLevel.Error); break;
            }
        }
        private void printValidCommands() =>
            Monitor.Log("Valid Commands: help, purchase, event, location, mailkey, discover, undiscover, showdebugnames, quality, tags, item", LogLevel.Info);

        private void printHelp() =>
            Monitor.Log(
                "Valid Commands:\n" +
                "PURCHASE           Completes the bundle whose menu is currently open or the closest.\n" +
                "EVENT KEY          Fires the ShopEvent script of the Bundle. Must be in the intended location\n" +
                "LOCATION           Prints the current location\n" +
                "MAILKEY KEY        Prints the resulting mail key of a bundle key\n" +
                "DISCOVER KEY       Discovers all bundle Shops matching KEY. Use key ALL to discover all bundles.\n" +
                "UNDISCOVER KEY     Forgets all bundle shops matching KEY. Use key ALL to forget all bundles.\n" +
                "SHOWDEBUGNAMES     Toggles bundle keys & bundle names in the bundle overview menu\n" +
                "QUALITY 0-4        Sets the quality of the currently held item\n" +
                "CONTEXTTAGS        Prints all context tags of the currently held item. Alt. TAGS\n" +
                "ITEM ID [AMOUNT]   Adds the specified item to your inventory. Accepts UB specific Syntax"
                    , LogLevel.Info);

        private void addItem(string[] args)
        {
            if (args.Length == 1) {
                Monitor.Log("No itemID specified", LogLevel.Error);
                return;
            }

            var id = Unlockable.getIDFromReqSplit(args[1]);
            var quality = Unlockable.getQualityFromReqSplit(args[1]);
            var stack = args.Length > 2 ? int.Parse(args[2]) : 1;

            var item = Unlockable.parseItem(id, stack, quality);

            if (item is AdvancedPricingItem apItem) {
                if (apItem.UsesFlavoredSyntax) {
                    apItem.ItemCopy.Quality = quality;
                    apItem.ItemCopy.Stack = stack;
                    item = apItem.ItemCopy;
                } else {
                    Monitor.Log($"The ITEM command does not accept advanced pricing syntax apart from auto generated flavored Items!", LogLevel.Error);
                    return;
                }
            }

            Game1.player.addItemToInventory(item);
        }

        private void printContextTags()
        {
            if (!Context.IsWorldReady) {
                Monitor.Log("No savefile loaded", LogLevel.Error);
                return;
            }

            var item = Game1.player.CurrentItem;

            if (item is null) {
                Monitor.Log("No Item selected", LogLevel.Error);
                return;
            }

            var res = $"All context tags for item '{item.Name}':";
            var tags = item.GetContextTags();
            foreach (var tag in tags)
                res += "\n" + tag;

            Monitor.Log(res, LogLevel.Info);
        }

        private void printLocationData()
        {
            if (!Context.IsWorldReady) {
                Monitor.Log("No savefile loaded", LogLevel.Error);
                return;
            }

            Monitor.Log($"\nLocation: {Game1.currentLocation.Name}\nUnique: {Game1.currentLocation.NameOrUniqueName}", LogLevel.Info);
        }

        private void printMailKey(string key)
            => Monitor.Log(Unlockable.getMailKey(key), LogLevel.Info);

        private void discoverBundles(string key, bool wasdiscovered)
        {
            if (!Context.IsWorldReady) {
                Monitor.Log("No savefile loaded", LogLevel.Error);
                return;
            }

            var shops = ShopObject.getAll();

            if (key.EndsWith('*'))
                shops = shops.Where(el => el.Unlockable.ID.ToLower().StartsWith(key.Remove(key.Length-1).ToLower())).ToList();
            else if (key.ToLower().Trim() != "all")
                shops = shops.Where(el => el.Unlockable.ID.ToLower() == key.ToLower()).ToList();

            shops.ForEach(el => el.WasDiscovered = wasdiscovered);

            Monitor.Log("Done!", LogLevel.Info);
        }

        private void debug()
        {
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }

        private void playEventScript(string key)
        {
            if (!Context.IsWorldReady) {
                Monitor.Log("No savefile loaded", LogLevel.Error);
                return;
            }

            var unlockables = Helper.GameContent.Load<Dictionary<string, UnlockableModel>>("UnlockableBundles/Bundles");
            Game1.globalFadeToBlack(() => Game1.currentLocation.startEvent(new UBEvent(new Unlockable(unlockables[key]), unlockables[key].ShopEvent, Game1.player)));
        }

        private void debugPurchase()
        {
            if (!Context.IsWorldReady) {
                Monitor.Log("No savefile loaded", LogLevel.Error);
                return;
            }

            if (Game1.activeClickableMenu is DialogueShopMenu menu) {
                menu.Unlockable.processPurchase();
                menu.ScreenSwipe = new ScreenSwipe(0);
                menu.CompletionTimer = 800;
                menu.Complete = true;
                menu.CanClick = false;

            } else if (Game1.activeClickableMenu is BundleMenu ccMenu) {
                ccMenu.Unlockable.processPurchase();
                ccMenu.ScreenSwipe = new ScreenSwipe(0);
                ccMenu.CompletionTimer = 800;
                ccMenu.Complete = true;
                ccMenu.CanClick = false;
            } else {
                var tile = Game1.player.Tile;
                var loc = Game1.player.currentLocation;
                StardewValley.Object obj;

                obj = loc.getObjectAtTile((int)tile.X, (int)tile.Y);
                if (obj is not ShopObject)
                    obj = loc.getObjectAtTile((int)tile.X, (int)tile.Y - 1);
                if (obj is not ShopObject)
                    obj = loc.getObjectAtTile((int)tile.X + 1, (int)tile.Y);
                if (obj is not ShopObject)
                    obj = loc.getObjectAtTile((int)tile.X, (int)tile.Y + 1);
                if (obj is not ShopObject)
                    obj = loc.getObjectAtTile((int)tile.X - 1, (int)tile.Y);

                if (obj is ShopObject shop) {
                    switch (shop.ShopType) {
                        case ShopType.CCBundle or ShopType.AltCCBundle or ShopType.Dialogue:
                            Monitor.Log("This bundle type requires its menu to be open to debug purchase", LogLevel.Warn); break;
                        case ShopType.ParrotPerch or ShopType.SpeechBubble:
                            Game1.activeClickableMenu = null;
                            shop.SpeechBubble.CurrentState.Value = UpgradeState.StartBuilding;
                            shop.Unlockable.processPurchase();
                            shop.SpeechBubble.WaitingForProcessShopEvent = true;
                            break;

                    }
                } else
                    Monitor.Log("No Bundle Shop in direct vicinity", LogLevel.Info);
            }
        }

        private void apiTest()
        {
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();

            IUnlockableBundlesAPI api = Helper.ModRegistry.GetApi<IUnlockableBundlesAPI>(Mod.ModManifest.UniqueID);

            var bundles = api.getBundles();
            var purchased = api.PurchasedBundles;
            var purchasedSince = api.PurchaseBundlesByLocation;
        }

        private void eventTest()
        {
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();

            IUnlockableBundlesAPI api = Helper.ModRegistry.GetApi<IUnlockableBundlesAPI>(Mod.ModManifest.UniqueID);
            api.BundlePurchasedEvent += eventTestMethod;
        }

        private void eventTestMethod(object source, IBundlePurchasedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }

        public override object GetApi()
        {
            _API = new UnlockableBundlesAPI();
            return _API;
        }
    }
}
