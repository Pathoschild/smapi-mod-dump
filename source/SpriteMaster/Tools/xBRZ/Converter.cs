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
using System.IO;

namespace xBRZ;

static class ConverterProgram {
	private record struct Job(Uri Path, int Scale);

	internal static int SubMain(string[] args) {
		bool info = true;

		var jobs = new HashSet<Job>();
		foreach (var arg in args) {
			jobs.Add(
				new(new Uri(arg), 6)
			);
		}

		if (info) {
			Console.WriteLine("Settings:");
			var settings = new[] {
				("LuminanceWeight", Config.Resample.xBRZ.LuminanceWeight),
				("EqualColorTolerance", Config.Resample.xBRZ.EqualColorTolerance),
				("DominantDirectionThreshold", Config.Resample.xBRZ.DominantDirectionThreshold),
				("SteepDirectionThreshold", Config.Resample.xBRZ.SteepDirectionThreshold),
				("CenterDirectionBias", Config.Resample.xBRZ.CenterDirectionBias)
			};

			var maxKeyKength = settings.Select(s => s.Item1.Length).Max();
			foreach (var setting in settings) {
				var key = setting.Item1;
				var value = setting.Item2;

				key = key.PadRight(maxKeyKength);
				Console.WriteLine($"  {key} : {value}");
			}
			Console.WriteLine();
		}

		if (jobs.Count == 0) {
			Console.Error.WriteLine("No files provided!");
			return -1;
		}

		foreach (var job in jobs) {
			try {
				ProcessJob(job);
			}
			catch (Exception ex) {
				Console.Error.WriteLine(ex.ToString());
			}
		}

		return 0;
	}

	private static unsafe void ProcessJob(in Job job) {
		Console.WriteLine($"Processing {job.Path}");

		var imageDataNarrow = Common.ReadFile(job.Path, out var imageSize);

		// Widen
		var imageData = Color16.Convert(imageDataNarrow);
		var originalImageData = imageData;

		// Reverse Alpha-Premultiplication
		SpriteMaster.Resample.Passes.PremultipliedAlpha.Reverse(imageData, imageSize);

		// Linearize
		//SpriteMaster.Resample.Passes.GammaCorrection.Linearize(imageData, imageSize);

		// TODO : padding?
		// Padding?

		var scalerConfig = new SpriteMaster.Resample.Scalers.xBRZ.Config(
			wrapped: Vector2B.False,
			luminanceWeight: Config.Resample.xBRZ.LuminanceWeight,
			equalColorTolerance: Config.Resample.xBRZ.EqualColorTolerance,
			dominantDirectionThreshold: Config.Resample.xBRZ.DominantDirectionThreshold,
			steepDirectionThreshold: Config.Resample.xBRZ.SteepDirectionThreshold,
			centerDirectionBias: Config.Resample.xBRZ.CenterDirectionBias
		);
		uint scale = 6;
		if (scale != 1) {
			var targetSize = imageSize * scale;
			var xBRZInterface = new SpriteMaster.Resample.Scalers.xBRZ.Scaler.ScalerInterface();
			imageData = xBRZInterface.Apply(
				scalerConfig,
				scaleMultiplier: scale,
				sourceData: imageData,
				sourceSize: imageSize,
				targetData: null,
				targetSize: targetSize
			);
			imageSize = targetSize;
		}

		// Delinearize
		//SpriteMaster.Resample.Passes.GammaCorrection.Delinearize(imageData, imageSize);

		// Alpha-Premultiplication
		SpriteMaster.Resample.Passes.PremultipliedAlpha.Apply(imageData, imageSize);

		// Narrow
		var resampledData = Color8.Convert(imageData);

		Bitmap resampledBitmap;
		fixed (Color8* ptr = resampledData) {
			int stride = imageSize.Width * sizeof(Color8);
			resampledBitmap = new Bitmap(imageSize.Width, imageSize.Height, stride, PixelFormat.Format32bppPArgb, (IntPtr)ptr);
		}

		using (resampledBitmap) {
			var path = job.Path.LocalPath;
			var extension = Path.GetExtension(path);
			path = Path.Combine(Path.GetDirectoryName(path)!, Path.GetFileNameWithoutExtension(path));
			path = $"{path}.resampled{extension}";
			resampledBitmap.Save(path, ImageFormat.Png);
		}
	}
}
