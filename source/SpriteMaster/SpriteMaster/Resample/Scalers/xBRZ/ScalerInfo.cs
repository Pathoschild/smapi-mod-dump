/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

namespace SpriteMaster.Resample.Scalers.xBRZ;

sealed class ScalerInfo : IScalerInfo {
	internal static readonly ScalerInfo Instance = new();

	public Resample.Scaler Scaler => Resample.Scaler.xBRZ;
	public int MinScale => 1;
	public int MaxScale => Config.MaxScale;
	public XNA.Graphics.TextureFilter Filter => XNA.Graphics.TextureFilter.Linear;
	public bool PremultiplyAlpha => true;
	public bool GammaCorrect => true;
	public bool BlockCompress => true;

	public IScaler Interface => xBRZ.Scaler.ScalerInterface.Instance;

	private ScalerInfo() { }
}
