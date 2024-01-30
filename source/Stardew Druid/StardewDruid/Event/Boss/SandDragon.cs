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
using StardewValley.Locations;
using System;
using xTile.Layers;
using xTile.Tiles;

namespace StardewDruid.Event.Boss
{
    public class SandDragon : BossHandle
    {

        public bool modifiedSandDragon;

        //public BossDragon bossMonster;
        public StardewDruid.Monster.Boss.Dragon bossMonster;

        public Vector2 returnPosition;

        public SandDragon(Vector2 target, Rite rite, Quest quest)
            : base(target, rite, quest)
        {

            targetVector = target;

            voicePosition = targetVector * 64 + new Vector2(0, -32);

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 120;

            returnPosition = rite.caster.Position;

        }

        public override void EventTrigger()
        {

            ModUtility.AnimateRadiusDecoration(targetLocation, targetVector, "Stars", 1f, 1f);

            ModUtility.AnimateMeteor(targetLocation, targetVector, true);

            base.EventTrigger();

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

            if (modifiedSandDragon)
            {
                
                modifiedSandDragon = false;
                
                Location.LocationData.SandDragonReset();

                EventQuery("LocationReset");

            }

            base.EventRemove();

        }

        public override bool EventExpire()
        {

            if (eventLinger == -1)
            {

                RemoveMonsters();

                if (modifiedSandDragon)
                {
                    
                    modifiedSandDragon = false;
                    
                    Location.LocationData.SandDragonReset();

                    EventQuery("LocationReset");

                    EventReposition();

                }

                eventLinger = 3;

                return true;

            }

            if (eventLinger == 2)
            {

                if (expireEarly)
                {

                    CastVoice("the power of the shamans lingers");

                    Vector2 debrisVector = Game1.player.getTileLocation() + new Vector2(0, 1);

                    if (!questData.name.Contains("Two"))
                    {

                        Game1.createObjectDebris(74, (int)debrisVector.X, (int)debrisVector.Y);

                    }

                    Game1.createObjectDebris(681, (int)debrisVector.X, (int)debrisVector.Y);

                    EventComplete();

                }
                else
                {

                    CastVoice("You're no match for me");

                    Mod.instance.CastMessage("Try again tomorrow");

                }

            }

            return base.EventExpire();

        }

        public void EventReposition()
        {

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

                        targetLocation.playSoundPitched("DragonRoar", 1200);

                        break;

                    case 5:

                        CastVoice("and my kin held dominion");

                        targetLocation.playSoundPitched("DragonRoar",800);

                        break;

                    case 7:

                        CastVoice("...my bones stir...");

                        targetLocation.playSoundPitched("DragonRoar", 400);

                        break;

                    default:

                        Vector2 randomVector = targetVector + new Vector2(0, 1) - new Vector2(randomIndex.Next(7), randomIndex.Next(3));

                        ModUtility.AnimateRadiusDecoration(targetLocation, randomVector, "Stars", 1f, 1f);

                        ModUtility.AnimateMeteor(targetLocation, randomVector, randomIndex.Next(2) == 0);

                        break;

                }

                return;

            }

            if (activeCounter == 9)
            {

                modifiedSandDragon = true;

                Location.LocationData.SandDragonEdit();

                EventQuery("LocationEdit");

                StardewValley.Monsters.Monster theMonster = MonsterData.CreateMonster(16, targetVector + new Vector2(-5, 0));

                bossMonster = theMonster as StardewDruid.Monster.Boss.Dragon;

                if (questData.name.Contains("Two"))
                {

                    bossMonster.HardMode();

                }

                targetLocation.characters.Add(bossMonster);

                bossMonster.currentLocation = targetLocation;

                bossMonster.update(Game1.currentGameTime, targetLocation);

                SetTrack("cowboy_boss");

                return;

            }

            if (activeCounter > 11 && !ModUtility.MonsterVitals(bossMonster, targetLocation))
            {

                expireEarly = true;

            }

        }

    }

}
