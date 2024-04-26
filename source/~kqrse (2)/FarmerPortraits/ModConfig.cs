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

namespace FarmerPortraits
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public bool ShowWithQuestions { get; set; } = true;
        public bool ShowWithEvents { get; set; } = false;
        public bool ShowWithNPCPortrait { get; set; } = true;
        public bool ShowMisc { get; set; } = false;
        public bool FacingFront { get; set; } = false;
        public bool UseCustomPortrait { get; set; } = false;
        public bool UseCustomBackground { get; set; } = true;
    }
}
