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
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.FruitTrees;
using StardewValley.GameData.Locations;
using StardewValley.GameData.Objects;
using StardewValley.GameData.WildTrees;
using StardewValley.ItemTypeDefinitions;
using StardewValley.TokenizableStrings;
using AutoForager.Extensions;

using Constants = AutoForager.Helpers.Constants;

namespace AutoForager.Classes
{
	public class ForageableItem : IComparable
	{
		private readonly string _itemId;
		public string ItemId => _itemId;

		private readonly string _qualifiedItemId;
		public string QualifiedItemId => _qualifiedItemId;

		private readonly string _internalName;
		public string InternalName => _internalName;

		private readonly string _displayName;
		public string DisplayName => TokenParser.ParseText(_displayName);

		private readonly Dictionary<string, string> _customFields;
		public Dictionary<string, string> CustomFields => _customFields;

		private readonly bool _defaultIsEnabled;
		private bool _isEnabled;
		public bool IsEnabled
		{
			get => _isEnabled;
			set => _isEnabled = value;
		}

		public void ResetToDefaultEnabled()
		{
			_isEnabled = _defaultIsEnabled;
		}

		public ForageableItem(string itemId, string qualifiedItemId, string internalName, string displayName, Dictionary<string, string> customFields, bool enabled = false)
		{
			_itemId = itemId;
			_qualifiedItemId = qualifiedItemId;
			_internalName = internalName;
			_displayName = displayName;
			_customFields = customFields;
			_isEnabled = enabled;
			_defaultIsEnabled = enabled;
		}

		public ForageableItem(ParsedItemData data, Dictionary<string, string> customFields, bool enabled = false)
			: this(data.ItemId, data.QualifiedItemId, data.InternalName, data.DisplayName, customFields, enabled)
		{ }

		public static IEnumerable<ForageableItem> ParseFruitTreeData(IDictionary<string, FruitTreeData> data, IDictionary<string, bool>? configValues = null, IMonitor? monitor = null)
		{
			var forageItems = new List<ForageableItem>();

			foreach (var kvp in data)
			{
				foreach (var fruit in kvp.Value.Fruit)
				{
					try
					{
						var customFields = kvp.Value.CustomFields;
						if (customFields is null || !customFields.ContainsKey(Constants.CustomFieldForageableKey)) continue;

						var enabled = true;
						var fruitData = ItemRegistry.GetData(fruit.ItemId);
						fruitData ??= ItemRegistry.GetData("(O)" + fruit.ItemId);

						if (fruitData is not null)
						{
							if (configValues is not null && configValues.TryGetValue(fruitData.InternalName, out var configEnabled))
							{
								enabled = configEnabled;
							}

							forageItems.AddDistinct(new ForageableItem(fruitData, customFields, enabled));
						}
					}
					catch (Exception ex)
					{
						monitor?.Log($"{kvp.Key} - {fruit?.ItemId} - {fruit?.ObjectInternalName}{Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.StackTrace}", LogLevel.Error);
					}

				}
			}

			return forageItems;
		}

		public static IEnumerable<ForageableItem> ParseWildTreeData(IDictionary<string, WildTreeData> data, IDictionary<string, bool>? configValues = null, IMonitor? monitor = null)
		{
			var forageItems = new List<ForageableItem>();

			foreach (var kvp in data)
			{
				var customFields = kvp.Value.CustomFields;
				if (customFields is null || !customFields.ContainsKey(Constants.CustomFieldForageableKey)) continue;

				var seedAndSeedItemIds = new List<string> { kvp.Value.SeedItemId };

				if (kvp.Value.SeedDropItems is not null)
				{
					seedAndSeedItemIds.AddRange(kvp.Value.SeedDropItems.Select(i => i.ItemId));
				}

				if (kvp.Value.ShakeItems is not null)
				{
					seedAndSeedItemIds.AddRange(kvp.Value.ShakeItems.Select(i => i.ItemId));
				}

				foreach (var seedItem in seedAndSeedItemIds)
				{
					try
					{
						if (seedItem is null) continue;

						var enabled = true;
						var seedData = ItemRegistry.GetData(seedItem);

						if (configValues is not null && configValues.TryGetValue(seedData.InternalName, out var configEnabled))
						{
							enabled = configEnabled;
						}

						forageItems.AddDistinct(new ForageableItem(seedData, customFields, enabled));
					}
					catch (Exception ex)
					{
						monitor?.Log($"{kvp.Key} - {seedItem}{Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.StackTrace}", LogLevel.Error);
					}
				}
			}

			return forageItems;
		}

		public static (IEnumerable<ForageableItem>, IEnumerable<ForageableItem>) ParseObjectData(IDictionary<string, ObjectData> data, ModConfig? config = null, IMonitor? monitor = null)
		{
			var forageItems = new List<ForageableItem>();
			var bushItems = new List<ForageableItem>();
			var forageableConfig = config?.ForageToggles[Constants.ForagingToggleKey];
			var bushConfig = config?.ForageToggles[Constants.BushToggleKey];

			foreach (var kvp in data)
			{
				try
				{
					var customFields = kvp.Value.CustomFields;

					if (customFields is not null)
					{
						var qualifiedItemId = "(O)" + kvp.Key;
						var itemData = ItemRegistry.GetData(qualifiedItemId) ?? ItemRegistry.GetData(kvp.Key);
						var internalName = itemData?.InternalName ?? kvp.Value.Name;

						if (customFields.TryGetValue(Constants.CustomFieldForageableKey, out var isForageable) && isForageable.IEquals("true"))
						{
							var enabled = true;

							if (forageableConfig is not null && forageableConfig.TryGetValue(internalName, out var configEnabled))
							{
								enabled = configEnabled;
							}

							if (itemData is not null)
							{
								forageItems.AddDistinct(new ForageableItem(itemData, customFields, enabled));
							}
							else
							{
								var kvpValue = kvp.Value;
								forageItems.AddDistinct(new ForageableItem(kvp.Key, qualifiedItemId, kvpValue.Name, kvpValue.DisplayName, customFields, enabled));
							}
						}

						if (customFields.TryGetValue(Constants.CustomFieldBushKey, out var isBush) && isBush.IEquals("true"))
						{
							var enabled = true;

							if (bushConfig is not null && bushConfig.TryGetValue(internalName, out var configEnabled))
							{
								enabled = configEnabled;
							}

							if (itemData is not null)
							{
								bushItems.AddDistinct(new ForageableItem(itemData, customFields, enabled));
							}
							else
							{
								var kvpValue = kvp.Value;
								bushItems.AddDistinct(new ForageableItem(kvp.Key, qualifiedItemId, kvpValue.Name, kvpValue.DisplayName, customFields, enabled));
							}
						}
					}
				}
				catch (Exception ex)
				{
					monitor?.Log($"{kvp.Key} - {kvp.Value.Name}{Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.StackTrace}", LogLevel.Error);
				}

			}

			return (forageItems, bushItems);
		}

		public static IEnumerable<ForageableItem> ParseLocationData(IDictionary<string, ObjectData> oData, IDictionary<string, LocationData> lData, IDictionary<string, bool>? configValues = null)
		{
			var forageItems = new List<ForageableItem>();

			foreach (var kvp in lData)
			{
				var artifactSpots = kvp.Value.ArtifactSpots;
				if (artifactSpots is null || artifactSpots.Count <= 0) continue;

				foreach (var artifact in artifactSpots)
				{
					List<string> itemIds;

					if (artifact.RandomItemId is not null)
					{
						itemIds = artifact.RandomItemId;
					}
					else if (artifact.ItemId is not null)
					{
						itemIds = new() { artifact.ItemId };
					}
					else
					{
						continue;
					}

					foreach (var itemId in itemIds)
					{
						var artifactId = itemId.Substring(itemId.IndexOf(')') + 1);
						if (!oData.ContainsKey(artifactId)) continue;

						var objData = oData[artifactId];
						if (objData is null || objData.CustomFields is null || !objData.CustomFields.ContainsKey(Constants.CustomFieldForageableKey)) continue;

						var enabled = true;
						var itemData = ItemRegistry.GetData(itemId);

						if (configValues is not null && configValues.TryGetValue(itemData.InternalName, out var configEnabled))
						{
							enabled = configEnabled;
						}

						forageItems.AddDistinct(new ForageableItem(itemData, objData.CustomFields, enabled));
					}
				}
			}

			return forageItems;
		}

		public int CompareTo(object? obj)
		{
			if (obj is null) return 1;

			if (obj is ForageableItem otherForageable)
			{
				return QualifiedItemId.CompareTo(otherForageable.QualifiedItemId);
			}
			else
			{
				throw new ArgumentException($"Object is not a ${nameof(ForageableItem)}");
			}
		}
	}
}
