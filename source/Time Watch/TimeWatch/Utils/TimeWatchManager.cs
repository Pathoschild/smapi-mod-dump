/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KyuubiRan/TimeWatch
**
*************************************************/

using Newtonsoft.Json;
using StardewValley;
using TimeWatch.Data;

namespace TimeWatch.Utils;

internal static class TimeWatchManager
{
    public static readonly Dictionary<long, MagicTimeWatch> TimeWatches = new();
    private const string Key = "kyuubiran.TimeWatch/TimeWatchData";

    public static MagicTimeWatch GetTimeWatch(long id)
    {
        return TimeWatches.TryGetValue(id, out var watch)
            ? watch
            : TimeWatches[id] = new MagicTimeWatch();
    }

    public static MagicTimeWatch CurrentPlayerTimeWatch => GetTimeWatch(Game1.player);

    public static MagicTimeWatch GetTimeWatch(Farmer player)
    {
        return GetTimeWatch(player.UniqueMultiplayerID);
    }

    public static int AddTime(Farmer player, int cnt)
    {
        return GetTimeWatch(player).Add(cnt);
    }

    public static void OnSave()
    {
        foreach (var farmer in Game1.getAllFarmers())
        {
            if (TimeWatches.TryGetValue(farmer.UniqueMultiplayerID, out var watch))
            {
                farmer.modData[Key] = JsonConvert.SerializeObject(watch);
            }
        }
    }

    public static void OnLoad()
    {
        foreach (var farmer in Game1.getAllFarmers())
        {
            if (!farmer.modData.TryGetValue(Key, out var data))
                continue;

            try
            {
                TimeWatches[farmer.UniqueMultiplayerID] = JsonConvert.DeserializeObject<MagicTimeWatch>(data)!;
            }
            catch
            {
                TimeWatches[farmer.UniqueMultiplayerID] = new MagicTimeWatch();
            }
        }
    }
}