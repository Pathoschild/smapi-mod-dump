using System.ComponentModel;
using TehPers.CoreMod.Api.Json;

namespace TehPers.CoreMod.Config {
    [JsonDescribe]
    internal class ModConfig {

        [Description("When crafting items, allows multiple recipes to be crafted at once, storing all the results under the cursor.")]
        public bool AllowMultiCrafting { get; set; } = false;
    }
}