/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace FireBreath
{
    public class ModConfig
    {
        public bool Enabled { get; set; } = true;
        public SButton FireButton { get; set; } = SButton.Insert;
        public bool ScaleWithSkill { get; set; } = true;
        public int FireDamage { get; set; } = 100;
        public int FireDistance { get; set; } = 256;
        public string FireSound { get; set; } = "furnace";
    }
}
