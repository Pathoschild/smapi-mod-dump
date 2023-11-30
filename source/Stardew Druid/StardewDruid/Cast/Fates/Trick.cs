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
using StardewDruid.Cast.Earth;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

namespace StardewDruid.Cast.Fates
{
    internal class Trick : CastHandle
    {

        NPC riteWitness;

        int source;

        int friendship;

        public Trick(Vector2 target, Rite rite, NPC witness, int Source)
            : base(target, rite)
        {

            riteWitness = witness;

            source = Source;

        }

        public override void CastEffect()
        {

            riteWitness.faceTowardFarmerForPeriod(3000, 4, false, targetPlayer);

            riteWitness.doEmote(8);

            DelayedAction.functionAfterDelay(CastAfterDelay, 1000);

            ModUtility.AnimateFate(targetLocation, targetPlayer.getTileLocation(), riteWitness.getTileLocation(), source);

            castFire = true;

        }

        public void CastAfterDelay()
        {

            friendship = 100;

            int friendshipRatio = 3;

            if (!riteData.castTask.ContainsKey("masterTrick"))
            {

                Mod.instance.UpdateTask("lessonTrick", 1);

            }
            else
            {

                friendshipRatio = 5;

            }

            int reaction = 500;

            switch (source)
            {
                case 768:

                    switch (randomIndex.Next(3))
                    {

                        case 0:

                            ModUtility.AnimateMeteor(targetLocation, riteWitness.getTileLocation() - new Vector2(0,1), true);

                            DelayedAction.functionAfterDelay(DeathSpray, 200);

                            friendship = -10;

                            break;

                        case 1:

                            ModUtility.AnimateRandomCritter(targetLocation, riteWitness.getTileLocation());

                            friendship = randomIndex.Next(1,friendshipRatio) * 25;

                            break;

                        default:

                            ModUtility.AnimateButterflySpray(targetLocation, riteWitness.getTileLocation());

                            friendship = randomIndex.Next(1,friendshipRatio) * 25;

                            break;
                    
                    }
                    
                    break;

                default:

                    switch (randomIndex.Next(3))
                    {

                        case 0:

                            ModUtility.AnimateBolt(targetLocation, riteWitness.getTileLocation() - new Vector2(0, 1));

                            DelayedAction.functionAfterDelay(DeathSpray, 200);

                            friendship = -10;

                            break;

                        case 1:

                            ModUtility.AnimateRandomCritter(targetLocation, riteWitness.getTileLocation());

                            friendship = randomIndex.Next(1, friendshipRatio) * 25;

                            break;

                        default:

                            ModUtility.AnimateRandomFish(targetLocation, riteWitness.getTileLocation());

                            friendship = randomIndex.Next(1, friendshipRatio) * 25;

                            break;
                    }
                    
                    break;
            
            }

            DelayedAction.functionAfterDelay(VillagerReaction, reaction);

        }

        public void VillagerReaction()
        {

            if(friendship >= 75)
            {

                riteWitness.doEmote(20);

            }
            else if (friendship >= 50)
            {

                riteWitness.doEmote(32);

            }
            else if (friendship >= 25)
            {

                riteWitness.doEmote(24);

            }
            else
            {

                riteWitness.doEmote(28);

            }

            ModUtility.GreetVillager(targetPlayer, riteWitness, friendship);

        }

        public void DeathSpray()
        {

            ModUtility.AnimateDeathSpray(targetLocation, riteWitness.GetBoundingBox().Center.ToVector2(), Color.Gray * 0.5f);

        }

    }

}
