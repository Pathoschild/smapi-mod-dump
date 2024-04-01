/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/justastranger/MushroomLogAdditions
**
*************************************************/

using StardewModdingAPI;

namespace MushroomLogAdditions
{
    internal class Config
    {
        public LogLevel loggingLevel { get; set; }
        public bool loadInternal { get; set; }
        public int scanRadius { get; set; }

        public Config()
        {
            loggingLevel = LogLevel.Trace;
            loadInternal = true;
            scanRadius = 3;
        }
    }
}
