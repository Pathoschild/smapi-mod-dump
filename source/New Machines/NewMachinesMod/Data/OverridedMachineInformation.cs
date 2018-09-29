using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Newtonsoft.Json;

namespace Igorious.StardewValley.NewMachinesMod.Data
{
    public class OverridedMachineInformation
    {
        #region	Constructors

        [JsonConstructor]
        public OverridedMachineInformation() { }

        public OverridedMachineInformation(DynamicID<CraftableID> id, MachineOutputInformation output)
        {
            ID = id;
            Output = output;
        }

        #endregion

        public DynamicID<CraftableID> ID { get; set; }

        public List<DynamicID<ItemID>> IgnoredItems { get; set; }

        public MachineOutputInformation Output { get; set; }

        [JsonProperty]
        public MachineDraw Draw { get; set; }
    }
}