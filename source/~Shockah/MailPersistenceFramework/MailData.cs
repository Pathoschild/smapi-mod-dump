/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Newtonsoft.Json;
using Shockah.CommonModCode.Stardew;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Shockah.MailPersistenceFramework
{
	internal class MailData<ItemType>
	{
		public string ModUniqueID { get; set; }
		public string ID { get; set; }
		public int Year { get; set; }
		public int SeasonIndex { get; set; }
		public int DayOfMonth { get; set; }
		public long AddresseeID { get; set; }
		public IReadOnlyDictionary<string, string> Tags { get; set; }
		public string? Title { get; set; }
		public string Text { get; set; }
		public IReadOnlyList<ItemType> Items { get; set; }
		public string? Recipe { get; set; }
		public MailBackground Background { get; set; }
		public MailTextColor? TextColor { get; set; }

		[JsonIgnore]
		public Season Season
			=> (Season)SeasonIndex;

		[JsonIgnore]
		public WorldDate Date
			=> new(Year, Enum.GetName(Season)?.ToLower(), DayOfMonth);

		public MailData(
			string modUniqueID,
			string id,
			WorldDate date,
			long addresseeID,
			IReadOnlyDictionary<string, string> tags,
			string? title,
			string text,
			IReadOnlyList<ItemType> items,
			string? recipe,
			MailBackground background,
			MailTextColor? textColor
		) : this(
			modUniqueID, id,
			date.Year, date.SeasonIndex, date.DayOfMonth,
			addresseeID, tags, title, text,
			items, recipe,
			background, textColor
		)
		{ }

		[JsonConstructor]
		public MailData(
			string modUniqueID,
			string id,
			int year,
			int seasonIndex,
			int dayOfMonth,
			long addresseeID,
			IReadOnlyDictionary<string, string> tags,
			string? title,
			string text,
			IReadOnlyList<ItemType> items,
			string? recipe,
			MailBackground background,
			MailTextColor? textColor
		)
		{
			if (items.Count != 0 && recipe is not null)
				throw new ArgumentException("A mail cannot have both items and a recipe.");
			if (typeof(ItemType) != typeof(string) && typeof(ItemType) != typeof(Item))
				throw new ArgumentException($"{nameof(ItemType)} has to be of type `string` or `{nameof(Item)}`");

			this.ModUniqueID = modUniqueID;
			this.ID = id;
			this.Year = year;
			this.SeasonIndex = seasonIndex;
			this.DayOfMonth = dayOfMonth;
			this.AddresseeID = addresseeID;
			this.Tags = tags;
			this.Title = title;
			this.Text = text;
			this.Items = items;
			this.Recipe = recipe;
			this.Background = background;
			this.TextColor = textColor;
		}

		public IReadOnlyDictionary<int /* MailApiAttribute */, object?> GetMailApiAttributes()
		{
			return new Dictionary<int, object?>
			{
				{ (int)MailApiAttribute.Tags, Tags },
				{ (int)MailApiAttribute.Title, Title },
				{ (int)MailApiAttribute.Text, Text },
				{
					(int)MailApiAttribute.Items,
					typeof(ItemType) == typeof(Item)
						? Items
						: ((IReadOnlyList<string>)Items).Select(si => MailDataExt.DeserializeItem(si)!).ToList()
				},
				{ (int)MailApiAttribute.Recipe, Recipe },
				{ (int)MailApiAttribute.Background, (int)Background },
				{ (int)MailApiAttribute.TextColor, TextColor is null ? null : (int)TextColor }
			};
		}
	}

	internal static class MailDataExt
	{
		private static readonly XmlSerializer ItemSerializer = new(typeof(Item));

		public static MailData<string> Serialize(this MailData<Item> self)
			=> new(
				self.ModUniqueID, self.ID,
				self.Year, self.SeasonIndex, self.DayOfMonth,
				self.AddresseeID, self.Tags, self.Title, self.Text,
				self.Items.Select(i => SerializeItem(i)!).ToList(), self.Recipe,
				self.Background, self.TextColor
			);

		public static MailData<Item> Deserialize(this MailData<string> self)
			=> new(
				self.ModUniqueID, self.ID,
				self.Year, self.SeasonIndex, self.DayOfMonth,
				self.AddresseeID, self.Tags, self.Title, self.Text,
				self.Items.Select(si => DeserializeItem(si)!).ToList(), self.Recipe,
				self.Background, self.TextColor
			);

		[return: NotNullIfNotNull("item")]
		internal static string? SerializeItem(Item? item)
		{
			if (item is null)
				return null;
			StringWriter writer = new();
			ItemSerializer.Serialize(writer, item);
			return writer.ToString();
		}

		[return: NotNullIfNotNull("item")]
		internal static Item? DeserializeItem(string? serializedItem)
		{
			if (serializedItem is null)
				return null;
			return (Item)ItemSerializer.Deserialize(new StringReader(serializedItem))!;
		}
	}
}