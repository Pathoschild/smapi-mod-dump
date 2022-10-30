/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

namespace ManaBar
{
    public class ModConfig
    {
        public bool RenderManaBar { get; set; }

        public int XManaBarOffset { get; set; }

        public int YManaBarOffset { get; set; }

        public float SizeMultiplier { get; set; }

        public ModConfig()
        {
            XManaBarOffset = 0;
            YManaBarOffset = 0;

            RenderManaBar = true;

            SizeMultiplier = 15f;
        }
    }
}
