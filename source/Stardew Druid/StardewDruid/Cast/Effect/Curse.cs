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
using Microsoft.Xna.Framework.Graphics;
using StardewDruid.Data;
using StardewDruid.Event;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Companions;
using StardewValley.Locations;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using static StardewDruid.Cast.SpellHandle;
using static StardewDruid.Data.IconData;


namespace StardewDruid.Cast.Effect
{
    public class Curse : EventHandle
    {

        public Dictionary<StardewValley.Monsters.Monster, CurseTarget> curseTargets = new();

        public Curse()
          : base()
        {

        }

        public void AddTarget(GameLocation location, StardewValley.Monsters.Monster monster, SpellHandle.effects effect)
        {

            if (!ModUtility.MonsterVitals(monster, location))
            {

                return;

            }

            if (curseTargets.ContainsKey(monster))
            {

                return;

            }

            if (monster is Monster.Boss boss)
            {

                effect = boss.IsCursable(effect);

            }

            switch (effect)
            {

                case SpellHandle.effects.none:

                    return;

                case SpellHandle.effects.blind:

                    if(monster is not Monster.Boss)
                    {

                        effect = SpellHandle.effects.daze;

                    }

                    break;

                case SpellHandle.effects.morph:

                    if (monster.isGlider.Value || monster is DustSpirit)
                    {

                        effect = SpellHandle.effects.daze;

                    }

                    DustSpirit morph = new(monster.Position);

                    morph.update(Game1.currentGameTime, location);

                    morph.MaxHealth = monster.MaxHealth * 2;

                    morph.Health = morph.MaxHealth;

                    morph.Scale *= 3;

                    morph.Scale /= 2;

                    location.characters.Add(morph);

                    monster.Health = 0;

                    monster.currentLocation.characters.Remove(monster);

                    monster = morph;

                    break;

                case SpellHandle.effects.knock:

                    if (monster.isGlider.Value)
                    {
                        
                        effect = SpellHandle.effects.daze;

                    }

                    break;


            }

            curseTargets.Add(monster, new(location, monster, 5, effect));

            activeLimit = eventCounter + 5;

        }

        public override void EventRemove()
        {

            for (int g = curseTargets.Count - 1; g >= 0; g--)
            {

                KeyValuePair<StardewValley.Monsters.Monster, CurseTarget> knockTarget = curseTargets.ElementAt(g);

                knockTarget.Value.ShutDown();

            }

            base.EventRemove();

        }

        public override void EventDecimal()
        {

            for (int g = curseTargets.Count - 1; g >= 0; g--)
            {

                KeyValuePair<StardewValley.Monsters.Monster, CurseTarget> knockTarget = curseTargets.ElementAt(g);

                if (!knockTarget.Value.Update())
                {

                    curseTargets.Remove(knockTarget.Key);

                }

            }

        }


    }

    public class CurseTarget
    {

        public GameLocation location;

        public StardewValley.Monsters.Monster victim;

        public TemporaryAnimatedSprite animation;

        public int timer;

        public int warptimer;

        public List<float> stats = new();

        public SpellHandle.effects type;

        public SpellHandle spell;

        public bool cooldown;

        public CurseTarget(GameLocation Location, StardewValley.Monsters.Monster Victim, int Timer, SpellHandle.effects Type = SpellHandle.effects.knock)
        {

            location = Location;

            victim = Victim;

            timer = Timer * 10;

            type = Type;

            IconData.displays display;

            switch (type)
            {

                case SpellHandle.effects.blind:

                    (victim as StardewDruid.Monster.Boss).SetCooldown(3);

                    display = displays.daze;

                    break;

                case SpellHandle.effects.morph:

                    display = displays.morph;

                    break;
                
                case SpellHandle.effects.mug:

                    if(victim.objectsToDrop.Count == 0)
                    {

                        SpawnData.MonsterDrops(victim, (SpawnData.drops)Mod.instance.randomIndex.Next(1,5));

                    }

                    string drop = victim.objectsToDrop[Mod.instance.randomIndex.Next(victim.objectsToDrop.Count)];

                    StardewValley.Object dropItem = new StardewValley.Object(drop, 1);

                    Game1.createItemDebris(dropItem, victim.Position + new Vector2(0, 32), 2, location, -1);

                    display = displays.herbalism;

                    break;
                
                case SpellHandle.effects.daze:

                    stats.Add((float)victim.speed);

                    victim.speed = victim.speed / 2;

                    victim.randomSquareMovement(Game1.currentGameTime);

                    display = displays.daze;

                    break;
                
                case SpellHandle.effects.doom:

                    if((double)victim.Health <= (double)(victim.MaxHealth * 0.07))
                    {

                        PerformInstantDeath();

                    }

                    timer *= 2;

                    display = displays.skull;

                    break;
                
                case SpellHandle.effects.immolate:

                    display = displays.blaze;

                    break;

                default:
                case SpellHandle.effects.knock:

                    display = displays.knock;

                    stats.Add(Victim.rotation);

                    Victim.Halt();

                    Victim.stunTime.Set(Math.Max(Victim.stunTime.Value, Timer * 1000));

                    if (Victim.Sprite.getWidth() > 16)
                    {

                        Victim.rotation = (float)(Math.PI);

                    }
                    else
                    {

                        Victim.rotation = (float)(Math.PI / 2);

                    }

                    break;

            }

            spell = new(Victim, display, timer * 6);

            Mod.instance.spellRegister.Add(spell);

        }

        public void ShutDown()
        {
            
            switch (type)
            {

                case SpellHandle.effects.knock:

                    victim.rotation = stats[0];

                    break;

                case SpellHandle.effects.daze:

                    victim.speed = (int)stats[0];

                    break;

            }

            spell.Shutdown();

            timer = 300;

            cooldown = true;
            
        }

        public bool Update()
        {

            timer--;

            if (timer <= 0)
            {

                if (cooldown)
                {

                    return false;

                }

                ShutDown();

                return true;

            }

            if (!ModUtility.MonsterVitals(victim, location))
            {

                if(type == SpellHandle.effects.immolate)
                {

                    PerformBarbeque();

                }

                ShutDown();

                return false;

            }

            if (victim is StardewValley.Monsters.Mummy mummy)
            {

                if (mummy.reviveTimer.Value > 0)
                {

                    ModUtility.HitMonster(Game1.player, mummy, 1, false);

                    return false;

                }

            }

            switch (type)
            {

                case SpellHandle.effects.daze:

                    if(timer % 10 == 0)
                    {

                        victim.randomSquareMovement(Game1.currentGameTime);

                    }

                    break;

                case SpellHandle.effects.knock:

                    victim.Halt();

                    if(victim.stunTime.Value <= 0)
                    {

                        victim.stunTime.Set(timer * 100);

                    }

                    break;

                case SpellHandle.effects.doom:

                    if(timer == 1)
                    {

                        PerformInstantDeath();

                    }

                    break;

                case SpellHandle.effects.immolate:

                    if (timer % 10 == 5)
                    {

                        ModUtility.HitMonster(Game1.player, victim, Mod.instance.CombatDamage() / 10, false, 0, 0);

                    }

                    break;

            }

            return true;

        }

        public void PerformInstantDeath()
        {

            SpellHandle death = new(Game1.player, new() { victim }, 999);

            death.display = impacts.deathbomb;

            Mod.instance.spellRegister.Add(death);

        }

        public void PerformBarbeque()
        {

            if (new Random().Next(5) == 0)
            {

                Mod.instance.iconData.ImpactIndicator(location, victim.Position, IconData.impacts.immolation, 2f, new());

                int barbeque = SpawnData.RandomBarbeque();

                ThrowHandle throwMeat = new(Game1.player, victim.Position, barbeque);

                throwMeat.register();

            }

        }

    }

}
