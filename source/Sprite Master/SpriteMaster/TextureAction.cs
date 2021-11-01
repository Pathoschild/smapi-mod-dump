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

namespace SpriteMaster {
	internal sealed class TextureAction {
		private readonly Action Executor;
		public readonly int Texels;

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		internal TextureAction(Action executor, int texels) {
			Executor = executor;
			Texels = texels;
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		internal void Invoke() {
			Executor.Invoke();
		}
	}
}
