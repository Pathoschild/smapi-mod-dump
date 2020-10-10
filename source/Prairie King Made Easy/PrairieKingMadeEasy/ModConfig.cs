/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mucchan/sv-mod-prairie-king
**
*************************************************/

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
