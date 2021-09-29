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

using StardewModdingAPI;

namespace ConfigurableBundleCosts
{
	internal class Globals
	{
		public static IManifest Manifest { get; set; }
		public static ModConfig InitialValues { get; set; }
		public static ModConfig CurrentValues { get; set; }
		public static ContentPackConfig Override { get; set; }
		public static IModHelper Helper { get; set; }
		public static IMonitor Monitor { get; set; }
		public static ContentPackHelper PackHelper { get; set; }
	}
}
