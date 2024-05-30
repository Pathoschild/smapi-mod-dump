/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.Interfaces;
using ArsVenefici.Framework.Interfaces.Spells;
using ArsVenefici.Framework.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using xTile.Tiles;

namespace ArsVenefici.Framework.Spells.Effects
{
    public class SpellProjectile : Projectile , IEntity
    {
        private ModEntry modEntry;

        public object entity { get { return this; } }

        private IEntity Source;
        private ISpell spell;

        private NetFloat Direction = new();
        private NetFloat Velocity = new();
        private NetInt Index = new();

        private Texture2D Tex;
        //private readonly NetString TexId = new();
        private static readonly Random Rand = new();

        private SpellProjectile(ModEntry modEntry, IEntity source)
        {
            this.modEntry = modEntry;
            this.Source = source;

            this.NetFields.AddField(this.Direction);
            this.NetFields.AddField(this.Velocity);
            this.NetFields.AddField(this.Index);
            //this.NetFields.AddField(this.TexId);

            this.Tex = modEntry.Helper.ModContent.Load<Texture2D>("assets/projectile/projectile.png");

            this.damagesMonsters.Value = true;
        }

        public SpellProjectile(ModEntry modEntry, IEntity source, ISpell spell, float direction, int index, float velocity)
            : this(modEntry, source)
        {
            this.Source = source;
            this.spell = spell;
            this.Index.Value = index;
            this.Direction.Value = direction;
            this.Velocity.Value = velocity;

            if(source.entity is Character character)
            {
                this.theOneWhoFiredMe.Set(character.currentLocation, character);

                this.position.Value = character.getStandingPosition();
                this.position.X += character.GetBoundingBox().Width;
                this.position.Y += character.GetBoundingBox().Height;
            }

            this.rotation = direction;
            this.xVelocity.Value = (float)Math.Cos(this.Direction.Value) * this.Velocity.Value;
            this.yVelocity.Value = (float)Math.Sin(this.Direction.Value) * this.Velocity.Value;

            //this.Tex = Content.LoadTexture($"magic/{this.Spell.ParentSchoolId}/{this.Spell.Id}/projectile.png");
            //this.TexId.Value = Content.LoadTextureKey($"magic/{this.Spell.ParentSchoolId}/{this.Spell.Id}/projectile.png");

            //this.Tex = modEntry.Helper.ModContent.Load<Texture2D>("assets/projectile/rock.png");
            //this.TexId.Value = modEntry.Helper.ModContent.($"magic/{this.Spell.ParentSchoolId}/{this.Spell.Id}/projectile.png");
        }

        //
        // Summary:
        //     Handle the projectile hitting an obstacle.
        //
        // Parameters:
        //   location:
        //     The location containing the projectile.
        //
        //   target:
        //     The target player or monster that was hit, if applicable.
        //
        //   terrainFeature:
        //     The terrain feature that was hit, if applicable.
        private void behaviorOnCollision(GameLocation location, Character target, TerrainFeature terrainFeature)
        {
            bool flag = true;
            Farmer farmer = target as Farmer;
            if (farmer == null)
            {
                NPC nPC = target as NPC;
                if (nPC != null)
                {
                    if (!nPC.IsInvisible)
                    {
                        behaviorOnCollisionWithMonster(nPC, location);
                    }
                    else
                    {
                        flag = false;
                    }
                }
                else if (terrainFeature != null)
                {
                    behaviorOnCollisionWithTerrainFeature(terrainFeature, terrainFeature.Tile, location);
                }
                else
                {
                    behaviorOnCollisionWithOther(location);
                }
            }
            else
            {
                behaviorOnCollisionWithPlayer(location, farmer);
            }

            if (flag && piercesLeft.Value <= 0 && hasLit && Utility.getLightSource(lightID) != null)
            {
                Utility.getLightSource(lightID).fadeOut.Value = 3;
            }
        }

        private void behaviorOnCollision(GameLocation location, Character target, StardewValley.Object obj)
        {
            bool flag = true;
            Farmer farmer = target as Farmer;
            if (farmer == null)
            {
                NPC nPC = target as NPC;
                if (nPC != null)
                {
                    if (!nPC.IsInvisible)
                    {
                        behaviorOnCollisionWithMonster(nPC, location);
                    }
                    else
                    {
                        flag = false;
                    }
                }
                else if (obj != null)
                {
                    behaviorOnCollisionWithObject(obj, location);
                }
                else
                {
                    behaviorOnCollisionWithOther(location);
                }
            }
            else
            {
                behaviorOnCollisionWithPlayer(location, farmer);
            }

            if (flag && piercesLeft.Value <= 0 && hasLit && Utility.getLightSource(lightID) != null)
            {
                Utility.getLightSource(lightID).fadeOut.Value = 3;
            }
        }

        private void behaviorOnCollision(GameLocation location, Character target, ResourceClump clump)
        {
            bool flag = true;
            Farmer farmer = target as Farmer;
            if (farmer == null)
            {
                NPC nPC = target as NPC;
                if (nPC != null)
                {
                    if (!nPC.IsInvisible)
                    {
                        behaviorOnCollisionWithMonster(nPC, location);
                    }
                    else
                    {
                        flag = false;
                    }
                }
                else if (clump != null)
                {
                    behaviorOnCollisionWithResourceClump(clump, clump.Tile, location);
                }
                else
                {
                    behaviorOnCollisionWithOther(location);
                }
            }
            else
            {
                behaviorOnCollisionWithPlayer(location, farmer);
            }

            if (flag && piercesLeft.Value <= 0 && hasLit && Utility.getLightSource(lightID) != null)
            {
                Utility.getLightSource(lightID).fadeOut.Value = 3;
            }
        }

        public bool isColliding(GameLocation location, out Character target, out StardewValley.Object obj)
        {
            target = null;
            obj = null;
            Microsoft.Xna.Framework.Rectangle boundingBox = getBoundingBox();
            if (!ignoreCharacterCollisions)
            {
                if (damagesMonsters.Value)
                {
                    Character character = location.doesPositionCollideWithCharacter(boundingBox);
                    if (character != null)
                    {
                        if (character is NPC && (character as NPC).IsInvisible)
                        {
                            return false;
                        }

                        target = character;
                        return true;
                    }
                }
                else if (Game1.player.currentLocation == location && Game1.player.GetBoundingBox().Intersects(boundingBox))
                {
                    target = Game1.player;
                    return true;
                }
            }

            foreach (Vector2 item in Utility.getListOfTileLocationsForBordersOfNonTileRectangle(boundingBox))
            {
                if (location.objects.TryGetValue(item, out var value) && !value.isPassable())
                {
                    obj = value;
                    return true;
                }
            }

            if (!location.isTileOnMap(position.Value / 64f) || (!ignoreLocationCollision && location.isCollidingPosition(boundingBox, Game1.viewport, isFarmer: false, 0, glider: true, theOneWhoFiredMe.Get(location), pathfinding: false, projectile: true)))
            {
                return true;
            }

            return false;
        }

        public bool isColliding(GameLocation location, out Character target, out ResourceClump clump)
        {
            target = null;
            clump = null;
            Rectangle boundingBox = getBoundingBox();
            if (!ignoreCharacterCollisions)
            {
                if (damagesMonsters.Value)
                {
                    Character character = location.doesPositionCollideWithCharacter(boundingBox);
                    if (character != null)
                    {
                        if (character is NPC && (character as NPC).IsInvisible)
                        {
                            return false;
                        }

                        target = character;
                        return true;
                    }
                }
                else if (Game1.player.currentLocation == location && Game1.player.GetBoundingBox().Intersects(boundingBox))
                {
                    target = Game1.player;
                    return true;
                }
            }

            foreach (Vector2 item in Utility.getListOfTileLocationsForBordersOfNonTileRectangle(boundingBox))
            {
                ICollection<ResourceClump> clumps = location.resourceClumps;

                if (location is Woods woods)
                    clumps = woods.resourceClumps;

                if (clumps != null)
                {
                    foreach (var rc in clumps)
                    {
                        if(rc != null)
                        {
                            if (new Rectangle((int)rc.Tile.X, (int)rc.Tile.Y, rc.width.Value, rc.height.Value).Contains(item.X, item.Y))
                            {
                                clump = rc;
                                return true;
                            }
                        }
                    }
                }
            }

            if (!location.isTileOnMap(position.Value / 64f) || (!ignoreLocationCollision && location.isCollidingPosition(boundingBox, Game1.viewport, isFarmer: false, 0, glider: true, theOneWhoFiredMe.Get(location), pathfinding: false, projectile: true)))
            {
                return true;
            }

            return false;
        }

        public override void behaviorOnCollisionWithMonster(NPC npc, GameLocation location)
        {
            if (npc is not Monster)
                return;

            CharacterHitResult hitResult = new CharacterHitResult(npc);

            SpellHelper spellHelper = SpellHelper.Instance();
            spellHelper.Invoke(modEntry, spell, this.Source, location, hitResult, 0, Index.Value, false);

            this.Disappear(location);
        }

        public override void behaviorOnCollisionWithOther(GameLocation location)
        {
            this.Disappear(location);
        }

        public override void behaviorOnCollisionWithPlayer(GameLocation location, Farmer player)
        {
            CharacterHitResult hitResult = new CharacterHitResult(player);

            SpellHelper spellHelper = SpellHelper.Instance();
            spellHelper.Invoke(modEntry, spell, this.Source, location, hitResult, 0, Index.Value, false);

            this.Disappear(location);
        }

        public override void behaviorOnCollisionWithTerrainFeature(TerrainFeature t, Vector2 tileLocation, GameLocation location)
        {
            TerrainFeatureHitResult hitResult = new TerrainFeatureHitResult(tileLocation, this.Direction.Value, new TilePos(tileLocation), false);

            SpellHelper spellHelper = SpellHelper.Instance();
            spellHelper.Invoke(modEntry, spell, this.Source, location, hitResult, 0, Index.Value, false);

            this.Disappear(location);
        }

        public virtual void behaviorOnCollisionWithObject(StardewValley.Object obj, GameLocation location)
        {

            TerrainFeatureHitResult hitResult = new TerrainFeatureHitResult(obj.TileLocation, this.Direction.Value, new TilePos(obj.TileLocation), false);

            SpellHelper spellHelper = SpellHelper.Instance();
            spellHelper.Invoke(modEntry, spell, this.Source, location, hitResult, 0, Index.Value, false);


            this.Disappear(location);

        }

        public virtual void behaviorOnCollisionWithResourceClump(ResourceClump clump, Vector2 tileLocation, GameLocation location)
        {
            TerrainFeatureHitResult hitResult = new TerrainFeatureHitResult(tileLocation, this.Direction.Value, new TilePos(tileLocation), false);

            SpellHelper spellHelper = SpellHelper.Instance();
            spellHelper.Invoke(modEntry, spell, this.Source, location, hitResult, 0, Index.Value, false);

            this.Disappear(location);
        }

        public override Microsoft.Xna.Framework.Rectangle getBoundingBox()
        {
            return new((int)(this.position.X - Game1.tileSize), (int)(this.position.Y - Game1.tileSize), Game1.tileSize / 2, Game1.tileSize / 2);
        }

        
        public override bool update(GameTime time, GameLocation location)
        {

            if (Game1.isTimePaused)
            {
                return false;
            }

            if (Game1.IsMasterGame && hostTimeUntilAttackable > 0f)
            {
                hostTimeUntilAttackable -= (float)time.ElapsedGameTime.TotalSeconds;
                if (hostTimeUntilAttackable <= 0f)
                {
                    ignoreMeleeAttacks.Value = false;
                    hostTimeUntilAttackable = -1f;
                }
            }

            if ((bool)light)
            {
                if (!hasLit)
                {
                    hasLit = true;
                    lightID = Game1.random.Next(int.MinValue, int.MaxValue);
                    if (location.Equals(Game1.currentLocation))
                    {
                        Game1.currentLightSources.Add(new LightSource(4, position.Value + new Vector2(32f, 32f), 1f, new Color(Utility.getOppositeColor(color.Value).ToVector4() * alpha.Value), lightID, LightSource.LightContext.None, 0L));
                    }
                }
                else
                {
                    LightSource lightSource = Utility.getLightSource(lightID);
                    if (lightSource != null)
                    {
                        lightSource.color.A = (byte)(255f * alpha.Value);
                    }

                    Utility.repositionLightSource(lightID, position.Value + new Vector2(32f, 32f));
                }
            }

            alpha.Value += alphaChange.Value;
            alpha.Value = Utility.Clamp(alpha.Value, 0f, 1f);
            rotation += rotationVelocity.Value;
            travelTime += time.ElapsedGameTime.Milliseconds;
            if (scaleGrow.Value != 0f)
            {
                localScale += scaleGrow.Value;
            }

            Vector2 value = position.Value;
            updatePosition(time);
            updateTail(time);
            travelDistance += (value - position.Value).Length();
            if (maxTravelDistance.Value >= 0)
            {
                if (travelDistance > (float)((int)maxTravelDistance - 128))
                {
                    alpha.Value = ((float)(int)maxTravelDistance - travelDistance) / 128f;
                }

                if (travelDistance >= (float)(int)maxTravelDistance)
                {
                    if (hasLit)
                    {
                        Utility.removeLightSource(lightID);
                    }

                    return true;
                }
            }

            if ((travelTime > 100 || ignoreTravelGracePeriod.Value))
            {

                if(isColliding(location, out var target, out TerrainFeature terrainFeature) && ShouldApplyCollisionLocally(location))
                {
                    if ((int)bouncesLeft <= 0 || target != null)
                    {
                        behaviorOnCollision(location, target, terrainFeature);
                        return piercesLeft.Value <= 0;
                    }

                    bouncesLeft.Value--;
                    bool[] array = Utility.horizontalOrVerticalCollisionDirections(getBoundingBox(), theOneWhoFiredMe.Get(location), projectile: true);
                    if (array[0])
                    {
                        xVelocity.Value = 0f - xVelocity.Value;
                    }

                    if (array[1])
                    {
                        yVelocity.Value = 0f - yVelocity.Value;
                    }

                    if (!string.IsNullOrEmpty(bounceSound.Value))
                    {
                        location?.playSound(bounceSound.Value);
                    }
                }
                else if(isColliding(location, out var target1, out StardewValley.Object obj) && ShouldApplyCollisionLocally(location))
                {
                    if ((int)bouncesLeft <= 0 || target != null)
                    {
                        behaviorOnCollision(location, target1, obj);
                        return piercesLeft.Value <= 0;
                    }

                    bouncesLeft.Value--;
                    bool[] array = Utility.horizontalOrVerticalCollisionDirections(getBoundingBox(), theOneWhoFiredMe.Get(location), projectile: true);
                    if (array[0])
                    {
                        xVelocity.Value = 0f - xVelocity.Value;
                    }

                    if (array[1])
                    {
                        yVelocity.Value = 0f - yVelocity.Value;
                    }

                    if (!string.IsNullOrEmpty(bounceSound.Value))
                    {
                        location?.playSound(bounceSound.Value);
                    }
                }
                else if (isColliding(location, out var target2, out ResourceClump clump) && ShouldApplyCollisionLocally(location))
                {
                    if ((int)bouncesLeft <= 0 || target != null)
                    {
                        behaviorOnCollision(location, target2, clump);
                        return piercesLeft.Value <= 0;
                    }

                    bouncesLeft.Value--;
                    bool[] array = Utility.horizontalOrVerticalCollisionDirections(getBoundingBox(), theOneWhoFiredMe.Get(location), projectile: true);
                    if (array[0])
                    {
                        xVelocity.Value = 0f - xVelocity.Value;
                    }

                    if (array[1])
                    {
                        yVelocity.Value = 0f - yVelocity.Value;
                    }

                    if (!string.IsNullOrEmpty(bounceSound.Value))
                    {
                        location?.playSound(bounceSound.Value);
                    }
                }

            }

            return false;
        }


        public override void updatePosition(GameTime time)
        {

            //this.position.X += this.xVelocity.Value;
            //this.position.Y += this.yVelocity.Value;

            this.xVelocity.Value += this.acceleration.X;
            this.yVelocity.Value += this.acceleration.Y;
            if ((double)this.maxVelocity.Value != -1.0 && Math.Sqrt((double)this.xVelocity.Value * (double)this.xVelocity.Value + (double)this.yVelocity.Value * (double)this.yVelocity.Value) >= (double)this.maxVelocity.Value)
            {
                this.xVelocity.Value -= this.acceleration.X;
                this.yVelocity.Value -= this.acceleration.Y;
            }
            this.position.X += this.xVelocity.Value;
            this.position.Y += this.yVelocity.Value;
        }

        public override void draw(SpriteBatch b)
        {
            //this.Tex ??= Game1.content.Load<Texture2D>(this.TexId.Value);
            Vector2 drawPos = Game1.GlobalToLocal(new Vector2(this.getBoundingBox().X + this.getBoundingBox().Width / 2, this.getBoundingBox().Y + this.getBoundingBox().Height / 2));
            b.Draw(this.Tex, drawPos, new Microsoft.Xna.Framework.Rectangle(0, 0, this.Tex.Width, this.Tex.Height), Color.White, this.Direction.Value, new Vector2(this.Tex.Width / 2, this.Tex.Height / 2), 2, SpriteEffects.None, (float)((this.position.Y + (double)(Game1.tileSize * 3 / 2)) / 10000.0));
            //Vector2 bdp = Game1.GlobalToLocal(new Vector2(getBoundingBox().X, getBoundingBox().Y));
            //b.Draw(Mod.instance.manaFg, new Rectangle((int)bdp.X, (int)bdp.Y, getBoundingBox().Width, getBoundingBox().Height), Color.White);
        }

        private void Disappear(GameLocation loc)
        {
            //if (this.Spell?.SoundHit != null)
            //    loc.LocalSoundAtPixel(this.Spell.SoundHit, this.position.Value);

            //Game1.createRadialDebris(loc, 22 + rand.Next( 2 ), ( int ) position.X / Game1.tileSize, ( int ) position.Y / Game1.tileSize, 3 + rand.Next(5), false);
            //Game1.createRadialDebris(loc, this.TexId.Value, Game1.getSourceRectForStandardTileSheet(Projectile.projectileSheet, 0), 4, (int)this.position.X, (int)this.position.Y, 6 + SpellProjectile.Rand.Next(10), (int)(this.position.Y / (double)Game1.tileSize) + 1, new Color(255, 255, 255, 8 + SpellProjectile.Rand.Next(64)), 2.0f);
            //Game1.createRadialDebris(loc, tex, new Rectangle(0, 0, tex.Width, tex.Height), 0, ( int ) position.X, ( int ) position.Y, 3 + rand.Next(5), ( int ) position.Y / Game1.tileSize, Color.White, 5.0f);
            
            this.destroyMe = true;

            loc.projectiles.RemoveWhere((Func<Projectile, bool>)(projectile =>
            {
                return projectile.destroyMe;
            }));
        }

        public GameLocation GetGameLocation()
        {
            return Source.GetGameLocation();
        }

        public Vector2 GetPosition()
        {
            return position.Get();
        }

        public Rectangle GetBoundingBox()
        {
            return getBoundingBox();
        }

        public int GetHorizontalMovement()
        {
            return 1;
        }

        public int GetVerticalMovement()
        {
            return 1;
        }
    }
}
