namespace ModSettingsTabApi.Framework.Interfaces
{
    public interface IModTabSettingsApi
    {
        ISettingsTabApi GetMod(string uniqueId);
    }
}
