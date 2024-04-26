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

using Leclair.Stardew.BetterCrafting.Models;
using Leclair.Stardew.Common.Crafting;
using Leclair.Stardew.Common.UI.FlowNode;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.GameData.Buffs;
using StardewValley.GameData.Objects;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using StardewValley.TokenizableStrings;

namespace Leclair.Stardew.BetterCrafting.DynamicRules;

public class BuffRuleHandler : IDynamicRuleHandler {

	// TODO: Add support for custom skills, like via SpaceCore.

	public const int FARMING = 0;
	public const int FISHING = 1;
	public const int MINING = 2;
	public const int LUCK = 4;
	public const int FORAGING = 5;
	public const int MAX_ENERGY = 7;
	public const int MAGNETISM = 8;
	public const int SPEED = 9;
	public const int DEFENSE = 10;
	public const int ATTACK = 11;

	public readonly int BuffIndex;
	public readonly string? BuffId;

	private readonly Texture2D? _Texture;

	public BuffRuleHandler(string? buffId, BuffData data) {
		BuffId = buffId;
		BuffIndex = int.MinValue;

		BuffName = TokenParser.ParseText(data.DisplayName);
		_Texture = ModEntry.Instance.Helper.GameContent.Load<Texture2D>(data.IconTexture);
		Source = Game1.getSourceRectForStandardTileSheet(Texture, data.IconSpriteIndex, 16, 16);
	}

	public BuffRuleHandler(int index) { 
		BuffIndex = index;
		_Texture = null;
		Source = new Rectangle(10 + 10 * BuffIndex, 428, 10, 10);
		BuffName = Game1.content.LoadString(@"Strings\UI:ItemHover_Buff" + BuffIndex, "").Trim();
	}

	public string BuffName;

	public string DisplayName => I18n.Filter_Buff(BuffName);
	public string Description => I18n.Filter_Buff_About(BuffName);

	public Texture2D Texture => _Texture ?? Game1.mouseCursors;
	public Rectangle Source { get; }

	public bool AllowMultiple => false;

	public bool HasEditor => false;

	public IClickableMenu? GetEditor(IClickableMenu parent, IDynamicRuleData type) => null;

	public object? ParseState(IDynamicRuleData type) {
		return null;
	}

	public float GetBuffLevel(BuffAttributesData? data) {
		if (data == null) return 0f;

		return BuffIndex switch {
			FARMING => data.FarmingLevel,
			FISHING => data.FishingLevel,
			MINING => data.MiningLevel,
			LUCK => data.LuckLevel,
			FORAGING => data.ForagingLevel,
			MAX_ENERGY => data.MaxStamina,
			MAGNETISM => data.MagneticRadius,
			SPEED => data.Speed,
			DEFENSE => data.Defense,
			ATTACK => data.Attack,
			_ => 0f,
		};
	}

	public bool DoesRecipeMatch(IRecipe recipe, Lazy<Item?> item, object? state) {
		if (BuffIndex < 0 || item.Value is not SObject sobj || sobj.bigCraftable.Value || sobj.Edibility <= -300 || !Game1.objectData.TryGetValue(sobj.ItemId, out var data))
			return false;

		// Make sure we have buffs.
		if ( data.Buffs is null || data.Buffs.Count == 0 )
			return false;

		foreach(ObjectBuffData? buff in data.Buffs) {
			if (buff is null)
				continue;

			if (BuffId != null && buff.BuffId == BuffId)
				return true;

			if (BuffIndex != int.MinValue && GetBuffLevel(buff.CustomAttributes) > 0f)
				return true;
		}

		// We didn't find it.
		return false;
	}

}
