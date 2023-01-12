
namespace DSPRE.Editors
{
  partial class SafariZoneEditor
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.comboBoxFileID = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
      // 
      // comboBoxFileID
      // 
      this.comboBoxFileID.FormattingEnabled = true;
      this.comboBoxFileID.Location = new System.Drawing.Point(4, 4);
      this.comboBoxFileID.Name = "comboBoxFileID";
      this.comboBoxFileID.Size = new System.Drawing.Size(121, 21);
      this.comboBoxFileID.TabIndex = 0;
      this.comboBoxFileID.SelectedIndexChanged += new System.EventHandler(this.comboBoxFileID_SelectedIndexChanged);
      // 
      // SafariZoneEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.comboBoxFileID);
      this.Name = "SafariZoneEditor";
      this.Size = new System.Drawing.Size(1042, 616);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ComboBox comboBoxFileID;
  }
}
