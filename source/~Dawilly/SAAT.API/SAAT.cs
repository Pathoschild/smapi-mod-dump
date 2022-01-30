/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dawilly/SAAT
**
*************************************************/

using StardewModdingAPI;

namespace SAAT.API {
    /// <summary>
    /// Implementation of <see cref="Mod"/> that provides access to <see cref="IAudioManager"/> for other mods.
    /// </summary>
    public class SAAT : Mod
    {
        private IAudioManager audioManager;

        /// <inheritdoc/>
        public override void Entry(IModHelper helper)
        {
            this.audioManager = new AudioManager(this.Monitor);

            helper.ConsoleCommands.Add("audioallocs", "Print the memory allocations for a specific audio track or all audio tracks.", this.ListMallocs);
        }

        /// <inheritdoc/>
        public override object GetApi()
        {
            return this.audioManager;
        }

        /// <summary>
        /// Callback method for the SMAPI Command.
        /// </summary>
        /// <param name="command">The called command.</param>
        /// <param name="argv">The argument value(s).</param>
        private void ListMallocs(string command, string[] argv)
        {
            if (argv.Length > 0)
            {
                this.audioManager.PrintTrackAllocationAndSettings(argv[0]);
                return;
            }

            this.audioManager.PrintMemoryAllocationInfo();
        }
    }
}
