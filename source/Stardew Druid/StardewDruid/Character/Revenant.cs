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
using StardewDruid.Data;
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
using System.Linq;

namespace StardewDruid.Character
{
    public class Revenant : StardewDruid.Character.Character
    {
        new public CharacterHandle.characters characterType = CharacterHandle.characters.Revenant;

        public Revenant()
        {
        }

        public Revenant(CharacterHandle.characters type)
          : base(type)
        {

            
        }

        public override void LoadOut()
        {
            
            characterType = CharacterHandle.characters.Revenant;

            base.LoadOut();

            idleFrames = new()
            {
                [0] = new List<Rectangle> { new Rectangle(160, 32, 32, 32), },
            };

        }

        public override void DrawStandby(SpriteBatch b, Vector2 localPosition, float drawLayer)
        {

            b.Draw(
                characterTexture,
                localPosition - new Vector2(32f, 64f),
                idleFrames[0][0],
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                netDirection.Value == 1 || netAlternative.Value == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                drawLayer
            );

            b.Draw(
                characterTexture,
                localPosition - new Vector2(30f, 60f),
                idleFrames[0][0],
                Color.Black * 0.25f,
                0f,
                Vector2.Zero,
                4f,
                netDirection.Value == 1 || netAlternative.Value == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                drawLayer - 0.001f
            );

            return;

        }

    }

}
