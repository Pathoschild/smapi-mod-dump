using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace ProducerFrameworkMod.ContentPack
{
    public class OutputConfig
    {
        public double OutputProbability = 0;
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
        public int OutputIndex = -1;
    }

    public class StackConfig
    {
        public double Probability = 0;
        public int OutputStack;
        public int OutputMaxStack;

        public StackConfig()
        {
            OutputStack = 1;
            OutputMaxStack = 1;
        }

        public StackConfig(int outputStack, int outputMaxStack)
        {
            OutputStack = outputStack;
            OutputMaxStack = outputMaxStack;
        }
    }
}
