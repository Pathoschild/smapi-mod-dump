/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

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
        public string ModUniqueID;
        public double OutputProbability = 0;
        public string OutputIdentifier;
        public int? MinutesUntilReady;
        public string OutputName;
        public string OutputTranslationKey;
        public string OutputGenericParentName;
        public string OutputGenericParentNameTranslationKey;
        public Object.PreserveType? PreserveType;
        public bool KeepInputParentIndex;
        public bool ReplaceWithInputParentIndex;
        public bool InputPriceBased;
        public int OutputPriceIncrement = 0;
        public double OutputPriceMultiplier = 1;
        public bool KeepInputQuality;
        public int OutputQuality = 0;
        public int OutputStack = 1;
        public int OutputMaxStack = 1;
        public StackConfig SilverQualityInput = new();
        public StackConfig GoldQualityInput = new();
        public StackConfig IridiumQualityInput = new();
        public ColoredObjectConfig OutputColorConfig;
        public Dictionary<string, int> RequiredFuel = new();
        public int? RequiredInputStack;
        public List<int> RequiredInputQuality = new();
        public List<string> RequiredSeason = new();
        public List<Weather> RequiredWeather = new();
        public List<string> RequiredLocation = new();
        public List<string> RequiredMail = new();
        public List<string> RequiredEvent = new();
        public bool? RequiredOutdoors = null;
        public List<string> RequiredInputParentIdentifier = new();

        //Generated Properties
        public string OutputItemId;
        public List<Tuple<string, int>> FuelList = new();

        public OutputConfig Clone()
        {
            return (OutputConfig) this.MemberwiseClone();
        }
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
