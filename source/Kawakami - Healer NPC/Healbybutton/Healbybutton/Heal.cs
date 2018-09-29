using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace Healbybutton
{
    public class Heal : Mod
    {
        public static HealConfig Config { get; set; }

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<HealConfig>();
            InputEvents.ButtonPressed += Healme;
        }



        private void Healme(object sender, EventArgsInput e)
        {
            if (!Context.IsWorldReady || Game1.currentLocation == null)
                return;

            if (e.Button.Equals(Config.HealKey) && (Game1.player.health < Game1.player.maxHealth || Game1.player.stamina < Game1.player.maxStamina))
            {
                this.Monitor.Log($"{Game1.player.Name} was healed and had energy restored.");
                Game1.player.health = Game1.player.maxHealth;
                Game1.player.stamina = Game1.player.maxStamina;
            }
        }
    }

    public class HealConfig
    {
        public SButton HealKey { get; set; } = SButton.H;
    }
}