/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/elizabethcd/MoreConversationTopics
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using System.Collections.Generic;

namespace MoreConversationTopics
{
    internal static class JojaEventAssetEditor
    {
        private static IMonitor Monitor;
        private static ModConfig Config;

        private static readonly string JOJAEVENTLOCATION = PathUtilities.NormalizeAssetName("Data/Events/Town");

        public static void Initialize(IMonitor monitor, ModConfig config)
        {
            Monitor = monitor;
            Config = config;
        }

        /// <summary>Edit a matched asset.</summary>
        /// <param name="e">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        internal static void Edit(AssetRequestedEventArgs e)
        {
            // Only pull data from the events in town
            
            if (e.NameWithoutLocale.IsEquivalentTo(JOJAEVENTLOCATION))
            {
                e.Edit(static (asset) =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    foreach ((string eventID, string eventScript) in data)
                    {
                        // Only edit the Joja completion ceremony event
                        if (eventID.StartsWith("502261/"))
                        {
                            // Check if the event script is null, just in case
                            if (eventScript is null)
                            {
                                Monitor.Log("Cannot edit Joja completion ceremony event due to null script.", LogLevel.Error);
                                return;
                            }

                            // Find the third slash and insert at that position.
                            int index = -1;
                            int counter = 3;
                            for (int i = 0; i < eventScript.Length; i++)
                            {
                                if (eventScript[i] == '/' && --counter <= 0)
                                {
                                    index = i;
                                    break;
                                }
                            }

                            // Check that there's enough commands in the event for it to be a valid event
                            if (index == -1)
                            {
                                Monitor.Log("Cannot edit Joja completion ceremony event due to script having too few commands to be a functional event.", LogLevel.Warn);
                                return;
                            }

                            data[eventID] = eventScript.Insert(index, $"/addConversationTopic joja_Complete {Config.JojaCompletionDuration}");
                            return;
                        }
                    }
                }, AssetEditPriority.Late);
            }
        }
    }
}