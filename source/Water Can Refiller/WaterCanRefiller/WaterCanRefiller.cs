using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using WaterCanRefiller.Framework;

namespace WaterCanRefiller
{
    public class WaterCanRefiller : Mod
    {
        private ModConfig _config;
        private readonly bool _debug = true;

        public override void Entry(IModHelper helper)
        {
            //Set up the config file.
            _config = helper.ReadConfig<ModConfig>();

            //Events
            helper.Events.GameLoop.OneSecondUpdateTicked += OneSecondUpdateTicked;//This event will trigger every second
            helper.Events.Input.ButtonPressed += ButtonPressed;//This event will trigger when a button is pressed.
        }

        //Event Triggers
        private void OneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !_config.ModEnabled)
                return;
            if (Game1.player.currentLocation.IsOutdoors && HasWateringCan() && (Game1.isRaining || Game1.isSnowing || Game1.isLightning))
            {
                foreach (var f in Game1.player.Items)
                {
                    if (f is WateringCan can && can.WaterLeft < can.waterCanMax)
                    {
                        can.WaterLeft += 1;
                        if(_debug || _config.ShowWaterLevel)
                            Game1.showGlobalMessage($"WaterLeft: {can.WaterLeft}\nWaterMax: {can.waterCanMax}");
                    }
                }
            }
        }

        private void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.IsDown(SButton.F5))
            {
                _config = Helper.ReadConfig<ModConfig>();
                Monitor.Log("The Config file was reloaded.", LogLevel.Info);
            }
        }

        //Private custom Voids
        private bool HasWateringCan(bool mustBeHeld = false)
        {
            bool found = false;
            //Go through the players inventory and look for the watering can.
            foreach (var f in Game1.player.Items)
            {
                if (f is WateringCan && !_config.WaterCanMustBeHeld)
                    found = true;
                if (_config.WaterCanMustBeHeld && Game1.player.CurrentTool is WateringCan)
                    found = true;
            }
            return found;
        }
    }
}
