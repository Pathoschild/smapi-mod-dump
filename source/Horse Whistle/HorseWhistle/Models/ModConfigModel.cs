using StardewModdingAPI;

namespace HorseWhistle.Models
{
    internal class ModConfigModel
    {
        public bool EnableGrid { get; set; } = false;
        public bool EnableWhistleAudio { get; set; } = true;
        public SButton EnableGridKey { get; set; } = SButton.G;
        public SButton TeleportHorseKey { get; set; } = SButton.V;
    }
}