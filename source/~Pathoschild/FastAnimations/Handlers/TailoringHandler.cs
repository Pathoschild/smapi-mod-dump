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
    /// <summary>Handles the tailoring animation.</summary>
    /// <remarks>See game logic in <see cref="TailoringMenu.receiveLeftClick"/>.</remarks>
    internal sealed class TailoringHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public TailoringHandler(float multiplier)
            : base(multiplier) { }

        /// <inheritdoc />
        public override bool TryApply(int playerAnimationId)
        {
            return
                Game1.activeClickableMenu is TailoringMenu menu
                && menu.IsBusy()
                && this.ApplySkipsWhile(() =>
                {
                    menu.update(Game1.currentGameTime);

                    return menu.IsBusy();
                });
        }
    }
}
