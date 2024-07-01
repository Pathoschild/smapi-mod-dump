/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections.Generic;
using StardewArchipelago.Archipelago;

namespace StardewArchipelago.Serialization
{
    public class ArchipelagoStateDto
    {
        public ArchipelagoConnectionInfo APConnectionInfo { get; set; }
        public List<ReceivedItem> ItemsReceived { get; set; }
        public List<string> LocationsChecked { get; set; }
        public Dictionary<string, ScoutedLocation> LocationsScouted { get; set; }
        public Dictionary<string, string> LettersGenerated { get; set; }
        public List<string> SeasonsOrder { get; set; }
        public AppearanceRandomization? AppearanceRandomizerOverride { get; set; }
        public TrapItemsDifficulty? TrapDifficultyOverride { get; set; }
        public int TravelingMerchantPurchases { get; set; }
        public int StoredStarTokens { get; set; }
        public List<string> EntrancesTraversed { get; set; }
        public List<bool> CurrentRaccoonBundleStatus { get; set; }

        public ArchipelagoStateDto()
        {
            ItemsReceived = new List<ReceivedItem>();
            LocationsChecked = new List<string>();
            LocationsScouted = new Dictionary<string, ScoutedLocation>();
            LettersGenerated = new Dictionary<string, string>();
            SeasonsOrder = new List<string>();
            AppearanceRandomizerOverride = null;
            TrapDifficultyOverride = null;
            TravelingMerchantPurchases = 0;
            StoredStarTokens = 0;
            EntrancesTraversed = new List<string>();
            CurrentRaccoonBundleStatus = new List<bool>();
        }
    }
}
