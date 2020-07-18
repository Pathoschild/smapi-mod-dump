using Microsoft.Xna.Framework.Graphics;
using xnaRect = Microsoft.Xna.Framework.Rectangle;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System.Collections.Generic;
using xTile.Tiles;

namespace ExtendedEmotes
{
	internal class TileSheetEditor : IAssetEditor
	{
		/*********
        ** Constants
        *********/
		public static readonly int tileSize = 16;
		public static readonly int tilesPerEmote = 4;
		public static readonly xnaRect emoteTextureSize = new xnaRect(0, 0, tilesPerEmote * tileSize, tileSize);


		/*********
        ** Accessors
        *********/
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;


		/*********
        ** Public methods
        *********/
		/// <summary>Get whether this instance can edit the given asset.</summary>
		/// <typeparam name="_T">The asset Type.</typeparam>
		/// <param name="asset">Basic metadata about the asset being loaded.</param>
		/// <returns>true for asset TileSheets\Emotes, false otherwise</returns>
		public bool CanEdit<_T> (IAssetInfo asset)
		{
			return asset.AssetNameEquals($"TileSheets\\Emotes");
		}

		/// <summary>Extend the TileSheets\Emotes spritesheet downwards to add new images onto the bottom.</summary>
		/// <typeparam name="_T">The asset Type</typeparam>
		/// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it</param>
		public void Edit<_T> (IAssetData asset)
		{
			var editor = asset.AsImage();

			// Get list/dictionary of default emote image assets
			var vanillaEmotes = new Dictionary<string, int>
			{
				["emptyCan"] = 1,
				["questionMark"] = 2,
				["angry"] = 3,
				["exclamation"] = 4,
				["heart"] = 5,
				["sleep"] = 6,
				["sad"] = 7,
				["happy"] = 8,
				["x"] = 9,
				["pause"] = 10,
				["noTackle"] = 11,
				["junimoNote"] = 12,
				["videoGame"] = 13,
				["musicNote"] = 14,
				["blush"] = 15
			};
			
			// Patch vanilla emote textures (users can therefore replace any they like)
			foreach (KeyValuePair<string, int> emote in vanillaEmotes)
			{
				// Does assets/vanilla contain a texture for this emote?
				if (true)
				{
					// The emotes list(s) need to be case sensitive and match the file names.
					Texture2D emoteTexture = Helper.Content.Load<Texture2D>($"assets\\vanilla\\{emote.Key}.png", ContentSource.ModFolder);
					if (emoteTexture == null)
					{ 
						continue; // Texture could not be loaded
					}
					if (emoteTexture.Bounds != emoteTextureSize)
					{
						Monitor.Log($"Texture for {emote.Key} is the incorrect size. Patch not applied.", LogLevel.Warn);
						continue; // Not the right size for emote tiles
					}
					xnaRect target = new xnaRect(0, emote.Value * tileSize, tilesPerEmote * tileSize, tileSize);
					editor.PatchImage(emoteTexture, targetArea: target);
					Monitor.Log($"Sucessfully loaded vanilla texture for {emote.Key} emote.", LogLevel.Info);
				}
			}

			var extendedEmotes = new Dictionary<string, int?>
			{
				["rainbow"] = null,
				["blank"] = null
			};

			int vanillaTilesheetHeight = editor.Data.Height;
			int vanillaEmotesNum = vanillaTilesheetHeight / tileSize - 1;
			int extendedEmotesNum = extendedEmotes.Count;
			// Expensive operation! Only do this once.
			editor.ExtendImage(minWidth: tilesPerEmote * tileSize, minHeight: vanillaTilesheetHeight + (extendedEmotesNum * tileSize));

			// Patch extended emote textures (from assets\extended\)
			int count = vanillaEmotesNum; 
			foreach (string emote in new List<string>(extendedEmotes.Keys))
			{
				// Does assets/extended contain a texture for this emote?
				if (true)
				{
					// The emotes list(s) need to be case sensitive and match the file names.
					Texture2D emoteTexture = Helper.Content.Load<Texture2D>($"assets\\extended\\{emote}.png", ContentSource.ModFolder);
					if (emoteTexture == null)
					{
						continue; // Texture could not be loaded
					}
					if (emoteTexture.Bounds != emoteTextureSize)
					{
						Monitor.Log($"Texture for {emote} is the incorrect size. Patch not applied.", LogLevel.Warn);
						continue; // Not the right size for emote tiles
					}
					count++;
					extendedEmotes[emote] = count;
					xnaRect target = new xnaRect(0, (int)extendedEmotes[emote] * tileSize, tilesPerEmote * tileSize, tileSize);
					editor.PatchImage(emoteTexture, targetArea: target);
					Monitor.Log($"Sucessfully loaded vanilla texture for {emote} emote.", LogLevel.Info);
				}
			}


			var emoteAliases = new Dictionary<string, string[]>
			{
				["emptyCan"] = new string[] { "nowater" },
				["questionMark"] = new string[] { "question" },
				["angry"] = new string[] { },
				["exclamation"] = new string[] { "exclamationmark" },
				["heart"] = new string[] { },
				["sleep"] = new string[] { },
				["sad"] = new string[] { },
				["happy"] = new string[] { },
				["x"] = new string[] { },
				["pause"] = new string[] { "dotdotdot" },
				["noTackle"] = new string[] { },
				["junimoNote"] = new string[] { "junimoscroll" },
				["videoGame"] = new string[] { "game" },
				["musicNote"] = new string[] { "note" },
				["blush"] = new string[] { }
			};

			var dynamicEmotes = new string[]
			{
				"sick",
				"surprised",
				"no",
				"uh",
				"yes",
				"laugh",
				"hi",
				"music",
				"jar",
				"taunt"
			};
		}
	}
}