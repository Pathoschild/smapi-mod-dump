namespace ModSettingsTab.Framework.Interfaces
{
    public interface IOptionSlider : IModOption
    {
        int SliderValue { get; set; }

        int SliderMaxValue { get; set; }

        int SliderMinValue { get; set; }

        int SliderStep { get; set; }
    }
}