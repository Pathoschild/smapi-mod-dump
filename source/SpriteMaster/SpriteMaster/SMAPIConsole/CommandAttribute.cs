/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
class CommandAttribute : Attribute {
	internal readonly string Name;
	internal readonly string Description;

	internal CommandAttribute(string name, string description) {
		Name = name;
		Description = description;
	}
}
