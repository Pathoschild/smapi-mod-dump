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

namespace StardewDruid.Event.Challenge
{
    public class Canoli : ChallengeHandle
    {

        public Canoli(Vector2 target, Rite rite, Quest quest)
            : base(target, rite, quest)
        {

            voicePosition = targetVector * 64 + new Vector2(-8, -56);

        }

        public override void EventTrigger()
        {
            cues = DialogueData.DialogueScene(questData.name);

            monsterHandle = new(targetVector + new Vector2(0, 2), riteData.castLocation);

            monsterHandle.spawnFrequency = 1;

            monsterHandle.spawnAmplitude = 1;

            if (questData.name.Contains("Two"))
            {
                
                monsterHandle.spawnCombat *= 2;

            }

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

            Mod.instance.RegisterEvent(this, "active");

        }

        public override bool EventExpire()
        {

            if (eventLinger == -1)
            {
                if (monsterHandle.monsterSpawns.Count <= 4)
                {
                    DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [0] = actors[0], }, 991);
                    int num = questData.name.Contains("Two") ? 2 : 1;
                    for (int index = 0; index < num; ++index)
                    {

                        new Throw(targetPlayer, targetVector * 64f, 347, 0).ThrowObject();
                    }

                }
                else if (monsterHandle.monsterSpawns.Count <= 10)
                {

                    DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [0] = actors[0], }, 992);
                    int num = questData.name.Contains("Two") ? 2 : 1;
                    for (int index = 0; index < num; ++index)
                    {

                        new Throw(targetPlayer, targetVector * 64f, 72, 0).ThrowObject();
                    }

                }
                else
                {

                    DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [0] = actors[0], }, 993);

                }

                int monsterDefeated = monsterHandle.spawnTotal - monsterHandle.monsterSpawns.Count;

                Mod.instance.CastMessage("Defeated " + monsterDefeated + " out of " + monsterHandle.spawnTotal +" opponents");


                EventComplete();

                eventLinger = 3;

                RemoveMonsters();

                return true;

            }

            return base.EventExpire();

        }

        public override void EventInterval()
        {
            activeCounter++;

            monsterHandle.SpawnCheck();

            if (eventLinger != -1)
            {

                return;

            }

            DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [0] = actors[0], }, activeCounter);

            if (activeCounter < 8)
            {
                switch (activeCounter)
                {

                    case 7:

                        SetTrack("cowboy_outlawsong");

                        break;

                    default:

                        targetLocation.playSound("dustMeep");

                        targetLocation.playSound("dustMeep");

                        break;

                }
            }
            else if (activeCounter == 38)
            {

                for (int i = 0; i < 2; i++)
                {
                    Throw throwObject = new(targetPlayer, targetVector * 64, 288, 0);

                    throwObject.ThrowObject();

                }

            }
            else if (activeCounter <= 57)
            {

                monsterHandle.SpawnInterval();

            }

        }

    }
}
