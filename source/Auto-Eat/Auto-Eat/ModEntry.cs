/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Permamiss/Auto-Eat
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace AutoEat
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Private and public variables
        *********/

        private static bool trueOverexertion = false; //is only set to true when we want the player to become over-exerted for the rest of the in-game day
        private static bool newDay = true; //only true at 6:00 am in-game
        private static bool goodPreviousFrame = false; //used to prevent loss of food when falling to 0 Stamina on the same frame that you receive a Lost Book or something similar, in that order.
        private static bool eatingFood = false; //just a boolean used to make it so that code doesn't run more than once.

        public static bool firstCall = false; //used in clearOldestHUDMessage()
        public static float eatAtAmount;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModConfig config = helper.ReadConfig<ModConfig>();
            eatAtAmount = config.StaminaThreshold;

            if (eatAtAmount < 0)
            {
                eatAtAmount = config.StaminaThreshold = 0;
                helper.WriteConfig(config);
            }

            helper.ConsoleCommands.Add("player_setstaminathreshold", "Sets the threshold at which the player will automatically consume food.\nUsage: player_setstaminathreshold <value>\n- value: the float/integer amount.", this.SetStaminaThreshold); //command that sets when to automatically eat (i.e. 25 energy instead of 0)
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked; //adding the method with the same name below to the corresponding event in order to make them connect
            helper.Events.GameLoop.Saving += this.OnSaving;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        }

        public static void ClearOldestHUDMessage() //I may have stolen this idea from CJBok (props to them)
        {
            firstCall = false; //we do this so that, as long as we check for firstCall to be true, this method will not be executed every single tick (if we did not do this, a message would be removed from the HUD every tick!)
            if (Game1.hudMessages.Count > 0) //if there is at least 1 message on the screen, then
                Game1.hudMessages.RemoveAt(Game1.hudMessages.Count - 1); //remove the oldest one (useful in case multiple messages are on the screen at once)
        }


        /*********
        ** Private methods
        *********/
        private void SetStaminaThreshold(string command, string[] args)
        {
            float newValue = (float)double.Parse(args[0]);

            if (newValue < 0.0f || newValue >= Game1.player.MaxStamina) //don't allow the stamina threshold to be set outside the possible bounds
                newValue = 0.0f;

            eatAtAmount = newValue;
            ModConfig newConfig = new ModConfig()
            {
                StaminaThreshold = newValue
            };
            this.Helper.WriteConfig(newConfig);

            this.Monitor.Log($"OK, set the stamina threshold to {newValue}.");
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsPlayerFree || trueOverexertion || newDay) //are they paused/in a menu, over-exerted, or it's the beginning of the day, then do not continue
            {
                goodPreviousFrame = false;
                return;
            }
            if (Game1.player.Stamina <= eatAtAmount) //if the player has run out of Energy, then:
            {
                if (!goodPreviousFrame) //makes it so that they have to be "good" (doing nothing, not in a menu) two frames in a row in order for this to pass - necessary thanks to Lost Book bug (tl;dr - wait a frame before continuing)
                {
                    goodPreviousFrame = true;
                    return;
                }
                if (firstCall) //if clearOldestHUDMessage has not been called yet, then
                    ClearOldestHUDMessage(); //get rid of the annoying over-exerted message without it noticeably popping up
                if (eatingFood || Game1.player.isEating) //if already eating food, then ignore the rest of the method in order to prevent unnecessary loop
                    return;
                Item cheapestFood = GetCheapestFood(); //currently set to "null" (aka none), as we have not found a food yet
                if (cheapestFood != null) //if a cheapest food was found, then:
                {
                    eatingFood = true;
                    Game1.showGlobalMessage("You consume " + cheapestFood.Name + " to avoid over-exertion."); //makes a message to inform the player of the reason they just stopped what they were doing to be forced to eat a food, lol.
                    Game1.player.eatObject((StardewValley.Object)cheapestFood); //cast the cheapestFood Item to be an Object since playerEatObject only accepts Objects, finally allowing the player to eat the cheapest food they have on them.
                    //Game1.playerEatObject((StardewValley.Object)cheapestFood); //<== pre-multiplayer beta version of above line of code.
                    cheapestFood.Stack--; //stack being the amount of the cheapestFood that the player has on them, we have to manually decrement this apparently, as playerEatObject does not do this itself for some reason.
                    if (cheapestFood.Stack == 0) //if the stack has hit the number 0, then
                        Game1.player.removeItemFromInventory(cheapestFood); //delete the item from the player's inventory..I don't want to know what would happen if they tried to use it when it was at 0!
                }
                else if (Game1.player.stamina <= 0.0f) //however, if no food was found and the player's stamina is at 0, then [shoutouts to RobertLSnead again for pointing out some flawed code here]
                    trueOverexertion = true; //the player will be over-exerted for the rest of the day, just like they normally would be. I made it this way intentionally, in order to keep this mod balanced!
            }
            else //if they have Energy (whether it's gained from food or it's the start of a day or whatever), then:
            {
                goodPreviousFrame = false;
                firstCall = true; //we set this to true here so that "clearOldestHUDMessage()" can seamlessly remove the "over-exerted" message whenever it needs to
                if (eatingFood) //if the player was eating food before, then:
                {
                    eatingFood = false; //they are no longer eating, meaning the above checks will be performed once more if they hit 0 Energy again.
                    //Game1.player.exhausted = false; //old way of doing it
                    Game1.player.exhausted.Value = false; //forcing the game to make the player not over-exerted anymore since that's what this mod's goal was
                    Game1.player.checkForExhaustion(Game1.player.Stamina); //forcing the game to make the player not over-exerted anymore since that's what this mod's goal was
                }
            }
        }

        //will return null if no item found; shoutouts to RobertLSnead
        private Item GetCheapestFood()
        {
            Item cheapestFood = null; //currently set to "null" (aka none), as we have not found a food yet
            foreach (Item curItem in Game1.player.Items) //check all of the player's inventory items sequentially (with "curItem" meaning "current item") for the following:
            {
                if (curItem is StardewValley.Object && ((StardewValley.Object)curItem).Edibility > 0) //is it an Object (rather than, say, a Tool), and is it a food with positive Edibility (aka Energy)? then,
                {
                    if (cheapestFood == null) //if we do not yet have a cheapest food set, then
                        cheapestFood = curItem; //the cheapest food has to be the current item, so that we can compare its price to another item without getting errors
                    else if ((curItem.salePrice() / ((StardewValley.Object)curItem).Edibility) < (cheapestFood.salePrice() / ((StardewValley.Object)cheapestFood).Edibility)) //however, if we already have a cheapest food, and the ratio of price-to-stamina of the current item is even less, then
                        cheapestFood = curItem; //the food with the least price-to-stamina ratio is actually the current item!
                }
            }
            return cheapestFood;
        }

        /// <summary>Raised before the game begins writes data to the save file (except the initial save creation).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaving(object sender, SavingEventArgs e)
        {
            newDay = true;
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            newDay = false; //reset the variable, allowing the UpdateTick method checks to occur once more
            trueOverexertion = false; //reset the variable, allowing the UpdateTick method checks to occur once more (in other words, allowing the player to avoid over-exertion once more)
            eatingFood = false; //reset the variable (this one isn't necessary as far as I know, but who knows? maybe a person will run out of stamina right as they hit 2:00 am in-game.)
        }
    }
}
