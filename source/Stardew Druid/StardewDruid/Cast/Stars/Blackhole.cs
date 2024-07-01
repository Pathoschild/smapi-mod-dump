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
using StardewDruid.Cast.Effect;
using StardewDruid.Data;
using StardewDruid.Event;
using StardewDruid.Journal;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Minecarts;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using xTile.Tiles;
using static StardewDruid.Data.IconData;

namespace StardewDruid.Cast.Stars
{
    public class Blackhole : EventHandle
    {

        public int channelCounter;

        public Vector2 target;

        public bool blackhole;

        public bool meteor;

        public Blackhole()
        {
            
        }

        public virtual void GravitySetup(Vector2 origin, string id, Vector2 Target)
        {
            
            base.EventSetup(origin, id);

            target = Target;

        }

        public override bool EventActive()
        {

            if (!inabsentia && !eventLocked)
            {

                if (Mod.instance.Config.riteButtons.GetState() != SButtonState.Held)
                {

                    return false;

                }

                if (Vector2.Distance(origin, Game1.player.Position) > 32)
                {

                    return false;

                }

            }
            
            return base.EventActive();

        }

        public override void EventDecimal()
        {

            if (!EventActive())
            {

                RemoveAnimations();

                return;

            }

            if (!inabsentia && !eventLocked)
            {

                channelCounter++;

                if (channelCounter == 5)
                {

                    TemporaryAnimatedSprite skyAnimation = Mod.instance.iconData.SkyIndicator(location, origin, IconData.skies.night, 1f, new() { interval = 1000,});

                    skyAnimation.scaleChange = 0.002f;

                    skyAnimation.motion = new(-0.064f, -0.064f);

                    skyAnimation.timeBasedMotion = true;

                    animations.Add(skyAnimation);

                    TemporaryAnimatedSprite startAnimation = new(0, 1000f, 1, 1, target + new Vector2(32, 32), false, false)
                    {

                        sourceRect = new(0, 0, 64, 64),

                        sourceRectStartingPos = new Vector2(0, 0),

                        texture = Mod.instance.iconData.gravityTexture,

                        scale = 0.001f,

                        scaleChange = 0.003f,

                        layerDepth = location.IsOutdoors ? target.Y / 10000 + 0.001f : 999f,

                        //motion = (impact - origin) / 1000 - new Vector2(96, 96) / 1000,

                        motion = new Vector2(-96, -96) / 1000,

                        timeBasedMotion = true,

                        rotationChange = -0.06f,

                        alpha = 0.75f,

                    };

                    location.temporarySprites.Add(startAnimation);

                    animations.Add(startAnimation);

                }

                if (channelCounter == 14)
                {

                    eventLocked = true;

                }

                return;

            }

            decimalCounter++;

            if(decimalCounter == 1)
            {

                GravityWell();

            }

            if(decimalCounter == 30)
            {

                eventComplete = true;

            }

        }

        public void GravityWell()
        {

            if (!Mod.instance.questHandle.IsComplete(QuestHandle.starsTwo))
            {

                Mod.instance.questHandle.UpdateTask(QuestHandle.starsTwo, 1);
            }

            SpellHandle hole = new(Game1.player, target, 5 * 64, Mod.instance.CombatDamage() * 4);

            hole.type = SpellHandle.spells.blackhole;

            if (location.IsFarm || location.IsGreenhouse || location is Shed)
            {

                hole.added = new() { SpellHandle.effects.harvest, };

            }
            else
            {

                hole.added = new() { SpellHandle.effects.gravity, };

            }

            Mod.instance.spellRegister.Add(hole);

            blackhole = true;

        }

    }

}
