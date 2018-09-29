namespace Igorious.StardewValley.DynamicApi2.Data
{
    public sealed class ShopItemInfo
    {
        public ShopItemInfo() { }

        public ShopItemInfo(int id)
        {
            ID = id;
        }

        public int ID { get; set; }
    }
}