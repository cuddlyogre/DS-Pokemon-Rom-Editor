﻿namespace DSPRE
{
    partial class MainProgram
    {
        /// <summary>
        /// Variabile di proGettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainProgram));
      this.mainTabControl = new System.Windows.Forms.TabControl();
      this.headerEditorTabPage = new System.Windows.Forms.TabPage();
      this.matrixEditorTabPage = new System.Windows.Forms.TabPage();
      this.matrixEditor = new DSPRE.Editors.MatrixEditor();
      this.mapEditorTabPage = new System.Windows.Forms.TabPage();
      this.mapEditor = new DSPRE.Editors.MapEditor();
      this.nsbtxEditorTabPage = new System.Windows.Forms.TabPage();
      this.nsbtxEditor = new DSPRE.Editors.NSBTXEditor();
      this.eventEditorTabPage = new System.Windows.Forms.TabPage();
      this.eventEditor = new DSPRE.Editors.EventEditor();
      this.scriptEditorTabPage = new System.Windows.Forms.TabPage();
      this.scriptEditor = new DSPRE.Editors.ScriptEditor();
      this.textEditorTabPage = new System.Windows.Forms.TabPage();
      this.textEditor = new DSPRE.TextEditor();
      this.cameraEditorTabPage = new System.Windows.Forms.TabPage();
      this.cameraEditor = new DSPRE.Editors.CameraEditor();
      this.trainerEditorTabPage = new System.Windows.Forms.TabPage();
      this.trainerEditor = new DSPRE.Editors.TrainerEditor();
      this.tableEditorTabPage = new System.Windows.Forms.TabPage();
      this.tableEditor = new DSPRE.Editors.TableEditor();
      this.mainTabImageList = new System.Windows.Forms.ImageList(this.components);
      this.gameIcon = new System.Windows.Forms.PictureBox();
      this.languageLabel = new System.Windows.Forms.Label();
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.openROMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.openFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.saveROMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.romToolboxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.headerSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.scriptCommandsDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.diamondAndPearlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.platinumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.heartGoldAndSoulSilverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.spawnEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.NarcUtilityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.buildFomFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.unpackToFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.listBasedBatchRenameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.listBasedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.contentBasedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.listBuilderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.fromCEnumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.fromFolderContentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.nSBMDUtilityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.texturizeNSBMDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.untexturizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.extractNSBTXFromNSBMDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.menuViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.essentialToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.simpleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.fullViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.statusStrip1 = new System.Windows.Forms.StatusStrip();
      this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
      this.mainToolStrip = new System.Windows.Forms.ToolStrip();
      this.loadRomButton = new System.Windows.Forms.ToolStripButton();
      this.readDataFromFolderButton = new System.Windows.Forms.ToolStripButton();
      this.saveRomButton = new System.Windows.Forms.ToolStripButton();
      this.separator_AfterOpenSave = new System.Windows.Forms.ToolStripSeparator();
      this.unpackAllButton = new System.Windows.Forms.ToolStripButton();
      this.updateMapNarcsButton = new System.Windows.Forms.ToolStripButton();
      this.separator_afterFolderUnpackers = new System.Windows.Forms.ToolStripSeparator();
      this.buildNarcFromFolderToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.unpackNARCtoFolderToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.separator_afterNarcUtils = new System.Windows.Forms.ToolStripSeparator();
      this.listBasedBatchRenameToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.contentBasedBatchRenameToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.separator_afterRenameUtils = new System.Windows.Forms.ToolStripSeparator();
      this.enumBasedListBuilderToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.folderBasedListBuilderToolStriButton = new System.Windows.Forms.ToolStripButton();
      this.separator_afterListUtils = new System.Windows.Forms.ToolStripSeparator();
      this.nsbmdAddTexButton = new System.Windows.Forms.ToolStripButton();
      this.nsbmdRemoveTexButton = new System.Windows.Forms.ToolStripButton();
      this.nsbmdExportTexButton = new System.Windows.Forms.ToolStripButton();
      this.separator_afterNsbmdUtils = new System.Windows.Forms.ToolStripSeparator();
      this.buildingEditorButton = new System.Windows.Forms.ToolStripButton();
      this.wildEditorButton = new System.Windows.Forms.ToolStripButton();
      this.scriptCommandsButton = new System.Windows.Forms.ToolStripButton();
      this.romToolboxToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.headerSearchToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.spawnEditorToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.separator_afterMiscButtons = new System.Windows.Forms.ToolStripSeparator();
      this.versionLabel = new System.Windows.Forms.Label();
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
      this.directoryEntry1 = new System.DirectoryServices.DirectoryEntry();
      this.directoryEntry2 = new System.DirectoryServices.DirectoryEntry();
      this.headerEditor = new DSPRE.Editors.HeaderEditor();
      this.mainTabControl.SuspendLayout();
      this.headerEditorTabPage.SuspendLayout();
      this.matrixEditorTabPage.SuspendLayout();
      this.mapEditorTabPage.SuspendLayout();
      this.nsbtxEditorTabPage.SuspendLayout();
      this.eventEditorTabPage.SuspendLayout();
      this.scriptEditorTabPage.SuspendLayout();
      this.textEditorTabPage.SuspendLayout();
      this.cameraEditorTabPage.SuspendLayout();
      this.trainerEditorTabPage.SuspendLayout();
      this.tableEditorTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.gameIcon)).BeginInit();
      this.menuStrip1.SuspendLayout();
      this.statusStrip1.SuspendLayout();
      this.mainToolStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // mainTabControl
      // 
      this.mainTabControl.AllowDrop = true;
      this.mainTabControl.Controls.Add(this.headerEditorTabPage);
      this.mainTabControl.Controls.Add(this.matrixEditorTabPage);
      this.mainTabControl.Controls.Add(this.mapEditorTabPage);
      this.mainTabControl.Controls.Add(this.nsbtxEditorTabPage);
      this.mainTabControl.Controls.Add(this.eventEditorTabPage);
      this.mainTabControl.Controls.Add(this.scriptEditorTabPage);
      this.mainTabControl.Controls.Add(this.textEditorTabPage);
      this.mainTabControl.Controls.Add(this.cameraEditorTabPage);
      this.mainTabControl.Controls.Add(this.trainerEditorTabPage);
      this.mainTabControl.Controls.Add(this.tableEditorTabPage);
      this.mainTabControl.ImageList = this.mainTabImageList;
      this.mainTabControl.Location = new System.Drawing.Point(11, 72);
      this.mainTabControl.Name = "mainTabControl";
      this.mainTabControl.SelectedIndex = 0;
      this.mainTabControl.Size = new System.Drawing.Size(1193, 646);
      this.mainTabControl.TabIndex = 5;
      this.mainTabControl.Visible = false;
      this.mainTabControl.SelectedIndexChanged += new System.EventHandler(this.mainTabControl_SelectedIndexChanged);
      // 
      // headerEditorTabPage
      // 
      this.headerEditorTabPage.BackColor = System.Drawing.SystemColors.Window;
      this.headerEditorTabPage.Controls.Add(this.headerEditor);
      this.headerEditorTabPage.ImageIndex = 0;
      this.headerEditorTabPage.Location = new System.Drawing.Point(4, 23);
      this.headerEditorTabPage.Name = "headerEditorTabPage";
      this.headerEditorTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.headerEditorTabPage.Size = new System.Drawing.Size(1185, 619);
      this.headerEditorTabPage.TabIndex = 0;
      this.headerEditorTabPage.Text = "Header Editor";
      // 
      // matrixEditorTabPage
      // 
      this.matrixEditorTabPage.BackColor = System.Drawing.SystemColors.Window;
      this.matrixEditorTabPage.Controls.Add(this.matrixEditor);
      this.matrixEditorTabPage.ImageIndex = 1;
      this.matrixEditorTabPage.Location = new System.Drawing.Point(4, 23);
      this.matrixEditorTabPage.Name = "matrixEditorTabPage";
      this.matrixEditorTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.matrixEditorTabPage.Size = new System.Drawing.Size(1185, 619);
      this.matrixEditorTabPage.TabIndex = 1;
      this.matrixEditorTabPage.Text = "Matrix Editor";
      // 
      // matrixEditor
      // 
      this.matrixEditor.Location = new System.Drawing.Point(6, 9);
      this.matrixEditor.matrixEditorIsReady = false;
      this.matrixEditor.Name = "matrixEditor";
      this.matrixEditor.Size = new System.Drawing.Size(1174, 607);
      this.matrixEditor.TabIndex = 0;
      // 
      // mapEditorTabPage
      // 
      this.mapEditorTabPage.BackColor = System.Drawing.SystemColors.Window;
      this.mapEditorTabPage.Controls.Add(this.mapEditor);
      this.mapEditorTabPage.ImageIndex = 2;
      this.mapEditorTabPage.Location = new System.Drawing.Point(4, 23);
      this.mapEditorTabPage.Name = "mapEditorTabPage";
      this.mapEditorTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.mapEditorTabPage.Size = new System.Drawing.Size(1185, 619);
      this.mapEditorTabPage.TabIndex = 2;
      this.mapEditorTabPage.Text = "Map Editor";
      this.mapEditorTabPage.Enter += new System.EventHandler(this.mapEditorTabPage_Enter);
      // 
      // mapEditor
      // 
      this.mapEditor.Location = new System.Drawing.Point(0, 0);
      this.mapEditor.mapEditorIsReady = false;
      this.mapEditor.Name = "mapEditor";
      this.mapEditor.Size = new System.Drawing.Size(1168, 620);
      this.mapEditor.TabIndex = 0;
      // 
      // nsbtxEditorTabPage
      // 
      this.nsbtxEditorTabPage.Controls.Add(this.nsbtxEditor);
      this.nsbtxEditorTabPage.ImageIndex = 6;
      this.nsbtxEditorTabPage.Location = new System.Drawing.Point(4, 23);
      this.nsbtxEditorTabPage.Name = "nsbtxEditorTabPage";
      this.nsbtxEditorTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.nsbtxEditorTabPage.Size = new System.Drawing.Size(1185, 619);
      this.nsbtxEditorTabPage.TabIndex = 6;
      this.nsbtxEditorTabPage.Text = "NSBTX Editor";
      this.nsbtxEditorTabPage.UseVisualStyleBackColor = true;
      // 
      // nsbtxEditor
      // 
      this.nsbtxEditor.Location = new System.Drawing.Point(6, 7);
      this.nsbtxEditor.Name = "nsbtxEditor";
      this.nsbtxEditor.nsbtxEditorIsReady = false;
      this.nsbtxEditor.Size = new System.Drawing.Size(1139, 571);
      this.nsbtxEditor.TabIndex = 0;
      // 
      // eventEditorTabPage
      // 
      this.eventEditorTabPage.BackColor = System.Drawing.SystemColors.Window;
      this.eventEditorTabPage.Controls.Add(this.eventEditor);
      this.eventEditorTabPage.ImageIndex = 3;
      this.eventEditorTabPage.Location = new System.Drawing.Point(4, 23);
      this.eventEditorTabPage.Name = "eventEditorTabPage";
      this.eventEditorTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.eventEditorTabPage.Size = new System.Drawing.Size(1185, 619);
      this.eventEditorTabPage.TabIndex = 3;
      this.eventEditorTabPage.Text = "Event Editor";
      this.eventEditorTabPage.Enter += new System.EventHandler(this.eventEditorTabPage_Enter);
      // 
      // eventEditor
      // 
      this.eventEditor.eventEditorIsReady = false;
      this.eventEditor.Location = new System.Drawing.Point(0, 0);
      this.eventEditor.Name = "eventEditor";
      this.eventEditor.Size = new System.Drawing.Size(1176, 613);
      this.eventEditor.TabIndex = 0;
      // 
      // scriptEditorTabPage
      // 
      this.scriptEditorTabPage.Controls.Add(this.scriptEditor);
      this.scriptEditorTabPage.ImageIndex = 5;
      this.scriptEditorTabPage.Location = new System.Drawing.Point(4, 23);
      this.scriptEditorTabPage.Name = "scriptEditorTabPage";
      this.scriptEditorTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.scriptEditorTabPage.Size = new System.Drawing.Size(1185, 619);
      this.scriptEditorTabPage.TabIndex = 4;
      this.scriptEditorTabPage.Text = "Script Editor";
      this.scriptEditorTabPage.UseVisualStyleBackColor = true;
      // 
      // scriptEditor
      // 
      this.scriptEditor.Location = new System.Drawing.Point(0, 0);
      this.scriptEditor.Name = "scriptEditor";
      this.scriptEditor.scriptEditorIsReady = false;
      this.scriptEditor.Size = new System.Drawing.Size(1177, 616);
      this.scriptEditor.TabIndex = 0;
      // 
      // textEditorTabPage
      // 
      this.textEditorTabPage.Controls.Add(this.textEditor);
      this.textEditorTabPage.ImageIndex = 4;
      this.textEditorTabPage.Location = new System.Drawing.Point(4, 23);
      this.textEditorTabPage.Name = "textEditorTabPage";
      this.textEditorTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.textEditorTabPage.Size = new System.Drawing.Size(1185, 619);
      this.textEditorTabPage.TabIndex = 5;
      this.textEditorTabPage.Text = "Text Editor";
      this.textEditorTabPage.UseVisualStyleBackColor = true;
      // 
      // textEditor
      // 
      this.textEditor.Location = new System.Drawing.Point(0, 0);
      this.textEditor.Name = "textEditor";
      this.textEditor.Size = new System.Drawing.Size(1167, 616);
      this.textEditor.TabIndex = 0;
      this.textEditor.textEditorIsReady = false;
      // 
      // cameraEditorTabPage
      // 
      this.cameraEditorTabPage.Controls.Add(this.cameraEditor);
      this.cameraEditorTabPage.ImageIndex = 7;
      this.cameraEditorTabPage.Location = new System.Drawing.Point(4, 23);
      this.cameraEditorTabPage.Name = "cameraEditorTabPage";
      this.cameraEditorTabPage.Size = new System.Drawing.Size(1185, 619);
      this.cameraEditorTabPage.TabIndex = 7;
      this.cameraEditorTabPage.Text = "Camera Editor";
      this.cameraEditorTabPage.UseVisualStyleBackColor = true;
      // 
      // cameraEditor
      // 
      this.cameraEditor.cameraEditorIsReady = false;
      this.cameraEditor.Location = new System.Drawing.Point(0, 0);
      this.cameraEditor.Name = "cameraEditor";
      this.cameraEditor.Size = new System.Drawing.Size(1179, 611);
      this.cameraEditor.TabIndex = 0;
      // 
      // trainerEditorTabPage
      // 
      this.trainerEditorTabPage.Controls.Add(this.trainerEditor);
      this.trainerEditorTabPage.ImageIndex = 8;
      this.trainerEditorTabPage.Location = new System.Drawing.Point(4, 23);
      this.trainerEditorTabPage.Name = "trainerEditorTabPage";
      this.trainerEditorTabPage.Size = new System.Drawing.Size(1185, 619);
      this.trainerEditorTabPage.TabIndex = 8;
      this.trainerEditorTabPage.Text = "Trainer Editor";
      this.trainerEditorTabPage.UseVisualStyleBackColor = true;
      // 
      // trainerEditor
      // 
      this.trainerEditor.Location = new System.Drawing.Point(0, 0);
      this.trainerEditor.Name = "trainerEditor";
      this.trainerEditor.Size = new System.Drawing.Size(1166, 610);
      this.trainerEditor.TabIndex = 0;
      this.trainerEditor.trainerEditorIsReady = false;
      // 
      // tableEditorTabPage
      // 
      this.tableEditorTabPage.Controls.Add(this.tableEditor);
      this.tableEditorTabPage.ImageIndex = 9;
      this.tableEditorTabPage.Location = new System.Drawing.Point(4, 23);
      this.tableEditorTabPage.Name = "tableEditorTabPage";
      this.tableEditorTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.tableEditorTabPage.Size = new System.Drawing.Size(1185, 619);
      this.tableEditorTabPage.TabIndex = 9;
      this.tableEditorTabPage.Text = "Table Editor";
      this.tableEditorTabPage.UseVisualStyleBackColor = true;
      // 
      // tableEditor
      // 
      this.tableEditor.Location = new System.Drawing.Point(-2, 0);
      this.tableEditor.Name = "tableEditor";
      this.tableEditor.Size = new System.Drawing.Size(1165, 619);
      this.tableEditor.TabIndex = 0;
      this.tableEditor.tableEditorIsReady = false;
      // 
      // mainTabImageList
      // 
      this.mainTabImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("mainTabImageList.ImageStream")));
      this.mainTabImageList.TransparentColor = System.Drawing.Color.White;
      this.mainTabImageList.Images.SetKeyName(0, "map_header.ico");
      this.mainTabImageList.Images.SetKeyName(1, "matrix_editor.png");
      this.mainTabImageList.Images.SetKeyName(2, "map_editor.png");
      this.mainTabImageList.Images.SetKeyName(3, "event_editor.png");
      this.mainTabImageList.Images.SetKeyName(4, "text_editor.png");
      this.mainTabImageList.Images.SetKeyName(5, "script_editor.png");
      this.mainTabImageList.Images.SetKeyName(6, "tileset_editor.png");
      this.mainTabImageList.Images.SetKeyName(7, "gamecamera_editor.png");
      this.mainTabImageList.Images.SetKeyName(8, "trainer_editor.png");
      this.mainTabImageList.Images.SetKeyName(9, "tableEditor.png");
      // 
      // gameIcon
      // 
      this.gameIcon.ErrorImage = null;
      this.gameIcon.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.gameIcon.InitialImage = null;
      this.gameIcon.Location = new System.Drawing.Point(1167, 35);
      this.gameIcon.Name = "gameIcon";
      this.gameIcon.Size = new System.Drawing.Size(32, 32);
      this.gameIcon.TabIndex = 8;
      this.gameIcon.TabStop = false;
      this.gameIcon.Paint += new System.Windows.Forms.PaintEventHandler(this.PaintGameIcon);
      // 
      // languageLabel
      // 
      this.languageLabel.AutoSize = true;
      this.languageLabel.Location = new System.Drawing.Point(1042, 52);
      this.languageLabel.Name = "languageLabel";
      this.languageLabel.Size = new System.Drawing.Size(58, 13);
      this.languageLabel.TabIndex = 10;
      this.languageLabel.Text = "Language:";
      this.languageLabel.Visible = false;
      // 
      // menuStrip1
      // 
      this.menuStrip1.BackColor = System.Drawing.SystemColors.Window;
      this.menuStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.menuViewToolStripMenuItem,
            this.aboutToolStripMenuItem1});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(1214, 24);
      this.menuStrip1.TabIndex = 12;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openROMToolStripMenuItem,
            this.openFolderToolStripMenuItem,
            this.saveROMToolStripMenuItem});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fileToolStripMenuItem.Text = "File";
      // 
      // openROMToolStripMenuItem
      // 
      this.openROMToolStripMenuItem.Image = global::DSPRE.Properties.Resources.open_rom;
      this.openROMToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.openROMToolStripMenuItem.Name = "openROMToolStripMenuItem";
      this.openROMToolStripMenuItem.Size = new System.Drawing.Size(155, 38);
      this.openROMToolStripMenuItem.Text = "Open ROM";
      this.openROMToolStripMenuItem.Click += new System.EventHandler(this.loadRom_Click);
      // 
      // openFolderToolStripMenuItem
      // 
      this.openFolderToolStripMenuItem.Image = global::DSPRE.Properties.Resources.open_file;
      this.openFolderToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
      this.openFolderToolStripMenuItem.Size = new System.Drawing.Size(155, 38);
      this.openFolderToolStripMenuItem.Text = "Open Folder";
      this.openFolderToolStripMenuItem.Click += new System.EventHandler(this.readDataFromFolderButton_Click);
      // 
      // saveROMToolStripMenuItem
      // 
      this.saveROMToolStripMenuItem.Enabled = false;
      this.saveROMToolStripMenuItem.Image = global::DSPRE.Properties.Resources.save_rom;
      this.saveROMToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.saveROMToolStripMenuItem.Name = "saveROMToolStripMenuItem";
      this.saveROMToolStripMenuItem.Size = new System.Drawing.Size(155, 38);
      this.saveROMToolStripMenuItem.Text = "Save ROM";
      this.saveROMToolStripMenuItem.Click += new System.EventHandler(this.saveRom_Click);
      // 
      // aboutToolStripMenuItem
      // 
      this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.romToolboxToolStripMenuItem,
            this.headerSearchToolStripMenuItem,
            this.scriptCommandsDatabaseToolStripMenuItem,
            this.spawnEditorToolStripMenuItem,
            this.NarcUtilityToolStripMenuItem,
            this.listBasedBatchRenameToolStripMenuItem,
            this.listBuilderToolStripMenuItem,
            this.nSBMDUtilityToolStripMenuItem});
      this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
      this.aboutToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
      this.aboutToolStripMenuItem.Text = "Tools";
      // 
      // romToolboxToolStripMenuItem
      // 
      this.romToolboxToolStripMenuItem.Enabled = false;
      this.romToolboxToolStripMenuItem.Name = "romToolboxToolStripMenuItem";
      this.romToolboxToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
      this.romToolboxToolStripMenuItem.Text = "ROM Toolbox";
      this.romToolboxToolStripMenuItem.Click += new System.EventHandler(this.romToolBoxToolStripMenuItem_Click);
      // 
      // headerSearchToolStripMenuItem
      // 
      this.headerSearchToolStripMenuItem.Enabled = false;
      this.headerSearchToolStripMenuItem.Name = "headerSearchToolStripMenuItem";
      this.headerSearchToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
      this.headerSearchToolStripMenuItem.Text = "Advanced Header Search";
      this.headerSearchToolStripMenuItem.Click += new System.EventHandler(this.advancedHeaderSearchToolStripMenuItem_Click);
      // 
      // scriptCommandsDatabaseToolStripMenuItem
      // 
      this.scriptCommandsDatabaseToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.diamondAndPearlToolStripMenuItem,
            this.platinumToolStripMenuItem,
            this.heartGoldAndSoulSilverToolStripMenuItem});
      this.scriptCommandsDatabaseToolStripMenuItem.Name = "scriptCommandsDatabaseToolStripMenuItem";
      this.scriptCommandsDatabaseToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
      this.scriptCommandsDatabaseToolStripMenuItem.Text = "Script Commands Database";
      // 
      // diamondAndPearlToolStripMenuItem
      // 
      this.diamondAndPearlToolStripMenuItem.Image = global::DSPRE.Properties.Resources.scriptDBIconDP;
      this.diamondAndPearlToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.diamondAndPearlToolStripMenuItem.Name = "diamondAndPearlToolStripMenuItem";
      this.diamondAndPearlToolStripMenuItem.Size = new System.Drawing.Size(211, 38);
      this.diamondAndPearlToolStripMenuItem.Text = "Diamond && Pearl";
      this.diamondAndPearlToolStripMenuItem.Click += new System.EventHandler(this.diamondAndPearlToolStripMenuItem_Click);
      // 
      // platinumToolStripMenuItem
      // 
      this.platinumToolStripMenuItem.Image = global::DSPRE.Properties.Resources.scriptDBIconPt;
      this.platinumToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.platinumToolStripMenuItem.Name = "platinumToolStripMenuItem";
      this.platinumToolStripMenuItem.Size = new System.Drawing.Size(211, 38);
      this.platinumToolStripMenuItem.Text = "Platinum";
      this.platinumToolStripMenuItem.Click += new System.EventHandler(this.platinumToolStripMenuItem_Click);
      // 
      // heartGoldAndSoulSilverToolStripMenuItem
      // 
      this.heartGoldAndSoulSilverToolStripMenuItem.Image = global::DSPRE.Properties.Resources.scriptDBIconHGSS;
      this.heartGoldAndSoulSilverToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.heartGoldAndSoulSilverToolStripMenuItem.Name = "heartGoldAndSoulSilverToolStripMenuItem";
      this.heartGoldAndSoulSilverToolStripMenuItem.Size = new System.Drawing.Size(211, 38);
      this.heartGoldAndSoulSilverToolStripMenuItem.Text = "HeartGold && SoulSilver";
      this.heartGoldAndSoulSilverToolStripMenuItem.Click += new System.EventHandler(this.heartGoldAndSoulSilverToolStripMenuItem_Click);
      // 
      // spawnEditorToolStripMenuItem
      // 
      this.spawnEditorToolStripMenuItem.Enabled = false;
      this.spawnEditorToolStripMenuItem.Name = "spawnEditorToolStripMenuItem";
      this.spawnEditorToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
      this.spawnEditorToolStripMenuItem.Text = "Spawn Point Editor";
      this.spawnEditorToolStripMenuItem.Click += new System.EventHandler(this.spawnEditorToolStripMenuItem_Click);
      // 
      // NarcUtilityToolStripMenuItem
      // 
      this.NarcUtilityToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buildFomFolderToolStripMenuItem,
            this.unpackToFolderToolStripMenuItem});
      this.NarcUtilityToolStripMenuItem.Name = "NarcUtilityToolStripMenuItem";
      this.NarcUtilityToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
      this.NarcUtilityToolStripMenuItem.Text = "NARC Utility";
      // 
      // buildFomFolderToolStripMenuItem
      // 
      this.buildFomFolderToolStripMenuItem.Image = global::DSPRE.Properties.Resources.folderToNarcIcon;
      this.buildFomFolderToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.buildFomFolderToolStripMenuItem.Name = "buildFomFolderToolStripMenuItem";
      this.buildFomFolderToolStripMenuItem.Size = new System.Drawing.Size(214, 38);
      this.buildFomFolderToolStripMenuItem.Text = "Build from Folder";
      this.buildFomFolderToolStripMenuItem.Click += new System.EventHandler(this.buildFromFolderToolStripMenuItem_Click);
      // 
      // unpackToFolderToolStripMenuItem
      // 
      this.unpackToFolderToolStripMenuItem.Image = global::DSPRE.Properties.Resources.narcToFolderIcon;
      this.unpackToFolderToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.unpackToFolderToolStripMenuItem.Name = "unpackToFolderToolStripMenuItem";
      this.unpackToFolderToolStripMenuItem.Size = new System.Drawing.Size(214, 38);
      this.unpackToFolderToolStripMenuItem.Text = "Unpack to Folder";
      this.unpackToFolderToolStripMenuItem.Click += new System.EventHandler(this.unpackToFolderToolStripMenuItem_Click);
      // 
      // listBasedBatchRenameToolStripMenuItem
      // 
      this.listBasedBatchRenameToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.listBasedToolStripMenuItem,
            this.contentBasedToolStripMenuItem});
      this.listBasedBatchRenameToolStripMenuItem.Name = "listBasedBatchRenameToolStripMenuItem";
      this.listBasedBatchRenameToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
      this.listBasedBatchRenameToolStripMenuItem.Text = "Batch Rename Utility";
      // 
      // listBasedToolStripMenuItem
      // 
      this.listBasedToolStripMenuItem.Image = global::DSPRE.Properties.Resources.listbasedRenameIcon;
      this.listBasedToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.listBasedToolStripMenuItem.Name = "listBasedToolStripMenuItem";
      this.listBasedToolStripMenuItem.Size = new System.Drawing.Size(185, 38);
      this.listBasedToolStripMenuItem.Text = "List-Based";
      this.listBasedToolStripMenuItem.Click += new System.EventHandler(this.listBasedToolStripMenuItem_Click);
      // 
      // contentBasedToolStripMenuItem
      // 
      this.contentBasedToolStripMenuItem.Image = global::DSPRE.Properties.Resources.contentbasedRenameIcon;
      this.contentBasedToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.contentBasedToolStripMenuItem.Name = "contentBasedToolStripMenuItem";
      this.contentBasedToolStripMenuItem.Size = new System.Drawing.Size(185, 38);
      this.contentBasedToolStripMenuItem.Text = "Content-Based";
      this.contentBasedToolStripMenuItem.Click += new System.EventHandler(this.contentBasedToolStripMenuItem_Click);
      // 
      // listBuilderToolStripMenuItem
      // 
      this.listBuilderToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fromCEnumToolStripMenuItem,
            this.fromFolderContentsToolStripMenuItem});
      this.listBuilderToolStripMenuItem.Name = "listBuilderToolStripMenuItem";
      this.listBuilderToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
      this.listBuilderToolStripMenuItem.Text = "Folder-Based List Builder";
      // 
      // fromCEnumToolStripMenuItem
      // 
      this.fromCEnumToolStripMenuItem.Image = global::DSPRE.Properties.Resources.enumToListIcon;
      this.fromCEnumToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.fromCEnumToolStripMenuItem.Name = "fromCEnumToolStripMenuItem";
      this.fromCEnumToolStripMenuItem.Size = new System.Drawing.Size(237, 38);
      this.fromCEnumToolStripMenuItem.Text = "From C Enum";
      this.fromCEnumToolStripMenuItem.Click += new System.EventHandler(this.enumBasedListBuilderToolStripButton_Click);
      // 
      // fromFolderContentsToolStripMenuItem
      // 
      this.fromFolderContentsToolStripMenuItem.Image = global::DSPRE.Properties.Resources.folderToListIcon;
      this.fromFolderContentsToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.fromFolderContentsToolStripMenuItem.Name = "fromFolderContentsToolStripMenuItem";
      this.fromFolderContentsToolStripMenuItem.Size = new System.Drawing.Size(237, 38);
      this.fromFolderContentsToolStripMenuItem.Text = "From Folder Contents";
      this.fromFolderContentsToolStripMenuItem.Click += new System.EventHandler(this.fromFolderContentsToolStripMenuItem_Click);
      // 
      // nSBMDUtilityToolStripMenuItem
      // 
      this.nSBMDUtilityToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.texturizeNSBMDToolStripMenuItem,
            this.untexturizeToolStripMenuItem,
            this.extractNSBTXFromNSBMDToolStripMenuItem});
      this.nSBMDUtilityToolStripMenuItem.Name = "nSBMDUtilityToolStripMenuItem";
      this.nSBMDUtilityToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
      this.nSBMDUtilityToolStripMenuItem.Text = "NSBMD Utility";
      // 
      // texturizeNSBMDToolStripMenuItem
      // 
      this.texturizeNSBMDToolStripMenuItem.Image = global::DSPRE.Properties.Resources.addTextureToNSBMD;
      this.texturizeNSBMDToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.texturizeNSBMDToolStripMenuItem.Name = "texturizeNSBMDToolStripMenuItem";
      this.texturizeNSBMDToolStripMenuItem.Size = new System.Drawing.Size(251, 38);
      this.texturizeNSBMDToolStripMenuItem.Text = "Add/Replace NSBMD textures";
      this.texturizeNSBMDToolStripMenuItem.Click += new System.EventHandler(this.nsbmdAddTexButton_Click);
      // 
      // untexturizeToolStripMenuItem
      // 
      this.untexturizeToolStripMenuItem.Image = global::DSPRE.Properties.Resources.removeTextureNSBMD;
      this.untexturizeToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.untexturizeToolStripMenuItem.Name = "untexturizeToolStripMenuItem";
      this.untexturizeToolStripMenuItem.Size = new System.Drawing.Size(251, 38);
      this.untexturizeToolStripMenuItem.Text = "Remove textures from NSBMD";
      this.untexturizeToolStripMenuItem.Click += new System.EventHandler(this.nsbmdRemoveTexButton_Click);
      // 
      // extractNSBTXFromNSBMDToolStripMenuItem
      // 
      this.extractNSBTXFromNSBMDToolStripMenuItem.Image = global::DSPRE.Properties.Resources.saveTextureFromNSBMD;
      this.extractNSBTXFromNSBMDToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.extractNSBTXFromNSBMDToolStripMenuItem.Name = "extractNSBTXFromNSBMDToolStripMenuItem";
      this.extractNSBTXFromNSBMDToolStripMenuItem.Size = new System.Drawing.Size(251, 38);
      this.extractNSBTXFromNSBMDToolStripMenuItem.Text = "Save textures from NSBMD";
      this.extractNSBTXFromNSBMDToolStripMenuItem.Click += new System.EventHandler(this.nsbmdExportTexButton_Click);
      // 
      // menuViewToolStripMenuItem
      // 
      this.menuViewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.essentialToolStripMenuItem,
            this.simpleToolStripMenuItem,
            this.fullViewToolStripMenuItem});
      this.menuViewToolStripMenuItem.Name = "menuViewToolStripMenuItem";
      this.menuViewToolStripMenuItem.Size = new System.Drawing.Size(78, 20);
      this.menuViewToolStripMenuItem.Text = "Menu View";
      // 
      // essentialToolStripMenuItem
      // 
      this.essentialToolStripMenuItem.Name = "essentialToolStripMenuItem";
      this.essentialToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
      this.essentialToolStripMenuItem.Text = "Essential";
      this.essentialToolStripMenuItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.simpleToolStripMenuItem_MouseDown);
      // 
      // simpleToolStripMenuItem
      // 
      this.simpleToolStripMenuItem.Name = "simpleToolStripMenuItem";
      this.simpleToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
      this.simpleToolStripMenuItem.Text = "Simple";
      this.simpleToolStripMenuItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.simpleToolStripMenuItem_MouseDown);
      // 
      // fullViewToolStripMenuItem
      // 
      this.fullViewToolStripMenuItem.Checked = true;
      this.fullViewToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
      this.fullViewToolStripMenuItem.Name = "fullViewToolStripMenuItem";
      this.fullViewToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
      this.fullViewToolStripMenuItem.Text = "Complete";
      this.fullViewToolStripMenuItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.simpleToolStripMenuItem_MouseDown);
      // 
      // aboutToolStripMenuItem1
      // 
      this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
      this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(52, 20);
      this.aboutToolStripMenuItem1.Text = "About";
      this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
      // 
      // statusStrip1
      // 
      this.statusStrip1.BackColor = System.Drawing.SystemColors.Control;
      this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.toolStripProgressBar});
      this.statusStrip1.Location = new System.Drawing.Point(0, 721);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Size = new System.Drawing.Size(1214, 22);
      this.statusStrip1.TabIndex = 13;
      this.statusStrip1.Text = "statusStrip1";
      // 
      // statusLabel
      // 
      this.statusLabel.Name = "statusLabel";
      this.statusLabel.Size = new System.Drawing.Size(39, 17);
      this.statusLabel.Text = "Ready";
      // 
      // toolStripProgressBar
      // 
      this.toolStripProgressBar.Name = "toolStripProgressBar";
      this.toolStripProgressBar.Size = new System.Drawing.Size(180, 18);
      this.toolStripProgressBar.Visible = false;
      // 
      // mainToolStrip
      // 
      this.mainToolStrip.AllowMerge = false;
      this.mainToolStrip.BackColor = System.Drawing.SystemColors.Menu;
      this.mainToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadRomButton,
            this.readDataFromFolderButton,
            this.saveRomButton,
            this.separator_AfterOpenSave,
            this.unpackAllButton,
            this.updateMapNarcsButton,
            this.separator_afterFolderUnpackers,
            this.buildNarcFromFolderToolStripButton,
            this.unpackNARCtoFolderToolStripButton,
            this.separator_afterNarcUtils,
            this.listBasedBatchRenameToolStripButton,
            this.contentBasedBatchRenameToolStripButton,
            this.separator_afterRenameUtils,
            this.enumBasedListBuilderToolStripButton,
            this.folderBasedListBuilderToolStriButton,
            this.separator_afterListUtils,
            this.nsbmdAddTexButton,
            this.nsbmdRemoveTexButton,
            this.nsbmdExportTexButton,
            this.separator_afterNsbmdUtils,
            this.buildingEditorButton,
            this.wildEditorButton,
            this.scriptCommandsButton,
            this.romToolboxToolStripButton,
            this.headerSearchToolStripButton,
            this.spawnEditorToolStripButton,
            this.separator_afterMiscButtons});
      this.mainToolStrip.Location = new System.Drawing.Point(0, 24);
      this.mainToolStrip.Name = "mainToolStrip";
      this.mainToolStrip.Size = new System.Drawing.Size(1214, 44);
      this.mainToolStrip.TabIndex = 16;
      this.mainToolStrip.Text = "mainToolStrip";
      // 
      // loadRomButton
      // 
      this.loadRomButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.loadRomButton.Image = global::DSPRE.Properties.Resources.open_rom;
      this.loadRomButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.loadRomButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.loadRomButton.Margin = new System.Windows.Forms.Padding(13, 6, 0, 2);
      this.loadRomButton.Name = "loadRomButton";
      this.loadRomButton.Size = new System.Drawing.Size(36, 36);
      this.loadRomButton.Text = "Open ROM";
      this.loadRomButton.Click += new System.EventHandler(this.loadRom_Click);
      // 
      // readDataFromFolderButton
      // 
      this.readDataFromFolderButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.readDataFromFolderButton.Image = global::DSPRE.Properties.Resources.open_file;
      this.readDataFromFolderButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.readDataFromFolderButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.readDataFromFolderButton.Margin = new System.Windows.Forms.Padding(0, 6, 0, 2);
      this.readDataFromFolderButton.Name = "readDataFromFolderButton";
      this.readDataFromFolderButton.Size = new System.Drawing.Size(36, 36);
      this.readDataFromFolderButton.Text = "Open Extracted Data";
      this.readDataFromFolderButton.ToolTipText = "Open Extracted Data";
      this.readDataFromFolderButton.Click += new System.EventHandler(this.readDataFromFolderButton_Click);
      // 
      // saveRomButton
      // 
      this.saveRomButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.saveRomButton.Enabled = false;
      this.saveRomButton.Image = global::DSPRE.Properties.Resources.save_rom;
      this.saveRomButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.saveRomButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.saveRomButton.Margin = new System.Windows.Forms.Padding(0, 6, 0, 2);
      this.saveRomButton.Name = "saveRomButton";
      this.saveRomButton.Size = new System.Drawing.Size(36, 36);
      this.saveRomButton.Text = "Save ROM";
      this.saveRomButton.Click += new System.EventHandler(this.saveRom_Click);
      // 
      // separator_AfterOpenSave
      // 
      this.separator_AfterOpenSave.Name = "separator_AfterOpenSave";
      this.separator_AfterOpenSave.Size = new System.Drawing.Size(6, 44);
      // 
      // unpackAllButton
      // 
      this.unpackAllButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.unpackAllButton.Enabled = false;
      this.unpackAllButton.Image = global::DSPRE.Properties.Resources.unpackAllIcon;
      this.unpackAllButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.unpackAllButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.unpackAllButton.Margin = new System.Windows.Forms.Padding(0, 6, 0, 2);
      this.unpackAllButton.Name = "unpackAllButton";
      this.unpackAllButton.Size = new System.Drawing.Size(36, 36);
      this.unpackAllButton.Text = "Unpack All Narcs";
      this.unpackAllButton.Click += new System.EventHandler(this.unpackAllButton_Click);
      // 
      // updateMapNarcsButton
      // 
      this.updateMapNarcsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.updateMapNarcsButton.Enabled = false;
      this.updateMapNarcsButton.Image = global::DSPRE.Properties.Resources.unpackBuildingNarcsIcon;
      this.updateMapNarcsButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.updateMapNarcsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.updateMapNarcsButton.Margin = new System.Windows.Forms.Padding(0, 6, 0, 2);
      this.updateMapNarcsButton.Name = "updateMapNarcsButton";
      this.updateMapNarcsButton.Size = new System.Drawing.Size(36, 36);
      this.updateMapNarcsButton.Text = "Unpack Building NARCs";
      this.updateMapNarcsButton.Click += new System.EventHandler(this.updateMapNarcsButton_Click);
      // 
      // separator_afterFolderUnpackers
      // 
      this.separator_afterFolderUnpackers.Name = "separator_afterFolderUnpackers";
      this.separator_afterFolderUnpackers.Size = new System.Drawing.Size(6, 44);
      // 
      // buildNarcFromFolderToolStripButton
      // 
      this.buildNarcFromFolderToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.buildNarcFromFolderToolStripButton.Image = global::DSPRE.Properties.Resources.folderToNarcIcon;
      this.buildNarcFromFolderToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.buildNarcFromFolderToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.buildNarcFromFolderToolStripButton.Margin = new System.Windows.Forms.Padding(2, 6, 2, 2);
      this.buildNarcFromFolderToolStripButton.Name = "buildNarcFromFolderToolStripButton";
      this.buildNarcFromFolderToolStripButton.Size = new System.Drawing.Size(68, 36);
      this.buildNarcFromFolderToolStripButton.Text = "Build NARC from Folder";
      this.buildNarcFromFolderToolStripButton.Click += new System.EventHandler(this.buildFromFolderToolStripMenuItem_Click);
      // 
      // unpackNARCtoFolderToolStripButton
      // 
      this.unpackNARCtoFolderToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.unpackNARCtoFolderToolStripButton.Image = global::DSPRE.Properties.Resources.narcToFolderIcon;
      this.unpackNARCtoFolderToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.unpackNARCtoFolderToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.unpackNARCtoFolderToolStripButton.Margin = new System.Windows.Forms.Padding(2, 6, 2, 2);
      this.unpackNARCtoFolderToolStripButton.Name = "unpackNARCtoFolderToolStripButton";
      this.unpackNARCtoFolderToolStripButton.Size = new System.Drawing.Size(68, 36);
      this.unpackNARCtoFolderToolStripButton.Text = "Unpack NARC to Folder";
      this.unpackNARCtoFolderToolStripButton.Click += new System.EventHandler(this.unpackToFolderToolStripMenuItem_Click);
      // 
      // separator_afterNarcUtils
      // 
      this.separator_afterNarcUtils.Name = "separator_afterNarcUtils";
      this.separator_afterNarcUtils.Size = new System.Drawing.Size(6, 44);
      // 
      // listBasedBatchRenameToolStripButton
      // 
      this.listBasedBatchRenameToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.listBasedBatchRenameToolStripButton.Image = global::DSPRE.Properties.Resources.listbasedRenameIcon;
      this.listBasedBatchRenameToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.listBasedBatchRenameToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.listBasedBatchRenameToolStripButton.Margin = new System.Windows.Forms.Padding(2, 6, 3, 2);
      this.listBasedBatchRenameToolStripButton.Name = "listBasedBatchRenameToolStripButton";
      this.listBasedBatchRenameToolStripButton.Size = new System.Drawing.Size(52, 36);
      this.listBasedBatchRenameToolStripButton.Text = "List-Based Batch Rename";
      this.listBasedBatchRenameToolStripButton.Click += new System.EventHandler(this.listBasedToolStripMenuItem_Click);
      // 
      // contentBasedBatchRenameToolStripButton
      // 
      this.contentBasedBatchRenameToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.contentBasedBatchRenameToolStripButton.Image = global::DSPRE.Properties.Resources.contentbasedRenameIcon;
      this.contentBasedBatchRenameToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.contentBasedBatchRenameToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.contentBasedBatchRenameToolStripButton.Margin = new System.Windows.Forms.Padding(3, 6, 2, 2);
      this.contentBasedBatchRenameToolStripButton.Name = "contentBasedBatchRenameToolStripButton";
      this.contentBasedBatchRenameToolStripButton.Size = new System.Drawing.Size(52, 36);
      this.contentBasedBatchRenameToolStripButton.Text = "Content-Based Batch Rename";
      this.contentBasedBatchRenameToolStripButton.Click += new System.EventHandler(this.contentBasedToolStripMenuItem_Click);
      // 
      // separator_afterRenameUtils
      // 
      this.separator_afterRenameUtils.Name = "separator_afterRenameUtils";
      this.separator_afterRenameUtils.Size = new System.Drawing.Size(6, 44);
      // 
      // enumBasedListBuilderToolStripButton
      // 
      this.enumBasedListBuilderToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.enumBasedListBuilderToolStripButton.Image = global::DSPRE.Properties.Resources.enumToListIcon;
      this.enumBasedListBuilderToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.enumBasedListBuilderToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.enumBasedListBuilderToolStripButton.Margin = new System.Windows.Forms.Padding(3, 6, 2, 2);
      this.enumBasedListBuilderToolStripButton.Name = "enumBasedListBuilderToolStripButton";
      this.enumBasedListBuilderToolStripButton.Size = new System.Drawing.Size(68, 36);
      this.enumBasedListBuilderToolStripButton.Text = "Enum-Based List Builder";
      this.enumBasedListBuilderToolStripButton.Click += new System.EventHandler(this.enumBasedListBuilderToolStripButton_Click);
      // 
      // folderBasedListBuilderToolStriButton
      // 
      this.folderBasedListBuilderToolStriButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.folderBasedListBuilderToolStriButton.Image = global::DSPRE.Properties.Resources.folderToListIcon;
      this.folderBasedListBuilderToolStriButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.folderBasedListBuilderToolStriButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.folderBasedListBuilderToolStriButton.Margin = new System.Windows.Forms.Padding(2, 6, 3, 2);
      this.folderBasedListBuilderToolStriButton.Name = "folderBasedListBuilderToolStriButton";
      this.folderBasedListBuilderToolStriButton.Size = new System.Drawing.Size(68, 36);
      this.folderBasedListBuilderToolStriButton.Text = "Folder-Based List Builder";
      this.folderBasedListBuilderToolStriButton.Click += new System.EventHandler(this.fromFolderContentsToolStripMenuItem_Click);
      // 
      // separator_afterListUtils
      // 
      this.separator_afterListUtils.Name = "separator_afterListUtils";
      this.separator_afterListUtils.Size = new System.Drawing.Size(6, 44);
      // 
      // nsbmdAddTexButton
      // 
      this.nsbmdAddTexButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.nsbmdAddTexButton.Image = global::DSPRE.Properties.Resources.addTextureToNSBMD;
      this.nsbmdAddTexButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.nsbmdAddTexButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.nsbmdAddTexButton.Margin = new System.Windows.Forms.Padding(4, 6, 0, 2);
      this.nsbmdAddTexButton.Name = "nsbmdAddTexButton";
      this.nsbmdAddTexButton.Size = new System.Drawing.Size(36, 36);
      this.nsbmdAddTexButton.Text = "Add texture to NSBMD";
      this.nsbmdAddTexButton.ToolTipText = "Add textures to NSBMD";
      this.nsbmdAddTexButton.Click += new System.EventHandler(this.nsbmdAddTexButton_Click);
      // 
      // nsbmdRemoveTexButton
      // 
      this.nsbmdRemoveTexButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.nsbmdRemoveTexButton.Image = global::DSPRE.Properties.Resources.removeTextureNSBMD;
      this.nsbmdRemoveTexButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.nsbmdRemoveTexButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.nsbmdRemoveTexButton.Margin = new System.Windows.Forms.Padding(4, 6, 0, 2);
      this.nsbmdRemoveTexButton.Name = "nsbmdRemoveTexButton";
      this.nsbmdRemoveTexButton.Size = new System.Drawing.Size(36, 36);
      this.nsbmdRemoveTexButton.Text = "Remove texture from NSBMD";
      this.nsbmdRemoveTexButton.ToolTipText = "Remove textures from NSBMD";
      this.nsbmdRemoveTexButton.Click += new System.EventHandler(this.nsbmdRemoveTexButton_Click);
      // 
      // nsbmdExportTexButton
      // 
      this.nsbmdExportTexButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.nsbmdExportTexButton.Image = global::DSPRE.Properties.Resources.saveTextureFromNSBMD;
      this.nsbmdExportTexButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.nsbmdExportTexButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.nsbmdExportTexButton.Margin = new System.Windows.Forms.Padding(4, 6, 0, 2);
      this.nsbmdExportTexButton.Name = "nsbmdExportTexButton";
      this.nsbmdExportTexButton.Size = new System.Drawing.Size(36, 36);
      this.nsbmdExportTexButton.Text = "Extract texture from NSBMD";
      this.nsbmdExportTexButton.ToolTipText = "Extract textures from NSBMD";
      this.nsbmdExportTexButton.Click += new System.EventHandler(this.nsbmdExportTexButton_Click);
      // 
      // separator_afterNsbmdUtils
      // 
      this.separator_afterNsbmdUtils.Name = "separator_afterNsbmdUtils";
      this.separator_afterNsbmdUtils.Size = new System.Drawing.Size(6, 44);
      // 
      // buildingEditorButton
      // 
      this.buildingEditorButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.buildingEditorButton.Enabled = false;
      this.buildingEditorButton.Image = global::DSPRE.Properties.Resources.buildingEditorButton;
      this.buildingEditorButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.buildingEditorButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.buildingEditorButton.Margin = new System.Windows.Forms.Padding(4, 6, 0, 2);
      this.buildingEditorButton.Name = "buildingEditorButton";
      this.buildingEditorButton.Size = new System.Drawing.Size(36, 36);
      this.buildingEditorButton.Text = "Buildings Editor";
      this.buildingEditorButton.ToolTipText = "Building Editor";
      this.buildingEditorButton.Click += new System.EventHandler(this.buildingEditorButton_Click);
      // 
      // wildEditorButton
      // 
      this.wildEditorButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.wildEditorButton.Enabled = false;
      this.wildEditorButton.Image = global::DSPRE.Properties.Resources.wildEditorButton;
      this.wildEditorButton.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.wildEditorButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.wildEditorButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.wildEditorButton.Margin = new System.Windows.Forms.Padding(4, 6, 0, 2);
      this.wildEditorButton.Name = "wildEditorButton";
      this.wildEditorButton.Size = new System.Drawing.Size(36, 36);
      this.wildEditorButton.Text = "Wild Pokémon Editor";
      this.wildEditorButton.Click += new System.EventHandler(this.wildEditorButton_Click);
      // 
      // scriptCommandsButton
      // 
      this.scriptCommandsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.scriptCommandsButton.Enabled = false;
      this.scriptCommandsButton.Image = global::DSPRE.Properties.Resources.scriptDBIcon;
      this.scriptCommandsButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.scriptCommandsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.scriptCommandsButton.Margin = new System.Windows.Forms.Padding(4, 6, 0, 2);
      this.scriptCommandsButton.Name = "scriptCommandsButton";
      this.scriptCommandsButton.Size = new System.Drawing.Size(36, 36);
      this.scriptCommandsButton.Text = "Script Commands Database";
      this.scriptCommandsButton.Click += new System.EventHandler(this.scriptCommandsDatabaseToolStripButton_Click);
      // 
      // romToolboxToolStripButton
      // 
      this.romToolboxToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.romToolboxToolStripButton.Enabled = false;
      this.romToolboxToolStripButton.Image = global::DSPRE.Properties.Resources.exploreKit;
      this.romToolboxToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.romToolboxToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.romToolboxToolStripButton.Margin = new System.Windows.Forms.Padding(4, 6, 0, 2);
      this.romToolboxToolStripButton.Name = "romToolboxToolStripButton";
      this.romToolboxToolStripButton.Size = new System.Drawing.Size(36, 36);
      this.romToolboxToolStripButton.Text = "ROM Toolbox";
      this.romToolboxToolStripButton.ToolTipText = "ROM Toolbox";
      this.romToolboxToolStripButton.Click += new System.EventHandler(this.romToolBoxToolStripMenuItem_Click);
      // 
      // headerSearchToolStripButton
      // 
      this.headerSearchToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.headerSearchToolStripButton.Enabled = false;
      this.headerSearchToolStripButton.Image = global::DSPRE.Properties.Resources.wideLensImage;
      this.headerSearchToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.headerSearchToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.headerSearchToolStripButton.Margin = new System.Windows.Forms.Padding(4, 6, 0, 2);
      this.headerSearchToolStripButton.Name = "headerSearchToolStripButton";
      this.headerSearchToolStripButton.Size = new System.Drawing.Size(36, 36);
      this.headerSearchToolStripButton.Text = "Advanced Search (Experimental)";
      this.headerSearchToolStripButton.ToolTipText = "Search Header by Property";
      this.headerSearchToolStripButton.Click += new System.EventHandler(this.headerSearchToolStripButton_Click);
      // 
      // spawnEditorToolStripButton
      // 
      this.spawnEditorToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.spawnEditorToolStripButton.Enabled = false;
      this.spawnEditorToolStripButton.Image = global::DSPRE.Properties.Resources.spawnCoordsMatrixeditorIcon;
      this.spawnEditorToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.spawnEditorToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.spawnEditorToolStripButton.Margin = new System.Windows.Forms.Padding(4, 6, 0, 2);
      this.spawnEditorToolStripButton.Name = "spawnEditorToolStripButton";
      this.spawnEditorToolStripButton.Size = new System.Drawing.Size(57, 36);
      this.spawnEditorToolStripButton.Text = "Spawn Point Editor";
      this.spawnEditorToolStripButton.Click += new System.EventHandler(this.spawnEditorToolStripButton_Click);
      // 
      // separator_afterMiscButtons
      // 
      this.separator_afterMiscButtons.Name = "separator_afterMiscButtons";
      this.separator_afterMiscButtons.Size = new System.Drawing.Size(6, 44);
      // 
      // versionLabel
      // 
      this.versionLabel.AutoSize = true;
      this.versionLabel.Location = new System.Drawing.Point(1042, 36);
      this.versionLabel.Name = "versionLabel";
      this.versionLabel.Size = new System.Drawing.Size(35, 13);
      this.versionLabel.TabIndex = 9;
      this.versionLabel.Text = "ROM:";
      this.versionLabel.Visible = false;
      // 
      // openFileDialog1
      // 
      this.openFileDialog1.FileName = "openFileDialog1";
      // 
      // headerEditor
      // 
      this.headerEditor.Location = new System.Drawing.Point(7, 7);
      this.headerEditor.Name = "headerEditor";
      this.headerEditor.Size = new System.Drawing.Size(1164, 599);
      this.headerEditor.TabIndex = 0;
      // 
      // MainProgram
      // 
      this.AllowDrop = true;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1214, 743);
      this.Controls.Add(this.versionLabel);
      this.Controls.Add(this.languageLabel);
      this.Controls.Add(this.gameIcon);
      this.Controls.Add(this.mainToolStrip);
      this.Controls.Add(this.statusStrip1);
      this.Controls.Add(this.mainTabControl);
      this.Controls.Add(this.menuStrip1);
      this.DoubleBuffered = true;
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MainMenuStrip = this.menuStrip1;
      this.MaximizeBox = false;
      this.Name = "MainProgram";
      this.Text = "DS Pokémon Rom Editor Reloaded 1.8 (Nømura, AdAstra/LD3005)";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainProgram_FormClosing);
      this.mainTabControl.ResumeLayout(false);
      this.headerEditorTabPage.ResumeLayout(false);
      this.matrixEditorTabPage.ResumeLayout(false);
      this.mapEditorTabPage.ResumeLayout(false);
      this.nsbtxEditorTabPage.ResumeLayout(false);
      this.eventEditorTabPage.ResumeLayout(false);
      this.scriptEditorTabPage.ResumeLayout(false);
      this.textEditorTabPage.ResumeLayout(false);
      this.cameraEditorTabPage.ResumeLayout(false);
      this.trainerEditorTabPage.ResumeLayout(false);
      this.tableEditorTabPage.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.gameIcon)).EndInit();
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.statusStrip1.ResumeLayout(false);
      this.statusStrip1.PerformLayout();
      this.mainToolStrip.ResumeLayout(false);
      this.mainToolStrip.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TabControl mainTabControl;
        public System.Windows.Forms.TabPage matrixEditorTabPage;
        private System.Windows.Forms.PictureBox gameIcon;
        private System.Windows.Forms.Label languageLabel;
        public System.Windows.Forms.TabPage mapEditorTabPage;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openROMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveROMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ImageList mainTabImageList;
        private System.Windows.Forms.ToolStripButton loadRomButton;
        private System.Windows.Forms.ToolStripButton saveRomButton;
        private System.Windows.Forms.ToolStrip mainToolStrip;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.ToolStripSeparator separator_afterNsbmdUtils;
        public System.Windows.Forms.TabPage eventEditorTabPage;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem romToolboxToolStripMenuItem;
        public System.Windows.Forms.TabPage scriptEditorTabPage;
        public System.Windows.Forms.TabPage textEditorTabPage;
        private System.Windows.Forms.ToolStripButton wildEditorButton;
        public System.Windows.Forms.TabPage nsbtxEditorTabPage;
        private System.Windows.Forms.ToolStripButton romToolboxToolStripButton;
        public System.Windows.Forms.TabPage headerEditorTabPage;
        private System.Windows.Forms.ToolStripButton buildingEditorButton;
        public System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.ToolStripButton unpackAllButton;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripButton updateMapNarcsButton;
        private System.Windows.Forms.ToolStripButton headerSearchToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem headerSearchToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator separator_afterRenameUtils;
        private System.Windows.Forms.ToolStripSeparator separator_AfterOpenSave;
        private System.Windows.Forms.ToolStripButton scriptCommandsButton;
        private System.Windows.Forms.ToolStripMenuItem scriptCommandsDatabaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem diamondAndPearlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem platinumToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem heartGoldAndSoulSilverToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton spawnEditorToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem spawnEditorToolStripMenuItem;
        private System.Windows.Forms.TabPage cameraEditorTabPage;
        private System.Windows.Forms.ToolStripButton nsbmdExportTexButton;
        private System.Windows.Forms.ToolStripButton nsbmdRemoveTexButton;
        private System.Windows.Forms.ToolStripButton nsbmdAddTexButton;
        private System.Windows.Forms.ToolStripSeparator separator_afterMiscButtons;
        private System.Windows.Forms.TabPage trainerEditorTabPage;
        private System.Windows.Forms.ToolStripButton readDataFromFolderButton;
        private System.Windows.Forms.TabPage tableEditorTabPage;
        private System.Windows.Forms.ToolStripMenuItem openFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem NarcUtilityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unpackToFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem buildFomFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listBasedBatchRenameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listBasedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contentBasedToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator separator_afterFolderUnpackers;
        private System.Windows.Forms.ToolStripButton listBasedBatchRenameToolStripButton;
        private System.Windows.Forms.ToolStripButton contentBasedBatchRenameToolStripButton;
        private System.Windows.Forms.ToolStripButton unpackNARCtoFolderToolStripButton;
        private System.Windows.Forms.ToolStripButton buildNarcFromFolderToolStripButton;
        private System.Windows.Forms.ToolStripSeparator separator_afterNarcUtils;
        private System.Windows.Forms.ToolStripMenuItem listBuilderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromCEnumToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromFolderContentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton folderBasedListBuilderToolStriButton;
        private System.Windows.Forms.ToolStripButton enumBasedListBuilderToolStripButton;
        private System.Windows.Forms.ToolStripSeparator separator_afterListUtils;
        private System.Windows.Forms.ToolStripMenuItem menuViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem essentialToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem simpleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fullViewToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.DirectoryServices.DirectoryEntry directoryEntry1;
        private System.DirectoryServices.DirectoryEntry directoryEntry2;
        private System.Windows.Forms.ToolStripMenuItem nSBMDUtilityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem texturizeNSBMDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem untexturizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractNSBTXFromNSBMDToolStripMenuItem;
        public TextEditor textEditor;
    public Editors.EventEditor eventEditor;
    public Editors.MapEditor mapEditor;
    public Editors.ScriptEditor scriptEditor;
    public Editors.TrainerEditor trainerEditor;
    public Editors.TableEditor tableEditor;
    private Editors.CameraEditor cameraEditor;
    public Editors.NSBTXEditor nsbtxEditor;
    public Editors.MatrixEditor matrixEditor;
    public Editors.HeaderEditor headerEditor;
  }
}

