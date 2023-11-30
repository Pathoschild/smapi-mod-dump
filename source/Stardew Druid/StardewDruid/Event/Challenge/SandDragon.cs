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

namespace StardewDruid.Event.Challenge
{
    public class SandDragon : ChallengeHandle
    {

        public bool modifiedSandDragon;

        //public BossDragon bossMonster;
        public StardewDruid.Monster.RedDragon bossMonster;

        public Vector2 returnPosition;

        public SandDragon(Vector2 target, Rite rite, Quest quest)
            : base(target, rite, quest)
        {

            targetVector = target;

            voicePosition = targetVector * 64 + new Vector2(0, -32);

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 90;

            returnPosition = rite.caster.Position;

        }

        public override void EventTrigger()
        {

            ModUtility.AnimateRadiusDecoration(targetLocation, targetVector, "Stars", 1f, 1f);

            ModUtility.AnimateMeteor(targetLocation, targetVector, true);

            Mod.instance.RegisterEvent(this, "active");

        }

        public override bool EventActive()
        {

            if (targetPlayer.currentLocation == targetLocation && !eventAbort)
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

                return EventExpire();
                
            }
            else
            {

                EventAbort();

            }

            return false;

        }

        public override void RemoveMonsters()
        {

            if (bossMonster != null)
            {

                riteData.castLocation.characters.Remove(bossMonster);

                bossMonster = null;

            }

            base.RemoveMonsters();

        }

        public override void EventRemove()
        {

            ResetSandDragon();

            base.EventRemove();

        }

        public override bool EventExpire()
        {

            if(eventLinger == -1)
            {

                ResetSandDragon();

                eventLinger = 4;

                return true;

            }

            if (eventLinger == 3)
            {

                if (expireEarly)
                {

                    CastVoice("the power of the shamans lingers");

                    Vector2 debrisVector = Game1.player.getTileLocation() + new Vector2(0, 1);

                    if (!questData.name.Contains("Two"))
                    {

                        Game1.createObjectDebris(74, (int)debrisVector.X, (int)debrisVector.Y);

                        Mod.instance.UpdateBlessing("shardSandDragon");

                    }

                    Game1.createObjectDebris(681, (int)debrisVector.X, (int)debrisVector.Y);

                    Mod.instance.CompleteQuest(questData.name);

                }
                else
                {

                    CastVoice("return when you have strength");

                }

            }

            return base.EventExpire();

        }

        public override void EventInterval()
        {

            activeCounter++;

            if (eventLinger != -1)
            {

                return;

            }

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

                        //ModUtility.AnimateMeteorZone(targetLocation, randomVector, Color.Red);
                        ModUtility.AnimateRadiusDecoration(targetLocation, randomVector, "Stars", 1f, 1f);

                        ModUtility.AnimateMeteor(targetLocation, randomVector, randomIndex.Next(2) == 0);

                        break;

                }

                return;

            }

            if (activeCounter == 9)
            {
                ModifySandDragon();

                //StardewValley.Monsters.Monster theMonster = MonsterData.CreateMonster(14, targetVector + new Vector2(-5, 0), riteData.combatModifier);
                StardewValley.Monsters.Monster theMonster = MonsterData.CreateMonster(16, targetVector + new Vector2(-5, 0), riteData.combatModifier);

                //bossMonster = theMonster as BossDragon;
                bossMonster = theMonster as StardewDruid.Monster.RedDragon;


                if (questData.name.Contains("Two"))
                {

                    bossMonster.HardMode();

                }

                riteData.castLocation.characters.Add(bossMonster);

                bossMonster.update(Game1.currentGameTime, riteData.castLocation);

                SetTrack("cowboy_boss");

                return;

            }

            if (bossMonster.defeated || bossMonster.Health <= 0 || bossMonster == null || !riteData.castLocation.characters.Contains(bossMonster))
            {

                expireEarly = true;

            }

        }

        public void ResetSandDragon()
        {

            if(!modifiedSandDragon)
            {

                return;

            }

            targetLocation.loadMap(targetLocation.mapPath.Value, true);

            if (Game1.eventUp || Game1.fadeToBlack || Game1.currentMinigame != null || Game1.isWarping || Game1.killScreen || Game1.player.currentLocation is not Desert)
            {
                return;

            }

            Game1.fadeScreenToBlack();

            targetPlayer.Position = returnPosition;

            if (soundTrack)
            {

                Game1.stopMusicTrack(Game1.MusicContext.Default);

                soundTrack = false;

            }

            modifiedSandDragon = false;

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
