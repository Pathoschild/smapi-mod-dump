/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
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
using StardewModdingAPI;
using static Unlockable_Bundles.ModEntry;
using Unlockable_Bundles.Lib.WalletCurrency;

namespace Unlockable_Bundles.Lib
{
    public sealed class ModData
    {
        public static ModData Instance = null;

        //Dic<UnlockableKey, Dic<locationUnique, SaveData>>
        public Dictionary<string, Dictionary<string, UnlockableSaveData>> UnlockableSaveData { get; set; } = new();
        //Dic<Location:x,y , List<Farmers>>
        public Dictionary<string, List<long>> FoundUniqueDigSpots { get; set; } = new();

        //Bundle Keys of SpecialPlacementRequirements
        public List<string> SPRTriggerActionKeys { get; set; } = new();
        //Dic< CurrencyId, Dic< playerId, value >>
        //if it's shared the main players will be used
        //If no entry in the second dictionary exists I'll treat it as the player not having obtained this currency yet
        public Dictionary<string, Dictionary<long, int>> WalletCurrencyData { get; set; } = new();
        public Dictionary<string, Dictionary<long, long>> WalletCurrencyTotal { get; set; } = new();

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

            Instance.UnlockableSaveData[key][location].AlreadyPaid.TryAdd(requirement, value); //TryAdd because this code can run twice in splitscreen
            if (index != -1)
                Instance.UnlockableSaveData[key][location].AlreadyPaidIndex.TryAdd(requirement, index);
            API.UnlockableBundlesAPI.clearCache();
        }

        public static void setDiscovered(string key, string location, bool value = true)
        {
            ensureExist(key, location);

            Instance.UnlockableSaveData[key][location].Discovered = value;
            API.UnlockableBundlesAPI.clearCache();
        }

        public static bool getDiscovered(string key, string location, bool value = true)
        {
            ensureExist(key, location);

            return Instance.UnlockableSaveData[key][location].Discovered;
        }

        //Returns the most recent purchase day of an unlockable or -1
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

        public static UnlockableSaveData getUnlockableSaveData(string key, string location)
        {
            ensureExist(key, location);

            return Instance.UnlockableSaveData[key][location];
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

            if (savedata.Purchased) //We currently don't need to apply savedata for purchased unlockables, since all we do with them is apply map patches
                return;

            applyPriceMigration(unlockable, savedata);
            applyRandomPriceEntries(unlockable, savedata);
            ensureBundleCanBeCompleted(unlockable);

            unlockable.AlreadyPaid = savedata.AlreadyPaid;
            unlockable.AlreadyPaidIndex = savedata.AlreadyPaidIndex;


        }

        private static void ensureBundleCanBeCompleted(UnlockableModel unlockable)
        {
            //Safefail when for whatever reason there's fewer items than have to be submitted
            if(unlockable.BundleSlots > unlockable.Price.Count)
                unlockable.BundleSlots = unlockable.Price.Count;
        }

        private static void applyRandomPriceEntries(UnlockableModel unlockable, UnlockableSaveData savedata)
        {
            if (unlockable.RandomPriceEntries > 0) {
                if (savedata.Price.Count == 0)
                    savedata.Price = new Dictionary<string, int>(unlockable.Price.OrderBy(x => Game1.random.Next()).Take(unlockable.RandomPriceEntries));

                unlockable.Price = savedata.Price;
            }
        }

        private static void applyPriceMigration(UnlockableModel unlockable, UnlockableSaveData savedata)
        {
            foreach (var migration in unlockable.PriceMigration) {
                if (!savedata.Price.ContainsKey(migration.Key) && !savedata.AlreadyPaid.ContainsKey(migration.Key) && !savedata.AlreadyPaidIndex.ContainsKey(migration.Key))
                    continue;

                if (migration.Value.Trim().ToLower() == "remove") { //Remove Keyword
                    savedata.Price.Remove(migration.Key);
                    savedata.AlreadyPaid.Remove(migration.Key);
                    savedata.AlreadyPaidIndex.Remove(migration.Key);
                    Monitor.Log($"Removed price savedata in bundle '{unlockable.ID}' for '{migration.Key}'");
                    continue;
                }

                if (migration.Value.Trim().ToLower() == "reroll") { //Reroll
                    if (unlockable.RandomPriceEntries <= 0) {
                        Monitor.LogOnce($"PriceMigration reroll requested for '{migration.Key}' of bundle '{unlockable.ID}', but this bundle does not remember its Price entries.\n"
                            + "Reroll request will be ignored", LogLevel.Warn);
                        continue;
                    }

                    var unusedPriceEntries = unlockable.Price.Where(e => !savedata.Price.ContainsKey(e.Key)).ToDictionary(x => x.Key, x => x.Value);

                    if (unusedPriceEntries.Count == 0) {
                        Monitor.LogOnce($"PriceMigration reroll requested for '{migration.Key}' of bundle '{unlockable.ID}', but there's no unused Price entries left\n"
                            + "Reroll request will be ignored", LogLevel.Warn);
                        continue;
                    }

                    var randomPrice = unusedPriceEntries.ElementAt(Game1.random.Next(unusedPriceEntries.Count));
                    applyPriceEntryMigration(unlockable, savedata, migration.Key, randomPrice.Key, randomPrice.Value);
                    continue;
                }

                if (!unlockable.Price.TryGetValue(migration.Value, out var newPriceAmount)) {
                    Monitor.LogOnce($"PriceMigration requested from '{migration.Key}' to '{migration.Value}' of bundle '{unlockable.ID}' without a matching Price entry.\n"
                        + "Migration request will be ignored.", LogLevel.Warn);
                    continue;
                }

                applyPriceEntryMigration(unlockable, savedata, migration.Key, migration.Value, newPriceAmount);
            }
        }

        private static void applyPriceEntryMigration(UnlockableModel unlockable, UnlockableSaveData savedata, string oldPriceKey, string newPriceKey, int newPriceAmount)
        {
            if (savedata.AlreadyPaid.TryGetValue(oldPriceKey, out var alreadyPaidAmount)) {
                savedata.AlreadyPaid.Remove(oldPriceKey);
                savedata.AlreadyPaid.Add(newPriceKey, alreadyPaidAmount);

                if (savedata.AlreadyPaidIndex.TryGetValue(oldPriceKey, out var alreadyPaidIndex)) {
                    savedata.AlreadyPaidIndex.Remove(oldPriceKey);
                    savedata.AlreadyPaidIndex.Add(newPriceKey, alreadyPaidIndex);
                }
            }

            if(savedata.Price.Count != 0) {
                savedata.Price.Remove(oldPriceKey);
                savedata.Price.Add(newPriceKey, newPriceAmount);
            }

            Monitor.Log($"Migrated Price in bundle '{unlockable.ID}' from '{oldPriceKey}' to '{newPriceKey}':'{newPriceAmount}'");
        }

        private static void addWalletCurrencyTotal(string currencyId, long who, int value)
        {
            if (value <= 0)
                return;

            if (!Instance.WalletCurrencyTotal.TryGetValue(currencyId, out var data))
                Instance.WalletCurrencyTotal.Add(currencyId, data = new());

            if (!data.ContainsKey(who))
                data.Add(who, 0);

            data[who] += value;
        }
        public static int addWalletCurrency(string currencyId, long who, int value)
        {
            addWalletCurrencyTotal(currencyId, who, value);

            if (!Instance.WalletCurrencyData.TryGetValue(currencyId, out var data))
                Instance.WalletCurrencyData.Add(currencyId, data = new());

            if (!data.ContainsKey(who))
                data.Add(who, 0);

            return data[who] = Math.Clamp(data[who] + value, 0, int.MaxValue);
        }

        public static int getWalletCurrency(string currencyId, long who)
        {
            if (!Instance.WalletCurrencyData.TryGetValue(currencyId, out var data))
                return 0;

            if (!data.ContainsKey(who))
                return 0;

            return data[who];
        }

        public static long getWalletCurrencyTotal(string currencyId, long who)
        {
            if (!Instance.WalletCurrencyTotal.TryGetValue(currencyId, out var data))
                return 0;

            if (!data.ContainsKey(who))
                return 0;

            return data[who];
        }

        public static long getCollectiveWalletCurrency(string currencyId)
        {
            if (!Instance.WalletCurrencyData.TryGetValue(currencyId, out var data))
                return 0;

            long value = 0;
            foreach (var e in data)
                value += e.Value;

            return value;
        }

        public static long getCollectiveWalletCurrencyTotal(string currencyId)
        {
            if (!Instance.WalletCurrencyTotal.TryGetValue(currencyId, out var data))
                return 0;

            long value = 0;
            foreach (var e in data)
                value += e.Value;

            return value;
        }

        public static bool walletCurrencyDiscovered(string currencyId, long who)
        {
            if (!Instance.WalletCurrencyData.TryGetValue(currencyId, out var data))
                return false;

            return data.ContainsKey(who);
        }
    }
}
