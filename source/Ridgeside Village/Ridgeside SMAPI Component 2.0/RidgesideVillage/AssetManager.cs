/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Rafseazz/Ridgeside-Village-Mod
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

namespace RidgesideVillage
{
    internal static class AssetManager
    {
        private static readonly string CUSTOM_EVENTS = PathUtilities.NormalizeAssetName("Data/Events/Custom_Ridgeside");
        internal static void LoadEmptyJsons(AssetRequestedEventArgs e)
        {
            // Load in files for events
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Events/AdventureGuild")
                || e.NameWithoutLocale.BaseName.StartsWith(CUSTOM_EVENTS))
            {
                e.LoadFrom(() => new Dictionary<string, string>(), AssetLoadPriority.Low);
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/CustomWeddingGuestPositions"))
            { // Zero clue what this is, is this fully implemented?
                e.LoadFrom(() => new Dictionary<string, string>(), AssetLoadPriority.Low);
            }
        }
    }
}
