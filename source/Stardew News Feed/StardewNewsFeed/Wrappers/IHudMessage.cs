using StardewNewsFeed.Enums;
namespace StardewNewsFeed.Wrappers {

    /// <summary>
    /// Wrapper for StardewValley.HUDMessage
    /// </summary>
    public interface IHudMessage {
        string GetMessageText();
        HudMessageType GetMessageType();
    }
}
