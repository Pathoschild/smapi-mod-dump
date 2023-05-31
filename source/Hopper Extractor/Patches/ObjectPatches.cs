/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BPavol/stardew-valley-mods
**
*************************************************/

using StardewModdingAPI;
using Microsoft.Xna.Framework;
using StardewValley;

namespace HopperExtractor.Patches
{
    internal class ObjectPatches
    {
        private static IMonitor Monitor;

        // call this method from your Entry class
        internal static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>The method to call after <see cref="Object.minutesElapsed"/>.</summary>
        internal static bool After_MinutesElapsed(Object __instance, GameLocation environment)
        {
            if (Context.IsMainPlayer == false) {
                // Only main player should execute hopper logic
                return true;
            }

            if (!IsHopper(__instance)) {
                return true;
            }

            var hopper = __instance as StardewValley.Objects.Chest;
            // Attempt extract item from object above
            if (environment.objects.TryGetValue(hopper.TileLocation - new Vector2(0, 1), out Object objAbove)) {
                if (objAbove.readyForHarvest.Value == true && objAbove.heldObject.Value != null)
                {
                    TransferItem(objAbove, hopper);
                }
            }

            // Attempt autoload object below hopper
            if (environment.objects.TryGetValue(hopper.TileLocation + new Vector2(0, 1), out Object objBelow))
            {
                var owner = GetOwner(objBelow);
                if (owner == null)
                {
                    return true;
                }

                /**
                 * Fake farmer used for muting sound in actions for actual player.
                 * Current location must be set because Cask can be loaded only in specific locations.
                 */
                var fakeFarmer = new Farmer();
                fakeFarmer.currentLocation = environment;
                AttemptAutoLoad(fakeFarmer, objBelow, hopper);
            }
                    
            return true;
        }

        /// <summary>Get the hopper instance if the object is a hopper.</summary>
        /// <param name="obj">The object to check.</param>
        /// <param name="hopper">The hopper instance.</param>
        /// <returns>Returns whether the object is a hopper.</returns>
        private static bool IsHopper(Object obj)
        {
            return obj is StardewValley.Objects.Chest { SpecialChestType: StardewValley.Objects.Chest.SpecialChestTypes.AutoLoader };
        }

        private static Farmer GetOwner(Object obj)
        {
            long ownerId = obj.owner.Value;
            //Monitor.Log($"Owner ID {obj.owner.Value}.", LogLevel.Debug);
            if (ownerId == 0) {
                return null;
            }

            return Game1.getFarmerMaybeOffline(ownerId);
        }

        private static void TransferItem(Object machine, StardewValley.Objects.Chest hopper)
        {
            var heldObject = machine.heldObject;
            hopper.addItem(heldObject.Value);

            machine.heldObject.Value = null;
            machine.readyForHarvest.Value = false;
            if (machine is StardewValley.Objects.Cask cask)
            {
                cask.MinutesUntilReady = 0;
                cask.agingRate.Value = 0;
                cask.daysToMature.Value = 0;
            }
            //Monitor.Log($"Extracting {objAbove.DisplayName}.", LogLevel.Debug);
        }

        private static void AttemptAutoLoad(Farmer who, Object machine, StardewValley.Objects.Chest hopper)
        {
            if (hopper is not StardewValley.Objects.Chest { SpecialChestType: StardewValley.Objects.Chest.SpecialChestTypes.AutoLoader }) {
                Monitor.Log($"Chest {hopper.DisplayName} is not autoloader.", LogLevel.Debug);
                return;
            }

            hopper.GetMutex().RequestLock((System.Action)(() =>
            {
                if (machine.heldObject.Value != null)
                {
                    //Monitor.Log($"Machine {machine.DisplayName} is not empty.", LogLevel.Debug);
                    hopper.GetMutex().ReleaseLock();
                    return;
                }

                foreach (Item obj in (Netcode.NetList<Item, Netcode.NetRef<Item>>)hopper.items)
                {
                    Object.autoLoadChest = hopper;
                    int num = machine.performObjectDropInAction(obj, true, who) ? 1 : 0;
                    machine.heldObject.Value = (Object)null;
                    if (num != 0)
                    {
                        //Monitor.Log($"Autoloading {obj.DisplayName} to {machine.DisplayName}.", LogLevel.Debug);
                        if (machine.performObjectDropInAction(obj, false, who))
                            machine.ConsumeInventoryItem(who, obj, 1);
                        Object.autoLoadChest = (StardewValley.Objects.Chest)null;

                        hopper.GetMutex().ReleaseLock();
                        return;
                    }
                }
                Object.autoLoadChest = (StardewValley.Objects.Chest)null;

                hopper.GetMutex().ReleaseLock();
            }));
        }
    }
}
