using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Tools;
using StardewValley;
using SFarmer = StardewValley.Farmer;

namespace HAS_Tweaks
{
    /// <summary>The main entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        private ModConfig Config;
        private float Health;
        private float Stamina;

        private double TimeSinceLastMoved;
        private double TimeSinceToolUsed;

        private float ElapsedSeconds => (float)(Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds / 1000);

        private int maxHealth;                  // Player's max health value
        private int maxStamina;                 // Player's max stamina value

        // Stardrop tallies
        public int starDropTally;               // Total number of Stardrops the player has legitimately collected.
        public bool drop1;                      // drop1-drop7 are used to identify each Stardrop collected ingame.
        public bool drop2;
        public bool drop3;
        public bool drop4;
        public bool drop5;
        public bool drop6;
        public bool drop7;

        // Iridium snake milk boolean
        public int snakeMilkTally;              // Total number of "Iridium snake milk" the player has collected. There's only one in the game as of writing this.
        public bool snakeMilk;                  // Checks if the player has recieved the "Iridium snake milk" from Mr. Qi.

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Input.ButtonPressed += OnButtonPressed;

            Monitor.Log("Initialized (press F5 to reload config)");
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.F5)
            {
                Config = Helper.ReadConfig<ModConfig>();
                Monitor.Log("Config reloaded", LogLevel.Info);
            }
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || Game1.paused || Game1.activeClickableMenu != null)
                return;

            SFarmer player = Game1.player;

            /********************************************************************************************************
             * The following section of code is for tallying and calculating the player's max health and stamina.
             *******************************************************************************************************/
            // checks Stardrops acquired ingame and tallies them.
            if (Game1.player.hasOrWillReceiveMail("CF_Fair") && !drop1)
            {
                drop1 = true;
                ++starDropTally;
            }
            if (Game1.player.hasOrWillReceiveMail("CF_Fish") && !drop2)
            {
                drop2 = true;
                ++starDropTally;
            }
            if (Game1.player.hasOrWillReceiveMail("CF_Mines") && !drop3)
            {
                drop3 = true;
                ++starDropTally;
            }
            if (Game1.player.hasOrWillReceiveMail("CF_Sewer") && !drop4)
            {
                drop4 = true;
                ++starDropTally;
            }
            if (Game1.player.hasOrWillReceiveMail("museumComplete") && !drop5)
            {
                drop5 = true;
                ++starDropTally;
            }
            if (Game1.player.hasOrWillReceiveMail("CF_Spouse") && !drop6)
            {
                drop6 = true;
                ++starDropTally;
            }
            if (Game1.player.hasOrWillReceiveMail("CF_Statue") && !drop7)
            {
                drop7 = true;
                ++starDropTally;
            }

            // checks Iridium Snake Milk acquired ingame and tallies them.
            if (Game1.player.mailReceived.Contains("qiCave") && !snakeMilk)
            {
                snakeMilk = true;
                ++snakeMilkTally;
            }

            // accumulated values that make up the player's maximum health and stamina.
            // it should be noted that the way this is setup, the player won't be able to gain additional life or stamina by cheating in Stardrops and Iridium Snake Milk.
            maxHealth = Config.StartingHealth + (starDropTally * Config.StarDropHealth) + (snakeMilkTally * Config.SnakeMilkHealth) + (player.combatLevel * Config.CombatLevelHealth);
            maxStamina = Config.StartingStamina + (starDropTally * Config.StarDropStamina) + (snakeMilkTally * Config.SnakeMilkStamina) + ((player.farmingLevel + player.foragingLevel + player.miningLevel + player.fishingLevel) * Config.SkillLevelStamina);

            // sets max health and stamina to their appropriate values based on the above calculation.
            if (player.maxHealth != maxHealth)
                player.maxHealth = maxHealth;

            if (player.MaxStamina != maxStamina)
                player.MaxStamina = maxStamina;

            /********************************************************************************************************
             * End max health and stamina section.
            ********************************************************************************************************/


            /********************************************************************************************************
             * The following section of code handles the regeneration of health and stamina.
             *******************************************************************************************************/
            // establish whether the player is fishing or there's an event occuring
            bool Fishing = player.usingTool && player.CurrentTool is FishingRod && ((FishingRod)player.CurrentTool).isFishing;
            bool Event = !Game1.shouldTimePass() && !Game1.player.canMove;

            // detect movement or tool use
            TimeSinceLastMoved += Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
            TimeSinceToolUsed += Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
            if (player.timerSinceLastMovement == 0)
                TimeSinceLastMoved = 0;
            if (player.UsingTool && !Fishing)
                TimeSinceToolUsed = 0;

            // health regen
            if (Config.RegenHealthConstant && TimeSinceToolUsed > Config.RegenHealthStillTimeRequiredMS)
            {
                if (TimeSinceLastMoved > Config.RegenHealthStillTimeRequiredMS)
                {
                    if ((!Config.RegenHealthEvent && Event) || (!Config.RegenHealthFishing && Fishing))
                        Health += 0;
                    else
                        Health += Config.RegenHealthConstantAmountPerSecond * ElapsedSeconds;
                }
                if (Config.RegenHealthMoving && TimeSinceLastMoved < Config.RegenHealthStillTimeRequiredMS)
                {
                    if ((!Config.RegenHealthEvent && Event) || (!Config.RegenHealthFishing && Fishing))
                        Health += 0;
                    else
                        Health += Config.RegenHealthConstantAmountPerSecond * ElapsedSeconds * Config.RegenHealthMovingMult;
                }
            }
            if (Config.RegenHealthPercent && TimeSinceToolUsed > Config.RegenHealthStillTimeRequiredMS)
            {
                if (TimeSinceLastMoved > Config.RegenHealthStillTimeRequiredMS)
                {
                    if ((!Config.RegenHealthEvent && Event) || (!Config.RegenHealthFishing && Fishing))
                        Health += 0;
                    else
                        Health += ((Config.RegenHealthPercentPerSecond / 100) * player.maxHealth) * ElapsedSeconds;
                }
                if (Config.RegenHealthMoving && TimeSinceLastMoved < Config.RegenHealthStillTimeRequiredMS)
                {
                    if ((Config.RegenHealthEvent && Event) || (!Config.RegenHealthFishing && Fishing))
                        Health += 0;
                    else
                        Health += ((Config.RegenHealthPercentPerSecond / 100) * player.maxHealth) * ElapsedSeconds * Config.RegenHealthMovingMult;
                }
            }

            // make sure health doesn't overflow.
            if (player.health + Health >= player.maxHealth)
            {
                player.health = player.maxHealth;
                Health = 0;
            }
            else if (Health >= 1)
            {
                player.health += 1;
                Health -= 1;
            }
            else if (Health <= -1)
            {
                player.health -= 1;
                Health += 1;
            }

            // stamina regen
            if (Config.RegenStaminaConstant && TimeSinceToolUsed > Config.RegenStaminaStillTimeRequiredMS)
            {
                if (TimeSinceLastMoved > Config.RegenStaminaStillTimeRequiredMS)
                {
                    if ((!Config.RegenStaminaEvent && Event) || (!Config.RegenStaminaFishing && Fishing))
                        Stamina += 0;
                    else
                        Stamina += Config.RegenStaminaConstantAmountPerSecond * ElapsedSeconds;
                }
                if (Config.RegenStaminaMoving && TimeSinceLastMoved < Config.RegenStaminaStillTimeRequiredMS)
                {
                    if ((!Config.RegenStaminaEvent && Event) || (!Config.RegenStaminaFishing && Fishing))
                        Stamina += 0;
                    else
                        Stamina += Config.RegenStaminaConstantAmountPerSecond * ElapsedSeconds * Config.RegenStaminaMovingMult;
                }
            }
            if (Config.RegenStaminaPercent && TimeSinceToolUsed > Config.RegenStaminaStillTimeRequiredMS)
            {
                if (TimeSinceLastMoved > Config.RegenStaminaStillTimeRequiredMS)
                {
                    if ((!Config.RegenStaminaEvent && Event) || (!Config.RegenStaminaFishing && Fishing))
                        Stamina += 0;
                    else
                        Stamina += ((Config.RegenStaminaPercentPerSecond / 100) * player.maxStamina) * ElapsedSeconds;
                }
                if (Config.RegenStaminaMoving && TimeSinceLastMoved < Config.RegenStaminaStillTimeRequiredMS)
                {
                    if ((!Config.RegenStaminaEvent && Event) || (!Config.RegenStaminaFishing && Fishing))
                        Stamina += 0;
                    else
                        Stamina += ((Config.RegenStaminaPercentPerSecond / 100) * player.maxStamina) * ElapsedSeconds * Config.RegenStaminaMovingMult;
                }
            }

            // make sure stamina doesn't overflow.
            if (player.Stamina + Stamina >= player.MaxStamina)
            {
                player.Stamina = player.MaxStamina;
                Stamina = 0;
            }
            else if (Stamina >= 1)
            {
                player.Stamina += 1;
                Stamina -= 1;
            }
            else if (Stamina <= -1)
            {
                player.Stamina -= 1;
                Stamina += 1;
            }
        }
    }
}


// The following is old code to return to in case something breaks.

/*
if (Config.RegenHealthConstant)
    Health += Config.RegenHealthConstantAmountPerSecond * ElapsedSeconds;
if (Config.RegenHealthStill)
{
    if (TimeSinceLastMoved > Config.RegenHealthStillTimeRequiredMS)
        Health += Config.RegenHealthStillAmountPerSecond * ElapsedSeconds;
}
*/


/*
if (Config.RegenStaminaConstant)
    Stamina += Config.RegenStaminaConstantAmountPerSecond * ElapsedSeconds;
if (Config.RegenStaminaStill)
{
    if (TimeSinceLastMoved > Config.RegenStaminaStillTimeRequiredMS)
        Stamina += Config.RegenStaminaStillAmountPerSecond * ElapsedSeconds;
}
*/
