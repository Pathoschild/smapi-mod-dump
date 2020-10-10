/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/nikperic/energytime
**
*************************************************/

using System;
using StardewModdingAPI;
namespace EnergyTime
{
    internal class ModConfig
    {
        public float EnergyRequirementMultiplier { get; set; } = 2.0F;
        public SButton PassTimeKey { get; set; } = SButton.U;
        public SButton PauseTimeKey { get; set; } = SButton.N;
        public SButton ReloadConfigKey { get; set; } = SButton.B;
        public SButton IncreaseMultiplierKey { get; set; } = SButton.OemPeriod;
        public SButton DecreaseMultiplierKey { get; set; } = SButton.OemComma;
        public SButton TimeModeToggleKey { get; set; } = SButton.OemSemicolon;
    }
}
