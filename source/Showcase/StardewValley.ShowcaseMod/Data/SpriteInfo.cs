using System.ComponentModel;
using System.Diagnostics;
using Igorious.StardewValley.ShowcaseMod.Constants;
using Newtonsoft.Json;

namespace Igorious.StardewValley.ShowcaseMod.Data
{
    [DebuggerDisplay("[{Kind}] {Index}")]
    public sealed class SpriteInfo
    {
        public SpriteInfo() { }

        public SpriteInfo(int index, TextureKind kind = TextureKind.Local)
        {
            Kind = kind;
            Index = index;
        }

        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Include)]
        public int Index { get; set; }

        [DefaultValue(TextureKind.Local)]
        public TextureKind Kind { get; set; } = TextureKind.Local;
    }
}