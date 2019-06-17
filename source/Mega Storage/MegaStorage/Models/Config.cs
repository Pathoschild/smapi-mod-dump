namespace MegaStorage.Models
{
    public class Config
    {
        public static Config Instance { get; private set; }

        public string LargeChestRecipe { get; set; }
        public string MagicChestRecipe { get; set; }

        public Config()
        {
            Instance = this;
        }
    }
}
