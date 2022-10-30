/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MissCoriel/Event-Repeater
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using StardewValley;

namespace EventRepeater.Framework;
internal static class AssetManager
{
    internal static readonly string MailToRepeatName = PathUtilities.NormalizeAssetName("Mods/EventRepeater/MailToRepeat");
    private static readonly string EventsToRepeatName = PathUtilities.NormalizeAssetName("Mods/EventRepeater/EventsToRepeat");
    private static readonly string ResponsesToRepeatName = PathUtilities.NormalizeAssetName("Mods/EventRepeater/ResponsesToRepeat");

    private static IAssetName EventsToRepeatAsset = null!;
    private static IAssetName ResponsesToRepeatAsset = null!;

    private static IMonitor Monitor = null!;

    internal static Lazy<HashSet<int>> EventsToForget { get; private set; } = new(() => PopulateAsset(EventsToRepeatName));
    internal static Lazy<HashSet<int>> ResponseToForget { get; private set; } = new(() => PopulateAsset(ResponsesToRepeatName));

    internal static void Initialize(IGameContentHelper helper, IMonitor monitor)
    {
        EventsToRepeatAsset = helper.ParseAssetName(EventsToRepeatName);
        ResponsesToRepeatAsset = helper.ParseAssetName(ResponsesToRepeatName);

        Monitor = monitor;
    }

    internal static void Apply(AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(MailToRepeatName)
            || e.NameWithoutLocale.IsEquivalentTo(EventsToRepeatAsset)
            || e.NameWithoutLocale.IsEquivalentTo(ResponsesToRepeatAsset))
        {
            e.LoadFrom(static () => new Dictionary<string, string>(), AssetLoadPriority.Exclusive); 
        }
    }

    internal static void Reset(IReadOnlySet<IAssetName>? assets)
    {
        if ((assets is null || assets.Contains(EventsToRepeatAsset)) && EventsToForget.IsValueCreated)
            EventsToForget = new(() => PopulateAsset(EventsToRepeatName));
        if ((assets is null || assets.Contains(ResponsesToRepeatAsset)) && ResponseToForget.IsValueCreated)
            ResponseToForget = new(() => PopulateAsset(ResponsesToRepeatName));
    }

    private static HashSet<int> PopulateAsset(string assetName)
    {
        HashSet<int> ret = new();
        foreach (var str in Game1.content.Load<Dictionary<string, string>>(assetName).Keys)
        {
            if (int.TryParse(str, out var val))
                ret.Add(val);
            else
                Monitor.Log($"{str} is not a valid ID for {assetName}, skipping", LogLevel.Warn);
        }
        return ret;
    }
}
