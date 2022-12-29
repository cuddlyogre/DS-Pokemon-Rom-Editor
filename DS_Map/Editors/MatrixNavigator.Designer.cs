
namespace DSPRE.Editors
{
  partial class MatrixNavigator
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
      this.eventMatrixPanel = new System.Windows.Forms.Panel();
      this.eventMatrixPictureBox = new System.Windows.Forms.PictureBox();
      this.matrixNavigatorLabel = new System.Windows.Forms.Label();
      this.eventMatrixPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.eventMatrixPictureBox)).BeginInit();
      this.SuspendLayout();
      // 
      // eventMatrixPanel
      // 
      this.eventMatrixPanel.AutoScroll = true;
      this.eventMatrixPanel.BackColor = System.Drawing.SystemColors.Menu;
      this.eventMatrixPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.eventMatrixPanel.Controls.Add(this.eventMatrixPictureBox);
      this.eventMatrixPanel.Location = new System.Drawing.Point(3, 15);
      this.eventMatrixPanel.Name = "eventMatrixPanel";
      this.eventMatrixPanel.Size = new System.Drawing.Size(448, 149);
      this.eventMatrixPanel.TabIndex = 47;
      // 
      // eventMatrixPictureBox
      // 
      this.eventMatrixPictureBox.Location = new System.Drawing.Point(0, -1);
      this.eventMatrixPictureBox.Name = "eventMatrixPictureBox";
      this.eventMatrixPictureBox.Size = new System.Drawing.Size(440, 150);
      this.eventMatrixPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      this.eventMatrixPictureBox.TabIndex = 46;
      this.eventMatrixPictureBox.TabStop = false;
      this.eventMatrixPictureBox.Click += new System.EventHandler(this.eventMatrixPictureBox_Click);
      // 
      // matrixNavigatorLabel
      // 
      this.matrixNavigatorLabel.AutoSize = true;
      this.matrixNavigatorLabel.Location = new System.Drawing.Point(2, 1);
      this.matrixNavigatorLabel.Name = "matrixNavigatorLabel";
      this.matrixNavigatorLabel.Size = new System.Drawing.Size(84, 13);
      this.matrixNavigatorLabel.TabIndex = 48;
      this.matrixNavigatorLabel.Text = "Matrix Navigator";
      // 
      // MatrixNavigator
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.eventMatrixPanel);
      this.Controls.Add(this.matrixNavigatorLabel);
      this.Name = "MatrixNavigator";
      this.Size = new System.Drawing.Size(454, 166);
      this.eventMatrixPanel.ResumeLayout(false);
      this.eventMatrixPanel.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.eventMatrixPictureBox)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel eventMatrixPanel;
    private System.Windows.Forms.PictureBox eventMatrixPictureBox;
    private System.Windows.Forms.Label matrixNavigatorLabel;
  }
}
