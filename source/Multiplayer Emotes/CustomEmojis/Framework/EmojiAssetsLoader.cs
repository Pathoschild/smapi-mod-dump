
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using CustomEmojis.Framework.Constants;
using CustomEmojis.Framework.Events;
using CustomEmojis.Framework.Extensions;
using CustomEmojis.Framework.Network;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace CustomEmojis.Framework {

	public class EmojiAssetsLoader : IAssetLoader {

		public Texture2D VanillaTexture { get; set; }
		public Texture2D CustomTexture { get; set; }

		public Texture2D CurrentTexture { get; set; }
		public Dictionary<long, List<TextureData>> SynchronizedPlayerTextureData = new Dictionary<long, List<TextureData>>();

		public List<TextureData> LoadedTextureData { get; set; } = new List<TextureData>();

		public int EmojisSize { get; set; }
		public bool CustomTextureAdded { get; set; }
		public int NumberCustomEmojisAdded => LoadedTextureData.Count();
		public int TotalNumberEmojis { get; private set; }
		public bool SaveGeneratedTexture { get; set; }
		public bool SaveCustomEmojiTexture { get; set; }
		public bool ForceTextureGeneration { get; set; }
		public bool ForceGeneratedTextureSave { get; set; }

		private readonly IModHelper modHelper;
		private readonly ModData modData;

		private readonly string[] imageExtensions;

		private readonly bool allowClientEmojis;

		public EmojiAssetsLoader(IModHelper modHelper, ModData modData, ModConfig modConfig, int emojisSize, bool forceTextureGeneration = false, bool forceGeneratedTextureSave = false) {

			this.modHelper = modHelper;
			this.modData = modData;

			VanillaTexture = Sprites.VanillaEmojis.Texture;

			this.CustomTextureAdded = false;
			this.EmojisSize = emojisSize;
			this.imageExtensions = modConfig.ImageExtensions;

			this.ForceTextureGeneration = forceTextureGeneration;
			this.ForceGeneratedTextureSave = forceGeneratedTextureSave;

			allowClientEmojis = modConfig.AllowClientEmojis;
			SubscribeEvents();

		}

		/// <summary>Get whether this instance can load the initial version of the given asset.</summary>
		/// <param name="asset">Basic metadata about the asset being loaded.</param>
		public bool CanLoad<T>(IAssetInfo asset) {
			return asset.AssetNameEquals(Sprites.VanillaEmojis.AssetName);
		}

		/// <summary>Load a matched asset.</summary>
		/// <param name="asset">Basic metadata about the asset being loaded.</param>
		public T Load<T>(IAssetInfo asset) {

#if DEBUG
			ModEntry.ModLogger.Log($"Generate Texture? {modData.ShouldGenerateTexture()}");
			ModEntry.ModLogger.Log($"Save Checksum Data? {modData.ShouldSaveData()}");

			Stopwatch swTotal = new Stopwatch();
			ModEntry.ModMonitor.Log($"[EmojiAssetsLoader TextureCreated/Loaded] Timer Started!");
			ModEntry.ModLogger.Log("[Start Asset Load] Timer Started!");
			swTotal.Start();
#endif
			string outputFolderPath = Path.Combine(modHelper.DirectoryPath, ModPaths.Assets.Folder);
			Directory.CreateDirectory(outputFolderPath);

			string inputFolderPath = Path.Combine(modHelper.DirectoryPath, ModPaths.Assets.InputFolder);

			// If file changes are detected, make again the texture
			if(!Directory.Exists(inputFolderPath)) {
				Directory.CreateDirectory(inputFolderPath);
#if DEBUG
				ModEntry.ModLogger.Log("[In Assed Load] No directory", $"Total time Elapsed: {swTotal.Elapsed}");
#endif
			} else if(ForceTextureGeneration || modData.ShouldGenerateTexture() || modData.Checksum()) {
#if DEBUG
				ModEntry.ModLogger.Log("[In Assed Load] [Generate Texture] Timer Started!", $"Total Time Elapsed: {swTotal.Elapsed}");

				Stopwatch sw = new Stopwatch();
				sw.Start();

				ModEntry.ModLogger.Log("[In Assed Load] [Merging Images]");
#endif
				LoadedTextureData = GetTextureDataList(modData.FilesChecksums.Values);
				CustomTexture = MergeTextures(VanillaTexture, LoadedTextureData);
#if DEBUG
				ModEntry.ModLogger.Log("[In Assed Load] [After Merging Images]", $"Time Elapsed: {sw.Elapsed}");

				ModEntry.ModLogger.Log("[In Assed Load] [Saving Merged Image]");
				sw.Restart();
#endif
				if(ForceGeneratedTextureSave || CustomTexture != null) {
					SaveTextureToPng(this.CustomTexture, Path.Combine(modHelper.DirectoryPath, Sprites.CustomEmojis.AssetName));
				}
#if DEBUG
				sw.Stop();
				ModEntry.ModLogger.Log("[In Assed Load] [After Saving Merged Image]", $"Time Elapsed: {sw.Elapsed}");
#endif
			} else if(File.Exists(Path.Combine(modHelper.DirectoryPath, Sprites.CustomEmojis.AssetName))) {
#if DEBUG
				ModEntry.ModLogger.Log("[In Assed Load] [Load Texture]");

				Stopwatch sw = new Stopwatch();
				sw.Start();
#endif
				LoadedTextureData = GetTextureDataList(modData.FilesChecksums.Values);
				CustomTexture = modHelper.Content.Load<Texture2D>(Sprites.CustomEmojis.AssetName, ContentSource.ModFolder);
#if DEBUG
				sw.Stop();
				ModEntry.ModLogger.Log("[In Assed Load] [After Load Texture]", $"Time Elapsed: {sw.Elapsed}");
#endif
			}

			if(CustomTexture != null) {
				this.CustomTextureAdded = true;
				this.CurrentTexture = this.CustomTexture;
			} else {
				this.CustomTextureAdded = false;
				this.CurrentTexture = this.VanillaTexture;
			}
#if DEBUG
			swTotal.Stop();
			ModEntry.ModLogger.Log($"[End Asset Load]", $"Total Time Elapsed: {swTotal.Elapsed}");
#endif
			// (T)(object) is a trick to cast anything to T if we know it's compatible
			return (T)(object)this.CurrentTexture;
		}

		private void SubscribeEvents() {
			MultiplayerExtension.OnReceiveEmojiTextureRequest += MultiplayerExtension_OnReceiveEmojiTextureRequest;
			MultiplayerExtension.OnReceiveEmojiTextureData += MultiplayerExtension_OnReceiveEmojiTextureData;
			MultiplayerExtension.OnReceiveEmojiTexture += MultiplayerExtension_OnReceiveEmojiTexture;
			MultiplayerExtension.OnPlayerDisconnected += MultiplayerExtension_OnPlayerDisconnected;
		}

		private void UnsubscribeEvents() {
			MultiplayerExtension.OnReceiveEmojiTextureRequest -= MultiplayerExtension_OnReceiveEmojiTextureRequest;
			MultiplayerExtension.OnReceiveEmojiTextureData -= MultiplayerExtension_OnReceiveEmojiTextureData;
			MultiplayerExtension.OnReceiveEmojiTexture -= MultiplayerExtension_OnReceiveEmojiTexture;
			MultiplayerExtension.OnPlayerDisconnected -= MultiplayerExtension_OnPlayerDisconnected;
		}

		private void MultiplayerExtension_OnReceiveEmojiTextureRequest(object sender, ReceivedEmojiTextureRequestEventArgs e) {
			SyncTextureData(e.SourceFarmer);
		}

		private void MultiplayerExtension_OnReceiveEmojiTextureData(object sender, ReceivedEmojiTextureDataEventArgs e) {

			// Get all synched data
			IEnumerable<TextureData> synchedData = SynchronizedPlayerTextureData.Values.SelectMany(x => x);

			// Get only the different textures
			e.TextureDataList = e.TextureDataList.Except(synchedData).ToList();

			// Add the different textures obtained
			SynchronizedPlayerTextureData[e.SourceFarmer.UniqueMultiplayerID] = e.TextureDataList;

			CurrentTexture = MergeTextures(VanillaTexture, synchedData);
			UpdateTotalEmojis(TotalNumberEmojis + e.TextureDataList.Count);

			// Replace with the new texture
			modHelper.Reflection.GetField<Texture2D>(Game1.chatBox.emojiMenu, "emojiTexture").SetValue(CurrentTexture);
			ChatBox.emojiTexture = CurrentTexture;

			// Update client textures
			Multiplayer multiplayer = modHelper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
			multiplayer.BroadcastEmojiTexture(CurrentTexture, TotalNumberEmojis);

		}

		private void MultiplayerExtension_OnReceiveEmojiTexture(object sender, ReceivedEmojiTextureEventArgs e) {

			CurrentTexture = e.EmojiTexture;
			UpdateTotalEmojis(e.NumberEmojis);

			modHelper.Reflection.GetField<Texture2D>(Game1.chatBox.emojiMenu, "emojiTexture").SetValue(CurrentTexture);
			ChatBox.emojiTexture = CurrentTexture;

		}

		private void MultiplayerExtension_OnPlayerDisconnected(object sender, PlayerDisconnectedEventArgs e) {
#if DEBUG
			ModEntry.ModLogger.LogTrace();
#endif
			if(SynchronizedPlayerTextureData.ContainsKey(e.Player.UniqueMultiplayerID)) {
				UpdateTotalEmojis(TotalNumberEmojis - SynchronizedPlayerTextureData[e.Player.UniqueMultiplayerID].Count);
				SynchronizedPlayerTextureData.Remove(e.Player.UniqueMultiplayerID);
			}

			CurrentTexture = MergeTextures(VanillaTexture, SynchronizedPlayerTextureData.Values);

			// Replace with the new texture
			modHelper.Reflection.GetField<Texture2D>(Game1.chatBox.emojiMenu, "emojiTexture").SetValue(CurrentTexture);
			ChatBox.emojiTexture = CurrentTexture;
			//Game1.chatBox.emojiMenu.receiveScrollWheelAction(1);
			//Game1.chatBox.emojiMenu.receiveScrollWheelAction(-1);

			// Update client textures
			Multiplayer multiplayer = modHelper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
			multiplayer.BroadcastEmojiTexture(CurrentTexture, TotalNumberEmojis);

		}

		public void SyncTextureData(Farmer farmer) {

			if(Context.IsMultiplayer) {

				long multiplayerID = Game1.player.UniqueMultiplayerID;

				if(!SynchronizedPlayerTextureData.ContainsKey(multiplayerID)) {
					SynchronizedPlayerTextureData[multiplayerID] = LoadedTextureData;
				}

				Multiplayer multiplayer = modHelper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
				if(Game1.IsMasterGame) {
					if(this.allowClientEmojis) {
						multiplayer.RequestEmojiTexture(farmer);
					} else {
						multiplayer.BroadcastEmojiTexture(CurrentTexture, TotalNumberEmojis);
					}
				} else {
					multiplayer.SendEmojisTextureDataList(farmer, LoadedTextureData);
				}

			}

		}

		public void UpdateTotalEmojis() {
			if(TotalNumberEmojis != EmojiMenu.totalEmojis) {
				//int requiredRows = (int)Math.Ceiling((double)emojiAssetsLoader.NumberEmojisAdded / 14);
				//int emojiPerRow = (emojiAssetsLoader.VanillaEmojisTexture.Width / emojiAssetsLoader.EmojisSize);
				//int excessToRemove = (emojiAssetsLoader.NumberEmojisAdded - emojiPerRow * requiredRows);
				//EmojiMenu.totalEmojis += excessToRemove;
				EmojiMenu.totalEmojis += NumberCustomEmojisAdded - (VanillaTexture.Width / EmojisSize) * (int)Math.Ceiling((double)NumberCustomEmojisAdded / 14);
				TotalNumberEmojis = EmojiMenu.totalEmojis;
			}
		}

		public void UpdateTotalEmojis(int newAmmount) {
			TotalNumberEmojis = newAmmount;
			EmojiMenu.totalEmojis = TotalNumberEmojis;
		}
		private void SaveTextureToPng(Texture2D texture, string path) {
			using(FileStream stream = File.Create(path)) {
				texture.SaveAsPng(stream, texture.Width, texture.Height);
			}
		}

		private List<Image> GetImageList(IEnumerable<string> imageFilePaths) {
			return imageFilePaths.Select(x => (Image)ResizeImage(Image.FromFile(x), EmojisSize, EmojisSize)).ToList();
		}

		private List<TextureData> GetTextureDataList(IEnumerable<string> imageFilePaths) {
			return GetTextureDataList(GetImageList(imageFilePaths));
		}

		private List<TextureData> GetTextureDataList(IEnumerable<Image> imageFilePaths) {

			List<TextureData> textureDataList = new List<TextureData>();
			if(imageFilePaths.Count() > 0) {
				foreach(Image image in imageFilePaths) {
					using(MemoryStream stream = new MemoryStream()) {
						image.Save(stream, ImageFormat.Png);
						textureDataList.Add(new TextureData(stream));
					}
				}
			}

			return textureDataList;
		}

		internal void ReloadAsset() {
			ModEntry.ModMonitor.Log($"Reloading emoji assets...");

			bool cacheInvalidated = modHelper.Content.InvalidateCache("LooseSprites/emojis");
			UpdateTotalEmojis();
			ModEntry.ModMonitor.Log($"CacheInvalidated: {cacheInvalidated}");
			//modHelper.Reflection.GetField<Texture2D>(Game1.chatBox.emojiMenu, "emojiTexture").SetValue(modHelper.Content.Load<Texture2D>($"{Assets.OutputFolder}/{Assets.OutputFile}"));
			//ChatBox.emojiTexture = modHelper.Content.Load<Texture2D>($"{Assets.OutputFolder}/{Assets.OutputFile}");
		}

		public Texture2D MergeTextures(Texture2D vanillaTexture, IEnumerable<List<TextureData>> textureDataListEnumerable) {
			return MergeTextures(vanillaTexture, textureDataListEnumerable.SelectMany(x => x));
		}

		public Texture2D MergeTextures(Texture2D vanillaTexture, IEnumerable<TextureData> textureDataEnumerable) {
			return MergeTextures(vanillaTexture, textureDataEnumerable.Select(x => x.GetTexture()));
		}

		public Texture2D MergeTextures(Texture2D vanillaTexture, List<TextureData> textureList) {
			return MergeTextures(vanillaTexture, textureList.Select(x => x.GetTexture()));
		}

		public Texture2D MergeTextures(Texture2D vanillaTexture, IEnumerable<Texture2D> textureEnumerable) {
			return MergeTextures(vanillaTexture, textureEnumerable.Select(x => TextureToImage(x)));
		}

		private Texture2D MergeTextures(Texture2D vanillaTexture, List<Texture2D> textureList) {
			return MergeTextures(vanillaTexture, textureList.Select(x => TextureToImage(x)));
		}

		private Texture2D MergeTextures(Texture2D vanillaTexture, IEnumerable<Image> imagesEnumerable) {
			return MergeTextures(TextureToImage(vanillaTexture), imagesEnumerable);
		}

		private Texture2D MergeTextures(Image vanillaTexture, IEnumerable<Image> images) {
			return MergeTextures(vanillaTexture, images.ToList());
		}

		private Texture2D MergeTextures(Texture2D vanillaTexture, List<Image> images) {
			return MergeTextures(TextureToImage(vanillaTexture), images);
		}

		private Texture2D MergeTextures(Image vanillaTexture, List<Image> images) {
			images.Insert(0, vanillaTexture);
			return MergeTextures(images);
		}

		private Texture2D MergeTextures(List<Image> images) {
			Bitmap outputImage = new Bitmap(images[0].Width, images[0].Height + ((int)Math.Ceiling((double)NumberCustomEmojisAdded / 14) * EmojisSize), PixelFormat.Format32bppArgb);
			using(Graphics graphics = Graphics.FromImage(outputImage)) {
				graphics.DrawImage(images[0], new Rectangle(new Point(), images[0].Size), new Rectangle(new Point(), images[0].Size), GraphicsUnit.Pixel);
			}

			int xPosition = 0;
			int yPosition = 0;
			for(int i = 1; i < images.Count(); i++) {
				xPosition = ((i - 1) % 14) * EmojisSize;
				if((i - 1) % 14 == 0) {
					yPosition = (((i - 1) / 14) * EmojisSize) + images[0].Height;
				}
				using(Graphics graphics = Graphics.FromImage(outputImage)) {
					graphics.DrawImage(images[i], new Rectangle(new Point(xPosition, yPosition), images[i].Size), new Rectangle(new Point(), images[i].Size), GraphicsUnit.Pixel);
				}
			}

			using(MemoryStream memoryStream = new MemoryStream()) {
				outputImage.Save(memoryStream, ImageFormat.Png);
				return Texture2D.FromStream(Game1.graphics.GraphicsDevice, memoryStream);
			}
		}
		/*
		private Texture2D MergeTextures(HashSet<Image> images) {

			Bitmap outputImage = new Bitmap(images.First().Width, images.First().Height + ((int)Math.Ceiling((double)NumberCustomEmojisAdded / 14) * EmojisSize), PixelFormat.Format32bppArgb);
			using(Graphics graphics = Graphics.FromImage(outputImage)) {
				graphics.DrawImage(images.First(), new Rectangle(new Point(), images.First().Size), new Rectangle(new Point(), images.First().Size), GraphicsUnit.Pixel);
			}

			int xPosition = 0;
			int yPosition = 0;
			int i = 1;
			foreach(Image image in images.Skip(1)) {				
				xPosition = ((i - 1) % 14) * EmojisSize;
				if((i - 1) % 14 == 0) {
					yPosition = (((i - 1) / 14) * EmojisSize) + images.First().Height;
				}
				using(Graphics graphics = Graphics.FromImage(outputImage)) {
					graphics.DrawImage(image, new Rectangle(new Point(xPosition, yPosition), image.Size), new Rectangle(new Point(), image.Size), GraphicsUnit.Pixel);
				}
				i++;
			}

			using(MemoryStream memoryStream = new MemoryStream()) {
				outputImage.Save(memoryStream, ImageFormat.Png);
				return Texture2D.FromStream(Game1.graphics.GraphicsDevice, memoryStream);
			}
		}
		*/

		private List<Image> GetImagesList(IEnumerable<string> filePathsList) {
			List<Image> images = new List<Image>();
			foreach(string filePath in filePathsList) {
				images.Add(ResizeImage(Image.FromFile(filePath), EmojisSize, EmojisSize));
			}
			return images;
		}

		/// <summary>Resize the image to the specified width and height.</summary>
		/// <param name="image">The image to resize.</param>
		/// <param name="width">The width to resize to.</param>
		/// <param name="height">The height to resize to.</param>
		/// <returns>The resized image.</returns>
		public static Bitmap ResizeImage(Image image, int width, int height) {

			var destRect = new Rectangle(0, 0, width, height);
			var destImage = new Bitmap(width, height);

			float imageHorizontalResolution = 96;
			float imageVerticalResolution = 96;

			// Sometimes files dont have embedded DPI data, check that isnt the case
			if(image.HorizontalResolution > 0 && image.VerticalResolution > 0) {
				imageHorizontalResolution = image.HorizontalResolution;
				imageVerticalResolution = image.VerticalResolution;
			}

			// Maintains DPI regardless of physical size -- may increase quality when reducing image dimensions or when printing
			destImage.SetResolution(imageHorizontalResolution, imageVerticalResolution);

			// Composite controls how pixels are blended with the background -- might not be needed since we're only drawing one thing.
			using(var graphics = Graphics.FromImage(destImage)) {

				// Determine whether pixels from a source image overwrite or are combined with background pixels.SourceCopy specifies that when a color is rendered, it overwrites the background color.
				graphics.CompositingMode = CompositingMode.SourceCopy;

				// Determines the rendering quality level of layered images.
				graphics.CompositingQuality = CompositingQuality.HighQuality;

				// Determines how intermediate values between two endpoints are calculated
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

				// Specify whether lines, curves, and the edges of filled areas use smoothing(also called antialiasing) -- probably only works on vectors
				graphics.SmoothingMode = SmoothingMode.HighQuality;

				// Rendering quality when drawing the new image
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				using(var wrapMode = new ImageAttributes()) {
					// Prevent ghosting around the image borders --naïve resizing will sample transparent pixels beyond the image boundaries, but by mirroring the image we can get a better sample(this setting is very noticeable)
					wrapMode.SetWrapMode(WrapMode.TileFlipXY);
					graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
				}


			}

			return destImage;
		}

		public static Image TextureToImage(Texture2D texture) {
			Image image;
			using(MemoryStream memoryStream = new MemoryStream()) {
				texture.SaveAsPng(memoryStream, texture.Width, texture.Height);
				memoryStream.Seek(0, SeekOrigin.Begin);
				image = Image.FromStream(memoryStream);
			}
			return image;
		}

		public byte[] ToByteArray(Texture2D texture) {
			return ToByteArray(TextureToImage(texture));
		}

		public byte[] ToByteArray(Image image) {
			using(MemoryStream ms = new MemoryStream()) {
				image.Save(ms, image.RawFormat);
				return ms.ToArray();
			}
		}
	}

}
