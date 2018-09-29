
namespace AutoAnimalDoors.StardewValleyWrapper
{
    class Logger
    {
        private static Logger logger;

        private StardewModdingAPI.IMonitor monitor;

        private Logger()
        {

        }

        public static Logger Instance
        {
            get
            {
                if (logger == null)
                {
                    logger = new Logger();
                }

                return logger;
            }
        }

        public void Initialize(StardewModdingAPI.IMonitor monitor)
        {
            this.monitor = monitor;
        }

        public void Log(string message, StardewModdingAPI.LogLevel logLevel = StardewModdingAPI.LogLevel.Debug)
        {
            this.monitor.Log(message, logLevel);
        }

    }
}
