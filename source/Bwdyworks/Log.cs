namespace bwdyworks
{
    public class Log
    {
        private StardewModdingAPI.IMonitor Monitor;
        internal Log(StardewModdingAPI.IMonitor monitor)
        {
            Monitor = monitor;
        }

        // Tracing info intended for developers, usually troubleshooting details that are useful 
        // when someone sends you their error log.Trace messages won't appear in the console window 
        // by default (unless you have the "SMAPI for developers" version), though they're always 
        // written to the log file.
        public void Trace(string message) { Monitor.Log(message, StardewModdingAPI.LogLevel.Trace); }

        // Troubleshooting info that may be relevant to the player.
        public void Debug(string message) { Monitor.Log(message, StardewModdingAPI.LogLevel.Debug); }

        // Info relevant to the player. This should be used judiciously. 
        public void Info(string message) { Monitor.Log(message, StardewModdingAPI.LogLevel.Info); }

        // An issue the player should be aware of. This should be used rarely. 
        public void Warn(string message) { Monitor.Log(message, StardewModdingAPI.LogLevel.Warn); }

        // A message indicating something went wrong.
        public void Error(string message) { Monitor.Log(message, StardewModdingAPI.LogLevel.Error); }

        // Important information to highlight for the player when player action is needed 
        // (e.g. new version available). This should be used rarely to avoid alert fatigue. 
        public void Alert(string message){ Monitor.Log(message, StardewModdingAPI.LogLevel.Alert);  }
    }
}
