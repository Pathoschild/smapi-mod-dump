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
using StardewValley.Menus;

namespace Leclair.Stardew.BetterCrafting.DynamicRules;

public class BuffRuleHandler : IDynamicRuleHandler {

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

	public BuffRuleHandler(int index) { 
		BuffIndex = index;
		Source = new Rectangle(10 + 10 * BuffIndex, 428, 10, 10);
	}

	public string BuffName => Game1.content.LoadString(@"Strings\UI:ItemHover_Buff" + BuffIndex, "").Trim();

	public string DisplayName => I18n.Filter_Buff(BuffName);
	public string Description => I18n.Filter_Buff_About(BuffName);

	public Texture2D Texture => Game1.mouseCursors;
	public Rectangle Source { get; }

	public bool AllowMultiple => false;

	public bool HasEditor => false;

	public IClickableMenu? GetEditor(IClickableMenu parent, IDynamicRuleData type) => null;

	public object? ParseState(IDynamicRuleData type) {
		return null;
	}

	public bool DoesRecipeMatch(IRecipe recipe, Lazy<Item?> item, object? state) {
		if (BuffIndex < 0 || item.Value is not SObject sobj || sobj.bigCraftable.Value || sobj.Edibility <= -300)
			return false;

		if (!Game1.objectInformation.TryGetValue(sobj.ParentSheetIndex, out string? info))
			return false;

		string[] parts = info.Split('/', StringSplitOptions.TrimEntries);

		if (parts.Length < 8 || !(parts[6].Equals("food") || parts[6].Equals("drink")))
			return false;

		string[] buffs = parts[7].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

		if (BuffIndex >= buffs.Length)
			return false;

		return int.TryParse(buffs[BuffIndex], out int buff) && buff > 0;
	}

}
