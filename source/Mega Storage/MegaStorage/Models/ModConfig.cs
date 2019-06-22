namespace MegaStorage.Models
{
    public class ModConfig
    {
        public CustomChestConfig LargeChest { get; set; }
        public CustomChestConfig MagicChest { get; set; }

        public ModConfig()
        {
            Instance = this;
        }

        public static ModConfig Instance { get; private set; }
    }
}
