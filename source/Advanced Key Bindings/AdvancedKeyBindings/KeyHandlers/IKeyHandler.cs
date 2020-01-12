using StardewModdingAPI;

namespace AdvancedKeyBindings.KeyHandlers
{
    public interface IKeyHandler
    {
        bool ReceiveButtonPress(SButton input);
    }
}