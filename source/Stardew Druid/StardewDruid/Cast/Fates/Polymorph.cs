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
using StardewDruid.Event;
using StardewDruid.Monster.Boss;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;

namespace StardewDruid.Cast.Fates
{
    public class Polymorph : EventHandle
    {

        public StardewValley.Monsters.Monster victim;

        public bool complete;

        public int decimalCounter;

        public Polymorph(Vector2 target,  StardewValley.Monsters.Monster Monster)
            : base(target)
        {

            victim = Monster;

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 10;

        }

        public override void EventTrigger()
        {

            Mod.instance.RegisterEvent(this, "polymorph");

        }

        public override bool EventActive()
        {

            if (expireEarly)
            {

                return false;

            }

            if (monsterHandle == null)
            {

                if (!ModUtility.MonsterVitals(victim, targetLocation))
                {

                    return false;

                }

                if (victim == null)
                {

                    return false;

                }

            }
            else
            {

                if (monsterHandle.monsterSpawns.Count == 0)
                {

                    return false;

                }

            }

            if (targetPlayer.currentLocation.Name != targetLocation.Name)
            {

                return false;

            }

            if (expireTime > Game1.currentGameTime.TotalGameTime.TotalSeconds)
            {

                return true;

            }

            return false;

        }

        public override void EventDecimal()
        {

            if (victim == null)
            {

                return;

            }

            decimalCounter++;

            if (!EventActive())
            {

                return;

            }

            if (animations.Count <= 0)
            {

                TargetFace();

            }
            else if (!targetLocation.temporarySprites.Contains(animations.First()))
            {

                animations.Clear();

                TargetFace();

            }

            Rectangle box = victim.GetBoundingBox();

            Point center = box.Center;

            animations[0].Position = center.ToVector2() - new Vector2(32, 128);

            animations[0].alpha = 0f + decimalCounter * 0.025f;

            animations[0].reset();

        }

        public override void EventInterval()
        {

            activeCounter++;

            /*if (activeCounter == 4)
            {

                expireTime += 120;

                Vector2 spawnAt = victim.Tile;

                List<int> spawnIndex = new() { 51, 52, };

                if (victim.Sprite.SpriteWidth > 16)
                {

                    spawnIndex = new() { 53, 54, };

                }

                if (victim.Sprite.SpriteWidth > 32)
                {

                    spawnIndex = new() { 55, 56, };

                }

                victim.currentLocation.characters.Remove(victim);

                victim = null;

                monsterHandle = new(spawnAt, Mod.instance.rite.castLocation);

                monsterHandle.spawnIndex = spawnIndex;

                monsterHandle.SpawnGround(spawnAt);

            }*/

        }

        public void TargetFace()
        {

            Rectangle box = victim.GetBoundingBox();

            Point center = box.Center;

            TemporaryAnimatedSprite warpTarget = new(0, 100f, 1, 1, center.ToVector2() - new Vector2(32, 128), false, false)
            {

                sourceRect = new(0, 0, 32, 32),

                sourceRectStartingPos = new Vector2(0, 0),

                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Polymorph.png")),

                scale = 2f,

                layerDepth = 999f,

                alpha = 0.4f + decimalCounter * 0.01f,

            };

            targetLocation.temporarySprites.Add(warpTarget);

            animations.Add(warpTarget);

        }

    }

}
