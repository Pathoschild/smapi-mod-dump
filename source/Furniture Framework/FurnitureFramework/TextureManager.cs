/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Leroymilo/FurnitureFramework
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace FurnitureFramework
{
	static class TextureManager
	{
		public static readonly Dictionary<string, Texture2D> textures = new();

		public static Texture2D load(IModContentHelper content_helper, string path)
		{

			string true_path;
			if (path.StartsWith("FF."))
			{
				content_helper = ModEntry.get_helper().ModContent;
				true_path = path[3..];
			}
			else
			{
				true_path = path;
			}

			if (!textures.ContainsKey(path))
			{
				textures[path] = content_helper.Load<Texture2D>(true_path);
				ModEntry.log($"loading texture at {true_path}", LogLevel.Trace);
			}
			return textures[path];
		}
	}
}