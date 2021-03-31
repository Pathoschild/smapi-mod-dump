/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Let-Me-Rest
**
*************************************************/

namespace LetMeRest
{
    public class ModConfig
    {
        public float Multiplier { get; set; } = 1;
        public bool SittingVerification { get; set; } = true;
        public bool RidingVerification { get; set; } = true;
        public bool StandingVerification { get; set; } = true;
        public bool EnableSecrets { get; set; } = true;
        public bool EnableBuffs { get; set; } = true;
    }
}
