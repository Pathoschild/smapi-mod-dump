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
using StardewValley.Tools;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the tool swinging animation.</summary>
    internal class WeaponSwingHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public WeaponSwingHandler(float multiplier)
            : base(multiplier) { }

        /// <inheritdoc />
        public override bool IsEnabled(int playerAnimationID)
        {
            return
                Game1.player.UsingTool
                && Game1.player.CurrentTool is MeleeWeapon weapon
                && !weapon.isScythe();
        }

        /// <inheritdoc />
        public override void Update(int playerAnimationID)
        {
            this.SpeedUpPlayer(until: () => !Game1.player.UsingTool);
        }
    }
}
