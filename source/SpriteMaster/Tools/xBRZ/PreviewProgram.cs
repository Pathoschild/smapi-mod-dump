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
using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using SpriteMaster.Resample.Scalers;
using SpriteMaster.Types;
using System.Drawing.Imaging;
using System.IO;
using static SpriteMaster.Configuration.Config;
using Path = System.IO.Path;

namespace xBRZ;

enum ResamplerType {
	XBrz = 0,
	Epx = 1
}

internal static class PreviewProgram {
	private static Color16[]? SpriteData = null;
	private static string? CurrentPath = null;
	private static Vector2I SpriteSize;
	private static ResamplerType Resampler = ResamplerType.XBrz;

	internal static bool GammaCorrection = false;
	internal static bool AlphaPremultiplication = true;
	internal static uint Scale = 1;
	internal static double LuminanceWeight = SpriteMaster.Configuration.Config.Resample.Common.LuminanceWeight;
	internal static uint EqualColorTolerance = (uint)SpriteMaster.Configuration.Config.Resample.Common.EqualColorTolerance;
	internal static double DominantDirectionThreshold = SpriteMaster.Configuration.Config.Resample.xBRZ.DominantDirectionThreshold;
	internal static double SteepDirectionThreshold = SpriteMaster.Configuration.Config.Resample.xBRZ.SteepDirectionThreshold;
	internal static double CenterDirectionBias = SpriteMaster.Configuration.Config.Resample.xBRZ.CenterDirectionBias;

	private static PictureBox PreviewBox = null!;
	private static Bitmap? PreviewBitmap = null;

	internal static int SubMain(Options options, List<Argument> args) {
		var previewFile = options.Paths.FirstOrDefault();
		CurrentPath = previewFile;

		Application.EnableVisualStyles();
		Application.Run(new PreviewWindow());

		return 0;
	}

	private static void InitializeMenu(PreviewWindow window) {
		var mainMenu = new MenuStrip();
		window.MainMenuStrip = mainMenu;
		mainMenu.RenderMode = ToolStripRenderMode.System;
		mainMenu.Dock = DockStyle.Top;
		mainMenu.BackColor = Color.Black;
		mainMenu.ForeColor = Color.LightGray;
		mainMenu.Items.Add(new ToolStripButton("Open", null, (sender, args) => OnOpen(sender, args, window)));
		{
			var resamplers = new ToolStripDropDownButton("Resampler");
			resamplers.DropDownItems.Add(new ToolStripButton("xBRZ", null, (sender, args) => OnResamplerChange(ResamplerType.XBrz, window)));
			resamplers.DropDownItems.Add(new ToolStripButton("EPX", null, (sender, args) => OnResamplerChange(ResamplerType.Epx, window)));
			mainMenu.Items.Add(resamplers);
		}
		window.Controls.Add(mainMenu);
	}

	private static string? PreviousDirectory = null;
	private static void OnOpen(object? sender, EventArgs e, PreviewWindow window) {
		using (var dialog = new OpenFileDialog() {
			InitialDirectory = PreviousDirectory ?? Directory.GetCurrentDirectory(),
			Filter = @"png files (*.png)|*.png|All files (*.*)|*.*",
			FilterIndex = 0,
			CheckFileExists = true,
			AutoUpgradeEnabled = true,
			ReadOnlyChecked = true
		}) {
			if (dialog.ShowDialog() is DialogResult.OK) {
				var path = dialog.FileName;
				if (path is not null && File.Exists(path)) {
					var directory = Path.GetDirectoryName(path);
					PreviousDirectory = directory;
					OnOpened(path, window);
				}
			}
		}
	}

	private static void OnOpened(string path, PreviewWindow window) {
		CurrentPath = path;
		if (CurrentPath is not null) {
			var fileData = Common.ReadFile(new Uri(CurrentPath), out var fileSize);

			SpriteData = Color16.Convert(fileData).ToArray();
			// It doesn't appear as though the file data is premultiplied!
			SpriteMaster.Resample.Passes.PremultipliedAlpha.Apply(SpriteData, fileSize);

			SpriteSize = fileSize;

			PreviewBox = window.ImagePreviewBox;
			ProcessSprite();
			PreviewBox.Image = PreviewBitmap;
			if (PreviewBitmap is not null) {
				PreviewBox.Size = PreviewBitmap.Size;
			}
		}
	}

	private static void OnResamplerChange(ResamplerType resampler, PreviewWindow window) {
		if (Resampler == resampler) {
			return;
		}

		Resampler = resampler;
		OnConfigChanged();
	}

	internal static void OnLoad(PreviewWindow window) {
		InitializeMenu(window);

		if (CurrentPath is not null) {
			OnOpened(CurrentPath, window);
		}
	}

	internal static void OnConfigChanged() {
		ProcessSprite();
		PreviewBox.Image = PreviewBitmap;
		if (PreviewBitmap is not null) {
			PreviewBox.Size = PreviewBitmap.Size;
		}
	}

	private static unsafe void ProcessSprite() {
		uint scale = Scale;
		var spriteSize = SpriteSize;
		var scaledSize = spriteSize * scale;

		var spriteDataArray = SpriteData!.CloneFast();
		var spriteData = spriteDataArray.AsSpan();
		if (GammaCorrection)
			SpriteMaster.Resample.Passes.GammaCorrection.Linearize(spriteData, spriteSize);
		if (AlphaPremultiplication)
			SpriteMaster.Resample.Passes.PremultipliedAlpha.Reverse(spriteData, spriteSize);

		if (scale != 1) {
			var resampleInterface = Resampler switch {
				ResamplerType.XBrz => (IScaler)new SpriteMaster.Resample.Scalers.xBRZ.Scaler.ScalerInterface(),
				ResamplerType.Epx => (IScaler)new SpriteMaster.Resample.Scalers.EPX.Scaler.ScalerInterface(),
				_ => ThrowHelper.ThrowInvalidOperationException<IScaler>($"Unknown Resampler: {Resampler}")
			};

			var resampleConfig = Resampler switch {
				ResamplerType.XBrz => (SpriteMaster.Resample.Scalers.Config)new SpriteMaster.Resample.Scalers.xBRZ.Config(
					wrapped: Vector2B.False,
					luminanceWeight: LuminanceWeight,
					gammaCorrected: !GammaCorrection,
					equalColorTolerance: EqualColorTolerance,
					dominantDirectionThreshold: DominantDirectionThreshold,
					steepDirectionThreshold: SteepDirectionThreshold,
					centerDirectionBias: CenterDirectionBias
				),
				ResamplerType.Epx => (SpriteMaster.Resample.Scalers.Config)new SpriteMaster.Resample.Scalers.EPX.Config(
					wrapped: Vector2B.False,
					luminanceWeight: LuminanceWeight,
					gammaCorrected: !GammaCorrection,
					equalColorTolerance: EqualColorTolerance
				),
				_ => ThrowHelper.ThrowInvalidOperationException<SpriteMaster.Resample.Scalers.Config>($"Unknown Resampler: {Resampler}")
			};

			spriteData = resampleInterface.Apply(
				resampleConfig,
				scaleMultiplier: scale,
				sourceData: spriteData,
				sourceSize: spriteSize,
				targetData: null,
				targetSize: scaledSize
			);
			spriteSize = scaledSize;
		}

		if (AlphaPremultiplication)
			SpriteMaster.Resample.Passes.PremultipliedAlpha.Apply(spriteData, spriteSize);
		if (GammaCorrection)
			SpriteMaster.Resample.Passes.GammaCorrection.Delinearize(spriteData, spriteSize);

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
