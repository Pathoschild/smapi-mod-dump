/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.Kokoro.Stardew;

public enum MultiplayerMode { SinglePlayer, Client, Server }

public static class GameExt
{
	private static readonly Lazy<Texture2D> LazyPixel = new(() =>
	{
		var texture = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
		texture.SetData(new[] { Color.White });
		return texture;
	});

	public static Texture2D Pixel
		=> LazyPixel.Value;

	public static MultiplayerMode GetMultiplayerMode()
		=> (MultiplayerMode)Game1.multiplayerMode;

	public static Farmer GetHostPlayer()
		=> Game1.getAllFarmers().First(p => p.slotCanHost);

	public static IReadOnlyList<GameLocation> GetAllLocations()
	{
		List<GameLocation> locations = [];
		Utility.ForEachLocation(l =>
		{
			if (l is not null)
				locations.Add(l);
			return true;
		}, includeInteriors: true, includeGenerated: true);
		return locations;
	}
}