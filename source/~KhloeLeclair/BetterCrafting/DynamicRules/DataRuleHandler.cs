/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

using Leclair.Stardew.BetterCrafting.Models;
using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Crafting;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;
using StardewValley.TokenizableStrings;

namespace Leclair.Stardew.BetterCrafting.DynamicRules;

public record struct DataRuleData(
	long[] searchIds
);

public class DataRuleHandler : IDynamicRuleHandler {

	public readonly ModEntry Mod;
	public readonly JsonDynamicRule Data;

	private long[]? SearchIds;
	private Lazy<SpriteInfo?> _Sprite;

	public DataRuleHandler(ModEntry mod, JsonDynamicRule data) {
		Mod = mod;
		Data = data;

		_Sprite = new(GetSpriteFromIcon);
	}

	public string DisplayName => TokenParser.ParseText(Data.DisplayName ?? Data.Id);

	public string? Description => string.IsNullOrEmpty(Data.Description) ? null : TokenParser.ParseText(Data.Description);

	public Texture2D Texture => _Sprite.Value?.Texture ?? Game1.mouseCursors;

	public Rectangle Source => _Sprite.Value?.BaseSource ?? ErrorIngredient.SOURCE;

	private SpriteInfo? GetSpriteFromIcon() {
		var icon = Data.Icon;
		switch (icon.Type) {
			case CategoryIcon.IconType.Item:
				if (!string.IsNullOrEmpty(icon.ItemId))
					return SpriteHelper.GetSprite(ItemRegistry.Create(icon.ItemId));

				return SpriteHelper.GetSprite(GetItems().First());

			case CategoryIcon.IconType.Texture:
				Texture2D? texture = icon.Source.HasValue ?
					SpriteHelper.GetTexture(icon.Source.Value)
					: null;

				if (!string.IsNullOrEmpty(icon.Path))
					try {
						texture = Mod.Helper.GameContent.Load<Texture2D>(icon.Path) ?? texture;
					} catch (Exception ex) {
						Mod.Log($"Unable to load texture \"{icon.Path}\" for data rule '{Data.Id}'", StardewModdingAPI.LogLevel.Warn, ex);
					}

				if (texture != null) {
					Rectangle rect = icon.Rect ?? texture.Bounds;
					return new SpriteInfo(
						texture,
						rect,
						baseScale: icon.Scale,
						baseFrames: icon.Frames
					);
				}

				break;
		}

		return null;
	}

	public bool AllowMultiple => false;

	public bool HasEditor => false;

	private IEnumerable<Item> GetItems() {
		SearchIds ??= Data.Rules.Select(_ => Mod.ItemCache.GetNextCachedQueryId()).ToArray();

		for (int i = 0; i < Data.Rules.Length; i++) {
			long id = SearchIds[i];
			var rule = Data.Rules[i];
			if (rule is null)
				continue;

			foreach (var match in Mod.ItemCache.GetItems(id, rule))
				yield return match;
		}
	}

	public bool DoesRecipeMatch(IRecipe recipe, Lazy<Item?> lazyItem, object? state) {
		if (lazyItem.Value is not Item item)
			return false;

		foreach (var match in GetItems())
			if (ItemEqualityComparer.Instance.Equals(item, match))
				return true;

		return false;
	}

	public IClickableMenu? GetEditor(IClickableMenu parent, IDynamicRuleData data) {
		return null;
	}

	public object? ParseState(IDynamicRuleData type) {
		return null;
	}
}
