/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/BattleRoyalley
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleRoyale
{
	class OverlayUI : IClickableMenu
	{
		public new const int width = 220;

		public new const int height = 165;

		private readonly Rectangle textureSegment = new Rectangle(384, 373, 18, 18);

		public int AlivePlayers { get; set; }
		private readonly int initalAlivePlayers;

		public OverlayUI(int alivePlayers) : base(Game1.viewport.Width - width + 32, 8, width, height, false)
		{
			//base.width = width;
			//base.height = height;
			AlivePlayers = alivePlayers;
			initalAlivePlayers = alivePlayers;
		}

		public override void draw(SpriteBatch spriteBatch)
		{
			UpdatePosition();
			drawTextureBox(spriteBatch, Game1.mouseCursors, textureSegment,
				base.xPositionOnScreen, base.yPositionOnScreen, base.width, base.height, Color.White, 4f, false);

			//Draw time
			{
				TimeSpan timeSpent = DateTime.Now - ModEntry.BRGame.TimeWhenGameStarted;
                
                string text = new StringBuilder().AppendFormat("{0:D2}:{1:D2}", timeSpent.Minutes, timeSpent.Seconds).ToString();

				Vector2 textDimensions = Game1.dialogueFont.MeasureString(text);
				spriteBatch.DrawString(Game1.dialogueFont, text, new Vector2(base.xPositionOnScreen + width / 2 - textDimensions.X / 2, base.yPositionOnScreen + 35), Color.Black);
			}

			//Player count
			{
                string aliveText = new StringBuilder().AppendFormat("<{0}/{1}", AlivePlayers, initalAlivePlayers).ToString();
                
                Vector2 textDimensions = Game1.dialogueFont.MeasureString(aliveText);
				spriteBatch.DrawString(Game1.dialogueFont, aliveText, new Vector2(base.xPositionOnScreen + width / 2 - textDimensions.X / 2, base.yPositionOnScreen + 85), Color.Black);
			}
		}

		private void UpdatePosition()
		{
			base.xPositionOnScreen = Game1.viewport.Width - width;
			//base.yPositionOnScreen = 8f;
			//Utility.makeSafe(ref Position, 300, 284)
		}
	}
}
