using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace AutoEat
{
    class ModConfig
    {
        public float StaminaThreshold { get; set; } = 0.0f;
    }
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Private and public variables
        *********/

        private static bool trueOverexertion = false; //is only set to true when we want the player to become over-exerted for the rest of the in-game day
        private static bool newDay = true; //only true at 6:00 am in-game; used to 
        private static bool eatingFood = false;
        private static bool goodPreviousFrame = false; //used to prevent loss of food when falling to 0 Stamina on the same frame that you receive a Lost Book or something similar, in that order.

        public static bool firstCall = false; //used in clearOldestHUDMessage()
        public static float eatAtAmount;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>

        public override void Entry(IModHelper helper)
        {
            ModConfig theConfig = helper.ReadConfig<ModConfig>();
            eatAtAmount = theConfig.StaminaThreshold;

            if (eatAtAmount < 0.0f)
            {
                eatAtAmount = 0.0f;
                ModConfig fixConfig = new ModConfig();
                fixConfig.StaminaThreshold = 0.0f;
                helper.WriteConfig(fixConfig);
            }

            helper.ConsoleCommands.Add("player_setstaminathreshold", "Sets the threshold at which the player will automatically consume food.\nUsage: player_setstaminathreshold <value>\n- value: the float/integer amount.", this.SetStaminaThreshold); //command that sets when to automatically eat (i.e. 25 energy instead of 0)
            GameEvents.UpdateTick += this.GameEvents_UpdateTick; //adding the method with the same name below to the corresponding event in order to make them connect
            SaveEvents.BeforeSave += this.SaveEvents_BeforeSave;
            TimeEvents.AfterDayStarted += this.TimeEvents_AfterDayStarted;
        }

        public static void clearOldestHUDMessage() //I may have stolen this idea from CJBok (props to them)
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
            float newValue = (float)Double.Parse(args[0]);

            if (newValue < 0.0f || newValue >= Game1.player.MaxStamina) //don't allow the stamina threshold to be set outside the possible bounds
                newValue = 0.0f;

            eatAtAmount = newValue;
            ModConfig newConfig = new ModConfig();
            newConfig.StaminaThreshold = newValue;

            this.Helper.WriteConfig(newConfig);

            this.Monitor.Log($"OK, set the stamina threshold to {newValue}.");
        }
        
        /// <summary>The method invoked when the player presses a keyboard button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (!Context.IsPlayerFree) //are they paused or in a menu or something? then do not continue
            {
                goodPreviousFrame = false;
                return;
            }
            if (trueOverexertion || newDay) //if already over-exerted, or if it's the beginning of the day, then do not continue
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
                    clearOldestHUDMessage(); //get rid of the annoying over-exerted message without it noticeably popping up
                if (eatingFood) //if already eating food, then ignore the rest of the method in order to prevent unnecessary loop
                    return;
                Item cheapestFood = null; //currently set to "null" (aka none), as we have not found a food yet
                foreach (Item curItem in Game1.player.Items) //check all of the player's inventory items sequentially (with "curItem" meaning "current item") for the following:
                {
                    if (curItem is StardewValley.Object && ((StardewValley.Object)curItem).Edibility > 0) //is it an Object (rather than, say, a Tool), and is it a food with positive Edibility (aka Energy)? then,
                    {
                        if (cheapestFood == null) //if we do not yet have a cheapest food set, then
                            cheapestFood = curItem; //the cheapest food has to be the current item, so that we can compare its price to another item without getting errors
                        else if ((curItem.salePrice() / ((StardewValley.Object)curItem).Edibility) < (cheapestFood.salePrice() / ((StardewValley.Object)cheapestFood).Edibility)) //however, if we already have a cheapest food, and the price of the current item is even less, then
                            cheapestFood = curItem; //the cheapest food we have is actually the current item!
                    }
                }
                if (cheapestFood != null) //if a cheapest food was found, then:
                {
                    eatingFood = true; //set to true in order to prevent all of this from repeating, as this might cause unwanted side effects (possibly eating multiple foods at once???)
                    Game1.showGlobalMessage("You consume " + cheapestFood.Name + " to avoid over-exertion."); //makes a message to inform the player of the reason they just stopped what they were doing to be forced to eat a food, lol.
                    Game1.player.eatObject((StardewValley.Object)cheapestFood); //cast the cheapestFood Item to be an Object since playerEatObject only accepts Objects, finally allowing the player to eat the cheapest food they have on them.
                    //Game1.playerEatObject((StardewValley.Object)cheapestFood); //<== pre-multiplayer beta version of above line of code.
                    cheapestFood.Stack--; //stack being the amount of the cheapestFood that the player has on them, we have to manually decrement this apparently, as playerEatObject does not do this itself for some reason.
                    if (cheapestFood.Stack == 0) //if the stack has hit the number 0, then
                        Game1.player.removeItemFromInventory(cheapestFood); //delete the item from the player's inventory..I don't want to know what would happen if they tried to use it when it was at 0!
                }
                else //however, if no food was found, then
                    trueOverexertion = true; //the player will be over-exerted for the rest of the day, just like they normally would be.
            }
            else //if they have Energy (whether it's gained from food or it's the start of a day or whatever), then:
            {
                goodPreviousFrame = false;
                firstCall = true; //we set this to true here so that "clearOldestHUDMessage()" can seamlessly remove the "over-exerted" message whenever it needs to
                if (eatingFood) //if the player was eating food before, then:
                {
                    eatingFood = false; //they are no longer eating, meaning the above checks will be performed once more if they hit 0 Energy again.
                    Game1.player.exhausted = false; //forcing the game to make the player not over-exerted anymore since that's what this mod's goal was
                    Game1.player.checkForExhaustion(Game1.player.Stamina); //forcing the game to make the player not over-exerted anymore since that's what this mod's goal was
                }
            }
        }

        private void SaveEvents_BeforeSave(object sender, EventArgs e)
        {
            newDay = true;
        }

        private void TimeEvents_AfterDayStarted(object sender, EventArgs e) //fires just as player wakes up
        {
            newDay = false; //reset the variable, allowing the UpdateTick method checks to occur once more
            trueOverexertion = false; //reset the variable, allowing the UpdateTick method checks to occur once more (in other words, allowing the player to avoid over-exertion once more)
            eatingFood = false; //reset the variable (this one isn't necessary as far as I know, but who knows? maybe a person will run out of stamina right as they hit 2:00 am in-game.)
        }
    }
}