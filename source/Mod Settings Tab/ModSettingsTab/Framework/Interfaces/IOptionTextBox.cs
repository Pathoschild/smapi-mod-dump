namespace ModSettingsTab.Framework.Interfaces
{
    public interface IOptionTextBox : IModOption
    {
        string TextBoxText { get; set; }

        bool TextBoxNumbersOnly { get; set; }

        bool TextBoxFloatOnly { get; set; }
    }
}