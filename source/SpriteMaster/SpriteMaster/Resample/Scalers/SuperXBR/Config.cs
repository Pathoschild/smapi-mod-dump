/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Types;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Resample.Scalers.SuperXBR;

sealed class Config : Scalers.Config {
	internal const int MaxScale = 8;

	// default, minimum, maximum, optional step

	internal readonly float EdgeStrength; 
	internal readonly float Weight;
	internal readonly float EdgeShape;
	internal readonly float TextureShape;
	internal readonly float AntiRinging;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Config(
		Vector2B wrapped,
		bool hasAlpha = true,
		float edgeStrength = 2.0f,
		float weight = 1.0f,
		float edgeShape = 0.0f,
		float textureShape = 0.0f,
		float antiRinging = 1.0f
	) : base(
		wrapped: wrapped,
		hasAlpha: hasAlpha
	){
		EdgeStrength = edgeStrength;
		Weight = weight;
		EdgeShape = edgeShape;
		TextureShape = textureShape;
		AntiRinging = antiRinging;
	}
}
