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
using System.Text;
using System.Threading.Tasks;
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
		private ModConfig _config;

		private readonly HashSet<Bush> _shakenBushes = new();

		private int _treesShaken;
		private int _fruitTressShaken;

		/// <summary>
		/// The mod entry point, called after the mod is first loaded.
		/// </summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			I18n.Init(helper.Translation);
			_config = helper.ReadConfig<ModConfig>();

			helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
			helper.Events.GameLoop.DayEnding += OnDayEnding;
			helper.Events.Input.ButtonsChanged += OnButtonsChanged;
			helper.Events.GameLoop.GameLaunched += (_,_) => _config.RegisterModConfigMenu(helper, ModManifest);
		}

		private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (!Context.IsWorldReady) return;
			if (!_config.IsShakerActive || (!_config.ShakeRegularTrees && !_config.ShakeFruitTrees && !_config.ShakeBushes)) return;
			if (Game1.currentLocation == null || Game1.player == null) return;
			if (Game1.CurrentEvent != null && (!Game1.CurrentEvent.playerControlSequence || !Game1.CurrentEvent.canPlayerUseTool())) return;
			if (Game1.player.currentLocation.terrainFeatures.Count() == 0 &&
				Game1.player.currentLocation.largeTerrainFeatures.Count == 0) return;

			var playerTileLocationPoint = Game1.player.getTileLocationPoint();
			var playerMagnetism = Game1.player.GetAppliedMagneticRadius();

			var radius = _config.UsePlayerMagnetism ? playerMagnetism / Game1.tileSize : _config.ShakeDistance;

			if (Game1.player.currentLocation.terrainFeatures.Count() > 0)
			{
				foreach (Vector2 vec in GetTilesToCheck(playerTileLocationPoint, radius))
				{
					if (Game1.currentLocation.terrainFeatures.TryGetValue(vec, out var feature)
						&& feature is Tree or FruitTree or Bush)
					{
						var featureTileLocation = feature.currentTileLocation;

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
							case Bush bushFeature when _shakenBushes.Contains(bushFeature):
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
			}

			if (_config.ShakeBushes)
			{
				foreach (var feature in Game1.player.currentLocation.largeTerrainFeatures)
				{
					if (feature is not Bush bush) continue;
					var location = bush.tilePosition;

					if (!IsInShakeRange(playerTileLocationPoint, location, radius) || _shakenBushes.Contains(bush)) continue;

					if (!bush.townBush.Value && bush.isActionable() && bush.inBloom(Game1.currentSeason, Game1.dayOfMonth))
					{
						bush.performUseAction(location, Game1.currentLocation);
						_shakenBushes.Add(bush);
					}
				}
			}
		}

		private void OnDayEnding(object sender, DayEndingEventArgs e)
		{
			if (_config.IsShakerActive)
			{
				StringBuilder statMessage = new(Utility.getDateString());
				statMessage.Append(':');

				if (_treesShaken == 0 && _fruitTressShaken == 0 && _shakenBushes.Count == 0)
				{
					statMessage.Append("Nothing shaken today.");
				}
				else
				{

					if (_config.ShakeRegularTrees) statMessage.Append($"\n\t[{_treesShaken}] Trees shaken");
					if (_config.ShakeFruitTrees) statMessage.Append($"\n\t[{_fruitTressShaken}] Fruit Trees shaken");
					if (_config.ShakeBushes) statMessage.Append($"\n\t[{_shakenBushes.Count}] Bushes shaken");

					Monitor.Log("Resetting daily counts...");
					_shakenBushes.Clear();
					_treesShaken = 0;
					_fruitTressShaken = 0;
				}

				Monitor.Log(statMessage.ToString(), LogLevel.Info);
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
					Task.Run(() => Helper.WriteConfig(_config)).ContinueWith((t) =>
						this.Monitor.Log(t.Status == TaskStatus.RanToCompletion
							? "Config saved successfully!"
							: $"Saving config unsuccessful {t.Status}"));

					var message = "AutoShaker has been " + (_config.IsShakerActive ? "ACTIVATED" : "DEACTIVATED");

					Monitor.Log(message, LogLevel.Info);
					Game1.addHUDMessage(new HUDMessage(message, null));
				}
			}
		}

		private static bool IsInShakeRange(Point playerLocation, Vector2 bushLocation, int radius)
			=> Math.Abs(bushLocation.X - playerLocation.X) <= radius && Math.Abs(bushLocation.Y - playerLocation.Y) <= radius;

		private static IEnumerable<Vector2> GetTilesToCheck(Point playerLocation, int radius)
		{
			for (int x = Math.Max(playerLocation.X - radius, 0); x <= playerLocation.X + radius; x++)
				for (int y = Math.Max(playerLocation.Y - radius, 0); y <= playerLocation.Y + radius; y++)
					yield return new Vector2(x, y);

			yield break;
		}
	}
}
