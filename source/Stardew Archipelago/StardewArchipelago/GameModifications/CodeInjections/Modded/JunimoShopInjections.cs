/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewArchipelago.GameModifications.Modded;
using StardewArchipelago.Locations.CodeInjections.Vanilla.Relationship;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;

namespace StardewArchipelago.GameModifications.CodeInjections.Modded
{
    public class JunimoShopInjections
    {
        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static ShopStockGenerator _shopStockGenerator;
        private static StardewItemManager _stardewItemManager;
        private static JunimoShopGenerator _junimoShopGenerator;
        private static readonly string Question = "Me love purple thing-a-ma-bobs!  Could give VERY special gift!  You want?";
        private static readonly string Money = "Weird yellow rocks for {0} {1}.";
        private static readonly string Dewdrop = "Blue tasties under pretty tree for {0} {1}.";
        private static readonly string Friendship = "Kiss many people on forehead for {0} {1}.";
        private static readonly Response Nothing = new("No", "Nothing for now!");
        private static Dictionary<string, PurpleJunimo> PurpleJunimoOptions { get; set; }
        private static NPC DaJunimo = new();
        private static readonly string _junimoDialogueKey = "PurpleJunimoVendor";


        private static readonly List<string> _junimoFirstItems = new(){
            "Legend", "Prismatic Shard", "Ancient Seeds", "Dinosaur Egg", "Tiny Crop (stage 1)", "Super Starfruit", "Magic Bait"
        };

        private static readonly Dictionary<string, string> _junimoPhrase = new(){
            {"Orange", "Look! Me gib pretty \nfor orange thing!" },
            {"Red", "Give old things \nfor red gubbins!"},
            {"Grey", "I trade rocks for \n grey what's-its!"},
            {"Yellow", "I hab seeds, gib \nyellow gubbins!"},
            {"Blue", "I hab fish! You \ngive blue pretty?"}

        };
        private static readonly Dictionary<string, string> _firstItemToColor = new(){
            {"Legend", "Blue"}, {"Prismatic Shard", "Grey"}, {"Dinosaur Egg", "Red"}, {"Ancient Seeds", "Yellow"},
            {"Tiny Crop (stage 1)", "Orange"}, {"Super Starfruit", "Purple"}, {"Magic Bait", "Purple"}
        };

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, ShopStockGenerator shopStockGenerator, StardewItemManager stardewItemManager, JunimoShopGenerator junimoShopGenerator)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _shopStockGenerator = shopStockGenerator;
            _stardewItemManager = stardewItemManager;
            _junimoShopGenerator = junimoShopGenerator;
        }



        private static ShopMenu _lastShopMenuUpdated = null;

        // public override void update(GameTime time)
        public static bool Update_JunimoWoodsAPShop_Prefix(ShopMenu __instance, GameTime time)
        {
            try
            {
                // We only run this once for each menu
                if (__instance.storeContext != "Custom_JunimoWoods")
                {
                    return true;
                }
                var firstItemForSale = "";
                if (__instance.forSale.Count == 0) // in case the shop is emptied on a second pass
                {
                    firstItemForSale = "";
                }
                else
                {
                    firstItemForSale = __instance.forSale.First().Name; // Fighting CP moment
                }
                if (!_junimoFirstItems.Contains(firstItemForSale) && _lastShopMenuUpdated == __instance)
                {
                    return true;
                }
                _lastShopMenuUpdated = __instance;
                var color = _firstItemToColor[firstItemForSale];
                if (color == "Purple")
                {
                    var purpleJunimo = __instance.portraitPerson;
                    __instance.exitThisMenuNoSound();
                    DaJunimo = purpleJunimo;
                    DaJunimo.Name = "Purple Junimo";
                    DaJunimo.displayName = "Purple Junimo";
                    PurpleJunimoSpecialShop();
                    return true;
                }
                __instance.itemPriceAndStock = _junimoShopGenerator.GetJunimoShopStock(color, __instance.itemPriceAndStock);
                __instance.forSale = __instance.itemPriceAndStock.Keys.ToList();
                __instance.potraitPersonDialogue = _junimoPhrase[color];
                return true; //  run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Update_JunimoWoodsAPShop_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private class PurpleJunimo
        {
            public StardewItem OfferedItem { get; set; }
            public int Amount { get; set; }
        }

        public static void PurpleJunimoSpecialShop()
        {
            var junimoWoods = Game1.player.currentLocation;
            var friendsMet = Game1.player.friendshipData.Count();
            var fakeStock = new Dictionary<string, int>(){
                {"Money", 10000},
                {"Dewdrop", 2500},
                {"Friendship", 75 * friendsMet}
            };
            PurpleJunimoOptions = new Dictionary<string, PurpleJunimo>();
            var purpleItems = _junimoShopGenerator.PurpleItems.Keys.ToArray();
            var currentWeek = (int)(Game1.stats.daysPlayed / 7) + 1;
            var random = new Random((int)Game1.uniqueIDForThisGame / 2 + currentWeek);
            foreach (var (item, price) in fakeStock)
            {
                var randomPurpleItem = purpleItems[random.Next(purpleItems.Length)];
                var randomPurpleItemValue = _junimoShopGenerator.PurpleItems[randomPurpleItem];
                var stardewItem = _stardewItemManager.GetObjectById(randomPurpleItem);
                var purpleExchangeRate = _junimoShopGenerator.ExchangeRate(price, randomPurpleItemValue);
                PurpleJunimoOptions[item] = new PurpleJunimo()
                {
                    OfferedItem = stardewItem,
                    Amount = Math.Max(purpleExchangeRate[1] / purpleExchangeRate[0], 1),
                };
            }
            junimoWoods.createQuestionDialogue(
                    Question,
                    new Response[4]
                    {
                        new("Money", string.Format(Money, PurpleJunimoOptions["Money"].Amount.ToString(), PurpleJunimoOptions["Money"].OfferedItem.Name)),
                        new("Dewdrop", string.Format(Dewdrop, PurpleJunimoOptions["Dewdrop"].Amount.ToString(), PurpleJunimoOptions["Dewdrop"].OfferedItem.Name)),
                        new("Friendship", string.Format(Friendship, PurpleJunimoOptions["Friendship"].Amount.ToString(), PurpleJunimoOptions["Friendship"].OfferedItem.Name)),
                                                Nothing

                    }, _junimoDialogueKey);


            return;

        }

        public static bool AnswerDialogueAction_Junimoshop_Prefix(GameLocation __instance, string questionAndAnswer, string[] questionParams, ref bool __result)
        {
            try
            {
                if (!questionAndAnswer.StartsWith(_junimoDialogueKey))
                {
                    return true; // run original logic
                }
                var answer = questionAndAnswer.Split("_")[1];
                if (answer == "No")
                {
                    DaJunimo.setNewDialogue($"Okie dokie, you know where to find me!  Eheh!");
                    Game1.drawDialogue(DaJunimo);
                    return false;
                }
                var checkOffer = PurpleJunimoOptions[answer];
                if (!Game1.player.hasItemInInventory(checkOffer.OfferedItem.Id, checkOffer.Amount))
                {
                    DaJunimo.setNewDialogue($"You no have thing, sorry!  Check later!");
                    Game1.drawDialogue(DaJunimo);
                    return false;
                }
                if (answer == "Money")
                {
                    PurpleJunimoMoney(checkOffer);
                    return false;
                }
                if (answer == "Dewdrop")
                {
                    PurpleJunimoDewdrop(checkOffer);
                    return false;
                }
                if (answer == "Friendship")
                {
                    PurpleJunimoKiss(checkOffer);
                    return false;
                }

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AnswerDialogueAction_Junimoshop_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void PurpleJunimoMoney(PurpleJunimo purpleJunimoOffer)
        {
            DaJunimo.setNewDialogue($"Oh very nice, have yellow rocks I found!  You like?");
            Game1.drawDialogue(DaJunimo);
            Game1.player.Money += 50000;
            Game1.player.removeItemsFromInventory(purpleJunimoOffer.OfferedItem.Id, purpleJunimoOffer.Amount);
        }

        private static void PurpleJunimoDewdrop(PurpleJunimo purpleJunimoOffer)
        {
            var dewdropBerryId = _stardewItemManager.GetItemByName("Dewdrop Berry").Id;
            var dewdropBerry = new StardewValley.Object(dewdropBerryId, 5);
            Game1.player.holdUpItemThenMessage(dewdropBerry);
            Game1.player.addItemByMenuIfNecessaryElseHoldUp(dewdropBerry);
            DaJunimo.setNewDialogue($"Awh okay I was gonna sleep with it, but here you go!");
            Game1.drawDialogue(DaJunimo);
            Game1.player.removeItemsFromInventory(purpleJunimoOffer.OfferedItem.Id, purpleJunimoOffer.Amount);
        }

        private static void PurpleJunimoKiss(PurpleJunimo purpleJunimoOffer)
        {
            if (Game1.player.mailReceived.Contains("purpleJunimoKiss"))
            {
                DaJunimo.setNewDialogue($"You want me to kiss them TWICE?!  Wowie, but sorry!");
                Game1.drawDialogue(DaJunimo);
                return;
            }
            DaJunimo.setNewDialogue($"I will tonight!  Yay!");
            Game1.drawDialogue(DaJunimo);
            Game1.player.mailReceived.Add("purpleJunimoKiss");
            Game1.player.removeItemsFromInventory(purpleJunimoOffer.OfferedItem.Id, purpleJunimoOffer.Amount);
        }

        // public void resetFriendshipsForNewDay()
        public static void ResetFriendshipsForNewDay_KissForeheads_Postfix(Farmer __instance)
        {
            try
            {
                if (!Game1.player.mailReceived.Contains("purpleJunimoKiss"))
                {
                    return;
                }
                foreach (var friendship in Game1.player.friendshipData.Keys)
                {
                    var friend = Game1.getCharacterFromName(friendship);
                    var npc = friend ?? Game1.getCharacterFromName<Child>(friendship, false);
                    if (npc == null)
                    {
                        continue;
                    }
                    Game1.player.changeFriendship(100, friend);
                }
                Game1.player.mailReceived.Remove("purpleJunimoKiss");
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ResetFriendshipsForNewDay_KissForeheads_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }
    }
}