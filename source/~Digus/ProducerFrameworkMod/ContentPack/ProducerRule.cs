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
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;
using ProducerFrameworkMod.Controllers;
using StardewModdingAPI;
using Object = StardewValley.Object;

namespace ProducerFrameworkMod.ContentPack
{
    public class ProducerRule : ProducerData
    {
        public string InputIdentifier;
        public int InputStack = 1;
        public List<string> ExcludeIdentifiers;
        public string FuelIdentifier;
        public int FuelStack = 1;
        public Dictionary<string, int> AdditionalFuel = new();
        public int MinutesUntilReady;
        public bool SubtractTimeOfDay;
        public List<OutputConfig> AdditionalOutputs = new();
        public List<string> Sounds = new();
        public List<Dictionary<string,int>> DelayedSounds = new();
        public PlacingAnimation? PlacingAnimation;
        public string PlacingAnimationColorName;
        public float PlacingAnimationOffsetX = 0.0f;
        public float PlacingAnimationOffsetY = 0.0f;
        public List<StardewStats> IncrementStatsOnInput = new();
        public List<string> IncrementStatsLabelOnInput = new();
        public InputSearchConfig LookForInputWhenReady;

        // Generated attributes
        public object InputKey;
        public List<Tuple<string,int>> FuelList = new();
        internal Color PlacingAnimationColor = Color.White;
        public List<OutputConfig> OutputConfigs = new();

        //Default output
        public string OutputIdentifier;
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

        private LogLevel? _warningsLogLevel;
        public LogLevel WarningsLogLevel
        {
            get => _warningsLogLevel.GetValueOrDefault(ModUniqueID == null ? LogLevel.Warn : ContentPackConfigController.GetDefaultWarningLogLevel(ModUniqueID));
            set => _warningsLogLevel = value;
        }

        public string ProducerIdentification => this.ProducerName ?? this.ProducerQualifiedItemId;
    }
}
