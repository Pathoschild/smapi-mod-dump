/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/


using StardewModdingAPI;

namespace CustomFarmAnimals
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public SButton ReloadButton { get; set; } = SButton.F5;
    }
}
