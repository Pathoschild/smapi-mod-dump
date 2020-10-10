/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JohnsonNicholas/SDVMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace TwilightShards.ClimatesOfFerngillV2
{
    public static class Letter
    {
        /// <summary>The sprite sheet containing the letter sprites.</summary>
        public static Texture2D Sheet => Game1.content.Load<Texture2D>("LooseSprites\\letterBG");

        /// <summary>The letter background (including edges and corners).</summary>
        public static readonly Rectangle Sprite = new Rectangle(0, 0, 320, 180);
    }
}
