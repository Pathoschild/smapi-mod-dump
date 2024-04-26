/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;

namespace LandGrants
{
    public class Config
    {
        public bool KeepFarmsActive { get; set; } = false;

        public int MaxPlayer { get; set; } = 16;

        public SButton BuildCabinKey { get; set; } = SButton.F10;
    }
}
