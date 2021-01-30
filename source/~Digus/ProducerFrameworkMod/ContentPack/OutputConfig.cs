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
        public double OutputProbability = 0;
        public string OutputIdentifier;
        public int? MinutesUntilReady;
        public string OutputName;
        public string OutputTranslationKey;
        public string OutputGenericParentName;
        public string OutputGenericParentNameTranslationKey;
        public Object.PreserveType? PreserveType;
        public bool KeepInputParentIndex;
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
        public ColoredObjectConfig OutputColorConfig;
        public Dictionary<string, int> RequiredFuel = new Dictionary<string, int>();
        public List<int> RequiredInputQuality = new List<int>();
        public List<string> RequiredSeason = new List<string>();
        public List<Weather> RequiredWeather = new List<Weather>();
        public List<string> RequiredLocation = new List<string>();
        public List<string> RequiredMail = new List<string>();
        public List<int> RequiredEvent = new List<int>();
        public bool? RequiredOutdoors = null;
        public List<string> RequiredInputParentIdentifier = new List<string>();

        //Generated Properties
        public int OutputIndex = -1;
        public List<Tuple<int, int>> FuelList = new List<Tuple<int, int>>();
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
