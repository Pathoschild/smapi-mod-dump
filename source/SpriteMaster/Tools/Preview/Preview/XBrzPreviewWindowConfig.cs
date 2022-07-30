/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

namespace SpriteMaster.Tools.Preview.Preview;

internal sealed class XBrzPreviewWindowConfig : PreviewWindowConfig {
	internal XBrzPreviewWindowConfig(PreviewWindow window) :
		base(window, ResamplerType.XBrz, typeof(Resample.Scalers.xBRZ.Config)) {

	}

	public override void Dispose() {
		base.Dispose();
	}
}
