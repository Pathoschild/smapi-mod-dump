/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpNetwork
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;

namespace WarpNetwork.api
{
	public interface IWarpNetAPI
	{
		void AddCustomDestinationHandler(string ID, IDestinationHandler handler);
		void RemoveCustomDestinationHandler(string ID);
		bool CanWarpTo(string ID, GameLocation where = null, Farmer who = null);
		bool DestinationExists(string ID);
		bool DestinationIsCustomHandler(string ID);
		bool WarpTo(string ID, GameLocation where = null, Farmer who = null);
		void ShowWarpMenu(bool Force = false, GameLocation where = null, Farmer who = null);
		void ShowWarpMenu(string Exclude, GameLocation where = null, Farmer who = null);
		string[] GetDestinations();
		string[] GetItems();
		void DoWarpEffects(Action doActual, Farmer who, GameLocation where);

		public interface IDestinationHandler
		{
			public string Label { get; }
			public Texture2D Icon { get; }
			public bool Activate(GameLocation location, Farmer who);
			public bool IsAccessible(GameLocation location, Farmer who);
			public bool IsVisible(GameLocation location, Farmer who);
			public void AfterWarp(string location, Point tile, IDestinationHandler handler);
		}
	}
}
