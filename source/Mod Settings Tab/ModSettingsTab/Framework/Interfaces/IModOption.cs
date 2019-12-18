namespace ModSettingsTab.Framework.Interfaces
{
    public interface IModOption
    {
        string Name { get; set; }
        
        string ModId { get; set; }

        string Label { get; set; }

        bool GreyedOut { get; set; }
    }
}