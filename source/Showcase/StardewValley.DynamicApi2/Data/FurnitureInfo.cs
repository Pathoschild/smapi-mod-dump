/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/Stardew_Valley_Showcase_Mod
**
*************************************************/

namespace Igorious.StardewValley.DynamicApi2.Data
{
    public class FurnitureInfo
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Kind { get; set; }
        public Size Size { get; set; }
        public Size BoundingBox { get; set; }
        public int Rotations { get; set; }
        public int Price { get; set; }

        public override string ToString()
        {
            return $"{Name}/{Kind}/{Size}/{BoundingBox}/{Rotations}/{Price}";
        }
    }
}