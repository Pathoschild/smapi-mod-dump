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
