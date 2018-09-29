using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioDevices
{
    public class AudioOptionsCheckBox : StardewValley.Menus.OptionsCheckbox
    {
        public AudioDeviceSettings.AudioSwitchToDefaultDeviceMode which;

        public AudioOptionsCheckBox(string label, AudioDeviceSettings.AudioSwitchToDefaultDeviceMode which) : base(label, int.MaxValue, -1, -1)
        {
            this.which = which;
            isChecked = ModEntry.mod.Settings.SwitchToDefaultDeviceMode == which;
        }

        public override void receiveLeftClick(int x, int y)
        {
            if (this.greyedOut)
                return;
            Game1.playSound("drumkit6");
            this.isChecked = true;
            ModEntry.UpdateAudioSetting(which);
        }

    }
}
