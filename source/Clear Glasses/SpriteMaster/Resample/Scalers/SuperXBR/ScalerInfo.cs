/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

#if !SHIPPING
namespace SpriteMaster.Resample.Scalers.SuperXBR;

sealed class ScalerInfo : IScalerInfo {
	internal static readonly ScalerInfo Instance = new();

	public Resample.Scaler Scaler => Resample.Scaler.SuperXBR;
	public int MinScale => 1;
	public int MaxScale => Config.MaxScale;
	public XGraphics.TextureFilter Filter => XGraphics.TextureFilter.Point;
	public bool PremultiplyAlpha => false;
	public bool GammaCorrect => false;
	public bool BlockCompress => false;

	public IScaler Interface => SuperXBR.Scaler.ScalerInterface.Instance;

	private ScalerInfo() { }
}
#endif
