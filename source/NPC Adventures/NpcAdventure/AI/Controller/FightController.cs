using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NpcAdventure.Loader;
using NpcAdventure.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NpcAdventure.AI.Controller
{
    internal class FightController : FollowController, Internal.IDrawable
    {
        private const int COOLDOWN_EFFECTIVE_THRESHOLD = 36;
        private const int COOLDOWN_INITAL = 50;
        private const int COOLDOWN_MINIMUM = COOLDOWN_INITAL - COOLDOWN_EFFECTIVE_THRESHOLD;
        private const float DEFEND_TILE_RADIUS = 7f;
        private bool potentialIddle = false;
        private readonly IModEvents events;
        private readonly MeleeWeapon weapon;
        private readonly Dictionary<string, string> bubbles;
        private readonly Character realLeader;
        private readonly float attackRadius;
        private readonly float backupRadius;
        private readonly double fightSpeechTriggerThres;
        private int weaponSwingCooldown = 0;
        private int fightBubbleCooldown = 0;
        private bool defendFistUsed;
        private List<FarmerSprite.AnimationFrame>[] attackAnimation;
        private int attackSpeedPitch = 0;

        public event EventHandler<EventArgs> VisibleChanged;
        public event EventHandler<EventArgs> DrawOrderChanged;

        public int SwingThreshold {
            get {
                int thres = COOLDOWN_EFFECTIVE_THRESHOLD - this.attackSpeedPitch;

                if (thres < 0)
                    return 0;

                return thres;
            }
        }
        public int CooldownTimeout {
            get {
                int timeout = COOLDOWN_INITAL - this.attackSpeedPitch;

                if (timeout < COOLDOWN_MINIMUM)
                    return COOLDOWN_MINIMUM;

                return timeout;
            }
        }

        public FightController(AI_StateMachine ai, IContentLoader content, IModEvents events, int sword) : base(ai)
        {
            this.attackRadius = 1.25f * Game1.tileSize;
            this.backupRadius = 0.9f * Game1.tileSize;
            this.realLeader = ai.player;
            this.leader = null;
            this.pathFinder.GoalCharacter = null;
            this.events = events;
            this.weapon = this.GetSword(sword);
            this.bubbles = content.LoadStrings("Strings/SpeechBubbles");
            this.fightSpeechTriggerThres = ai.Csm.HasSkill("warrior") ? 0.33 : 0.15;

            this.attackAnimation = new List<FarmerSprite.AnimationFrame>[4]
            {
                // Up
                new List<FarmerSprite.AnimationFrame>() {
                    new FarmerSprite.AnimationFrame(8, 100),
                    new FarmerSprite.AnimationFrame(9, 250),
                },
                // Right
                new List<FarmerSprite.AnimationFrame>() {
                    new FarmerSprite.AnimationFrame(6, 100),
                    new FarmerSprite.AnimationFrame(7, 250),
                },
                // Down
                new List<FarmerSprite.AnimationFrame>() {
                    new FarmerSprite.AnimationFrame(0, 100),
                    new FarmerSprite.AnimationFrame(1, 250),
                },
                // Left
                new List<FarmerSprite.AnimationFrame>() {
                    new FarmerSprite.AnimationFrame(14, 100),
                    new FarmerSprite.AnimationFrame(15, 250),
                },
        };
        }

        private int GetSwordIndex(int fallbackSword)
        {
            Farmer farmer = this.ai.player as Farmer;

            switch (farmer?.CombatLevel)
            {
                case 0:
                    return fallbackSword;
                case 1:
                    return 11; // Steel smallsword
                case 2:
                    return 1; // Silver Saber
                case 3:
                    return 15; // Forest sword
                case 4:
                    return 6; // Iron Edge
                case 5:
                    return 49; // Rapier
                case 6:
                    return 10; // Claymore
                case 7:
                    return 14; // Neptune's Glaive
                case 8:
                    return 50; // Steel Falchion
                case 9:
                    return 8; // Obsidian edge
                default:
                    return 4; // Galaxy sword
            }
        }

        private MeleeWeapon GetSword(int sword)
        {
            sword = this.GetSwordIndex(sword);

            return sword >= 0 ? new MeleeWeapon(sword) : null;
        }

        private void World_NpcListChanged(object sender, NpcListChangedEventArgs e)
        {
            if (e.Removed != null && e.Removed.Count() > 0 && e.Removed.Contains(this.leader)) {
                this.leader = null;
                this.pathFinder.GoalCharacter = null;
            }
        }

        public override bool IsIdle => this.CheckIdleState();

        public bool Visible => throw new NotImplementedException();

        public int DrawOrder => throw new NotImplementedException();

        /// <summary>
        /// Checks if spoted monster is a valid monster
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        private bool IsValidMonster(Monster monster)
        {
            // Invisible monsters are invalid
            if (monster.IsInvisible)
                return false;

            // Only moving rock crab is valid
            if (monster is RockCrab crab)
                return crab.isMoving();

            // Only unarmored bug is valid
            if (monster is Bug bug)
                return !bug.isArmoredBug.Value;

            // Only live mummy is valid
            if (monster is Mummy mummy)
                return mummy.reviveTimer.Value <= 0;

            // All other monsters all valid
            return true;
        }

        /// <summary>
        /// Check if is here any monster to fight
        /// </summary>
        private void CheckMonsterToFight()
        {
            Monster monster = Helper.GetNearestMonsterToCharacter(this.follower, DEFEND_TILE_RADIUS);

            if (monster == null || !this.IsValidMonster(monster))
            {
                this.potentialIddle = true;
                this.leader = null;
                this.pathFinder.GoalCharacter = null;
                return;
            }

            this.potentialIddle = false;
            this.leader = monster;
            this.pathFinder.GoalCharacter = this.leader;
            this.DoFightSpeak();
        }

        private void DoFightSpeak()
        {
            // Cooldown not expired? Say nothing
            if (this.fightBubbleCooldown != 0)
                return;

            if (Game1.random.NextDouble() < this.fightSpeechTriggerThres && DialogueHelper.GetBubbleString(this.bubbles, this.follower, "fight", out string text))
            {
                bool isRed = this.ai.Csm.HasSkill("warrior") && Game1.random.NextDouble() < 0.1;
                this.follower.showTextAboveHead(text, isRed ? 2 : -1);
                this.fightBubbleCooldown = 600;
            }
            else if (this.ai.Csm.HasSkill("warrior") && Game1.random.NextDouble() < this.fightSpeechTriggerThres / 2)
            {
                this.follower.clearTextAboveHead();
                this.follower.doEmote(12);
                this.fightBubbleCooldown = 550;
            }
            else if (Game1.random.NextDouble() > (this.fightSpeechTriggerThres + this.fightSpeechTriggerThres * .33f))
            {
                this.follower.clearTextAboveHead();
                this.follower.doEmote(16);
                this.fightBubbleCooldown = 500;
            }
        }

        public bool CheckIdleState()
        {
            // Go iddle instantly when companion and monster is in different locations
            if (this.leader != null && this.follower.currentLocation != this.leader.currentLocation)
                return true;

            // Don't go iddle when cooldown is not under threshold (swing animation plays)
            if (this.weaponSwingCooldown > this.SwingThreshold)
                return false;

            // Go iddle instantly when farmer is too far
            if (Helper.Distance(this.realLeader.getTileLocationPoint(), this.follower.getTileLocationPoint()) > 11f)
                return true;

            // Go iddle instantly when potential monster is not valid
            if (this.leader is Monster && !this.IsValidMonster(this.leader as Monster))
                return true;

            return this.potentialIddle; // By default propagate potential iddle state as iddle state
        }

        public void DoWeaponSwing()
        {
            if (this.weaponSwingCooldown > 0)
            {
                this.weaponSwingCooldown--;
            }

            if (this.weaponSwingCooldown > this.SwingThreshold && !this.defendFistUsed)
            {
                this.DoDamage();
            }
        }

        public override void Update(UpdateTickedEventArgs e)
        {
            if (this.IsIdle || (!Context.IsPlayerFree && !Context.IsMultiplayer))
                return;

            if (this.leader == null)
                this.CheckMonsterToFight();

            if (this.fightBubbleCooldown > 0)
                --this.fightBubbleCooldown;

            this.DoWeaponSwing();
            base.Update(e);
        }

        /// <summary>
        /// Try to give damage to a monster
        /// </summary>
        private void DoDamage(bool criticalFist = false)
        {
            if (this.leader == null)
                return;

            Rectangle effectiveArea = this.follower.GetBoundingBox();
            Rectangle enemyBox = this.leader.GetBoundingBox();
            Rectangle companionBox = this.follower.GetBoundingBox();
            WeaponAttributes attrs = new WeaponAttributes();

            if (!criticalFist && this.weapon != null)
            {
                attrs.SetFromWeapon(this.weapon);
            }

            if (criticalFist)
            {
                attrs.knockBack *= 3.6f;
                attrs.smashAround /= 2f;
            }

            if (this.ai.Csm.HasSkill("warrior"))
            {
                // Enhanced skills ONLY for WARRIORS
                attrs.minDamage += (int)Math.Round(attrs.minDamage * .03f); // 3% added min damage
                attrs.knockBack += attrs.knockBack * (Game1.random.Next(2, 5) / 100); // 2-5% added knock back
                attrs.addedEffectiveArea += (int)Math.Round(attrs.addedEffectiveArea * .01f); // 1% added effective area
                attrs.critChance += Math.Max(0, (float)Game1.player.DailyLuck / 2); // added critical chance is half of daily luck. If luck is negative, no added critical chance
            }

            if (criticalFist && this.follower.FacingDirection != 0)
            {
                this.follower.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 960, 128, 128), 60f, 4, 0, this.follower.Position, false, this.follower.FacingDirection == 3, 1f, 0.0f, Color.White, .5f, 0.0f, 0.0f, 0.0f, false));
            }

            companionBox.Inflate(4, 4); // Personal space
            effectiveArea.Inflate((int)(effectiveArea.Width * attrs.smashAround + attrs.addedEffectiveArea), (int)(effectiveArea.Height * attrs.smashAround + attrs.addedEffectiveArea));

            if (!criticalFist && !this.defendFistUsed && this.ai.Csm.HasSkill("warrior") && companionBox.Intersects(enemyBox) && this.weaponSwingCooldown == this.CooldownTimeout)
            {
                this.ai.Monitor.Log("Critical dangerous: Using defense fists!");
                this.defendFistUsed = true;
                this.DoDamage(true); // Force fist when no damage given to a monster with weapon
                return;
            }

            if (this.follower.currentLocation.damageMonster(effectiveArea, attrs.minDamage, attrs.maxDamage, false, attrs.knockBack, attrs.addedPrecision, attrs.critChance, attrs.critMultiplier, !criticalFist, this.realLeader as Farmer))
            {
                if (criticalFist)
                {
                    this.follower.currentLocation.playSound("clubSmash");
                }
                else
                {
                    this.follower.currentLocation.playSound("clubhit");
                }

                if (criticalFist || (Game1.random.NextDouble() > .7f && Game1.random.NextDouble() < .3f))
                {
                    this.DoFightSpeak();
                }
            }
        }

        /// <summary>
        /// Do swipe sword effects
        /// </summary>
        /// <param name="type"></param>
        /// <param name="facingDirection"></param>
        /// <param name="c"></param>
        public void DoSwipe(int type, int facingDirection, Character c)
        {
            this.WeaponUpdate(facingDirection, 0, c);

            switch(type)
            {
                case 0:
                case 1:
                    c.currentLocation.localSound("daggerswipe");
                    break;
                case 2:
                    c.currentLocation.localSound("clubswipe");
                    break;
                case 3:
                    c.currentLocation.localSound("swordswipe");
                    break;
            }
        }

        private void WeaponUpdate(int direction, int farmerMotionFrame, Character who)
        {
            int offset = 0;
            switch (direction)
            {
                case 0:
                    offset = 3;
                    break;
                case 1:
                    offset = 2;
                    break;
                case 3:
                    offset = 2;
                    break;
            }
            if (farmerMotionFrame < 1)
            {
                this.weapon.CurrentParentTileIndex = this.weapon.InitialParentTileIndex;
            }
            else if (who.FacingDirection == 0 || (who.FacingDirection == 2 && farmerMotionFrame >= 2))
            {
                this.weapon.CurrentParentTileIndex = this.weapon.InitialParentTileIndex + 1;
            }
            this.weapon.CurrentParentTileIndex += offset;
        }

        private int GetAttackPitch()
        {
            Farmer farmer = this.ai.player as Farmer;
            int weaponSpeed = this.weapon?.speed.Value ?? 400;
            int swipeDelay = (400 - weaponSpeed) / 4;
            int combatLevel = farmer?.combatLevel ?? 0;
            double skill = combatLevel * Math.Log(Math.Pow(combatLevel, 2) + 1) + Math.Pow(combatLevel, 2) + combatLevel;

            return (int)Math.Round(skill) + Game1.random.Next(-10, 10) - swipeDelay + (int)Math.Round(Game1.player.DailyLuck);
        }

        private void AnimateMe()
        {
            this.attackSpeedPitch = this.GetAttackPitch();
            this.weaponSwingCooldown = this.CooldownTimeout;
            this.defendFistUsed = false;
            this.follower.Sprite.StopAnimation();
            this.follower.Sprite.faceDirectionStandard(this.GetFacingDirectionFromMovement(new Vector2(this.leader.Position.X, this.leader.Position.Y)));
            this.follower.Sprite.setCurrentAnimation(this.attackAnimation[this.follower.FacingDirection]);

            if (this.weapon != null)
            {
                this.DoSwipe(this.weapon.type.Value, this.follower.FacingDirection, this.follower);
            }

            this.DoDamage();
        }

        protected override float GetMovementSpeedBasedOnDistance(float distanceFromTarget)
        {
            if (this.weaponSwingCooldown > this.SwingThreshold)
                return 0;

            if (distanceFromTarget <= this.backupRadius)
            {
                if (this.weaponSwingCooldown == 0)
                {
                    this.AnimateMe();
                }

                return -0.65f;
            }
            else if (distanceFromTarget <= this.attackRadius)
            {
                if (this.weaponSwingCooldown == 0)
                {
                    this.AnimateMe();
                }

                return Math.Max(this.speed - 0.1f, 0.1f);
            }

            return 5.28f;
        }

        protected override void PathfindingRemakeCheck()
        {
            base.PathfindingRemakeCheck();

            if (this.pathToFollow == null)
            {
                this.potentialIddle = true;
                this.ai.Monitor.Log($"Fight controller iddle, because can't find a path to monster '{this.leader?.Name}'");
            }
        }

        public override void Activate()
        {
            this.events.World.NpcListChanged += this.World_NpcListChanged;
            this.weaponSwingCooldown = 0;
            this.fightBubbleCooldown = 0;
            this.potentialIddle = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (this.weapon != null && this.weaponSwingCooldown > this.SwingThreshold && !this.defendFistUsed)
            {
                int frames = this.weapon.type.Value == 3 ? 4 : 7;
                int duration = this.CooldownTimeout - this.SwingThreshold;
                int tick = Math.Abs(this.weaponSwingCooldown - this.CooldownTimeout);
                int currentFrame = this.CurrentFrame(tick, duration, frames);

                Helper.DrawDuringUse(currentFrame, this.follower.FacingDirection, spriteBatch, this.follower.getLocalPosition(Game1.viewport), this.follower, MeleeWeapon.getSourceRect(this.weapon.InitialParentTileIndex), this.weapon.type.Value, this.weapon.isOnSpecial);
            }
        }

        private int CurrentFrame(int tick, int duration, int frames)
        {
            return (tick % duration) / (duration / frames);
        }

        public override void Deactivate()
        {
            this.follower.Sprite.StopAnimation();
            this.events.World.NpcListChanged -= this.World_NpcListChanged;
            this.leader = null;
            this.pathFinder.GoalCharacter = null;
            this.potentialIddle = true;
        }

        private class WeaponAttributes
        {
            public int minDamage = 1;
            public int maxDamage = 3;
            public int addedPrecision = 0;
            public float critChance = 0f;
            public float critMultiplier = .2f;
            public float knockBack = 1;
            public float smashAround = 1.5f;
            public int addedEffectiveArea = 0;

            public void SetFromWeapon(MeleeWeapon weapon)
            {
                this.minDamage = weapon.minDamage.Value;
                this.maxDamage = weapon.maxDamage.Value;
                this.knockBack = weapon.knockback.Value;
                this.critChance = weapon.critChance.Value;
                this.critMultiplier = weapon.critMultiplier.Value;
                this.addedPrecision = weapon.addedPrecision.Value;
                this.addedEffectiveArea = weapon.addedAreaOfEffect.Value;
            }
        }
    }
}
