/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System;
using StardewModdingAPI;

namespace ContentPatcher.Framework.Commands.Commands
{
    /// <summary>A console command which immediately refreshes the condition context and rechecks all patches.</summary>
    internal class UpdateCommand : BaseCommand
    {
        /*********
        ** Fields
        *********/
        /// <summary>A callback which immediately updates the current condition context.</summary>
        private readonly Action UpdateContext;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="monitor">Encapsulates monitoring and logging.</param>
        /// <param name="updateContext">A callback which immediately updates the current condition context.</param>
        public UpdateCommand(IMonitor monitor, Action updateContext)
            : base(monitor, "update")
        {
            this.UpdateContext = updateContext;
        }

        /// <inheritdoc />
        public override string GetDescription()
        {
            return @"
                patch update
                   Usage: patch update
                   Immediately refreshes the condition context and rechecks all patches.
            ";
        }

        /// <inheritdoc />
        public override void Handle(string[] args)
        {
            this.UpdateContext();
            this.Monitor.Log("Updated the Content Patcher context.", LogLevel.Info);
        }
    }
}
