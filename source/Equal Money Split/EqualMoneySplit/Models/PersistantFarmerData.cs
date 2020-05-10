namespace EqualMoneySplit.Models
{
    /// <summary>
    /// Contains information on the local Farmer's past member values
    /// </summary>
    public static class PersistantFarmerData
    {
        /// <summary>
        /// Money the local Farmer had in the previos game tick
        /// </summary>
        public static int PocketMoney { get; set; }

        /// <summary>
        /// Money the local Farmer had when the day began ending
        /// </summary>
        public static int ShippingBinMoney { get; set; }

        /// <summary>
        /// Money the local Farmer will send to other connected Farmer's
        /// </summary>
        public static int ShareToSend { get; set; }
    }
}
