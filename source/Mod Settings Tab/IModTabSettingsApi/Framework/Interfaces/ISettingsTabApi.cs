using System;
using ModSettingsTabApi.Events;

namespace ModSettingsTabApi.Framework.Interfaces
{
    public interface ISettingsTabApi
    {
        event EventHandler<OptionsChangedEventArgs> OptionsChanged;
    }
}