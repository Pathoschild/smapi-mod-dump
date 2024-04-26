/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

namespace MeteorDefence
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public bool StrikeAnywhere { get; set; } = false;
        public int MinMeteorites { get; set; } = 1;
        public int MaxMeteorites { get; set; } = 1;
        public string DefenceObject { get; set; } = "Space Laser";
        public int MeteorsPerObject { get; set; } = 1;
        public string DefenceSound { get; set; } = "debuffSpell";
        public string ExplodeSound { get; set; } = "explosion";

    }
}
