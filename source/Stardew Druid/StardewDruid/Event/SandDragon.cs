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
using StardewDruid.Monster;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using xTile.Layers;
using xTile.Tiles;

namespace StardewDruid.Event
{
    public class SandDragon: ChallengeHandle
    {

        public bool modifiedSandDragon;

        public BossDragon bossMonster;

        public SandDragon(Mod Mod, Vector2 target, Rite rite, Quest quest)
            : base(Mod, target, rite, quest)
        {

            targetVector = questData.vectorList["targetVector"];

            voicePosition = (targetVector *64) + new Vector2(0, -32);

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 90;

        }

        public override void EventTrigger()
        {

            ModUtility.AnimateMeteorZone(targetLocation, targetVector, Color.Red, 2);

            ModUtility.AnimateMeteor(targetLocation, targetVector, true);
           
            mod.RegisterChallenge(this, "active");

        }

        public override bool EventActive()
        {
            
            if (targetPlayer.currentLocation == targetLocation)
            {

                double nowTime = Game1.currentGameTime.TotalGameTime.TotalSeconds;

                if (expireTime >= nowTime && !expireEarly)
                {
                    int diffTime = (int)Math.Round(expireTime - nowTime);

                    if (activeCounter != 0 && diffTime % 10 == 0 && diffTime != 0)
                    {

                        Game1.addHUDMessage(new HUDMessage($"{diffTime} more minutes left!", "2"));

                    }

                    return true;

                }

                DelayedAction.functionAfterDelay(EventReward,2000);

            }
            else
            {

                EventAbort();

            }

            return false;

        }

        public override void EventRemove()
        {

            if (modifiedSandDragon)
            {

                ResetSandDragon();

            }

            if (bossMonster != null)
            {

                riteData.castLocation.characters.Remove(bossMonster);

                bossMonster = null;

            }

            base.EventRemove();

        }

        public override void EventReward()
        {
            if (expireEarly)
            {

                CastVoice("the power of the shamans lingers");

                Vector2 debrisVector = Game1.player.getTileLocation() + new Vector2(0, 1);

                Dictionary<string, int> blessingList = mod.BlessingList();

                if (!blessingList.ContainsKey("shardSandDragon"))
                {

                    Game1.createObjectDebris(74, (int)debrisVector.X, (int)debrisVector.Y);

                    mod.UpdateBlessing("shardSandDragon");

                }

                Game1.createObjectDebris(681, (int)debrisVector.X, (int)debrisVector.Y);

                mod.CompleteQuest(questData.name);

            }
            else
            {

                CastVoice("return when you have strength");

            }

        }

        public override void EventInterval()
        {

            activeCounter++;

            if (activeCounter < 9)
            {

                switch (activeCounter)
                {
                    case 1:

                        CastVoice("a taste of the stars");

                        break;

                    case 3:

                        CastVoice("from the time when the shamans sang to us");

                        break;

                    case 5:

                        CastVoice("and my kin held dominion");

                        break;

                    case 7:

                        CastVoice("...my bones stir...");

                        break;

                    default:

                        Vector2 randomVector = targetVector + new Vector2(0, 1) - new Vector2(randomIndex.Next(7), randomIndex.Next(3));

                        ModUtility.AnimateMeteorZone(targetLocation, randomVector, Color.Red, 2);

                        ModUtility.AnimateMeteor(targetLocation, randomVector, randomIndex.Next(2) == 0);

                        break;

                }

                return;

            }

            if (activeCounter == 9)
            {
                ModifySandDragon();

                StardewValley.Monsters.Monster theMonster = MonsterData.CreateMonster(14, targetVector + new Vector2(-5, 0), riteData.combatModifier);

                bossMonster = theMonster as BossDragon;

                riteData.castLocation.characters.Add(bossMonster);

                bossMonster.update(Game1.currentGameTime, riteData.castLocation);

                Game1.changeMusicTrack("cowboy_boss", false, Game1.MusicContext.Default);

                return;

            }

            if (bossMonster.defeated || bossMonster.Health <= 0)
            {

                expireEarly = true;

            }

        }

        public void ResetSandDragon()
        {

            targetLocation.loadMap(targetLocation.mapPath.Value, true);

            if (Game1.eventUp || Game1.fadeToBlack || Game1.currentMinigame != null || Game1.isWarping || Game1.killScreen || Game1.player.currentLocation is not Desert)
            {
                return;

            }

            Game1.fadeScreenToBlack();

            targetPlayer.Position = targetVector * 64 - new Vector2(0, 64);

        }

        public void ModifySandDragon()
        {

            modifiedSandDragon = true;

            // ----------------------------- clear sheet

            Layer backLayer = targetLocation.map.GetLayer("Back");

            Layer buildingsLayer = targetLocation.map.GetLayer("Buildings");

            Layer frontLayer = targetLocation.map.GetLayer("Front");

            Layer alwaysfrontLayer = targetLocation.map.GetLayer("AlwaysFront");

            TileSheet desertSheet = targetLocation.map.GetTileSheet("desert-new");

            Vector2 offsetVector = targetVector - new Vector2(8, 5);

            for (int i = 0; i < 9; i++)
            {

                for (int j = 0; j < 10; j++)
                {

                    Vector2 tileVector = offsetVector + new Vector2(j, i);

                    if (buildingsLayer.Tiles[(int)tileVector.X, (int)tileVector.Y] != null)
                    {

                        int tileIndex = buildingsLayer.Tiles[(int)tileVector.X, (int)tileVector.Y].TileIndex;

                        if (tileIndex < 192 || tileIndex == 219)
                        {
                            buildingsLayer.Tiles[(int)tileVector.X, (int)tileVector.Y] = null;

                        }

                    }

                    if (frontLayer.Tiles[(int)tileVector.X, (int)tileVector.Y] != null)
                    {

                        int tileIndex = frontLayer.Tiles[(int)tileVector.X, (int)tileVector.Y].TileIndex;

                        if (tileIndex < 192 || tileIndex == 203)
                        {
                            frontLayer.Tiles[(int)tileVector.X, (int)tileVector.Y] = null;

                        }

                    }

                    if (alwaysfrontLayer.Tiles[(int)tileVector.X, (int)tileVector.Y] != null)
                    {

                        int tileIndex = alwaysfrontLayer.Tiles[(int)tileVector.X, (int)tileVector.Y].TileIndex;

                        if (tileIndex < 192)
                        {
                            alwaysfrontLayer.Tiles[(int)tileVector.X, (int)tileVector.Y] = null;

                        }

                    }


                    if (randomIndex.Next(4) != 0)
                    {
                        backLayer.Tiles[(int)tileVector.X, (int)tileVector.Y] = new StaticTile(backLayer, desertSheet, BlendMode.Alpha, 65);
                    }
                    else if (randomIndex.Next(5) == 0)
                    {
                        backLayer.Tiles[(int)tileVector.X, (int)tileVector.Y] = new StaticTile(backLayer, desertSheet, BlendMode.Alpha, 96);
                    }
                    else if (randomIndex.Next(5) == 0)
                    {
                        backLayer.Tiles[(int)tileVector.X, (int)tileVector.Y] = new StaticTile(backLayer, desertSheet, BlendMode.Alpha, 97);
                    }
                    else
                    {
                        backLayer.Tiles[(int)tileVector.X, (int)tileVector.Y] = new StaticTile(backLayer, desertSheet, BlendMode.Alpha, 98);
                    }

                }

            }

        }


    }
}
