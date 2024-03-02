/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using System;

namespace Randomizer
{
    /// <summary>
    /// An extension of Random to log when it's not actually meant to be called
    /// This is to ensure that players don't get out of sync with one another
    /// </summary>
    public class SaveLoadRNG : Random
    {
        /// <summary>
        /// Whether we should not use the class anymore
        /// </summary>
        public bool IsPostFileLoad { get; set; }

        public SaveLoadRNG(int seed) : base(seed) { }

        public override int Next()
        {
            TryLogPostFileLoadWarning();
            return base.Next();
        }

        public override int Next(int maxValue)
        {
            TryLogPostFileLoadWarning();
            return base.Next(maxValue);
        }

        public override int Next(int minValue, int maxValue)
        {
            TryLogPostFileLoadWarning();
            return base.Next(minValue, maxValue);
        }

        /// <summary>
        /// Tries to log the error message telling users to let the dev know that the wrong
        /// RNG was used after the game was loaded
        /// </summary>
        private void TryLogPostFileLoadWarning()
        {
            if (IsPostFileLoad)
            {
                Globals.ConsoleWarn("WARNING: The file load RNG object was used after the file was loaded! Please let the developers know that this was logged (include your farm name).");
            }
        }
    }
}
