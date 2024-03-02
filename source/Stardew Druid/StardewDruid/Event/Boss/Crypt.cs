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

        public List<StardewDruid.Monster.Boss.Dragon> bossMonsters;

        public Crypt(Vector2 target, Rite rite, Quest quest)
          : base(target, rite, quest)
        {

            targetVector = target;

            voicePosition = new(targetVector.X * 64f, targetVector.Y * 64f - 32f);

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 90.0;

            bossMonsters = new();

            cues = DialogueData.DialogueScene(questData.name);

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

                    StardewDruid.Monster.Boss.Dragon boss = bossMonsters[i];

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

                voicePosition = new(targetVector.X * 64, targetVector.Y * 64 - 32f);

                SetTrack("tribal");

                return;

            }

            if (activeCounter == 2)
            {

                EventQuery("LocationEdit");

                EventQuery("LocationPortal");

                int difficulty = 1;

                if (questData.name.Contains("Two"))
                {

                    difficulty = 2;
                
                }
                else
                {
                    Monster.Boss.Shadowtin bossShadowtin = MonsterData.CreateMonster(18, targetVector + new Vector2(0, 5f)) as Monster.Boss.Shadowtin;

                    targetLocation.characters.Add(bossShadowtin);

                    bossShadowtin.currentLocation = riteData.castLocation;

                    bossShadowtin.update(Game1.currentGameTime, riteData.castLocation);

                    bossMonsters.Add(bossShadowtin);

                }

                for(int j = 0; j < difficulty; j++)
                {

                    Scavenger bossScavenger = MonsterData.CreateMonster(19, targetVector + new Vector2(4 - (2* difficulty), 6f - (3 * difficulty))) as Scavenger;

                    if (questData.name.Contains("Two"))
                    {
                        bossScavenger.HardMode();
                    }

                    targetLocation.characters.Add(bossScavenger);

                    bossScavenger.currentLocation = riteData.castLocation;

                    bossScavenger.update(Game1.currentGameTime, riteData.castLocation);

                    bossMonsters.Add(bossScavenger);


                    Rogue bossRogue = MonsterData.CreateMonster(20, targetVector + new Vector2(-4 + (2 * difficulty), 6f - (3 * difficulty))) as Rogue;

                    if (questData.name.Contains("Two"))
                    {
                        bossRogue.HardMode();
                    }

                    targetLocation.characters.Add(bossRogue);

                    bossRogue.currentLocation = riteData.castLocation;

                    bossRogue.update(Game1.currentGameTime, riteData.castLocation);

                    bossMonsters.Add(bossRogue);

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

                StardewDruid.Monster.Boss.Dragon boss = bossMonsters[i];

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