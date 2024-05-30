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
using StardewValley;
using System.Collections.Generic;

namespace FarmTypeManager
{
    /// <summary>FTM's public API, to be provided through SMAPI's mod helper.</summary>
    public class FtmApiWrapper : IFtmApi
    {
        /// <summary>The "real" API instance to use within this wrapper.</summary>
        private IFtmApi internalApi;

        /// <param name="actualApi">The "real" API instance to use within this wrapper.</param>
        public FtmApiWrapper(IFtmApi internalApi)
        {
            this.internalApi = internalApi;
        }

        /// <inheritdoc/>
        public IDictionary<string, IEnumerable<string>> GetForageIDsFromContentPacks(bool includePlacedItems = false, bool includeContainers = false)
        {
            return internalApi.GetForageIDsFromContentPacks(includePlacedItems, includeContainers);
        }
    }

    public partial class ModEntry : Mod
    {
        /// <summary>FTM's API implementation. Due to issues with FTM's class structure, this should be provided within a <see cref="FtmApiWrapper"/>, rather than used directly.</summary>
        public class FtmApi : IFtmApi
        {
            /// <summary>The manifest of the mod using this API instance.</summary>
            private IManifest manifest;

            /// <param name="manifest">The manifest of the mod using this API instance.</param>
            public FtmApi(IManifest manifest)
            {
                this.manifest = manifest;
            }

            /// <inheritdoc/>
            public IDictionary<string, IEnumerable<string>> GetForageIDsFromContentPacks(bool includePlacedItems = false, bool includeContainers = false)
            {
                List<FarmData> data;

                if (Context.IsWorldReady && Utility.FarmDataList?.Count > 0) //if farm data appears to be loaded for an active save
                {
                    data = Utility.FarmDataList; //use the already loaded data (including from the current farm's personal config)
                }
                else //if farm data is NOT already loaded
                {
                    data = Utility.LoadFarmDataForApi(manifest); //load all available content pack data
                }

                var idDict = new Dictionary<string, IEnumerable<string>>(); //create a dictionary of content pack IDs (keys) and forage ID lists (values)

                foreach (var dataEntry in data) //for each entry in the data (e.g. each content pack)
                {
                    if (!dataEntry.Config.ForageSpawnEnabled) //if this pack's forage is NOT enabled
                        continue; //skip it

                    List<SavedObject> parsedObjects = new List<SavedObject>(); //create a list of parsed object types

                    //parse "global" item entries for this entry
                    if (dataEntry.Config.Forage_Spawn_Settings.SpringItemIndex?.Length > 0)
                        parsedObjects.AddRange(Utility.ParseSavedObjectsFromItemList(dataEntry.Config.Forage_Spawn_Settings.SpringItemIndex));
                    if (dataEntry.Config.Forage_Spawn_Settings.SummerItemIndex?.Length > 0)
                        parsedObjects.AddRange(Utility.ParseSavedObjectsFromItemList(dataEntry.Config.Forage_Spawn_Settings.SummerItemIndex));
                    if (dataEntry.Config.Forage_Spawn_Settings.FallItemIndex?.Length > 0)
                        parsedObjects.AddRange(Utility.ParseSavedObjectsFromItemList(dataEntry.Config.Forage_Spawn_Settings.FallItemIndex));
                    if (dataEntry.Config.Forage_Spawn_Settings.WinterItemIndex?.Length > 0)
                        parsedObjects.AddRange(Utility.ParseSavedObjectsFromItemList(dataEntry.Config.Forage_Spawn_Settings.WinterItemIndex));

                    foreach (var area in dataEntry.Config.Forage_Spawn_Settings.Areas) //for each forage area in this entry
                    {
                        //parse any area-specific item entries
                        if (area.SpringItemIndex?.Length > 0)
                            parsedObjects.AddRange(Utility.ParseSavedObjectsFromItemList(area.SpringItemIndex, area.UniqueAreaID));
                        if (area.SummerItemIndex?.Length > 0)
                            parsedObjects.AddRange(Utility.ParseSavedObjectsFromItemList(area.SummerItemIndex, area.UniqueAreaID));
                        if (area.FallItemIndex?.Length > 0)
                            parsedObjects.AddRange(Utility.ParseSavedObjectsFromItemList(area.FallItemIndex, area.UniqueAreaID));
                        if (area.FallItemIndex?.Length > 0)
                            parsedObjects.AddRange(Utility.ParseSavedObjectsFromItemList(area.WinterItemIndex, area.UniqueAreaID));
                    }

                    HashSet<string> forageIDs = new HashSet<string>(); //create a set of unique forage IDs

                    foreach (var parsed in parsedObjects)
                    {
                        if (parsed.ConfigItem == null || parsed.ConfigItem?.Type is SavedObject.ObjectType.Object) //if this is a basic object type
                        {
                            forageIDs.Add("(O)" + parsed.StringID); //qualify it and add it to the set
                        }
                        else if (includePlacedItems && parsed.ConfigItem?.Type is SavedObject.ObjectType.Item or SavedObject.ObjectType.DGA) //if placed items should be included, and this is a placed item (or DGA furniture, which is similarly non-standard)
                        {
                            if (Utility.CreateItem(parsed) is Item item) //try to create an instance of this item (note: inefficient, but less complicated than converting FTM categories into ID qualifiers)
                                forageIDs.Add(item.QualifiedItemId); //add its qualified ID to the list
                        }
                        else if (includeContainers && parsed.ConfigItem?.Type is SavedObject.ObjectType.Container) //if containers should be included, and this is a container
                        {
                            if (Utility.CreateItem(parsed) is Item container) //try to create an instance of this container (note: inefficient, but less complicated than converting FTM categories into ID qualifiers)
                                forageIDs.Add(container.QualifiedItemId); //add its qualified ID to the list
                        }
                    }

                    string entryID = dataEntry.Pack?.Manifest.UniqueID ?? ""; //create an ID for this entry (blank if it's a personal config)

                    idDict.Add(entryID, forageIDs); //add this entry's IDs to the dictionary
                }

                return idDict; //return the completed dictionary
            }
        }

        /**************/
        /* API method */
        /**************/

        /// <summary>Generates an API instance for another SMAPI mod.</summary>
        /// <remarks>See <see cref="IFtmApi"/> for documentation.</remarks>
        /// <returns>A new API instance.</returns>
        public override object GetApi(IModInfo mod) => new FtmApiWrapper(new FtmApi(mod.Manifest));
    }
}
