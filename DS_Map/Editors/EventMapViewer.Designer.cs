
namespace DSPRE.Editors
{
  partial class EventMapViewer
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
      this.eventPanel = new System.Windows.Forms.Panel();
      this.openGlControl = new DSPRE.SimpleOpenGlControl2();
      this.openGlPictureBox = new System.Windows.Forms.PictureBox();
      this.eventShiftRightButton = new System.Windows.Forms.Button();
      this.eventShiftLeftButton = new System.Windows.Forms.Button();
      this.eventShiftDownButton = new System.Windows.Forms.Button();
      this.eventShiftUpButton = new System.Windows.Forms.Button();
      this.eventPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.openGlPictureBox)).BeginInit();
      this.SuspendLayout();
      // 
      // eventPanel
      // 
      this.eventPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.eventPanel.Controls.Add(this.openGlControl);
      this.eventPanel.Controls.Add(this.openGlPictureBox);
      this.eventPanel.Location = new System.Drawing.Point(36, 34);
      this.eventPanel.Name = "eventPanel";
      this.eventPanel.Size = new System.Drawing.Size(546, 546);
      this.eventPanel.TabIndex = 24;
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
      this.openGlControl.Size = new System.Drawing.Size(544, 544);
      this.openGlControl.StencilBits = ((byte)(0));
      this.openGlControl.TabIndex = 2;
      // 
      // openGlPictureBox
      // 
      this.openGlPictureBox.BackColor = System.Drawing.Color.White;
      this.openGlPictureBox.Location = new System.Drawing.Point(0, 0);
      this.openGlPictureBox.Name = "openGlPictureBox";
      this.openGlPictureBox.Size = new System.Drawing.Size(544, 544);
      this.openGlPictureBox.TabIndex = 3;
      this.openGlPictureBox.TabStop = false;
      this.openGlPictureBox.Click += new System.EventHandler(this.eventPictureBox_Click);
      this.openGlPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.eventPictureBox_MouseMove);
      // 
      // eventShiftRightButton
      // 
      this.eventShiftRightButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
      this.eventShiftRightButton.Image = global::DSPRE.Properties.Resources.arrowright;
      this.eventShiftRightButton.Location = new System.Drawing.Point(587, 228);
      this.eventShiftRightButton.Name = "eventShiftRightButton";
      this.eventShiftRightButton.Size = new System.Drawing.Size(25, 156);
      this.eventShiftRightButton.TabIndex = 45;
      this.eventShiftRightButton.UseVisualStyleBackColor = true;
      this.eventShiftRightButton.Click += new System.EventHandler(this.eventShiftRightButton_Click);
      // 
      // eventShiftLeftButton
      // 
      this.eventShiftLeftButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
      this.eventShiftLeftButton.Image = global::DSPRE.Properties.Resources.arrowleft;
      this.eventShiftLeftButton.Location = new System.Drawing.Point(5, 228);
      this.eventShiftLeftButton.Name = "eventShiftLeftButton";
      this.eventShiftLeftButton.Size = new System.Drawing.Size(25, 156);
      this.eventShiftLeftButton.TabIndex = 44;
      this.eventShiftLeftButton.UseVisualStyleBackColor = true;
      this.eventShiftLeftButton.Click += new System.EventHandler(this.eventShiftLeftButton_Click);
      // 
      // eventShiftDownButton
      // 
      this.eventShiftDownButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
      this.eventShiftDownButton.Image = global::DSPRE.Properties.Resources.arrowdown;
      this.eventShiftDownButton.Location = new System.Drawing.Point(230, 584);
      this.eventShiftDownButton.Name = "eventShiftDownButton";
      this.eventShiftDownButton.Size = new System.Drawing.Size(156, 25);
      this.eventShiftDownButton.TabIndex = 42;
      this.eventShiftDownButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
      this.eventShiftDownButton.UseVisualStyleBackColor = true;
      this.eventShiftDownButton.Click += new System.EventHandler(this.eventShiftDownButton_Click);
      // 
      // eventShiftUpButton
      // 
      this.eventShiftUpButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
      this.eventShiftUpButton.Image = global::DSPRE.Properties.Resources.arrowup;
      this.eventShiftUpButton.Location = new System.Drawing.Point(230, 3);
      this.eventShiftUpButton.Name = "eventShiftUpButton";
      this.eventShiftUpButton.Size = new System.Drawing.Size(156, 25);
      this.eventShiftUpButton.TabIndex = 43;
      this.eventShiftUpButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
      this.eventShiftUpButton.UseVisualStyleBackColor = true;
      this.eventShiftUpButton.Click += new System.EventHandler(this.eventShiftUpButton_Click);
      // 
      // EventMapViewer
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.eventPanel);
      this.Controls.Add(this.eventShiftRightButton);
      this.Controls.Add(this.eventShiftUpButton);
      this.Controls.Add(this.eventShiftLeftButton);
      this.Controls.Add(this.eventShiftDownButton);
      this.Name = "EventMapViewer";
      this.Size = new System.Drawing.Size(617, 612);
      this.eventPanel.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.openGlPictureBox)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel eventPanel;
    private SimpleOpenGlControl2 openGlControl;
    private System.Windows.Forms.PictureBox openGlPictureBox;
    private System.Windows.Forms.Button eventShiftRightButton;
    private System.Windows.Forms.Button eventShiftLeftButton;
    private System.Windows.Forms.Button eventShiftDownButton;
    private System.Windows.Forms.Button eventShiftUpButton;
  }
}
