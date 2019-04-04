
using CustomEmojis.Framework.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace CustomEmojis.Framework.Types {

	public class MessageEmoji {

		public int Index { get; set; }
		public int HorizontalPosition { get; set; }
		public int VerticalPosition { get; set; }

		private int MessageHash { get; set; }
		private bool ShouldCacheTexture { get; set; }

		public Texture2D Texture { get => GetEmojiTexture(Index, MessageHash); }

		private Texture2D GetEmojiTexture() {
			throw new NotImplementedException();
		}

		public MessageEmoji(int emojiIndex, int horizontalPosition, int verticalPosition, int messageHash, bool cacheEmojiTexture = false) {

			Index = emojiIndex;
			HorizontalPosition = horizontalPosition;
			VerticalPosition = verticalPosition;
			ShouldCacheTexture = cacheEmojiTexture;
			MessageHash = messageHash;

		}

		private Texture2D GetEmojiTexture(int index, int messageHash) {

			ObjectCache cache = MemoryCache.Default;

			if(!(cache[$"{messageHash}_emoji[{index}]"] is Texture2D cachedEmojiTexture)) {

				Rectangle sourceRectangle = GetSourceRectangle(Index, EmojiMenu.EMOJI_SIZE, ChatBox.emojiTexture.Width);

				// Get emoji texture color data
				Color[] originalImageData = new Color[ChatBox.emojiTexture.Width * ChatBox.emojiTexture.Height];
				ChatBox.emojiTexture.GetData(originalImageData);

				// Crop interested emoji
				Color[] croppedImageData = ImageUtilities.GetImageData(originalImageData, ChatBox.emojiTexture.Width, sourceRectangle);

				// Create texture from cropped emoji
				cachedEmojiTexture = new Texture2D(Game1.graphics.GraphicsDevice, sourceRectangle.Width, sourceRectangle.Height);
				cachedEmojiTexture.SetData(croppedImageData);

				if(ShouldCacheTexture) {

					// Cache texture for later use
					cache[$"{messageHash}_emoji[{index}]"] = cachedEmojiTexture;

				}
#if DEBUG
				ModEntry.ModLogger.LogToMonitor = false;
				ModEntry.ModLogger.Log("Cached values:");
				foreach(KeyValuePair<string, object> entry in cache.ToList()) {
					ModEntry.ModLogger.Log($"Key: {entry.Key}", $"Value: {entry.Value}");
				}
				ModEntry.ModLogger.LogToMonitor = true;
#endif
			}

			return cachedEmojiTexture;
		}

		public Rectangle GetSourceRectangle(int index, int emojiSize, int textureWidth) {
			 return new Rectangle(index * emojiSize % textureWidth, index * emojiSize / textureWidth * emojiSize, emojiSize, emojiSize);
		}

		public void DrawEmoji(SpriteBatch b, Vector2 position, float alpha) {
			b.Draw(Texture, position, new Rectangle?(new Rectangle(0, 0, 9, 9)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
		}
	}

}
