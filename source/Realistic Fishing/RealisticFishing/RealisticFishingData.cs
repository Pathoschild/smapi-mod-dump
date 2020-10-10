/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kevinforrestconnors/RealisticFishing
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using RealiticFishing;
using StardewValley;

namespace RealisticFishing
{
    public class RealisticFishingData
    {
        public Dictionary<string, bool> endangeredFish { get; set; }

        public List<Tuple<int, List<FishModel>>> inventory { get; set; }
        public Dictionary<String, Dictionary<Vector2, List<Tuple<int, List<FishModel>>>>> chests;

        public int CurrentFishIDCounter;
        public int NumFishCaughtToday { get; set; }
        public List<Tuple<string, int>> AllFishCaughtToday { get; set; }
        public FishPopulation fp { get; set; }
        public Dictionary<String, List<FishModel>> population { get; set; }

        public RealisticFishingData()
        {
            this.endangeredFish = new Dictionary<string, bool>();
            this.inventory = new List<Tuple<int, List<FishModel>>>();
            this.chests = new Dictionary<String, Dictionary<Vector2, List<Tuple<int, List<FishModel>>>>>();
            this.NumFishCaughtToday = 0;
            this.AllFishCaughtToday = new List<Tuple<string, int>>();
            this.fp = new FishPopulation();
            this.population = this.fp.population;
            this.CurrentFishIDCounter = this.fp.CurrentFishIDCounter;
        }
    }
}
