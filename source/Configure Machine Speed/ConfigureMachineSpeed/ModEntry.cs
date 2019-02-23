using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Buildings;
using StardewValley.Objects;
using System.Linq;

namespace ConfigureMachineSpeed
{
    public class ModEntry : Mod
    {
        private ModConfig Config;

        /*
         * Mod Entry & Config Validation
         */

         public override void Entry(IModHelper helper)
        {
            this.Config = processConfig(helper.ReadConfig<ModConfig>());
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.UpdateTicking += this.OnUpdateTicking;
        }
 
        // Validate and alter the input from config.json
        private ModConfig processConfig(ModConfig cfg)
        {
            if (cfg.UpdateInterval <= 0)
                cfg.UpdateInterval = 1;
            foreach (MachineConfig machine in cfg.Machines)
            {
                if (machine.Minutes <= 0)
                    machine.Minutes = 10;
                machine.Minutes = ((int)machine.Minutes / 10) * 10 - 1; // Kind of a cheap hack to make slowing down machines work, but it eliminates the need to keep a big table of all the machines so I'm going with it unless it makes the mod incompatible with some kinda other mod
            }
            return cfg;
        }

        /*
         * Event Callbacks
         */

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;
            configureAllMachines();
        }

        private void OnUpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;
            if (e.IsMultipleOf(this.Config.UpdateInterval))
            {
                configureAllMachines();
            }
        }

        /*
         * Helper Methods
         */

        // Sweep through all the machines in the world and configure them with the appropriate configuration.
        private void configureAllMachines()
        {
            IEnumerable<GameLocation> locations = GetLocations();
            foreach (MachineConfig cfg in this.Config.Machines)
            {
                foreach (GameLocation loc in locations)
                {
                    Func<KeyValuePair<Vector2, StardewValley.Object>, bool> nameMatch = p => p.Value.name == cfg.Name;
                    foreach (KeyValuePair<Vector2, StardewValley.Object> pair in loc.objects.Pairs)
                    {
                        if (nameMatch(pair))
                            configureMachine(cfg, pair.Value);
                    }
                }
            }
        }

        // Configures a given machine with a given configuration.
        // Be sure to check that the second parameter is a machine before passing it to this function.
        private void configureMachine (MachineConfig cfg, StardewValley.Object obj)
        {
            if (obj.MinutesUntilReady > 0 && obj.MinutesUntilReady % 10 == 0)
            {
                    obj.MinutesUntilReady = cfg.Minutes;
            }
            if (obj is Cask)
            {
                ((Cask)obj).daysToMature.Value = cfg.Minutes / 1440;
            }
        }

        /// Get all game locations.
        /// Copied with permission from Pathoschild
        public static IEnumerable<GameLocation> GetLocations()
        {
            return Game1.locations
                .Concat(
                    from location in Game1.locations.OfType<BuildableGameLocation>()
                    from building in location.buildings
                    where building.indoors.Value != null
                    select building.indoors.Value
                );
        }

    }
}
