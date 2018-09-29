using StardewModdingAPI;

namespace PrairieKingMadeEasy
{
    public class ModConfig : Config
    {
        public bool alwaysInvincible { get; set; }
        public bool infiniteCoins { get; set; }
        public bool infiniteLives { get; set; }
        public bool rapidFire { get; set; }

        public override T GenerateDefaultConfig<T>()
        {
            this.alwaysInvincible = false;
            this.infiniteCoins = false;
            this.infiniteLives = false;
            this.rapidFire = false;

            return (this as T);
        }
    }
}
