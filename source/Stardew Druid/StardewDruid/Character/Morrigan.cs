/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewDruid.Cast;
using StardewDruid.Cast.Mists;
using StardewDruid.Cast.Weald;
using StardewDruid.Event;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.FruitTrees;
using StardewValley.Internal;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection.Metadata.Ecma335;

namespace StardewDruid.Character
{
    public class Morrigan : StardewDruid.Character.Character
    {
        
        new public CharacterHandle.characters characterType = CharacterHandle.characters.Morrigan;

        public Texture2D hatsTexture;

        public Morrigan()
        {
        }

        public Morrigan(CharacterHandle.characters type)
          : base(type)
        {

            
        }

        public override void LoadOut()
        {
            base.LoadOut();

            hatsTexture = Game1.content.Load<Texture2D>("Characters\\Farmer\\hats");
        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {

            if (IsInvisible || !Utility.isOnScreen(Position, 128))
            {
                return;
            }

            if (characterTexture == null)
            {

                return;

            }

            Vector2 localPosition = getLocalPosition(Game1.viewport);

            float drawLayer = (float)StandingPixel.Y / 10000f;

            DrawEmote(b);
            if (netSpecialActive.Value)
            {

                b.Draw(
                    characterTexture,
                    localPosition - new Vector2(32, 64f),
                    new(32, 0, 32, 32),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    drawLayer
                );

                b.Draw(
                    hatsTexture,
                    localPosition + new Vector2(0, -84f),
                    Game1.getSourceRectForStandardTileSheet(hatsTexture, 358, 20, 20),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    3.5f,
                    flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    drawLayer + 0.0001f
                );

            }
            else if (netDirection.Value == 2)
            {

                b.Draw(
                    characterTexture,
                    localPosition - new Vector2(32, 64f),
                    new(64, 0, 32, 32),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    drawLayer
                );

                b.Draw(
                    hatsTexture,
                    localPosition + new Vector2(0, -84f),
                    Game1.getSourceRectForStandardTileSheet(hatsTexture, 358, 20, 20),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    3.5f,
                    flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    drawLayer + 0.0001f
                );

            }
            else
            {

                b.Draw(
                    characterTexture,
                    localPosition - new Vector2(32, 64f),
                    new(0, 0, 32, 32),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    drawLayer
                );

                b.Draw(
                    hatsTexture,
                    localPosition + new Vector2(0, -84f),
                    Game1.getSourceRectForStandardTileSheet(hatsTexture, 358, 20, 20),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    3.5f,
                    flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    drawLayer + 0.0001f
                );

            }

        }

        public override void behaviorOnFarmerPushing()
        {

            return;

        }

        public override bool checkAction(Farmer who, GameLocation l)
        {

            return false;

        }

    }

}
