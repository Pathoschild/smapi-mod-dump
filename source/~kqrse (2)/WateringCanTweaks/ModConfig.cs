/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/


using StardewModdingAPI;

namespace WateringCanTweaks
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public float ChargedStaminaMult { get; set; } = 1;
        public float VolumeMult { get; set; } = 1;
        public float WaterMult { get; set; } = 1;
        public bool FillAdjacent { get; set; } = true;
    }
}
