using System.ComponentModel;

namespace DSPRE.Editors {
  partial class HeadbuttEncounterEditor {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private IContainer components = null;

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

    #region Component Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.textBoxPath = new System.Windows.Forms.TextBox();
      this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
      this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
      this.buttonLoad = new System.Windows.Forms.Button();
      this.buttonSaveAs = new System.Windows.Forms.Button();
      this.buttonSave = new System.Windows.Forms.Button();
      this.tabControl2 = new System.Windows.Forms.TabControl();
      this.tabPageNormal = new System.Windows.Forms.TabPage();
      this.headbuttEncounterEditorTabNormal = new DSPRE.Editors.HeadbuttEncounterEditorTab();
      this.tabPageSpecial = new System.Windows.Forms.TabPage();
      this.headbuttEncounterEditorTabSpecial = new DSPRE.Editors.HeadbuttEncounterEditorTab();
      this.mapRenderPanel = new System.Windows.Forms.Panel();
      this.openGlControl = new DSPRE.SimpleOpenGlControl2();
      this.openGlPictureBox = new System.Windows.Forms.PictureBox();
      this.radio3D = new System.Windows.Forms.RadioButton();
      this.radio2D = new System.Windows.Forms.RadioButton();
      this.wireframeCheckBox = new System.Windows.Forms.CheckBox();
      this.mapScreenshotButton = new System.Windows.Forms.Button();
      this.tabControl2.SuspendLayout();
      this.tabPageNormal.SuspendLayout();
      this.tabPageSpecial.SuspendLayout();
      this.mapRenderPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.openGlPictureBox)).BeginInit();
      this.SuspendLayout();
      // 
      // textBoxPath
      // 
      this.textBoxPath.Location = new System.Drawing.Point(3, 32);
      this.textBoxPath.Name = "textBoxPath";
      this.textBoxPath.ReadOnly = true;
      this.textBoxPath.Size = new System.Drawing.Size(413, 20);
      this.textBoxPath.TabIndex = 12;
      // 
      // openFileDialog1
      // 
      this.openFileDialog1.FileName = "openFileDialog1";
      // 
      // buttonLoad
      // 
      this.buttonLoad.Location = new System.Drawing.Point(3, 3);
      this.buttonLoad.Name = "buttonLoad";
      this.buttonLoad.Size = new System.Drawing.Size(75, 23);
      this.buttonLoad.TabIndex = 8;
      this.buttonLoad.Text = "Load";
      this.buttonLoad.UseVisualStyleBackColor = true;
      this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
      // 
      // buttonSaveAs
      // 
      this.buttonSaveAs.Location = new System.Drawing.Point(341, 3);
      this.buttonSaveAs.Name = "buttonSaveAs";
      this.buttonSaveAs.Size = new System.Drawing.Size(75, 23);
      this.buttonSaveAs.TabIndex = 9;
      this.buttonSaveAs.Text = "Save As";
      this.buttonSaveAs.UseVisualStyleBackColor = true;
      this.buttonSaveAs.Click += new System.EventHandler(this.buttonSaveAs_Click);
      // 
      // buttonSave
      // 
      this.buttonSave.Location = new System.Drawing.Point(260, 3);
      this.buttonSave.Name = "buttonSave";
      this.buttonSave.Size = new System.Drawing.Size(75, 23);
      this.buttonSave.TabIndex = 10;
      this.buttonSave.Text = "Save";
      this.buttonSave.UseVisualStyleBackColor = true;
      this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
      // 
      // tabControl2
      // 
      this.tabControl2.Controls.Add(this.tabPageNormal);
      this.tabControl2.Controls.Add(this.tabPageSpecial);
      this.tabControl2.Location = new System.Drawing.Point(3, 58);
      this.tabControl2.Name = "tabControl2";
      this.tabControl2.SelectedIndex = 0;
      this.tabControl2.Size = new System.Drawing.Size(413, 544);
      this.tabControl2.TabIndex = 21;
      // 
      // tabPageNormal
      // 
      this.tabPageNormal.Controls.Add(this.headbuttEncounterEditorTabNormal);
      this.tabPageNormal.Location = new System.Drawing.Point(4, 22);
      this.tabPageNormal.Name = "tabPageNormal";
      this.tabPageNormal.Padding = new System.Windows.Forms.Padding(3);
      this.tabPageNormal.Size = new System.Drawing.Size(405, 518);
      this.tabPageNormal.TabIndex = 0;
      this.tabPageNormal.Text = "Normal";
      this.tabPageNormal.UseVisualStyleBackColor = true;
      // 
      // headbuttEncounterEditorTabNormal
      // 
      this.headbuttEncounterEditorTabNormal.Location = new System.Drawing.Point(2, 2);
      this.headbuttEncounterEditorTabNormal.Name = "headbuttEncounterEditorTabNormal";
      this.headbuttEncounterEditorTabNormal.Size = new System.Drawing.Size(402, 519);
      this.headbuttEncounterEditorTabNormal.TabIndex = 0;
      // 
      // tabPageSpecial
      // 
      this.tabPageSpecial.Controls.Add(this.headbuttEncounterEditorTabSpecial);
      this.tabPageSpecial.Location = new System.Drawing.Point(4, 22);
      this.tabPageSpecial.Name = "tabPageSpecial";
      this.tabPageSpecial.Padding = new System.Windows.Forms.Padding(3);
      this.tabPageSpecial.Size = new System.Drawing.Size(405, 518);
      this.tabPageSpecial.TabIndex = 1;
      this.tabPageSpecial.Text = "Special";
      this.tabPageSpecial.UseVisualStyleBackColor = true;
      // 
      // headbuttEncounterEditorTabSpecial
      // 
      this.headbuttEncounterEditorTabSpecial.Location = new System.Drawing.Point(2, 2);
      this.headbuttEncounterEditorTabSpecial.Name = "headbuttEncounterEditorTabSpecial";
      this.headbuttEncounterEditorTabSpecial.Size = new System.Drawing.Size(402, 518);
      this.headbuttEncounterEditorTabSpecial.TabIndex = 0;
      // 
      // mapRenderPanel
      // 
      this.mapRenderPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.mapRenderPanel.Controls.Add(this.openGlControl);
      this.mapRenderPanel.Controls.Add(this.openGlPictureBox);
      this.mapRenderPanel.Location = new System.Drawing.Point(422, 3);
      this.mapRenderPanel.Name = "mapRenderPanel";
      this.mapRenderPanel.Size = new System.Drawing.Size(610, 610);
      this.mapRenderPanel.TabIndex = 24;
      // 
      // openGlControl
      // 
      this.openGlControl.AccumBits = ((byte)(0));
      this.openGlControl.AutoCheckErrors = false;
      this.openGlControl.AutoFinish = false;
      this.openGlControl.AutoMakeCurrent = true;
      this.openGlControl.AutoSwapBuffers = true;
      this.openGlControl.BackColor = System.Drawing.Color.Black;
      this.openGlControl.ColorBits = ((byte)(32));
      this.openGlControl.DepthBits = ((byte)(64));
      this.openGlControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.openGlControl.ForeColor = System.Drawing.Color.Black;
      this.openGlControl.Location = new System.Drawing.Point(0, 0);
      this.openGlControl.Name = "openGlControl";
      this.openGlControl.Size = new System.Drawing.Size(608, 608);
      this.openGlControl.StencilBits = ((byte)(0));
      this.openGlControl.TabIndex = 2;
      // 
      // openGlPictureBox
      // 
      this.openGlPictureBox.BackColor = System.Drawing.Color.White;
      this.openGlPictureBox.Location = new System.Drawing.Point(0, 0);
      this.openGlPictureBox.Name = "openGlPictureBox";
      this.openGlPictureBox.Size = new System.Drawing.Size(608, 608);
      this.openGlPictureBox.TabIndex = 3;
      this.openGlPictureBox.TabStop = false;
      // 
      // radio3D
      // 
      this.radio3D.Appearance = System.Windows.Forms.Appearance.Button;
      this.radio3D.AutoSize = true;
      this.radio3D.Checked = true;
      this.radio3D.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.radio3D.Location = new System.Drawing.Point(1042, 541);
      this.radio3D.Name = "radio3D";
      this.radio3D.Size = new System.Drawing.Size(31, 23);
      this.radio3D.TabIndex = 26;
      this.radio3D.TabStop = true;
      this.radio3D.Text = "3D";
      this.radio3D.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.radio3D.UseVisualStyleBackColor = true;
      // 
      // radio2D
      // 
      this.radio2D.Appearance = System.Windows.Forms.Appearance.Button;
      this.radio2D.AutoSize = true;
      this.radio2D.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.radio2D.Location = new System.Drawing.Point(1042, 565);
      this.radio2D.Name = "radio2D";
      this.radio2D.Size = new System.Drawing.Size(31, 23);
      this.radio2D.TabIndex = 25;
      this.radio2D.Text = "2D";
      this.radio2D.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.radio2D.UseVisualStyleBackColor = true;
      this.radio2D.CheckedChanged += new System.EventHandler(this.radio2D_CheckedChanged);
      // 
      // wireframeCheckBox
      // 
      this.wireframeCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
      this.wireframeCheckBox.AutoSize = true;
      this.wireframeCheckBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.wireframeCheckBox.Location = new System.Drawing.Point(1042, 589);
      this.wireframeCheckBox.Name = "wireframeCheckBox";
      this.wireframeCheckBox.Size = new System.Drawing.Size(31, 23);
      this.wireframeCheckBox.TabIndex = 27;
      this.wireframeCheckBox.Text = " W";
      this.wireframeCheckBox.UseVisualStyleBackColor = true;
      this.wireframeCheckBox.CheckedChanged += new System.EventHandler(this.wireframeCheckBox_CheckedChanged);
      // 
      // mapScreenshotButton
      // 
      this.mapScreenshotButton.Image = global::DSPRE.Properties.Resources.cameraIcon;
      this.mapScreenshotButton.Location = new System.Drawing.Point(1037, 498);
      this.mapScreenshotButton.Name = "mapScreenshotButton";
      this.mapScreenshotButton.Size = new System.Drawing.Size(41, 40);
      this.mapScreenshotButton.TabIndex = 39;
      this.mapScreenshotButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.mapScreenshotButton.UseVisualStyleBackColor = true;
      this.mapScreenshotButton.Click += new System.EventHandler(this.mapScreenshotButton_Click);
      // 
      // HeadbuttEncounterEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.Controls.Add(this.mapRenderPanel);
      this.Controls.Add(this.tabControl2);
      this.Controls.Add(this.mapScreenshotButton);
      this.Controls.Add(this.wireframeCheckBox);
      this.Controls.Add(this.textBoxPath);
      this.Controls.Add(this.buttonLoad);
      this.Controls.Add(this.radio2D);
      this.Controls.Add(this.buttonSaveAs);
      this.Controls.Add(this.buttonSave);
      this.Controls.Add(this.radio3D);
      this.Location = new System.Drawing.Point(15, 15);
      this.Name = "HeadbuttEncounterEditor";
      this.Size = new System.Drawing.Size(1083, 617);
      this.tabControl2.ResumeLayout(false);
      this.tabPageNormal.ResumeLayout(false);
      this.tabPageSpecial.ResumeLayout(false);
      this.mapRenderPanel.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.openGlPictureBox)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    private System.Windows.Forms.TextBox textBoxPath;
    private System.Windows.Forms.OpenFileDialog openFileDialog1;
    private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    private System.Windows.Forms.Button buttonLoad;
    private System.Windows.Forms.Button buttonSaveAs;
    private System.Windows.Forms.Button buttonSave;

        #endregion
    private System.Windows.Forms.TabControl tabControl2;
    private System.Windows.Forms.TabPage tabPageNormal;
    private System.Windows.Forms.TabPage tabPageSpecial;
    private HeadbuttEncounterEditorTab headbuttEncounterEditorTabNormal;
    private HeadbuttEncounterEditorTab headbuttEncounterEditorTabSpecial;
    private System.Windows.Forms.Panel mapRenderPanel;
    public SimpleOpenGlControl2 openGlControl;
    private System.Windows.Forms.PictureBox openGlPictureBox;
    private System.Windows.Forms.RadioButton radio3D;
    private System.Windows.Forms.RadioButton radio2D;
    private System.Windows.Forms.CheckBox wireframeCheckBox;
    private System.Windows.Forms.Button mapScreenshotButton;
  }
}

