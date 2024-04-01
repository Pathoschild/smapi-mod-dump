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
using StardewDruid.Monster.Boss;
using StardewValley;
using System.Collections.Generic;
using System.IO;
using static StardewValley.IslandGemBird;

namespace StardewDruid.Event.Challenge
{
    public class Quarry : ChallengeHandle
    {

        public List<TemporaryAnimatedSprite> challengeAnimations;

        public int specialBoss;

        public bool keepJester;

        public Quarry(Vector2 target,  Quest quest)
            : base(target, quest)
        {

            challengeAnimations = new();

        }

        public override void EventTrigger()
        {
            cues = DialogueData.DialogueScene(questData.name);

            monsterHandle = new(targetVector, Mod.instance.rite.castLocation);

            monsterHandle.spawnWithin = targetVector + new Vector2(-5, -5);

            monsterHandle.spawnRange = new Vector2(11, 11);

            Mod.instance.RegisterEvent(this, "active");

            if (Mod.instance.characters.ContainsKey("Jester"))
            {

                StardewDruid.Character.Character jester = Mod.instance.characters["Jester"];

                jester.ResetActives();

                jester.SwitchFollowMode();

                if (jester.currentLocation.Name != targetLocation.Name)
                {

                    jester.currentLocation.characters.Remove(jester);

                    jester.currentLocation = targetLocation;

                    jester.currentLocation.characters.Add(jester);

                }

                if (!Utility.isOnScreen(jester.Position,0))
                {

                    jester.Position = targetVector * 64 + new Vector2(0, 128);

                    jester.update(Game1.currentGameTime, targetLocation);

                    ModUtility.AnimateQuickWarp(targetLocation, jester.Position);

                }

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
            
            EventComplete();

            if (Mod.instance.characters.ContainsKey("Jester"))
            {

                keepJester = true;

                Mod.instance.dialogue["Jester"].AddSpecial("Jester", "ThanatoshiTwo");

            }

            return false;

        }

        public override void EventInterval()
        {

            activeCounter++;

            monsterHandle.SpawnCheck();

            if (eventLinger != -1)
            {

                return;

            }

            if (Mod.instance.characters.ContainsKey("Jester"))
            {

                DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [0] = Mod.instance.characters["Jester"], }, activeCounter);

            }

            if (activeCounter < 7)
            {
                
                Vector2 meteorVector = new(-5 + randomIndex.Next(10), -5 + randomIndex.Next(10));

                Cast.Stars.Meteor meteorCast = new(meteorVector+targetVector, Mod.instance.DamageLevel());

                meteorCast.targetLocation = targetLocation;

                meteorCast.CastEffect();

                if (activeCounter == 4)
                {

                    TemporaryAnimatedSprite challengeAnimation = new(0, 99999f, 1, 1, targetVector * 64 - new Vector2(64, 64), false, false)
                    {

                        sourceRect = new(192, 0, 64, 64),

                        sourceRectStartingPos = new Vector2(192,0),

                        texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Decorations.png")),

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

                    SetTrack("tribal");

                }

                return;
            }

            if (activeCounter <= 56 && activeCounter % 3 == 0)
            {

                Vector2 monsterVector = monsterHandle.SpawnVector();

                if(monsterVector != new Vector2(-1))
                {

                    Gargoyle newMonster;

                    string scheme = "Solar";

                    if (activeCounter > 20 && specialBoss < 1)
                    {

                        newMonster = new(monsterVector, Mod.instance.CombatModifier());

                        newMonster.ChampionMode(2);

                        specialBoss = 1;

                    }
                    else if (activeCounter > 40 && specialBoss < 2)
                    {

                        newMonster = new(monsterVector, Mod.instance.CombatModifier());

                        newMonster.ChampionMode(2);

                        scheme = "Void";

                        specialBoss = 2;

                    }
                    else
                    {

                        newMonster = new(monsterVector, Mod.instance.CombatModifier());

                        scheme = randomIndex.Next(2) == 0 ? "Solar" : "Void";

                    }

                    newMonster.netScheme.Set(scheme);

                    newMonster.SchemeLoad();

                    if (questData.name.Contains("Two"))
                    {

                        newMonster.HardMode();

                    }

                    monsterHandle.SpawnImport(newMonster);

                }

            }

        }

    }

}
