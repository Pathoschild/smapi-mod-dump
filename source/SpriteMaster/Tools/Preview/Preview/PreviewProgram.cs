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
using SpriteMaster.Extensions;
using SpriteMaster.Resample.Scalers;
using SpriteMaster.Types;
using System.Drawing.Imaging;
using System.IO;

using Path = System.IO.Path;

namespace SpriteMaster.Tools.Preview.Preview;

enum ResamplerType {
	XBrz = 0,
	Epx = 1
}

internal class PreviewProgram : AbstractProgram {
	internal static PreviewProgram? Instance => AbstractProgram.Instance as PreviewProgram;
	private readonly PreviewWindow Window;

	private Color16[]? SpriteData = null;
	private string? CurrentPath = null;
	private Vector2I SpriteSize;

	private PictureBox PreviewBox = null!;
	private Bitmap? PreviewBitmap = null;

	internal sealed class State {
		private ResamplerType ResamplerInternal = ResamplerType.XBrz;
		internal ResamplerType Resampler {
			get => ResamplerInternal;
			set {
				var previousResampler = ResamplerInternal;
				ResamplerInternal = value;
				Update(previousResampler);
			}
		}

		internal uint Scale = 1;
		internal bool GammaCorrection = false;
		internal bool AlphaPremultiplication = true;
		internal double LuminanceWeight = Configuration.Config.Resample.Common.LuminanceWeight;

		internal byte EqualColorTolerance =
			Configuration.Config.Resample.Common.EqualColorTolerance;

		internal double DominantDirectionThreshold =
			Configuration.Config.Resample.xBRZ.DominantDirectionThreshold;

		internal double SteepDirectionThreshold =
			Configuration.Config.Resample.xBRZ.SteepDirectionThreshold;

		internal double CenterDirectionBias = Configuration.Config.Resample.xBRZ.CenterDirectionBias;

		private void Update(ResamplerType previous) {

		}
	}

	internal readonly State CurrentState = new();

	internal PreviewProgram() {
		Application.EnableVisualStyles();
		Window = new(this);
		Application.Run(Window);
	}

	internal override async Task<int> OnSubMainAsync(Options options, List<Argument> args) {
		var previewFile = options.Paths.FirstOrDefault();
		CurrentPath = previewFile;

		return 0;
	}

	private void InitializeMenu(PreviewWindow window) {
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

	private string? PreviousDirectory = null;
	private void OnOpen(object? sender, EventArgs e, PreviewWindow window) {
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

	private void OnOpened(string? path, PreviewWindow window) {
		CurrentPath = path;
		if (CurrentPath is not null) {
			var fileData = Common.ReadFile(new Uri(CurrentPath), out var fileSize);

			SpriteData = Color16.Convert(fileData).ToArray();
			// It doesn't appear as though the file data is premultiplied!
			Resample.Passes.PremultipliedAlpha.Apply(SpriteData, fileSize, true);

			SpriteSize = fileSize;

			PreviewBox = window.ImagePreviewBox;
			ProcessSprite(CurrentState);
			PreviewBox.Image = PreviewBitmap;
			if (PreviewBitmap is not null) {
				PreviewBox.Size = PreviewBitmap.Size;
			}
		}
	}

	private void OnResamplerChange(ResamplerType resampler, PreviewWindow window) {
		if (CurrentState.Resampler == resampler) {
			return;
		}

		CurrentState.Resampler = resampler;
		OnConfigChanged();
	}

	internal void OnLoad(PreviewWindow window) {
		InitializeMenu(window);

		if (CurrentPath is not null) {
			OnOpened(CurrentPath, window);
		}
	}

	internal void OnConfigChanged() {
		Window.OnConfigChanged();
		ProcessSprite(CurrentState);
		PreviewBox.Image = PreviewBitmap;
		if (PreviewBitmap is not null) {
			PreviewBox.Size = PreviewBitmap.Size;
		}
	}

	private unsafe void ProcessSprite(State state) {
		var scale = state.Scale;
		var spriteSize = SpriteSize;
		var scaledSize = spriteSize * scale;

		if (SpriteData is null) {
			return;
		}

		var spriteDataArray = SpriteData.CloneFast();
		var spriteData = spriteDataArray.AsSpan();
		if (state.GammaCorrection)
			Resample.Passes.GammaCorrection.Linearize(spriteData, spriteSize);
		if (state.AlphaPremultiplication)
			Resample.Passes.PremultipliedAlpha.Reverse(spriteData, spriteSize, true);

		if (scale != 1) {
			var resampleInterface = state.Resampler switch {
				ResamplerType.XBrz => Resample.Scalers.xBRZ.Scaler.ScalerInterface.Instance,
				ResamplerType.Epx => Resample.Scalers.EPX.Scaler.ScalerInterface.Instance,
				_ => ThrowHelper.ThrowInvalidOperationException<IScaler>($"Unknown Resampler: {state.Resampler}")
			};

			var resampleConfig = state.Resampler switch {
				ResamplerType.XBrz => new Resample.Scalers.xBRZ.Config(
					wrapped: Vector2B.False,
					luminanceWeight: state.LuminanceWeight,
					gammaCorrected: !state.GammaCorrection,
					equalColorTolerance: state.EqualColorTolerance,
					dominantDirectionThreshold: state.DominantDirectionThreshold,
					steepDirectionThreshold: state.SteepDirectionThreshold,
					centerDirectionBias: state.CenterDirectionBias
				),
				ResamplerType.Epx => new Resample.Scalers.EPX.Config(
					wrapped: Vector2B.False,
					luminanceWeight: state.LuminanceWeight,
					gammaCorrected: !state.GammaCorrection,
					equalColorTolerance: state.EqualColorTolerance
				),
				_ => ThrowHelper.ThrowInvalidOperationException<Config>($"Unknown Resampler: {state.Resampler}")
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

		if (state.AlphaPremultiplication)
			Resample.Passes.PremultipliedAlpha.Apply(spriteData, spriteSize, true);
		if (state.GammaCorrection)
			Resample.Passes.GammaCorrection.Delinearize(spriteData, spriteSize);

		var resampledData = Color8.Convert(spriteData);

		Bitmap resampledBitmap;
		fixed (Color8* ptr = resampledData) {
			var stride = spriteSize.Width * sizeof(Color8);
			resampledBitmap = new Bitmap(spriteSize.Width, spriteSize.Height, stride, PixelFormat.Format32bppPArgb, (IntPtr)ptr);
		}

		var oldBitmap = PreviewBitmap;
		PreviewBitmap = resampledBitmap;
		oldBitmap?.Dispose();
	}
}
