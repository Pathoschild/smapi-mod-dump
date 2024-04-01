/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Kokoro;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.HatsOnHats;

public class ModEntry : BaseMod
{
	private static Hat? ExtraHatOverride { get; set; }
	private static Texture2D? OriginalHairstylesTexture { get; set; }
	private static Texture2D? OriginalAccessoriesTexture { get; set; }
	private static Hat? OriginalHat { get; set; }

	private static readonly Lazy<Texture2D> ClearPixel = new(() =>
	{
		var texture = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
		texture.SetData(new[] { Color.Transparent });
		return texture;
	});

	public override void Entry(IModHelper helper)
	{
		base.Entry(helper);

		helper.Events.GameLoop.GameLaunched += OnGameLaunched;
	}

	private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
	{
		Harmony harmony = new(ModManifest.UniqueID);

		harmony.TryPatch(
			monitor: Monitor,
			original: () => AccessTools.DeclaredMethod(typeof(FarmerRenderer), nameof(FarmerRenderer.drawHairAndAccesories)),
			prefix: new HarmonyMethod(GetType(), nameof(FarmerRenderer_drawHairAndAccesories_Prefix)),
			finalizer: new HarmonyMethod(GetType(), nameof(FarmerRenderer_drawHairAndAccesories_Finalizer))
		);
		harmony.TryPatch(
			monitor: Monitor,
			original: () => AccessTools.DeclaredMethod(typeof(Farmer), nameof(Farmer.GetDisplayPants)),
			postfix: new HarmonyMethod(GetType(), nameof(Farmer_GetDisplayPants_Postfix))
		);
		harmony.TryPatch(
			monitor: Monitor,
			original: () => AccessTools.DeclaredMethod(typeof(Farmer), nameof(Farmer.GetDisplayShirt)),
			postfix: new HarmonyMethod(GetType(), nameof(Farmer_GetDisplayShirt_Postfix))
		);
	}

	private static IEnumerable<Hat> GetHats(Farmer player)
	{
		IEnumerable<Hat> GetUnsortedHats()
		{
			var currentHat = OriginalHat ?? player.hat.Value;
			if (currentHat is not null)
				yield return currentHat;
			foreach (var item in player.Items)
				if (item is Hat hat && !ReferenceEquals(hat, currentHat))
					yield return hat;
		}

		return GetUnsortedHats()
			.OrderByDescending(h => h.isMask)
			.ThenByDescending(h => h.hairDrawType.Value);
	}

	private static void FarmerRenderer_drawHairAndAccesories_Prefix(Farmer who)
	{
		if (ExtraHatOverride is null)
		{
			OriginalHairstylesTexture = FarmerRenderer.hairStylesTexture;
			OriginalAccessoriesTexture = FarmerRenderer.accessoriesTexture;
			OriginalHat = who.hat.Value;

			who.hat.Value = GetHats(who).FirstOrDefault();
		}
		else
		{
			who.hat.Value = ExtraHatOverride;
		}
	}

	private static void FarmerRenderer_drawHairAndAccesories_Finalizer(FarmerRenderer __instance, SpriteBatch b, int facingDirection, Farmer who, Vector2 position, Vector2 origin, float scale, int currentFrame, float rotation, Color overrideColor, float layerDepth)
	{
		if (ExtraHatOverride is not null)
			return;
		who.hat.Value = OriginalHat;

		try
		{
			FarmerRenderer.hairStylesTexture = ClearPixel.Value;
			FarmerRenderer.accessoriesTexture = ClearPixel.Value;

			int index = 0;
			int layer = 0;
			foreach (var hat in GetHats(who))
			{
				if (ReferenceEquals(hat, OriginalHat))
					continue;
				if (index != 0)
				{
					ExtraHatOverride = hat;
					__instance.drawHairAndAccesories(b, facingDirection, who, new(position.X, position.Y - 24 * layer), origin, scale, currentFrame, rotation, overrideColor, layerDepth + 3.9E-05f + 0.01f * index);
				}

				index++;
				if (!hat.isMask)
					layer++;
			}
		}
		finally
		{
			FarmerRenderer.hairStylesTexture = OriginalHairstylesTexture;
			FarmerRenderer.accessoriesTexture = OriginalAccessoriesTexture;
			who.hat.Value = OriginalHat;
			ExtraHatOverride = null;
		}
	}

	private static void Farmer_GetDisplayPants_Postfix(ref Texture2D texture)
	{
		if (ExtraHatOverride is not null)
			texture = ClearPixel.Value;
	}

	private static void Farmer_GetDisplayShirt_Postfix(ref Texture2D texture)
	{
		if (ExtraHatOverride is not null)
			texture = ClearPixel.Value;
	}
}