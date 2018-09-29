using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewValley;

namespace RealisticFishing
{
    public class FishModel : IComparable<FishModel>
    {
        public int uniqueID;
        public string name;
        public int minLength;
        public int maxLength;
        public double length;

        public int quality;

        [JsonConstructor]
        public FishModel(int id, string name, int minLength, int maxLength, double length, int quality)
        {
            this.uniqueID = id;
            this.name = name;
            this.minLength = minLength;
            this.maxLength = maxLength;
            this.length = length;
            this.quality = quality;
        }

        public int CompareTo(FishModel o)
        {
            return this.length.CompareTo(o.length);
        }

        public FishModel MakeBaby() 
        {
            return new FishModel(this.uniqueID, this.name, this.minLength, this.maxLength, EvolutionHelpers.GetMutatedFishLength(this.length, this.minLength, this.maxLength), this.quality);
        }

        public override string ToString() 
        {
            return this.name + " " + Math.Round(this.length);
        }
    }
}
