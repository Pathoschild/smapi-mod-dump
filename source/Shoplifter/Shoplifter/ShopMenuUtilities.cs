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

        private static int fineamount;
      
        public static void gethelpers(IMonitor monitor, IManifest manifest)
        {
            ShopMenuUtilities.monitor = monitor;
            ShopMenuUtilities.manifest = manifest;
        }

        /// <summary>
        /// Subtracts friendship from any npc that sees the player shoplifting
        /// </summary>
        /// <param name="location">The current location instance</param>
        /// <param name="who">The player</param>
        public static void SeenShoplifting(GameLocation location, Farmer who)
        {
            foreach(NPC i in location.characters)
            {
                // Is NPC in range?
                if (i.currentLocation == who.currentLocation && Utility.tileWithinRadiusOfPlayer(i.getTileX(), i.getTileY(), 7, who))
                {
                    // Emote NPC
                    i.doEmote(12, false, false);

                    // Has player met NPC?
                    if (Game1.player.friendshipData.ContainsKey(i.Name) == true)
                    {
                        // Lower friendship by 500 or frienship level, whichever is lower
                        int frienshiploss = -Math.Min(500, Game1.player.getFriendshipLevelForNPC(i.Name));
                        Game1.player.changeFriendship(frienshiploss, Game1.getCharacterFromName(i.Name, true));
                        monitor.Log($"{i.Name} saw you shoplifting... {-frienshiploss} friendship points lost");
                    }

                    else
                    {
                        monitor.Log($"{i.Name} saw you shoplifting... You've never talked to {i.Name}, no friendship to lose");
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
        public static bool ShouldBeCaught(string which, Farmer who, GameLocation location)
        {
            NPC npc = Game1.getCharacterFromName(which);
            
            // Is NPC in range?
            if (npc != null && npc.currentLocation == who.currentLocation && Utility.tileWithinRadiusOfPlayer(npc.getTileX(), npc.getTileY(), 7, who))
            {
                string dialogue;
                fineamount = Math.Min(Game1.player.Money, 1000); 
                
                try
                {
                    // Is NPC primary shopowner
                    if (which == "Pierre" || which == "Willy" || which == "Robin" || which == "Marnie" || which == "Gus" || which == "Harvey" || which == "Clint")
                    {
                        // Yes, they have special dialogue
                        dialogue = (fineamount > 0)
                            ? ModEntry.shopliftingstrings[$"TheMightyAmondee.Shoplifter/Caught{which}"].Replace("{0}", fineamount.ToString())
                            : ModEntry.shopliftingstrings[$"TheMightyAmondee.Shoplifter/Caught{which}_NoMoney"];

                        
                    }

                    else
                    {
                        // No, use generic dialogue
                        dialogue = (fineamount > 0)
                            ? ModEntry.shopliftingstrings[$"TheMightyAmondee.Shoplifter/CaughtGeneric"].Replace("{0}", fineamount.ToString())
                            : ModEntry.shopliftingstrings[$"TheMightyAmondee.Shoplifter/CaughtGeneric_NoMoney"];
                    }

                    // Is the player now banned? (uses 2 as dialogue is loaded before count is adjusted) Append additional dialogue
                    dialogue = (Game1.player.modData[$"{manifest.UniqueID}_{location.NameOrUniqueName}"].StartsWith("2") == true)
                        ? dialogue + ModEntry.shopliftingstrings[$"TheMightyAmondee.Shoplifter/BanFromShop"]
                        : dialogue;

                    npc.setNewDialogue(dialogue, add: true);
                }
                catch
                {
                    // If any string could not be found, use placeholder
                    npc.setNewDialogue(ModEntry.shopliftingstrings["Placeholder"], add: true);
                }

                // Draw dialogue for NPC, dialogue box opens
                Game1.drawDialogue(npc);                
                monitor.Log($"{which} caught you shoplifting... You were fined {fineamount}g");

                return true;
            }

            return false;
        }

        /// <summary>
        /// Applies shoplifting penalties, tracks whether to ban player
        /// </summary>
        /// <param name="location">The current location instance</param>
        public static void ShopliftingPenalties(GameLocation location)
        {
            // Subtract monetary penalty if it applies
            if (fineamount > 0)
            {
                Game1.player.Money -= fineamount;
            }

            Game1.warpFarmer(location.warps[0].TargetName, location.warps[0].TargetX, location.warps[0].TargetY, false);

            string locationname = location.NameOrUniqueName;

            var data = Game1.player.modData;

            string[] fields = data[$"{manifest.UniqueID}_{locationname}"].Split('/');

            // Add one to first part of data (shoplifting count)
            fields[0] = (int.Parse(fields[0]) + 1).ToString();

            // If this is the first shoplift, record day of month in second part of data (day of first steal)
            if (fields[0] == "1")
            {
                fields[1] = Game1.dayOfMonth.ToString();
            }

            // After being caught three times (within 28 days) ban player from shop for three days
            if (fields[0] == "3")
            {
                fields[0] = "-1";
                ModEntry.PerScreenShopsBannedFrom.Value.Add($"{locationname}");
                monitor.Log($"{locationname} added to banned shop list");
            }

            // Join manipulated data for recording in save file
            data[$"{manifest.UniqueID}_{locationname}"] = string.Join("/", fields);
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
            else if (ModEntry.PerScreenStolenToday.Value == false)
            {
                // Create option to steal
                location.createQuestionDialogue("Shoplift?", location.createYesNoResponses(), delegate (Farmer _, string answer)
                {
                    // Player answered yes
                    if (answer == "Yes")
                    {
                        SeenShoplifting(location, Game1.player);

                        // Player is caught
                        if (ShouldBeCaught("Willy", Game1.player, location) == true)
                        {
                            // After dialogue, apply penalties
                            Game1.afterDialogues = delegate
                            {
                                ShopliftingPenalties(location);
                            };

                            return;
                        }

                        // Not caught, generate stock for shoplifting, on purchase make sure player can't steal again
                        Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(3, 3, "FishShop"), 3, null, delegate
                        {
                            ModEntry.PerScreenStolenToday.Value = true;
                            return false;
                        }, null, "");
                    }
                });                               
            }
            
            // Player can't steal and Willy can't sell
            else
            {
                if (ModEntry.shopliftingstrings.ContainsKey("TheMightyAmondee.Shoplifter/AlreadyShoplifted") == true)
                {
                    Game1.drawObjectDialogue(ModEntry.shopliftingstrings["TheMightyAmondee.Shoplifter/AlreadyShoplifted"]);
                }

                else
                {
                    Game1.drawObjectDialogue(ModEntry.shopliftingstrings["Placeholder"]);
                }
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
                if (ModEntry.PerScreenStolenToday.Value == false)
                {
                    // Create option to steal
                    location.createQuestionDialogue("Shoplift?", location.createYesNoResponses(), delegate (Farmer _, string answer)
                    {
                        // Player answered yes
                        if (answer == "Yes")
                        {
                            SeenShoplifting(location, Game1.player);

                            // Player is caught
                            if (ShouldBeCaught("Sandy", Game1.player, location) == true)
                            {
                                // After dialogue, apply penalties
                                Game1.afterDialogues = delegate
                                {
                                    ShopliftingPenalties(location);
                                };

                                return;
                            }

                            // Not caught, generate stock for shoplifting, on purchase make sure player can't steal again
                            Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(3, 3, "SandyShop"), 3, null, delegate
                            {
                                ModEntry.PerScreenStolenToday.Value = true;
                                return false;
                            }, null, "");
                        }
                    });
                }

                else
                {
                    if (ModEntry.shopliftingstrings.ContainsKey("TheMightyAmondee.Shoplifter/AlreadyShoplifted") == true)
                    {
                        Game1.drawObjectDialogue(ModEntry.shopliftingstrings["TheMightyAmondee.Shoplifter/AlreadyShoplifted"]);
                    }

                    else
                    {
                        Game1.drawObjectDialogue(ModEntry.shopliftingstrings["Placeholder"]);
                    }
                }
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

            // Pierre is not at shop and on island, player can purchase stock properly or steal
            else if (location.getCharacterFromName("Pierre") == null && Game1.IsVisitingIslandToday("Pierre"))
            {
                Game1.dialogueUp = false;
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:SeedShop_MoneyBox"));
                Game1.afterDialogues = delegate
                {
                    location.createQuestionDialogue("Shoplift?", location.createYesNoResponses(), delegate (Farmer _, string answer)
                    {                       
                        if (answer == "Yes")
                        {
                            SeenShoplifting(location, Game1.player);

                            if (ShouldBeCaught("Pierre", Game1.player, location) == true || ShouldBeCaught("Caroline", Game1.player, location) == true || ShouldBeCaught("Abigail", Game1.player, location) == true)
                            {
                                Game1.afterDialogues = delegate
                                {
                                    ShopliftingPenalties(location);
                                };

                                return;
                            }

                            Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(5, 5, "SeedShop"), 3, null, delegate
                            {
                                ModEntry.PerScreenStolenToday.Value = true;
                                return false;
                            }, null, "");
                        }

                        else
                        {
                            Game1.activeClickableMenu = new ShopMenu((location as SeedShop).shopStock());
                        }
                    });
                };
            }

            // Pierre not at counter, player can steal
            else
            {
                Game1.dialogueUp = false;
                location.createQuestionDialogue("Shoplift?", location.createYesNoResponses(), delegate (Farmer _, string answer)
                {                   
                    if (answer == "Yes")
                    {
                        SeenShoplifting(location, Game1.player);

                        if (ShouldBeCaught("Pierre", Game1.player, location) == true || ShouldBeCaught("Caroline", Game1.player, location) == true || ShouldBeCaught("Abigail", Game1.player, location) == true)
                        {
                            Game1.afterDialogues = delegate
                            {
                                ShopliftingPenalties(location);
                            };

                            return;
                        }

                        Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(5, 5, "SeedShop"), 3, null, delegate
                        {
                            ModEntry.PerScreenStolenToday.Value = true;
                            return false;
                        }, null, "");
                    }
                });
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
                // Player can steal
                if (ModEntry.PerScreenStolenToday.Value == false)
                {
                    // Robin is on island and not at sciencehouse, she can't sell but player can purchase properly if they want
                    if (location.getCharacterFromName("Robin") == null && Game1.IsVisitingIslandToday("Robin"))
                    {
                        // Close any current dialogue boxes
                        Game1.dialogueUp = false;
                        // Show normal dialogue
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_MoneyBox"));
                        // Create question to shoplift after dialogue box is exited
                        Game1.afterDialogues = delegate
                        {
                            // Create question dialogue
                            location.createQuestionDialogue("Shoplift?", location.createYesNoResponses(), delegate (Farmer _, string answer)
                            {
                                // Player answered yes
                                if (answer == "Yes")
                                {
                                    // All NPCs in area lose friendship
                                    SeenShoplifting(location, Game1.player);

                                    // Should player be caught by any shopowner?
                                    if (ShouldBeCaught("Robin", Game1.player, location) == true || ShouldBeCaught("Demetrius", Game1.player, location) == true || ShouldBeCaught("Maru", Game1.player, location) == true || ShouldBeCaught("Sebastian", Game1.player, location) == true)
                                    {
                                        // Yes, apply penalties after dialogue
                                        Game1.afterDialogues = delegate
                                        {
                                            ShopliftingPenalties(location);
                                        };

                                        // Leave method early
                                        return;
                                    }

                                    // Not caught, show shoplifting menu
                                    Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(2, 20, "Carpenters"), 3, null, delegate
                                    {
                                        // On purchase, make sure player can not steal again
                                        ModEntry.PerScreenStolenToday.Value = true;
                                        return false;
                                    }, null, "");
                                }

                                // Player answered no, bring up normal shop menu
                                else
                                {
                                    Game1.activeClickableMenu = new ShopMenu(Utility.getCarpenterStock());
                                }
                            });
                        };
                    }

                    // Robin is absent and can't sell, player can steal
                    else if (Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Tue") && location.carpenters(tileLocation) == true && location.getCharacterFromName("Robin") == null)
                    {
                        Game1.dialogueUp = false;
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_RobinAbsent").Replace('\n', '^'));
                        Game1.afterDialogues = delegate
                        {
                            location.createQuestionDialogue("Shoplift?", location.createYesNoResponses(), delegate (Farmer _, string answer)
                            {
                                if (answer == "Yes")
                                {
                                    SeenShoplifting(location, Game1.player);

                                    if (ShouldBeCaught("Robin", Game1.player, location) == true || ShouldBeCaught("Demetrius", Game1.player, location) == true || ShouldBeCaught("Maru", Game1.player, location) == true || ShouldBeCaught("Sebastian", Game1.player, location) == true)
                                    {
                                        Game1.afterDialogues = delegate
                                        {
                                            ShopliftingPenalties(location);
                                        };

                                        return;
                                    }

                                    Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(2, 20, "Carpenters"), 3, null, delegate
                                    {
                                        ModEntry.PerScreenStolenToday.Value = true;
                                        return false;
                                    }, null, "");
                                }
                            });
                        };

                    }

                    // Robin can't sell. Period
                    else if (location.carpenters(tileLocation) == false)
                    {
                        location.createQuestionDialogue("Shoplift?", location.createYesNoResponses(), delegate (Farmer _, string answer)
                        {
                            if (answer == "Yes")
                            {
                                SeenShoplifting(location, Game1.player);

                                if (ShouldBeCaught("Robin", Game1.player, location) == true || ShouldBeCaught("Demetrius", Game1.player, location) == true || ShouldBeCaught("Maru", Game1.player, location) == true || ShouldBeCaught("Sebastian", Game1.player, location) == true)
                                {
                                    Game1.afterDialogues = delegate
                                    {
                                        ShopliftingPenalties(location);
                                    };

                                    return;
                                }

                                Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(2, 20, "Carpenters"), 3, null, delegate
                                {
                                    ModEntry.PerScreenStolenToday.Value = true;
                                    return false;
                                }, null, "");
                            }
                        });
                    }
                }

                // Robin can sell and player can't steal
                else if (location.carpenters(tileLocation) == true && ModEntry.PerScreenStolenToday.Value == true)
                {
                    return;
                }

                // Robin can't sell and player can't steal
                else
                {
                    if (ModEntry.shopliftingstrings.ContainsKey("TheMightyAmondee.Shoplifter/AlreadyShoplifted") == true)
                    {
                        Game1.drawObjectDialogue(ModEntry.shopliftingstrings["TheMightyAmondee.Shoplifter/AlreadyShoplifted"]);
                    }

                    else
                    {
                        Game1.drawObjectDialogue(ModEntry.shopliftingstrings["Placeholder"]);
                    }                   
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
                // Player can steal
                if (ModEntry.PerScreenStolenToday.Value == false)
                {
                    // Marnie is not in the location, she is on the island
                    if (location.getCharacterFromName("Marnie") == null && Game1.IsVisitingIslandToday("Marnie"))
                    {
                        Game1.dialogueUp = false;
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:AnimalShop_MoneyBox"));
                        Game1.afterDialogues = delegate
                        {
                            location.createQuestionDialogue("Shoplift?", location.createYesNoResponses(), delegate (Farmer _, string answer)
                            {
                                if (answer == "Yes")
                                {
                                    SeenShoplifting(location, Game1.player);

                                    if (ShouldBeCaught("Marnie", Game1.player, location) == true || ShouldBeCaught("Shane", Game1.player, location) == true)
                                    {
                                        Game1.afterDialogues = delegate
                                        {
                                            ShopliftingPenalties(location);
                                        };

                                        return;
                                    }

                                    Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(1, 15, "AnimalShop"), 3, null, delegate
                                    {
                                        ModEntry.PerScreenStolenToday.Value = true;
                                        return false;
                                    }, null, "");
                                }

                                else
                                {
                                    Game1.activeClickableMenu = new ShopMenu(Utility.getAnimalShopStock());
                                }
                            });

                        };
                    }

                    // Marnie is not at the location and is absent for the day
                    else if (Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Tue") && location.animalShop(tileLocation) == true && location.getCharacterFromName("Marnie") == null)
                    {
                        Game1.dialogueUp = false;
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Absent").Replace('\n', '^'));
                        Game1.afterDialogues = delegate
                        {
                            location.createQuestionDialogue("Shoplift?", location.createYesNoResponses(), delegate (Farmer _, string answer)
                            {
                                if (answer == "Yes")
                                {
                                    SeenShoplifting(location, Game1.player);

                                    if (ShouldBeCaught("Marnie", Game1.player, location) == true || ShouldBeCaught("Shane", Game1.player, location) == true)
                                    {
                                        Game1.afterDialogues = delegate
                                        {
                                            ShopliftingPenalties(location);
                                        };

                                        return;
                                    }

                                    Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(1, 15, "AnimalShop"), 3, null, delegate
                                    {
                                        ModEntry.PerScreenStolenToday.Value = true;
                                        return false;
                                    }, null, "");
                                }
                            });
                        };
                    }

                    // Marnie can't sell. Period.
                    else if (location.animalShop(tileLocation) == false)
                    {
                        location.createQuestionDialogue("Shoplift?", location.createYesNoResponses(), delegate (Farmer _, string answer)
                        {
                            if (answer == "Yes")
                            {
                                SeenShoplifting(location, Game1.player);

                                if (ShouldBeCaught("Marnie", Game1.player, location) == true || ShouldBeCaught("Shane", Game1.player, location) == true)
                                {
                                    Game1.afterDialogues = delegate
                                    {
                                        ShopliftingPenalties(location);
                                    };

                                    return;
                                }

                                Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(1, 15, "AnimalShop"), 3, null, delegate
                                {
                                    ModEntry.PerScreenStolenToday.Value = true;
                                    return false;
                                }, null, "");
                            }
                        });
                    }
                }

                // Marnie can sell and player can't steal
                else if (location.animalShop(tileLocation) == true && ModEntry.PerScreenStolenToday.Value == true)
                {
                    return;
                }

                else
                {
                    if (ModEntry.shopliftingstrings.ContainsKey("TheMightyAmondee.Shoplifter/AlreadyShoplifted") == true)
                    {
                        Game1.drawObjectDialogue(ModEntry.shopliftingstrings["TheMightyAmondee.Shoplifter/AlreadyShoplifted"]);
                    }
                    else
                    {
                        Game1.drawObjectDialogue(ModEntry.shopliftingstrings["Placeholder"]);
                    }
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
                if (ModEntry.PerScreenStolenToday.Value == false)
                {
                    location.createQuestionDialogue("Shoplift?", location.createYesNoResponses(), delegate (Farmer _, string answer)
                    {                       
                        if (answer == "Yes")
                        {
                            SeenShoplifting(location, Game1.player);

                            if (ShouldBeCaught("Harvey", Game1.player, location) == true || ShouldBeCaught("Maru", Game1.player, location) == true)
                            {
                                Game1.afterDialogues = delegate
                                {
                                    ShopliftingPenalties(location);
                                };

                                return;
                            }

                            Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(1, 3, "HospitalShop"), 3, null, delegate
                            {
                                ModEntry.PerScreenStolenToday.Value = true;
                                return false;
                            }, null, "");
                        }
                    });
                }

                else
                {
                    if (ModEntry.shopliftingstrings.ContainsKey("TheMightyAmondee.Shoplifter/AlreadyShoplifted") == true)
                    {
                        Game1.drawObjectDialogue(ModEntry.shopliftingstrings["TheMightyAmondee.Shoplifter/AlreadyShoplifted"]);
                    }

                    else
                    {
                        Game1.drawObjectDialogue(ModEntry.shopliftingstrings["Placeholder"]);
                    }
                }
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
                if (ModEntry.PerScreenStolenToday.Value == false)
                {
                    location.createQuestionDialogue("Shoplift?", location.createYesNoResponses(), delegate (Farmer _, string answer)
                    {
                        if (answer == "Yes")
                        {
                            SeenShoplifting(location, Game1.player);

                            if (ShouldBeCaught("Clint", Game1.player, location) == true)
                            {
                                Game1.afterDialogues = delegate
                                {
                                    ShopliftingPenalties(location);
                                };

                                return;
                            }

                            Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(3, 10, "Blacksmith"), 3, null, delegate
                            {
                                ModEntry.PerScreenStolenToday.Value = true;
                                return false;
                            }, null, "");
                        }
                    });
                }

                else
                {
                    if (ModEntry.shopliftingstrings.ContainsKey("TheMightyAmondee.Shoplifter/AlreadyShoplifted") == true)
                    {
                        Game1.drawObjectDialogue(ModEntry.shopliftingstrings["TheMightyAmondee.Shoplifter/AlreadyShoplifted"]);
                    }

                    else
                    {
                        Game1.drawObjectDialogue(ModEntry.shopliftingstrings["Placeholder"]);
                    }
                }

            }
        }

        /// <summary>
        /// Create the shoplifting menu with Saloon stock if necessary
        /// </summary>
        /// <param name="location">The current location instance</param>
        public static void SaloonShopliftingMenu(GameLocation location, Location tilelocation)
        {
            // Gus is not in the location, he is on the island
            if (location.getCharacterFromName("Gus") == null && Game1.IsVisitingIslandToday("Gus"))
            {
                if (ModEntry.PerScreenStolenToday.Value == false)
                {
                    Game1.dialogueUp = false;
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Saloon_MoneyBox"));
                    Game1.afterDialogues = delegate
                    {
                        location.createQuestionDialogue("Shoplift?", location.createYesNoResponses(), delegate (Farmer _, string answer)
                        {                            
                            if (answer == "Yes")
                            {
                                SeenShoplifting(location, Game1.player);

                                if (ShouldBeCaught("Gus", Game1.player, location) == true || ShouldBeCaught("Emily", Game1.player, location) == true)
                                {
                                    Game1.afterDialogues = delegate
                                    {
                                        ShopliftingPenalties(location);
                                    };

                                    return;
                                }

                                Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(2, 1, "Saloon"), 3, null, delegate
                                {
                                    ModEntry.PerScreenStolenToday.Value = true;
                                    return false;
                                }, null, "");
                            }

                            else
                            {
                                Game1.activeClickableMenu = new ShopMenu(Utility.getSaloonStock(), 0, null, delegate (ISalable item, Farmer farmer, int amount)
                                {
                                    Game1.player.team.synchronizedShopStock.OnItemPurchased(SynchronizedShopStock.SynchedShop.Saloon, item, amount);
                                    return false;
                                });
                            }
                        });
                    };
                }

                // Gus can sell, player can't steal
                else if (location.saloon(tilelocation) == true && ModEntry.PerScreenStolenToday.Value == true)
                {
                    return;
                }

                else
                {
                    if (ModEntry.shopliftingstrings.ContainsKey("TheMightyAmondee.Shoplifter/AlreadyShoplifted") == true)
                    {
                        Game1.drawObjectDialogue(ModEntry.shopliftingstrings["TheMightyAmondee.Shoplifter/AlreadyShoplifted"]);
                    }

                    else
                    {
                        Game1.drawObjectDialogue(ModEntry.shopliftingstrings["Placeholder"]);
                    }
                }
                
                return;
            }

            // Gus can't sell. Period.
            else if (location.saloon(tilelocation) == false)
            {
                if (ModEntry.PerScreenStolenToday.Value == false)
                {
                    location.createQuestionDialogue("Shoplift?", location.createYesNoResponses(), delegate (Farmer _, string answer)
                    {                       
                        if (answer == "Yes")
                        {
                            SeenShoplifting(location, Game1.player);

                            if (ShouldBeCaught("Gus", Game1.player, location) == true || ShouldBeCaught("Emily", Game1.player, location) == true)
                            {
                                Game1.afterDialogues = delegate
                                {
                                    ShopliftingPenalties(location);
                                };

                                return;
                            }

                            Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(2, 1, "Saloon"), 3, null, delegate
                            {
                                ModEntry.PerScreenStolenToday.Value = true;
                                return false;
                            }, null, "");
                        }
                    });
                }

                // Gus can't sell, player can't steal
                else
                {
                    if (ModEntry.shopliftingstrings.ContainsKey("TheMightyAmondee.Shoplifter/AlreadyShoplifted") == true)
                    {
                        Game1.drawObjectDialogue(ModEntry.shopliftingstrings["TheMightyAmondee.Shoplifter/AlreadyShoplifted"]);
                    }

                    else
                    {
                        Game1.drawObjectDialogue(ModEntry.shopliftingstrings["Placeholder"]);
                    }
                }
            }
        }
    }
}
