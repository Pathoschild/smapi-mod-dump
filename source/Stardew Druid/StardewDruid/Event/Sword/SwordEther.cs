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
using StardewDruid.Location;
using StardewDruid.Monster;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace StardewDruid.Event.Sword
{
    public class SwordEther : EventHandle
    {

        public int activeSection;

        public StardewDruid.Monster.Reaper reaper;

        public Vector2 relicPosition = Vector2.Zero;

        public Warp warpExit;

        public int woundedCounter;

        public SwordEther()
        {

        }

        public override bool TriggerActive()
        {

            if (TriggerLocation())
            {

                EventActivate();

                return true;

            }

            return false;

        }

        public override void EventActivate()
        {

            mainEvent = true;

            base.EventActivate();

            EventBar("The Tomb of Tyrannus",0);

            eventProximity = -1;

            activeLimit = 120;

            cues = DialogueData.DialogueScene(eventId);

            narrators = DialogueData.DialogueNarrators(eventId);

            if (!Mod.instance.trackers.ContainsKey(CharacterHandle.characters.Jester))
            {

                Mod.instance.characters[CharacterHandle.characters.Jester].SwitchToMode(Character.Character.mode.track, Game1.player);

            }

            voices[0] = Mod.instance.characters[CharacterHandle.characters.Jester];

            foreach (KeyValuePair<CharacterHandle.characters, TrackHandle> tracker in Mod.instance.trackers)
            {

                Mod.instance.characters[tracker.Key].Halt();

                Mod.instance.characters[tracker.Key].idleTimer = 300;

            }

            location.warps.Clear();

            warpExit = new Warp(26, 32, "SkullCave", 5, 5, flipFarmer: false);

        }

        public override bool AttemptReset()
        {

            if(location.Name == LocationData.druid_tomb_name)
            {
                
                DialogueCue(991);

                Game1.player.warpFarmer(warpExit, 2);

                Mod.instance.CastMessage("You managed to escape! Enter the tomb to try again.", 3);

            }

            EventRemove();

            eventActive = false;

            eventAbort = false;

            triggerEvent = true;

            activeCounter = 0;

            return true;

        }

        public override bool EventExpire()
        {

            return AttemptReset();

        }

        public override void EventRemove()
        {

            if (reaper != null)
            {

                reaper.currentLocation.characters.Remove(reaper);

                reaper.currentLocation = null;

                reaper = null;

            }

            if (location != null)
            {

                location.updateWarps();

            }

            base.EventRemove();

        }

        public override void EventInterval()
        {

            activeCounter++;

            if (reaper != null)
            {
                
                if (reaper.netWoundedActive.Value)
                {

                    if (woundedCounter == 0)
                    {

                        DialogueCue(992);

                        woundedCounter = 4;

                    }

                    woundedCounter--;

                    if(woundedCounter <= 1)
                    {

                        Mod.instance.iconData.ImpactIndicator(location, reaper.Position, IconData.impacts.deathbomb, 5f, new());

                        reaper.currentLocation.characters.Remove(reaper);

                        reaper = null;


                    }

                    return;

                }
                else if (!ModUtility.MonsterVitals(reaper, location))
                {

                    reaper.netWoundedActive.Set(true);

                    eventComplete = true;

                    return;

                }
                else
                {

                    relicPosition = reaper.Position;

                }

            }

            if (activeCounter == 1)
            {


                reaper = new(ModUtility.PositionToTile(origin), Mod.instance.CombatDifficulty());

                reaper.netScheme.Set(2);

                reaper.SetMode(3);

                reaper.netHaltActive.Set(true);

                reaper.idleTimer = 300;

                location.characters.Add(reaper);

                reaper.update(Game1.currentGameTime, location);

                reaper.setWounded = true;

                voices[1] = reaper;

                EventDisplay bossBar = Mod.instance.CastDisplay(narrators[0], narrators[0]);

                bossBar.boss = reaper;

                bossBar.type = EventDisplay.displayTypes.bar;

                bossBar.colour = Microsoft.Xna.Framework.Color.Red;

            }

            if(activeCounter == 120)
            {

                DialogueCue(991);

                Game1.player.warpFarmer(warpExit, 2);

                Mod.instance.CastMessage("You managed to escape! Try again tomorrow.", 3);

            }

            DialogueCue(activeCounter);

        }

        public override void EventCompleted()
        {

            ThrowHandle swordThrow = new(Game1.player, relicPosition, new StardewValley.Tools.MeleeWeapon("57"));

            swordThrow.register();

            ThrowHandle throwNotes = new(Game1.player, relicPosition, IconData.relics.courtesan_pin);

            throwNotes.delay = 120;

            throwNotes.register();

            Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.courtesan_pin.ToString());

            Mod.instance.questHandle.CompleteQuest(eventId);

        }

    }

}
