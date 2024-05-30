/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lisyce/SDV_Allergies_Mod
**
*************************************************/

using StardewModdingAPI;

namespace BZP_Allergies.Config
{
    internal sealed class ModConfigModel
    {
        public SButton AllergyPageButton { get; set; } = SButton.L;
        public bool EnableTab { get; set; } = true;
        public bool HintBeforeEating { get; set; } = true;
        public int NumberRandomAllergies { get; set; } = -1;
        public bool AllergenCountHint { get; set; } = true;

        public bool HoldingReaction { get; set; } = false;
        public bool CookingReaction { get; set; } = false;
        
        public bool EnableNausea { get; set; } = true;
        public int EatingDebuffLengthSeconds { get; set; } = 120;
        public float DebuffSeverityMultiplier { get; set; } = 1;
    }
}
