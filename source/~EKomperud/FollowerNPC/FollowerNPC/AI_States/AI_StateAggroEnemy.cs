using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Tools;
using StardewValley.Projectiles;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FollowerNPC.AI_States
{
    public partial class AI_StateAggroEnemy : AI_StateFollowCharacter
    {
        public class MonsterIgnoreTimer
        {
            public MonsterIgnoreTimer(Monster m, int ignoreTimer)
            {
                this.m = m;
                this.ignoreTimer = ignoreTimer;
            }
            public Monster m;
            public int ignoreTimer;
        }

        Character pathfindingTarget;
        private MeleeWeapon weapon;
        private int weaponSwingCooldown;
        private List<FarmerSprite.AnimationFrame>[] attackAnimations;
        private List<MonsterIgnoreTimer> ignoreList;
        private Vector2 aggroMonsterLastTileLocation;

        private Rectangle aoe;

        private bool aggroMonsterDefeated;
        private bool aggroMonsterLeftAggroRadius;

        // Leader radii //
        private float hesitationRadius;
        private float returnRadius;
        private float pathNullReturnRadius;
        // *************** //

        // Aggro radii //
        private float backupRadius;
        private float attackRadius;
        private float stayAggroRadius;
        private float defendRadius;
        // *********** //

        // Cached Reflection Fields //
        private FieldInfo multiplayer;
        private static Vector2 meleeWeaponCenter = new Vector2(1f, 15f);
        // ************************ //

        #region Core Methods

        public AI_StateAggroEnemy(Character me, Character leader, AI_StateMachine machine) : base(me, leader, machine)
        {
            followThreshold = 2f * fullTile;
            decelerateThreshold = 1.25f * fullTile;
            deceleration = 0.125f;

            returnRadius = 11f * fullTile;
            pathNullReturnRadius = 4f * fullTile;

            backupRadius = 0.9f * fullTile;
            attackRadius = 1.25f * fullTile;
            stayAggroRadius = 8.5f * fullTile;
            defendRadius = 9f * fullTile;

            multiplayer = typeof(Game1).GetField("multiplayer",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            weapon = new MeleeWeapon(GetMeleeWeapon());
            typeof(MeleeWeapon).GetField("lastUser",
                BindingFlags.NonPublic | BindingFlags.Instance).SetValue(weapon, leader);
            weaponSwingCooldown = 0;
            attackAnimations = new List<FarmerSprite.AnimationFrame>[4];
            attackAnimations[0] = new List<FarmerSprite.AnimationFrame>()
            {
                new FarmerSprite.AnimationFrame(8, 100),
                new FarmerSprite.AnimationFrame(9, 250)
            };
            attackAnimations[1] = new List<FarmerSprite.AnimationFrame>()
            {
                new FarmerSprite.AnimationFrame(6, 100),
                new FarmerSprite.AnimationFrame(7, 250)
            };
            attackAnimations[2] = new List<FarmerSprite.AnimationFrame>()
            {
                new FarmerSprite.AnimationFrame(0, 100),
                new FarmerSprite.AnimationFrame(1, 250)
            };
            attackAnimations[3] = new List<FarmerSprite.AnimationFrame>()
            {
                new FarmerSprite.AnimationFrame(14, 100),
                new FarmerSprite.AnimationFrame(15, 250)
            };
        }

        public override void EnterState()
        {
            base.EnterState();
            ignoreList = new List<MonsterIgnoreTimer>();
            ModEntry.modHelper.Events.World.NpcListChanged += World_NpcListChanged;
            ModEntry.modHelper.Events.Display.RenderedWorld += Display_RenderedWorld;
        }

        

        public override void ExitState()
        {
            base.ExitState();
            ignoreList.Clear();
            ModEntry.modHelper.Events.World.NpcListChanged -= World_NpcListChanged;
            ModEntry.modHelper.Events.Display.RenderedWorld -= Display_RenderedWorld;
        }

        public void SetMonster(Monster m)
        {
            aggroMonster = m;
            aStar.GoalCharacter = m;
            pathfindingTarget = m;
            aStar.gameLocation = pathfindingTarget.currentLocation;
            aggroMonsterDefeated = false;
        }

        public override void Update(UpdateTickedEventArgs e)
        {
            me.farmerPassesThrough = true;
            CheckLeaderRadius();
            eAI_State potentialNewState = TransitionsCheck();
            if (potentialNewState != eAI_State.nil)
            {
                machine.ChangeState(potentialNewState);
                return;
            }

            PathfindingNodeUpdateCheck();
            MovementAndAnimationUpdate();
            UpdateIgnoreList();

            if (e.IsMultipleOf(30))
            {
                aggroMonster = ReassessTargets();
            }
            if (e.IsMultipleOf(15))
            {
                PathfindingRemakeCheck();
            }

            if (weaponSwingCooldown > 36)
            {
                DoDamage();
                //Game1.spriteBatch.End();
            }
        }

        private eAI_State TransitionsCheck()
        {
            if (weaponSwingCooldown > 36)
            {
                return eAI_State.nil;
            }
            else if (aggroMonsterDefeated)
            {
                aggroMonsterDefeated = false;
                aggroMonster = SearchForNewTarget();
                if (!(aggroMonster != null))
                    return eAI_State.followFarmer;
                pathfindingTarget = aggroMonster;
                aStar.GoalCharacter = aggroMonster;
            }
            else if (aggroMonsterLeftAggroRadius)
            {
                aggroMonsterLeftAggroRadius = false;
                aggroMonster = SearchForNewTarget();
                if (!(aggroMonster != null))
                    return eAI_State.followFarmer;
                pathfindingTarget = aggroMonster;
                aStar.GoalCharacter = aggroMonster;
            }

            return eAI_State.nil;
        }

        private void World_NpcListChanged(object sender, NpcListChangedEventArgs e)
        {
            if (e.Removed != null && e.Removed.Count() != 0 && e.Removed.Contains(aggroMonster))
            {
                aggroMonsterDefeated = true;
                pathfindingTarget = leader;
                aStar.GoalCharacter = leader;
            }
        }

        private void Display_RenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (weaponSwingCooldown > 36)
            {
                DrawDuringUse(Math.Abs(weaponSwingCooldown - 50) / 2, me.facingDirection, e.SpriteBatch,
                    me.getLocalPosition(Game1.viewport), me, MeleeWeapon.getSourceRect(weapon.InitialParentTileIndex), weapon.type.Value, weapon.isOnSpecial);
                //Vector2 o1 = new Vector2(aoe.X, aoe.Y);
                //Vector2 o2 = Game1.GlobalToLocal(o1);
                //e.SpriteBatch.Draw(Game1.mouseCursors,
                //    o2, new Rectangle?(new Rectangle(194, 388, 64, 64)), Color.Red);
            }
        }

        #endregion

        #region Helpers

        protected int GetMeleeWeapon()
        {
            string n = machine.owner.stateMachine.companion.Name;
            if (n.Equals("Abigail") || n.Equals("Alex") || n.Equals("Emily") || n.Equals("Sebastian"))
            {
                switch (machine.owner.stateMachine.manager.farmer.CombatLevel)
                {
                    case 0:
                        return GetUniqueWeapon(n);
                    case 1:
                        return 11; // Steel Smallsword
                    case 2:
                        return 44; // Cutlass
                    case 3:
                        return 13; // Insect Head
                    case 4:
                        return 6; // Iron Edge
                    case 5:
                        return 3; // Holy Blade
                    case 6:
                        return 7; // Templar's Blade
                    case 7:
                        return 14; // Neptune's Glaive
                    case 8:
                        return 50; // Steel Falchion
                    case 9:
                        return 8; // Obsidian Edge
                    case 10:
                        return 9; // Lava Katana
                    default:
                        return 4; // Galaxy Sword
                }
            }
            return GetUniqueWeapon(n);
        }

        protected int GetUniqueWeapon(string name)
        {
            switch (name)
            {
                case "Abigail":
                    return 40;
                case "Alex":
                    return 25;
                case "Elliott":
                    return 35;
                case "Emily":
                    return 0;
                case "Haley":
                    return 42;
                case "Harvey":
                    return 37;
                case "Leah":
                    return 39;
                case "Maru":
                    return 36;
                case "Penny":
                    return 38;
                case "Sam":
                    return 30;
                case "Sebastian":
                    return 41;
                case "Shane":
                    return 0;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Provides updates to the companion's movement.
        /// </summary>
        protected override void MovementAndAnimationUpdate()
        {
            if (pathfindingTarget == null)
                return;

            Point c = me.GetBoundingBox().Center;
            Vector2 myBoundingBox = new Vector2(c.X, c.Y);
            Point t = pathfindingTarget.GetBoundingBox().Center;
            lastFrameMovement = myBoundingBox - lastFramePosition;
            Vector2 targetFacingDiff;
            if (aggroMonster != null)
            {
                Point m = aggroMonster.GetBoundingBox().Center;
                targetFacingDiff = (new Vector2(m.X, m.Y) - myBoundingBox);
            }
            else
            {
                targetFacingDiff = new Vector2(t.X, t.Y) - myBoundingBox;
            }

            targetFacingDiff.Normalize();
            Vector2 diff = new Vector2(t.X, t.Y) - myBoundingBox;
            float diffLen = diff.Length();
            weaponSwingCooldown = weaponSwingCooldown > 0 ? weaponSwingCooldown - 1 : weaponSwingCooldown;
            currentMovespeed = GetMovementSpeedBasedOnTarget(diffLen);
            if (currentMovespeed != 0 && currentPathNode != negativeOne)
            {
                Point n = new Point(((int) currentPathNode.X * fullTile) + halfTile,
                    ((int) currentPathNode.Y * fullTile) + halfTile);
                Vector2 nodeDiff = new Vector2(n.X, n.Y) - new Vector2(c.X, c.Y);
                float nodeDiffLen = nodeDiff.Length();
                if (nodeDiffLen <= pathNodeTolerance)
                    return;
                nodeDiff /= nodeDiffLen;

                me.xVelocity = nodeDiff.X * currentMovespeed;
                me.yVelocity = -nodeDiff.Y * currentMovespeed;
                if (me.xVelocity != 0 && me.yVelocity != 0)
                {
                    me.xVelocity *= 1.2645f;
                    me.yVelocity *= 1.2645f;
                }

                HandleWallSliding();
                HandleGates();
                lastFrameVelocity = new Vector2(me.xVelocity, me.yVelocity);
                lastFramePosition = new Vector2(me.GetBoundingBox().Center.X, me.GetBoundingBox().Center.Y);

                animationUpdateSum += new Vector2(targetFacingDiff.X, targetFacingDiff.Y);
                AnimationSubUpdate();
                me.MovePosition(Game1.currentGameTime, Game1.viewport, me.currentLocation);
                lastMovementDirection = targetFacingDiff;

                movedLastFrame = true;
            }
            else if (movedLastFrame)
            {
                me.Halt();
                me.Sprite.faceDirectionStandard(
                    GetFacingDirectionFromMovement(new Vector2(lastMovementDirection.X, lastMovementDirection.Y)));
                movedLastFrame = false;
            }
            else
            {
                me.xVelocity = 0f;
                me.yVelocity = 0f;
            }
        }

        /// <summary>
        /// Returns a movement speed based on target and target distance
        /// </summary>
        private float GetMovementSpeedBasedOnTarget(float distance)
        {
            if (weaponSwingCooldown > 36)
                return 0;

            if (pathfindingTarget.Equals(aggroMonster))
            {
                if (distance <= backupRadius)
                {
                    if (weaponSwingCooldown == 0)
                    {
                        SetMeAnimating();
                    }
                    return -0.65f;
                }
                else if (distance <= attackRadius)
                {
                    if (weaponSwingCooldown == 0)
                    {
                        SetMeAnimating();
                    }
                    return Math.Max(currentMovespeed - 0.1f, 0.1f);
                }
                else if (distance <= stayAggroRadius)
                {
                    return 5.28f;
                }
                else if (distance <= defendRadius)
                {
                    return Math.Max(currentMovespeed - 0.1f, 0f);
                }
                else
                {
                    aggroMonsterLeftAggroRadius = true;
                    return 0f;
                }
            }
            else if (pathfindingTarget.Equals(leader))
            {
                return 5.28f;
            }

            return 4f;
        }

        /// <summary>
        /// Provides updates to the companion's animation;
        /// </summary>
        protected override void AnimationSubUpdate()
        {
            if (++switchDirectionSpeed == 5)
            {
                facingDirection = GetFacingDirectionFromMovement(animationUpdateSum);
                animationUpdateSum = Vector2.Zero;
                switchDirectionSpeed = 0;
            }

            if (facingDirection >= 0)
            {
                me.faceDirection(facingDirection);
                SetMovementDirectionAnimation(facingDirection);
            }

            
        }

        /// <summary>
        /// Remakes our path if the farmer has changed tiles since the last time
        /// this function was called. (Every quarter second as of right now)
        /// </summary>
        protected override void PathfindingRemakeCheck()
        {
            Vector2 targetCurrentTile = pathfindingTarget.getTileLocation();

            if (targetLastTile != targetCurrentTile)
            {
                path = aStar.Pathfind(me.getTileLocation(), targetCurrentTile);
                if (path != null && path.Count != 0 && me.getTileLocation() != path.Peek())
                    currentPathNode = path.Dequeue();
                else
                    currentPathNode = negativeOne;
            }

            targetLastTile = targetCurrentTile;
        }

        /// <summary>
        /// Changes pathfinding targets depending on distance from leader
        /// </summary>
        public void CheckLeaderRadius()
        {
            Vector2 i = new Vector2(me.GetBoundingBox().Center.X, me.GetBoundingBox().Center.Y);
            Vector2 l = new Vector2(leader.GetBoundingBox().Center.X, leader.GetBoundingBox().Center.Y);
            float distance = (l - i).Length();
            float retRadius = path != null ? returnRadius : pathNullReturnRadius;
            if (distance >= retRadius && pathfindingTarget.Equals(aggroMonster))
            {
                // move back towards leader
                pathfindingTarget = leader;
                aStar.GoalCharacter = leader;
            }
            else if (distance < returnRadius && pathfindingTarget.Equals(leader))
            {
                aggroMonster = SearchForNewTarget();
                if (aggroMonster != null)
                {
                    pathfindingTarget = aggroMonster;
                    aStar.GoalCharacter = aggroMonster;
                }
                else
                {
                    aggroMonsterLeftAggroRadius = true;
                }
            }
        }

        /// <summary>
        /// Searches and returns a nearby monster, otherwise returns null
        /// </summary>
        private Monster SearchForNewTarget()
        {
            Monster aggroMonster = null;
            float aggroMonsterDistance = float.PositiveInfinity;
            Vector2 i = new Vector2(me.GetBoundingBox().Center.X, me.GetBoundingBox().Center.Y);
            foreach (Character c in aStar.gameLocation.characters)
            {
                Monster asMonster = c as Monster;
                if (asMonster != null && IsValidMonster(asMonster))
                {
                    Vector2 m = new Vector2(asMonster.GetBoundingBox().Center.X, asMonster.GetBoundingBox().Center.Y);
                    float distance = (m - i).Length();
                    if (distance <= stayAggroRadius && distance < aggroMonsterDistance)
                    {
                        aggroMonster = asMonster;
                        aggroMonsterDistance = distance;
                    }
                }
            }
            if (aggroMonster == null)
                aggroMonsterLeftAggroRadius = true;

            return aggroMonster;
        }

        /// <summary>
        /// Reasses the existing monsters, changing targets if there is another close-by monster.
        /// Extra creedence is given to the current aggro monster if the AI is currently pathing to them,
        /// while creedence is taken away if the AI has no current path to them.
        /// </summary>
        private Monster ReassessTargets()
        {
            Vector2 i = new Vector2(me.GetBoundingBox().Center.X, me.GetBoundingBox().Center.Y);

            Monster aggroMonster = this.aggroMonster;
            Vector2 m;
            float aggroMonsterDistance = float.PositiveInfinity;
            bool pathToMonsterExists = path != null;
            if (aggroMonster != null && IsValidMonster(aggroMonster))
            {
                m = new Vector2(this.aggroMonster.GetBoundingBox().Center.X,
                    this.aggroMonster.GetBoundingBox().Center.Y);
                aggroMonsterDistance = (m - i).LengthSquared() + (pathToMonsterExists ? -(fullTile * fullTile) : (fullTile * fullTile));
            }
            else
            {
                aggroMonster = null;
            }

            foreach (Character c in aStar.gameLocation.characters)
            {
                Monster asMonster = c as Monster;
                if (asMonster != null && IsValidMonster(asMonster) && !asMonster.Equals(aggroMonster) && !IsOnIgnoreList(asMonster))
                {
                    Vector2 m2 = new Vector2(asMonster.GetBoundingBox().Center.X, asMonster.GetBoundingBox().Center.Y);
                    float distance = (m2 - i).LengthSquared();
                    if (distance <= stayAggroRadius && distance < aggroMonsterDistance)
                    {
                        aggroMonster = asMonster;
                        aggroMonsterDistance = distance;
                    }
                }
            }

            if (this.aggroMonster != aggroMonster && !pathToMonsterExists)
            {
                ignoreList.Add(new MonsterIgnoreTimer(this.aggroMonster, 90));
            }
            if (aggroMonster == null)
                aggroMonsterLeftAggroRadius = true;

            return aggroMonster;
        }

        private void UpdateIgnoreList()
        {
            for (int i = 0; i < ignoreList.Count; i++)
            {
                if (--ignoreList[i].ignoreTimer == 0)
                {
                    ignoreList.RemoveAt(i);
                    i--;
                }
            }
        }

        private bool IsOnIgnoreList(Monster m)
        {
            foreach (MonsterIgnoreTimer mon in ignoreList)
            {
                if (mon.m.Equals(m))
                    return true;
            }
            return false;
        }

        private bool IsValidMonster(Monster m)
        {
            Bug b = m as Bug;
            if (b != null)
                return !b.isArmoredBug.Value;

            Mummy mum = m as Mummy;
            if (mum != null)
            {
                FieldInfo reviveTimer =
                    typeof(Mummy).GetField("reviveTimer", BindingFlags.NonPublic | BindingFlags.Instance);
                int t = (int) reviveTimer.GetValue(mum);
                return t <= 0;
            }

            RockCrab crab = m as RockCrab;
            if (crab != null)
            {
                return crab.isMoving();
            }

            return true;
        }

        public void SetMeAnimating()
        {
            weaponSwingCooldown = 50;
            //me.Sprite.StopAnimation();
            float swipeSpeed = (400 - weapon.speed.Value * 40 - me.addedSpeed * 40);
            if (weapon.type.Value != -1)
            {
                DoSwipe(weapon.type.Value, me.FacingDirection, swipeSpeed / (float)((weapon.type.Value == 2) ? 5 : 8), me);
                DoDamage();
            }
            else
            {
                me.currentLocation.playSound("daggerswipe");
                swipeSpeed /= 4f;
                me.Sprite.setCurrentAnimation(attackAnimations[me.FacingDirection]);
                WeaponUpdate(me.FacingDirection, 0, me);
                DoDamage();
            }
        }

        public void DoSwipe(int type, int facingDirection, float swipeSpeed, Character c)
        {
            swipeSpeed *= 1.3f;
            //c.Sprite.setCurrentAnimation(attackAnimations[c.FacingDirection]);
            WeaponUpdate(c.FacingDirection, 0, c);
            if (type == 3)
            {
                c.currentLocation.localSound("swordswipe");
            }
            else if (type == 2)
            {
                c.currentLocation.localSound("clubswipe");
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
                weapon.CurrentParentTileIndex = weapon.InitialParentTileIndex;
            }
            else if (who.FacingDirection == 0 || (who.FacingDirection == 2 && farmerMotionFrame >= 2))
            {
                weapon.CurrentParentTileIndex = weapon.InitialParentTileIndex + 1;
            }
            weapon.CurrentParentTileIndex += offset;
        }

        /// <summary>
        /// Makes the AI attack using their weapon
        /// </summary>
        private void DoDamage()
        {
            Vector2 actionTile = GetToolLocation();
            Vector2 tileLocation = Vector2.Zero;
            Vector2 tileLocation2 = Vector2.Zero;

            // Need to look at this! //
            Rectangle areaOfEffect = weapon.getAreaOfEffect((int) actionTile.X, (int) actionTile.Y, me.facingDirection,
                ref tileLocation, ref tileLocation2, me.GetBoundingBox(), Math.Abs(weaponSwingCooldown - 50) / 2);
            aoe = areaOfEffect;
            ///////////////////////////
            
            weapon.mostRecentArea = areaOfEffect;
            if (aStar.gameLocation.damageMonster(areaOfEffect, weapon.minDamage.Value, weapon.maxDamage.Value, false,
                    weapon.knockback.Value, weapon.addedPrecision.Value, weapon.critChance.Value,
                    weapon.critMultiplier.Value,
                    weapon.type.Value != 1 || weapon.isOnSpecial, (leader as Farmer)) && 
                weapon.type.Value == 2)
            {
                aStar.gameLocation.playSound("clubhit");
            }

            string soundToPlay = "";
            aStar.gameLocation.projectiles.Filter(delegate(Projectile projectile)
            {
                if (areaOfEffect.Intersects(projectile.getBoundingBox()))
                {
                    projectile.behaviorOnCollisionWithOther(aStar.gameLocation);
                }

                return !projectile.destroyMe;
            });
            foreach (Vector2 v in Utility.removeDuplicates(
                Utility.getListOfTileLocationsForBordersOfNonTileRectangle(areaOfEffect)))
            {
                if (aStar.gameLocation.terrainFeatures.ContainsKey(v) && aStar.gameLocation.terrainFeatures[v]
                        .performToolAction(weapon, 0, v, aStar.gameLocation))
                {
                    aStar.gameLocation.terrainFeatures.Remove(v);
                }

                if (aStar.gameLocation.objects.ContainsKey(v) &&
                    aStar.gameLocation.objects[v].performToolAction(weapon, aStar.gameLocation))
                {
                    aStar.gameLocation.objects.Remove(v);
                }

                if (aStar.gameLocation.performToolAction(weapon, (int) v.X, (int) v.Y))
                {
                    break;
                }
            }

            if (!soundToPlay.Equals(""))
            {
                Game1.playSound(soundToPlay);
            }

            //base.CurrentParentTileIndex = base.IndexOfMenuItemView;
            //if (who != null && who.isRidingHorse())
            //{
            //    who.completelyStopAnimatingOrDoingAction();
            //}
        }

        /// <summary>
        /// Damages a got-damn monster
        /// </summary>
        private bool DamageMonster(Rectangle areaOfEffect, int minDamage, int maxDamage, bool isBomb,
            float knockBackModifier, int addedPrecision, float critChance, float critMultiplier,
            bool triggerMonsterInvincibleTimer, Character who)
        {
            MethodInfo isMonsterDamageApplicable = typeof(NPC).GetMethod("GameLocation",
                BindingFlags.NonPublic | BindingFlags.Instance, null,
                new Type[] {typeof(Farmer), typeof(Monster), typeof(bool)}, null);
            GameLocation gl = aStar.gameLocation;
            bool didAnyDamage = false;
            int j = 0;
            int i = 0;
            for (j = aStar.gameLocation.characters.Count - 1; j >= 0; j = i - 1)
            {
                if (gl.characters[j].GetBoundingBox().Intersects(areaOfEffect) && gl.characters[j].IsMonster &&
                    !gl.characters[j].IsInvisible && !(gl.characters[j] as Monster).isInvincible() &&
                    !(gl.characters[j] as Monster).isInvincible() &&
                    (isBomb || IsMonsterDamageApplicable(who, gl.characters[j] as Monster, true) ||
                     IsMonsterDamageApplicable(who, gl.characters[j] as Monster, false)))
                {
                    bool isDagger = weapon.type.Value == 1;
                    didAnyDamage = true;
                    Rectangle monsterBox = gl.characters[j].GetBoundingBox();
                    Vector2 trajectory = GetAwayFromCharacterTrajectory(monsterBox, who);
                    trajectory = knockBackModifier > 0f ? trajectory * knockBackModifier : new Vector2(gl.characters[j].xVelocity, gl.characters[j].yVelocity);
                    if ((gl.characters[j] as Monster).Slipperiness == -1)
                        trajectory = Vector2.Zero;
                    bool crit = false;
                    int damageAmount;
                    if (maxDamage >= 0)
                    {
                        damageAmount = Game1.random.Next(minDamage, maxDamage + 1);
                        if (who != null && Game1.random.NextDouble() < (double)(critChance + (float)(leader as Farmer).LuckLevel * (critChance / 40f)))
                        {
                            crit = true;
                            gl.playSound("crit");
                        }
                        damageAmount = (crit ? ((int)((float)damageAmount * critMultiplier)) : damageAmount);
                        damageAmount = Math.Max(1, damageAmount + ((who != null) ? ((leader as Farmer).attack * 3) : 0));
                        //damageAmount = MonsterTakeDamage((Monster)gl.characters[j], damageAmount, (int)trajectory.X, (int)trajectory.Y, isBomb, (double)addedPrecision / 10.0, who);
                        damageAmount = ((Monster) gl.characters[j]).takeDamage(damageAmount, (int) trajectory.X,
                            (int) trajectory.Y, isBomb, (double) addedPrecision / 10.0, "hitEnemy");
                        if (damageAmount == -1)
                        {
                            gl.debris.Add(new Debris("Miss", 1, new Vector2((float)monsterBox.Center.X, (float)monsterBox.Center.Y), Color.LightGray, 1f, 0f));
                        }
                        else
                        {
                            gl.debris.Filter((Debris d) => d.toHover == null || !d.toHover.Equals(gl.characters[j]) || d.nonSpriteChunkColor.Equals(Color.Yellow) || d.timeSinceDoneBouncing <= 900f);
                            gl.debris.Add(new Debris(damageAmount, new Vector2((float)(monsterBox.Center.X + 16), (float)monsterBox.Center.Y), crit ? Color.Yellow : new Color(255, 130, 0), crit ? (1f + (float)damageAmount / 300f) : 1f, gl.characters[j]));
                        }
                        if (triggerMonsterInvincibleTimer)
                        {
                            (gl.characters[j] as Monster).setInvincibleCountdown(450 / (isDagger ? 3 : 2));
                        }
                    }
                    else
                    {
                        damageAmount = -2;
                        gl.characters[j].setTrajectory(trajectory);
                        if (((Monster)gl.characters[j]).Slipperiness > 10)
                        {
                            gl.characters[j].xVelocity /= 2f;
                            gl.characters[j].yVelocity /= 2f;
                        }
                    }
                    Multiplayer multiplayer = (Multiplayer)this.multiplayer.GetValue(null);
                    if (weapon != null && weapon.Name.Equals("Galaxy Sword"))
                    {
                        multiplayer.broadcastSprites(gl, new TemporaryAnimatedSprite[]
                        {
                            new TemporaryAnimatedSprite(362, (float)Game1.random.Next(50, 120), 6, 1, new Vector2((float)(monsterBox.Center.X - 32), (float)(monsterBox.Center.Y - 32)), false, false)
                        });
                    }
                    if (((Monster) gl.characters[j]).Health <= 0)
                    {
                        if (!gl.IsFarm)
                        {
                            (leader as Farmer).checkForQuestComplete(null, 1, 1, null, gl.characters[j].Name, 4, -1);
                        }
                        gl.monsterDrop((Monster)gl.characters[j], monsterBox.Center.X, monsterBox.Center.Y);
                        if ((leader as Farmer) != null)
                        {
                            (leader as Farmer).gainExperience(4, ((Monster)gl.characters[j]).ExperienceGained);
                        }
                        gl.characters.RemoveAt(j);
                        Game1.stats.MonstersKilled += 1;
                    }
                    else if (damageAmount > 0)
                    {
                        ((Monster)gl.characters[j]).shedChunks(Game1.random.Next(1, 3));
                        if (crit)
                        {
                            multiplayer.broadcastSprites(gl, new TemporaryAnimatedSprite[]
                            {
                                new TemporaryAnimatedSprite(362, (float)Game1.random.Next(15, 50), 6, 1, gl.characters[j].getStandingPosition() - new Vector2(32f, 32f), false, Game1.random.NextDouble() < 0.5)
                                {
                                    scale = 0.75f,
                                    alpha = (crit ? 0.75f : 0.5f)
                                }
                            });
                            multiplayer.broadcastSprites(gl, new TemporaryAnimatedSprite[]
                            {
                                new TemporaryAnimatedSprite(362, (float)Game1.random.Next(15, 50), 6, 1, gl.characters[j].getStandingPosition() - new Vector2((float)(32 + Game1.random.Next(-21, 21) + 32), (float)(32 + Game1.random.Next(-21, 21))), false, Game1.random.NextDouble() < 0.5)
                                {
                                    scale = 0.5f,
                                    delayBeforeAnimationStart = 50,
                                    alpha = (crit ? 0.75f : 0.5f)
                                }
                            });
                            multiplayer.broadcastSprites(gl, new TemporaryAnimatedSprite[]
                            {
                                new TemporaryAnimatedSprite(362, (float)Game1.random.Next(15, 50), 6, 1, gl.characters[j].getStandingPosition() - new Vector2((float)(32 + Game1.random.Next(-21, 21) - 32), (float)(32 + Game1.random.Next(-21, 21))), false, Game1.random.NextDouble() < 0.5)
                                {
                                    scale = 0.5f,
                                    delayBeforeAnimationStart = 100,
                                    alpha = (crit ? 0.75f : 0.5f)
                                }
                            });
                            multiplayer.broadcastSprites(gl, new TemporaryAnimatedSprite[]
                            {
                                new TemporaryAnimatedSprite(362, (float)Game1.random.Next(15, 50), 6, 1, gl.characters[j].getStandingPosition() - new Vector2((float)(32 + Game1.random.Next(-21, 21) + 32), (float)(32 + Game1.random.Next(-21, 21))), false, Game1.random.NextDouble() < 0.5)
                                {
                                    scale = 0.5f,
                                    delayBeforeAnimationStart = 150,
                                    alpha = (crit ? 0.75f : 0.5f)
                                }
                            });
                            multiplayer.broadcastSprites(gl, new TemporaryAnimatedSprite[]
                            {
                                new TemporaryAnimatedSprite(362, (float)Game1.random.Next(15, 50), 6, 1, gl.characters[j].getStandingPosition() - new Vector2((float)(32 + Game1.random.Next(-21, 21) - 32), (float)(32 + Game1.random.Next(-21, 21))), false, Game1.random.NextDouble() < 0.5)
                                {
                                    scale = 0.5f,
                                    delayBeforeAnimationStart = 200,
                                    alpha = (crit ? 0.75f : 0.5f)
                                }
                            });
                        }
                    }
                }
                i = j;
            }
            return didAnyDamage;
        }

        private bool IsMonsterDamageApplicable(Character who, Monster monster, bool horizontalBias = true)
        {
            //if (!monster.isGlider.Value)
            //{
            //    Point farmerStandingPoint = who.getTileLocationPoint();
            //    Point monsterStandingPoint = monster.getTileLocationPoint();
            //    if (Math.Abs(farmerStandingPoint.X - monsterStandingPoint.X) + Math.Abs(farmerStandingPoint.Y - monsterStandingPoint.Y) > 1)
            //    {
            //        int xDif = monsterStandingPoint.X - farmerStandingPoint.X;
            //        int yDif = monsterStandingPoint.Y - farmerStandingPoint.Y;
            //        Vector2 pointInQuestion = new Vector2((float)farmerStandingPoint.X, (float)farmerStandingPoint.Y);
            //        while (xDif != 0 || yDif != 0)
            //        {
            //            if (horizontalBias)
            //            {
            //                if (Math.Abs(xDif) >= Math.Abs(yDif))
            //                {
            //                    pointInQuestion.X += (float)Math.Sign(xDif);
            //                    xDif -= Math.Sign(xDif);
            //                }
            //                else
            //                {
            //                    pointInQuestion.Y += (float)Math.Sign(yDif);
            //                    yDif -= Math.Sign(yDif);
            //                }
            //            }
            //            else if (Math.Abs(yDif) >= Math.Abs(xDif))
            //            {
            //                pointInQuestion.Y += (float)Math.Sign(yDif);
            //                yDif -= Math.Sign(yDif);
            //            }
            //            else
            //            {
            //                pointInQuestion.X += (float)Math.Sign(xDif);
            //                xDif -= Math.Sign(xDif);
            //            }
            //            if (aStar.gameLocation.objects.ContainsKey(pointInQuestion) || aStar.gameLocation.getTileIndexAt((int)pointInQuestion.X, (int)pointInQuestion.Y, "Buildings") != -1)
            //            {
            //                return false;
            //            }
            //        }
            //    }
            //}
            return true;
        }

        public Vector2 GetAwayFromCharacterTrajectory(Rectangle monsterBox, Character who)
        {
            float arg_44_0 = (float)(-(float)(who.GetBoundingBox().Center.X - monsterBox.Center.X));
            float ySlope = (float)(who.GetBoundingBox().Center.Y - monsterBox.Center.Y);
            float total = Math.Abs(arg_44_0) + Math.Abs(ySlope);
            if (total < 1f)
            {
                total = 5f;
            }
            float arg_8D_0 = arg_44_0 / total * (float)(50 + Game1.random.Next(-20, 20));
            ySlope = ySlope / total * (float)(50 + Game1.random.Next(-20, 20));
            return new Vector2(arg_8D_0, ySlope);
        }

        public static void DrawDuringUse(int frameOfFarmerAnimation, int facingDirection, SpriteBatch spriteBatch, Vector2 playerPosition, Character c, Rectangle sourceRect, int type, bool isOnSpecial)
        {
            //spriteBatch.Begin();
            if (type != 1)
            {
                if (isOnSpecial)
                {
                    if (type == 3)
                    {
                        switch (c.FacingDirection)
                        {
                            case 0:
                                spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 8f, playerPosition.Y - 44f), new Rectangle?(sourceRect), Color.White, -1.76714587f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() - 1) / 10000f));
                                return;
                            case 1:
                                spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 8f, playerPosition.Y - 4f), new Rectangle?(sourceRect), Color.White, -0.5890486f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 1) / 10000f));
                                return;
                            case 2:
                                spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 52f, playerPosition.Y + 4f), new Rectangle?(sourceRect), Color.White, -5.105088f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 2) / 10000f));
                                return;
                            case 3:
                                spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 56f, playerPosition.Y - 4f), new Rectangle?(sourceRect), Color.White, -0.981747746f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 1) / 10000f));
                                return;
                            default:
                                return;
                        }
                    }
                    else if (type == 2)
                    {
                        if (facingDirection == 1)
                        {
                            switch (frameOfFarmerAnimation)
                            {
                                case 0:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 32f - 12f, playerPosition.Y - 80f), new Rectangle?(sourceRect), Color.White, -1.17809725f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                case 1:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f, playerPosition.Y - 64f - 48f), new Rectangle?(sourceRect), Color.White, 0.3926991f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                case 2:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 128f - 16f, playerPosition.Y - 64f - 12f), new Rectangle?(sourceRect), Color.White, 1.17809725f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                case 3:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 72f, playerPosition.Y - 64f + 16f - 32f), new Rectangle?(sourceRect), Color.White, 0.3926991f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                case 4:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 96f, playerPosition.Y - 64f + 16f - 16f), new Rectangle?(sourceRect), Color.White, 0.7853982f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                case 5:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 96f - 12f, playerPosition.Y - 64f + 16f), new Rectangle?(sourceRect), Color.White, 0.7853982f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                case 6:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 96f - 16f, playerPosition.Y - 64f + 40f - 8f), new Rectangle?(sourceRect), Color.White, 0.7853982f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                case 7:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 96f - 8f, playerPosition.Y + 40f), new Rectangle?(sourceRect), Color.White, 0.981747746f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                default:
                                    return;
                            }
                        }
                        else if (facingDirection == 3)
                        {
                            switch (frameOfFarmerAnimation)
                            {
                                case 0:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 4f + 8f, playerPosition.Y - 56f - 64f), new Rectangle?(sourceRect), Color.White, 0.3926991f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                case 1:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 32f, playerPosition.Y - 32f), new Rectangle?(sourceRect), Color.White, -1.96349549f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                case 2:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 12f, playerPosition.Y + 8f), new Rectangle?(sourceRect), Color.White, -2.74889374f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                case 3:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 32f - 4f, playerPosition.Y + 8f), new Rectangle?(sourceRect), Color.White, -2.3561945f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                case 4:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 16f - 24f, playerPosition.Y + 64f + 12f - 64f), new Rectangle?(sourceRect), Color.White, 4.31969f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                case 5:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 20f, playerPosition.Y + 64f + 40f - 64f), new Rectangle?(sourceRect), Color.White, 3.926991f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                case 6:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 16f, playerPosition.Y + 64f + 56f), new Rectangle?(sourceRect), Color.White, 3.926991f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                case 7:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 8f, playerPosition.Y + 64f + 64f), new Rectangle?(sourceRect), Color.White, 3.73064137f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                default:
                                    return;
                            }
                        }
                        else
                        {
                            switch (frameOfFarmerAnimation)
                            {
                                case 0:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 24f, playerPosition.Y - 21f - 8f - 64f), new Rectangle?(sourceRect), Color.White, -0.7853982f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                                    break;
                                case 1:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 16f, playerPosition.Y - 21f - 64f + 4f), new Rectangle?(sourceRect), Color.White, -0.7853982f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                                    break;
                                case 2:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 16f, playerPosition.Y - 21f + 20f - 64f), new Rectangle?(sourceRect), Color.White, -0.7853982f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                                    break;
                                case 3:
                                    if (facingDirection == 2)
                                    {
                                        spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f + 8f, playerPosition.Y + 32f), new Rectangle?(sourceRect), Color.White, -3.926991f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                                    }
                                    else
                                    {
                                        spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 16f, playerPosition.Y - 21f + 32f - 64f), new Rectangle?(sourceRect), Color.White, -0.7853982f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                                    }
                                    break;
                                case 4:
                                    if (facingDirection == 2)
                                    {
                                        spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f + 8f, playerPosition.Y + 32f), new Rectangle?(sourceRect), Color.White, -3.926991f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                                    }
                                    break;
                                case 5:
                                    if (facingDirection == 2)
                                    {
                                        spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f + 12f, playerPosition.Y + 64f - 20f), new Rectangle?(sourceRect), Color.White, 2.3561945f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                                    }
                                    break;
                                case 6:
                                    if (facingDirection == 2)
                                    {
                                        spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f + 12f, playerPosition.Y + 64f + 54f), new Rectangle?(sourceRect), Color.White, 2.3561945f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                                    }
                                    break;
                                case 7:
                                    if (facingDirection == 2)
                                    {
                                        spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f + 12f, playerPosition.Y + 64f + 58f), new Rectangle?(sourceRect), Color.White, 2.3561945f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                                    }
                                    break;
                            }

                            return;
                            //if (f.FacingDirection == 0)
                            //{
                            //    f.FarmerRenderer.draw(spriteBatch, f.FarmerSprite, f.FarmerSprite.SourceRect, f.getLocalPosition(Game1.viewport), new Vector2(0f, (f.yOffset + 128f - (float)(f.GetBoundingBox().Height / 2)) / 4f + 4f), Math.Max(0f, (float)f.getStandingY() / 10000f + 0.0099f), Color.White, 0f, c);
                            //    return;
                            //}
                        }
                    }
                }
                else if (facingDirection == 1)
                {
                    switch (frameOfFarmerAnimation)
                    {
                        case 0:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 40f, playerPosition.Y - 64f + 8f), new Rectangle?(sourceRect), Color.White, -0.7853982f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() - 1) / 10000f));
                            return;
                        case 1:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 56f, playerPosition.Y - 64f + 28f), new Rectangle?(sourceRect), Color.White, 0f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() - 1) / 10000f));
                            return;
                        case 2:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 4f, playerPosition.Y - 16f), new Rectangle?(sourceRect), Color.White, 0.7853982f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() - 1) / 10000f));
                            return;
                        case 3:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 4f, playerPosition.Y - 4f), new Rectangle?(sourceRect), Color.White, 1.57079637f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                            return;
                        case 4:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 28f, playerPosition.Y + 4f), new Rectangle?(sourceRect), Color.White, 1.96349549f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                            return;
                        case 5:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 48f, playerPosition.Y + 4f), new Rectangle?(sourceRect), Color.White, 2.3561945f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                            return;
                        case 6:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 48f, playerPosition.Y + 4f), new Rectangle?(sourceRect), Color.White, 2.3561945f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                            return;
                        case 7:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 16f, playerPosition.Y + 64f + 12f), new Rectangle?(sourceRect), Color.White, 1.96349537f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                            return;
                        default:
                            return;
                    }
                }
                else if (facingDirection == 3)
                {
                    switch (frameOfFarmerAnimation)
                    {
                        case 0:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 16f, playerPosition.Y - 64f - 16f), new Rectangle?(sourceRect), Color.White, 0.7853982f, meleeWeaponCenter, 4f, SpriteEffects.FlipHorizontally, Math.Max(0f, (float)(c.getStandingY() - 1) / 10000f));
                            return;
                        case 1:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 48f, playerPosition.Y - 64f + 20f), new Rectangle?(sourceRect), Color.White, 0f, meleeWeaponCenter, 4f, SpriteEffects.FlipHorizontally, Math.Max(0f, (float)(c.getStandingY() - 1) / 10000f));
                            return;
                        case 2:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 64f + 32f, playerPosition.Y + 16f), new Rectangle?(sourceRect), Color.White, -0.7853982f, meleeWeaponCenter, 4f, SpriteEffects.FlipHorizontally, Math.Max(0f, (float)(c.getStandingY() - 1) / 10000f));
                            return;
                        case 3:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 4f, playerPosition.Y + 44f), new Rectangle?(sourceRect), Color.White, -1.57079637f, meleeWeaponCenter, 4f, SpriteEffects.FlipHorizontally, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                            return;
                        case 4:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 44f, playerPosition.Y + 52f), new Rectangle?(sourceRect), Color.White, -1.96349549f, meleeWeaponCenter, 4f, SpriteEffects.FlipHorizontally, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                            return;
                        case 5:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 80f, playerPosition.Y + 40f), new Rectangle?(sourceRect), Color.White, -2.3561945f, meleeWeaponCenter, 4f, SpriteEffects.FlipHorizontally, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                            return;
                        case 6:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 80f, playerPosition.Y + 40f), new Rectangle?(sourceRect), Color.White, -2.3561945f, meleeWeaponCenter, 4f, SpriteEffects.FlipHorizontally, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                            return;
                        case 7:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 44f, playerPosition.Y + 96f), new Rectangle?(sourceRect), Color.White, -5.105088f, meleeWeaponCenter, 4f, SpriteEffects.FlipVertically, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                            return;
                        default:
                            return;
                    }
                }
                else if (facingDirection == 0)
                {
                    switch (frameOfFarmerAnimation)
                    {
                        case 0:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 32f, playerPosition.Y - 32f), new Rectangle?(sourceRect), Color.White, -2.3561945f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() - 32 - 8) / 10000f));
                            return;
                        case 1:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 32f, playerPosition.Y - 48f), new Rectangle?(sourceRect), Color.White, -1.57079637f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() - 32 - 8) / 10000f));
                            return;
                        case 2:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 48f, playerPosition.Y - 52f), new Rectangle?(sourceRect), Color.White, -1.17809725f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() - 32 - 8) / 10000f));
                            return;
                        case 3:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 48f, playerPosition.Y - 52f), new Rectangle?(sourceRect), Color.White, -0.3926991f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() - 32 - 8) / 10000f));
                            return;
                        case 4:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 8f, playerPosition.Y - 40f), new Rectangle?(sourceRect), Color.White, 0f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() - 32 - 8) / 10000f));
                            return;
                        case 5:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f, playerPosition.Y - 40f), new Rectangle?(sourceRect), Color.White, 0.3926991f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() - 32 - 8) / 10000f));
                            return;
                        case 6:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f, playerPosition.Y - 40f), new Rectangle?(sourceRect), Color.White, 0.3926991f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() - 32 - 8) / 10000f));
                            return;
                        case 7:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 44f, playerPosition.Y + 64f), new Rectangle?(sourceRect), Color.White, -1.96349537f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() - 32 - 8) / 10000f));
                            return;
                        default:
                            return;
                    }
                }
                else if (facingDirection == 2)
                {
                    switch (frameOfFarmerAnimation)
                    {
                        case 0:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 56f, playerPosition.Y - 16f), new Rectangle?(sourceRect), Color.White, 0.3926991f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                            return;
                        case 1:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 52f, playerPosition.Y - 8f), new Rectangle?(sourceRect), Color.White, 1.57079637f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                            return;
                        case 2:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 40f, playerPosition.Y), new Rectangle?(sourceRect), Color.White, 1.57079637f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                            return;
                        case 3:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 16f, playerPosition.Y + 4f), new Rectangle?(sourceRect), Color.White, 2.3561945f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                            return;
                        case 4:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 8f, playerPosition.Y + 8f), new Rectangle?(sourceRect), Color.White, 3.14159274f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                            return;
                        case 5:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 12f, playerPosition.Y), new Rectangle?(sourceRect), Color.White, 3.53429174f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                            return;
                        case 6:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 12f, playerPosition.Y), new Rectangle?(sourceRect), Color.White, 3.53429174f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                            return;
                        case 7:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 44f, playerPosition.Y + 64f), new Rectangle?(sourceRect), Color.White, -5.105088f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                            return;
                        default:
                            return;
                    }
                }
            }
            else
            {
                frameOfFarmerAnimation %= 2;
                if (facingDirection == 1)
                {
                    if (frameOfFarmerAnimation == 0)
                    {
                        spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 16f, playerPosition.Y - 16f), new Rectangle?(sourceRect), Color.White, 0.7853982f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                        return;
                    }
                    if (frameOfFarmerAnimation != 1)
                    {
                        return;
                    }
                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 8f, playerPosition.Y - 24f), new Rectangle?(sourceRect), Color.White, 0.7853982f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                    return;
                }
                else if (facingDirection == 3)
                {
                    if (frameOfFarmerAnimation == 0)
                    {
                        spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 16f, playerPosition.Y - 16f), new Rectangle?(sourceRect), Color.White, -2.3561945f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                        return;
                    }
                    if (frameOfFarmerAnimation != 1)
                    {
                        return;
                    }
                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 8f, playerPosition.Y - 24f), new Rectangle?(sourceRect), Color.White, -2.3561945f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                    return;
                }
                else if (facingDirection == 0)
                {
                    if (frameOfFarmerAnimation == 0)
                    {
                        spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 4f, playerPosition.Y - 40f), new Rectangle?(sourceRect), Color.White, -0.7853982f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() - 32) / 10000f));
                        return;
                    }
                    if (frameOfFarmerAnimation != 1)
                    {
                        return;
                    }
                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 16f, playerPosition.Y - 48f), new Rectangle?(sourceRect), Color.White, -0.7853982f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() - 32) / 10000f));
                    return;
                }
                else if (facingDirection == 2)
                {
                    if (frameOfFarmerAnimation == 0)
                    {
                        spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 32f, playerPosition.Y - 12f), new Rectangle?(sourceRect), Color.White, 2.3561945f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                        return;
                    }
                    if (frameOfFarmerAnimation != 1)
                    {
                        return;
                    }
                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 21f, playerPosition.Y), new Rectangle?(sourceRect), Color.White, 2.3561945f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                }
            }
        }

        public Vector2 GetToolLocation(bool ignoreClick = false)
        {
            Vector2 lastClick;
            if (aggroMonster != null)
            {
                lastClick = aggroMonster.getTileLocation();
                aggroMonsterLastTileLocation = lastClick;
            }
            else
            {
                lastClick = aggroMonsterLastTileLocation;
            }
            if ((int)(lastClick.X) == me.getTileX() && (int)(lastClick.Y) == me.getTileY())
            {
                Rectangle bb = me.GetBoundingBox();
                switch (me.FacingDirection)
                {
                    case 0:
                        return new Vector2((float)(bb.X + bb.Width / 2), (float)(bb.Y - 64));
                    case 1:
                        return new Vector2((float)(bb.X + bb.Width + 64), (float)(bb.Y + bb.Height / 2));
                    case 2:
                        return new Vector2((float)(bb.X + bb.Width / 2), (float)(bb.Y + bb.Height + 64));
                    case 3:
                        return new Vector2((float)(bb.X - 64), (float)(bb.Y + bb.Height / 2));
                }
            }
            if (!ignoreClick && !lastClick.Equals(Vector2.Zero) && ((int)(lastClick.X) != me.getTileX() || (int)(lastClick.Y) != me.getTileY()) && Utility.distance(lastClick.X, (float)me.getStandingX(), lastClick.Y, (float)me.getStandingY()) <= 128f)
            {
                return lastClick;
            }
            Rectangle boundingBox = me.GetBoundingBox();
            switch (me.FacingDirection)
            {
                case 0:
                    return new Vector2((float)(boundingBox.X + boundingBox.Width / 2), (float)(boundingBox.Y - 32));
                case 1:
                    return new Vector2((float)(boundingBox.X + boundingBox.Width + 32), (float)(boundingBox.Y + boundingBox.Height / 2));
                case 2:
                    return new Vector2((float)(boundingBox.X + boundingBox.Width / 2), (float)(boundingBox.Y + boundingBox.Height + 32));
                case 3:
                    return new Vector2((float)(boundingBox.X - 32), (float)(boundingBox.Y + boundingBox.Height / 2));
            }
            return new Vector2((float)me.getStandingX(), (float)me.getStandingY());
        }

        #endregion
    }

    public partial class AI_StateAggroEnemy
    {
        private class CombatBehavior
        {
            protected AI_StateAggroEnemy combatState;

            public CombatBehavior(AI_StateAggroEnemy combatState)
            {
                this.combatState = combatState;
            }

            public virtual void StartBehavior()
            {
            }

            public virtual void StopBehavior()
            {

            }

            public virtual void SelfTransition()
            {
                StartBehavior();
            }

            public virtual void Update(UpdateTickedEventArgs e)
            {
                if (TransitionsCheck())
                    return;
            }

            public virtual bool TransitionsCheck()
            {
                return false;
            }

        }

        private class AggressiveMeleeWeaponBehavior : CombatBehavior
        {
            public AggressiveMeleeWeaponBehavior(AI_StateAggroEnemy combatState) : base(combatState)
            {
            }
        }

        private class DefendingMeleeWeaponBehavior : CombatBehavior
        {
            public DefendingMeleeWeaponBehavior(AI_StateAggroEnemy combatState) : base(combatState)
            {
            }
        }

        private class CoweringMeleeWeaponBehavior : CombatBehavior
        {
            public CoweringMeleeWeaponBehavior(AI_StateAggroEnemy combatState) : base(combatState)
            {
            }
        }

    }
}
