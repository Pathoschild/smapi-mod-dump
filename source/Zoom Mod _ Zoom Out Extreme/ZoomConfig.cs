/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rockinrolla/ZoomMod
**
*************************************************/

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
