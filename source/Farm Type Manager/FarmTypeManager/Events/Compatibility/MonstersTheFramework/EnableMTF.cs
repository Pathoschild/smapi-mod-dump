/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A SMAPI GameLaunched event that enables Monsters The Framework support.</summary>
        public void EnableMTF(object sender, GameLaunchedEventArgs e)
        {
            try
            {
                if (!Helper.ModRegistry.IsLoaded("spacechase0.MonstersTheFramework"))
                {
                    Monitor.Log($"Mod not found: Monsters The Framework (MTF).", LogLevel.Trace);
                    return;
                }

                Monitor.Log($"Mod found: Monsters The Framework (MTF).", LogLevel.Trace);
                Utility.MonstersTheFrameworkAPI = new MTFPseudoAPI(); //add a pseudo-API to this mod's static utility property
            }
            catch (Exception ex)
            {
                Utility.Monitor.Log($"An error happened while loading FTM's Monsters The Framework (MTF) interface. FTM will be unable to spawn custom monsters from that mod. The auto-generated error message has been added to the log.", LogLevel.Warn);
                Utility.Monitor.Log($"----------", LogLevel.Trace);
                Utility.Monitor.Log($"{ex}", LogLevel.Trace);
            }
        }

        /// <summary>A pseudo-API for Monsters The Framework (MTF) that validates and spawns custom monsters.</summary>
        public class MTFPseudoAPI
        {
            /// <summary>A set of known monster IDs from MTF's data asset. Using <see cref="IsKnownMonsterType"/> with an ID will automatically update it within this set.</summary>
            internal HashSet<string> KnownMonsterTypes = new HashSet<string>();

            /// <summary>The type representing MTF's "CustomMonster" class.</summary>
            internal Type CustomMonsterClass = null;

            /// <summary>Loads MTF's data asset and checks whether it contains the given monster ID.</summary>
            /// <param name="id">The unique ID of the monster to check, i.e. a key in the "spacechase0.MonstersTheFramework/Monsters" asset.</param>
            /// <param name="getCachedResult">True if the most recent cached result should be returned. False if the data should be loaded and validated.</param>
            /// <returns>True if the monster ID exists in MTF's data. False otherwise.</returns>
            public bool IsKnownMonsterType(string id, bool getCachedResult)
            {
                if (getCachedResult)
                    return KnownMonsterTypes.Contains(id); //return the most recent cached result (false if never validated)

                ICollection<string> assetKeys; //MTF's current monster IDs
                try
                {
                    assetKeys = Game1.content.Load<IDictionary>("spacechase0.MonstersTheFramework/Monsters")?.Keys as ICollection<string>; //try to load the asset
                    if (assetKeys == null) //if the asset is null for some reason
                        return false; //treat this monster as unknown
                }
                catch (Exception ex)
                {
                    Utility.Monitor.LogOnce($"Failed to read custom monster data for Monsters The Framework (MTF). FTM might be unable to spawn custom monsters from that mod. Auto-generated error message: \n{ex}", LogLevel.Warn);
                    return false; //treat this monster as unknown
                }

                if (assetKeys.Contains(id)) //if the asset has a key matching the given ID (case-sensitive)
                {
                    KnownMonsterTypes.Add(id); //add the ID to the known ID set (if necessary)
                    return true; //this monster is known to MTF
                }
                else //if the asset does NOT have a matching key
                {
                    KnownMonsterTypes.Remove(id); //remove the ID from the known ID set (if necessary)
                    return false; //this monster is unknown to MTF
                }
            }

            /// <summary>Loads MTF's monster data and returns every currently available monster ID.</summary>
            /// <returns>Every currently available monster ID from MTF.</returns>
            public IEnumerable<string> GetAllMonsterTypes()
            {
                IDictionary asset; //MTF's monster data asset
                try
                {
                    asset = Game1.content.Load<IDictionary>("spacechase0.MonstersTheFramework/Monsters"); //try to load the asset (without referencing its value type)
                    if (asset == null) //if the asset is null for some reason
                        return null;
                }
                catch (Exception ex)
                {
                    Utility.Monitor.Log($"Failed to read custom monster data for Monsters The Framework (MTF). Auto-generated error message: \n{ex}", LogLevel.Warn);
                    return null;
                }

                return asset.Keys as ICollection<string>; //return the asset's keys, i.e. every available MTF monster ID
            }

            /// <summary>Create an instance of a custom monster from MTF.</summary>
            /// <param name="id">The unique ID of the monster to create, i.e. a key in the "spacechase0.MonstersTheFramework/Monsters" asset.</param>
            /// <returns>The created monster.</returns>
            public Monster CreateMonster(string id)
            {
                if (CustomMonsterClass == null)
                    CustomMonsterClass = Utility.GetTypeFromName("MonstersTheFramework.CustomMonster");
                return (Monster)Activator.CreateInstance(CustomMonsterClass, id); //create a MTF custom monster with the "string key" constructor
            }
        }
    }
}