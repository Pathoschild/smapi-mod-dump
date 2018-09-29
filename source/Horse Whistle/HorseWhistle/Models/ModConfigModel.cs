using Microsoft.Xna.Framework.Input;

namespace HorseWhistle.Models
{
    class ModConfigModel
    {
        public string EnableGridKey { get; set; }
        public string TeleportHorseKey { get; set; }

        public ModConfigModel()
        {
            EnableGridKey = Keys.G.ToString();
            TeleportHorseKey = Keys.V.ToString();
        }
    }
}
