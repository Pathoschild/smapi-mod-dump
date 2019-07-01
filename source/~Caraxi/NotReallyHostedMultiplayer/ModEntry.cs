using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;

namespace NotReallyHostedMultiplayer
{
    class ModEntry : Mod
    {
        bool isPaused = false;
        bool triedStartServer = false;
        int s = 0;
        private ModConfig Config;
        int sleepCountdown = 10;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();
            this.sleepCountdown = this.Config.SleepCountdownTimer;

            helper.Events.GameLoop.OneSecondUpdateTicked += onOneSecondUpdateTicked;
            helper.Events.GameLoop.DayStarted += onDayStarted;

            this.Helper.ConsoleCommands.Add("nrhm_toggle", "Toggle NotReallyHostedMultiplayer main loop", delegate
            {
                this.Config.Enabled = !this.Config.Enabled;
                this.Monitor.Log(this.Config.Enabled ? "Enabled" : "Disabled", LogLevel.Info);
                this.Helper.WriteConfig<ModConfig>(this.Config);
            });

            this.Helper.ConsoleCommands.Add("nrhm_autostart_toggle", "Toggle launching the game into the currently loaded save.", delegate
            {
                if (this.Config.AutoLoad)
                {
                    this.Config.AutoLoad = false;
                    this.Config.AutoLoadFile = "";
                    this.Monitor.Log("Auto Load Disabled", LogLevel.Info);
                    this.Helper.WriteConfig<ModConfig>(this.Config);
                }
                else
                {
                    if (Game1.hasLoadedGame && Game1.IsMasterGame)
                    {
                        this.Monitor.Log(Constants.SaveFolderName);
                        this.Config.AutoLoadFile = Constants.SaveFolderName;
                        this.Config.AutoLoad = true;
                        this.Monitor.Log("Auto Load Enabled", LogLevel.Info);
                        this.Helper.WriteConfig<ModConfig>(this.Config);
                    }
                    else
                    {
                        this.Monitor.Log("Load the save first.", LogLevel.Warn);
                    }
                }
            });
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onDayStarted(object sender, DayStartedEventArgs e)
        {
            Helper.Events.Specialised.UnvalidatedUpdateTicked -= this.endOfNightHandle;
        }

        private String getInviteCode()
        {
            if (Game1.server != null)
            {
                String s = Game1.server.getInviteCode();
                if (s == null && triedStartServer == false)
                {
                    triedStartServer = true;
                    this.Monitor.Log("Waiting for Invite Code");
                    Game1.server.offerInvite();
                    return null;
                }
                if (s != null && s.Length > 0)
                {
                    return s;
                }
                else
                {
                    return null;
                }
            } else { return null; }
        }

        /// <summary>Raised once per second after the game state is updated.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void onOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (!this.Config.Enabled) return;

            if (!Game1.hasLoadedGame)
            {

                if (this.Config.AutoLoad)
                {

                    if (Game1.activeClickableMenu is TitleMenu menu)
                    {
                        s += 1;
                        if (s > 1)
                        {
                            Game1.activeClickableMenu.exitThisMenu(false);
                            this.Monitor.Log("Trying to Load");
                            Game1.multiplayerMode = 2;
                            SaveGame.Load(this.Config.AutoLoadFile);
                            
                        }
                        
                    } else
                    {
                        this.Monitor.Log(Game1.activeClickableMenu.ToString());
                    }
                    

                    return;
                }


                //this.Monitor.Log("Not Loaded");
                return;
            }
            if (!Game1.IsServer)
            {
                //this.Monitor.Log("Not Server");
                return;
            }
            if (this.getInviteCode() == null)
            {
                return;
            }

            if (Game1.player.isInBed.Equals(false))
            {
                if (sleepCountdown <= 0)
                {
                    Vector2 bed = Utility.PointToVector2(Utility.getHomeOfFarmer(Game1.player).getBedSpot()) * 64f;
                    bed.X -= 64f;
                    Game1.warpFarmer(Game1.player.homeLocation.Value, (int)bed.X / 64, (int)bed.Y / 64, 2, false);
                    sleepCountdown = this.Config.SleepCountdownTimer;
                } else
                {
                    sleepCountdown -= 1;
                }
                return;
            }

            
            if (Game1.otherFarmers.Count == 0 || this.isPaused)
            {
                if (Game1.netWorldState.Value.IsPaused != true)
                {
                    Game1.netWorldState.Value.IsPaused = true;
                    this.Monitor.Log("Nobody Online: Pausing");
                    this.Monitor.Log("Invite Code: " + this.getInviteCode());
                }
                return;
            }

            
            
            if (Game1.otherFarmers.Count > 0 && this.isPaused == false)
            {
                if (Game1.netWorldState.Value.IsPaused == true)
                {
                    this.Monitor.Log("A Player is Online: Unpausing");
                    Game1.netWorldState.Value.IsPaused = false;
                    this.Monitor.Log("Invite Code: " + this.getInviteCode());
                }

                int inBed = Game1.player.team.GetNumberReady("sleep");

                if (inBed == Game1.otherFarmers.Count)
                {

                    Game1.player.team.SetLocalReady("sleep", true);

                    sleepCountdown -= 1;
                    if (sleepCountdown <= 0)
                    {
                        this.Monitor.Log("Sleeping", LogLevel.Info);
                        sleepCountdown = this.Config.SleepCountdownTimer;
                        Game1.NewDay(600F);

                        int i = 0;

                        Helper.Events.Specialised.UnvalidatedUpdateTicked += this.endOfNightHandle;
                    }
                }
            } 
            else
            {
                sleepCountdown = this.Config.SleepCountdownTimer;
                Game1.player.team.SetLocalReady("sleep", false);
            }
        }


        public void endOfNightHandle(object sender, EventArgs e)
        {
            if (Game1.ticks % 30 == 0)
            {
                this.Monitor.Log("Waiting For New Day", LogLevel.Warn);
               
                if (Game1.activeClickableMenu is ShippingMenu m)
                {
                    if (m.okButton.visible)
                    {
                        this.Helper.Reflection.GetMethod(m, "okClicked", true).Invoke<object[]>();
                        m.okButton.visible = false;
                    }
                }
            }
        }
    }
}
