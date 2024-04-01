/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

namespace SpriteMaster.Resample.Scalers.EPX;

internal sealed class ScalerInfo : IScalerInfo {
	internal static readonly ScalerInfo Instance = new(Resample.Scaler.EPX);
	internal static readonly ScalerInfo InstanceLegacy = new(Resample.Scaler.EPXLegacy);

	public Resample.Scaler Scaler { get; }
	public int MinScale => 1;
	public int MaxScale => Config.MaxScale;
	public XGraphics.TextureFilter Filter => XGraphics.TextureFilter.Point;
	public bool PremultiplyAlpha => true;
	public bool GammaCorrect => false;
	public bool BlockCompress => false;

	public IScaler Interface => (Scaler is Resample.Scaler.EPX)
		? EPX.Scaler.ScalerInterface.Instance
		: EPX.Scaler.ScalerInterface.InstanceLegacy;

	private ScalerInfo(Resample.Scaler scaler) {
		Scaler = scaler;
	}
}
