/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewDruid.Cast;
using System.Linq;
using System.Collections.Generic;
using static StardewValley.AnimatedSprite;

namespace StardewDruid.Character
{
    public class Jester : StardewDruid.Character.Character
    {

        //public Dictionary<Vector3, FarmerSprite.AnimationFrame> flipFrames;

        //public Vector3 flipCurrent;


        public Jester()
            : base()
        {

        }

        public Jester(Vector2 position, string map)
            : base(position, map, "Jester")
        {

            //flipFrames = new();

            //flipCurrent = new(0);
        
        }

        public override Microsoft.Xna.Framework.Rectangle GetBoundingBox()
        {
            
            Vector2 vector = Position;

            return new Microsoft.Xna.Framework.Rectangle((int)vector.X + 8, (int)vector.Y + 16, 48, 32);

        }


        public override void update(GameTime time, GameLocation location)
        {

            base.update(time, location);

            if (timers.ContainsKey("stop"))
            {

                return;

            }

            if((timers.ContainsKey("sprint") || timers.ContainsKey("attack")) && Sprite.currentFrame < 24)
            {

                Sprite.currentFrame += 24;

                Sprite.interval = 125f;

                Sprite.UpdateSourceRect();

            }
            else
            {

                Sprite.interval = 175f;

            }

            /*if (moveDirection % 2 == 0)
            {
                
                if (altDirection == 3)
                {
                    
                    if(Sprite.currentFrame < 6)
                    {

                        Sprite.currentFrame += 48;

                    }
                    else if (Sprite.currentFrame < 18)
                    {

                        Sprite.currentFrame += 42;

                    }
                    else if (Sprite.currentFrame < 30)
                    {

                        Sprite.currentFrame += 36;

                    }
                    else if (Sprite.currentFrame < 42)
                    {

                        Sprite.currentFrame += 30;

                    }

                    Sprite.UpdateSourceRect();

                }

            }*/

        }

        /*public void AdjustFrame(int back)
        {

            if (altDirection == 3)
            {

                Vector3 flipCoordinate = new(back, 3, Sprite.currentAnimationIndex);

                if (flipCurrent == flipCoordinate)
                {

                    return;

                }

                if (flipFrames.ContainsKey(flipCoordinate))
                {

                    Sprite.CurrentAnimation[Sprite.currentAnimationIndex] = flipFrames[flipCoordinate];

                    return;

                }

                FarmerSprite.AnimationFrame currentFrame = Sprite.CurrentAnimation[Sprite.currentAnimationIndex];

                FarmerSprite.AnimationFrame flipFrame = new(
                    currentFrame.frame,
                    currentFrame.milliseconds,
                    currentFrame.positionOffset,
                    currentFrame.secondaryArm,
                    true,
                    currentFrame.frameStartBehavior,
                    currentFrame.frameEndBehavior,
                    currentFrame.xOffset

                );

                Sprite.CurrentAnimation[Sprite.currentAnimationIndex] = flipFrame;

                flipFrames[flipCoordinate] = flipFrame;

                Vector3 backCoordinate = new(back, 1, Sprite.currentAnimationIndex);

                flipFrames[backCoordinate] = currentFrame;

                return;

            }


            if (altDirection == 1)
            {
                
                Vector3 flipCoordinate = new(back, 3, Sprite.currentAnimationIndex);

                if(flipCurrent == flipCoordinate)
                {

                    Vector3 backCoordinate = new(back, 1, Sprite.currentAnimationIndex);

                    Sprite.CurrentAnimation[Sprite.currentAnimationIndex] = flipFrames[backCoordinate];

                    flipCurrent = backCoordinate;

                }

            }

        }*/


        public override void AnimateMovement(GameTime time)
        {

            if (timers.ContainsKey("attack"))
            {
                
                if(targetOpponents.Count > 0)
                {
                    
                    if (Vector2.Distance(Position, targetOpponents.First().Position) <= 96f)
                    {

                        if (Sprite.CurrentFrame % 6 == 2)
                        {

                            return;

                        }

                    }

                }

            }

            base.AnimateMovement(time);

        }

        public override bool checkAction(Farmer who, GameLocation l)
        {
            
            if(!base.checkAction(who, l))
            {

                return false;

            }

            if (!Mod.instance.dialogue.ContainsKey("Jester"))
            {

                Mod.instance.dialogue["Jester"] = new Dialogue.Jester() { npc = this };

            }

            Mod.instance.dialogue["Jester"].DialogueApproach();

            return true;

        }

        public override void DealDamageToMonster(StardewValley.Monsters.Monster monster)
        {

            base.DealDamageToMonster(monster);

            List<int> daze = new();

            for (int i = 0; i < 5; i++)
            {

                string eventName = "daze" + i.ToString();

                if (!Mod.instance.eventRegister.ContainsKey(eventName))
                {
                    
                    daze.Add(i);

                }
                else if ((Mod.instance.eventRegister[eventName] as Event.World.Daze).victim == monster)
                {

                    return;

                }

            }

            if(daze.Count > 0)
            {

                Rite jesterRite = Mod.instance.NewRite(false);

                Event.World.Daze dazeEvent = new(getTileLocation(), jesterRite, monster, daze.First());

                monster.Halt();

                monster.stunTime = 4000;

                dazeEvent.EventTrigger();

            }

        }

        public override void SwitchFollowMode()
        {

            base.SwitchFollowMode();

            Buff luckBuff = new("Fortune's Favour", 999999, "The Jester Of Fate", 4);

            luckBuff.buffAttributes[4] = 2;

            luckBuff.which = 184654;

            if (!Game1.buffsDisplay.hasBuff(184654))
            {

                Game1.buffsDisplay.addOtherBuff(luckBuff);

            }

        }

        public override void SwitchDefaultMode()
        {

            base.SwitchDefaultMode();

            foreach (Buff buff in Game1.buffsDisplay.otherBuffs)
            {
                
                if(buff.which == 184654)
                {
                    buff.removeBuff();

                }

            }

        }

    }

}
