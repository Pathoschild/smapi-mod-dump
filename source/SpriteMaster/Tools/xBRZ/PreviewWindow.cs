/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xBRZ;
public partial class PreviewWindow : Form {
	public PreviewWindow() {
		InitializeComponent();
	}

	private void PreviewWindow_Load(object sender, EventArgs e) {
		PreviewProgram.OnLoad(this);
	}

	private void MainRightPanel_Paint(object sender, PaintEventArgs e) {

	}

	private void Scale_ValueChanged(object sender, EventArgs e) {
		if (sender is NumericUpDown nud) {
			PreviewProgram.Scale = (uint)nud.Value;
			PreviewProgram.OnConfigChanged();
		}
	}

	private void EqualColorTolerance_Scroll(object sender, ScrollEventArgs e) {
		if (sender is HScrollBar bar) {
			PreviewProgram.EqualColorTolerance = (uint)bar.Value;
			EqualColorToleranceCurrent.Text = PreviewProgram.EqualColorTolerance.ToString();
			PreviewProgram.OnConfigChanged();
		}
	}

	private void DominantDirectionThreshold_Scroll(object sender, ScrollEventArgs e) {
		if (sender is HScrollBar bar) {
			PreviewProgram.DominantDirectionThreshold = (double)bar.Value / 100.0;
			DominantDirectionThresholdCurrent.Text = PreviewProgram.DominantDirectionThreshold.ToString();
			PreviewProgram.OnConfigChanged();
		}
	}

	private void SteepDirectionThreshold_Scroll(object sender, ScrollEventArgs e) {
		if (sender is HScrollBar bar) {
			PreviewProgram.SteepDirectionThreshold = (double)bar.Value / 100.0;
			SteepDirectionThresholdCurrent.Text = PreviewProgram.SteepDirectionThreshold.ToString();
			PreviewProgram.OnConfigChanged();
		}
	}

	private void CenterDirectionBias_Scroll(object sender, ScrollEventArgs e) {
		if (sender is HScrollBar bar) {
			PreviewProgram.CenterDirectionBias = (double)bar.Value / 100.0;
			CenterDirectionBiasCurrent.Text = PreviewProgram.CenterDirectionBias.ToString();
			PreviewProgram.OnConfigChanged();
		}
	}

	private void GammaCorrection_CheckedChanged(object sender, EventArgs e) {
		if (sender is CheckBox cbox) {
			PreviewProgram.GammaCorrection = cbox.Checked;
			PreviewProgram.OnConfigChanged();
		}
	}

	private void AlphaPremultiplication_CheckedChanged(object sender, EventArgs e) {
		if (sender is CheckBox cbox) {
			PreviewProgram.AlphaPremultiplication = cbox.Checked;
			PreviewProgram.OnConfigChanged();
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
