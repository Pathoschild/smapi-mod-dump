using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework.Input;
using CleanFarm.CleanTasks;
using System;

namespace CleanFarm
{
    public class CleanFarm : Mod
    {
        /// <summary>The config for this mod.</summary>
        private ModConfig Config;

        /// <summary>The clean tasks to run.</summary>
        private List<ICleanTask> CleanTasks;

        /// <summary>Gets the farm location.</summary>
        private Farm PlayerFarm => Game1.locations.Find(loc => loc is Farm) as Farm;


        /// <summary>Mod entry point.</summary>
        /// <param name="helper">Mod helper interface.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();
            InitTasks(this.Config);

            TimeEvents.AfterDayStarted += OnNewDay;

            InitConsoleCommands(helper);
        }

        /// <summary>Creates the clean tasks. This is just it isn't duplicated for the debug command.</summary>
        /// <param name="config">Mod config object.</param>
        private void InitTasks(ModConfig config)
        {
            this.CleanTasks = new List<ICleanTask>()
            {
                new ObjectCleanTask(config),
                new ResourceClumpCleanTask(config),
                new TerrainFeatureCleanTask(config),
                new LargeTerrainFeatureCleanTask(config)
            };
        }

        /// <summary>Callback for the OnNewDay event. Runs the clean once the day has finished transitioning.</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNewDay(object sender, EventArgs e)
        {
            Clean();
        }

        /// <summary>Runs the clean tasks.</summary>
        private void Clean()
        {
            if (this.PlayerFarm == null)
            {
                this.Monitor.Log("Cannot clean farm: farm location is invalid.", LogLevel.Warn);
                return;
            }

            this.Monitor.Log("Cleaning up the farm...", LogLevel.Info);

            // Run the tasks
            foreach (ICleanTask cleanTask in this.CleanTasks)
            {
                if (!cleanTask.CanRun())
                    continue;
                try
                {
                    cleanTask.Run(this.PlayerFarm);
                    cleanTask.ReportRemovedItems(this.Monitor);
                }
                catch (Exception ex)
                {
                    this.Monitor.Log($"{cleanTask.ToString()} failed to run: {ex}", LogLevel.Error);
                }
            }

            this.Monitor.Log("Cleanup complete! Use the 'cf_restore' console command to restore the removed items.", LogLevel.Info);
        }

        #region DebugCommands
        private void InitConsoleCommands(IModHelper helper)
        {
#if DEBUG
            // Manually run the clean
            ControlEvents.KeyPressed += (sender, e) =>
            {
                if (e.KeyPressed == Keys.V)
                    Clean();
            };
#endif

            // Convenience for testing only with command line
            helper.ConsoleCommands.Add("cf_clean", "Manually runs the clean.", (name, args) => Clean());

            helper.ConsoleCommands.Add("cf_restore", "Restores the items removed from the farm by the last clean command that occured this session.", (sender, e) =>
            {
                this.Monitor.Log("Restoring removed items...", LogLevel.Info);
                if (this.PlayerFarm == null)
                {
                    this.Monitor.Log("Farm is invalid", LogLevel.Error);
                    return;
                }

                int itemsRestored = 0;
                int errorCount = 0;
                try
                {
                    foreach (var task in this.CleanTasks)
                        itemsRestored += task.RestoreRemovedItems(this.PlayerFarm);
                }
                catch (Exception ex)
                {
                    this.Monitor.Log($"Error while trying to restore items: {ex}", LogLevel.Error);
                    ++errorCount;
                }
                this.Monitor.Log($"Finished restoring {itemsRestored} items with {errorCount} errors.", LogLevel.Info);
            });

            // Reloads config and re-creates tasks. Used for quickly testing different config settings without restarting.
            helper.ConsoleCommands.Add("cf_reload", "Reloads the config.", (sender, e) =>
            {
                this.Monitor.Log("Reloading CleanFarm config", LogLevel.Info);
                try
                {
                    this.Config = helper.ReadConfig<ModConfig>();
                    InitTasks(this.Config);
                }
                catch (Exception ex)
                {
                    this.Monitor.Log($"Failed to reload config: {ex}", LogLevel.Error);
                    return;
                }
                this.Monitor.Log("Config reloaded successfully.", LogLevel.Info);
            });
        }
#endregion DebugCommands
    }
}
