namespace MegaStorage.Framework.Models
{
    public class ModConfig
    {
        public bool EnableCategories { get; set; }
        public CustomChestConfig LargeChest { get; set; }
        public CustomChestConfig MagicChest { get; set; }
        public CustomChestConfig SuperMagicChest { get; set; }
        public ModConfig()
        {
            Instance = this;

            EnableCategories = true;

            LargeChest = new CustomChestConfig
            {
                SpritePath = "LargeChest.png",
                SpriteBWPath = "LargeChestBW.png",
                SpriteBracesPath = "LargeChestBraces.png"
            };

            MagicChest = new CustomChestConfig
            {
                SpritePath = "MagicChest.png",
                SpriteBWPath = "MagicChestBW.png",
                SpriteBracesPath = "MagicChestBraces.png"
            };

            SuperMagicChest = new CustomChestConfig
            {
                SpritePath = "SuperMagicChest.png",
                SpriteBWPath = "SuperMagicChestBW.png",
                SpriteBracesPath = "SuperMagicChestBraces.png"
            };
        }
        public static ModConfig Instance { get; private set; }
    }
}