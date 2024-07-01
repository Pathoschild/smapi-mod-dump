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
using StardewDruid.Data;
using StardewDruid.Journal;
using StardewDruid.Monster;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static StardewDruid.Cast.SpellHandle;
using static StardewDruid.Data.IconData;
using static System.Net.Mime.MediaTypeNames;

namespace StardewDruid.Event.Challenge
{
    public class ChallengeStars : EventHandle
    {

        public StardewDruid.Monster.Blobfiend blobking;

        public Vector2 relicPosition = Vector2.Zero;

        public ChallengeStars()
        {

            activeLimit = 60;

            mainEvent = true;

        }

        public override void EventActivate()
        {

            base.EventActivate();

            monsterHandle = new(origin, location);

            monsterHandle.spawnSchedule = new();

            for (int i = 1; i <= 15; i++)
            {

                List<SpawnHandle> blobSpawns = new();

                blobSpawns.Add(new(MonsterHandle.bosses.blobfiend, Boss.temperment.random, Boss.difficulty.medium));

                switch (Mod.instance.randomIndex.Next(3))
                {

                    case 0:

                        blobSpawns.Add(new(MonsterHandle.bosses.blobfiend, Boss.temperment.random, Boss.difficulty.medium));

                        break;

                    case 1:

                        blobSpawns.Add(new(MonsterHandle.bosses.blobfiend, Boss.temperment.coward, Boss.difficulty.basic));

                        break;

                }

                monsterHandle.spawnSchedule.Add(i, blobSpawns);

            }

            monsterHandle.spawnWithin = ModUtility.PositionToTile(origin) - new Vector2(7, 6);

            monsterHandle.spawnRange = new(14, 13);

            EventBar("The Slime Infestation",0);

            eventProximity = 1280;

            cues = DialogueData.DialogueScene(eventId);

            narrators = DialogueData.DialogueNarrators(eventId);

            ModUtility.AnimateHands(Game1.player, Game1.player.FacingDirection, 600);

            Mod.instance.rite.CastMeteors();

            if (Mod.instance.trackers.ContainsKey(CharacterHandle.characters.Effigy))
            {

                voices[1] = Mod.instance.characters[CharacterHandle.characters.Effigy];

                Mod.instance.characters[CharacterHandle.characters.Effigy].Halt();

                Mod.instance.characters[CharacterHandle.characters.Effigy].idleTimer = 300;

            }

        }

        public override void RemoveMonsters()
        {

            if (blobking != null)
            {

                blobking.currentLocation.characters.Remove(blobking);

                blobking.currentLocation = null;

                blobking = null;

            }

            base.RemoveMonsters();

        }

        public override void EventInterval()
        {

            activeCounter++;

            monsterHandle.SpawnCheck();

            if (activeCounter % 5 == 1)
            {

                monsterHandle.SpawnInterval();

            }

            if(blobking != null)
            {

                if (!ModUtility.MonsterVitals(blobking,location))
                {

                    blobking.currentLocation.characters.Remove(blobking);

                    blobking = null;

                    cues.Clear();

                }
                else
                {

                    relicPosition = blobking.Position;

                }

            }

            switch (activeCounter)
            {

                case 14:

                    blobking = new(ModUtility.PositionToTile(origin) - new Vector2(3,3), Mod.instance.CombatDifficulty());

                    blobking.netScheme.Set(2);

                    blobking.SetMode(3);

                    blobking.netPosturing.Set(true);

                    location.characters.Add(blobking);

                    blobking.update(Game1.currentGameTime, location);

                    voices[0] = blobking;

                    Mod.instance.iconData.ImpactIndicator(location, blobking.Position, IconData.impacts.splatter, 5f, new());

                    ModUtility.Explode(location, ModUtility.PositionToTile(blobking.Position), Game1.player, 5, 3);

                    break;

                case 37:

                    blobking.netPosturing.Set(false);

                    blobking.MaxHealth *= 2;

                    blobking.Health = blobking.MaxHealth;

                    EventDisplay bossBar = Mod.instance.CastDisplay(narrators[0], narrators[0]);

                    bossBar.boss = blobking;

                    bossBar.type = EventDisplay.displayTypes.bar;

                    bossBar.colour = Microsoft.Xna.Framework.Color.Red;

                    blobking.focusedOnFarmers = true;

                    break;

                case 59:

                    if(blobking != null)
                    {

                        SpellHandle meteor = new(Game1.player, blobking.Position, 8 * 64, Mod.instance.CombatDamage()*4);

                        meteor.type = spells.orbital;

                        meteor.missile = missiles.meteor;

                        meteor.projectile = 5;

                        meteor.sound = sounds.explosion;

                        meteor.environment = 8;

                        meteor.power = 3;

                        Mod.instance.spellRegister.Add(meteor);

                        blobking.Halt();

                        blobking.idleTimer = 2000;

                        blobking.Health = 1;

                    }

                    break;

                case 60:

                    eventComplete = true;

                    break;

            }

            DialogueCue(activeCounter);

        }

        public override void EventCompleted()
        {

            int slimesKilled = monsterHandle.spawnTotal - monsterHandle.monsterSpawns.Count;

            int friendship = 100;

            friendship += slimesKilled * 8;

            Mod.instance.CastDisplay($"Destroyed {slimesKilled} slimes, gained " + friendship + " friendship with forest residents", 2);

            VillagerData.CommunityFriendship("forest", friendship);

            ThrowHandle throwRelic = new(Game1.player, relicPosition, IconData.relics.runestones_moon);

            throwRelic.register();

            Mod.instance.questHandle.CompleteQuest(eventId);

        }


    }

}
