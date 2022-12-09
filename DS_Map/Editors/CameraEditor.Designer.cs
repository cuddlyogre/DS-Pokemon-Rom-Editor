﻿
namespace DSPRE.Editors
{
  partial class CameraEditor
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
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      this.cameraEditorDataGridView = new System.Windows.Forms.DataGridView();
      this.DistanceGVCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.VertRotGVCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.HoriRotGVCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.zRotGVCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.OrthoGVCol = new System.Windows.Forms.DataGridViewCheckBoxColumn();
      this.FovGVCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.NearClipGVCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.FarClipGVCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.XDispGVCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.YDispGVCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.ZDispGVCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.ExportBTN = new System.Windows.Forms.DataGridViewButtonColumn();
      this.ImportBTN = new System.Windows.Forms.DataGridViewButtonColumn();
      this.saveCameraTableButton = new System.Windows.Forms.Button();
      this.importCameraTableButton = new System.Windows.Forms.Button();
      this.exportCameraTableButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.cameraEditorDataGridView)).BeginInit();
      this.SuspendLayout();
      // 
      // cameraEditorDataGridView
      // 
      this.cameraEditorDataGridView.AllowUserToAddRows = false;
      this.cameraEditorDataGridView.AllowUserToDeleteRows = false;
      this.cameraEditorDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
      this.cameraEditorDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlDarkDark;
      this.cameraEditorDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.cameraEditorDataGridView.CausesValidation = false;
      this.cameraEditorDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
      this.cameraEditorDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
      dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.cameraEditorDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
      this.cameraEditorDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.cameraEditorDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DistanceGVCol,
            this.VertRotGVCol,
            this.HoriRotGVCol,
            this.zRotGVCol,
            this.OrthoGVCol,
            this.FovGVCol,
            this.NearClipGVCol,
            this.FarClipGVCol,
            this.XDispGVCol,
            this.YDispGVCol,
            this.ZDispGVCol,
            this.ExportBTN,
            this.ImportBTN});
      this.cameraEditorDataGridView.Location = new System.Drawing.Point(3, 3);
      this.cameraEditorDataGridView.MultiSelect = false;
      this.cameraEditorDataGridView.Name = "cameraEditorDataGridView";
      this.cameraEditorDataGridView.RowHeadersWidth = 60;
      this.cameraEditorDataGridView.RowTemplate.DividerHeight = 1;
      this.cameraEditorDataGridView.RowTemplate.Height = 32;
      this.cameraEditorDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.None;
      this.cameraEditorDataGridView.Size = new System.Drawing.Size(1172, 551);
      this.cameraEditorDataGridView.TabIndex = 0;
      this.cameraEditorDataGridView.TabStop = false;
      this.cameraEditorDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.cameraEditorDataGridView_CellContentClick);
      this.cameraEditorDataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.cameraEditorDataGridView_CellValidated);
      // 
      // DistanceGVCol
      // 
      this.DistanceGVCol.FillWeight = 44.49141F;
      this.DistanceGVCol.HeaderText = "Distance";
      this.DistanceGVCol.Name = "DistanceGVCol";
      this.DistanceGVCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      // 
      // VertRotGVCol
      // 
      this.VertRotGVCol.FillWeight = 28.66745F;
      this.VertRotGVCol.HeaderText = "Vertical Rotation";
      this.VertRotGVCol.Name = "VertRotGVCol";
      this.VertRotGVCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      // 
      // HoriRotGVCol
      // 
      this.HoriRotGVCol.FillWeight = 28.66745F;
      this.HoriRotGVCol.HeaderText = "Horizontal Rotation";
      this.HoriRotGVCol.Name = "HoriRotGVCol";
      this.HoriRotGVCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      // 
      // zRotGVCol
      // 
      this.zRotGVCol.FillWeight = 28F;
      this.zRotGVCol.HeaderText = "Z Rotation";
      this.zRotGVCol.Name = "zRotGVCol";
      this.zRotGVCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      // 
      // OrthoGVCol
      // 
      this.OrthoGVCol.FillWeight = 44.49141F;
      this.OrthoGVCol.HeaderText = "Orthographic";
      this.OrthoGVCol.Name = "OrthoGVCol";
      // 
      // FovGVCol
      // 
      this.FovGVCol.FillWeight = 19.11163F;
      this.FovGVCol.HeaderText = "FOV";
      this.FovGVCol.Name = "FovGVCol";
      this.FovGVCol.Resizable = System.Windows.Forms.DataGridViewTriState.True;
      this.FovGVCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      // 
      // NearClipGVCol
      // 
      this.NearClipGVCol.FillWeight = 44.49141F;
      this.NearClipGVCol.HeaderText = "Near Clip Distance";
      this.NearClipGVCol.Name = "NearClipGVCol";
      this.NearClipGVCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      // 
      // FarClipGVCol
      // 
      this.FarClipGVCol.FillWeight = 44.49141F;
      this.FarClipGVCol.HeaderText = "Far Clip Distance";
      this.FarClipGVCol.Name = "FarClipGVCol";
      this.FarClipGVCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      // 
      // XDispGVCol
      // 
      this.XDispGVCol.FillWeight = 44.49141F;
      this.XDispGVCol.HeaderText = "X Displacement";
      this.XDispGVCol.Name = "XDispGVCol";
      this.XDispGVCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      // 
      // YDispGVCol
      // 
      this.YDispGVCol.FillWeight = 44.49141F;
      this.YDispGVCol.HeaderText = "Y Displacement";
      this.YDispGVCol.Name = "YDispGVCol";
      this.YDispGVCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      // 
      // ZDispGVCol
      // 
      this.ZDispGVCol.FillWeight = 44.49141F;
      this.ZDispGVCol.HeaderText = "Z Displacement";
      this.ZDispGVCol.Name = "ZDispGVCol";
      this.ZDispGVCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      // 
      // ExportBTN
      // 
      this.ExportBTN.FillWeight = 44.49141F;
      this.ExportBTN.HeaderText = "";
      this.ExportBTN.Name = "ExportBTN";
      this.ExportBTN.Text = "Export";
      this.ExportBTN.UseColumnTextForButtonValue = true;
      // 
      // ImportBTN
      // 
      this.ImportBTN.FillWeight = 44.49141F;
      this.ImportBTN.HeaderText = "";
      this.ImportBTN.Name = "ImportBTN";
      this.ImportBTN.Text = "Import";
      this.ImportBTN.UseColumnTextForButtonValue = true;
      // 
      // saveCameraTableButton
      // 
      this.saveCameraTableButton.Image = global::DSPRE.Properties.Resources.save_rom;
      this.saveCameraTableButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.saveCameraTableButton.Location = new System.Drawing.Point(1058, 562);
      this.saveCameraTableButton.Name = "saveCameraTableButton";
      this.saveCameraTableButton.Size = new System.Drawing.Size(115, 45);
      this.saveCameraTableButton.TabIndex = 15;
      this.saveCameraTableButton.Text = "&Save \r\nCam Table";
      this.saveCameraTableButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.saveCameraTableButton.UseVisualStyleBackColor = true;
      this.saveCameraTableButton.Click += new System.EventHandler(this.saveCameraTableButton_Click);
      // 
      // importCameraTableButton
      // 
      this.importCameraTableButton.Image = global::DSPRE.Properties.Resources.importArrow;
      this.importCameraTableButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.importCameraTableButton.Location = new System.Drawing.Point(121, 562);
      this.importCameraTableButton.Name = "importCameraTableButton";
      this.importCameraTableButton.Size = new System.Drawing.Size(115, 45);
      this.importCameraTableButton.TabIndex = 14;
      this.importCameraTableButton.Text = "Import\r\nCamera Table";
      this.importCameraTableButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.importCameraTableButton.UseVisualStyleBackColor = true;
      this.importCameraTableButton.Click += new System.EventHandler(this.importCameraTableButton_Click);
      // 
      // exportCameraTableButton
      // 
      this.exportCameraTableButton.Image = global::DSPRE.Properties.Resources.exportArrow;
      this.exportCameraTableButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.exportCameraTableButton.Location = new System.Drawing.Point(1, 562);
      this.exportCameraTableButton.Name = "exportCameraTableButton";
      this.exportCameraTableButton.Size = new System.Drawing.Size(115, 45);
      this.exportCameraTableButton.TabIndex = 16;
      this.exportCameraTableButton.Text = "Export \r\nCamera Table";
      this.exportCameraTableButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.exportCameraTableButton.UseVisualStyleBackColor = true;
      this.exportCameraTableButton.Click += new System.EventHandler(this.exportCameraTableButton_Click);
      // 
      // CameraEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.exportCameraTableButton);
      this.Controls.Add(this.cameraEditorDataGridView);
      this.Controls.Add(this.importCameraTableButton);
      this.Controls.Add(this.saveCameraTableButton);
      this.Name = "CameraEditor";
      this.Size = new System.Drawing.Size(1179, 611);
      ((System.ComponentModel.ISupportInitialize)(this.cameraEditorDataGridView)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridView cameraEditorDataGridView;
    private System.Windows.Forms.DataGridViewTextBoxColumn DistanceGVCol;
    private System.Windows.Forms.DataGridViewTextBoxColumn VertRotGVCol;
    private System.Windows.Forms.DataGridViewTextBoxColumn HoriRotGVCol;
    private System.Windows.Forms.DataGridViewTextBoxColumn zRotGVCol;
    private System.Windows.Forms.DataGridViewCheckBoxColumn OrthoGVCol;
    private System.Windows.Forms.DataGridViewTextBoxColumn FovGVCol;
    private System.Windows.Forms.DataGridViewTextBoxColumn NearClipGVCol;
    private System.Windows.Forms.DataGridViewTextBoxColumn FarClipGVCol;
    private System.Windows.Forms.DataGridViewTextBoxColumn XDispGVCol;
    private System.Windows.Forms.DataGridViewTextBoxColumn YDispGVCol;
    private System.Windows.Forms.DataGridViewTextBoxColumn ZDispGVCol;
    private System.Windows.Forms.DataGridViewButtonColumn ExportBTN;
    private System.Windows.Forms.DataGridViewButtonColumn ImportBTN;
    private System.Windows.Forms.Button saveCameraTableButton;
    private System.Windows.Forms.Button importCameraTableButton;
    private System.Windows.Forms.Button exportCameraTableButton;
  }
}
