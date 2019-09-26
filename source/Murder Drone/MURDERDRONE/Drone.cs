using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Projectiles;
using StardewValley.Monsters;
using StardewModdingAPI;
using Netcode;
using StardewValley.Tools;

namespace MURDERDRONE
{
    public class Drone : NPC
    {
        private readonly float r = 80f;
        private float t;
        private readonly float offsetY = 20f;
        private readonly float offsetX = 5f;
        private bool throwing;
        private bool thrown;
        private Monster target;
        private BasicProjectile basicProjectile;
        private int damage;
        private readonly float projectileVelocity;
        private readonly IModHelper helper;

        public Drone()
        {
        }

        public Drone(int speed, int damage, float projectileVelocity, IModHelper helper)
        : base(new AnimatedSprite("Sidekick/Drone", 1, 12, 12), Game1.player.Position, 1, "Drone")
        {
            this.speed = speed;
            this.hideShadow.Value = true;
            this.damage = damage;
            this.projectileVelocity = projectileVelocity;
            this.helper = helper;
        }

        public override bool CanSocialize => false;

        public override bool canTalk()
        {
            return false;
        }

        public override void doEmote(int whichEmote, bool playSound, bool nextEventCommand = true)
        {
        }

        public override void update(GameTime time, GameLocation location)
        {
            float newX = Game1.player.position.X + offsetX + r * (float)Math.Cos(t * 2 * Math.PI);
            float newY = Game1.player.position.Y - offsetY + r * (float)Math.Sin(t * 2 * Math.PI);
            position.Set(new Vector2(newX, newY));

            t = (t + (float)time.ElapsedGameTime.TotalMilliseconds/(1000 * speed)) % 1;

            if (!throwing)
            {
                foreach (var npc in Game1.currentLocation.getCharacters())
                {
                    if (npc.IsMonster && npc.withinPlayerThreshold(3))
                    {
                        throwing = true;
                        target = (Monster)npc;
                        break;
                    }
                }
            }

            if (throwing && target.IsMonster)
                ShootTheBastard(time, location, target);
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
        }

        public virtual void ShootTheBastard(GameTime time, GameLocation location, Monster monster)
        {
            if (!thrown)
            {
                if (damage == -1)
                {
                    damage = monster.Health;
                }

                BasicProjectile.onCollisionBehavior collisionBehavior = new BasicProjectile.onCollisionBehavior(
                    delegate(GameLocation loc, int x, int y, Character who)
                    {
                        Tool currentTool = null;

                        if (Game1.player.CurrentTool != null && Game1.player.CurrentTool is Tool)
                            currentTool = Game1.player.CurrentTool;

                        if (monster is Bug bug && bug.isArmoredBug)
                            helper.Reflection.GetField<NetBool>(bug, "isArmoredBug").SetValue(new NetBool(false));

                        if (monster is RockCrab rockCrab)
                        {
                            if (Game1.player.CurrentTool != null && Game1.player.CurrentTool is Tool && currentTool != null && Game1.player.CurrentTool is Pickaxe)
                                Game1.player.CurrentTool = new MeleeWeapon(4);

                            helper.Reflection.GetField<NetBool>(rockCrab, "shellGone").SetValue(new NetBool(true));
                            helper.Reflection.GetField<NetInt>(rockCrab, "shellHealth").SetValue(new NetInt(0));
                        }

                        loc.damageMonster(monster.GetBoundingBox(), damage, damage + 1, true, !(who is Farmer) ? Game1.player : who as Farmer);

                        if (Game1.player.CurrentTool != null && Game1.player.CurrentTool is Tool && currentTool != null)
                            Game1.player.CurrentTool = currentTool;
                    }
                );

                string collisionSound = "hitEnemy";

                Vector2 velocityTowardMonster = Utility.getVelocityTowardPoint(Position, monster.Position, projectileVelocity);
                basicProjectile = new BasicProjectile(
                    damage,
                    Projectile.shadowBall,
                    0,
                    0,
                    0,
                    velocityTowardMonster.X,
                    velocityTowardMonster.Y,
                    position,
                    collisionSound,
                    firingSound: "daggerswipe",
                    explode: false,
                    damagesMonsters: true,
                    location: location,
                    firer: this,
                    false,
                    collisionBehavior
                )
                {
                    IgnoreLocationCollision = (Game1.currentLocation.currentEvent != null)
                };

                location.projectiles.Add(basicProjectile);
                thrown = true;
            }

            if (thrown && basicProjectile is BasicProjectile && basicProjectile.destroyMe)
            {
                throwing = false;
                thrown = false;
                basicProjectile = null;
            }
        }
    }
}