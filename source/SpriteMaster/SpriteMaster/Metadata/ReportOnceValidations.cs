/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Metadata;

static class ReportOnceValidations {
	[Conditional("DEBUG"), MethodImpl(Runtime.MethodImpl.Hot)]
	private static void DebugValidate(in Bounds sourceBounds, XTexture2D referenceTexture) {
		Bounds referenceBounds = referenceTexture.Bounds;

		if (!referenceBounds.Contains(sourceBounds)) {
			EmitOverlappingWarning(sourceBounds, referenceTexture);
		}

		if (sourceBounds.Right < sourceBounds.Left || sourceBounds.Bottom < sourceBounds.Top) {
			EmitInvertedWarning(sourceBounds, referenceTexture);
		}

		if (sourceBounds.Degenerate) {
			EmitDegenerateWarning(sourceBounds, referenceTexture);
		}
#if false
		if (source.Left < 0 || source.Top < 0 || source.Right >= reference.Width || source.Bottom >= reference.Height) {
			if (source.Right - reference.Width > 1 || source.Bottom - reference.Height > 1)
				Debug.Warning($"Out of range source '{source}' for texture '{reference.SafeName()}' ({reference.Width}, {reference.Height})");
		}
		if (source.Right < source.Left || source.Bottom < source.Top) {
			Debug.Warning($"Inverted range source '{source}' for texture '{reference.SafeName()}'");
		}
#endif
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void Validate(in Bounds sourceBounds, XTexture2D referenceTexture) {
		DebugValidate(sourceBounds, referenceTexture);
	}

	[Conditional("DEBUG")]
	private static void EmitOverlappingWarning(in Bounds sourceBounds, XTexture2D referenceTexture) {
		if (referenceTexture is not InternalTexture2D && referenceTexture.Meta().ShouldReportError(ReportOnceErrors.OverlappingSource)) {
			Debug.Warning($"Overlapping sprite source '{sourceBounds}' for texture '{referenceTexture.NormalizedName()}' ({referenceTexture.Extent()})");
		}
	}

	[Conditional("DEBUG")]
	private static void EmitInvertedWarning(in Bounds sourceBounds, XTexture2D referenceTexture) {
		if (referenceTexture is not InternalTexture2D && referenceTexture.Meta().ShouldReportError(ReportOnceErrors.InvertedSource)) {
			Debug.Warning($"Inverted sprite source '{sourceBounds}' for texture '{referenceTexture.NormalizedName()}' ({referenceTexture.Extent()})");
		}
	}

	[Conditional("DEBUG")]
	private static void EmitDegenerateWarning(in Bounds sourceBounds, XTexture2D referenceTexture) {
		if (referenceTexture is not InternalTexture2D && referenceTexture.Meta().ShouldReportError(ReportOnceErrors.DegenerateSource)) {
			Debug.Warning($"Degenerate sprite source '{sourceBounds}' for texture '{referenceTexture.NormalizedName()}' ({referenceTexture.Extent()})");
		}
	}
}
