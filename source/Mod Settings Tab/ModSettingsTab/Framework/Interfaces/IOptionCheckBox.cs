namespace ModSettingsTab.Framework.Interfaces
{
    public interface IOptionCheckBox : IModOption
    {
        bool CheckBoxIsChecked { get; set; }
    }
}