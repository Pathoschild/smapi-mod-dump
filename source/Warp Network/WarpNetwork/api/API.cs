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
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using WarpNetwork.framework;

namespace WarpNetwork.api
{
	public class API : IWarpNetAPI
	{
		internal static API api = new();

		public void AddCustomDestinationHandler(string ID, IWarpNetAPI.IDestinationHandler handler)
		{
			Utils.CustomLocs.Remove(ID);
			Utils.CustomLocs.Add(ID, handler);
		}

		public bool CanWarpTo(string ID, GameLocation where = null, Farmer who = null)
			=> Utils.GetWarpLocations().TryGetValue(ID, out var loc) && loc.IsAccessible(where, who);

		public string[] GetItems() 
			=> Utils.GetWarpItems().Keys.ToArray();

		public string[] GetDestinations()
			=> Utils.GetWarpLocations().Keys.ToArray();

		public bool DestinationExists(string ID) 
			=> GetDestinations().Contains(ID, StringComparer.OrdinalIgnoreCase);

		public bool DestinationIsCustomHandler(string ID) 
			=> Utils.CustomLocs.ContainsKey(ID);

		public void RemoveCustomDestinationHandler(string ID) 
			=> Utils.CustomLocs.Remove(ID);

		public void ShowWarpMenu(bool force = false, GameLocation where = null, Farmer who = null) 
			=> ShowWarpMenu(force ? "_force" : "");

		public void ShowWarpMenu(string Exclude, GameLocation where = null, Farmer who = null) 
			=> WarpHandler.ShowWarpMenu(where, who, Exclude);

		public bool WarpTo(string ID, GameLocation where = null, Farmer who = null) 
			=> WarpHandler.DirectWarp(ID, true, where, who);

		public void DoWarpEffects(Action action, Farmer who, GameLocation where)
		{
			for (int index = 0; index < 12; ++index)
				Game1.Multiplayer.broadcastSprites(where, new TemporaryAnimatedSprite(
					354,
					Game1.random.Next(25, 75), 6, 1,
					new Vector2(
						Game1.random.Next((int)who.Position.X - 256, (int)who.Position.X + 192),
						Game1.random.Next((int)who.Position.Y - 256, (int)who.Position.Y + 192)),
					false,
					Game1.random.NextDouble() < 0.5)
				);
			where.playSound("wand", who.Position);
			Game1.displayFarmer = false;
			who.temporarilyInvincible = true;
			who.temporaryInvincibilityTimer = -2000;
			who.freezePause = 1000;
			Game1.flashAlpha = 1f;
			int num = 0;
			var tile = who.TilePoint;
			for (int index = tile.X + 8; index >= tile.X - 8; --index)
			{
				Game1.Multiplayer.broadcastSprites(where, new TemporaryAnimatedSprite(6, new Vector2(index, tile.Y) * 64f, Color.White, 8, false, 50f, 0, -1, -1f, -1, 0)
				{
					layerDepth = 1f,
					delayBeforeAnimationStart = num * 25,
					motion = new Vector2(-0.25f, 0.0f)
				});
				++num;
			}

			DelayedAction.fadeAfterDelay(new Game1.afterFadeFunction(() =>
			{
				action();
				Game1.changeMusicTrack("none");
				Game1.fadeToBlackAlpha = 0.99f;
				Game1.screenGlow = false;
				Game1.player.temporarilyInvincible = false;
				Game1.player.temporaryInvincibilityTimer = 0;
				Game1.displayFarmer = true;
				WarpHandler.fromWand.Value = false;
			}), 1000);
		}
	}
}
