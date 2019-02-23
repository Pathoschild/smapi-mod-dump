using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using StardewModdingAPI.Framework;
using StardewModdingAPI.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Starvation
{
    class ModEntry : Mod
    {
        private Texture2D HungerBar;

        private double Hunger = 100;
        private int StaminaCooldown = 0;
        private float LastStamina;

        public override void Entry(IModHelper helper)
        {
            HungerBar = this.Helper.Content.Load<Texture2D>("assets/HungerBarBack.png");

            helper.Events.Display.RenderingHud += this.DisplayRenderingHud;
            helper.Events.GameLoop.UpdateTicked += this.GameLoopUpdateTicked;
        }

        private void GameLoopUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || Game1.paused)
                return;

            else
            {
                //if farmhouse has not been upgraded yet
                if (Game1.MasterPlayer.HouseUpgradeLevel < 1)
                    if (Hunger > 0 && e.IsMultipleOf(420))
                        Hunger -= 1;

                //if farmhouse has been upgraded before
                else if (Game1.MasterPlayer.HouseUpgradeLevel >= 1)
                    if (Hunger > 0 && e.IsMultipleOf(120))
                        Hunger -= 1;

                //Hunger checks
                if (Hunger > 90 && e.IsOneSecond)
                {
                    if (LastStamina > Game1.player.Stamina)
                        StaminaCooldown = 4;

                    if (StaminaCooldown > 0)
                        StaminaCooldown--;

                    if (StaminaCooldown <= 0)
                        Game1.player.Stamina = Math.Min(Game1.player.MaxStamina, Game1.player.Stamina + 1);
                }
            }

            LastStamina = Game1.player.Stamina;
        }

        private void DisplayRenderingHud(object sender, RenderingHudEventArgs e)
        {
            if (!Context.IsWorldReady || Game1.activeClickableMenu != null)
                return;

            else
            {
            }
        }
    }
}
