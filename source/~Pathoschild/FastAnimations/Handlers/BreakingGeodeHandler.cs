/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using Pathoschild.Stardew.FastAnimations.Framework;
using StardewValley;
using StardewValley.Menus;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the geode-breaking animation.</summary>
    /// <remarks>See game logic in <see cref="GeodeMenu.receiveLeftClick"/>.</remarks>
    internal class BreakingGeodeHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public BreakingGeodeHandler(float multiplier)
            : base(multiplier) { }

        /// <inheritdoc />
        public override bool IsEnabled(int playerAnimationID)
        {
            return Game1.activeClickableMenu is GeodeMenu { geodeAnimationTimer: > 0 };
        }

        /// <inheritdoc />
        public override void Update(int playerAnimationID)
        {
            GeodeMenu menu = (GeodeMenu)Game1.activeClickableMenu;

            this.ApplySkips(
                () => menu.update(Game1.currentGameTime)
            );
        }
    }
}
