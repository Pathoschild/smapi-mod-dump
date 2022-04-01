/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace Shockah.MailPersistenceFramework
{
	internal class ModOverrideEntry
	{
		public IManifest Mod { get; private set; }
		public Action<string, string, string?, Action<string?>>? Title { get; private set; }
		public Action<string, string, string, Action<string>>? Text { get; private set; }
		public Action<string, string, IReadOnlyList<Item>, Action<IEnumerable<Item>>>? Items { get; private set; }
		public Action<string, string, string?, Action<string?>>? Recipe { get; private set; }
		public Action<string, string, int, Action<int>>? Background { get; private set; }
		public Action<string, string, int?, Action<int?>>? TextColor { get; private set; }

		public ModOverrideEntry(
			IManifest mod,
			Action<string, string, string?, Action<string?>>? title,
			Action<string, string, string, Action<string>>? text,
			Action<string, string, IReadOnlyList<Item>, Action<IEnumerable<Item>>>? items,
			Action<string, string, string?, Action<string?>>? recipe,
			Action<string, string, int, Action<int>>? background,
			Action<string, string, int?, Action<int?>>? textColor
		)
		{
			this.Mod = mod;
			this.Title = title;
			this.Text = text;
			this.Items = items;
			this.Recipe = recipe;
			this.Background = background;
			this.TextColor = textColor;
		}

		public static ModOverrideEntry FromDictionary(IManifest mod, IReadOnlyDictionary<int /* MailApiAttribute */, Delegate> overrides)
			=> new(
				mod,
				overrides.GetValueOrDefault((int)MailApiAttribute.Title) as Action<string, string, string?, Action<string?>>,
				overrides.GetValueOrDefault((int)MailApiAttribute.Text) as Action<string, string, string, Action<string>>,
				overrides.GetValueOrDefault((int)MailApiAttribute.Title) as Action<string, string, IReadOnlyList<Item>, Action<IEnumerable<Item>>>,
				overrides.GetValueOrDefault((int)MailApiAttribute.Recipe) as Action<string, string, string?, Action<string?>>,
				overrides.GetValueOrDefault((int)MailApiAttribute.Background) as Action<string, string, int, Action<int>>,
				overrides.GetValueOrDefault((int)MailApiAttribute.TextColor) as Action<string, string, int?, Action<int?>>
			);
	}
}
