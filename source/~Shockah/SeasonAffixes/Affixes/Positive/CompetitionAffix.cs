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
using Newtonsoft.Json;
using Shockah.CommonModCode.GMCM;
using Shockah.Kokoro;
using Shockah.Kokoro.GMCM;
using Shockah.Kokoro.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Minigames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Shockah.SeasonAffixes;

partial class ModConfig
{
	[JsonProperty] public float CompetitionQiCoinSellValue { get; internal set; } = 0.5f;
	[JsonProperty] public float CompetitionQiCoinExchangeCap { get; internal set; } = 5f;
	[JsonProperty] public float CompetitionPrairieKingMonsterHealthMoney { get; internal set; } = 1f;
	[JsonProperty] public int CompetitionPrairieKingCoinMoney { get; internal set; } = 20;
	[JsonProperty] public int CompetitionJunimoKartCoinMoney { get; internal set; } = 5;
	[JsonProperty] public float CompetitionJunimoKartScoreMoney { get; internal set; } = 0.04f;
}

internal sealed class CompetitionAffix : BaseSeasonAffix, ISeasonAffix
{
	private static bool IsHarmonySetup = false;
	private static CompetitionAffix Instance = null!;

	private static readonly Lazy<Func<MineCart, int>> MineCartGameModeGetter = new(() => AccessTools.Field(typeof(MineCart), "gameMode").EmitInstanceGetter<MineCart, int>());
	private static readonly Lazy<Func<MineCart, int>> MineCartScoreGetter = new(() => AccessTools.Field(typeof(MineCart), "score").EmitInstanceGetter<MineCart, int>());

	private static string ShortID => "Competition";
	public string LocalizedDescription => Mod.Helper.Translation.Get($"{I18nPrefix}.description");
	public TextureRectangle Icon => new(Game1.emoteSpriteSheet, new(32, 208, 16, 16));

	private static readonly PerScreen<bool> RunOriginalBuyQiCoins = new(() => false);
	private static readonly PerScreen<int> QiCoinsLeftToSell = new(() => 0);
	private static readonly ConditionalWeakTable<AbigailGame.CowboyMonster, StructRef<int>> PrairieKingMonsterMaxHealth = new();
	private static readonly PerScreen<float> ArcadeMoneyToReceive = new(() => 0);

	public CompetitionAffix() : base(ShortID, "positive")
	{
		Instance = this;
	}

	public int GetPositivity(OrdinalSeason season)
		=> 1;

	public int GetNegativity(OrdinalSeason season)
		=> 0;

	public void OnRegister()
		=> Apply(Mod.Harmony);

	public void OnActivate(AffixActivationContext context)
	{
		Mod.Helper.Events.GameLoop.DayStarted += OnDayStarted;
		Mod.Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
	}

	public void OnDeactivate(AffixActivationContext context)
	{
		Mod.Helper.Events.GameLoop.DayStarted -= OnDayStarted;
		Mod.Helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
	}

	public void SetupConfig(IManifest manifest)
	{
		var api = Mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu")!;
		GMCMI18nHelper helper = new(api, Mod.ModManifest, Mod.Helper.Translation);

		helper.AddSectionTitle($"{I18nPrefix}.config.casino");
		helper.AddNumberOption($"{I18nPrefix}.config.qiCoinSellValue", () => Mod.Config.CompetitionQiCoinSellValue, min: 0.01f, max: 1f, interval: 0.01f, value => $"{value:0.##}");
		helper.AddNumberOption($"{I18nPrefix}.config.qiCoinExchangeCap", () => Mod.Config.CompetitionQiCoinExchangeCap, min: 0f, max: 25f, interval: 0.1f, value => $"{value:0.##}x");

		helper.AddSectionTitle($"{I18nPrefix}.config.prairieKing");
		helper.AddNumberOption($"{I18nPrefix}.config.prairieKing.monsterHealthMoney", () => Mod.Config.CompetitionPrairieKingMonsterHealthMoney, min: 0f, max: 10f, interval: 0.1f, value => $"{value:0.##}");
		helper.AddNumberOption($"{I18nPrefix}.config.prairieKing.coinMoney", () => Mod.Config.CompetitionPrairieKingCoinMoney, min: 0, max: 250, interval: 1);

		helper.AddSectionTitle($"{I18nPrefix}.config.junimoKart");
		helper.AddNumberOption($"{I18nPrefix}.config.junimoKart.coinMoney", () => Mod.Config.CompetitionJunimoKartCoinMoney, min: 0, max: 250, interval: 1);
		helper.AddNumberOption($"{I18nPrefix}.config.junimoKart.scoreMoney", () => Mod.Config.CompetitionJunimoKartScoreMoney, min: 0f, max: 1f, interval: 0.005f, value => $"{value:0.##}");
	}

	private void Apply(Harmony harmony)
	{
		if (IsHarmonySetup)
			return;
		IsHarmonySetup = true;

		harmony.TryPatch(
			monitor: Mod.Monitor,
			original: () => AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction)),
			prefix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(GameLocation_performAction_Prefix)))
		);

		harmony.TryPatch(
			monitor: Mod.Monitor,
			original: () => AccessTools.Method(typeof(GameLocation), nameof(GameLocation.answerDialogueAction)),
			prefix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(GameLocation_answerDialogueAction_NonPassthroughPrefix)))
		);

		harmony.TryPatch(
			monitor: Mod.Monitor,
			original: () => AccessTools.Method(typeof(GameLocation), nameof(GameLocation.answerDialogueAction)),
			prefix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(GameLocation_answerDialogueAction_Prefix))),
			postfix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(GameLocation_answerDialogueAction_Postfix)))
		);

		harmony.TryPatch(
			monitor: Mod.Monitor,
			original: () => AccessTools.Method(typeof(AbigailGame.CowboyMonster), nameof(AbigailGame.CowboyMonster.takeDamage)),
			prefix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(AbigailGame_CowbowMonster_takeDamage_Prefix))),
			postfix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(AbigailGame_CowbowMonster_takeDamage_Postfix)))
		);

		harmony.TryPatch(
			monitor: Mod.Monitor,
			original: () => AccessTools.Method(typeof(AbigailGame.CowboyMonster), nameof(AbigailGame.CowboyMonster.spikeyEndBehavior)),
			postfix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(AbigailGame_CowbowMonster_spikeyEndBehavior_Postfix)))
		);

		harmony.TryPatch(
			monitor: Mod.Monitor,
			original: () => AccessTools.Method(typeof(AbigailGame), nameof(AbigailGame.getPowerUp)),
			prefix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(AbigailGame_getPowerUp_Prefix))),
			postfix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(AbigailGame_getPowerUp_Postfix)))
		);

		harmony.TryPatch(
			monitor: Mod.Monitor,
			original: () => AccessTools.Method(typeof(AbigailGame), nameof(AbigailGame.usePowerup)),
			prefix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(AbigailGame_usePowerup_Prefix))),
			postfix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(AbigailGame_usePowerup_Postfix)))
		);

		harmony.TryPatch(
			monitor: Mod.Monitor,
			original: () => AccessTools.Method(typeof(MineCart), nameof(MineCart.CollectCoin)),
			postfix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(MineCart_CollectCoin_Postfix)))
		);

		harmony.TryPatch(
			monitor: Mod.Monitor,
			original: () => AccessTools.Method(typeof(MineCart), nameof(MineCart.tick)),
			prefix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(MineCart_tick_Prefix))),
			postfix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(MineCart_tick_Postfix)))
		);
	}

	private void OnDayStarted(object? sender, DayStartedEventArgs e)
	{
		QiCoinsLeftToSell.Value = 0;
	}

	private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
	{
		if (!Context.IsPlayerFree)
			return;
		if (Game1.currentMinigame is not null)
			return;

		var money = (int)Math.Floor(ArcadeMoneyToReceive.Value);
		if (money <= 0)
			return;

		Kokoro.Kokoro.Instance.QueueObjectDialogue(Mod.Helper.Translation.Get($"{I18nPrefix}.arcadeMoneyReward", new { Money = money }));
		Game1.player.Money += money;
		ArcadeMoneyToReceive.Value = 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static int GetMoneyForQiCoins(int qiCoins)
		=> (int)Math.Floor(qiCoins * Mod.Config.CompetitionQiCoinSellValue);

	private static IEnumerable<int> InfiniteEnumerable(int start)
	{
		int value = start;
		while (true)
			yield return value++;
	}

	private static int GetMinimumQiCoinsToSell()
		=> InfiniteEnumerable(1).First(qiCoins => GetMoneyForQiCoins(qiCoins) > 0);

	private static bool GameLocation_performAction_Prefix(GameLocation __instance, string action)
	{
		if (!Mod.IsAffixActive(a => a is CompetitionAffix))
			return true;

		if (action == "BuyQiCoins")
		{
			if (RunOriginalBuyQiCoins.Value)
			{
				RunOriginalBuyQiCoins.Value = false;
				return true;
			}

			Response buyResponse = new("BuyQiCoins", Mod.Helper.Translation.Get($"{Instance.I18nPrefix}.buySellQiCoinsQuestion.buy"));
			Response sellResponse = new("SellQiCoins", Mod.Helper.Translation.Get($"{Instance.I18nPrefix}.buySellQiCoinsQuestion.sell"));
			__instance.createQuestionDialogue(Mod.Helper.Translation.Get($"{Instance.I18nPrefix}.buySellQiCoinsQuestion"), new Response[] { buyResponse, sellResponse }, $"{Instance.UniqueID}.BuySellQiCoins");
			return false;
		}

		return true;
	}

	private static bool GameLocation_answerDialogueAction_NonPassthroughPrefix(GameLocation __instance, string questionAndAnswer)
	{
		if (!Mod.IsAffixActive(a => a is CompetitionAffix))
			return true;

		if (questionAndAnswer == $"{Instance.UniqueID}.BuySellQiCoins_BuyQiCoins")
		{
			var tileLocation = Game1.player.getTileLocation();
			RunOriginalBuyQiCoins.Value = true;
			__instance.performAction("BuyQiCoins", Game1.player, new((int)tileLocation.X, (int)tileLocation.Y));
		}
		else if (questionAndAnswer == $"{Instance.UniqueID}.BuySellQiCoins_SellQiCoins")
		{
			int minCoins = GetMinimumQiCoinsToSell();
			if (Game1.player.clubCoins < minCoins)
			{
				Kokoro.Kokoro.Instance.QueueObjectDialogue(Mod.Helper.Translation.Get($"{Instance.I18nPrefix}.sellQiCoins.notEnoughQiCoins"));
				return false;
			}
			if (QiCoinsLeftToSell.Value < minCoins)
			{
				Kokoro.Kokoro.Instance.QueueObjectDialogue(Mod.Helper.Translation.Get($"{Instance.I18nPrefix}.sellQiCoins.cappedQiCoins"));
				return false;
			}

			int coinsToSell = Math.Min(Game1.player.clubCoins, QiCoinsLeftToSell.Value);
			int money = GetMoneyForQiCoins(coinsToSell);
			__instance.createQuestionDialogue(Mod.Helper.Translation.Get($"{Instance.I18nPrefix}.sellQiCoinsQuestion", new { Amount = coinsToSell, Money = money }), __instance.createYesNoResponses(), $"{Mod.ModManifest.UniqueID}.{ShortID}.SellQiCoins");
			return false;
		}
		else if (questionAndAnswer == $"{Instance.UniqueID}.SellQiCoins_Yes")
		{
			int coinsToSell = Math.Min(Game1.player.clubCoins, QiCoinsLeftToSell.Value);
			int money = GetMoneyForQiCoins(coinsToSell);
			Game1.player.clubCoins -= coinsToSell;
			Game1.player.Money += money;
			QiCoinsLeftToSell.Value = 0;
			return false;
		}

		return true;
	}

	private static void GameLocation_answerDialogueAction_Prefix(string questionAndAnswer, ref int __state)
	{
		if (questionAndAnswer != "BuyQiCoins_Yes")
			return;
		if (!Mod.IsAffixActive(a => a is CompetitionAffix))
			return;
		__state = Game1.player.clubCoins;
	}

	private static void GameLocation_answerDialogueAction_Postfix(string questionAndAnswer, ref int __state)
	{
		if (questionAndAnswer != "BuyQiCoins_Yes")
			return;
		if (!Mod.IsAffixActive(a => a is CompetitionAffix))
			return;

		int coinsBought = Game1.player.clubCoins - __state;
		if (coinsBought > 0)
			QiCoinsLeftToSell.Value += (int)Math.Round(coinsBought * Mod.Config.CompetitionQiCoinExchangeCap);
	}

	private static void AbigailGame_CowbowMonster_takeDamage_Prefix(AbigailGame.CowboyMonster __instance)
	{
		if (AbigailGame.playingWithAbigail)
			return;
		if (!Mod.IsAffixActive(a => a is CompetitionAffix))
			return;
		if (!PrairieKingMonsterMaxHealth.TryGetValue(__instance, out _))
			PrairieKingMonsterMaxHealth.AddOrUpdate(__instance, new(__instance.health));
	}

	private static void AbigailGame_CowbowMonster_takeDamage_Postfix(AbigailGame.CowboyMonster __instance)
	{
		if (AbigailGame.playingWithAbigail)
			return;
		if (!Mod.IsAffixActive(a => a is CompetitionAffix))
			return;
		if (__instance.health > 0)
			return;
		if (!PrairieKingMonsterMaxHealth.TryGetValue(__instance, out var maxHealthRef))
			return;
		ArcadeMoneyToReceive.Value += maxHealthRef.Value * Mod.Config.CompetitionPrairieKingMonsterHealthMoney;
	}

	private static void AbigailGame_CowbowMonster_spikeyEndBehavior_Postfix(AbigailGame.CowboyMonster __instance)
	{
		if (AbigailGame.playingWithAbigail)
			return;
		if (!Mod.IsAffixActive(a => a is CompetitionAffix))
			return;
		if (__instance.health > 0)
			return;
		if (PrairieKingMonsterMaxHealth.TryGetValue(__instance, out var maxHealthRef))
			maxHealthRef.Value += 5;
	}

	private static void AbigailGame_getPowerUp_Prefix(AbigailGame __instance, ref int __state)
	{
		if (AbigailGame.playingWithAbigail)
			return;
		if (!Mod.IsAffixActive(a => a is CompetitionAffix))
			return;
		__state = __instance.coins;
	}

	private static void AbigailGame_getPowerUp_Postfix(AbigailGame __instance, ref int __state)
	{
		if (AbigailGame.playingWithAbigail)
			return;
		if (!Mod.IsAffixActive(a => a is CompetitionAffix))
			return;

		int coinsGained = __instance.coins - __state;
		if (coinsGained <= 0)
			return;

		ArcadeMoneyToReceive.Value += coinsGained * Mod.Config.CompetitionPrairieKingCoinMoney;
	}

	private static void AbigailGame_usePowerup_Prefix(AbigailGame __instance, ref int __state)
	{
		if (AbigailGame.playingWithAbigail)
			return;
		if (!Mod.IsAffixActive(a => a is CompetitionAffix))
			return;
		__state = __instance.coins;
	}

	private static void AbigailGame_usePowerup_Postfix(AbigailGame __instance, ref int __state)
	{
		if (AbigailGame.playingWithAbigail)
			return;
		if (!Mod.IsAffixActive(a => a is CompetitionAffix))
			return;

		int coinsGained = __instance.coins - __state;
		if (coinsGained <= 0)
			return;

		ArcadeMoneyToReceive.Value += coinsGained * Mod.Config.CompetitionPrairieKingCoinMoney;
	}

	private static void MineCart_CollectCoin_Postfix(MineCart __instance, int amount)
	{
		if (!Mod.IsAffixActive(a => a is CompetitionAffix))
			return;
		if (MineCartGameModeGetter.Value(__instance) != MineCart.progressMode)
			return;

		ArcadeMoneyToReceive.Value += amount * Mod.Config.CompetitionJunimoKartCoinMoney;
	}

	private static void MineCart_tick_Prefix(MineCart __instance, ref int __state)
	{
		if (!Mod.IsAffixActive(a => a is CompetitionAffix))
			return;
		if (MineCartGameModeGetter.Value(__instance) != MineCart.infiniteMode)
			return;
		__state = MineCartScoreGetter.Value(__instance);
	}

	private static void MineCart_tick_Postfix(MineCart __instance, ref int __state)
	{
		if (!Mod.IsAffixActive(a => a is CompetitionAffix))
			return;
		if (MineCartGameModeGetter.Value(__instance) != MineCart.infiniteMode)
			return;

		int scoreDifference = MineCartScoreGetter.Value(__instance) - __state;
		if (scoreDifference <= 0)
			return;

		ArcadeMoneyToReceive.Value += scoreDifference * Mod.Config.CompetitionJunimoKartScoreMoney;
	}
}