/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

namespace SpriteMaster.Tools.Preview.Preview;

internal sealed class EpxPreviewWindowConfig : PreviewWindowConfig {
	internal EpxPreviewWindowConfig(PreviewWindow window) :
		base(window, ResamplerType.Epx, typeof(Resample.Scalers.EPX.Config)) {

	}

	public override void Dispose() {
		base.Dispose();
	}
}
