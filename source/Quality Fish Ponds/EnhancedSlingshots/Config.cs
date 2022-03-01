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

namespace EnhancedSlingshots
{
    class Config
    {
        public bool EnableGalaxySligshot { get; set; }
        public int GalaxySlingshotPrice { get; set; }
        public int InfinitySlingshotId { get; set; }       

        public Config()
        {
            EnableGalaxySligshot = true;
            GalaxySlingshotPrice = 75000;
            InfinitySlingshotId = 135;
        }
    }
}
