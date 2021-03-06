/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace HorseOverhaul
{
    using Microsoft.Xna.Framework.Graphics;
    using StardewValley;
    using System;

    public interface IBetterRanchingApi
    {
        void DrawHeartBubble(SpriteBatch spriteBatch, Character character, Func<bool> displayHeart);

        void DrawHeartBubble(SpriteBatch spriteBatch, float xPosition, float yPosition, int spriteWidth, Func<bool> displayHeart);

        void DrawItemBubble(SpriteBatch spriteBatch, FarmAnimal animal, bool ranchingInProgress);

        void DrawItemBubble(SpriteBatch spriteBatch, float xPosition, float yPosition, int spriteWidth, bool isShortTarget, int produceIcon, Func<bool> displayItem, Func<bool> displayHeart);
    }
}