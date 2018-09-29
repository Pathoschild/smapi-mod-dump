using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;

namespace AutoGrabberMod.Models
{
    public class NearbyChest
    {
        public enum NextType { Seed, Fertilizer };
        public Vector2 Tile { get; set; }
        public Chest Chest { get; set; }

        public IEnumerable<StardewValley.Object> Seeds
        {
            get
            {
                foreach (StardewValley.Object item in Chest.items.OfType<StardewValley.Object>())
                {
                    if (item != null && item.Type != null && item.Type.ToLower().Contains("seed"))
                    {
                        yield return item;
                    }
                }
            }
        }

        public IEnumerable<StardewValley.Object> Fertilizers
        {
            get
            {
                foreach (StardewValley.Object item in Chest.items.OfType<StardewValley.Object>())
                {
                    if (item != null && Utilities.IsFertilizer(item))
                    {
                        yield return item;
                    }
                }
            }
        }

        public IEnumerable<StardewValley.Object> Sprinklers
        {
            get
            {
                foreach (StardewValley.Object item in Chest.items.OfType<StardewValley.Object>())
                {
                    if (item != null && (item.ParentSheetIndex == 599 || item.ParentSheetIndex == 621 || item.ParentSheetIndex == 645))
                    {
                        yield return item;
                    }
                }
            }
        }

        public int TotalSeeds => Count(Seeds);

        public int TotalFertilizers => Count(Fertilizers);

        public int SprinklerCapacity { get => Sprinklers.Sum((item) => Utilities.GetSprinklerCapacity(item)); }

        public NearbyChest(Vector2 tile, Chest chest)
        {
            Tile = tile;
            Chest = chest;
        }

        private int Count(IEnumerable<StardewValley.Object> list)
        {
            return list.Select(s => s.Stack == 0 ? 1 : s.Stack).Sum();
        }
    }
}
