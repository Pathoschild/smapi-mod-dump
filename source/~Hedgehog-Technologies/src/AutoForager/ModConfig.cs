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
using System.Linq;
using HedgeTech.Common.Extensions;
using HedgeTech.Common.Helpers;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using AutoForager.Classes;
using AutoForager.Extensions;
using HedgeTech.Common.Interfaces;

using Constants = AutoForager.Helpers.Constants;

namespace AutoForager
{
	public class ModConfig
	{
		private const string _gmcmUniqueId = "spacechase0.GenericModConfigMenu";

		private readonly ForageableItemTracker _forageableTracker;
		private CategoryComparer _comparer = new();
		private IMonitor? _monitor;
		private JsonHelper _jsonHelper;

		#region General Properties

		public KeybindList ToggleForagerKeybind { get; set; } = new();
		public bool UsePlayerMagnetism { get; set; }
		public int ShakeDistance { get; set; }
		public bool RequireHoe { get; set; }
		public bool RequireToolMoss { get; set; }
		public bool IgnoreMushroomLogTrees { get; set; }

		private int _fruitsReadyToShake;
		public int FruitsReadyToShake
		{
			get => _fruitsReadyToShake;
			set => _fruitsReadyToShake = Math.Clamp(value, Constants.MinFruitsReady, Constants.MaxFruitsReady);
		}

		public bool ForageArtifactSpots { get; set; }

		public bool ForageSeedSpots { get; set; }

		public bool ForageMushroomBoxes { get; set; }

		public bool ForageMushroomLogs { get; set; }

		public bool ForageTappers { get; set; }

		public Dictionary<string, Dictionary<string, bool>> ForageToggles { get; set; }

		private bool _anyBushesEnabled;
		public bool AnyBushEnabled() => _anyBushesEnabled;

		public bool GetTeaBushesEnabled() => ForageToggles[Constants.BushToggleKey][Constants.TeaBushKey];
		private void SetTeaBushesEnabled(bool value) => ForageToggles[Constants.BushToggleKey][Constants.TeaBushKey] = value;

		public bool GetWalnutBushesEnabled() => ForageToggles[Constants.BushToggleKey][Constants.WalnutBushKey];
		private bool SetWalnutBushesEnabled(bool value) => ForageToggles[Constants.BushToggleKey][Constants.WalnutBushKey] = value;

		#endregion

		public ModConfig()
		{
			_forageableTracker = ForageableItemTracker.Instance;

			ForageToggles = new()
			{
				{ Constants.BushToggleKey, new() },
				{ Constants.ForagingToggleKey, new() },
				{ Constants.FruitTreeToggleKey, new() },
				{ Constants.WildTreeToggleKey, new() }
			};

			ResetToDefault();
		}

		public void UpdateUtilities(IMonitor monitor, IEnumerable<IContentPack> packs, JsonHelper jsonHelper)
		{
			_monitor = monitor;
			_comparer = new CategoryComparer(packs);
			_jsonHelper = jsonHelper;
		}

		public void AddFtmCategories(Dictionary<string, string> ftmCategories)
		{
			_comparer.AddFtmCategories(ftmCategories);
		}

		public void ResetToDefault()
		{
			ToggleForagerKeybind = new KeybindList(
				new Keybind(SButton.LeftAlt, SButton.H),
				new Keybind(SButton.RightAlt, SButton.H));

			UsePlayerMagnetism = false;
			ShakeDistance = 2;
			RequireHoe = true;
			RequireToolMoss = true;
			IgnoreMushroomLogTrees = true;
			FruitsReadyToShake = Constants.MinFruitsReady;

			ForageArtifactSpots = true;
			ForageSeedSpots = true;
			ForageMushroomBoxes = true;
			ForageMushroomLogs = true;
			ForageTappers = true;

			foreach (var toggleDict in ForageToggles)
			{
				if (_forageableTracker is not null || toggleDict.Key == Constants.BushToggleKey)
				{
					if (toggleDict.Key.Equals(Constants.BushToggleKey))
					{
						toggleDict.Value[Constants.TeaBushKey] = true;
						toggleDict.Value[Constants.WalnutBushKey] = false;
					}
					else if (toggleDict.Key.Equals(Constants.ForagingToggleKey))
					{
						ResetTracker(_forageableTracker?.ObjectForageables, toggleDict.Value);
					}
					else if (toggleDict.Key.Equals(Constants.FruitTreeToggleKey))
					{
						ResetTracker(_forageableTracker?.FruitTreeForageables, toggleDict.Value);
					}
					else if (toggleDict.Key.Equals(Constants.WildTreeToggleKey))
					{
						ResetTracker(_forageableTracker?.WildTreeForageables, toggleDict.Value);
					}
				}
			}
		}

		public void RegisterModConfigMenu(IModHelper helper, IManifest manifest)
		{
			if (!helper.ModRegistry.IsLoaded(_gmcmUniqueId)) return;

			var gmcmApi = helper.ModRegistry.GetApi<IGenericModConfigMenu>(_gmcmUniqueId);
			if (gmcmApi is null) return;

			try
			{
				gmcmApi.Unregister(manifest);
			}
			catch { }

			gmcmApi.Register(
				mod: manifest,
				reset: ResetToDefault,
				save: () =>
				{
					helper.WriteConfig(this);
					_monitor?.Log(_jsonHelper.Serialize(this), LogLevel.Trace);
				});

			/* General */

			gmcmApi.AddSectionTitle(
				mod: manifest,
				text: I18n.Section_General_Text);

			// ToggleForager
			gmcmApi.AddKeybindList(
				mod: manifest,
				fieldId: Constants.ToggleForagerId,
				name: I18n.Option_ToggleForager_Name,
				tooltip: I18n.Option_ToggleForager_Tooltip,
				getValue: () => ToggleForagerKeybind,
				setValue: val => ToggleForagerKeybind = val);

			// UsePlayerMagnetism
			gmcmApi.AddBoolOption(
				mod: manifest,
				fieldId: Constants.UsePlayerMagnetismId,
				name: I18n.Option_UsePlayerMagnetism_Name,
				tooltip: () => I18n.Option_UsePlayerMagnetism_Tooltip(I18n.Option_ShakeDistance_Name()),
				getValue: () => UsePlayerMagnetism,
				setValue: val => UsePlayerMagnetism = val);

			// ShakeDistance
			gmcmApi.AddNumberOption(
				mod: manifest,
				fieldId: Constants.ShakeDistanceId,
				name: I18n.Option_ShakeDistance_Name,
				tooltip: () => I18n.Option_ShakeDistance_Tooltip(I18n.Option_UsePlayerMagnetism_Name()),
				getValue: () => ShakeDistance,
				setValue: val => ShakeDistance = val);

			// RequireHoe
			gmcmApi.AddBoolOption(
				mod: manifest,
				fieldId: Constants.RequireHoeId,
				name: () => I18n.Option_RequireHoe_Name(Environment.NewLine),
				tooltip: I18n.Option_RequireHoe_Tooltip,
				getValue: () => RequireHoe,
				setValue: val => RequireHoe = val);

			// RequireToolMoss
			gmcmApi.AddBoolOption(
				mod: manifest,
				fieldId: Constants.RequireToolMossId,
				name: () => I18n.Option_RequireToolMoss_Name(Environment.NewLine),
				tooltip: I18n.Option_RequireToolMoss_Tooltip,
				getValue: () => RequireToolMoss,
				setValue: val => RequireToolMoss = val);

			// IgnoreMushroomLogTrees
			gmcmApi.AddBoolOption(
				mod: manifest,
				fieldId: Constants.IgnoreMushroomLogTreesId,
				name: () => I18n.Option_IgnoreMushroomLogTrees_Name(Environment.NewLine),
				tooltip: I18n.Option_IgnoreMushroomLogTrees_Tooltip,
				getValue: () => IgnoreMushroomLogTrees,
				setValue: (val) => IgnoreMushroomLogTrees = val);

			/* Page Links Section */

			gmcmApi.AddPageLink(
				mod: manifest,
				pageId: Constants.BushesPageId,
				text: I18n.Link_Bushes_Text);

			gmcmApi.AddPageLink(
				mod: manifest,
				pageId: Constants.ForageablesPageId,
				text: I18n.Link_Forageables_Text);

			gmcmApi.AddPageLink(
				mod: manifest,
				pageId: Constants.FruitTreesPageId,
				text: I18n.Link_FruitTrees_Text);

			gmcmApi.AddPageLink(
				mod: manifest,
				pageId: Constants.WildTreesPageId,
				text: I18n.Link_WildTrees_Text);

			/* Wild Trees */

			gmcmApi.AddPage(
				mod: manifest,
				pageId: Constants.WildTreesPageId,
				pageTitle: I18n.Page_WildTrees_Title);

			gmcmApi.AddSectionTitle(
				mod: manifest,
				text: I18n.Section_WildTree_Text);

			gmcmApi.AddParagraph(
				mod: manifest,
				text: I18n.Page_WildTrees_Description);

			foreach (var currentGroup in _forageableTracker.WildTreeForageables.GroupByCategory(helper, comparer: _comparer))
			{
				gmcmApi.AddSectionTitle(
					mod: manifest,
					text: () => currentGroup.Key);

				foreach (var item in currentGroup)
				{
					gmcmApi.AddBoolOption(
						mod: manifest,
						name: () => I18n.Option_ToggleAction_Name(item.DisplayName),
						tooltip: () => $"{item.ItemId} - {item.InternalName}",
						getValue: () => item.IsEnabled,
						setValue: val =>
						{
							item.IsEnabled = val;
							ForageToggles[Constants.WildTreeToggleKey].AddOrUpdate(item.InternalName, val);
							UpdateEnabled();
						});
				}
			}

			/* Fruit Trees */

			gmcmApi.AddPage(
				mod: manifest,
				pageId: Constants.FruitTreesPageId,
				pageTitle: I18n.Page_FruitTrees_Title);

			// FruitsReadyToShake
			gmcmApi.AddNumberOption(
				mod: manifest,
				fieldId: Constants.FruitsReadyToShakeId,
				name: I18n.Option_FruitsReadyToShake_Name,
				tooltip: I18n.Option_FruitsReadyToShake_Tooltip,
				getValue: () => FruitsReadyToShake,
				setValue: val => FruitsReadyToShake = val,
				min: Constants.MinFruitsReady,
				max: Constants.MaxFruitsReady);

			gmcmApi.AddSectionTitle(
				mod: manifest,
				text: I18n.Section_FruitTrees_Text);

			gmcmApi.AddParagraph(
				mod: manifest,
				text: I18n.Page_FruitTrees_Description);

			foreach (var currentGroup in _forageableTracker.FruitTreeForageables.GroupByCategory(helper, comparer: _comparer))
			{
				gmcmApi.AddSectionTitle(
					mod: manifest,
					text: () => currentGroup.Key);

				foreach (var item in currentGroup)
				{
					gmcmApi.AddBoolOption(
						mod: manifest,
						name: () => I18n.Option_ToggleAction_Name(item.DisplayName),
						tooltip: () => $"{item.ItemId} - {item.InternalName}",
						getValue: () => item.IsEnabled,
						setValue: val =>
						{
							item.IsEnabled = val;
							ForageToggles[Constants.FruitTreeToggleKey].AddOrUpdate(item.InternalName, val);
							UpdateEnabled();
						});
				}
			}

			/* Bushes */

			gmcmApi.AddPage(
				mod: manifest,
				pageId: Constants.BushesPageId,
				pageTitle: I18n.Page_Bushes_Title);

			gmcmApi.AddSectionTitle(
				mod: manifest,
				text: I18n.Section_Bushes_Text);

			gmcmApi.AddParagraph(
				mod: manifest,
				text: I18n.Page_Bushes_Description);

			// Bush Blooms
			foreach (var currentGroup in _forageableTracker.BushForageables
				.Where(b => b.CustomFields.ContainsKey(Constants.CustomFieldBushBloomCategory)).ToList()
				.GroupByCategory(helper, Constants.CustomFieldBushBloomCategory, _comparer))
			{
				gmcmApi.AddSectionTitle(
					mod: manifest,
					text: () => currentGroup.Key);

				foreach (var item in currentGroup)
				{
					gmcmApi.AddBoolOption(
						mod: manifest,
						name: () => I18n.Option_ToggleAction_Name(item.DisplayName),
						tooltip: () => I18n.Option_ToggleAction_Description_Reward(
							I18n.Action_Shake_Future().ToLowerInvariant(),
							I18n.Subject_Bushes(),
							item.DisplayName),
						getValue: () => item.IsEnabled,
						setValue: val =>
						{
							item.IsEnabled = val;
							ForageToggles[Constants.BushToggleKey].AddOrUpdate(item.InternalName, val);
							UpdateEnabled();
						});
				}
			}

			// Custom Bushes
			foreach (var currentGroup in _forageableTracker.BushForageables
				.Where(b => b.CustomFields.ContainsKey(Constants.CustomFieldCustomBushCategory)).ToList()
				.GroupByCategory(helper, Constants.CustomFieldCustomBushCategory, _comparer))
			{
				gmcmApi.AddSectionTitle(
					mod: manifest,
					text: () => currentGroup.Key);

				foreach (var item in currentGroup)
				{
					gmcmApi.AddBoolOption(
						mod: manifest,
						name: () => I18n.Option_ToggleAction_Name(item.DisplayName),
						tooltip: () => I18n.Option_ToggleAction_Description_Reward(
							I18n.Action_Shake_Future().ToLowerInvariant(),
							I18n.Subject_Bushes(),
							item.DisplayName),
						getValue: () => item.IsEnabled,
						setValue: val =>
						{
							item.IsEnabled = val;
							ForageToggles[Constants.BushToggleKey].AddOrUpdate(item.InternalName, val);
							UpdateEnabled();
						});
				}
			}

			gmcmApi.AddSectionTitle(
				mod: manifest,
				text: I18n.Category_Vanilla);

			// ShakeTeaBushes
			gmcmApi.AddBoolOption(
				mod: manifest,
				fieldId: Constants.ShakeTeaBushesId,
				name: () => I18n.Option_ToggleAction_Name(I18n.Subject_TeaBushes()),
				tooltip: () => I18n.Option_ToggleAction_Description_Reward(
					I18n.Action_Shake_Future().ToLowerInvariant(),
					I18n.Subject_TeaBushes(),
					I18n.Reward_TeaLeaves()),
				getValue: GetTeaBushesEnabled,
				setValue: val =>
				{
					SetTeaBushesEnabled(val);
					UpdateEnabled();
				});

			// ShakeWalnutBushes
			gmcmApi.AddBoolOption(
				mod: manifest,
				fieldId: Constants.ShakeWalnutBushesId,
				name: () => I18n.Option_ToggleAction_Name(I18n.Subject_WalnutBushes()),
				tooltip: () => I18n.Option_ToggleAction_Description_Reward_Note(
					I18n.Action_Shake_Future().ToLowerInvariant(),
					I18n.Subject_WalnutBushes(),
					I18n.Reward_GoldenWalnuts(),
					I18n.Note_ShakeWalnutBushes()),
				getValue: GetWalnutBushesEnabled,
				setValue: val =>
				{
					SetWalnutBushesEnabled(val);
					UpdateEnabled();
				});

			/* Forageables */

			gmcmApi.AddPage(
				mod: manifest,
				pageId: Constants.ForageablesPageId,
				pageTitle: I18n.Page_Forageables_Title);

			// Artifact Spots
			gmcmApi.AddBoolOption(
				mod: manifest,
				name: () => I18n.Option_ToggleAction_Name(I18n.Subject_ArtifactSpot()),
				tooltip: () => I18n.Option_ToggleAction_Description_Reward(
					I18n.Action_Dig_Future().ToLowerInvariant(),
					I18n.Subject_ArtifactSpot(),
					I18n.Reward_Buried_Items()),
				getValue: () => ForageArtifactSpots,
				setValue: (val) => ForageArtifactSpots = val);

			// Seed Spots
			gmcmApi.AddBoolOption(
				mod: manifest,
				name: () => I18n.Option_ToggleAction_Name(I18n.Subject_SeedSpot()),
				tooltip: () => I18n.Option_ToggleAction_Description_Reward(
					I18n.Action_Dig_Future().ToLowerInvariant(),
					I18n.Subject_SeedSpot(),
					I18n.Reward_Buried_Seeds()),
				getValue: () => ForageSeedSpots,
				setValue: (val) => ForageSeedSpots = val);

			// Mushroom Boxes
			gmcmApi.AddBoolOption(
				mod: manifest,
				name: () => I18n.Option_ToggleAction_Name(I18n.Subject_MushroomBoxes()),
				tooltip: () => I18n.Option_ToggleAction_Description_Reward(
					I18n.Action_Forage_Future().ToLowerInvariant(),
					I18n.Subject_MushroomBoxes(),
					I18n.Reward_Mushrooms()),
				getValue: () => ForageMushroomBoxes,
				setValue: (val) => ForageMushroomBoxes = val);

			// Mushroom Logs
			gmcmApi.AddBoolOption(
				mod: manifest,
				name: () => I18n.Option_ToggleAction_Name(I18n.Subject_MushroomLogs()),
				tooltip: () => I18n.Option_ToggleAction_Description_Reward(
					I18n.Action_Forage_Future().ToLowerInvariant(),
					I18n.Subject_MushroomLogs(),
					I18n.Reward_Mushrooms()),
				getValue: () => ForageMushroomLogs,
				setValue: (val) => ForageMushroomLogs = val);

			// Tappers
			gmcmApi.AddBoolOption(
				mod: manifest,
				name: () => I18n.Option_ToggleAction_Name(I18n.Subject_Tappers()),
				tooltip: () => I18n.Option_ToggleAction_Description_Reward(
					I18n.Action_Forage_Future().ToLowerInvariant(),
					I18n.Subject_Tappers(),
					I18n.Reward_TappedTree()),
				getValue: () => ForageTappers,
				setValue: (val) => ForageTappers = val);

			gmcmApi.AddParagraph(
				mod: manifest,
				text: I18n.Page_Forageables_Description);

			foreach (var currentGroup in _forageableTracker.ObjectForageables.GroupByCategory(helper, comparer: _comparer))
			{
				gmcmApi.AddSectionTitle(
					mod: manifest,
					text: () => currentGroup.Key);

				foreach (var item in currentGroup)
				{
					gmcmApi.AddBoolOption(
						mod: manifest,
						name: () => I18n.Option_ToggleAction_Name(item.DisplayName),
						tooltip: () => $"{item.ItemId} - {item.InternalName}",
						getValue: () => item.IsEnabled,
						setValue: val =>
						{
							item.IsEnabled = val;
							ForageToggles[Constants.ForagingToggleKey].AddOrUpdate(item.InternalName, val);
							UpdateEnabled();
						});
				}
			}
		}

		public void UpdateEnabled(IModHelper? helper = null)
		{
			if (_forageableTracker is not null)
			{
				foreach (var toggleDict in ForageToggles)
				{
					if (toggleDict.Key.Equals(Constants.BushToggleKey)
						&& toggleDict.Value.Keys.Any())
					{
						_anyBushesEnabled = toggleDict.Value.Any(b => b.Value)
							|| toggleDict.Value[Constants.TeaBushKey]
							|| toggleDict.Value[Constants.WalnutBushKey];
					}
					else if (toggleDict.Key.Equals(Constants.ForagingToggleKey))
					{
						UpdateTrackerEnables(_forageableTracker.ObjectForageables, toggleDict.Value);
					}
					else if (toggleDict.Key.Equals(Constants.FruitTreeToggleKey))
					{
						UpdateTrackerEnables(_forageableTracker.FruitTreeForageables, toggleDict.Value);
					}
					else if (toggleDict.Key.Equals(Constants.WildTreeToggleKey))
					{
						UpdateTrackerEnables(_forageableTracker.WildTreeForageables, toggleDict.Value);
					}
				}
			}

			helper?.WriteConfig(this);
		}

		private static void UpdateTrackerEnables(List<ForageableItem> items, Dictionary<string, bool> dict)
		{
			if (items.Count == 0) return;

			foreach (var toggle in dict)
			{
				var item = items.FirstOrDefault(f => f?.InternalName.Equals(toggle.Key) ?? false, null);

				if (item is not null)
				{
					item.IsEnabled = toggle.Value;
				}
			}

			dict.Clear();

			foreach (var item in items)
			{
				dict.AddOrUpdate(item.InternalName, item.IsEnabled);
			}
		}

		private static void ResetTracker(List<ForageableItem>? items, Dictionary<string, bool> dict)
		{
			if (items.IsNullOrEmpty()) return;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
			foreach (var item in items)
			{
				item.ResetToDefaultEnabled();
				dict.Add(item.InternalName, item.IsEnabled);
			}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
		}
	}
}
