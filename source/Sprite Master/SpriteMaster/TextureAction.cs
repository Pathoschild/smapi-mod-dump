using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster {
	internal sealed class TextureAction {
		private readonly Action Executor;
		public readonly int Texels;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal TextureAction(Action executor, int texels) {
			Executor = executor;
			Texels = texels;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Invoke() {
			Executor.Invoke();
		}
	}
}
