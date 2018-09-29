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