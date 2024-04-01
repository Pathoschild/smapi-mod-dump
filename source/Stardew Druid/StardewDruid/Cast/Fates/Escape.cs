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
using StardewDruid.Map;
using StardewValley;
using StardewValley.GameData.Minecarts;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace StardewDruid.Cast.Fates
{
    public class Escape : EventHandle
    {

        public Dictionary<int, TemporaryAnimatedSprite> escapeAnimations;

        public Vector2 escapeCorner;

        public Vector2 escapeAnchor;

        public Escape(Vector2 target)
            : base(target)
        {

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 6;

            escapeAnimations = new();

            escapeAnchor = targetVector;

            escapeCorner = escapeAnchor - new Vector2(64, 64);

        }

        public override void EventTrigger()
        {

            Mod.instance.RegisterEvent(this, "escape");

        }

        public override bool EventActive()
        {

            if (expireEarly)
            {

                return false;

            }

            if (targetPlayer.currentLocation.Name != targetLocation.Name)
            {

                return false;

            }

            if (expireTime < Game1.currentGameTime.TotalGameTime.TotalSeconds)
            {

                return false;

            }

            return true;

        }

        public override void EventRemove()
        {
            if (escapeAnimations.Count > 0)
            {

                foreach (KeyValuePair<int, TemporaryAnimatedSprite> animation in escapeAnimations)
                {

                    targetLocation.temporarySprites.Remove(animation.Value);

                }

            }

            escapeAnimations.Clear();

        }

        public override void EventInterval()
        {

            if (Vector2.Distance(targetVector, Mod.instance.rite.caster.Position) <= 32 && Mod.instance.rite.castLevel > activeCounter)
            {

                activeCounter++;

            }
            else
            {

                EventRemove();

                expireEarly = true;

                return;

            }

            if (activeCounter == 3)
            {

                TemporaryAnimatedSprite startNightAnimation = new(0, 2000f, 1, 1, escapeAnchor, false, false)
                {

                    sourceRect = new(0, 0, 64, 64),

                    sourceRectStartingPos = new Vector2(0, 0),

                    texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Nightsky.png")),

                    scale = 1f,

                    scaleChange = 0.002f,

                    motion = new(-0.064f, -0.064f),

                    layerDepth = 0.0001f,

                    alpha = 0.75f,

                    timeBasedMotion = true,

                };

                targetLocation.temporarySprites.Add(startNightAnimation);

                escapeAnimations[1] = startNightAnimation;

            }

            if(activeCounter == 4)
            {

                escapeAnimations[1].scaleChange = 0;
                escapeAnimations[1].motion = Vector2.Zero;

            }

            if (activeCounter == 5)
            {

                Vector2 destination = new Vector2(-1);

                if (targetLocation is MineShaft)
                {

                    destination = WarpData.WarpXZone(targetLocation,targetVector);

                    if (destination != new Vector2(-1))
                    {

                        Game1.flashAlpha = 1;

                        Mod.instance.rite.caster.Position = destination;

                        Mod.instance.AbortAllEvents();

                    }

                }
                else
                {
                    destination = WarpData.WarpEntrance(targetLocation, targetVector);

                    if (destination != new Vector2(-1))
                    {
                        Game1.flashAlpha = 1;

                        Mod.instance.rite.caster.Position = destination;

                        Mod.instance.AbortAllEvents();

                    }

                }

            }

        }

    }

}
