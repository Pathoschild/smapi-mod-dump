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

namespace StardewDruid.Event.World
{
    public class Escape : EventHandle
    {

        public Dictionary<int, TemporaryAnimatedSprite> escapeAnimations;

        public Vector2 escapeCorner;

        public Vector2 escapeAnchor;

        public Escape(Vector2 target, Rite rite)
            : base(target, rite)
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

            if (Vector2.Distance(targetVector, riteData.caster.Position) <= 32 && Mod.instance.activeData.castLevel > activeCounter)
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

                TemporaryAnimatedSprite startAnimation = new(0, 1000f, 1, 1, escapeAnchor, false, false)
                {

                    sourceRect = new(0, 0, 64, 64),

                    sourceRectStartingPos = new Vector2(0, 0),

                    texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Portal.png")),

                    scale = 1f,

                    scaleChange = 0.002f,

                    motion = new(-0.064f, -0.064f),

                    layerDepth = 0.0002f,

                    timeBasedMotion = true,

                    rotationChange = -0.06f,

                };

                targetLocation.temporarySprites.Add(startAnimation);

                escapeAnimations[0] = startAnimation;

                TemporaryAnimatedSprite startNightAnimation = new(0, 1000f, 1, 1, escapeAnchor, false, false)
                {

                    sourceRect = new(0, 0, 64, 64),

                    sourceRectStartingPos = new Vector2(0, 0),

                    texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Nightsky.png")),

                    scale = 1f,

                    scaleChange = 0.002f,

                    motion = new(-0.064f, -0.064f),

                    layerDepth = 0.0001f,

                    timeBasedMotion = true,

                };

                targetLocation.temporarySprites.Add(startNightAnimation);

                escapeAnimations[1] = startNightAnimation;


                TemporaryAnimatedSprite staticAnimation = new(0, 1000f, 1, 1, escapeCorner, false, false)
                {

                    sourceRect = new(0, 0, 64, 64),

                    sourceRectStartingPos = new Vector2(0, 0),

                    texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Portal.png")),

                    scale = 3f,

                    layerDepth = 0.0002f,

                    timeBasedMotion = true,

                    rotationChange = -0.06f,

                    delayBeforeAnimationStart = 1000,

                };

                targetLocation.temporarySprites.Add(staticAnimation);

                escapeAnimations[2] = staticAnimation;

                TemporaryAnimatedSprite staticNightAnimation = new(0, 1000f, 1, 1, escapeCorner, false, false)
                {

                    sourceRect = new(0, 0, 64, 64),

                    sourceRectStartingPos = new Vector2(0, 0),

                    texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Nightsky.png")),

                    scale = 3f,

                    layerDepth = 0.0001f,

                    timeBasedMotion = true,

                    delayBeforeAnimationStart = 1000,

                };

                targetLocation.temporarySprites.Add(staticNightAnimation);

                escapeAnimations[3] = staticNightAnimation;

            }

            if (activeCounter == 5)
            {
                if (targetLocation is MineShaft)
                {

                    if (PerformXzone())
                    {

                        Mod.instance.AbortAllEvents();

                    }

                }
                else
                {
                    if (PerformEscape())
                    {

                        Mod.instance.AbortAllEvents();

                    }

                }

            }

        }

        public bool PerformEscape()
        {

            List<Vector2> destinations = new();

            float newDistance;

            float furthestDistance = 0f;

            List<string> surveyed = new();

            foreach (Warp warp in targetLocation.warps)
            {

                if (surveyed.Contains(warp.TargetName))
                {

                    continue;

                }
                
                surveyed.Add(warp.TargetName);

                Vector2 destination;

                if (WarpData.WarpExclusions(targetLocation, warp))
                {

                    destination = WarpData.WarpVectors(targetLocation);

                    if (destination == Vector2.Zero)
                    {

                        continue;

                    }

                }
                else
                {

                    destination = WarpData.WarpReverse(targetLocation, warp);

                    if (destination == Vector2.Zero)
                    {

                        continue;

                    }

                }

                Vector2 possibility = destination * 64;

                if (destinations.Count == 0)
                {

                    destinations.Add(possibility);

                    furthestDistance = Vector2.Distance(targetVector, possibility);

                }
                else
                {

                    newDistance = Vector2.Distance(targetVector, possibility);

                    if (riteData.caster.getGeneralDirectionTowards(possibility, 0, false, false) == riteData.caster.facingDirection && newDistance > furthestDistance)
                    {

                        destinations.Clear();

                        destinations.Add(possibility);

                    }

                }

            }

            if (destinations.Count > 0)
            {
                Game1.flashAlpha = 1;
                
                riteData.caster.Position = destinations[0];

                ModUtility.AnimateQuickWarp(targetLocation, destinations[0], "Escape");

                return true;

            }

            return false;

        }

        public bool PerformXzone()
        {

            Type reflectType = typeof(MineShaft);

            FieldInfo reflectField = reflectType.GetField("netTileBeneathLadder", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var tile = reflectField.GetValue((targetLocation as MineShaft));

            if (tile == null)
            {

                return false;
            }

            string tileString = tile.ToString();

            Match m = Regex.Match(tileString, @"\{*X\:(\d+)\sY\:(\d+)\}", RegexOptions.IgnoreCase);

            if (!m.Success)
            {

                return false;

            }

            int tileX = Convert.ToInt32(m.Groups[1].Value);

            int tileY = Convert.ToInt32(m.Groups[2].Value);

            Vector2 destination = new Vector2(tileX, tileY) * 64;

            Game1.flashAlpha = 1;

            riteData.caster.Position = destination;

            ModUtility.AnimateQuickWarp(targetLocation, destination, "Escape");

            return true;

        }

    }

}
