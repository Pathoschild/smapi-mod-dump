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
using System.Collections.Generic;
using System.IO;

namespace StardewDruid.Event.Challenge
{
    public class Quarry : ChallengeHandle
    {

        public List<TemporaryAnimatedSprite> challengeAnimations;

        public bool keepJester;

        public Quarry(Vector2 target, Rite rite, Quest quest)
            : base(target, rite, quest)
        {
            challengeAnimations = new();
        }

        public override void EventTrigger()
        {

            monsterHandle = new(targetVector, riteData);

            monsterHandle.spawnIndex = new() { 51, 52, 53, 54, };

            monsterHandle.spawnFrequency = 1;

            if (questData.name.Contains("Two"))
            {

                monsterHandle.spawnFrequency = 1;

                monsterHandle.spawnAmplitude = 2;

            }

            monsterHandle.spawnWithin = targetVector + new Vector2(-5, -5);

            monsterHandle.spawnRange = new Vector2(11, 11);

            Mod.instance.RegisterEvent(this, "active");

            if (Mod.instance.characters.ContainsKey("Jester"))
            {

                StardewDruid.Character.Character jester = Mod.instance.characters["Jester"];

                jester.SwitchFollowMode();

                jester.WarpToTarget();


            }


        }

        public override void EventRemove()
        {

            base.EventRemove();
            if (Mod.instance.characters.ContainsKey("Jester"))
            {
                if (!keepJester)
                {

                    Mod.instance.characters["Jester"].WarpToDefault();

                    Mod.instance.characters["Jester"].SwitchRoamMode();


                }
            }
            if (challengeAnimations.Count > 0)
            {

                foreach (var animation in challengeAnimations)
                {

                    targetLocation.temporarySprites.Remove(animation);

                }

            }

        }

        public override bool EventExpire()
        {
            Mod.instance.CompleteQuest(questData.name);



            if (Mod.instance.characters.ContainsKey("Jester"))
            {
                keepJester = true;
                Mod.instance.characters["Jester"].timers.Clear();

                Mod.instance.dialogue["Jester"].specialDialogue.Add("afterQuarry", new() {
                "Jester of Fate:" +
                "^I wasn't expecting the rite to produce a portal to the Undervalley. " +
                "I don't think even Fortumei could have foreseen that.",
                "Did you learn anything about the fallen one?" });
            }
            return false;

        }

        public override void EventInterval()
        {

            activeCounter++;

            if (eventLinger != -1)
            {

                return;

            }

            if (activeCounter < 7)
            {
                riteData.castLevel++;

                riteData.CastStars();

                riteData.CastEffect(false);


                if (activeCounter == 4)
                {

                    TemporaryAnimatedSprite challengeAnimation = new(0, 99999f, 1, 1, targetVector * 64 - new Vector2(64, 64), false, false)
                    {

                        sourceRect = new(0, 0, 64, 64),

                        sourceRectStartingPos = new Vector2(0, 0),

                        texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Portal.png")),

                        scale = 3f,

                        layerDepth = 0.00002f,

                        rotationChange = -0.06f,

                    };

                    targetLocation.temporarySprites.Add(challengeAnimation);

                    challengeAnimations.Add(challengeAnimation);

                    TemporaryAnimatedSprite nightAnimation = new(0, 99999f, 1, 1, targetVector * 64 - new Vector2(64, 64), false, false)
                    {

                        sourceRect = new(0, 0, 64, 64),

                        sourceRectStartingPos = new Vector2(0, 0),

                        texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Nightsky.png")),

                        scale = 3f,

                        layerDepth = 0.00001f,

                    };

                    targetLocation.temporarySprites.Add(nightAnimation);

                    challengeAnimations.Add(nightAnimation);

                }

                if (activeCounter == 5)
                {

                    JesterVoice("uh... portal?", 3000);

                    SetTrack("tribal");

                }

                return;
            }


            if (activeCounter == 8)
            {

                JesterVoice("get ready for a fight!", 3000);

            }

            if (activeCounter == 20)
            {

                Vector2 spawnVector = monsterHandle.SpawnVector();

                if (spawnVector != new Vector2(-1))
                {
                    monsterHandle.specialIndex = new() { 55, };
                    monsterHandle.SpawnGround(spawnVector, true);

                }

                JesterVoice("whoa that one's massive!", 3000);

            }

            if (activeCounter == 30)
            {

                JesterVoice("keep it up farmer!", 3000);

            }

            if (activeCounter == 34)
            {

                JesterVoice("meet your fate voidspawn!", 3000);

            }

            if (activeCounter == 40)
            {

                Vector2 spawnVector = monsterHandle.SpawnVector();

                if (spawnVector != new Vector2(-1))
                {
                    monsterHandle.specialIndex = new() { 56, };
                    monsterHandle.SpawnGround(spawnVector, true);

                }

                JesterVoice("if only Lucky could see this", 3000);

            }

            if (activeCounter == 52)
            {

                JesterVoice("whew...the portal is closing", 3000);

            }

            if (activeCounter <= 56)
            {

                monsterHandle.SpawnInterval();

            }
            else
            {

                monsterHandle.SpawnCheck();

            }

        }

        public void JesterVoice(string speech, int interval)
        {

            if (Mod.instance.characters.ContainsKey("Jester"))
            {
                Mod.instance.characters["Jester"].showTextAboveHead(speech, interval);

            }

        }
    
    }

}
