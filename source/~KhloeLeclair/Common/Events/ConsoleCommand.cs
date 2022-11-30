/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;

namespace Leclair.Stardew.Common.Events;

[AttributeUsage(AttributeTargets.Method)]
public class ConsoleCommand : Attribute {

	public string Name { get; }
	public string? Description { get; }
	public ConsoleCommand(string name, string? description = null) {
		Name = name;
		Description = description;
	}

}
