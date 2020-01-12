using System;
using System.Collections.Generic;
using ModSettingsTabApi.Events;
using ModSettingsTabApi.Framework.Interfaces;

namespace ModSettingsTab.Framework
{
    public class SettingsTabApi : ISettingsTabApi
    {
        public event EventHandler<OptionsChangedEventArgs> OptionsChanged;

        public bool Send(object sender,Dictionary<string, Value> options)
        {
            var eventArgs = new OptionsChangedEventArgs(options);

            OptionsChanged?.Invoke(sender, eventArgs);

            return eventArgs.Reloaded;
        }
    }
}