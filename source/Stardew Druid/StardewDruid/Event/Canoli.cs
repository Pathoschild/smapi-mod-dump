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
using System;
using System.Collections.Generic;

namespace StardewDruid.Event
{
    public class Canoli : ChallengeHandle
    {

        public Canoli(Mod Mod, Vector2 target, Rite rite, Quest quest)
            : base(Mod, target, rite, quest)
        {

            targetVector = questData.vectorList["targetVector"];

            voicePosition = (targetVector * 64) + new Vector2(-8, -56);

        }

        public override void EventTrigger()
        {

            monsterHandle = new(mod, targetVector + new Vector2(0, 2), riteData);

            monsterHandle.spawnFrequency = 1;

            monsterHandle.spawnAmplitude = 2;

            monsterHandle.spawnIndex = new() { 4, };

            monsterHandle.spawnWithin = new(Math.Max(targetVector.X - 3, 0), targetVector.Y + 1);

            monsterHandle.spawnRange = new Vector2(8, 8);

            StardewValley.Locations.Woods woodsLocation = riteData.castLocation as StardewValley.Locations.Woods;

            woodsLocation.temporarySprites.Add(new TemporaryAnimatedSprite(10, new Vector2(8f, 7f) * 64f, Color.White, 9, flipped: false, 50f));
            woodsLocation.temporarySprites.Add(new TemporaryAnimatedSprite(10, new Vector2(9f, 7f) * 64f, Color.Orange, 9, flipped: false, 70f));
            woodsLocation.temporarySprites.Add(new TemporaryAnimatedSprite(10, new Vector2(8f, 6f) * 64f, Color.White, 9, flipped: false, 60f));
            woodsLocation.temporarySprites.Add(new TemporaryAnimatedSprite(10, new Vector2(9f, 6f) * 64f, Color.OrangeRed, 9, flipped: false, 120f));
            woodsLocation.temporarySprites.Add(new TemporaryAnimatedSprite(10, new Vector2(8f, 5f) * 64f, Color.Red, 9));
            woodsLocation.temporarySprites.Add(new TemporaryAnimatedSprite(10, new Vector2(9f, 5f) * 64f, Color.White, 9, flipped: false, 170f));
            woodsLocation.temporarySprites.Add(new TemporaryAnimatedSprite(11, new Vector2(544f, 464f), Color.Orange, 9, flipped: false, 40f));
            woodsLocation.temporarySprites.Add(new TemporaryAnimatedSprite(11, new Vector2(608f, 464f), Color.White, 9, flipped: false, 90f));
            woodsLocation.temporarySprites.Add(new TemporaryAnimatedSprite(11, new Vector2(544f, 400f), Color.OrangeRed, 9, flipped: false, 190f));
            woodsLocation.temporarySprites.Add(new TemporaryAnimatedSprite(11, new Vector2(608f, 400f), Color.White, 9, flipped: false, 80f));
            woodsLocation.temporarySprites.Add(new TemporaryAnimatedSprite(11, new Vector2(544f, 336f), Color.Red, 9, flipped: false, 69f));
            woodsLocation.temporarySprites.Add(new TemporaryAnimatedSprite(11, new Vector2(608f, 336f), Color.OrangeRed, 9, flipped: false, 130f));
            woodsLocation.temporarySprites.Add(new TemporaryAnimatedSprite(10, new Vector2(480f, 464f), Color.Orange, 9, flipped: false, 40f));
            woodsLocation.temporarySprites.Add(new TemporaryAnimatedSprite(11, new Vector2(672f, 368f), Color.White, 9, flipped: false, 90f));
            woodsLocation.temporarySprites.Add(new TemporaryAnimatedSprite(10, new Vector2(480f, 464f), Color.Red, 9, flipped: false, 30f));
            woodsLocation.temporarySprites.Add(new TemporaryAnimatedSprite(11, new Vector2(672f, 368f), Color.White, 9, flipped: false, 180f));
            woodsLocation.localSound("secret1");
            woodsLocation.map.GetLayer("Front").Tiles[8, 6].TileIndex = 1117;
            woodsLocation.map.GetLayer("Front").Tiles[9, 6].TileIndex = 1118;

            ModUtility.AnimateBolt(targetLocation, targetVector + new Vector2(0, -1));

            mod.RegisterChallenge(this, "active");

        }

        public override void EventReward()
        {
            
            if (monsterHandle.monsterSpawns.Count <= 10)
            {

                CastVoice("the dust settles");

                Throw throwObject = new(347, 0);

                throwObject.ThrowObject(targetPlayer, targetVector);

                mod.CompleteQuest(questData.name);

            }
            else
            {

                CastVoice("dust overwhelming");

            }

        }

        public override void EventInterval()
        {
            activeCounter++;

            if (activeCounter < 8)
            {
                switch (activeCounter)
                {
                    case 1:

                        CastVoice("can you feel it");

                        break;

                    case 3:

                        CastVoice("all around us");

                        break;

                    case 5:

                        CastVoice("THE DUST RISES");

                        break;

                    case 7:

                        Game1.changeMusicTrack("cowboy_outlawsong", false, Game1.MusicContext.Default);

                        break;

                    default:

                        targetLocation.playSound("dustMeep");

                        break;

                }
            }
            else if (activeCounter == 35)
            {

                CastVoice("ha ha ha");

            }
            else if (activeCounter == 38)
            {

                CastVoice("dust them");

                for (int i = 0; i < 2; i++)
                {
                    Throw throwObject = new(288, 0);

                    throwObject.ThrowObject(targetPlayer, targetVector);

                }

            }
            else if (activeCounter == 47)
            {

                CastVoice("the monarchs sleep",3000);

            }
            else if (activeCounter == 50)
            {

                CastVoice("and meeps creep into the world",3000);

            }
            else if (activeCounter <= 55)
            {

                monsterHandle.SpawnInterval();

            }
            else
            {

                monsterHandle.SpawnCheck();

            }

        }

    }
}
