/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Items.Machines;
using SObject = StardewValley.Object;

namespace TehPers.CoreMod.Items.Machines {
    internal static class MachineDelegator {
        private static bool _patched;

        private static readonly Dictionary<LocationPosition, IMachineInformation> _trackedMachines = new Dictionary<LocationPosition, IMachineInformation>();

        public static void PatchIfNeeded() {
            if (MachineDelegator._patched) {
                return;
            }

            MachineDelegator._patched = true;

            HarmonyInstance harmony = HarmonyInstance.Create("TehPers.CoreMod.Machines");

            // GameLocation.UpdateWhenCurrentLocation
            MethodInfo target = typeof(GameLocation).GetMethod(nameof(GameLocation.UpdateWhenCurrentLocation), BindingFlags.Public | BindingFlags.Instance);
            MethodInfo prefix = typeof(MachineDelegator).GetMethod(nameof(MachineDelegator.UpdateWhenCurrentLocationPrefix), BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(target, new HarmonyMethod(prefix));

            // SObject.draw
            target = typeof(SObject).GetMethod(nameof(SObject.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) });
            MethodInfo postfix = typeof(MachineDelegator).GetMethod(nameof(MachineDelegator.DrawPostfix), BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(target, postfix: new HarmonyMethod(postfix));

            // SObject.checkForAction
            target = typeof(SObject).GetMethod(nameof(SObject.checkForAction), BindingFlags.Public | BindingFlags.Instance);
            prefix = typeof(MachineDelegator).GetMethod(nameof(MachineDelegator.CheckForActionPrefix), BindingFlags.NonPublic | BindingFlags.Static);
            postfix = typeof(MachineDelegator).GetMethod(nameof(MachineDelegator.CheckForActionPostfix), BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(target, new HarmonyMethod(prefix), new HarmonyMethod(postfix));
        }

        public static void RegisterEvents(IMod mod) {
            throw new NotImplementedException();

            // Track changes in game locations
            // mod.Helper.Events.World.ObjectListChanged += (sender, changed) => {
            //     // Create new states for added machines
            //     foreach ((Vector2 position, SObject addedObject) in changed.Added) {
            //         // Check if the object added was a registered machine
            //         if (!(ItemDelegator.TryGetInformation(addedObject.ParentSheetIndex, out IObjectInformation info) && info.Manager is IMachine machine)) {
            //             continue;
            //         }
            // 
            //         // Start tracking the state of the machine
            //         IMachineInformation state = new MachineInformation(changed.Location, position);
            //         MachineDelegator._trackedMachines[new LocationPosition(changed.Location, position)] = state;
            // 
            //         // Signal the machine's manager
            //         machine.Placed(state);
            //     }
            // 
            //     // Remove tracked states for removed objects
            //     foreach ((Vector2 position, SObject removedObject) in changed.Removed) {
            //         // Try to get the object's machine state
            //         if (!MachineDelegator.TryGetMachineState(removedObject, changed.Location, out IMachine machine, out IMachineInformation state)) {
            //             return;
            //         }
            // 
            //         // Stop tracking the state of the machine
            //         MachineDelegator._trackedMachines.Remove(new LocationPosition(changed.Location, position));
            // 
            //         // Signal the machine's manager
            //         machine.Removed(state);
            //     }
            // };
        }

        private static void UpdateWhenCurrentLocationPrefix(GameLocation __instance) {
            // Check each object in the location
            foreach (SObject curObject in __instance.Objects.Values) {
                // Try to get the object's machine state
                if (!MachineDelegator.TryGetMachineState(curObject, __instance, out IMachine machine, out IMachineInformation state)) {
                    return;
                }

                // Update the state of the machine
                machine.UpdateTick(state);

                // Check if the machine has output available for the farmer
                IEnumerable<ObjectRequest> output = machine.RemoveItem(state, Game1.player, out _);

                // Display the output if so
                if (output?.FirstOrDefault() is ObjectRequest visibleOutput) {
                    curObject.heldObject.Value = visibleOutput.Item;
                    curObject.readyForHarvest.Value = true;
                } else {
                    curObject.heldObject.Value = null;
                    curObject.readyForHarvest.Value = false;
                }

                // TODO: call the delegate returned by RemoveItem when a player right clicks the machine to get the held object
            }
        }

        private static void DrawPostfix(SObject __instance, SpriteBatch spriteBatch, int x, int y) {
            // Try to get the object's machine state
            if (!MachineDelegator.TryGetMachineState(__instance, Game1.currentLocation, out IMachine machine, out IMachineInformation state)) {
                return;
            }

            // Signal the machine's manager
            machine.AfterDraw(state, spriteBatch, x, y);
        }

        private static bool CheckForActionPrefix(SObject __instance, ref CheckForActionState __state, ref bool __result, Farmer who, bool justCheckingForActivity) {
            return true;

            if (who?.ActiveObject != null) {
                // Try to get the object's machine state
                if (!MachineDelegator.TryGetMachineState(__instance, Game1.currentLocation, out IMachine machine, out IMachineInformation state)) {
                    return true;
                }

                // Get the machine's insert action
                List<ObjectRequest> requestedPayload = machine.InsertItem(state, who.ActiveObject, who.Items.ToArray(), who, out Action doInsert)?.ToList() ?? new List<ObjectRequest>();

                // Check if there is an insert action
                if (doInsert != null) {
                    // Check if the game is just probing
                    if (justCheckingForActivity) {
                        __result = true;
                        __state = new CheckForActionState(false, null);
                        return false;
                    }

                    // Track the original sizes of each modified item in the player's inventory
                    Dictionary<Item, int> originalStacks = new Dictionary<Item, int>();

                    // Try to satisfy every request
                    HashSet<ObjectRequest> unsatisfiedRequests = requestedPayload.ToHashSet();
                    bool failed = false;
                    foreach (ObjectRequest request in requestedPayload) {
                        // Keep track of how much of this request is left
                        int remaining = request.Quantity;

                        // Keep searching the player's inventory until either the request is satisfied or there are no more items to satisfy it with
                        while (remaining > 0) {
                            // Try to find that exact item in the player's inventory
                            Item inventoryItem = who.Items.FirstOrDefault(i => i != null && i.Stack > 0 && i == request.Item);

                            // Try to find another item that matches the requested item if the exact item wasn't found
                            if (inventoryItem == null) {
                                inventoryItem = who.Items.FirstOrDefault(i => i != null && i.Stack > 0 && i.ParentSheetIndex == request.Item.ParentSheetIndex);
                            }

                            // Check if the request can't be satisfied
                            if (inventoryItem == null) {
                                failed = true;
                                break;
                            }

                            // Track the original stack size of the item
                            if (!originalStacks.ContainsKey(inventoryItem)) {
                                originalStacks.Add(inventoryItem, inventoryItem.Stack);
                            }

                            // Decrement the quantity as much as possible
                            int removedFromItem = Math.Min(inventoryItem.Stack, remaining);
                            remaining -= removedFromItem;
                            inventoryItem.Stack -= removedFromItem;
                        }

                        // Check if the request failed
                        if (failed) {
                            // Stop trying to satisfy requests
                            break;
                        }

                        // Check if the request was satisfied
                        if (remaining == 0) {
                            // Add the satisfied request
                            unsatisfiedRequests.Remove(request);
                        }
                    }

                    // Check if the request failed
                    if (failed) {
                        // Revert changes to the player's inventory
                        foreach ((Item item, int quantity) in originalStacks) {
                            item.Stack = quantity;
                        }
                    } else {
                        // Remove empty stacks from the player's inventory
                        foreach ((Item item, _) in originalStacks) {
                            if (item.Stack == 0) {
                                who.removeItemFromInventory(item);
                            }
                        }

                        // Update the state of the machine
                        doInsert();

                        // Set the state so the postfix doesn't check if an item was removed
                        __state = new CheckForActionState(false, null);
                    }
                }
            }

            // Track this object to see if an item was removed from it
            __state = new CheckForActionState(__instance.readyForHarvest.Value, __instance.heldObject.Value);
            return true;
        }

        private static void CheckForActionPostfix(SObject __instance, CheckForActionState __state, Farmer who) {
            // Check if object was ready for harvest
            if (!__state.Ready) {
                return;
            }

            // Check what changed
            int removedCount;
            if (__instance.heldObject?.Value == null || __instance.heldObject.Value.ParentSheetIndex != __state.Held.ParentSheetIndex) {
                removedCount = __state.Stack;
            } else {
                removedCount = __state.Stack - __instance.heldObject.Value.Stack;
            }

            // Make sure stuff was removed
            if (removedCount <= 0) {
                return;
            }

            // Try to get the object's machine state
            if (!MachineDelegator.TryGetMachineState(__instance, Game1.currentLocation, out IMachine machine, out IMachineInformation state)) {
                return;
            }

            // Get the machine's remove action
            _ = machine.RemoveItem(state, who, out Action doRemove);

            // Update the state of the machine
            doRemove();
        }

        private static bool TryGetMachineState(SObject obj, GameLocation location, out IMachine machine, out IMachineInformation state) {
            throw new NotImplementedException();

            // Check if this object is a registered machine
            // if (ItemDelegator.TryGetInformation(obj.ParentSheetIndex, out IObjectInformation info) && info.Manager is IMachine manager) {
            //     // Check if the machine's state is being tracked here
            //     if (MachineDelegator._trackedMachines.TryGetValue(new LocationPosition(location, obj.TileLocation), out state)) {
            //         machine = manager;
            //         return true;
            //     }
            // }
            // 
            // machine = default;
            // state = default;
            // return false;
        }

        private class CheckForActionState {
            public bool Ready { get; }
            public SObject Held { get; }
            public int Stack { get; }

            public CheckForActionState(bool ready, SObject held) {
                this.Ready = ready;
                this.Held = held;
                this.Stack = held?.Stack ?? 0;
            }
        }
    }
}
