using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace priceDrops
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        // Heart levels and corresponding discounts
        public static int HEART_LEVEL_1 = -1, HEART_LEVEL_2 = -1, HEART_LEVEL_3 = -1;
        public static int DISC_1 = -1, DISC_2 = -1, DISC_3 = -1, BONUS_DISC = -1;
        private List<string> CUSTOM_NPCS = new List<string>();

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Read config
            ModConfig config = helper.ReadConfig<ModConfig>();
            HEART_LEVEL_1 = config.heartLevel1;
            HEART_LEVEL_2 = config.heartLevel2;
            HEART_LEVEL_3 = config.heartLevel3;
            DISC_1 = config.disc1;
            DISC_2 = config.disc2;
            DISC_3 = config.disc3;
            BONUS_DISC = config.bonusDisc;
            CUSTOM_NPCS = config.customNPCs;

            // Security checks
            // Make sure that hearts are between 0 and 10, if not, assume standard values
            if (HEART_LEVEL_1 < 0 || HEART_LEVEL_1 > 10)
                HEART_LEVEL_1 = 3;
            if (HEART_LEVEL_2 < 0 || HEART_LEVEL_2 > 10)
                HEART_LEVEL_2 = 7;
            if (HEART_LEVEL_3 < 0 || HEART_LEVEL_3 > 10)
                HEART_LEVEL_3 = 10;

            // If relations between hearts are funky
            if(HEART_LEVEL_2 < HEART_LEVEL_1 || HEART_LEVEL_3 < HEART_LEVEL_2 || HEART_LEVEL_3 < HEART_LEVEL_1)
            {
                HEART_LEVEL_1 = 3;
                HEART_LEVEL_2 = 7;
                HEART_LEVEL_3 = 10;
            }

            // if discounts exceed 99% (100% crashes calculations for Krobus)
            if (DISC_1 > 99)
                DISC_1 = 10;
            if (DISC_2 > 99)
                DISC_2 = 25;
            if (DISC_3 > 99)
                DISC_3 = 50;
            if (BONUS_DISC > 99)
                BONUS_DISC = 5;

            // Asset editors for Robin's prices and mail
            helper.Content.AssetEditors.Add(new robinEditor());
            helper.Content.AssetEditors.Add(new mailEditor(helper.Translation));
            
            // When menus change (ie. a shop window is opened), go do the magic
            helper.Events.Display.MenuChanged += this.OnMenuChanged;

            // TODO: Find better time to do this           
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.Saving += this.OnSaving;
        }

        /// <summary>Raised before the game begins writes data to the save file (except the initial save creation).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        public void OnSaving(object sender, SavingEventArgs e)
        {
            // Check for new mail before sleeping
            // If we only checked for new mail before sleeping, players who install this mod mid-game would have a one day delay in receiving already existing discounts
            this.Postman();
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        public void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // Check for new mail after new day has started
            // If we only checked for new mail after the day has started, players would have a one day delay in receiving mail when NPC disposition changes, so we do both           
            Postman();
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        public void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            // Force a reload of Robin's prices
            Helper.Content.InvalidateCache("Data\\Blueprints.xnb");

            // Marnie's animal shop menu
            if (e.NewMenu is PurchaseAnimalsMenu animalsMenu)
            {               
                //this.Monitor.Log($"Animals are up.");

                // Get all animals in the menu
                List<ClickableTextureComponent> entries = animalsMenu.animalsToPurchase;

                // Get player's relationship with Marnie and set discount accordingly
                int hearts = Game1.player.getFriendshipHeartLevelForNPC("Marnie");

                //this.Monitor.Log($"Player has " + hearts + " hearts with Marnie.");

                int percentage = 0;

                if (hearts >= HEART_LEVEL_3)
                    percentage = DISC_3;
                else if (hearts >= HEART_LEVEL_2)
                    percentage = DISC_2;
                else if (hearts >= HEART_LEVEL_1)
                    percentage = DISC_1;

                //this.Monitor.Log($"Player gets " + percentage + "% off.");

                // Now update prices
                // Get through the animals and update their respective prices
                for (int i = 0; i < entries.Count; i++)
                {
                    StardewValley.Object currentAnimal = (StardewValley.Object)entries[i].item;

                    //this.Monitor.Log($"Animal " + i + " costs " + currentAnimal.price);

                    // Bonus discount on chickens if the player has seen the blue chicken event with Shane
                    if (Game1.player.eventsSeen.Contains(3900074) && i == 0)
                        currentAnimal.Price = getPercentage(currentAnimal.Price, (percentage + BONUS_DISC));
                    else
                        currentAnimal.Price = getPercentage(currentAnimal.Price, percentage);

                    //this.Monitor.Log($"Now it costs " + currentAnimal.price);                    
                }             
            }

            // Regular shops
            applyShopDiscounts("Marnie");
            applyShopDiscounts("Pierre");
            applyShopDiscounts("Robin");
            applyShopDiscounts("Harvey");
            applyShopDiscounts("Gus");
            applyShopDiscounts("Clint");
            applyShopDiscounts("Sandy");
            applyShopDiscounts("Willy");            
            applyShopDiscounts("Dwarf");
            applyShopDiscounts("Krobus");            

            // Custom NPCs
            foreach (string name in CUSTOM_NPCS)
            {
                if(!name.StartsWith("placeHolder"))
                    applyShopDiscounts(name);
            }
        }

        /* ***************************************************************************************************************************** */

        // Returns a percentage of a base value
        public static int getPercentage(double val, double perc)
        {

            double subst = (val / 100) * perc;
            int result = (int)val - (int)subst;

            return result;
        }
        
        private void applyShopDiscounts(string characterName)
        {
            if (Game1.activeClickableMenu is ShopMenu shopMenu /*&& shopMenu.portraitPerson == Game1.getCharacterFromName(characterName, true)*/)
            {         
                // Get player's relationship with character and set discount accordingly
                int hearts = Game1.player.getFriendshipHeartLevelForNPC(characterName);

                int percentage = 0;

                if (hearts >= HEART_LEVEL_3)
                    percentage = DISC_3;
                else if (hearts >= HEART_LEVEL_2)
                    percentage = DISC_2;
                else if (hearts >= HEART_LEVEL_1)
                    percentage = DISC_1;

                // Special additional discounts                
                if (characterName.Equals("Robin"))
                {
                    // If player is married to Maru or Sebastian, give additional 5% discount
                    if (Game1.getCharacterFromName("Maru", true).isMarried() || Game1.getCharacterFromName("Sebastian", true).isMarried())
                    {
                        percentage += BONUS_DISC;
                    }
                }
                if(characterName.Equals("Pierre"))
                {
                    // If player is married to Abigail, give 5%. If he's also best friends with Caroline, give an additional 5%
                    if (Game1.getCharacterFromName("Abigail", true).isMarried())
                        percentage += BONUS_DISC;
                    if (Game1.player.getFriendshipHeartLevelForNPC("Caroline") == 10)
                        percentage += BONUS_DISC;
                }

                // Prices are in the itemPriceAndStock dictionary. The first number of the int[] is the price. Second is stock probably?
                Dictionary<Item, int[]> priceAndStock = this.Helper.Reflection.GetField<Dictionary<Item, int[]>>(shopMenu, "itemPriceAndStock").GetValue();
                bool isHarvey = false;

                if (characterName.Equals("Harvey"))
                {
                    // If player is married to Harvey, give an additional 5%
                    if (Game1.getCharacterFromName("Harvey", true).isMarried())
                        percentage += BONUS_DISC;

                    // Determine if shop is Harvey's clinic
                    foreach (KeyValuePair<Item, int[]> kvp in priceAndStock)
                    {
                        //this.Monitor.Log("Are we in Harvey's shop?");

                        if (kvp.Key.Name.Equals("Muscle Remedy")) { 
                            isHarvey = true;
                            //this.Monitor.Log("Yup");
                        }
                    }
                }


                if (shopMenu.portraitPerson == Game1.getCharacterFromName(characterName, true) || (characterName.Equals("Harvey") && isHarvey)) { 

                    //this.Monitor.Log($"Player has " + hearts + " hearts with " + characterName+" and receives "+percentage+"% off.");

                    // Change supply shop prices here
                    foreach (KeyValuePair<Item, int[]> kvp in priceAndStock)
                    {
                        //this.Monitor.Log($"Old price: " + kvp.Value.GetValue(0));
                        kvp.Value.SetValue(getPercentage(double.Parse(kvp.Value.GetValue(0).ToString()), percentage), 0);
                        //this.Monitor.Log($"New price: " + kvp.Value.GetValue(0));                        
                    }

                }
            }
        }

        // Checks if any new mail has to be sent tomorrow
        private void Postman()
        {
            // Check if any mail has to be sent
            if (Game1.player.getFriendshipHeartLevelForNPC("Robin") >= HEART_LEVEL_1 && DISC_1 > 0)
            {
                // First mail from Robin                
                Game1.addMailForTomorrow("robin1");
                //Monitor.Log("First Robin letter received ... ");
            }
            if (Game1.player.getFriendshipHeartLevelForNPC("Robin") >= HEART_LEVEL_2 && DISC_2 > 0)
            {
                // Second mail from Robin                
                Game1.addMailForTomorrow("robin2");
                //Monitor.Log("Second Robin letter received ... " + robinMail2);
            }
            if (Game1.player.getFriendshipHeartLevelForNPC("Robin") >= HEART_LEVEL_3 && DISC_3 > 0)
            {
                // Third mail from Robin                
                Game1.addMailForTomorrow("robin3");
                // Monitor.Log("Third Robin letter received ... " + robinMail3);
            }
            if (Game1.getCharacterFromName("Maru", true).isMarried() && BONUS_DISC > 0)
            {
                //MailDao.SaveLetter(robinMaruLetter);
                Game1.addMailForTomorrow("robinMaru");
            }
            if (Game1.getCharacterFromName("Sebastian", true).isMarried() && BONUS_DISC > 0)
            {                
                Game1.addMailForTomorrow("robinSebastian");
            }


            if (Game1.player.getFriendshipHeartLevelForNPC("Marnie") >= HEART_LEVEL_1 && DISC_1 > 0)
            {                
                Game1.addMailForTomorrow("marnie1");
                //Monitor.Log("First Marnie letter in queue... ");
            }
            if (Game1.player.getFriendshipHeartLevelForNPC("Marnie") >= HEART_LEVEL_2 && DISC_2 > 0)
            {
                Game1.addMailForTomorrow("marnie2");
            }
            if (Game1.player.getFriendshipHeartLevelForNPC("Marnie") >= HEART_LEVEL_3 && DISC_3 > 0)
            {
                Game1.addMailForTomorrow("marnie3");
            }
            // If the player has seen the blue chicken event
            if (Game1.player.eventsSeen.Contains(3900074) && BONUS_DISC > 0)
            {
                //this.Monitor.Log("Player has seen the blue chickens.");
                Game1.addMailForTomorrow("marnieShane");
            }

            if (Game1.player.getFriendshipHeartLevelForNPC("Pierre") >= HEART_LEVEL_1 && DISC_1 > 0)
            {                
                Game1.addMailForTomorrow("pierre1");
            }
            if (Game1.player.getFriendshipHeartLevelForNPC("Pierre") >= HEART_LEVEL_2 && DISC_2 > 0)
            {
                Game1.addMailForTomorrow("pierre2");
            }
            if (Game1.player.getFriendshipHeartLevelForNPC("Pierre") >= HEART_LEVEL_3 && DISC_3 > 0)
            {
                Game1.addMailForTomorrow("pierre3");
            }
            if (Game1.getCharacterFromName("Abigail", true).isMarried() && BONUS_DISC > 0)
            {
                Game1.addMailForTomorrow("pierreAbigail");
            }
            if (Game1.player.getFriendshipHeartLevelForNPC("Caroline") == 10 && BONUS_DISC > 0)
            {
                Game1.addMailForTomorrow("pierreCaroline");
            }

            if (Game1.player.getFriendshipHeartLevelForNPC("Harvey") >= HEART_LEVEL_1 && DISC_1 > 0)
            {
                Game1.addMailForTomorrow("harvey1");
            }
            if (Game1.player.getFriendshipHeartLevelForNPC("Harvey") >= HEART_LEVEL_2 && DISC_2 > 0)
            {
                Game1.addMailForTomorrow("harvey2");
            }
            if (Game1.player.getFriendshipHeartLevelForNPC("Harvey") >= HEART_LEVEL_3 && DISC_3 > 0)
            {
                Game1.addMailForTomorrow("harvey3");
            }
            if (Game1.getCharacterFromName("Harvey", true).isMarried() && BONUS_DISC > 0)
            {
                Game1.addMailForTomorrow("harveyMarried");
            }

            if (Game1.player.getFriendshipHeartLevelForNPC("Gus") >= HEART_LEVEL_1 && DISC_1 > 0)
            {
                Game1.addMailForTomorrow("gus1");
            }
            if (Game1.player.getFriendshipHeartLevelForNPC("Gus") >= HEART_LEVEL_2 && DISC_2 > 0)
            {
                Game1.addMailForTomorrow("gus2");
            }
            if (Game1.player.getFriendshipHeartLevelForNPC("Gus") >= HEART_LEVEL_3 && DISC_3 > 0)
            {
                Game1.addMailForTomorrow("gus3");
            }

            if (Game1.player.getFriendshipHeartLevelForNPC("Clint") >= HEART_LEVEL_1 && DISC_1 > 0)
            {
                Game1.addMailForTomorrow("clint1");
            }
            if (Game1.player.getFriendshipHeartLevelForNPC("Clint") >= HEART_LEVEL_2 && Game1.player.eventsSeen.Contains(97) && DISC_2 > 0)
            {
                Game1.addMailForTomorrow("clint2");
            }
            if (Game1.player.getFriendshipHeartLevelForNPC("Clint") >= HEART_LEVEL_3 && DISC_3 > 0)
            {
                Game1.addMailForTomorrow("clint3");
            }

            if (Game1.player.getFriendshipHeartLevelForNPC("Sandy") >= HEART_LEVEL_1 && DISC_1 > 0)
            {
                Game1.addMailForTomorrow("sandy1");
            }
            if (Game1.player.getFriendshipHeartLevelForNPC("Sandy") >= HEART_LEVEL_2 && DISC_2 > 0)
            {
                Game1.addMailForTomorrow("sandy2");
            }
            if (Game1.player.getFriendshipHeartLevelForNPC("Sandy") >= HEART_LEVEL_3 && DISC_3 > 0)
            {
                Game1.addMailForTomorrow("sandy3");
            }

            if (Game1.player.getFriendshipHeartLevelForNPC("Willy") >= HEART_LEVEL_1 && DISC_1 > 0)
            {
                Game1.addMailForTomorrow("willy1");
            }
            if (Game1.player.getFriendshipHeartLevelForNPC("Willy") >= HEART_LEVEL_2 && DISC_2 > 0)
            {
                Game1.addMailForTomorrow("willy2");
            }
            if (Game1.player.getFriendshipHeartLevelForNPC("Willy") >= HEART_LEVEL_3 && DISC_3 > 0)
            {
                Game1.addMailForTomorrow("willy3");
            }

            if (Game1.player.getFriendshipHeartLevelForNPC("Dwarf") >= HEART_LEVEL_1 && DISC_1 > 0)
            {
                Game1.addMailForTomorrow("dwarf1");
            }
            if (Game1.player.getFriendshipHeartLevelForNPC("Dwarf") >= HEART_LEVEL_2 && DISC_2 > 0)
            {
                Game1.addMailForTomorrow("dwarf2");
            }
            if (Game1.player.getFriendshipHeartLevelForNPC("Dwarf") >= HEART_LEVEL_3 && DISC_3 > 0)
            {
                Game1.addMailForTomorrow("dwarf3");
            }

            if (Game1.player.getFriendshipHeartLevelForNPC("Krobus") >= HEART_LEVEL_1 && DISC_1 > 0)
            {
                Game1.addMailForTomorrow("krobus1");
            }
            if (Game1.player.getFriendshipHeartLevelForNPC("Krobus") >= HEART_LEVEL_2 && DISC_2 > 0)
            {
                Game1.addMailForTomorrow("krobus2");
            }
            if (Game1.player.getFriendshipHeartLevelForNPC("Krobus") >= HEART_LEVEL_3 && DISC_3 > 0)
            {
                Game1.addMailForTomorrow("krobus3");
            }

            if (Game1.player.getFriendshipHeartLevelForNPC("Wizard") >= HEART_LEVEL_1 && DISC_1 > 0)
            {
                Game1.addMailForTomorrow("wizard1");
            }
            if (Game1.player.getFriendshipHeartLevelForNPC("Wizard") >= HEART_LEVEL_2 && DISC_2 > 0)
            {
                Game1.addMailForTomorrow("wizard2");
            }
            if (Game1.player.getFriendshipHeartLevelForNPC("Wizard") >= HEART_LEVEL_3 && DISC_3 > 0)
            {
                Game1.addMailForTomorrow("wizard3");
            }
        }
    }    
}