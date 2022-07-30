/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace SP_Regen
{
    class ModEntry : Mod
    {
        public GameTime time;
        public static SPConfig Config;

        public override void Entry(IModHelper helper)//Load helpers and read config
        {
            helper.Events.GameLoop.UpdateTicked += UpdateTicked;
            helper.Events.GameLoop.SaveLoaded += SaveLoaded;
            this.ReadConfig();
        }

        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            /*if(Config.RegenTime < 300)
            {
                this.Monitor.Log("Regen Timer can't be less than 300, Disabling mod", LogLevel.Warn);
                Helper.Events.GameLoop.UpdateTicked -= UpdateTicked;
            }
            else if (Config.RegenTime > 2000)
            {
                this.Monitor.Log("Regen Timer can't be more than 2000, Disabling mod", LogLevel.Warn);
                Helper.Events.GameLoop.UpdateTicked -= UpdateTicked;
            }
            else
            {
                return;
            }*/
            if (Config.RegenTime <= 0)
            {
                this.Monitor.Log("Have to have some limits, time can't be less than or equal to 0", LogLevel.Warn);
                Helper.Events.GameLoop.UpdateTicked -= UpdateTicked;
            }
            else
                return;
        }

        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)//if world isn't loaded, do nothing
                return;

            time = Game1.currentGameTime; //assigns time variable to currentGameTime
            if(Game1.player.isInBed)//Check if the player is in bed
            {
                Game1.player.regenTimer -= time.ElapsedGameTime.Milliseconds;//Set the regenTimer
                if(Game1.player.regenTimer < 0)//if the regenTimer is below 0 Milliseconds, add stamina and health
                {
                    Game1.player.regenTimer = Config.RegenTime;
                    if (Game1.player.stamina < Game1.player.MaxStamina)
                        ++Game1.player.stamina;
                    if (Game1.player.health < Game1.player.maxHealth)
                        ++Game1.player.health;
                }
            }
        }

        private void ReadConfig()
        {
            Config = (SPConfig)Helper.ReadConfig<SPConfig>();
            Helper.WriteConfig<SPConfig>(Config);
        }
    }
}
