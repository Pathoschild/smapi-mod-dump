using System;
using System.Collections.Generic;
using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>A static class that handles monster IDs and certain custom behavior.</summary>
            public static class MonsterTracker
            {
                /// <summary>The highest allowed value of tracked monster IDs.</summary>
                public const int HighestID = -2;

                /// <summary>The next ID to assign to a new monster.</summary>
                /// <remarks>
                /// The base SDV system doesn't seem to pay attention to Monster IDs, and other NPC types generally use positive integers.
                /// As of this mod's design, SDV always assigns the value -1 to monsters. This mod uses values below that to minimize potential issues.
                /// </remarks>
                private static int nextID = HighestID;

                /// <summary>A dictionary of monster ID keys and sets of saved objects (i.e. "loot" items).</summary>
                private static Dictionary<int, IEnumerable<SavedObject>> MonsterLoot = new Dictionary<int, IEnumerable<SavedObject>>();

                /// <summary>Clears all of this class's internal values.</summary>
                public static void Clear()
                {
                    MonsterLoot = new Dictionary<int, IEnumerable<SavedObject>>();
                    nextID = HighestID;
                }

                /// <summary>Adds a new monster ID to the tracker and returns it.</summary>
                /// <returns>A unique ID assigned to the monster. Null if a new ID could not be added.</returns>
                public static int? AddMonster()
                {
                    if (nextID == int.MinValue) //if there are no more valid IDs left
                    {
                        return null;
                    }

                    MonsterLoot.Add(nextID, null); //add the monster with a blank set of loot

                    return nextID--; //return the ID used, then decrement the static field
                }

                /// <summary>Removes the provided monster ID from the tracker.</summary>
                /// <param name="ID">The unique ID assigned to the monster.</param>
                public static void RemoveMonster(int ID)
                {
                    if (MonsterLoot.ContainsKey(ID)) //if the ID exists
                    {
                        MonsterLoot.Remove(ID);
                    }
                    else //if the ID doesn't exist
                    {
                        Monitor.Log($"MonsterTracker error: Attempted to stop tracking monster with an unrecognized ID: {ID}", LogLevel.Debug);
                    }
                }

                /// <summary>Assigns a custom loot set to a monster.</summary>
                /// <param name="ID">The unique ID assigned to the monster.</param>
                /// <param name="loot">A set of saved objects representing items this monster should drop when defeated.</param>
                public static void SetLoot(int ID, IEnumerable<SavedObject> loot)
                {
                    if (MonsterLoot.ContainsKey(ID)) //if the ID exists
                    {
                        MonsterLoot[ID] = loot;
                    }
                    else //if the ID doesn't exist
                    {
                        Monitor.Log($"MonsterTracker error: Attempted to set loot for a monster with an unrecognized ID: {ID}", LogLevel.Debug);
                    }
                }

                /// <summary>Get the set of loot assigned to a monster.</summary>
                /// <param name="ID">The unique ID assigned to the monster.</param>
                /// <returns>A set of saved objects representing items this monster should drop when defeated.</returns>
                public static IEnumerable<SavedObject> GetLoot(int ID)
                {
                    if (MonsterLoot.ContainsKey(ID)) //if the ID exists
                    {
                        return MonsterLoot[ID];
                    }
                    else //if the ID doesn't exist
                    {
                        Monitor.Log($"MonsterTracker error: Attempted to get loot for a monster with an unrecognized ID: {ID}", LogLevel.Debug);
                        return null;
                    }
                }
            }
        }
    }
}