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
using StardewDruid.Event;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;



namespace StardewDruid.Cast.Effect
{
    public class WarpStrike : EventHandle
    {

        public Dictionary<StardewValley.Monsters.Monster, WarpTarget> warpTargets = new();

        public WarpStrike()
        {

        }

        public override void EventActivate()
        {

            base.EventActivate();

            Mod.instance.RegisterClick(eventId, 2);

        }

        public virtual void AddTarget(GameLocation Location, StardewValley.Monsters.Monster Victim, int Timer)
        {
            
            if (!ModUtility.MonsterVitals(Victim, Location))
            {
                return;
            }

            if (warpTargets.ContainsKey(Victim))
            {

                return;

            }

            warpTargets.Add(Victim, new(Location, Victim, Timer));

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + Timer + 1;

        }

        public override bool EventPerformAction(SButton Button, string Type)
        {

            if (Type != "Action")
            {
                
                return false;

            }

            if (!EventActive())
            {
                
                return false;

            }

            List<StardewValley.Monsters.Monster> victims = ModUtility.MonsterProximity(Game1.player.currentLocation, new() { Game1.player.Position }, 640, true);

            if(victims.Count == 0)
            {
                return false;

            }

            List<int> directions = new()
            {
                (Game1.player.FacingDirection * 2 + 7) % 8,
                Game1.player.FacingDirection * 2,
                (Game1.player.FacingDirection * 2 + 1),

            };

            foreach(StardewValley.Monsters.Monster victim in victims)
            {

                if (!warpTargets.ContainsKey(victim))
                {
                    continue;

                }

                int direction = ModUtility.DirectionToTarget(Game1.player.Position, victim.Position)[2];

                if(!directions.Contains(direction))
                {

                    continue;

                }

                WarpTarget warpTarget = warpTargets[victim];

                if (!warpTarget.Update())
                {
                    
                    warpTargets.Remove(victim);

                    continue;

                }

                int between = ModUtility.DirectionToTarget(victim.Position, Game1.player.Position)[2];

                List<Vector2> strikeVectors = ModUtility.GetOccupiableTilesNearby(location, ModUtility.PositionToTile(victim.Position), between, 1, 1);

                if (strikeVectors.Count == 0)
                {

                    continue;

                }

                Mod.instance.iconData.AnimateQuickWarp(location, Game1.player.Position, true);

                Game1.player.Position = strikeVectors.First() * 64;

                /*if (!Mod.instance.eventRegister.ContainsKey("shield"))
                {

                    ShieldEvent shieldEvent = new(Game1.player.Position);

                    shieldEvent.EventTrigger();

                }*/

                ModUtility.DamageMonsters(Game1.player.currentLocation, new() { victim }, Game1.player, Mod.instance.CombatDamage(), 0.25f, 2f,true);

                warpTarget.consumed = true;

                return true;

            }

            return false;

        }

        public override void EventRemove()
        {

            for (int g = warpTargets.Count - 1; g >= 0; g--)
            {

                KeyValuePair<StardewValley.Monsters.Monster, WarpTarget> warpTarget = warpTargets.ElementAt(g);

                warpTarget.Value.ShutDown();

            }

            base.EventRemove();

        }

        public override void EventDecimal()
        {

            if (!EventActive())
            {

                return;

            }

            for (int g = warpTargets.Count - 1; g >= 0; g--)
            {

                KeyValuePair<StardewValley.Monsters.Monster, WarpTarget> warpTarget = warpTargets.ElementAt(g);

                if (!warpTarget.Value.Update())
                {

                    warpTargets.Remove(warpTarget.Key);

                }

            }

        }

    }

    public class WarpTarget
    {

        public GameLocation location;

        public StardewValley.Monsters.Monster victim;

        public TemporaryAnimatedSprite animation;

        public int timer;

        public bool consumed;

        public WarpTarget(GameLocation Location, StardewValley.Monsters.Monster Victim, int Timer)
        {
            
            location = Location;

            victim = Victim;

            timer = Timer;

            TargetIcon();

        }

        public void ShutDown()
        {

            location.TemporarySprites.Remove(animation);

        }

        public bool Update()
        {

            if(!ModUtility.MonsterVitals(victim, location) || consumed)
            {

                ShutDown();

                return false;

            }

            if (victim is StardewValley.Monsters.Mummy mummy)
            {

                if (mummy.reviveTimer.Value > 0)
                {

                    ModUtility.HitMonster(location, Game1.player, mummy, 1, false);

                    ShutDown();

                    return false;

                }

            }

            Microsoft.Xna.Framework.Rectangle box = victim.GetBoundingBox();

            Point center = box.Center;

            Vector2 animateAt = center.ToVector2() - new Vector2(16, 0) + new Vector2(0, 32);

            animation.Position = animateAt;

            return true;

        }

        public void TargetIcon()
        {

            Microsoft.Xna.Framework.Rectangle box = victim.GetBoundingBox();

            Point center = box.Center;

            /*TemporaryAnimatedSprite warpAnimate = new(0, timer*1000, 1, 1, center.ToVector2() - new Vector2(16, 0) + new Vector2(0, 32), false, false)
            {

                sourceRect = new(0, 0, 32, 32),

                sourceRectStartingPos = new Vector2(0, 0),

                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Target.png")),

                scale = 1.5f,

                layerDepth = 999f,

                alpha = 0.75f,

                color = new(0.4f, 0, 0.4f, 1f),

                rotation = (float)Math.PI,

                Parent = victim.currentLocation,

            };

            location.temporarySprites.Add(warpAnimate);

            animation = warpAnimate;*/

        }

    }

}
