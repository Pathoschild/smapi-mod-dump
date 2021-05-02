/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardustCore.Animations;

namespace Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCStatusEffects
{
    public class SE_Freeze:SSCStatusEffects.StatusEffect
    {

        public SE_Freeze()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Sprite"></param>
        /// <param name="Duration">The number of milliseconds the effect lasts.</param>
        /// <param name="Frequency">How many milliseconds pass between the effect triggering.</param>
        /// <param name="ChanceToAfflict">The chance this effect has to trigger. Value between 0-1.0</param>
        /// <param name="CanStack">Can this status effect stack?</param>
        public SE_Freeze(StardustCore.Animations.AnimatedSprite Sprite, double Duration, double Frequency, double ChanceToAfflict, bool CanStack, bool ResetsSameTimers) : base(Sprite, Duration, Frequency, ChanceToAfflict, CanStack, ResetsSameTimers)
        {
        }

        /// <summary>
        /// Spawn a new burn status effect with the given paramaters.
        /// </summary>
        /// <param name="position">The position to draw the effect.</param>
        /// <param name="Duration">The duration of the effect in milliseconds.</param>
        /// <param name="Frequency">The freuency in milliseconds between triggers.</param>
        /// <param name="Chance">The chance to be afflicted.</param>
        /// <param name="Damage">The damage it does per trigger.</param>
        /// <returns></returns>
        public static SE_Freeze SpawnFreezeEffect(Vector2 position, double Duration, double Frequency, double Chance)
        {
            return new SE_Freeze(new StardustCore.Animations.AnimatedSprite("Freeze", position, new StardustCore.Animations.AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("SSCUI", "Freeze"), new Animation(0, 0, 16, 16)), Color.White), Duration, Frequency, Chance, false, false);
        }

    }
}
