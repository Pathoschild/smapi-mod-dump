/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NpcAdventure.Compatibility;
using NpcAdventure.Dialogues;
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

namespace NpcAdventure.AI.Controller
{
    internal class FightController : FollowController, PurrplingCore.Internal.IDrawable
    {
        public const float DEFEND_TILE_RADIUS = 6f;
        public const float DEFEND_TILE_RADIUS_WARRIOR = 9f;

        private const int COOLDOWN_EFFECTIVE_THRESHOLD = 36;
        private const int COOLDOWN_INITAL = 50;
        private const int COOLDOWN_MINIMUM = COOLDOWN_INITAL - COOLDOWN_EFFECTIVE_THRESHOLD;
        private const float FARMER_AGGRO_RADIUS = 48f;
        private const float RETURN_RADIUS = 11f * Game1.tileSize;
        private const float PATH_NULL_RETURN_RADIUS = 4f * Game1.tileSize;
        private bool potentialIdle = false;
        private readonly IModEvents events;
        private readonly MeleeWeapon weapon;
        private readonly Dictionary<string, string> bubbles;
        private readonly Character realLeader;
        private readonly float attackRadius;
        private readonly float backupRadius;
        private readonly double fightSpeechTriggerThres;
        private readonly List<FarmerSprite.AnimationFrame>[] attackAnimation;
        private int weaponSwingCooldown = 0;
        private int fightBubbleCooldown = 0;
        private int fistCoolDown = 0;
        private bool defendFistUsed;
        private int attackSpeedPitch = 0;
        private WeaponAttributes weaponAttrs;
        private Dictionary<string, string> kissFrames;

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
            this.realLeader = ai.farmer;
            this.leader = null;
            this.pathFinder.GoalCharacter = null;
            this.events = events;
            this.weapon = this.GetSword(sword);
            this.bubbles = content.LoadStrings("Strings/SpeechBubbles");
            this.fightSpeechTriggerThres = ai.Csm.HasSkill("warrior") ? 0.25 : 0.7;

            if (Compat.IsModLoaded(ModUids.PACIFISTMOD_UID))
            {
                this.kissFrames = this.ai.ContentLoader.LoadData<string, string>("Data/FightFrames");
            }

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
            Farmer farmer = this.ai.farmer;
            int level = farmer?.CombatLevel ?? 0;
            var swords = this.ai.ContentLoader.LoadMergedData<int, string>(
                $"Data/Weapons/{this.follower.Name}", 
                "Data/Weapons");

            if (level == 0 && fallbackSword != -2)
            {
                return fallbackSword;
            }

            if (swords.TryGetValue(level, out string swordName))
            {
                int swordId = Helper.GetSwordId(swordName);

                if (swordId < 0 && swordName != "-1")
                    this.ai.Monitor.Log($"Unknown sword: {swordName}", LogLevel.Error);

                return swordId;
            }

            return -1;
        }

        private MeleeWeapon GetSword(int fallBackSword)
        {
            fallBackSword = this.GetSwordIndex(fallBackSword);

            return fallBackSword >= 0 ? new MeleeWeapon(fallBackSword) : null;
        }

        private void World_NpcListChanged(object sender, NpcListChangedEventArgs e)
        {
            if (e.Removed != null && e.Removed.Count() > 0 && e.Removed.Contains(this.leader)) {
                this.leader = null;
                this.pathFinder.GoalCharacter = null;
            }
        }

        public override bool IsIdle => this.CheckIdleState();

        /// <summary>
        /// Check aggro fight/defend radius for 
        /// a valid monster for fight or returns companion to farmer
        /// if no valid monster in specified fight/defend radius.
        /// </summary>
        private void CheckLeaderRadius()
        {
            Vector2 i = this.follower.GetBoundingBox().Center.ToVector2();
            Vector2 l = this.realLeader.GetBoundingBox().Center.ToVector2();
            float distance = (l - i).Length();
            float retRadius = this.joystick.pathToFollow != null ? RETURN_RADIUS : PATH_NULL_RETURN_RADIUS;

            if (distance >= retRadius && this.leader is Monster)
            {
                // move back towards leader
                this.potentialIdle = true;
                this.leader = null;
                this.pathFinder.GoalCharacter = null;
            }
            else if (distance < RETURN_RADIUS && this.leader == null)
            {
                this.leader = this.SearchForNewTarget();
                if (this.leader != null)
                {
                    this.pathFinder.GoalCharacter = this.leader;
                    this.PathfindingRemakeCheck(); // Force find path to monster immediatelly
                }
                else
                {
                    this.potentialIdle = true;
                    this.pathFinder.GoalCharacter = null;
                }
            }
        }

        /// <summary>
        /// Check if is here any monster to fight
        /// </summary>
        private Monster SearchForNewTarget()
        {
            float defendRadius = this.ai.Csm.HasSkill("warrior") ? DEFEND_TILE_RADIUS_WARRIOR : DEFEND_TILE_RADIUS;
            Monster monster = this.FindMonster(defendRadius);

            if (monster != null && Helper.IsValidMonster(monster))
            {
                if (Game1.random.NextDouble() < .3f)
                    this.DoFightSpeak(true);
                this.ai.Monitor.Log("Found valid monster to defeat");
                return monster;
            }

            return null;   
        }

        /// <summary>
        /// Find a monster in defined radius. 
        /// </summary>
        /// <param name="defendRadius"></param>
        /// <returns>Null if no monster found, otherwise the monster</returns>
        private Monster FindMonster(float defendRadius)
        {
            return Helper.GetNearestMonsterToCharacter(this.follower, defendRadius,
                m => Vector2.Distance(
                    m.GetBoundingBox()
                        .Center
                        .ToVector2(),
                    this.realLeader
                        .GetBoundingBox()
                        .Center
                        .ToVector2()) >= FARMER_AGGRO_RADIUS);
        }

        private void DoFightSpeak(bool onlyEmote = false)
        {
            // Cooldown not expired? Say nothing
            if (this.fightBubbleCooldown != 0)
                return;

            if (!onlyEmote && Game1.random.NextDouble() < this.fightSpeechTriggerThres && DialogueProvider.GetBubbleString(this.bubbles, this.follower, "fight", out string text))
            {
                bool isRed = this.ai.Csm.HasSkill("warrior") && Game1.random.NextDouble() < 0.1;
                this.follower.showTextAboveHead(text, isRed ? 2 : -1);
                this.fightBubbleCooldown = 600;
            }
            else if (this.ai.Csm.HasSkill("warrior") && Game1.random.NextDouble() < this.fightSpeechTriggerThres / 2)
            {
                if (Compat.IsModLoaded(ModUids.PACIFISTMOD_UID))
                    return;

                this.follower.clearTextAboveHead();
                this.follower.doEmote(12);
                this.fightBubbleCooldown = 550;
            }
            else if (Game1.random.NextDouble() > (this.fightSpeechTriggerThres + this.fightSpeechTriggerThres * .33f))
            {
                if (Compat.IsModLoaded(ModUids.PACIFISTMOD_UID))
                    return;

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

            // Go iddle instantly when potential monster is not valid
            if (this.leader is Monster && !Helper.IsValidMonster(this.leader as Monster))
                return true;

            return this.potentialIdle; // By default propagate potential iddle state as iddle state
        }

        public void DoWeaponSwing()
        {
            if (this.weaponSwingCooldown > 0)
            {
                this.weaponSwingCooldown--;
            }

            if (this.fistCoolDown > 0)
            {
                --this.fistCoolDown;
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

            this.CheckLeaderRadius();

            if (e.IsOneSecond && !Helper.IsValidMonster(this.leader as Monster))
            {
                this.leader = null;
                this.potentialIdle = true;
                return;
            }

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
            this.weaponAttrs = new WeaponAttributes(criticalFist ? null : this.weapon);

            if (criticalFist)
            {
                this.weaponAttrs.knockBack *= 1.7f;
                this.weaponAttrs.smashAround /= 2f;
                this.fistCoolDown = 240;
            }

            if (this.ai.Csm.HasSkill("warrior"))
            {
                // Enhanced skills ONLY for WARRIORS
                this.weaponAttrs.minDamage += (int)Math.Round(this.weaponAttrs.minDamage * .02f); // 2% added min damage
                this.weaponAttrs.knockBack += this.weaponAttrs.knockBack * (Game1.random.Next(1, 3) / 100); // 1-3% added knock back
                this.weaponAttrs.addedEffectiveArea += (int)Math.Round(this.weaponAttrs.addedEffectiveArea * .01f); // 1% added effective area
                this.weaponAttrs.critChance += Math.Max(0, (float)Game1.player.DailyLuck / 2); // added critical chance is half of daily luck. If luck is negative, no added critical chance
            }

            if (criticalFist && this.follower.FacingDirection != 0)
            {
                if (!Compat.IsModLoaded(ModUids.PACIFISTMOD_UID))
                {
                    this.follower.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 960, 128, 128), 60f, 4, 0, this.follower.Position, false, this.follower.FacingDirection == 3, 1f, 0.0f, Color.White, .5f, 0.0f, 0.0f, 0.0f, false));
                }
                else
                {
                    this.follower.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(211, 428, 7, 6), 2000f, 1, 0, new Vector2((float)this.follower.getTileX(), (float)this.follower.getTileY()) * 64f + new Vector2(16f, -64f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, false)
                    {
                        motion = new Vector2(0.0f, -0.5f),
                        alphaFade = 0.01f
                    });
                }
            }

            companionBox.Inflate(4, 4); // Personal space
            effectiveArea.Inflate((int)(effectiveArea.Width * this.weaponAttrs.smashAround + this.weaponAttrs.addedEffectiveArea), (int)(effectiveArea.Height * this.weaponAttrs.smashAround + this.weaponAttrs.addedEffectiveArea));

            if (Compat.IsModLoaded(ModUids.PACIFISTMOD_UID))
            {
                int frame = -1;
                bool flip = false;

                if (this.kissFrames.ContainsKey(this.follower.Name))
                {
                    frame = int.Parse(this.kissFrames[this.follower.Name].Split(' ')[0]);
                    flip = this.kissFrames[this.follower.Name].Split(' ')[1] == "true";
                }

                this.follower.doEmote(20);
            }

            if (!criticalFist && !this.defendFistUsed && this.ai.Csm.HasSkill("warrior") && companionBox.Intersects(enemyBox) && this.weaponSwingCooldown == this.CooldownTimeout && this.fistCoolDown <= 0)
            {
                this.ai.Monitor.Log("Critical dangerous: Using defense fists!");
                this.defendFistUsed = true;
                this.DoDamage(true); // Force fist when monster is too close
                return;
            }

            if (this.follower.currentLocation.DamageMonsterByCompanion(effectiveArea, this.weaponAttrs.minDamage, this.weaponAttrs.maxDamage,
                this.weaponAttrs.knockBack, this.weaponAttrs.addedPrecision, this.weaponAttrs.critChance, this.weaponAttrs.critMultiplier, !criticalFist,
                this.follower, this.realLeader as Farmer))
            {
                if (criticalFist)
                {
                    this.follower.currentLocation.playSound(Compat.IsModLoaded(ModUids.PACIFISTMOD_UID) ? "dwop" : "clubSmash");
                    this.ai.Csm.CompanionManager.Hud.GlowSkill("warrior", Color.Red, 1);
                    this.fistCoolDown = 1200;
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

            if (Compat.IsModLoaded(ModUids.PACIFISTMOD_UID)) {
                c.currentLocation.localSound("dwop");
                return;
            }

            switch (type)
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
            Farmer farmer = this.ai.farmer;
            int combatLevel = farmer.CombatLevel;
            int weaponSpeed = this.weapon?.speed.Value ?? 400;
            int swipeDelay = (400 - weaponSpeed) / 4;

            if (this.ai.Csm.HasSkill("warrior"))
            {
                // Warriors are more skilled
                combatLevel += combatLevel > 5 ? 2 : 1;
            }

            double skill = Math.Max(combatLevel, 8) * Math.Log(Math.Pow(combatLevel, 2) + 1) + Math.Pow(combatLevel * 0.75, 2) + Math.Max(combatLevel, 5);

            return (int)Math.Round(skill) 
                + Game1.random.Next(-10, 10) 
                - swipeDelay
                + (int)Math.Round(Game1.player.DailyLuck);
        }

        private void AnimateMe()
        {
            this.attackSpeedPitch = this.GetAttackPitch();
            this.weaponSwingCooldown = this.CooldownTimeout;
            this.defendFistUsed = false;
            this.follower.Sprite.StopAnimation();
            this.follower.Sprite.faceDirectionStandard(this.joystick.GetFacingDirectionFromMovement(new Vector2(this.leader.Position.X, this.leader.Position.Y)));
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

                this.joystick.NoCharging = true;
                return -0.65f;
            }
            else if (distanceFromTarget <= this.attackRadius)
            {
                if (this.weaponSwingCooldown == 0)
                {
                    this.AnimateMe();
                }

                if (this.ai.Csm.HasSkill("scared"))
                {
                    this.follower.shake(100);
                }

                this.joystick.NoCharging = true;
                return Math.Max(this.joystick.Speed - 0.1f, 0.1f);
            }

            this.joystick.NoCharging = false;
            return 6.11f;
        }

        protected override void PathfindingRemakeCheck()
        {
            base.PathfindingRemakeCheck();

            if (this.pathToFollow == null)
            {
                this.potentialIdle = true;
                this.ai.Monitor.Log($"Fight controller iddle, because can't find a path to monster '{this.leader?.Name}'");
            }
        }

        public override void Activate()
        {
            this.events.World.NpcListChanged += this.World_NpcListChanged;
            this.joystick.BlockedTimer = 30;
            this.weaponSwingCooldown = 0;
            this.fightBubbleCooldown = 0;
            this.potentialIdle = false;
            this.fistCoolDown = 0;
            this.CheckLeaderRadius();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (this.weapon != null && this.weaponSwingCooldown > this.SwingThreshold && !this.defendFistUsed)
            {
                int frames = this.weapon.type.Value == 3 ? 4 : 7;
                int duration = this.CooldownTimeout - this.SwingThreshold;
                int tick = Math.Abs(this.weaponSwingCooldown - this.CooldownTimeout);
                int currentFrame = this.CurrentFrame(tick, duration, frames);

                if (Compat.IsModLoaded(ModUids.PACIFISTMOD_UID))
                    currentFrame = 1;

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
            this.potentialIdle = true;
        }

        private struct WeaponAttributes
        {
            public int minDamage;
            public int maxDamage;
            public int addedPrecision;
            public float critChance;
            public float critMultiplier;
            public float knockBack;
            public float smashAround;
            public int addedEffectiveArea;

            public WeaponAttributes(MeleeWeapon weapon)
            {
                this.minDamage = 1;
                this.maxDamage = 3;
                this.addedPrecision = 0;
                this.critChance = 0f;
                this.critMultiplier = .2f;
                this.knockBack = 1;
                this.smashAround = 1.5f;
                this.addedEffectiveArea = 0;

                if (weapon != null)
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
}
