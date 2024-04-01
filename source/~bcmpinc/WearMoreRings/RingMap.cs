/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bcmpinc/StardewHack
**
*************************************************/

using StardewValley;
using StardewValley.Objects;
using System;
using System.Runtime.CompilerServices;

namespace StardewHack.WearMoreRings
{
    /// <summary>
    /// CombinedRing Wrapper which allows it to be used as a container that accepts empty slots (= null values).
    /// </summary>
    public class RingMap {
        public const string RING_NAME = "Wear More Rings container ring for {0} (do not touch!)";
        public const string DATA_KEY = "bcmpinc.WearMoreRings/slot-map";
        public static int MAX_RINGS = 20;
        readonly Farmer who;
        readonly int[] slot_map = new int[MAX_RINGS];
        public readonly CombinedRing container;

        public RingMap(Farmer _who) {
            who = _who;
            for (int i=0; i<slot_map.Length; i++) slot_map[i] = -1;
            if (who.leftRing.Value is CombinedRing && who.rightRing.Value == null && Load()) {
                container = who.leftRing.Value as CombinedRing;
                int equipped = container.combinedRings.Count;

                // Reset any indices that are out of bounds
                for (int i=0; i<slot_map.Length; i++) {
                    if (slot_map[i] >= equipped) { 
                        ModEntry.getInstance().Monitor.Log("Slot_map contains invalid index "+i, StardewModdingAPI.LogLevel.Warn);
                        slot_map[i] = -1;
                    }
                }

                // Add any missing indices
                for (int i=0; i<equipped; i++) {
                    if (!Array.Exists(slot_map, val => val == i)) {
                        ModEntry.getInstance().Monitor.Log("Ring "+i+" missing from slot_map", StardewModdingAPI.LogLevel.Warn);
                        var new_pos = Array.FindIndex(slot_map, val => val < 0);
                        if (new_pos >= 0) {
                            slot_map[new_pos] = i;
                        } else {
                            ModEntry.getInstance().Monitor.Log("Failed to insert ring "+i, StardewModdingAPI.LogLevel.Error);
                        }
                    }
                }
            } else {
                // Create a new combined ring for storage.
                container = new CombinedRing();
                this[0] = who.leftRing.Value;
                this[1] = who.rightRing.Value;
                who.leftRing.Value = container;
                who.rightRing.Value = null;
            }
            Save();
        }

        /// <summary>
        /// Drop all rings in slots numbered `capacity` or above.
        /// </summary>
        /// <param name="capacity"></param>
        public void limitSize(int capacity) {
            for (int i=capacity; i<slot_map.Length; i++) {
                if (slot_map[i] >= 0) { 
                    this[i].onUnequip(who);
                    Utility.CollectOrDrop(this[i]);
                    this[i] = null;
                }
            }
        }

        int get_pos(int index) {
            var pos = slot_map[index];
            if (pos < 0)
                return -1;
            if (pos >= container.combinedRings.Count) {
                ModEntry.getInstance().Monitor.Log($"Ring {index} was unequipped outside of WMR", StardewModdingAPI.LogLevel.Error);
                slot_map[index] = -1;
                return -1;
            }
            return pos;
        }

        public Ring this[int index] {
            get {
                var pos = get_pos(index);
                if (pos < 0)
                    return null;
                return container.combinedRings[pos];
            }
            set {
                // Prevent recursion.
                if (value == container) {
                    throw new Exception("Really, don't touch the WMR container ring please!");
                }

                // Equip the ring
                var pos = get_pos(index);
                if (pos < 0) {
                    if (value == null) return; // Nothing changed.
                    slot_map[index] = container.combinedRings.Count;
                    container.combinedRings.Add(value);
                } else {
                    if (value == null) {
                        container.combinedRings.RemoveAt(pos);
                        slot_map[index] = -1;
                        for (int i=0; i<slot_map.Length; i++) {
                            if (slot_map[i] > pos) slot_map[i]--;
                        }
                    } else {
                        container.combinedRings[pos] = value;
                    }
                }
                //container.
                Save();
            }
        }

        public bool AddRing(int slot_hint, Ring r) {
            var new_pos = slot_hint;
            if (slot_map[slot_hint] >= 0) {
                // Hint already occupied, find empty slot.
                new_pos = Array.FindIndex(slot_map, val => val < 0);
            }
            if (new_pos < 0 || new_pos >= ModEntry.getConfig().Rings) return false;
            r.onEquip(who);
            this[new_pos] = r;
            return true;
        }

        public bool Load() {
            if (who.modData.ContainsKey(DATA_KEY)) {
                var data = who.modData[DATA_KEY].Split(",");
                for (int i=0; i<Math.Min(MAX_RINGS, data.Length); i++) {
                    slot_map[i] = int.Parse(data[i]);
                }
                return true;
            } else {
                return false;
            }
        }

        public void Save() {
            // Ensure the combined ring stays in place!
            if (who.leftRing.Value != container) {
                Utility.CollectOrDrop(who.leftRing.Value);
                who.leftRing.Value = container;
                ModEntry.getInstance().Monitor.Log($"The container ring got unequipped. Trying to fix that.", StardewModdingAPI.LogLevel.Error);
            }

            // Mark the combined ring for its purpose.
            // container.UpdateDescription();
            // container.DisplayName = string.Format(RING_NAME, who.displayName);

            // Save ring position data.
            who.modData[DATA_KEY] = string.Join(",", slot_map);
        }
    }
}
