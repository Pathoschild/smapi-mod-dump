/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using StardewModdingAPI;

namespace GameZoomer
{
    internal class GameZoomConfig
    {
        public SButton ZoomInKey { get; set; } = SButton.Add;
        public SButton ZoomOutKey { get; set; } = SButton.Subtract;
        public SButton ZoomButtonIn { get; set; } = SButton.DPadDown;
        public SButton ZoomButtonOut { get; set; } = SButton.DPadUp;
    }
}
