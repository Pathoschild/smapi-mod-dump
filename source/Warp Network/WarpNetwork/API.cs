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
using System.Collections.Generic;
using System.Linq;

namespace WarpNetwork
{
    public class API : IWarpNetAPI
    {
        public void AddCustomDestinationHandler(string ID, Action<string> onWarp, Func<string, bool> getEnabled, Func<string, string> GetLabel)
        {
            if (WarpHandler.CustomLocs.ContainsKey(ID))
            {
                WarpHandler.CustomLocs.Remove(ID);
            }
            WarpHandler.CustomLocs.Add(ID, new CustomLocationHandler(onWarp, getEnabled, GetLabel));
        }
        public void AddCustomDestinationHandler(string ID, Action<string> onWarp, bool enabled, string Label)
        {
            AddCustomDestinationHandler(ID, onWarp, (s) => enabled, (s) => Label);
        }
        public bool CanWarpTo(string ID)
        {
            if (WarpHandler.CustomLocs.ContainsKey(ID))
            {
                return WarpHandler.CustomLocs[ID].GetEnabled(ID);
            }
            Dictionary<string, WarpLocation> dict = Utils.GetWarpLocations();
            if (dict.ContainsKey(ID))
            {
                return dict[ID].Enabled;
            }
            return false;
        }
        public string[] GetItems()
        {
            return Utils.GetWarpItems().Keys.ToArray();
        }
        public string[] GetDestinations()
        {
            return Utils.GetWarpLocations().Keys.Concat(WarpHandler.CustomLocs.Keys).ToArray();
        }
        public bool DestinationExists(string ID)
        {
            return GetDestinations().Contains(ID, StringComparer.OrdinalIgnoreCase);
        }
        public bool DestinationIsCustomHandler(string ID)
        {
            return WarpHandler.CustomLocs.ContainsKey(ID);
        }
        public void RemoveCustomDestinationHandler(string ID)
        {
            WarpHandler.CustomLocs.Remove(ID);
        }
        public void ShowWarpMenu(bool force = false)
        {
            ShowWarpMenu(force ? "_force" : "");
        }
        public void ShowWarpMenu(string Exclude)
        {
            WarpHandler.ShowWarpMenu(Exclude);
        }
        public bool WarpTo(string ID)
        {
            return WarpHandler.DirectWarp(ID, true);
        }
        public void AddDestination(string ID, string Location, int X, int Y, bool DefaultEnabled, string Label)
        {
            AddDestination(ID, Location, X, Y, DefaultEnabled, Label, false, false);
        }
        public void AddDestination(string ID, string Location, int X, int Y, bool DefaultEnabled, string Label, bool OverrideMapProperty, bool AlwaysHide)
        {
            DataPatcher.ApiLocs[ID] = new WarpLocation
            {
                Enabled = DefaultEnabled,
                Label = Label,
                X = X,
                Y = Y,
                OverrideMapProperty = OverrideMapProperty,
                Location = Location,
                AlwaysHide = AlwaysHide
            };
        }
        public void RemoveDestination(string ID)
        {
            DataPatcher.ApiLocs.Remove(ID);
        }
        public void SetDestinationEnabled(string ID, bool Enabled)
        {
            if (DataPatcher.ApiLocs.ContainsKey(ID))
            {
                DataPatcher.ApiLocs[ID].Enabled = Enabled;
            }
        }
        public void AddWarpItem(int ObjectID, string Destination, string Color)
        {
            AddWarpItem(ObjectID, Destination, Color, false);
        }
        public void AddWarpItem(int ObjectID, string Destination, string Color, bool IgnoreDisabled)
        {
            AddWarpItem(ObjectID, Destination, Color, IgnoreDisabled, true);
        }
        public void AddWarpItem(int ObjectID, string Destination, string Color, bool IgnoreDisabled, bool Consume)
        {
            DataPatcher.ApiItems[ObjectID.ToString()] = new WarpItem
            {
                Destination = Destination,
                IgnoreDisabled = IgnoreDisabled,
                Color = Color,
                Consume = Consume
            };
        }
        public void RemoveWarpItem(int ObjectID)
        {
            DataPatcher.ApiItems.Remove(ObjectID.ToString());
        }
        public void SetDestinationLocation(string ID, string Location)
        {
            if (DataPatcher.ApiLocs.ContainsKey(ID))
            {
                DataPatcher.ApiLocs[ID].Location = Location;
            }
        }
        public void SetDestinationPosition(string ID, int X, int Y)
        {
            if (DataPatcher.ApiLocs.ContainsKey(ID))
            {
                DataPatcher.ApiLocs[ID].X = X;
                DataPatcher.ApiLocs[ID].Y = Y;
            }
        }
        public void SetDestinationLabel(string ID, string Label)
        {
            if (DataPatcher.ApiLocs.ContainsKey(ID))
            {
                DataPatcher.ApiLocs[ID].Label = Label;
            }
        }
        public void SetDestinationOverrideMap(string ID, bool OverrideMapProperty)
        {
            if (DataPatcher.ApiLocs.ContainsKey(ID))
            {
                DataPatcher.ApiLocs[ID].OverrideMapProperty = OverrideMapProperty;
            }
        }
        public void SetDestinationAlwaysHidden(string ID, bool AlwaysHide)
        {
            if (DataPatcher.ApiLocs.ContainsKey(ID))
            {
                DataPatcher.ApiLocs[ID].AlwaysHide = AlwaysHide;
            }
        }
        public void SetWarpItemDestination(int ObjectID, string Destination)
        {
            string ID = ObjectID.ToString();
            if (DataPatcher.ApiItems.ContainsKey(ID))
            {
                DataPatcher.ApiItems[ID].Destination = Destination;
            }
        }
        public void SetWarpItemColor(int ObjectID, string Color)
        {
            string ID = ObjectID.ToString();
            if (DataPatcher.ApiItems.ContainsKey(ID))
            {
                DataPatcher.ApiItems[ID].Color = Color;
            }
        }
        public void SetWarpItemIgnoreDisabled(int ObjectID, bool IgnoreDIsabled)
        {
            string ID = ObjectID.ToString();
            if (DataPatcher.ApiItems.ContainsKey(ID))
            {
                DataPatcher.ApiItems[ID].IgnoreDisabled = IgnoreDIsabled;
            }
        }

        public void SetWarpItemConsume(int ObjectID, bool Consume)
        {
            string ID = ObjectID.ToString();
            if (DataPatcher.ApiItems.ContainsKey(ID))
            {
                DataPatcher.ApiItems[ID].Consume = Consume;
            }
        }
    }
}
