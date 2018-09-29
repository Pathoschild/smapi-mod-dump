using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI.Attributes;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Newtonsoft.Json;

namespace Igorious.StardewValley.NewMachinesMod.Data
{
    public class MachineOutputInformation
    {
        #region	Constructors

        [JsonConstructor]
        public MachineOutputInformation() { }

        public MachineOutputInformation(Dictionary<DynamicID<ItemID, CategoryID>, OutputItem> items)
        {
            Items = items;
        }

        #endregion

        /// <summary>
        /// Default output ID. 
        /// </summary>
        [JsonProperty]
        public DynamicID<ItemID> ID { get; set; }

        /// <summary>
        /// List of inputs and specified outputs.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public Dictionary<DynamicID<ItemID, CategoryID>, OutputItem> Items { get; set; }

        /// <summary>
        /// Default output price.
        /// </summary>
        [JsonProperty, Expression(typeof(PriceExpression))] 
        public string Price { get; set; }

        /// <summary>
        /// Default output quality.
        /// </summary>
        [JsonProperty, Expression(typeof(QualityExpression))] 
        public string Quality { get; set; }

        /// <summary>
        /// Default output count.
        /// </summary>
        [JsonProperty, Expression(typeof(CountExpression))] 
        public string Count { get; set; }

        /// <summary>
        /// Default output name format. Use {0} to insert default name of output item, {1} to insert name of input item.
        /// </summary>
        [JsonProperty]
        public string Name { get; set; }

        /// <summary>
        /// Specified default time of output item processing.
        /// </summary>
        [JsonProperty]
        public int? MinutesUntilReady { get; set; }

        /// <summary>
        /// List of used sounds after drop-in.
        /// </summary>
        [JsonProperty]
        public List<Sound> Sounds { get; set; }

        [JsonProperty]
        public Animation? Animation { get; set; }
    }
}