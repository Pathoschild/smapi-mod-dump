using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Phrasefable_Modding_Tools {

    public partial class PhrasefableModdingTools {

        private readonly ToggleableEventLoggerCollection _loggers = new ToggleableEventLoggerCollection();
        private readonly string[] _enable = {"1", "true", "t"};
        private readonly string[] _disable = {"0", "false", "f"};


        private void SetUp_EventLogging() {
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

            var desc = "args: [event...][ {1|true|t|0|false|f}]";
            Helper.ConsoleCommands.Add("log-events", desc, Callback);

        }


        private void Callback(string command, [NotNull] string[] args) {
            var action = ToggleAction.Toggle;
            var targets = new List<string>();
            var validIds = _loggers.Ids.ToList();

            foreach (var arg in args) {
                if (_enable.Contains(arg)) {
                    action = ToggleAction.Enable;
                } else if (_disable.Contains(arg)) {
                    action = ToggleAction.Disable;
                } else if (action == ToggleAction.Toggle && validIds.Contains(arg)) {
                    targets.Add(arg);
                } else {
                    Monitor.Log($"Argument '{arg}' malformed. Command aborted.");
                    return;
                }
            }

            if (targets.Any()) {
                _loggers.Set(targets, action);
            } else if (action != ToggleAction.Toggle) {
                targets = validIds;
                _loggers.Set(targets, action);
            }

            var message = new StringBuilder("Enabled:");
            foreach (var logger in _loggers.Where(l => l.IsEnabled).OrderBy(l => l.Id)) {
                message.Append($" {(targets.Contains(logger.Id) ? "*" : "")}{logger.Id}");
            }

            Monitor.Log(message.ToString(), LogLevel.Info);

            message = new StringBuilder("Disabled:");
            foreach (var logger in _loggers.Where(l => !l.IsEnabled).OrderBy(l => l.Id)) {
                message.Append($" {(targets.Contains(logger.Id) ? "*" : "")}{logger.Id}");
            }

            Monitor.Log(message.ToString(), LogLevel.Info);
        }


        [NotNull]
        private ToggleableEventLogger<T> BuildLogger<T>([NotNull] string name, Func<T, string> message)
            where T : EventArgs {
            var logger = new ToggleableEventLogger<T>(name, Monitor, message);
            _loggers.Add(logger);
            return logger;
        }


        private void BuildLogger_World_DebrisListChanged() {

            var logger = BuildLogger("debris", (DebrisListChangedEventArgs args) =>
                $"World.DebrisListChanged {args.Location.Name} +{args.Added.Count()} -{args.Removed.Count()}");
            Helper.Events.World.DebrisListChanged += logger.OnEvent;
        }


        private void BuildLogger_World_ObjectListChanged() {
            var logger = BuildLogger("objects",
                (ObjectListChangedEventArgs args) =>
                    $"World.ObjectListChanged {args.Location.Name} +{args.Added.Count()} -{args.Removed.Count()}");
            Helper.Events.World.ObjectListChanged += logger.OnEvent;
        }


        private void BuildLogger_World_LocationListChanged() {
            var logger = BuildLogger("locations",
                (LocationListChangedEventArgs args) =>
                    $"World.LocationListChanged +{args.Added.Count()} -{args.Removed.Count()}");
            Helper.Events.World.LocationListChanged += logger.OnEvent;
        }


        private void BuildLogger_World_TerrainFeatureListChanged() {
            var logger = BuildLogger("terrain", (TerrainFeatureListChangedEventArgs args) =>
                $"World.TerrainFeatureListChanged {args.Location.Name} +{args.Added.Count()} -{args.Removed.Count()}");
            Helper.Events.World.TerrainFeatureListChanged += logger.OnEvent;
        }


        private void BuildLogger_World_LargeTerrainFeatureListChanged() {
            var logger = BuildLogger("large-terrain", (LargeTerrainFeatureListChangedEventArgs args) =>
                $"World.LargeTerrainFeatureListChanged {args.Location.Name} +{args.Added.Count()} -{args.Removed.Count()}");
            Helper.Events.World.LargeTerrainFeatureListChanged += logger.OnEvent;
        }


        private void BuildLogger_GameLoop_Saving() {
            var logger = BuildLogger("saving", (SavingEventArgs args) => "GameLoop.Saving");
            Helper.Events.GameLoop.Saving += logger.OnEvent;
        }


        private void BuildLogger_GameLoop_Saved() {
            var logger = BuildLogger("saved", (SavedEventArgs args) => "GameLoop.Saved");
            Helper.Events.GameLoop.Saved += logger.OnEvent;
        }


        private void BuildLogger_GameLoop_SaveLoaded() {
            var logger = BuildLogger("save-loaded", (SaveLoadedEventArgs args) => "GameLoop.SaveLoaded");
            Helper.Events.GameLoop.SaveLoaded += logger.OnEvent;
        }


        private void BuildLogger_GameLoop_DayStarted() {
            var logger = BuildLogger("day-started", (DayStartedEventArgs args) => "GameLoop.DayStarted");
            Helper.Events.GameLoop.DayStarted += logger.OnEvent;
        }


        private void BuildLogger_GameLoop_DayEnding() {
            var logger = BuildLogger("day-ending", (DayEndingEventArgs args) => "GameLoop.DayEnding");
            Helper.Events.GameLoop.DayEnding += logger.OnEvent;
        }


    }


    internal enum ToggleAction {
        Enable,
        Disable,
        Toggle
    }


    internal class ToggleableEventLoggerCollection : IEnumerable<IToggleableEventLogger> {
        private readonly IDictionary<string, IToggleableEventLogger> _loggers =
            new Dictionary<string, IToggleableEventLogger>();

        public IEnumerator<IToggleableEventLogger> GetEnumerator() => _loggers.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _loggers.Values).GetEnumerator();


        public void Add([NotNull] IToggleableEventLogger item) {
            _loggers[item.Id] = item;
        }


        public void Set([NotNull] IEnumerable<string> loggers, ToggleAction action) {
            foreach (var logger in loggers) {
                _loggers[logger].Set(action);
            }
        }


        [NotNull] public IEnumerable<string> Ids => _loggers.Keys;
    }


    internal interface IToggleableEventLogger {

        bool IsEnabled { get; }

        string Id { get; }

        void Set(ToggleAction action);
    }


    internal class ToggleableEventLogger<TArgs> : IToggleableEventLogger where TArgs : EventArgs {
        private readonly IMonitor _monitor;
        private readonly Func<TArgs, string> _message;
        public string Id { get; }

        public bool IsEnabled { get; private set; }


        public ToggleableEventLogger([NotNull] string id, IMonitor monitor, Func<TArgs, string> message) {
            if (string.IsNullOrEmpty(id)) {
                throw new ArgumentNullException($"invalid event logger id: `{id}`");
            }

            Id = id;
            _message = message;
            _monitor = monitor;
        }


        public void OnEvent(object sender, TArgs args) {
            if (IsEnabled) {
                _monitor.Log(_message(args), LogLevel.Info);
            }
        }


        public void Set(ToggleAction action) {
            switch (action) {
                case ToggleAction.Enable:
                    IsEnabled = true;
                    break;
                case ToggleAction.Disable:
                    IsEnabled = false;
                    break;
                case ToggleAction.Toggle:
                    IsEnabled = !IsEnabled;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }
    }

}