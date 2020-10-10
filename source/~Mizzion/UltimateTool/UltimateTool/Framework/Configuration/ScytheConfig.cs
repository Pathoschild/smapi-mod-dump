/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

namespace UltimateTool.Framework.Configuration
{
   internal class ScytheConfig
    {
        public bool HarvestForage { get; set; } = true;
        public bool HarvestCrops { get; set; } = true;
        public bool HarvestFlowers { get; set; } = false;
        public bool HarvestFruit { get; set; } = true;
        public bool HarvestGrass { get; set; } = true;
        public bool CutDeadCrops { get; set; } = true;
        public bool CutWeeds { get; set; } = true;
    }
}
