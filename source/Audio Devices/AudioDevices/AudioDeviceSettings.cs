
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
