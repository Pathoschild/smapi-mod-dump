/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/arannya/BoomerangMod
**
*************************************************/

using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.TerrainFeatures;

namespace Boomerang
{
    [XmlType("Mods_Arannya_Boomerang_ThrownBoomerang")]
    public class ThrownBoomerang : Projectile
    {
        private const int damage_c = 25;
        private const int speed_c = 12;
        private readonly NetInt Damage = new(3);
        public readonly NetVector2 Target = new();
        private readonly NetFloat Speed = new(1);
        public bool Destroyed = false;
        public List<NPC> NpcsHit = new();

        public ThrownBoomerang()
        {
            this.NetFields.AddField(this.Damage).AddField(this.Target).AddField(this.Speed);
        }

        public ThrownBoomerang(Farmer thrower, Vector2 target)
        {
            this.position.X = thrower.getStandingPosition().X - 16;
            this.position.Y = thrower.getStandingPosition().Y - 64;
            this.theOneWhoFiredMe.Set(thrower.currentLocation, thrower);
            this.damagesMonsters.Value = true;
            this.Damage.Value = damage_c;
            this.Target.Value = target;
            this.Speed.Value = speed_c;
            this.boundingBoxWidth.Set(64);
            this.NetFields.AddField(this.Damage).AddField(this.Target).AddField(this.Speed);
        }

        public override void behaviorOnCollisionWithMonster(NPC n, GameLocation location)
        {
            if (this.NpcsHit.Contains(n))
                return;

            this.NpcsHit.Add(n);
            if (n is Monster)
                location.damageMonster(this.getBoundingBox(), this.Damage.Value, this.Damage.Value, false, (Farmer)this.theOneWhoFiredMe.Get(location));
        }

        public override void behaviorOnCollisionWithOther(GameLocation location)
        {
        }

        public override void behaviorOnCollisionWithPlayer(GameLocation location, Farmer player)
        {
        }

        public override void behaviorOnCollisionWithTerrainFeature(TerrainFeature t, Vector2 tileLocation, GameLocation location)
        {
        }

        public override bool update(GameTime time, GameLocation location)
        {
            base.update(time, location);
            return this.Destroyed;
        }

        public override void updatePosition(GameTime time)
        {
            Vector2 targetDiff = this.Target.Value - this.position.Value;
            Vector2 targetDir = targetDiff;
            targetDir.Normalize();

            if (targetDiff.Length() < this.Speed.Value)
                this.position.Value = this.Target.Value;
            else
                this.position.Value += targetDir * this.Speed.Value;
        }

        public override void draw(SpriteBatch b)
        {
            var sourceRect = Game1.getSquareSourceRectForNonStandardTileSheet(ItemRegistry.GetData(ModEntry.itemID_c).GetTexture(), 16, 16, 1);
            b.Draw(ItemRegistry.GetData(ModEntry.itemID_c).GetTexture(), Game1.GlobalToLocal(Game1.viewport, this.position.Value + new Vector2(32, 32)), sourceRect, Color.White, this.rotation, new Vector2(8, 8), 4, SpriteEffects.FlipVertically, 1);
            this.rotation += 0.25f;
        }

        public Vector2 GetPosition()
        {
            return this.position.Value;
        }
        
    }
}
