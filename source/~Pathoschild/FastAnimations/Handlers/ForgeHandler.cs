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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the volcano forge animations.</summary>
    /// <remarks>See game logic in <see cref="ForgeMenu.update"/>.</remarks>
    internal class ForgeHandler : BaseAnimationHandler
    {
        /*********
        ** Fields
        *********/
        /// <summary>An API for accessing inaccessible code.</summary>
        private readonly IReflectionHelper Reflection;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="multiplier">The animation speed multiplier to apply.</param>
        /// <param name="reflection">An API for accessing inaccessible code.</param>
        public ForgeHandler(float multiplier, IReflectionHelper reflection)
            : base(multiplier)
        {
            this.Reflection = reflection;
        }

        /// <inheritdoc />
        public override bool IsEnabled(int playerAnimationID)
        {
            return
                Game1.activeClickableMenu is ForgeMenu menu
                && (
                    this.Reflection.GetField<int>(menu, "_timeUntilCraft").GetValue() > 0
                    || this.Reflection.GetField<int>(menu, "_sparklingTimer").GetValue() > 0
                );
        }

        /// <inheritdoc />
        public override void Update(int playerAnimationID)
        {
            ForgeMenu menu = (ForgeMenu)Game1.activeClickableMenu;

            this.ApplySkips(
                () => menu.update(Game1.currentGameTime)
            );
        }
    }
}
