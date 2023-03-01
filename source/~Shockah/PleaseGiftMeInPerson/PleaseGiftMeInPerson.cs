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
using Microsoft.Xna.Framework.Graphics;
using Nanoray.Shrike.Harmony;
using Nanoray.Shrike;
using Shockah.CommonModCode.GMCM;
using Shockah.CommonModCode.GMCM.Helper;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Shockah.Kokoro;
using Shockah.Kokoro.Stardew;
using Shockah.Kokoro.GMCM;
using Shockah.Kokoro.SMAPI;

namespace Shockah.PleaseGiftMeInPerson
{
	public class PleaseGiftMeInPerson : BaseMod<ModConfig>
	{
		private static readonly string MailServicesMod_GiftShipmentController_QualifiedName = "MailServicesMod.GiftShipmentController, MailServicesMod";

		internal static readonly string OverrideAssetPath = "Data/PleaseGiftMeInPerson";
		private static readonly string GiftEntriesSaveDataKey = "GiftEntries";
		private static readonly string GiftEntriesMessageType = "GiftEntries";

		private static readonly Rectangle CursorsMailSourceRect = new(189, 423, 16, 13);
		private static readonly Rectangle EmojisLoveSourceRect = new(9, 27, 9, 9);
		private static readonly Rectangle EmojisLikeSourceRect = new(0, 0, 9, 9);
		private static readonly Rectangle EmojisDislikeSourceRect = new(99, 0, 9, 9);
		private static readonly Rectangle EmojisHateSourceRect = new(0, 9, 9, 9);

		internal static PleaseGiftMeInPerson Instance { get; set; } = null!;
		private IFreeLoveApi? FreeLoveApi;
		private ModConfig.Entry LastDefaultConfigEntry = null!;

		private Farmer? CurrentGiftingPlayer;
		private GiftMethod? CurrentGiftMethod;
		private GiftTaste OriginalGiftTaste;
		private GiftTaste ModifiedGiftTaste;
		private int TicksUntilConfigSetup = 5;
		private Texture2D EmojisTexture = null!;
		internal bool AcceptedInPersonGiftDialogue = false;

		private Lazy<IReadOnlyList<(string Name, string DisplayName)>> Characters = null!;

		private IDictionary<long, IDictionary<string, IList<GiftEntry>>> GiftEntries = new Dictionary<long, IDictionary<string, IList<GiftEntry>>>();
		private readonly IDictionary<long, IList<Item>> ItemsToReturn = new Dictionary<long, IList<Item>>();

		public override void MigrateConfig(ISemanticVersion? configVersion, ISemanticVersion modVersion)
		{
			if (configVersion is null)
			{
				if (Config.Spouse is not null && Config.Spouse == Config.Default)
					Config.Spouse = null;
			}
		}

		public override void OnEntry(IModHelper helper)
		{
			Instance = this;
			LastDefaultConfigEntry = new(Config.Default);
			if (Config.Spouse is null)
				Config.Spouse = new(Config.Default);

			Characters = new(() =>
			{
				var npcDispositions = Game1.content.Load<Dictionary<string, string>>("Data/NPCDispositions");
				var antiSocialNpcs = Helper.ModRegistry.IsLoaded("SuperAardvark.AntiSocial")
					? Game1.content.Load<Dictionary<string, string>>("Data/AntiSocialNPCs")
					: new();

				var characters = npcDispositions
					.Select(c => (Name: c.Key, DisplayName: c.Value.Split('/').Length >= 12 ? c.Value.Split('/')[11] : null))
					.Where(c => !antiSocialNpcs.ContainsKey(c.Name))
					.ToArray();

				foreach (var (name, displayName) in characters)
					if (displayName is null)
						this.Monitor.Log($"Could not create configuration for character {name}, as its NPCDispositions are malformed.", LogLevel.Warn);

				return characters
					.Where(c => c.DisplayName is not null)
					.Select(c => (Name: c.Name, DisplayName: c.DisplayName!))
					.OrderBy(c => c.DisplayName)
					.ToArray();
			});

			UpdateEmojisTexture();

			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
			helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
			helper.Events.GameLoop.Saving += OnSaving;
			helper.Events.Content.AssetRequested += OnAssetRequested;
			helper.Events.Multiplayer.PeerConnected += OnPeerConnected;
			helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
		}

		/// <inheritdoc cref="IGameLoopEvents.GameLaunched" />
		private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
		{
			FreeLoveApi = Helper.ModRegistry.GetApi<IFreeLoveApi>("aedenthorn.FreeLove");

			var harmony = new Harmony(ModManifest.UniqueID);
			harmony.TryPatch(
				original: () => AccessTools.Method(AccessTools.TypeByName(MailServicesMod_GiftShipmentController_QualifiedName), "GiftToNpc"),
				Monitor,
				prefix: new HarmonyMethod(typeof(PleaseGiftMeInPerson), nameof(GiftShipmentController_GiftToNpc_Prefix)),
				postfix: new HarmonyMethod(typeof(PleaseGiftMeInPerson), nameof(GiftShipmentController_GiftToNpc_Postfix))
			);
			harmony.TryPatch(
				original: () => AccessTools.Method(AccessTools.TypeByName(MailServicesMod_GiftShipmentController_QualifiedName), "CreateResponsePage"),
				Monitor,
				prefix: new HarmonyMethod(typeof(PleaseGiftMeInPerson), nameof(GiftShipmentController_CreateResponsePage_Prefix))
			);
			harmony.TryPatchVirtual(
				original: () => AccessTools.Method(typeof(NPC), nameof(NPC.tryToReceiveActiveObject)),
				Monitor,
				prefix: new HarmonyMethod(typeof(PleaseGiftMeInPerson), nameof(NPC_tryToReceiveActiveObject_Prefix)),
				postfix: new HarmonyMethod(typeof(PleaseGiftMeInPerson), nameof(NPC_tryToReceiveActiveObject_Postfix))
			);
			harmony.TryPatch(
				original: () => AccessTools.Method(typeof(NPC), nameof(NPC.tryToReceiveActiveObject)),
				Monitor,
				transpiler: new HarmonyMethod(typeof(PleaseGiftMeInPerson), nameof(NPC_tryToReceiveActiveObject_Transpiler))
			);
			harmony.TryPatch(
				original: () => AccessTools.Method(typeof(NPC), nameof(NPC.getGiftTasteForThisItem)),
				Monitor,
				postfix: new HarmonyMethod(typeof(PleaseGiftMeInPerson), nameof(NPC_getGiftTasteForThisItem_Postfix))
			);
			harmony.TryPatch(
				original: () => AccessTools.Method(typeof(NPC), nameof(NPC.receiveGift)),
				Monitor,
				postfix: new HarmonyMethod(typeof(PleaseGiftMeInPerson), nameof(NPC_receiveGift_Postfix))
			);
			harmony.TryPatch(
				original: () => AccessTools.Method(typeof(DialogueBox), nameof(DialogueBox.draw), new[] { typeof(SpriteBatch) }),
				Monitor,
				transpiler: new HarmonyMethod(typeof(PleaseGiftMeInPerson), nameof(DialogueBox_draw_Transpiler))
			);
		}

		/// <inheritdoc cref="IGameLoopEvents.UpdateTicked" />
		private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
		{
			if (--TicksUntilConfigSetup > 0)
				return;

			PopulateConfig(Config);
			SetupConfig();
			Helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
		}

		/// <inheritdoc cref="IGameLoopEvents.SaveLoaded" />
		private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
		{
			if (GameExt.GetMultiplayerMode() != MultiplayerMode.Client)
			{
				GiftEntries = Helper.Data.ReadSaveData<IDictionary<long, IDictionary<string, IList<GiftEntry>>>>(GiftEntriesSaveDataKey)
					?? new Dictionary<long, IDictionary<string, IList<GiftEntry>>>();
			}
		}

		/// <inheritdoc cref="IGameLoopEvents.Saving" />
		private void OnSaving(object? sender, SavingEventArgs e)
		{
			if (GameExt.GetMultiplayerMode() == MultiplayerMode.Client)
				return;

			CleanUpGiftEntries();
			Helper.Data.WriteSaveData(GiftEntriesSaveDataKey, GiftEntries);
		}

		/// <inheritdoc cref="IContentEvents.AssetRequested" />
		private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
		{
			if (!e.Name.IsEquivalentTo(OverrideAssetPath))
				return;
			
			e.LoadFrom(() =>
			{
				var asset = new Dictionary<string, string>();
				if (Config.EnableNPCOverrides)
				{
					asset["Dwarf"] = $"{GiftPreference.Neutral}/{GiftPreference.Hates}";
					asset["Elliott"] = $"{GiftPreference.Neutral}/{GiftPreference.Neutral}";
					asset["Krobus"] = $"{GiftPreference.Neutral}/{GiftPreference.Hates}";
					asset["Leo"] = $"{GiftPreference.Neutral}/{GiftPreference.LovesInfrequent}";
					asset["Linus"] = $"{GiftPreference.Neutral}/{GiftPreference.DislikesAndHatesFrequent}";
					asset["Penny"] = $"{GiftPreference.Neutral}/{GiftPreference.Neutral}";
					asset["Sandy"] = $"{GiftPreference.LikesInfrequent}/{GiftPreference.LikesInfrequentButDislikesFrequent}";
					asset["Sebastian"] = $"{GiftPreference.Dislikes}/{GiftPreference.Neutral}";
					asset["Wizard"] = $"{GiftPreference.DislikesFrequent}/{GiftPreference.Neutral}";
				}
				return asset;
			}, AssetLoadPriority.Exclusive);
		}

		private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
		{
			if (GameExt.GetMultiplayerMode() != MultiplayerMode.Server)
				return;
			if (e.Peer.GetMod(ModManifest.UniqueID) is null)
				return;

			Helper.Multiplayer.SendMessage(
				GiftEntries,
				GiftEntriesMessageType,
				new[] { ModManifest.UniqueID },
				new[] { e.Peer.PlayerID }
			);
		}

		private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
		{
			if (e.FromModID != ModManifest.UniqueID)
				return;

			if (e.Type == GiftEntriesMessageType)
			{
				var message = e.ReadAs<Dictionary<long, IDictionary<string, IList<GiftEntry>>>>();
				GiftEntries = message;
			}
			else if (e.Type == typeof(NetMessage.RecordGift).FullName)
			{
				var message = e.ReadAs<NetMessage.RecordGift>();
				var player = Game1.getAllFarmers().First(p => p.UniqueMultiplayerID == message.PlayerID);
				RecordGiftEntryForNPC(player, message.NpcName, message.GiftEntry);
			}
			else
			{
				Monitor.Log($"Received unknown message of type {e.Type}.", LogLevel.Warn);
			}
		}

		private void PopulateConfig(ModConfig config)
		{
			foreach (var (name, _) in Characters.Value)
				if (!config.PerNPC.ContainsKey(name))
					config.PerNPC[name] = new(Config.Default);
		}

		private void SetupConfig()
		{
			var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
			if (api is null)
				return;
			GMCMI18nHelper helper = new(api, ModManifest, Helper.Translation);

			api.Register(
				ModManifest,
				reset: () =>
				{
					Config = new();
					PopulateConfig(Config);
					LastDefaultConfigEntry = new(Config.Default);
				},
				save: () =>
				{
					if (Config.Default != LastDefaultConfigEntry)
					{
						foreach (var (_, entry) in Config.PerNPC)
							if (entry == LastDefaultConfigEntry)
								entry.CopyFrom(Config.Default);
						if (Config.Spouse is not null && Config.Spouse == LastDefaultConfigEntry)
							Config.Spouse.CopyFrom(Config.Default);
					}

					ModConfig copy = new(Config);

					var toRemove = new List<string>();
					foreach (var (npcName, entry) in copy.PerNPC)
						if (entry == copy.Default || entry == LastDefaultConfigEntry)
							toRemove.Add(npcName);
					foreach (var npcName in toRemove)
						copy.PerNPC.Remove(npcName);

					if (copy.Spouse is not null && (copy.Spouse == copy.Default || copy.Spouse == LastDefaultConfigEntry))
						copy.Spouse = null;

					WriteConfig(copy);
					LastDefaultConfigEntry = new(Config.Default);

					LogConfig(copy);
				}
			);

			helper.AddSectionTitle("config.section.general");
			helper.AddBoolOption("config.enableNpcOverrides", () => Config.EnableNPCOverrides);
			helper.AddBoolOption("config.confirmUnlikedInPersonGifts", () => Config.ConfirmUnlikedInPersonGifts);
			helper.AddBoolOption("config.showCurrentMailModifier", () => Config.ShowCurrentMailModifier);

			void SetupConfigEntryMenu(Func<ModConfig.Entry> entry)
			{
				helper.AddSectionTitle("config.section.npcPreferences");
				helper.AddEnumOption(
					keyPrefix: "config.inPersonPreference",
					valuePrefix: "config.giftPreference",
					getValue: () => entry().InPersonPreference,
					setValue: v => entry().InPersonPreference = v
				);
				helper.AddEnumOption(
					keyPrefix: "config.byMailPreference",
					valuePrefix: "config.giftPreference",
					getValue: () => entry().ByMailPreference,
					setValue: v => entry().ByMailPreference = v
				);
				helper.AddNumberOption("config.infrequentGiftPercent", () => entry().InfrequentGiftPercent, v => entry().InfrequentGiftPercent = v, min: 0f, max: 1f, interval: 0.01f);
				helper.AddNumberOption("config.frequentGiftPercent", () => entry().FrequentGiftPercent, v => entry().FrequentGiftPercent = v, min: 0f, max: 1f, interval: 0.01f);

				helper.AddSectionTitle("config.section.npc");
				helper.AddBoolOption("config.enableModOverrides", () => entry().EnableModOverrides, v => entry().EnableModOverrides = v);
				helper.AddNumberOption("config.giftsToRemember", () => entry().GiftsToRemember, v => entry().GiftsToRemember = v, min: 0);
				helper.AddNumberOption("config.daysToRemember", () => entry().DaysToRemember, v => entry().DaysToRemember = v, min: 0);
			}

			SetupConfigEntryMenu(() => Config.Default);

			helper.AddSectionTitle("config.section.overrides");
			helper.AddPageLink("spouse", "config.spouse");

			helper.AddMultiPageLinkOption(
				keyPrefix: "config.npcOverrides",
				columns: _ => 3,
				pageID: character => $"character_{character.Name}",
				pageName: character => character.DisplayName,
				pageValues: Characters.Value.ToArray()
			);

			helper.AddPage("config.spouse", "spouse");
			SetupConfigEntryMenu(() => Config.Spouse!);

			foreach (var (name, displayName) in Characters.Value)
			{
				api.AddPage(ModManifest, $"character_{name}", () => displayName);
				SetupConfigEntryMenu(() => Config.PerNPC[name]);
			}
		}

		private void UpdateEmojisTexture()
		{
			EmojisTexture = Game1.content.Load<Texture2D>("LooseSprites\\emojis");
		}

		private void CleanUpGiftEntries()
		{
			WorldDate newDate = new(Game1.Date);
			foreach (var (playerID, allGiftEntries) in GiftEntries)
			{
				foreach (var (npcName, giftEntries) in allGiftEntries)
				{
					var configEntry = Config.GetForNPC(npcName);
					var toRemove = new HashSet<GiftEntry>();
					toRemove.UnionWith(giftEntries.Where(e => newDate.TotalDays - e.Date.TotalDays > configEntry.DaysToRemember));
					toRemove.UnionWith(giftEntries.Take(Math.Max(giftEntries.Count - configEntry.GiftsToRemember, 0)));
					foreach (var entry in toRemove)
						giftEntries.Remove(entry);
				}
			}

			if (GameExt.GetMultiplayerMode() == MultiplayerMode.Server)
			{
				Helper.Multiplayer.SendMessage(
				GiftEntries,
				GiftEntriesMessageType,
				new[] { ModManifest.UniqueID },
				Game1.getOnlineFarmers()
					.Where(p => p.UniqueMultiplayerID != GameExt.GetHostPlayer().UniqueMultiplayerID)
					.Select(p => p.UniqueMultiplayerID)
					.ToArray()
				);
			}
		}

		private IEnumerable<GiftEntry> GetGiftEntriesForNPC(Farmer player, string npcName)
		{
			if (GiftEntries.TryGetValue(player.UniqueMultiplayerID, out var allGiftEntries))
				if (allGiftEntries.TryGetValue(npcName, out var giftEntries))
					return giftEntries;
			return Enumerable.Empty<GiftEntry>();
		}

		private void RecordGiftEntryForNPC(Farmer player, string npcName, GiftEntry giftEntry)
		{
			Monitor.Log($"{GameExt.GetMultiplayerMode()} {player.Name} gifted {giftEntry.GiftTaste} {giftEntry.GiftMethod} to {npcName}", LogLevel.Trace);
			if (!GiftEntries.TryGetValue(player.UniqueMultiplayerID, out var allGiftEntries))
			{
				allGiftEntries = new Dictionary<string, IList<GiftEntry>>();
				GiftEntries[player.UniqueMultiplayerID] = allGiftEntries;
			}
			if (!allGiftEntries.TryGetValue(npcName, out var giftEntries))
			{
				giftEntries = new List<GiftEntry>();
				allGiftEntries[npcName] = giftEntries;
			}
			giftEntries.Add(giftEntry);

			if (GameExt.GetMultiplayerMode() != MultiplayerMode.SinglePlayer)
			{
				long[] playerIDsToSendTo;
				if (GameExt.GetMultiplayerMode() == MultiplayerMode.Server)
					playerIDsToSendTo = Game1.getOnlineFarmers()
						.Where(p => p != player && p.UniqueMultiplayerID != GameExt.GetHostPlayer().UniqueMultiplayerID)
						.Select(p => p.UniqueMultiplayerID)
						.ToArray();
				else
					playerIDsToSendTo = GameExt.GetHostPlayer() == player ? Array.Empty<long>() : new[] { GameExt.GetHostPlayer().UniqueMultiplayerID };

				if (playerIDsToSendTo.Length != 0)
				{
					Instance.Helper.Multiplayer.SendMessage(
						new NetMessage.RecordGift(player.UniqueMultiplayerID, npcName, giftEntry),
						new[] { Instance.ModManifest.UniqueID },
						playerIDsToSendTo
					);
				}
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0018:Inline variable declaration", Justification = "Better semi-repeated code")]
		private GiftTaste GetGiftTasteModifier(Farmer player, string npcName, GiftMethod method)
		{
			var giftEntries = GetGiftEntriesForNPC(player, npcName);
			var viaMail = giftEntries.Count(e => e.GiftMethod == GiftMethod.ByMail);
			var configEntry = IsSpouse(player, npcName) ? Config.Spouse! : Config.GetForNPC(npcName);
			if (player.spouse != npcName && configEntry.EnableModOverrides && configEntry.HasSameValues(LastDefaultConfigEntry))
			{
				var asset = Game1.content.Load<Dictionary<string, string>>(OverrideAssetPath);
				if (asset.TryGetValue(npcName, out var line))
				{
					var split = line.Split('/');
					configEntry = new(configEntry);
					GiftPreference parsedGiftPreference;
					float parsedFloat;

					if (split.Length > 0 && Enum.TryParse(split[0].Trim(), true, out parsedGiftPreference))
						configEntry.InPersonPreference = parsedGiftPreference;
					if (split.Length > 1 && Enum.TryParse(split[1].Trim(), true, out parsedGiftPreference))
						configEntry.ByMailPreference = parsedGiftPreference;
					if (split.Length > 2 && float.TryParse(split[2].Trim(), out parsedFloat))
						configEntry.InfrequentGiftPercent = parsedFloat;
					if (split.Length > 3 && float.TryParse(split[3].Trim(), out parsedFloat))
						configEntry.FrequentGiftPercent = parsedFloat;
				}
			}

			float sameMethodPercent = 1f * giftEntries.Count(e => e.GiftMethod == method) / configEntry.GiftsToRemember;
			var preference = method switch
			{
				GiftMethod.InPerson => configEntry.InPersonPreference,
				GiftMethod.ByMail => configEntry.ByMailPreference,
				_ => throw new ArgumentException($"{nameof(GiftMethod)} has an invalid value."),
			};

			switch (preference)
			{
				case GiftPreference.Hates:
					return GiftTaste.Hate;
				case GiftPreference.HatesFrequent:
					if (sameMethodPercent >= configEntry.FrequentGiftPercent)
						return GiftTaste.Hate;
					else if (sameMethodPercent >= configEntry.InfrequentGiftPercent)
						return GiftTaste.Dislike;
					else
						return GiftTaste.Neutral;
				case GiftPreference.DislikesAndHatesFrequent:
					if (sameMethodPercent >= configEntry.FrequentGiftPercent)
						return GiftTaste.Hate;
					else
						return GiftTaste.Dislike;
				case GiftPreference.Dislikes:
					return GiftTaste.Dislike;
				case GiftPreference.DislikesFrequent:
					if (sameMethodPercent >= configEntry.FrequentGiftPercent)
						return GiftTaste.Dislike;
					else
						return GiftTaste.Neutral;
				case GiftPreference.Neutral:
					return GiftTaste.Neutral;
				case GiftPreference.LikesInfrequentButDislikesFrequent:
					if (sameMethodPercent >= configEntry.FrequentGiftPercent)
						return GiftTaste.Dislike;
					else if (sameMethodPercent >= configEntry.InfrequentGiftPercent)
						return GiftTaste.Neutral;
					else
						return GiftTaste.Like;
				case GiftPreference.LikesInfrequent:
					if (sameMethodPercent < configEntry.InfrequentGiftPercent)
						return GiftTaste.Like;
					else
						return GiftTaste.Neutral;
				case GiftPreference.Likes:
					return GiftTaste.Like;
				case GiftPreference.LovesInfrequent:
					if (sameMethodPercent < configEntry.InfrequentGiftPercent)
						return GiftTaste.Love;
					else if (sameMethodPercent < configEntry.FrequentGiftPercent)
						return GiftTaste.Like;
					else
						return GiftTaste.Neutral;
				case GiftPreference.LikesAndLovesInfrequent:
					if (sameMethodPercent < configEntry.InfrequentGiftPercent)
						return GiftTaste.Love;
					else
						return GiftTaste.Like;
				case GiftPreference.Loves:
					return GiftTaste.Love;
				default:
					throw new ArgumentException($"{nameof(GiftPreference)} has an invalid value.");
			}
		}

		private bool IsSpouse(Farmer farmer, string npcName)
		{
			if (FreeLoveApi is null)
				return farmer.spouse == npcName;
			else
				return FreeLoveApi.GetSpouses(farmer).ContainsKey(npcName);
		}

		private static void GiftShipmentController_GiftToNpc_Prefix()
		{
			Instance.CurrentGiftingPlayer = Game1.player;
			Instance.CurrentGiftMethod = GiftMethod.ByMail;
		}

		private static void GiftShipmentController_GiftToNpc_Postfix()
		{
			Instance.CurrentGiftingPlayer = null;
			Instance.CurrentGiftMethod = null;
		}

		private static void GiftShipmentController_CreateResponsePage_Prefix()
		{
			Instance.UpdateEmojisTexture();
		}

		private static void NPC_tryToReceiveActiveObject_Prefix(Farmer __0 /* who */)
		{
			Instance.CurrentGiftingPlayer = __0;
			Instance.CurrentGiftMethod = GiftMethod.InPerson;
		}

		private static void NPC_tryToReceiveActiveObject_Postfix()
		{
			Instance.CurrentGiftingPlayer = null;
			Instance.CurrentGiftMethod = null;
		}

		private static IEnumerable<CodeInstruction> NPC_tryToReceiveActiveObject_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
		{
			try
			{
				return new SequenceBlockMatcher<CodeInstruction>(instructions)
					.Find(
						ILMatches.Ldarg(0),
						ILMatches.AnyLdloc,
						ILMatches.Ldfld("who"),
						ILMatches.Call(AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.ActiveObject))),
						ILMatches.AnyLdloc,
						ILMatches.Ldfld("who"),
						ILMatches.AnyLdcI4,
						ILMatches.Instruction(OpCodes.Ldc_R4),
						ILMatches.AnyLdcI4,
						ILMatches.Call(AccessTools.Method(typeof(NPC), nameof(NPC.receiveGift)))
					)
					.PointerMatcher(SequenceMatcherRelativeElement.First)
					.ExtractLabels(out var findHeadLabels)
					.CreateLabel(il, out var receiveGiftLabel)
					.Insert(
						SequenceMatcherPastBoundsDirection.Before, true,

						new CodeInstruction(OpCodes.Ldarg_0),
						new CodeInstruction(OpCodes.Ldarg_1),
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PleaseGiftMeInPerson), nameof(NPC_tryToReceiveActiveObject_Transpiler_ConfirmationDialogueCheck))),
						new CodeInstruction(OpCodes.Brfalse, receiveGiftLabel),
						new CodeInstruction(OpCodes.Ret)
					)
					.PointerMatcher(SequenceMatcherRelativeElement.First)
					.AddLabels(findHeadLabels)
					.AllElements();
			}
			catch (Exception ex)
			{
				Instance.Monitor.Log($"Could not patch methods - {Instance.ModManifest.Name} probably won't work.\nReason: {ex}", LogLevel.Error);
				return instructions;
			}
		}

		public static bool NPC_tryToReceiveActiveObject_Transpiler_ConfirmationDialogueCheck(NPC __instance, Farmer who)
		{
			if (!Instance.Config.ConfirmUnlikedInPersonGifts)
				return false;
			if (Instance.AcceptedInPersonGiftDialogue)
			{
				Instance.AcceptedInPersonGiftDialogue = false;
				return false;
			}
			var giftTasteModifier = Instance.GetGiftTasteModifier(who, __instance.Name, GiftMethod.InPerson);
			if ((int)giftTasteModifier >= 0)
				return false;

			var questionTranslationKey = giftTasteModifier switch
			{
				GiftTaste.Dislike => "inPersonGift.question.dislike",
				GiftTaste.Hate => "inPersonGift.question.hate",
				_ => throw new InvalidOperationException($"{nameof(GiftTaste)} has an invalid value."),
			};
			who.currentLocation.createQuestionDialogue(
				Instance.Helper.Translation.Get(questionTranslationKey, new { Name = __instance.displayName }),
				who.currentLocation.createYesNoResponses(), (who, answer) =>
				{
					if (answer == "Yes")
					{
						Instance.AcceptedInPersonGiftDialogue = true;
						__instance.tryToReceiveActiveObject(who);
					}
				}
			);
			return true;
		}

		private static void NPC_getGiftTasteForThisItem_Postfix(NPC __instance, ref int __result)
		{
			if (Instance.CurrentGiftingPlayer is null || Instance.CurrentGiftMethod is null)
				return;

			Instance.OriginalGiftTaste = GiftTasteExt.From(__result);
			__result = Instance.OriginalGiftTaste
				.GetModified((int)Instance.GetGiftTasteModifier(Instance.CurrentGiftingPlayer, __instance.Name, Instance.CurrentGiftMethod.Value))
				.GetStardewValue();
			Instance.ModifiedGiftTaste = GiftTasteExt.From(__result);
		}

		private static void NPC_receiveGift_Postfix(NPC __instance, Farmer giver)
		{
			if (Instance.CurrentGiftMethod is null)
				return;

			Instance.RecordGiftEntryForNPC(
				giver,
				__instance.Name,
				new(
					new WorldDate(Game1.Date),
					Instance.OriginalGiftTaste,
					Instance.CurrentGiftMethod.Value
				)
			);
		}

		private static IEnumerable<CodeInstruction> DialogueBox_draw_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			try
			{
				return new SequenceBlockMatcher<CodeInstruction>(instructions)
					.AsAnchorable<CodeInstruction, Guid, Guid, SequencePointerMatcher<CodeInstruction>, SequenceBlockMatcher<CodeInstruction>>()
					.Find(
						ILMatches.Ldarg(0),
						ILMatches.Ldfld(AccessTools.Field(typeof(DialogueBox), nameof(DialogueBox.x))),
						ILMatches.AnyLdcI4,
						ILMatches.Instruction(OpCodes.Add),
						ILMatches.AnyLdloc.WithAutoAnchor(out Guid responseYLocalInstruction),
						ILMatches.AnyLdcI4,
						ILMatches.Instruction(OpCodes.Sub),
						ILMatches.Ldarg(0),
						ILMatches.Ldfld(AccessTools.Field(typeof(IClickableMenu), nameof(IClickableMenu.width))),
						ILMatches.AnyLdcI4,
						ILMatches.Instruction(OpCodes.Sub),
						ILMatches.Ldarg(0),
						ILMatches.Ldfld(AccessTools.Field(typeof(DialogueBox), nameof(DialogueBox.responses))),
						ILMatches.AnyLdloc.WithAutoAnchor(out Guid iLocalInstruction),
						ILMatches.AnyCall,
						ILMatches.Ldfld(AccessTools.Field(typeof(Response), nameof(Response.responseText))),
						ILMatches.Ldarg(0),
						ILMatches.Ldfld(AccessTools.Field(typeof(IClickableMenu), nameof(IClickableMenu.width))),
						ILMatches.Call(AccessTools.Method(typeof(SpriteText), nameof(SpriteText.getHeightOfString))),
						ILMatches.AnyLdcI4,
						ILMatches.Instruction(OpCodes.Add)
					)
					.AnchorBlock(out Guid findBlock)
					.MoveToPointerAnchor(responseYLocalInstruction)
					.CreateLdlocInstruction(out var responseYLoadInstruction)
					.MoveToPointerAnchor(iLocalInstruction)
					.CreateLdlocInstruction(out var iLoadInstruction)
					.MoveToBlockAnchor(findBlock)
					.Insert(
						SequenceMatcherPastBoundsDirection.After, true,

						new CodeInstruction(OpCodes.Ldarg_1),
						new CodeInstruction(OpCodes.Ldarg_0),
						new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(DialogueBox), nameof(DialogueBox.responses))),
						iLoadInstruction,
						new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(IList<Response>), "get_Item")),
						new CodeInstruction(OpCodes.Ldarg_0),
						new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(DialogueBox), nameof(DialogueBox.x))),
						responseYLoadInstruction,
						new CodeInstruction(OpCodes.Ldarg_0),
						new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(IClickableMenu), nameof(IClickableMenu.width))),
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PleaseGiftMeInPerson), nameof(DialogueBox_draw_Transpiler_DrawAccessory)))
					)
					.AllElements();
			}
			catch (Exception ex)
			{
				Instance.Monitor.Log($"Could not patch methods - {Instance.ModManifest.Name} probably won't work.\nReason: {ex}", LogLevel.Error);
				return instructions;
			}
		}

		private static void DialogueBox_draw_Transpiler_DrawAccessory(SpriteBatch b, Response response, int x, int y, int width)
		{
			if (!Instance.Config.ShowCurrentMailModifier)
				return;
			if (Game1.currentLocation.lastQuestionKey != "MailServiceMod_GiftShipment")
				return;
			if (!Instance.Characters.Value.Any(c => c.Name == response.responseKey))
				return;

			int height = SpriteText.getHeightOfString(response.responseText, width) + 16;
			float scale = 4f;

			b.Draw(
				Game1.mouseCursors,
				new Vector2(x + width - 8, y + height / 2f),
				CursorsMailSourceRect, Color.White,
				0f, new Vector2(CursorsMailSourceRect.Width, CursorsMailSourceRect.Height / 2f), scale,
				SpriteEffects.None, 1f
			);

			Texture2D tasteTexture;
			Rectangle tasteSourceRect;
			switch (Instance.GetGiftTasteModifier(Game1.player, response.responseKey, GiftMethod.ByMail))
			{
				case GiftTaste.Love:
					tasteTexture = Instance.EmojisTexture;
					tasteSourceRect = EmojisLoveSourceRect;
					break;
				case GiftTaste.Like:
					tasteTexture = Instance.EmojisTexture;
					tasteSourceRect = EmojisLikeSourceRect;
					break;
				case GiftTaste.Dislike:
					tasteTexture = Instance.EmojisTexture;
					tasteSourceRect = EmojisDislikeSourceRect;
					break;
				case GiftTaste.Hate:
					tasteTexture = Instance.EmojisTexture;
					tasteSourceRect = EmojisHateSourceRect;
					break;
				default:
					return;
			}

			b.Draw(
				tasteTexture,
				new Vector2(x + width - 8, y + height / 2f - (CursorsMailSourceRect.Height - 3) * scale),
				tasteSourceRect, Color.White,
				0f, new Vector2(tasteSourceRect.Width, 0f), scale,
				SpriteEffects.None, 1f
			);
		}
	}
}
