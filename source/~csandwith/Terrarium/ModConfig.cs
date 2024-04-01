/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

namespace Terrarium
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public int Frogs { get; set; } = 2;
        public string PlaySound { get; set; } = "croak";
        public bool LoadCustomTerrarium{ get; set; } = true;
    }
}
