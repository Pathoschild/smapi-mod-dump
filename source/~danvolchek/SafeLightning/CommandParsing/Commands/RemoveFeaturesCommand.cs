/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;

namespace SafeLightning.CommandParsing.Commands
{
    /// <summary>Removes every terrain feature in the player's farm.</summary>
    internal class RemoveFeaturesCommand : BaseCommand
    {
        /*********
        ** Public methods
        *********/

        /// <summary></summary>
        /// <param name="monitor">The monitor used for command output.</param>
        public RemoveFeaturesCommand(IMonitor monitor) : base(monitor, "remove_features", "Removes all terrain features from your farm.")
        {
        }

        /// <summary>Invoke the command.</summary>
        /// <param name="args">The command arguments</param>
        public override void Invoke(string[] args)
        {
            Game1.getFarm().terrainFeatures.Clear();
            this.monitor.Log("Okay, removed all terrain features from your farm.", LogLevel.Info);
        }
    }
}
