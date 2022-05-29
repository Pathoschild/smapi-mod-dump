/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using JetBrains.Annotations;
using System;

[MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
[AttributeUsage(AttributeTargets.Method)]
internal class CommandAttribute : Attribute {
	internal readonly string Name;
	internal readonly string Description;

	internal CommandAttribute(string name, string description) {
		Name = name;
		Description = description;
	}
}
