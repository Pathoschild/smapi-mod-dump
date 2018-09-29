using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Omegasis.NightOwl.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

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



        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();

            TimeEvents.TimeOfDayChanged += this.TimeEvents_TimeOfDayChanged;
            TimeEvents.AfterDayStarted += this.TimeEvents_AfterDayStarted;
            SaveEvents.AfterLoad += this.SaveEvents_AfterLoad;
            GameEvents.FourthUpdateTick += this.GameEvents_FourthUpdateTick;
        }


        /*********
        ** Private methods
        *********/
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
                Game1.farmerShouldPassOut = false;

                // transition to the next day
                if (this.ShouldResetPlayerAfterCollapseNow)
                {
                    this.ShouldResetPlayerAfterCollapseNow = false;

                    if (this.Config.KeepStaminaAfterCollapse)
                        Game1.player.stamina = this.PreCollapseStamina;
                    if (this.Config.KeepHealthAfterCollapse)
                        Game1.player.health = this.PreCollapseHealth;
                    if (this.Config.KeepMoneyAfterCollapse)
                        Game1.player.money = this.PreCollapseMoney;
                    if (this.Config.KeepPositionAfterCollapse)
                        Game1.warpFarmer(this.PreCollapseMap, this.PreCollapseTile.X, this.PreCollapseTile.Y, false);
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
                }

                // collapse player at 6am to save & reset
                if (Game1.timeOfDay == 550)
                    this.IsUpLate = true;
                if (this.IsUpLate && Game1.timeOfDay == 600 && !this.JustCollapsed)
                {
                    this.JustCollapsed = true;

                    this.ShouldResetPlayerAfterCollapseNow = true;
                    this.PreCollapseTile = new Point(Game1.player.getTileX(), Game1.player.getTileY());
                    this.PreCollapseMap = Game1.player.currentLocation.Name;
                    this.PreCollapseStamina = Game1.player.stamina;
                    this.PreCollapseHealth = Game1.player.health;
                    this.PreCollapseMoney = Game1.player.money;

                    if (Game1.currentMinigame != null)
                        Game1.currentMinigame = null;
                    Game1.farmerShouldPassOut = true;
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
    }
}
