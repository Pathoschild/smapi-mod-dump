/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

namespace xBRZ;

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
		if (disposing && (components != null)) {
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
			this.mainContainer = new System.Windows.Forms.SplitContainer();
			this.previewBox = new System.Windows.Forms.PictureBox();
			this.alphaPremultiplication = new System.Windows.Forms.CheckBox();
			this.gammaCorrection = new System.Windows.Forms.CheckBox();
			this.currentCenterDirectionBias = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.CenterDirectionBias = new System.Windows.Forms.HScrollBar();
			this.currentSteepDirectionThreshold = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.SteepDirectionThreshold = new System.Windows.Forms.HScrollBar();
			this.currentDominantDirectionThreshold = new System.Windows.Forms.Label();
			this.ddt_label = new System.Windows.Forms.Label();
			this.dominantDirectionThreshold = new System.Windows.Forms.HScrollBar();
			this.currentEqualColorTolerance = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.equalColorTolerance = new System.Windows.Forms.HScrollBar();
			this.currentLuminanceWeight = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.luminanceWeight = new System.Windows.Forms.HScrollBar();
			this.label1 = new System.Windows.Forms.Label();
			this.scale = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.mainContainer)).BeginInit();
			this.mainContainer.Panel1.SuspendLayout();
			this.mainContainer.Panel2.SuspendLayout();
			this.mainContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.previewBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.scale)).BeginInit();
			this.SuspendLayout();
			// 
			// mainContainer
			// 
			this.mainContainer.Cursor = System.Windows.Forms.Cursors.VSplit;
			resources.ApplyResources(this.mainContainer, "mainContainer");
			this.mainContainer.Name = "mainContainer";
			// 
			// mainContainer.Panel1
			// 
			this.mainContainer.Panel1.Controls.Add(this.previewBox);
			// 
			// mainContainer.Panel2
			// 
			this.mainContainer.Panel2.Controls.Add(this.alphaPremultiplication);
			this.mainContainer.Panel2.Controls.Add(this.gammaCorrection);
			this.mainContainer.Panel2.Controls.Add(this.currentCenterDirectionBias);
			this.mainContainer.Panel2.Controls.Add(this.label7);
			this.mainContainer.Panel2.Controls.Add(this.CenterDirectionBias);
			this.mainContainer.Panel2.Controls.Add(this.currentSteepDirectionThreshold);
			this.mainContainer.Panel2.Controls.Add(this.label6);
			this.mainContainer.Panel2.Controls.Add(this.SteepDirectionThreshold);
			this.mainContainer.Panel2.Controls.Add(this.currentDominantDirectionThreshold);
			this.mainContainer.Panel2.Controls.Add(this.ddt_label);
			this.mainContainer.Panel2.Controls.Add(this.dominantDirectionThreshold);
			this.mainContainer.Panel2.Controls.Add(this.currentEqualColorTolerance);
			this.mainContainer.Panel2.Controls.Add(this.label4);
			this.mainContainer.Panel2.Controls.Add(this.equalColorTolerance);
			this.mainContainer.Panel2.Controls.Add(this.currentLuminanceWeight);
			this.mainContainer.Panel2.Controls.Add(this.label2);
			this.mainContainer.Panel2.Controls.Add(this.luminanceWeight);
			this.mainContainer.Panel2.Controls.Add(this.label1);
			this.mainContainer.Panel2.Controls.Add(this.scale);
			this.mainContainer.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer1_Panel2_Paint);
			// 
			// previewBox
			// 
			resources.ApplyResources(this.previewBox, "previewBox");
			this.previewBox.Name = "previewBox";
			this.previewBox.TabStop = false;
			this.previewBox.Click += new System.EventHandler(this.pictureBox1_Click);
			// 
			// alphaPremultiplication
			// 
			resources.ApplyResources(this.alphaPremultiplication, "alphaPremultiplication");
			this.alphaPremultiplication.Checked = true;
			this.alphaPremultiplication.CheckState = System.Windows.Forms.CheckState.Checked;
			this.alphaPremultiplication.Name = "alphaPremultiplication";
			this.alphaPremultiplication.UseVisualStyleBackColor = true;
			this.alphaPremultiplication.CheckedChanged += new System.EventHandler(this.alphaPremultiplication_CheckedChanged);
			// 
			// gammaCorrection
			// 
			resources.ApplyResources(this.gammaCorrection, "gammaCorrection");
			this.gammaCorrection.Checked = true;
			this.gammaCorrection.CheckState = System.Windows.Forms.CheckState.Checked;
			this.gammaCorrection.Name = "gammaCorrection";
			this.gammaCorrection.UseVisualStyleBackColor = true;
			this.gammaCorrection.CheckedChanged += new System.EventHandler(this.gammaCorrection_CheckedChanged);
			// 
			// currentCenterDirectionBias
			// 
			resources.ApplyResources(this.currentCenterDirectionBias, "currentCenterDirectionBias");
			this.currentCenterDirectionBias.Name = "currentCenterDirectionBias";
			this.currentCenterDirectionBias.Click += new System.EventHandler(this.currentCenterDirectionBias_Click);
			// 
			// label7
			// 
			resources.ApplyResources(this.label7, "label7");
			this.label7.Name = "label7";
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
			// currentSteepDirectionThreshold
			// 
			resources.ApplyResources(this.currentSteepDirectionThreshold, "currentSteepDirectionThreshold");
			this.currentSteepDirectionThreshold.Name = "currentSteepDirectionThreshold";
			// 
			// label6
			// 
			resources.ApplyResources(this.label6, "label6");
			this.label6.Name = "label6";
			this.label6.Click += new System.EventHandler(this.label6_Click);
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
			// currentDominantDirectionThreshold
			// 
			resources.ApplyResources(this.currentDominantDirectionThreshold, "currentDominantDirectionThreshold");
			this.currentDominantDirectionThreshold.Name = "currentDominantDirectionThreshold";
			// 
			// ddt_label
			// 
			resources.ApplyResources(this.ddt_label, "ddt_label");
			this.ddt_label.Name = "ddt_label";
			this.ddt_label.Click += new System.EventHandler(this.label5_Click);
			// 
			// dominantDirectionThreshold
			// 
			resources.ApplyResources(this.dominantDirectionThreshold, "dominantDirectionThreshold");
			this.dominantDirectionThreshold.LargeChange = 5;
			this.dominantDirectionThreshold.Maximum = 1000;
			this.dominantDirectionThreshold.Name = "dominantDirectionThreshold";
			this.dominantDirectionThreshold.Value = 440;
			this.dominantDirectionThreshold.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dominantDirectionThreshold_Scroll);
			// 
			// currentEqualColorTolerance
			// 
			resources.ApplyResources(this.currentEqualColorTolerance, "currentEqualColorTolerance");
			this.currentEqualColorTolerance.Name = "currentEqualColorTolerance";
			this.currentEqualColorTolerance.Click += new System.EventHandler(this.label3_Click);
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			this.label4.Click += new System.EventHandler(this.label4_Click);
			// 
			// equalColorTolerance
			// 
			resources.ApplyResources(this.equalColorTolerance, "equalColorTolerance");
			this.equalColorTolerance.LargeChange = 5;
			this.equalColorTolerance.Maximum = 255;
			this.equalColorTolerance.Name = "equalColorTolerance";
			this.equalColorTolerance.Value = 20;
			this.equalColorTolerance.Scroll += new System.Windows.Forms.ScrollEventHandler(this.equalColorTolerance_Scroll);
			// 
			// currentLuminanceWeight
			// 
			resources.ApplyResources(this.currentLuminanceWeight, "currentLuminanceWeight");
			this.currentLuminanceWeight.Name = "currentLuminanceWeight";
			this.currentLuminanceWeight.Click += new System.EventHandler(this.currentLuminanceWeight_Click);
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			this.label2.Click += new System.EventHandler(this.label2_Click);
			// 
			// luminanceWeight
			// 
			this.luminanceWeight.LargeChange = 5;
			resources.ApplyResources(this.luminanceWeight, "luminanceWeight");
			this.luminanceWeight.Maximum = 1000;
			this.luminanceWeight.Name = "luminanceWeight";
			this.luminanceWeight.Value = 100;
			this.luminanceWeight.Scroll += new System.Windows.Forms.ScrollEventHandler(this.luminanceWeight_Scroll);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			this.label1.Click += new System.EventHandler(this.label1_Click);
			// 
			// scale
			// 
			resources.ApplyResources(this.scale, "scale");
			this.scale.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
			this.scale.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.scale.Name = "scale";
			this.scale.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.scale.ValueChanged += new System.EventHandler(this.scale_ValueChanged);
			// 
			// PreviewWindow
			// 
			this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.Controls.Add(this.mainContainer);
			this.DoubleBuffered = true;
			this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "PreviewWindow";
			this.Load += new System.EventHandler(this.PreviewWindow_Load);
			this.mainContainer.Panel1.ResumeLayout(false);
			this.mainContainer.Panel2.ResumeLayout(false);
			this.mainContainer.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.mainContainer)).EndInit();
			this.mainContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.previewBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.scale)).EndInit();
			this.ResumeLayout(false);

	}

	#endregion

	private SplitContainer mainContainer;
	private PictureBox previewBox;
	private Label label1;
	private NumericUpDown scale;
	private HScrollBar luminanceWeight;
	private Label label2;
	private Label currentLuminanceWeight;
	private Label currentEqualColorTolerance;
	private Label label4;
	private HScrollBar equalColorTolerance;
	private Label currentDominantDirectionThreshold;
	private Label ddt_label;
	private HScrollBar dominantDirectionThreshold;
	private Label currentCenterDirectionBias;
	private Label label7;
	private HScrollBar CenterDirectionBias;
	private Label currentSteepDirectionThreshold;
	private Label label6;
	private HScrollBar SteepDirectionThreshold;
	private CheckBox gammaCorrection;
	private CheckBox alphaPremultiplication;
}
