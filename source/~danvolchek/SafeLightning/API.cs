/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace SafeLightning
{
    public class API
    {
        internal ModEntry.StrikeLightningDelegate methodToCall;

        public API(ModEntry.StrikeLightningDelegate methodToCall)
        {
            this.methodToCall = methodToCall;
        }

        /// <summary>
        ///     Method to call when another mod wants to safely create lightning.
        /// </summary>
        /// <param name="position">Where to create the lightning</param>
        /// <param name="effects">Whether to create appropriate sound and visual effects</param>
        public void StrikeLightningSafely(Vector2 position, bool effects = true)
        {
            this.methodToCall(position, effects);
        }
    }
}
