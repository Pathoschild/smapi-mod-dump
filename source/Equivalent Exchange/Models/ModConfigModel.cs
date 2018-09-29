using Microsoft.Xna.Framework.Input;

namespace EquivalentExchange.Models
{
    class ConfigurationModel
    {
        public string TransmuteKey { get; set; }
        public string NormalizeKey { get; set; }
        public bool DisableRecipeItems { get; set; }

        public ConfigurationModel()
        {
            TransmuteKey = Keys.V.ToString();
            NormalizeKey = Keys.N.ToString();
            DisableRecipeItems = false;
        }
    }
}