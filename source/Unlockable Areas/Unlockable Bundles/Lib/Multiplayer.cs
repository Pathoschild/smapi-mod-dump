/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unlockable_Bundles.Lib.Enums;
using Unlockable_Bundles.Lib.MapFeatures;
using Unlockable_Bundles.Lib.ShopTypes;
using Unlockable_Bundles.Lib.WalletCurrency;
using Unlockable_Bundles.NetLib;
using static Unlockable_Bundles.ModEntry;

namespace Unlockable_Bundles.Lib
{
    internal class Multiplayer
    {
        public static PerScreen<bool> IsScreenReady = new();
        public static void Initialize()
        {
            Helper.Events.Multiplayer.ModMessageReceived += modMessageReceived;
            Helper.Events.Multiplayer.PeerConnected += peerConnected;
        }

        private static void peerConnected(object sender, PeerConnectedEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            var saveData = ModData.Instance.UnlockableSaveData;
            var unlockables = Helper.GameContent.Load<Dictionary<string, UnlockableModel>>("UnlockableBundles/Bundles");
            List<UnlockableModel> applyList = new();

            foreach (var keyDicPairs in saveData)
                foreach (var locationValue in keyDicPairs.Value)
                    if (unlockables.TryGetValue(keyDicPairs.Key, out UnlockableModel unlockable)) {
                        unlockable.ID = keyDicPairs.Key;
                        unlockable.LocationUnique = locationValue.Key;
                        unlockable.applyDefaultValues();

                        if (locationValue.Value.Purchased)
                            applyList.Add((UnlockableModel)new Unlockable(unlockable)); //Cloning
                    }

            Helper.Multiplayer.SendMessage(new KeyValuePair<List<UnlockableModel>, ModData>(applyList, ModData.Instance), "UnlockablesReady", modIDs: new[] { ModManifest.UniqueID }, playerIDs: new[] { e.Peer.PlayerID });
            Helper.Multiplayer.SendMessage(AssetRequested.MailData, "UpdateMailData", modIDs: new[] { ModManifest.UniqueID });
        }

        private static void modMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != ModManifest.UniqueID)
                return;

            if (e.Type == "UnlockablesReady") {
                if (!Context.IsOnHostComputer)
                    ShopPlacement.resetDay();

                //I have 0 control over when this is run and whether my bundles asset is loaded at this point or not
                //So instead of reading the bundles asset and using ModData for Unlockable data, I transfer all unlockables that need to be applied
                //While mighty unfortunate, I'd rather have lots of redundancy than unreliable and finicky architecture
                //Eventually if this framework becomes popular enough for this to be a performance concern,
                //I could create a new transfer model with all of the fields applyUnlockable needs
                var transferData = e.ReadAs<KeyValuePair<List<UnlockableModel>, ModData>>();

                if (!Context.IsOnHostComputer)
                    ModData.Instance = transferData.Value;
                var applyList = transferData.Key;

                //If the world isn't ready at this point (which it might be) the map patches aren't being applied
                //In that case we apply them in ShopPlacement.DayStarted
                Monitor.Log($"{getDebugName()} IsWorldReady: {Context.IsWorldReady}", DebugLogLevel);

                if (Context.IsWorldReady)
                    foreach (var unlockable in applyList)
                        MapPatches.applyUnlockable(new Unlockable(unlockable), !Context.IsOnHostComputer);
                else
                    ShopPlacement.UnappliedMapPatches.Value = applyList;

                ModAPI.raiseIsReady(new API.IsReadyEventArgs(Game1.player));

            } else if (e.Type == "BundlePurchased") {
                var unlockable = new Unlockable(e.ReadAs<UnlockableModel>());

                ModData.setPurchased(unlockable.ID, unlockable.LocationUnique);
                ModAPI.raiseShopPurchased(new API.BundlePurchasedEventArgs(Game1.player, unlockable.Location, unlockable.LocationUnique, unlockable.ID, false));

                MapPatches.applyUnlockable(unlockable, !MapPatches.AppliedUnlockables.Any(el => el.ID == unlockable.ID && el.LocationUnique == unlockable.LocationUnique));

                if (Game1.activeClickableMenu != null
                    && Game1.activeClickableMenu.GetType() == typeof(DialogueShopMenu)
                    && (Game1.activeClickableMenu as DialogueShopMenu).Unlockable.ID == unlockable.ID)
                    Game1.activeClickableMenu.exitThisMenu();
            } else if (e.Type == "BundleContributed") {
                var unlockable = new Unlockable(e.ReadAs<UnlockableModel>());

                var last = unlockable._alreadyPaid.Pairs.Last();
                var index = unlockable._alreadyPaidIndex.ContainsKey(last.Key) ? unlockable._alreadyPaidIndex[last.Key] : -1;
                ModData.setPartiallyPurchased(unlockable.ID, unlockable.LocationUnique, last.Key, last.Value, index);
                ModAPI.raiseShopContributed(new API.BundleContributedEventArgs(Game1.player, new KeyValuePair<string, int>(last.Key, last.Value), unlockable.Location, unlockable.LocationUnique, unlockable.ID, false));

            } else if (e.Type == "BundleDiscovered") {
                var data = e.ReadAs<BundleDiscoveredTransferModel>();
                ShopObject.setDiscovered(data.id, data.location, data.value);

            } else if (e.Type == "OverviewMenuRequestMissing") {
                var data = e.ReadAs<Dictionary<string, string>>();
                var bundles = ShopObject.getAll();

                var transfer = new List<UnlockableModel>();
                foreach (var bundle in bundles) {
                    if (bundle.WasDiscovered && !data.Contains(new KeyValuePair<string, string>(bundle.Unlockable.ID, bundle.Unlockable.LocationUnique)))
                        transfer.Add((UnlockableModel)bundle.Unlockable);
                }

                Helper.Multiplayer.SendMessage(transfer, "OverviewMenuSendMissing", modIDs: new[] { ModManifest.UniqueID }, playerIDs: new[] { e.FromPlayerID });

            } else if (e.Type == "OverviewMenuSendMissing") {
                if (Game1.activeClickableMenu is not BundleOverviewMenu menu)
                    return;

                var data = e.ReadAs<List<UnlockableModel>>();
                menu.appendMissingBundles(data);

            } else if (e.Type == "UpdateMailData") {
                AssetRequested.MailData = e.ReadAs<Dictionary<string, string>>();

                //Translating the mail if possible
                var unlockables = Helper.GameContent.Load<Dictionary<string, UnlockableModel>>("UnlockableBundles/Bundles");
                foreach (var unlockable in unlockables) {
                    var mailKey = Unlockable.getMailKey(unlockable.Key);

                    if (AssetRequested.MailData.ContainsKey(mailKey))
                        AssetRequested.MailData[mailKey] = unlockable.Value.BundleCompletedMail;
                }

                Helper.GameContent.InvalidateCache("Data/Mail");

            } else if (e.Type == "ReapplyLocation") {
                if (!Context.IsWorldReady)
                    return;

                var location = e.ReadAs<string>();

                var needToBeApplied = MapPatches.AppliedUnlockables.Where(el => el.LocationUnique == location);

                foreach (var unlockable in needToBeApplied)
                    MapPatches.applyUnlockable(new Unlockable(unlockable), false);

            } else if (e.Type == "DebugWarpToHost") {

                var master = Game1.MasterPlayer;
                var masterLocation = master.currentLocation;
                Game1.warpFarmer(masterLocation.Name, master.TilePoint.X + 1, master.TilePoint.Y, 2);

                Game1.player.previousLocationName = Game1.player.currentLocation.Name;
                Game1.currentLocation = Game1.getLocationFromName(masterLocation.NameOrUniqueName, masterLocation.isStructure.Value);
                Game1.currentLocation.reloadMap();
                Game1.locationRequest = new LocationRequest(masterLocation.NameOrUniqueName, masterLocation.isStructure.Value, Game1.currentLocation);
                Game1.xLocationAfterWarp = master.TilePoint.X;
                Game1.yLocationAfterWarp = master.TilePoint.Y;
                Game1.player.position.Value = new Microsoft.Xna.Framework.Vector2(master.Position.X, master.Position.Y);
                Helper.Reflection.GetField<bool>(typeof(Game1), "_isWarping").SetValue(true);

            } else if (e.Type == "SPRUpdated") {
                var data = e.ReadAs<KeyValuePair<PlacementRequirementType, string>>();

                switch (data.Key) {
                    case PlacementRequirementType.TriggerAction:
                        ModData.Instance.SPRTriggerActionKeys.Add(data.Value);
                        break;
                }

                PlacementRequirement.CheckShopPlacement(data.Key);

            } else if (e.Type == "ResetDigSpot") {
                var data = e.ReadAs<DigSpotTransferData>();

                var location = Game1.getLocationFromName(data.Location);
                if (data.Who != 0) {
                    var farmer = Game1.getFarmer(data.Who);
                    DigSpot.resetDigspot(farmer, location, data.X, data.Y, data.ResetMailFlag);

                } else foreach (var farmer in Game1.getAllFarmers())
                        DigSpot.resetDigspot(farmer, location, data.X, data.Y, data.ResetMailFlag);

            } else if (e.Type == "WalletCurrencyChanged") {
                var data = e.ReadAs<CurrencyTransferModel>();
                var currency = WalletCurrencyHandler.getCurrencyById(data.CurrencyId);

                WalletCurrencyHandler.addWalletCurrency(currency, data.Who, data.AddedValue, false, currency.Shared);
            }
        }
        public static string getDebugName() => $"{(Context.IsOnHostComputer ? "P" + Context.ScreenId : "NotOnHostComputer")} {Game1.player.UniqueMultiplayerID}";
    }
}
