/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bpendragon/GreenhouseSprinklers
**
*************************************************/

namespace Bpendragon.GreenhouseSprinklers.Data
{
    class ModData
    {
        public bool FirstUpgrade { get; set; } = false;
        public bool SecondUpgrade { get; set; } = false;
        public bool FinalUpgrade { get; set; } = false;
        public bool SaveHasBeenUpgraded { get; set; } = false;


        public int GetLevel() => (FirstUpgrade ? 1 : 0) + (SecondUpgrade ? 1 : 0) + (FinalUpgrade ? 1 : 0);
    }
}
