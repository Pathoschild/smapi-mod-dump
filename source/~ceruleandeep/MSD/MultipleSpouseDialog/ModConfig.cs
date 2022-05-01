/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

namespace MultipleSpouseDialog
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public int MinHeartsForChat { get; set; } = 9;
        public bool AllowSpousesToChat { get; set; } = true;
        public bool ChatWithPlayer { get; set; } = true;
        public float SpouseChatChance { get; set; } = 0.05f;
        public float MinDistanceToChat { get; set; } = 100f;
        public float MaxDistanceToChat { get; set; } = 350f;
        public float MinSpouseChatInterval { get; set; } = 10f;
        public bool PreventRelativesFromChatting { get; set; }
        public bool ExtraDebugOutput { get; set; }
    }
}