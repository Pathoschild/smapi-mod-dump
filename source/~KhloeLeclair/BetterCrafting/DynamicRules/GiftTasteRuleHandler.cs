/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System.Collections.Generic;

using Leclair.Stardew.BetterCrafting.Models;
using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Crafting;
using Leclair.Stardew.Common.UI;
using Leclair.Stardew.Common.UI.FlowNode;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.BetterCrafting.DynamicRules;

public enum GiftTaste {
	Like = NPC.gift_taste_like,
	Love = NPC.gift_taste_love,
	Dislike = NPC.gift_taste_dislike,
	Hate = NPC.gift_taste_hate
};

public class GiftTasteRuleHandler : DynamicTypeHandler<NPC?>, IOptionInputRuleHandler {

	private readonly ModEntry Mod;
	public readonly GiftTaste Taste;

	public GiftTasteRuleHandler(ModEntry mod, GiftTaste taste) {
		Mod = mod;
		Taste = taste;
	}

	public string TasteName => Taste switch {
		GiftTaste.Love => I18n.Filter_GiftTaste_Loves(),
		GiftTaste.Like => I18n.Filter_GiftTaste_Likes(),
		GiftTaste.Dislike => I18n.Filter_GiftTaste_Dislikes(),
		_ => I18n.Filter_GiftTaste_Hates()
	};

	public override string DisplayName => I18n.Filter_GiftTaste(TasteName);

	public override string? Description => I18n.Filter_GiftTaste_About(TasteName);

	public override Texture2D GetTexture(NPC? state) {
		if (state is null)
			return base.GetTexture(state);

		return Game1.content.Load<Texture2D>(@"Characters\" + state.getTextureName());
	}

	public override Rectangle GetSource(NPC? state) {
		if (state is null)
			return base.GetSource(state);

		Mod.GetHeads().TryGetValue(state.Name, out HeadSize? info);

		return new Rectangle(
			info?.OffsetX ?? 0,
			info?.OffsetY ?? 0,
			info?.Width ?? 16,
			info?.Height ?? 15
		);
	}

	public override Texture2D Texture => Game1.mouseCursors;

	public override Rectangle Source => new(211, 428, 7, 6);

	public override bool AllowMultiple => true;

	public override bool HasEditor => true;

	public IEnumerable<KeyValuePair<string, string>> GetOptions(bool cooking) {
		List<KeyValuePair<string, string>> result = new();

		List<SObject> craftables = [];
		foreach (var recipe in Mod.Recipes.GetRecipes(cooking))
			if (recipe.CreateItemSafe() is SObject sobj && !sobj.bigCraftable.Value)
				craftables.Add(sobj);

		Dictionary<NPC, int> chars = [];

		bool show_all = Mod.Config.EffectiveShowAllTastes;

		Utility.ForEachCharacter(npc => {
			if (npc.CanSocialize) {
				// Check that the NPC should appear.
				var data = npc.GetData();
				if (data?.SocialTab != StardewValley.GameData.Characters.SocialTabBehavior.AlwaysShown && (!Game1.player.friendshipData.TryGetValue(npc.Name, out var friendship) || friendship == null))
					return true;

				int count = 0;

				foreach (var sobj in craftables) {
					if (!show_all && !Game1.player.hasGiftTasteBeenRevealed(npc, sobj.ItemId))
						continue;

					int taste;
					try {
						taste = npc.getGiftTasteForThisItem(sobj);
					} catch {
						continue;
					}

					if (taste == (int) Taste)
						count++;
				}

				chars[npc] = count;
			}

			return true;
		});

		// Build a nice list.
		foreach (var entry in chars) {
			string count = I18n.Filter_RecipeCount(entry.Value);
			result.Add(new(entry.Key.Name, $"{entry.Key.displayName}\n@h{count}"));
		}

		result.Sort((a, b) => {
			return a.Value.CompareTo(b.Value);
		});

		return result;
	}

	public string HelpText => string.Empty;

	public override bool DoesRecipeMatch(IRecipe recipe, System.Lazy<Item?> item, NPC? npc) {
		if (npc is null || item.Value is not SObject sobj || sobj.bigCraftable.Value)
			return false;

		if (!Mod.Config.EffectiveShowAllTastes && !Game1.player.hasGiftTasteBeenRevealed(npc, sobj.ItemId))
			return false;

		int taste;
		try {
			taste = npc.getGiftTasteForThisItem(sobj);
		} catch {
			return false;
		}

		return taste == (int) Taste;
	}

	public override IClickableMenu? GetEditor(IClickableMenu parent, IDynamicRuleData data) {
		return null;
	}

	public override IFlowNode[]? GetExtraInfo(NPC? state) {
		if (state is null)
			return null;

		return FlowHelper.Builder()
			.Text(state.displayName, shadow: false)
			.Build();
	}

	public override NPC? ParseStateT(IDynamicRuleData type) {
		if (!type.Fields.TryGetValue("Input", out var token))
			return null;

		string? rawInput = (string?) token;
		if (string.IsNullOrWhiteSpace(rawInput))
			return null;

		return Game1.getCharacterFromName(rawInput);
	}

}
