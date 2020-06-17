using Microsoft.Xna.Framework;
using StardewValley.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GekosBows {
	class ProjectileArrow : BasicProjectile {

		public ProjectileArrow(
			int damage,
			int parentSheetIndex,
			int tailLength,
			float xVelocity,
			float yVelocity,
			Vector2 startingPosition) : base(damage, parentSheetIndex, 0, tailLength, 0, xVelocity, yVelocity, startingPosition, "flameSpellHit", "flameSpell", true, false, null, null, false, (BasicProjectile.onCollisionBehavior)null) {

		}

	}
}
