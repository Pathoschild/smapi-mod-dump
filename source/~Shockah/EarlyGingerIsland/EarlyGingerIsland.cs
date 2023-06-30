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
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using Shockah.CommonModCode.GMCM;
using Shockah.Kokoro;
using Shockah.Kokoro.GMCM;
using Shockah.Kokoro.Stardew;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;

namespace Shockah.EarlyGingerIsland
{
	public class EarlyGingerIsland : BaseMod<ModConfig>
	{
		public static EarlyGingerIsland Instance { get; private set; } = null!;
		private bool IsConfigRegistered { get; set; } = false;
		private UnlockCondition NewUnlockCondition = new();

		private const int BatteryPackID = 787;
		private const int HardwoodID = 709;
		private const int IridiumBarID = 337;

		public override void MigrateConfig(ISemanticVersion? configVersion, ISemanticVersion modVersion)
		{
			// no migration required, for now
		}

		public override void OnEntry(IModHelper helper)
		{
			Instance = this;

			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.GameLoop.DayStarted += OnDayStarted;
			helper.Events.Content.AssetRequested += OnAssetRequested;
			helper.Events.Player.Warped += OnWarped;

			var harmony = new Harmony(ModManifest.UniqueID);
			harmony.TryPatch(
				monitor: Monitor,
				original: () => AccessTools.Method(typeof(BoatTunnel), nameof(BoatTunnel.checkAction)),
				transpiler: new HarmonyMethod(AccessTools.Method(typeof(EarlyGingerIsland), nameof(BoatTunnel_checkAction_Transpiler)), priority: Priority.VeryLow)
			);
			harmony.TryPatch(
				monitor: Monitor,
				original: () => AccessTools.Method(typeof(BoatTunnel), nameof(BoatTunnel.answerDialogue)),
				transpiler: new HarmonyMethod(AccessTools.Method(typeof(EarlyGingerIsland), nameof(BoatTunnel_answerDialogue_Transpiler)), priority: Priority.VeryLow)
			);
			harmony.TryPatch(
				monitor: Monitor,
				original: () => AccessTools.Method(typeof(BoatTunnel), nameof(BoatTunnel.GetTicketPrice)),
				postfix: new HarmonyMethod(AccessTools.Method(typeof(EarlyGingerIsland), nameof(BoatTunnel_GetTicketPrice_Postfix)), priority: Priority.VeryLow)
			);
			harmony.TryPatch(
				monitor: Monitor,
				original: () => AccessTools.Method(typeof(BoatTunnel), nameof(BoatTunnel.StartDeparture)),
				postfix: new HarmonyMethod(AccessTools.Method(typeof(EarlyGingerIsland), nameof(BoatTunnel_StartDeparture_Postfix)))
			);
			harmony.TryPatch(
				monitor: Monitor,
				original: () => AccessTools.Method(typeof(ParrotUpgradePerch), nameof(ParrotUpgradePerch.IsAvailable)),
				postfix: new HarmonyMethod(AccessTools.Method(typeof(EarlyGingerIsland), nameof(ParrotUpgradePerch_IsAvailable_Postfix))),
				transpiler: new HarmonyMethod(AccessTools.Method(typeof(EarlyGingerIsland), nameof(ParrotUpgradePerch_IsAvailable_Transpiler)))
			);
			harmony.TryPatch(
				monitor: Monitor,
				original: () => AccessTools.Method(typeof(HoeDirt), nameof(HoeDirt.canPlantThisSeedHere)),
				postfix: new HarmonyMethod(AccessTools.Method(typeof(EarlyGingerIsland), nameof(HoeDirt_canPlantThisSeedHere_Postfix)))
			);
			harmony.TryPatch(
				monitor: Monitor,
				original: () => AccessTools.Method(typeof(IslandWest), nameof(IslandWest.checkAction)),
				transpiler: new HarmonyMethod(AccessTools.Method(typeof(EarlyGingerIsland), nameof(IslandWest_checkAction_Transpiler)))
			);
		}

		private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
		{
			SetupConfig();
		}

		private void OnDayStarted(object? sender, DayStartedEventArgs e)
		{
			if (ShouldGingerIslandBeUnlocked())
			{
				if (!Game1.player.hasOrWillReceiveMail("willyBackRoomInvitation"))
					Game1.addMail("willyBackRoomInvitation");
				if (Config.BoatFixHardwoodRequired <= 0 && !Game1.player.hasOrWillReceiveMail("willyBoatHull"))
					Game1.addMail("willyBoatHull", noLetter: true);
				if (Config.BoatFixIridiumBarsRequired <= 0 && !Game1.player.hasOrWillReceiveMail("willyBoatAnchor"))
					Game1.addMail("willyBoatAnchor", noLetter: true);
				if (Config.BoatFixBatteryPacksRequired <= 0 && !Game1.player.hasOrWillReceiveMail("willyBoatTicketMachine"))
					Game1.addMail("willyBoatTicketMachine", noLetter: true);
				if (Config.BoatFixHardwoodRequired <= 0 && Config.BoatFixIridiumBarsRequired <= 0 && Config.BoatFixBatteryPacksRequired <= 0 && !Game1.player.hasOrWillReceiveMail("willyBoatFixed"))
					Game1.addMail("willyBoatFixed", noLetter: true);
			}
		}

		private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
		{
			if (e.Name.IsEquivalentTo("Strings/Locations"))
			{
				e.Edit(rawAsset =>
				{
					var asset = rawAsset.AsDictionary<string, string>();
					asset.Data["BoatTunnel_DonateBatteries"] = asset.Data["BoatTunnel_DonateBatteries"].Replace("5", $"{Config.BoatFixBatteryPacksRequired}");
					asset.Data["BoatTunnel_DonateHardwood"] = asset.Data["BoatTunnel_DonateHardwood"].Replace("200", $"{Config.BoatFixHardwoodRequired}");
					asset.Data["BoatTunnel_DonateIridium"] = asset.Data["BoatTunnel_DonateIridium"].Replace("5", $"{Config.BoatFixIridiumBarsRequired}");
					asset.Data["BoatTunnel_DonateBatteriesHint"] = asset.Data["BoatTunnel_DonateBatteriesHint"].Replace("5", $"{Config.BoatFixBatteryPacksRequired}");
					asset.Data["BoatTunnel_DonateHardwoodHint"] = asset.Data["BoatTunnel_DonateHardwoodHint"].Replace("200", $"{Config.BoatFixHardwoodRequired}");
					asset.Data["BoatTunnel_DonateIridiumHint"] = asset.Data["BoatTunnel_DonateIridiumHint"].Replace("5", $"{Config.BoatFixIridiumBarsRequired}");
					asset.Data["qiNutDoor"] = asset.Data["qiNutDoor"].Replace("100", $"{Config.GoldenWalnutsRequiredForQiRoom}");
				});
			}
		}

		private void OnWarped(object? sender, WarpedEventArgs e)
		{
			if (e.NewLocation is IslandLocation islandLocation)
				UpdateParrotUpgradeCosts(islandLocation, islandLocation.parrotUpgradePerches);
		}

		private void SetupConfig()
		{
			var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
			if (api is null)
				return;
			GMCMI18nHelper helper = new(api, ModManifest, Helper.Translation);

			if (IsConfigRegistered)
				api.Unregister(ModManifest);

			api.Register(
				ModManifest,
				reset: () => Config = new ModConfig(),
				save: () =>
				{
					while (Config.UnlockConditions.Count != 0 && Config.UnlockConditions[Config.UnlockConditions.Count - 1].Date.Year < 1)
						Config.UnlockConditions.RemoveAt(Config.UnlockConditions.Count - 1);
					if (NewUnlockCondition.Date.Year >= 1)
					{
						Config.UnlockConditions.Add(NewUnlockCondition);
						NewUnlockCondition = new(WorldDateExt.New(-1, Season.Spring, 1), 0);
					}

					WriteConfig();
					LogConfig();
					Helper.GameContent.InvalidateCache("Strings/Locations");
					SetupConfig();
				}
			);

			helper.AddBoolOption(
				keyPrefix: "config.skipBoatCutscene",
				property: () => Config.SkipBoatCutscene
			);

			helper.AddNumberOption(
				keyPrefix: "config.boatTicketPrice",
				property: () => Config.BoatTicketPrice
			);

			helper.AddBoolOption(
				keyPrefix: "config.allowIslandFarmBeforeCC",
				property: () => Config.AllowIslandFarmBeforeCC
			);

			helper.AddEnumOption(
				keyPrefix: "config.plantingOnIslandFarmBeforeCC",
				property: () => Config.PlantingOnIslandFarmBeforeCC
			);

			helper.AddNumberOption(
				keyPrefix: "config.goldenWalnutsRequiredForQiRoom",
				property: () => Config.GoldenWalnutsRequiredForQiRoom,
				min: 0
			);

			helper.AddSectionTitle("config.boatFix.section");

			helper.AddNumberOption(
				keyPrefix: "config.boatFix.hardwoodRequired",
				property: () => Config.BoatFixHardwoodRequired,
				min: 0
			);

			helper.AddNumberOption(
				keyPrefix: "config.boatFix.iridiumBarsRequired",
				property: () => Config.BoatFixIridiumBarsRequired,
				min: 0
			);

			helper.AddNumberOption(
				keyPrefix: "config.boatFix.batteryPacksRequired",
				property: () => Config.BoatFixBatteryPacksRequired,
				min: 0
			);

			helper.AddSectionTitle("config.unlockCosts.section");

			helper.AddBoolOption(
				keyPrefix: "config.unlockCosts.ignoreFreeUnlockRequirements",
				property: () => Config.IgnoreFreeUnlockRequirements
			);

			helper.AddNumberOption(
				keyPrefix: "config.unlockCosts.firstUnlock",
				property: () => Config.FirstUnlockCost,
				min: 0
			);

			helper.AddNumberOption(
				keyPrefix: "config.unlockCosts.westUnlock",
				property: () => Config.WestUnlockCost,
				min: 0
			);

			helper.AddNumberOption(
				keyPrefix: "config.unlockCosts.farmhouseUnlock",
				property: () => Config.FarmhouseUnlockCost,
				min: 0
			);

			helper.AddNumberOption(
				keyPrefix: "config.unlockCosts.mailboxUnlock",
				property: () => Config.MailboxUnlockCost,
				min: 0
			);

			helper.AddNumberOption(
				keyPrefix: "config.unlockCosts.obeliskUnlock",
				property: () => Config.ObeliskUnlockCost,
				min: 0
			);

			helper.AddNumberOption(
				keyPrefix: "config.unlockCosts.digsiteUnlock",
				property: () => Config.DigsiteUnlockCost,
				min: 0
			);

			helper.AddNumberOption(
				keyPrefix: "config.unlockCosts.traderUnlock",
				property: () => Config.TraderUnlockCost,
				min: 0
			);

			helper.AddNumberOption(
				keyPrefix: "config.unlockCosts.volcanoBridgeUnlock",
				property: () => Config.VolcanoBridgeUnlockCost,
				min: 0
			);

			helper.AddNumberOption(
				keyPrefix: "config.unlockCosts.volcanoExitShortcut",
				property: () => Config.VolcanoExitShortcutUnlockCost,
				min: 0
			);

			helper.AddNumberOption(
				keyPrefix: "config.unlockCosts.resortUnlock",
				property: () => Config.ResortUnlockCost,
				min: 0
			);

			helper.AddNumberOption(
				keyPrefix: "config.unlockCosts.parrotExpressUnlock",
				property: () => Config.ParrotExpressUnlockCost,
				min: 0
			);

			void RegisterUnlockConditionSection(int? index)
			{
				helper.AddSectionTitle("config.unlockConditions.section", new { Number = index is null ? Config.UnlockConditions.Count + 1 : index.Value + 1 });
				helper.AddTextOption(
					keyPrefix: "config.unlockConditions.date",
					getValue: () =>
					{
						var date = (index is null ? NewUnlockCondition : Config.UnlockConditions[index.Value]).Date;
						if (date.Year >= 1)
							return date.ToParsable();
						else
							return "";
					},
					setValue: value =>
					{
						var parsed = WorldDateExt.ParseDate(value) ?? WorldDateExt.New(-1, Season.Spring, 1);
						if (index is null)
							NewUnlockCondition = new(parsed, NewUnlockCondition.HeartsWithWilly);
						else
							Config.UnlockConditions[index.Value] = new(parsed, Config.UnlockConditions[index.Value].HeartsWithWilly);
					}
				);
				helper.AddNumberOption(
					keyPrefix: "config.unlockConditions.heartsWithWilly",
					getValue: () => (index is null ? NewUnlockCondition : Config.UnlockConditions[index.Value]).HeartsWithWilly,
					setValue: value =>
					{
						if (index is null)
							NewUnlockCondition = new(NewUnlockCondition.Date, value);
						else
							Config.UnlockConditions[index.Value] = new(Config.UnlockConditions[index.Value].Date, value);
					},
					min: 0
				);
			}

			for (int i = 0; i < Config.UnlockConditions.Count; i++)
				RegisterUnlockConditionSection(i);
			RegisterUnlockConditionSection(null);

			IsConfigRegistered = true;
		}

		private bool ShouldGingerIslandBeUnlocked()
		{
			if (!Game1.MasterPlayer.mailReceived.Contains("spring_2_1")) // Willy introduction mail
				return false;
			foreach (var condition in Config.UnlockConditions)
			{
				if (Game1.Date.TotalDays < condition.Date.TotalDays)
					continue;
				foreach (var player in Game1.getAllFarmers())
					if (player.getFriendshipHeartLevelForNPC("Willy") < condition.HeartsWithWilly)
						goto outerContinue;
				return true;
				outerContinue:;
			}
			return ShouldGingerIslandBeUnlockedInVanilla();
		}

		private static bool ShouldGingerIslandBeUnlockedInVanilla()
			=> Game1.MasterPlayer.eventsSeen.Contains(191393) || Game1.MasterPlayer.eventsSeen.Contains(502261) || Game1.MasterPlayer.hasCompletedCommunityCenter();

		private bool ShouldAllowPlanting(GameLocation location, IntPoint point)
		{
			if (location is not IslandWest)
				return true;

			switch (Config.PlantingOnIslandFarmBeforeCC)
			{
				case PlantingOnIslandFarmBeforeCC.Disabled:
					return false;
				case PlantingOnIslandFarmBeforeCC.Enabled:
					return true;
				case PlantingOnIslandFarmBeforeCC.OnlyOneCrop:
					for (int y = 0; y < location.Map.DisplayHeight / Game1.tileSize; y++)
						for (int x = 0; x < location.Map.DisplayWidth / Game1.tileSize; x++)
							if (location.terrainFeatures.TryGetValue(new(x, y), out var terrainFeature) && terrainFeature is HoeDirt dirt && dirt.crop is not null)
								return false;
					return true;
				default:
					throw new ArgumentException($"{nameof(PlantingOnIslandFarmBeforeCC)} has an invalid value.");
			}
		}

		private void UpdateParrotUpgradeCosts(GameLocation location, IEnumerable<ParrotUpgradePerch> perches)
		{
			bool changedAny;

			void UpdateParrotUpgradeCost(ParrotUpgradePerch perch)
			{
				void UpdateParrotUpgradeCost(int cost)
				{
					if (perch.requiredNuts.Value != cost)
					{
						perch.requiredNuts.Value = cost;
						changedAny = true;
					}

					if (perch.currentState.Value == ParrotUpgradePerch.UpgradeState.Idle && perch.requiredNuts.Value == 0 && (Config.IgnoreFreeUnlockRequirements || perch.IsAvailable()))
					{
						bool leoCutsceneHack = perch.upgradeName.Value == "Hut" && Game1.player.currentLocation == location;
						if (leoCutsceneHack)
						{
							Game1.globalFade = false;
							Game1.fadeIn = false;
							Game1.fadeToBlack = false;
						}

						perch.ApplyUpgrade();
						perch.UpdateCompletionStatus();

						if (leoCutsceneHack)
							Game1.fadeToBlackAlpha = 1f;

						changedAny = true;
					}
				}

				switch (perch.upgradeName.Value)
				{
					case "Hut":
						UpdateParrotUpgradeCost(Config.FirstUnlockCost);
						break;
					case "Turtle":
						UpdateParrotUpgradeCost(Config.WestUnlockCost);
						break;
					case "Resort":
						UpdateParrotUpgradeCost(Config.ResortUnlockCost);
						break;
					case "Bridge":
						UpdateParrotUpgradeCost(Config.DigsiteUnlockCost);
						break;
					case "Trader":
						UpdateParrotUpgradeCost(Config.TraderUnlockCost);
						break;
					case "House":
						UpdateParrotUpgradeCost(Config.FarmhouseUnlockCost);
						break;
					case "House_Mailbox":
						UpdateParrotUpgradeCost(Config.MailboxUnlockCost);
						break;
					case "Obelisk":
						UpdateParrotUpgradeCost(Config.ObeliskUnlockCost);
						break;
					case "ParrotPlatforms":
						UpdateParrotUpgradeCost(Config.ParrotExpressUnlockCost);
						break;
					case "VolcanoBridge":
						UpdateParrotUpgradeCost(Config.VolcanoBridgeUnlockCost);
						break;
					case "VolcanoShortcutOut":
						UpdateParrotUpgradeCost(Config.VolcanoExitShortcutUnlockCost);
						break;
					default:
						break;
				}
			}

			do
			{
				changedAny = false;
				foreach (var perch in perches)
					UpdateParrotUpgradeCost(perch);
			} while (changedAny);
		}

		[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Used in transpiled code")]
		public static int ModifyTicketPrice(int price)
			=> Instance.Config.BoatTicketPrice;

		private static IEnumerable<CodeInstruction> BoatTunnel_checkAction_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			try
			{
				return new SequenceBlockMatcher<CodeInstruction>(instructions)
					.AsGuidAnchorable()

					// replacing boat ticket price call - the original method gets inlined
					.Do(matcher =>
					{
						return matcher
							.ForEach(
								SequenceMatcherRelativeBounds.WholeSequence,
								new IElementMatch<CodeInstruction>[]
								{
									ILMatches.Ldarg(0),
									ILMatches.Call(AccessTools.Method(typeof(BoatTunnel), nameof(BoatTunnel.GetTicketPrice)))
								},
								matcher =>
								{
									return matcher
										.Insert(
											SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion,
											new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EarlyGingerIsland), nameof(ModifyTicketPrice)))
										);
								},
								minExpectedOccurences: 2,
								maxExpectedOccurences: 2
							);
					})

					// replacing material costs
					.Do(matcher =>
					{
						return matcher
							.Find(
								ILMatches.LdcI4(BatteryPackID),
								ILMatches.LdcI4(5).WithAutoAnchor(out Guid countAnchor),
								ILMatches.LdcI4(0),
								ILMatches.Call(AccessTools.Method(typeof(Farmer), nameof(Farmer.hasItemInInventory)))
							)
							.PointerMatcher(countAnchor)
							.Replace(CodeInstruction.CallClosure<Func<int>>(() => Instance.Config.BoatFixBatteryPacksRequired));
					})
					.Do(matcher =>
					{
						return matcher
							.Find(
								ILMatches.LdcI4(HardwoodID),
								ILMatches.LdcI4(200).WithAutoAnchor(out Guid countAnchor),
								ILMatches.LdcI4(0),
								ILMatches.Call(AccessTools.Method(typeof(Farmer), nameof(Farmer.hasItemInInventory)))
							)
							.PointerMatcher(countAnchor)
							.Replace(CodeInstruction.CallClosure<Func<int>>(() => Instance.Config.BoatFixHardwoodRequired));
					})
					.Do(matcher =>
					{
						return matcher
							.Find(
								ILMatches.LdcI4(IridiumBarID),
								ILMatches.LdcI4(5).WithAutoAnchor(out Guid countAnchor),
								ILMatches.LdcI4(0),
								ILMatches.Call(AccessTools.Method(typeof(Farmer), nameof(Farmer.hasItemInInventory)))
							)
							.PointerMatcher(countAnchor)
							.Replace(CodeInstruction.CallClosure<Func<int>>(() => Instance.Config.BoatFixIridiumBarsRequired));
					})

					.AllElements();
			}
			catch (Exception ex)
			{
				Instance.Monitor.Log($"Could not patch methods - {Instance.ModManifest.Name} probably won't work.\nReason: {ex}", LogLevel.Error);
				return instructions;
			}
		}

		private static IEnumerable<CodeInstruction> BoatTunnel_answerDialogue_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			try
			{
				return new SequenceBlockMatcher<CodeInstruction>(instructions)
					.AsGuidAnchorable()

					// replacing boat ticket price call - the original method gets inlined
					.Do(matcher =>
					{
						return matcher
							.Find(
								ILMatches.Ldarg(0),
								ILMatches.Call(AccessTools.Method(typeof(BoatTunnel), nameof(BoatTunnel.GetTicketPrice)))
							)
							.Insert(
								SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion,
								new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EarlyGingerIsland), nameof(ModifyTicketPrice)))
							);
					})

					// replacing material costs
					.Do(matcher =>
					{
						return matcher
							.Find(
								ILMatches.Call(AccessTools.PropertyGetter(typeof(Game1), nameof(Game1.player))),
								ILMatches.LdcI4(BatteryPackID),
								ILMatches.LdcI4(5).WithAutoAnchor(out Guid countAnchor),
								ILMatches.Call(AccessTools.Method(typeof(Farmer), nameof(Farmer.removeItemsFromInventory)))
							)
							.PointerMatcher(countAnchor)
							.Replace(CodeInstruction.CallClosure<Func<int>>(() => Instance.Config.BoatFixBatteryPacksRequired));
					})
					.Do(matcher =>
					{
						return matcher
							.Find(
								ILMatches.Call(AccessTools.PropertyGetter(typeof(Game1), nameof(Game1.player))),
								ILMatches.LdcI4(HardwoodID),
								ILMatches.LdcI4(200).WithAutoAnchor(out Guid countAnchor),
								ILMatches.Call(AccessTools.Method(typeof(Farmer), nameof(Farmer.removeItemsFromInventory)))
							)
							.PointerMatcher(countAnchor)
							.Replace(CodeInstruction.CallClosure<Func<int>>(() => Instance.Config.BoatFixHardwoodRequired));
					})
					.Do(matcher =>
					{
						return matcher
							.Find(
								ILMatches.Call(AccessTools.PropertyGetter(typeof(Game1), nameof(Game1.player))),
								ILMatches.LdcI4(IridiumBarID),
								ILMatches.LdcI4(5).WithAutoAnchor(out Guid countAnchor),
								ILMatches.Call(AccessTools.Method(typeof(Farmer), nameof(Farmer.removeItemsFromInventory)))
							)
							.PointerMatcher(countAnchor)
							.Replace(CodeInstruction.CallClosure<Func<int>>(() => Instance.Config.BoatFixIridiumBarsRequired));
					})

					.AllElements();
			}
			catch (Exception ex)
			{
				Instance.Monitor.Log($"Could not patch methods - {Instance.ModManifest.Name} probably won't work.\nReason: {ex}", LogLevel.Error);
				return instructions;
			}
		}

		private static void BoatTunnel_GetTicketPrice_Postfix(ref int __result)
		{
			__result = Instance.Config.BoatTicketPrice;
		}

		private static void BoatTunnel_StartDeparture_Postfix()
		{
			if (!Instance.Config.SkipBoatCutscene)
				return;

			if (!Game1.player.hasOrWillReceiveMail("seenBoatJourney"))
				Game1.addMailForTomorrow("seenBoatJourney", noLetter: true);
		}

		private static void ParrotUpgradePerch_IsAvailable_Postfix(ParrotUpgradePerch __instance, ref bool __result)
		{
			if (__instance.upgradeName.Value == "Turtle" && !Instance.Config.AllowIslandFarmBeforeCC && !ShouldGingerIslandBeUnlockedInVanilla())
				__result = false;
		}

		private static IEnumerable<CodeInstruction> ParrotUpgradePerch_IsAvailable_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			try
			{
				return new SequenceBlockMatcher<CodeInstruction>(instructions)
					.Find(
						ILMatches.Ldarg(0),
						ILMatches.Ldfld(AccessTools.Field(typeof(ParrotUpgradePerch), nameof(ParrotUpgradePerch.requiredMail))),
						ILMatches.Call("get_Value"),
						ILMatches.LdcI4(44),
						ILMatches.AnyLdcI4,
						ILMatches.Call("Split"),
						ILMatches.AnyStloc
					)
					.PointerMatcher(SequenceMatcherRelativeElement.Last)
					.CreateLdlocInstruction(out var requiredMailsLdlocInstruction)
					.CreateStlocInstruction(out var requiredMailsStlocInstruction)
					.Advance()
					.Insert(
						SequenceMatcherPastBoundsDirection.Before, SequenceMatcherInsertionResultingBounds.JustInsertion,

						requiredMailsLdlocInstruction,
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EarlyGingerIsland), nameof(ParrotUpgradePerch_IsAvailable_Transpiler_ModifyRequiredMails))),
						requiredMailsStlocInstruction
					)
					.AllElements();
			}
			catch (Exception ex)
			{
				Instance.Monitor.Log($"Could not patch methods - {Instance.ModManifest.Name} probably won't work.\nReason: {ex}", LogLevel.Error);
				return instructions;
			}
		}

		public static string[] ParrotUpgradePerch_IsAvailable_Transpiler_ModifyRequiredMails(string[] requiredMails)
		{
			if (!Instance.Config.AllowIslandFarmBeforeCC)
			{
				for (int i = 0; i < requiredMails.Length; i++)
					if (requiredMails[i] is "Island_Turtle" or "Island_W_Obelisk" or "Island_UpgradeHouse_Mailbox" or "Island_UpgradeHouse" or "Island_UpgradeParrotPlatform")
						requiredMails[i] = "Island_FirstParrot";
			}
			return requiredMails;
		}

		private static void HoeDirt_canPlantThisSeedHere_Postfix(HoeDirt __instance, int tileX, int tileY, bool isFertilizer, ref bool __result)
		{
			if (isFertilizer)
				return;
			if (__instance.crop is not null)
				return;

			if (!Instance.ShouldAllowPlanting(Game1.currentLocation, new(tileX, tileY)))
				__result = false;
		}

		private static IEnumerable<CodeInstruction> IslandWest_checkAction_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
		{
			try
			{
				return new SequenceBlockMatcher<CodeInstruction>(instructions)
					.AsGuidAnchorable()
					.Find(
						ILMatches.Ldloc<int>(originalMethod.GetMethodBody()!.LocalVariables),
						ILMatches.LdcI4(100).WithAutoAnchor(out Guid countAnchor),
						ILMatches.Bge
					)
					.PointerMatcher(countAnchor)
					.Replace(CodeInstruction.CallClosure<Func<int>>(() => Instance.Config.GoldenWalnutsRequiredForQiRoom))
					.AllElements();
			}
			catch (Exception ex)
			{
				Instance.Monitor.Log($"Could not patch methods - {Instance.ModManifest.Name} probably won't work.\nReason: {ex}", LogLevel.Error);
				return instructions;
			}
		}
	}
}