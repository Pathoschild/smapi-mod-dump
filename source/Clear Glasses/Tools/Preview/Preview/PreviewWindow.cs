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

public partial class PreviewWindow : Form {
	private readonly PreviewProgram Program;
	private PreviewProgram.State CurrentState => Program.CurrentState;

	private PreviewWindowConfig CurrentConfig;

	private PreviewWindowConfig GetNewConfig(ResamplerType resampler) => resampler switch {
		ResamplerType.XBrz => new XBrzPreviewWindowConfig(this),
		ResamplerType.Epx => new EpxPreviewWindowConfig(this),
		_ => ThrowHelper.ThrowUnknownArgumentEnumException<ResamplerType, PreviewWindowConfig>(nameof(resampler), CurrentState.Resampler)
	};

internal PreviewWindow(PreviewProgram program) {
		Program = program;
		InitializeComponent();
		CurrentConfig = GetNewConfig(CurrentState.Resampler);
	}

	internal void OnConfigChanged() {
		if (CurrentConfig.Resampler == CurrentState.Resampler) {
			return;
		}

		CurrentConfig.Dispose();
		CurrentConfig = GetNewConfig(CurrentState.Resampler);
	}

	private void PreviewWindow_Load(object sender, EventArgs e) {
		Program.OnLoad(this);
	}

	private void MainRightPanel_Paint(object sender, PaintEventArgs e) {

	}

	private void Scale_ValueChanged(object sender, EventArgs e) {
		if (sender is NumericUpDown nud) {
			CurrentState.Scale = (uint)nud.Value;
			Program.OnConfigChanged();
		}
	}

	private void EqualColorTolerance_Scroll(object sender, ScrollEventArgs e) {
		if (sender is HScrollBar bar) {
			CurrentState.EqualColorTolerance = (byte)bar.Value;
			EqualColorToleranceCurrent.Text = CurrentState.EqualColorTolerance.ToString();
			Program.OnConfigChanged();
		}
	}

	private void DominantDirectionThreshold_Scroll(object sender, ScrollEventArgs e) {
		if (sender is HScrollBar bar) {
			CurrentState.DominantDirectionThreshold = (double)bar.Value / 100.0;
			DominantDirectionThresholdCurrent.Text = CurrentState.DominantDirectionThreshold.ToString();
			Program.OnConfigChanged();
		}
	}

	private void SteepDirectionThreshold_Scroll(object sender, ScrollEventArgs e) {
		if (sender is HScrollBar bar) {
			CurrentState.SteepDirectionThreshold = (double)bar.Value / 100.0;
			SteepDirectionThresholdCurrent.Text = CurrentState.SteepDirectionThreshold.ToString();
			Program.OnConfigChanged();
		}
	}

	private void CenterDirectionBias_Scroll(object sender, ScrollEventArgs e) {
		if (sender is HScrollBar bar) {
			CurrentState.CenterDirectionBias = (double)bar.Value / 100.0;
			CenterDirectionBiasCurrent.Text = CurrentState.CenterDirectionBias.ToString();
			Program.OnConfigChanged();
		}
	}

	private void GammaCorrection_CheckedChanged(object sender, EventArgs e) {
		if (sender is CheckBox cbox) {
			CurrentState.GammaCorrection = cbox.Checked;
			Program.OnConfigChanged();
		}
	}

	private void AlphaPremultiplication_CheckedChanged(object sender, EventArgs e) {
		if (sender is CheckBox cbox) {
			CurrentState.AlphaPremultiplication = cbox.Checked;
			Program.OnConfigChanged();
		}
	}

	private int[]? CustomColors = null;

	private void BackgroundColorButton_Click(object sender, EventArgs e) {
		if (CustomColors is null) {
			CustomColors = new int[] { MainLeftPanel.BackColor.ToArgb() };
		}

		using var colorDialog = new ColorDialog {
			AllowFullOpen = true,
			ShowHelp = false,
			Color = MainLeftPanel.BackColor,
			CustomColors = CustomColors
		};

		if (colorDialog.ShowDialog() == DialogResult.OK) {
			MainLeftPanel.BackColor = colorDialog.Color;
		}

		CustomColors = colorDialog.CustomColors;
	}
}
