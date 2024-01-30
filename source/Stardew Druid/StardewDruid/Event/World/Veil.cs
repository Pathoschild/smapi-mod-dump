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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.IO;

namespace StardewDruid.Event.World
{
    public class Veil : EventHandle
    {
        public Dictionary<int, TemporaryAnimatedSprite> veilAnimations;
        public Vector2 veilCorner;
        public Vector2 veilAnchor;

        public Veil(Vector2 target, Rite rite)
          : base(target, rite)
        {
            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 6.0;
            veilAnimations = new Dictionary<int, TemporaryAnimatedSprite>();
            veilAnchor = new Vector2(target.X, target.Y-32f);
            veilCorner = veilAnchor - new Vector2(128f,128f);
        }

        public override void EventTrigger()
        {

            Mod.instance.RegisterEvent(this, "veil");

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
            
            RemoveAnimations();

        }

        public override void RemoveAnimations()
        {
            if (veilAnimations.Count > 0)
            {
                foreach (KeyValuePair<int, TemporaryAnimatedSprite> veilAnimation in veilAnimations)
                    targetLocation.temporarySprites.Remove(veilAnimation.Value);
            }
            veilAnimations.Clear();
        }

        public override void EventInterval()
        {
            
            activeCounter++;

            if (activeCounter == 1)
            {
                TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite(0, 1000f, 1, 1, veilAnchor, false, false)
                {
                    sourceRect = new Rectangle(0, 0, 64, 64),
                    sourceRectStartingPos = new Vector2(0.0f, 0.0f),
                    texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Veil.png")),
                    scaleChange = 4f / 1000f,
                    motion = new Vector2(-0.128f, -0.128f),
                    layerDepth = 999f,
                    timeBasedMotion = true,
                    rotationChange = -0.03f,
                    alpha = 0.2f,
                    color = new Color(0.75f, 0.75f, 1f, 1f)
                };
                targetLocation.temporarySprites.Add(temporaryAnimatedSprite);
                veilAnimations[0] = temporaryAnimatedSprite;

                Vector2 mistCorner = veilCorner + new Vector2(32, 32);

                for (int i = 0; i < 4; i++)
                {

                    for (int j = 0; j < 4; j++)
                    {

                        if ((i == 0 || i == 3) && (j == 0 || j == 3))
                        {
                            continue;
                        }

                        Vector2 glowVector = mistCorner + new Vector2(i * 48, j * 48);

                        TemporaryAnimatedSprite glowSprite = new TemporaryAnimatedSprite(0, 5250f, 1, 1, glowVector, false, false)
                        {
                            sourceRect = new Microsoft.Xna.Framework.Rectangle(88, 1779, 30, 30),
                            sourceRectStartingPos = new Vector2(88, 1779),
                            texture = Game1.mouseCursors,
                            motion = new Vector2(-0.0004f + randomIndex.Next(5) * 0.0002f, -0.0004f + randomIndex.Next(5) * 0.0002f),
                            scale = 4f,
                            layerDepth = 999f,
                            timeBasedMotion = true,
                            alpha = 1f,
                            color = new Color(0.75f, 0.75f, 1f, 1f),
                            delayBeforeAnimationStart = 750,
                        };

                        targetLocation.temporarySprites.Add(glowSprite);

                        veilAnimations[2 + i] = glowSprite;
                    }

                }
            }
            
            if (activeCounter == 2)
            {
                
                TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite(0, 5000f, 1, 1, veilCorner, false, false)
                {
                    sourceRect = new Rectangle(0, 0, 64, 64),
                    sourceRectStartingPos = new Vector2(0.0f, 0.0f),
                    texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Veil.png")),
                    scale = 5f,
                    layerDepth = 999f,
                    timeBasedMotion = true,
                    rotationChange = -0.03f,
                    alpha = 0.2f,
                    color = new Color(0.75f, 0.75f, 1f, 1f)
                };
                targetLocation.temporarySprites.Add(temporaryAnimatedSprite);
                veilAnimations[1] = temporaryAnimatedSprite;
                expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 6.0;


            }
            
            if ((double)Vector2.Distance(targetVector, riteData.caster.Position) > 192.0)
            {
                return;
            }
            
            int num = Math.Min(riteData.caster.maxHealth / 12, riteData.caster.maxHealth - riteData.caster.health);
            
            if(num > 0 )
            {

                riteData.caster.health += num;

                Rectangle healthBox = riteData.caster.GetBoundingBox();

                targetLocation.debris.Add(
                    new Debris(
                        num, 
                        new Vector2(healthBox.Center.X + 16, healthBox.Center.Y), 
                        Color.Green, 
                        1f, 
                        riteData.caster
                    )
                );

            }

            // Mod.instance.CastMessage("Mists grant +" + num.ToString() + " health", 5);

        }
    
    }

}
