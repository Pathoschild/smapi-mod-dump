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
using StardewDruid.Cast;
using StardewDruid.Map;
using StardewValley;
using StardewValley.Tools;
using System.Collections.Generic;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace StardewDruid.Event
{
    public class FishSpot : ChallengeHandle
    {

        public int fishCounter;

        public FishSpot(Mod Mod, Vector2 target, Rite rite, Quest quest)
            : base(Mod, target, rite, quest)
        {

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 300;

        }

        public override void EventTrigger()
        {

            ModUtility.AnimateBolt(targetLocation, targetVector);

            Utility.addSprinklesToLocation(targetLocation, (int)targetVector.X, (int)targetVector.Y, 3, 3, 999, 333, Color.White);

            mod.RegisterChallenge(this,"inactive");

        }

        public override bool EventActive()
        {

            if (expireTime >= Game1.currentGameTime.TotalGameTime.TotalSeconds && targetPlayer.currentLocation == targetLocation)
            {

                return true;

            }

            return false;

        }

        public override void EventAbort()
        {

        }

        public override void EventRemove()
        {

        }

        public override void EventInterval()
        {

            if (Game1.player.CurrentTool == null)
            {

                return;

            };

            fishCounter--;

            if (fishCounter <= 0)
            {

                ModUtility.AnimateFishJump(targetLocation, targetVector);

                Utility.addSprinklesToLocation(targetLocation, (int)targetVector.X, (int)targetVector.Y, 3, 3, 5000, 500, Color.White * 0.75f);

                fishCounter = 5;

            }

            if (Game1.player.CurrentTool.GetType().Name != "FishingRod")
            {
                return;

            }

            FishingRod fishingRod = Game1.player.CurrentTool as FishingRod;

            if (!fishingRod.isFishing)
            {

                return;

            }

            Vector2 portalPosition = new(targetVector.X * 64, targetVector.Y * 64);

            int checkTime = 4000; // check bait

            if (fishingRod.attachments[0] != null)
            {
                checkTime = 2000;

            };

            if (fishingRod.fishingBiteAccumulator <= checkTime)
            {

                return;

            }

            Vector2 bobberPosition = fishingRod.bobber;

            Microsoft.Xna.Framework.Rectangle splashRectangle = new((int)portalPosition.X - 56, (int)portalPosition.Y - 56, 176, 176);

            Microsoft.Xna.Framework.Rectangle bobberRectangle = new((int)bobberPosition.X, (int)bobberPosition.Y, 64, 64);

            if (bobberRectangle.Intersects(splashRectangle))
            {

                fishingRod.fishingBiteAccumulator = 0f;
                fishingRod.timeUntilFishingBite = -1f;
                fishingRod.isNibbling = true;
                fishingRod.timePerBobberBob = 1f;
                fishingRod.timeUntilFishingNibbleDone = FishingRod.maxTimeToNibble;
                fishingRod.hit = true;

                bool enableRare = false;

                if (!riteData.castTask.ContainsKey("masterFishspot"))
                {

                    mod.UpdateTask("lessonFishspot", 1);

                }
                else
                {

                    enableRare = true;

                }

                int objectIndex = SpawnData.RandomHighFish(targetLocation, enableRare);

                int animationRow = 10;

                Microsoft.Xna.Framework.Rectangle animationRectangle = new(0, animationRow * 64, 64, 64);

                float animationInterval = 100f;

                int animationLength = 8;

                int animationLoops = 1;

                Color animationColor = new(0.6f, 1, 0.6f, 1); // light green

                float animationSort = (targetVector.X * 1000) + targetVector.Y + 1;

                TemporaryAnimatedSprite newAnimation = new("TileSheets\\animations", animationRectangle, animationInterval, animationLength, animationLoops, portalPosition, false, false, animationSort, 0f, animationColor, 1f, 0f, 0f, 0f)
                {
                    endFunction = fishingRod.startMinigameEndFunction,
                    extraInfoForEndBehavior = objectIndex,
                    id = animationSort
                };

                Game1.player.currentLocation.temporarySprites.Add(newAnimation);

                Game1.player.currentLocation.playSound("squid_bubble");

                DelayedAction.playSoundAfterDelay("FishHit", 800, Game1.player.currentLocation);

            }

            return;

        }
    }
}
