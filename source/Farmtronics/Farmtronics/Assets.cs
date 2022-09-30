/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoeStrout/Farmtronics
**
*************************************************/

using System.IO;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace Farmtronics {
	// Contains required assets
	static class Assets {
		public static Texture2D BotSprites    {get; private set;}
		public static Texture2D ScreenOverlay {get; private set;}
		public static Texture2D FontAtlas     {get; private set;}
		public static string[]  FontList      {get; private set;}
		
		
		public static void Initialize(IModHelper helper) {
			BotSprites    = helper.ModContent.Load<Texture2D>(Path.Combine("assets", "BotSprites.png"));
			ScreenOverlay = helper.ModContent.Load<Texture2D>(Path.Combine("assets", "ScreenOverlay.png"));
			FontAtlas 	  = helper.ModContent.Load<Texture2D>(Path.Combine("assets", "fontAtlas.png"));
			FontList	  = System.IO.File.ReadAllLines(Path.Combine(helper.DirectoryPath, "assets", "fontList.txt"));
		}
	}
}