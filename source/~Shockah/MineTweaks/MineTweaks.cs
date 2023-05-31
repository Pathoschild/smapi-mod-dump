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
using Shockah.CommonModCode.GMCM;
using Shockah.Kokoro;
using Shockah.Kokoro.GMCM;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Linq;

namespace Shockah.MineTweaks
{
	public class MineTweaks : BaseMod<ModConfig>
	{
		private static MineTweaks Instance = null!;

		public override void OnEntry(IModHelper helper)
		{
			Instance = this;
			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
		}

		private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
		{
			SetupConfig();

			Harmony harmony = new(ModManifest.UniqueID);
			harmony.TryPatch(
				monitor: Monitor,
				original: () => AccessTools.Method(typeof(MineShaft), "adjustLevelChances"),
				postfix: new HarmonyMethod(GetType(), nameof(MineShaft_adjustLevelChances_Postfix))
			);
			harmony.TryPatch(
				monitor: Monitor,
				original: () => AccessTools.Method(typeof(VolcanoDungeon), "adjustLevelChances"),
				postfix: new HarmonyMethod(GetType(), nameof(VolcanoDungeon_adjustLevelChances_Postfix))
			);
		}

		private void SetupConfig()
		{
			var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
			if (api is null)
				return;
			GMCMI18nHelper helper = new(api, ModManifest, Helper.Translation);

			api.Register(
				ModManifest,
				reset: () => Config = new ModConfig(),
				save: () =>
				{
					WriteConfig();
					LogConfig();
				}
			);

			void SetupMineTypeConfig(string sectionTitleKey, Func<MineTypeConfig> config)
			{
				helper.AddSectionTitle(sectionTitleKey);

				helper.AddNumberOption(
					"config.stoneChanceMultiplier",
					() => config().StoneChanceMultiplier,
					v => config().StoneChanceMultiplier = v,
					min: 0f, max: 25f, interval: 0.05f
				);
				helper.AddNumberOption(
					"config.gemStoneChanceMultiplier",
					() => config().GemStoneChanceMultiplier,
					v => config().GemStoneChanceMultiplier = v,
					min: 0f, max: 25f, interval: 0.05f
				);
				helper.AddNumberOption(
					"config.itemChanceMultiplier",
					() => config().ItemChanceMultiplier,
					v => config().ItemChanceMultiplier = v,
					min: 0f, max: 25f, interval: 0.05f
				);
				helper.AddNumberOption(
					"config.monsterChanceMultiplier",
					() => config().MonsterChanceMultiplier,
					v => config().MonsterChanceMultiplier = v,
					min: 0f, max: 25f, interval: 0.05f
				);
				helper.AddNumberOption(
					"config.monsterMuskChanceMultiplier",
					() => config().MonsterMuskChanceMultiplier,
					v => config().MonsterMuskChanceMultiplier = v,
					min: 0f, max: 25f, interval: 0.05f
				);
			}

			SetupMineTypeConfig("config.section.mine", () => Config.Mine);
			SetupMineTypeConfig("config.section.skullCavern", () => Config.SkullCavern);
			SetupMineTypeConfig("config.section.volcano", () => Config.Volcano);
		}

		private static void MineShaft_adjustLevelChances_Postfix(MineShaft __instance, ref double stoneChance, ref double monsterChance, ref double itemChance, ref double gemStoneChance)
		{
			if (__instance.mineLevel == MineShaft.quarryMineShaft)
				return;
			MineTypeConfig config = __instance.mineLevel >= MineShaft.desertArea ? Instance.Config.SkullCavern : Instance.Config.Mine;
			ApplyChances(config, ref stoneChance, ref monsterChance, ref itemChance, ref gemStoneChance);
		}

		private static void VolcanoDungeon_adjustLevelChances_Postfix(ref double stoneChance, ref double monsterChance, ref double itemChance, ref double gemStoneChance)
		{
			MineTypeConfig config = Instance.Config.Volcano;
			ApplyChances(config, ref stoneChance, ref monsterChance, ref itemChance, ref gemStoneChance);
		}

		private static void ApplyChances(MineTypeConfig config, ref double stoneChance, ref double monsterChance, ref double itemChance, ref double gemStoneChance)
		{
			stoneChance *= config.StoneChanceMultiplier;
			monsterChance *= config.MonsterChanceMultiplier;
			itemChance *= config.ItemChanceMultiplier;
			gemStoneChance *= config.GemStoneChanceMultiplier;

			bool AnyOnlineFarmerHasBuff(int buffID)
				=> Game1.getOnlineFarmers().Any(player => player.hasBuff(buffID));

			if (AnyOnlineFarmerHasBuff(24))
				monsterChance *= 0.5f * config.MonsterMuskChanceMultiplier;
		}
	}
}