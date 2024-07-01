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
    /// <summary>Handles the dialogue-typing animation.</summary>
    /// <remarks>See game logic in <see cref="DialogueBox.update"/>.</remarks>
    internal sealed class DialogueTypingHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public DialogueTypingHandler(float multiplier)
            : base(multiplier) { }

        /// <inheritdoc />
        public override bool TryApply(int playerAnimationId)
        {
            return
                Game1.activeClickableMenu is DialogueBox { transitioning: false } menu
                && menu.characterIndexInDialogue < menu.getCurrentString().Length
                && this.ApplySkipsWhile(() =>
                {
                    menu.update(Game1.currentGameTime);

                    return menu.characterIndexInDialogue < menu.getCurrentString().Length;
                });
        }
    }
}
