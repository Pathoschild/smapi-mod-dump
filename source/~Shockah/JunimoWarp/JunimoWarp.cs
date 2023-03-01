/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Shockah.CommonModCode.GMCM;
using Shockah.Kokoro;
using Shockah.Kokoro.GMCM;
using Shockah.Kokoro.SMAPI;
using Shockah.Kokoro.Stardew;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.JunimoWarp
{
	public class JunimoWarp : BaseMod<ModConfig>
	{
		internal static JunimoWarp Instance { get; private set; } = null!;

		private readonly PerScreen<Dictionary<Guid, Action<GameLocation, IntPoint>>> AwaitingNextWarpResponse = new(() => new());

		public override void OnEntry(IModHelper helper)
		{
			Instance = this;

			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;

			var harmony = new Harmony(ModManifest.UniqueID);
			ItemGrabMenuPatches.Apply(harmony);
		}

		private void SetupConfig()
		{
			var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
			if (api is null)
				return;
			GMCMI18nHelper helper = new(api, ModManifest, Helper.Translation);

			api.Register(
				ModManifest,
				reset: () => Config = new(),
				save: () =>
				{
					WriteConfig();
					LogConfig();
				}
			);

			helper.AddBoolOption("config.requiredEmptyChest", () => Config.RequiredEmptyChest);
		}

		private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
		{
			SetupConfig();
		}

		private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
		{
			if (e.FromModID != ModManifest.UniqueID)
				return;

			if (GameExt.GetMultiplayerMode() == MultiplayerMode.Client)
			{
				if (e.Type == typeof(NetMessage.NextWarpResponse).FullName)
				{
					var message = e.ReadAs<NetMessage.NextWarpResponse>();
					if (!AwaitingNextWarpResponse.Value.TryGetValue(message.ID, out var callback))
					{
						Monitor.Log($"Received message of type {e.Type}, but did not expect one - ignoring it.", LogLevel.Warn);
						return;
					}
					AwaitingNextWarpResponse.Value.Remove(message.ID);
					callback(Game1.getLocationFromName(message.LocationName), message.Point);
					return;
				}
			}
			else
			{
				if (e.Type == typeof(NetMessage.NextWarpRequest).FullName)
				{
					var message = e.ReadAs<NetMessage.NextWarpRequest>();
					var (warpLocation, warpPoint) = GetNextWarp(Game1.getLocationFromName(message.LocationName), message.Point);
					Helper.Multiplayer.SendMessage(
						new NetMessage.NextWarpResponse(message.ID, warpLocation.NameOrUniqueName, warpPoint),
						new[] { Instance.ModManifest.UniqueID },
						new[] { e.FromPlayerID }
					);
					return;
				}
			}

			Monitor.Log($"Received message of type {e.Type}, but did not expect one - ignoring it.", LogLevel.Warn);
		}

		private static IEnumerable<(GameLocation Location, IntPoint Point)> GetPossibleWarpLocations()
		{
			foreach (var location in GameExt.GetAllLocations())
				if (location.NameOrUniqueName is not null)
					foreach (var @object in location.Objects.Values.OrderBy(o => o.TileLocation.Y).ThenBy(o => o.TileLocation.X))
						if (@object is Chest chest && chest.SpecialChestType == Chest.SpecialChestTypes.JunimoChest)
							yield return (location, new((int)@object.TileLocation.X, (int)@object.TileLocation.Y));
		}

		private static (GameLocation Location, IntPoint Point) GetNextWarp(GameLocation location, IntPoint point)
		{
			var allWarps = GetPossibleWarpLocations().ToList();
			for (int i = 0; i < allWarps.Count; i++)
				if (allWarps[i].Location == location && allWarps[i].Point == point)
				{
					var warp = allWarps[(i + 1) % allWarps.Count];
					foreach (var neighbor in new IntPoint[] { warp.Point + IntPoint.Bottom, warp.Point + IntPoint.Top, warp.Point + IntPoint.Left, warp.Point + IntPoint.Right })
						if (warp.Location.isTileLocationTotallyClearAndPlaceableIgnoreFloors(new(neighbor.X, neighbor.Y)))
							return (warp.Location, neighbor);
				}
			return (location, point);
		}

		internal void RequestNextWarp(GameLocation location, IntPoint point, Action<GameLocation, IntPoint> callback)
		{
			if (GameExt.GetMultiplayerMode() == MultiplayerMode.Client)
			{
				var messageID = Guid.NewGuid();
				AwaitingNextWarpResponse.Value[messageID] = callback;
				Helper.Multiplayer.SendMessage(
					new NetMessage.NextWarpRequest(messageID, location.NameOrUniqueName, point),
					new[] { Instance.ModManifest.UniqueID },
					new[] { GameExt.GetHostPlayer().UniqueMultiplayerID }
				);
			}
			else
			{
				var (warpLocation, warpPoint) = GetNextWarp(location, point);
				callback(warpLocation, warpPoint);
			}
		}

		internal static void AnimatePlayerWarp(GameLocation location, IntPoint point)
		{
			for (int i = 0; i < 12; i++)
				GameExt.Multiplayer.broadcastSprites(Game1.player.currentLocation, new TemporaryAnimatedSprite(354, Game1.random.Next(25, 75), 6, 1, new Vector2(Game1.random.Next((int)Game1.player.position.X - 256, (int)Game1.player.position.X + 192), Game1.random.Next((int)Game1.player.position.Y - 256, (int)Game1.player.position.Y + 192)), flicker: false, Game1.random.NextDouble() < 0.5));
			Game1.player.currentLocation.playSound("wand");
			Game1.displayFarmer = false;
			Game1.player.temporarilyInvincible = true;
			Game1.player.temporaryInvincibilityTimer = -2000;
			Game1.player.Halt();
			Game1.player.faceDirection(2);
			Game1.player.CanMove = false;
			Game1.player.freezePause = 2000;
			Game1.flashAlpha = 1f;

			DelayedAction.fadeAfterDelay(() =>
			{
				Game1.warpFarmer(location.NameOrUniqueName, point.X, point.Y, false);
				if (!Game1.isStartingToGetDarkOut() && !Game1.isRaining)
					Game1.playMorningSong();
				else
					Game1.changeMusicTrack("none");
				Game1.fadeToBlackAlpha = 0.99f;
				Game1.screenGlow = false;
				Game1.player.temporarilyInvincible = false;
				Game1.player.temporaryInvincibilityTimer = 0;
				Game1.displayFarmer = true;
				Game1.player.CanMove = true;
			}, 1000);

			int j = 0;
			for (int xTile = Game1.player.getTileX() + 8; xTile >= Game1.player.getTileX() - 8; xTile--)
			{
				GameExt.Multiplayer.broadcastSprites(Game1.player.currentLocation, new TemporaryAnimatedSprite(6, new Vector2(xTile, Game1.player.getTileY()) * 64f, Color.White, 8, flipped: false, 50f)
				{
					layerDepth = 1f,
					delayBeforeAnimationStart = j * 25,
					motion = new Vector2(-0.25f, 0f)
				});
				j++;
			}
		}
	}
}