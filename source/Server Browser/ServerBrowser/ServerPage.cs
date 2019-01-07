using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerBrowser
{
	class ServerPage : IClickableMenu
	{
		public string FarmName { get; set; }
		public string ServerDescription { get; set; }

		public string PlayersOnline { get; set; }
		public string PlayerSlots { get; set; }
		public string CabinCountText { get; set; }

		public bool ShowPasswordLockIcon { get; set; } = false;//TODO

		private ListBox ServerModsBox;
		private ListBox RequiredModsBox;
		private readonly bool missingRequiredMod = false;

		private ClickableTextureComponent connectButton;

		private readonly Action callback;

		public ServerPage(int x, int y, int width, int height, string requiredMods, string serverMods, Action callback, Action onExit) : base(x, y, width, height, true)
		{
			this.callback = callback;

			base.exitFunction = () => onExit?.Invoke();

			connectButton = new ClickableTextureComponent(new Rectangle(x + width - 25, y + height - 35, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46, -1, -1), 1f, false);
			
			string[] installedMods = ModEntry.ModHelper.ModRegistry.GetAll().Select(a => a.Manifest.UniqueID).ToArray();

			var serverModDictionary = new Dictionary<string, Color>();
			foreach (string serverMod in serverMods.Split(',').Where(a => !string.IsNullOrWhiteSpace(a)))
			{
				serverModDictionary.Add(serverMod, installedMods.Contains(serverMod) ? Color.Green : Color.Red);
			}

			ServerModsBox = new ListBox(x + width - (width / 2 - 70 - 70), y + 121, (width / 2 - 70 - 70 - 66), Math.Max(1, (height - 350) / (ListBox.rowHeight*2)), serverModDictionary);
			
			var requiredModDictionary = new Dictionary<string, Color>();
			foreach (string requiredMod in requiredMods.Split(',').Where(a => !string.IsNullOrWhiteSpace(a)))
			{
				bool hasMod = installedMods.Contains(requiredMod);

				if (!hasMod)
				{
					missingRequiredMod = true;
					Console.WriteLine($"Missing mod {requiredMod}");
				}

				requiredModDictionary.Add(requiredMod, hasMod ? Color.Green : Color.Red);
			}
				

			RequiredModsBox = new ListBox(x + width - (width / 2 - 70 - 70), yPositionOnScreen + 93 + height / 2, (width / 2 - 70 - 70 - 66), Math.Max(1, (height - 350) / (ListBox.rowHeight * 2)), requiredModDictionary);
		}

		public override void draw(SpriteBatch spriteBatch)
		{
			drawBackground(spriteBatch);

			drawTextureBox(spriteBatch, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), base.xPositionOnScreen, base.yPositionOnScreen, base.width, base.height, Color.White, 4f, false);

			SpriteText.drawString(spriteBatch, FarmName, xPositionOnScreen + 50, yPositionOnScreen + 45);
			SpriteText.drawString(spriteBatch, "Server Mods", xPositionOnScreen - 195 - (ServerModsBox.width/2) + width, yPositionOnScreen + 45);
			
			int color = !missingRequiredMod || Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1000 < 500 ? -1 : 2;
			SpriteText.drawString(spriteBatch, "Required Mods", xPositionOnScreen - 209 - (ServerModsBox.width / 2) + width , yPositionOnScreen + 17 + height/2, color:color);

			spriteBatch.DrawString(Game1.smallFont, $"Online players: {PlayersOnline}", new Vector2(xPositionOnScreen + 50, yPositionOnScreen + 100), Color.Black);
			spriteBatch.DrawString(Game1.smallFont, $"Max players: {PlayerSlots}", new Vector2(xPositionOnScreen + 50, yPositionOnScreen + 130), Color.Black);
			spriteBatch.DrawString(Game1.smallFont, $"Free cabins: {CabinCountText}", new Vector2(xPositionOnScreen + 50, yPositionOnScreen + 160), Color.Black);

			#region Server message
			SpriteText.drawString(spriteBatch, "Server message", xPositionOnScreen + 50, yPositionOnScreen + 213);
			
			var bounds = new Rectangle(xPositionOnScreen + 76, yPositionOnScreen + 290, width/2 - 70, height - yPositionOnScreen - 302);
			DrawTools.DrawBox(spriteBatch, bounds.X, bounds.Y, bounds.Width, bounds.Height);

			bounds.X += 6;
			bounds.Y += 10;
			bounds.Width -= 21;
			DrawWrappedText(spriteBatch, ServerDescription, bounds, Game1.smallFont, Color.SaddleBrown);
			#endregion

			ServerModsBox.Draw(spriteBatch);
			RequiredModsBox.Draw(spriteBatch);

			if (!missingRequiredMod)
				connectButton.draw(spriteBatch);

			base.draw(spriteBatch);
			drawMouse(spriteBatch);
		}

		public override void performHoverAction(int x, int y)
		{
			ServerModsBox.TryHover(x, y);
			RequiredModsBox.TryHover(x, y);

			if (!missingRequiredMod)
				connectButton.tryHover(x, y, 0.2f);

			base.performHoverAction(x, y);
		}

		public override void releaseLeftClick(int x, int y)
		{
			ServerModsBox.ReleaseClick(x, y);
			RequiredModsBox.ReleaseClick(x, y);
			
			if (connectButton.containsPoint(x, y) && !missingRequiredMod)
			{
				Console.WriteLine("Connect button clicked");
				callback?.Invoke();
			}

			base.releaseLeftClick(x, y);
		}

		private void DrawWrappedText(SpriteBatch spriteBatch, string text, Rectangle bounds, SpriteFont font, Color color, int lineSpacing = 0)
		{
			List<string> lines = new List<string>();

			StringBuilder currentLine = new StringBuilder(256);
			for (int i = 0; i < text.Length; i++)
			{
				if (i + 1 != text.Length && text[i] == '\\' && text[i] == 'n')
				{
					lines.Add(currentLine.ToString());
					currentLine.Clear();
					i++;
				}

				currentLine.Append(text[i]);

				if (i + 1 == text.Length)
				{
					lines.Add(currentLine.ToString());
					currentLine.Clear();
				}
				else if (font.MeasureString(currentLine.Append(text[i + 1])).X > bounds.Width)
				{
					lines.Add(currentLine.ToString());
					currentLine.Clear();
				}
				else
				{
					currentLine.Remove(currentLine.Length - 1, 1);//Remove the last character that was added as a check
				}
			}

			int lineHeight = lines.Count == 0 ? 0 : (int)font.MeasureString(lines[0]).Y;

			int currentHeight = 0;
			foreach (string line in lines)
			{
				currentHeight += lineSpacing + lineHeight;
				if (currentHeight < bounds.Height)
				{
					spriteBatch.DrawString(font, line, new Vector2(bounds.X, bounds.Y + currentHeight - lineSpacing - lineHeight), color);
				}
			}
		}
	}
}
