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

	private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e) {

	}

	private void pictureBox1_Click(object sender, EventArgs e) {

	}

	private void label1_Click(object sender, EventArgs e) {

	}

	private void scale_ValueChanged(object sender, EventArgs e) {
		if (sender is NumericUpDown nud) {
			PreviewProgram.Scale = (uint)nud.Value;
			PreviewProgram.OnConfigChanged();
		}
	}

	private void luminanceWeight_Scroll(object sender, ScrollEventArgs e) {
		if (sender is HScrollBar bar) {
			PreviewProgram.LuminanceWeight = (double)bar.Value / 100.0;
			currentLuminanceWeight.Text = PreviewProgram.LuminanceWeight.ToString();
			PreviewProgram.OnConfigChanged();
		}
	}

	private void equalColorTolerance_Scroll(object sender, ScrollEventArgs e) {
		if (sender is HScrollBar bar) {
			PreviewProgram.EqualColorTolerance = (double)bar.Value;
			currentEqualColorTolerance.Text = ((int)PreviewProgram.EqualColorTolerance).ToString();
			PreviewProgram.OnConfigChanged();
		}
	}

	private void dominantDirectionThreshold_Scroll(object sender, ScrollEventArgs e) {
		if (sender is HScrollBar bar) {
			PreviewProgram.DominantDirectionThreshold = (double)bar.Value / 100.0;
			currentDominantDirectionThreshold.Text = PreviewProgram.DominantDirectionThreshold.ToString();
			PreviewProgram.OnConfigChanged();
		}
	}

	private void SteepDirectionThreshold_Scroll(object sender, ScrollEventArgs e) {
		if (sender is HScrollBar bar) {
			PreviewProgram.SteepDirectionThreshold = (double)bar.Value / 100.0;
			currentSteepDirectionThreshold.Text = PreviewProgram.SteepDirectionThreshold.ToString();
			PreviewProgram.OnConfigChanged();
		}
	}

	private void CenterDirectionBias_Scroll(object sender, ScrollEventArgs e) {
		if (sender is HScrollBar bar) {
			PreviewProgram.CenterDirectionBias = (double)bar.Value / 100.0;
			currentCenterDirectionBias.Text = PreviewProgram.CenterDirectionBias.ToString();
			PreviewProgram.OnConfigChanged();
		}
	}

	private void gammaCorrection_CheckedChanged(object sender, EventArgs e) {
		if (sender is CheckBox cbox) {
			PreviewProgram.GammaCorrection = cbox.Checked;
			PreviewProgram.OnConfigChanged();
		}
	}

	private void alphaPremultiplication_CheckedChanged(object sender, EventArgs e) {
		if (sender is CheckBox cbox) {
			PreviewProgram.AlphaPremultiplication = cbox.Checked;
			PreviewProgram.OnConfigChanged();
		}
	}

	private void label2_Click(object sender, EventArgs e) {

	}

	private void currentLuminanceWeight_Click(object sender, EventArgs e) {

	}

	private void label4_Click(object sender, EventArgs e) {

	}

	private void label3_Click(object sender, EventArgs e) {

	}

	private void label5_Click(object sender, EventArgs e) {

	}

	private void label6_Click(object sender, EventArgs e) {

	}

	private void currentCenterDirectionBias_Click(object sender, EventArgs e) {

	}
}
