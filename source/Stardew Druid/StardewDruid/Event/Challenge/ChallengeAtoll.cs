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
using StardewDruid.Cast.Effect;
using StardewDruid.Cast.Mists;
using StardewDruid.Character;
using StardewDruid.Data;
using StardewDruid.Journal;
using StardewDruid.Location;
using StardewDruid.Monster;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using static StardewDruid.Cast.SpellHandle;
using static StardewDruid.Data.IconData;

namespace StardewDruid.Event.Challenge
{
    public class ChallengeAtoll : EventHandle
    {

        public StardewDruid.Monster.Phantom bossPhantom;

        public Vector2 relicPosition = Vector2.Zero;

        public List<Vector2> cannonPositions = new();

        public ChallengeAtoll()
        {

            activeLimit = 70;

            mainEvent = true;

        }

        public override void EventActivate()
        {

            base.EventActivate();

            monsterHandle = new(origin, location);

            monsterHandle.spawnSchedule = new();

            for (int i = 1; i <= 12; i++)
            {

                monsterHandle.spawnSchedule.Add(
                    i, 
                    new() { 
                        new(MonsterHandle.bosses.phantom, Boss.temperment.random, Boss.difficulty.medium), 
                        new(MonsterHandle.bosses.phantom, Boss.temperment.random, Boss.difficulty.medium), 
                    }
                );

            }

            monsterHandle.spawnWithin = ModUtility.PositionToTile(origin) - new Vector2(8, 4);

            monsterHandle.spawnRange = new(16, 7);

            EventBar("The Lost Seafarers",0);

            SetTrack("PIRATE_THEME");

            eventProximity = -1;

            cues = DialogueData.DialogueScene(eventId);

            narrators = DialogueData.DialogueNarrators(eventId);

            ModUtility.AnimateHands(Game1.player, Game1.player.FacingDirection, 600);

            //Mod.instance.iconData.AnimateBolt(location, origin);

            Mod.instance.spellRegister.Add(new(origin, 128, IconData.impacts.puff, new()) { type = SpellHandle.spells.bolt });

            if (Mod.instance.trackers.ContainsKey(CharacterHandle.characters.Effigy))
            {

                voices[1] = Mod.instance.characters[CharacterHandle.characters.Effigy];

                Mod.instance.characters[CharacterHandle.characters.Effigy].Halt();

                Mod.instance.characters[CharacterHandle.characters.Effigy].idleTimer = 300;

            }

            location.playSound("thunder_small");

            cannonPositions = new()
            {
                monsterHandle.spawnWithin + new Vector2(1,9),

                monsterHandle.spawnWithin + new Vector2(7,9),

                monsterHandle.spawnWithin + new Vector2(13,9),

                monsterHandle.spawnWithin + new Vector2(-2,4),

                monsterHandle.spawnWithin + new Vector2(4,4),

                monsterHandle.spawnWithin + new Vector2(10,4),

                monsterHandle.spawnWithin + new Vector2(16,4),

            };


            Wisps wispNew = new();

            wispNew.EventSetup(Game1.player.Position, "wisps");

            wispNew.EventActivate();

            wispNew.WispArray();

            wispNew.eventLocked = true;

            location.warps.Clear();

            (location as Atoll).ambientDarkness = true;

        }

        public override void RemoveMonsters()
        {

            if (bossPhantom != null)
            {

                bossPhantom.currentLocation.characters.Remove(bossPhantom);

                bossPhantom.currentLocation = null;

                bossPhantom = null;

            }

            if (location != null)
            {

                location.updateWarps();

            }

            base.RemoveMonsters();

        }

        public override void EventInterval()
        {

            activeCounter++;

            monsterHandle.SpawnCheck();

            if (bossPhantom != null)
            {

                if (!ModUtility.MonsterVitals(bossPhantom, location))
                {

                    bossPhantom.currentLocation.characters.Remove(bossPhantom);

                    bossPhantom = null;

                    cues.Clear();

                }
                else
                {

                    relicPosition = bossPhantom.Position;

                }

            }

            switch (activeCounter)
            {

                case 1:

                    bossPhantom = new(ModUtility.PositionToTile(origin) - new Vector2(3, 2), Mod.instance.CombatDifficulty());

                    bossPhantom.SetMode(3);

                    bossPhantom.netPosturing.Set(true);

                    location.characters.Add(bossPhantom);

                    bossPhantom.update(Game1.currentGameTime, location);

                    bossPhantom.LookAtFarmer();

                    bossPhantom.fadeFactor = 0.75f;

                    voices[0] = bossPhantom;

                    SpawnData.MonsterDrops(bossPhantom, SpawnData.drops.seafarer);

                    break;

                case 49:

                    bossPhantom.netPosturing.Set(false);

                    bossPhantom.specialInterval = 30;

                    EventDisplay bossBar = Mod.instance.CastDisplay(narrators[0], narrators[0]);

                    bossBar.boss = bossPhantom;

                    bossBar.type = EventDisplay.displayTypes.bar;

                    bossBar.colour = Microsoft.Xna.Framework.Color.Red;

                    bossPhantom.focusedOnFarmers = true;

                    break;

                case 70:

                    eventComplete = true;

                    break;


            }

            if (activeCounter % 5 == 0 && activeCounter <= 65)
            {

                monsterHandle.SpawnInterval();

            }

            switch (activeCounter)
            {

                case 10: CannonsAtTheReady(); break;
                case 13: CannonsToFire(); break;

                case 28: CannonsAtTheReady(); break;
                case 31: CannonsToFire(); break;

                case 43: CannonsAtTheReady(); break;
                case 46: CannonsToFire(); break;

                default: break;

            }

            DialogueCue(activeCounter);

        }

        public override void EventCompleted()
        {

            ThrowHandle throwRelic = new(Game1.player, relicPosition, IconData.relics.runestones_farm);

            throwRelic.register();

            Mod.instance.questHandle.CompleteQuest(eventId);

        }


        public void CannonsToFire()
        {

            DialogueCue(995);

            for (int k = 0; k < cannonPositions.Count; k++)
            {

                Vector2 impact = cannonPositions[k] * 64;

                SpellHandle missile = new(location, impact, new(impact.X > 27*64 ? 55*64 : 0,impact.Y), 192, bossPhantom.GetThreat(), Mod.instance.CombatDamage());

                missile.type = SpellHandle.spells.ballistic;

                missile.projectile = 3;

                missile.missile = missiles.cannonball;

                missile.display = IconData.impacts.bomb;

                missile.scheme = IconData.schemes.death;

                missile.indicator = IconData.cursors.death;

                if(k % 2 == 0)
                {

                    missile.sound = sounds.explosion;

                }

                Mod.instance.spellRegister.Add(missile);

            }

            foreach (StardewValley.Monsters.Monster monsterSpawn in monsterHandle.monsterSpawns)
            {

                if (monsterSpawn is Phantom phantom)
                {

                    DialogueData.DisplayText(phantom, -1, 2);

                }

            }
        }

        public void CannonsAtTheReady()
        {

            DialogueCue(994);

            bossPhantom.netSpecialActive.Set(true);

            bossPhantom.specialTimer = 300;

            bossPhantom.specialInterval = 180;

            for (int k = 0; k < cannonPositions.Count; k++)
            {

                Vector2 impact = cannonPositions[k] * 64;

                CursorAdditional addEffects = new() { interval = 3000, scale = 3, scheme = IconData.schemes.death, alpha = 0.4f, };

                animations.Add(Mod.instance.iconData.CursorIndicator(location, impact, IconData.cursors.death, addEffects));

            }

            foreach (StardewValley.Monsters.Monster monsterSpawn in monsterHandle.monsterSpawns)
            {

                if (monsterSpawn is Phantom phantom)
                {

                    DialogueData.DisplayText(phantom, -1, 2);

                }

            }

        }

    }

}
