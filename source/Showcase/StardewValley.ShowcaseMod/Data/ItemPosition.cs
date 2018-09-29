using System.Diagnostics;
using Newtonsoft.Json;

namespace Igorious.StardewValley.ShowcaseMod.Data
{
    [DebuggerDisplay("x:{X}, y:{Y}")]
    public sealed class ItemPosition
    {
        public ItemPosition() { }

        public ItemPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        [JsonProperty(Required = Required.Always)]
        public int X { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int Y { get; set; }
    }
}