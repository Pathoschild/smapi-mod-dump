/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/Stardew_Valley_Showcase_Mod
**
*************************************************/

using System.ComponentModel;
using System.Diagnostics;
using Igorious.StardewValley.DynamicApi2.Constants;
using Igorious.StardewValley.DynamicApi2.Data;
using Igorious.StardewValley.ShowcaseMod.Constants;
using Igorious.StardewValley.ShowcaseMod.Data;
using Newtonsoft.Json;

namespace Igorious.StardewValley.ShowcaseMod.ModConfig
{
    [DebuggerDisplay("ID:{ID}, Name=\"{Name}\"")]
    public class ShowcaseConfig
    {
        [JsonProperty(Required = Required.Always)]
        public int ID { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }

        public string Description { get; set; }

        [JsonProperty(Required = Required.Always)]
        public SpriteInfo Sprite { get; set; }

        public SpriteInfo SecondSprite { get; set; }

        public SpriteInfo Tint { get; set; }

        public SpriteInfo SecondTint { get; set; }

        public bool AutoTint { get; set; }

        public Size Size { get; set; } = Size.Default;
        public bool ShouldSerializeSize() => Size != Size.Default;

        public Size BoundingBox { get; set; } = Size.Default;
        public bool ShouldSerializeBoundingBox() => BoundingBox != Size.Default;

        public int Price { get; set; }

        [JsonProperty(Required = Required.Always)]
        public FurnitureKind Kind { get; set; }

        public string Filter { get; set; }

        [DefaultValue(1)]
        public int Rotations { get; set; } = 1;

        [JsonProperty(Required = Required.Always)]
        public LayoutConfig Layout { get; set; } = new LayoutConfig { Type = ShowcaseLayoutKind.Fixed };
    }
}