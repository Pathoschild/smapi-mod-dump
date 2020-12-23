/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-ItemBags
**
*************************************************/

using ItemBags.Bags;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemBags.Community_Center
{
    public class CommunityCenterBundles
    {
        public static CommunityCenterBundles Instance { get; set; }

        public CommunityCenter Building { get; }
        public bool IsJojaMember { get; }

        /// <summary>Warning - The Vault room has been omitted since its Tasks require Gold instead of Items.</summary>
        public ReadOnlyCollection<BundleRoom> Rooms { get; }

        /// <summary>Key = Size of BundleBag. Value = Dictionary where Key = Item Id, Value = All distinct minimum qualities for the item.<para/>
        /// For example, Parsnip Quality = 0 is required by Spring Crops Bundle, while Parsnip Quality = 2 is required by Quality Crops Bundle. 
        /// So the Value at Parsnip's Id would be a set containing <see cref="ObjectQuality.Regular"/> and <see cref="ObjectQuality.Gold"/></summary>
        public Dictionary<ContainerSize, Dictionary<int, HashSet<ObjectQuality>>> IncompleteBundleItemIds { get; }

        public CommunityCenterBundles()
        {
            try
            {
                //Possible TODO: Load the current language's Bundle .xnb file (Does SMAPI Automatically do this for us when loading game content?)
                //Refer to: LocalizedContentManager.CurrentLanguageCode and use that code to build the string of the content filename, such as Data\Bundles.pt-BR if language code is portuguese

                this.Building = Game1.getLocationFromName("CommunityCenter") as CommunityCenter;
                this.IsJojaMember = Game1.MasterPlayer.mailReceived.Contains("JojaMember"); // Possible TODO Do names of received mail depend on current language?

                Dictionary<string, string> RawBundleData = Game1.netWorldState.Value.BundleData; //Game1.content.Load<Dictionary<string, string>>(@"Data\Bundles");
                Dictionary<string, List<Tuple<int, string>>> GroupedByRoomName = new Dictionary<string, List<Tuple<int, string>>>();
                foreach (KeyValuePair<string, string> KVP in RawBundleData)
                {
                    string RoomName = KVP.Key.Split('/').First();
                    int TaskIndex = int.Parse(KVP.Key.Split('/').Last());

                    List<Tuple<int, string>> Tasks;
                    if (!GroupedByRoomName.TryGetValue(RoomName, out Tasks))
                    {
                        Tasks = new List<Tuple<int, string>>();
                        GroupedByRoomName.Add(RoomName, Tasks);
                    }
                    Tasks.Add(Tuple.Create(TaskIndex, KVP.Value));
                }
                GroupedByRoomName.Remove("Vault");
                this.Rooms = GroupedByRoomName.Select(x => new BundleRoom(this, x.Key, x.Value)).ToList().AsReadOnly();

                Dictionary<int, BundleTask> IndexedTasks = Rooms.SelectMany(x => x.Tasks).ToDictionary(x => x.BundleIndex);

                //  Fill in data for which items of which tasks have been completed
                foreach (var KVP in Building.bundlesDict()) // maybe use Game1.netWorldState.Value.Bundles instead?
                {
                    if (IndexedTasks.TryGetValue(KVP.Key, out BundleTask Task))
                    {
                        try
                        {
                            for (int i = 0; i < Task.Items.Count; i++)
                            {
                                Task.Items[i].IsCompleted = KVP.Value[i];
                            }
                        }
                        catch (Exception exception)
                        {
                            ItemBagsMod.ModInstance.Monitor.Log(string.Format("Error while initializing completed items for bundle index = {0}: {1}", KVP.Key, exception.Message), LogLevel.Error);
                        }
                    }
                }

                //  Fill in data for which tasks have been completed
                foreach (BundleTask Task in IndexedTasks.Values)
                {
                    if (Building.isBundleComplete(Task.BundleIndex) || IsJojaMember)
                    {
                        Task.Items.ToList().ForEach(x => x.IsCompleted = true);
                    }
                }

                //  Index the required bundle items by their Id and accepted Qualities
                this.IncompleteBundleItemIds = new Dictionary<ContainerSize, Dictionary<int, HashSet<ObjectQuality>>>();
                foreach (ContainerSize Size in BundleBag.ValidSizes)
                    IncompleteBundleItemIds.Add(Size, new Dictionary<int, HashSet<ObjectQuality>>());
                IterateAllBundleItems(Item =>
                {
                    if (!Item.IsCompleted)
                    {
                        int Id = Item.Id;
                        ObjectQuality Quality = Item.MinQuality;
                        string RoomName = Item.Task.Room.Name;

                        foreach (ContainerSize Size in BundleBag.ValidSizes)
                        {
                            if (!BundleBag.InvalidRooms[Size].Contains(RoomName))
                            {
                                Dictionary<int, HashSet<ObjectQuality>> IndexedItems = IncompleteBundleItemIds[Size];
                                if (!IndexedItems.TryGetValue(Id, out HashSet<ObjectQuality> Qualities))
                                {
                                    Qualities = new HashSet<ObjectQuality>();
                                    IndexedItems.Add(Id, Qualities);
                                }
                                Qualities.Add(Quality);
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                this.IsJojaMember = false;
                this.Rooms = new List<BundleRoom>().AsReadOnly();
                this.IncompleteBundleItemIds = new Dictionary<ContainerSize, Dictionary<int, HashSet<ObjectQuality>>>();

                ItemBagsMod.ModInstance.Monitor.Log(string.Format("Error while instantiating CommunityCenterBundles: {0}", ex.Message), LogLevel.Error);
                ItemBagsMod.ModInstance.Monitor.Log(string.Format("Error while instantiating CommunityCenterBundles: {0}", ex.ToString()), LogLevel.Error);
            }
        }

        /// <summary>Invokes the given Action on all <see cref="BundleItem"/> within every <see cref="BundleTask"/> of every <see cref="BundleRoom"/></summary>
        public void IterateAllBundleItems(Action<BundleItem> Action)
        {
            foreach (BundleRoom Room in Rooms)
            {
                foreach (BundleTask Task in Room.Tasks)
                {
                    foreach (BundleItem Item in Task.Items)
                    {
                        Action(Item);
                    }
                }
            }
        }
    }
}
