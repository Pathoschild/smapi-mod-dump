/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AHilyard/UpgradeablePan
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;

namespace UpgradablePan
{
	public class PanHat : Hat
	{
		public static Texture2D panHatTexture;

		public int UpgradeLevel { get; set; } = 0;

		public override string DisplayName
		{
			get
			{
				Pan tmp = new Pan();
				tmp.BaseName = "Pan";
				tmp.UpgradeLevel = UpgradeLevel;
				return tmp.Name;
			}
		}

		public PanHat(Pan pan) : base(71)
		{
			UpgradeLevel = pan.UpgradeLevel;
		}

		public override string getDescription()
		{
			// To avoid having to worry about localization, we won't have much of a description.
			return "...";
		}

		public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
		{
			float originalScale = scaleSize;
			scaleSize *= 0.75f;
			spriteBatch.Draw(panHatTexture, location + new Vector2(32f, 32f), new Rectangle((int)(UpgradeLevel - 2) * 20 % FarmerRenderer.hatsTexture.Width, (int)(UpgradeLevel - 2) * 20 / FarmerRenderer.hatsTexture.Width * 20 * 4, 20, 20), isPrismatic ? (Utility.GetPrismaticColor() * transparency) : (color * transparency), 0f, new Vector2(10f, 10f), 4f * scaleSize, SpriteEffects.None, layerDepth);
		}
	}
}