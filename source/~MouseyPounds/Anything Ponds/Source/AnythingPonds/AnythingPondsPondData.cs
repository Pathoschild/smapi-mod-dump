using System.Collections.Generic;
using StardewValley.GameData.FishPond;

namespace AnythingPonds
{
    class AnythingPondsPondData
    {
        public string Format { get; set; }
        public IList<FishPondData> Entries { get; set; }
        public AnythingPondsPondData()
        {
            this.Format = "1.0";
            this.Entries = new List<FishPondData>();
        }

        public AnythingPondsPondData(string PondType) : this()
        {
            if (PondType.Equals("Generic"))
            {
                FishPondData pond = new FishPondData();
                // The '!' is a "not" condition so this will apply to anything without that tag
                pond.RequiredTags = new List<string> { "!anything_ponds_exclude" };
                pond.SpawnTime = 999999;
                pond.PopulationGates = null;
                // The reward here is a Rotten Plant, but it would only potentially spawn if
                // such a pond was manually filled to size 10.
                FishPondReward pondItem = new FishPondReward();
                pondItem.RequiredPopulation = 10;
                pondItem.Chance = 0.01f;
                pondItem.ItemID = 747;
                pondItem.MinQuantity = 1;
                pondItem.MaxQuantity = 1;
                pond.ProducedItems = new List<FishPondReward>();
                pond.ProducedItems.Add(pondItem);
                Entries.Add(pond);
            }
            else if (PondType.Equals("Algae"))
            {
                // All 3 ponds will have very similar definitions.
                Dictionary<int, string> Items = new Dictionary<int, string> {
                    { AnythingPondsConstants.GreenAlgae, "item_green_algae" },
                    { AnythingPondsConstants.Seaweed, "item_seaweed" },
                    { AnythingPondsConstants.WhiteAlgae, "item_white_algae" }
                };

                foreach (int Id in Items.Keys)
                {
                    FishPondData pond = new FishPondData();
                    pond.RequiredTags = new List<string> { Items[Id] };
                    pond.SpawnTime = 2;
                    // population gates at 4 & 7, requiring regular & quality fertilizer
                    pond.PopulationGates = new Dictionary<int, List<string>>();
                    pond.PopulationGates.Add(4, new List<string> { "368 3" });
                    pond.PopulationGates.Add(7, new List<string> { "369 5" });
                    // production. Mostly just increasing chance/amount of the item itself
                    // with some occasional fiber or wild bait
                    pond.ProducedItems = new List<FishPondReward>();
                    // pop 9, 85% chance of 2-3 item itself, then 33% 5 wild bait, default 3 fiber
                    FishPondReward pondItem = new FishPondReward();
                    pondItem.RequiredPopulation = 9;
                    pondItem.Chance = 0.85f;
                    pondItem.ItemID = Id;
                    pondItem.MinQuantity = 2;
                    pondItem.MaxQuantity = 3;
                    pond.ProducedItems.Add(pondItem);
                    pondItem = new FishPondReward();
                    pondItem.RequiredPopulation = 9;
                    pondItem.Chance = 0.33f;
                    pondItem.ItemID = 774;
                    pondItem.MinQuantity = 5;
                    pondItem.MaxQuantity = 5;
                    pond.ProducedItems.Add(pondItem);
                    pondItem = new FishPondReward();
                    pondItem.RequiredPopulation = 9;
                    pondItem.Chance = 1.0f;
                    pondItem.ItemID = 771;
                    pondItem.MinQuantity = 3;
                    pondItem.MaxQuantity = 3;
                    pond.ProducedItems.Add(pondItem);
                    // pop 6, 75% chance of 2 item, then 80% 2-3 fiber, then fall through
                    pondItem = new FishPondReward();
                    pondItem.RequiredPopulation = 6;
                    pondItem.Chance = 0.75f;
                    pondItem.ItemID = Id;
                    pondItem.MinQuantity = 2;
                    pondItem.MaxQuantity = 2;
                    pond.ProducedItems.Add(pondItem);
                    pondItem = new FishPondReward();
                    pondItem.RequiredPopulation = 6;
                    pondItem.Chance = 0.8f;
                    pondItem.ItemID = 771;
                    pondItem.MinQuantity = 2;
                    pondItem.MaxQuantity = 3;
                    pond.ProducedItems.Add(pondItem);
                    // pop 3, 75% chance of 1-2 item, then 80% 2 fiber, then fall through
                    pondItem = new FishPondReward();
                    pondItem.RequiredPopulation = 3;
                    pondItem.Chance = 0.75f;
                    pondItem.ItemID = Id;
                    pondItem.MinQuantity = 1;
                    pondItem.MaxQuantity = 2;
                    pond.ProducedItems.Add(pondItem);
                    pondItem = new FishPondReward();
                    pondItem.RequiredPopulation = 3;
                    pondItem.Chance = 0.8f;
                    pondItem.ItemID = 771;
                    pondItem.MinQuantity = 2;
                    pondItem.MaxQuantity = 2;
                    pond.ProducedItems.Add(pondItem);
                    // pop 1, 65% chance of 1 item, then default 1 fiber
                    pondItem = new FishPondReward();
                    pondItem.RequiredPopulation = 1;
                    pondItem.Chance = 0.65f;
                    pondItem.ItemID = Id;
                    pondItem.MinQuantity = 1;
                    pondItem.MaxQuantity = 1;
                    pond.ProducedItems.Add(pondItem);
                    pondItem = new FishPondReward();
                    pondItem.RequiredPopulation = 1;
                    pondItem.Chance = 1.0f;
                    pondItem.ItemID = 771;
                    pondItem.MinQuantity = 1;
                    pondItem.MaxQuantity = 1;
                    pond.ProducedItems.Add(pondItem);

                    Entries.Add(pond);
                }
            }
        }
    }
}
