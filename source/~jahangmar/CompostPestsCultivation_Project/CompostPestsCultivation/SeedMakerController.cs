// Copyright (c) 2019 Jahangmar
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace CompostPestsCultivation
{
    public class SeedMakerEventArgs : System.EventArgs
    {
        public SeedMakerEventArgs(Object seedMaker, Object heldItem)
        {
            SeedMaker = seedMaker;
            HeldItem = heldItem;
        }

        public readonly Object SeedMaker;
        public readonly Object HeldItem;
    }

    public static class SeedMakerController
    {
        private static IModHelper Helper;
        private static Dictionary<Object, Object> SeedMakers;
        public static System.EventHandler<SeedMakerEventArgs> HeldItemAdded, HeldItemRemoved;

        public static void Init(IModHelper helper)
        {
            Helper = helper;
            SeedMakers = new Dictionary<Object, Object>();

            Helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            Helper.Events.World.ObjectListChanged += World_ObjectListChanged;
            Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
        }

        private static void FindSeedMakers()
        {
            SeedMakers.Clear();
            foreach (GameLocation location in Game1.locations)
            {
                foreach (Object obj in location.Objects.Values)
                {
                    if (obj.Name.Equals("Seed Maker"))
                    {
                        SeedMakers.Add(obj, obj.heldObject?.Value);
                    }

                }
            }
        }

        private static void GameLoop_DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            FindSeedMakers();
        }

        private static void World_ObjectListChanged(object sender, StardewModdingAPI.Events.ObjectListChangedEventArgs e)
        {
            FindSeedMakers();
        }

        private static void GameLoop_UpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            foreach (KeyValuePair<Object, Object> pair in SeedMakers)
            {
                Object seedMaker = pair.Key;
                Object heldItem = pair.Value;
                bool wasHolding = heldItem != null;
                if (HasHeldItem(seedMaker) && (!wasHolding || heldItem != GetHeldItem(seedMaker)))
                {
                    if (HeldItemAdded != null)
                        HeldItemAdded.Invoke(null, new SeedMakerEventArgs(seedMaker, GetHeldItem(seedMaker)));
                }
                if (wasHolding && (!HasHeldItem(seedMaker) || GetHeldItem(seedMaker) != heldItem))
                {
                    if (HeldItemRemoved != null)
                        HeldItemRemoved.Invoke(null, new SeedMakerEventArgs(seedMaker, heldItem));
                }
            }

            foreach (Object seedMaker in new List<Object>(SeedMakers.Keys))
            {
                SeedMakers[seedMaker] = GetHeldItem(seedMaker);
            }
        }

        private static Object GetHeldItem(Object seedMaker) => seedMaker.heldObject?.Value;
        private static bool HasHeldItem(Object seedMaker) => GetHeldItem(seedMaker) != null;
    }
}
