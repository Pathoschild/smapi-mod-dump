/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JhonnieRandler/TVBrasileira
**
*************************************************/

namespace TVBrasileira.frameworks
{
    public class ModConfig
    {
        public bool PalmirinhaToggle { get; set; }
        public bool GloboRuralToggle { get; set; }
        public bool EdnaldoPereiraToggle { get; set; }

        public ModConfig()
        {
            EdnaldoPereiraToggle = true;
            PalmirinhaToggle = true;
            GloboRuralToggle = true;
        }
    }
}