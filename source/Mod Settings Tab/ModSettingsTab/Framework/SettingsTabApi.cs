using System;
using System.Collections.Generic;
using ModSettingsTab.Events;
using ModSettingsTab.Framework.Interfaces;

namespace ModSettingsTab.Framework
{
    public class SettingsTabApi : ISettingsTabApi
    {
        public event EventHandler<OptionsChangedEventArgs> OptionsChanged;

        public void Send(object sender,Dictionary<string, Value> options)
        {
            OptionsChanged?.Invoke(sender, new OptionsChangedEventArgs(options));
        }
    }
}