/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hedgehog-Technologies/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.FruitTrees;
using StardewValley.GameData.Locations;
using StardewValley.GameData.Objects;
using StardewValley.GameData.WildTrees;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using AutoForager.Classes;
using AutoForager.Extensions;
using AutoForager.Helpers;
using AutoForager.Integrations;

using SObject = StardewValley.Object;
using Constants = AutoForager.Helpers.Constants;
using Utilities = AutoForager.Helpers.Utilities;

namespace AutoForager
{
	/// <summary>
	/// The mod entry point.
	/// </summary>
	public class ModEntry : Mod
	{
		private ModConfig _config;
		private JsonHelper _jsonHelper;

		private bool _isForagerActive = true;
		private bool _gameStarted = false;
		private Vector2 _previousTilePosition;

		private readonly List<string> _overrideItemIds;
		private readonly List<string> _ignoreItemIds;

		private readonly ForageableItemTracker _forageableTracker;

		private readonly Dictionary<string, Dictionary<string, int>> _trackingCounts;

		private DateTime _nextErrorMessage;

		private readonly Dictionary<string, string> _cpForageables;
		private readonly Dictionary<string, string> _cpFruitTrees;
		private readonly Dictionary<string, string> _cpWildTrees;

		private BushBloomWrapper _bbw;
		private readonly Dictionary<string, string> _bushBloomItems;

		private CustomBushWrapper _cbw;
		private readonly Dictionary<string, string> _customTeaBushItems;

		#region Asset Cache

		private Dictionary<string, FruitTreeData> _fruitTreeCache = new();
		private Dictionary<string, FruitTreeData> FruitTreeCache
		{
			get => _fruitTreeCache;
			set
			{
				_fruitTreeCache = value;
				ParseAssets(value);
			}
		}

		private Dictionary<string, LocationData> _locationCache = new();
		private Dictionary<string, LocationData> LocationCache
		{
			get => _locationCache;
			set
			{
				_locationCache = value;
				ParseAssets(value);
			}
		}

		private Dictionary<string, ObjectData> _objectCache = new();
		private Dictionary<string, ObjectData> ObjectCache
		{
			get => _objectCache;
			set
			{
				_objectCache = value;
				ParseAssets(value);
			}
		}

		private Dictionary<string, WildTreeData> _wildTreeCache = new();
		private Dictionary<string, WildTreeData> WildTreeCache
		{
			get => _wildTreeCache;
			set
			{
				_wildTreeCache = value;
				ParseAssets(value);
			}
		}

		private void ParseAssets<T>(Dictionary<string, T> data)
		{
			if (data is Dictionary<string, FruitTreeData> fruitTreeData)
			{
				_forageableTracker.FruitTreeForageables.Clear();
				_forageableTracker.FruitTreeForageables.AddRange(ForageableItem.ParseFruitTreeData(fruitTreeData, _config?.ForageToggles[Constants.FruitTreeToggleKey], Monitor));
				_forageableTracker.FruitTreeForageables.SortByDisplayName();
				Monitor.Log("Parsing Fruit Tree Data", LogLevel.Debug);
			}
			else if (data is Dictionary<string, LocationData> locationData)
			{
				if (ObjectCache is null || ObjectCache.Count == 0)
				{
					ObjectCache = Game1.content.Load<Dictionary<string, ObjectData>>(Constants.ObjectsAssetName);
					Monitor.Log("Sub-Location: Grabbing Object Data", LogLevel.Debug);
				}

				_forageableTracker.ArtifactForageables.Clear();
				_forageableTracker.ArtifactForageables.AddRange(ForageableItem.ParseLocationData(ObjectCache, locationData, _config?.ForageToggles[Constants.ForagingToggleKey]));
				_forageableTracker.ArtifactForageables.SortByDisplayName();
				Monitor.Log("Parsing Location Data", LogLevel.Debug);
			}
			else if (data is Dictionary<string, ObjectData> objectData)
			{
				var parsedObjectForageableItems = ForageableItem.ParseObjectData(objectData, _config, Monitor);

				_forageableTracker.ObjectForageables.Clear();
				_forageableTracker.ObjectForageables.AddRange(parsedObjectForageableItems.Item1);
				_forageableTracker.ObjectForageables.SortByDisplayName();

				_forageableTracker.BushForageables.Clear();
				_forageableTracker.BushForageables.AddRange(parsedObjectForageableItems.Item2);
				_forageableTracker.BushForageables.SortByDisplayName();

				Monitor.Log("Parsing Object Data", LogLevel.Debug);

				if (LocationCache is not null && LocationCache.Count > 0)
				{
					_forageableTracker.ArtifactForageables.Clear();
					_forageableTracker.ArtifactForageables.AddRange(ForageableItem.ParseLocationData(objectData, LocationCache, _config?.ForageToggles[Constants.ForagingToggleKey]));
					_forageableTracker.ArtifactForageables.SortByDisplayName();
					Monitor.Log("Sub-Object: Parsing Location Data", LogLevel.Debug);
				}
			}
			else if (data is Dictionary<string, WildTreeData> wildTreeData)
			{
				_forageableTracker.WildTreeForageables.Clear();
				_forageableTracker.WildTreeForageables.AddRange(ForageableItem.ParseWildTreeData(wildTreeData, _config?.ForageToggles[Constants.WildTreeToggleKey], Monitor));
				_forageableTracker.WildTreeForageables.SortByDisplayName();
				Monitor.Log("Parsing Wild Tree Data", LogLevel.Debug);
			}

			if (_config is not null && _gameStarted)
			{
				Monitor.Log("Reregistering Generic Mod Config Menu", LogLevel.Debug);
				_config.RegisterModConfigMenu(Helper, ModManifest);
			}
		}

		#endregion Asset Cache

		public ModEntry()
		{
			_config = new();
			_jsonHelper = new();
			_gameStarted = false;

			_cpForageables = new();
			_cpFruitTrees = new();
			_cpWildTrees = new();
			_bushBloomItems = new();
			_customTeaBushItems = new();

			_overrideItemIds = new()
			{
				"(O)152", // Seaweed
				"(O)416", // Snow Yam
				"(O)430", // Truffle
				"(O)851", // Magma Cap
				"(O)Moss" // Moss
			};

			_ignoreItemIds = new()
			{
				"(O)78"   // Cave Carrot
			};

			_forageableTracker = ForageableItemTracker.Instance;

			_trackingCounts = new()
			{
				{ Constants.BushKey, new() },
				{ Constants.ForageableKey, new() },
				{ Constants.FruitTreeKey, new() },
				{ Constants.WildTreeKey, new() }
			};

			_nextErrorMessage = DateTime.MinValue;
		}

		public override void Entry(IModHelper helper)
		{
			I18n.Init(helper.Translation);
			var packs = helper.ContentPacks.GetOwned();

			_config = helper.ReadConfig<ModConfig>();
			_config.UpdateUtilities(Monitor, packs, _jsonHelper);
			_config.UpdateEnabled(helper);

			ParseContentPacks(packs);

			helper.Events.Content.AssetReady += OnAssetReady;
			helper.Events.Content.AssetRequested += OnAssetRequested;
			helper.Events.Content.LocaleChanged += OnLocaleChanged;
			helper.Events.GameLoop.DayEnding += OnDayEnding;
			helper.Events.GameLoop.DayStarted += OnDayStarted;
			helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;
			helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
			helper.Events.Input.ButtonsChanged += OnButtonsChanged;
		}

		[EventPriority(EventPriority.Low)]
		private void OnAssetReady(object? sender, AssetReadyEventArgs e)
		{
			if (!_gameStarted) return;

			var name = e.Name.BaseName;

			if (name.IEquals(Constants.FruitTreesAssetName))
			{
				FruitTreeCache = Game1.content.Load<Dictionary<string, FruitTreeData>>(Constants.FruitTreesAssetName);
			}
			else if (name.IEquals(Constants.LocationsAssetName))
			{
				LocationCache = Game1.content.Load<Dictionary<string, LocationData>>(Constants.LocationsAssetName);
			}
			else if (name.IEquals(Constants.ObjectsAssetName))
			{
				ObjectCache = Game1.content.Load<Dictionary<string, ObjectData>>(Constants.ObjectsAssetName);
			}
			else if (name.IEquals(Constants.WildTreesAssetName))
			{
				WildTreeCache = Game1.content.Load<Dictionary<string, WildTreeData>>(Constants.WildTreesAssetName);
			}
		}

		[EventPriority(EventPriority.Low)]
		private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
		{
			var assetName = e.Name.BaseName;

			if (assetName.IEquals(Constants.FruitTreesAssetName))
			{
				e.Edit(EditFruitTrees);
			}
			else if (assetName.IEquals(Constants.ObjectsAssetName))
			{
				e.Edit(EditObjects);
			}
			else if (assetName.IEquals(Constants.WildTreesAssetName))
			{
				e.Edit(EditWildTrees);
			}
		}

		private void OnLocaleChanged(object? sender, LocaleChangedEventArgs e)
		{
			ItemRegistry.ResetCache();

			FruitTreeCache = Game1.content.Load<Dictionary<string, FruitTreeData>>(Constants.FruitTreesAssetName);
			WildTreeCache = Game1.content.Load<Dictionary<string, WildTreeData>>(Constants.WildTreesAssetName);
			ObjectCache = Game1.content.Load<Dictionary<string, ObjectData>>(Constants.ObjectsAssetName);
			LocationCache = Game1.content.Load<Dictionary<string, LocationData>>(Constants.LocationsAssetName);

			_config.UpdateUtilities(Monitor, Helper.ContentPacks.GetOwned(), _jsonHelper);
			_config.RegisterModConfigMenu(Helper, ModManifest);
			_config.UpdateEnabled(Helper);
		}

		private void OnDayEnding(object? sender, DayEndingEventArgs e)
		{
			StringBuilder statMessage = new($"{Environment.NewLine}{Utility.getDateString()}:{Environment.NewLine}");
			statMessage.AppendLine(I18n.Log_Eod_TotalStat(_trackingCounts.SumAll()));

			foreach (var category in _trackingCounts)
			{
				if (category.Value.Count == 0) continue;

				statMessage.AppendLine($"[{category.Value.SumAll()}] {Helper.Translation.Get(category.Key)}:");

				foreach (var interactable in category.Value)
				{
					if (interactable.Value <= 0)
					{
						Monitor.Log($"Invalid forageable value for {interactable.Key}; {interactable.Value}. How did we get here?", LogLevel.Warn);
						continue;
					}

					statMessage.AppendLine(I18n.Log_Eod_Stat(interactable.Value, interactable.Key));
				}

				category.Value.Clear();
			}

			Monitor.Log(statMessage.ToString(), LogLevel.Info);
		}

		private void OnDayStarted(object? sender, DayStartedEventArgs e)
		{
			_previousTilePosition = Game1.player.Tile;
		}

		[EventPriority(EventPriority.High)]
		private async void OnOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
		{
			if (IsTitleMenuInteractable())
			{
				Helper.Events.GameLoop.OneSecondUpdateTicked -= OnOneSecondUpdateTicked;

				_bbw = new BushBloomWrapper(Monitor, Helper);
				var schedules = await _bbw.UpdateSchedules();

				foreach (var sched in schedules)
				{
					var itemId = Utilities.GetItemIdFromName(sched.ItemId);

					if (itemId is not null)
					{
						if (_bushBloomItems.ContainsKey(itemId))
						{
							Monitor.LogOnce($"Already found an item with ItemId [{itemId}] with category [{_bushBloomItems[itemId]}] when trying to add category [{I18n.Category_BushBlooms()}]. Please verify you don't have duplicate or conflicting content packs.", LogLevel.Warn);
						}
						else
						{
							Monitor.LogOnce($"Found Bush Bloom Schedule for: [{itemId}]", LogLevel.Debug);
							_bushBloomItems.Add(itemId, "Category.BushBlooms");
						}
					}
				}

				_cbw = new CustomBushWrapper(Monitor, Helper);
				var customBushDrops = await _cbw.GetDrops();

				foreach (var drop in customBushDrops)
				{
					var itemId = Utilities.GetItemIdFromName(drop);

					if (itemId is not null)
					{
						if (_customTeaBushItems.ContainsKey(itemId))
						{
							Monitor.LogOnce($"Already found an item with ItemID [{itemId}] with category [{_customTeaBushItems[itemId]}] when trying to add category [{I18n.Category_CustomBushes()}]. Please verify you don't have duplicate or conflicting content packs.", LogLevel.Warn);
						}
						else
						{
							Monitor.LogOnce($"Found Custom Bush for: [{itemId}]", LogLevel.Debug);
							_customTeaBushItems.Add(itemId, "Category.Custombushes");
						}
					}
				}

				try
				{
					Helper.GameContent.InvalidateCache(Constants.ObjectsAssetName);
				}
				catch (Exception ex)
				{
					Monitor.Log($"{ex.Message}{Environment.NewLine}{ex.StackTrace}", LogLevel.Warn);
				}

				FruitTreeCache = Game1.content.Load<Dictionary<string, FruitTreeData>>(Constants.FruitTreesAssetName);
				WildTreeCache = Game1.content.Load<Dictionary<string, WildTreeData>>(Constants.WildTreesAssetName);
				ObjectCache = Game1.content.Load<Dictionary<string, ObjectData>>(Constants.ObjectsAssetName);
				LocationCache = Game1.content.Load<Dictionary<string, LocationData>>(Constants.LocationsAssetName);

				_config.RegisterModConfigMenu(Helper, ModManifest);
				_config.UpdateEnabled(Helper);

				Monitor.Log(_jsonHelper.Serialize(_config), LogLevel.Trace);

				_gameStarted = true;
			}
		}

		private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
		{
			if (!Context.IsPlayerFree) return;
			if (Game1.currentLocation is null || Game1.player is null) return;
			if (Game1.player.Tile.Equals(_previousTilePosition)) return;

			_previousTilePosition = Game1.player.Tile;
			var playerTileLocationPoint = Game1.player.TilePoint;
			var playerMagnetism = Game1.player.GetAppliedMagneticRadius();
			var radius = _config.UsePlayerMagnetism ? playerMagnetism / Game1.tileSize : _config.ShakeDistance;

			foreach (var vec in GetTilesToCheck(playerTileLocationPoint, radius))
			{
				if (Game1.currentLocation.terrainFeatures.TryGetValue(vec, out var feature))
				{
					switch (feature)
					{
						case Tree tree:
							if (tree.stump.Value) continue;
							if (tree.growthStage.Value < 5 || (!tree.hasSeed.Value && !tree.hasMoss.Value && (!tree.GetData().ShakeItems?.Any() ?? false))) continue;

							var seedItemIds = tree.GetSeedAndSeedItemIds();
							if (!tree.wasShakenToday.Value && (Game1.IsMultiplayer || Game1.player.ForagingLevel >= 1) && tree.isActionable()
								&& ((tree.hasSeed.Value && _forageableTracker.WildTreeForageables.Any(i => (seedItemIds.Contains(i.QualifiedItemId) || seedItemIds.Contains(i.ItemId)) && i.IsEnabled))
								|| (tree.GetData().ShakeItems?.Any(s => _forageableTracker.WildTreeForageables.Any(i => (s.ItemId.Contains(i.QualifiedItemId) || s.ItemId.Contains(i.ItemId)) && i.IsEnabled)) ?? false)))
							{
								tree.performUseAction(tree.Tile);
								Monitor.Log($"Tree shaken: {string.Join(", ", seedItemIds)}", LogLevel.Debug);

								foreach (var id in seedItemIds)
								{
									var name = id;

									if (_forageableTracker.WildTreeForageables.TryGetItem(id, out var wtItem))
									{
										name = wtItem?.DisplayName ?? id;
									}

									_trackingCounts[Constants.WildTreeKey].AddOrIncrement(name);
								}
							}
							else
							{
								Monitor.LogOnce($"Tree not shaken: {string.Join(",", seedItemIds)}; HasSeed: {tree.hasSeed.Value}", LogLevel.Trace);
							}

							if (tree.hasMoss.Value
								&& _forageableTracker.ObjectForageables.TryGetItem("(O)Moss", out var mossItem)
								&& (mossItem?.IsEnabled ?? false))
							{
								Tool? tool = new GenericTool();

								if (_config.RequireToolMoss)
								{
									tool = Game1.player.CurrentTool;
									tool ??= Game1.player.Items.FirstOrDefault(i => i is Tool, null) as Tool;

									if (tool is null)
									{
										if (_nextErrorMessage < DateTime.UtcNow)
										{
											Game1.addHUDMessage(new HUDMessage(I18n.Message_MissingToolMoss(), HUDMessage.error_type));
											_nextErrorMessage = DateTime.UtcNow.AddSeconds(10);
										}

										Monitor.LogOnce(I18n.Log_MissingToolMoss(I18n.Option_RequireToolMoss_Name(" ")), LogLevel.Info);
										continue;
									}
								}

								tool.lastUser = Game1.player;
								tree.performToolAction(tool, -1, tree.Tile);
								_trackingCounts[Constants.ForageableKey].AddOrIncrement(mossItem.DisplayName);
							}

							break;

						case FruitTree fruitTree:
							if (fruitTree.stump.Value) continue;
							if (fruitTree.growthStage.Value < 4) continue;

							var fruitCount = fruitTree.fruit.Count;
							if (fruitCount <= 0 || fruitCount < _config.FruitsReadyToShake) continue;

							var fruitItemIds = fruitTree.GetFruitItemIds();
							if (_forageableTracker.FruitTreeForageables.Any(i => fruitItemIds.Contains(i.QualifiedItemId) && i.IsEnabled))
							{
								fruitTree.performUseAction(fruitTree.Tile);
								Monitor.Log($"Fruit Tree shaken: {string.Join(", ", fruitItemIds)}", LogLevel.Debug);

								foreach (var id in fruitItemIds)
								{
									var name = id;

									if (_forageableTracker.FruitTreeForageables.TryGetItem(id, out var ftItem))
									{
										name = ftItem?.DisplayName ?? id;
									}

									_trackingCounts[Constants.FruitTreeKey].AddOrIncrement(name);
								}
							}
							else
							{
								Monitor.Log($"Fruit Tree not shaken: {string.Join(", ", fruitItemIds)}", LogLevel.Debug);
							}

							break;

						case Bush bush:
							if (!CheckBush(bush)) continue;

							bush.performUseAction(bush.Tile);

							break;

						case HoeDirt hoeDirt:
							if (!(hoeDirt.crop?.forageCrop.Value ?? false) || (hoeDirt.crop?.whichForageCrop.Value.IsNullOrEmpty() ?? true)) continue;

							Vector2 tile;
							var whichCrop = hoeDirt.crop.whichForageCrop.Value;

							switch (whichCrop)
							{
								case Crop.forageCrop_springOnionID:
									var springOnion = _forageableTracker.ObjectForageables.FirstOrDefault(i => i.QualifiedItemId.Equals("(O)399"));

									if (springOnion != default(ForageableItem) && springOnion.IsEnabled)
									{
										tile = hoeDirt.Tile;
										var x = (int)tile.X;
										var y = (int)tile.Y;

										ForageItem(ItemRegistry.Create<SObject>("(O)399"), tile, Utility.CreateDaySaveRandom(x * 1000, y * 2000), 3);
										hoeDirt.destroyCrop(false);
										Game1.playSound("harvest");

										_trackingCounts[Constants.ForageableKey].AddOrIncrement(springOnion.DisplayName);
									}

									break;

								case Crop.forageCrop_gingerID:
									var ginger = _forageableTracker.ObjectForageables.FirstOrDefault(i => i.QualifiedItemId == "(O)829");

									if (ginger != default(ForageableItem) && ginger.IsEnabled)
									{
										if (_config.RequireHoe && !Game1.player.Items.Any(i => i is Hoe))
										{
											if (_nextErrorMessage < DateTime.UtcNow)
											{
												Game1.addHUDMessage(new HUDMessage(I18n.Message_MissingHoe(I18n.Subject_GingerRoots()), HUDMessage.error_type));
												_nextErrorMessage = DateTime.UtcNow.AddSeconds(10);
											}

											Monitor.LogOnce(I18n.Log_MissingHoe(I18n.Subject_GingerRoots(), I18n.Option_RequireHoe_Name(" ")), LogLevel.Info);
											continue;
										}

										tile = hoeDirt.Tile;
										hoeDirt.crop?.hitWithHoe((int)tile.X, (int)tile.Y, hoeDirt.Location, hoeDirt);
										hoeDirt.destroyCrop(false);

										_trackingCounts[Constants.ForageableKey].AddOrIncrement(ginger.DisplayName);
									}

									break;

								default:
									// $TODO - Improve error message
									Monitor.LogOnce($"No good case: {whichCrop}", LogLevel.Debug);
									break;
							}

							break;
					}
				}

				if (Game1.currentLocation.Objects.TryGetValue(vec, out var obj))
				{
					// Forageable Item
					if (_forageableTracker.ObjectForageables.TryGetItem(obj.QualifiedItemId, out var objItem) && (objItem?.IsEnabled ?? false))
					{
						ForageItem(obj, vec, Utility.CreateDaySaveRandom(vec.X, vec.Y * 777f), 7, true);

						Game1.player.currentLocation.removeObject(vec, false);
						Game1.playSound("harvest");

						_trackingCounts[Constants.ForageableKey].AddOrIncrement(objItem.DisplayName);
					}
					else if ((obj.QualifiedItemId.IEquals("(O)590") && _config.ForageArtifactSpots)
						|| (obj.QualifiedItemId.IEquals("(O)SeedSpot") && _config.ForageSeedSpots))
					{
						if (_config.RequireHoe && !Game1.player.Items.Any(i => i is Hoe))
						{
							if (_nextErrorMessage < DateTime.UtcNow)
							{
								Game1.addHUDMessage(new HUDMessage(I18n.Message_MissingHoe(obj.Name), HUDMessage.error_type));
								_nextErrorMessage = DateTime.UtcNow.AddSeconds(10);
							}

							Monitor.LogOnce(I18n.Log_MissingHoe(obj.Name, I18n.Option_RequireHoe_Name(" ")), LogLevel.Info);
							continue;
						}

						var tool = Game1.player.Items.FirstOrDefault(i => i is Hoe, null) as Tool;

						if (!_config.RequireHoe && tool is null)
						{
							tool = new Hoe();
							Monitor.Log($"Failed to get instance of existing Hoe Tool - {_config.RequireHoe}", LogLevel.Debug);
						}

						// TODO - Improve logging
						if (tool is null)
						{
							Monitor.Log($"Failed to get instance of Hoe Tool - {_config.RequireHoe}", LogLevel.Debug);
						}
						else
						{
							tool.lastUser = Game1.player;
							obj.performToolAction(tool);
							_trackingCounts[Constants.ForageableKey].AddOrIncrement(obj.DisplayName);
						}
					}
				}

				var largeTerrainFeature = Game1.currentLocation.getLargeTerrainFeatureAt((int)vec.X, (int)vec.Y);
				if (largeTerrainFeature is not null && largeTerrainFeature is Bush largeBush)
				{
					if (CheckBush(largeBush))
					{
						largeBush.performUseAction(vec);
					}
				}
			}
		}

		private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
		{
			if (Game1.activeClickableMenu is not null) return;
			if (!_config.ToggleForagerKeybind.JustPressed()) return;

			_isForagerActive = !_isForagerActive;
			Task.Run(() => Helper.WriteConfig(_config)).ContinueWith(t =>
				Monitor.Log(t.Status == TaskStatus.RanToCompletion ? "Config saved successfully!" : $"Saving config unsuccessful {t.Status}", LogLevel.Debug));

			var state = _isForagerActive ? I18n.State_Activated() : I18n.State_Deactivated();
			var message = I18n.Message_AutoForagerToggled(state);

			Monitor.Log(message, LogLevel.Info);
			Game1.addHUDMessage(new HUDMessage(message) { noIcon = true });

			if (_isForagerActive)
			{
				Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
			}
			else
			{
				Helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
			}
		}

		private bool CheckBush(Bush bush)
		{
			if (!_config.AnyBushEnabled()) return false;
			if (bush.townBush.Value) return false;
			if (!bush.inBloom()) return false;
			if (bush.tileSheetOffset.Value != 1) return false;

			if (!bush.isActionable())
			{
				Monitor.Log($"A bush feature of size [{bush.size.Value}] was marked as not actionable. This shouldn't be possible.", LogLevel.Warn);
				Monitor.Log($"Size: [{bush.size.Value}]; Location: [{bush.Location.NameOrUniqueName}]; Tile Location [{bush.Tile}]; Town Bush: [{bush.townBush.Value}]", LogLevel.Debug);
			}

			switch (bush.size.Value)
			{
				// Blooming Bushes
				case 0:
				case 1:
				case 2:
					var bloomItem = bush.GetShakeOffItem();

					if (bloomItem is null) return false;

					if (!_forageableTracker.BushForageables.TryGetItem(bloomItem, out var item))
					{
						_forageableTracker.BushForageables.TryGetItem("(O)" + bloomItem, out item);
					}

					if (item is null || !item.IsEnabled) return false;

					_trackingCounts[Constants.BushKey].AddOrIncrement(item.DisplayName);

					break;

				// Tea Bushes
				case 3:
					if (_cbw.IsCustomBush(bush))
					{
						var shakeOffItem = bush.modData[CustomBushWrapper.ShakeOffItemKey];

						if (!_forageableTracker.BushForageables.TryGetItem(shakeOffItem, out var bItem) || !(bItem?.IsEnabled ?? false))
						{
							Monitor.LogOnce($"{shakeOffItem} was not shaken from custom bush as it does not exist or is disabled.", LogLevel.Debug);
							return false;
						}
						else
						{
							_trackingCounts[Constants.BushKey].AddOrIncrement(bItem.DisplayName);
						}
					}
					else
					{
						if (!_config.GetTeaBushesEnabled())
						{
							Monitor.LogOnce(I18n.Log_DisabledConfig(I18n.Subject_TeaBushes(), I18n.Option_ToggleAction_Name(I18n.Subject_TeaBushes())), LogLevel.Info);
							return false;
						}
						else
						{
							_trackingCounts[Constants.BushKey].AddOrIncrement(I18n.Subject_TeaBushes());
						}
					}


					break;

				// Walnut Bushes
				case 4:
					if (!_config.GetWalnutBushesEnabled())
					{
						Monitor.LogOnce(I18n.Log_DisabledConfig(I18n.Subject_WalnutBushes(), I18n.Option_ToggleAction_Name(I18n.Subject_WalnutBushes())), LogLevel.Info);
						return false;
					}

					_trackingCounts[Constants.BushKey].AddOrIncrement(I18n.Subject_WalnutBushes());

					break;

				default:
					Monitor.Log($"Unknown Bush size: [{bush.size.Value}]", LogLevel.Warn);
					return false;
			}

			return true;
		}

		private void EditFruitTrees(IAssetData asset)
		{
			var fruitTreeData = asset.AsDictionary<string, FruitTreeData>();

			foreach (var fruitTree in fruitTreeData.Data)
			{
				fruitTree.Value.CustomFields ??= new();
				fruitTree.Value.CustomFields.AddOrUpdate(Constants.CustomFieldForageableKey, "true");

				if (Constants.VanillaFruitTrees.Contains(fruitTree.Key))
				{
					fruitTree.Value.CustomFields.AddOrUpdate(Constants.CustomFieldCategoryKey, "Category.Vanilla");
				}
				else if (_cpFruitTrees.TryGetValue(fruitTree.Key, out var category))
				{
					fruitTree.Value.CustomFields.AddOrUpdate(Constants.CustomFieldCategoryKey, category);
				}
			}
		}

		private void EditObjects(IAssetData asset)
		{
			var objectData = asset.AsDictionary<string, ObjectData>();

			foreach (var obj in objectData.Data)
			{
				if (_ignoreItemIds.Any(i => obj.Key.IEquals(i.Substring(3)))) continue;

				string? category = null;

				if (_cpForageables.TryGetValue(obj.Key, out var cpCategory))
				{
					category = cpCategory;
				}
				else if ((obj.Value.ContextTags?.Contains("forage_item") ?? false)
					|| _overrideItemIds.Any(i => obj.Key.IEquals(i.Substring(3))))
				{
					if (Constants.KnownCategoryLookup.TryGetValue(obj.Key, out var knownCategory))
					{
						category = knownCategory;
					}
					else
					{
						category = string.Empty;
					}
				}

				if (category is not null)
				{
					obj.Value.CustomFields ??= new();
					obj.Value.CustomFields.AddOrUpdate(Constants.CustomFieldForageableKey, "true");

					if (category != string.Empty)
					{
						obj.Value.CustomFields.AddOrUpdate(Constants.CustomFieldCategoryKey, category);
					}
				}

				if (_bushBloomItems.TryGetValue(obj.Key, out var bushCategory))
				{
					obj.Value.CustomFields ??= new();
					obj.Value.CustomFields.AddOrUpdate(Constants.CustomFieldBushKey, "true");
					obj.Value.CustomFields.AddOrUpdate(Constants.CustomFieldBushBloomCategory, bushCategory);
				}

				if (_customTeaBushItems.TryGetValue(obj.Key, out var customBushCategory))
				{
					obj.Value.CustomFields ??= new();
					obj.Value.CustomFields.AddOrUpdate(Constants.CustomFieldBushKey, "true");
					obj.Value.CustomFields.AddOrUpdate(Constants.CustomFieldCustomBushCategory, customBushCategory);
				}
			}
		}

		private void EditWildTrees(IAssetData asset)
		{
			var wildTreeData = asset.AsDictionary<string, WildTreeData>();

			foreach (var wildTree in wildTreeData.Data)
			{
				// Just say no to mushroom trees
				if (wildTree.Key.Equals("7")) continue;

				wildTree.Value.CustomFields ??= new();
				wildTree.Value.CustomFields.AddOrUpdate(Constants.CustomFieldForageableKey, "true");

				if (Constants.VanillaWildTrees.Contains(wildTree.Key))
				{
					wildTree.Value.CustomFields.AddOrUpdate(Constants.CustomFieldCategoryKey, "Category.Vanilla");
				}
				else if (_cpWildTrees.TryGetValue(wildTree.Key, out var category))
				{
					wildTree.Value.CustomFields.AddOrUpdate(Constants.CustomFieldCategoryKey, category);
				}
			}
		}

		// Taken from Spacechase0's Generic Mod Config Menu
		// https://github.com/spacechase0/StardewValleyMods/blob/develop/GenericModConfigMenu/Mod.cs#L168
		private bool IsTitleMenuInteractable()
		{
			if (Game1.activeClickableMenu is not TitleMenu titleMenu || TitleMenu.subMenu is not null)
			{
				return false;
			}

			var method = Helper.Reflection.GetMethod(titleMenu, "ShouldAllowInteraction", false);

			if (method is not null)
			{
				return method.Invoke<bool>();
			}
			else
			{
				return Helper.Reflection.GetField<bool>(titleMenu, "titleInPosition").GetValue();
			}
		}

		private void ParseContentPacks(IEnumerable<IContentPack> packs)
		{
			foreach (var pack in packs)
			{
				if (pack is not null)
				{
					try
					{
						var content = pack.ReadJsonFile<ContentEntry>("content.json");
						Monitor.LogOnce($"Found content pack: {pack.DirectoryPath}", LogLevel.Debug);

						if (content?.Forageables is not null)
						{
							foreach (var itemId in content.Forageables)
							{
								if (_cpForageables.ContainsKey(itemId))
								{
									Monitor.LogOnce($"Already found an item with ItemId [{itemId}] with category [{_cpForageables[itemId]}] when trying to add category [{content.Category}]. Please verify you don't have duplicate or conflicting content packs.", LogLevel.Warn);
								}
								else
								{
									Monitor.LogOnce($"Found content pack forageable: {itemId} - {content.Category}", LogLevel.Debug);
									_cpForageables.Add(itemId, content.Category);
								}
							}
						}

						if (content?.FruitTrees is not null)
						{
							foreach (var treeId in content.FruitTrees)
							{
								if (_cpFruitTrees.ContainsKey(treeId))
								{
									Monitor.LogOnce($"Already found a Fruit Tree with Id [{treeId}] with category [{_cpFruitTrees[treeId]}] when trying to add category [{content.Category}]. Please verify you don't have duplicate or conflicting content packs.", LogLevel.Warn);
								}
								else
								{
									_cpFruitTrees.Add(treeId, content.Category);
								}
							}
						}

						if (content?.WildTrees is not null)
						{
							foreach (var treeId in content.WildTrees)
							{
								if (_cpWildTrees.ContainsKey(treeId))
								{
									Monitor.LogOnce($"Already found a Wild Tree with Id [{treeId}] with category [{_cpWildTrees[treeId]}] when trying to add category [{content.Category}]. Please verify you don't have duplicate or conflicting content packs.", LogLevel.Warn);
								}
								else
								{
									_cpWildTrees.Add(treeId, content.Category);
								}
							}
						}

						if (content?.IgnoredItems is not null)
						{
							foreach (var itemId in content.IgnoredItems)
							{
								var qualifiedItemId = itemId;
								if (!qualifiedItemId.StartsWith("(O)"))
								{
									qualifiedItemId = $"(O){qualifiedItemId}";
								}

								_ignoreItemIds.AddDistinct(qualifiedItemId);
							}
						}
					}
					catch
					{
						Monitor.Log(I18n.Log_ContentPack_LoadError(Path.Combine(pack.DirectoryPath, "content.json")), LogLevel.Error);
					}
				}
			}
		}

		private static void ForageItem(SObject obj, Vector2 vec, Random random, int xpGained = 0, bool checkGatherer = false)
		{
			var foragingLevel = Game1.player.ForagingLevel;
			var professions = Game1.player.professions;

			if (professions.Contains(16))
			{
				obj.Quality = 4;
			}
			else if (random.NextDouble() < (double)(foragingLevel / 30f))
			{
				obj.Quality = 2;
			}
			else if (random.NextDouble() < (double)(foragingLevel / 15f))
			{
				obj.Quality = 1;
			}

			vec *= 64.0f;

			Game1.player.gainExperience(2, xpGained);
			Game1.createItemDebris(obj.getOne(), vec, -1, null, -1);

			if (checkGatherer && professions.Contains(13) && random.NextDouble() < 0.2)
			{
				Game1.player.gainExperience(2, xpGained);
				Game1.createItemDebris(obj.getOne(), vec, -1, null, -1);
			}
		}

		private static IEnumerable<Vector2> GetTilesToCheck(Point playerLocation, int radius)
		{
			for (int x = Math.Max(playerLocation.X - radius, 0); x <= playerLocation.X + radius; x++)
				for (int y = Math.Max(playerLocation.Y - radius, 0); y <= playerLocation.Y + radius; y++)
					yield return new Vector2(x, y);

			yield break;
		}
	}
}
