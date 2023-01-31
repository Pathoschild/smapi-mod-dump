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
        Globals.EventHelper.Content.AssetRequested += LoadOrEditAssets;
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

    private static void LoadOrEditAssets(object sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(Globals.ContentPath))
        {
            e.LoadFrom(
                () => new Dictionary<string, string>
                {
                    ["Introduction"] = "true"
                },
                AssetLoadPriority.Medium
            );
        }

        // Remove the flag for cc_Greenhouse NOT in progress - otherwise this event never fires
        else if (e.NameWithoutLocale.IsEquivalentTo("Data/Events/Farm"))
        {
            e.Edit(asset =>
            {
                var data = asset.AsDictionary<string, string>().Data;

                data["900553/t 600 1130/Hn ccPantry/w sunny"] =
                    data["900553/t 600 1130/Hn ccPantry/A cc_Greenhouse/w sunny"];
                data.Remove("900553/t 600 1130/Hn ccPantry/A cc_Greenhouse/w sunny");
            });
        }
    }
}
