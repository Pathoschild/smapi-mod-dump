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
using static StardewDruid.Cast.SpellHandle;
using static StardewDruid.Data.IconData;

namespace StardewDruid.Event.Challenge
{
    public class ChallengeDragon : EventHandle
    {

        public int activeSection;

        public StardewDruid.Monster.Dragon terror;

        public Vector2 relicPosition = Vector2.Zero;

        public Warp warpExit;

        public ChallengeDragon()
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

            EventBar("The Terror Beneath",0);

            eventProximity = -1;

            activeLimit = 120;

            cues = DialogueData.DialogueScene(eventId);

            narrators = DialogueData.DialogueNarrators(eventId);

        }

        public override bool AttemptReset()
        {

            if(location.Name == LocationData.druid_vault_name)
            {

                Game1.player.warpFarmer(warpExit, 2);

                Mod.instance.CastMessage("You managed to escape! Enter the lair to try again.", 3);

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

            if (terror != null)
            {

                terror.currentLocation.characters.Remove(terror);

                terror.currentLocation = null;

                terror = null;

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

            if (terror != null)
            {

                if (!ModUtility.MonsterVitals(terror, location))
                {

                    terror.currentLocation.characters.Remove(terror);

                    terror = null;

                    cues.Clear();

                    eventComplete = true;

                    return;

                }
                else
                {

                    relicPosition = terror.Position;

                }

            }

            if (activeCounter == 1)
            {

                foreach(KeyValuePair< CharacterHandle.characters,TrackHandle> tracker in Mod.instance.trackers)
                {

                    Mod.instance.characters[tracker.Key].Halt();

                    Mod.instance.characters[tracker.Key].idleTimer = 300;

                }

                location.warps.Clear();

                warpExit = new Warp(26, 32, "Mine", 17, 6, flipFarmer: false);

                terror = new(ModUtility.PositionToTile(origin), Mod.instance.CombatDifficulty());

                terror.netScheme.Set(2);

                terror.SetMode(3);

                terror.netHaltActive.Set(true);

                terror.idleTimer = 300;

                location.characters.Add(terror);

                terror.update(Game1.currentGameTime, location);

                voices[0] = terror;

                EventDisplay bossBar = Mod.instance.CastDisplay(narrators[0], narrators[0]);

                bossBar.boss = terror;

                bossBar.type = EventDisplay.displayTypes.bar;

                bossBar.colour = Microsoft.Xna.Framework.Color.Red;

                location.playSound("DragonRoar");

            }

            if(activeCounter == 4)
            {

                location.playSound("DragonRoar");

            }

            DialogueCue(activeCounter);

        }

        public override void EventCompleted()
        {
            
            ThrowHandle throwRelic = new(Game1.player, relicPosition, IconData.relics.runestones_cat);

            throwRelic.register();

            Mod.instance.questHandle.CompleteQuest(eventId);

            (location as Vault).AddCrateTile(24, 10, 1);

            (location as Vault).AddCrateTile(28, 10, 2);

            (location as Vault).AddCrateTile(32, 10, 3);

        }

    }

}
