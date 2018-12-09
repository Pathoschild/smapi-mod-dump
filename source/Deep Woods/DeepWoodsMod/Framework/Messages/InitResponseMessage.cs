namespace DeepWoodsMod.Framework.Messages
{
    internal class InitResponseMessage
    {
        public DeepWoodsSettings Settings { get; set; }
        public DeepWoodsStateData State { get; set; }
        public string[] LevelNames { get; set; }
    }
}
