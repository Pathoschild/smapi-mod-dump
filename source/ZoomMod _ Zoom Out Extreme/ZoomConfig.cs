using Microsoft.Xna.Framework.Input;

namespace RockinMods
{
    class ZoomConfig
    {
        public Keys KeyIn { get; set; }
        public Keys KeyOut { get; set; }

        public Buttons ButtonIn { get; set; }
        public Buttons ButtonOut { get; set; }

        public ZoomConfig()
        {
            KeyIn = Keys.OemPlus;
            KeyOut = Keys.OemMinus;

            ButtonIn = Buttons.DPadDown;
            ButtonOut = Buttons.DPadUp;
        }
    }
}
