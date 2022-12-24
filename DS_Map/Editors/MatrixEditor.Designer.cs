﻿
namespace DSPRE.Editors
{
  partial class MatrixEditor
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
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MatrixEditor));
      this.saveMatrixButton = new System.Windows.Forms.Button();
      this.selectMatrixComboBox = new System.Windows.Forms.ComboBox();
      this.widthLabel = new System.Windows.Forms.Label();
      this.widthUpDown = new System.Windows.Forms.NumericUpDown();
      this.heightUpDown = new System.Windows.Forms.NumericUpDown();
      this.matrixNameTextBox = new System.Windows.Forms.TextBox();
      this.matrixTabControl = new System.Windows.Forms.TabControl();
      this.headersTabPage = new System.Windows.Forms.TabPage();
      this.headersGridView = new System.Windows.Forms.DataGridView();
      this.heightsTabPage = new System.Windows.Forms.TabPage();
      this.heightsGridView = new System.Windows.Forms.DataGridView();
      this.mapFilesTabPage = new System.Windows.Forms.TabPage();
      this.mapFilesGridView = new System.Windows.Forms.DataGridView();
      this.matrixNameLabel = new System.Windows.Forms.Label();
      this.labelMatrices = new System.Windows.Forms.Label();
      this.addHeadersButton = new System.Windows.Forms.Button();
      this.addHeightsButton = new System.Windows.Forms.Button();
      this.removeHeadersButton = new System.Windows.Forms.Button();
      this.removeHeightsButton = new System.Windows.Forms.Button();
      this.addMatrixButton = new System.Windows.Forms.Button();
      this.removeMatrixButton = new System.Windows.Forms.Button();
      this.exportMatrixButton = new System.Windows.Forms.Button();
      this.importMatrixButton = new System.Windows.Forms.Button();
      this.importColorTableButton = new System.Windows.Forms.Button();
      this.resetColorTableButton = new System.Windows.Forms.Button();
      this.setSpawnPointButton = new System.Windows.Forms.Button();
      this.locateCurrentMatrixFile = new System.Windows.Forms.Button();
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.widthUpDown)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.heightUpDown)).BeginInit();
      this.matrixTabControl.SuspendLayout();
      this.headersTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.headersGridView)).BeginInit();
      this.heightsTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.heightsGridView)).BeginInit();
      this.mapFilesTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.mapFilesGridView)).BeginInit();
      this.SuspendLayout();
      // 
      // saveMatrixButton
      // 
      this.saveMatrixButton.Image = global::DSPRE.Properties.Resources.save_rom;
      this.saveMatrixButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.saveMatrixButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.saveMatrixButton.Location = new System.Drawing.Point(5, 558);
      this.saveMatrixButton.Name = "saveMatrixButton";
      this.saveMatrixButton.Size = new System.Drawing.Size(117, 43);
      this.saveMatrixButton.TabIndex = 22;
      this.saveMatrixButton.Text = "Save Matrix";
      this.saveMatrixButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.saveMatrixButton.UseVisualStyleBackColor = true;
      this.saveMatrixButton.Click += new System.EventHandler(this.saveMatrixButton_Click);
      // 
      // selectMatrixComboBox
      // 
      this.selectMatrixComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.selectMatrixComboBox.FormattingEnabled = true;
      this.selectMatrixComboBox.Location = new System.Drawing.Point(7, 19);
      this.selectMatrixComboBox.Name = "selectMatrixComboBox";
      this.selectMatrixComboBox.Size = new System.Drawing.Size(112, 21);
      this.selectMatrixComboBox.TabIndex = 12;
      this.selectMatrixComboBox.SelectedIndexChanged += new System.EventHandler(this.selectMatrixComboBox_SelectedIndexChanged);
      // 
      // widthLabel
      // 
      this.widthLabel.AutoSize = true;
      this.widthLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.widthLabel.Location = new System.Drawing.Point(4, 93);
      this.widthLabel.Name = "widthLabel";
      this.widthLabel.Size = new System.Drawing.Size(27, 13);
      this.widthLabel.TabIndex = 13;
      this.widthLabel.Text = "Size";
      // 
      // widthUpDown
      // 
      this.widthUpDown.Location = new System.Drawing.Point(35, 90);
      this.widthUpDown.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
      this.widthUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
      this.widthUpDown.Name = "widthUpDown";
      this.widthUpDown.Size = new System.Drawing.Size(37, 20);
      this.widthUpDown.TabIndex = 15;
      this.widthUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
      this.widthUpDown.ValueChanged += new System.EventHandler(this.widthUpDown_ValueChanged);
      // 
      // heightUpDown
      // 
      this.heightUpDown.Location = new System.Drawing.Point(78, 90);
      this.heightUpDown.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
      this.heightUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
      this.heightUpDown.Name = "heightUpDown";
      this.heightUpDown.Size = new System.Drawing.Size(37, 20);
      this.heightUpDown.TabIndex = 16;
      this.heightUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
      this.heightUpDown.ValueChanged += new System.EventHandler(this.heightUpDown_ValueChanged);
      // 
      // matrixNameTextBox
      // 
      this.matrixNameTextBox.Location = new System.Drawing.Point(7, 61);
      this.matrixNameTextBox.MaxLength = 16;
      this.matrixNameTextBox.Name = "matrixNameTextBox";
      this.matrixNameTextBox.Size = new System.Drawing.Size(112, 20);
      this.matrixNameTextBox.TabIndex = 17;
      this.matrixNameTextBox.TextChanged += new System.EventHandler(this.matrixNameTextBox_TextChanged);
      // 
      // matrixTabControl
      // 
      this.matrixTabControl.Controls.Add(this.headersTabPage);
      this.matrixTabControl.Controls.Add(this.heightsTabPage);
      this.matrixTabControl.Controls.Add(this.mapFilesTabPage);
      this.matrixTabControl.Location = new System.Drawing.Point(133, 3);
      this.matrixTabControl.Multiline = true;
      this.matrixTabControl.Name = "matrixTabControl";
      this.matrixTabControl.SelectedIndex = 0;
      this.matrixTabControl.Size = new System.Drawing.Size(1040, 610);
      this.matrixTabControl.TabIndex = 18;
      // 
      // headersTabPage
      // 
      this.headersTabPage.Controls.Add(this.headersGridView);
      this.headersTabPage.Location = new System.Drawing.Point(4, 22);
      this.headersTabPage.Name = "headersTabPage";
      this.headersTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.headersTabPage.Size = new System.Drawing.Size(1032, 584);
      this.headersTabPage.TabIndex = 1;
      this.headersTabPage.Text = "Map Headers";
      this.headersTabPage.UseVisualStyleBackColor = true;
      // 
      // headersGridView
      // 
      this.headersGridView.AllowUserToAddRows = false;
      this.headersGridView.AllowUserToDeleteRows = false;
      this.headersGridView.AllowUserToResizeColumns = false;
      this.headersGridView.AllowUserToResizeRows = false;
      this.headersGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
      dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
      dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.Info;
      dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.headersGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
      this.headersGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
      dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
      dataGridViewCellStyle2.Format = "D4";
      dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
      this.headersGridView.DefaultCellStyle = dataGridViewCellStyle2;
      this.headersGridView.Location = new System.Drawing.Point(0, 0);
      this.headersGridView.MultiSelect = false;
      this.headersGridView.Name = "headersGridView";
      dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
      dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
      dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
      dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.headersGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
      this.headersGridView.RowHeadersWidth = 50;
      this.headersGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
      dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      dataGridViewCellStyle4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.headersGridView.RowsDefaultCellStyle = dataGridViewCellStyle4;
      this.headersGridView.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      this.headersGridView.RowTemplate.Height = 18;
      this.headersGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
      this.headersGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
      this.headersGridView.ShowCellErrors = false;
      this.headersGridView.Size = new System.Drawing.Size(1032, 584);
      this.headersGridView.TabIndex = 1;
      this.headersGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.headersGridView_CellFormatting);
      this.headersGridView.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.headersGridView_CellMouseDoubleClick);
      this.headersGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.headersGridView_CellValueChanged);
      this.headersGridView.SelectionChanged += new System.EventHandler(this.headersGridView_SelectionChanged);
      // 
      // heightsTabPage
      // 
      this.heightsTabPage.Controls.Add(this.heightsGridView);
      this.heightsTabPage.Location = new System.Drawing.Point(4, 22);
      this.heightsTabPage.Name = "heightsTabPage";
      this.heightsTabPage.Size = new System.Drawing.Size(1032, 584);
      this.heightsTabPage.TabIndex = 2;
      this.heightsTabPage.Text = "Map Heights";
      this.heightsTabPage.UseVisualStyleBackColor = true;
      // 
      // heightsGridView
      // 
      this.heightsGridView.AllowUserToAddRows = false;
      this.heightsGridView.AllowUserToDeleteRows = false;
      this.heightsGridView.AllowUserToResizeColumns = false;
      this.heightsGridView.AllowUserToResizeRows = false;
      dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      this.heightsGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
      this.heightsGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
      dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
      dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
      dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
      dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.heightsGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
      this.heightsGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Window;
      dataGridViewCellStyle7.Font = new System.Drawing.Font("Tahoma", 8.25F);
      dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.ControlText;
      dataGridViewCellStyle7.Format = "D2";
      dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
      this.heightsGridView.DefaultCellStyle = dataGridViewCellStyle7;
      this.heightsGridView.Location = new System.Drawing.Point(0, 0);
      this.heightsGridView.MultiSelect = false;
      this.heightsGridView.Name = "heightsGridView";
      dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control;
      dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
      dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
      dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.heightsGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle8;
      this.heightsGridView.RowHeadersWidth = 50;
      this.heightsGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
      dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      dataGridViewCellStyle9.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.heightsGridView.RowsDefaultCellStyle = dataGridViewCellStyle9;
      this.heightsGridView.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      this.heightsGridView.RowTemplate.Height = 18;
      this.heightsGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
      this.heightsGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
      this.heightsGridView.Size = new System.Drawing.Size(1032, 584);
      this.heightsGridView.TabIndex = 2;
      this.heightsGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.heightsGridView_CellFormatting);
      this.heightsGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.heightsGridView_CellValueChanged);
      this.heightsGridView.SelectionChanged += new System.EventHandler(this.heightsGridView_SelectionChanged);
      // 
      // mapFilesTabPage
      // 
      this.mapFilesTabPage.Controls.Add(this.mapFilesGridView);
      this.mapFilesTabPage.Location = new System.Drawing.Point(4, 22);
      this.mapFilesTabPage.Name = "mapFilesTabPage";
      this.mapFilesTabPage.Size = new System.Drawing.Size(1032, 584);
      this.mapFilesTabPage.TabIndex = 3;
      this.mapFilesTabPage.Text = "Map Files";
      this.mapFilesTabPage.UseVisualStyleBackColor = true;
      // 
      // mapFilesGridView
      // 
      this.mapFilesGridView.AllowUserToAddRows = false;
      this.mapFilesGridView.AllowUserToDeleteRows = false;
      this.mapFilesGridView.AllowUserToResizeColumns = false;
      this.mapFilesGridView.AllowUserToResizeRows = false;
      dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      this.mapFilesGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle10;
      this.mapFilesGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
      dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Control;
      dataGridViewCellStyle11.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
      dataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.WindowText;
      dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.mapFilesGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle11;
      this.mapFilesGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Window;
      dataGridViewCellStyle12.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.ControlText;
      dataGridViewCellStyle12.Format = "D4";
      dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
      this.mapFilesGridView.DefaultCellStyle = dataGridViewCellStyle12;
      this.mapFilesGridView.Location = new System.Drawing.Point(0, 0);
      this.mapFilesGridView.MultiSelect = false;
      this.mapFilesGridView.Name = "mapFilesGridView";
      dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      dataGridViewCellStyle13.BackColor = System.Drawing.SystemColors.Control;
      dataGridViewCellStyle13.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
      dataGridViewCellStyle13.ForeColor = System.Drawing.SystemColors.WindowText;
      dataGridViewCellStyle13.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.mapFilesGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle13;
      this.mapFilesGridView.RowHeadersWidth = 50;
      this.mapFilesGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
      dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      dataGridViewCellStyle14.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.mapFilesGridView.RowsDefaultCellStyle = dataGridViewCellStyle14;
      this.mapFilesGridView.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      this.mapFilesGridView.RowTemplate.Height = 18;
      this.mapFilesGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
      this.mapFilesGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
      this.mapFilesGridView.Size = new System.Drawing.Size(1032, 584);
      this.mapFilesGridView.TabIndex = 2;
      this.mapFilesGridView.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.mapFilesGridView_CellMouseDoubleClick);
      this.mapFilesGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.mapFilesGridView_CellFormatting);
      this.mapFilesGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.mapFilesGridView_CellValueChanged);
      this.mapFilesGridView.SelectionChanged += new System.EventHandler(this.mapFilesGridView_SelectionChanged);
      // 
      // matrixNameLabel
      // 
      this.matrixNameLabel.AutoSize = true;
      this.matrixNameLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.matrixNameLabel.Location = new System.Drawing.Point(4, 47);
      this.matrixNameLabel.Name = "matrixNameLabel";
      this.matrixNameLabel.Size = new System.Drawing.Size(64, 13);
      this.matrixNameLabel.TabIndex = 20;
      this.matrixNameLabel.Text = "Matrix name";
      // 
      // labelMatrices
      // 
      this.labelMatrices.AutoSize = true;
      this.labelMatrices.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.labelMatrices.Location = new System.Drawing.Point(3, 3);
      this.labelMatrices.Name = "labelMatrices";
      this.labelMatrices.Size = new System.Drawing.Size(47, 13);
      this.labelMatrices.TabIndex = 21;
      this.labelMatrices.Text = "Matrices";
      // 
      // addHeadersButton
      // 
      this.addHeadersButton.Image = ((System.Drawing.Image)(resources.GetObject("addHeadersButton.Image")));
      this.addHeadersButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.addHeadersButton.Location = new System.Drawing.Point(5, 275);
      this.addHeadersButton.Name = "addHeadersButton";
      this.addHeadersButton.Size = new System.Drawing.Size(117, 35);
      this.addHeadersButton.TabIndex = 23;
      this.addHeadersButton.Text = "Add Header Tab";
      this.addHeadersButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.addHeadersButton.UseVisualStyleBackColor = true;
      this.addHeadersButton.Click += new System.EventHandler(this.addHeaderSectionButton_Click);
      // 
      // addHeightsButton
      // 
      this.addHeightsButton.Image = ((System.Drawing.Image)(resources.GetObject("addHeightsButton.Image")));
      this.addHeightsButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.addHeightsButton.Location = new System.Drawing.Point(5, 358);
      this.addHeightsButton.Name = "addHeightsButton";
      this.addHeightsButton.Size = new System.Drawing.Size(117, 35);
      this.addHeightsButton.TabIndex = 24;
      this.addHeightsButton.Text = "Add Heights Tab";
      this.addHeightsButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.addHeightsButton.UseVisualStyleBackColor = true;
      this.addHeightsButton.Click += new System.EventHandler(this.addHeightsButton_Click);
      // 
      // removeHeadersButton
      // 
      this.removeHeadersButton.Image = ((System.Drawing.Image)(resources.GetObject("removeHeadersButton.Image")));
      this.removeHeadersButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.removeHeadersButton.Location = new System.Drawing.Point(5, 311);
      this.removeHeadersButton.Name = "removeHeadersButton";
      this.removeHeadersButton.Size = new System.Drawing.Size(117, 35);
      this.removeHeadersButton.TabIndex = 25;
      this.removeHeadersButton.Text = "Remove Headers";
      this.removeHeadersButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.removeHeadersButton.UseVisualStyleBackColor = true;
      this.removeHeadersButton.Click += new System.EventHandler(this.removeHeadersButton_Click);
      // 
      // removeHeightsButton
      // 
      this.removeHeightsButton.Image = ((System.Drawing.Image)(resources.GetObject("removeHeightsButton.Image")));
      this.removeHeightsButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.removeHeightsButton.Location = new System.Drawing.Point(5, 394);
      this.removeHeightsButton.Name = "removeHeightsButton";
      this.removeHeightsButton.Size = new System.Drawing.Size(117, 35);
      this.removeHeightsButton.TabIndex = 26;
      this.removeHeightsButton.Text = "Remove Heights";
      this.removeHeightsButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.removeHeightsButton.UseVisualStyleBackColor = true;
      this.removeHeightsButton.Click += new System.EventHandler(this.removeHeightsButton_Click);
      // 
      // addMatrixButton
      // 
      this.addMatrixButton.Image = ((System.Drawing.Image)(resources.GetObject("addMatrixButton.Image")));
      this.addMatrixButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.addMatrixButton.Location = new System.Drawing.Point(5, 175);
      this.addMatrixButton.Name = "addMatrixButton";
      this.addMatrixButton.Size = new System.Drawing.Size(51, 35);
      this.addMatrixButton.TabIndex = 2;
      this.addMatrixButton.Text = "Add";
      this.addMatrixButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.addMatrixButton.UseVisualStyleBackColor = true;
      this.addMatrixButton.Click += new System.EventHandler(this.addMatrixButton_Click);
      // 
      // removeMatrixButton
      // 
      this.removeMatrixButton.Image = ((System.Drawing.Image)(resources.GetObject("removeMatrixButton.Image")));
      this.removeMatrixButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.removeMatrixButton.Location = new System.Drawing.Point(58, 175);
      this.removeMatrixButton.Name = "removeMatrixButton";
      this.removeMatrixButton.Size = new System.Drawing.Size(64, 35);
      this.removeMatrixButton.TabIndex = 27;
      this.removeMatrixButton.Text = "Delete Last";
      this.removeMatrixButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.removeMatrixButton.UseVisualStyleBackColor = true;
      this.removeMatrixButton.Click += new System.EventHandler(this.removeMatrixButton_Click);
      // 
      // exportMatrixButton
      // 
      this.exportMatrixButton.Image = global::DSPRE.Properties.Resources.exportArrow;
      this.exportMatrixButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.exportMatrixButton.Location = new System.Drawing.Point(5, 114);
      this.exportMatrixButton.Name = "exportMatrixButton";
      this.exportMatrixButton.Size = new System.Drawing.Size(117, 29);
      this.exportMatrixButton.TabIndex = 28;
      this.exportMatrixButton.Text = "Export Matrix";
      this.exportMatrixButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.exportMatrixButton.UseVisualStyleBackColor = true;
      this.exportMatrixButton.Click += new System.EventHandler(this.exportMatrixButton_Click);
      // 
      // importMatrixButton
      // 
      this.importMatrixButton.Image = global::DSPRE.Properties.Resources.importArrow;
      this.importMatrixButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.importMatrixButton.Location = new System.Drawing.Point(5, 144);
      this.importMatrixButton.Name = "importMatrixButton";
      this.importMatrixButton.Size = new System.Drawing.Size(117, 29);
      this.importMatrixButton.TabIndex = 29;
      this.importMatrixButton.Text = "Replace Matrix";
      this.importMatrixButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.importMatrixButton.UseVisualStyleBackColor = true;
      this.importMatrixButton.Click += new System.EventHandler(this.importMatrixButton_Click);
      // 
      // importColorTableButton
      // 
      this.importColorTableButton.Image = global::DSPRE.Properties.Resources.loadColorTable;
      this.importColorTableButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.importColorTableButton.Location = new System.Drawing.Point(5, 471);
      this.importColorTableButton.Name = "importColorTableButton";
      this.importColorTableButton.Size = new System.Drawing.Size(117, 32);
      this.importColorTableButton.TabIndex = 30;
      this.importColorTableButton.Text = "Import Color Table";
      this.importColorTableButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.importColorTableButton.UseVisualStyleBackColor = true;
      this.importColorTableButton.Click += new System.EventHandler(this.importColorTableButton_Click);
      // 
      // resetColorTableButton
      // 
      this.resetColorTableButton.Image = global::DSPRE.Properties.Resources.resetColorTable;
      this.resetColorTableButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.resetColorTableButton.Location = new System.Drawing.Point(5, 438);
      this.resetColorTableButton.Name = "resetColorTableButton";
      this.resetColorTableButton.Size = new System.Drawing.Size(117, 32);
      this.resetColorTableButton.TabIndex = 31;
      this.resetColorTableButton.Text = "Reset Color Table";
      this.resetColorTableButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.resetColorTableButton.UseVisualStyleBackColor = true;
      this.resetColorTableButton.Click += new System.EventHandler(this.resetColorTableButton_Click);
      // 
      // setSpawnPointButton
      // 
      this.setSpawnPointButton.Image = global::DSPRE.Properties.Resources.spawnCoordsMatrixeditorIcon;
      this.setSpawnPointButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.setSpawnPointButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.setSpawnPointButton.Location = new System.Drawing.Point(5, 513);
      this.setSpawnPointButton.Name = "setSpawnPointButton";
      this.setSpawnPointButton.Size = new System.Drawing.Size(117, 43);
      this.setSpawnPointButton.TabIndex = 32;
      this.setSpawnPointButton.Text = "Set Spawn\r\nto Selection";
      this.setSpawnPointButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.toolTip1.SetToolTip(this.setSpawnPointButton, "Sets the current matrix cell as the player\'s Spawn Point");
      this.setSpawnPointButton.UseVisualStyleBackColor = true;
      this.setSpawnPointButton.Click += new System.EventHandler(this.setSpawnPointButton_Click);
      // 
      // locateCurrentMatrixFile
      // 
      this.locateCurrentMatrixFile.Image = global::DSPRE.Properties.Resources.open_file;
      this.locateCurrentMatrixFile.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.locateCurrentMatrixFile.Location = new System.Drawing.Point(44, 224);
      this.locateCurrentMatrixFile.Name = "locateCurrentMatrixFile";
      this.locateCurrentMatrixFile.Size = new System.Drawing.Size(41, 38);
      this.locateCurrentMatrixFile.TabIndex = 33;
      this.locateCurrentMatrixFile.UseVisualStyleBackColor = true;
      this.locateCurrentMatrixFile.Click += new System.EventHandler(this.locateCurrentMatrixFile_Click);
      // 
      // MatrixEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Window;
      this.Controls.Add(this.locateCurrentMatrixFile);
      this.Controls.Add(this.matrixTabControl);
      this.Controls.Add(this.setSpawnPointButton);
      this.Controls.Add(this.saveMatrixButton);
      this.Controls.Add(this.resetColorTableButton);
      this.Controls.Add(this.selectMatrixComboBox);
      this.Controls.Add(this.importColorTableButton);
      this.Controls.Add(this.widthLabel);
      this.Controls.Add(this.importMatrixButton);
      this.Controls.Add(this.widthUpDown);
      this.Controls.Add(this.exportMatrixButton);
      this.Controls.Add(this.heightUpDown);
      this.Controls.Add(this.removeMatrixButton);
      this.Controls.Add(this.matrixNameTextBox);
      this.Controls.Add(this.addMatrixButton);
      this.Controls.Add(this.matrixNameLabel);
      this.Controls.Add(this.removeHeightsButton);
      this.Controls.Add(this.labelMatrices);
      this.Controls.Add(this.removeHeadersButton);
      this.Controls.Add(this.addHeadersButton);
      this.Controls.Add(this.addHeightsButton);
      this.Name = "MatrixEditor";
      this.Size = new System.Drawing.Size(1174, 615);
      ((System.ComponentModel.ISupportInitialize)(this.widthUpDown)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.heightUpDown)).EndInit();
      this.matrixTabControl.ResumeLayout(false);
      this.headersTabPage.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.headersGridView)).EndInit();
      this.heightsTabPage.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.heightsGridView)).EndInit();
      this.mapFilesTabPage.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.mapFilesGridView)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    #endregion

    private System.Windows.Forms.Button saveMatrixButton;
    public System.Windows.Forms.ComboBox selectMatrixComboBox;
    private System.Windows.Forms.Label widthLabel;
    private System.Windows.Forms.NumericUpDown widthUpDown;
    private System.Windows.Forms.NumericUpDown heightUpDown;
    private System.Windows.Forms.TextBox matrixNameTextBox;
    public System.Windows.Forms.TabControl matrixTabControl;
    public System.Windows.Forms.TabPage headersTabPage;
    public System.Windows.Forms.DataGridView headersGridView;
    private System.Windows.Forms.TabPage heightsTabPage;
    private System.Windows.Forms.DataGridView heightsGridView;
    private System.Windows.Forms.TabPage mapFilesTabPage;
    private System.Windows.Forms.DataGridView mapFilesGridView;
    private System.Windows.Forms.Label matrixNameLabel;
    private System.Windows.Forms.Label labelMatrices;
    private System.Windows.Forms.Button addHeadersButton;
    private System.Windows.Forms.Button addHeightsButton;
    private System.Windows.Forms.Button removeHeadersButton;
    private System.Windows.Forms.Button removeHeightsButton;
    private System.Windows.Forms.Button addMatrixButton;
    private System.Windows.Forms.Button removeMatrixButton;
    private System.Windows.Forms.Button exportMatrixButton;
    private System.Windows.Forms.Button importMatrixButton;
    private System.Windows.Forms.Button importColorTableButton;
    private System.Windows.Forms.Button resetColorTableButton;
    private System.Windows.Forms.Button setSpawnPointButton;
    private System.Windows.Forms.ToolTip toolTip1;
    private System.Windows.Forms.Button locateCurrentMatrixFile;
  }
}
