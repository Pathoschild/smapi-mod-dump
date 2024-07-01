/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/unlockable-bundles
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unlockable_Bundles.Lib.Enums;
using static Unlockable_Bundles.ModEntry;

namespace Unlockable_Bundles.Lib
{
    public class PlacementRequirement
    {
        public const string TriggerAction = "UB_PlaceBundle";

        public PlacementRequirementType Type;

        public bool Fulfilled;

        public string BundleKey = "";

        public bool BundleSharesBuilding;

        public int Time;

        public string TriggerActionKey = "";
        public static void Initialize()
        {
            Helper.Events.GameLoop.TimeChanged += TimeChanged;
            TriggerActionManager.RegisterAction(TriggerAction, ProcessTriggerAction);
        }

        private static bool ProcessTriggerAction(string[] args, TriggerActionContext context, out string error)
        {
            if (!ArgUtility.TryGet(args, 1, out var bundleKey, out error))
                return false;

            if(!Context.IsMainPlayer) {
                Helper.Multiplayer.SendMessage(new KeyValuePair<PlacementRequirementType, string>(PlacementRequirementType.TriggerAction, bundleKey), "SPRUpdated", modIDs: new[] { ModManifest.UniqueID }, playerIDs: new[] { Game1.MasterPlayer.UniqueMultiplayerID });
                return true;
            }

            if (!ModData.Instance.SPRTriggerActionKeys.Contains(bundleKey)) {
                ModData.Instance.SPRTriggerActionKeys.Add(bundleKey);

                CheckShopPlacement(PlacementRequirementType.TriggerAction);
            }

            return true;
        }

        //This is called whenever a SPR might have changed and bundle shop placements need to be reevaluated
        public static void CheckShopPlacement(PlacementRequirementType Type)
        {
            foreach (var unlockable in ShopPlacement.BundlesWaitingForTrigger.ToList()) {
                var allFulfilled = true;
                foreach (var requirement in unlockable.SpecialPlacementRequirements) {
                    if (requirement.Type == Type && !requirement.Fulfilled)
                        requirement.UpdateFulfilled(unlockable);

                    if (!requirement.Fulfilled)
                        allFulfilled = false;
                }

                if (allFulfilled) {
                    var location = Game1.getLocationFromName(unlockable.LocationUnique);
                    ShopPlacement.placeShop(unlockable, location);
                    ShopPlacement.BundlesWaitingForTrigger.Remove(unlockable);
                }
            }
        }

        private static void TimeChanged(object sender, StardewModdingAPI.Events.TimeChangedEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            CheckShopPlacement(PlacementRequirementType.TimeReached);
        }

        public PlacementRequirement Clone()
        {
            return new PlacementRequirement() {
                Type = Type,
                Fulfilled = Fulfilled,
                BundleKey = BundleKey,
                BundleSharesBuilding = BundleSharesBuilding,
                Time = Time
            };
        }

        public static List<PlacementRequirement> CloneList(List<PlacementRequirement> list)
        {
            List<PlacementRequirement> ret = new();

            foreach (var item in list)
                ret.Add(item.Clone());

            return ret;
        }

        public bool UpdateFulfilled(Unlockable unlockable)
        {
            switch (Type) {
                case PlacementRequirementType.TimeReached:
                    return Fulfilled = Game1.timeOfDay >= Time;

                case PlacementRequirementType.BundleCompletion:
                    if (!ModData.Instance.UnlockableSaveData.TryGetValue(BundleKey, out var RelevantEntries))
                        return Fulfilled = false;

                    if (!BundleSharesBuilding)
                        return Fulfilled = RelevantEntries.Any(el => el.Value.Purchased);

                    if (!RelevantEntries.TryGetValue(unlockable.LocationUnique, out var RelevantBundle))
                        return Fulfilled = false;

                    return Fulfilled = RelevantBundle.Purchased;

                case PlacementRequirementType.TriggerAction:
                    return Fulfilled = ModData.Instance.SPRTriggerActionKeys.Contains(TriggerActionKey == "" ? unlockable.ID : TriggerActionKey);
            }

            return true;
        }
    }
}
