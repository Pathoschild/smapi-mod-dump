/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hedgehog-Technologies/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley.GameData;
using StardewValley;
using StardewValley.TerrainFeatures;
using HedgeTech.Common.Extensions;
using HedgeTech.Common.Interfaces;

namespace AutoForager.Integrations
{
	internal class CustomBushWrapper
	{
		private const string _minVersion = "1.0.4";
		private const string _cbUniqueId = "furyx639.CustomBush";
		private const string _cpUniqueId = "Pathoschild.ContentPatcher";
		private const int _readyRetries = 120;
		private const int _readyRetryWaitMs = 500;

		public static string ShakeOffItemKey => _cbUniqueId + "/ShakeOff";

		private readonly IMonitor _monitor;
		private readonly IModHelper _helper;

		private readonly ICustomBushApi? _customBushApi;
		private readonly IContentPatcherApi? _contentPatcherApi;


		public CustomBushWrapper(IMonitor monitor, IModHelper helper)
		{
			_monitor = monitor;
			_helper = helper;

			if (helper.ModRegistry.IsLoaded(_cbUniqueId) && helper.ModRegistry.IsLoaded(_cpUniqueId))
			{
				var customBush = helper.ModRegistry.Get(_cbUniqueId);

				if (customBush is not null)
				{
					var cbName = customBush.Manifest.Name;
					var cbVersion = customBush.Manifest.Version;

					if (cbVersion.IsEqualToOrNewerThan(_minVersion))
					{
						monitor.Log(I18n.Log_Wrapper_ModFound(cbName, I18n.Category_CustomBushes()), LogLevel.Info);
						_customBushApi = helper.ModRegistry.GetApi<ICustomBushApi>(_cbUniqueId);
						_contentPatcherApi = helper.ModRegistry.GetApi<IContentPatcherApi>(_cpUniqueId);
					}
					else
					{
						monitor.Log(I18n.Log_Wrapper_OldVersion(cbName, cbVersion, _minVersion), LogLevel.Warn);
					}
				}
				else
				{
					monitor.Log(I18n.Log_Wrapper_ManifestError("Custom Bush"), LogLevel.Warn);
				}
			}
		}

		public bool IsCustomBush(Bush bush) => _customBushApi?.IsCustomBush(bush) ?? false;

		public async Task<IEnumerable<string>> GetDrops()
		{
			var customDrops = new List<string>();

			if (_customBushApi is not null && _contentPatcherApi is not null)
			{
				var remainingRetries = _readyRetries;

				while (!_contentPatcherApi.IsConditionsApiReady && remainingRetries-- > 0)
				{
					await Task.Delay(_readyRetryWaitMs);
				}

				remainingRetries += 1;
				var retryTime = (_readyRetries - remainingRetries) * _readyRetryWaitMs;

				_monitor.Log($"Custom Bush status: Content Patcher Ready: {_contentPatcherApi.IsConditionsApiReady} - Remaining Retries: {remainingRetries} / {_readyRetries} - Total time: {retryTime}ms", LogLevel.Debug);

				if (_contentPatcherApi.IsConditionsApiReady)
				{
					var bushes = _customBushApi.GetData();

					foreach (var bush in bushes)
					{
						_monitor.Log(bush.Id, LogLevel.Debug);

						if (_customBushApi.TryGetDrops(bush.Id, out var drops))
						{
							customDrops.AddRange(drops?.Select(d => d.ItemId) ?? new List<string>());
						}
					}
				}
				else
				{
					_monitor.Log($"Custom Bush or Content Patcher was not ready within {retryTime}ms. Continuing without Custom Bush integration.", LogLevel.Warn);
				}
			}

			return customDrops;
		}
	}

	public interface ICustomBushDrop : ISpawnItemData
	{
		/// <summary>Gets the specific season when the item can be produced.</summary>
		public Season? Season { get; }

		/// <summary>Gets the probability that the item will be produced.</summary>
		public float Chance { get; }

		/// <summary>A game state query which indicates whether the item should be added. Defaults to always added.</summary>
		public string? Condition { get; }

		/// <summary>An ID for this entry within the current list (not the item itself, which is <see cref="P:StardewValley.GameData.GenericSpawnItemData.ItemId" />). This only needs to be unique within the current list. For a custom entry, you should use a globally unique ID which includes your mod ID like <c>ExampleMod.Id_ItemName</c>.</summary>
		public string? Id { get; }
	}

	public interface ICustomBush
	{
		/// <summary>Gets the age needed to produce.</summary>
		public int AgeToProduce { get; }

		/// <summary>Gets the day of month to begin producing.</summary>
		public int DayToBeginProducing { get; }

		/// <summary>Gets the description of the bush.</summary>
		public string Description { get; }

		/// <summary>Gets the display name of the bush.</summary>
		public string DisplayName { get; }

		/// <summary>Gets the default texture used when planted indoors.</summary>
		public string IndoorTexture { get; }

		/// <summary>Gets the season in which this bush will produce its drops.</summary>
		public List<Season> Seasons { get; }

		/// <summary>Gets the rules which override the locations that custom bushes can be planted in.</summary>
		public List<PlantableRule> PlantableLocationRules { get; }

		/// <summary>Gets the texture of the tea bush.</summary>
		public string Texture { get; }

		/// <summary>Gets the row index for the custom bush's sprites.</summary>
		public int TextureSpriteRow { get; }

		/// <summary>Retrieves the items produced by the custom bush.</summary>
		/// <returns>An enumerable collection of objects implementing the ICustomBushDrop interface. Each object represents an item produced by the custom bush.</returns>
		//public IEnumerable<ICustomBushDrop> GetItemsProduced();
	}

	public interface ICustomBushApi
	{
		/// <summary>Gets the data model for all Custom Bush.</summary>
		public IEnumerable<(string Id, ICustomBush Data)> GetData();

		/// <summary>Determines if the given Bush instance is a custom bush.</summary>
		/// <param name="bush">The bush instance to check.</param>
		/// <returns>True if the bush is a custom bush, otherwise false.</returns>
		public bool IsCustomBush(Bush bush);

		/// <summary>Tries to get the custom bush model associated with the given bush.</summary>
		/// <param name="bush">The bush.</param>
		/// <param name="customBush">When this method returns, contains the custom bush associated with the given bush, if found; otherwise, it contains null.</param>
		/// <returns>true if the custom bush associated with the given bush is found; otherwise, false.</returns>
		public bool TryGetCustomBush(Bush bush, out ICustomBush? customBush);

		/// <summary>Tries to get the custom bush drop associated with the given bush id.</summary>
		/// <param name="id">The id of the bush.</param>
		/// <param name="drops">When this method returns, contains the items produced by the custom bush.</param>
		/// <returns>true if the drops associated with the given id is found; otherwise, false.</returns>
		public bool TryGetDrops(string id, out IList<ICustomBushDrop>? drops);
	}
}
