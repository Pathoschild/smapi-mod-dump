/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/cropbeasts
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.IO;
using System.Linq;

namespace Cropbeasts
{
	public class SandblastDebuff : Buff
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		protected static ModConfig Config => ModConfig.Instance;

		public static readonly int MaxDuration = 60000;
		public static readonly int AddIncrement = 1000;
		public static readonly int HealIncrement = 250;

		public static readonly int BatchIncrement = 250;
		public static readonly int MaxBatches = 120;
		public static readonly int SpritesPerBatch = 5;

		private Texture2D texture;
		private static readonly int TextureSize = 128;
		private static readonly int TextureCount = 4;

		private int randomSeed;

		protected SandblastDebuff (int duration)
		: base (Helper.Translation.Get ("SandblastDebuff.name") +
			Environment.NewLine + Helper.Translation.Get ("SandblastDebuff.description"),
			duration, null, 36)
		{
			texture = Helper.Content.Load<Texture2D>
				(Path.Combine ("assets", "sandblast.png"));
			randomSeed = Game1.random.Next ();
		}

		protected static SandblastDebuff Current =>
			Game1.buffsDisplay.otherBuffs.FirstOrDefault
				((buff) => buff is SandblastDebuff) as SandblastDebuff;

		public static int Duration
		{
			get
			{
				return Current?.millisecondsDuration ?? 0;
			}
			set
			{
				SandblastDebuff current = Current;
				value = Math.Max (0, Math.Min (MaxDuration, value));
				if (current != null)
					current.millisecondsDuration = value;
				else if (value > 0)
					Game1.buffsDisplay.addOtherBuff (new SandblastDebuff (value));
			}
		}

		public static void Draw (SpriteBatch b)
		{
			Current?.draw (b);
		}

		protected virtual void draw (SpriteBatch b)
		{
			int batches = millisecondsDuration / BatchIncrement;
			float lastOpacity = 1f;

			if (batches <= 0)
				return;
			else if (batches > MaxBatches)
				batches = MaxBatches;
			else
				lastOpacity = millisecondsDuration % BatchIncrement /
					(float) BatchIncrement;

			Random rng = new Random (randomSeed);
			for (int i = 0; i < SpritesPerBatch * batches; ++i)
			{
				Vector2 position = new Vector2
					(rng.Next (0, Game1.viewport.Width),
					rng.Next (0, Game1.viewport.Height));
				Rectangle sourceRect = new Rectangle (rng.Next (0, TextureCount)
					* TextureSize, 0, TextureSize, TextureSize);
				float opacity = (i >= SpritesPerBatch * (batches - 1))
					? lastOpacity : 1f;
				float rotation = (float) (2.0 * Math.PI * rng.NextDouble ());
				b.Draw (texture, position, sourceRect, Color.White * opacity,
					rotation, new Vector2 (TextureSize / 2f, TextureSize / 2f), 2f,
					SpriteEffects.None, 1f);
			}
		}
	}
}
