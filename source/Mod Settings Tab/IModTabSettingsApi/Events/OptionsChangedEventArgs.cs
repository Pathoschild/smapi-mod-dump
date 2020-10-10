/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GilarF/SVM
**
*************************************************/

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