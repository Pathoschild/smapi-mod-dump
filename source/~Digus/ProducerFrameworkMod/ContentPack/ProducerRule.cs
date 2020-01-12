using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;
using Object = StardewValley.Object;

namespace ProducerFrameworkMod.ContentPack
{
    public class ProducerRule
    {
        public string ProducerName;
        public string InputIdentifier;
        public int InputStack = 1;
        public List<string> ExcludeIdentifiers;
        public string FuelIdentifier;
        public int FuelStack = 1;
        public Dictionary<string, int> AdditionalFuel = new Dictionary<string, int>();
        public int MinutesUntilReady;
        public List<OutputConfig> AdditionalOutputs = new List<OutputConfig>();
        public List<string> Sounds = new List<string>();
        public List<Dictionary<string,int>> DelayedSounds = new List<Dictionary<string, int>> ();
        public PlacingAnimation? PlacingAnimation;
        public string PlacingAnimationColorName;
        public float PlacingAnimationOffsetX = 0.0f;
        public float PlacingAnimationOffsetY = 0.0f;
        public List<StardewStats> IncrementStatsOnInput = new List<StardewStats>();

        // Generated attributes
        public List<Tuple<int,int>> FuelList = new List<Tuple<int, int>>();
        internal Color PlacingAnimationColor = Color.White;
        public List<OutputConfig> OutputConfigs = new List<OutputConfig>();

        //Default output
        public string OutputIdentifier;
        public string OutputName;
        public string OutputTranslationKey;
        public Object.PreserveType? PreserveType;
        public bool InputPriceBased;
        public int OutputPriceIncrement = 0;
        public double OutputPriceMultiplier = 1;
        public bool KeepInputQuality;
        public int OutputQuality = 0;
        public int OutputStack = 1;
        public int OutputMaxStack = 1;
        public StackConfig SilverQualityInput = new StackConfig();
        public StackConfig GoldQualityInput = new StackConfig();
        public StackConfig IridiumQualityInput = new StackConfig();
    }
}
