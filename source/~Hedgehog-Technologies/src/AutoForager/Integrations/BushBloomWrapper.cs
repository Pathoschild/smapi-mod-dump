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
using System.Threading.Tasks;
using HedgeTech.Common.Extensions;
using StardewModdingAPI;
using StardewValley;

namespace AutoForager.Integrations
{
	internal class BushBloomWrapper
	{
		private const string _minVersion = "1.1.9";
		private const string _bbUniqueId = "NCarigon.BushBloomMod";
		private const int _readyRetries = 120;
		private const int _readyRetryWaitMs = 500;

		private readonly IMonitor _monitor;
		private readonly IModHelper _helper;

		private readonly IBushBloomApi? _bushBloomApi;

		private readonly List<BloomSchedule> _schedules;
		public List<BloomSchedule> Schedules => _schedules;

		public BushBloomWrapper(IMonitor monitor, IModHelper helper)
		{
			_monitor = monitor;
			_helper = helper;
			_schedules = new();

			if (helper.ModRegistry.IsLoaded(_bbUniqueId))
			{
				var bushBloom = helper.ModRegistry.Get(_bbUniqueId);

				if (bushBloom is not null)
				{
					var bbName = bushBloom.Manifest.Name;
					var bbVersion = bushBloom.Manifest.Version;

					if (bbVersion.IsEqualToOrNewerThan(_minVersion))
					{
						monitor.Log(I18n.Log_Wrapper_ModFound(bbName, I18n.Subject_BushBloomSchedules()), LogLevel.Info);
						_bushBloomApi = helper.ModRegistry.GetApi<IBushBloomApi>(_bbUniqueId);
					}
					else
					{
						monitor.Log(I18n.Log_Wrapper_OldVersion(bbName, bbVersion, _minVersion), LogLevel.Warn);
					}
				}
				else
				{
					monitor.Log(I18n.Log_Wrapper_ManifestError("Bush Bloom Mod"), LogLevel.Warn);
				}
			}
		}

		public async Task<List<BloomSchedule>> UpdateSchedules()
		{
			var defaultBlooms = new BloomSchedule[]
			{
				new BloomSchedule("296", new WorldDate(1, Season.Spring, 15), new WorldDate(1, Season.Spring, 18)),
				new BloomSchedule("410", new WorldDate(1, Season.Fall, 8), new WorldDate(1, Season.Fall, 11))
			};

			if (_bushBloomApi is not null)
			{
				var remainingRetries = _readyRetries;

				while (!_bushBloomApi.IsReady() && remainingRetries-- > 0)
				{
					await Task.Delay(_readyRetryWaitMs);
				}

				remainingRetries += 1;
				var retryTime = (_readyRetries - remainingRetries) * _readyRetryWaitMs;

				_monitor.Log($"Bush Bloom Mod status: Ready: {_bushBloomApi.IsReady()} - Remaining retries: {remainingRetries} / {_readyRetries} - Total time: {retryTime}ms", LogLevel.Debug);

				if (_bushBloomApi.IsReady())
				{
					foreach (var sched in _bushBloomApi.GetAllSchedules())
					{
						_schedules.Add(new BloomSchedule(sched));
					}
				}
				else
				{
					_monitor.Log($"Bush Bloom Mod not ready within {retryTime}ms. Continuing with only default bush blooms.", LogLevel.Warn);
				}
			}

			if (_schedules.Count == 0)
			{
				_schedules.AddRange(defaultBlooms);
			}

			return Schedules;
		}
	}

	public class BloomSchedule
	{
		public string ItemId { get; }
		public WorldDate StartDate { get; }
		public WorldDate EndDate { get; }

		public BloomSchedule(string itemId, WorldDate startDate, WorldDate endDate)
		{
			ItemId = itemId;
			StartDate = startDate;
			EndDate = endDate;
		}

		public BloomSchedule((string, WorldDate, WorldDate) schedule)
			: this(schedule.Item1, schedule.Item2, schedule.Item3)
		{ }
	}

	public interface IBushBloomApi
	{
		/// <summary>
		/// Specifies whether BBM successfully parsed all schedules.
		/// </summary>
		bool IsReady();

		/// <summary>
		/// Returns an array of (item_id, first_day, last_day) for all blooming schedules.
		/// </summary>
		(string, WorldDate, WorldDate)[] GetAllSchedules();
	}
}
