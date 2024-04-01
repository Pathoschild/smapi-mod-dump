/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/stardew-mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace DialogueBoxRedesign
{
    public interface IHDPortraitsAPI
    {
        /// <summary>
        /// Retrieves the texture and texture region to use for a portrait
        /// </summary>
        /// <param name="npc">NPC</param>
        /// <param name="index">Portrait index</param>
        /// <param name="elapsed">Time since last call (for animation)</param>
        /// <param name="reset">Whether or not to reset animations this tick</param>
        /// <returns>The source region & the texture to use</returns>
        public (Rectangle, Texture2D) GetTextureAndRegion(NPC npc, int index, int elapsed = -1, bool reset = false);
    }
}
