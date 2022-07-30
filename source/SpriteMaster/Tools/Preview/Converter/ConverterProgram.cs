/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using LinqFasterer;
using SpriteMaster.Configuration;
using SpriteMaster.Types;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace SpriteMaster.Tools.Preview.Converter;

internal class ConverterProgram : AbstractProgram {
	private record struct Job(Uri Path, uint Scale);

	internal override async Task<int> OnSubMainAsync(Options options, List<Argument> args) {
		var info = true;

		foreach (var arg in args) {
			if (arg.IsCommand) {
				throw new ArgumentException($"Unknown Argument: {arg}");
			}
		}

		const uint scale = 6;

		var jobs = new HashSet<Job>(options.Paths.SelectF<string, Job>(path => new(new(path), scale)));

		if (info) {
			Console.WriteLine("Settings:");
			var settings = new[] {
				("LuminanceWeight", Config.Resample.Common.LuminanceWeight),
				("EqualColorTolerance", Config.Resample.Common.EqualColorTolerance),
				("DominantDirectionThreshold", Config.Resample.xBRZ.DominantDirectionThreshold),
				("SteepDirectionThreshold", Config.Resample.xBRZ.SteepDirectionThreshold),
				("CenterDirectionBias", Config.Resample.xBRZ.CenterDirectionBias)
			};

			var maxKeyLength = settings.Select(s => s.Item1.Length).Max();
			foreach (var (key, value) in settings) {
				Console.WriteLine($@"  {key.PadRight(maxKeyLength)} : {value}");
			}
			Console.WriteLine();
		}

		if (jobs.Count == 0) {
			Console.Error.WriteLine("No files provided!");
			return -1;
		}

		void HandleException(Exception? ex, string message, StringBuilder? sb = null, int depth = 0) {
			sb ??= new();

			if (ex is not null) {
				sb.AppendLine($"{new(' ', depth * 2)}{message}: {ex}");
			}
			else {
				sb.AppendLine($"{new(' ', depth * 2)}{message}");
			}

			if (ex is AggregateException aggregate) {
				if (aggregate.InnerExceptions.Count != 0) {
					int innerDepth = depth + 1;
					foreach (var innerException in aggregate.InnerExceptions) {
						HandleException(innerException, message, sb, innerDepth);
					}
				}
			}

			if (depth == 0) {
				Console.Error.WriteLine(sb.ToString());
			}
		}

		List<Job> faultedJobs = new();

		var tasks = new Task[jobs.Count];
		int index = 0;
		foreach (var job in jobs) {
			tasks[index++] = Task.Run(() => ProcessJob(job)).ContinueWith(
				(task, j) => {
					var innerJob = (Job)j!;
					HandleException(task.Exception, $"Task Failed ({innerJob.Path})");
					faultedJobs.Add(innerJob);
				}, job, TaskContinuationOptions.OnlyOnFaulted);
		}

		try {
			await Task.WhenAll(tasks);
		}
		catch (Exception ex) {
			HandleException(ex, "Tasks Failed");
		}

		return faultedJobs.Count == 0 ? 0 : -5;
	}

	private static unsafe void ProcessJob(Job job) {
		Console.Out.WriteLine($"Processing {job.Path}");

		var imageDataNarrow = Common.ReadFile(job.Path, out var imageSize);

		var analysis = Resample.Passes.Analysis.AnalyzeLegacy(
			data: imageDataNarrow,
			bounds: imageSize,
			wrapped: Vector2B.False
		);

		// Widen
		var imageData = Color16.Convert(imageDataNarrow);
		var originalImageData = imageData;

		// Reverse Alpha-Premultiplication
		Resample.Passes.PremultipliedAlpha.Reverse(imageData, imageSize, true);

		// Linearize
		//SpriteMaster.Resample.Passes.GammaCorrection.Linearize(imageData, imageSize);

		// TODO : padding?
		// Padding?

		var scalerConfig = new Resample.Scalers.xBRZ.Config(
			wrapped: Vector2B.False,
			luminanceWeight: Config.Resample.Common.LuminanceWeight,
			equalColorTolerance: Config.Resample.Common.EqualColorTolerance,
			dominantDirectionThreshold: Config.Resample.xBRZ.DominantDirectionThreshold,
			steepDirectionThreshold: Config.Resample.xBRZ.SteepDirectionThreshold,
			centerDirectionBias: Config.Resample.xBRZ.CenterDirectionBias
		);
		uint scale = job.Scale;
		if (scale != 1) {
			var targetSize = imageSize * scale;
			var scalerInterface = new Resample.Scalers.xBRZ.Scaler.ScalerInterface();
			imageData = scalerInterface.Apply(
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
		Resample.Passes.PremultipliedAlpha.Apply(imageData, imageSize, true);

		// Narrow
		var resampledData = Color8.Convert(imageData);

		// Remove excess padding


		Bitmap resampledBitmap;
		fixed (Color8* ptr = resampledData) {
			var stride = imageSize.Width * sizeof(Color8);
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
