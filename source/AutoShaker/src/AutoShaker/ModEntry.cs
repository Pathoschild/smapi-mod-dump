/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jag3dagster/AutoShaker
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace AutoShaker
{
	/// <summary>
	/// The mod entry point.
	/// </summary>
	public class ModEntry : Mod
	{
		private const int SingleTileDistance = 64;

		private ModConfig _config;

		private readonly List<Bush> _shakenBushes = new List<Bush>();

		private int _treesShaken;
		private int _fruitTressShaken;

		/// <summary>
		/// The mod entry point, called after the mod is first loaded.
		/// </summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			_config = helper.ReadConfig<ModConfig>();

			helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
			helper.Events.GameLoop.DayEnding += OnDayEnding;
			helper.Events.Input.ButtonsChanged += OnButtonsChanged;
			helper.Events.GameLoop.GameLaunched += (_,__) => _config.RegisterModConfigMenu(helper, ModManifest);
		}

		private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (!Game1.game1?.IsActive ?? true) return;
			if (Game1.gameMode.Equals(Game1.loadingMode)) return;
			if (!Game1.hasLoadedGame) return;
			if (!_config.IsShakerActive || (!_config.ShakeRegularTrees && !_config.ShakeFruitTrees && !_config.ShakeBushes)) return;
			if (Game1.currentLocation == null || Game1.player == null) return;
			if (Game1.CurrentEvent != null && (!Game1.CurrentEvent.playerControlSequence || !Game1.CurrentEvent.canPlayerUseTool())) return;
			if (Game1.player.currentLocation.terrainFeatures.ToList().Count == 0 &&
				Game1.player.currentLocation.largeTerrainFeatures.ToList().Count == 0) return;

			var playerTileLocationPoint = Game1.player.getTileLocationPoint();
			var playerMagnetism = Game1.player.GetAppliedMagneticRadius();

			if (_config.ShakeRegularTrees || _config.ShakeFruitTrees)
			{
				var terrainFeatures = Game1.player.currentLocation.terrainFeatures.Pairs
					.Select(p => p.Value)
					.Where(v => v is Tree || v is FruitTree || v is Bush);

				foreach (var feature in terrainFeatures)
				{
					var featureTileLocation = feature.currentTileLocation;

					if (!IsInShakeRange(playerTileLocationPoint, featureTileLocation, playerMagnetism)) continue;

					switch (feature)
					{
						// Tree cases
						case Tree _ when !_config.ShakeRegularTrees:
							continue;
						case Tree treeFeature when treeFeature.stump.Value:
							continue;
						case Tree treeFeature when !treeFeature.hasSeed.Value:
							continue;
						case Tree treeFeature when !treeFeature.isActionable():
							continue;
						case Tree _ when Game1.player.ForagingLevel < 1:
							continue;
						case Tree treeFeature:
							treeFeature.performUseAction(featureTileLocation, Game1.player.currentLocation);
							_treesShaken += 1;
							break;

						// Fruit Tree cases
						case FruitTree _ when !_config.ShakeFruitTrees:
							continue;
						case FruitTree fruitTree when fruitTree.stump.Value:
							continue;
						case FruitTree fruitTree when fruitTree.fruitsOnTree.Value < _config.FruitsReadyToShake:
							continue;
						case FruitTree fruitTree when !fruitTree.isActionable():
							continue;
						case FruitTree fruitTree:
							fruitTree.performUseAction(featureTileLocation, Game1.player.currentLocation);
							_fruitTressShaken += 1;
							break;

						case Bush _ when !_config.ShakeTeaBushes:
							continue;
						case Bush bushFeature when bushFeature.townBush.Value:
							continue;
						case Bush bushFeature when !bushFeature.isActionable():
							continue;
						case Bush bushFeature when !bushFeature.inBloom(Game1.currentSeason, Game1.dayOfMonth):
							continue;
						case Bush bushFeature:
							bushFeature.performUseAction(featureTileLocation, Game1.player.currentLocation);
							_shakenBushes.Add(bushFeature);
							break;

						// This should never happen
						default:
							Monitor.Log("I am an unknown terrain feature, ignore me I guess...", LogLevel.Debug);
							break;
					}
				}
			}

			if (_config.ShakeBushes)
			{
				var largeBushes = Game1.player.currentLocation.largeTerrainFeatures.Where(feature => feature is Bush);

				foreach (var bush in largeBushes)
				{
					var location = bush.tilePosition;

					if (!IsInShakeRange(playerTileLocationPoint, location, playerMagnetism)) continue;
					if (_shakenBushes.Contains(bush)) continue;

					switch (bush)
					{
						// Large Bush cases
						case Bush bushFeature when bushFeature.townBush.Value:
							continue;
						case Bush bushFeature when !bushFeature.isActionable():
							continue;
						case Bush bushFeature when !bushFeature.inBloom(Game1.currentSeason, Game1.dayOfMonth):
							continue;
						case Bush bushFeature:
							bushFeature.performUseAction(location, Game1.player.currentLocation);
							_shakenBushes.Add(bushFeature);
							break;

						// This should never happen
						default:
							Monitor.Log("I am an unknown large terrain feature, ignore me I guess...", LogLevel.Debug);
							break;
					}
				}
			}
		}

		private void OnDayEnding(object sender, DayEndingEventArgs e)
		{
			if (_config.IsShakerActive)
			{
				var statMessage = $"{Utility.getDateString()}: ";

				if (_treesShaken == 0 && _fruitTressShaken == 0 && _shakenBushes.Count == 0)
				{
					statMessage += "Nothing shaken today.";
				}
				else
				{
					var stats = new List<string>();

					if (_config.ShakeRegularTrees) stats.Add($"[{_treesShaken}] Trees shaken");
					if (_config.ShakeFruitTrees) stats.Add($"[{_fruitTressShaken}] Fruit Trees shaken");
					if (_config.ShakeBushes) stats.Add($"[{_shakenBushes.Count}] Bushes shaken");

					if (stats.Count > 0) statMessage += String.Join("; ", stats);

					Monitor.Log("Resetting daily counts...");
					_shakenBushes.Clear();
					_treesShaken = 0;
					_fruitTressShaken = 0;
				}

				Monitor.Log(statMessage, LogLevel.Info);
			}
			else
			{
				Monitor.Log("AutoShaken is deactivated; nothing was nor will be shaken until it is reactivated.", LogLevel.Warn);
			}
		}

		private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
		{
			if (Game1.activeClickableMenu == null)
			{
				if (_config.ToggleShaker.JustPressed())
				{
					_config.IsShakerActive = !_config.IsShakerActive;
					Helper.WriteConfig(_config);

					var message = "AutoShaker has been " + (_config.IsShakerActive ? "ACTIVATED" : "DEACTIVATED");

					Monitor.Log(message, LogLevel.Info);
					Game1.addHUDMessage(new HUDMessage(message, null));
				}
			}
		}

		private bool IsInShakeRange(Point playerLocation, Vector2 bushLocation, int playerMagnetism)
		{
			var pickUpDistance = _config.ShakeDistance;

			if (_config.UsePlayerMagnetism)
			{
				pickUpDistance = (int)Math.Floor(playerMagnetism / (double)SingleTileDistance);
			}

			var inRange = Math.Abs(bushLocation.X - playerLocation.X) <= pickUpDistance &&
							Math.Abs(bushLocation.Y - playerLocation.Y) <= pickUpDistance;

			return inRange;
		}
	}
}
