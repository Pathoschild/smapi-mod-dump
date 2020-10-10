/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/taggartaa/AutoAnimalDoors
**
*************************************************/


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
