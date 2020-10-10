/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/AudioDevices
**
*************************************************/


namespace AudioDevices
{
    public class AudioDeviceSettings
    {
        public enum AudioSwitchToDefaultDeviceMode
        {
            Always,
            WhenCurrentDeviceLost,
            Never
        }
        public AudioSwitchToDefaultDeviceMode SwitchToDefaultDeviceMode { get; set; } = AudioSwitchToDefaultDeviceMode.Always;
        public string SelectedAudioDevice { get; set; } = "";
        public int CheckAudioDevicesInterval { get; set; } = 10;
    }
}
