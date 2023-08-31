/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-bundles
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Netcode;
using StardewValley;
using Newtonsoft.Json.Linq;
using StardewValley.Network;
using Unlockable_Bundles.NetLib;
using Newtonsoft.Json;

namespace Unlockable_Bundles.Lib
{
    public sealed class ModData
    {
        public static ModData Instance = new ModData();

        //Dic<UnlockableKey, Dic<locationUnique, SaveData>>
        public Dictionary<string, Dictionary<string, UnlockableSaveData>> UnlockableSaveData { get; set; } = new Dictionary<string, Dictionary<string, UnlockableSaveData>>();

        public static bool isUnlockablePurchased(string key, string location)
        {
            ensureExist(key, location);

            return Instance.UnlockableSaveData[key][location].Purchased;

        }

        private static void ensureExist(string key, string location)
        {
            if (Instance == null) //Can happen when a player connects during the day //TODO: Check if still useful
                Instance = new ModData();

            if (!Instance.UnlockableSaveData.ContainsKey(key))
                Instance.UnlockableSaveData[key] = new Dictionary<string, UnlockableSaveData>();

            if (!Instance.UnlockableSaveData[key].ContainsKey(location))
                Instance.UnlockableSaveData[key][location] = new UnlockableSaveData();
        }

        public static void setPurchased(string key, string location, bool value = true)
        {
            ensureExist(key, location);

            Instance.UnlockableSaveData[key][location].Purchased = value;
            Instance.UnlockableSaveData[key][location].DayPurchased = Game1.Date.TotalDays;
            API.UnlockableBundlesAPI.clearCache();
        }

        public static void setPartiallyPurchased(string key, string location, string requirement, int value, int index = -1)
        {
            ensureExist(key, location);

            Instance.UnlockableSaveData[key][location].AlreadyPaid.Add(requirement, value);
            if (index != -1)
                Instance.UnlockableSaveData[key][location].AlreadyPaidIndex.Add(requirement, index);
        }

        //Returns the most recent purchase day spawn of an unlockable or -1
        public static int getDaysSincePurchase(string key)
        {
            if (Instance == null)
                Instance = new ModData();

            if (!Instance.UnlockableSaveData.ContainsKey(key))
                return -1;

            var entries = Instance.UnlockableSaveData[key].Where(e => e.Value.Purchased);

            if (entries.Count() == 0)
                return -1;

            return Game1.Date.TotalDays - entries.OrderBy(e => e.Value.DayPurchased).First().Value.DayPurchased;
        }

        public static void checkLegacySaveData()
        {
            const string customDataKey = "smapi/mod-data/delixx.unlockable_areas/main";

            if (!Game1.CustomData.ContainsKey(customDataKey))
                return;

            var legacy = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, bool>>>>(Game1.CustomData[customDataKey]);

            Game1.CustomData.Remove(customDataKey);

            if (!legacy.ContainsKey("UnlockablePurchased"))
                return;

            foreach (var key in legacy["UnlockablePurchased"])
                foreach (var location in key.Value)
                    setPurchased(key.Key, location.Key, location.Value);
        }

        public static void applySaveData(UnlockableModel unlockable)
        {
            ensureExist(unlockable.ID, unlockable.LocationUnique);
            var savedata = Instance.UnlockableSaveData[unlockable.ID][unlockable.LocationUnique];

            unlockable.AlreadyPaid = savedata.AlreadyPaid;
            unlockable.AlreadyPaidIndex = savedata.AlreadyPaidIndex;

            if (unlockable.RandomPriceEntries > 0) {
                if(savedata.Price.Count == 0)
                    savedata.Price = new Dictionary<string, int>(unlockable.Price.OrderBy(x => Game1.random.Next()).Take(unlockable.RandomPriceEntries));

                unlockable.Price = savedata.Price;
            }
                
        }
    }
}
