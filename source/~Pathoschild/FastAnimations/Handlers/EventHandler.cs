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
using Pathoschild.Stardew.FastAnimations.Framework;
using StardewValley;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the event-run animation.</summary>
    /// <remarks>See game logic in <see cref="Event.Update"/>.</remarks>
    internal sealed class EventHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public EventHandler(float multiplier)
            : base(multiplier) { }

        /// <inheritdoc />
        public override bool TryApply(int playerAnimationId)
        {
            return
                this.ShouldApply()
                && this.ApplySkipsWhile(
                () =>
                {
                    if (this.IsPauseCommand(Game1.CurrentEvent.GetCurrentCommand()))
                    {
                        // We need to update both the pause timer and event logic, since commands may happen in
                        // parallel (e.g. a pause while an NPC moves into position).
                        Game1.updatePause(Game1.currentGameTime);
                    }

                    Game1.CurrentEvent.Update(Game1.currentLocation, Game1.currentGameTime);

                    return this.ShouldApply();
                });
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get whether the handler should be applied now.</summary>
        private bool ShouldApply()
        {
            return
                Game1.eventUp
                && Game1.CurrentEvent != null
                && !Game1.isFestival()
                && !Game1.fadeToBlack
                && !string.Equals(ArgUtility.Get(Game1.CurrentEvent.eventCommands, 3), $"{nameof(Event.DefaultCommands.PlayerControl)} parrotRide", StringComparison.OrdinalIgnoreCase); // handled by ParrotExpressHandler
        }

        /// <summary>Get whether a raw command line is a <see cref="Event.DefaultCommands.Pause"/> command.</summary>
        /// <param name="rawCommand">The unparsed command line.</param>
        private bool IsPauseCommand(string rawCommand)
        {
            const string pauseCommand = nameof(Event.DefaultCommands.Pause);

            // quick check
            if (!rawCommand.Contains(pauseCommand, StringComparison.InvariantCultureIgnoreCase))
                return false;

            // parse command
            string[] args = ArgUtility.SplitBySpaceQuoteAware(rawCommand);
            return string.Equals(args[0], pauseCommand, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
