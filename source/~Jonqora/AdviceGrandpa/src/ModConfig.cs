using StardewModdingAPI;

namespace AdviceGrandpa
{
    class ModConfig
    {
        public SButton debugKey { get; set; }

        public ModConfig()
        {
            debugKey = SButton.J;
        }
    }
}
