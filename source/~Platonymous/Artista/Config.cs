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

namespace Artista
{
    public class Config
    {
        public bool FreeCanvas { get; set; } = false;

        public SButton FillButton { get; set; } = SButton.A;
        public SButton LineButton { get; set; } = SButton.LeftShift;
        public SButton ReverseButton { get; set; } = SButton.Z;

        public bool CPFCompatibility { get; set; } = true;
        public SButton CPFStartFramingKey { get; set; } = SButton.F10;
        public SButton CPFSSwitchFrameKey { get; set; } = SButton.F11;

        public SButton ChangeCPFRotation { get; set; } = SButton.F9;
        public SButton ChangeCPFScaleDown { get; set; } = SButton.F7;
        public SButton ChangeCPFScaleUP { get; set; } = SButton.F8;
        public SButton OpenOnlineMenu { get; set; } = SButton.F4;
    }
}
