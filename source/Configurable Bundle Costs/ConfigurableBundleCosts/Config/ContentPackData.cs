/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dtomlinson-ga/ConfigurableBundleCosts
**
*************************************************/

// Copyright (C) 2021 Vertigon
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://www.gnu.org/licenses/.

#nullable enable

using ContentPatcher;
using StardewModdingAPI.Utilities;
using System.Collections.Generic;

namespace ConfigurableBundleCosts
{
	/// <summary>
	/// Possible Actions for a Patch to enact.
	/// </summary>
	public enum Actions
	{
		Set,
		Add,
		Subtract,
		Multiply,
		Divide
	}

	/// <summary>
	/// Data for a single Patch.
	/// </summary>
	public class ContentPackItem
	{
		/// <summary>
		/// Patch Name. If not set in the content.json file, determined programmatically by parser.
		/// </summary>
		public string? Name = "";

		/// <summary>
		/// See <cref></cref>
		/// </summary>
		public Actions? Action = null;
		public float? Value = null;
		public string Target = "";
		public string Frequency = "Once";
		public Dictionary<string, string>? Conditions = null;

		/// <summary>
		/// Stores the date this Patch was last applied. Used to determine whether Patch is ready to apply again according to its Frequency.
		/// </summary>
		private SDate? DateApplied;
		public SDate? GetDateApplied()
		{ return DateApplied; }
		public void SetDateApplied(SDate? date)
		{ DateApplied = date; }

		/// <summary>
		/// A reference to the IManagedConditions used by CP's Conditions API. Used to cache values of parsing the raw conditions.
		/// </summary>
		private IManagedConditions? ManagedConditions;
		public IManagedConditions? GetManagedConditions()
		{ return ManagedConditions; }
		public void SetManagedConditions(IManagedConditions? conditions)
		{ ManagedConditions = conditions; }

		/// <summary>
		/// A reference to the ContentPack this Patch comes from.
		/// </summary>
		private ContentPackData? ContentPack;
		public ContentPackData? GetContentPack()
		{ return ContentPack; }
		public void SetContentPack(ContentPackData pack)
		{ ContentPack = pack; }

		public ContentPackItem()
		{ }

		/// <summary>
		/// Copy constructor. Used for splitting patches with multiple targets.
		/// </summary>
		/// <param name="copyPatch">The patch to copy from.</param>
		public ContentPackItem(ContentPackItem copyPatch)
		{
			Name = copyPatch.Name;
			Action = copyPatch.Action;
			Value = copyPatch.Value;
			Target = copyPatch.Target;
			Frequency = copyPatch.Frequency;
			Conditions = copyPatch.Conditions;
			DateApplied = copyPatch.DateApplied;
			ManagedConditions = copyPatch.ManagedConditions;
			ContentPack = copyPatch.ContentPack;
		}

	}

	/// <summary>
	/// Data for a single Content Pack. Contains a set of Default configs which take priority over the mod's config.json, as well as 0 or more Patches.
	/// </summary>
	public class ContentPackData
	{
		/// <summary>
		/// The ContentPack's included config values.
		/// </summary>
		public ContentPackConfig Default = new();

		/// <summary>
		/// The ContentPack's included set of Patches.
		/// </summary>
		public List<ContentPackItem> Patches = new();

		private string FolderName = "";
		public string GetFolderName()
		{ return FolderName; }
		public void SetFolderName(string name)
		{ FolderName = name; }
	}
}
