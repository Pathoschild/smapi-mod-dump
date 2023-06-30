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
using Shockah.Kokoro;
using Shockah.Kokoro.Stardew;
using Shockah.Kokoro.UI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Shockah.SeasonAffixes;

internal sealed class LootAffix : BaseSeasonAffix, ISeasonAffix
{
	private static bool IsHarmonySetup = false;
	private static readonly WeakCounter<GameLocation> MonsterDropCallCounter = new();
	private static readonly ConditionalWeakTable<GameLocation, Random> MonsterDropRandomCache = new();

	private static string ShortID => "Loot";
	public string LocalizedDescription => Mod.Helper.Translation.Get($"{I18nPrefix}.description");
	public TextureRectangle Icon => new(Game1.objectSpriteSheet, new(352, 96, 16, 16));

	public LootAffix() : base(ShortID, "positive") { }

	public int GetPositivity(OrdinalSeason season)
		=> 1;

	public int GetNegativity(OrdinalSeason season)
		=> 0;

	public IReadOnlySet<string> Tags { get; init; } = new HashSet<string> { VanillaSkill.Combat.UniqueID };

	public void OnRegister()
		=> Apply(Mod.Harmony);

	private void Apply(Harmony harmony)
	{
		if (IsHarmonySetup)
			return;
		IsHarmonySetup = true;

		harmony.TryPatchVirtual(
			monitor: Mod.Monitor,
			original: () => AccessTools.Method(typeof(GameLocation), nameof(GameLocation.monsterDrop)),
			prefix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(GameLocation_monsterDrop_Prefix)), priority: Priority.First),
			finalizer: new HarmonyMethod(AccessTools.Method(GetType(), nameof(GameLocation_monsterDrop_Finalizer)), priority: Priority.Last)
		);
	}

	private static void GameLocation_monsterDrop_Prefix(GameLocation __instance)
	{
		if (!Mod.IsAffixActive(a => a is LootAffix))
			return;

		uint counter = MonsterDropCallCounter.Push(__instance);
		if (counter != 1)
			return;

		MonsterDropRandomCache.AddOrUpdate(__instance, Game1.random);
		Game1.random = new CustomRandom(Game1.random);
	}

	private static void GameLocation_monsterDrop_Finalizer(GameLocation __instance)
	{
		if (!Mod.IsAffixActive(a => a is LootAffix))
			return;

		uint counter = MonsterDropCallCounter.Pop(__instance);
		if (counter != 0)
			return;

		if (!MonsterDropRandomCache.TryGetValue(__instance, out var cachedRandom))
			throw new InvalidOperationException("Expected a Random instance to be cached.");
		Game1.random = cachedRandom;
	}

	private sealed class CustomRandom : Random
	{
		private Random Wrapped { get; init; }

		public CustomRandom(Random wrapped)
		{
			this.Wrapped = wrapped;
		}

		public override int Next()
			=> Wrapped.Next();

		public override int Next(int maxValue)
			=> Wrapped.Next(maxValue);

		public override int Next(int minValue, int maxValue)
			=> Wrapped.Next(minValue, maxValue);

		public override void NextBytes(byte[] buffer)
			=> Wrapped.NextBytes(buffer);

		public override void NextBytes(Span<byte> buffer)
			=> Wrapped.NextBytes(buffer);

		public override double NextDouble()
			=> Math.Pow(Wrapped.NextDouble(), 2.0 / 3.0);
	}
}