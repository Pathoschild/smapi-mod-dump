using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Phrasefable_Modding_Tools
{
    public partial class PhrasefableModdingTools
    {
        private readonly ToggleableEventLoggerCollection _loggers = new ToggleableEventLoggerCollection();
        private readonly string[] _enable = {"1", "true", "t"};
        private readonly string[] _disable = {"0", "false", "f"};


        private void SetUp_EventLogging()
        {
            BuildLogger_World_DebrisListChanged();
            BuildLogger_World_ObjectListChanged();
            BuildLogger_World_LocationListChanged();
            BuildLogger_World_TerrainFeatureListChanged();
            BuildLogger_World_LargeTerrainFeatureListChanged();
            BuildLogger_GameLoop_Saving();
            BuildLogger_GameLoop_Saved();
            BuildLogger_GameLoop_SaveLoaded();
            BuildLogger_GameLoop_DayStarted();
            BuildLogger_GameLoop_DayEnding();

            const string desc = "Usage: log-events [event...][ {1|true|t|0|false|f}]";
            Helper.ConsoleCommands.Add("log-events", desc, Callback);
        }


        private void Callback(string command, [NotNull] string[] args)
        {
            var action = ToggleAction.Toggle;
            var targets = new List<string>();
            List<string> validIds = _loggers.Ids.ToList();

            foreach (string arg in args)
            {
                if (_enable.Contains(arg))
                {
                    action = ToggleAction.Enable;
                }
                else if (_disable.Contains(arg))
                {
                    action = ToggleAction.Disable;
                }
                else if (action == ToggleAction.Toggle && validIds.Contains(arg))
                {
                    targets.Add(arg);
                }
                else
                {
                    Monitor.Log($"Argument '{arg}' malformed. Command aborted.");
                    return;
                }
            }

            if (targets.Any())
            {
                _loggers.Set(targets, action);
            }
            else if (action != ToggleAction.Toggle)
            {
                targets = validIds;
                _loggers.Set(targets, action);
            }

            var message = new StringBuilder("Enabled:");
            foreach (IToggleableEventLogger logger in _loggers.Where(l => l.IsEnabled).OrderBy(l => l.Id))
            {
                message.Append($" {(targets.Contains(logger.Id) ? "*" : "")}{logger.Id}");
            }

            Monitor.Log(message.ToString(), LogLevel.Info);

            message = new StringBuilder("Disabled:");
            foreach (IToggleableEventLogger logger in _loggers.Where(l => !l.IsEnabled).OrderBy(l => l.Id))
            {
                message.Append($" {(targets.Contains(logger.Id) ? "*" : "")}{logger.Id}");
            }

            Monitor.Log(message.ToString(), LogLevel.Info);
        }


        [NotNull]
        private ToggleableEventLogger<T> BuildLogger<T>([NotNull] string name, Func<T, string> message)
            where T : EventArgs
        {
            var logger = new ToggleableEventLogger<T>(name, Monitor, message);
            _loggers.Add(logger);
            return logger;
        }


        private void BuildLogger_World_DebrisListChanged()
        {
            ToggleableEventLogger<DebrisListChangedEventArgs> logger = BuildLogger(
                "debris",
                (DebrisListChangedEventArgs args) =>
                    $"World.DebrisListChanged {args.Location.Name} +{args.Added.Count()} -{args.Removed.Count()}"
            );
            Helper.Events.World.DebrisListChanged += logger.OnEvent;
        }


        private void BuildLogger_World_ObjectListChanged()
        {
            ToggleableEventLogger<ObjectListChangedEventArgs> logger = BuildLogger(
                "objects",
                (ObjectListChangedEventArgs args) =>
                    $"World.ObjectListChanged {args.Location.Name} +{args.Added.Count()} -{args.Removed.Count()}"
            );
            Helper.Events.World.ObjectListChanged += logger.OnEvent;
        }


        private void BuildLogger_World_LocationListChanged()
        {
            ToggleableEventLogger<LocationListChangedEventArgs> logger = BuildLogger(
                "locations",
                (LocationListChangedEventArgs args) =>
                    $"World.LocationListChanged +{args.Added.Count()} -{args.Removed.Count()}"
            );
            Helper.Events.World.LocationListChanged += logger.OnEvent;
        }


        private void BuildLogger_World_TerrainFeatureListChanged()
        {
            ToggleableEventLogger<TerrainFeatureListChangedEventArgs> logger = BuildLogger(
                "terrain",
                (TerrainFeatureListChangedEventArgs args) =>
                    $"World.TerrainFeatureListChanged {args.Location.Name} +{args.Added.Count()} -{args.Removed.Count()}"
            );
            Helper.Events.World.TerrainFeatureListChanged += logger.OnEvent;
        }


        private void BuildLogger_World_LargeTerrainFeatureListChanged()
        {
            ToggleableEventLogger<LargeTerrainFeatureListChangedEventArgs> logger = BuildLogger(
                "large-terrain",
                (LargeTerrainFeatureListChangedEventArgs args) =>
                    $"World.LargeTerrainFeatureListChanged {args.Location.Name} +{args.Added.Count()} -{args.Removed.Count()}"
            );
            Helper.Events.World.LargeTerrainFeatureListChanged += logger.OnEvent;
        }


        private void BuildLogger_GameLoop_Saving()
        {
            ToggleableEventLogger<SavingEventArgs> logger = BuildLogger(
                "saving",
                (SavingEventArgs args) => "GameLoop.Saving"
            );
            Helper.Events.GameLoop.Saving += logger.OnEvent;
        }


        private void BuildLogger_GameLoop_Saved()
        {
            ToggleableEventLogger<SavedEventArgs> logger = BuildLogger(
                "saved",
                (SavedEventArgs args) => "GameLoop.Saved"
            );
            Helper.Events.GameLoop.Saved += logger.OnEvent;
        }


        private void BuildLogger_GameLoop_SaveLoaded()
        {
            ToggleableEventLogger<SaveLoadedEventArgs> logger = BuildLogger(
                "save-loaded",
                (SaveLoadedEventArgs args) => "GameLoop.SaveLoaded"
            );
            Helper.Events.GameLoop.SaveLoaded += logger.OnEvent;
        }


        private void BuildLogger_GameLoop_DayStarted()
        {
            ToggleableEventLogger<DayStartedEventArgs> logger = BuildLogger(
                "day-started",
                (DayStartedEventArgs args) => "GameLoop.DayStarted"
            );
            Helper.Events.GameLoop.DayStarted += logger.OnEvent;
        }


        private void BuildLogger_GameLoop_DayEnding()
        {
            ToggleableEventLogger<DayEndingEventArgs> logger = BuildLogger(
                "day-ending",
                (DayEndingEventArgs args) => "GameLoop.DayEnding"
            );
            Helper.Events.GameLoop.DayEnding += logger.OnEvent;
        }
    }
}
