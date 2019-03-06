using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework.Input;
using CleanFarm.CleanTasks;
using System;
using System.Linq;

namespace CleanFarm
{
    public class CleanFarm : Mod
    {
        /// <summary>The config for this mod.</summary>
        private ModConfig Config;

        /// <summary>The clean tasks to run.</summary>
        private List<ICleanTask> CleanTasks;

        /// <summary>Gets the farm location.</summary>
        private Farm PlayerFarm => Game1.locations.First(loc => loc is Farm) as Farm;

        /// <summary>The commands to execute during tick. We do this to prevent modifying the collections while they're being used by the game (ie. draw).</summary>
        private Queue<ICommand> CommandQueue;

        /// <summary>Mod entry point.</summary>
        /// <param name="helper">Mod helper interface.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();
            InitTasks(this.Config);

            this.CommandQueue = new Queue<ICommand>();

            helper.Events.GameLoop.DayStarted += OnNewDay;

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

        private void QueueCommand(ICommand command)
        {
            this.CommandQueue.Enqueue(command);
            // Ensure we only subscribe once.
            if (this.CommandQueue.Count == 1)
            {
                this.Helper.Events.GameLoop.UpdateTicked += GameEvents_UpdateTick;
            }
        }

        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            this.Monitor.Log($"Processing {this.CommandQueue.Count} commands");
            while (this.CommandQueue.Count > 0)
            {
                this.CommandQueue.Dequeue().Execute();
            }
            this.Helper.Events.GameLoop.UpdateTicked -= GameEvents_UpdateTick;
        }

        /// <summary>Callback for the OnNewDay event. Runs the clean once the day has finished transitioning.</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNewDay(object sender, EventArgs e)
        {
            // No need to queue this one since this event is raised during update.
            Clean();
        }

        /// <summary>Runs the clean tasks.</summary>
        private void Clean()
        {
            if (!Context.IsWorldReady)
            {
                this.Monitor.Log("You must have a save loaded to run this command.", LogLevel.Info);
                return;
            }

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

        private void Restore()
        {
            if (!Context.IsWorldReady)
            {
                this.Monitor.Log("You must have a save loaded to run this command.", LogLevel.Info);
                return;
            }

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
        }

        private void ReloadConfig(IModHelper helper)
        {
            this.Monitor.Log("Reloading CleanFarm config", LogLevel.Info);
            try
            {
                // Don't re-create the tasks or we'll lose any removed item data, preventing us from restoring it.
                this.Config = helper.ReadConfig<ModConfig>();
                InitTasks(this.Config);
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"Failed to reload config: {ex}", LogLevel.Error);
                return;
            }
            this.Monitor.Log("Config reloaded successfully.", LogLevel.Info);
        }

        #region DebugCommands
        private void InitConsoleCommands(IModHelper helper)
        {
#if DEBUG
            // Manually run the clean
            helper.Events.Input.ButtonPressed += (sender, e) =>
            {
                if (e.Button == SButton.V)
                    QueueCommand(new Command(() => Clean()));
            };
#endif

            helper.ConsoleCommands.Add("cf_clean", "Manually runs the clean.", (name, args) =>
            {
                QueueCommand(new Command(() => Clean()));
            });

            helper.ConsoleCommands.Add("cf_restore", "Restores the items removed from the farm by the last clean command that occured this session.", (sender, e) =>
            {
                QueueCommand(new Command(() => Restore()));
            });

            // Reloads config and re-creates tasks. Used for quickly testing different config settings without restarting.
            helper.ConsoleCommands.Add("cf_reload", "Reloads the config.", (sender, e) =>
            {
                QueueCommand(new Command(() => ReloadConfig(helper)));
            });
        }
#endregion DebugCommands
    }
}
