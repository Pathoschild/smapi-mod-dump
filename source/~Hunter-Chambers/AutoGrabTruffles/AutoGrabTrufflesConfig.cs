/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hunter-Chambers/StardewValleyMods
**
*************************************************/

namespace AutoGrabTruffles
{
    public sealed class AutoGrabTrufflesConfig
    {
        public bool ApplyGathererProfession { get; set; } = true;
        public string WhoseGathererProfessionToUse { get; set; } = "Owner";
        public bool ApplyBotanistProfession { get; set; } = true;
        public string WhoseBotanistProfessionToUse { get; set; } = "Owner";
        public bool GainExperience {  get; set; } = true;
        public string WhoGainsExperience { get; set; } = "Owner";
        public bool UpdateGameStats { get; set; } = true;
    }
}
