/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MissCoriel/Sit-n-Relax
**
*************************************************/

using System;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using Newtonsoft.Json;
using StardewValley.Monsters;

namespace SitnRelax
{
    public class ModEntry : Mod
    {

        private readonly PerScreen<int> SecondsUntilRegen = new PerScreen<int>();
        private readonly PerScreen<bool> initialSit = new PerScreen<bool>();
        private readonly PerScreen<string> farmerName = new PerScreen<string>();
        private configModel Config;
        private readonly PerScreen<int> sittingHP = new PerScreen<int>();

        
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += this.UpdateTicked;
           // helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            this.Config = helper.ReadConfig<configModel>();
            
        }
        /*private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == this.Config.sitKey && this.Config.allowSitAnywhere == true)
            {
                Monitor.Log("Sit Anywhere was used!", LogLevel.Info);
                if (!Game1.player.isSitting)
                {
                    

                }

                else if (Game1.player.isSitting.Value == true)
                {
                    Game1.player.isStopSitting = true;
                    
                }
            }
        }*/
        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Game1.player.isSitting)
            {
                this.SecondsUntilRegen.Value = this.Config.regenRate;
                initialSit.Value = false;
                return;
            }

            // wait until regen time
            if(Game1.player.hasMenuOpen == false)
            {
                if (this.SecondsUntilRegen.Value > 0)
                {
                    if (initialSit.Value == false)
                    {
                        this.farmerName.Value = Game1.player.Name;
                        Game1.addHUDMessage(new HUDMessage($"{Game1.player.name} {Config.restMessage}", 4));
                        // sittingHP = Game1.player.health;
                        initialSit.Value = true;
                    }

                    if (e.IsOneSecond)
                        this.SecondsUntilRegen.Value--;

                    if (this.SecondsUntilRegen.Value > 0)
                        return;
                }
                //apply regen 
                if (Game1.player.stamina >= Game1.player.maxStamina.Value && Game1.player.health >= Game1.player.maxHealth)
                {
                    Monitor.Log("Stamina and Health are full.. doing nothing...", LogLevel.Trace);

                }
                else if (Game1.player.stamina < Game1.player.maxStamina.Value)
                {
                    Game1.player.stamina = Math.Min(Game1.player.stamina + this.Config.stamRegen, Game1.player.maxStamina.Value);
                    Monitor.Log("Regenerating Stamina", LogLevel.Trace);
                    SecondsUntilRegen.Value = this.Config.regenRate;

                }
                else if (Game1.player.stamina >= Game1.player.maxStamina.Value)
                {
                    if (Game1.player.health < Game1.player.maxHealth)
                    {
                        Game1.player.health = Game1.player.health + this.Config.healthRegen;
                        Monitor.Log("Stamina Full... Regenerating Health", LogLevel.Trace);
                        SecondsUntilRegen.Value = this.Config.regenRate;
                    }
                }
                if (Game1.player.health < sittingHP.Value)
                {
                    double damageTaken = sittingHP.Value - Game1.player.health;
                    double addDamage = damageTaken * .50;
                    Game1.player.takeDamage((int)addDamage, false, null);
                    Game1.addHUDMessage(new HUDMessage("Hey!! What are you doing?! Don't let your guard down!!", 3));
                    Game1.player.isStopSitting = true;
                    Game1.player.currentTemporaryInvincibilityDuration = 0;
                    sittingHP.Value = Game1.player.health;
                }

            }


            
        }
    }
}
