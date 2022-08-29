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
using Shockah.CommonModCode.Stardew;
using StardewValley;
using System;
using System.Linq;

namespace Shockah.MachineStatus
{
	internal readonly struct LocationDescriptor : IEquatable<LocationDescriptor>
	{
		public readonly string Name { get; }
		public readonly string TypeName { get; }
		public readonly string MapPath { get; }

		public LocationDescriptor(string name, string typeName, string mapPath)
		{
			this.Name = name;
			this.TypeName = typeName;
			this.MapPath = mapPath;
		}

		private static string GetNameForLocation(GameLocation location)
		{
			var selfName = location.NameOrUniqueName ?? "";
			var rootName = location.Root?.Value?.NameOrUniqueName ?? "";
			if (selfName == rootName)
				return selfName;
			else if (selfName == "" && rootName == "")
				return "";
			else if (selfName == "" && rootName != "")
				return $"<unknown> @ {rootName}";
			else if (selfName != "" && rootName == "")
				return selfName;
			else
				return $"{selfName} @ {rootName}";
		}

		public static LocationDescriptor Create(GameLocation location)
			=> new(GetNameForLocation(location), location.GetType().GetBestName(), location.mapPath.Value ?? "");

		public bool Matches(GameLocation location)
			=> Name == GetNameForLocation(location) && TypeName == location.GetType().GetBestName() && MapPath == (location.mapPath.Value ?? "");

		public GameLocation? Retrieve()
		{
			var self = this;
			return GameExt.GetAllLocations().FirstOrDefault(l => self.Matches(l));
		}

		public override string ToString()
			=> Name;

		public bool Equals(LocationDescriptor other)
			=> Name == other.Name && TypeName == other.TypeName && MapPath == other.MapPath;

		public override bool Equals(object? obj)
			=> obj is LocationDescriptor descriptor && Equals(descriptor);

		public override int GetHashCode()
			=> (Name, TypeName, MapPath).GetHashCode();

		public static bool operator ==(LocationDescriptor lhs, LocationDescriptor rhs)
			=> lhs.Equals(rhs);

		public static bool operator !=(LocationDescriptor lhs, LocationDescriptor rhs)
			=> !lhs.Equals(rhs);
	}
}
