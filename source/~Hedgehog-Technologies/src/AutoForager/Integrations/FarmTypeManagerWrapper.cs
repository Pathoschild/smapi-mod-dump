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
using StardewModdingAPI;
using HedgeTech.Common.Extensions;

namespace AutoForager.Integrations
{
	internal class FarmTypeManagerWrapper
	{
		private const string _minVersion = "1.20.0";
		private const string _ftmUniqueId = "Esca.FarmTypeManager";
		private const int _readyRetries = 120;
		private const int _readyRetryWaitMs = 500;

		private readonly IMonitor _monitor;
		private readonly IModHelper _helper;

		private readonly IFarmTypeManagerApi? _ftmApi;

		private IDictionary<string, IEnumerable<string>> _forageIdsPerContentPack;
		public IDictionary<string, IEnumerable<string>> ForageIdsPerContentPack => _forageIdsPerContentPack;

		public FarmTypeManagerWrapper(IMonitor monitor, IModHelper helper)
		{
			_monitor = monitor;
			_helper = helper;
			_forageIdsPerContentPack = new Dictionary<string, IEnumerable<string>>();

			if (helper.ModRegistry.IsLoaded(_ftmUniqueId))
			{
				var ftm = helper.ModRegistry.Get(_ftmUniqueId);

				if (ftm is not null)
				{
					var ftmName = ftm.Manifest.Name;
					var ftmVersion = ftm.Manifest.Version;

					if (ftmVersion.IsEqualToOrNewerThan(_minVersion))
					{
						monitor.Log(I18n.Log_Wrapper_ModFound(ftmName, I18n.Subject_SpawnableForageIds()), LogLevel.Info);
						_ftmApi = helper.ModRegistry.GetApi<IFarmTypeManagerApi>(_ftmUniqueId);
					}
					else
					{
						monitor.Log(I18n.Log_Wrapper_OldVersion(ftmName, ftmVersion, _minVersion), LogLevel.Warn);
					}
				}
				else
				{
					monitor.Log(I18n.Log_Wrapper_ManifestError("Farm Type Manager"), LogLevel.Warn);
				}
			}
		}

		public IDictionary<string, IEnumerable<string>> UpdateForageIds()
		{
			if (_ftmApi is not null)
			{
				_forageIdsPerContentPack = _ftmApi.GetForageIDsFromContentPacks();
			}

			return _forageIdsPerContentPack;
		}
	}

	/// <summary>The public API interface for Farm Type Manager (FTM), provided through SMAPI's mod helper.</summary>
	public interface IFarmTypeManagerApi
	{
		/// <summary>Gets information about all the valid forage IDs in loaded FTM content packs. Keys are content pack IDs (or "" for other sources). Values are a list of each valid, qualified item ID.</summary>
		/// <remarks>
		/// This method will produce information about the current in-game day.
		/// It is available as soon as SMAPI has loaded all content packs, e.g. during GameLaunched events.
		/// Results may change after FTM's DayStarted events, during which FTM reloads all content pack data.
		/// Data from save-specific personal config files will only be included if "Context.IsWorldReady" is true.
		/// </remarks>
		/// <param name="includePlacedItems">If true, this list will include the IDs of forage items that are NOT normal <see cref="StardewValley.Object"/> instances. These spawn inside a custom <see cref="TerrainFeature"/> subclass, but imitate most normal forage behavior.</param>
		/// <param name="includeContainers">If true, this list will include the IDs of containers that are spawned as forage (chests, breakable barrels and crates, etc).</param>
		public IDictionary<string, IEnumerable<string>> GetForageIDsFromContentPacks(bool includePlacedItems = false, bool includeContainers = false);
	}
}
