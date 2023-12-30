/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/myuusubi/SteamDew
**
*************************************************/

using HarmonyLib;
using System;

namespace SteamDew.Patches {

public class Patcher {

public Type DeclaringType = null;
public string Name = null;
public Type[] Parameters = null;

public HarmonyMethod Prefix = null;
public HarmonyMethod Postfix = null;
public HarmonyMethod Transpiler = null;

public void Apply(Harmony harmony)
{
	if (this.Name == null) {
		harmony.Patch(
			original: AccessTools.Constructor(this.DeclaringType, this.Parameters),
			prefix: this.Prefix,
			postfix: this.Postfix,
			transpiler: this.Transpiler
		);
	} else {
		harmony.Patch(
			original: AccessTools.Method(this.DeclaringType, this.Name),
			prefix: this.Prefix,
			postfix: this.Postfix,
			transpiler: this.Transpiler
		);
	}
}

} /* class Patcher */

} /* namespace SteamDew */