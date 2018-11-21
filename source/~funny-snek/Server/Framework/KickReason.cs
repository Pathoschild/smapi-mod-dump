namespace FunnySnek.AntiCheat.Server.Framework
{
    /// <summary>A reason for a player to be kicked.</summary>
    public enum KickReason
    {
        /// <summary>The player doesn't have SMAPI installed.</summary>
        NeedsSmapi,

        /// <summary>The player has prohibited mods installed.</summary>
        BlockedMods
    }
}