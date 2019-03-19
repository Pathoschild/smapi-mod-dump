using Microsoft.Xna.Framework;

namespace FarmHelper.Framework
{
    internal class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public string ActivationKey { get; set; } = "NumPad7";
        public bool AutomaticMode { get; set; } = true;
        public bool EnablePetting { get; set; } = true;
        public bool EnableNotification { get; set; } = true;
        public bool EnableCost { get; set; } = true;
        public int HelperCost { get; set; } = 50;
        public bool AddItemsToInventory { get; set; } = true;
        public bool HarvestAnimals { get; set; } = true;
        public bool HarvestTruffles { get; set; } = true;
        public Vector2 ChestLocation { get; set; } = new Vector2(0, 0);
    }
}
