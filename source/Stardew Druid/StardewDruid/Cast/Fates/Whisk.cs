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
using StardewDruid.Data;
using StardewDruid.Event;
using StardewDruid.Journal;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewDruid.Cast.Fates
{
    public class Whisk : EventHandle
    {

        public SpellHandle whiskSpell;

        public List<SpellHandle> warpSpells = new();

        public bool whiskreturn;

        public Vector2 destination;

        public int strikeTimer;

        public Whisk()
        {

        }

        public override void EventSetup(Vector2 target, string id, bool trigger = false)
        {
            
            origin = target;

            destination = origin;

            eventId = id;

            staticEvent = true;

            location = Game1.player.currentLocation;

            Mod.instance.RegisterEvent(this,eventId);

            Mod.instance.RegisterClick(eventId, 1);

            WhiskSetup();

        }

        public virtual void WhiskSetup()
        {

            whiskSpell = new(location, origin, Game1.player.Position, 64);

            whiskSpell.type = SpellHandle.spells.missile;

            whiskSpell.missile = IconData.missiles.whisk;

            whiskSpell.indicator = IconData.cursors.fates;

            whiskSpell.instant = true;

            whiskSpell.projectile = 4;

            whiskSpell.projectileSpeed = 2;

            whiskSpell.projectilePeak = -1;

            Mod.instance.spellRegister.Add(whiskSpell);

        }

        public override bool StaticActive()
        {

            if (Game1.player.currentLocation.Name != location.Name)
            {

                eventComplete = true;

            }

            if (whiskSpell.shutdown)
            {

                eventComplete = true;

            }

            if (eventComplete)
            {

                return false;

            }

            return true;

        }

        public override bool EventPerformAction(SButton Button, actionButtons Action = actionButtons.action)
        {

            if (Action == actionButtons.special)
            {

                return true;

            }

            if (!StaticActive())
            {

                return false;

            }

            if (whiskreturn)
            {

                PerformStrike();
   
                return false;

            }

            if (Action == actionButtons.action)
            {

                Vector2 distance = (origin - Game1.player.Position) * ((int)(whiskSpell.counter / 10) + 1) / 13;

                List<Vector2> whiskTiles = ModUtility.GetTilesBetweenPositions(location, origin, Game1.player.Position + distance);

                for (int i = 0; i < whiskTiles.Count; i++)
                {

                    if (ModUtility.TileAccessibility(location, whiskTiles[i]) != 0)
                    {

                        continue;

                    }

                    destination = whiskTiles[i] * 64;

                }

            }

            if (PerformStrike())
            {

                return false;

            }

            PerformWarp();

            return false;

        }

        public bool PerformStrike()
        {

            if (!Mod.instance.questHandle.IsGiven(Journal.QuestHandle.fatesTwo))
            {

                return false;

            }

            if (Mod.instance.eventRegister.ContainsKey("curse"))
            {

                if (Mod.instance.eventRegister["curse"] is Cast.Effect.Curse curseEffect)
                {

                    List<StardewValley.Monsters.Monster> validTargets = new();

                    if (curseEffect.curseTargets.Count > 0)
                    {

                        int delay = 0;

                        for (int g = curseEffect.curseTargets.Count - 1; g >= 0; g--)
                        {

                            KeyValuePair<StardewValley.Monsters.Monster, CurseTarget> curseTarget = curseEffect.curseTargets.ElementAt(g);

                            if (!Utility.isOnScreen(curseTarget.Key.Position,64))
                            {
                                
                                continue;
                            
                            }


                            int strikes = 1;

                            if (Mod.instance.questHandle.IsComplete(QuestHandle.fatesTwo))
                            {

                                List<float> critical = Mod.instance.CombatCritical();

                                if (Mod.instance.randomIndex.NextDouble() <= (double)critical[0])
                                {

                                    strikes = 2 + (int)critical[1];

                                }

                            }

                            List<int> directions = new() { 0, 1, 2, 3, 4, 5, 6, 7, };

                            for (int i = 0; i < strikes; i++)
                            {

                                SpellHandle sweep = new(Game1.player, new() { curseTarget.Key }, Mod.instance.CombatDamage());

                                sweep.type = SpellHandle.spells.warpstrike;

                                sweep.counter = 0 - delay;

                                int d = directions[Mod.instance.randomIndex.Next(directions.Count)];

                                directions.Remove(d);

                                sweep.projectile = d;

                                if(i == 0)
                                {
                                    sweep.display = IconData.impacts.slash;

                                }

                                Mod.instance.spellRegister.Add(sweep);

                                warpSpells.Add(sweep);

                                delay += 15;

                            }

                        }

                        staticEvent = false;

                        eventActive = true;

                        activeLimit = 5;

                        Game1.displayFarmer = false;

                        Game1.player.temporarilyInvincible = true;

                        Game1.player.temporaryInvincibilityTimer = 1;

                        Game1.player.currentTemporaryInvincibilityDuration = 5000;
                        
                        return true;

                    }

                }

            }

            return false;

        }

        public void PerformWarp()
        {

            if (!Mod.instance.questHandle.IsComplete(QuestHandle.fatesOne))
            {

                Mod.instance.questHandle.UpdateTask(QuestHandle.fatesOne, 1);

            }

            SpellHandle teleport = new(Game1.player.currentLocation, destination, Game1.player.Position);

            teleport.type = SpellHandle.spells.teleport;

            teleport.instant = true;

            teleport.Update();

            Mod.instance.spellRegister.Add(teleport);

            whiskSpell.Shutdown();

            eventComplete = true;

        }

        public override void EventDecimal()
        {

            if (!EventActive())
            {

                return;

            }




            for(int s = warpSpells.Count - 1; s >= 0; s--) 
            {

                SpellHandle warpSpell = warpSpells[s];

                if (warpSpell.shutdown)
                {

                    warpSpells.Remove(warpSpell);

                }

            }

            if (warpSpells.Count == 0)
            {

                Game1.displayFarmer = true;

                Game1.player.temporarilyInvincible = false;

                Game1.player.temporaryInvincibilityTimer = 0;

                Game1.player.currentTemporaryInvincibilityDuration = 0;

                Game1.player.stopGlowing();

                PerformWarp();

                eventComplete = true;

            }

        }

        public override void EventRemove()
        {
            base.EventRemove();

            Game1.displayFarmer = true;

        }

    }

}
