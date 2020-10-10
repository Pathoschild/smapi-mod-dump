/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/AudioDevices
**
*************************************************/

using StardewValley;

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
