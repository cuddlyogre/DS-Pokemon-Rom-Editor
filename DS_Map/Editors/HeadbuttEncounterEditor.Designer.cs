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
      this.tabControl2.SuspendLayout();
      this.tabPageNormal.SuspendLayout();
      this.tabPageSpecial.SuspendLayout();
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
      this.tabControl2.Size = new System.Drawing.Size(413, 548);
      this.tabControl2.TabIndex = 21;
      // 
      // tabPageNormal
      // 
      this.tabPageNormal.Controls.Add(this.headbuttEncounterEditorTabNormal);
      this.tabPageNormal.Location = new System.Drawing.Point(4, 22);
      this.tabPageNormal.Name = "tabPageNormal";
      this.tabPageNormal.Padding = new System.Windows.Forms.Padding(3);
      this.tabPageNormal.Size = new System.Drawing.Size(405, 522);
      this.tabPageNormal.TabIndex = 0;
      this.tabPageNormal.Text = "Normal";
      this.tabPageNormal.UseVisualStyleBackColor = true;
      // 
      // headbuttEncounterEditorTabNormal
      // 
      this.headbuttEncounterEditorTabNormal.Location = new System.Drawing.Point(0, 0);
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
      this.tabPageSpecial.Size = new System.Drawing.Size(405, 522);
      this.tabPageSpecial.TabIndex = 1;
      this.tabPageSpecial.Text = "Special";
      this.tabPageSpecial.UseVisualStyleBackColor = true;
      // 
      // headbuttEncounterEditorTabSpecial
      // 
      this.headbuttEncounterEditorTabSpecial.Location = new System.Drawing.Point(0, 0);
      this.headbuttEncounterEditorTabSpecial.Name = "headbuttEncounterEditorTabSpecial";
      this.headbuttEncounterEditorTabSpecial.Size = new System.Drawing.Size(402, 519);
      this.headbuttEncounterEditorTabSpecial.TabIndex = 0;
      // 
      // HeadbuttEncounterEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.Controls.Add(this.tabControl2);
      this.Controls.Add(this.textBoxPath);
      this.Controls.Add(this.buttonLoad);
      this.Controls.Add(this.buttonSaveAs);
      this.Controls.Add(this.buttonSave);
      this.Location = new System.Drawing.Point(15, 15);
      this.Name = "HeadbuttEncounterEditor";
      this.Size = new System.Drawing.Size(420, 611);
      this.tabControl2.ResumeLayout(false);
      this.tabPageNormal.ResumeLayout(false);
      this.tabPageSpecial.ResumeLayout(false);
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
  }
}

