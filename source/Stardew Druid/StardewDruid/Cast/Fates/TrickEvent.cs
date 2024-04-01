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
using StardewValley;
using System.Collections.Generic;
using System;
using xTile.Dimensions;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using StardewDruid.Event;

namespace StardewDruid.Cast.Fates
{
    internal class TrickEvent : EventHandle
    {

        public NPC riteWitness;

        public int friendship;

        public string trick;

        public int decimalCounter;

        public int deathCounter;

        public int reactionCounter;

        public TrickEvent(Vector2 target,  NPC witness)
            : base(target)
        {

            riteWitness = witness;

            friendship = 100;

            trick = "butterflies";

            reactionCounter = 16;

        }

        public override void EventTrigger()
        {

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 2;

            Mod.instance.RegisterEvent(this,"trick"+riteWitness.Name);

            riteWitness.faceTowardFarmerForPeriod(3000, 4, false, targetPlayer);

            riteWitness.doEmote(8);

        }

        public override bool EventActive()
        {

            if (riteWitness == null || riteWitness.currentLocation.Name != targetLocation.Name) { return false; }

            return base.EventActive();

        }

        public override void EventDecimal()
        {

            if (!EventActive())
            {

                return;

            }

            decimalCounter++;

            if(decimalCounter == 1)
            {

                SpellHandle whiskSpell = new(targetLocation, targetPlayer.Position, riteWitness.Position,3);

                whiskSpell.scheme = SpellHandle.schemes.fates;

                whiskSpell.indicator = SpellHandle.indicators.fates;

                whiskSpell.TargetCircle();

                whiskSpell.LaunchMissile();

                animations = whiskSpell.animations;

            }

            if(decimalCounter == 11)
            {

                int friendshipRatio = 3;

                if (!Mod.instance.rite.castTask.ContainsKey("masterTrick"))
                {

                    Mod.instance.UpdateTask("lessonTrick", 1);

                }
                else
                {

                    friendshipRatio = 5;

                }

                int interval = 2000;

                int trickInt = randomIndex.Next(6);

                switch (trickInt)
                {

                    case 0:

                        SpellHandle meteor = new(targetLocation, riteWitness.Position - new Vector2(0, 32), targetPlayer.Position);

                        meteor.type = SpellHandle.barrages.meteor;

                        Mod.instance.spellRegister.Add(meteor);

                        friendship = -10;

                        trick = "meteors";

                        deathCounter = 13;

                        break;

                    case 1:

                        SpellHandle bolt = new(targetLocation, riteWitness.Position - new Vector2(0, 32), targetPlayer.Position);

                        bolt.type = SpellHandle.barrages.bolt;

                        Mod.instance.spellRegister.Add(bolt);

                        trick = "bolts of lightning";

                        friendship = -10;

                        deathCounter = 13;

                        break;

                    case 2:

                        ModUtility.AnimateRandomCritter(targetLocation, riteWitness.Tile);

                        friendship = randomIndex.Next(1, friendshipRatio) * 25;

                        trick = "critters";

                        break;

                    case 3:

                        Levitate levitation = new(targetVector, riteWitness);

                        levitation.EventTrigger();

                        friendship = randomIndex.Next(1, friendshipRatio) * 25;

                        trick = "levitations";

                        break;

                    case 4:

                        ModUtility.AnimateRandomFish(targetLocation, riteWitness.Tile);

                        friendship = randomIndex.Next(1, friendshipRatio) * 25;

                        trick = "fishies";

                        break;

                    default:

                        ModUtility.AnimateButterflySpray(targetLocation, riteWitness.Tile);

                        friendship = randomIndex.Next(1, friendshipRatio) * 25;

                        break;
                }


                TemporaryAnimatedSprite radiusAnimation = new(0, interval, 1, 1, riteWitness.Position - new Microsoft.Xna.Framework.Vector2(64, 64), false, false)
                {

                    sourceRect = new(192, 0, 64, 64),

                    sourceRectStartingPos = new Vector2(192, 0),

                    texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Decorations.png")),

                    scale = 3f, //* size,

                    timeBasedMotion = true,

                    layerDepth = 0.0001f,

                    rotationChange = 0.06f,

                };

                targetLocation.temporarySprites.Add(radiusAnimation);

                animations.Add(radiusAnimation);

            }

            if(decimalCounter == reactionCounter)
            {

                ModUtility.GreetVillager(targetPlayer, riteWitness, friendship);

                List<string> context = new() { trick, };

                StardewDruid.Dialogue.Reaction.ReactTo(riteWitness, "Fates", friendship, context);

            }

            if(decimalCounter == deathCounter)
            {

                ModUtility.AnimateDeathSpray(targetLocation, riteWitness.Position, Color.Gray * 0.5f);

            }

        }


    }

}
