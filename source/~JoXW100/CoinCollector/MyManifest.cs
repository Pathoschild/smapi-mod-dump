/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CoinCollector
{
	public class MyManifest : IManifest
	{
		public MyManifest(string uniqueID, string name, string author, string description, ISemanticVersion version, string contentPackFor = null)
		{
			Name = name;
			Author = author;
			Description = description;
			Version = version;
			UniqueID = uniqueID;
			UpdateKeys = new string[0];
			ContentPackFor = new MyManifestContentPackFor
			{
				UniqueID = contentPackFor
			};
		}

		[OnDeserialized]
		public void OnDeserialized(StreamingContext context)
		{
			if (Dependencies == null)
			{
				Dependencies = new IManifestDependency[0];
			}
			if (UpdateKeys == null)
			{
				UpdateKeys = new string[0];
			}
		}

		public string Name { get; set; }

		public string Description { get; set; }

		public string Author { get; set; }

		public ISemanticVersion Version { get; set; }

		public ISemanticVersion MinimumApiVersion { get; set; }

		public string UniqueID { get; set; }

		public string EntryDll { get; set; }

		public IManifestContentPackFor ContentPackFor { get; set; }

		public IManifestDependency[] Dependencies { get; set; }

		public string[] UpdateKeys { get; set; }

		public IDictionary<string, object> ExtraFields { get; set; }
	}
}