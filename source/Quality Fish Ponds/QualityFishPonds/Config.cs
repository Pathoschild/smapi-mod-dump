/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/YTSC/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace QualityFishPonds
{
    class Config
    {
        public bool EnableGaranteedIridum { get; set; }       

        public Config()
        {
            EnableGaranteedIridum = true;          
        }
    }
}
