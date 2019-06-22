namespace MegaStorage.Models
{
    public class DeserializedChest
    {
        public string Name { get; set; }
        public long PlayerId { get; set; }
        public string LocationName { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public int InventoryIndex { get; set; }
        public ChestType ChestType { get; set; }
    }
}
