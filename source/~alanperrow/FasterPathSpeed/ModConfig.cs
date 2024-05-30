/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alanperrow/StardewModding
**
*************************************************/

namespace FasterPathSpeed
{
    public class ModConfig
    {
        public float DefaultPathSpeedBuff { get; set; } = 1f;   // Original: 0.1f

        public bool IsPathSpeedBuffOnlyOnTheFarm { get; set; } = false;

        public bool IsPathAffectHorseSpeed { get; set; } = true;

        public float HorsePathSpeedBuffModifier { get; set; } = 1f;

        public bool IsEnablePathReplace { get; set; } = true;

        public bool IsTownPathSpeedBuff { get; set; } = true;

        public bool IsUseCustomPathSpeedBuffValues { get; set; } = false;

        public CustomPathSpeedBuffValues CustomPathSpeedBuffValues { get; set; } = new();
    }
}
