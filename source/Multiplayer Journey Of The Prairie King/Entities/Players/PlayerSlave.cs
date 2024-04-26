/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/scayze/multiprairie
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MultiPlayerPrairie;
using StardewValley;
using static MultiPlayerPrairie.GameMultiplayerPrairieKing;

namespace MultiplayerPrairieKing.Entities
{
    public class PlayerSlave : BasePlayer
    {

        public PlayerSlave(GameMultiplayerPrairieKing game) : base(game)
        {
			
        }

        public override void Tick(GameTime time)
		{
			base.Tick(time);

			motionAnimationTimer += time.ElapsedGameTime.Milliseconds;
			motionAnimationTimer %= 400;

			if (movementDirections.Count > 0)
			{
				float speed = GetMovementSpeed(3f, movementDirections.Count);
				for (int j = 0; j < movementDirections.Count; j++)
				{
					Vector2 newPlayerPosition = position;
					switch (movementDirections[j])
					{
						case 0:
							newPlayerPosition.Y -= speed;
							break;
						case 3:
							newPlayerPosition.X -= speed;
							break;
						case 2:
							newPlayerPosition.Y += speed;
							break;
						case 1:
							newPlayerPosition.X += speed;
							break;
					}
				}
				boundingBox.X = (int)position.X + TileSize / 4;
				boundingBox.Y = (int)position.Y + TileSize / 4;
				boundingBox.Width = TileSize / 2;
				boundingBox.Height = TileSize / 2;
			}
		}
	}
}
