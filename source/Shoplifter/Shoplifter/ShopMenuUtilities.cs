/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/Shoplifter
**
*************************************************/

using System;
using StardewValley.Locations;
using StardewModdingAPI;
using StardewValley.Util;
using StardewValley.Menus;
using StardewValley;
using Microsoft.Xna.Framework;
using xTile.Dimensions;

namespace Shoplifter
{
    public class ShopMenuUtilities
    {
        private static IMonitor monitor;
        private static IManifest manifest;
        private static ModConfig config;

        private static int fineamount;
      
        public static void gethelpers(IMonitor monitor, IManifest manifest, ModConfig config)
        {
            ShopMenuUtilities.monitor = monitor;
            ShopMenuUtilities.manifest = manifest;
            ShopMenuUtilities.config = config;
        }

        /// <summary>
        /// Determines whether player can shoplift based on config settings
        /// </summary>
        /// <param name="shop">The shop to check</param>
        /// <returns>true if player can shoplift, false otherwise</returns>
        public static bool CanShoplift(string shop)
        {
            if (ModEntry.PerScreenShopliftCounter.Value >= config.MaxShopliftsPerDay || (ModEntry.PerScreenShopliftedShops.Value.ContainsKey(shop) == true && ModEntry.PerScreenShopliftedShops.Value[shop] >= config.MaxShopliftsPerStore))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Subtracts friendship from any npc that sees the player shoplifting
        /// </summary>
        /// <param name="location">The current location instance</param>
        /// <param name="who">The player</param>
        public static void SeenShoplifting(GameLocation location, Farmer who)
        {
            foreach (var npc in location.characters)
            {
                // Is NPC in range?
                if (npc.currentLocation == who.currentLocation && Utility.tileWithinRadiusOfPlayer(npc.getTileX(), npc.getTileY(), (int)config.CaughtRadius, who))
                {
                    // Emote NPC
                    npc.doEmote(12, false, false);

                    // Has player met NPC?
                    if (Game1.player.friendshipData.ContainsKey(npc.Name) == true)
                    {
                        // Lower friendship by some amount or frienship level, whichever is lower
                        int frienshiploss = -Math.Min((int)config.FriendshipPenalty, Game1.player.getFriendshipLevelForNPC(npc.Name));
                        Game1.player.changeFriendship(frienshiploss, Game1.getCharacterFromName(npc.Name, true));
                        monitor.Log($"{npc.Name} saw you shoplifting... {-frienshiploss} friendship points lost");
                    }

                    else
                    {
                        monitor.Log($"{npc.Name} saw you shoplifting... You've never talked to {npc.Name}, no friendship to lose");
                    }
                }               
            }
        }

        /// <summary>
        /// Determines whether the player is caught, makes corrections to dialogue based on return value
        /// </summary>
        /// <param name="which">Who should catch the player</param>
        /// <param name="who">The player to catch</param>
        /// <param name="location">The current location instance</param>
        /// <returns>Whether the player was caught</returns>      
        public static bool ShouldBeCaught(string[] which, Farmer who, GameLocation location)
        {
            foreach(string character in which)
            {
                foreach (var npc in location.characters)
                {
                    // Is NPC in range?
                    if (npc.Name == character && npc.currentLocation == who.currentLocation && Utility.tileWithinRadiusOfPlayer(npc.getTileX(), npc.getTileY(), (int)config.CaughtRadius, who))
                    {
                        string dialogue;
                        string banneddialogue = (config.DaysBannedFor == 1)
                            ? i18n.string_BanFromShop_Single()
                            : i18n.string_BanFromShop();

                        fineamount = Math.Min(Game1.player.Money, (int)config.MaxFine);

                        // Is NPC primary shopowner
                        if (character == "Pierre" || character == "Willy" || character == "Robin" || character == "Marnie" || character == "Gus" || character == "Harvey" || character == "Clint" || character == "Sandy" || character == "Alex")
                        {
                            // Yes, they have special dialogue
                            dialogue = (fineamount > 0)
                                ? i18n.string_Caught(character)
                                : i18n.string_Caught_NoMoney(character);


                        }

                        else
                        {
                            // No, use generic dialogue
                            dialogue = (fineamount > 0)
                                ? i18n.string_Caught("Generic")
                                : i18n.string_Caught_NoMoney("Generic");
                        }

                        // Is the player now banned? (uses catch before as dialogue is loaded before count is adjusted) Append additional dialogue
                        dialogue = (Game1.player.modData.ContainsKey($"{manifest.UniqueID}_{location.NameOrUniqueName}") == true && Game1.player.modData[$"{manifest.UniqueID}_{location.NameOrUniqueName}"].StartsWith($"{config.CatchesBeforeBan - 1}") == true)
                            ? dialogue + banneddialogue
                            : dialogue;

                        npc.setNewDialogue(dialogue, add: true);

                        // Draw dialogue for NPC, dialogue box opens
                        Game1.drawDialogue(npc);
                        monitor.Log($"{character} caught you shoplifting... You were fined {fineamount}g");

                        return true;
                    }
                }
            }           
            return false;
        }

        /// <summary>
        /// Applies shoplifting penalties, tracks whether to ban player
        /// </summary>
        /// <param name="location">The current location instance</param>
        /// <param name="bannable">Whether the player can be banned from the shop</param>
        public static void ShopliftingPenalties(GameLocation location, bool bannable = true)
        {
            // Subtract monetary penalty if it applies
            if (fineamount > 0)
            {
                Game1.player.Money -= fineamount;
            }

            if (bannable == true)
            {
                Game1.warpFarmer(location.warps[0].TargetName, location.warps[0].TargetX, location.warps[0].TargetY, false);
            }
            
            string locationname = location.NameOrUniqueName;

            var data = Game1.player.modData;        

            if (config.DaysBannedFor == 0 || bannable == false)
            {
                return;
            }

            string[] fields = data[$"{manifest.UniqueID}_{locationname}"].Split('/') ?? new string[] { };
            
            if (fields.Length < 2 || fields.Length < 2)
            {
                monitor.Log($"Required save field for {locationname} couldn't be accessed. Ban logic may not work. Try resetting the mod save data if issue persists.");
                return;
            }

            // Add one to first part of data (shoplifting count)
            fields[0] = (int.Parse(fields[0]) + 1).ToString();

            // If this is the first shoplift, record day of month in second part of data (day of first steal)
            if (fields[0] == "1")
            {
                fields[1] = Game1.dayOfMonth.ToString();
            }

            // After being caught some times (within 28 days) ban player from shop for three days
            if (int.Parse(fields[0]) == config.CatchesBeforeBan)
            {
                fields[0] = "-1";
                ModEntry.PerScreenShopsBannedFrom.Value.Add($"{locationname}");
                monitor.Log($"{locationname} added to banned shop list");
            }

            // Join manipulated data for recording in save file
            data[$"{manifest.UniqueID}_{locationname}"] = string.Join("/", fields);
        }

        /// <summary>
        /// Create the shoplifting menu and determine what to do depending on the option pressed
        /// </summary>
        /// <param name="location">The current location instance</param>
        /// <param name="shopkeepers">A list of npcs that can ban the player if caught</param>
        /// <param name="shop">The shop the menu is for</param>
        /// <param name="maxstock">The max number of stock items to generate</param>
        /// <param name="maxquantity">The max quantity of each stock item to generate</param>
        /// <param name="islandvisit">Whether the owner is on the island, special menu case if true</param>
        /// <param name="bannable">Whether the player can be banned from the shop</param>
        public static void ShopliftingMenu(GameLocation location, string[] shopkeepers, string shop, int maxstock, int maxquantity, bool islandvisit = false, bool bannable = true)
        {
            // Check player hasn't reached daily shoplifting limit
            if (ModEntry.PerScreenShopliftCounter.Value >= config.MaxShopliftsPerDay)
            {
                // Player has reached limit, exit method after displaying dialogue box
                Game1.drawObjectDialogue(config.MaxShopliftsPerDay != 1
                        ? i18n.string_AlreadyShoplifted()
                        : i18n.string_AlreadyShoplifted_Single());
                return;
            }

            // Check player hasn't reached daily shoplifting limit for the shop
            else if (config.MaxShopliftsPerStore > 1 && ModEntry.PerScreenShopliftedShops.Value.ContainsKey(shop) == true && ModEntry.PerScreenShopliftedShops.Value[shop] >= config.MaxShopliftsPerStore)
            {
                // Player has reached limit, exit method after displaying dialogue box
                Game1.drawObjectDialogue(i18n.string_AlreadyShopliftedSameShop());
                return;
            }

            // Create option to steal
            location.createQuestionDialogue(i18n.string_Shoplift(), location.createYesNoResponses(), delegate (Farmer _, string answer)
            {
                // Player answered yes
                if (answer == "Yes")
                {
                    SeenShoplifting(location, Game1.player);

                    // Player is caught
                    if (ShouldBeCaught(shopkeepers, Game1.player, location) == true)
                    {
                        // After dialogue, apply penalties
                        Game1.afterDialogues = delegate
                        {
                            ShopliftingPenalties(location, bannable);
                        };

                        return;
                    }

                    // Set stolen to true so counters will increment
                    ModEntry.PerScreenStolen.Value = false;

                    // Not caught, generate stock for shoplifting, on purchase adjust counters
                    Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(maxstock, maxquantity, shop), 3, null, delegate
                    {
                        if (ModEntry.PerScreenStolen.Value == false)
                        {
                            // Increment times shoplifted by one
                            ModEntry.PerScreenShopliftCounter.Value++;
                        }                        

                        if (config.MaxShopliftsPerStore > 1 && ModEntry.PerScreenShopliftedShops.Value.ContainsKey(shop) == false)
                        {
                            // Add shop to list of shoplifted shops if needed
                            ModEntry.PerScreenShopliftedShops.Value.Add(shop, 1);
                        }

                        else if (ModEntry.PerScreenStolen.Value == false && config.MaxShopliftsPerStore > 1 && ModEntry.PerScreenShopliftedShops.Value.ContainsKey(shop) == true)
                        {
                            // Shop on shoplifted list, increment value by one
                            ModEntry.PerScreenShopliftedShops.Value[shop]++;
                        }

                        // Set stolen to false so counters won't increment on next purchase while in shoplifting window, will become true next time window opens so it can increment again
                        ModEntry.PerScreenStolen.Value = true;

                        return false;
                    }, null, "");
                }

                else if (answer != "Yes" && islandvisit == true)
                {
                    switch (shop)
                    {
                        case "SeedShop":
                            Game1.activeClickableMenu = new ShopMenu((location as SeedShop).shopStock());
                            break;
                        case "Carpenters":
                            Game1.activeClickableMenu = new ShopMenu(Utility.getCarpenterStock());
                            break;
                        case "AnimalShop":
                            Game1.activeClickableMenu = new ShopMenu(Utility.getAnimalShopStock());
                            break;
                        case "Saloon":
                            Game1.activeClickableMenu = new ShopMenu(Utility.getSaloonStock(), 0, null, delegate (ISalable item, Farmer farmer, int amount)
                            {
                                Game1.player.team.synchronizedShopStock.OnItemPurchased(SynchronizedShopStock.SynchedShop.Saloon, item, amount);
                                return false;
                            });
                            break;
                        default:
                            break;
                    }                   
                }
            });
        }

        /// <summary>
        /// Create the shoplifting menu with Fishshop stock if necessary
        /// </summary>
        /// <param name="location">The current location instance</param>
        public static void FishShopShopliftingMenu(GameLocation location)
        {
            // Willy can sell, don't do anything
            if (location.getCharacterFromName("Willy") != null && location.getCharacterFromName("Willy").getTileLocation().Y < (float)Game1.player.getTileY())
            {
                return;
            }

            // Player can steal
            else 
            {
                ShopliftingMenu(location, new string[1] { "Willy" }, "FishShop", 3, 3);                              
            }
            
        }

        /// <summary>
        /// Create the shoplifting menu with SandyShop stock if necessary
        /// </summary>
        /// <param name="location">The current location instance</param>
        public static void SandyShopShopliftingMenu(GameLocation location)
        {
            NPC sandy = location.getCharacterFromName("Sandy");
            if (sandy == null || sandy.currentLocation != location)
            {
                ShopliftingMenu(location, new string[1] { "Sandy" }, "SandyShop", 3, 3);
            }
        }

        /// <summary>
        /// Create the shoplifting menu with Seedshop stock if necessary
        /// </summary>
        /// <param name="location">The current location instance</param>
        public static void SeedShopShopliftingMenu(GameLocation location)
        {
            // Pierre can sell
            if (location.getCharacterFromName("Pierre") != null && location.getCharacterFromName("Pierre").getTileLocation().Equals(new Vector2(4f, 17f)) && Game1.player.getTileY() > location.getCharacterFromName("Pierre").getTileY())
            {
                return;
            }

            // Pierre is not at shop and on island, player can purchase stock properly or steal, ignore if can't shoplift
            else if (location.getCharacterFromName("Pierre") == null && Game1.IsVisitingIslandToday("Pierre") == true && CanShoplift("SeedShop") == true)
            {
                Game1.dialogueUp = false;
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:SeedShop_MoneyBox"));
                Game1.afterDialogues = delegate
                {
                    ShopliftingMenu(location, new string[3] { "Pierre", "Caroline", "Abigail" }, "SeedShop", 5, 5, true);
                };
            }

            // Pierre not at counter, player can steal
            else
            {
                Game1.dialogueUp = false;
                ShopliftingMenu(location, new string[3] { "Pierre", "Caroline", "Abigail" }, "SeedShop", 5, 5);
            }
        }

        /// <summary>
        /// Create the shoplifting menu with Carpenter stock if necessary
        /// </summary>
        /// <param name="location">The current location instance</param>
        /// <param name="who">The player</param>
        /// <param name="tileLocation">The clicked tilelocation</param>
        public static void CarpenterShopliftingMenu(GameLocation location, Farmer who, Location tileLocation)
        {
            // Player is in correct position for buying
            if (who.getTileY() > tileLocation.Y)
            {
                // Robin is on island and not at sciencehouse, she can't sell but player can purchase properly if they want, ignore if can't shoplift
                if (location.getCharacterFromName("Robin") == null && Game1.IsVisitingIslandToday("Robin") == true && CanShoplift("Carpenters") == true)
                {
                    // Close any current dialogue boxes
                    Game1.dialogueUp = false;
                    // Show normal dialogue
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_MoneyBox"));
                    // Create question to shoplift after dialogue box is exited
                    Game1.afterDialogues = delegate
                    {
                        ShopliftingMenu(location, new string[4] { "Robin", "Demetrius", "Maru", "Sebastian" }, "Carpenters", 2, 20, true);
                    };
                }

                // Robin is absent and can't sell, player can steal
                else if (Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Tue") && location.carpenters(tileLocation) == true && location.getCharacterFromName("Robin") == null)
                {
                    Game1.dialogueUp = false;
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_RobinAbsent").Replace('\n', '^'));
                    Game1.afterDialogues = delegate
                    {
                        ShopliftingMenu(location, new string[4] { "Robin", "Demetrius", "Maru", "Sebastian" }, "Carpenters", 2, 20);
                    };

                }

                // Robin can't sell. Period
                else if (location.carpenters(tileLocation) == false)
                {
                    ShopliftingMenu(location, new string[4] { "Robin", "Demetrius", "Maru", "Sebastian" }, "Carpenters", 2, 20);
                }

                // Robin can sell and player can't steal
                else if (location.carpenters(tileLocation) == true && ModEntry.PerScreenStolen.Value == true)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Create the shoplifting menu with AnimalShop stock if necessary
        /// </summary>
        /// <param name="location">The current location instance</param>
        /// <param name="who">The player</param>
        /// <param name="tileLocation">The clicked tilelocation</param>
        public static void AnimalShopShopliftingMenu(GameLocation location, Farmer who, Location tileLocation)
        {
            // Player is in correct position for buying
            if (who.getTileY() > tileLocation.Y)
            {
                // Marnie is not in the location, she is on the island, ignore if can't shoplift
                if (location.getCharacterFromName("Marnie") == null && Game1.IsVisitingIslandToday("Marnie") == true && CanShoplift("AnimalShop") == true)
                {
                    Game1.dialogueUp = false;
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:AnimalShop_MoneyBox"));
                    Game1.afterDialogues = delegate
                    {
                        ShopliftingMenu(location, new string[2] { "Marnie", "Shane" }, "AnimalShop", 1, 15, true);
                    };
                }

                // Marnie is not at the location and is absent for the day
                else if (Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Tue") && location.animalShop(tileLocation) == true && location.getCharacterFromName("Marnie") == null)
                {
                    Game1.dialogueUp = false;
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Absent").Replace('\n', '^'));
                    Game1.afterDialogues = delegate
                    {
                        ShopliftingMenu(location, new string[2] { "Marnie", "Shane" }, "AnimalShop", 1, 15);
                    };
                }

                // Marnie can't sell. Period.
                else if (location.animalShop(tileLocation) == false)
                {
                    ShopliftingMenu(location, new string[2] { "Marnie", "Shane" }, "AnimalShop", 1, 15);
                }

                // Marnie can sell and player can't steal
                else if (location.animalShop(tileLocation) == true && ModEntry.PerScreenStolen.Value == true)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Create the shoplifting menu with Hospital stock if necessary
        /// </summary>
        /// <param name="location">The current location instance</param>
        /// <param name="who">The player</param>
        public static void HospitalShopliftingMenu(GameLocation location, Farmer who)
        {
            // Character is not at the required tile, noone can sell
            if (location.isCharacterAtTile(who.getTileLocation() + new Vector2(0f, -2f)) == null && location.isCharacterAtTile(who.getTileLocation() + new Vector2(-1f, -2f)) == null)
            {
                ShopliftingMenu(location, new string[2] { "Harvey", "Maru" }, "HospitalShop", 1, 3);
            }
        }

        /// <summary>
        /// Create the shoplifting menu with Blacksmith stock if necessary
        /// </summary>
        /// <param name="location">The current location instance</param>
        /// <param name="tileLocation">The clicked tilelocation</param>
        public static void BlacksmithShopliftingMenu(GameLocation location, Location tileLocation)
        {
            // Clint can't sell. Period.
            if (location.blacksmith(tileLocation) == false)
            {
                ShopliftingMenu(location, new string[1] { "Clint" }, "Blacksmith", 3, 10);
            }
        }

        /// <summary>
        /// Create the shoplifting menu with Saloon stock if necessary
        /// </summary>
        /// <param name="location">The current location instance</param>
        public static void SaloonShopliftingMenu(GameLocation location, Location tilelocation)
        {           
            // Gus is not in the location, he is on the island, ignore if can't shoplift
            if (location.getCharacterFromName("Gus") == null && Game1.IsVisitingIslandToday("Gus") == true && CanShoplift("Saloon") == true)
            {
                Game1.dialogueUp = false;
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Saloon_MoneyBox"));
                Game1.afterDialogues = delegate
                {
                    ShopliftingMenu(location, new string[2] { "Gus", "Emily" }, "Saloon", 2, 1, true);
                };                               
            }

            // Gus can sell, player can't steal
            else if (location.saloon(tilelocation) == true && ModEntry.PerScreenStolen.Value == true)
            {
                return;
            }

            // Gus can't sell. Period.
            else if (location.saloon(tilelocation) == false)
            {
                ShopliftingMenu(location, new string[2] { "Gus", "Emily" }, "Saloon", 2, 1);
            }
        }

        public static void IceCreamShopliftingMenu(GameLocation location, Location tilelocation)
        {
            if (location.isCharacterAtTile(new Vector2(tilelocation.X, tilelocation.Y - 2)) == null && location.isCharacterAtTile(new Vector2(tilelocation.X, tilelocation.Y - 1)) == null)
            {
                ShopliftingMenu(location, new string[1] { "Alex" }, "IceCreamStand", 1, 5, false, false);               
            }
        }
    }
}
