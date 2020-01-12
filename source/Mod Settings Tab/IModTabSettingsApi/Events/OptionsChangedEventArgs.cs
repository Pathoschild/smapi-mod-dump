using System;
using System.Collections.Generic;

namespace ModSettingsTabApi.Events
{
    public class Value
    {
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
    public class OptionsChangedEventArgs : EventArgs
    {
        public Dictionary<string, Value> Options;
        public bool Reloaded { get; set; }

        public OptionsChangedEventArgs(Dictionary<string, Value> options)
        {
            Options = options;
            Reloaded = false;
        }
    }
}