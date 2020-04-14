namespace SeedShortage
{
    public class ModConfig
    {
        public string[] VendorsWithoutSeeds { get; set; } =
        {
            "Pierre",
            "Sandy",
        };
        public string[] VendorsPrice { get; set; } =
        {
            "Joja",
            "Magic Boat"
        };
        public string PriceIncrease { get; set; } = "10x";
        public string[] CropExceptions { get; set; } =
        {
            "Parsnip Seeds",
            "Pink Cat Seeds"
        };
    }
}
