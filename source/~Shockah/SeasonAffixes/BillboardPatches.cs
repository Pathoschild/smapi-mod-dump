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
using Shockah.Kokoro.Stardew;
using Shockah.Kokoro.UI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Shockah.SeasonAffixes;

internal static class BillboardPatches
{
	private const int AffixComponentID = 1629701; // {NexusID}01
	private const int IconWidth = 44;
	private const int IconHeight = 44;
	private const int IconSpacing = 12;

	private static readonly Lazy<Func<Billboard, bool>> BillboardDailyQuestBoardGetter = new(() => AccessTools.Field(typeof(Billboard), "dailyQuestBoard").EmitInstanceGetter<Billboard, bool>());
	private static readonly Lazy<Action<Billboard, string>> BillboardHoverTextSetter = new(() => AccessTools.Field(typeof(Billboard), "hoverText").EmitInstanceSetter<Billboard, string>());
	private static readonly PerScreen<bool> PerScreenHoveringOverAffixes = new(() => false);
	private static readonly ConditionalWeakTable<Billboard, Dictionary<ISeasonAffix, TextureRectangle>> AffixIconCache = new();

	private static bool HoveringOverAffixes
	{
		get => PerScreenHoveringOverAffixes.Value;
		set => PerScreenHoveringOverAffixes.Value = value;
	}

	internal static void Apply(Harmony harmony)
	{
		harmony.TryPatch(
			monitor: SeasonAffixes.Instance.Monitor,
			original: () => AccessTools.Method(typeof(Billboard), nameof(Billboard.performHoverAction)),
			postfix: new HarmonyMethod(AccessTools.Method(typeof(BillboardPatches), nameof(performHoverAction_Postfix)))
		);

		harmony.TryPatch(
			monitor: SeasonAffixes.Instance.Monitor,
			original: () => AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.populateClickableComponentList)),
			postfix: new HarmonyMethod(AccessTools.Method(typeof(BillboardPatches), nameof(IClickableMenu_populateClickableComponentList_Postfix)))
		);

		harmony.TryPatch(
				monitor: SeasonAffixes.Instance.Monitor,
				original: () => AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.draw), new Type[] { typeof(SpriteBatch) }),
				prefix: new HarmonyMethod(AccessTools.Method(typeof(BillboardPatches), nameof(IClickableMenu_draw_Prefix)))
			);

		harmony.TryPatch(
			monitor: SeasonAffixes.Instance.Monitor,
			original: () => AccessTools.Method(typeof(Billboard), nameof(Billboard.draw), new Type[] { typeof(SpriteBatch) }),
			postfix: new HarmonyMethod(AccessTools.Method(typeof(BillboardPatches), nameof(Billboard_draw_Postfix)))
		);
	}

	private static void performHoverAction_Postfix(Billboard __instance, int x, int y)
	{
		HoveringOverAffixes = false;
		if (BillboardDailyQuestBoardGetter.Value(__instance))
			return;

		var affixes = SeasonAffixes.Instance.GetUIOrderedAffixes(new(Game1.Date.Year, Game1.Date.GetSeason()), SeasonAffixes.Instance.ActiveAffixes);
		int width = affixes.Count * IconWidth + (affixes.Count - 1) * IconSpacing;
		Rectangle bounds = new(
			__instance.xPositionOnScreen + __instance.width - 144 - width - IconWidth / 2,
			__instance.yPositionOnScreen + 100 - IconHeight / 2,
			width,
			IconHeight
		);
		if (bounds.Contains(x, y))
		{
			HoveringOverAffixes = true;
			BillboardHoverTextSetter.Value(__instance, "");
		}
	}

	private static void IClickableMenu_populateClickableComponentList_Postfix(IClickableMenu __instance)
	{
		if (__instance is not Billboard menu)
			return;
		if (BillboardDailyQuestBoardGetter.Value(menu))
			return;

		var affixes = SeasonAffixes.Instance.GetUIOrderedAffixes(new(Game1.Date.Year, Game1.Date.GetSeason()), SeasonAffixes.Instance.ActiveAffixes);
		int width = affixes.Count * IconWidth + (affixes.Count - 1) * IconSpacing;
		Rectangle bounds = new(
			__instance.xPositionOnScreen + __instance.width - 144 - width - IconWidth / 2,
			__instance.yPositionOnScreen + 100 - IconHeight / 2,
			width,
			IconHeight
		);
		__instance.allClickableComponents.Add(new ClickableComponent(bounds, "") { myID = AffixComponentID, downNeighborID = 7 });

		for (int i = 1; i <= 7; i++)
		{
			var component = __instance.getComponentWithID(i);
			if (component is not null)
				component.upNeighborID = AffixComponentID;
		}
	}

	private static void IClickableMenu_draw_Prefix(IClickableMenu __instance, SpriteBatch b)
	{
		if (__instance is not Billboard menu)
			return;
		if (BillboardDailyQuestBoardGetter.Value(menu))
			return;
		if (!AffixIconCache.TryGetValue(menu, out var affixIconCache))
		{
			affixIconCache = new();
			AffixIconCache.AddOrUpdate(menu, affixIconCache);
		}

		var affixes = SeasonAffixes.Instance.GetUIOrderedAffixes(new(Game1.Date.Year, Game1.Date.GetSeason()), SeasonAffixes.Instance.ActiveAffixes);
		int width = affixes.Count * IconWidth + (affixes.Count - 1) * IconSpacing;
		for (int i = 0; i < affixes.Count; i++)
		{
			var affix = affixes[i];
			if (!affixIconCache.TryGetValue(affix, out var icon))
			{
				icon = affix.Icon;
				affixIconCache[affix] = icon;
			}

			float iconScale = 1f;
			if (icon.Rectangle.Width * iconScale < IconWidth)
				iconScale = 1f * IconWidth / icon.Rectangle.Width;
			if (icon.Rectangle.Height * iconScale < IconHeight)
				iconScale = 1f * IconHeight / icon.Rectangle.Height;
			if (icon.Rectangle.Width * iconScale > IconWidth)
				iconScale = 1f * IconWidth / icon.Rectangle.Width;
			if (icon.Rectangle.Height * iconScale > IconHeight)
				iconScale = 1f * IconHeight / icon.Rectangle.Height;

			var iconPosition = new Vector2(menu.xPositionOnScreen + menu.width - 144 - width + (IconWidth + IconSpacing) * i, menu.yPositionOnScreen + 100);
			b.Draw(icon.Texture, iconPosition + new Vector2(-iconScale, iconScale), icon.Rectangle, Color.Black * 0.3f, 0f, new Vector2(icon.Rectangle.Width / 2f, icon.Rectangle.Height / 2f), iconScale, SpriteEffects.None, 4f);
			b.Draw(icon.Texture, iconPosition, icon.Rectangle, Color.White, 0f, new Vector2(icon.Rectangle.Width / 2f, icon.Rectangle.Height / 2f), iconScale, SpriteEffects.None, 4f);
		}
	}

	private static void Billboard_draw_Postfix(Billboard __instance, SpriteBatch b)
	{
		if (BillboardDailyQuestBoardGetter.Value(__instance))
			return;

		if (HoveringOverAffixes)
		{
			var affixes = SeasonAffixes.Instance.GetUIOrderedAffixes(new(Game1.Date.Year, Game1.Date.GetSeason()), SeasonAffixes.Instance.ActiveAffixes);
			IClickableMenu.drawToolTip(b, SeasonAffixes.Instance.GetSeasonDescription(affixes), SeasonAffixes.Instance.GetSeasonName(affixes), null);
		}
	}
}