using StardewModdingAPI;

namespace ModSettingsTab.Framework.Interfaces
{
    public interface IOptionInputListener : IModOption
    {
        SButton InputListenerButton { get; set; }
    }
}