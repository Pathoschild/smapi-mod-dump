using Microsoft.Xna.Framework;
using NpcAdventure.Model;
using NpcAdventure.Objects;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.Driver
{
    class StuffDriver
    {
        public List<BagDumpInfo> DumpedBags { get; set; }
        public IMonitor Monitor { get; }

        public StuffDriver(IDataHelper dataHelper, IMonitor monitor)
        {
            this.DataHelper = dataHelper;
            this.DumpedBags = new List<BagDumpInfo>();
            this.Monitor = monitor;
        }

        public void RegisterEvents(IModEvents events)
        {
            events.GameLoop.Saving += this.GameLoop_Saving;
            events.GameLoop.SaveLoaded += this.GameLoop_SaveLoaded;
            events.GameLoop.Saved += this.GameLoop_Saved;
        }

        private void GameLoop_Saved(object sender, SavedEventArgs e)
        {
            this.ReviveDeliveredBags();
        }

        public void PrepareDeliveredBagsToSave()
        {
            FarmHouse house = Game1.getLocationFromName("FarmHouse") as FarmHouse;

            this.DumpedBags.Clear();

            foreach (var objKv in house.objects.Pairs)
            {
                if (!(objKv.Value is Package bag))
                    continue;

                BagDumpInfo bagInfo = new BagDumpInfo()
                {
                    source = bag.GivenFrom,
                    giftboxIndex = bag.giftboxIndex.Value,
                    message = bag.Message,
                    posX = (int)objKv.Key.X,
                    posY = (int)objKv.Key.Y,
                };
                Chest chest = new Chest(true)
                {
                    TileLocation = bag.TileLocation
                };
                chest.items.AddRange(bag.items);

                house.objects[objKv.Key] = chest;

                this.DumpedBags.Add(bagInfo);
                this.Monitor.Log($"Found bag to save from {bagInfo.source} at position {bagInfo.posX},{bagInfo.posY} with {chest.items.Count} items");
            }

            this.Monitor.Log($"Detected {this.DumpedBags.Count} bags to save.");
        }

        public IDataHelper DataHelper { get; }

        public void ReviveDeliveredBags()
        {
            int reviveCount = 0;
            FarmHouse house = Game1.getLocationFromName("FarmHouse") as FarmHouse;

            foreach(BagDumpInfo bagInfo in this.DumpedBags)
            {
                Vector2 position = new Vector2(bagInfo.posX, bagInfo.posY);

                if (!house.objects.TryGetValue(position, out StardewValley.Object obj) && !(obj is Chest))
                {
                    this.Monitor.Log($"Bag at position ${position} can't be revived!", LogLevel.Warn);
                    continue;
                }

                Chest chest = obj as Chest;
                Package bag = new Package(chest.items.ToList(), position, 0)
                {
                    GivenFrom = bagInfo.source,
                    Message = bagInfo.message
                };

                house.objects[position] = bag;
                reviveCount++;

                this.Monitor.Log($"Revive dumpedBag on position ${bag.TileLocation} (Items count: {bag.items.Count})");
            }

            this.Monitor.Log($"Revived {reviveCount} dumped bags!", LogLevel.Info);
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            try
            {
                List<BagDumpInfo> dumpedBags = this.DataHelper.ReadSaveData<List<BagDumpInfo>>("dumped-bags");
                this.DumpedBags = dumpedBags ?? new List<BagDumpInfo>();
                this.Monitor.Log($"Count of possible bags: {this.DumpedBags.Count}");
                this.Monitor.Log("Dumped bags loaded from save file", LogLevel.Info);
                this.ReviveDeliveredBags();
            }
            catch (InvalidOperationException ex)
            {
                this.Monitor.Log($"Error while loading dumped bag from savefile: {ex.Message}");
            }
        }

        private void GameLoop_Saving(object sender, SavingEventArgs e)
        {
            try
            {
                this.PrepareDeliveredBagsToSave();
                this.DataHelper.WriteSaveData("dumped-bags", this.DumpedBags ?? new List<BagDumpInfo>());
                this.Monitor.Log("Dumped bags successfully saved to savefile.", LogLevel.Info);
            }
            catch (InvalidOperationException ex)
            {
                this.Monitor.Log($"Error while saving dumped bags: {ex.Message}", LogLevel.Error);
            }
        }
    }
}
