using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevator
{
	public class ElevatorBuilding : Building
	{
		public ElevatorBuilding(BluePrint b, Vector2 tileLocation)
			: base(b, tileLocation)
		{
		}

		public ElevatorBuilding()
		{

		}

		public override bool doAction(Vector2 tileLocation, Farmer who)
		{
			if (who.IsLocalPlayer && tileLocation.X == (float)(humanDoor.X + tileX.Value) && tileLocation.Y == (float)(humanDoor.Y + tileY.Value))
			{
				if (who.mount != null)
				{
					Game1.showRedMessage(Game1.content.LoadString("Strings\\Buildings:DismountBeforeEntering"));
					return false;
				}
				if (who.team.buildLock.IsLocked())
				{
					Game1.showRedMessage(Game1.content.LoadString("Strings\\Buildings:CantEnter"));
					return false;
				}
				//indoors.Value.isStructure.Value = true;
				who.currentLocation.playSoundAt("doorClose", tileLocation);
				if (Game1.activeClickableMenu == null)
					Game1.activeClickableMenu = new ElevatorMenu();

				//Game1.warpFarmer(indoors.Value.uniqueName.Value, ((NetList<Warp, NetRef<Warp>>)indoors.Value.warps)[0].X, ((NetList<Warp, NetRef<Warp>>)indoors.Value.warps)[0].Y - 1, Game1.player.FacingDirection, true);
				return true;
			}
			return true;
		}

		public override void draw(SpriteBatch b)
		{
			if (!isMoving)
			{
				if (daysOfConstructionLeft.Value > 0)
				{
					drawInConstruction(b);
				}
				else
				{
					drawShadow(b, -1, -1);
					//b.Draw(base.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(tileX.Value + base.animalDoor.X), (float)(tileY.Value + base.animalDoor.Y)) * 64f), new Rectangle(16, 112, 16, 16), Color.White * (float)base.alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-06f);
					//b.Draw(base.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)((tileX.Value + base.animalDoor.X) * 64), (float)((tileY.Value + base.animalDoor.Y) * 64 + (int)yPositionOfAnimalDoor))), new Rectangle(0, 112, 16, 16), Color.White * (float)base.alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(((int)base.tileY + (int)base.tilesHigh) * 64) / 10000f - 1E-07f);

					b.Draw(base.texture.Value, 
						Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(tileX.Value * 64), 
						(float)(tileY.Value * 64 + base.tilesHigh.Value * 64))), 
						new Rectangle(0, 0, 96, 112), 
						base.color.Value * alpha.Value, 0f, 
						new Vector2(0f, 112f), 
						4f, SpriteEffects.None, (float)((tileY.Value + tilesHigh.Value) * 64) / 10000f);

					/*if (base.daysUntilUpgrade.Value > 0)
					{
						b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, getUpgradeSignLocation()), new Rectangle(367, 309, 16, 15), Color.White * (float)base.alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(((int)base.tileY + (int)base.tilesHigh) * 64) / 10000f + 0.0001f);
					}*/
				}
			}
		}
	}
}
