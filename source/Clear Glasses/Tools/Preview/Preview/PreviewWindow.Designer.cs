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

partial class PreviewWindow {
	/// <summary>
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	/// <summary>
	/// Clean up any resources being used.
	/// </summary>
	/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	protected override void Dispose(bool disposing) {
		if (disposing && (components is not null)) {
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	#region Windows Form Designer generated code

	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreviewWindow));
			this.MainContainer = new System.Windows.Forms.SplitContainer();
			this.BackgroundColorButton = new System.Windows.Forms.Button();
			this.ImagePreviewBox = new System.Windows.Forms.PictureBox();
			this.AlphaPremultiplication = new System.Windows.Forms.CheckBox();
			this.GammaCorrection = new System.Windows.Forms.CheckBox();
			this.CenterDirectionBiasCurrent = new System.Windows.Forms.Label();
			this.CenterDirectionBiasLabel = new System.Windows.Forms.Label();
			this.CenterDirectionBias = new System.Windows.Forms.HScrollBar();
			this.SteepDirectionThresholdCurrent = new System.Windows.Forms.Label();
			this.SteepDirectionThresholdLabel = new System.Windows.Forms.Label();
			this.SteepDirectionThreshold = new System.Windows.Forms.HScrollBar();
			this.DominantDirectionThresholdCurrent = new System.Windows.Forms.Label();
			this.DominantDirectionThresholdLabel = new System.Windows.Forms.Label();
			this.DominantDirectionThreshold = new System.Windows.Forms.HScrollBar();
			this.EqualColorToleranceCurrent = new System.Windows.Forms.Label();
			this.EqualColorToleranceLabel = new System.Windows.Forms.Label();
			this.EqualColorTolerance = new System.Windows.Forms.HScrollBar();
			this.ScaleLabel = new System.Windows.Forms.Label();
			this.ScaleChanger = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.MainContainer)).BeginInit();
			this.MainContainer.Panel1.SuspendLayout();
			this.MainContainer.Panel2.SuspendLayout();
			this.MainContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ImagePreviewBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ScaleChanger)).BeginInit();
			this.SuspendLayout();
			// 
			// MainContainer
			// 
			this.MainContainer.Cursor = System.Windows.Forms.Cursors.VSplit;
			resources.ApplyResources(this.MainContainer, "MainContainer");
			this.MainContainer.Name = "MainContainer";
			// 
			// MainContainer.Panel1
			// 
			resources.ApplyResources(this.MainContainer.Panel1, "MainContainer.Panel1");
			this.MainContainer.Panel1.Controls.Add(this.BackgroundColorButton);
			this.MainContainer.Panel1.Controls.Add(this.ImagePreviewBox);
			// 
			// MainContainer.Panel2
			// 
			this.MainContainer.Panel2.Controls.Add(this.AlphaPremultiplication);
			this.MainContainer.Panel2.Controls.Add(this.GammaCorrection);
			this.MainContainer.Panel2.Controls.Add(this.CenterDirectionBiasCurrent);
			this.MainContainer.Panel2.Controls.Add(this.CenterDirectionBiasLabel);
			this.MainContainer.Panel2.Controls.Add(this.CenterDirectionBias);
			this.MainContainer.Panel2.Controls.Add(this.SteepDirectionThresholdCurrent);
			this.MainContainer.Panel2.Controls.Add(this.SteepDirectionThresholdLabel);
			this.MainContainer.Panel2.Controls.Add(this.SteepDirectionThreshold);
			this.MainContainer.Panel2.Controls.Add(this.DominantDirectionThresholdCurrent);
			this.MainContainer.Panel2.Controls.Add(this.DominantDirectionThresholdLabel);
			this.MainContainer.Panel2.Controls.Add(this.DominantDirectionThreshold);
			this.MainContainer.Panel2.Controls.Add(this.EqualColorToleranceCurrent);
			this.MainContainer.Panel2.Controls.Add(this.EqualColorToleranceLabel);
			this.MainContainer.Panel2.Controls.Add(this.EqualColorTolerance);
			this.MainContainer.Panel2.Controls.Add(this.ScaleLabel);
			this.MainContainer.Panel2.Controls.Add(this.ScaleChanger);
			this.MainContainer.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.MainRightPanel_Paint);
			// 
			// BackgroundColorButton
			// 
			resources.ApplyResources(this.BackgroundColorButton, "BackgroundColorButton");
			this.BackgroundColorButton.ForeColor = System.Drawing.Color.Black;
			this.BackgroundColorButton.Name = "BackgroundColorButton";
			this.BackgroundColorButton.UseVisualStyleBackColor = true;
			this.BackgroundColorButton.Click += new System.EventHandler(this.BackgroundColorButton_Click);
			// 
			// ImagePreviewBox
			// 
			resources.ApplyResources(this.ImagePreviewBox, "ImagePreviewBox");
			this.ImagePreviewBox.Name = "ImagePreviewBox";
			this.ImagePreviewBox.TabStop = false;
			// 
			// AlphaPremultiplication
			// 
			resources.ApplyResources(this.AlphaPremultiplication, "AlphaPremultiplication");
			this.AlphaPremultiplication.Checked = true;
			this.AlphaPremultiplication.CheckState = System.Windows.Forms.CheckState.Checked;
			this.AlphaPremultiplication.Name = "AlphaPremultiplication";
			this.AlphaPremultiplication.UseVisualStyleBackColor = true;
			this.AlphaPremultiplication.CheckedChanged += new System.EventHandler(this.AlphaPremultiplication_CheckedChanged);
			// 
			// GammaCorrection
			// 
			resources.ApplyResources(this.GammaCorrection, "GammaCorrection");
			this.GammaCorrection.Checked = true;
			this.GammaCorrection.CheckState = System.Windows.Forms.CheckState.Checked;
			this.GammaCorrection.Name = "GammaCorrection";
			this.GammaCorrection.UseVisualStyleBackColor = true;
			this.GammaCorrection.CheckedChanged += new System.EventHandler(this.GammaCorrection_CheckedChanged);
			// 
			// CenterDirectionBiasCurrent
			// 
			resources.ApplyResources(this.CenterDirectionBiasCurrent, "CenterDirectionBiasCurrent");
			this.CenterDirectionBiasCurrent.Name = "CenterDirectionBiasCurrent";
			// 
			// CenterDirectionBiasLabel
			// 
			resources.ApplyResources(this.CenterDirectionBiasLabel, "CenterDirectionBiasLabel");
			this.CenterDirectionBiasLabel.Name = "CenterDirectionBiasLabel";
			// 
			// CenterDirectionBias
			// 
			resources.ApplyResources(this.CenterDirectionBias, "CenterDirectionBias");
			this.CenterDirectionBias.LargeChange = 5;
			this.CenterDirectionBias.Maximum = 1000;
			this.CenterDirectionBias.Name = "CenterDirectionBias";
			this.CenterDirectionBias.Value = 300;
			this.CenterDirectionBias.Scroll += new System.Windows.Forms.ScrollEventHandler(this.CenterDirectionBias_Scroll);
			// 
			// SteepDirectionThresholdCurrent
			// 
			resources.ApplyResources(this.SteepDirectionThresholdCurrent, "SteepDirectionThresholdCurrent");
			this.SteepDirectionThresholdCurrent.Name = "SteepDirectionThresholdCurrent";
			// 
			// SteepDirectionThresholdLabel
			// 
			resources.ApplyResources(this.SteepDirectionThresholdLabel, "SteepDirectionThresholdLabel");
			this.SteepDirectionThresholdLabel.Name = "SteepDirectionThresholdLabel";
			// 
			// SteepDirectionThreshold
			// 
			resources.ApplyResources(this.SteepDirectionThreshold, "SteepDirectionThreshold");
			this.SteepDirectionThreshold.LargeChange = 5;
			this.SteepDirectionThreshold.Maximum = 1000;
			this.SteepDirectionThreshold.Name = "SteepDirectionThreshold";
			this.SteepDirectionThreshold.Value = 220;
			this.SteepDirectionThreshold.Scroll += new System.Windows.Forms.ScrollEventHandler(this.SteepDirectionThreshold_Scroll);
			// 
			// DominantDirectionThresholdCurrent
			// 
			resources.ApplyResources(this.DominantDirectionThresholdCurrent, "DominantDirectionThresholdCurrent");
			this.DominantDirectionThresholdCurrent.Name = "DominantDirectionThresholdCurrent";
			// 
			// DominantDirectionThresholdLabel
			// 
			resources.ApplyResources(this.DominantDirectionThresholdLabel, "DominantDirectionThresholdLabel");
			this.DominantDirectionThresholdLabel.Name = "DominantDirectionThresholdLabel";
			// 
			// DominantDirectionThreshold
			// 
			resources.ApplyResources(this.DominantDirectionThreshold, "DominantDirectionThreshold");
			this.DominantDirectionThreshold.LargeChange = 5;
			this.DominantDirectionThreshold.Maximum = 1000;
			this.DominantDirectionThreshold.Name = "DominantDirectionThreshold";
			this.DominantDirectionThreshold.Value = 440;
			this.DominantDirectionThreshold.Scroll += new System.Windows.Forms.ScrollEventHandler(this.DominantDirectionThreshold_Scroll);
			// 
			// EqualColorToleranceCurrent
			// 
			resources.ApplyResources(this.EqualColorToleranceCurrent, "EqualColorToleranceCurrent");
			this.EqualColorToleranceCurrent.Name = "EqualColorToleranceCurrent";
			// 
			// EqualColorToleranceLabel
			// 
			resources.ApplyResources(this.EqualColorToleranceLabel, "EqualColorToleranceLabel");
			this.EqualColorToleranceLabel.Name = "EqualColorToleranceLabel";
			// 
			// EqualColorTolerance
			// 
			resources.ApplyResources(this.EqualColorTolerance, "EqualColorTolerance");
			this.EqualColorTolerance.LargeChange = 5;
			this.EqualColorTolerance.Maximum = 255;
			this.EqualColorTolerance.Name = "EqualColorTolerance";
			this.EqualColorTolerance.Value = 20;
			this.EqualColorTolerance.Scroll += new System.Windows.Forms.ScrollEventHandler(this.EqualColorTolerance_Scroll);
			// 
			// ScaleLabel
			// 
			resources.ApplyResources(this.ScaleLabel, "ScaleLabel");
			this.ScaleLabel.Name = "ScaleLabel";
			// 
			// ScaleChanger
			// 
			resources.ApplyResources(this.ScaleChanger, "ScaleChanger");
			this.ScaleChanger.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
			this.ScaleChanger.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.ScaleChanger.Name = "ScaleChanger";
			this.ScaleChanger.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.ScaleChanger.ValueChanged += new System.EventHandler(this.Scale_ValueChanged);
			// 
			// PreviewWindow
			// 
			this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.Controls.Add(this.MainContainer);
			this.DoubleBuffered = true;
			this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "PreviewWindow";
			this.Load += new System.EventHandler(this.PreviewWindow_Load);
			this.MainContainer.Panel1.ResumeLayout(false);
			this.MainContainer.Panel1.PerformLayout();
			this.MainContainer.Panel2.ResumeLayout(false);
			this.MainContainer.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.MainContainer)).EndInit();
			this.MainContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ImagePreviewBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ScaleChanger)).EndInit();
			this.ResumeLayout(false);

	}

	#endregion

	internal SplitContainer MainContainer;
	internal Panel MainLeftPanel => MainContainer.Panel1;
	internal Panel MainRightPanel => MainContainer.Panel2;
	internal PictureBox ImagePreviewBox;
	internal Label ScaleLabel;
	internal NumericUpDown ScaleChanger;
	internal Label EqualColorToleranceCurrent;
	internal Label EqualColorToleranceLabel;
	internal HScrollBar EqualColorTolerance;
	internal Label DominantDirectionThresholdCurrent;
	internal Label DominantDirectionThresholdLabel;
	internal HScrollBar DominantDirectionThreshold;
	internal Label CenterDirectionBiasCurrent;
	internal Label CenterDirectionBiasLabel;
	internal HScrollBar CenterDirectionBias;
	internal Label SteepDirectionThresholdCurrent;
	internal Label SteepDirectionThresholdLabel;
	internal HScrollBar SteepDirectionThreshold;
	internal CheckBox GammaCorrection;
	internal CheckBox AlphaPremultiplication;
	internal Button BackgroundColorButton;
}
