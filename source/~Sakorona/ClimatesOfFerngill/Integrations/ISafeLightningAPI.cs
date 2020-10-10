/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sakorona/SDVMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace ClimatesOfFerngillRebuild.Integrations
{
    public interface ISafeLightningAPI
    {
        /// <summary>
        /// Method to call when you want to safely create lightning.
        /// </summary>
        /// <param name="position">Where to create the lightning</param>
        /// <param name="effects">Whether to create appropriate sound and visual effects</param>
        void StrikeLightningSafely(Vector2 position, bool effects = true);
    }
}
