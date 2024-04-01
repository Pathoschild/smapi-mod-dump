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
using StardewValley.GameData;
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

        public SandDragon(Vector2 target,  Quest quest)
            : base(target, quest)
        {

            targetVector = target;

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 120;

            returnPosition = Game1.player.Position;

        }

        public override void EventTrigger()
        {
            cues = DialogueData.DialogueScene(questData.name);

            AddActor(targetVector * 64 + new Vector2(0, -32));

            Cast.Stars.Meteor meteorCast = new(targetVector, Mod.instance.DamageLevel());

            meteorCast.targetLocation = targetLocation;

            meteorCast.CastEffect();

            base.EventTrigger();

        }

        public override void RemoveMonsters()
        {

            if (bossMonster != null)
            {

                Mod.instance.rite.castLocation.characters.Remove(bossMonster);

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

                    DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [0] = actors[0], }, 991);

                    Vector2 debrisVector = Game1.player.Tile + new Vector2(0, 1);

                    if (!questData.name.Contains("Two"))
                    {

                        Game1.createObjectDebris("74", (int)debrisVector.X, (int)debrisVector.Y);

                    }

                    Game1.createObjectDebris("681", (int)debrisVector.X, (int)debrisVector.Y);

                    EventComplete();

                }
                else
                {
                    DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [0] = actors[0], }, 992);

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

                Game1.stopMusicTrack(MusicContext.Default);

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
                
                DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [0] = actors[0], }, activeCounter);

                switch (activeCounter)
                {
                    case 1:

                        targetLocation.playSound("DragonRoar", targetVector*64, 1500);
                        break;

                    case 3:

                        targetLocation.playSound("DragonRoar", targetVector * 64, 1200);

                        break;

                    case 5:

                        targetLocation.playSound("DragonRoar", targetVector * 64, 800);

                        break;

                    case 7:

                        targetLocation.playSound("DragonRoar", targetVector * 64, 400);

                        break;

                }

                if(activeCounter % 2 == 0)
                {
                    
                    Vector2 randomVector = targetVector - new Vector2(4, 4) + new Vector2(randomIndex.Next(8), randomIndex.Next(8));

                    Cast.Stars.Meteor meteorCast = new(randomVector, Mod.instance.DamageLevel());

                    meteorCast.targetLocation = targetLocation;

                    meteorCast.CastEffect();

                }

                return;

            }

            if (activeCounter == 9)
            {

                modifiedSandDragon = true;

                Location.LocationData.SandDragonEdit();

                EventQuery("LocationEdit");

                bossMonster = new(targetVector + new Vector2(-5, 0),Mod.instance.CombatModifier());

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
