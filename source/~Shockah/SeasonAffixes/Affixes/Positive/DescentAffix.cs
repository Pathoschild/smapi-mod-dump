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
using Shockah.Kokoro;
using Shockah.Kokoro.Stardew;
using Shockah.Kokoro.UI;
using StardewValley;
using StardewValley.Locations;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.SeasonAffixes;

internal sealed class DescentAffix : BaseSeasonAffix, ISeasonAffix
{
	private static bool IsHarmonySetup = false;

	private static string ShortID => "Descent";
	public string LocalizedDescription => Mod.Helper.Translation.Get($"{I18nPrefix}.description");
	public TextureRectangle Icon => new(Game1.bigCraftableSpriteSheet, new(112, 272, 16, 16));

	public DescentAffix() : base(ShortID, "positive") { }

	public int GetPositivity(OrdinalSeason season)
		=> 1;

	public int GetNegativity(OrdinalSeason season)
		=> 0;

	public IReadOnlySet<string> Tags { get; init; } = new HashSet<string> { VanillaSkill.MetalAspect, VanillaSkill.GemAspect, VanillaSkill.Combat.UniqueID };

	public double GetProbabilityWeight(OrdinalSeason season)
	{
		bool finishedMine = MineShaft.lowestLevelReached >= 120;
		bool busUnlocked = Game1.getAllFarmers().Any(p => p.mailReceived.Contains("ccVault") || p.mailReceived.Contains("jojaVault"));
		return busUnlocked || !finishedMine ? 1 : 0;
	}

	public void OnRegister()
		=> Apply(Mod.Harmony);

	private void Apply(Harmony harmony)
	{
		if (IsHarmonySetup)
			return;
		IsHarmonySetup = true;

		harmony.TryPatch(
			monitor: Mod.Monitor,
			original: () => AccessTools.Method(typeof(MineShaft), nameof(MineShaft.monsterDrop)),
			postfix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(MineShaft_monsterDrop_Postfix)))
		);
	}

	private static void MineShaft_monsterDrop_Postfix(MineShaft __instance, int x, int y)
	{
		if (!Mod.IsAffixActive(a => a is DescentAffix))
			return;
		if (__instance.mustKillAllMonstersToAdvance())
			return;
		if (__instance.EnemyCount > 1)
			return;
		__instance.recursiveTryToCreateLadderDown(new Vector2((int)(x / 64f), (int)(y / 64f)), "newArtifact", 200);
	}
}