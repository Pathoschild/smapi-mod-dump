/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System.Collections.Generic;
using Netcode;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Network;

namespace LastingCTs;

internal class EventHookManager
{
    internal static void InitializeEventHooks()
    {
        Globals.EventHelper.Content.AssetRequested += LoadAssets;
        Globals.EventHelper.GameLoop.DayStarted += ProcessConversationTopics;
    }

    [EventPriority(EventPriority.Low)]
    private static void ProcessConversationTopics(object sender, DayStartedEventArgs e)
    {
        // grab the player's active CTs
        NetStringDictionary<int, NetInt> activeCTs = Game1.player.activeDialogueEvents;

        // flush and reload the asset which tells us which CTs to preserve
        Globals.GameContent.InvalidateCache(Globals.ContentPath);
        var lastingCTs = Globals.GameContent
            .Load<Dictionary<string, string>>(Globals.ContentPath).Keys;

        foreach (string ct in lastingCTs)
        {
            if (!activeCTs.ContainsKey(ct))
            {
                activeCTs.Add(ct, 1);
            }
            else if (activeCTs[ct] < 1)
            {
                activeCTs[ct] = 1;
            }
        }
    }

    private static void LoadAssets(object sender, AssetRequestedEventArgs e)
    {
        if (e.Name.IsEquivalentTo(Globals.ContentPath))
        {
            e.LoadFrom(
                () => new Dictionary<string, string>
                {
                    ["Introduction"] = "true"
                },
                AssetLoadPriority.Medium
            );
        }
    }
}
