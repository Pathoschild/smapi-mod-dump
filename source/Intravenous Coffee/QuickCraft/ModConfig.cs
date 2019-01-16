using Newtonsoft.Json;
using Pathoschild.Stardew.Common;
using StardewModdingAPI;

namespace mpcomplete.Stardew.QuickCraft.Framework
{
    internal class ModConfig
    {
        public ModConfigControls Controls { get; set; } = new ModConfigControls();

        internal class ModConfigControls
        {
            [JsonConverter(typeof(StringEnumArrayConverter))]
            public SButton[] HoldToActivate { get; set; } = { SButton.LeftControl };

            [JsonConverter(typeof(StringEnumArrayConverter))]
            public SButton[] HoldFor5x { get; set; } = { SButton.LeftShift };
        }
    }
}
