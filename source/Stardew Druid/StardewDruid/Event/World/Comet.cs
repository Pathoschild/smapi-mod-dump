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
using StardewDruid.Cast;
using StardewDruid.Map;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using xTile.Dimensions;

namespace StardewDruid.Event.World
{
    public class Comet : EventHandle
    {

        public Dictionary<int, TemporaryAnimatedSprite> cometAnimations;

        public Vector2 cometVector;

        public float damage;

        public Comet(Vector2 target, Rite rite, float Damage)
            : base(target, rite)
        {

            cometVector = target * 64;

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 6;

            cometAnimations = new();

            damage = Damage * 5;

        }

        public override void EventTrigger()
        {

            Mod.instance.RegisterEvent(this, "comet");

        }

        public override void EventRemove()
        {
            if (cometAnimations.Count > 0)
            {

                foreach (KeyValuePair<int, TemporaryAnimatedSprite> animation in cometAnimations)
                {

                    targetLocation.temporarySprites.Remove(animation.Value);

                }

            }

            cometAnimations.Clear();

        }

        public override void EventInterval()
        {

            activeCounter++;

            if (activeCounter == 1)
            {

                TemporaryAnimatedSprite startAnimation = new(0, 2000f, 1, 1, cometVector - new Vector2(128,128), false, false)
                {

                    sourceRect = new(0, 0, 64, 64),

                    sourceRectStartingPos = new Vector2(0, 0),

                    texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Stars.png")),

                    scale = 5f,

                    //scaleChange = 0.001f,

                    //motion = new Vector2(-0.032f, -0.032f),

                    layerDepth = 0.0001f,

                    rotationChange = 0.06f,

                    timeBasedMotion = true,

                    alpha = 0.75f,

                };

                targetLocation.temporarySprites.Add(startAnimation);

                cometAnimations[0] = startAnimation;

            }

            if (activeCounter == 2)
            {

                Vector2 cometPosition = new(cometVector.X + 320, cometVector.Y - 720);

                Vector2 cometMotion = new Vector2(-0.32f, 0.64f);

                TemporaryAnimatedSprite cometAnimation = new(0, 1000f, 1, 1, cometPosition, false, false)
                {

                    sourceRect = new(0, 0, 32, 32),

                    sourceRectStartingPos = new Vector2(0, 0),

                    texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Fireball.png")),

                    scale = 4f,

                    motion = cometMotion,

                    timeBasedMotion = true,

                    rotationChange = -0.08f,

                    layerDepth = (targetVector.Y / 1000) + 0.005f,

                };

                targetLocation.temporarySprites.Add(cometAnimation);

                cometAnimations[1] = cometAnimation;

            }

            if (activeCounter == 3)
            {

                targetLocation.playSound("explosion");

                Mod.instance.CastMessage("Meteor Impact");

                ModUtility.DamageMonsters(targetLocation, ModUtility.MonsterProximity(targetLocation, targetVector * 64, 8, true), targetPlayer, (int)damage, true);

                List<Vector2> impactVectors = ModUtility.Explode(targetLocation, targetVector, targetPlayer, 8, 3, 5);

                foreach (Vector2 vector in impactVectors)
                {

                    ModUtility.AnimateDestruction(targetLocation, vector);

                }
                expireEarly = true;

            }

        }

    }

}
