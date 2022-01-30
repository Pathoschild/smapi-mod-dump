/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster;
using SpriteMaster.Types;
using System.Drawing.Imaging;

namespace xBRZ;

static class PreviewProgram {
	private static Color16[]? SpriteData = null;
	private static Vector2I SpriteSize;

	internal static bool GammaCorrection = false;
	internal static bool AlphaPremultiplication = true;
	internal static uint Scale = 1;
	internal static double LuminanceWeight = Config.Resample.xBRZ.LuminanceWeight;
	internal static double EqualColorTolerance = Config.Resample.xBRZ.EqualColorTolerance;
	internal static double DominantDirectionThreshold = Config.Resample.xBRZ.DominantDirectionThreshold;
	internal static double SteepDirectionThreshold = Config.Resample.xBRZ.SteepDirectionThreshold;
	internal static double CenterDirectionBias = Config.Resample.xBRZ.CenterDirectionBias;

	private static PictureBox PreviewBox = null!;
	private static Bitmap? PreviewBitmap = null;

	internal static int SubMain(string[] args) {
		var actualArgs = args.Where(s => s != "--ui" && s != "--preview");
		var previewFile = actualArgs.FirstOrDefault();
		if (previewFile is not null) {
			var fileData = Common.ReadFile(new Uri(previewFile), out var fileSize);
			SpriteData = Color16.Convert(fileData).ToArray();
			SpriteSize = fileSize;
		}

		Application.EnableVisualStyles();
		Application.Run(new PreviewWindow());
		return 0;
	}

	internal static void OnLoad(PreviewWindow window) {
		if (SpriteData is null) {
			return;
		}

		PreviewBox = (window.Controls.Find("previewBox", true)[0] as PictureBox)!;
		ProcessSprite();
		PreviewBox.Image = PreviewBitmap;
	}

	internal static void OnConfigChanged() {
		ProcessSprite();
		PreviewBox.Image = PreviewBitmap;
	}

	private static unsafe void ProcessSprite() {
		uint scale = Scale;
		var spriteSize = SpriteSize;
		var scaledSize = spriteSize * scale;
		
		var spriteDataArray = SpriteData!.Clone() as Color16[];
		var spriteData = spriteDataArray.AsSpan();
		if (AlphaPremultiplication)
			SpriteMaster.Resample.Passes.PremultipliedAlpha.Reverse(spriteData, spriteSize);
		if (GammaCorrection)
			SpriteMaster.Resample.Passes.GammaCorrection.Linearize(spriteData, spriteSize);

		if (scale != 1) {
			var xBRZInterface = new SpriteMaster.Resample.Scalers.xBRZ.Scaler.ScalerInterface();

			spriteData = xBRZInterface.Apply(
				new SpriteMaster.Resample.Scalers.xBRZ.Config(
					wrapped: Vector2B.False,
					luminanceWeight: LuminanceWeight,
					equalColorTolerance: EqualColorTolerance,
					dominantDirectionThreshold: DominantDirectionThreshold,
					steepDirectionThreshold: SteepDirectionThreshold,
					centerDirectionBias: CenterDirectionBias
				),
				scaleMultiplier: scale,
				sourceData: spriteData,
				sourceSize: spriteSize,
				targetData: null,
				targetSize: scaledSize
			);
			spriteSize = scaledSize;
		}

		if (GammaCorrection)
			SpriteMaster.Resample.Passes.GammaCorrection.Delinearize(spriteData, spriteSize);
		if (AlphaPremultiplication)
			SpriteMaster.Resample.Passes.PremultipliedAlpha.Apply(spriteData, spriteSize);

		var resampledData = Color8.Convert(spriteData);

		Bitmap resampledBitmap;
		fixed (Color8* ptr = resampledData) {
			int stride = spriteSize.Width * sizeof(Color8);
			resampledBitmap = new Bitmap(spriteSize.Width, spriteSize.Height, stride, PixelFormat.Format32bppPArgb, (IntPtr)ptr);
		}

		var oldBitmap = PreviewBitmap;
		PreviewBitmap = resampledBitmap;
		oldBitmap?.Dispose();
	}
}
