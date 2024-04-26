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
using System.Text;
using System.Threading.Tasks;

using Leclair.Stardew.BetterCrafting.DynamicRules;
using Leclair.Stardew.BetterCrafting.Models;
using Leclair.Stardew.Common.Crafting;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.GameData.Objects;
using StardewValley.Menus;

namespace Leclair.Stardew.BetterCrafting.Integrations.SpaceCore;

public class SCBuffRuleHandler : IDynamicRuleHandler {

	public readonly ISCSkill Skill;
	public readonly string FieldKey;

	private readonly Lazy<Texture2D?> SkillTexture;

	public SCBuffRuleHandler(ISCSkill skill) {
		Skill = skill;
		FieldKey = $"{SCIntegration.SKILL_BUFF_PREFIX}{SkillId}";

		SkillTexture = new(() => {
			try {
				return Skill.SkillsPageIcon;
			} catch(Exception ex) {
				ModEntry.Instance.Log($"Unable to access SpaceCore custom skill. Did the mod author make it internal or private?", StardewModdingAPI.LogLevel.Warn, ex, once: true);
				return null;
			}
		});
	}

	public string SkillId => Skill.Id;

	#region IDynamicRuleHandler

	public string DisplayName => I18n.Filter_Buff(Skill.SafeGetName());

	public string Description => I18n.Filter_Buff_About(Skill.SafeGetName());

	public Texture2D Texture => SkillTexture.Value ?? Game1.mouseCursors;

	public Rectangle Source => SkillTexture.Value is null ? new(268, 470, 16, 16) : SkillTexture.Value.Bounds;

	public bool AllowMultiple => false;

	public bool HasEditor => false;

	public IClickableMenu? GetEditor(IClickableMenu parent, IDynamicRuleData data) => null;

	public bool DoesRecipeMatch(IRecipe recipe, Lazy<Item?> item, object? state) {
		if (string.IsNullOrEmpty(SkillId) || item.Value is not SObject sobj || sobj.bigCraftable.Value || sobj.Edibility <= -300 || !Game1.objectData.TryGetValue(sobj.ItemId, out var data))
			return false;

		// Make sure we have buffs.
		if (data.Buffs is null || data.Buffs.Count == 0)
			return false;

		foreach (ObjectBuffData? buff in data.Buffs) {
			// Check for the field we want, and see if we have a positive value.
			if (buff?.CustomFields is not null && buff.CustomFields.TryGetValue(FieldKey, out string? val) && int.TryParse(val, out int value) && value > 0)
				return true;
		}

		// We didn't find it.
		return false;
	}

	public object? ParseState(IDynamicRuleData data) => null;

	#endregion

}
