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
using WarpNetwork.models;

namespace WarpNetwork.api
{
	public class API : IWarpNetAPI
	{
		public void AddCustomDestinationHandler(string ID, IWarpNetHandler handler)
		{
			Utils.CustomLocs.Remove(ID);
			Utils.CustomLocs.Add(ID, handler);
		}
		public void AddCustomDestinationHandler(string ID, Func<bool> getEnabled, Func<string> getLabel, Func<string> getIconName, Action warp)
		{
			Utils.CustomLocs.Remove(ID);
			Utils.CustomLocs.Add(ID, new WarpNetHandler(getEnabled, getIconName, getLabel, warp));
		}
		public bool CanWarpTo(string ID)
		{
			Dictionary<string, WarpLocation> dict = Utils.GetWarpLocations();
			if (dict.ContainsKey(ID))
				return dict[ID].Enabled;
			return false;
		}
		public string[] GetItems() => Utils.GetWarpItems().Keys.ToArray();
		public string[] GetDestinations() => Utils.GetWarpLocations().Keys.ToArray();
		public bool DestinationExists(string ID) => GetDestinations().Contains(ID, StringComparer.OrdinalIgnoreCase);
		public bool DestinationIsCustomHandler(string ID) => Utils.CustomLocs.ContainsKey(ID);
		public void RemoveCustomDestinationHandler(string ID) => Utils.CustomLocs.Remove(ID);
		public void ShowWarpMenu(bool force = false) => ShowWarpMenu(force ? "_force" : "");
		public void ShowWarpMenu(string Exclude) => WarpHandler.ShowWarpMenu(Exclude);
		public bool WarpTo(string ID) => WarpHandler.DirectWarp(ID, true);
	}
}
