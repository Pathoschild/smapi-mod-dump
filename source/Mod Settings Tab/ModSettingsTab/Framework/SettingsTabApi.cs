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