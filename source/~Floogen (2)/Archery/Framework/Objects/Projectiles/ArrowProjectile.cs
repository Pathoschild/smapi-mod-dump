/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Archery.Framework.Interfaces.Internal;
using Archery.Framework.Interfaces.Internal.Events;
using Archery.Framework.Models.Weapons;
using Archery.Framework.Objects.Items;
using Archery.Framework.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using System.Collections.Generic;
using System.Linq;

namespace Archery.Framework.Objects.Projectiles
{
    internal class ArrowProjectile : BasicProjectile
    {
        private const int VANILLA_STONE_SPRITE_ID = 390;

        private WeaponModel _weaponModel;
        private AmmoModel _ammoModel;
        private Farmer _owner;

        private int _tailTimer;
        private Queue<Vector2> _tail;

        private float _startingAlpha;
        private int _lightId;

        private int _baseDamage;
        private int _collectiveDamage;
        private float _breakChance;
        private float _criticalChance;
        private float _criticalDamageMultiplier;
        private float _knockback;

        private bool _isExplosive;
        private int _explosionRadius;
        private int _explosionDamage;

        private bool _shouldCheckForOnFire;

        public ArrowProjectile(WeaponModel weaponModel, AmmoModel ammoModel, Farmer owner, float rotationVelocity, float xVelocity, float yVelocity, Vector2 startingPosition, string collisionSound, string firingSound, bool damagesMonsters = false, GameLocation location = null, bool spriteFromObjectSheet = false, onCollisionBehavior collisionBehavior = null) : base(0, VANILLA_STONE_SPRITE_ID, 0, ammoModel is not null && ammoModel.Trail is not null ? ammoModel.Trail.Amount : 0, rotationVelocity, xVelocity, yVelocity, startingPosition, collisionSound, firingSound, false, damagesMonsters, location, owner, spriteFromObjectSheet, collisionBehavior)
        {
            _weaponModel = weaponModel;
            _ammoModel = ammoModel;
            _owner = owner;

            _tailTimer = 0;
            _tail = new Queue<Vector2>();

            _startingAlpha = 1f;

            _knockback = weaponModel.Knockback * (1f + _owner.knockbackModifier);

            _baseDamage = ammoModel.Damage;
            _breakChance = ammoModel.BreakChance;
            _collectiveDamage = (int)(weaponModel.DamageRange.Get(Game1.random, maxOffset: _baseDamage) * (1f + _owner.attackIncreaseModifier));
            _criticalChance = Utility.Clamp(_weaponModel.CriticalChance + _ammoModel.CriticalChance, 0f, 1f) * (1f + _owner.critChanceModifier);
            _criticalDamageMultiplier = Utility.Clamp(_weaponModel.CriticalDamageMultiplier + _ammoModel.CriticalDamageMultiplier, 1f, float.MaxValue) * (1f + _owner.critPowerModifier);

            _isExplosive = ammoModel.Explosion is not null;
            _explosionRadius = ammoModel.Explosion is not null ? ammoModel.Explosion.Radius : 0;
            _explosionDamage = ammoModel.Explosion is not null ? ammoModel.Explosion.Damage : 0;

            base.bouncesLeft.Value = _ammoModel.BounceCountRange is null ? 0 : _ammoModel.BounceCountRange.Get(Game1.random);

            base.maxTravelDistance.Value = _ammoModel.MaxTravelDistance;

            base.rotationVelocity.Value = _ammoModel.RotationVelocity;

            if (ammoModel.Light is not null)
            {
                base.light.Value = true;
            }

            if (_ammoModel.Enchantment is not null && Archery.internalApi.GetEnchantmentTriggerType(_ammoModel.Enchantment.Id) is TriggerType.OnFire && _ammoModel.Enchantment.ShouldTrigger(Game1.random))
            {
                _shouldCheckForOnFire = true;
            }
        }

        internal ProjectileData GetData()
        {
            return new ProjectileData()
            {
                AmmoId = _ammoModel.Id,
                BaseDamage = _baseDamage,
                BreakChance = _criticalChance,
                CriticalChance = _criticalChance,
                CriticalDamageMultiplier = _criticalDamageMultiplier,
                Position = base.position.Value,
                Velocity = new Vector2(base.xVelocity.Value, base.yVelocity.Value),
                InitialSpeed = _weaponModel.ProjectileSpeed,
                Rotation = base.rotation,
                DoesExplodeOnImpact = _isExplosive,
                ExplosionRadius = _explosionRadius,
                ExplosionDamage = _explosionDamage,
                Knockback = _knockback
            };
        }

        internal void Override(IProjectileData projectileData)
        {
            if (Archery.modelManager.GetSpecificModel<AmmoModel>(projectileData.AmmoId) is AmmoModel ammoModel)
            {
                _ammoModel = ammoModel;
            }

            if (projectileData.BaseDamage is not null)
            {
                _baseDamage = projectileData.BaseDamage.Value;
                _collectiveDamage = (int)(_weaponModel.DamageRange.Get(Game1.random, maxOffset: _baseDamage) * (1f + _owner.attackIncreaseModifier));
            }

            if (projectileData.BreakChance is not null)
            {
                _breakChance = projectileData.BreakChance.Value;
            }

            if (projectileData.CriticalChance is not null)
            {
                _criticalChance = projectileData.CriticalChance.Value;
            }
            if (projectileData.CriticalDamageMultiplier is not null)
            {
                _criticalDamageMultiplier = projectileData.CriticalDamageMultiplier.Value;
            }

            if (projectileData.Position is not null)
            {
                base.position.Value = projectileData.Position.Value;
            }

            if (projectileData.Velocity is not null)
            {
                base.xVelocity.Value = projectileData.Velocity.Value.X;
                base.yVelocity.Value = projectileData.Velocity.Value.Y;
            }

            if (projectileData.Rotation is not null)
            {
                base.rotation = projectileData.Rotation.Value;
            }

            if (projectileData.Knockback is not null)
            {
                _knockback = projectileData.Knockback.Value;
            }
        }

        internal void HandleProjectileBreakage(GameLocation location, Vector2 position, int facingDirection)
        {
            //var playerLuckChance = Utility.Clamp(Game1.player.LuckLevel / 10f, 0f, 1f) + Game1.player.DailyLuck;
            if (AmmoModel.CanBreak(_breakChance) && (AmmoModel.ShouldAlwaysBreak(_breakChance) || Game1.random.NextDouble() < _breakChance))
            {
                // Draw debris based on ammo's sprite
                if (_ammoModel.Debris is not null)
                {
                    Game1.createRadialDebris(location, _ammoModel.TexturePath, _ammoModel.Debris.Source, (int)(base.position.X + 32f) / 64, (int)(base.position.Y + 32f) / 64, _ammoModel.Debris.Amount);
                }
            }
            else
            {
                // Drop the ammo
                Game1.createItemDebris(Arrow.CreateInstance(_ammoModel), position, facingDirection, location);
            }
        }

        public override bool update(GameTime time, GameLocation location)
        {
            if (Game1.IsMasterGame && base.hostTimeUntilAttackable > 0f)
            {
                base.hostTimeUntilAttackable -= (float)time.ElapsedGameTime.TotalSeconds;
                if (base.hostTimeUntilAttackable <= 0f)
                {
                    base.ignoreMeleeAttacks.Value = false;
                    base.hostTimeUntilAttackable = -1f;
                }
            }

            if (base.light.Value)
            {
                var lightModel = _ammoModel.Light;
                if (!base.hasLit)
                {
                    base.hasLit = true;
                    _lightId = Game1.random.Next(int.MinValue, int.MaxValue);
                    Game1.currentLightSources.Add(new LightSource(lightModel.GetTextureSource(), base.position + lightModel.Offset, lightModel.GetRadius(), lightModel.GetColor(), _lightId, LightSource.LightContext.None, 0L));
                }
                else
                {
                    if (lightModel.RecalculateIfElapsed(time) && Utility.getLightSource(_lightId) is LightSource lightSource)
                    {
                        lightSource.radius.Value = lightModel.GetRadius();
                        lightSource.color.Value *= _startingAlpha;
                    }

                    Utility.repositionLightSource(_lightId, base.position + lightModel.Offset);
                }
            }

            base.rotation += base.rotationVelocity.Value;
            base.travelTime += time.ElapsedGameTime.Milliseconds;
            if (base.scaleGrow.Value != 0f)
            {
                base.localScale += base.scaleGrow.Value;
            }

            Vector2 old_position = base.position.Value;
            base.updatePosition(time);

            // Update the arrow tail
            updateTail(time);

            base.travelDistance += (old_position - base.position.Value).Length();
            if (base.maxTravelDistance.Value >= 0)
            {
                if (base.travelDistance > base.maxTravelDistance.Value - 128)
                {
                    // Fade arrows if starting to go past travel distance
                    _startingAlpha = (base.maxTravelDistance.Value - base.travelDistance) / 128f;
                }

                if (base.travelDistance >= base.maxTravelDistance.Value)
                {
                    // Remove light source
                    if (base.hasLit)
                    {
                        Utility.removeLightSource(_lightId);
                    }

                    return true;
                }
            }

            if (this.isColliding(location) && (base.travelTime > 100 || base.ignoreTravelGracePeriod.Value))
            {
                if (base.bouncesLeft.Value <= 0)
                {
                    return this.behaviorOnCollision(location);
                }

                base.bouncesLeft.Value--;
                bool[] array = Utility.horizontalOrVerticalCollisionDirections(this.getBoundingBox(), base.theOneWhoFiredMe.Get(location), projectile: true);
                if (array[0])
                {
                    base.xVelocity.Value = 0f - base.xVelocity.Value;
                }
                if (array[1])
                {
                    base.yVelocity.Value = 0f - base.yVelocity.Value;
                }
            }

            // Check for any enchantment OnFire behavior
            if (_shouldCheckForOnFire && Archery.internalApi.HandleEnchantment(_ammoModel.Type, _ammoModel.Enchantment.Id, _ammoModel.Enchantment.Generate(this, Game1.currentGameTime, Game1.currentLocation, _owner, null, null)) is false)
            {
                _shouldCheckForOnFire = false;
            }

            return false;
        }

        public bool behaviorOnCollision(GameLocation location)
        {
            if (base.hasLit)
            {
                // Handle removing light source
                Utility.removeLightSource(_lightId);
            }

            foreach (Vector2 tile in Utility.getListOfTileLocationsForBordersOfNonTileRectangle(this.getBoundingBox()))
            {
                if (location.terrainFeatures.ContainsKey(tile) && !location.terrainFeatures[tile].isPassable())
                {
                    base.behaviorOnCollisionWithTerrainFeature(location.terrainFeatures[tile], tile, location);
                    return true;
                }

                if (base.damagesMonsters.Value)
                {
                    NPC i = location.doesPositionCollideWithCharacter(this.getBoundingBox());
                    if (i is not null && i.IsMonster)
                    {
                        this.behaviorOnCollisionWithMonster(i, location);
                        return true;
                    }
                }
            }

            // Note: behaviorOnCollisionWithOther handles collisions with walls / barriers
            this.behaviorOnCollisionWithOther(location);

            // Play ammo impact sound
            Toolkit.PlaySound(_ammoModel.ImpactSound, _ammoModel.Id, base.position.Value);

            return true;
        }

        public override void behaviorOnCollisionWithOther(GameLocation location)
        {
            if (_isExplosive)
            {
                Archery.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(362, Game1.random.Next(30, 90), 6, 1, base.position, flicker: false, (Game1.random.NextDouble() < 0.5) ? true : false));

                location.explode(new Vector2(base.position.X / 64, base.position.Y / 64), _explosionRadius, _owner, false, _explosionDamage);
            }

            // See if the ammo should break
            HandleProjectileBreakage(location, base.position.Value, Game1.down);

            base.behaviorOnCollisionWithOther(location);
        }

        public override void behaviorOnCollisionWithMonster(NPC n, GameLocation location)
        {
            if (n is not Monster)
            {
                return;
            }
            var monster = (Monster)n;

            // See if the ammo should break
            HandleProjectileBreakage(location, monster.getStandingPosition(), monster.FacingDirection);

            // Damage the monster
            int damageDone = monster.Health;
            location.damageMonster(monster.GetBoundingBox(), _collectiveDamage, _collectiveDamage, isBomb: false, _knockback, 0, _criticalChance, _criticalDamageMultiplier, triggerMonsterInvincibleTimer: false, (base.theOneWhoFiredMe.Get(location) is Farmer) ? (base.theOneWhoFiredMe.Get(location) as Farmer) : Game1.player);
            damageDone -= monster.Health;

            if (_isExplosive)
            {
                Archery.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(362, Game1.random.Next(30, 90), 6, 1, base.position, flicker: false, (Game1.random.NextDouble() < 0.5) ? true : false));

                location.explode(new Vector2(base.position.X / 64, base.position.Y / 64), _explosionRadius, _owner, false, _explosionDamage);
            }

            // Check for any enchantment OnImpact behavior
            if (_ammoModel.Enchantment is not null && Archery.internalApi.GetEnchantmentTriggerType(_ammoModel.Enchantment.Id) is TriggerType.OnImpact && _ammoModel.Enchantment.ShouldTrigger(Game1.random))
            {
                Archery.internalApi.HandleEnchantment(_ammoModel.Type, _ammoModel.Enchantment.Id, _ammoModel.Enchantment.Generate(this, Game1.currentGameTime, Game1.currentLocation, _owner, monster, damageDone));
            }

            // Trigger event
            Archery.internalApi.TriggerOnAmmoHitMonster(new AmmoHitMonsterEventArgs() { WeaponId = _weaponModel.Id, AmmoId = _ammoModel.Id, Monster = monster, Projectile = this, Origin = this.position.Value, DamageDone = damageDone });
        }

        public override bool isColliding(GameLocation location)
        {
            var collisionBox = this.getBoundingBox();
            foreach (var monster in location.characters)
            {
                if (monster.IsMonster is false)
                {
                    continue;
                }

                if (monster.GetBoundingBox().Intersects(collisionBox))
                {
                    return true;
                }
            }

            return base.isColliding(location);
        }

        public override Rectangle getBoundingBox()
        {
            Vector2 pos = base.position.Value;

            Rectangle collisionBox;
            if (_ammoModel.CollisionBox is not null)
            {
                collisionBox = _ammoModel.CollisionBox.Value;
            }
            else
            {
                var projectileSprite = _ammoModel.GetProjectileSprite(_owner);
                collisionBox = new Rectangle(0, 0, projectileSprite.Source.Width, projectileSprite.Source.Height);
            }

            float currentScale = base.localScale * 4f;
            var rotationVector = Vector2.Transform(new Vector2(collisionBox.X, collisionBox.Y), Matrix.CreateRotationZ(base.rotation));
            collisionBox.X = (int)(currentScale * rotationVector.X);
            collisionBox.Y = (int)(currentScale * rotationVector.Y);
            collisionBox.Width = (int)currentScale * collisionBox.Width;
            collisionBox.Height = (int)currentScale * collisionBox.Height;

            return new Rectangle((int)pos.X - (collisionBox.Width / 2) + collisionBox.X, (int)pos.Y - (collisionBox.Height / 2) + collisionBox.Y, collisionBox.Width, collisionBox.Height);
        }

        // Re-implementing this class, as it is private in vanilla / not overridable
        private void updateTail(GameTime time)
        {
            _tailTimer -= time.ElapsedGameTime.Milliseconds;
            if (_tailTimer <= 0)
            {
                _tailTimer = _ammoModel.Trail is not null ? _ammoModel.Trail.SpawnIntervalInMilliseconds : 50;
                _tail.Enqueue(this.position);
                if (_tail.Count > base.tailLength.Value)
                {
                    _tail.Dequeue();
                }
            }
        }

        public override void draw(SpriteBatch b)
        {
            var ammoSprite = _ammoModel.GetProjectileSprite(_owner);
            if (ammoSprite is null)
            {
                return;
            }

            // Draw the arrow trail / tail
            float current_scale = 4f * base.localScale;
            float alpha = 1f;

            if (_ammoModel.Trail is not null)
            {
                for (int i = _tail.Count - 1; i >= 0; i--)
                {
                    b.Draw(_ammoModel.Texture, Game1.GlobalToLocal(Game1.viewport, Vector2.Lerp((i == _tail.Count - 1) ? ((Vector2)base.position) : _tail.ElementAt(i + 1), _tail.ElementAt(i), _ammoModel.Trail.SpacingStep)), _ammoModel.Trail.Source, base.color.Value * alpha * _startingAlpha, base.rotation, _ammoModel.Trail.Offset, current_scale, SpriteEffects.None, (base.position.Y - (float)(_tail.Count - i) + 96f) / 10000f);

                    if (_ammoModel.Trail.AlphaStep is not null)
                    {
                        alpha -= _ammoModel.Trail.AlphaStep.Value;
                    }

                    if (_ammoModel.Trail.ScaleStep is not null)
                    {
                        current_scale -= (i * _ammoModel.Trail.ScaleStep.Value);
                    }
                }
            }

            // Draw the arrow
            b.Draw(_ammoModel.Texture, Game1.GlobalToLocal(Game1.viewport, base.position), ammoSprite.Source, base.color.Value * _startingAlpha, base.rotation, ammoSprite.Source.Size.ToVector2(), 4f * base.localScale, SpriteEffects.None, (base.position.Y + 96f) / 10000f);

            // Draw collision box, if enabled
            if (Archery.shouldShowAmmoCollisionBox)
            {
                Toolkit.DrawHitBox(b, getBoundingBox());
            }
        }
    }
}
