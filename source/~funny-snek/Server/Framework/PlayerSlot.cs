namespace FunnySnek.AntiCheat.Server.Framework
{
    /// <summary>A connected player slot.</summary>
    internal class PlayerSlot
    {
        /// <summary>The unique multiplayer ID.</summary>
        public long PlayerID { get; set; }

        /// <summary>Whether the server is waiting for the player info after they joined.</summary>
        public bool IsCountingDown { get; set; }

        /// <summary>The number of seconds until the player should be kicked if we haven't received their local mod ping.</summary>
        public int CountDownSeconds { get; set; }
    }
}
