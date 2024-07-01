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
    /// <summary>Handles title menu transitions.</summary>
    /// <remarks>See game logic in <see cref="TitleMenu"/>.</remarks>
    internal sealed class TitleMenuHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public TitleMenuHandler(float multiplier)
            : base(multiplier) { }

        /// <inheritdoc />
        public override bool TryApply(int playerAnimationId)
        {
            return
                Game1.activeClickableMenu is TitleMenu { isTransitioningButtons: true } menu
                && this.ApplySkipsWhile(() =>
                {
                    menu.update(Game1.currentGameTime);

                    return menu.isTransitioningButtons;
                });
        }
    }
}
