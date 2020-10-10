/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System.Collections.Generic;
using System.ComponentModel;
using Igorious.StardewValley.DynamicAPI.Attributes;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Newtonsoft.Json;

namespace Igorious.StardewValley.NewMachinesMod.Data
{
    public sealed class OutputItem
    {
        #region	Constructors

        [JsonConstructor]
        public OutputItem() { }

        public OutputItem(DynamicID<ItemID> id, string name = null, MachineDraw draw = null) : this(1, id, name, draw) { }

        public OutputItem(int inputCount, DynamicID<ItemID> id, string name = null, MachineDraw draw = null)
        {
            InputCount = inputCount;
            ID = id;
            Name = name;
            Draw = draw;
        }

        #endregion

        #region	Properties

        [JsonProperty]
        public List<OutputItem> Switch { get; set; }

        [JsonProperty, DefaultValue(1)]
        public int InputCount { get; set; } = 1;

        [JsonProperty]
        public AdditionalItemInfo And { get; set; }

        /// <summary>
        /// ID of output item.
        /// </summary>
        [JsonProperty]
        public DynamicID<ItemID> ID { get; set; }

        [JsonProperty]
        public decimal? Chance { get; set; }

        /// <summary>
        /// Name format of output item. Use {0} to insert default name of output item, {1} to insert name of input item.
        /// </summary>
        [JsonProperty]
        public string Name { get; set; }

        /// <summary>
        /// Specified price of output item. If it's <c>null</c>, default value will be used.
        /// </summary>
        [JsonProperty, Expression(typeof(PriceExpression))] 
        public string Price { get; set; }

        /// <summary>
        /// Specified quality of output item. If it's <c>null</c>, default value will be used.
        /// </summary>
        [JsonProperty, Expression(typeof(QualityExpression))] 
        public string Quality { get; set; }

        /// <summary>
        /// Specified count of output item. If it's <c>null</c>, default value will be used.
        /// </summary>
        [JsonProperty, Expression(typeof(CountExpression))] 
        public string Count { get; set; }

        /// <summary>
        /// Specified time of output item processing. If it's <c>null</c>, default value will be used.
        /// </summary>
        [JsonProperty]
        public int? MinutesUntilReady { get; set; }

        /// <summary>
        /// Color of output item. If it's <c>null</c>, color will not be used. 
        /// If it's <c>"@"</c>, auto-generated value will be used.
        /// </summary>
        [JsonProperty]
        public string Color { get; set; }

        [JsonProperty]
        public MachineDraw Draw { get; set; }

        #endregion
    }
}