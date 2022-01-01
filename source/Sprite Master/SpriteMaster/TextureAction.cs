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

namespace SpriteMaster;
sealed record TextureAction(Action Executor, int Texels) {
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal void Invoke() => Executor.Invoke();
}
