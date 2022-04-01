/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/


using StardewModdingAPI;

namespace AdvancedMenuPositioning
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public SButton DetachKey { get; set; } = SButton.X;
        public SButton MoveKey { get; set; } = SButton.MouseLeft;
        public SButton CloseKey { get; set; } = SButton.Z;
        public SButton DetachModKey { get; set; } = SButton.LeftShift;
        public SButton MoveModKey { get; set; } = SButton.LeftShift;
        public SButton CloseModKey { get; set; } = SButton.LeftShift;
    }
}
