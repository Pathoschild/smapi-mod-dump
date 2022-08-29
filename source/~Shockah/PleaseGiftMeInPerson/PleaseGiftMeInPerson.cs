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
using Shockah.CommonModCode;
using Shockah.CommonModCode.GMCM;
using Shockah.CommonModCode.GMCM.Helper;
using Shockah.CommonModCode.IL;
using Shockah.CommonModCode.SMAPI;
using Shockah.CommonModCode.Stardew;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using SObject = StardewValley.Object;

namespace Shockah.PleaseGiftMeInPerson
{
	public class PleaseGiftMeInPerson : Mod
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
		internal ModConfig Config { get; private set; } = null!;
		private ModConfig.Entry LastDefaultConfigEntry = null!;

		private Farmer? CurrentGiftingPlayer;
		private GiftMethod? CurrentGiftMethod;
		private GiftTaste OriginalGiftTaste;
		private GiftTaste ModifiedGiftTaste;
		private int TicksUntilConfigSetup = 5;
		private Texture2D EmojisTexture = null!;
		internal bool AcceptedInPersonGiftDialogue = false;

		private Lazy<IReadOnlyList<(string name, string displayName)>> Characters = null!;

		private IDictionary<long, IDictionary<string, IList<GiftEntry>>> GiftEntries = new Dictionary<long, IDictionary<string, IList<GiftEntry>>>();
		private readonly IDictionary<long, IList<Item>> ItemsToReturn = new Dictionary<long, IList<Item>>();

		public override void Entry(IModHelper helper)
		{
			Instance = this;
			Config = helper.ReadConfig<ModConfig>();
			LastDefaultConfigEntry = new(Config.Default);
			Helper.Content.AssetLoaders.Add(new OverrideAssetLoader());

			Characters = new(() =>
			{
				var npcDispositions = Game1.content.Load<Dictionary<string, string>>("Data/NPCDispositions");
				var antiSocialNpcs = Helper.ModRegistry.IsLoaded("SuperAardvark.AntiSocial")
					? Game1.content.Load<Dictionary<string, string>>("Data/AntiSocialNPCs")
					: new();

				var characters = npcDispositions
					.Select(c => (name: c.Key, displayName: c.Value.Split('/')[11]))
					.Where(c => !antiSocialNpcs.ContainsKey(c.name))
					.OrderBy(c => c.displayName)
					.ToArray();
				return characters;
			});

			UpdateEmojisTexture();

			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
			helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
			helper.Events.GameLoop.Saving += OnSaving;
			helper.Events.Multiplayer.PeerConnected += OnPeerConnected;
			helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
		}

		private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
		{
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

		private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
		{
			if (--TicksUntilConfigSetup > 0)
				return;

			PopulateConfig(Config);
			SetupConfig();
			Helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
		}

		private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
		{
			if (GameExt.GetMultiplayerMode() != MultiplayerMode.Client)
			{
				GiftEntries = Helper.Data.ReadSaveData<IDictionary<long, IDictionary<string, IList<GiftEntry>>>>(GiftEntriesSaveDataKey)
					?? new Dictionary<long, IDictionary<string, IList<GiftEntry>>>();
			}
		}

		private void OnSaving(object? sender, SavingEventArgs e)
		{
			if (GameExt.GetMultiplayerMode() == MultiplayerMode.Client)
				return;

			CleanUpGiftEntries();
			Helper.Data.WriteSaveData(GiftEntriesSaveDataKey, GiftEntries);
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
					}

					ModConfig copy = new(Config);
					var toRemove = new List<string>();
					foreach (var (npcName, entry) in copy.PerNPC)
						if (entry == copy.Default || entry == LastDefaultConfigEntry)
							toRemove.Add(npcName);

					foreach (var npcName in toRemove)
						copy.PerNPC.Remove(npcName);
					Helper.WriteConfig(copy);
					LastDefaultConfigEntry = new(Config.Default);
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
				pageID: character => $"character_{character.name}",
				pageName: character => character.displayName,
				pageValues: Characters.Value.ToArray()
			);

			helper.AddPage("config.spouse", "spouse");
			SetupConfigEntryMenu(() => Config.Spouse);

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
			var configEntry = player.spouse == npcName ? Config.Spouse : Config.GetForNPC(npcName);
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

		private void ReturnItemIfNeeded(SObject item, string originalAddresseeNpcName, GiftTaste originalGiftTaste, GiftTaste modifiedGiftTaste)
		{
			if ((int)Instance.ModifiedGiftTaste > (int)GiftTaste.Dislike)
				return;

			//var returnItem = Instance.Config.ReturnUnlikedItems switch
			//{
			//	ModConfig.ReturningBehavior.Never => false,
			//	ModConfig.ReturningBehavior.NormallyLiked => (int)originalGiftTaste >= (int)GiftTaste.Neutral,
			//	ModConfig.ReturningBehavior.Always => true,
			//	_ => throw new ArgumentException($"{nameof(ModConfig.ReturningBehavior)} has an invalid value."),
			//};
			//if (!returnItem)
			//	return;

			// TODO: actually send a mail
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

		private static void NPC_tryToReceiveActiveObject_Prefix(NPC __instance, Farmer __0 /* who */)
		{
			Instance.CurrentGiftingPlayer = __0;
			Instance.CurrentGiftMethod = GiftMethod.InPerson;
		}

		private static void NPC_tryToReceiveActiveObject_Postfix(NPC __instance)
		{
			Instance.CurrentGiftingPlayer = null;
			Instance.CurrentGiftMethod = null;
		}

		private static IEnumerable<CodeInstruction> NPC_tryToReceiveActiveObject_Transpiler(IEnumerable<CodeInstruction> enumerableInstructions, ILGenerator il)
		{
			var instructions = enumerableInstructions.ToList();

			// IL to find:
			// IL_1984: ldarg.0
			// IL_1985: ldloc.0
			// IL_1986: ldfld class StardewValley.Farmer StardewValley.NPC/'<>c__DisplayClass231_0'::who
			// IL_198b: callvirt instance class StardewValley.Object StardewValley.Farmer::get_ActiveObject()
			// IL_1990: ldloc.0
			// IL_1991: ldfld class StardewValley.Farmer StardewValley.NPC/'<>c__DisplayClass231_0'::who
			// IL_1996: ldc.i4.1
			// IL_1997: ldc.r4 1
			// IL_199c: ldc.i4.1
			// IL_199d: call instance void StardewValley.NPC::receiveGift(class StardewValley.Object, class StardewValley.Farmer, bool, float32, bool)
			var worker = TranspileWorker.FindInstructions(instructions, new Func<CodeInstruction, bool>[]
			{
				i => i.IsLdarg(0),
				i => i.IsLdloc(),
				i => i.opcode == OpCodes.Ldfld,
				i => i.Calls(AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.ActiveObject))),
				i => i.IsLdloc(),
				i => i.opcode == OpCodes.Ldfld,
				i => i.IsLdcI4(),
				i => i.opcode == OpCodes.Ldc_R4,
				i => i.IsLdcI4(),
				i => i.Calls(AccessTools.Method(typeof(NPC), nameof(NPC.receiveGift)))
			});
			if (worker is null)
			{
				Instance.Monitor.Log($"Could not patch methods - Please Gift Me In Person probably won't work.\nReason: Could not find IL to transpile.", LogLevel.Error);
				return instructions;
			}

			var receiveGiftLabel = il.DefineLabel();
			worker.Prefix(new[]
			{
				new CodeInstruction(OpCodes.Ldarg_0),
				new CodeInstruction(OpCodes.Ldarg_1),
				new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PleaseGiftMeInPerson), nameof(NPC_tryToReceiveActiveObject_Transpiler_ConfirmationDialogueCheck))),
				new CodeInstruction(OpCodes.Brfalse, receiveGiftLabel),
				new CodeInstruction(OpCodes.Ret)
			});
			worker[5].labels.Add(receiveGiftLabel);

			return instructions;
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

		private static void NPC_receiveGift_Postfix(NPC __instance, SObject o, Farmer giver)
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

			if (Instance.CurrentGiftingPlayer == giver)
				Instance.ReturnItemIfNeeded(o, __instance.Name, Instance.OriginalGiftTaste, Instance.ModifiedGiftTaste);
		}

		private static IEnumerable<CodeInstruction> DialogueBox_draw_Transpiler(IEnumerable<CodeInstruction> enumerableInstructions)
		{
			var instructions = enumerableInstructions.ToList();

			// IL to find:
			// IL_013c: ldarg.0
			// IL_013d: ldfld int32 StardewValley.Menus.DialogueBox::x
			// IL_0142: ldc.i4.4
			// IL_0143: add
			// IL_0144: ldloc.0
			// IL_0145: ldc.i4.8
			// IL_0146: sub
			// IL_0147: ldarg.0
			// IL_0148: ldfld int32 StardewValley.Menus.IClickableMenu::width
			// IL_014d: ldc.i4.8
			// IL_014e: sub
			// IL_014f: ldarg.0
			// IL_0150: ldfld class [System.Collections] System.Collections.Generic.List`1<class StardewValley.Response> StardewValley.Menus.DialogueBox::responses
			// IL_0155: ldloc.1
			// IL_0156: callvirt instance !0 class [System.Collections] System.Collections.Generic.List`1<class StardewValley.Response>::get_Item(int32)
			// IL_015b: ldfld string StardewValley.Response::responseText
			// IL_0160: ldarg.0
			// IL_0161: ldfld int32 StardewValley.Menus.IClickableMenu::width
			// IL_0166: call int32 StardewValley.BellsAndWhistles.SpriteText::getHeightOfString(string, int32)
			// IL_016b: ldc.i4.s 16
			// IL_016d: add
			var worker = TranspileWorker.FindInstructions(instructions, new Func<CodeInstruction, bool>[]
			{
				i => i.IsLdarg(0),
				i => i.LoadsField(AccessTools.Field(typeof(DialogueBox), nameof(DialogueBox.x))),
				i => i.IsLdcI4(),
				i => i.opcode == OpCodes.Add,
				i => i.IsLdloc(),
				i => i.IsLdcI4(),
				i => i.opcode == OpCodes.Sub,
				i => i.IsLdarg(0),
				i => i.LoadsField(AccessTools.Field(typeof(IClickableMenu), nameof(IClickableMenu.width))),
				i => i.IsLdcI4(),
				i => i.opcode == OpCodes.Sub,
				i => i.IsLdarg(0),
				i => i.LoadsField(AccessTools.Field(typeof(DialogueBox), nameof(DialogueBox.responses))),
				i => i.IsLdloc(),
				i => i.opcode == OpCodes.Callvirt,
				i => i.LoadsField(AccessTools.Field(typeof(Response), nameof(Response.responseText))),
				i => i.IsLdarg(0),
				i => i.LoadsField(AccessTools.Field(typeof(IClickableMenu), nameof(IClickableMenu.width))),
				i => i.Calls(AccessTools.Method(typeof(SpriteText), nameof(SpriteText.getHeightOfString))),
				i => i.IsLdcI4(),
				i => i.opcode == OpCodes.Add,
			});
			if (worker is null)
			{
				Instance.Monitor.Log($"Could not patch methods - Please Gift Me In Person probably won't work.\nReason: Could not find IL to transpile.", LogLevel.Error);
				return instructions;
			}

			var responseYLocalInstruction = worker[4];
			var iLocalInstruction = worker[13];

			// IL to find (after the previous IL):
			// IL_01d1: call void StardewValley.BellsAndWhistles.SpriteText::drawString(class [MonoGame.Framework]Microsoft.Xna.Framework.Graphics.SpriteBatch, string, int32, int32, int32, int32, int32, float32, float32, bool, int32, string, int32, valuetype StardewValley.BellsAndWhistles.SpriteText/ScrollTextAlignment)
			worker = TranspileWorker.FindInstructions(instructions, new Func<CodeInstruction, bool>[]
			{
				i => i.Calls(AccessTools.Method(typeof(SpriteText), nameof(SpriteText.drawString)))
			}, startIndex: worker.EndIndex);
			if (worker is null)
			{
				Instance.Monitor.Log($"Could not patch methods - Please Gift Me In Person probably won't work.\nReason: Could not find IL to transpile.", LogLevel.Error);
				return instructions;
			}

			worker.Postfix(new[]
			{
				new CodeInstruction(OpCodes.Ldarg_1),
				new CodeInstruction(OpCodes.Ldarg_0),
				new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(DialogueBox), nameof(DialogueBox.responses))),
				iLocalInstruction.ToLoadLocal()!,
				new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(IList<Response>), "get_Item")),
				new CodeInstruction(OpCodes.Ldarg_0),
				new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(DialogueBox), nameof(DialogueBox.x))),
				responseYLocalInstruction.ToLoadLocal()!,
				new CodeInstruction(OpCodes.Ldarg_0),
				new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(IClickableMenu), nameof(IClickableMenu.width))),
				new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PleaseGiftMeInPerson), nameof(DialogueBox_draw_Transpiler_DrawAccessory)))
			});

			return instructions;
		}

		private static void DialogueBox_draw_Transpiler_DrawAccessory(SpriteBatch b, Response response, int x, int y, int width)
		{
			if (!Instance.Config.ShowCurrentMailModifier)
				return;
			if (Game1.currentLocation.lastQuestionKey != "MailServiceMod_GiftShipment")
				return;
			if (!Instance.Characters.Value.Any(c => c.name == response.responseKey))
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
