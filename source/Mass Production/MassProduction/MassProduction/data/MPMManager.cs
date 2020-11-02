/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JacquePott/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace MassProduction
{
    /// <summary>
    /// Keeps track of where machines have been upgraded to mass production versions.
    /// </summary>
    public class MPMManager
    {
        public const string SAVE_KEY = "UpgradedMachineLocations";
        private List<SavedMPMInfo> UpgradedMachineLocations;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MPMManager()
        {
            UpgradedMachineLocations = new List<SavedMPMInfo>();

            try
            {
                ModEntry.Instance.Monitor.Log("Entering MPMManager contructor.", LogLevel.Debug);

                if (!Context.IsMultiplayer || Context.IsMainPlayer)
                {
                    SavedMPMInfo[] SavedData = ModEntry.Instance.Helper.Data.ReadSaveData<SavedMPMInfo[]>(SAVE_KEY);
                    ModEntry.Instance.Monitor.Log("SavedData = " + SavedData, LogLevel.Debug);

                    if (SavedData != null)
                    {
                        foreach (SavedMPMInfo info in SavedData)
                        {
                            GameLocation location = Game1.getLocationFromName(info.LocationName);
                            Vector2 coordinates = info.GetCoordinates();

                            if (location.Objects.ContainsKey(coordinates))
                            {
                                ModEntry.Instance.Monitor.Log($"{location.Objects[coordinates].name} at {info.LocationName} {coordinates} " +
                                    $"has been given mass producer key {info.UpgradeKey}.", LogLevel.Debug);
                                location.Objects[coordinates].SetMassProducerKey(info.UpgradeKey);
                            }
                            else
                            {
                                ModEntry.Instance.Monitor.Log($"Machine unexpectedly missing from location {info.LocationName} {coordinates} when loading; " +
                                    "couldn't get mass production settings for that machine.", LogLevel.Warn);
                            }
                        }
                    }
                }

                ModEntry.Instance.Monitor.Log("Ended try block in MPMManager contructor.", LogLevel.Debug);
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Error while loading:\n{e}", LogLevel.Error);
            }
        }

        /// <summary>
        /// To be called whenever an object's machine upgrade key is changed. Keeps record of the upgrades at every location.
        /// </summary>
        /// <param name="o"></param>
        public void OnMachineUpgradeKeyChanged(SObject o)
        {
            ModEntry.Instance.Monitor.Log($"Entered MPMManager.OnMachineUpgradeKeyChanged. o = {o.name}; mp key = {o.GetMassProducerKey()}", LogLevel.Debug);

            foreach (GameLocation location in Game1.locations)
            {
                if (location.Objects.ContainsKey(o.TileLocation) && location.Objects[o.TileLocation].Equals(o))
                {
                    IEnumerable<SavedMPMInfo> query = from info in UpgradedMachineLocations
                                                      where info.LocationName.Equals(location.Name) && info.GetCoordinates().Equals(o.TileLocation)
                                                      select info;
                    if (query.Count() > 0)
                    {
                        if (string.IsNullOrEmpty(o.GetMassProducerKey()))
                        {
                            ModEntry.Instance.Monitor.Log("Removed entry.", LogLevel.Debug);
                            UpgradedMachineLocations.Remove(query.First());
                        }
                        else
                        {
                            ModEntry.Instance.Monitor.Log("Overwrote old entry.", LogLevel.Debug);
                            query.First().UpgradeKey = o.GetMassProducerKey();
                        }
                    }
                    else if (!string.IsNullOrEmpty(o.GetMassProducerKey()))
                    {
                        ModEntry.Instance.Monitor.Log("Added new entry.", LogLevel.Debug);
                        UpgradedMachineLocations.Add(new SavedMPMInfo()
                        {
                            LocationName = location.Name,
                            CoordinateX = (int)o.TileLocation.X,
                            CoordinateY = (int)o.TileLocation.Y,
                            UpgradeKey = o.GetMassProducerKey()
                        });
                    }

                    break;
                }
            }

            ModEntry.Instance.Monitor.Log("Exited MPMManager.OnMachineUpgradeKeyChanged.", LogLevel.Debug);
        }

        /// <summary>
        /// Removes a machine from tracking by location and coordinates.
        /// </summary>
        /// <param name="locationName"></param>
        /// <param name="coordinates"></param>
        public void Remove(string locationName, Vector2 coordinates)
        {
            IEnumerable<SavedMPMInfo> query = from info in UpgradedMachineLocations
                                              where info.LocationName.Equals(locationName) && info.GetCoordinates().Equals(coordinates)
                                              select info;

            foreach (SavedMPMInfo info in query.ToArray())
            {
                UpgradedMachineLocations.Remove(info);
                ModEntry.Instance.Monitor.Log($"Removed machine in {locationName} ({coordinates}) from tracking.", LogLevel.Debug);
            }
        }

        /// <summary>
        /// Saves the data to file.
        /// </summary>
        public void Save()
        {
            ModEntry.Instance.Helper.Data.WriteSaveData(SAVE_KEY, UpgradedMachineLocations.ToArray());
        }

        /// <summary>
        /// Empties the tracking list.
        /// </summary>
        public void Clear()
        {
            UpgradedMachineLocations.Clear();
        }
    }
}
