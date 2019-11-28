namespace InfiniteBackpack.Framework.Configs
{
    internal class IbConfig
    {
        public int MaxTabsAllowed { get; set; } = 10;
        public int InitialTabCost { get; set; } = 15000;
        public string PrevTabButton { get; set; } = "NumPad4";
        public string NextTabButton { get; set; } = "NumPad3";
    }
}
