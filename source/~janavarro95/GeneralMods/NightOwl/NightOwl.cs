using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Netcode;
using Omegasis.NightOwl.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;

/*TODO:
Issues:
-Mail can't be wiped without destroying all mail.
-Lighting transition does not work if it is raining.
        -set the weather to clear if you are stayig up late.
        -transition still doesnt work. However atleast it is dark now.

-Known glitched
*/
namespace Omegasis.NightOwl
{
    /// <summary>The mod entry point.</summary>
    public class NightOwl : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration.</summary>
        private ModConfig Config;

        /****
        ** Context
        ****/
        /// <summary>Whether the player stayed up all night.</summary>
        private bool IsUpLate;

        /// <summary>Whether the player should be reset to their pre-collapse details for the morning transition on the next update.</summary>
        private bool ShouldResetPlayerAfterCollapseNow;

        /// <summary>Whether the player just started a new day.</summary>
        private bool JustStartedNewDay;

        /// <summary>Whether the player just collapsed for the morning transition.</summary>
        private bool JustCollapsed;

        /****
        ** Pre-collapse state
        ****/
        /// <summary>The player's location name before they collapsed.</summary>
        private string PreCollapseMap;

        /// <summary>The player's tile position before they collapsed.</summary>
        private Point PreCollapseTile;

        /// <summary>The player's money before they collapsed.</summary>
        private int PreCollapseMoney;

        /// <summary>The player's stamina before they collapsed.</summary>
        private float PreCollapseStamina;

        /// <summary>The player's health before they collapsed.</summary>
        private int PreCollapseHealth;

        /// <summary>
        /// Checks if the player was bathing or not before passing out.
        /// </summary>
        private bool isBathing;

        /// <summary>
        /// Checks if the player was in their swimsuit before passing out.
        /// </summary>
        private bool isInSwimSuit;
        
        /// <summary>
        /// The horse the player was riding before they collapsed.
        /// </summary>
        private Horse horse;

        /// <summary>
        /// Determines whehther or not to rewarp the player's horse to them.
        /// </summary>
        private bool shouldWarpHorse;

        /// <summary>
        /// Event in the night taht simulates the earthquake event that should happen.
        /// </summary>
        StardewValley.Events.SoundInTheNightEvent eve;


        private List<NetByte> oldAnimalHappiness;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.oldAnimalHappiness = new List<NetByte>();
            this.Config = helper.ReadConfig<ModConfig>();

            if (Config.UseInternalNightFishAssetEditor)
            {
                this.Helper.Content.AssetEditors.Add(new NightFishing());
            }
            TimeEvents.TimeOfDayChanged += this.TimeEvents_TimeOfDayChanged;
            TimeEvents.AfterDayStarted += this.TimeEvents_AfterDayStarted;
            SaveEvents.AfterLoad += this.SaveEvents_AfterLoad;
            SaveEvents.BeforeSave += this.SaveEvents_BeforeSave;
            GameEvents.FourthUpdateTick += this.GameEvents_FourthUpdateTick;
            GameEvents.UpdateTick += GameEvents_UpdateTick;
            shouldWarpHorse = false;
        }



        /*********
        ** Private methods
        *********/

        /// <summary>
        /// Updates the earthquake event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (eve == null) return;
            else
            {
                eve.tickUpdate(Game1.currentGameTime);
            }
        }

        /// <summary>The method invoked every fourth game update (roughly 15 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        public void GameEvents_FourthUpdateTick(object sender, EventArgs e)
        {
            try
            {
                // reset position after collapse
                if (Context.IsWorldReady && this.JustStartedNewDay && this.Config.KeepPositionAfterCollapse)
                {
                    if (this.PreCollapseMap != null)
                        Game1.warpFarmer(this.PreCollapseMap, this.PreCollapseTile.X, this.PreCollapseTile.Y, false);

                    this.PreCollapseMap = null;
                    this.JustStartedNewDay = false;
                    this.JustCollapsed = false;
                }
            }
            catch (Exception ex)
            {
                this.Monitor.Log(ex.ToString(), LogLevel.Error);
                this.WriteErrorLog();
            }
        }
        
        /// <summary>The method invoked before the game saves.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        public void SaveEvents_BeforeSave(object sender, EventArgs e)
        {
            int collapseFee = 0;
            string[] passOutFees = Game1.player.mailbox
                 .Where(p => p.Contains("passedOut"))
                 .ToArray();
             for (int idx=0; idx< passOutFees.Length; idx++) 
             {
                 string[] msg = passOutFees[idx].Split(' ');
                 collapseFee += Int32.Parse(msg[1]);
             }
             
             if (this.Config.KeepMoneyAfterCollapse) 
             {
                 Game1.player.money += collapseFee;
             }
             
        }

        /// <summary>The method invoked after the player loads a save.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        public void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            this.IsUpLate = false;
            this.JustStartedNewDay = false;
            this.JustCollapsed = false;
        }

        /// <summary>The method invoked when a new day starts.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        public void TimeEvents_AfterDayStarted(object sender, EventArgs e)
        {
            try
            {
                // reset data
                this.IsUpLate = false;

                // transition to the next day
                if (this.ShouldResetPlayerAfterCollapseNow)
                {
                    this.ShouldResetPlayerAfterCollapseNow = false;

                    if (this.Config.KeepStaminaAfterCollapse)
                        Game1.player.stamina = this.PreCollapseStamina;
                    if (this.Config.KeepHealthAfterCollapse)
                        Game1.player.health = this.PreCollapseHealth;
                    if (this.Config.KeepPositionAfterCollapse)
                        if (Game1.weddingToday == false)
                        {
                            Game1.warpFarmer(this.PreCollapseMap, this.PreCollapseTile.X, this.PreCollapseTile.Y, false);
                        }
                    if (horse != null && shouldWarpHorse==true)
                    {
                        Game1.warpCharacter(horse, Game1.player.currentLocation, Game1.player.position);
                        shouldWarpHorse = false;
                    }
                    if (isInSwimSuit)
                    {
                        Game1.player.changeIntoSwimsuit();
                    }
                    if (isBathing)
                    {
                        Game1.player.swimming.Value = true;
                    }
                    //Reflction to ensure that the railroad becomes properly unblocked.
                    if (Game1.dayOfMonth == 1 && Game1.currentSeason == "summer" && Game1.year == 1)
                    {
                        Mountain mountain = (Mountain)Game1.getLocationFromName("Mountain");

                        var reflect2 = Helper.Reflection.GetField<NetBool>(mountain, "railroadAreaBlocked", true);
                        var netBool2 = reflect2.GetValue();
                        netBool2.Value = false;
                        reflect2.SetValue(netBool2);


                        var reflect3 = Helper.Reflection.GetField<Rectangle>(mountain, "railroadBlockRect", true);
                        var netBool3 = reflect3.GetValue();
                        netBool3 = new Rectangle(0, 0, 0, 0);
                        reflect3.SetValue(netBool3);

                        
                        eve = new StardewValley.Events.SoundInTheNightEvent(4);
                        eve.setUp();
                        eve.makeChangesToLocation();
                        
                    }
                }

                if(Game1.currentSeason!="spring" && Game1.year >= 1)
                {
                    clearRailRoadBlock();
                }

                // delete annoying charge messages (if only I could do this with mail IRL)
                if (this.Config.SkipCollapseMail)
                {
                    string[] validMail = Game1.mailbox
                        .Where(p => !p.Contains("passedOut"))
                        .ToArray();

                    Game1.mailbox.Clear();
                    foreach (string mail in validMail)
                        Game1.mailbox.Add(mail);
                }

                this.JustStartedNewDay = true;
            }
            catch (Exception ex)
            {
                this.Monitor.Log(ex.ToString(), LogLevel.Error);
                this.WriteErrorLog();
            }
        }

        /// <summary>
        /// If the user for this mod never gets the event that makes the railroad blok go away we will always force it to go away if they have met the conditions for it. I.E not being in spring of year 1.
        /// </summary>
        private void clearRailRoadBlock()
        {
            Mountain mountain = (Mountain)Game1.getLocationFromName("Mountain");

            var reflect2 = Helper.Reflection.GetField<NetBool>(mountain, "railroadAreaBlocked", true);
            var netBool2 = reflect2.GetValue();
            netBool2.Value = false;
            reflect2.SetValue(netBool2);


            var reflect3 = Helper.Reflection.GetField<Rectangle>(mountain, "railroadBlockRect", true);
            var netBool3 = reflect3.GetValue();
            netBool3 = new Rectangle(0, 0, 0, 0);
            reflect3.SetValue(netBool3);
        }

        /// <summary>The method invoked when <see cref="Game1.timeOfDay"/> changes.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void TimeEvents_TimeOfDayChanged(object sender, EventArgsIntChanged e)
        {
            if (!Context.IsWorldReady)
                return;

            try
            {
                // transition morning light more realistically
                if (this.Config.MorningLightTransition && Game1.timeOfDay > 400 && Game1.timeOfDay < 600)
                {
                    float colorMod = (1300 - Game1.timeOfDay) / 1000f;
                    Game1.outdoorLight = Game1.ambientLight * colorMod;
                }

                // transition to next morning
                if (this.Config.StayUp && Game1.timeOfDay == 2550)
                {
                    Game1.isRaining = false; // remove rain, otherwise lighting gets screwy
                    Game1.updateWeatherIcon();
                    Game1.timeOfDay = 150; //change it from 1:50 am late, to 1:50 am early
                    foreach(FarmAnimal animal in Game1.getFarm().getAllFarmAnimals())
                    {
                        this.oldAnimalHappiness.Add(animal.happiness);
                    }

                }

                // collapse player at 6am to save & reset
                if (Game1.timeOfDay == 550)
                    this.IsUpLate = true;
                if (this.IsUpLate && Game1.timeOfDay == 600 && !this.JustCollapsed)
                {
                    if (Game1.player.isRidingHorse())
                    {
                        foreach (var character in Game1.player.currentLocation.characters)
                        {
                            try
                            {
                                if (character is Horse)
                                {
                                    (character as Horse).dismount();
                                    horse = (character as Horse);
                                    shouldWarpHorse = true;
                                }
                               
                            }
                            catch (Exception err)
                            {

                            }
                        }
                    }
                    this.JustCollapsed = true;

                    this.ShouldResetPlayerAfterCollapseNow = true;
                    this.PreCollapseTile = new Point(Game1.player.getTileX(), Game1.player.getTileY());
                    this.PreCollapseMap = Game1.player.currentLocation.Name;
                    this.PreCollapseStamina = Game1.player.stamina;
                    this.PreCollapseHealth = Game1.player.health;
                    this.PreCollapseMoney = Game1.player.money;
                    this.isInSwimSuit = Game1.player.bathingClothes.Value;
                    this.isBathing = Game1.player.swimming.Value;



                    if (Game1.currentMinigame != null)
                        Game1.currentMinigame = null;

                    if (Game1.activeClickableMenu != null) Game1.activeClickableMenu.exitThisMenu(true); //Exit menus.

                    Game1.timeOfDay += 2400; //Recalculate for the sake of technically being up a whole day.

                    //Reset animal happiness since it drains over night.
                    for(int i=0; i < oldAnimalHappiness.Count; i++)
                    {
                        Game1.getFarm().getAllFarmAnimals()[i].happiness.Value = oldAnimalHappiness[i].Value;
                    }

                    Game1.player.startToPassOut();


                }
            }
            catch (Exception ex)
            {
                this.Monitor.Log(ex.ToString(), LogLevel.Error);
                this.WriteErrorLog();
            }
        }

        /// <summary>Write the current mod state to the error log file.</summary>
        private void WriteErrorLog()
        {
            var state = new
            {
                this.Config,
                this.IsUpLate,
                this.ShouldResetPlayerAfterCollapseNow,
                this.JustStartedNewDay,
                this.JustCollapsed,
                this.PreCollapseMap,
                this.PreCollapseTile,
                this.PreCollapseMoney,
                this.PreCollapseStamina,
                this.PreCollapseHealth
            };
            string path = Path.Combine(this.Helper.DirectoryPath, "Error_Logs", "Mod_State.json");
            this.Helper.WriteJsonFile(path, state);
        }

        /// <summary>
        /// Try and emulate the old Game1.shouldFarmerPassout logic.
        /// </summary>
        /// <returns></returns>
        public bool shouldFarmerPassout()
        {
            if (Game1.player.stamina <= 0 || Game1.player.health <= 0 || Game1.timeOfDay >= 2600) return true;
            else return false;
        }
    }
}
