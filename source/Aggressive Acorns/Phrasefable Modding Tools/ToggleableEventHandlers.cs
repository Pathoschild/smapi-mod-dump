using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using StardewModdingAPI;

namespace Phrasefable_Modding_Tools
{
    internal enum ToggleAction
    {
        Enable,
        Disable,
        Toggle
    }


    internal class ToggleableEventHandler<TArgs> where TArgs : EventArgs
    {
        public bool IsEnabled { get; private set; }
        private readonly Action<object, TArgs> _handler;


        public ToggleableEventHandler(Action<object, TArgs> handler)
        {
            _handler = handler;
        }


        public void OnEvent(object sender, TArgs args)
        {
            if (IsEnabled) _handler.Invoke(sender, args);
        }


        public void Set(ToggleAction action)
        {
            switch (action)
            {
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


    internal interface IToggleableEventLogger
    {
        bool IsEnabled { get; }

        string Id { get; }

        void Set(ToggleAction action);
    }


    internal class ToggleableEventLogger<TArgs> : ToggleableEventHandler<TArgs>, IToggleableEventLogger
        where TArgs : EventArgs
    {
        public string Id { get; }


        public ToggleableEventLogger([NotNull] string id, IMonitor monitor, Func<TArgs, string> message)
            : base((s, args) => monitor.Log(message(args), LogLevel.Info))
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException($"invalid event logger id: `{id}`");
            }

            Id = id;
        }
    }


    internal class ToggleableEventLoggerCollection : IEnumerable<IToggleableEventLogger>
    {
        private readonly IDictionary<string, IToggleableEventLogger> _loggers =
            new Dictionary<string, IToggleableEventLogger>();

        public IEnumerator<IToggleableEventLogger> GetEnumerator() => _loggers.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _loggers.Values).GetEnumerator();


        public void Add([NotNull] IToggleableEventLogger item)
        {
            _loggers[item.Id] = item;
        }


        public void Set([NotNull] IEnumerable<string> loggers, ToggleAction action)
        {
            foreach (string logger in loggers)
            {
                _loggers[logger].Set(action);
            }
        }


        [NotNull] public IEnumerable<string> Ids => _loggers.Keys;
    }
}
