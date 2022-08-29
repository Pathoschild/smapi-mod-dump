/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using MailFrameworkMod;
using Shockah.CommonModCode;
using Shockah.CommonModCode.Stardew;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.MailPersistenceFramework
{
	public class MailPersistenceFramework : Mod, IMailPersistenceFrameworkApi
	{
		private static readonly string PersistedMailsSaveDataKey = "PersistedMails";
		private static readonly string NewMailMessage = "NewMail";
		private static readonly string ReadMailMessage = "ReadMail";
		private static readonly string MailListMessage = "MailList";

		private readonly IList<MailData<Item>> Mails = new List<MailData<Item>>();
		private readonly IList<ModOverrideEntry> Overrides = new List<ModOverrideEntry>();

		public override void Entry(IModHelper helper)
		{
			helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
			helper.Events.GameLoop.Saving += OnSaving;
			helper.Events.Multiplayer.PeerConnected += OnPeerConnected;
			helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
		}

		public override object GetApi()
			=> this;

		private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
		{
			if (GameExt.GetMultiplayerMode() != MultiplayerMode.Client)
			{
				Mails.Clear();
				var serializedMails = Helper.Data.ReadSaveData<List<MailData<string>>>(PersistedMailsSaveDataKey) ?? new List<MailData<string>>();
				foreach (var serializedMail in serializedMails)
					Mails.Add(serializedMail.Deserialize());
			}
			UpdateMails();
		}

		private void OnSaving(object? sender, SavingEventArgs e)
		{
			if (GameExt.GetMultiplayerMode() != MultiplayerMode.Client)
			{
				var serializedMails = new List<MailData<string>>(Mails.Select(m => m.Serialize()));
				Helper.Data.WriteSaveData(PersistedMailsSaveDataKey, serializedMails);
			}
			UpdateMails();
		}

		private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
		{
			if (GameExt.GetMultiplayerMode() != MultiplayerMode.Server)
				return;
			if (e.Peer.GetMod(ModManifest.UniqueID) is null)
				return;

			Helper.Multiplayer.SendMessage(
				Mails.Select(m => m.Serialize()).ToList(),
				MailListMessage,
				new[] { ModManifest.UniqueID },
				new[] { e.Peer.PlayerID }
			);
		}

		private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
		{
			if (e.FromModID != ModManifest.UniqueID)
				return;

			if (e.Type == MailListMessage)
			{
				Mails.Clear();
				var serializedMails = e.ReadAs<List<MailData<string>>>();
				foreach (var serializedMail in serializedMails)
					Mails.Add(serializedMail.Deserialize());
				UpdateMails();
			}
			else if (e.Type == NewMailMessage)
			{
				var serializedMail = e.ReadAs<MailData<string>>();
				Mails.Add(serializedMail.Deserialize());

				if (GameExt.GetMultiplayerMode() == MultiplayerMode.Server)
				{
					Helper.Multiplayer.SendMessage(
						serializedMail,
						NewMailMessage,
						new[] { ModManifest.UniqueID },
						Game1.getOnlineFarmers()
							.Select(p => p.UniqueMultiplayerID)
							.Where(id => id != GameExt.GetHostPlayer().UniqueMultiplayerID && id != e.FromPlayerID)
							.ToArray()
					);
				}

				if (serializedMail.AddresseeID == Game1.player.UniqueMultiplayerID)
					UpdateMails();
			}
			else if (e.Type == ReadMailMessage)
			{
				var (modUniqueID, mailID) = e.ReadAs<(string, string)>();
				var mail = Mails.FirstOrDefault(m => m.ModUniqueID == modUniqueID && m.ID == mailID);
				if (mail is not null && mail.Title is null)
				{
					Mails.Remove(mail);

					if (GameExt.GetMultiplayerMode() == MultiplayerMode.Server)
					{
						Helper.Multiplayer.SendMessage(
							(modUniqueID, mailID),
							ReadMailMessage,
							new[] { ModManifest.UniqueID },
							Game1.getOnlineFarmers()
								.Select(p => p.UniqueMultiplayerID)
								.Where(id => id != GameExt.GetHostPlayer().UniqueMultiplayerID && id != e.FromPlayerID)
								.ToArray()
						);
					}

					if (mail.AddresseeID == Game1.player.UniqueMultiplayerID)
						UpdateMails();
				}
			}
			else
			{
				Monitor.Log($"Received unknown message of type {e.Type}.", LogLevel.Trace);
			}
		}

		private void UpdateMails()
		{
			foreach (var mail in Mails)
			{
				if (mail.AddresseeID != Game1.player.UniqueMultiplayerID)
					continue;

				string letterID = $"{nameof(MailPersistenceFramework)}_{mail.ModUniqueID}_{mail.ID}";
				var letter = MailDao.FindLetter(letterID);
				var isExistingLetter = letter is not null;
				if (letter is not null && mail.Title is null)
					MailDao.RemoveLetter(letter);

				string? mailTitle = mail.Title;
				foreach (var @override in Overrides)
					@override.Title?.Invoke(
						mail.ModUniqueID, mail.ID,
						mailTitle,
						v =>
						{
							if (mailTitle is null && v is not null)
								throw new InvalidOperationException("Cannot add a title to a mail that didn't start with a title to begin with.");
							mailTitle = v;
						}
					);

				string mailText = mail.Text;
				foreach (var @override in Overrides)
					@override.Text?.Invoke(
						mail.ModUniqueID, mail.ID,
						mailText,
						v => mailText = v
					);

				IReadOnlyList<Item> mailItems = mail.Items;
				foreach (var @override in Overrides)
					@override.Items?.Invoke(
						mail.ModUniqueID, mail.ID,
						mailItems,
						v => mailItems = new List<Item>(v)
					);

				string? mailRecipe = mail.Recipe;
				foreach (var @override in Overrides)
					@override.Recipe?.Invoke(
						mail.ModUniqueID, mail.ID,
						mailRecipe,
						v => mailRecipe = v
					);

				MailBackground mailBackground = mail.Background;
				foreach (var @override in Overrides)
					@override.Background?.Invoke(
						mail.ModUniqueID, mail.ID,
						(int)mailBackground,
						v => mailBackground = (MailBackground)v
					);

				MailTextColor? mailTextColor = mail.TextColor;
				foreach (var @override in Overrides)
					@override.TextColor?.Invoke(
						mail.ModUniqueID, mail.ID,
						mailTextColor.HasValue ? (int)mailTextColor.Value : null,
						v => mailTextColor = v.HasValue ? (MailTextColor)v : null
					);

				if (letter is null)
				{
					letter = new(
						id: letterID,
						text: mailText,
						recipe: mailRecipe,
						condition: l => Game1.player.UniqueMultiplayerID == mail.AddresseeID && !Game1.player.mailReceived.Contains(l.Id),
						callback: l => { },
						whichBG: (int)mailBackground
					);
				}

				letter.DynamicItems = l => new List<Item>(mail.Items);
				if (mailTitle is not null)
					letter.Title = mailTitle;
				if (mailTextColor is not null)
					letter.TextColor = (int)mailTextColor.Value;

				letter.Callback = l =>
				{
					if (mailTitle is not null)
					{
						Game1.player.mailReceived.Add(l.Id);
						return;
					}

					Mails.Remove(mail);
					MailDao.RemoveLetter(letter);

					Helper.Multiplayer.SendMessage(
						(mail.ModUniqueID, mail.ID),
						ReadMailMessage,
						new[] { ModManifest.UniqueID },
						(GameExt.GetMultiplayerMode() == MultiplayerMode.Server)
							? Game1.getOnlineFarmers()
								.Select(p => p.UniqueMultiplayerID)
								.Where(id => id != GameExt.GetHostPlayer().UniqueMultiplayerID)
								.ToArray()
							: new[] { GameExt.GetHostPlayer().UniqueMultiplayerID }
					);
				};
				if (!isExistingLetter)
					MailDao.SaveLetter(letter);
			}
		}

		private void SendAllMailsToEveryone()
		{
			if (GameExt.GetMultiplayerMode() != MultiplayerMode.Server)
				return;

			Helper.Multiplayer.SendMessage(
				Mails.Select(m => m.Serialize()).ToList(),
				MailListMessage,
				new[] { ModManifest.UniqueID },
				Game1.getOnlineFarmers()
					.Select(p => p.UniqueMultiplayerID)
					.Where(id => id != GameExt.GetHostPlayer().UniqueMultiplayerID)
					.ToArray()
			);
		}

		public void RegisterMailAttributeOverrides(IManifest mod, IReadOnlyDictionary<int /* MailApiAttribute */, Delegate> overrides)
		{
			var indexToRemove = Overrides.FirstIndex(o => o.Mod == mod);
			if (indexToRemove is not null)
				Overrides.RemoveAt(indexToRemove.Value);
			Overrides.Add(ModOverrideEntry.FromDictionary(mod, overrides));
		}

		public string SendMail(IManifest mod, Farmer addressee, IReadOnlyDictionary<int /* MailApiAttribute */, object?> attributes)
		{
			attributes.TryGetValue((int)MailApiAttribute.Tags, out var tags);

			if (!attributes.TryGetValue((int)MailApiAttribute.Text, out var rawText) || rawText is null)
				rawText = "";
			string text = (string)rawText;

			attributes.TryGetValue((int)MailApiAttribute.Title, out var rawTitle);
			string? title = rawTitle as string;

			if (!attributes.TryGetValue((int)MailApiAttribute.Items, out var rawItems) || rawItems is null)
				rawItems = Enumerable.Empty<Item>();
			IReadOnlyList<Item> items;
			if (rawItems is IEnumerable<Item> enumerableItems)
				items = new List<Item>(enumerableItems);
			else if (rawItems is Item item)
				items = new List<Item> { item };
			else
				throw new ArgumentException($"Invalid type {rawItems.GetType().Name} of attribute {nameof(MailApiAttribute.Items)}.");

			attributes.TryGetValue((int)MailApiAttribute.Recipe, out var rawRecipe);
			string? recipe = (string?)rawRecipe;

			attributes.TryGetValue((int)MailApiAttribute.Background, out var rawBackground);
			MailBackground background = MailBackground.Classic;
			if (rawBackground is not null)
				background = rawBackground is MailBackground background1 ? background1 : (MailBackground)(int)rawBackground!;

			attributes.TryGetValue((int)MailApiAttribute.TextColor, out var rawTextColor);
			MailTextColor? textColor = null;
			if (rawTextColor is not null)
				textColor = rawTextColor is MailTextColor textColor1 ? textColor1 : (MailTextColor)(int)rawTextColor!;

			string mailID = $"{Guid.NewGuid()}";
			MailData<Item> mail = new(
				mod.UniqueID, mailID,
				Game1.Date,
				addressee.UniqueMultiplayerID,
				(IReadOnlyDictionary<string, string>)(tags is null ? new Dictionary<string, string>() : ObjectTokens.Extract(tags)),
				title, text,
				items, recipe,
				background, textColor
			);

			if (Game1.player.UniqueMultiplayerID == addressee.UniqueMultiplayerID || GameExt.GetMultiplayerMode() == MultiplayerMode.Server)
				Mails.Add(mail);

			if (GameExt.GetMultiplayerMode() != MultiplayerMode.SinglePlayer)
			{
				var messageTarget = (GameExt.GetMultiplayerMode() == MultiplayerMode.Server ? addressee : GameExt.GetHostPlayer()).UniqueMultiplayerID;
				Helper.Multiplayer.SendMessage(
					mail.Serialize(),
					NewMailMessage,
					new[] { ModManifest.UniqueID },
					(GameExt.GetMultiplayerMode() == MultiplayerMode.Server)
						? Game1.getOnlineFarmers()
							.Select(p => p.UniqueMultiplayerID)
							.Where(id => id != GameExt.GetHostPlayer().UniqueMultiplayerID)
							.ToArray()
						: new[] { GameExt.GetHostPlayer().UniqueMultiplayerID }
				);
			}
			return mailID;
		}

		public string SendMailToLocalPlayer(IManifest mod, IReadOnlyDictionary<int /* MailApiAttribute */, object?> attributes)
			=> SendMail(mod, Game1.player, attributes);

		public IEnumerable<string> GetMailIDs(string modUniqueID)
			=> Mails.Where(m => m.ModUniqueID == modUniqueID).Select(m => m.ID);

		public bool HasMail(string modUniqueID, string mailID)
			=> Mails.Any(m => m.ModUniqueID == modUniqueID && m.ID == mailID);

		public Farmer GetMailAddressee(string modUniqueID, string mailID)
		{
			var mail = Mails.FirstOrDefault(m => m.ModUniqueID == modUniqueID && m.ID == mailID);
			if (mail is null)
				throw new ArgumentException($"There is no mail with ID {mailID} for mod {modUniqueID}.");
			return Game1.getAllFarmers().First(p => p.UniqueMultiplayerID == mail.AddresseeID);
		}

		public IReadOnlyDictionary<string, string> GetMailTags(string modUniqueID, string mailID)
		{
			var mail = Mails.FirstOrDefault(m => m.ModUniqueID == modUniqueID && m.ID == mailID);
			if (mail is null)
				throw new ArgumentException($"There is no mail with ID {mailID} for mod {modUniqueID}.");
			return mail.Tags;
		}

		public IReadOnlyDictionary<int /* MailApiAttribute */, object?> GetMailAttributes(string modUniqueID, string mailID)
		{
			var mail = Mails.FirstOrDefault(m => m.ModUniqueID == modUniqueID && m.ID == mailID);
			if (mail is null)
				throw new ArgumentException($"There is no mail with ID {mailID} for mod {modUniqueID}.");
			return mail.GetMailApiAttributes();
		}
	}
}
