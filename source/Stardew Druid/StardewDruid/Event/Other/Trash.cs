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
using StardewDruid.Map;
using StardewValley;
using System;
using System.Collections.Generic;

namespace StardewDruid.Event.Other
{
    public class Trash : TriggerHandle
    {

        public Trash(GameLocation location, Quest quest)
            : base(location, quest)
        {

        }

        public override bool SetMarker()
        {

            targetVector = questData.triggerVector;

            TemporaryAnimatedSprite newAnimation;

            List<Vector2> trashVectors = new()
            {
                new(targetVector.X+1,targetVector.Y-4),
                new(targetVector.X+2,targetVector.Y+4),
                new(targetVector.X+6,targetVector.Y-1),

            };

            foreach (Vector2 trashVector in trashVectors)
            {

                Rectangle targetRectangle = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 168, 16, 16);

                newAnimation = new(
                    "Maps\\springobjects",
                    targetRectangle,
                    trashVector * 64 - new Vector2(16, 0),
                    flipped: false,
                    0f,
                    Color.White * 0.5f
                )
                {
                    interval = 99999f,
                    totalNumberOfLoops = 99999,
                    scale = 3f,
                };

                targetLocation.temporarySprites.Add(newAnimation);

                animationList.Add(newAnimation);

            }

            return true;

        }

        public override void EventInterval()
        {

            TemporaryAnimatedSprite newAnimation;

            List<Vector2> trashVectors = new()
            {
                new(targetVector.X+1,targetVector.Y-4),
                new(targetVector.X+2,targetVector.Y+4),
                new(targetVector.X+6,targetVector.Y-1),

            };

            foreach (Vector2 trashVector in trashVectors)
            {

                newAnimation = new(
                    "LooseSprites\\Cursors",
                    new Rectangle(372, 1956, 10, 10),
                    trashVector * 64,
                    flipped: false,
                    0.002f,
                    Color.Green
                )
                {
                    alpha = 0.75f,
                    motion = new Vector2(0f, -0.5f),
                    //acceleration = new Vector2(0.002f, 0f),
                    interval = 99999f,
                    layerDepth = 0.001f,
                    scale = 2f,
                    scaleChange = 0.02f,
                    rotationChange = Game1.random.Next(-5, 6) * MathF.PI / 256f,
                    //delayBeforeAnimationStart = delay
                };

                targetLocation.temporarySprites.Add(newAnimation);

                animationList.Add(newAnimation);

            }

            base.EventInterval();

        }

    }

}
