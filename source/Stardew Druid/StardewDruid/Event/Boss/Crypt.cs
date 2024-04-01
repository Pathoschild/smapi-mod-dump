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
using StardewDruid.Character;
using StardewDruid.Event.Challenge;
using StardewDruid.Location;
using StardewDruid.Map;
using StardewDruid.Monster.Boss;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.IO;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace StardewDruid.Event.Boss
{
    public class Crypt : BossHandle
    {

        public List<StardewDruid.Monster.Boss.Boss> bossMonsters;

        public Crypt(Vector2 target,  Quest quest)
          : base(target, quest)
        {

            targetVector = target;

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 90.0;

            bossMonsters = new();

            cues = DialogueData.DialogueScene(questData.name);

            AddActor(new (target.X * 64f, target.Y * 64f - 32f));

        }

        public override void EventRemove()
        {

            base.EventRemove();

        }

        public override void RemoveMonsters()
        {
            if(bossMonsters.Count > 0)
            {

                for (int i = bossMonsters.Count - 1; i >= 0; i--)
                {

                    StardewDruid.Monster.Boss.Boss boss = bossMonsters[i];

                    boss.currentLocation.characters.Remove(boss);

                }

                bossMonsters.Clear();

            }

            base.RemoveMonsters();

        }

        public override void EventAbort()
        {

            base.EventAbort();

            if(Game1.player.currentLocation.Name == "18465_Crypt")
            {

                WarpBackToTown();

            }

        }


        public override bool EventExpire()
        {

            if (eventLinger == -1)
            {

                RemoveMonsters();

                eventLinger = 3;

                return true;

            }

            if (eventLinger == 2)
            {

                if (expireEarly)
                {

                    EventComplete();

                    QuestData.NextProgress();

                }
                else
                {

                    WarpBackToTown();

                    Mod.instance.CastMessage("Try again tomorrow");

                }

            }

            return base.EventExpire();

        }

        public void WarpBackToTown()
        {

            int warpX = (int)questData.triggerVector.X;

            int warpY = (int)questData.triggerVector.Y;

            Game1.warpFarmer("town", warpX, warpY, 2);

            Game1.xLocationAfterWarp = warpX;

            Game1.yLocationAfterWarp = warpY;

            EventQuery("LocationReturn");

        }

        public override void EventInterval()
        {
            
            activeCounter++;

            if (activeCounter == 1)
            {

                Location.LocationData.CryptEdit();

                targetLocation = Game1.getLocationFromName("18465_Crypt");

                targetVector = new Vector2(20, 15);

                Game1.warpFarmer("18465_Crypt", 20, 10, 2);

                Game1.xLocationAfterWarp = 20;

                Game1.yLocationAfterWarp = 10;

                RemoveActors();

                actors.Clear();

                AddActor(new(targetVector.X * 64, targetVector.Y * 64 - 32f));

                SetTrack("tribal");

                return;

            }

            if (activeCounter == 2)
            {

                EventQuery("LocationEdit");

                EventQuery("LocationPortal");

                int difficulty = 2;

                if (questData.name.Contains("Two"))
                {

                    difficulty = 4;
                
                }
                else
                {
                    Monster.Boss.Shadowtin bossShadowtin = new(targetVector + new Vector2(0, 5f),Mod.instance.CombatModifier());

                    targetLocation.characters.Add(bossShadowtin);

                    bossShadowtin.currentLocation = Mod.instance.rite.castLocation;

                    bossShadowtin.update(Game1.currentGameTime, Mod.instance.rite.castLocation);

                    bossMonsters.Add(bossShadowtin);

                }

                for(int j = 0; j < difficulty; j++)
                {

                    StardewDruid.Monster.Boss.Boss thief;

                    Vector2 startVector = targetVector + new Vector2(-4 + (3 * j), 3f);

                    string smacktalk = "meow?";

                    switch (randomIndex.Next(4))
                    {
                        case 1:
                            thief = new Scavenger(startVector, Mod.instance.CombatModifier());
                            break;
                        case 2:
                            thief = new Shadowfox(startVector, Mod.instance.CombatModifier());
                            smacktalk = "arwooooo!";
                            break;
                        case 3:
                            thief = new Goblin(startVector, Mod.instance.CombatModifier());
                            smacktalk = "heh heh";
                            break;
                        default: //0
                            thief = new Rogue(startVector, Mod.instance.CombatModifier());
                            smacktalk = "heh heh";
                            break;

                    }

                    targetLocation.characters.Add(thief);

                    thief.update(Game1.currentGameTime, targetLocation);

                    if (questData.name.Contains("Two"))
                    {
                        thief.HardMode();
                    }

                    thief.showTextAboveHead(smacktalk);

                    bossMonsters.Add(thief);
                }

                braziers.Add(new(targetLocation, new(13, 13)));

                braziers.Add(new(targetLocation, new(13, 26)));

                braziers.Add(new(targetLocation, new(26, 13)));

                braziers.Add(new(targetLocation, new(26, 26)));

                return;

            }

            if (activeCounter < 5) {

                return;

            }

            for(int i = bossMonsters.Count - 1; i >= 0; i--)
            {

                StardewDruid.Monster.Boss.Boss boss = bossMonsters[i];

                if (!ModUtility.MonsterVitals(boss, targetLocation))
                {
                    bossMonsters.RemoveAt(i);
                }

            }

            if(bossMonsters.Count == 0)
            {
                expireEarly = true;

                return;

            }

            DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [0] = bossMonsters[0], }, activeCounter);

            if (activeCounter % 60 == 0)
            {

                ResetBraziers();

            }

        }

    }

}