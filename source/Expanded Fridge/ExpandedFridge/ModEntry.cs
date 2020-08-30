using StardewModdingAPI;


namespace ExpandedFridge
{
    /// <summary>
    /// The entry point of the mod handled by SMAPI.
    /// </summary>
    public class ModEntry : Mod
    {
        /// Instance check for logging.
        private static bool _instanceInitiated = false;

        /// Instance for static logging.
        private static ModEntry _instance = null;
        
        /// Mod options instance.
        public ModConfig Config { get; private set; }

        /// Manager instance.
        public Manager Manager { get; private set; }

        /// Setup instance and mini fridge manager on entry.
        public override void Entry(IModHelper helper)
        {
            _instance = this;
            _instanceInitiated = true;
            
            Config = helper.ReadConfig<ModConfig>();
            Manager = new Manager(this);
        }

        /// Prints message in console log with given log level.
        public static void DebugLog(string message, LogLevel level = LogLevel.Trace)
        {
            if (_instanceInitiated)
                _instance.Monitor.Log(message, level);
        }
    }
}
