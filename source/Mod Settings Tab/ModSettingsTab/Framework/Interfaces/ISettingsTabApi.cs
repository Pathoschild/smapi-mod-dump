using System;
using ModSettingsTab.Events;

namespace ModSettingsTab.Framework.Interfaces
{
    public interface ISettingsTabApi
    {
        event EventHandler<OptionsChangedEventArgs> OptionsChanged;
    }
}