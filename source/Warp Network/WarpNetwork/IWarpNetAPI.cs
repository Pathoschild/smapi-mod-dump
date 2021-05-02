/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpNetwork
**
*************************************************/

using System;

namespace WarpNetwork
{
    public interface IWarpNetAPI
    {
        void AddDestination(string ID, string Location, int X, int Y, bool DefaultEnabled, string Label);
        void AddDestination(string ID, string Location, int X, int Y, bool DefaultEnabled, string Label, bool OverrideMapProperty, bool AlwaysHide);
        void RemoveDestination(string ID);
        void SetDestinationEnabled(string ID, bool Enabled);
        void SetDestinationLocation(string ID, string Location);
        void SetDestinationPosition(string ID, int X, int Y);
        void SetDestinationLabel(string ID, string Label);
        void SetDestinationOverrideMap(string ID, bool OverrideMapProperty);
        void SetDestinationAlwaysHidden(string ID, bool AlwaysHide);
        void AddWarpItem(int ObjectID, string Destination, string Color);
        void AddWarpItem(int ObjectID, string Destination, string Color, bool IgnoreDisabled);
        void AddWarpItem(int ObjectID, string Destination, string Color, bool IgnoreDisabled, bool Consume);
        void RemoveWarpItem(int ObjectID);
        void SetWarpItemDestination(int ObjectID, string Destination);
        void SetWarpItemColor(int ObjectID, string Color);
        void SetWarpItemIgnoreDisabled(int ObjectID, bool IgnoreDIsabled);
        void SetWarpItemConsume(int ObjectID, bool Consume);
        void AddCustomDestinationHandler(string ID, Action<string> Warp, Func<string, bool> GetEnabled, Func<string, string> GetLabel);
        void AddCustomDestinationHandler(string ID, Action<string> Warp, bool Enabled, string Label);
        void RemoveCustomDestinationHandler(string ID);
        bool CanWarpTo(string ID);
        bool DestinationExists(string ID);
        bool DestinationIsCustomHandler(string ID);
        bool WarpTo(string ID);
        void ShowWarpMenu(bool Force = false);
        void ShowWarpMenu(string Exclude);
        string[] GetDestinations();
        string[] GetItems();
    }
}
