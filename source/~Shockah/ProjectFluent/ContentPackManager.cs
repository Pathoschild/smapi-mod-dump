/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Shockah.CommonModCode;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.ProjectFluent
{
	internal interface IContentPackManager
	{
		void RegisterAllContentPacks();
		void RegisterContentPack(IContentPack pack);
	}

	internal class ContentPackManager : IContentPackManager, IContentPackProvider
	{
		public event Action<IContentPackProvider>? ContentPacksContentsChanged;

		private IMonitor Monitor { get; set; }
		private IContentPackHelper ContentPackHelper { get; set; }
		private IContentPackParser ContentPackParser { get; set; }

		private IList<(IContentPack pack, ContentPackContent content)> ContentPackContents { get; set; } = new List<(IContentPack pack, ContentPackContent content)>();

		public ContentPackManager(IMonitor monitor, IContentPackHelper contentPackHelper, IContentPackParser contentPackParser)
		{
			this.Monitor = monitor;
			this.ContentPackHelper = contentPackHelper;
			this.ContentPackParser = contentPackParser;
		}

		public void RegisterAllContentPacks()
		{
			Monitor.Log("Loading content packs...", LogLevel.Info);
			foreach (var pack in ContentPackHelper.GetOwned())
				RegisterContentPack(pack);
		}

		public void RegisterContentPack(IContentPack pack)
		{
			bool changedContentPacks = false;
			try
			{
				Monitor.Log($"Loading content pack `{pack.Manifest.UniqueID}`...", LogLevel.Info);

				(IContentPack pack, ContentPackContent content)? existingEntry = ContentPackContents.FirstOrNull(e => e.pack.Manifest.UniqueID == pack.Manifest.UniqueID);
				if (existingEntry is not null)
				{
					ContentPackContents.Remove(existingEntry.Value);
					changedContentPacks = true;
				}

				if (!pack.HasFile("content.json"))
					return;

				try
				{
					var rawContent = pack.ReadJsonFile<RawContentPackContent>("content.json");
					if (rawContent is null)
						return;

					var parseResult = ContentPackParser.Parse(pack.Manifest, rawContent);
					foreach (var error in parseResult.Errors)
						Monitor.Log($"`{pack.Manifest.UniqueID}`: `content.json`: {error}", LogLevel.Error);
					foreach (var warning in parseResult.Warnings)
						Monitor.Log($"`{pack.Manifest.UniqueID}`: `content.json`: {warning}", LogLevel.Warn);
					if (parseResult.Parsed is not null)
					{
						ContentPackContents.Add((pack: pack, content: parseResult.Parsed));
						changedContentPacks = true;
					}
				}
				catch (Exception ex)
				{
					Monitor.Log($"There was an error while reading `content.json` for the `{pack.Manifest.UniqueID}` content pack:\n{ex}", LogLevel.Error);
				}
			}
			finally
			{
				if (changedContentPacks)
					ContentPacksContentsChanged?.Invoke(this);
			}
		}

		public IEnumerable<ContentPackContent> GetContentPackContents()
			=> ContentPackContents.Select(c => c.content);
	}
}