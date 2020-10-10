/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/Stardew_Valley_Showcase_Mod
**
*************************************************/

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