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
using System.Runtime.CompilerServices;

namespace SpriteMaster.Types.Exceptions;

internal sealed class ReversePatchException : InvalidOperationException {
	internal ReversePatchException(string message, string member) : base($"Reverse Patch '{member}' : {message}") {
	}

	internal ReversePatchException([CallerMemberName] string member = null!) : base($"Reverse Patch '{member}'") {
	}
}
