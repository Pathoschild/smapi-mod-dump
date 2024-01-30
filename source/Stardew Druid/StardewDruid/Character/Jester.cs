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
using StardewDruid.Event.World;
using StardewDruid.Map;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StardewDruid.Character
{
    public class Jester : StardewDruid.Character.Character
    {

        public Jester()
        {
        }

        public Jester(Vector2 position, string map)
          : base(position, map, nameof(Jester))
        {
            
        }

        public override void LoadOut()
        {

            barrages = new();

            roamVectors = new List<Vector2>();

            eventVectors = new List<Vector2>();

            targetVectors = new();

            opponentThreshold = 640;

            gait = 1.2f;

            modeActive = mode.random;

            behaviourActive = behaviour.idle;

            idleInterval = 90;

            moveLength = 6;

            moveInterval = 12;

            specialInterval = 30;

            walkFrames = WalkFrames(32, 32);

            dashFrames = WalkFrames(32, 32, 0, 128);

            haltFrames = new()
            {
                [0] = new(0, 256, 32, 32),

                [1] = new(32, 256, 32, 32),

                [2] = new(64, 256, 32, 32),

                [3] = new(96, 256, 32, 32),

                [4] = new(128, 256, 32, 32),

                [5] = new(160, 256, 32, 32),

            };

            specialFrames = new()
            {

                [0] = new(128, 192, 32, 32),

                [1] = new(128, 160, 32, 32),

                [2] = new(128, 128, 32, 32),

                [3] = new(128, 224, 32, 32),

            };

            loadedOut = true;

        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {

            if (IsInvisible || !Utility.isOnScreen(Position, 128))
            {
                return;
            }

            if (base.IsEmoting && !Game1.eventUp)
            {
                Vector2 localPosition2 = getLocalPosition(Game1.viewport);
                localPosition2.Y -= 32 + Sprite.SpriteHeight * 4;
                b.Draw(Game1.emoteSpriteSheet, localPosition2, new Microsoft.Xna.Framework.Rectangle(base.CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, base.CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, getStandingY() / 10000f);
            }

            Vector2 localPosition = getLocalPosition(Game1.viewport);

            b.Draw(
                Game1.shadowTexture,
                localPosition +new Vector2(6, 44f),
                Game1.shadowTexture.Bounds,
                Color.White * alpha, 0f,
                Vector2.Zero,
                4f,
                SpriteEffects.None,
                Math.Max(0.0f, getStandingY() / 10000f) - 0.0001f
                );

            if (netHaltActive.Value)
            {

                int chooseFrame = idleFrame.Value % 8;

                Rectangle sourceRectangle = walkFrames[netDirection.Value][0];

                if(chooseFrame > 1)
                {

                    sourceRectangle = haltFrames[chooseFrame - 2];

                }

                b.Draw(
                    Sprite.Texture,
                    localPosition - new Vector2(32, 64f),
                    sourceRectangle,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    flip || (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    Math.Max(0f, drawOnTop ? 0.991f : (getStandingY() / 10000f))
                );

            }
            else if (netDashActive.Value)
            {

                b.Draw(
                    Sprite.Texture,
                    localPosition - new Vector2(32, 64f),
                    dashFrames[netDirection.Value][moveFrame.Value],
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    flip || (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    Math.Max(0f, drawOnTop ? 0.991f : (getStandingY() / 10000f))
                );

            }
            else if (netSpecialActive.Value)
            {

                b.Draw(
                    Sprite.Texture,
                    localPosition - new Vector2(32, 64f),
                    new Rectangle?(specialFrames[netDirection.Value]), 
                    Color.White, 
                    0.0f,
                    Vector2.Zero,
                    4f,
                    flip || (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, 
                    Math.Max(0f, drawOnTop ? 0.991f : (getStandingY() / 10000f))
                );

            }
            else
            {
                b.Draw(
                    Sprite.Texture,
                    localPosition - new Vector2(32, 64f),
                    walkFrames[netDirection.Value][moveFrame.Value],
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    flip || (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    Math.Max(0f, drawOnTop ? 0.991f : (getStandingY() / 10000f))
                );

            }

        }

        public override Rectangle GetBoundingBox()
        {

            if(netDirection.Value % 2 == 0)
            {

                return new Rectangle((int)Position.X + 8, (int)Position.Y, 48, 64);

            }

            return new Rectangle((int)Position.X - 16, (int)Position.Y, 96, 64);

        }

        public override void UpdateMove()
        {

            base.UpdateMove();

            if (netDashActive.Value)
            {

                float distance = Vector2.Distance(Position, targetVectors.First());

                if (distance < 320 && moveFrame.Value > 2)
                {

                    moveFrame.Set(2);

                }

            }

        }

        public override bool MonsterAttack(StardewValley.Monsters.Monster targetMonster)
        {

            float distance = Vector2.Distance(Position, targetMonster.Position);

            if (distance >= 128f && distance <= 640f)
            {

                Vector2 vector2 = new(targetMonster.Position.X - Position.X - 32f, targetMonster.Position.Y - Position.Y);//Vector2.op_Subtraction(((StardewValley.Character)targetOpponents.First<Monster>()).Position, Vector2.op_Addition(Position, new Vector2(32f, 0.0f)));

                if ((double)Math.Abs(vector2.Y) <= 128.0)
                {
                    
                    netSpecialActive.Set(true);

                    behaviourActive = behaviour.special;

                    specialTimer = 60;

                    NextTarget(targetMonster.Position, -1);

                    ResetAll();

                    BarrageHandle beam = new(currentLocation, targetMonster.getTileLocation(), getTileLocation(), 2, 1, "Blue", -1, Mod.instance.DamageLevel());

                    beam.type = BarrageHandle.barrageType.beam;

                    barrages.Add(beam);

                }
                else
                {


                    behaviourActive = behaviour.dash;

                    moveTimer = moveInterval;

                    netDashActive.Set(true);

                    NextTarget(targetMonster.Position, -1);

                }

                return true;

            }

            return false;

        }

        public override bool checkAction(Farmer who, GameLocation l)
        {
            if (!base.checkAction(who, l))
            {
                return false;
            }

            if (!Mod.instance.dialogue.ContainsKey(nameof(Jester)))
            {
               
                Dictionary<string, StardewDruid.Dialogue.Dialogue> dialogue = Mod.instance.dialogue;

                StardewDruid.Dialogue.Jester jester = new StardewDruid.Dialogue.Jester();

                jester.npc = this;

                dialogue[nameof(Jester)] = jester;

            }

            Mod.instance.dialogue[nameof(Jester)].DialogueApproach();

            return true;

        }

        public override void HitMonster(StardewValley.Monsters.Monster monsterCharacter)
        {

            DealDamageToMonster(monsterCharacter,true);

        }

        public override void DealDamageToMonster(StardewValley.Monsters.Monster monster, bool kill = false, int damage = -1, bool push = true)
        {
            
            base.DealDamageToMonster(monster, true, damage, push);
            
            if (Mod.instance.CurrentProgress() < 25)
            {
            
                return;
            
            }
                
            ApplyDazeEffect(monster);

        }

        public void ApplyDazeEffect(StardewValley.Monsters.Monster monster)
        {
            if (Mod.instance.eventRegister.ContainsKey("Gravity"))
                return;
            List<int> source = new List<int>();
            for (int index = 0; index < 5; ++index)
            {
                string key = "daze" + index.ToString();
                if (!Mod.instance.eventRegister.ContainsKey(key))
                    source.Add(index);
                else if ((Mod.instance.eventRegister[key] as Daze).victim == monster)
                    return;
            }
            if (source.Count <= 0)
                return;
            Rite rite = Mod.instance.NewRite(false);
            Daze daze = new Daze(getTileLocation(), rite, monster, source.First<int>(), 1,Mod.instance.DamageLevel());
            if (!MonsterData.CustomMonsters().Contains(monster.GetType()))
            {
                monster.Halt();
                monster.stunTime = 4000;
            }
            daze.EventTrigger();
        }

        public override void SwitchFollowMode(Farmer follow)
        {
            base.SwitchFollowMode(follow);
            Buff buff = new Buff("Fortune's Favour", 999999, nameof(Jester), 4);
            buff.buffAttributes[4] = 2;
            buff.which = 184654;
            if (Game1.buffsDisplay.hasBuff(184654))
                return;
            Game1.buffsDisplay.addOtherBuff(buff);
        }

        public override void SwitchDefaultMode()
        {
            base.SwitchDefaultMode();
            foreach (Buff otherBuff in Game1.buffsDisplay.otherBuffs)
            {
                if (otherBuff.which == 184654)
                    otherBuff.removeBuff();
            }
        }
    }
}
