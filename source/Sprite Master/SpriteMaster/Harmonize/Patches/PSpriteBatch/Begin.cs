using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Harmonize.Patches.PSpriteBatch {
	static class Begin {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Harmonize("Begin", fixation: HarmonizeAttribute.Fixation.Postfix, priority: Harmonize.PriorityLevel.Last)]
		internal static void OnBegin (SpriteBatch __instance, SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix transformMatrix) {
			DrawState.OnBegin(
				__instance,
				sortMode,
				blendState ?? BlendState.AlphaBlend,
				samplerState ?? SamplerState.PointClamp,
				depthStencilState ?? DepthStencilState.None,
				rasterizerState ?? RasterizerState.CullCounterClockwise,
				effect,
				transformMatrix
			);
		}
	}
}
