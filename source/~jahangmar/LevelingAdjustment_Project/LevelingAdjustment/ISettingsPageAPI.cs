namespace ModSettingsTab.Framework.Interfaces
{
    public interface ISettingsPageApi
    {
        //IModOptions GetOptions(string uniqueId);

        void DisableStaticConfig(string uniqueId);

        void DisableStaticConfig(string uniqueId, string path);
    }
}