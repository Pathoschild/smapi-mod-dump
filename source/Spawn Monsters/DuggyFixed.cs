using Microsoft.Xna.Framework;
using StardewValley;

namespace Spawn_Monsters
{
	class DuggyFixed : StardewValley.Monsters.Duggy
	{
		public DuggyFixed(Vector2 pos) : base(pos) {

		}

		public override void behaviorAtGameTick(GameTime time) {
			//base.behaviorAtGameTick(time); !I cant explain why but it works like this
			this.isEmoting = false;
			this.Sprite.loop = false;
			Rectangle boundingBox = this.GetBoundingBox();
			if (this.Sprite.currentFrame < 4) {
				boundingBox.Inflate(128, 128);
				if (!this.IsInvisible || boundingBox.Contains(this.Player.getStandingX(), this.Player.getStandingY())) {
					if (this.IsInvisible) {
						if (this.currentLocation.map.GetLayer("Back").Tiles[(int)this.Player.getTileLocation().X, (int)this.Player.getTileLocation().Y].Properties.ContainsKey("NPCBarrier") || !this.currentLocation.map.GetLayer("Back").Tiles[(int)this.Player.getTileLocation().X, (int)this.Player.getTileLocation().Y].TileIndexProperties.ContainsKey("Diggable") && this.currentLocation.map.GetLayer("Back").Tiles[(int)this.Player.getTileLocation().X, (int)this.Player.getTileLocation().Y].TileIndex != 0) {
							return;
						}
							
						this.Position = new Vector2(this.Player.Position.X, this.Player.Position.Y + (float)this.Player.Sprite.SpriteHeight - (float)this.Sprite.SpriteHeight);
						this.currentLocation.localSound(nameof(StardewValley.Monsters.Duggy));
						this.Position = this.Player.getTileLocation() * 64f;
					}
					this.IsInvisible = false;
					this.Sprite.interval = 100f;
					this.Sprite.AnimateDown(time, 0, "");
				}
			}
			if (this.Sprite.currentFrame >= 4 && this.Sprite.currentFrame < 8) {
				boundingBox.Inflate((int)sbyte.MinValue, (int)sbyte.MinValue);
				this.currentLocation.isCollidingPosition(boundingBox, Game1.viewport, false, 8, false, (Character)this);
				this.Sprite.AnimateRight(time, 0, "");
				this.Sprite.interval = 220f;
				this.DamageToFarmer = 8;
			}
			if (this.Sprite.currentFrame >= 8)
				this.Sprite.AnimateUp(time, 0, "");
			if (this.Sprite.currentFrame < 10)
				return;
			this.IsInvisible = true;
			this.Sprite.currentFrame = 0;
			Vector2 tileLocation = this.getTileLocation();
			//this.currentLocation.map.GetLayer("Back").Tiles[(int)tileLocation.X, (int)tileLocation.Y].TileIndex = 0;
			//this.currentLocation.removeEverythingExceptCharactersFromThisTile((int)tileLocation.X, (int)tileLocation.Y);
			this.DamageToFarmer = 0;
		}
	}
}
