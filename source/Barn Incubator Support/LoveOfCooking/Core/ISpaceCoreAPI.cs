/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using System;

namespace LoveOfCooking
{
	public interface ISpaceCoreAPI
	{
		/// <summary>
		/// Call after SpaceCore has been loaded.
		/// Must have [XmlType("Mods_SOMETHINGHERE")] attribute (required to start with "Mods_")
		/// </summary>
		void RegisterSerializerType(Type type);
	}
}
