using System.ComponentModel;

namespace DSPRE {
  partial class LevelScriptEditor {
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
      this.buttonLoad = new System.Windows.Forms.Button();
      this.buttonSave = new System.Windows.Forms.Button();
      this.listBoxTriggers = new DSPRE.ListBox2();
      this.textBoxPath = new System.Windows.Forms.TextBox();
      this.buttonSaveAs = new System.Windows.Forms.Button();
      this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
      this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
      this.radioButtonVariableValue = new System.Windows.Forms.RadioButton();
      this.radioButtonMapChange = new System.Windows.Forms.RadioButton();
      this.radioButtonScreenReset = new System.Windows.Forms.RadioButton();
      this.radioButtonLoadGame = new System.Windows.Forms.RadioButton();
      this.textBoxScriptID = new System.Windows.Forms.TextBox();
      this.textBoxVariableName = new System.Windows.Forms.TextBox();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.groupBoxVariable = new System.Windows.Forms.GroupBox();
      this.groupBoxValue = new System.Windows.Forms.GroupBox();
      this.textBoxVariableValue = new System.Windows.Forms.TextBox();
      this.checkBox1 = new System.Windows.Forms.CheckBox();
      this.buttonAdd = new System.Windows.Forms.Button();
      this.buttonRemove = new System.Windows.Forms.Button();
      this.radioButtonDecimal = new System.Windows.Forms.RadioButton();
      this.radioButtonHex = new System.Windows.Forms.RadioButton();
      this.radioButtonAuto = new System.Windows.Forms.RadioButton();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.groupBoxVariable.SuspendLayout();
      this.groupBoxValue.SuspendLayout();
      this.SuspendLayout();
      // 
      // buttonLoad
      // 
      this.buttonLoad.Location = new System.Drawing.Point(3, 3);
      this.buttonLoad.Name = "buttonLoad";
      this.buttonLoad.Size = new System.Drawing.Size(75, 23);
      this.buttonLoad.TabIndex = 0;
      this.buttonLoad.Text = "Load";
      this.buttonLoad.UseVisualStyleBackColor = true;
      this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
      // 
      // buttonSave
      // 
      this.buttonSave.Location = new System.Drawing.Point(97, 3);
      this.buttonSave.Name = "buttonSave";
      this.buttonSave.Size = new System.Drawing.Size(75, 23);
      this.buttonSave.TabIndex = 3;
      this.buttonSave.Text = "Save";
      this.buttonSave.UseVisualStyleBackColor = true;
      this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
      // 
      // listBoxTriggers
      // 
      this.listBoxTriggers.FormattingEnabled = true;
      this.listBoxTriggers.Location = new System.Drawing.Point(3, 81);
      this.listBoxTriggers.Name = "listBoxTriggers";
      this.listBoxTriggers.Size = new System.Drawing.Size(397, 134);
      this.listBoxTriggers.TabIndex = 4;
      this.listBoxTriggers.SelectedIndexChanged += new System.EventHandler(this.listBoxTriggers_SelectedIndexChanged);
      // 
      // textBoxPath
      // 
      this.textBoxPath.Location = new System.Drawing.Point(3, 32);
      this.textBoxPath.Name = "textBoxPath";
      this.textBoxPath.ReadOnly = true;
      this.textBoxPath.Size = new System.Drawing.Size(397, 20);
      this.textBoxPath.TabIndex = 7;
      // 
      // buttonSaveAs
      // 
      this.buttonSaveAs.Location = new System.Drawing.Point(178, 3);
      this.buttonSaveAs.Name = "buttonSaveAs";
      this.buttonSaveAs.Size = new System.Drawing.Size(75, 23);
      this.buttonSaveAs.TabIndex = 3;
      this.buttonSaveAs.Text = "Save As";
      this.buttonSaveAs.UseVisualStyleBackColor = true;
      this.buttonSaveAs.Click += new System.EventHandler(this.buttonSaveAs_Click);
      // 
      // openFileDialog1
      // 
      this.openFileDialog1.FileName = "openFileDialog1";
      // 
      // radioButtonVariableValue
      // 
      this.radioButtonVariableValue.AutoSize = true;
      this.radioButtonVariableValue.Checked = true;
      this.radioButtonVariableValue.Location = new System.Drawing.Point(6, 19);
      this.radioButtonVariableValue.Name = "radioButtonVariableValue";
      this.radioButtonVariableValue.Size = new System.Drawing.Size(107, 17);
      this.radioButtonVariableValue.TabIndex = 8;
      this.radioButtonVariableValue.TabStop = true;
      this.radioButtonVariableValue.Text = "Variable == value";
      this.radioButtonVariableValue.UseVisualStyleBackColor = true;
      this.radioButtonVariableValue.CheckedChanged += new System.EventHandler(this.radioButtonVariableValue_CheckedChanged);
      // 
      // radioButtonMapChange
      // 
      this.radioButtonMapChange.AutoSize = true;
      this.radioButtonMapChange.Location = new System.Drawing.Point(119, 19);
      this.radioButtonMapChange.Name = "radioButtonMapChange";
      this.radioButtonMapChange.Size = new System.Drawing.Size(86, 17);
      this.radioButtonMapChange.TabIndex = 8;
      this.radioButtonMapChange.Text = "Player enters";
      this.radioButtonMapChange.UseVisualStyleBackColor = true;
      this.radioButtonMapChange.CheckedChanged += new System.EventHandler(this.radioButtonMapChange_CheckedChanged);
      // 
      // radioButtonScreenReset
      // 
      this.radioButtonScreenReset.AutoSize = true;
      this.radioButtonScreenReset.Location = new System.Drawing.Point(211, 19);
      this.radioButtonScreenReset.Name = "radioButtonScreenReset";
      this.radioButtonScreenReset.Size = new System.Drawing.Size(88, 17);
      this.radioButtonScreenReset.TabIndex = 8;
      this.radioButtonScreenReset.Text = "Screen fades";
      this.radioButtonScreenReset.UseVisualStyleBackColor = true;
      this.radioButtonScreenReset.CheckedChanged += new System.EventHandler(this.radioButtonScreenReset_CheckedChanged);
      // 
      // radioButtonLoadGame
      // 
      this.radioButtonLoadGame.AutoSize = true;
      this.radioButtonLoadGame.Location = new System.Drawing.Point(305, 19);
      this.radioButtonLoadGame.Name = "radioButtonLoadGame";
      this.radioButtonLoadGame.Size = new System.Drawing.Size(81, 17);
      this.radioButtonLoadGame.TabIndex = 8;
      this.radioButtonLoadGame.Text = "Game loads";
      this.radioButtonLoadGame.UseVisualStyleBackColor = true;
      this.radioButtonLoadGame.CheckedChanged += new System.EventHandler(this.radioButtonLoadGame_CheckedChanged);
      // 
      // textBoxScriptID
      // 
      this.textBoxScriptID.Location = new System.Drawing.Point(6, 19);
      this.textBoxScriptID.Name = "textBoxScriptID";
      this.textBoxScriptID.Size = new System.Drawing.Size(116, 20);
      this.textBoxScriptID.TabIndex = 9;
      // 
      // textBoxVariableName
      // 
      this.textBoxVariableName.Location = new System.Drawing.Point(6, 19);
      this.textBoxVariableName.Name = "textBoxVariableName";
      this.textBoxVariableName.Size = new System.Drawing.Size(116, 20);
      this.textBoxVariableName.TabIndex = 9;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.radioButtonVariableValue);
      this.groupBox1.Controls.Add(this.radioButtonMapChange);
      this.groupBox1.Controls.Add(this.radioButtonScreenReset);
      this.groupBox1.Controls.Add(this.radioButtonLoadGame);
      this.groupBox1.Location = new System.Drawing.Point(3, 221);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(397, 48);
      this.groupBox1.TabIndex = 11;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "When this happens in the map:";
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.textBoxScriptID);
      this.groupBox2.Location = new System.Drawing.Point(3, 275);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(128, 48);
      this.groupBox2.TabIndex = 12;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Run this script ID";
      // 
      // groupBoxVariable
      // 
      this.groupBoxVariable.Controls.Add(this.textBoxVariableName);
      this.groupBoxVariable.Location = new System.Drawing.Point(137, 275);
      this.groupBoxVariable.Name = "groupBoxVariable";
      this.groupBoxVariable.Size = new System.Drawing.Size(128, 48);
      this.groupBoxVariable.TabIndex = 13;
      this.groupBoxVariable.TabStop = false;
      this.groupBoxVariable.Text = "when this variable ==";
      // 
      // groupBoxValue
      // 
      this.groupBoxValue.Controls.Add(this.textBoxVariableValue);
      this.groupBoxValue.Location = new System.Drawing.Point(271, 275);
      this.groupBoxValue.Name = "groupBoxValue";
      this.groupBoxValue.Size = new System.Drawing.Size(129, 48);
      this.groupBoxValue.TabIndex = 13;
      this.groupBoxValue.TabStop = false;
      this.groupBoxValue.Text = "this value";
      // 
      // textBoxVariableValue
      // 
      this.textBoxVariableValue.Location = new System.Drawing.Point(6, 19);
      this.textBoxVariableValue.Name = "textBoxVariableValue";
      this.textBoxVariableValue.Size = new System.Drawing.Size(117, 20);
      this.textBoxVariableValue.TabIndex = 9;
      // 
      // checkBox1
      // 
      this.checkBox1.AutoSize = true;
      this.checkBox1.Location = new System.Drawing.Point(259, 7);
      this.checkBox1.Name = "checkBox1";
      this.checkBox1.Size = new System.Drawing.Size(141, 17);
      this.checkBox1.TabIndex = 14;
      this.checkBox1.Text = "Word-alignment padding";
      this.checkBox1.UseVisualStyleBackColor = true;
      // 
      // buttonAdd
      // 
      this.buttonAdd.Location = new System.Drawing.Point(244, 329);
      this.buttonAdd.Name = "buttonAdd";
      this.buttonAdd.Size = new System.Drawing.Size(75, 23);
      this.buttonAdd.TabIndex = 15;
      this.buttonAdd.Text = "Add";
      this.buttonAdd.UseVisualStyleBackColor = true;
      this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
      // 
      // buttonRemove
      // 
      this.buttonRemove.Enabled = false;
      this.buttonRemove.Location = new System.Drawing.Point(325, 329);
      this.buttonRemove.Name = "buttonRemove";
      this.buttonRemove.Size = new System.Drawing.Size(75, 23);
      this.buttonRemove.TabIndex = 15;
      this.buttonRemove.Text = "Remove";
      this.buttonRemove.UseVisualStyleBackColor = true;
      this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
      // 
      // radioButtonDecimal
      // 
      this.radioButtonDecimal.AutoSize = true;
      this.radioButtonDecimal.Location = new System.Drawing.Point(106, 58);
      this.radioButtonDecimal.Name = "radioButtonDecimal";
      this.radioButtonDecimal.Size = new System.Drawing.Size(63, 17);
      this.radioButtonDecimal.TabIndex = 8;
      this.radioButtonDecimal.Text = "Decimal";
      this.radioButtonDecimal.UseVisualStyleBackColor = true;
      this.radioButtonDecimal.CheckedChanged += new System.EventHandler(this.radioButtonDecimal_CheckedChanged);
      // 
      // radioButtonHex
      // 
      this.radioButtonHex.AutoSize = true;
      this.radioButtonHex.Location = new System.Drawing.Point(56, 58);
      this.radioButtonHex.Name = "radioButtonHex";
      this.radioButtonHex.Size = new System.Drawing.Size(44, 17);
      this.radioButtonHex.TabIndex = 8;
      this.radioButtonHex.Text = "Hex";
      this.radioButtonHex.UseVisualStyleBackColor = true;
      this.radioButtonHex.CheckedChanged += new System.EventHandler(this.radioButtonHex_CheckedChanged);
      // 
      // radioButtonAuto
      // 
      this.radioButtonAuto.AutoSize = true;
      this.radioButtonAuto.Checked = true;
      this.radioButtonAuto.Location = new System.Drawing.Point(3, 58);
      this.radioButtonAuto.Name = "radioButtonAuto";
      this.radioButtonAuto.Size = new System.Drawing.Size(47, 17);
      this.radioButtonAuto.TabIndex = 8;
      this.radioButtonAuto.TabStop = true;
      this.radioButtonAuto.Text = "Auto";
      this.radioButtonAuto.UseVisualStyleBackColor = true;
      this.radioButtonAuto.CheckedChanged += new System.EventHandler(this.radioButtonAuto_CheckedChanged);
      // 
      // LevelScriptEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.Controls.Add(this.radioButtonAuto);
      this.Controls.Add(this.radioButtonHex);
      this.Controls.Add(this.buttonRemove);
      this.Controls.Add(this.radioButtonDecimal);
      this.Controls.Add(this.buttonAdd);
      this.Controls.Add(this.checkBox1);
      this.Controls.Add(this.groupBoxValue);
      this.Controls.Add(this.groupBoxVariable);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.textBoxPath);
      this.Controls.Add(this.buttonLoad);
      this.Controls.Add(this.buttonSaveAs);
      this.Controls.Add(this.buttonSave);
      this.Controls.Add(this.listBoxTriggers);
      this.Location = new System.Drawing.Point(15, 15);
      this.Name = "LevelScriptEditor";
      this.Size = new System.Drawing.Size(407, 359);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.groupBoxVariable.ResumeLayout(false);
      this.groupBoxVariable.PerformLayout();
      this.groupBoxValue.ResumeLayout(false);
      this.groupBoxValue.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    #endregion

        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Button buttonSave;
        private DSPRE.ListBox2 listBoxTriggers;
        private System.Windows.Forms.TextBox textBoxPath;
        private System.Windows.Forms.Button buttonSaveAs;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    private System.Windows.Forms.RadioButton radioButtonVariableValue;
    private System.Windows.Forms.RadioButton radioButtonMapChange;
    private System.Windows.Forms.RadioButton radioButtonScreenReset;
    private System.Windows.Forms.RadioButton radioButtonLoadGame;
    private System.Windows.Forms.TextBox textBoxScriptID;
    private System.Windows.Forms.TextBox textBoxVariableName;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.GroupBox groupBoxVariable;
    private System.Windows.Forms.GroupBox groupBoxValue;
    private System.Windows.Forms.TextBox textBoxVariableValue;
    private System.Windows.Forms.CheckBox checkBox1;
    private System.Windows.Forms.Button buttonAdd;
    private System.Windows.Forms.Button buttonRemove;
    private System.Windows.Forms.RadioButton radioButtonDecimal;
    private System.Windows.Forms.RadioButton radioButtonHex;
    private System.Windows.Forms.RadioButton radioButtonAuto;
  }
}

