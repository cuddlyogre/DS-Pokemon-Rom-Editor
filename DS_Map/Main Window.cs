using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NarcAPI;
using Tao.OpenGl;
using LibNDSFormats.NSBMD;
using LibNDSFormats.NSBTX;
using DSPRE.Resources;
using DSPRE.ROMFiles;
using static DSPRE.RomInfo;
using Images;
using Ekona.Images;
using Microsoft.WindowsAPICodePack.Dialogs;
using ScintillaNET;
using ScintillaNET.Utils;
using System.Globalization;
using DSPRE.Editors;
using static DSPRE.ROMFiles.Event;
using static ScintillaNET.Style;
using static OpenTK.Graphics.OpenGL.GL;
using NSMBe4.NSBMD;

namespace DSPRE {
    public partial class MainProgram : Form {
        public MainProgram() {
            InitializeComponent();
            SetMenuLayout(Properties.Settings.Default.menuLayout); //Read user settings for menu layout
        }
        #region Program Window

        #region Variables
        public bool iconON = false;

        /* Editors Setup */
        public bool matrixEditorIsReady { get; private set; } = false;
        public bool mapEditorIsReady { get; private set; } = false;
        public bool nsbtxEditorIsReady { get; private set; } = false;
        public bool scriptEditorIsReady { get; private set; } = false;
        public bool cameraEditorIsReady { get; private set; } = false;
        public bool trainerEditorIsReady { get; private set; } = false;
        public bool tableEditorIsReady { get; private set; } = false;

        /* ROM Information */
        public static string gameCode;
        public static byte europeByte;

        #endregion

        #region Subroutines
        private void MainProgram_FormClosing(object sender, FormClosingEventArgs e) {
            if (MessageBox.Show("Are you sure you want to quit?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) {
                e.Cancel = true;
            }
            Properties.Settings.Default.Save();
        }
        private string[] GetBuildingsList(bool interior) {
            List<string> names = new List<string>();
            string path = Helpers.romInfo.GetBuildingModelsDirPath(interior);
            int buildModelsCount = Directory.GetFiles(path).Length;

            for (int i = 0; i < buildModelsCount; i++) {
                using (DSUtils.EasyReader reader = new DSUtils.EasyReader(path + "\\" + i.ToString("D4"), 0x38)) {
                    string nsbmdName = Encoding.UTF8.GetString(reader.ReadBytes(16)).TrimEnd();
                    names.Add(nsbmdName);
                }
            }
            return names.ToArray();
        }

        public void statusLabelMessage(string msg = "Ready") {
            statusLabel.Text = msg;
            statusLabel.Font = new Font(statusLabel.Font, FontStyle.Regular);
            statusLabel.ForeColor = Color.Black;
            statusLabel.Invalidate();
        }
        private void statusLabelError(string errorMsg, bool severe = true) {
            statusLabel.Text = errorMsg;
            statusLabel.Font = new Font(statusLabel.Font, FontStyle.Bold);
            statusLabel.ForeColor = severe ? Color.Red : Color.DarkOrange;
            statusLabel.Invalidate();
        }
        private void PaintGameIcon(object sender, PaintEventArgs e) {
            if (iconON) {
                FileStream banner;

                try {
                    banner = File.OpenRead(RomInfo.workDir + @"banner.bin");
                } catch (FileNotFoundException) {
                    MessageBox.Show("Couldn't load " + '"' + "banner.bin" + '"' + '.', "Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                BinaryReader readIcon = new BinaryReader(banner);
                #region Read Icon Palette
                readIcon.BaseStream.Position = 0x220;
                byte firstByte, secondByte;
                int palR, palG, palB;
                int palCounter = 0;
                int[] paletteArray = new int[48];
                
                for (int i = 0; i < 16; i++) {
                    palR = 0;
                    palG = 0;
                    palB = 0;
                    secondByte = readIcon.ReadByte();
                    firstByte = readIcon.ReadByte();

                    if ((firstByte & (1 << 6)) != 0)
                        palB |= (1 << 4);
                    if ((firstByte & (1 << 5)) != 0)
                        palB |= (1 << 3);
                    if ((firstByte & (1 << 4)) != 0)
                        palB |= (1 << 2);
                    if ((firstByte & (1 << 3)) != 0)
                        palB |= (1 << 1);
                    if ((firstByte & (1 << 2)) != 0)
                        palB |= (1 << 0);
                    if ((firstByte & (1 << 1)) != 0)
                        palG |= (1 << 4);
                    if ((firstByte & (1 << 0)) != 0)
                        palG |= (1 << 3);
                    if ((secondByte & (1 << 7)) != 0)
                        palG |= (1 << 2);
                    if ((secondByte & (1 << 6)) != 0)
                        palG |= (1 << 1);
                    if ((secondByte & (1 << 5)) != 0)
                        palG |= (1 << 0);
                    if ((secondByte & (1 << 4)) != 0)
                        palR |= (1 << 4);
                    if ((secondByte & (1 << 3)) != 0)
                        palR |= (1 << 3);
                    if ((secondByte & (1 << 2)) != 0)
                        palR |= (1 << 2);
                    if ((secondByte & (1 << 1)) != 0)
                        palR |= (1 << 1);
                    if ((secondByte & (1 << 0)) != 0)
                        palR |= (1 << 0);

                    paletteArray[palCounter++] = palR * 8;
                    paletteArray[palCounter++] = palG * 8;
                    paletteArray[palCounter++] = palB * 8;
                }
                #endregion
                #region Read Icon Image
                readIcon.BaseStream.Position = 0x20;
                int iconY = 0;
                int xTile = 0;
                int yTile = 0;
                for (int o = 0; o < 4; o++) {
                    for (int a = 0; a < 4; a++) {
                        for (int i = 0; i < 8; i++) {
                            int iconX = xTile;

                            for (int counter = 0; counter < 4; counter++) {
                                byte pixelByte = readIcon.ReadByte();
                                int pixelPalId = pixelByte & 0x0F;
                                Brush icon = new SolidBrush(Color.FromArgb(255, paletteArray[pixelPalId * 3], paletteArray[pixelPalId * 3 + 1], paletteArray[pixelPalId * 3 + 2]));
                                e.Graphics.FillRectangle(icon, iconX, i + yTile, 1, 1);
                                iconX++;
                                pixelPalId = (pixelByte & 0xF0) >> 4;
                                icon = new SolidBrush(Color.FromArgb(255, paletteArray[pixelPalId * 3], paletteArray[pixelPalId * 3 + 1], paletteArray[pixelPalId * 3 + 2]));
                                e.Graphics.FillRectangle(icon, iconX, i + yTile, 1, 1);
                                iconX++;
                            }
                            iconY++;
                        }
                        iconY = 0;
                        xTile += 8;
                    }
                    xTile = 0;
                    yTile += 8;
                }
                #endregion
                readIcon.Close();
            }
        }
        private void updateBuildingListComboBox(bool interior) {
            string[] bldList = GetBuildingsList(interior);

            buildIndexComboBox.Items.Clear();
            for (int i = 0; i < bldList.Length; i++) {
                buildIndexComboBox.Items.Add("[" + i + "] " + bldList[i]);
            }
            toolStripProgressBar.Value++;
        }

        public void SetupScriptEditor() {
            /* Extract essential NARCs sub-archives*/
            statusLabelMessage("Setting up Script Editor...");
            Update();

            DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.scripts }); //12 = scripts Narc Dir

            selectScriptFileComboBox.Items.Clear();
            int scriptCount = Directory.GetFiles(RomInfo.gameDirs[DirNames.scripts].unpackedDir).Length;
            for (int i = 0; i < scriptCount; i++) {
                selectScriptFileComboBox.Items.Add("Script File " + i);
            }

            UpdateScriptNumberCheckBox((NumberStyles)Properties.Settings.Default.scriptEditorFormatPreference);
            selectScriptFileComboBox.SelectedIndex = 0;
            statusLabelMessage();
        }

        private int UnpackRomCheckUserChoice() {
            // Check if extracted data for the ROM exists, and ask user if they want to load it.
            // Returns true if user aborted the process
            if (Directory.Exists(RomInfo.workDir)) {
                DialogResult d = MessageBox.Show("Extracted data of this ROM has been found.\n" +
                    "Do you want to load it and unpack it?", "Data detected", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (d == DialogResult.Cancel) {
                    return -1; //user wants to abort loading
                } else if (d == DialogResult.Yes) {
                    return 0; //user wants to load data
                } else {
                    DialogResult nd = MessageBox.Show("All data of this ROM will be re-extracted. Proceed?\n",
                        "Existing data will be deleted", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (nd == DialogResult.No) {
                        return -1; //user wants to abort loading
                    } else {
                        return 1; //user wants to re-extract data
                    }
                }
            } else {
                return 2; //No data found
            }
        }
        private bool UnpackRom(string ndsFileName) {
            statusLabelMessage("Unpacking ROM contents to " + RomInfo.workDir + " ...");
            Update();

            Directory.CreateDirectory(RomInfo.workDir);
            Process unpack = new Process();
            unpack.StartInfo.FileName = @"Tools\ndstool.exe";
            unpack.StartInfo.Arguments = "-x " + '"' + ndsFileName + '"'
                + " -9 " + '"' + RomInfo.arm9Path + '"'
                + " -7 " + '"' + RomInfo.workDir + "arm7.bin" + '"'
                + " -y9 " + '"' + RomInfo.workDir + "y9.bin" + '"'
                + " -y7 " + '"' + RomInfo.workDir + "y7.bin" + '"'
                + " -d " + '"' + RomInfo.workDir + "data" + '"'
                + " -y " + '"' + RomInfo.workDir + "overlay" + '"'
                + " -t " + '"' + RomInfo.workDir + "banner.bin" + '"'
                + " -h " + '"' + RomInfo.workDir + "header.bin" + '"';
            Application.DoEvents();
            unpack.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            unpack.StartInfo.CreateNoWindow = true;
            try {
                unpack.Start();
                unpack.WaitForExit();
            } catch (System.ComponentModel.Win32Exception) {
                MessageBox.Show("Failed to call ndstool.exe" + Environment.NewLine + "Make sure DSPRE's Tools folder is intact.",
                    "Couldn't unpack ROM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        #endregion

        private void romToolBoxToolStripMenuItem_Click(object sender, EventArgs e) {
            using (ROMToolboxDialog window = new ROMToolboxDialog()) {
                window.ShowDialog();
                if (ROMToolboxDialog.flag_standardizedItems && eventEditor.eventEditorIsReady) {
                    eventEditor.UpdateItemComboBox(RomInfo.GetItemNames());
                }
                if (ROMToolboxDialog.flag_DynamicHeadersPatchApplied) {
                    addHeaderBTN.Enabled = true;
                    removeLastHeaderBTN.Enabled = true;
                }
            }
        }

        private void scriptCommandsDatabaseToolStripButton_Click(object sender, EventArgs e) {
            OpenCommandsDatabase(RomInfo.ScriptCommandNamesDict, RomInfo.ScriptCommandParametersDict, RomInfo.ScriptActionNamesDict, RomInfo.ScriptComparisonOperatorsDict);
        }
        private void nsbmdExportTexButton_Click(object sender, EventArgs e) {
            OpenFileDialog of = new OpenFileDialog {
                Filter = MapFile.TexturedNSBMDFilter
            };
            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            byte[] modelFile = DSUtils.ReadFromFile(of.FileName);
            if (DSUtils.CheckNSBMDHeader(modelFile) == DSUtils.NSBMD_DOESNTHAVE_TEXTURE) {
                MessageBox.Show("This NSBMD file is untextured.", "No textures to extract", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //============================================================
            MessageBox.Show("Choose where to save the textures.", "Choose destination path", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SaveFileDialog texSf = new SaveFileDialog {
                Filter = "NSBTX File(*.nsbtx)|*.nsbtx",
                FileName = Path.GetFileNameWithoutExtension(of.FileName)
            };
            if (texSf.ShowDialog() != DialogResult.OK) {
                return;
            }

            DSUtils.WriteToFile(texSf.FileName, DSUtils.GetTexturesFromTexturedNSBMD(modelFile));
            MessageBox.Show("The textures of " + of.FileName + " have been extracted and saved.", "Textures saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void nsbmdRemoveTexButton_Click(object sender, EventArgs e) {
            OpenFileDialog of = new OpenFileDialog {
                Filter = MapFile.TexturedNSBMDFilter
            };
            
            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            byte[] modelFile = DSUtils.ReadFromFile(of.FileName);
            if (DSUtils.CheckNSBMDHeader(modelFile) == DSUtils.NSBMD_DOESNTHAVE_TEXTURE) {
                MessageBox.Show("This NSBMD file is already untextured.", "No textures to remove", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string extramsg = "";
            DialogResult d = MessageBox.Show("Would you like to save the removed textures to a file?", "Save textures?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (d.Equals(DialogResult.Yes)) {

                MessageBox.Show("Choose where to save the textures.", "Choose destination path", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SaveFileDialog texSf = new SaveFileDialog {
                    Filter = "NSBTX File(*.nsbtx)|*.nsbtx",
                    FileName = Path.GetFileNameWithoutExtension(of.FileName)
                };

                if (texSf.ShowDialog() == DialogResult.OK) {
                    DSUtils.WriteToFile(texSf.FileName, DSUtils.GetTexturesFromTexturedNSBMD(modelFile));
                    extramsg = " exported and";
                }
            }

            //============================================================
            MessageBox.Show("Choose where to save the untextured model.", "Choose destination path", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SaveFileDialog sf = new SaveFileDialog {
                Filter = "Untextured NSBMD File(*.nsbmd)|*.nsbmd",
                FileName = Path.GetFileNameWithoutExtension(of.FileName) + "_untextured"
            };
            if (sf.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            DSUtils.WriteToFile(sf.FileName, DSUtils.GetModelWithoutTextures(modelFile));
            MessageBox.Show("Textures correctly" + extramsg + " removed!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void nsbmdAddTexButton_Click(object sender, EventArgs e) {
            OpenFileDialog of = new OpenFileDialog {
                Filter = MapFile.UntexturedNSBMDFilter
            };
            if (of.ShowDialog(this) != DialogResult.OK)
                return;

            byte[] modelFile = File.ReadAllBytes(of.FileName);
            if (DSUtils.CheckNSBMDHeader(modelFile) == DSUtils.NSBMD_HAS_TEXTURE) {
                DialogResult d = MessageBox.Show("This NSBMD file is already textured.\nDo you want to overwrite its textures?", "Textures found", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (d.Equals(DialogResult.No)) {
                    return;
                }
            }

            MessageBox.Show("Select the new NSBTX texture file.", "Choose NSBTX", MessageBoxButtons.OK, MessageBoxIcon.Information);

            OpenFileDialog openNsbtx = new OpenFileDialog {
                Filter = "NSBTX File(*.nsbtx)|*.nsbtx"
            };
            if (openNsbtx.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            byte[] textureFile = File.ReadAllBytes(openNsbtx.FileName);


            //============================================================
            MessageBox.Show("Choose where to save the new textured model.", "Choose destination path", MessageBoxButtons.OK, MessageBoxIcon.Information);

            string texturedPath = Path.GetFileNameWithoutExtension(of.FileName);
            if (texturedPath.Contains("_untextured")) {
                texturedPath = texturedPath.Substring(0, texturedPath.Length - "_untextured".Length);
            }

            SaveFileDialog sf = new SaveFileDialog {
                Filter = MapFile.TexturedNSBMDFilter,
                FileName = Path.GetFileNameWithoutExtension(of.FileName) + "_textured"
            };

            if (sf.ShowDialog(this) != DialogResult.OK)
                return;

            DSUtils.WriteToFile(sf.FileName, DSUtils.BuildNSBMDwithTextures(modelFile, textureFile), fmode: FileMode.Create);
            MessageBox.Show("Textures correctly written to NSBMD file.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void OpenCommandsDatabase(Dictionary<ushort, string> namesDict, Dictionary<ushort, byte[]> paramsDict, Dictionary<ushort, string> actionsDict,
            Dictionary<ushort, string> comparisonOPsDict) {
            statusLabelMessage("Setting up Commands Database. Please wait...");
            Update();
            CommandsDatabase form = new CommandsDatabase(namesDict, paramsDict, actionsDict, comparisonOPsDict);
            form.Show();
            statusLabelMessage();
        }
        private void headerSearchToolStripButton_Click(object sender, EventArgs e) {
            mainTabControl.SelectedIndex = 0; //Select Header Editor
            using (HeaderSearch h = new HeaderSearch(ref internalNames, headerListBox, statusLabel)) {
                h.ShowDialog();
            }
        }
        private void advancedHeaderSearchToolStripMenuItem_Click(object sender, EventArgs e) {
            headerSearchToolStripButton_Click(null, null);
        }
        private void buildingEditorButton_Click(object sender, EventArgs e) {
            unpackBuildingEditorNARCs();

            using (BuildingEditor editor = new BuildingEditor(Helpers.romInfo))
                editor.ShowDialog();
        }
        private void unpackBuildingEditorNARCs(bool forceUnpack = false) {
            toolStripProgressBar.Visible = true;

            statusLabelMessage("Attempting to unpack Building Editor NARCs... Please wait. This might take a while");
            toolStripProgressBar.Visible = true;
            toolStripProgressBar.Maximum = 4;
            toolStripProgressBar.Value = 0;
            Update();

            List<DirNames> toUnpack = new List<DirNames> {
                DirNames.exteriorBuildingModels,
                DirNames.buildingConfigFiles,
                DirNames.buildingTextures,
                DirNames.areaData
            };

            if (forceUnpack) {
                DSUtils.ForceUnpackNarcs(toUnpack);

                if (RomInfo.gameFamily == gFamEnum.HGSS) {
                    DSUtils.ForceUnpackNarcs(new List<DirNames> { DirNames.interiorBuildingModels });// Last = interior buildings dir
                }
            } else {
                DSUtils.TryUnpackNarcs(toUnpack);

                if (RomInfo.gameFamily == gFamEnum.HGSS) {
                    DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.interiorBuildingModels });
                }
            }

            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Visible = false;
            statusLabelMessage();
            Update();
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            string message = "DS Pokémon ROM Editor Reloaded by AdAstra/LD3005" + Environment.NewLine + "version 1.8.0" + Environment.NewLine
                + Environment.NewLine + "Based on Nømura's DS Pokémon ROM Editor 1.0.4, largely inspired by Markitus95's \"Spiky's DS Map Editor\" (SDSME), from which certain assets were also recycled. " +
                "Credits go to Markitus, Ark, Zark, Florian, and everyone else who deserves credit for SDSME." + Environment.NewLine
                + Environment.NewLine + "Special thanks to Trifindo, Mikelan98, JackHack96, Pleonex and BagBoy."
                + Environment.NewLine + "Their help, research and expertise in many fields of NDS ROM Hacking made the development of this tool possible.";

            MessageBox.Show(message, "About...", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void loadRom_Click(object sender, EventArgs e) {
            OpenFileDialog openRom = new OpenFileDialog {
                Filter = DSUtils.NDSRomFilter
            }; // Select ROM
            if (openRom.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            SetupROMLanguage(openRom.FileName);
            /* Set ROM gameVersion and language */
            Helpers.romInfo = new RomInfo(gameCode, openRom.FileName, useSuffix: true);

            if (string.IsNullOrWhiteSpace(RomInfo.romID) || string.IsNullOrWhiteSpace(RomInfo.fileName)) {
                return;
            }

            CheckROMLanguage();

            int userchoice = UnpackRomCheckUserChoice();
            switch (userchoice) {
                case -1:
                    statusLabelMessage("Loading aborted");
                    Update();
                    return;
                case 0:
                    break;
                case 1:
                case 2:
                    Application.DoEvents();
                    if (userchoice == 1) {
                        statusLabelMessage("Deleting old data...");
                        try {
                            Directory.Delete(RomInfo.workDir, true);
                        } catch (IOException) {
                            MessageBox.Show("Concurrent access detected: \n" + RomInfo.workDir +
                                "\nMake sure no other process is using the extracted ROM folder while DSPRE is running.", "Concurrent Access", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        Update();
                    }

                    try {
                        if (!UnpackRom(openRom.FileName)) {
                            statusLabelError("ERROR");
                            languageLabel.Text = "";
                            versionLabel.Text = "Error";
                            return;
                        }
                        DSUtils.ARM9.EditSize(-12);
                    } catch (IOException) {
                        MessageBox.Show("Can't access temp directory: \n" + RomInfo.workDir + "\nThis might be a temporary issue.\nMake sure no other process is using it and try again.", "Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        statusLabelError("ERROR: Concurrent access to " + RomInfo.workDir);
                        Update();
                        return;
                    }
                    break;
            }

            iconON = true;
            gameIcon.Refresh();  // Paint game icon
            statusLabelMessage("Attempting to unpack NARCs from folder...");
            Update();

            ReadROMInitData();
        }

        private void CheckROMLanguage() {
            versionLabel.Visible = true;
            languageLabel.Visible = true;

            versionLabel.Text = RomInfo.gameVersion.ToString() + " " + "[" + RomInfo.romID + "]";
            languageLabel.Text = "Lang: " + RomInfo.gameLanguage;

            if (RomInfo.gameLanguage == gLangEnum.English) {
                if (europeByte == 0x0A) {
                    languageLabel.Text += " [Europe]";
                } else {
                    languageLabel.Text += " [America]";
                }
            }
        }

        private void readDataFromFolderButton_Click(object sender, EventArgs e) {
            CommonOpenFileDialog romFolder = new CommonOpenFileDialog {
                IsFolderPicker = true,
                Multiselect = false
            };
            if (romFolder.ShowDialog() != CommonFileDialogResult.Ok) {
                return;
            }

            try {
                SetupROMLanguage(Directory.GetFiles(romFolder.FileName).First(x => x.Contains("header.bin")));
            } catch (InvalidOperationException) {
                MessageBox.Show("This folder does not seem to contain any data from a NDS Pokémon ROM.", "No ROM Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            /* Set ROM gameVersion and language */
            Helpers.romInfo = new RomInfo(gameCode, romFolder.FileName, useSuffix: false);

            if (string.IsNullOrWhiteSpace(RomInfo.romID) || string.IsNullOrWhiteSpace(RomInfo.fileName)) {
                return;
            }

            CheckROMLanguage();
            
            iconON = true;
            gameIcon.Refresh();  // Paint game icon

            ReadROMInitData();
        }

        private void SetupROMLanguage(string headerPath) {
            using (DSUtils.EasyReader br = new DSUtils.EasyReader(headerPath, 0xC)) {
                gameCode = Encoding.UTF8.GetString(br.ReadBytes(4));
                br.BaseStream.Position = 0x1E;
                europeByte = br.ReadByte();
            }
        }

        private void ReadROMInitData() {
            if ( DSUtils.ARM9.CheckCompressionMark() ) {
                if ( !RomInfo.gameFamily.Equals(gFamEnum.HGSS) ) {
                    MessageBox.Show("Unexpected compressed ARM9. It is advised that you double check the ARM9.");
                }
                if (!DSUtils.ARM9.Decompress(RomInfo.arm9Path)) {
                    MessageBox.Show("ARM9 decompression failed. The program can't proceed.\nAborting.",
                                "Error with ARM9 decompression", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            /* Setup essential editors */
            SetupHeaderEditor();
            eventEditor.eventOpenGlControl.InitializeContexts();
            mapOpenGlControl.InitializeContexts();

            mainTabControl.Show();
            loadRomButton.Enabled = false;
            readDataFromFolderButton.Enabled = false;
            saveRomButton.Enabled = true;
            saveROMToolStripMenuItem.Enabled = true;
            openROMToolStripMenuItem.Enabled = false;
            openFolderToolStripMenuItem.Enabled = false;

            unpackAllButton.Enabled = true;
            updateMapNarcsButton.Enabled = true;

            buildingEditorButton.Enabled = true;
            wildEditorButton.Enabled = true;

            romToolboxToolStripButton.Enabled = true;
            romToolboxToolStripMenuItem.Enabled = true;
            headerSearchToolStripButton.Enabled = true;
            headerSearchToolStripMenuItem.Enabled = true;
            spawnEditorToolStripButton.Enabled = true;
            spawnEditorToolStripMenuItem.Enabled = true;

            scriptCommandsButton.Enabled = true;
            statusLabelMessage();
            this.Text += "  -  " + RomInfo.fileName;
        }

        private void saveRom_Click(object sender, EventArgs e) {
            SaveFileDialog saveRom = new SaveFileDialog {
                Filter = DSUtils.NDSRomFilter
            };
            if (saveRom.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            statusLabelMessage("Repacking NARCS...");
            Update();

            // Repack NARCs
            foreach (KeyValuePair<DirNames, (string packedDir, string unpackedDir)> kvp in RomInfo.gameDirs) {
                DirectoryInfo di = new DirectoryInfo(kvp.Value.unpackedDir);
                if (di.Exists) {
                    Narc.FromFolder(kvp.Value.unpackedDir).Save(kvp.Value.packedDir); // Make new NARC from folder
                }
            }


            if ( DSUtils.ARM9.CheckCompressionMark() ) {
                statusLabelMessage("Awaiting user response...");
                DialogResult d = MessageBox.Show("The ARM9 file of this ROM is currently uncompressed, but marked as compressed.\n" +
                    "This will prevent your ROM from working on native hardware.\n\n" +
                "Do you want to mark the ARM9 as uncompressed?", "ARM9 compression mismatch detected",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (d == DialogResult.Yes) {
                    DSUtils.ARM9.WriteBytes( new byte[4] { 0, 0, 0, 0 }, (uint)(RomInfo.gameFamily == gFamEnum.DP ? 0xB7C : 0xBB4) );
                }
            }

            statusLabelMessage("Repacking ROM...");

            if (DSUtils.CheckOverlayHasCompressionFlag(1)) {
                if (ROMToolboxDialog.overlay1MustBeRestoredFromBackup) {
                    DSUtils.RestoreOverlayFromCompressedBackup(1, eventEditor.eventEditorIsReady);
                } else {
                    if (!DSUtils.OverlayIsCompressed(1)) {
                        DSUtils.CompressOverlay(1);
                    }
                }
            }

            if (DSUtils.CheckOverlayHasCompressionFlag(RomInfo.initialMoneyOverlayNumber)) {
                if (!DSUtils.OverlayIsCompressed(RomInfo.initialMoneyOverlayNumber)) {
                    DSUtils.CompressOverlay(RomInfo.initialMoneyOverlayNumber);
                }
            }

            
            Update();

            DSUtils.RepackROM(saveRom.FileName);

            if (RomInfo.gameFamily != gFamEnum.DP && RomInfo.gameFamily != gFamEnum.Plat) {
                if (eventEditor.eventEditorIsReady) {
                    if (DSUtils.OverlayIsCompressed(1)) {
                        DSUtils.DecompressOverlay(1);
                    }
                }
            }

            Properties.Settings.Default.Save();
            statusLabelMessage();
        }
        private void unpackAllButton_Click(object sender, EventArgs e) {
            statusLabelMessage("Awaiting user response...");
            DialogResult d = MessageBox.Show("Do you wish to unpack all extracted NARCS?\n" +
                "This operation might be long and can't be interrupted.\n\n" +
                "Any unsaved changes made to the ROM in this session will be lost." +
                "\nProceed?", "About to unpack all NARCS",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (d == DialogResult.Yes) {
                toolStripProgressBar.Maximum = RomInfo.gameDirs.Count;
                toolStripProgressBar.Visible = true;
                toolStripProgressBar.Value = 0;
                statusLabelMessage("Attempting to unpack all NARCs... Be patient. This might take a while...");
                Update();

                DSUtils.ForceUnpackNarcs(Enum.GetValues(typeof(DirNames)).Cast<DirNames>().ToList());
                MessageBox.Show("Operation completed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                toolStripProgressBar.Value = 0;
                toolStripProgressBar.Visible = false;

                SetupHeaderEditor();
                SetupMatrixEditor();
                SetupMapEditor();
                SetupNSBTXEditor();
                eventEditor.SetupEventEditor();
                SetupScriptEditorTextAreas();
                SetupScriptEditor();
                textEditor.SetupTextEditor();
                SetupTrainerEditor();

                statusLabelMessage();
                Update();
            }
        }
        private void updateMapNarcsButton_Click(object sender, EventArgs e) {
            statusLabelMessage("Awaiting user response...");
            DialogResult d = MessageBox.Show("Do you wish to unpack all NARC files necessary for the Building Editor ?\n" +
               "This operation might be long and can't be interrupted.\n\n" +
               "Any unsaved changes made to building models and textures in this session will be lost." +
               "\nProceed?", "About to unpack Building NARCs",
               MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (d == DialogResult.Yes) {
                unpackBuildingEditorNARCs(forceUnpack: true);

                MessageBox.Show("Operation completed.", "Success",
                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                statusLabelMessage();

                if (mapEditorIsReady) {
                    updateBuildingListComboBox(interiorbldRadioButton.Checked);
                }
                Update();
            }
        }
        private void diamondAndPearlToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenCommandsDatabase(RomInfo.BuildCommandNamesDatabase(gFamEnum.DP), RomInfo.BuildCommandParametersDatabase(gFamEnum.DP),
                RomInfo.BuildActionNamesDatabase(gFamEnum.DP), RomInfo.BuildComparisonOperatorsDatabase(gFamEnum.DP));
        }
        private void platinumToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenCommandsDatabase(RomInfo.BuildCommandNamesDatabase(gFamEnum.Plat), RomInfo.BuildCommandParametersDatabase(gFamEnum.Plat),
                RomInfo.BuildActionNamesDatabase(gFamEnum.Plat), RomInfo.BuildComparisonOperatorsDatabase(gFamEnum.Plat));
        }
        private void heartGoldAndSoulSilverToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenCommandsDatabase(RomInfo.BuildCommandNamesDatabase(gFamEnum.HGSS), RomInfo.BuildCommandParametersDatabase(gFamEnum.HGSS),
                RomInfo.BuildActionNamesDatabase(gFamEnum.HGSS), RomInfo.BuildComparisonOperatorsDatabase(gFamEnum.HGSS));
        }

        private void mainTabControl_SelectedIndexChanged(object sender, EventArgs e) {
            if (mainTabControl.SelectedTab == headerEditorTabPage) {
                //
            } else if (mainTabControl.SelectedTab == matrixEditorTabPage) {
                if (!matrixEditorIsReady) {
                    SetupMatrixEditor();
                    matrixEditorIsReady = true;
                }
            } else if (mainTabControl.SelectedTab == mapEditorTabPage) {
                if (!mapEditorIsReady) {
                    SetupMapEditor();
                    mapOpenGlControl.MouseWheel += new MouseEventHandler(mapOpenGlControl_MouseWheel);
                    mapEditorIsReady = true;
                }
            } else if (mainTabControl.SelectedTab == nsbtxEditorTabPage) {
                if (!nsbtxEditorIsReady) {
                    SetupNSBTXEditor();
                    nsbtxEditorIsReady = true;
                }
            } else if (mainTabControl.SelectedTab == eventEditorTabPage) {
                if (!eventEditor.eventEditorIsReady) {
                    eventEditor.SetupEventEditor();
                    eventEditor.eventEditorIsReady = true;
                }
            } else if (mainTabControl.SelectedTab == scriptEditorTabPage) {
                if (!scriptEditorIsReady) {
                    SetupScriptEditorTextAreas();
                    SetupScriptEditor();
                    scriptEditorIsReady = true;
                }
            } else if (mainTabControl.SelectedTab == textEditorTabPage) {
                if (!textEditor.textEditorIsReady) {
                    textEditor.SetupTextEditor();
                    textEditor.textEditorIsReady = true;
                }
            } else if (mainTabControl.SelectedTab == cameraEditorTabPage) {
                if (!cameraEditorIsReady) {
                    SetupCameraEditor();
                    cameraEditorIsReady = true;
                }
            } else if (mainTabControl.SelectedTab == trainerEditorTabPage) {
                if (!trainerEditorIsReady) {
                    SetupTrainerEditor();
                    trainerEditorIsReady = true;
                }
            } else if (mainTabControl.SelectedTab == tableEditorTabPage) {
                if (!tableEditorIsReady) {
                    resetHeaderSearch();
                    SetupConditionalMusicTable();
                    SetupBattleEffectsTables();
                    tableEditorIsReady = true;
                }
            }
        }

        private void spawnEditorToolStripButton_Click(object sender, EventArgs e) {
            if (!matrixEditorIsReady) {
                SetupMatrixEditor();
            }
            using (SpawnEditor ed = new SpawnEditor(headerListBoxNames)) {
                ed.ShowDialog();
            }
        }
        private void spawnEditorToolStripMenuItem_Click(object sender, EventArgs e) {
            spawnEditorToolStripButton_Click(null, null);
        }
        private void wildEditorButton_Click(object sender, EventArgs e) {
            openWildEditor(loadCurrent: false);
        }
        private void openWildEditorWithIdButtonClick(object sender, EventArgs e) {
            openWildEditor(loadCurrent: true);
        }
        private void openWildEditor(bool loadCurrent) {
            statusLabelMessage("Attempting to extract Wild Encounters NARC...");
            Update();

            DSUtils.TryUnpackNarcs(new List<DirNames>() { DirNames.encounters, DirNames.monIcons });

            statusLabelMessage("Passing control to Wild Pokémon Editor...");
            Update();

            int encToOpen = loadCurrent ? (int)wildPokeUpDown.Value : 0;

            string wildPokeUnpackedPath = gameDirs[DirNames.encounters].unpackedDir;
            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                case gFamEnum.Plat:
                    using (WildEditorDPPt editor = new WildEditorDPPt(wildPokeUnpackedPath, RomInfo.GetPokemonNames(), encToOpen))
                        editor.ShowDialog();
                    break;
                default:
                    using (WildEditorHGSS editor = new WildEditorHGSS(wildPokeUnpackedPath, RomInfo.GetPokemonNames(), encToOpen))
                        editor.ShowDialog();
                    break;
            }
            statusLabelMessage();
        }
        #endregion

        #region Header Editor

        #region Variables
        public MapHeader currentHeader;
        public List<string> internalNames;
        public List<string> headerListBoxNames;
        #endregion
        private void SetupHeaderEditor() {
            /* Extract essential NARCs sub-archives*/

            statusLabelMessage("Attempting to unpack Header Editor NARCs... Please wait.");
            Update();

            DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.synthOverlay, DirNames.textArchives, DirNames.dynamicHeaders });

            statusLabelMessage("Reading internal names... Please wait.");
            Update();

            internalNames = new List<string>();
            headerListBoxNames = new List<string>();
            int headerCount;
            if (ROMToolboxDialog.flag_DynamicHeadersPatchApplied || ROMToolboxDialog.CheckFilesDynamicHeadersPatchApplied()) {
                addHeaderBTN.Enabled = true;
                removeLastHeaderBTN.Enabled = true;
                headerCount = Directory.GetFiles(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir).Length;
            } else {
                headerCount = RomInfo.GetHeaderCount();
            }

            /* Read Header internal names */
            try {
                using (DSUtils.EasyReader reader = new DSUtils.EasyReader(RomInfo.internalNamesLocation)) {
                    for (int i = 0; i < headerCount; i++) {
                        byte[] row = reader.ReadBytes(RomInfo.internalNameLength);

                        string internalName = Encoding.ASCII.GetString(row);//.TrimEnd();
                        headerListBoxNames.Add(i.ToString("D3") + MapHeader.nameSeparator + internalName);
                        internalNames.Add(internalName.TrimEnd('\0'));
                    }
                }

                headerListBox.Items.Clear();
                headerListBox.Items.AddRange(headerListBoxNames.ToArray());
            } catch (FileNotFoundException) {
                MessageBox.Show(RomInfo.internalNamesLocation + " doesn't exist.", "Couldn't read internal names", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            /*Add list of options to each control */
            textEditor.currentTextArchive = new TextArchive(RomInfo.locationNamesTextNumber);
            textEditor.ReloadHeaderEditorLocationsList(textEditor.currentTextArchive.messages);

            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                    areaIconComboBox.Enabled = false;
                    areaIconPictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject("dpareaicon");
                    areaSettingsLabel.Text = "Show nametag:";
                    cameraComboBox.Items.Clear();
                    musicDayComboBox.Items.Clear();
                    musicNightComboBox.Items.Clear();
                    areaSettingsComboBox.Items.Clear();
                    cameraComboBox.Items.AddRange(PokeDatabase.CameraAngles.DPPtCameraDict.Values.ToArray());
                    musicDayComboBox.Items.AddRange(PokeDatabase.MusicDB.DPMusicDict.Values.ToArray());
                    musicNightComboBox.Items.AddRange(PokeDatabase.MusicDB.DPMusicDict.Values.ToArray());
                    areaSettingsComboBox.Items.AddRange(PokeDatabase.ShowName.DPShowNameValues);
                    weatherComboBox.Items.AddRange(PokeDatabase.Weather.DPWeatherDict.Values.ToArray());
                    wildPokeUpDown.Maximum = 65535;

                    battleBackgroundLabel.Location = new Point(battleBackgroundLabel.Location.X - 25, battleBackgroundLabel.Location.Y - 8);
                    battleBackgroundUpDown.Location = new Point(battleBackgroundUpDown.Location.X - 25, battleBackgroundUpDown.Location.Y - 8);
                    break;
                case gFamEnum.Plat:
                    areaSettingsLabel.Text = "Show nametag:";
                    areaIconComboBox.Items.Clear();
                    cameraComboBox.Items.Clear();
                    musicDayComboBox.Items.Clear();
                    musicNightComboBox.Items.Clear();
                    areaSettingsComboBox.Items.Clear();
                    weatherComboBox.Items.Clear();
                    areaIconComboBox.Items.AddRange(PokeDatabase.Area.PtAreaIconValues);
                    cameraComboBox.Items.AddRange(PokeDatabase.CameraAngles.DPPtCameraDict.Values.ToArray());
                    musicDayComboBox.Items.AddRange(PokeDatabase.MusicDB.PtMusicDict.Values.ToArray());
                    musicNightComboBox.Items.AddRange(PokeDatabase.MusicDB.PtMusicDict.Values.ToArray());
                    areaSettingsComboBox.Items.AddRange(PokeDatabase.ShowName.PtShowNameValues);
                    weatherComboBox.Items.AddRange(PokeDatabase.Weather.PtWeatherDict.Values.ToArray());
                    wildPokeUpDown.Maximum = 65535;

                    battleBackgroundLabel.Location = new Point(battleBackgroundLabel.Location.X - 25, battleBackgroundLabel.Location.Y - 8);
                    battleBackgroundUpDown.Location = new Point(battleBackgroundUpDown.Location.X - 25, battleBackgroundUpDown.Location.Y - 8);
                    break;
                default:
                    areaSettingsLabel.Text = "Area Settings:";
                    areaIconComboBox.Items.Clear();
                    cameraComboBox.Items.Clear();
                    areaSettingsComboBox.Items.Clear();
                    musicDayComboBox.Items.Clear();
                    musicNightComboBox.Items.Clear();
                    weatherComboBox.Items.Clear();
                    areaIconComboBox.Items.AddRange(PokeDatabase.Area.HGSSAreaIconsDict.Values.ToArray());
                    cameraComboBox.Items.AddRange(PokeDatabase.CameraAngles.HGSSCameraDict.Values.ToArray());
                    areaSettingsComboBox.Items.AddRange(PokeDatabase.Area.HGSSAreaProperties);
                    musicDayComboBox.Items.AddRange(PokeDatabase.MusicDB.HGSSMusicDict.Values.ToArray());
                    musicNightComboBox.Items.AddRange(PokeDatabase.MusicDB.HGSSMusicDict.Values.ToArray());
                    weatherComboBox.Items.AddRange(PokeDatabase.Weather.HGSSWeatherDict.Values.ToArray());
                    wildPokeUpDown.Maximum = 255;

                    followModeComboBox.Visible = true;
                    followModeLabel.Visible = true;
                    johtoRadioButton.Visible = true;
                    kantoRadioButton.Visible = true;

                    flag6CheckBox.Visible = true;
                    flag5CheckBox.Visible = true;
                    flag4CheckBox.Visible = true;
                    flag6CheckBox.Text = "Flag ?";
                    flag5CheckBox.Text = "Flag ?";
                    flag4CheckBox.Text = "Flag ?";

                    worldmapCoordsGroupBox.Enabled = true;
                    break;
            }
            if (headerListBox.Items.Count > 0) {
                headerListBox.SelectedIndex = 0;
            }
            statusLabelMessage();
        }
        private void addHeaderBTN_Click(object sender, EventArgs e) {
            // Add new file in the dynamic headers directory
            string sourcePath = RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + "0000";
            string destPath = RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + RomInfo.GetHeaderCount().ToString("D4");
            File.Copy(sourcePath, destPath);

            // Add row to internal names table
            const string newmap = "NEWMAP";
            DSUtils.WriteToFile(RomInfo.internalNamesLocation, StringToInternalName(newmap), (uint)RomInfo.GetHeaderCount() * RomInfo.internalNameLength);

            // Update headers ListBox and internal names list
            headerListBox.Items.Add(headerListBox.Items.Count + MapHeader.nameSeparator + " " + newmap);
            headerListBoxNames.Add(headerListBox.Items.Count + MapHeader.nameSeparator + " " + newmap);
            internalNames.Add(newmap);

            // Select new header
            headerListBox.SelectedIndex = headerListBox.Items.Count - 1;
        }
        private void removeLastHeaderBTN_Click(object sender, EventArgs e) {
            /* Check if currently selected file is the last one, and in that case select the one before it */
            int lastIndex = headerListBox.Items.Count - 1;

            if (lastIndex > 0) { //there are at least 2 elements
                if (headerListBox.SelectedIndex == lastIndex) {
                    headerListBox.SelectedIndex--;
                }

                /* Physically delete last header file */
                File.Delete(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + lastIndex.ToString("D4"));
                using (DSUtils.EasyWriter ew = new DSUtils.EasyWriter(RomInfo.internalNamesLocation)) {
                    ew.EditSize(-internalNameLength); //Delete internalNameLength amount of bytes from file end
                }

                /* Remove item from collections */
                headerListBox.Items.RemoveAt(lastIndex);
                internalNames.RemoveAt(lastIndex);
                headerListBoxNames.RemoveAt(lastIndex);
            } else {
                MessageBox.Show("You must have at least one header!", "Can't delete last header", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void areaDataUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }
            currentHeader.areaDataID = (byte)areaDataUpDown.Value;
        }
        private void internalNameBox_TextChanged(object sender, EventArgs e) {
            if (internalNameBox.Text.Length > 13) {
                internalNameLenLabel.ForeColor = Color.FromArgb(255, 0, 0);
            } else if (internalNameBox.Text.Length > 7) {
                internalNameLenLabel.ForeColor = Color.FromArgb(190, 190, 0);
            } else {
                internalNameLenLabel.ForeColor = Color.FromArgb(0, 180, 0);
            }

            internalNameLenLabel.Text = "[ " + (internalNameBox.Text.Length) + " ]";
        }
        private void areaIconComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }

            string imageName;
            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                    break;
                case gFamEnum.Plat:
                    ((HeaderPt)currentHeader).areaIcon = (byte)areaIconComboBox.SelectedIndex;
                    imageName = "areaicon0" + areaIconComboBox.SelectedIndex.ToString();
                    areaIconPictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(imageName);
                    break;
                default:
                    ((HeaderHGSS)currentHeader).areaIcon = (byte)areaIconComboBox.SelectedIndex;
                    imageName = PokeDatabase.System.AreaPics.hgssAreaPicDict[areaIconComboBox.SelectedIndex];
                    areaIconPictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(imageName);
                    break;
            }
        }
        private void eventFileUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }
            currentHeader.eventFileID = (ushort)eventFileUpDown.Value;
        }
        private void battleBackgroundUpDown_ValueChanged(object sender, EventArgs e) {
            currentHeader.battleBackground = (byte)battleBackgroundUpDown.Value;
        }
        private void followModeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }

            if (RomInfo.gameFamily == gFamEnum.HGSS) {
                HeaderHGSS currentHeaderHGSS = (HeaderHGSS)currentHeader;
                currentHeaderHGSS.followMode = (byte)followModeComboBox.SelectedIndex;
            }
        }

        private void kantoRadioButton_CheckedChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }
            if (RomInfo.gameFamily == gFamEnum.HGSS) {
                HeaderHGSS currentHeaderHGSS = (HeaderHGSS)currentHeader;
                currentHeaderHGSS.kantoFlag = kantoRadioButton.Checked;
            }
        }
        private void headerFlagsCheckBoxes_CheckedChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }

            byte flagVal = 0;
            if (flag0CheckBox.Checked)
                flagVal += (byte)Math.Pow(2, 0);

            if (flag1CheckBox.Checked)
                flagVal += (byte)Math.Pow(2, 1);

            if (flag2CheckBox.Checked)
                flagVal += (byte)Math.Pow(2, 2);

            if (flag3CheckBox.Checked)
                flagVal += (byte)Math.Pow(2, 3);

            if (RomInfo.gameFamily == gFamEnum.HGSS) {
                if (flag4CheckBox.Checked)
                    flagVal += (byte)Math.Pow(2, 4);
                if (flag5CheckBox.Checked)
                    flagVal += (byte)Math.Pow(2, 5);
                if (flag6CheckBox.Checked)
                    flagVal += (byte)Math.Pow(2, 6);
                //if (flag7CheckBox.Checked)
                //    flagVal += (byte)Math.Pow(2, 7);
            }
            currentHeader.flags = flagVal;
        }
        private void headerListBox_SelectedValueChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers || headerListBox.SelectedIndex < 0) {
                return;
            }

            /* Obtain current header ID from listbox*/
            if (!ushort.TryParse(headerListBox.SelectedItem.ToString().Substring(0, 3), out ushort headerNumber)) {
                headerListBox.SelectedIndex = -1;
                return;
            }

            /* Check if dynamic headers patch has been applied, and load header from arm9 or a/0/5/0 accordingly */
            if (ROMToolboxDialog.flag_DynamicHeadersPatchApplied || ROMToolboxDialog.CheckFilesDynamicHeadersPatchApplied()) {
                currentHeader = MapHeader.LoadFromFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + headerNumber.ToString("D4"), headerNumber, 0);
            } else {
                currentHeader = MapHeader.LoadFromARM9(headerNumber);
            }
            RefreshHeaderEditorFields();
        }

        private void RefreshHeaderEditorFields() {
            /* Setup controls for common fields across headers */
            if (currentHeader == null) {
                return;
            }

            internalNameBox.Text = internalNames[currentHeader.ID];
            matrixUpDown.Value = currentHeader.matrixID;
            areaDataUpDown.Value = currentHeader.areaDataID;
            scriptFileUpDown.Value = currentHeader.scriptFileID;
            levelScriptUpDown.Value = currentHeader.levelScriptID;
            eventFileUpDown.Value = currentHeader.eventFileID;
            textFileUpDown.Value = currentHeader.textArchiveID;
            wildPokeUpDown.Value = currentHeader.wildPokemon;
            weatherUpDown.Value = currentHeader.weatherID;
            cameraUpDown.Value = currentHeader.cameraAngleID;
            battleBackgroundUpDown.Value = currentHeader.battleBackground;

            if (RomInfo.gameFamily == gFamEnum.HGSS) {
                areaSettingsComboBox.SelectedIndex = ((HeaderHGSS)currentHeader).locationType;
            }

            openWildEditorWithIdButton.Enabled = currentHeader.wildPokemon != RomInfo.nullEncounterID;

            /* Setup controls for fields with version-specific differences */
            try {
                switch (RomInfo.gameFamily) {
                    case gFamEnum.DP: {
                            HeaderDP h = (HeaderDP)currentHeader;

                            locationNameComboBox.SelectedIndex = h.locationName;
                            musicDayUpDown.Value = h.musicDayID;
                            musicNightUpDown.Value = h.musicNightID;
                            areaSettingsComboBox.SelectedIndex = areaSettingsComboBox.FindString("[" + $"{currentHeader.locationSpecifier:D3}");
                            break;
                        }
                    case gFamEnum.Plat: {
                            HeaderPt h = (HeaderPt)currentHeader;

                            areaIconComboBox.SelectedIndex = h.areaIcon;
                            locationNameComboBox.SelectedIndex = h.locationName;
                            musicDayUpDown.Value = h.musicDayID;
                            musicNightUpDown.Value = h.musicNightID;
                            areaSettingsComboBox.SelectedIndex = areaSettingsComboBox.FindString("[" + $"{currentHeader.locationSpecifier:D3}");
                            break;
                        }
                    default: {
                            HeaderHGSS h = (HeaderHGSS)currentHeader;

                            areaIconComboBox.SelectedIndex = h.areaIcon;
                            locationNameComboBox.SelectedIndex = h.locationName;
                            musicDayUpDown.Value = h.musicDayID;
                            musicNightUpDown.Value = h.musicNightID;
                            worldmapXCoordUpDown.Value = h.worldmapX;
                            worldmapYCoordUpDown.Value = h.worldmapY;
                            followModeComboBox.SelectedIndex = h.followMode;
                            kantoRadioButton.Checked = h.kantoFlag;
                            johtoRadioButton.Checked = !h.kantoFlag;
                            break;
                        }
                }
            } catch (ArgumentOutOfRangeException) {
                MessageBox.Show("This header contains an irregular/unsupported field.", "Error loading header file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            RefreshFlags();
            updateWeatherPicAndComboBox();
            updateCameraPicAndComboBox();
        }
        private void RefreshFlags() {
            BitArray ba = new BitArray(new byte[] { currentHeader.flags });

            flag0CheckBox.Checked = ba[0];
            flag1CheckBox.Checked = ba[1];
            flag2CheckBox.Checked = ba[2];
            flag3CheckBox.Checked = ba[3];

            if (RomInfo.gameFamily == gFamEnum.HGSS) {
                flag4CheckBox.Checked = ba[4];
                flag5CheckBox.Checked = ba[5];
                flag6CheckBox.Checked = ba[6];
                //flag6CheckBox.Checked = ba[7];
            }
        }
        private void eventsTabControl_SelectedIndexChanged(object sender, EventArgs e) {

        }
        private void headerListBox_Leave(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }
            headerListBox.Refresh();
        }
        private void levelScriptUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }
            currentHeader.levelScriptID = (ushort)levelScriptUpDown.Value;
        }
        private void mapNameComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }

            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                    ((HeaderDP)currentHeader).locationName = (ushort)locationNameComboBox.SelectedIndex;
                    break;
                case gFamEnum.Plat:
                    ((HeaderPt)currentHeader).locationName = (byte)locationNameComboBox.SelectedIndex;
                    break;
                default:
                    ((HeaderHGSS)currentHeader).locationName = (byte)locationNameComboBox.SelectedIndex;
                    break;
            }
        }
        private void matrixUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }
            currentHeader.matrixID = (ushort)matrixUpDown.Value;
        }
        private void musicDayComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }
            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                    currentHeader.musicDayID = (ushort)(musicDayUpDown.Value = PokeDatabase.MusicDB.DPMusicDict.Keys.ElementAt(musicDayComboBox.SelectedIndex));
                    break;
                case gFamEnum.Plat:
                    currentHeader.musicDayID = (ushort)(musicDayUpDown.Value = PokeDatabase.MusicDB.PtMusicDict.Keys.ElementAt(musicDayComboBox.SelectedIndex));
                    break;
                default:
                    currentHeader.musicDayID = (ushort)(musicDayUpDown.Value = PokeDatabase.MusicDB.HGSSMusicDict.Keys.ElementAt(musicDayComboBox.SelectedIndex));
                    break;
            }
        }
        private void musicNightComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }

            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                    currentHeader.musicNightID = (ushort)(musicNightUpDown.Value = PokeDatabase.MusicDB.DPMusicDict.Keys.ElementAt(musicNightComboBox.SelectedIndex));
                    break;
                case gFamEnum.Plat:
                    currentHeader.musicNightID = (ushort)(musicNightUpDown.Value = PokeDatabase.MusicDB.PtMusicDict.Keys.ElementAt(musicNightComboBox.SelectedIndex));
                    break;
                default:
                    currentHeader.musicNightID = (ushort)(musicNightUpDown.Value = PokeDatabase.MusicDB.HGSSMusicDict.Keys.ElementAt(musicNightComboBox.SelectedIndex));
                    break;
            }
        }
        private void musicDayUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }

            Helpers.disableHandlers = true;
            ushort updValue = (ushort)((NumericUpDown)sender).Value;
            currentHeader.musicDayID = updValue;
            try {
                switch (RomInfo.gameFamily) {
                    case gFamEnum.DP:
                        musicDayComboBox.SelectedItem = PokeDatabase.MusicDB.DPMusicDict[updValue];
                        break;
                    case gFamEnum.Plat:
                        musicDayComboBox.SelectedItem = PokeDatabase.MusicDB.PtMusicDict[updValue];
                        break;
                    default:
                        musicDayComboBox.SelectedItem = PokeDatabase.MusicDB.HGSSMusicDict[updValue];
                        break;
                }
            } catch (KeyNotFoundException) {
                musicDayComboBox.SelectedItem = null;
            }
            Helpers.disableHandlers = false;
        }
        private void musicNightUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }

            Helpers.disableHandlers = true;
            ushort updValue = (ushort)((NumericUpDown)sender).Value;
            currentHeader.musicNightID = updValue;
            try {
                switch (RomInfo.gameFamily) {
                    case gFamEnum.DP:
                        musicNightComboBox.SelectedItem = PokeDatabase.MusicDB.DPMusicDict[updValue];
                        break;
                    case gFamEnum.Plat:
                        musicNightComboBox.SelectedItem = PokeDatabase.MusicDB.PtMusicDict[updValue];
                        break;
                    default:
                        musicNightComboBox.SelectedItem = PokeDatabase.MusicDB.HGSSMusicDict[updValue];
                        break;
                }
            } catch (KeyNotFoundException) {
                musicNightComboBox.SelectedItem = null;
            }
            Helpers.disableHandlers = false;
        }
        private void worldmapXCoordUpDown_ValueChanged(object sender, EventArgs e) {
            ((HeaderHGSS)currentHeader).worldmapX = (byte)worldmapXCoordUpDown.Value;
        }
        private void worldmapYCoordUpDown_ValueChanged(object sender, EventArgs e) {
            ((HeaderHGSS)currentHeader).worldmapY = (byte)worldmapYCoordUpDown.Value;
        }
        private void updateWeatherPicAndComboBox() {
            if (Helpers.disableHandlers) {
                return;
            }

            /* Update Weather Combobox*/
            Helpers.disableHandlers = true;
            try {
                switch (RomInfo.gameFamily) {
                    case gFamEnum.DP:
                        weatherComboBox.SelectedItem = PokeDatabase.Weather.DPWeatherDict[currentHeader.weatherID];
                        break;
                    case gFamEnum.Plat:
                        weatherComboBox.SelectedItem = PokeDatabase.Weather.PtWeatherDict[currentHeader.weatherID];
                        break;
                    default:
                        weatherComboBox.SelectedItem = PokeDatabase.Weather.HGSSWeatherDict[currentHeader.weatherID];
                        break;
                }
            } catch (KeyNotFoundException) {
                weatherComboBox.SelectedItem = null;
            }
            Helpers.disableHandlers = false;

            /* Update Weather Picture */
            try {
                Dictionary<byte[], string> dict;
                switch (RomInfo.gameFamily) {
                    case gFamEnum.DP:
                        dict = PokeDatabase.System.WeatherPics.dpWeatherImageDict;
                        break;
                    case gFamEnum.Plat:
                        dict = PokeDatabase.System.WeatherPics.ptWeatherImageDict;
                        break;
                    default:
                        dict = PokeDatabase.System.WeatherPics.hgssweatherImageDict;
                        break;
                }

                bool found = false;
                foreach (KeyValuePair<byte[], string> dictEntry in dict) {
                    if (Array.IndexOf(dictEntry.Key, (byte)weatherUpDown.Value) >= 0) {
                        weatherPictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(dictEntry.Value);
                        found = true;
                        break;
                    }
                }
                if (!found)
                    throw new KeyNotFoundException();
            } catch (KeyNotFoundException) {
                weatherPictureBox.Image = null;
            }
        }
        private void updateCameraPicAndComboBox() {
            if (Helpers.disableHandlers) {
                return;
            }

            /* Update Camera Combobox*/
            Helpers.disableHandlers = true;
            try {
                switch (RomInfo.gameFamily) {
                    case gFamEnum.DP:
                        cameraComboBox.SelectedItem = PokeDatabase.CameraAngles.DPPtCameraDict[currentHeader.cameraAngleID];
                        break;
                    case gFamEnum.Plat:
                        cameraComboBox.SelectedItem = PokeDatabase.CameraAngles.DPPtCameraDict[currentHeader.cameraAngleID];
                        break;
                    default:
                        cameraComboBox.SelectedItem = PokeDatabase.CameraAngles.HGSSCameraDict[currentHeader.cameraAngleID];
                        break;
                }
            } catch (KeyNotFoundException) {
                cameraComboBox.SelectedItem = null;
            }
            Helpers.disableHandlers = false;

            /* Update Camera Picture */
            string imageName;
            try {
                switch (RomInfo.gameFamily) {
                    case gFamEnum.DP:
                        currentHeader.cameraAngleID = (byte)cameraComboBox.SelectedIndex;
                        imageName = "dpcamera" + cameraUpDown.Value.ToString();
                        break;
                    case gFamEnum.Plat:
                        currentHeader.cameraAngleID = (byte)cameraComboBox.SelectedIndex;
                        imageName = "ptcamera" + cameraUpDown.Value.ToString();
                        break;
                    default:
                        currentHeader.cameraAngleID = (byte)cameraComboBox.SelectedIndex;
                        imageName = "hgsscamera" + cameraUpDown.Value.ToString();
                        break;
                }
                cameraPictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(imageName);
            } catch (NullReferenceException) {
                MessageBox.Show("The current header uses an unrecognized camera.\n", "Unknown camera settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void weatherComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers || weatherComboBox.SelectedIndex < 0) {
                return;
            }

            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                    weatherUpDown.Value = PokeDatabase.Weather.DPWeatherDict.Keys.ElementAt(weatherComboBox.SelectedIndex);
                    break;
                case gFamEnum.Plat:
                    weatherUpDown.Value = PokeDatabase.Weather.PtWeatherDict.Keys.ElementAt(weatherComboBox.SelectedIndex);
                    break;
                default:
                    weatherUpDown.Value = PokeDatabase.Weather.HGSSWeatherDict.Keys.ElementAt(weatherComboBox.SelectedIndex);
                    break;
            }
            currentHeader.weatherID = (byte)weatherUpDown.Value;
        }
        private void weatherUpDown_ValueChanged(object sender, EventArgs e) {
            currentHeader.weatherID = (byte)weatherUpDown.Value;
            updateWeatherPicAndComboBox();
        }
        private void cameraComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers || cameraComboBox.SelectedIndex < 0) {
                return;
            }

            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                    cameraUpDown.Value = PokeDatabase.CameraAngles.DPPtCameraDict.Keys.ElementAt(cameraComboBox.SelectedIndex);
                    break;
                case gFamEnum.Plat:
                    cameraUpDown.Value = PokeDatabase.CameraAngles.DPPtCameraDict.Keys.ElementAt(cameraComboBox.SelectedIndex);
                    break;
                default:
                    cameraUpDown.Value = PokeDatabase.CameraAngles.HGSSCameraDict.Keys.ElementAt(cameraComboBox.SelectedIndex);
                    break;
            }
            currentHeader.cameraAngleID = (byte)cameraUpDown.Value;
        }
        private void cameraUpDown_ValueChanged(object sender, EventArgs e) {
            currentHeader.cameraAngleID = (byte)cameraUpDown.Value;
            updateCameraPicAndComboBox();
        }
        private void openAreaDataButton_Click(object sender, EventArgs e) {
            if (!nsbtxEditorIsReady) {
                SetupNSBTXEditor();
                nsbtxEditorIsReady = true;
            }

            selectAreaDataListBox.SelectedIndex = (int)areaDataUpDown.Value;
            texturePacksListBox.SelectedIndex = (mapTilesetRadioButton.Checked ? (int)areaDataMapTilesetUpDown.Value : (int)areaDataBuildingTilesetUpDown.Value);
            mainTabControl.SelectedTab = nsbtxEditorTabPage;

            if (texturesListBox.Items.Count > 0)
                texturesListBox.SelectedIndex = 0;
            if (palettesListBox.Items.Count > 0)
                palettesListBox.SelectedIndex = 0;
        }
        private void openEventsButton_Click(object sender, EventArgs e) {
            if (!eventEditor.eventEditorIsReady) {
                eventEditor.SetupEventEditor();
                eventEditor.eventEditorIsReady = true;
            }

            if (matrixUpDown.Value != 0) {
                eventEditor.eventAreaDataUpDown.Value = areaDataUpDown.Value; // Use Area Data for textures if matrix is not 0
            }

            eventEditor.eventMatrixUpDown.Value = matrixUpDown.Value; // Open the right matrix in event editor
            eventEditor.selectEventComboBox.SelectedIndex = (int)eventFileUpDown.Value; // Select event file
            mainTabControl.SelectedTab = eventEditorTabPage;

            eventMatrixUpDown_ValueChanged(null, null);
        }
        private void openMatrixButton_Click(object sender, EventArgs e) {
            if (!matrixEditorIsReady) {
                SetupMatrixEditor();
                matrixEditorIsReady = true;
            }
            mainTabControl.SelectedTab = matrixEditorTabPage;
            int matrixNumber = (int)matrixUpDown.Value;
            selectMatrixComboBox.SelectedIndex = matrixNumber;

            if (currentMatrix.hasHeadersSection) {
                matrixTabControl.SelectedTab = headersTabPage;

                //Autoselect cell containing current header, if such cell exists [and if current matrix has headers sections]
                for (int i = 0; i < headersGridView.RowCount; i++) {
                    for (int j = 0; j < headersGridView.ColumnCount; j++) {
                        if (currentHeader.ID.ToString() == headersGridView.Rows[i].Cells[j].Value.ToString()) {
                            headersGridView.CurrentCell = headersGridView.Rows[i].Cells[j];
                            return;
                        }
                    }
                }
            }
        }
        private void openTextArchiveButton_Click(object sender, EventArgs e) {
            if (!textEditor.textEditorIsReady) {
                textEditor.SetupTextEditor();
                textEditor.textEditorIsReady = true;
            }
            textEditor.selectTextFileComboBox.SelectedIndex = (int)textFileUpDown.Value;
            mainTabControl.SelectedTab = textEditorTabPage;
        }
        private void saveHeaderButton_Click(object sender, EventArgs e) {
            /* Check if dynamic headers patch has been applied, and save header to arm9 or a/0/5/0 accordingly */
            if (ROMToolboxDialog.flag_DynamicHeadersPatchApplied || ROMToolboxDialog.CheckFilesDynamicHeadersPatchApplied()) {
                DSUtils.WriteToFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + currentHeader.ID.ToString("D4"), currentHeader.ToByteArray(), 0, 0, fmode: FileMode.Create);
            } else {
                uint headerOffset = (uint)(RomInfo.headerTableOffset + MapHeader.length * currentHeader.ID);
                DSUtils.ARM9.WriteBytes(currentHeader.ToByteArray(), headerOffset);
            }
            Helpers.disableHandlers = true;

            updateCurrentInternalName();
            updateHeaderNameShown(headerListBox.SelectedIndex);
            headerListBox.Focus();
            Helpers.disableHandlers = false;
        }
        private byte[] StringToInternalName(string text) {
            if (text.Length > internalNameLength) {
                MessageBox.Show("Internal names can't be longer than " + internalNameLength + " characters!", "Length error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return Encoding.ASCII.GetBytes(text.Substring(0, Math.Min(text.Length, internalNameLength)).PadRight(internalNameLength, '\0'));
        }
        private void updateCurrentInternalName() {
            /* Update internal name according to internalNameBox text*/
            ushort headerID = currentHeader.ID;

            using (DSUtils.EasyWriter writer = new DSUtils.EasyWriter(RomInfo.internalNamesLocation, headerID * RomInfo.internalNameLength)) { 
                writer.Write(StringToInternalName(internalNameBox.Text));
            }

            internalNames[headerID] = internalNameBox.Text;
            string elem = headerID.ToString("D3") + MapHeader.nameSeparator + internalNames[headerID];
            headerListBoxNames[headerID] = elem;

            if (eventEditor.eventEditorIsReady) {
                eventEditor.eventEditorWarpHeaderListBox.Items[headerID] = elem;
            }
        }
        private void updateHeaderNameShown(int thisIndex) {
            Helpers.disableHandlers = true;
            string val = (string)(headerListBox.Items[thisIndex] = headerListBoxNames[currentHeader.ID]);
            if (eventEditor.eventEditorIsReady) {
                eventEditor.eventEditorWarpHeaderListBox.Items[thisIndex] = val;
            }
            Helpers.disableHandlers = false;
        }
        private void resetButton_Click(object sender, EventArgs e) {
            resetHeaderSearch();
        }

        void resetHeaderSearch() {
            searchLocationTextBox.Clear();
            HeaderSearch.ResetResults(headerListBox, headerListBoxNames, prependNumbers: false);
            statusLabelMessage();
        }

        private void searchHeaderTextBox_KeyPress(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                startSearchGameLocation();
            }
        }
        private void searchHeaderButton_Click(object sender, EventArgs e) {
            startSearchGameLocation();
        }
        private void startSearchGameLocation() {
            if (searchLocationTextBox.Text.Length != 0) {
                headerListBox.Items.Clear();
                bool noResult = true;

                /* Check if dynamic headers patch has been applied, and load header from arm9 or a/0/5/0 accordingly */
                for (ushort i = 0; i < internalNames.Count; i++) {
                    MapHeader h;
                    if (ROMToolboxDialog.flag_DynamicHeadersPatchApplied || ROMToolboxDialog.CheckFilesDynamicHeadersPatchApplied()) {
                        h = MapHeader.LoadFromFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + i.ToString("D4"), i, 0);
                    } else {
                        h = MapHeader.LoadFromARM9(i);
                    }

                    string locationName = "";
                    switch (RomInfo.gameFamily) {
                        case gFamEnum.DP:
                            locationName = locationNameComboBox.Items[((HeaderDP)h).locationName].ToString();
                            break;
                        case gFamEnum.Plat:
                            locationName = locationNameComboBox.Items[((HeaderPt)h).locationName].ToString();
                            break;
                        case gFamEnum.HGSS:
                            locationName = locationNameComboBox.Items[((HeaderHGSS)h).locationName].ToString();
                            break;
                    }

                    if (locationName.IndexOf(searchLocationTextBox.Text, StringComparison.InvariantCultureIgnoreCase) >= 0) {
                        headerListBox.Items.Add(i.ToString("D3") + MapHeader.nameSeparator + internalNames[i]);
                        noResult = false;
                    }
                }


                if (noResult) {
                    headerListBox.Items.Add("No result for " + '"' + searchLocationTextBox.Text + '"');
                    headerListBox.Enabled = false;
                } else {
                    headerListBox.SelectedIndex = 0;
                    headerListBox.Enabled = true;
                }
            } else if (headerListBox.Items.Count < internalNames.Count) {
                HeaderSearch.ResetResults(headerListBox, headerListBoxNames, prependNumbers: false);
            }
        }
        private void scriptFileUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }
            currentHeader.scriptFileID = (ushort)scriptFileUpDown.Value;
        }
        private void areaSettingsComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers || areaSettingsComboBox.SelectedItem is null) {
                return;
            }

            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                case gFamEnum.Plat:
                    currentHeader.locationSpecifier = Byte.Parse(areaSettingsComboBox.SelectedItem.ToString().Substring(1, 3));
                    break;
                case gFamEnum.HGSS:
                    HeaderHGSS ch = (HeaderHGSS)currentHeader;
                    ch.locationType = (byte)areaSettingsComboBox.SelectedIndex;
                    //areaImageLabel.Text = "Area icon";
                    //areaIconComboBox.Enabled = true;
                    //areaIconPictureBox.Visible = true;
                    break;
            }
        }
        private void textFileUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }
            currentHeader.textArchiveID = (ushort)textFileUpDown.Value;
        }

        private void wildPokeUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }

            currentHeader.wildPokemon = (ushort)wildPokeUpDown.Value;
            if (wildPokeUpDown.Value == RomInfo.nullEncounterID) {
                wildPokeUpDown.ForeColor = Color.Red;
            } else {
                wildPokeUpDown.ForeColor = Color.Black;
            }

            if (currentHeader.wildPokemon == RomInfo.nullEncounterID)
                openWildEditorWithIdButton.Enabled = false;
            else
                openWildEditorWithIdButton.Enabled = true;
        }
        private void importHeaderFromFileButton_Click(object sender, EventArgs e) {
            OpenFileDialog of = new OpenFileDialog {
                Filter = MapHeader.DefaultFilter
            };
            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            MapHeader h = null;
            try {
                if (new FileInfo(of.FileName).Length > 48)
                    throw new FileFormatException();

                h = MapHeader.LoadFromFile(of.FileName, currentHeader.ID, 0);
                if (h == null)
                    throw new FileFormatException();

            } catch (FileFormatException) {
                MessageBox.Show("The file you tried to import is either malformed or not a Header file.\nNo changes have been made.",
                        "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            currentHeader = h;
            /* Check if dynamic headers patch has been applied, and save header to arm9 or a/0/5/0 accordingly */
            if (ROMToolboxDialog.flag_DynamicHeadersPatchApplied || ROMToolboxDialog.CheckFilesDynamicHeadersPatchApplied()) {
                DSUtils.WriteToFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + currentHeader.ID.ToString("D4"), currentHeader.ToByteArray(), 0, 0, fmode: FileMode.Create);
            } else {
                uint headerOffset = (uint)(RomInfo.headerTableOffset + MapHeader.length * currentHeader.ID);
                DSUtils.ARM9.WriteBytes(currentHeader.ToByteArray(), headerOffset);
            }

            try {
                using (DSUtils.EasyReader reader = new DSUtils.EasyReader(of.FileName, MapHeader.length + 8)) {
                    internalNameBox.Text = Encoding.UTF8.GetString(reader.ReadBytes(RomInfo.internalNameLength));
                }
                updateCurrentInternalName();
                updateHeaderNameShown(headerListBox.SelectedIndex);
            } catch (EndOfStreamException) { }

            RefreshHeaderEditorFields();
        }

        private void exportHeaderToFileButton_Click(object sender, EventArgs e) {
            SaveFileDialog sf = new SaveFileDialog {
                Filter = MapHeader.DefaultFilter,
                FileName = "Header " + currentHeader.ID + " - " + internalNames[currentHeader.ID] + " (" + locationNameComboBox.SelectedItem.ToString() + ")"
            };

            if (sf.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            using (DSUtils.EasyWriter writer = new DSUtils.EasyWriter(sf.FileName)) {
                writer.Write(currentHeader.ToByteArray()); //Write full header
                writer.Write((byte)0x00); //Padding
                writer.Write(Encoding.UTF8.GetBytes("INTNAME")); //Signature
                writer.Write(Encoding.UTF8.GetBytes(internalNames[currentHeader.ID])); //Save Internal name
            }
        }

        #region CopyPaste Buttons
        /*Copy Paste Functions*/
        #region Variables
        int locationNameCopy;
        string internalNameCopy;
        decimal encountersIDCopy;
        int shownameCopy;
        int areaIconCopy;

        int musicdayCopy;
        int musicnightCopy;
        int weatherCopy;
        int camAngleCopy;
        int areaSettingsCopy;

        decimal scriptsCopy;
        decimal levelScriptsCopy;
        decimal eventsCopy;
        decimal textsCopy;

        decimal matrixCopy;
        decimal areadataCopy;
        decimal worldmapXCoordCopy;
        decimal worldmapYCoordCopy;
        decimal battleBGCopy;

        byte flagsCopy;
        int followingPokeCopy;
        bool kantoFlagCopy;

        #endregion
        private void copyHeaderButton_Click(object sender, EventArgs e) {
            locationNameCopy = locationNameComboBox.SelectedIndex;
            internalNameCopy = internalNameBox.Text;
            shownameCopy = areaSettingsComboBox.SelectedIndex;
            areaIconCopy = areaIconComboBox.SelectedIndex;
            areaSettingsCopy = areaSettingsComboBox.SelectedIndex;
            encountersIDCopy = wildPokeUpDown.Value;

            musicdayCopy = musicDayComboBox.SelectedIndex;
            musicnightCopy = musicNightComboBox.SelectedIndex;
            weatherCopy = weatherComboBox.SelectedIndex;
            camAngleCopy = cameraComboBox.SelectedIndex;

            scriptsCopy = scriptFileUpDown.Value;
            levelScriptsCopy = levelScriptUpDown.Value;
            eventsCopy = eventFileUpDown.Value;
            textsCopy = textFileUpDown.Value;

            matrixCopy = matrixUpDown.Value;
            areadataCopy = areaDataUpDown.Value;
            worldmapXCoordCopy = worldmapXCoordUpDown.Value;
            worldmapYCoordCopy = worldmapYCoordUpDown.Value;

            battleBGCopy = battleBackgroundUpDown.Value;
            flagsCopy = currentHeader.flags;
            followingPokeCopy = followModeComboBox.SelectedIndex;
            kantoFlagCopy = kantoRadioButton.Checked;

            /*Enable paste buttons*/
            pasteHeaderButton.Enabled = true;

            pasteLocationNameButton.Enabled = true;
            pasteInternalNameButton.Enabled = true;
            pasteAreaSettingsButton.Enabled = true;
            pasteAreaIconButton.Enabled = true;
            pasteWildEncountersButton.Enabled = true;

            pasteMusicDayButton.Enabled = true;
            pasteMusicNightButton.Enabled = true;
            pasteWeatherButton.Enabled = true;
            pasteCameraAngleButton.Enabled = true;

            pasteScriptsButton.Enabled = true;
            pasteLevelScriptsButton.Enabled = true;
            pasteEventsButton.Enabled = true;
            pasteTextsButton.Enabled = true;

            pasteMatrixButton.Enabled = true;
            pasteAreaDataButton.Enabled = true;

            worldmapCoordsCopyButton.Enabled = true;

            pasteMapSettingsButton.Enabled = true;

            headerListBox.Focus();
        }
        private void copyInternalNameButton_Click(object sender, EventArgs e) {
            internalNameCopy = internalNameBox.Text;
            Clipboard.SetData(DataFormats.Text, internalNameCopy);
            pasteInternalNameButton.Enabled = true;
        }
        private void copyLocationNameButton_Click(object sender, EventArgs e) {
            locationNameCopy = locationNameComboBox.SelectedIndex;
            pasteLocationNameButton.Enabled = true;
        }
        private void copyAreaSettingsButton_Click(object sender, EventArgs e) {
            areaSettingsCopy = areaSettingsComboBox.SelectedIndex;
            pasteAreaSettingsButton.Enabled = true;
        }
        private void copyAreaIconButton_Click(object sender, EventArgs e) {
            areaIconCopy = areaIconComboBox.SelectedIndex;
            pasteAreaIconButton.Enabled = true;
        }
        private void copyWildEncountersButton_Click(object sender, EventArgs e) {
            encountersIDCopy = wildPokeUpDown.Value;
            Clipboard.SetData(DataFormats.Text, encountersIDCopy);
            pasteWildEncountersButton.Enabled = true;
        }
        private void copyMusicDayButton_Click(object sender, EventArgs e) {
            musicdayCopy = musicDayComboBox.SelectedIndex;
            pasteMusicDayButton.Enabled = true;
        }
        private void copyWeatherButton_Click(object sender, EventArgs e) {
            weatherCopy = weatherComboBox.SelectedIndex;
            pasteWeatherButton.Enabled = true;
        }
        private void copyMusicNightButton_Click(object sender, EventArgs e) {
            musicnightCopy = musicNightComboBox.SelectedIndex;
            pasteMusicNightButton.Enabled = true;
        }
        private void copyCameraAngleButton_Click(object sender, EventArgs e) {
            camAngleCopy = cameraComboBox.SelectedIndex;
            pasteCameraAngleButton.Enabled = true;
        }
        private void copyScriptsButton_Click(object sender, EventArgs e) {
            scriptsCopy = scriptFileUpDown.Value;
            Clipboard.SetData(DataFormats.Text, scriptsCopy);
            pasteScriptsButton.Enabled = true;
        }
        private void copyLevelScriptsButton_Click(object sender, EventArgs e) {
            levelScriptsCopy = levelScriptUpDown.Value;
            Clipboard.SetData(DataFormats.Text, levelScriptsCopy);
            pasteLevelScriptsButton.Enabled = true;
        }
        private void copyEventsButton_Click(object sender, EventArgs e) {
            eventsCopy = eventFileUpDown.Value;
            Clipboard.SetData(DataFormats.Text, eventsCopy);
            pasteEventsButton.Enabled = true;
        }
        private void copyTextsButton_Click(object sender, EventArgs e) {
            textsCopy = textFileUpDown.Value;
            Clipboard.SetData(DataFormats.Text, textsCopy);
            pasteTextsButton.Enabled = true;
        }
        private void copyMatrixButton_Click(object sender, EventArgs e) {
            matrixCopy = matrixUpDown.Value;
            Clipboard.SetData(DataFormats.Text, matrixCopy);
            pasteMatrixButton.Enabled = true;
        }
        private void copyAreaDataButton_Click(object sender, EventArgs e) {
            areadataCopy = areaDataUpDown.Value;
            Clipboard.SetData(DataFormats.Text, areadataCopy);
            pasteAreaDataButton.Enabled = true;
        }
        private void worldmapCoordsCopyButton_Click(object sender, EventArgs e) {
            worldmapXCoordCopy = worldmapXCoordUpDown.Value;
            worldmapYCoordCopy = worldmapYCoordUpDown.Value;
            worldmapCoordsPasteButton.Enabled = true;
        }
        private void copyMapSettingsButton_Click(object sender, EventArgs e) {
            flagsCopy = currentHeader.flags;
            battleBGCopy = currentHeader.battleBackground;
            followingPokeCopy = followModeComboBox.SelectedIndex;
            kantoFlagCopy = kantoRadioButton.Checked;
            pasteMapSettingsButton.Enabled = true;
        }

        /* Paste Buttons */
        private void pasteHeaderButton_Click(object sender, EventArgs e) {
            locationNameComboBox.SelectedIndex = locationNameCopy;
            internalNameBox.Text = internalNameCopy;
            wildPokeUpDown.Value = encountersIDCopy;

            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                case gFamEnum.Plat:
                    areaSettingsComboBox.SelectedIndex = shownameCopy;
                    break;
                case gFamEnum.HGSS:
                    areaSettingsComboBox.SelectedIndex = areaSettingsCopy;
                    break;
            }
            areaIconComboBox.SelectedIndex = areaIconCopy;

            musicDayComboBox.SelectedIndex = musicdayCopy;
            musicNightComboBox.SelectedIndex = musicnightCopy;
            weatherComboBox.SelectedIndex = weatherCopy;
            cameraComboBox.SelectedIndex = camAngleCopy;

            scriptFileUpDown.Value = scriptsCopy;
            levelScriptUpDown.Value = levelScriptsCopy;
            eventFileUpDown.Value = eventsCopy;
            textFileUpDown.Value = textsCopy;

            matrixUpDown.Value = matrixCopy;
            areaDataUpDown.Value = areadataCopy;

            currentHeader.flags = flagsCopy;
            worldmapXCoordUpDown.Value = worldmapXCoordCopy;
            worldmapYCoordUpDown.Value = worldmapYCoordCopy;
            battleBackgroundUpDown.Value = battleBGCopy;
            RefreshFlags();
        }
        private void pasteInternalNameButton_Click(object sender, EventArgs e) {
            internalNameBox.Text = internalNameCopy;
        }
        private void pasteLocationNameButton_Click(object sender, EventArgs e) {
            locationNameComboBox.SelectedIndex = locationNameCopy;
        }
        private void pasteAreaSettingsButton_Click(object sender, EventArgs e) {
            areaSettingsComboBox.SelectedIndex = shownameCopy;
        }
        private void pasteAreaIconButton_Click(object sender, EventArgs e) {
            if (areaIconComboBox.Enabled) { 
                areaIconComboBox.SelectedIndex = areaIconCopy;
            }
        }
        private void pasteWildEncountersButton_Click(object sender, EventArgs e) {
            wildPokeUpDown.Value = encountersIDCopy;
        }
        private void pasteMusicDayButton_Click(object sender, EventArgs e) {
            musicDayComboBox.SelectedIndex = musicdayCopy;
        }
        private void pasteScriptsButton_Click(object sender, EventArgs e) {
            scriptFileUpDown.Value = scriptsCopy;
        }
        private void pasteLevelScriptsButton_Click(object sender, EventArgs e) {
            levelScriptUpDown.Value = levelScriptsCopy;
        }
        private void pasteEventsButton_Click(object sender, EventArgs e) {
            eventFileUpDown.Value = eventsCopy;
        }
        private void pasteTextsButton_Click(object sender, EventArgs e) {
            textFileUpDown.Value = textsCopy;
        }
        private void pasteMatrixButton_Click(object sender, EventArgs e) {
            matrixUpDown.Value = matrixCopy;
        }
        private void pasteAreaDataButton_Click(object sender, EventArgs e) {
            areaDataUpDown.Value = areadataCopy;
        }
        private void pasteWeatherButton_Click(object sender, EventArgs e) {
            weatherComboBox.SelectedIndex = weatherCopy;
        }
        private void pasteMusicNightButton_Click(object sender, EventArgs e) {
            musicNightComboBox.SelectedIndex = musicnightCopy;
        }
        private void pasteCameraAngleButton_Click(object sender, EventArgs e) {
            cameraComboBox.SelectedIndex = camAngleCopy;
        }
        private void worldmapCoordsPasteButton_Click(object sender, EventArgs e) {
            worldmapXCoordUpDown.Value = worldmapXCoordCopy;
            worldmapYCoordUpDown.Value = worldmapYCoordCopy;
        }
        private void pasteMapSettingsButton_Click(object sender, EventArgs e) {
            currentHeader.flags = flagsCopy;
            battleBackgroundUpDown.Value = battleBGCopy;

            followModeComboBox.SelectedIndex = followingPokeCopy;
            kantoRadioButton.Checked = kantoFlagCopy;
            RefreshFlags();
        }
        #endregion

        #endregion

        #region Matrix Editor

        GameMatrix currentMatrix;

        #region Subroutines
        private void ClearMatrixTables() {
            headersGridView.Rows.Clear();
            headersGridView.Columns.Clear();
            heightsGridView.Rows.Clear();
            heightsGridView.Columns.Clear();
            mapFilesGridView.Rows.Clear();
            mapFilesGridView.Columns.Clear();
            matrixTabControl.TabPages.Remove(headersTabPage);
            matrixTabControl.TabPages.Remove(heightsTabPage);
        }
        private (Color background, Color foreground) FormatMapCell(uint cellValue) {
            foreach (KeyValuePair<List<uint>, (Color background, Color foreground)> entry in Helpers.romInfo.MapCellsColorDictionary) {
                if (entry.Key.Contains(cellValue))
                    return entry.Value;
            }
            return (Color.White, Color.Black);
        }
        private void GenerateMatrixTables() {
            /* Generate table columns */
            if (currentMatrix is null) {
                return;
            }

            for (int i = 0; i < currentMatrix.width; i++) {
                headersGridView.Columns.Add("Column" + i, i.ToString("D"));
                headersGridView.Columns[i].Width = 32; // Set column size
                headersGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                headersGridView.Columns[i].Frozen = false;

                heightsGridView.Columns.Add("Column" + i, i.ToString("D"));
                heightsGridView.Columns[i].Width = 21; // Set column size
                heightsGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                heightsGridView.Columns[i].Frozen = false;

                mapFilesGridView.Columns.Add("Column" + i, i.ToString("D"));
                mapFilesGridView.Columns[i].Width = 32; // Set column size
                mapFilesGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                mapFilesGridView.Columns[i].Frozen = false;
            }

            /* Generate table rows */
            for (int i = 0; i < currentMatrix.height; i++) {
                mapFilesGridView.Rows.Add();
                mapFilesGridView.Rows[i].HeaderCell.Value = i.ToString();

                headersGridView.Rows.Add();
                headersGridView.Rows[i].HeaderCell.Value = i.ToString();

                heightsGridView.Rows.Add();
                heightsGridView.Rows[i].HeaderCell.Value = i.ToString();
            }

            /* Fill tables */
            for (int i = 0; i < currentMatrix.height; i++) {
                for (int j = 0; j < currentMatrix.width; j++) {
                    headersGridView.Rows[i].Cells[j].Value = currentMatrix.headers[i, j];
                    heightsGridView.Rows[i].Cells[j].Value = currentMatrix.altitudes[i, j];
                    mapFilesGridView.Rows[i].Cells[j].Value = currentMatrix.maps[i, j];
                }
            }

            if (currentMatrix.hasHeadersSection) {
                matrixTabControl.TabPages.Add(headersTabPage);
            }

            if (currentMatrix.hasHeightsSection) {
                matrixTabControl.TabPages.Add(heightsTabPage);
            }
        }
        #endregion
        private void SetupMatrixEditor() {
            statusLabelMessage("Setting up Matrix Editor...");
            Update();

            DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.matrices });

            Helpers.disableHandlers = true;

            /* Add matrix entries to ComboBox */
            selectMatrixComboBox.Items.Clear();
            selectMatrixComboBox.Items.Add("Matrix 0 - Main");
            for (int i = 1; i < Helpers.romInfo.GetMatrixCount(); i++) {
                selectMatrixComboBox.Items.Add(new GameMatrix(i));
            }

            if (!ReadColorTable(Properties.Settings.Default.lastColorTablePath, silent: true)) {
                Helpers.romInfo.ResetMapCellsColorDictionary();
            }
            RomInfo.SetupSpawnSettings();

            Helpers.disableHandlers = false;
            selectMatrixComboBox.SelectedIndex = 0;
            statusLabelMessage();
        }
        private void addHeaderSectionButton_Click(object sender, EventArgs e) {
            if (!currentMatrix.hasHeadersSection) {
                currentMatrix.hasHeadersSection = true;
                matrixTabControl.TabPages.Add(headersTabPage);
            }
        }
        private void addHeightsButton_Click(object sender, EventArgs e) {
            if (!currentMatrix.hasHeightsSection) {
                currentMatrix.hasHeightsSection = true;
                matrixTabControl.TabPages.Add(heightsTabPage);
            }
        }
        private void addMatrixButton_Click(object sender, EventArgs e) {
            GameMatrix blankMatrix = new GameMatrix();

            /* Add new matrix file to matrix folder */
            blankMatrix.SaveToFile(RomInfo.gameDirs[DirNames.matrices].unpackedDir + "\\" + Helpers.romInfo.GetMatrixCount().ToString("D4"), false);

            /* Update ComboBox*/
            selectMatrixComboBox.Items.Add( selectMatrixComboBox.Items.Count.ToString() + blankMatrix );
            selectMatrixComboBox.SelectedIndex = selectMatrixComboBox.Items.Count - 1;

            if (eventEditor.eventEditorIsReady) {
                eventEditor.eventMatrixUpDown.Maximum++;
            }
        }
        private void exportMatrixButton_Click(object sender, EventArgs e) {
            currentMatrix.SaveToFileExplorePath("Matrix " + selectMatrixComboBox.SelectedIndex);
        }
        private void saveMatrixButton_Click(object sender, EventArgs e) {
            currentMatrix.SaveToFileDefaultDir(selectMatrixComboBox.SelectedIndex);
            GameMatrix saved = new GameMatrix(selectMatrixComboBox.SelectedIndex);
            selectMatrixComboBox.Items[selectMatrixComboBox.SelectedIndex] = saved.ToString();
            eventEditor.eventMatrix = saved;
        }
        private void headersGridView_SelectionChanged(object sender, EventArgs e) {
            DisplaySelection(headersGridView.SelectedCells);
        }

        private void heightsGridView_SelectionChanged(object sender, EventArgs e) {
            DisplaySelection(heightsGridView.SelectedCells);
        }

        private void mapFilesGridView_SelectionChanged(object sender, EventArgs e) {
            DisplaySelection(mapFilesGridView.SelectedCells);
        }
        private void DisplaySelection(DataGridViewSelectedCellCollection selectedCells) {
            if (selectedCells.Count > 0) {
                statusLabelMessage("Selection:   " + selectedCells[0].ColumnIndex + ", " + selectedCells[0].RowIndex);
            }
        }
        private void headersGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
            if (headerListBox.Items.Count < internalNames.Count) {
                HeaderSearch.ResetResults(headerListBox, headerListBoxNames, prependNumbers: false);
            }

            if (e.RowIndex >= 0 && e.ColumnIndex >= 0) {
                int headerNumber = Convert.ToInt32(headersGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                headerListBox.SelectedIndex = headerNumber;
                mainTabControl.SelectedTab = headerEditorTabPage;
            }
        }
        private void headersGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }
            if (e.RowIndex > -1 && e.ColumnIndex > -1) {
                /* If input is junk, use 0000 as placeholder value */
                ushort cellValue;
                try {
                    if (!ushort.TryParse(headersGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out cellValue)) {
                        throw new NullReferenceException();
                    }
                } catch (NullReferenceException) {
                    cellValue = 0;
                }
                /* Change value in matrix object */
                currentMatrix.headers[e.RowIndex, e.ColumnIndex] = cellValue;
            }
        }
        private void headersGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (e.Value is null) {
                return;
            }

            Helpers.disableHandlers = true;

            /* Format table cells corresponding to border maps or void */
            if (!ushort.TryParse(mapFilesGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out ushort colorValue)) {
                colorValue = GameMatrix.EMPTY;
            }

            (Color back, Color fore) = FormatMapCell(colorValue);
            e.CellStyle.BackColor = back;
            e.CellStyle.ForeColor = fore;

            /* If invalid input is entered, show 00 */
            if (!ushort.TryParse(headersGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out _)) {
                e.Value = 0;
            }

            Helpers.disableHandlers = false;

        }
        private void heightsGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
            if (Helpers.disableHandlers) { 
                return; 
            }
            if (e.RowIndex > -1 && e.ColumnIndex > -1) {
                /* If input is junk, use 00 as placeholder value */
                byte cellValue = 0;
                try {
                    cellValue = byte.Parse(heightsGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                } catch { }

                /* Change value in matrix object */
                currentMatrix.altitudes[e.RowIndex, e.ColumnIndex] = cellValue;
            }
        }
        private void widthUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }
            Helpers.disableHandlers = true;

            /* Add or remove rows in DataGridView control */
            int delta = (int)widthUpDown.Value - currentMatrix.width;
            for (int i = 0; i < Math.Abs(delta); i++) {
                if (delta < 0) {
                    headersGridView.Columns.RemoveAt(currentMatrix.width - 1 - i);
                    heightsGridView.Columns.RemoveAt(currentMatrix.width - 1 - i);
                    mapFilesGridView.Columns.RemoveAt(currentMatrix.width - 1 - i);
                } else {
                    /* Add columns */
                    int index = currentMatrix.width + i;
                    headersGridView.Columns.Add(" ", (index).ToString());
                    heightsGridView.Columns.Add(" ", (index).ToString());
                    mapFilesGridView.Columns.Add(" ", (index).ToString());

                    /* Adjust column width */
                    headersGridView.Columns[index].Width = 34;
                    heightsGridView.Columns[index].Width = 22;
                    mapFilesGridView.Columns[index].Width = 34;

                    /* Fill new rows */
                    for (int j = 0; j < currentMatrix.height; j++) {
                        headersGridView.Rows[j].Cells[index].Value = 0;
                        heightsGridView.Rows[j].Cells[index].Value = 0;
                        mapFilesGridView.Rows[j].Cells[index].Value = GameMatrix.EMPTY;
                    }
                }
            }

            /* Modify matrix object */
            currentMatrix.ResizeMatrix((int)heightUpDown.Value, (int)widthUpDown.Value);
            Helpers.disableHandlers = false;
        }
        private void heightUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }
            Helpers.disableHandlers = true;

            /* Add or remove rows in DataGridView control */
            int delta = (int)heightUpDown.Value - currentMatrix.height;
            for (int i = 0; i < Math.Abs(delta); i++) {
                if (delta < 0) { // Remove rows
                    headersGridView.Rows.RemoveAt(currentMatrix.height - 1 - i);
                    heightsGridView.Rows.RemoveAt(currentMatrix.height - 1 - i);
                    mapFilesGridView.Rows.RemoveAt(currentMatrix.height - 1 - i);
                } else {
                    /* Add row in DataGridView */
                    headersGridView.Rows.Add();
                    heightsGridView.Rows.Add();
                    mapFilesGridView.Rows.Add();

                    int index = currentMatrix.height + i;
                    headersGridView.Rows[index].HeaderCell.Value = (index).ToString();
                    heightsGridView.Rows[index].HeaderCell.Value = (index).ToString();
                    mapFilesGridView.Rows[index].HeaderCell.Value = (index).ToString();

                    /* Fill new rows */
                    for (int j = 0; j < currentMatrix.width; j++) {
                        headersGridView.Rows[index].Cells[j].Value = 0;
                        heightsGridView.Rows[index].Cells[j].Value = 0;
                        mapFilesGridView.Rows[index].Cells[j].Value = GameMatrix.EMPTY;
                    }
                }
            }

            /* Modify matrix object */
            currentMatrix.ResizeMatrix((int)heightUpDown.Value, (int)widthUpDown.Value);
            Helpers.disableHandlers = false;
        }
        private void heightsGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (e.Value is null) {
                return;
            }

            Helpers.disableHandlers = true;

            /* Format table cells corresponding to border maps or void */
            ushort colorValue = 0;
            try {
                colorValue = ushort.Parse(mapFilesGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
            } catch { }

            (Color back, Color fore) = FormatMapCell(colorValue);
            e.CellStyle.BackColor = back;
            e.CellStyle.ForeColor = fore;

            /* If invalid input is entered, show 00 */
            byte cellValue = 0;
            try {
                cellValue = byte.Parse(heightsGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
            } catch { }

            e.Value = cellValue;
            Helpers.disableHandlers = false;
        }
        private void importMatrixButton_Click(object sender, EventArgs e) {
            /* Prompt user to select .mtx file */
            if (selectMatrixComboBox.SelectedIndex == 0) {
                statusLabelMessage("Awaiting user response...");
                DialogResult d = MessageBox.Show("Replacing a matrix - especially Matrix 0 - with a new file is risky.\n" +
                    "Do not do it unless you are absolutely sure.\nProceed?", "Risky operation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (d == DialogResult.No) {
                    return;
                }
            }

            OpenFileDialog importMatrix = new OpenFileDialog {
                Filter = GameMatrix.DefaultFilter
            };
            if (importMatrix.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            /* Update matrix object in memory */
            currentMatrix = new GameMatrix(new FileStream(importMatrix.FileName, FileMode.Open));

            /* Refresh DataGridView tables */
            ClearMatrixTables();
            GenerateMatrixTables();

            /* Setup matrix editor controls */
            Helpers.disableHandlers = true;
            matrixNameTextBox.Text = currentMatrix.name;
            widthUpDown.Value = currentMatrix.width;
            heightUpDown.Value = currentMatrix.height;
            Helpers.disableHandlers = false;

            /* Display success message */
            MessageBox.Show("Matrix imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            statusLabelMessage();
        }
        private void mapFilesGridView_CellMouseDoubleClick(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0) {
                if (currentMatrix.maps[e.RowIndex, e.ColumnIndex] == GameMatrix.EMPTY) {
                    MessageBox.Show("You can't load an empty map.\nSelect a valid map and try again.\n\n" +
                        "If you only meant to change the value of this cell, wait some time between one mouse click and the other.\n" +
                        "Alternatively, highlight the cell and press F2 on your keyboard.",
                        "User attempted to load VOID", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!mapEditorIsReady) {
                    SetupMapEditor();
                    mapOpenGlControl.MouseWheel += new MouseEventHandler(mapOpenGlControl_MouseWheel);
                    mapEditorIsReady = true;
                }

                int mapCount = Helpers.romInfo.GetMapCount();
                if ( currentMatrix.maps[e.RowIndex, e.ColumnIndex] >= mapCount) {
                    MessageBox.Show("This matrix cell points to a map file that doesn't exist.",
                        "There " + ((mapCount > 1) ? "are only " + mapCount + " map files." : "is only 1 map file."), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                /* Determine area data */
                ushort headerID = 0;
                if (currentMatrix.hasHeadersSection) {
                    headerID = currentMatrix.headers[e.RowIndex, e.ColumnIndex];
                } else {
                    ushort[] result = HeaderSearch.AdvancedSearch(0, (ushort)internalNames.Count, internalNames, (int)MapHeader.SearchableFields.MatrixID, (int)HeaderSearch.NumOperators.Equal, selectMatrixComboBox.SelectedIndex.ToString())
                        .Select(x => ushort.Parse(x.Split()[0]))
                        .ToArray();

                    if (result.Length < 1) {
                        headerID = currentHeader.ID;
                        statusLabelMessage("This Matrix is not linked to any Header. DSPRE can't determine the most appropriate AreaData (and textures) to use.\nDisplaying Textures from the last selected Header (" + headerID + ")'s AreaData...");
                    } else {
                        if (result.Length > 1) {
                            if (result.Contains(currentHeader.ID)) {
                                headerID = currentHeader.ID;

                                statusLabelMessage("Multiple Headers are associated to this Matrix, including the last selected one [Header " + headerID + "]. Now using its textures.");
                            } else {
                                if (gameFamily.Equals(gFamEnum.DP)) {
                                    foreach (ushort r in result) {
                                        HeaderDP hdp;

                                        ////Dynamic headers patch unsupported in DP
                                        //if (ROMToolboxDialog.flag_DynamicHeadersPatchApplied || ROMToolboxDialog.CheckFilesDynamicHeadersPatchApplied()) {
                                        //    hdp = (HeaderDP)MapHeader.LoadFromFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + r.ToString("D4"), r, 0);
                                        //} else {
                                            hdp = (HeaderDP)MapHeader.LoadFromARM9(r);
                                        //}

                                        if (hdp.locationName != 0) {
                                            headerID = hdp.ID;
                                            break;
                                        }
                                    }
                                } else if (gameFamily.Equals(gFamEnum.Plat)) {
                                    foreach (ushort r in result) {
                                        HeaderPt hpt;
                                        if (ROMToolboxDialog.flag_DynamicHeadersPatchApplied || ROMToolboxDialog.CheckFilesDynamicHeadersPatchApplied()) {
                                            hpt = (HeaderPt)MapHeader.LoadFromFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + r.ToString("D4"), r, 0);
                                        } else {
                                            hpt = (HeaderPt)MapHeader.LoadFromARM9(r);
                                        }

                                        if (hpt.locationName != 0) {
                                            headerID = hpt.ID;
                                            break;
                                        }
                                    }
                                } else {
                                    foreach (ushort r in result) {
                                        HeaderHGSS hgss;
                                        if (ROMToolboxDialog.flag_DynamicHeadersPatchApplied || ROMToolboxDialog.CheckFilesDynamicHeadersPatchApplied()) {
                                            hgss = (HeaderHGSS)MapHeader.LoadFromFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + r.ToString("D4"), r, 0);
                                        } 
                                        else {
                                            hgss = (HeaderHGSS)MapHeader.LoadFromARM9(r);
                                        }

                                        if (hgss.locationName != 0) {
                                            headerID = hgss.ID;
                                            break;
                                        }
                                    }
                                }

                                statusLabelMessage("Multiple Headers are using this Matrix. Header " + headerID + "'s textures are currently being displayed.");
                            }
                        } else {
                            headerID = result[0];
                            statusLabelMessage("Loading Header " + headerID + "'s textures.");
                        }
                    }
                }
                Update();

                if (headerID > internalNames.Count) {
                    MessageBox.Show("This map is associated to a non-existent header.\nThis will lead to unpredictable behaviour and, possibily, problems, if you attempt to load it in game.",
                        "Invalid header", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    headerID = 0;
                }

                /* get texture file numbers from area data */
                MapHeader h;
                if (ROMToolboxDialog.flag_DynamicHeadersPatchApplied || ROMToolboxDialog.CheckFilesDynamicHeadersPatchApplied()) {
                    h = MapHeader.LoadFromFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + headerID.ToString("D4"), headerID, 0);
                } else {
                    h = MapHeader.LoadFromARM9(headerID);
                }

                /* Load Map File and switch to Map Editor tab */
                Helpers.disableHandlers = true;

                AreaData areaData = new AreaData(h.areaDataID);
                selectMapComboBox.SelectedIndex = currentMatrix.maps[e.RowIndex, e.ColumnIndex];
                mapTextureComboBox.SelectedIndex = areaData.mapTileset + 1;
                buildTextureComboBox.SelectedIndex = areaData.buildingsTileset + 1;
                mainTabControl.SelectedTab = mapEditorTabPage;

                if (areaData.areaType == AreaData.TYPE_INDOOR) {
                    interiorbldRadioButton.Checked = true;
                } else {
                    exteriorbldRadioButton.Checked = true;
                }

                Helpers.disableHandlers = false;
                selectMapComboBox_SelectedIndexChanged(null, null);
            }
        }
        private void mapFilesGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0) {
                /* If input is junk, use '\' (FF FF) as placeholder value */
                ushort cellValue = GameMatrix.EMPTY;
                try {
                    cellValue = ushort.Parse(mapFilesGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                } catch { }

                /* Change value in matrix object */
                currentMatrix.maps[e.RowIndex, e.ColumnIndex] = cellValue;
            }
        }
        private void mapFilesGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            Helpers.disableHandlers = true;

            /* Format table cells corresponding to border maps or void */
            ushort colorValue = GameMatrix.EMPTY;
            try {
                colorValue = ushort.Parse(mapFilesGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
            } catch { }

            (Color backColor, Color foreColor) cellColors = FormatMapCell(colorValue);
            e.CellStyle.BackColor = cellColors.backColor;
            e.CellStyle.ForeColor = cellColors.foreColor;

            if (colorValue == GameMatrix.EMPTY)
                e.Value = '-';

            Helpers.disableHandlers = false;
        }
        private void matrixNameTextBox_TextChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }
            currentMatrix.name = matrixNameTextBox.Text;
        }
        private void removeHeadersButton_Click(object sender, EventArgs e) {
            matrixTabControl.TabPages.Remove(headersTabPage);
            currentMatrix.hasHeadersSection = false;
        }
        private void removeHeightsButton_Click(object sender, EventArgs e) {
            matrixTabControl.TabPages.Remove(heightsTabPage);
            currentMatrix.hasHeightsSection = false;
        }
        private void removeMatrixButton_Click(object sender, EventArgs e) {
            if (selectMatrixComboBox.Items.Count > 1) {
                DialogResult d = MessageBox.Show("Are you sure you want to delete the last matrix?", "Confirm deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (d.Equals(DialogResult.Yes)) {
                    /* Delete matrix file */
                    int matrixToDelete = Helpers.romInfo.GetMatrixCount() - 1;

                    string matrixPath = RomInfo.gameDirs[DirNames.matrices].unpackedDir + "\\" + matrixToDelete.ToString("D4");
                    File.Delete(matrixPath);

                    /* Change selected index if the matrix to be deleted is currently selected */
                    if (selectMatrixComboBox.SelectedIndex == matrixToDelete) {
                        selectMatrixComboBox.SelectedIndex--;
                    }

                    if (eventEditor.eventEditorIsReady) {
                        eventEditor.eventMatrixUpDown.Maximum--;
                    }

                    /* Remove entry from ComboBox, and decrease matrix count */
                    selectMatrixComboBox.Items.RemoveAt(matrixToDelete);
                }
            } else {
                MessageBox.Show("At least one matrix must be kept.", "Can't delete Matrix", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void setSpawnPointButton_Click(object sender, EventArgs e) {
            DataGridViewCell selectedCell = null;
            switch (matrixTabControl.SelectedIndex) {
                case 0: //Maps
                    selectedCell = mapFilesGridView.SelectedCells[0];
                    selectedCell = headersGridView.Rows[selectedCell.RowIndex].Cells[selectedCell.ColumnIndex];
                    break;
                case 1: //Headers
                    selectedCell = headersGridView.SelectedCells[0];
                    break;
                case 2: //Altitudes
                    selectedCell = heightsGridView.SelectedCells[0];
                    selectedCell = headersGridView.Rows[selectedCell.RowIndex].Cells[selectedCell.ColumnIndex];
                    break;
            }

            ushort headerNumber = 0;
            HashSet<string> result = null;
            if (currentMatrix.hasHeadersSection) {
                headerNumber = Convert.ToUInt16(selectedCell.Value);
            } else {
                DialogResult d;
                d = MessageBox.Show("This Matrix doesn't have a Header Tab. " +
                    Environment.NewLine + "Do you want to check if any Header uses this Matrix and choose that one as your Spawn Header? " +
                    Environment.NewLine + "\nChoosing 'No' will pick the last selected Header.", "Couldn't find Header Tab", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (d == DialogResult.Yes) {
                    result = HeaderSearch.AdvancedSearch(0, (ushort)internalNames.Count, internalNames, (int)MapHeader.SearchableFields.MatrixID, (int)HeaderSearch.NumOperators.Equal, selectMatrixComboBox.SelectedIndex.ToString());
                    if (result.Count < 1) {
                        MessageBox.Show("The current Matrix isn't assigned to any Header.\nThe default choice has been set to the last selected Header.", "No result", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        headerNumber = currentHeader.ID;
                    } else if (result.Count == 1) {
                        headerNumber = ushort.Parse(result.First().Split()[0]);
                    } else {
                        MessageBox.Show("Multiple Headers are using this Matrix.\nPick one from the list or reset the filter results to choose a different Header.", "Multiple results", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                } else {
                    headerNumber = currentHeader.ID;
                }
            }

            int matrixX = selectedCell.ColumnIndex;
            int matrixY = selectedCell.RowIndex;

            using (SpawnEditor ed = new SpawnEditor(result, headerListBoxNames, headerNumber, matrixX, matrixY)) {
                ed.ShowDialog();
            }
        }
        private void selectMatrixComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }
            ClearMatrixTables();
            currentMatrix = new GameMatrix(selectMatrixComboBox.SelectedIndex);
            GenerateMatrixTables();

            /* Setup matrix editor controls */
            Helpers.disableHandlers = true;
            matrixNameTextBox.Text = currentMatrix.name;
            widthUpDown.Value = currentMatrix.width;
            heightUpDown.Value = currentMatrix.height;
            Helpers.disableHandlers = false;
        }
        private void importColorTableButton_Click(object sender, EventArgs e) {
            OpenFileDialog of = new OpenFileDialog {
                Filter = "DSPRE Color Table File (*.ctb)|*.ctb"
            };
            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            ReadColorTable(of.FileName, silent: false);
        }

        private bool ReadColorTable(string fileName, bool silent) {
            if (string.IsNullOrWhiteSpace(fileName)) {
                return false;
            }

            string[] fileTableContent = File.ReadAllLines(fileName);

            if (fileTableContent.Length > 0) {
                const string mapKeyword = "[Maplist]";
                const string colorKeyword = "[Color]";
                const string textColorKeyword = "[TextColor]";
                const string dashSeparator = "-";
                string problematicSegment = "incomplete line";

                Dictionary<List<uint>, (Color background, Color foreground)> colorsDict = new Dictionary<List<uint>, (Color background, Color foreground)>();
                List<string> linesWithErrors = new List<string>();

                for (int i = 0; i < fileTableContent.Length; i++) {
                    if (fileTableContent[i].Length > 0) {
                        string[] lineParts = fileTableContent[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        try {
                            int j = 0;
                            if (!lineParts[j].Equals(mapKeyword)) {
                                problematicSegment = nameof(mapKeyword);
                                throw new FormatException();
                            }
                            j++;

                            List<uint> mapList = new List<uint>();
                            while (!lineParts[j].Equals(dashSeparator)) {

                                if (lineParts[j].Equals("and")) {
                                    j++;
                                }
                                uint firstValue = uint.Parse(lineParts[j++]);
                                mapList.Add(firstValue);

                                if (lineParts[j].Equals("to")) {
                                    j++;
                                    uint finalValue = uint.Parse(lineParts[j++]);
                                    //Add all numbers ranging from maplist[0] to finalValue
                                    if (firstValue > finalValue)
                                        Swap(ref firstValue, ref finalValue);

                                    for (uint k = firstValue + 1; k <= finalValue; k++) {
                                        mapList.Add(k);
                                    }
                                }
                            }

                            if (!lineParts[j].Equals(dashSeparator)) {
                                problematicSegment = nameof(dashSeparator);
                                throw new FormatException();
                            }
                            j++;

                            if (!lineParts[j].Equals(colorKeyword)) {
                                problematicSegment = nameof(colorKeyword);
                                throw new FormatException();
                            }
                            j++;

                            int r = Int32.Parse(lineParts[j++]);
                            int g = Int32.Parse(lineParts[j++]);
                            int b = Int32.Parse(lineParts[j++]);

                            if (!lineParts[j].Equals(dashSeparator)) {
                                problematicSegment = nameof(dashSeparator);
                                throw new FormatException();
                            }
                            j++;

                            if (!lineParts[j].Equals(textColorKeyword)) {
                                problematicSegment = nameof(textColorKeyword);
                                throw new FormatException();
                            }
                            j++;

                            colorsDict.Add(mapList, (Color.FromArgb(r, g, b), Color.FromName(lineParts[j++])));
                        } catch {
                            if (!silent) {
                                linesWithErrors.Add(i + 1 + " (err. " + problematicSegment + ")\n");
                            }
                            continue;
                        }
                    }
                }
                colorsDict.Add(new List<uint> { GameMatrix.EMPTY }, (Color.Black, Color.White));

                string errorMsg = "";
                MessageBoxIcon iconType = MessageBoxIcon.Information;
                
                if (!silent) {
                    if (linesWithErrors.Count > 0) {
                        errorMsg = "\nHowever, the following lines couldn't be parsed correctly:\n";

                        foreach (string s in linesWithErrors) {
                            errorMsg += "- Line " + s;
                        }

                        iconType = MessageBoxIcon.Warning;
                    }
                }
                Helpers.romInfo.MapCellsColorDictionary = colorsDict;
                ClearMatrixTables();
                GenerateMatrixTables();

                Properties.Settings.Default.lastColorTablePath = fileName;

                if (!silent) { 
                    MessageBox.Show("Color file has been read." + errorMsg, "Operation completed", MessageBoxButtons.OK, iconType);
                }
                return true;
            } else {
                if (!silent) {
                    MessageBox.Show("No readable content was found in this file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
        }

        public void Swap(ref uint a, ref uint b) {
            uint temp = a;
            a = b;
            b = temp;
        }
        private void resetColorTableButton_Click(object sender, EventArgs e) {
            Helpers.romInfo.ResetMapCellsColorDictionary();
            ClearMatrixTables();
            GenerateMatrixTables();

            Properties.Settings.Default.lastColorTablePath = "";
        }

        /*
        private void ExportAllMovePermissionsInMatrix(object sender, EventArgs e) {
            CommonOpenFileDialog romFolder = new CommonOpenFileDialog();
            romFolder.IsFolderPicker = true;
            romFolder.Multiselect = false;

            if (romFolder.ShowDialog() != CommonFileDialogResult.Ok) {
                return;
            }

            for (int i = 0; i < currentMatrix.height; i++) {
                for (int j = 0; j < currentMatrix.width; j++) {
                    ushort val = currentMatrix.maps[i, j];
                    if (val < ushort.MaxValue) {
                        string path = romFolder.FileName + "\\" + currentMatrix.id + j.ToString("D2") + "_" + i.ToString("D2") + ".per";
                        File.WriteAllBytes(path, new MapFile(val).CollisionsToByteArray());
                    }
                }
            }
        }
        */
        #endregion

        #region Map Editor

        #region Variables & Constants 
        public const int mapEditorSquareSize = 19;

        /* Map Rotation vars */
        public bool lRot;
        public bool rRot;
        public bool uRot;
        public bool dRot;

        /* Screenshot Interpolation mode */
        public InterpolationMode intMode;

        /*  Camera settings */
        public bool mapTexturesOn = true;
        public bool bldTexturesOn = true;
        public static float ang = 0.0f;
        public static float dist = 12.8f;
        public static float elev = 50.0f;
        public float perspective = 45f;

        private byte bldDecimalPositions = 1;

        /* Renderers */
        public static NSBMDGlRenderer mapRenderer = new NSBMDGlRenderer();
        public static NSBMDGlRenderer buildingsRenderer = new NSBMDGlRenderer();

        /* Map file */
        MapFile currentMapFile;

        /* Permission painters */
        public Pen paintPen;
        public SolidBrush paintBrush;
        public SolidBrush textBrush;
        public byte paintByte;
        StringFormat sf;
        public Rectangle mainCell;
        public Rectangle smallCell;
        public Rectangle painterBox = new Rectangle(0, 0, 100, 100);
        public Font textFont;
        #endregion

        #region Subroutines
        private void FillBuildingsBox() {
            buildingsListBox.Items.Clear();

            uint id = 0;

            for (int i = 0; i < currentMapFile.buildings.Count; i++) {
                id = currentMapFile.buildings[i].modelID;
                string baseName = (i + 1).ToString("D2") + MapHeader.nameSeparator;
                try {
                    buildingsListBox.Items.Add(baseName + buildIndexComboBox.Items[(int)id]);
                } catch (ArgumentOutOfRangeException) {
                    DialogResult d = MessageBox.Show("Building #" + id + " couldn't be found in the Building List.\n" +
                        "Do you want to load Building 0 in its place?\n" +
                        "(Choosing \"Cancel\" will discard this building altogether.)", "Building not found", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error);
                    if (d == DialogResult.Yes) {
                        buildingsListBox.Items.Add(baseName + buildIndexComboBox.Items[0]);
                    } else if (d == DialogResult.No) {
                        buildingsListBox.Items.Add(baseName + "MISSING " + (int)id + '!');
                    } // else do nothing
                }
            }

        }




        #endregion
        private void SetupMapEditor() {
            /* Extract essential NARCs sub-archives*/
            toolStripProgressBar.Visible = true;
            toolStripProgressBar.Maximum = 9;
            toolStripProgressBar.Value = 0;
            statusLabelMessage("Attempting to unpack Map Editor NARCs... Please wait.");
            Update();

            DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.maps,
                DirNames.exteriorBuildingModels,
                DirNames.buildingConfigFiles,
                DirNames.buildingTextures,
                DirNames.mapTextures,
                DirNames.areaData,
            });

            if (RomInfo.gameFamily == gFamEnum.HGSS) {
                DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.interiorBuildingModels });
            }

            Helpers.disableHandlers = true;

            collisionPainterPictureBox.Image = new Bitmap(100, 100);
            typePainterPictureBox.Image = new Bitmap(100, 100);
            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                case gFamEnum.Plat:
                    mapPartsTabControl.TabPages.Remove(bgsTabPage);
                    break;
                default:
                    interiorbldRadioButton.Enabled = true;
                    exteriorbldRadioButton.Enabled = true;
                    break;
            };


            /* Add map names to box */
            selectMapComboBox.Items.Clear();
            int mapCount = Helpers.romInfo.GetMapCount();

            for (int i = 0; i < mapCount; i++) {
                using (DSUtils.EasyReader reader = new DSUtils.EasyReader(RomInfo.gameDirs[DirNames.maps].unpackedDir + "\\" + i.ToString("D4"))) {
                    switch (RomInfo.gameFamily) {
                        case gFamEnum.DP:
                        case gFamEnum.Plat:
                            reader.BaseStream.Position = 0x10 + reader.ReadUInt32() + reader.ReadUInt32();
                            break;
                        default:
                            reader.BaseStream.Position = 0x12;
                            short bgsSize = reader.ReadInt16();
                            long backupPos = reader.BaseStream.Position;

                            reader.BaseStream.Position = 0;
                            reader.BaseStream.Position = backupPos + bgsSize + reader.ReadUInt32() + reader.ReadUInt32();
                            break;
                    };

                    reader.BaseStream.Position += 0x14;
                    selectMapComboBox.Items.Add(i.ToString("D3") + MapHeader.nameSeparator + DSUtils.ReadNSBMDname(reader));
                }

            }
            toolStripProgressBar.Value++;

            /* Fill building models list */
            updateBuildingListComboBox(false);

            /*  Fill map textures list */
            mapTextureComboBox.Items.Clear();
            mapTextureComboBox.Items.Add("Untextured");
            for (int i = 0; i < Helpers.romInfo.GetMapTexturesCount(); i++) {
                mapTextureComboBox.Items.Add("Map Texture Pack [" + i.ToString("D2") + "]");
            }
            toolStripProgressBar.Value++;

            /*  Fill building textures list */
            buildTextureComboBox.Items.Clear();
            buildTextureComboBox.Items.Add("Untextured");
            for (int i = 0; i < Helpers.romInfo.GetBuildingTexturesCount(); i++) {
                buildTextureComboBox.Items.Add("Building Texture Pack [" + i.ToString("D2") + "]");
            }

            toolStripProgressBar.Value++;

            collisionPainterComboBox.Items.Clear();
            foreach (string s in PokeDatabase.System.MapCollisionPainters.Values) {
                collisionPainterComboBox.Items.Add(s);
            }

            collisionTypePainterComboBox.Items.Clear();
            foreach (string s in PokeDatabase.System.MapCollisionTypePainters.Values) {
                collisionTypePainterComboBox.Items.Add(s);
            }

            toolStripProgressBar.Value++;

            /* Set controls' initial values */
            selectCollisionPanel.BackColor = Color.MidnightBlue;
            collisionTypePainterComboBox.SelectedIndex = 0;
            collisionPainterComboBox.SelectedIndex = 1;

            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Visible = false;
            Helpers.disableHandlers = false;

            //Default selections
            selectMapComboBox.SelectedIndex = 0;
            exteriorbldRadioButton.Checked = true;
            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                case gFamEnum.Plat:
                    mapTextureComboBox.SelectedIndex = 7;
                    buildTextureComboBox.SelectedIndex = 1;
                    break;
                case gFamEnum.HGSS:
                    mapTextureComboBox.SelectedIndex = 3;
                    buildTextureComboBox.SelectedIndex = 1;
                    break;
                default:
                    mapTextureComboBox.SelectedIndex = 2;
                    buildTextureComboBox.SelectedIndex = 1;
                    break;
            };

            statusLabelMessage();
        }
        private void addMapFileButton_Click(object sender, EventArgs e) {
            /* Add new map file to map folder */
            new MapFile(0, RomInfo.gameFamily, discardMoveperms: true).SaveToFileDefaultDir(selectMapComboBox.Items.Count);

            /* Update ComboBox and select new file */
            selectMapComboBox.Items.Add(selectMapComboBox.Items.Count.ToString("D3") + MapHeader.nameSeparator + "newmap");
            selectMapComboBox.SelectedIndex = selectMapComboBox.Items.Count - 1;
        }
        private void replaceMapBinButton_Click(object sender, EventArgs e) {
            /* Prompt user to select .bin file */
            OpenFileDialog of = new OpenFileDialog {
                Filter = "Map BIN File (*.bin)|*.bin"
            };
            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            MapFile temp = new MapFile(of.FileName, RomInfo.gameFamily, false);

            if (temp.correctnessFlag) {
                UpdateMapBinAndRefresh(temp, "Map BIN imported successfully!");
                return;
            } else {
                if (RomInfo.gameFamily == gFamEnum.HGSS) {
                    //If HGSS didn't work try reading as Platinum Map
                    temp = new MapFile(of.FileName, gFamEnum.Plat, false); 
                } else {
                    //If Plat didn't work try reading as HGSS Map
                    temp = new MapFile(of.FileName, gFamEnum.HGSS, false);
                }

                if (temp.correctnessFlag) {
                    UpdateMapBinAndRefresh(temp, "Map BIN imported and adapted successfully!");
                    return;
                }
            }
            
            MessageBox.Show("The BIN file you imported is corrupted!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void UpdateMapBinAndRefresh(MapFile newerVersion, string message) {
            currentMapFile = newerVersion;

            /* Update map BIN file */
            currentMapFile.SaveToFileDefaultDir(selectMapComboBox.SelectedIndex, showSuccessMessage: false);

            /* Refresh controls */
            selectMapComboBox_SelectedIndexChanged(null, null);

            /* Display success message */
            MessageBox.Show(message, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buildTextureComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            int btIndex = buildTextureComboBox.SelectedIndex;
            
            if (Helpers.disableHandlers || btIndex < 0) {
                return;
            }

            if (btIndex == 0) {
                bldTexturesOn = false;
            } else {
                string texturePath = RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir + "\\" + (btIndex - 1).ToString("D4");
                byte[] textureFile = File.ReadAllBytes(texturePath);

                Stream str = new MemoryStream(textureFile);
                foreach (Building building in currentMapFile.buildings) {
                    str.Position = 0;
                    NSBMD file = building.NSBMDFile;

                    if (file != null) {
                        file.materials = NSBTXLoader.LoadNsbtx(str, out file.Textures, out file.Palettes);

                        try {
                            file.MatchTextures();
                            bldTexturesOn = true;
                        } catch {
                            string itemAtIndex = buildTextureComboBox.Items[btIndex].ToString();
                            if (!itemAtIndex.StartsWith("Error!")) {
                                Helpers.disableHandlers = true;
                                buildTextureComboBox.Items[btIndex] = itemAtIndex.Insert(0, "Error! - ");
                                Helpers.disableHandlers = false;
                            }
                            bldTexturesOn = false;
                        }
                    }
                }
                //buildTextureComboBox.Items[buildTextureComboBox.SelectedIndex] = "Error - Building Texture Pack too small [" + (buildTextureComboBox.SelectedIndex - 1).ToString("D2") + "]";
            }

            Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }
        private void mapTextureComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) { 
                return; 
            }

            if (mapTextureComboBox.SelectedIndex == 0)
                mapTexturesOn = false;
            else {
                mapTexturesOn = true;

                string texturePath = RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + (mapTextureComboBox.SelectedIndex - 1).ToString("D4");
                currentMapFile.mapModel.materials = NSBTXLoader.LoadNsbtx(new MemoryStream(File.ReadAllBytes(texturePath)), out currentMapFile.mapModel.Textures, out currentMapFile.mapModel.Palettes);
                try {
                    currentMapFile.mapModel.MatchTextures();
                } catch { }
            }
            Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }
        private void mapEditorTabPage_Enter(object sender, EventArgs e) {
            mapOpenGlControl.MakeCurrent();
            if (selectMapComboBox.SelectedIndex > -1)
                Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }
        private void mapOpenGlControl_MouseWheel(object sender, MouseEventArgs e) {
            if (mapPartsTabControl.SelectedTab == buildingsTabPage && bldPlaceWithMouseCheckbox.Checked) {
                return;
            }
            dist -= (float)e.Delta / 200;
           Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }
        private void mapOpenGlControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            byte multiplier = 2;
            if (e.Modifiers == Keys.Shift) {
                multiplier = 1;
            } else if (e.Modifiers == Keys.Control) {
                multiplier = 4;
            }

            switch (e.KeyCode) {
                case Keys.Right:
                    rRot = true;
                    lRot = false;
                    break;
                case Keys.Left:
                    rRot = false;
                    lRot = true;
                    break;
                case Keys.Up:
                    dRot = false;
                    uRot = true;
                    break;
                case Keys.Down:
                    dRot = true;
                    uRot = false;
                    break;
            }

            if (rRot ^ lRot) {
                if (rRot) {
                    ang += 1 * multiplier;
                } else if (lRot) {
                    ang -= 1 * multiplier;
                }
            }

            if (uRot ^ dRot) {
                if (uRot) {
                    elev -= 1 * multiplier;
                } else if (dRot) {
                    elev += 1 * multiplier;
                }
            }
            
            mapOpenGlControl.Invalidate();
           Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }
        private void mapOpenGlControl_KeyUp(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Right:
                    rRot = false;
                    break;
                case Keys.Left:
                    lRot = false;
                    break;
                case Keys.Up:
                    uRot = false;
                    break;
                case Keys.Down:
                    dRot = false;
                    break;
            }
        }
        private void mapOpenGlControl_Click(object sender, EventArgs e) {
            if (radio2D.Checked && bldPlaceWithMouseCheckbox.Checked) {
                PointF coordinates = mapRenderPanel.PointToClient(Cursor.Position);
                PointF mouseTilePos = new PointF(coordinates.X / mapEditorSquareSize, coordinates.Y / mapEditorSquareSize);

                if (buildingsListBox.SelectedIndex > -1) {
                    if (!bldPlaceLockXcheckbox.Checked)
                        xBuildUpDown.Value = (decimal)(Math.Round(mouseTilePos.X, bldDecimalPositions) - 16);
                    if (!bldPlaceLockZcheckbox.Checked)
                        zBuildUpDown.Value = (decimal)(Math.Round(mouseTilePos.Y, bldDecimalPositions) - 16);
                }
            }
        }
        private void bldRoundWhole_CheckedChanged(object sender, EventArgs e) {
            bldDecimalPositions = 0;
        }
        private void bldRoundDec_CheckedChanged(object sender, EventArgs e) {
            bldDecimalPositions = 1;
        }
        private void bldRoundCent_CheckedChanged(object sender, EventArgs e) {
            bldDecimalPositions = 2;
        }
        private void bldRoundMil_CheckedChanged(object sender, EventArgs e) {
            bldDecimalPositions = 3;
        }
        private void bldRoundDecmil_CheckedChanged(object sender, EventArgs e) {
            bldDecimalPositions = 4;
        }
        private void bldRoundCentMil_CheckedChanged(object sender, EventArgs e) {
            bldDecimalPositions = 5;
        }
        private void bldPlaceWithMouseCheckbox_CheckedChanged(object sender, EventArgs e) {
            bool status = bldPlaceWithMouseCheckbox.Checked && radio2D.Checked;
            bldPlaceLockXcheckbox.Enabled = status;
            bldPlaceLockZcheckbox.Enabled = status;
            bldRoundGroupbox.Enabled = status;
            lockXZgroupbox.Enabled = status;

            if (status) {
                SetCam2D();
            }
        }
        private void bldPlaceLockXcheckbox_CheckedChanged(object sender, EventArgs e) {
            ExclusiveCBInvert(bldPlaceLockZcheckbox);
        }

        private void bldPlaceLockZcheckbox_CheckedChanged(object sender, EventArgs e) {
            ExclusiveCBInvert(bldPlaceLockXcheckbox);
        }
        private void mapPartsTabControl_SelectedIndexChanged(object sender, EventArgs e) {
            if (mapPartsTabControl.SelectedTab == buildingsTabPage) {
                radio2D.Checked = false;

                Helpers.hideBuildings = false;
                radio3D.Enabled = true;
                radio2D.Enabled = true;
                wireframeCheckBox.Enabled = true;

                mapOpenGlControl.BringToFront();

               Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
            } else if (mapPartsTabControl.SelectedTab == permissionsTabPage) {
                radio2D.Checked = true;

                Helpers.hideBuildings = false;
                radio3D.Enabled = false;
                radio2D.Enabled = false;
                wireframeCheckBox.Enabled = false;

                SetCam2D();
               Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

                movPictureBox.BackgroundImage = Helpers.GrabMapScreenshot(movPictureBox.Width, movPictureBox.Height);
                movPictureBox.BringToFront();
            } else if (mapPartsTabControl.SelectedTab == modelTabPage) {
                radio2D.Checked = false;

                Helpers.hideBuildings = true;
                radio3D.Enabled = true;
                radio2D.Enabled = true;
                wireframeCheckBox.Enabled = true;

                mapOpenGlControl.BringToFront();

               Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
            } else { // Terrain and BGS
                radio2D.Checked = true;

                Helpers.hideBuildings = false;
                radio3D.Enabled = false;
                radio2D.Enabled = false;
                wireframeCheckBox.Enabled = false;

                mapOpenGlControl.BringToFront();

               Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
            }
        }
        private void radio2D_CheckedChanged(object sender, EventArgs e) {
            bool _2dmodeSelected = radio2D.Checked;

            if (_2dmodeSelected) {
                SetCam2D();
            } else {
                SetCam3D();
            }

            bldPlaceWithMouseCheckbox.Enabled = _2dmodeSelected;
            radio3D.Checked = !_2dmodeSelected;

            bldPlaceWithMouseCheckbox_CheckedChanged(null, null);
        }
        private void SetCam2D() {
            perspective = 4f;
            ang = 0f;
            dist = 115.2f;
            elev = 90f;

           Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }
        private void SetCam3D() {
            perspective = 45f;
            ang = 0f;
            dist = 12.8f;
            elev = 50.0f;

           Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }
        private void mapScreenshotButton_Click(object sender, EventArgs e) {
            MessageBox.Show("Choose where to save the map screenshot.", "Choose destination path", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SaveFileDialog imageSFD = new SaveFileDialog {
                Filter = "PNG File(*.png)|*.png",
            };
            if (imageSFD.ShowDialog() != DialogResult.OK) {
                return;
            }

           Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

            int newW = 512, newH = 512;
            Bitmap newImage = new Bitmap(newW, newH);
            using (var graphCtr = Graphics.FromImage(newImage)) {
                graphCtr.SmoothingMode = SmoothingMode.HighQuality;
                graphCtr.InterpolationMode = InterpolationMode.NearestNeighbor;
                graphCtr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphCtr.DrawImage(Helpers.GrabMapScreenshot(mapOpenGlControl.Width, mapOpenGlControl.Height), 0, 0, newW, newH);
            }
            newImage.Save(imageSFD.FileName);
            MessageBox.Show("Screenshot saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void removeLastMapFileButton_Click(object sender, EventArgs e) {
            DialogResult d = MessageBox.Show("Are you sure you want to delete the last Map BIN File?", "Confirm deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (d.Equals(DialogResult.Yes)) {
                /* Delete last map file */
                File.Delete(RomInfo.gameDirs[DirNames.maps].unpackedDir + "\\" + (selectMapComboBox.Items.Count - 1).ToString("D4"));

                /* Check if currently selected file is the last one, and in that case select the one before it */
                int lastIndex = selectMapComboBox.Items.Count - 1;
                if (selectMapComboBox.SelectedIndex == lastIndex)
                    selectMapComboBox.SelectedIndex--;

                /* Remove item from ComboBox */
                selectMapComboBox.Items.RemoveAt(lastIndex);
            }
        }
        private void saveMapButton_Click(object sender, EventArgs e) {
            currentMapFile.SaveToFileDefaultDir(selectMapComboBox.SelectedIndex);
        }
        private void exportCurrentMapBinButton_Click(object sender, EventArgs e) {
            currentMapFile.SaveToFileExplorePath(selectMapComboBox.SelectedItem.ToString());
        }
        private void selectMapComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }

            /* Load map data into MapFile class instance */
            currentMapFile = new MapFile(selectMapComboBox.SelectedIndex, RomInfo.gameFamily);

            /* Load map textures for renderer */
            if (mapTextureComboBox.SelectedIndex > 0) {
                Helpers.MW_LoadModelTextures(currentMapFile.mapModel, RomInfo.gameDirs[DirNames.mapTextures].unpackedDir, mapTextureComboBox.SelectedIndex - 1);
            }

            /* Load buildings nsbmd and textures for renderer into MapFile's building objects */
            for (int i = 0; i < currentMapFile.buildings.Count; i++) {
                currentMapFile.buildings[i].LoadModelData(Helpers.romInfo.GetBuildingModelsDirPath(interiorbldRadioButton.Checked)); // Load building nsbmd
                if (buildTextureComboBox.SelectedIndex > 0) {
                    Helpers.MW_LoadModelTextures(currentMapFile.buildings[i].NSBMDFile, RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir, buildTextureComboBox.SelectedIndex - 1); // Load building textures                
                }
            }

            /* Render the map */
           Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

            /* Draw permissions in the small selection boxes */
            DrawSmallCollision();
            DrawSmallTypeCollision();

            /* Draw selected permissions category */
            if (selectCollisionPanel.BackColor == Color.MidnightBlue) {
                DrawCollisionGrid();
            } else {
                DrawTypeGrid();
            }
            /* Set map screenshot as background picture in permissions editor PictureBox */
            movPictureBox.BackgroundImage = Helpers.GrabMapScreenshot(movPictureBox.Width, movPictureBox.Height);

            RestorePainter();

            /* Fill buildings ListBox, and if not empty select first item */
            FillBuildingsBox();
            if (buildingsListBox.Items.Count > 0) {
                buildingsListBox.SelectedIndex = 0;
            }

            modelSizeLBL.Text = currentMapFile.mapModelData.Length.ToString() + " B";
            terrainSizeLBL.Text = currentMapFile.bdhc.Length.ToString() + " B";

            if (RomInfo.gameFamily == gFamEnum.HGSS) {
                BGSSizeLBL.Text = currentMapFile.bgs.Length.ToString() + " B";
            }
        }
        private void wireframeCheckBox_CheckedChanged(object sender, EventArgs e) {
            if (wireframeCheckBox.Checked) {
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE);
            } else {
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
            }

           Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }

        #region Building Editor
        private void addBuildingButton_Click(object sender, EventArgs e) {
            AddBuildingToMap(new Building());
        }
        private void duplicateBuildingButton_Click(object sender, EventArgs e) {
            if (buildingsListBox.SelectedIndex > -1) {
                AddBuildingToMap(new Building(currentMapFile.buildings[buildingsListBox.SelectedIndex]));
            }
        }
        private void AddBuildingToMap(Building b) {
            currentMapFile.buildings.Add(b);

            /* Load new building's model and textures for the renderer */
            b.LoadModelData(Helpers.romInfo.GetBuildingModelsDirPath(interiorbldRadioButton.Checked));
            Helpers.MW_LoadModelTextures(b.NSBMDFile, RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir, buildTextureComboBox.SelectedIndex - 1);
            currentMapFile.buildings[currentMapFile.buildings.Count - 1] = b;

            /* Add new entry to buildings ListBox */
            buildingsListBox.Items.Add((buildingsListBox.Items.Count + 1).ToString("D2") + MapHeader.nameSeparator + buildIndexComboBox.Items[(int)b.modelID]);
            buildingsListBox.SelectedIndex = buildingsListBox.Items.Count - 1;

            /* Redraw scene with new building */
           Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }
        private void buildIndexComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers || buildingsListBox.SelectedIndex < 0) { 
                return;
            }

            Helpers.disableHandlers = true;
            buildingsListBox.Items[buildingsListBox.SelectedIndex] = (buildingsListBox.SelectedIndex + 1).ToString("D2") + MapHeader.nameSeparator + buildIndexComboBox.SelectedItem;
            Helpers.disableHandlers = false;

            currentMapFile.buildings[buildingsListBox.SelectedIndex].modelID = (uint)buildIndexComboBox.SelectedIndex;
            currentMapFile.buildings[buildingsListBox.SelectedIndex].LoadModelData(Helpers.romInfo.GetBuildingModelsDirPath(interiorbldRadioButton.Checked));
            Helpers.MW_LoadModelTextures(currentMapFile.buildings[buildingsListBox.SelectedIndex].NSBMDFile, RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir, buildTextureComboBox.SelectedIndex - 1);

           Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }
        private void buildingsListBox_SelectedIndexChanged(object sender, EventArgs e) {
            int buildingNumber = buildingsListBox.SelectedIndex;
            if (Helpers.disableHandlers || buildingNumber < 0) {
                return;
            }
            bool oldStatus = Helpers.disableHandlers;
            Helpers.disableHandlers = true;

            Building selected = currentMapFile.buildings[buildingNumber];
            if (selected.NSBMDFile != null) {
                buildIndexComboBox.SelectedIndex = (int)selected.modelID;

                xBuildUpDown.Value = selected.xPosition + (decimal)selected.xFraction / 65535;
                yBuildUpDown.Value = selected.yPosition + (decimal)selected.yFraction / 65535;
                zBuildUpDown.Value = selected.zPosition + (decimal)selected.zFraction / 65535;

                xRotBuildUpDown.Value = selected.xRotation;
                yRotBuildUpDown.Value = selected.yRotation;
                zRotBuildUpDown.Value = selected.zRotation;

                xRotDegBldUpDown.Value = (decimal)Building.U16ToDeg((ushort)xRotBuildUpDown.Value);
                yRotDegBldUpDown.Value = (decimal)Building.U16ToDeg((ushort)yRotBuildUpDown.Value);
                zRotDegBldUpDown.Value = (decimal)Building.U16ToDeg((ushort)zRotBuildUpDown.Value);

                buildingWidthUpDown.Value = selected.width;
                buildingHeightUpDown.Value = selected.height;
                buildingLengthUpDown.Value = selected.length;
            }

            Helpers.disableHandlers = oldStatus;
        }
        private void xRotBuildUpDown_ValueChanged(object sender, EventArgs e) {
            int selection = buildingsListBox.SelectedIndex;

            if (selection <= -1 || Helpers.disableHandlers) {
                return;
            }
            Helpers.disableHandlers = true;

            xRotDegBldUpDown.Value = (decimal)Building.U16ToDeg(currentMapFile.buildings[selection].xRotation = (ushort)((int)xRotBuildUpDown.Value & ushort.MaxValue));
           Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

            Helpers.disableHandlers = false;
        }

        private void yRotBuildUpDown_ValueChanged(object sender, EventArgs e) {
            int selection = buildingsListBox.SelectedIndex;

            if (selection <= -1 || Helpers.disableHandlers) {
                return;
            }
            Helpers.disableHandlers = true;

            yRotDegBldUpDown.Value = (decimal)Building.U16ToDeg(currentMapFile.buildings[selection].yRotation = (ushort)((int)yRotBuildUpDown.Value & ushort.MaxValue));
           Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

            Helpers.disableHandlers = false;
        }

        private void zRotBuildUpDown_ValueChanged(object sender, EventArgs e) {
            int selection = buildingsListBox.SelectedIndex;

            if (selection <= -1 || Helpers.disableHandlers) {
                return;
            }
            Helpers.disableHandlers = true;

            zRotDegBldUpDown.Value = (decimal)Building.U16ToDeg(currentMapFile.buildings[selection].zRotation = (ushort)((int)zRotBuildUpDown.Value & ushort.MaxValue));
           Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

            Helpers.disableHandlers = false;
        }

        private void xRotDegBldUpDown_ValueChanged(object sender, EventArgs e) {
            if (buildingsListBox.SelectedIndex <= -1 || Helpers.disableHandlers) {
                return;
            }
            Helpers.disableHandlers = true;

            currentMapFile.buildings[buildingsListBox.SelectedIndex].xRotation = (ushort)(xRotBuildUpDown.Value =
                Building.DegToU16((float)xRotDegBldUpDown.Value));
           Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

            Helpers.disableHandlers = false;
        }

        private void yRotDegBldUpDown_ValueChanged(object sender, EventArgs e) {
            if (buildingsListBox.SelectedIndex <= -1 || Helpers.disableHandlers) {
                return;
            }
            Helpers.disableHandlers = true;

            currentMapFile.buildings[buildingsListBox.SelectedIndex].yRotation = (ushort)(yRotBuildUpDown.Value =
                Building.DegToU16((float)yRotDegBldUpDown.Value));
           Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

            Helpers.disableHandlers = false;
        }

        private void zRotDegBldUpDown_ValueChanged(object sender, EventArgs e) {
            if (buildingsListBox.SelectedIndex <= -1 || Helpers.disableHandlers) {
                return;
            }
            Helpers.disableHandlers = true;

            currentMapFile.buildings[buildingsListBox.SelectedIndex].zRotation = (ushort)(zRotBuildUpDown.Value =
                Building.DegToU16((float)zRotDegBldUpDown.Value));
           Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

            Helpers.disableHandlers = false;
        }
        private void buildingHeightUpDown_ValueChanged(object sender, EventArgs e) {
            if (buildingsListBox.SelectedIndex > -1) {
                currentMapFile.buildings[buildingsListBox.SelectedIndex].height = (uint)buildingHeightUpDown.Value;
               Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
            }
        }
        private void buildingLengthUpDown_ValueChanged(object sender, EventArgs e) {
            if (buildingsListBox.SelectedIndex > -1) {
                currentMapFile.buildings[buildingsListBox.SelectedIndex].length = (uint)buildingLengthUpDown.Value;
               Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
            }
        }
        private void buildingWidthUpDown_ValueChanged(object sender, EventArgs e) {
            if (buildingsListBox.SelectedIndex > -1) {
                currentMapFile.buildings[buildingsListBox.SelectedIndex].width = (uint)buildingWidthUpDown.Value;
               Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
            }
        }
        private void exportBuildingsButton_Click(object sender, EventArgs e) {
            SaveFileDialog sf = new SaveFileDialog {
                Filter = MapFile.BuildingsFilter,
                FileName = selectMapComboBox.SelectedItem.ToString()
            };
            if (sf.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            File.WriteAllBytes(sf.FileName, currentMapFile.BuildingsToByteArray());

            MessageBox.Show("Buildings exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void importBuildingsButton_Click(object sender, EventArgs e) {
            OpenFileDialog ib = new OpenFileDialog {
                Filter = MapFile.BuildingsFilter
            };
            if (ib.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            currentMapFile.ImportBuildings(File.ReadAllBytes(ib.FileName));
            FillBuildingsBox();
            if (buildingsListBox.Items.Count > 0) buildingsListBox.SelectedIndex = 0;

            for (int i = 0; i < currentMapFile.buildings.Count; i++) {
                currentMapFile.buildings[i].LoadModelData(Helpers.romInfo.GetBuildingModelsDirPath(interiorbldRadioButton.Checked)); // Load building nsbmd
                Helpers.MW_LoadModelTextures(currentMapFile.buildings[i].NSBMDFile, RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir, buildTextureComboBox.SelectedIndex - 1); // Load building textures                
            }

           Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
            MessageBox.Show("Buildings imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void interiorRadioButton_CheckedChanged(object sender, EventArgs e) {
            Helpers.disableHandlers = true;
            int index = buildIndexComboBox.SelectedIndex;
            buildIndexComboBox.Items.Clear();

            /* Fill building models list */
            updateBuildingListComboBox(interiorbldRadioButton.Checked);
            FillBuildingsBox();

            try {
                buildIndexComboBox.SelectedIndex = index;
            } catch (ArgumentOutOfRangeException) {
                buildIndexComboBox.SelectedIndex = 0;
                currentMapFile.buildings[buildIndexComboBox.SelectedIndex].modelID = 0;
            }

            /* Load buildings nsbmd and textures for renderer into MapFile's building objects */
            for (int i = 0; i < currentMapFile.buildings.Count; i++) {
                currentMapFile.buildings[i].LoadModelData(Helpers.romInfo.GetBuildingModelsDirPath(interiorbldRadioButton.Checked)); // Load building nsbmd
                Helpers.MW_LoadModelTextures(currentMapFile.buildings[i].NSBMDFile, RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir, buildTextureComboBox.SelectedIndex - 1); // Load building textures                
            }

            /* Render the map */
           Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
            Helpers.disableHandlers = false;
        }
        private void removeBuildingButton_Click(object sender, EventArgs e) {
            int toRemoveListBoxID = buildingsListBox.SelectedIndex;
            if (toRemoveListBoxID > -1) {
                Helpers.disableHandlers = true;

                /* Remove building object from list and the corresponding entry in the ListBox */

                currentMapFile.buildings.RemoveAt(toRemoveListBoxID);
                buildingsListBox.Items.RemoveAt(toRemoveListBoxID);

                FillBuildingsBox(); // Update ListBox
               Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

                Helpers.disableHandlers = false;

                if (buildingsListBox.Items.Count > 0) {
                    if (toRemoveListBoxID > 0) {
                        buildingsListBox.SelectedIndex = toRemoveListBoxID - 1;
                    } else {
                        buildingsListBox.SelectedIndex = 0;
                    }
                }
            }
        }
        private void xBuildUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers || buildingsListBox.SelectedIndex < 0) {
                return;
            }

            var wholePart = Math.Truncate(xBuildUpDown.Value);
            var decPart = xBuildUpDown.Value - wholePart;

            if (decPart < 0) {
                decPart += 1;
                wholePart -= 1;
            }

            currentMapFile.buildings[buildingsListBox.SelectedIndex].xPosition = (short)wholePart;
            currentMapFile.buildings[buildingsListBox.SelectedIndex].xFraction = (ushort)(decPart * 65535);
           Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }
        private void zBuildUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers || buildingsListBox.SelectedIndex < 0)
                return;

            var wholePart = Math.Truncate(zBuildUpDown.Value);
            var decPart = zBuildUpDown.Value - wholePart;

            if (decPart < 0) {
                decPart += 1;
                wholePart -= 1;
            }

            currentMapFile.buildings[buildingsListBox.SelectedIndex].zPosition = (short)wholePart;
            currentMapFile.buildings[buildingsListBox.SelectedIndex].zFraction = (ushort)(decPart * 65535);
           Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }
        private void yBuildUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers || buildingsListBox.SelectedIndex < 0)
                return;

            var wholePart = Math.Truncate(yBuildUpDown.Value);
            var decPart = yBuildUpDown.Value - wholePart;

            if (decPart < 0) {
                decPart += 1;
                wholePart -= 1;
            }

            currentMapFile.buildings[buildingsListBox.SelectedIndex].yPosition = (short)wholePart;
            currentMapFile.buildings[buildingsListBox.SelectedIndex].yFraction = (ushort)(decPart * 65535);
           Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }
        #endregion

        #region Movement Permissions Editor

        #region Subroutines

        private void DrawCollisionGrid() {
            Bitmap mainBm = new Bitmap(608, 608);
            using (Graphics gMain = Graphics.FromImage(mainBm)) {
                for (int i = 0; i < 32; i++) {
                    for (int j = 0; j < 32; j++) {
                        PrepareCollisionPainterGraphics(currentMapFile.collisions[i, j]);

                        /* Draw collision on the main grid */
                        mainCell = new Rectangle(19 * j, 19 * i, 19, 19);
                        gMain.DrawRectangle(paintPen, mainCell);
                        gMain.FillRectangle(paintBrush, mainCell);
                    }
                }
            }
            movPictureBox.Image = mainBm;
            movPictureBox.Invalidate();
        }
        private void DrawSmallCollision() {
            Bitmap smallBm = new Bitmap(100, 100);
            using (Graphics gSmall = Graphics.FromImage(smallBm)) {
                for (int i = 0; i < 32; i++) {
                    for (int j = 0; j < 32; j++) {
                        PrepareCollisionPainterGraphics(currentMapFile.collisions[i, j]);

                        /* Draw collision on the small image */
                        smallCell = new Rectangle(3 * j, 3 * i, 3, 3);
                        gSmall.DrawRectangle(paintPen, smallCell);
                        gSmall.FillRectangle(paintBrush, smallCell);
                    }
                }
            }
            collisionPictureBox.Image = smallBm;
            collisionPictureBox.Invalidate();
        }
        private void DrawTypeGrid() {
            Bitmap mainBm = new Bitmap(608, 608);
            using (Graphics gMain = Graphics.FromImage(mainBm)) {
                for (int i = 0; i < 32; i++) {
                    for (int j = 0; j < 32; j++) {
                        PrepareTypePainterGraphics(currentMapFile.types[i, j]);

                        /* Draw cell with color */
                        mainCell = new Rectangle(19 * j, 19 * i, 19, 19);
                        gMain.DrawRectangle(paintPen, mainCell);
                        gMain.FillRectangle(paintBrush, mainCell);

                        /* Draw byte on cell */
                        StringFormat sf = new StringFormat {
                            LineAlignment = StringAlignment.Center,
                            Alignment = StringAlignment.Center
                        };
                        gMain.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                        gMain.DrawString(currentMapFile.types[i, j].ToString("X2"), textFont, textBrush, mainCell, sf);
                    }
                }
            }
            movPictureBox.Image = mainBm;
            movPictureBox.Invalidate();
        }
        private void DrawSmallTypeCollision() {
            Bitmap smallBm = new Bitmap(100, 100);
            using (Graphics gSmall = Graphics.FromImage(smallBm)) {
                for (int i = 0; i < 32; i++) {
                    for (int j = 0; j < 32; j++) {
                        PrepareTypePainterGraphics(paintByte = currentMapFile.types[i, j]);

                        /* Draw collision on the small image */
                        smallCell = new Rectangle(3 * j, 3 * i, 3, 3);
                        gSmall.DrawRectangle(paintPen, smallCell);
                        gSmall.FillRectangle(paintBrush, smallCell);
                    }
                }
            }
            typePictureBox.Image = smallBm;
            typePictureBox.Invalidate();
        }
        private void EditCell(int xPosition, int yPosition) {
            try {
                mainCell = new Rectangle(xPosition * mapEditorSquareSize, yPosition * mapEditorSquareSize, mapEditorSquareSize, mapEditorSquareSize);
                smallCell = new Rectangle(xPosition * 3, yPosition * 3, 3, 3);

                using (Graphics mainG = Graphics.FromImage(movPictureBox.Image)) {
                    /*  Draw new cell on main grid */
                    mainG.SetClip(mainCell);
                    mainG.Clear(Color.Transparent);
                    mainG.DrawRectangle(paintPen, mainCell);
                    mainG.FillRectangle(paintBrush, mainCell);
                    if (selectTypePanel.BackColor == Color.MidnightBlue) {
                        sf = new StringFormat {
                            LineAlignment = StringAlignment.Center,
                            Alignment = StringAlignment.Center
                        };
                        mainG.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                        mainG.DrawString(paintByte.ToString("X2"), textFont, textBrush, mainCell, sf);
                    }
                }

                if (selectCollisionPanel.BackColor == Color.MidnightBlue) {
                    using (Graphics smallG = Graphics.FromImage(collisionPictureBox.Image)) {
                        /* Draw new cell on small grid */
                        smallG.SetClip(smallCell);
                        smallG.Clear(Color.Transparent);
                        smallG.DrawRectangle(paintPen, smallCell);
                        smallG.FillRectangle(paintBrush, smallCell);
                    }
                    currentMapFile.collisions[yPosition, xPosition] = paintByte;
                    collisionPictureBox.Invalidate();
                } else {
                    using (Graphics smallG = Graphics.FromImage(typePictureBox.Image)) {
                        /* Draw new cell on small grid */
                        smallG.SetClip(smallCell);
                        smallG.Clear(Color.Transparent);
                        smallG.DrawRectangle(paintPen, smallCell);
                        smallG.FillRectangle(paintBrush, smallCell);
                    }
                    currentMapFile.types[yPosition, xPosition] = paintByte;
                    typePictureBox.Invalidate();
                }
                movPictureBox.Invalidate();
            } catch { return; }
        }
        private void FloodFillUtil(byte[,] screen, int x, int y, byte prevC, byte newC, int sizeX, int sizeY) {
            // Base cases 
            if (x < 0 || x >= sizeX || y < 0 || y >= sizeY) {
                return;
            }

            if (screen[y, x] != prevC) {
                return;
            }

            // Replace the color at (x, y) 
            screen[y, x] = newC;

            // Recur for north, east, south and west 
            FloodFillUtil(screen, x + 1, y, prevC, newC, sizeX, sizeY);
            FloodFillUtil(screen, x - 1, y, prevC, newC, sizeX, sizeY);
            FloodFillUtil(screen, x, y + 1, prevC, newC, sizeX, sizeY);
            FloodFillUtil(screen, x, y - 1, prevC, newC, sizeX, sizeY);
        }
        private void FloodFillCell(int x, int y) {
            byte toPaint = paintByte;
            if (selectCollisionPanel.BackColor == Color.MidnightBlue) {
                if (currentMapFile.collisions[y, x] != paintByte) {
                    FloodFillUtil(currentMapFile.collisions, x, y, currentMapFile.collisions[y, x], paintByte, 32, 32);
                    DrawCollisionGrid();
                    DrawSmallCollision();
                    PrepareCollisionPainterGraphics(paintByte);
                }
            } else {
                if (currentMapFile.types[y, x] != paintByte) {
                    FloodFillUtil(currentMapFile.types, x, y, currentMapFile.types[y, x], paintByte, 32, 32);
                    DrawTypeGrid();
                    DrawSmallTypeCollision();
                    PrepareTypePainterGraphics(paintByte);
                }
            }

            /* Draw permissions in the small selection boxes */


        }
        private void RestorePainter() {
            if (selectCollisionPanel.BackColor == Color.MidnightBlue) {
                collisionPainterComboBox_SelectedIndexChange(null, null);
            } else if (collisionTypePainterComboBox.Enabled) {
                typePainterComboBox_SelectedIndexChanged(null, null);
            } else {
                typePainterUpDown_ValueChanged(null, null);
            }
        }
        private void PrepareCollisionPainterGraphics(byte collisionValue) {
            switch (collisionValue) {
                case 0x0:
                    paintPen = new Pen(Color.FromArgb(128, Color.White));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.White));
                    break;
                case 0x80:
                    paintPen = new Pen(Color.FromArgb(128, Color.Red));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.Red));
                    break;
                default:
                    paintPen = new Pen(Color.FromArgb(128, Color.LimeGreen));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.LimeGreen));
                    break;
            }
        }
        private void PrepareTypePainterGraphics(byte typeValue) {
            switch (typeValue) {
                case 0x0:
                    paintPen = new Pen(Color.FromArgb(128, Color.White));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.White));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x2:
                    paintPen = new Pen(Color.FromArgb(128, Color.LimeGreen));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.LimeGreen));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x3:
                    paintPen = new Pen(Color.FromArgb(128, Color.Green));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.Green));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x8:
                case 0xC:
                    paintPen = new Pen(Color.FromArgb(128, Color.BurlyWood));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.BurlyWood));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0x10:
                    paintPen = new Pen(Color.FromArgb(128, Color.SkyBlue));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.SkyBlue));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x13:
                    paintPen = new Pen(Color.FromArgb(128, Color.SteelBlue));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.SteelBlue));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x15:
                    paintPen = new Pen(Color.FromArgb(128, Color.RoyalBlue));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.RoyalBlue));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x16:
                    paintPen = new Pen(Color.FromArgb(128, Color.LightSlateGray));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.LightSlateGray));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x20:
                    paintPen = new Pen(Color.FromArgb(128, Color.Cyan));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.Cyan));
                    textBrush = new SolidBrush(Color.Black);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x21:
                    paintPen = new Pen(Color.FromArgb(128, Color.PeachPuff));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.PeachPuff));
                    textBrush = new SolidBrush(Color.Black);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x30:
                case 0x31:
                case 0x32:
                case 0x33:
                    paintPen = new Pen(Color.FromArgb(128, Color.Red));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.Red));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x3C:
                case 0x3D:
                case 0x3E:
                    paintPen = new Pen(Color.FromArgb(0x7F654321));
                    paintBrush = new SolidBrush(Color.FromArgb(0x7F654321));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0x38:
                case 0x39:
                case 0x3A:
                case 0x3B:
                    paintPen = new Pen(Color.FromArgb(128, Color.Maroon));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.Maroon));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0x40:
                case 0x41:
                case 0x42:
                case 0x43:
                    paintPen = new Pen(Color.FromArgb(128, Color.Gold));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.Gold));
                    textBrush = new SolidBrush(Color.Black);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x4B:
                case 0x4C:
                    paintPen = new Pen(Color.FromArgb(128, Color.Sienna));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.Sienna));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x5E:
                    paintPen = new Pen(Color.FromArgb(128, Color.DarkOrchid));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.DarkOrchid));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0x5F:
                case 0x62:
                case 0x63:
                case 0x64:
                case 0x65:
                case 0x69:
                    paintPen = new Pen(Color.FromArgb(128, Color.DarkOrchid));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.DarkOrchid));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0x6C:
                case 0x6D:
                case 0x6E:
                case 0x6F:
                    paintPen = new Pen(Color.FromArgb(128, Color.DarkOrchid));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.DarkOrchid));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0xA1:
                case 0xA2:
                case 0xA3:
                    paintPen = new Pen(Color.FromArgb(128, Color.Honeydew));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.Honeydew));
                    textBrush = new SolidBrush(Color.Black);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0xA4:
                    paintPen = new Pen(Color.FromArgb(128, Color.Peru));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.Peru));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0xA6:
                    paintPen = new Pen(Color.FromArgb(128, Color.SeaGreen));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.SeaGreen));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                default:
                    paintPen = new Pen(Color.FromArgb(128, Color.White));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.White));
                    textBrush = new SolidBrush(Color.Black);
                    textFont = new Font("Arial", 8.65f);
                    break;
            }
        }
        #endregion

        private void clearCurrentButton_Click(object sender, EventArgs e) {
            PictureBox smallBox = selectCollisionPanel.BackColor == Color.MidnightBlue ? collisionPictureBox : typePictureBox;

            using (Graphics smallG = Graphics.FromImage(smallBox.Image)) {
                using (Graphics mainG = Graphics.FromImage(movPictureBox.Image)) {
                    smallG.Clear(Color.Transparent);
                    mainG.Clear(Color.Transparent);
                    PrepareCollisionPainterGraphics(0x0);

                    for (int i = 0; i < 32; i++) {
                        for (int j = 0; j < 32; j++) {
                            mainCell = new Rectangle(19 * j, 19 * i, 19, 19);
                            mainG.DrawRectangle(paintPen, mainCell);
                            mainG.FillRectangle(paintBrush, mainCell);
                        }
                    }
                }
            }

            if (selectCollisionPanel.BackColor == Color.MidnightBlue) {
                currentMapFile.collisions = new byte[32, 32]; // Set all collision bytes to clear (0x0)               
            } else {
                currentMapFile.types = new byte[32, 32]; // Set all type bytes to clear (0x0)
            }

            movPictureBox.Invalidate(); // Refresh main image
            smallBox.Invalidate();
            RestorePainter();
        }

        private void collisionPictureBox_Click(object sender, EventArgs e) {
            selectTypePanel.BackColor = Color.Transparent;
            typeGroupBox.Enabled = false;
            selectCollisionPanel.BackColor = Color.MidnightBlue;
            collisionGroupBox.Enabled = true;

            DrawCollisionGrid();
            RestorePainter();
        }
        private void exportMovButton_Click(object sender, EventArgs e) {
            SaveFileDialog em = new SaveFileDialog {
                Filter = MapFile.MovepermsFilter,
                FileName = selectMapComboBox.SelectedItem.ToString()
            };
            if (em.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            File.WriteAllBytes(em.FileName, currentMapFile.CollisionsToByteArray());

            MessageBox.Show("Permissions exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void importMovButton_Click(object sender, EventArgs e) {
            OpenFileDialog ip = new OpenFileDialog {
                Filter = MapFile.MovepermsFilter
            };
            if (ip.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            currentMapFile.ImportPermissions(File.ReadAllBytes(ip.FileName));

            DrawSmallCollision();
            DrawSmallTypeCollision();

            if (selectCollisionPanel.BackColor == Color.MidnightBlue) {
                DrawCollisionGrid();
            } else {
                DrawTypeGrid();
            }
            RestorePainter();

            MessageBox.Show("Permissions imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void movPictureBox_Click(object sender, EventArgs e) {
            MouseEventArgs mea = (MouseEventArgs)e;

            int xCoord = movPictureBox.PointToClient(MousePosition).X / mapEditorSquareSize;
            int yCoord = movPictureBox.PointToClient(MousePosition).Y / mapEditorSquareSize;

            if (mea.Button == MouseButtons.Middle) {
                FloodFillCell(xCoord, yCoord);
            } else if (mea.Button == MouseButtons.Left) {
                EditCell(xCoord, yCoord);
            } else {
                if (selectCollisionPanel.BackColor == Color.MidnightBlue) {
                    byte newValue = currentMapFile.collisions[yCoord, xCoord];
                    updateCollisions(newValue);
                } else {
                    byte newValue = currentMapFile.types[yCoord, xCoord];
                    typePainterUpDown.Value = newValue;
                    updateTypeCollisions(newValue);
                };
            }
        }
        private void movPictureBox_MouseMove(object sender, MouseEventArgs e) {
            if ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left) {
                EditCell(e.Location.X / mapEditorSquareSize, e.Location.Y / mapEditorSquareSize);
            }
        }
        private void collisionPainterComboBox_SelectedIndexChange(object sender, EventArgs e) {
            byte? collisionByte = StringToCollisionByte((string)collisionPainterComboBox.SelectedItem);

            if (collisionByte != null) {
                updateCollisions((byte)collisionByte);
            }
        }
        private void typePainterComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            byte? collisionByte = StringToCollisionByte((string)collisionTypePainterComboBox.SelectedItem);

            if (collisionByte != null) {
                updateTypeCollisions((byte)collisionByte);
            }
        }

        private byte? StringToCollisionByte(string selectedItem) {
            byte? result;
            try {
                result = Convert.ToByte(selectedItem.Substring(1, 2), 16);
            } catch (FormatException) {
                Console.WriteLine("Format incompatible");
                result = null;
            }
            return result;
        }
        private void typePainterUpDown_ValueChanged(object sender, EventArgs e) {
            updateTypeCollisions((byte)typePainterUpDown.Value);
        }
        private void updateCollisions(byte typeValue) {
            PrepareCollisionPainterGraphics(typeValue);
            paintByte = (byte)typeValue;

            sf = new StringFormat {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };

            using (Graphics g = Graphics.FromImage(collisionPainterPictureBox.Image)) {
                g.Clear(Color.FromArgb(255, paintBrush.Color));
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.DrawString(typeValue.ToString("X2"), new Font("Microsoft Sans Serif", 24), textBrush, painterBox, sf);
            }

            if (PokeDatabase.System.MapCollisionPainters.TryGetValue(typeValue, out string dictResult)) {
                collisionPainterComboBox.SelectedItem = dictResult;
            }
            collisionPainterPictureBox.Invalidate();
        }
        private void updateTypeCollisions(byte typeValue) {
            PrepareTypePainterGraphics(typeValue);
            paintByte = typeValue;

            sf = new StringFormat {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };

            using (Graphics g = Graphics.FromImage(typePainterPictureBox.Image)) {
                g.Clear(Color.FromArgb(255, paintBrush.Color));
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.DrawString(typeValue.ToString("X2"), new Font("Microsoft Sans Serif", 24), textBrush, painterBox, sf);
            }

            if (PokeDatabase.System.MapCollisionTypePainters.TryGetValue(typeValue, out string dictResult)) {
                collisionTypePainterComboBox.SelectedItem = dictResult;
            } else {
                valueTypeRadioButton.Checked = true;
                typePainterUpDown.Value = typeValue;
            }
            typePainterPictureBox.Invalidate();
        }
        private void typePictureBox_Click(object sender, EventArgs e) {
            selectCollisionPanel.BackColor = Color.Transparent;
            collisionGroupBox.Enabled = false;
            selectTypePanel.BackColor = Color.MidnightBlue;
            typeGroupBox.Enabled = true;

            DrawTypeGrid();
            RestorePainter();
        }
        private void typesRadioButton_CheckedChanged(object sender, EventArgs e) {
            if (knownTypesRadioButton.Checked) {
                typePainterUpDown.Enabled = false;
                collisionTypePainterComboBox.Enabled = true;
                typePainterComboBox_SelectedIndexChanged(null, null);
            }
        }
        private void valueTypeRadioButton_CheckedChanged(object sender, EventArgs e) {
            if (valueTypeRadioButton.Checked) {
                collisionTypePainterComboBox.Enabled = false;
                typePainterUpDown.Enabled = true;
                typePainterUpDown_ValueChanged(null, null);
            }
        }
        #endregion

        #region 3D Model Editor
        public const ushort MAPMODEL_CRITICALSIZE = 61000;
        private void importMapButton_Click(object sender, EventArgs e) {
            OpenFileDialog im = new OpenFileDialog {
                Filter = MapFile.NSBMDFilter
            };
            if (im.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            currentMapFile.LoadMapModel(DSUtils.ReadFromFile(im.FileName));

            if (mapTextureComboBox.SelectedIndex > 0) {
                Helpers.MW_LoadModelTextures(currentMapFile.mapModel, RomInfo.gameDirs[DirNames.mapTextures].unpackedDir, mapTextureComboBox.SelectedIndex - 1);
            }
           Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

            modelSizeLBL.Text = currentMapFile.mapModelData.Length.ToString() + " B";

            string message;
            string title;
            if (currentMapFile.mapModelData.Length > MAPMODEL_CRITICALSIZE) {
                message = "You imported a map model that exceeds " + MAPMODEL_CRITICALSIZE + " bytes." + Environment.NewLine
                    + "This may lead to unexpected behavior in game.";
                title = "Imported correctly, but...";
            } else {
                message = "Map model imported successfully!";
                title = "Success!";
            }
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void exportMapButton_Click(object sender, EventArgs e) {
            SaveFileDialog em = new SaveFileDialog {
                FileName = selectMapComboBox.SelectedItem.ToString()
            };

            byte[] modelToWrite;

            if (embedTexturesInMapModelCheckBox.Checked) { /* Textured NSBMD file */
                em.Filter = MapFile.TexturedNSBMDFilter;
                if (em.ShowDialog(this) != DialogResult.OK) {
                    return;
                }
                
                string texturePath = RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + (mapTextureComboBox.SelectedIndex - 1).ToString("D4");
                byte[] texturesToEmbed = File.ReadAllBytes(texturePath);
                modelToWrite = DSUtils.BuildNSBMDwithTextures(currentMapFile.mapModelData, texturesToEmbed);
            } else { /* Untextured NSBMD file */
                em.Filter = MapFile.UntexturedNSBMDFilter;
                if (em.ShowDialog(this) != DialogResult.OK) {
                    return;
                }

                modelToWrite = currentMapFile.mapModelData;
            }

            File.WriteAllBytes(em.FileName, modelToWrite);
            MessageBox.Show("Map model exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void daeExportButton_Click(object sender, EventArgs e) {
            DSUtils.ModelToDAE(
                modelName: selectMapComboBox.SelectedItem.ToString().TrimEnd('\0'),
                modelData: currentMapFile.mapModelData,
                textureData: mapTextureComboBox.SelectedIndex < 0 ? null : File.ReadAllBytes(RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + (mapTextureComboBox.SelectedIndex - 1).ToString("D4"))
            );
        }
        #endregion

        #region BDHC Editor
        private void bdhcImportButton_Click(object sender, EventArgs e) {
            OpenFileDialog it = new OpenFileDialog() {
                Filter = RomInfo.gameFamily == gFamEnum.DP ? MapFile.BDHCFilter : MapFile.BDHCamFilter
            };

            if (it.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            currentMapFile.ImportTerrain(File.ReadAllBytes(it.FileName));
            terrainSizeLBL.Text = currentMapFile.bdhc.Length.ToString() + " B";
            MessageBox.Show("Terrain settings imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void bdhcExportButton_Click(object sender, EventArgs e) {
            SaveFileDialog sf = new SaveFileDialog {
                FileName = selectMapComboBox.SelectedItem.ToString(),
                Filter = RomInfo.gameFamily == gFamEnum.DP ? MapFile.BDHCFilter : MapFile.BDHCamFilter
            };

            if (sf.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            File.WriteAllBytes(sf.FileName, currentMapFile.bdhc);

            terrainSizeLBL.Text = currentMapFile.bdhc.Length.ToString() + " B";
            MessageBox.Show("Terrain settings exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void soundPlatesImportButton_Click(object sender, EventArgs e) {
            OpenFileDialog it = new OpenFileDialog {
                Filter = MapFile.BGSFilter
            };

            if (it.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            currentMapFile.ImportSoundPlates(File.ReadAllBytes(it.FileName));
            BGSSizeLBL.Text = currentMapFile.bgs.Length.ToString() + " B";
            MessageBox.Show("BackGround Sound data imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void soundPlatesExportButton_Click(object sender, EventArgs e) {
            SaveFileDialog sf = new SaveFileDialog {
                Filter = MapFile.BGSFilter,
                FileName = selectMapComboBox.SelectedItem.ToString()
            };
            if (sf.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            File.WriteAllBytes(sf.FileName, currentMapFile.bgs);

            BGSSizeLBL.Text = currentMapFile.bgs.Length.ToString() + " B";
            MessageBox.Show("BackGround Sound data exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void soundPlatesBlankButton_Click(object sender, EventArgs e) {
            currentMapFile.bgs = MapFile.blankBGS;
            BGSSizeLBL.Text = currentMapFile.bgs.Length.ToString() + " B";
            MessageBox.Show("BackGround Sound data successfull blanked.\nRemember to save the current map file.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #endregion

        #region Event Editor

        #region Variables      
        public NSMBe4.NSBMD.NSBTX_File overworldFrames;


        /* Painters to draw the matrix grid */
        #endregion

        #region Subroutines
        private void itemsSelectorHelpBtn_Click(object sender, EventArgs e) {

        }

        private void centerEventViewOnSelectedEvent_Click(object sender, EventArgs e) {

        }
        private void eventPictureBox_MouseMove(object sender, MouseEventArgs e) {

        }









        #endregion

        private void addEventFileButton_Click(object sender, EventArgs e) {

        }
        private void eventEditorTabPage_Enter(object sender, EventArgs e) {
            eventEditor.eventOpenGlControl.MakeCurrent();
        }
        private void eventMatrixPictureBox_Click(object sender, EventArgs e) {

        }
        private void eventMatrixUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void eventShiftLeftButton_Click(object sender, EventArgs e) {

        }
        private void eventShiftUpButton_Click(object sender, EventArgs e) {

        }
        private void eventShiftRightButton_Click(object sender, EventArgs e) {

        }
        private void eventShiftDownButton_Click(object sender, EventArgs e) {

        }
        private void eventMatrixCoordsUpDown_ValueChanged(object sender, EventArgs e) {

        }

        private void exportEventFileButton_Click(object sender, EventArgs e) {
 
        }
        private void saveEventsButton_Click(object sender, EventArgs e) {

        }
        private void importEventFileButton_Click(object sender, EventArgs e) {

        }
        private void removeEventFileButton_Click(object sender, EventArgs e) {

        }
        private void selectEventComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }
        private void showEventsCheckBoxes_CheckedChanged(object sender, EventArgs e) {

        }
        private void eventAreaDataUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void eventPictureBox_Click(object sender, EventArgs e) { 

        }
        #region Spawnables Tab
        private void addSpawnableButton_Click(object sender, EventArgs e) {

        }
        private void removeSpawnableButton_Click(object sender, EventArgs e) {

        }
        private void duplicateSpawnableButton_Click(object sender, EventArgs e) {

        }
        private void spawnablesListBox_SelectedIndexChanged(object sender, EventArgs e) {

        }
        private void spawnableMatrixXUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void spawnableMatrixYUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void spawnableScriptUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void spawnableMapXUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void spawnableMapYUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void spawnableZUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void spawnableDirComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }
        private void spawnableTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }
        #endregion

        #region Overworlds Tab
        private void addOverworldButton_Click(object sender, EventArgs e) {

        }
        private void removeOverworldButton_Click(object sender, EventArgs e) {

        }
        private void duplicateOverworldsButton_Click(object sender, EventArgs e) {

        }
        private void OWTypeChanged(object sender, EventArgs e) {

        }
        private void owItemComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }
        private void overworldsListBox_SelectedIndexChanged(object sender, EventArgs e) {

        }
        private void owFlagNumericUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void owIDNumericUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void owMovementComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }
        private void owOrientationComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }
        private void owScriptNumericUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void owSightRangeUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void owSpriteComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }
        private void owPartnerTrainerCheckBox_CheckedChanged(object sender, EventArgs e) {

        }
        private void owTrainerComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }
        private void owXMapUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void owXRangeUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void owYRangeUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void owYMapUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void owZPositionUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void owXMatrixUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void owYMatrixUpDown_ValueChanged(object sender, EventArgs e) {

        }

        #endregion

        #region Warps Tab
        private void addWarpButton_Click(object sender, EventArgs e) {

        }
        private void removeWarpButton_Click(object sender, EventArgs e) {

        }
        private void duplicateWarpsButton_Click(object sender, EventArgs e) {

        }
        private void warpAnchorUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void eventEditorWarpHeaderListBox_SelectedIndexChanged(object sender, EventArgs e) {

        }




        private void warpsListBox_SelectedIndexChanged(object sender, EventArgs e) {

        }
        private void warpMatrixXUpDown_ValueChanged(object sender, EventArgs e) {
        }
        private void warpMatrixYUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void warpXMapUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void warpYMapUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void warpZUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void goToWarpDestination_Click(object sender, EventArgs e) {

        }
        #endregion

        #region Triggers Tab
        private void addTriggerButton_Click(object sender, EventArgs e) {

        }
        private void removeTriggerButton_Click(object sender, EventArgs e) {

        }
        private void duplicateTriggersButton_Click(object sender, EventArgs e) {

        }
        private void triggersListBox_SelectedIndexChanged(object sender, EventArgs e) {

        }
        private void triggerVariableWatchedUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void expectedVarValueTriggerUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void triggerScriptUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void triggerXMapUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void triggerYMapUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void triggerZUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void triggerXMatrixUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void triggerYMatrixUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void triggerWidthUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void triggerLengthUpDown_ValueChanged(object sender, EventArgs e) {

        }
        #endregion
        #endregion

        #region Script Editor
        #region Variables
        //private static Mutex tooltipMutex = new Mutex();
        //private ScriptTooltip customTooltip;

        private bool scriptsDirty = false;
        private bool functionsDirty = false;
        private bool actionsDirty = false;

        private string cmdKeyWords = "";
        private string secondaryKeyWords = "";
        private ScriptFile currentScriptFile;
        #endregion
        #region Helper Methods
        private ScintillaNET.Scintilla ScriptTextArea;
        private ScintillaNET.Scintilla FunctionTextArea;
        private ScintillaNET.Scintilla ActionTextArea;

        private SearchManager scriptSearchManager;
        private SearchManager functionSearchManager;
        private SearchManager actionSearchManager;

        private Scintilla currentScintillaEditor;
        private SearchManager currentSearchManager;
        private void ScriptEditorSetClean() {
            Helpers.disableHandlers = true;
            
            scriptsTabPage.Text = ScriptFile.containerTypes.Script.ToString() + "s";
            functionsTabPage.Text = ScriptFile.containerTypes.Function.ToString() + "s";
            actionsTabPage.Text = ScriptFile.containerTypes.Action.ToString() + "s";
            scriptsDirty = functionsDirty = actionsDirty = false;

            Helpers.disableHandlers = false;
        }
        private void scriptEditorTabControl_TabIndexChanged(object sender, EventArgs e) {
            if (scriptEditorTabControl.SelectedTab == scriptsTabPage) {
                currentSearchManager = scriptSearchManager;
                currentScintillaEditor = ScriptTextArea;
            } else if (scriptEditorTabControl.SelectedTab == functionsTabPage) {
                currentSearchManager = functionSearchManager;
                currentScintillaEditor = FunctionTextArea;
            } else { //Actions
                currentSearchManager = actionSearchManager;
                currentScintillaEditor = ActionTextArea;
            }
        }
        private void SetupScriptEditorTextAreas() {
            //PREPARE SCRIPT EDITOR KEYWORDS
            cmdKeyWords = String.Join(" ", ScriptCommandNamesDict.Values) + 
                " " + String.Join(" ", ScriptDatabase.movementsDictIDName.Values);
            cmdKeyWords += " " + cmdKeyWords.ToUpper() + " " + cmdKeyWords.ToLower();

            secondaryKeyWords = String.Join(" ", RomInfo.ScriptComparisonOperatorsDict.Values) +
                " " + String.Join(" ", ScriptDatabase.specialOverworlds.Values) +
                " " + String.Join(" ", ScriptDatabase.overworldDirections.Values) +
                " " + ScriptFile.containerTypes.Script.ToString() +
                " " + ScriptFile.containerTypes.Function.ToString() +
                " " + ScriptFile.containerTypes.Action.ToString() +
                " " + EventType.Overworld +
                " " + Overworld.MovementCodeKW;
            secondaryKeyWords += " " + secondaryKeyWords.ToUpper() + " " + secondaryKeyWords.ToLower();


            // CREATE CONTROLS
            ScriptTextArea = new ScintillaNET.Scintilla();
            scriptSearchManager = new SearchManager(this, ScriptTextArea, panelSearchScriptTextBox, PanelSearchScripts);
            scintillaScriptsPanel.Controls.Add(ScriptTextArea);

            FunctionTextArea = new ScintillaNET.Scintilla();
            functionSearchManager = new SearchManager(this, FunctionTextArea, panelSearchFunctionTextBox, PanelSearchFunctions);
            scintillaFunctionsPanel.Controls.Add(FunctionTextArea);

            ActionTextArea = new ScintillaNET.Scintilla();
            actionSearchManager = new SearchManager(this, ActionTextArea, panelSearchActionTextBox, PanelSearchActions);
            scintillaActionsPanel.Controls.Add(ActionTextArea);

            currentScintillaEditor = ScriptTextArea;
            currentSearchManager = scriptSearchManager;

            // BASIC CONFIG
            ScriptTextArea.TextChanged += (this.OnTextChangedScript);
            FunctionTextArea.TextChanged += (this.OnTextChangedFunction);
            ActionTextArea.TextChanged += (this.OnTextChangedAction);

            // INITIAL VIEW CONFIG
            InitialViewConfig(ScriptTextArea);
            InitialViewConfig(FunctionTextArea);
            InitialViewConfig(ActionTextArea);

            InitSyntaxColoring(ScriptTextArea);
            InitSyntaxColoring(FunctionTextArea);
            InitSyntaxColoring(ActionTextArea);

            // NUMBER MARGIN
            InitNumberMargin(ScriptTextArea, ScriptTextArea_MarginClick);
            InitNumberMargin(FunctionTextArea, FunctionTextArea_MarginClick);
            InitNumberMargin(ActionTextArea, ActionTextArea_MarginClick);

            // BOOKMARK MARGIN
            InitBookmarkMargin(ScriptTextArea);
            InitBookmarkMargin(FunctionTextArea);
            InitBookmarkMargin(ActionTextArea);

            // CODE FOLDING MARGIN
            InitCodeFolding(ScriptTextArea);
            InitCodeFolding(FunctionTextArea);
            InitCodeFolding(ActionTextArea);

            // INIT HOTKEYS
            InitHotkeys(ScriptTextArea, scriptSearchManager);
            InitHotkeys(FunctionTextArea, functionSearchManager);
            InitHotkeys(ActionTextArea, actionSearchManager);

            // INIT TOOLTIPS DWELLING
            /*
            ScriptTextArea.MouseDwellTime = 300;
            ScriptTextArea.DwellEnd += TextArea_DwellEnd;
            ScriptTextArea.DwellStart += TextArea_DwellStart;

            FunctionTextArea.MouseDwellTime = 300;
            FunctionTextArea.DwellEnd += TextArea_DwellEnd;
            FunctionTextArea.DwellStart += TextArea_DwellStart;
            */
        }

        /*
        private void TextArea_DwellStart(object sender, DwellEventArgs e) {
            TextArea_DwellEnd(sender, e);
            Scintilla ctr = sender as Scintilla;
            string hoveredWord = ctr.GetWordFromPosition(e.Position);
            ushort cmdID;

            string commandName = "";
            if (RomInfo.ScriptCommandNamesReverseDict.TryGetValue(hoveredWord, out cmdID)) {
                commandName = hoveredWord;
            } else {
                if (!ushort.TryParse(hoveredWord, NumberStyles.HexNumber, new CultureInfo("en-US"), out cmdID)) {
                    return;
                }
            }
            string tip = "";

            tooltipMutex.WaitOne();
            tip += cmdID.ToString("X4") + ": " + commandName + "(";
            byte[] parameters = ScriptCommandParametersDict[cmdID];
            for (int i = 0; i < parameters.Length; i++) {
                if (parameters[i] == 0) {
                    break;
                } else if (parameters[i] == 1) {
                    tip += "byte";
                } else {
                    tip += "uint" + 8 * parameters[i];
                }
                if (i != parameters.Length - 1) {
                    tip += ", ";
                }
            }
            tip += ")";
            tip += Environment.NewLine + "Command descriptions aren't available yet.";

            Point globalCtrCoords = ctr.PointToScreen(ctr.Location);
            Point incrementedCoords = new Point(globalCtrCoords.X + e.X, globalCtrCoords.Y + e.Y);

            customTooltip = new ScriptTooltip(cmdKeyWords, tip);
            customTooltip.Visible = false;
            customTooltip.Show();

            int newy = incrementedCoords.Y - customTooltip.Size.Height - 5;
            customTooltip.Location = new Point(incrementedCoords.X, newy);
            customTooltip.BringToFront();
            customTooltip.Visible = true;
            Thread t = new Thread(() => {
                customTooltip.Invoke((MethodInvoker)delegate {
                    customTooltip.ctrl.Visible = true;
                    customTooltip.FadeIn(16, 9);
                    customTooltip.WriteText(4);
                });
            });
            t.Start();
            tooltipMutex.ReleaseMutex();
        }
        private void TextArea_DwellEnd(object sender, DwellEventArgs e) {
            if (customTooltip != null && !customTooltip.IsDisposed) {
                tooltipMutex.WaitOne();
                Thread t = new Thread(() => {
                    customTooltip.Invoke((MethodInvoker)delegate {
                        customTooltip.FadeOut(16, 9);
                        customTooltip.Close();
                        customTooltip.Dispose();
                    });

                });
                t.Start();
                tooltipMutex.ReleaseMutex();
            }
        }
        */

        private void InitNumberMargin(Scintilla textArea, EventHandler<MarginClickEventArgs> textArea_MarginClick) {
            textArea.Styles[Style.LineNumber].BackColor = BACK_COLOR;
            textArea.Styles[Style.LineNumber].ForeColor = FORE_COLOR;
            textArea.Styles[Style.IndentGuide].ForeColor = FORE_COLOR;
            textArea.Styles[Style.IndentGuide].BackColor = BACK_COLOR;

            var nums = textArea.Margins[NUMBER_MARGIN];
            nums.Type = MarginType.Number;
            nums.Sensitive = true;
            nums.Mask = 0;

            textArea.MarginClick += textArea_MarginClick;
        }

        private void InitHotkeys(Scintilla scintillaTb, SearchManager sm) {
            // register the hotkeys with the form
            HotKeyManager.AddHotKey(scintillaTb, sm.OpenSearch, Keys.F, true);
            HotKeyManager.AddHotKey(scintillaTb, () => Uppercase(scintillaTb), Keys.U, true);
            HotKeyManager.AddHotKey(scintillaTb, () => Lowercase(scintillaTb), Keys.L, true);
            HotKeyManager.AddHotKey(scintillaTb, () => ZoomIn(scintillaTb), Keys.Oemplus, true);
            HotKeyManager.AddHotKey(scintillaTb, () => ZoomOut(scintillaTb), Keys.OemMinus, true);
            HotKeyManager.AddHotKey(scintillaTb, () => ZoomDefault(scintillaTb), Keys.D0, true);
            HotKeyManager.AddHotKey(scintillaTb, sm.CloseSearch, Keys.Escape);

            // remove conflicting hotkeys from scintilla
            scintillaTb.ClearCmdKey(Keys.Control | Keys.F);
            scintillaTb.ClearCmdKey(Keys.Control | Keys.R);
            scintillaTb.ClearCmdKey(Keys.Control | Keys.H);
            scintillaTb.ClearCmdKey(Keys.Control | Keys.L);
            scintillaTb.ClearCmdKey(Keys.Control | Keys.U);
        }

        private void InitSyntaxColoring(Scintilla textArea) {

            // Configure the default style
            textArea.StyleResetDefault();
            textArea.Styles[Style.Default].Font = "Consolas";
            textArea.Styles[Style.Default].Size = 12;
            textArea.Styles[Style.Default].BackColor = Color.FromArgb(0x212121);
            textArea.Styles[Style.Default].ForeColor = Color.FromArgb(0xFFFFFF);
            textArea.StyleClearAll();

            // Configure the lexer styles
            textArea.Styles[Style.Python.Identifier].ForeColor = Color.FromArgb(0xD0DAE2);
            textArea.Styles[Style.Python.CommentLine].ForeColor = Color.FromArgb(0x40BF57);
            textArea.Styles[Style.Python.Number].ForeColor = Color.FromArgb(0xFFFF00);
            textArea.Styles[Style.Python.String].ForeColor = Color.FromArgb(0xFF00FF);
            textArea.Styles[Style.Python.Character].ForeColor = Color.FromArgb(0xE95454);
            textArea.Styles[Style.Python.Operator].ForeColor = Color.FromArgb(0xFFFF00);
            textArea.Styles[Style.Python.Word].ForeColor = Color.FromArgb(0x48A8EE);
            textArea.Styles[Style.Python.Word2].ForeColor = Color.FromArgb(0xF98906);

            textArea.Lexer = Lexer.Python;

            textArea.SetKeywords(0, cmdKeyWords);
            textArea.SetKeywords(1, secondaryKeyWords);
        }
        private void openSearchScriptEditorButton_Click(object sender, EventArgs e) {
            currentSearchManager.OpenSearch();
        }

        private void OnTextChangedScript(object sender, EventArgs e) {
            ScriptTextArea.Margins[NUMBER_MARGIN].Width = ScriptTextArea.Lines.Count.ToString().Length * 13;
            scriptsDirty = true;
            scriptsTabPage.Text = ScriptFile.containerTypes.Script.ToString() + "s" + "*";
        }
        private void OnTextChangedFunction(object sender, EventArgs e) {
            FunctionTextArea.Margins[NUMBER_MARGIN].Width = FunctionTextArea.Lines.Count.ToString().Length * 13;
            functionsDirty = true;
            functionsTabPage.Text = ScriptFile.containerTypes.Function.ToString() + "s" + "*";
        }
        private void OnTextChangedAction(object sender, EventArgs e) {
            ActionTextArea.Margins[NUMBER_MARGIN].Width = ActionTextArea.Lines.Count.ToString().Length * 13;
            actionsDirty = true;
            actionsTabPage.Text = ScriptFile.containerTypes.Action.ToString() + "s" + "*";
        }


        #region Numbers, Bookmarks, Code Folding

        /// <summary>
        /// the background color of the text area
        /// </summary>
        private readonly Color BACK_COLOR = Color.FromArgb(0x2A211C);

        /// <summary>
        /// default text color of the text area
        /// </summary>
        private readonly Color FORE_COLOR = Color.FromArgb(0xB7B7B7);

        /// <summary>
        /// change this to whatever margin you want the line numbers to show in
        /// </summary>
        private const int NUMBER_MARGIN = 1;

        /// <summary>
        /// change this to whatever margin you want the bookmarks/breakpoints to show in
        /// </summary>
        private const int BOOKMARK_MARGIN = 2;
        private const int BOOKMARK_MARKER = 2;

        /// <summary>
        /// change this to whatever margin you want the code folding tree (+/-) to show in
        /// </summary>
        private const int FOLDING_MARGIN = 3;

        /// <summary>
        /// set this true to show circular buttons for code folding (the [+] and [-] buttons on the margin)
        /// </summary>
        private const bool CODEFOLDING_CIRCULAR = true;


        private void InitialViewConfig(Scintilla textArea) {
            textArea.Dock = DockStyle.Fill;
            textArea.WrapMode = ScintillaNET.WrapMode.Word;
            textArea.IndentationGuides = IndentView.LookBoth;
            textArea.CaretPeriod = 500;
            textArea.CaretForeColor = Color.White;
            textArea.SetSelectionBackColor(true, Color.FromArgb(0x114D9C));
            textArea.WrapIndentMode = WrapIndentMode.Same;
        }

        private void InitBookmarkMargin(Scintilla textArea) {
            //TextArea.SetFoldMarginColor(true, IntToColor(BACK_COLOR));

            var margin = textArea.Margins[BOOKMARK_MARGIN];
            margin.Width = 20;
            margin.Sensitive = true;
            margin.Type = MarginType.Symbol;
            margin.Mask = (1 << BOOKMARK_MARKER);
            //margin.Cursor = MarginCursor.Arrow;

            var marker = textArea.Markers[BOOKMARK_MARKER];
            marker.Symbol = MarkerSymbol.Circle;
            marker.SetBackColor(Color.FromArgb(0xFF003B));
            marker.SetForeColor(Color.FromArgb(0x000000));
            marker.SetAlpha(100);
        }

        private void InitCodeFolding(Scintilla textArea) {
            textArea.SetFoldMarginColor(true, BACK_COLOR);
            textArea.SetFoldMarginHighlightColor(true, BACK_COLOR);

            // Enable code folding
            textArea.SetProperty("fold", "1");
            textArea.SetProperty("fold.compact", "1");

            // Configure a margin to display folding symbols
            textArea.Margins[FOLDING_MARGIN].Type = MarginType.Symbol;
            textArea.Margins[FOLDING_MARGIN].Mask = Marker.MaskFolders;
            textArea.Margins[FOLDING_MARGIN].Sensitive = true;
            textArea.Margins[FOLDING_MARGIN].Width = 20;

            // Set colors for all folding markers
            for (int i = 25; i <= 31; i++) {
                textArea.Markers[i].SetForeColor(BACK_COLOR); // styles for [+] and [-]
                textArea.Markers[i].SetBackColor(FORE_COLOR); // styles for [+] and [-]
            }

            // Configure folding markers with respective symbols
            textArea.Markers[Marker.Folder].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CirclePlus : MarkerSymbol.BoxPlus;
            textArea.Markers[Marker.FolderOpen].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CircleMinus : MarkerSymbol.BoxMinus;
            textArea.Markers[Marker.FolderEnd].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CirclePlusConnected : MarkerSymbol.BoxPlusConnected;
            textArea.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            textArea.Markers[Marker.FolderOpenMid].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CircleMinusConnected : MarkerSymbol.BoxMinusConnected;
            textArea.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            textArea.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Enable automatic folding
            textArea.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);
        }

        private void ScriptTextArea_MarginClick(object sender, MarginClickEventArgs e) {
            MarginClick(ScriptTextArea, e);
        }

        private void FunctionTextArea_MarginClick(object sender, MarginClickEventArgs e) {
            MarginClick(FunctionTextArea, e);
        }

        private void ActionTextArea_MarginClick(object sender, MarginClickEventArgs e) {
            MarginClick(ActionTextArea, e);
        }

        private void MarginClick(Scintilla textArea, MarginClickEventArgs e) {
            if (e.Margin == BOOKMARK_MARGIN) {
                // Do we have a marker for this line?
                const uint mask = (1 << BOOKMARK_MARKER);
                var line = textArea.Lines[textArea.LineFromPosition(e.Position)];
                if ((line.MarkerGet() & mask) > 0) {
                    // Remove existing bookmark
                    line.MarkerDelete(BOOKMARK_MARKER);
                } else {
                    // Add bookmark
                    line.MarkerAdd(BOOKMARK_MARKER);
                }
            }
        }

        #endregion

        #region Main Menu Commands

        //private void selectLineToolStripMenuItem_Click(object sender, EventArgs e) {
        //    Line line = TextArea.Lines[TextArea.CurrentLine];
        //    TextArea.SetSelection(line.Position + line.Length, line.Position);
        //}
        private void scriptEditorWordWrapCheckbox_CheckedChanged(object sender, EventArgs e) {
            ScriptTextArea.WrapMode = scriptEditorWordWrapCheckbox.Checked ? ScintillaNET.WrapMode.Word : ScintillaNET.WrapMode.None;
            FunctionTextArea.WrapMode = scriptEditorWordWrapCheckbox.Checked ? ScintillaNET.WrapMode.Word : ScintillaNET.WrapMode.None;
            ActionTextArea.WrapMode = scriptEditorWordWrapCheckbox.Checked ? ScintillaNET.WrapMode.Word : ScintillaNET.WrapMode.None;
        }
        //private void indentGuidesCheckbox_CheckedChanged(object sender, EventArgs e) {
        //    ScriptTextArea.IndentationGuides = scriptEditorIndentGuidesCheckbox.Checked ? IndentView.LookBoth : IndentView.None;
        //    FunctionTextArea.IndentationGuides = scriptEditorIndentGuidesCheckbox.Checked ? IndentView.LookBoth : IndentView.None;
        //    ActionTextArea.IndentationGuides = scriptEditorIndentGuidesCheckbox.Checked ? IndentView.LookBoth : IndentView.None;
        //}

        private void viewWhiteSpacesButton_Click(object sender, EventArgs e) {
            ScriptTextArea.ViewWhitespace = scriptEditorWhitespacesCheckbox.Checked ? WhitespaceMode.VisibleAlways : WhitespaceMode.Invisible;
            FunctionTextArea.ViewWhitespace = scriptEditorWhitespacesCheckbox.Checked ? WhitespaceMode.VisibleAlways : WhitespaceMode.Invisible;
            ActionTextArea.ViewWhitespace = scriptEditorWhitespacesCheckbox.Checked ? WhitespaceMode.VisibleAlways : WhitespaceMode.Invisible;
        }

        private void scriptEditorZoomInButton_Click(object sender, EventArgs e) {
            ZoomIn(currentScintillaEditor);
        }
        
        private void scriptEditorZoomOutButton_Click(object sender, EventArgs e) {
            ZoomOut(currentScintillaEditor);
        }
        
        private void scriptEditorZoomResetButton_Click(object sender, EventArgs e) {
            ZoomDefault(currentScintillaEditor);
        }

        private void ScriptEditorCollapseButton_Click(object sender, EventArgs e) {
            currentScintillaEditor.FoldAll(FoldAction.Contract);
        }

        private void ScriptEditorExpandButton_Click(object sender, EventArgs e) {
            currentScintillaEditor.FoldAll(FoldAction.Expand);
        }


        #endregion

        #region Uppercase / Lowercase

        private void Lowercase(Scintilla textArea) {

            // save the selection
            int start = textArea.SelectionStart;
            int end = textArea.SelectionEnd;

            // modify the selected text
            textArea.ReplaceSelection(textArea.GetTextRange(start, end - start).ToLower());

            // preserve the original selection
            textArea.SetSelection(start, end);
        }

        private void Uppercase(Scintilla textArea) {
            // save the selection
            int start = textArea.SelectionStart;
            int end = textArea.SelectionEnd;

            // modify the selected text
            textArea.ReplaceSelection(textArea.GetTextRange(start, end - start).ToUpper());

            // preserve the original selection
            textArea.SetSelection(start, end);
        }

        #endregion

        #region Indent / Outdent

        private void GenerateKeystrokes(string keys, Scintilla textArea) {
            //Example
            //GenerateKeystrokes("+{TAB}");
            HotKeyManager.Enable = false;
            textArea.Focus();
            SendKeys.Send(keys);
            HotKeyManager.Enable = true;
        }

        #endregion

        #region Zoom

        private void ZoomIn(Scintilla textArea) {
            textArea.ZoomIn();
        }

        private void ZoomOut(Scintilla textArea) {
            textArea.ZoomOut();
        }

        private void ZoomDefault(Scintilla textArea) {
            textArea.Zoom = 0;
        }
        #endregion

        #region Quick Search Bar
        private void BtnPrevSearchScript_Click(object sender, EventArgs e) {
            scriptSearchManager.Find(false, false);
        }

        private void BtnNextSearchScript_Click(object sender, EventArgs e) {
            scriptSearchManager.Find(true, false);
        }

        private void BtnPrevSearchFunc_Click(object sender, EventArgs e) {
            functionSearchManager.Find(false, false);
        }

        private void BtnNextSearchFunc_Click(object sender, EventArgs e) {
            functionSearchManager.Find(true, false);
        }

        private void BtnPrevSearchActions_Click(object sender, EventArgs e) {
            actionSearchManager.Find(false, false);
        }

        private void BtnNextSearchActions_Click(object sender, EventArgs e) {
            actionSearchManager.Find(true, false);
        }

        private void BtnCloseSearchScript_Click(object sender, EventArgs e) {
            scriptSearchManager.CloseSearch();
        }

        private void BtnCloseSearchFunc_Click(object sender, EventArgs e) {
            functionSearchManager.CloseSearch();
        }

        private void BtnCloseSearchActions_Click(object sender, EventArgs e) {
            actionSearchManager.CloseSearch();
        }

        private void scriptTxtSearch_KeyDown(object sender, KeyEventArgs e) {
            TxtSearchKeyDown(scriptSearchManager, e);
        }
        private void functionTxtSearch_KeyDown(object sender, KeyEventArgs e) {
            TxtSearchKeyDown(functionSearchManager, e);
        }
        private void actiontTxtSearch_KeyDown(object sender, KeyEventArgs e) {
            TxtSearchKeyDown(actionSearchManager, e);
        }

        private void TxtSearchKeyDown(SearchManager sm, KeyEventArgs e) {
            if (HotKeyManager.IsHotkey(e, Keys.Enter)) {
                sm.Find(true, false);
            }
            if (HotKeyManager.IsHotkey(e, Keys.Enter, true) || HotKeyManager.IsHotkey(e, Keys.Enter, false, true)) {
                sm.Find(false, false);
            }
        }

        private void panelSearchScriptTextBox_TextChanged(object sender, EventArgs e) {
            scriptSearchManager.Find(true, true);
        }
        private void panelSearchFunctionTextBox_TextChanged(object sender, EventArgs e) {
            functionSearchManager.Find(true, true);
        }
        private void panelSearchActionTextBox_TextChanged(object sender, EventArgs e) {
            actionSearchManager.Find(true, true);
        }

        #endregion
        private void addScriptFileButton_Click(object sender, EventArgs e) {
            /* Add new event file to event folder */
            string scriptFilePath = RomInfo.gameDirs[DirNames.scripts].unpackedDir + "\\" + selectScriptFileComboBox.Items.Count.ToString("D4");
            File.WriteAllBytes(scriptFilePath, new ScriptFile(0).ToByteArray());

            /* Update ComboBox and select new file */
            selectScriptFileComboBox.Items.Add("Script File " + selectScriptFileComboBox.Items.Count);
            selectScriptFileComboBox.SelectedIndex = selectScriptFileComboBox.Items.Count - 1;
        }
        private void exportScriptFileButton_Click(object sender, EventArgs e) {
            string suggestion = "Script File ";
            if (currentScriptFile.isLevelScript) {
                suggestion = "Level " + suggestion;
            }
            currentScriptFile.SaveToFileExplorePath(suggestion + selectScriptFileComboBox.SelectedIndex, blindmode: true);
        }
        private void saveScriptFileButton_Click(object sender, EventArgs e) {
            /* Create new ScriptFile object */
            int idToAssign = selectScriptFileComboBox.SelectedIndex;

            ScriptFile userEdited = new ScriptFile(
                scriptLines: ScriptTextArea.Lines.ToStringsList(trim: true), 
                functionLines: FunctionTextArea.Lines.ToStringsList(trim: true), 
                actionLines: ActionTextArea.Lines.ToStringsList(trim: true), 
                selectScriptFileComboBox.SelectedIndex
            );

            /* Write new scripts to file */
            if (userEdited.fileID == null) {
                MessageBox.Show("This " + typeof(ScriptFile).Name + " couldn't be saved, due to a processing error.", "Can't save", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            } else if (userEdited.fileID == int.MaxValue) {
                MessageBox.Show("This " + typeof(ScriptFile).Name + " is couldn't be saved since it's empty.", "Can't save", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            } else { //check if ScriptFile instance was created succesfully
                if (userEdited.SaveToFileDefaultDir(selectScriptFileComboBox.SelectedIndex)) {
                    currentScriptFile = userEdited;
                    ScriptEditorSetClean();
                }
            }
        }
        private void clearCurrentLevelScriptButton_Click(object sender, EventArgs e) {
            string path = RomInfo.gameDirs[DirNames.scripts].unpackedDir + "\\" + selectScriptFileComboBox.SelectedIndex.ToString("D4");
            File.WriteAllBytes(path, new byte[4]);
            MessageBox.Show("Level script correctly cleared.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void importScriptFileButton_Click(object sender, EventArgs e) {
            /* Prompt user to select .scr or .bin file */
            OpenFileDialog of = new OpenFileDialog {
                Filter = "Script File (*.scr, *.bin)|*.scr;*.bin"
            };
            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            /* Update scriptFile object in memory */
            string path = RomInfo.gameDirs[DirNames.scripts].unpackedDir + "\\" + selectScriptFileComboBox.SelectedIndex.ToString("D4");
            File.Copy(of.FileName, path, true);

            /* Refresh controls */
            selectScriptFileComboBox_SelectedIndexChanged(null, null);

            /* Display success message */
            MessageBox.Show("Scripts imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void openScriptButton_Click(object sender, EventArgs e) {
            if (!scriptEditorIsReady) {
                SetupScriptEditorTextAreas();
                SetupScriptEditor();
                scriptEditorIsReady = true;
            }

            scriptEditorTabControl.SelectedIndex = 0;
            selectScriptFileComboBox.SelectedIndex = (int)scriptFileUpDown.Value;
            mainTabControl.SelectedTab = scriptEditorTabPage;
        }
        private void openLevelScriptButton_Click(object sender, EventArgs e) {
            if (!scriptEditorIsReady) {
                SetupScriptEditorTextAreas();
                SetupScriptEditor();
                scriptEditorIsReady = true;
            }

            selectScriptFileComboBox.SelectedIndex = (int)levelScriptUpDown.Value;
            mainTabControl.SelectedTab = scriptEditorTabPage;
        }
        private void removeScriptFileButton_Click(object sender, EventArgs e) {
            DialogResult d = MessageBox.Show("Are you sure you want to delete the last Script File?", "Confirm deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (d.Equals(DialogResult.Yes)) {
                /* Delete script file */
                File.Delete(RomInfo.gameDirs[DirNames.scripts].unpackedDir + "\\" + (selectScriptFileComboBox.Items.Count - 1).ToString("D4"));

                /* Check if currently selected file is the last one, and in that case select the one before it */
                int lastIndex = selectScriptFileComboBox.Items.Count - 1;
                if (selectScriptFileComboBox.SelectedIndex == lastIndex) {
                    selectScriptFileComboBox.SelectedIndex--;
                }

                /* Remove item from ComboBox */
                selectScriptFileComboBox.Items.RemoveAt(lastIndex);
            }
        }
        private void searchInScriptsButton_Click(object sender, EventArgs e) {
            if (searchInScriptsTextBox.Text == "") {
                return;
            }

            int firstArchive;
            int lastArchive;

            if (searchOnlyCurrentScriptCheckBox.Checked) {
                firstArchive = selectScriptFileComboBox.SelectedIndex;
                lastArchive = firstArchive + 1;
            } else {
                firstArchive = 0;
                lastArchive = Helpers.romInfo.GetScriptCount();
            }

            searchInScriptsResultListBox.Items.Clear();
            string searchString = searchInScriptsTextBox.Text;
            searchProgressBar.Maximum = selectScriptFileComboBox.Items.Count;

            List<string> results = new List<string>();

            string scriptKw = ScriptFile.containerTypes.Script.ToString();
            string functionKw = ScriptFile.containerTypes.Function.ToString();

            for (int i = firstArchive; i < lastArchive; i++) {
                try {
                    Console.WriteLine("Attempting to load script " + i);
                    ScriptFile file = new ScriptFile(i, readActions: false);

                    if (scriptSearchCaseSensitiveCheckBox.Checked) {
                        results.AddRange(SearchInScripts(i, file.allScripts, scriptKw, (string s) => s.Contains(searchString)));
                        results.AddRange(SearchInScripts(i, file.allFunctions, functionKw, (string s) => s.Contains(searchString)));
                    } else {
                        results.AddRange(SearchInScripts(i, file.allScripts, scriptKw, (string s) => s.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) >= 0));
                        results.AddRange(SearchInScripts(i, file.allFunctions, functionKw, (string s) => s.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) >= 0));
                    }
                } catch { }
                searchProgressBar.Value = i;
            }

            searchProgressBar.Value = 0;
            searchInScriptsResultListBox.Items.AddRange(results.ToArray());
        }
        private List<string> SearchInScripts(int fileID, List<CommandContainer> cmdList, string entryType, Func<string, bool> criteria) {
            List<string> results = new List<string>();

            for (int j = 0; j < cmdList.Count; j++) { 
                if (cmdList[j].commands is null) {
                    continue;
                }
                foreach (ScriptCommand cur in cmdList[j].commands) {
                    if (criteria(cur.name)) {
                        results.Add($"File {fileID} - {entryType} {j + 1}: {cur.name}{Environment.NewLine}");
                    }
                }
            }
            return results;
        }
        private void searchInScripts_GoToEntryResult(object sender, MouseEventArgs e) {
            if (searchInScriptsResultListBox.SelectedIndex < 0) {
                return;
            }

            string[] split = searchInScriptsResultListBox.SelectedItem.ToString().Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            selectScriptFileComboBox.SelectedIndex = int.Parse(split[1]);
            string cmdNameAndParams = String.Join(" ", split.Skip(5).Take(split.Length - 5));

            if (split[3].StartsWith(ScriptFile.containerTypes.Script.ToString())) {
                if (scriptEditorTabControl.SelectedTab != scriptsTabPage) {
                    scriptEditorTabControl.SelectedTab = scriptsTabPage;
                }
                scriptSearchManager.Find(true, false, ScriptFile.containerTypes.Script.ToString() + " " + split[4].Replace(":", ""));
                scriptSearchManager.Find(true, false, cmdNameAndParams);
            } else if (split[3].StartsWith(ScriptFile.containerTypes.Function.ToString())) {
                if (scriptEditorTabControl.SelectedTab != functionsTabPage) {
                    scriptEditorTabControl.SelectedTab = functionsTabPage;
                }
                functionSearchManager.Find(true, false, ScriptFile.containerTypes.Function.ToString() + " " + split[4].Replace(":", ""));
                functionSearchManager.Find(true, false, cmdNameAndParams);
            } else if (split[3].StartsWith(ScriptFile.containerTypes.Action.ToString())) {
                if (scriptEditorTabControl.SelectedTab != actionsTabPage) {
                    scriptEditorTabControl.SelectedTab = actionsTabPage;
                }
                actionSearchManager.Find(true, false, ScriptFile.containerTypes.Action.ToString() + " " + split[4].Replace(":", ""));
                actionSearchManager.Find(true, false, cmdNameAndParams);
            }
        }
        private void searchInScriptsResultListBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                searchInScripts_GoToEntryResult(null, null);
            }
        }
        private void searchInScriptsTextBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                searchInScriptsButton_Click(null, null);
            }
        }
        private void selectScriptFileComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            ReloadScript();
        }

        private bool ReloadScript() {
            Console.WriteLine("Script Reload has been requested");
            /* clear controls */
            if (Helpers.disableHandlers || selectScriptFileComboBox.SelectedIndex < 0) {
                return false;
            }

            if (scriptsDirty || functionsDirty || actionsDirty) {
                DialogResult d = MessageBox.Show("There are unsaved changes in this Script File.\nDo you wish to discard them?", "Unsaved work", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (!d.Equals(DialogResult.Yes)) {
                    Helpers.disableHandlers = true;
                    selectScriptFileComboBox.SelectedIndex = (int)currentScriptFile.fileID;
                    Helpers.disableHandlers = false;
                    return false;
                }
            }
            currentScriptFile = new ScriptFile(selectScriptFileComboBox.SelectedIndex); // Load script file

            ScriptTextArea.ClearAll();
            FunctionTextArea.ClearAll();
            ActionTextArea.ClearAll();

            scriptsNavListbox.Items.Clear();
            functionsNavListbox.Items.Clear();
            actionsNavListbox.Items.Clear();

            if (currentScriptFile.isLevelScript) {
                ScriptTextArea.Text += "LevelScript files are currently not supported.\nYou can use AdAstra's Level Scripts Editor.";
                addScriptFileButton.Visible = false;
                removeScriptFileButton.Visible = false;

                clearCurrentLevelScriptButton.Visible = true;
            } else {
                Helpers.disableHandlers = true;
                addScriptFileButton.Visible = true;
                removeScriptFileButton.Visible = true;

                clearCurrentLevelScriptButton.Visible = false;

                string buffer = "";

                /* Add scripts */
                for (int i = 0; i < currentScriptFile.allScripts.Count; i++) {
                    CommandContainer currentScript = currentScriptFile.allScripts[i];

                    /* Write header */
                    string header = ScriptFile.containerTypes.Script.ToString() + " " + (i + 1);
                    buffer += header + ':' + Environment.NewLine;
                    scriptsNavListbox.Items.Add(header);

                    /* If current script is identical to another, print UseScript instead of commands */
                    if (currentScript.usedScript < 0) {
                        for (int j = 0; j < currentScript.commands.Count; j++) {
                            if (!ScriptDatabase.endCodes.Contains(currentScript.commands[j].id)) {
                                buffer += '\t';
                            }
                            buffer += currentScript.commands[j].name + Environment.NewLine;
                        }
                    } else {
                        buffer += '\t' + "UseScript_#" + currentScript.usedScript + Environment.NewLine;
                    }

                    ScriptTextArea.AppendText(buffer + Environment.NewLine);
                    buffer = "";
                }


                /* Add functions */
                for (int i = 0; i < currentScriptFile.allFunctions.Count; i++) {
                    CommandContainer currentFunction = currentScriptFile.allFunctions[i];

                    /* Write Heaader */
                    string header = ScriptFile.containerTypes.Function.ToString() + " " + (i + 1);
                    buffer += header + ':' + Environment.NewLine;
                    functionsNavListbox.Items.Add(header);

                    /* If current function is identical to a script, print UseScript instead of commands */
                    if (currentFunction.usedScript < 0) {
                        for (int j = 0; j < currentFunction.commands.Count; j++) {
                            if (!ScriptDatabase.endCodes.Contains(currentFunction.commands[j].id)) {
                                buffer += '\t';
                            }
                            buffer += currentFunction.commands[j].name + Environment.NewLine;
                        }
                    } else {
                        buffer += '\t' + "UseScript_#" + currentFunction.usedScript + Environment.NewLine;
                    }

                    FunctionTextArea.AppendText(buffer + Environment.NewLine);
                    buffer = "";
                }

                /* Add movements */
                for (int i = 0; i < currentScriptFile.allActions.Count; i++) {
                    ActionContainer currentAction = currentScriptFile.allActions[i];

                    string header = ScriptFile.containerTypes.Action.ToString() + " " + (i + 1);
                    buffer += header + ':' + Environment.NewLine;
                    actionsNavListbox.Items.Add(header);

                    for (int j = 0; j < currentAction.actionCommandsList.Count; j++) {
                        if (currentAction.actionCommandsList[j].id != 0x00FE) {
                            buffer += '\t';
                        }
                        buffer += currentAction.actionCommandsList[j].name + Environment.NewLine;
                    }

                    ActionTextArea.AppendText(buffer + Environment.NewLine);
                    buffer = "";
                }
            }

            ScriptEditorSetClean();
            statusLabelMessage();
            Helpers.disableHandlers = false;
            return true;
        }

        private void UpdateScriptNumberFormatDec(object sender, EventArgs e) {
            if (!Helpers.disableHandlers && scriptEditorNumberFormatDecimal.Checked) {
                NumberStyles old = (NumberStyles)Properties.Settings.Default.scriptEditorFormatPreference; //Local Backup
                Properties.Settings.Default.scriptEditorFormatPreference = (int)NumberStyles.Integer;
                

                if (!ReloadScript()) {
                    UpdateScriptNumberCheckBox(old); //Restore old checkbox status! Script couldn't be redrawn
                }
            }
        }
        private void UpdateScriptNumberFormatHex(object sender, EventArgs e) {
            if (!Helpers.disableHandlers && scriptEditorNumberFormatHex.Checked) {
                NumberStyles old = (NumberStyles)Properties.Settings.Default.scriptEditorFormatPreference; //Local Backup
                Properties.Settings.Default.scriptEditorFormatPreference = (int)NumberStyles.HexNumber;

                if (!ReloadScript()) {
                    UpdateScriptNumberCheckBox(old); //Restore old checkbox status! Script couldn't be redrawn
                }
            }
        }
        private void UpdateScriptNumberFormatNoPref(object sender, EventArgs e) {
            if (!Helpers.disableHandlers && scriptEditorNumberFormatNoPreference.Checked) {
                NumberStyles old = (NumberStyles)Properties.Settings.Default.scriptEditorFormatPreference; //Local Backup
                Properties.Settings.Default.scriptEditorFormatPreference = (int)NumberStyles.None;

                if (!ReloadScript()) {
                    UpdateScriptNumberCheckBox(old); //Restore old checkbox status! Script couldn't be redrawn
                }
            }
        }

        private void UpdateScriptNumberCheckBox(NumberStyles toSet) {
                
            Helpers.disableHandlers = true;
            Properties.Settings.Default.scriptEditorFormatPreference = (int)toSet;

            switch ((NumberStyles)Properties.Settings.Default.scriptEditorFormatPreference) {
                case NumberStyles.None:
                    scriptEditorNumberFormatNoPreference.Checked = true;
                    break;
                case NumberStyles.HexNumber:
                    scriptEditorNumberFormatHex.Checked = true;
                    break;
                case NumberStyles.Integer:
                    scriptEditorNumberFormatDecimal.Checked = true;
                    break;
            }
            Console.WriteLine("changed style to " + Properties.Settings.Default.scriptEditorFormatPreference);
            Helpers.disableHandlers = false;
        }

        private void scriptsNavListbox_SelectedIndexChanged(object sender, EventArgs e) {
            NavigatorGoTo((ListBox)sender, 0, scriptSearchManager, ScriptFile.containerTypes.Script.ToString());
        }

        private void functionsNavListbox_SelectedIndexChanged(object sender, EventArgs e) {
            NavigatorGoTo((ListBox)sender, 1, functionSearchManager, ScriptFile.containerTypes.Function.ToString());
        }

        private void actionsNavListbox_SelectedIndexChanged(object sender, EventArgs e) {
            NavigatorGoTo((ListBox)sender, 2, actionSearchManager, ScriptFile.containerTypes.Action.ToString());
        }

        private void NavigatorGoTo(ListBox currentLB, int indexToSwitchTo, SearchManager entrusted, string keyword) {
            if (currentLB.SelectedIndex < 0) {
                return;
            }
            
            if (scriptEditorTabControl.SelectedIndex != indexToSwitchTo) {
                scriptEditorTabControl.SelectedIndex = indexToSwitchTo;
            }

            entrusted.Find(true, false, keyword + ' ' + (currentLB.SelectedIndex + 1) + ':');
        }
        #endregion
        #endregion

        #region Text Editor

        #region Variables
        #endregion

        #region Subroutines

        #endregion

        private void addTextArchiveButton_Click(object sender, EventArgs e) {

        }
        private void addStringButton_Click(object sender, EventArgs e) {

        }
        private void exportTextFileButton_Click(object sender, EventArgs e) {

        }

        private void saveTextArchiveButton_Click(object sender, EventArgs e) {

        }
        private void selectedLineMoveUpButton_Click(object sender, EventArgs e) {

        }

        private void selectedLineMoveDownButton_Click(object sender, EventArgs e) {

        }

        private void importTextFileButton_Click(object sender, EventArgs e) {

        }
        private void removeMessageFileButton_Click(object sender, EventArgs e) {

        }
        private void removeStringButton_Click(object sender, EventArgs e) {

        }
        private void searchMessageButton_Click(object sender, EventArgs e) {

        }

        private void searchMessageTextBox_KeyDown(object sender, KeyEventArgs e) {

        }
        private void replaceMessageButton_Click(object sender, EventArgs e) {

        }
        private void selectTextFileComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }



        private void textEditorDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {

        }
        private void textEditorDataGridView_CurrentCellChanged(object sender, EventArgs e) {

        }
        private void textSearchResultsListBox_GoToEntryResult(object sender, MouseEventArgs e) {

        }
        private void textSearchResultsListBox_KeyDown(object sender, KeyEventArgs e) {

        }
        private void hexRadiobutton_CheckedChanged(object sender, EventArgs e) {

        }

        #endregion

        #region NSBTX Editor
        public NSBTX_File currentNsbtx;
        public AreaData currentAreaData;

        public void FillTilesetBox() {
            texturePacksListBox.Items.Clear();

            int tilesetFileCount;
            if (mapTilesetRadioButton.Checked) {
                tilesetFileCount = Helpers.romInfo.GetMapTexturesCount();
            } else {
                tilesetFileCount = Helpers.romInfo.GetBuildingTexturesCount();
            }

            for (int i = 0; i < tilesetFileCount; i++) {
                texturePacksListBox.Items.Add("Texture Pack " + i);
            }
        }
        private void SetupNSBTXEditor() {
            statusLabelMessage("Attempting to unpack Tileset Editor NARCs... Please wait.");
            Update();

            DSUtils.TryUnpackNarcs(new List<DirNames> {
                DirNames.buildingTextures,
                DirNames.mapTextures,
                DirNames.buildingConfigFiles,
                DirNames.areaData
            });

            /* Fill Tileset ListBox */
            FillTilesetBox();

            /* Fill AreaData ComboBox */
            selectAreaDataListBox.Items.Clear();
            int areaDataCount = Helpers.romInfo.GetAreaDataCount();
            for (int i = 0; i < areaDataCount; i++) {
                selectAreaDataListBox.Items.Add("AreaData File " + i);
            }

            /* Enable gameVersion-specific controls */
            string[] lightTypes;

            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                case gFamEnum.Plat:
                    lightTypes = new string[3] { "Day/Night Light", "Model's light", "Unknown Light" };
                    break;
                default:
                    lightTypes = new string[3] { "Model's light", "Day/Night Light", "Unknown Light" };
                    areaDataDynamicTexturesNumericUpDown.Enabled = true;
                    areaTypeGroupbox.Enabled = true;
                    break;
            };

            areaDataLightTypeComboBox.Items.Clear();
            areaDataLightTypeComboBox.Items.AddRange(lightTypes);

            if (selectAreaDataListBox.Items.Count > 0) {
                selectAreaDataListBox.SelectedIndex = 0;
            }

            if (texturePacksListBox.Items.Count > 0) {
                texturePacksListBox.SelectedIndex = 0;
            }

            if (texturesListBox.Items.Count > 0) {
                texturesListBox.SelectedIndex = 0;
            }

            if (palettesListBox.Items.Count > 0) {
                palettesListBox.SelectedIndex = 0;
            }
            statusLabelMessage();
        }
        private void buildingsTilesetRadioButton_CheckedChanged(object sender, EventArgs e) {
            FillTilesetBox();
            texturePacksListBox.SelectedIndex = (int)areaDataBuildingTilesetUpDown.Value;
            if (texturesListBox.Items.Count > 0) {
                texturesListBox.SelectedIndex = 0;
            }
            if (palettesListBox.Items.Count > 0) {
                palettesListBox.SelectedIndex = 0;
            }
        }
        private void exportNSBTXButton_Click(object sender, EventArgs e) {
            SaveFileDialog sf = new SaveFileDialog {
                Filter = "NSBTX File (*.nsbtx)|*.nsbtx",
                FileName = "Texture Pack " + texturePacksListBox.SelectedIndex
            };
            if (sf.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            string tilesetPath = mapTilesetRadioButton.Checked
                ? RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + texturePacksListBox.SelectedIndex.ToString("D4")
                : RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir + "\\" + texturePacksListBox.SelectedIndex.ToString("D4");
            File.Copy(tilesetPath, sf.FileName);

            MessageBox.Show("NSBTX tileset exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void importNSBTXButton_Click(object sender, EventArgs e) {
            /* Prompt user to select .nsbtx file */
            OpenFileDialog ofd = new OpenFileDialog {
                Filter = "NSBTX File (*.nsbtx)|*.nsbtx"
            };
            if (ofd.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            /* Update nsbtx file */
            string tilesetPath = mapTilesetRadioButton.Checked
                ? RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + texturePacksListBox.SelectedIndex.ToString("D4")
                : RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir + "\\" + texturePacksListBox.SelectedIndex.ToString("D4");
            File.Copy(ofd.FileName, tilesetPath, true);

            /* Update nsbtx object in memory and controls */
            currentNsbtx = new NSMBe4.NSBMD.NSBTX_File(new FileStream(ofd.FileName, FileMode.Open));
            texturePacksListBox_SelectedIndexChanged(null, null);
            MessageBox.Show("NSBTX tileset imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void mapTilesetRadioButton_CheckedChanged(object sender, EventArgs e) {
            FillTilesetBox();

            try {
                if (mapTilesetRadioButton.Checked) {
                    texturePacksListBox.SelectedIndex = (int)areaDataMapTilesetUpDown.Value;
                } else if (buildingsTilesetRadioButton.Checked) {
                    texturePacksListBox.SelectedIndex = (int)areaDataBuildingTilesetUpDown.Value;
                }
            } catch (ArgumentOutOfRangeException) {
                texturePacksListBox.SelectedIndex = 0;
            }
        }
        private void palettesListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }

            palettesLabel.Text = $"Palettes [{palettesListBox.SelectedIndex + 1}/{palettesListBox.Items.Count}]";

            int ctrlCode = NSBTXRender(tex: texturesListBox.SelectedIndex, pal: palettesListBox.SelectedIndex, scale: nsbtxScaleFactor);
            if (ctrlCode > 0) {
                statusLabelError($"ERROR! The selected palette doesn't have enough colors for this Palette{ctrlCode} texture.");
            } else {
                statusLabelMessage();
            }
        }
        private void texturePacksListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }
            Helpers.disableHandlers = true;

            /* Clear ListBoxes */
            texturesListBox.Items.Clear();
            palettesListBox.Items.Clear();

            /* Load tileset file */
            string tilesetPath = mapTilesetRadioButton.Checked
                ? RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + texturePacksListBox.SelectedIndex.ToString("D4")
                : RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir + "\\" + texturePacksListBox.SelectedIndex.ToString("D4");

            currentNsbtx = new NSBTX_File(new FileStream(tilesetPath, FileMode.Open));
            string currentItemName = texturePacksListBox.Items[texturePacksListBox.SelectedIndex].ToString();

            if (currentNsbtx.texInfo.names is null || currentNsbtx.palInfo.names is null) {
                if (!currentItemName.StartsWith("Error!")) {
                    texturePacksListBox.Items[texturePacksListBox.SelectedIndex] = "Error! - " + currentItemName;
                }

                Helpers.disableHandlers = false;
                return;
            }
            /* Add textures and palette slot names to ListBoxes */
            texturesListBox.Items.AddRange(currentNsbtx.texInfo.names.ToArray());
            palettesListBox.Items.AddRange(currentNsbtx.palInfo.names.ToArray());

            Helpers.disableHandlers = false;

            if (texturesListBox.Items.Count > 0) {
                texturesListBox.SelectedIndex = 0;
            }
        }
        private void texturesListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }

            bool disableHandlersBackup = Helpers.disableHandlers;
            Helpers.disableHandlers = true;

            texturesLabel.Text = $"Textures [{texturesListBox.SelectedIndex + 1}/{texturesListBox.Items.Count}]";

            string findThis = texturesListBox.SelectedItem.ToString();
            string matchingPalette = findAndSelectMatchingPalette(findThis);
            if (matchingPalette == null) {
                statusLabelError("Couldn't find a palette to match " + '"' + findThis + '"', severe: false);
            } else {
                //palettesListBox.SelectedIndex = 0;
                palettesListBox.SelectedItem = matchingPalette;
                statusLabelMessage("Ready");
            }

            Helpers.disableHandlers = disableHandlersBackup;

            int ctrlCode = NSBTXRender(tex: Math.Max(0, texturesListBox.SelectedIndex), pal: Math.Max(0, palettesListBox.SelectedIndex), scale: nsbtxScaleFactor);
            if (matchingPalette != null && ctrlCode > 0) {
                statusLabelError($"ERROR! The selected palette doesn't have enough colors for this Palette{ctrlCode} texture.");
            }
        }
        private string findAndSelectMatchingPalette(string findThis) {
            statusLabelMessage("Searching palette...");

            string copy = findThis;
            while (copy.Length > 0) {
                if (palettesListBox.Items.Contains(copy + "_pl")) {
                    return copy + "_pl";
                }
                if (palettesListBox.Items.Contains(copy)) {
                    return copy;
                }
                copy = copy.Substring(0, copy.Length - 1);
            }

            foreach (string palette in palettesListBox.Items) {
                if (palette.StartsWith(findThis)) {
                    return palette;
                }
            }

            return null;
        }
        private void areaDataBuildingTilesetUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }
            currentAreaData.buildingsTileset = (ushort)areaDataBuildingTilesetUpDown.Value;
        }
        private void areaDataDynamicTexturesUpDown_ValueChanged(object sender, EventArgs e) {
            if (areaDataDynamicTexturesNumericUpDown.Value == areaDataDynamicTexturesNumericUpDown.Maximum) {
                areaDataDynamicTexturesNumericUpDown.ForeColor = Color.Red;
            } else {
                areaDataDynamicTexturesNumericUpDown.ForeColor = Color.Black;
            }

            if (Helpers.disableHandlers) {
                return;
            }
            currentAreaData.dynamicTextureType = (ushort)areaDataDynamicTexturesNumericUpDown.Value;
        }
        private void areaDataLightTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }
            currentAreaData.lightType = (byte)areaDataLightTypeComboBox.SelectedIndex;
        }
        private void areaDataMapTilesetUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }
            currentAreaData.mapTileset = (ushort)areaDataMapTilesetUpDown.Value;
        }
        private void saveAreaDataButton_Click(object sender, EventArgs e) {
            currentAreaData.SaveToFileDefaultDir(selectAreaDataListBox.SelectedIndex);
        }
        private void selectAreaDataListBox_SelectedIndexChanged(object sender, EventArgs e) {
            currentAreaData = new AreaData((byte)selectAreaDataListBox.SelectedIndex);

            areaDataBuildingTilesetUpDown.Value = currentAreaData.buildingsTileset;
            areaDataMapTilesetUpDown.Value = currentAreaData.mapTileset;
            areaDataLightTypeComboBox.SelectedIndex = currentAreaData.lightType;

            Helpers.disableHandlers = true;
            if (RomInfo.gameFamily == gFamEnum.HGSS) {
                areaDataDynamicTexturesNumericUpDown.Value = currentAreaData.dynamicTextureType;

                bool interior = currentAreaData.areaType == 0;
                indoorAreaRadioButton.Checked = interior;
                outdoorAreaRadioButton.Checked = !interior;
            }
            Helpers.disableHandlers = false;
        }
        private void indoorAreaRadioButton_CheckedChanged(object sender, EventArgs e) {
            currentAreaData.areaType = indoorAreaRadioButton.Checked ? AreaData.TYPE_INDOOR : AreaData.TYPE_OUTDOOR;
        }
        private void addNSBTXButton_Click(object sender, EventArgs e) {
            /* Add new NSBTX file to the correct folder */
            if (mapTilesetRadioButton.Checked) {
                File.Copy(RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + 0.ToString("D4"), RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + texturePacksListBox.Items.Count.ToString("D4"));
                
                if (mapEditorIsReady) {
                    mapTextureComboBox.Items.Add("Map Texture Pack [" + mapTextureComboBox.Items.Count.ToString("D2") + "]");
                }
            } else {
                File.Copy(RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir + "\\" + 0.ToString("D4"), RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir + "\\" + texturePacksListBox.Items.Count.ToString("D4"));
                File.Copy(RomInfo.gameDirs[DirNames.buildingConfigFiles].unpackedDir + "\\" + 0.ToString("D4"), RomInfo.gameDirs[DirNames.buildingConfigFiles].unpackedDir + "\\" + texturePacksListBox.Items.Count.ToString("D4"));
               
                if (mapEditorIsReady) {
                    buildTextureComboBox.Items.Add("Building Texture Pack [" + buildTextureComboBox.Items.Count.ToString("D2") + "]");
                }
            }

            /* Update ComboBox and select new file */
            texturePacksListBox.Items.Add("Texture Pack " + texturePacksListBox.Items.Count);
            texturePacksListBox.SelectedIndex = texturePacksListBox.Items.Count - 1;
        }
        private void removeNSBTXButton_Click(object sender, EventArgs e) {
            if (texturePacksListBox.Items.Count > 1) {
                /* Delete NSBTX file */
                DialogResult d = MessageBox.Show("Are you sure you want to delete the last Texture Pack?", "Confirm deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (d.Equals(DialogResult.Yes)) {
                    if (mapTilesetRadioButton.Checked) {
                        File.Delete(RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + (texturePacksListBox.Items.Count - 1).ToString("D4"));

                        if (mapEditorIsReady) {
                            mapTextureComboBox.Items.RemoveAt(mapTextureComboBox.Items.Count - 1);
                        }
                    } else {
                        File.Delete(RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir + "\\" + (texturePacksListBox.Items.Count - 1).ToString("D4"));
                        File.Delete(RomInfo.gameDirs[DirNames.buildingConfigFiles].unpackedDir + "\\" + (texturePacksListBox.Items.Count - 1).ToString("D4"));

                        if (mapEditorIsReady) {
                            buildTextureComboBox.Items.RemoveAt(buildTextureComboBox.Items.Count - 1);
                        }
                    }

                    /* Check if currently selected file is the last one, and in that case select the one before it */
                    int lastIndex = texturePacksListBox.Items.Count - 1;
                    if (texturePacksListBox.SelectedIndex == lastIndex) {
                        texturePacksListBox.SelectedIndex--;
                    }

                    /* Remove item from ComboBox */
                    texturePacksListBox.Items.RemoveAt(lastIndex);
                }
            } else {
                MessageBox.Show("At least one tileset must be kept.", "Can't delete tileset", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void addAreaDataButton_Click(object sender, EventArgs e) {
            /* Add new NSBTX file to the correct folder */
            string areaDataDirPath = RomInfo.gameDirs[DirNames.areaData].unpackedDir;
            File.Copy(areaDataDirPath + "\\" + 0.ToString("D4"), areaDataDirPath + "\\" + selectAreaDataListBox.Items.Count.ToString("D4"));

            /* Update ComboBox and select new file */
            selectAreaDataListBox.Items.Add("AreaData File " + selectAreaDataListBox.Items.Count);
            selectAreaDataListBox.SelectedIndex = selectAreaDataListBox.Items.Count - 1;

            if (eventEditor.eventEditorIsReady) {
                eventEditor.eventAreaDataUpDown.Maximum++;
            }
        }
        private void removeAreaDataButton_Click(object sender, EventArgs e) {
            if (selectAreaDataListBox.Items.Count > 1) {
                /* Delete AreaData file */
                File.Delete(RomInfo.gameDirs[DirNames.areaData].unpackedDir + "\\" + (selectAreaDataListBox.Items.Count - 1).ToString("D4"));

                /* Check if currently selected file is the last one, and in that case select the one before it */
                int lastIndex = selectAreaDataListBox.Items.Count - 1;
                if (selectAreaDataListBox.SelectedIndex == lastIndex) {
                    selectAreaDataListBox.SelectedIndex--;
                }

                /* Remove item from ComboBox */
                selectAreaDataListBox.Items.RemoveAt(lastIndex);

                if (eventEditor.eventEditorIsReady) {
                    eventEditor.eventAreaDataUpDown.Maximum--;
                }
            } else {
                MessageBox.Show("At least one AreaData file must be kept.", "Can't delete AreaData", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        private void exportAreaDataButton_Click(object sender, EventArgs e) {
            currentAreaData.SaveToFileExplorePath("Area Data " + selectAreaDataListBox.SelectedIndex);
        }
        private void importAreaDataButton_Click(object sender, EventArgs e) {
            if (selectAreaDataListBox.SelectedIndex < 0) {
                return;
            }

            OpenFileDialog of = new OpenFileDialog {
                Filter = "AreaData File (*.bin)|*.bin"
            };
            
            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            /* Update areadata object in memory */
            string path = RomInfo.gameDirs[DirNames.areaData].unpackedDir + "\\" + selectAreaDataListBox.SelectedIndex.ToString("D4");
            File.Copy(of.FileName, path, true);

            /* Refresh controls */
            selectAreaDataListBox_SelectedIndexChanged(sender, e);

            /* Display success message */
            MessageBox.Show("AreaData File imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #region Camera Editor
        GameCamera[] currentCameraTable;
        uint overlayCameraTblOffset;

        private void SetupCameraEditor() {
            RomInfo.PrepareCameraData();
            cameraEditorDataGridView.Rows.Clear();

            if (DSUtils.CheckOverlayHasCompressionFlag(RomInfo.cameraTblOverlayNumber)) {
                DialogResult d1 = MessageBox.Show("It is STRONGLY recommended to configure Overlay1 as uncompressed before proceeding.\n\n" +
                        "More details in the following dialog.\n\n" + "Do you want to know more?",
                        "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                bool userConfirmed = (d1 == DialogResult.Yes && ROMToolboxDialog.ConfigureOverlay1Uncompressed());
                

                if (!userConfirmed) {
                    MessageBox.Show("You chose not to apply the patch. Use this editor responsibly.\n\n" +
                            "If you change your mind, you can apply it later by accessing the ROM Toolbox.",
                            "Caution", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (DSUtils.OverlayIsCompressed(RomInfo.cameraTblOverlayNumber)) {
                        DSUtils.DecompressOverlay(RomInfo.cameraTblOverlayNumber);
                    }
                }
            }


            uint[] RAMaddresses = new uint[RomInfo.cameraTblOffsetsToRAMaddress.Length];
            string camOverlayPath = DSUtils.GetOverlayPath(RomInfo.cameraTblOverlayNumber);
            using (DSUtils.EasyReader br = new DSUtils.EasyReader(camOverlayPath)) {
                for (int i = 0; i < RomInfo.cameraTblOffsetsToRAMaddress.Length; i++) {
                    br.BaseStream.Position = RomInfo.cameraTblOffsetsToRAMaddress[i];
                    RAMaddresses[i] = br.ReadUInt32();
                }
            }

            uint referenceAddress = RAMaddresses[0];
            for (int i = 1; i < RAMaddresses.Length; i++) {
                uint ramAddress = RAMaddresses[i];
                if (ramAddress != referenceAddress) {
                    MessageBox.Show("Value of RAM Pointer to the overlay table is different between Offset #1 and Offset #" + (i + 1) + Environment.NewLine +
                        "The camera values might be wrong.", "Possible errors ahead", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            overlayCameraTblOffset = RAMaddresses[0] - DSUtils.GetOverlayRAMAddress(RomInfo.cameraTblOverlayNumber);
            using (DSUtils.EasyReader br = new DSUtils.EasyReader(camOverlayPath, overlayCameraTblOffset)) {
                if (RomInfo.gameFamily == gFamEnum.HGSS) {
                    currentCameraTable = new GameCamera[17];
                    for (int i = 0; i < currentCameraTable.Length; i++) {
                        currentCameraTable[i] = new GameCamera(br.ReadUInt32(), br.ReadInt16(), br.ReadInt16(), br.ReadInt16(),
                                                br.ReadInt16(), br.ReadByte(), br.ReadByte(),
                                                br.ReadUInt16(), br.ReadUInt32(), br.ReadUInt32(),
                                                br.ReadInt32(), br.ReadInt32(), br.ReadInt32());

                    }
                } else {
                    currentCameraTable = new GameCamera[16];
                    for (int i = 0; i < 3; i++) {
                        cameraEditorDataGridView.Columns.RemoveAt(cameraEditorDataGridView.Columns.Count - 3);
                    }
                    for (int i = 0; i < currentCameraTable.Length; i++) {
                        currentCameraTable[i] = new GameCamera(br.ReadUInt32(), br.ReadInt16(), br.ReadInt16(), br.ReadInt16(),
                                                br.ReadInt16(), br.ReadByte(), br.ReadByte(),
                                                br.ReadUInt16(), br.ReadUInt32(), br.ReadUInt32());
                    }
                }

                cameraEditorDataGridView.RowTemplate.Height = 32 * 16 / currentCameraTable.Length;
                for (int i = 0; i < currentCameraTable.Length; i++) {
                    currentCameraTable[i].ShowInGridView(cameraEditorDataGridView, i);
                }
            }
        }
        private void saveCameraTableButton_Click(object sender, EventArgs e) {
            SaveCameraTable(DSUtils.GetOverlayPath(RomInfo.cameraTblOverlayNumber), overlayCameraTblOffset);
        }
        private void cameraEditorDataGridView_CellValidated(object sender, DataGridViewCellEventArgs e) {
            //cameraEditorDataGridView.Columns[0].ValueType = typeof(int);
            currentCameraTable[e.RowIndex][e.ColumnIndex] = cameraEditorDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            cameraEditorDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = currentCameraTable[e.RowIndex][e.ColumnIndex];
        }
        private void exportCameraTableButton_Click(object sender, EventArgs e) {
            SaveFileDialog of = new SaveFileDialog {
                Filter = "Camera Table File (*.bin)|*.bin",
                FileName = Path.GetFileNameWithoutExtension(RomInfo.fileName) + " - CameraTable.bin"
            };
            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            File.Delete(of.FileName);
            SaveCameraTable(of.FileName, 0);
        }
        private void SaveCameraTable(string path, uint destFileOffset) {
            for (int i = 0; i < currentCameraTable.Length; i++) {
                DSUtils.WriteToFile(path, currentCameraTable[i].ToByteArray(), (uint)(destFileOffset + i * RomInfo.cameraSize));
            }
            MessageBox.Show("Camera table correctly saved.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void cameraEditorDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e) {
            var senderTable = (DataGridView)sender;

            if (senderTable.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0) {
                string type = "Camera File";
                if (e.ColumnIndex == cameraEditorDataGridView.Columns.Count - 2) { //Export
                    SaveFileDialog sf = new SaveFileDialog {
                        Filter = type + " (*.bin)|*.bin",
                        FileName = Path.GetFileNameWithoutExtension(RomInfo.fileName) + " - Camera " + e.RowIndex + ".bin"
                    };

                    if (sf.ShowDialog(this) != DialogResult.OK) {
                        return;
                    }

                    DSUtils.WriteToFile(sf.FileName, currentCameraTable[e.RowIndex].ToByteArray(), fmode: FileMode.Create);
                    MessageBox.Show("Camera correctly saved.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                } else if (e.ColumnIndex == cameraEditorDataGridView.Columns.Count - 1) { //Import
                    OpenFileDialog of = new OpenFileDialog {
                        Filter = type + " (*.bin)|*.bin",
                    };

                    if (of.ShowDialog(this) != DialogResult.OK) {
                        return;
                    }

                    currentCameraTable[e.RowIndex] = new GameCamera(File.ReadAllBytes(of.FileName));
                    currentCameraTable[e.RowIndex].ShowInGridView(senderTable, e.RowIndex);
                    MessageBox.Show("Camera correctly imported.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void importCameraTableButton_Click(object sender, EventArgs e) {
            string fileType = "Camera Table File";
            OpenFileDialog of = new OpenFileDialog {
                Filter = fileType + " (*.bin)|*.bin",
            };

            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            long l = new FileInfo(of.FileName).Length;
            if (l % RomInfo.cameraSize != 0) {
                MessageBox.Show("This is not a " + RomInfo.gameFamily + ' ' + fileType +
                    "\nMake sure the file length is a multiple of " + RomInfo.cameraSize + " and try again.", "Wrong file!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            byte nCameras = (byte)(l / RomInfo.cameraSize);
            for (byte b = 0; b < nCameras; b++) {
                currentCameraTable[b] = new GameCamera(DSUtils.ReadFromFile(of.FileName, b * RomInfo.cameraSize, RomInfo.cameraSize));
                currentCameraTable[b].ShowInGridView(cameraEditorDataGridView, b);
            }
            MessageBox.Show("Camera Table imported correctly.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #region Trainer Editor
        private List<ComboBox> partyPokemonComboboxList = new List<ComboBox>();
        private List<ComboBox> partyItemsComboboxList = new List<ComboBox>();
        private List<GroupBox> partyMovesGroupboxList = new List<GroupBox>();
        private List<NumericUpDown> partyLevelUpdownList = new List<NumericUpDown>();
        private List<NumericUpDown> partyIVUpdownList = new List<NumericUpDown>();
        private List<NumericUpDown> partyBallUpdownList = new List<NumericUpDown>();
        private List<GroupBox> partyGroupboxList = new List<GroupBox>();
        private List<PictureBox> partyPokemonPictureBoxList = new List<PictureBox>();
        private List<PictureBox> partyPokemonItemIconList = new List<PictureBox>();

        TrainerFile currentTrainerFile;
        PaletteBase trainerPal;
        ImageBase trainerTile;
        SpriteBase trainerSprite;

        private readonly PaletteBase[] monIconPals = new PaletteBase[6];
        private readonly ImageBase[] monIconTiles = new ImageBase[6];
        private readonly SpriteBase[] monIconSprites = new SpriteBase[6];

        Dictionary<byte, (uint entryOffset, ushort musicD, ushort? musicN)> trainerClassEncounterMusicDict;
        private void SetupTrainerClassEncounterMusicTable() {
            RomInfo.SetEncounterMusicTableOffsetToRAMAddress();
            trainerClassEncounterMusicDict = new Dictionary<byte, (uint entryOffset, ushort musicD, ushort? musicN)>();

            uint encounterMusicTableTableStartAddress = BitConverter.ToUInt32(DSUtils.ARM9.ReadBytes(RomInfo.encounterMusicTableOffsetToRAMAddress, 4), 0) - 0x02000000;
            
            uint entrySize = 4;
            uint tableSizeOffset = 10;
            if (gameFamily == gFamEnum.HGSS) {
                entrySize += 2;
                tableSizeOffset += 2;
                encounterSSEQAltUpDown.Enabled = true;
            }

            byte tableEntriesCount = DSUtils.ARM9.ReadByte(RomInfo.encounterMusicTableOffsetToRAMAddress - tableSizeOffset);
            using (DSUtils.ARM9.Reader ar = new DSUtils.ARM9.Reader(encounterMusicTableTableStartAddress) ) {
                for (int i = 0; i < tableEntriesCount; i++) {
                    uint entryOffset = (uint)ar.BaseStream.Position;
                    byte tclass = (byte)ar.ReadUInt16();
                    ushort musicD = ar.ReadUInt16();
                    ushort? musicN = gameFamily == gFamEnum.HGSS ? ar.ReadUInt16() : (ushort?)null;
                    trainerClassEncounterMusicDict[tclass] = (entryOffset, musicD, musicN);
                }
            }
        }
        private void SetupTrainerEditor() {
            Helpers.disableHandlers = true;
            SetupTrainerClassEncounterMusicTable();
            /* Extract essential NARCs sub-archives*/
            statusLabelMessage("Setting up Trainer Editor...");
            Update();

            DSUtils.TryUnpackNarcs(new List<DirNames> { 
                DirNames.trainerProperties, 
                DirNames.trainerParty, 
                DirNames.trainerGraphics, 
                DirNames.textArchives,
                DirNames.monIcons
            });

            partyPokemonComboboxList.Clear();
            partyPokemonComboboxList.Add(partyPokemon1ComboBox);
            partyPokemonComboboxList.Add(partyPokemon2ComboBox);
            partyPokemonComboboxList.Add(partyPokemon3ComboBox);
            partyPokemonComboboxList.Add(partyPokemon4ComboBox);
            partyPokemonComboboxList.Add(partyPokemon5ComboBox);
            partyPokemonComboboxList.Add(partyPokemon6ComboBox);

            partyItemsComboboxList.Clear();
            partyItemsComboboxList.Add(partyItem1ComboBox);
            partyItemsComboboxList.Add(partyItem2ComboBox);
            partyItemsComboboxList.Add(partyItem3ComboBox);
            partyItemsComboboxList.Add(partyItem4ComboBox);
            partyItemsComboboxList.Add(partyItem5ComboBox);
            partyItemsComboboxList.Add(partyItem6ComboBox);

            partyLevelUpdownList.Clear();
            partyLevelUpdownList.Add(partyLevel1UpDown);
            partyLevelUpdownList.Add(partyLevel2UpDown);
            partyLevelUpdownList.Add(partyLevel3UpDown);
            partyLevelUpdownList.Add(partyLevel4UpDown);
            partyLevelUpdownList.Add(partyLevel5UpDown);
            partyLevelUpdownList.Add(partyLevel6UpDown);

            partyIVUpdownList.Clear();
            partyIVUpdownList.Add(partyIV1UpDown);
            partyIVUpdownList.Add(partyIV2UpDown);
            partyIVUpdownList.Add(partyIV3UpDown);
            partyIVUpdownList.Add(partyIV4UpDown);
            partyIVUpdownList.Add(partyIV5UpDown);
            partyIVUpdownList.Add(partyIV6UpDown);

            partyBallUpdownList.Clear();
            partyBallUpdownList.Add(partyBall1UpDown);
            partyBallUpdownList.Add(partyBall2UpDown);
            partyBallUpdownList.Add(partyBall3UpDown);
            partyBallUpdownList.Add(partyBall4UpDown);
            partyBallUpdownList.Add(partyBall5UpDown);
            partyBallUpdownList.Add(partyBall6UpDown);

            partyMovesGroupboxList.Clear();
            partyMovesGroupboxList.Add(poke1MovesGroupBox);
            partyMovesGroupboxList.Add(poke2MovesGroupBox);
            partyMovesGroupboxList.Add(poke3MovesGroupBox);
            partyMovesGroupboxList.Add(poke4MovesGroupBox);
            partyMovesGroupboxList.Add(poke5MovesGroupBox);
            partyMovesGroupboxList.Add(poke6MovesGroupBox);

            partyGroupboxList.Clear();
            partyGroupboxList.Add(party1GroupBox);
            partyGroupboxList.Add(party2GroupBox);
            partyGroupboxList.Add(party3GroupBox);
            partyGroupboxList.Add(party4GroupBox);
            partyGroupboxList.Add(party5GroupBox);
            partyGroupboxList.Add(party6GroupBox);

            partyPokemonPictureBoxList.Clear();
            partyPokemonPictureBoxList.Add(partyPokemon1PictureBox);
            partyPokemonPictureBoxList.Add(partyPokemon2PictureBox);
            partyPokemonPictureBoxList.Add(partyPokemon3PictureBox);
            partyPokemonPictureBoxList.Add(partyPokemon4PictureBox);
            partyPokemonPictureBoxList.Add(partyPokemon5PictureBox);
            partyPokemonPictureBoxList.Add(partyPokemon6PictureBox);

            partyPokemonItemIconList.Clear();
            partyPokemonItemIconList.Add(partyPokemonItemPictureBox1);
            partyPokemonItemIconList.Add(partyPokemonItemPictureBox2);
            partyPokemonItemIconList.Add(partyPokemonItemPictureBox3);
            partyPokemonItemIconList.Add(partyPokemonItemPictureBox4);
            partyPokemonItemIconList.Add(partyPokemonItemPictureBox5);
            partyPokemonItemIconList.Add(partyPokemonItemPictureBox6);

            int trainerCount = Directory.GetFiles(RomInfo.gameDirs[DirNames.trainerProperties].unpackedDir).Length;
            trainerComboBox.Items.Clear();
            trainerComboBox.Items.AddRange(Helpers.GetTrainerNames());

            string[] classNames = RomInfo.GetTrainerClassNames();
            trainerClassListBox.Items.Clear();
            if (classNames.Length > byte.MaxValue + 1) {
                MessageBox.Show("There can't be more than 256 trainer classes! [Found " + classNames.Length + "].\nAborting.",
                    "Too many trainer classes", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            for (int i = 0; i < classNames.Length; i++) {
                trainerClassListBox.Items.Add("[" + i.ToString("D3") + "]" + " " + classNames[i]);
            }

            string[] itemNames = RomInfo.GetItemNames();
            string[] pokeNames = RomInfo.GetPokemonNames();
            string[] moveNames = RomInfo.GetAttackNames();

            foreach(Control c in trainerItemsGroupBox.Controls) {
                if (c is ComboBox) {
                    (c as ComboBox).DataSource = new BindingSource(itemNames, string.Empty);
                }
            }

            foreach (ComboBox CB in partyPokemonComboboxList) {
                CB.DataSource = new BindingSource(pokeNames, string.Empty);
            }

            foreach (ComboBox CB in partyItemsComboboxList) {
                CB.DataSource = new BindingSource(itemNames, string.Empty);
            }

            foreach (GroupBox movesGroup in partyMovesGroupboxList) {
                foreach (Control c in movesGroup.Controls) {
                    if (c is ComboBox) {
                        (c as ComboBox).DataSource = new BindingSource(moveNames, string.Empty);
                    }
                }
            }

            trainerComboBox.SelectedIndex = 0;

            Helpers.disableHandlers = false;
            trainerComboBox_SelectedIndexChanged(null, null);
            statusLabelMessage();
        }
        private void trainerComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }
            Helpers.disableHandlers = true;

            int currentIndex = trainerComboBox.SelectedIndex;
            string suffix = "\\" + currentIndex.ToString("D4");
            string[] trNames = RomInfo.GetSimpleTrainerNames();

            bool error = currentIndex >= trNames.Length;

            currentTrainerFile = new TrainerFile(
                new TrainerProperties(
                    (ushort)trainerComboBox.SelectedIndex, 
                    new FileStream(RomInfo.gameDirs[DirNames.trainerProperties].unpackedDir + suffix, FileMode.Open)
                ),
                new FileStream(RomInfo.gameDirs[DirNames.trainerParty].unpackedDir + suffix, FileMode.Open),
                error ? TrainerFile.NAME_NOT_FOUND : trNames[currentIndex]
            );

            RefreshTrainerPartyGUI();
            RefreshTrainerPropertiesGUI();

            Helpers.disableHandlers = false;

            if (error) {
                MessageBox.Show("This Trainer File doesn't have a corresponding name.\n\n" +
                    "If you edited this ROM's Trainers with another tool before, don't worry.\n" +
                    "DSPRE will attempt to add the missing line to the Trainer Names Text Archive [" + RomInfo.trainerNamesMessageNumber + "] upon resaving.",
                    "Trainer name not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void RefreshTrainerPropertiesGUI() {
            trainerNameTextBox.Text = currentTrainerFile.name;

            trainerClassListBox.SelectedIndex = currentTrainerFile.trp.trainerClass;
            trainerDoubleCheckBox.Checked = currentTrainerFile.trp.doubleBattle;
            trainerMovesCheckBox.Checked = currentTrainerFile.trp.hasMoves;
            trainerItemsCheckBox.Checked = currentTrainerFile.trp.hasItems;
            partyCountUpDown.Value = currentTrainerFile.trp.partyCount;

            IList trainerItems = trainerItemsGroupBox.Controls;
            for (int i = 0; i < trainerItems.Count; i++) {
                (trainerItems[i] as ComboBox).SelectedIndex = currentTrainerFile.trp.trainerItems[i];
            }

            IList trainerAI = TrainerAIGroupBox.Controls;
            for (int i = 0; i < trainerAI.Count; i++) {
                (trainerAI[i] as CheckBox).Checked = currentTrainerFile.trp.AI[i];
            }
        }
        public void RefreshTrainerPartyGUI() {
            for (int i = 0; i < TrainerFile.POKE_IN_PARTY; i++) {
                partyPokemonComboboxList[i].SelectedIndex = currentTrainerFile.party[i].pokeID ?? 0;
                partyItemsComboboxList[i].SelectedIndex = currentTrainerFile.party[i].heldItem ?? 0;
                partyLevelUpdownList[i].Value = Math.Max((ushort)1, currentTrainerFile.party[i].level);
                partyIVUpdownList[i].Value = currentTrainerFile.party[i].unknown1_DATASTART;
                partyBallUpdownList[i].Value = currentTrainerFile.party[i].unknown2_DATAEND;

                if (currentTrainerFile.party[i].moves == null) {
                    for (int j = 0; j < Party.MOVES_PER_POKE; j++) {
                        (partyMovesGroupboxList[i].Controls[j] as ComboBox).SelectedIndex = 0;
                    }
                } else {
                    for (int j = 0; j < Party.MOVES_PER_POKE; j++) {
                        (partyMovesGroupboxList[i].Controls[j] as ComboBox).SelectedIndex = currentTrainerFile.party[i].moves[j];
                    }
                }
            }
        }

        private void ShowPartyPokemonPic(byte partyPos) {
            ComboBox cb = partyPokemonComboboxList[partyPos];
            int species = cb.SelectedIndex > 0 ? cb.SelectedIndex : 0;

            PictureBox pb = partyPokemonPictureBoxList[partyPos];

            partyPokemonPictureBoxList[partyPos].Image = GetPokePic(species, pb.Width, pb.Height, monIconPals[partyPos], monIconTiles[partyPos], monIconSprites[partyPos]);
        }
        private Image GetPokePic(int species, int w, int h, PaletteBase paletteBase, ImageBase imageBase, SpriteBase spriteBase) {
            bool fiveDigits = false; // some extreme future proofing
            string filename = "0000";

            try {
                paletteBase = new NCLR(gameDirs[DirNames.monIcons].unpackedDir + "\\" + filename, 0, filename);
            } catch (FileNotFoundException) {
                filename += '0';
                paletteBase = new NCLR(gameDirs[DirNames.monIcons].unpackedDir + "\\" + filename, 0, filename);
                fiveDigits = true;
            }

            // read arm9 table to grab pal ID
            int paletteId = 0;
            byte[] iconPalTableBuf;

            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                    iconPalTableBuf = DSUtils.ARM9.ReadBytes(0x6B838, 4);
                    break;
                case gFamEnum.Plat:
                    iconPalTableBuf = DSUtils.ARM9.ReadBytes(0x79F80, 4);
                    break;
                case gFamEnum.HGSS:
                default:
                    iconPalTableBuf = DSUtils.ARM9.ReadBytes(0x74408, 4);
                    break;
            }

            int iconPalTableAddress = (iconPalTableBuf[3] & 0xFF) << 24 | (iconPalTableBuf[2] & 0xFF) << 16 | (iconPalTableBuf[1] & 0xFF) << 8 | (iconPalTableBuf[0] & 0xFF) /* << 0 */;
            string iconTablePath;

            int iconPalTableOffsetFromFileStart;
            if (iconPalTableAddress >= RomInfo.synthOverlayLoadAddress) { // if the pointer shows the table was moved to the synthetic overlay
                iconPalTableOffsetFromFileStart = iconPalTableAddress - (int)RomInfo.synthOverlayLoadAddress;
                iconTablePath = gameDirs[DirNames.synthOverlay].unpackedDir + "\\" + ROMToolboxDialog.expandedARMfileID.ToString("D4");
            } else {
                iconPalTableOffsetFromFileStart = iconPalTableAddress - 0x02000000;
                iconTablePath = RomInfo.arm9Path;
            }
            
            using (DSUtils.EasyReader idReader = new DSUtils.EasyReader(iconTablePath, iconPalTableOffsetFromFileStart + species)) {
                paletteId = idReader.ReadByte();
            }

            if (paletteId != 0) {
                paletteBase.Palette[0] = paletteBase.Palette[paletteId]; // update pal 0 to be the new pal
            }

            // grab tiles
            int spriteFileID = species + 7;
            string spriteFilename = spriteFileID.ToString("D" + (fiveDigits ? "5" : "4"));
            imageBase = new NCGR(gameDirs[DirNames.monIcons].unpackedDir + "\\" + spriteFilename, spriteFileID, spriteFilename);

            // grab sprite
            int ncerFileId = 2;
            string ncerFileName = ncerFileId.ToString("D" + (fiveDigits ? "5" : "4"));
            spriteBase = new NCER(gameDirs[DirNames.monIcons].unpackedDir + "\\" + ncerFileName, 2, ncerFileName);

            // copy this from the trainer
            int bank0OAMcount = spriteBase.Banks[0].oams.Length;
            int[] OAMenabled = new int[bank0OAMcount];
            for (int i = 0; i < OAMenabled.Length; i++) {
                OAMenabled[i] = i;
            }

            // finally compose image
            try {
                return spriteBase.Get_Image(imageBase, paletteBase, 0, w, h, false, false, false, true, true, -1, OAMenabled);
            } catch (FormatException) {
                return Properties.Resources.IconPokeball;
            }
            // default:
            //partyPokemonPictureBoxList[partyPos].Image = cb.SelectedIndex > 0 ? (Image)Properties.PokePics.ResourceManager.GetObject(FixPokenameString(PokeDatabase.System.pokeNames[(ushort)cb.SelectedIndex])) : global::DSPRE.Properties.Resources.IconPokeball;
        }
        private void partyPokemon1ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            ShowPartyPokemonPic(0);
        }
        private void partyPokemon2ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            ShowPartyPokemonPic(1);
        }

        private void partyPokemon3ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            ShowPartyPokemonPic(2);
        }

        private void partyPokemon4ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            ShowPartyPokemonPic(3);
        }

        private void partyPokemon5ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            ShowPartyPokemonPic(4);
        }

        private void partyPokemon6ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            ShowPartyPokemonPic(5);
        }

        private void showTrainerEditorItemPic(byte partyPos) {
            ComboBox cb = partyItemsComboboxList[partyPos];
            partyPokemonItemIconList[partyPos].Visible = cb.SelectedIndex > 0;
        }

        private void partyItem1ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            showTrainerEditorItemPic(0);
        }

        private void partyItem2ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            showTrainerEditorItemPic(1);
        }

        private void partyItem3ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            showTrainerEditorItemPic(2);
        }

        private void partyItem4ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            showTrainerEditorItemPic(3);
        }

        private void partyItem5ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            showTrainerEditorItemPic(4);
        }

        private void partyItem6ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            showTrainerEditorItemPic(5);
        }

        private void DVExplainButton_Click(object sender, EventArgs e) {
            MessageBox.Show("DV, or \"Difficulty Value\", is used by the game engine to calculate how tough an opponent Pokemon should be.\n" +
                "The DV affects a Pokemon's Nature and IVs - the higher the value, the stronger the Pokemon.\n" +
                "DVs will go from 1 (0 IVs) to 255 (31 IVs). Natures are chosen semi-randomly." +
                "\nIVs will be the same value for all Stats at any DV, so Hidden Power will only be Fighting or Dark Type." +
                "\n\nFor the time being, DSPRE Reloaded is unable to calculate the target DV of a Pokémon for a given Nature and set of IVs.", "Difficulty Value", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void partyCountUpDown_ValueChanged(object sender, EventArgs e) {
            for (int i = 0; i < TrainerFile.POKE_IN_PARTY; i++) {
                partyGroupboxList[i].Enabled = (partyCountUpDown.Value > i);
                partyPokemonPictureBoxList[i].Visible = partyGroupboxList[i].Enabled;
            }
            for (int i = Math.Min(currentTrainerFile.trp.partyCount, (int)partyCountUpDown.Value); i < TrainerFile.POKE_IN_PARTY; i++) {
                currentTrainerFile.party[i] = new PartyPokemon(currentTrainerFile.trp.hasItems, currentTrainerFile.trp.hasMoves);
            }

            //if (!Helpers.disableHandlers) {
            //    RefreshTrainerPartyGUI();
            //    RefreshTrainerPropertiesGUI();
            //}
        }

        private void trainerMovesCheckBox_CheckedChanged(object sender, EventArgs e) {
            for (int i = 0; i < TrainerFile.POKE_IN_PARTY; i++) {
                for (int j = 0; j < Party.MOVES_PER_POKE; j++) {
                    (partyMovesGroupboxList[i].Controls[j] as ComboBox).Enabled = trainerMovesCheckBox.Checked;
                }
                currentTrainerFile.party[i].moves = trainerMovesCheckBox.Checked ? new ushort[Party.MOVES_PER_POKE] : null;
            }
        }
        private void trainerItemsCheckBox_CheckedChanged(object sender, EventArgs e) {
            for (int i = 0; i < TrainerFile.POKE_IN_PARTY; i++) {
                partyItemsComboboxList[i].Enabled = trainerItemsCheckBox.Checked;
            }
        }
        private void partyMoveComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (!Helpers.disableHandlers) {
                for (int i = 0; i < TrainerFile.POKE_IN_PARTY; i++) {
                    ushort[] moves = currentTrainerFile.party[i].moves;

                    if (moves != null) {
                        for (int j = 0; j < Party.MOVES_PER_POKE; j++) {
                            moves[j] = (ushort)(partyMovesGroupboxList[i].Controls[j] as ComboBox).SelectedIndex;
                        }
                    }
                }
            }
        }

        private void trainerSaveCurrentButton_Click(object sender, EventArgs e) {
            currentTrainerFile.trp.partyCount = (byte)partyCountUpDown.Value;
            currentTrainerFile.trp.hasMoves = trainerMovesCheckBox.Checked;
            currentTrainerFile.trp.hasItems = trainerItemsCheckBox.Checked;
            currentTrainerFile.trp.doubleBattle = trainerDoubleCheckBox.Checked;

            IList trainerItems = trainerItemsGroupBox.Controls;
            for (int i = 0; i < trainerItems.Count; i++) {
                currentTrainerFile.trp.trainerItems[i] = (ushort)(trainerItems[i] as ComboBox).SelectedIndex;
            }

            IList trainerAI = TrainerAIGroupBox.Controls;
            for (int i = 0; i < trainerAI.Count; i++) {
                currentTrainerFile.trp.AI[i] = (trainerAI[i] as CheckBox).Checked;
            }

            for (int i = 0; i < TrainerFile.POKE_IN_PARTY; i++) {
                currentTrainerFile.party[i].moves = trainerMovesCheckBox.Checked ? new ushort[4] : null;
            }

            for (int i = 0; i < partyCountUpDown.Value; i++) {
                currentTrainerFile.party[i].pokeID = (ushort)partyPokemonComboboxList[i].SelectedIndex;
                currentTrainerFile.party[i].level = (ushort)partyLevelUpdownList[i].Value;

                if (trainerMovesCheckBox.Checked) {
                    IList movesList = partyMovesGroupboxList[i].Controls;
                    for (int j = 0; j < Party.MOVES_PER_POKE; j++) {
                        currentTrainerFile.party[i].moves[j] = (ushort)(movesList[j] as ComboBox).SelectedIndex;
                    }
                }

                if (trainerItemsCheckBox.Checked) {
                    currentTrainerFile.party[i].heldItem = (ushort)partyItemsComboboxList[i].SelectedIndex;
                }

                currentTrainerFile.party[i].unknown1_DATASTART = (ushort)partyIVUpdownList[i].Value;
                currentTrainerFile.party[i].unknown2_DATAEND = (ushort)partyBallUpdownList[i].Value;
            }

            /*Write to File*/
            string indexStr = "\\" + trainerComboBox.SelectedIndex.ToString("D4");
            File.WriteAllBytes(RomInfo.gameDirs[DirNames.trainerProperties].unpackedDir + indexStr, currentTrainerFile.trp.ToByteArray());
            File.WriteAllBytes(RomInfo.gameDirs[DirNames.trainerParty].unpackedDir + indexStr, currentTrainerFile.party.ToByteArray());

            UpdateCurrentTrainerName(newName: trainerNameTextBox.Text);
            UpdateCurrentTrainerShownName();

            MessageBox.Show("Trainer saved successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateCurrentTrainerShownName() {
            string trClass = GetTrainerClassNameFromListbox(trainerClassListBox.SelectedItem);

            string editedTrainer = "[" + currentTrainerFile.trp.trainerID.ToString("D2") + "] " + trClass + " " + currentTrainerFile.name;

            Helpers.disableHandlers = true;
            trainerComboBox.Items[trainerComboBox.SelectedIndex] = editedTrainer;
            Helpers.disableHandlers = false;

            if (eventEditor.eventEditorIsReady) {
                eventEditor.owTrainerComboBox.Items[trainerComboBox.SelectedIndex] = editedTrainer;
            }
        }

        private string GetTrainerClassNameFromListbox(object selectedItem) {
            string lbname = selectedItem.ToString();
            return lbname.Substring(lbname.IndexOf(" ") + 1);
        }

        private void UpdateCurrentTrainerName(string newName) {
            currentTrainerFile.name = newName;
            TextArchive trainerNames = new TextArchive(RomInfo.trainerNamesMessageNumber);
            if (currentTrainerFile.trp.trainerID < trainerNames.messages.Count) {
                trainerNames.messages[currentTrainerFile.trp.trainerID] = newName;
            } else {
                trainerNames.messages.Add(newName);
            }
            trainerNames.SaveToFileDefaultDir(RomInfo.trainerNamesMessageNumber, showSuccessMessage: false);
        }
        private void UpdateCurrentTrainerClassName(string newName) {             
            TextArchive trainerClassNames = new TextArchive(RomInfo.trainerClassMessageNumber);
            trainerClassNames.messages[trainerClassListBox.SelectedIndex] = newName;
            trainerClassNames.SaveToFileDefaultDir(RomInfo.trainerClassMessageNumber, showSuccessMessage: false);
        }

        private void trainerClassListBox_SelectedIndexChanged(object sender, EventArgs e) {
            int selection = trainerClassListBox.SelectedIndex;
            if (selection < 0) {
                return;
            }

            try {
                int maxFrames = LoadTrainerClassPic(selection);
                UpdateTrainerClassPic(trainerClassPicBox);

                trClassFramePreviewUpDown.Maximum = maxFrames;
                trainerClassFrameMaxLabel.Text = "/" + maxFrames;
            } catch {
                trClassFramePreviewUpDown.Maximum = 0;
            }

            trainerClassNameTextbox.Text = GetTrainerClassNameFromListbox(trainerClassListBox.SelectedItem);

            if (trainerClassEncounterMusicDict.TryGetValue((byte)selection, out (uint entryOffset, ushort musicD, ushort? musicN) output)) {
                encounterSSEQMainUpDown.Enabled = eyeContactMusicLabel.Enabled = true;
                encounterSSEQMainUpDown.Value = output.musicD;
            } else {
                encounterSSEQMainUpDown.Enabled = eyeContactMusicLabel.Enabled = false;
                encounterSSEQMainUpDown.Value = 0;
            }

            eyeContactMusicAltLabel.Enabled = encounterSSEQAltUpDown.Enabled = (encounterSSEQMainUpDown.Enabled && gameFamily == gFamEnum.HGSS);
            encounterSSEQAltUpDown.Value = output.musicN != null ? (ushort)output.musicN : 0;
            currentTrainerFile.trp.trainerClass = (byte)selection;
        }

        private int LoadTrainerClassPic(int trClassID) {
            int paletteFileID = (trClassID * 5 + 1);
            string paletteFilename = paletteFileID.ToString("D4");
            trainerPal = new NCLR(gameDirs[DirNames.trainerGraphics].unpackedDir + "\\" + paletteFilename, paletteFileID, paletteFilename);

            int tilesFileID = trClassID * 5;
            string tilesFilename = tilesFileID.ToString("D4");
            trainerTile = new NCGR(gameDirs[DirNames.trainerGraphics].unpackedDir + "\\" + tilesFilename, tilesFileID, tilesFilename);

            if (gameFamily == gFamEnum.DP) {
                return 0;
            }
            
            int spriteFileID = (trClassID * 5 + 2);
            string spriteFilename = spriteFileID.ToString("D4");
            trainerSprite = new NCER(gameDirs[DirNames.trainerGraphics].unpackedDir + "\\" + spriteFilename, spriteFileID, spriteFilename);

            return trainerSprite.Banks.Length - 1;
        }
        private void UpdateTrainerClassPic(PictureBox pb, int frameNumber = 0) {
            if (trainerSprite == null) {
                Console.WriteLine("Sprite is null!");
                return;
            }

            int bank0OAMcount = trainerSprite.Banks[0].oams.Length;
            int[] OAMenabled = new int[bank0OAMcount];
            for (int i = 0; i < OAMenabled.Length; i++) {
                OAMenabled[i] = i;
            }

            frameNumber = Math.Min(trainerSprite.Banks.Length, frameNumber);
            Image trSprite = trainerSprite.Get_Image(trainerTile, trainerPal, frameNumber, trainerClassPicBox.Width, trainerClassPicBox.Height, false, false, false, true, true, -1, OAMenabled);
            pb.Image = trSprite;
            pb.Update();
        }

        private void addTrainerButton_Click(object sender, EventArgs e) {
            /* Add new trainer file to 2 folders */
            string suffix = "\\" + trainerComboBox.Items.Count.ToString("D4");

            string trainerPropertiesPath = gameDirs[DirNames.trainerProperties].unpackedDir + suffix;
            string partyFilePath = gameDirs[DirNames.trainerParty].unpackedDir + suffix;

            File.WriteAllBytes(trainerPropertiesPath, new TrainerProperties((ushort)trainerComboBox.Items.Count).ToByteArray());
            File.WriteAllBytes(partyFilePath, new PartyPokemon().ToByteArray());

            TextArchive trainerClasses = new TextArchive(RomInfo.trainerClassMessageNumber);
            TextArchive trainerNames = new TextArchive(RomInfo.trainerNamesMessageNumber);

            /* Update ComboBox and select new file */
            trainerComboBox.Items.Add(trainerClasses.messages[0]);
            trainerNames.messages.Add("");
            trainerNames.SaveToFileDefaultDir(RomInfo.trainerNamesMessageNumber, showSuccessMessage: false);

            trainerComboBox.SelectedIndex = trainerComboBox.Items.Count - 1;
            UpdateCurrentTrainerShownName();
        }

        private void exportTrainerButton_Click(object sender, EventArgs e) {
            currentTrainerFile.SaveToFileExplorePath("G4 Trainer File " + trainerComboBox.SelectedItem);
        }

        private void importTrainerButton_Click(object sender, EventArgs e) {
            OpenFileDialog of = new OpenFileDialog {
                Filter = "Gen IV Trainer File (*.trf)|*.trf"
            };
            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            /* Update trainer on disk */
            using (DSUtils.EasyReader reader = new DSUtils.EasyReader(of.FileName)) {
                string trName = reader.ReadString();

                byte datSize = reader.ReadByte();
                byte[] trDat = reader.ReadBytes(datSize);

                byte partySize = reader.ReadByte();
                byte[] pDat = reader.ReadBytes(partySize);

                string pathData = RomInfo.gameDirs[DirNames.trainerProperties].unpackedDir + "\\" + trainerComboBox.SelectedIndex.ToString("D4");
                string pathParty = RomInfo.gameDirs[DirNames.trainerParty].unpackedDir + "\\" + trainerComboBox.SelectedIndex.ToString("D4");
                File.WriteAllBytes(pathData, trDat);
                File.WriteAllBytes(pathParty, pDat);

                UpdateCurrentTrainerName(trName);
            }
            /* Refresh controls and re-read file */
            trainerComboBox_SelectedIndexChanged(null, null);
            UpdateCurrentTrainerShownName();

            /* Display success message */
            MessageBox.Show("Trainer File imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void exportPropertiesButton_Click(object sender, EventArgs e) {
            currentTrainerFile.trp.SaveToFileExplorePath("G4 Trainer Properties " + trainerComboBox.SelectedItem);
        }

        private void importReplacePropertiesButton_Click(object sender, EventArgs e) {
            OpenFileDialog of = new OpenFileDialog {
                Filter = "Gen IV Trainer Properties (*.trp)|*.trp"
            };
            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            /* Update trp object in memory */
            currentTrainerFile.trp = new TrainerProperties((ushort)trainerComboBox.SelectedIndex, new FileStream(of.FileName, FileMode.Open));
            RefreshTrainerPropertiesGUI();

            /* Display success message */
            MessageBox.Show("Trainer Properties imported successfully!\nRemember to save the current Trainer File.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void exportPartyButton_Click(object sender, EventArgs e) {
            currentTrainerFile.party.exportCondensedData = true;
            currentTrainerFile.party.SaveToFileExplorePath("G4 Party Data " + trainerComboBox.SelectedItem);
            currentTrainerFile.party.exportCondensedData = false;
        }

        private void importReplacePartyButton_Click(object sender, EventArgs e) {
            OpenFileDialog of = new OpenFileDialog {
                Filter = "Gen IV Party File (*.pdat)|*.pdat"
            };
            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            /* Update trp object in memory */
            currentTrainerFile.party = new Party(readFirstByte: true, TrainerFile.POKE_IN_PARTY, new FileStream(of.FileName, FileMode.Open), currentTrainerFile.trp);
            RefreshTrainerPropertiesGUI();
            RefreshTrainerPartyGUI();

            /* Display success message */
            MessageBox.Show("Trainer Party imported successfully!\nRemember to save the current Trainer File.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void saveTrainerClassButton_Click(object sender, EventArgs e) {
            Helpers.disableHandlers = true;

            int selectedTrClass = trainerClassListBox.SelectedIndex;

            byte b_selectedTrClass = (byte)selectedTrClass;
            ushort eyeMusicID = (ushort)encounterSSEQMainUpDown.Value;

            if (trainerClassEncounterMusicDict.TryGetValue(b_selectedTrClass, out var dictEntry)) {
                DSUtils.ARM9.WriteBytes(BitConverter.GetBytes(eyeMusicID), dictEntry.entryOffset);
                trainerClassEncounterMusicDict[b_selectedTrClass] = (dictEntry.entryOffset, eyeMusicID, dictEntry.musicN);
            }

            string newName = trainerClassNameTextbox.Text;
            UpdateCurrentTrainerClassName(newName);
            trainerClassListBox.Items[selectedTrClass] = "[" + selectedTrClass.ToString("D3") + "]" + " " + newName;

            if (currentTrainerFile.trp.trainerClass == trainerClassListBox.SelectedIndex) { 
                UpdateCurrentTrainerShownName();
            }
            Helpers.disableHandlers = false;

            //trainerClassListBox_SelectedIndexChanged(null, null);
            if ( gameFamily.Equals(gFamEnum.HGSS) && tableEditorIsReady ) { 
                pbEffectsTrainerCombobox.Items[selectedTrClass] = trainerClassListBox.Items[selectedTrClass];
                for (int i = 0; i < vsTrainerEffectsList.Count; i++) {
                    if (vsTrainerEffectsList[i].trainerClass == selectedTrClass) {
                        pbEffectsVsTrainerListbox.Items[i] = pbEffectsTrainerCombobox.Items[selectedTrClass] + " uses Combo #" + vsTrainerEffectsList[i].comboID;
                    }
                }
            }
            MessageBox.Show("Trainer Class settings saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void trClassFramePreviewUpDown_ValueChanged(object sender, EventArgs e) {
            UpdateTrainerClassPic(trainerClassPicBox, (int)((NumericUpDown)sender).Value);
        }
        #endregion

        #region Table Editor
        #region Variables

        string[] pokeNames;
        string[] trcNames;

        List<(ushort header, ushort flag, ushort music)> conditionalMusicTable;
        uint conditionalMusicTableStartAddress;

        List<(int trainerClass, int comboID)> vsTrainerEffectsList;
        List<(int pokemonID, int comboID)> vsPokemonEffectsList;
        List<(ushort vsGraph, ushort battleSSEQ)> effectsComboTable;

        uint vsTrainerTableStartAddress;
        uint vsPokemonTableStartAddress;
        uint effectsComboMainTableStartAddress;

        //Show Pokemon Icons
        private readonly PaletteBase tableEditorMonIconPal;
        private readonly ImageBase tableEditorMonIconTile;
        private readonly SpriteBase tableEditorMonIconSprite;
        #endregion

        private void SetupConditionalMusicTable() {
            switch (RomInfo.gameFamily) {
                case gFamEnum.HGSS:
                    RomInfo.SetConditionalMusicTableOffsetToRAMAddress();
                    conditionalMusicTable = new List<(ushort, ushort, ushort)>();

                    conditionalMusicTableStartAddress = BitConverter.ToUInt32(DSUtils.ARM9.ReadBytes(RomInfo.conditionalMusicTableOffsetToRAMAddress, 4), 0) - 0x02000000;
                    byte tableEntriesCount = DSUtils.ARM9.ReadByte(RomInfo.conditionalMusicTableOffsetToRAMAddress - 8);

                    conditionalMusicTableListBox.Items.Clear();
                    using (DSUtils.ARM9.Reader ar = new DSUtils.ARM9.Reader(conditionalMusicTableStartAddress)) {
                        for (int i = 0; i < tableEntriesCount; i++) {
                            ushort header = ar.ReadUInt16();
                            ushort flag = ar.ReadUInt16();
                            ushort musicID = ar.ReadUInt16();

                            conditionalMusicTable.Add((header, flag, musicID));
                            conditionalMusicTableListBox.Items.Add(headerListBox.Items[header]);
                        }
                    }

                    headerConditionalMusicComboBox.Items.Clear();
                    foreach (string location in headerListBox.Items) {
                        headerConditionalMusicComboBox.Items.Add(location);
                    }

                    if (conditionalMusicTableListBox.Items.Count > 0) {
                        conditionalMusicTableListBox.SelectedIndex = 0;
                    }
                    break;

                case gFamEnum.Plat:
                    pbEffectsMonGroupBox.Enabled = false;
                    pbEffectsTrainerGroupBox.Enabled = false;
                    break;

                default:
                    pbEffectsGroupBox.Enabled = false;
                    pbEffectsMonGroupBox.Enabled = false;
                    pbEffectsTrainerGroupBox.Enabled = false;
                    conditionalMusicGroupBox.Enabled = false;
                    break;
            }
        }
        private void SetupBattleEffectsTables() {
            if (RomInfo.gameFamily == gFamEnum.HGSS || RomInfo.gameFamily == gFamEnum.Plat) {
                DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.trainerGraphics, DirNames.textArchives });
                RomInfo.SetBattleEffectsData();

                effectsComboTable = new List<(ushort vsGraph, ushort battleSSEQ)>();
                
                effectsComboMainTableStartAddress = BitConverter.ToUInt32(DSUtils.ARM9.ReadBytes(RomInfo.effectsComboTableOffsetToRAMAddress, 4), 0);
                ROMToolboxDialog.flag_MainComboTableRepointed = (effectsComboMainTableStartAddress >= RomInfo.synthOverlayLoadAddress);
                effectsComboMainTableStartAddress -= ROMToolboxDialog.flag_MainComboTableRepointed ? RomInfo.synthOverlayLoadAddress : DSUtils.ARM9.address;

                byte comboTableEntriesCount;

                if (RomInfo.gameFamily == gFamEnum.HGSS) {
                    comboTableEntriesCount = DSUtils.ARM9.ReadByte(RomInfo.effectsComboTableOffsetToSizeLimiter);

                    vsPokemonEffectsList = new List<(int pokemonID, int comboID)>();
                    vsTrainerEffectsList = new List<(int trainerClass, int comboID)>();

                    vsPokemonTableStartAddress = BitConverter.ToUInt32(DSUtils.ARM9.ReadBytes(RomInfo.vsPokemonEntryTableOffsetToRAMAddress, 4), 0);
                    ROMToolboxDialog.flag_PokemonBattleTableRepointed = (vsPokemonTableStartAddress >= RomInfo.synthOverlayLoadAddress);
                    vsPokemonTableStartAddress -= ROMToolboxDialog.flag_PokemonBattleTableRepointed ? RomInfo.synthOverlayLoadAddress : DSUtils.ARM9.address;

                    vsTrainerTableStartAddress = BitConverter.ToUInt32(DSUtils.ARM9.ReadBytes(RomInfo.vsTrainerEntryTableOffsetToRAMAddress, 4), 0);
                    ROMToolboxDialog.flag_TrainerClassBattleTableRepointed = (vsTrainerTableStartAddress >= RomInfo.synthOverlayLoadAddress);
                    vsTrainerTableStartAddress -= ROMToolboxDialog.flag_TrainerClassBattleTableRepointed ? RomInfo.synthOverlayLoadAddress : DSUtils.ARM9.address;


                    pbEffectsPokemonCombobox.Items.Clear();
                    pokeNames = RomInfo.GetPokemonNames();
                    for (int i = 0; i < pokeNames.Length; i++) {
                        pbEffectsPokemonCombobox.Items.Add("[" + i + "]" + " " + pokeNames[i]);
                    }

                    RepopulateTableEditorTrainerClasses();

                    pbEffectsVsTrainerListbox.Items.Clear();
                    pbEffectsVsPokemonListbox.Items.Clear();
                } else {
                    comboTableEntriesCount = 35;
                }

                pbEffectsCombosListbox.Items.Clear();

                String expArmPath = RomInfo.gameDirs[DirNames.synthOverlay].unpackedDir + '\\' + ROMToolboxDialog.expandedARMfileID.ToString("D4");

                if (RomInfo.gameFamily == gFamEnum.HGSS) {
                    using (DSUtils.EasyReader ar = new DSUtils.EasyReader(ROMToolboxDialog.flag_TrainerClassBattleTableRepointed ? expArmPath : RomInfo.arm9Path, vsTrainerTableStartAddress)) {
                        byte trainerTableEntriesCount = DSUtils.ARM9.ReadByte(RomInfo.vsTrainerEntryTableOffsetToSizeLimiter);

                        for (int i = 0; i < trainerTableEntriesCount; i++) {
                            ushort entry = ar.ReadUInt16();
                            int classID = entry & 1023;
                            int comboID = entry >> 10;
                            vsTrainerEffectsList.Add((classID, comboID));
                            pbEffectsVsTrainerListbox.Items.Add(pbEffectsTrainerCombobox.Items[classID] + " uses Combo #" + comboID);
                        }
                    }

                    using (DSUtils.EasyReader ar = new DSUtils.EasyReader(ROMToolboxDialog.flag_PokemonBattleTableRepointed ? expArmPath : RomInfo.arm9Path, vsPokemonTableStartAddress)) {
                        byte pokemonTableEntriesCount = DSUtils.ARM9.ReadByte(RomInfo.vsPokemonEntryTableOffsetToSizeLimiter);

                        for (int i = 0; i < pokemonTableEntriesCount; i++) {
                            ushort entry = ar.ReadUInt16();
                            int pokeID = entry & 1023;
                            int comboID = entry >> 10;
                            vsPokemonEffectsList.Add((pokeID, comboID));

                            string pokeName;
                            try {
                                pokeName = pokeNames[pokeID];
                            } catch (IndexOutOfRangeException) {
                                pokeName = "UNKNOWN";
                            }
                            pbEffectsVsPokemonListbox.Items.Add("[" + pokeID.ToString("D3") + "]" + " " + pokeName + " uses Combo #" + comboID);
                        }
                    }
                }

                using (DSUtils.EasyReader ar = new DSUtils.EasyReader(ROMToolboxDialog.flag_MainComboTableRepointed ? expArmPath : RomInfo.arm9Path, effectsComboMainTableStartAddress)) {
                    for (int i = 0; i < comboTableEntriesCount; i++) {
                        ushort battleIntroEffect = ar.ReadUInt16();
                        ushort battleMusic = ar.ReadUInt16();
                        effectsComboTable.Add((battleIntroEffect, battleMusic));
                        pbEffectsCombosListbox.Items.Add("Combo " + i.ToString("D2") + " - " + "Effect #" + battleIntroEffect + ", " + "Music #" + battleMusic);
                    }
                }

                if (RomInfo.gameFamily == gFamEnum.HGSS) {
                    var items = pbEffectsCombosListbox.Items.Cast<Object>().ToArray();

                    pbEffectsPokemonChooseMainCombobox.Items.Clear();
                    pbEffectsPokemonChooseMainCombobox.Items.AddRange(items);
                    pbEffectsTrainerChooseMainCombobox.Items.Clear();
                    pbEffectsTrainerChooseMainCombobox.Items.AddRange(items);

                    if (pbEffectsVsTrainerListbox.Items.Count > 0) {
                        pbEffectsVsTrainerListbox.SelectedIndex = 0;
                    }
                    if (pbEffectsVsPokemonListbox.Items.Count > 0) {
                        pbEffectsVsPokemonListbox.SelectedIndex = 0;
                    }
                }

                if (pbEffectsCombosListbox.Items.Count > 0) {
                    pbEffectsCombosListbox.SelectedIndex = 0;
                }
                
            } else {
                pbEffectsGroupBox.Enabled = false;
            }
        }

        private void RepopulateTableEditorTrainerClasses() {
            pbEffectsTrainerCombobox.Items.Clear();
            trcNames = RomInfo.GetTrainerClassNames();
            for (int i = 0; i < trcNames.Length; i++) {
                pbEffectsTrainerCombobox.Items.Add("[" + i.ToString("D3") + "]" + " " + trcNames[i]);
            }
        }

        private void conditionalMusicTableListBox_SelectedIndexChanged(object sender, EventArgs e) {
            int selection = conditionalMusicTableListBox.SelectedIndex;
            headerConditionalMusicComboBox.SelectedIndex = conditionalMusicTable[selection].header;

            Helpers.disableHandlers = true;

            flagConditionalMusicUpDown.Value = conditionalMusicTable[selection].flag;
            musicIDconditionalMusicUpDown.Value = conditionalMusicTable[selection].music;
            
            Helpers.disableHandlers = false;
        }
        private void headerConditionalMusicComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }

            (ushort header, ushort flag, ushort music) oldTuple = conditionalMusicTable[conditionalMusicTableListBox.SelectedIndex];
            (ushort header, ushort flag, ushort music) newTuple = ((ushort)headerConditionalMusicComboBox.SelectedIndex, oldTuple.flag, oldTuple.music);
            conditionalMusicTable[conditionalMusicTableListBox.SelectedIndex] = newTuple;

            MapHeader selected = MapHeader.LoadFromARM9(newTuple.header);
            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                    locationNameConditionalMusicLBL.Text = RomInfo.GetLocationNames()[(selected as HeaderDP).locationName];
                    break;
                case gFamEnum.Plat:
                    locationNameConditionalMusicLBL.Text = RomInfo.GetLocationNames()[(selected as HeaderPt).locationName];
                    break;
                case gFamEnum.HGSS:
                    locationNameConditionalMusicLBL.Text = RomInfo.GetLocationNames()[(selected as HeaderHGSS).locationName];
                    break;
            }
        }
        private void flagConditionalMusicUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }

            (ushort header, ushort flag, ushort music) oldTuple = conditionalMusicTable[conditionalMusicTableListBox.SelectedIndex];
            conditionalMusicTable[conditionalMusicTableListBox.SelectedIndex] = (oldTuple.header, (ushort)flagConditionalMusicUpDown.Value, oldTuple.music);
        }

        private void musicIDconditionalMusicUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.disableHandlers) {
                return;
            }

            (ushort header, ushort flag, ushort music) oldTuple = conditionalMusicTable[conditionalMusicTableListBox.SelectedIndex];
            conditionalMusicTable[conditionalMusicTableListBox.SelectedIndex] = (oldTuple.header, oldTuple.flag, (ushort)musicIDconditionalMusicUpDown.Value);
        }
        private void HOWconditionalMusicTableButton_Click(object sender, EventArgs e) {
            MessageBox.Show("For each Location in the list, override Header's music with chosen Music ID, if Flag is set.", "How this table works", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void saveConditionalMusicTableBTN_Click(object sender, EventArgs e) {
            for (int i = 0; i < conditionalMusicTable.Count; i++) {
                DSUtils.ARM9.WriteBytes(BitConverter.GetBytes(conditionalMusicTable[i].header), (uint)(conditionalMusicTableStartAddress + 6 * i));
                DSUtils.ARM9.WriteBytes(BitConverter.GetBytes(conditionalMusicTable[i].flag), (uint)(conditionalMusicTableStartAddress + 6 * i + 2));
                DSUtils.ARM9.WriteBytes(BitConverter.GetBytes(conditionalMusicTable[i].music), (uint)(conditionalMusicTableStartAddress + 6 * i + 4));
            }
        }

        private void TBLEditortrainerClassPreviewPic_ValueChanged(object sender, EventArgs e) {
            UpdateTrainerClassPic(tbEditorTrClassPictureBox, (int)((NumericUpDown)sender).Value);
        }

        private void saveEffectComboBTN_Click(object sender, EventArgs e) {
            int index = pbEffectsCombosListbox.SelectedIndex;
            ushort battleIntroEffect = (ushort)pbEffectsVSAnimationUpDown.Value;
            ushort battleMusic = (ushort)pbEffectsBattleSSEQUpDown.Value;

            effectsComboTable[index] = (battleIntroEffect, battleMusic);

            String expArmPath = RomInfo.gameDirs[DirNames.synthOverlay].unpackedDir + '\\' + ROMToolboxDialog.expandedARMfileID.ToString("D4");
            using (DSUtils.EasyWriter wr = new DSUtils.EasyWriter(ROMToolboxDialog.flag_MainComboTableRepointed ? expArmPath : RomInfo.arm9Path, effectsComboMainTableStartAddress + 4 * index)) {
                wr.Write(battleIntroEffect);
                wr.Write(battleMusic);
            };

            Helpers.disableHandlers = true;
            pbEffectsCombosListbox.Items[index] = pbEffectsTrainerChooseMainCombobox.Items[index] = pbEffectsPokemonChooseMainCombobox.Items[index] = "Combo " + index.ToString("D2") + " - " + "Effect #" + battleIntroEffect + ", " + "Music #" + battleMusic;
            Helpers.disableHandlers = false;
        }

        private void saveVSPokemonEntryBTN_Click(object sender, EventArgs e) {
            int index = pbEffectsVsPokemonListbox.SelectedIndex;
            ushort pokemonID = (ushort)pbEffectsPokemonCombobox.SelectedIndex;
            ushort comboID = (ushort)pbEffectsPokemonChooseMainCombobox.SelectedIndex;

            vsPokemonEffectsList[index] = (pokemonID, comboID);

            String expArmPath = RomInfo.gameDirs[DirNames.synthOverlay].unpackedDir + '\\' + ROMToolboxDialog.expandedARMfileID.ToString("D4");
            using (DSUtils.EasyWriter wr = new DSUtils.EasyWriter(ROMToolboxDialog.flag_PokemonBattleTableRepointed ? expArmPath : RomInfo.arm9Path, vsPokemonTableStartAddress + 2 * index)) {
                wr.Write((ushort)((pokemonID & 1023) + (comboID << 10))); //PokemonID
            };

            Helpers.disableHandlers = true;
            pbEffectsVsPokemonListbox.Items[index] = "[" + pokemonID.ToString("D3") + "]" + " " + pokeNames[pokemonID] + " uses Combo #" + comboID;
            Helpers.disableHandlers = false;
        }

        private void saveVSTrainerEntryBTN_Click(object sender, EventArgs e) {
            int index = pbEffectsVsTrainerListbox.SelectedIndex;
            ushort trainerClass = (ushort)pbEffectsTrainerCombobox.SelectedIndex;
            ushort comboID = (ushort)pbEffectsTrainerChooseMainCombobox.SelectedIndex;

            vsTrainerEffectsList[index] = (trainerClass, comboID);
            String expArmPath = RomInfo.gameDirs[DirNames.synthOverlay].unpackedDir + '\\' + ROMToolboxDialog.expandedARMfileID.ToString("D4");
            using (DSUtils.EasyWriter wr = new DSUtils.EasyWriter(ROMToolboxDialog.flag_TrainerClassBattleTableRepointed ? expArmPath : RomInfo.arm9Path, vsTrainerTableStartAddress + 2 * index)) { 
                wr.Write((ushort)((trainerClass & 1023) + (comboID << 10))); 
            };

            Helpers.disableHandlers = true;
            pbEffectsVsTrainerListbox.Items[index] = "[" + trainerClass.ToString("D3") + "]" + " " + trcNames[trainerClass] + " uses Combo #" + comboID;
            Helpers.disableHandlers = false;
        }

        private void HOWpbEffectsTableButton_Click(object sender, EventArgs e) {
            MessageBox.Show("An entry of this table is a combination of VS. Graphics + Battle Theme.\n\n" +
                (RomInfo.gameFamily.Equals(gFamEnum.HGSS) ? "Each entry can be \"inherited\" by one or more Pokémon or Trainer classes." : ""), 
                "How this table works", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void HOWvsPokemonButton_Click(object sender, EventArgs e) {
            MessageBox.Show("Each entry of this table links a \"Wild\" Pokémon to an Effect Combo from the Combos Table.\n\n" +
                "Whenever that Pokémon is encountered in the tall grass or via script command, its VS. Sequence and Battle Theme will be automatically triggered.",
                 "How this table works", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private void HOWVsTrainerButton_Click(object sender, EventArgs e) {
            MessageBox.Show("Each entry of this table links a Trainer Class to an Effect Combo from the Combos Table.\n\n" +
                "Every Trainer Class with a given combo will start the same VS. Sequence and Battle Theme.", "How this table works", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pbEffectsVsTrainerListbox_SelectedIndexChanged(object sender, EventArgs e) {
            int trainerSelection = pbEffectsVsTrainerListbox.SelectedIndex;
            if (Helpers.disableHandlers || trainerSelection < 0) {
                return;
            }

            (int trainerClass, int comboID) entry = vsTrainerEffectsList[trainerSelection];
            pbEffectsTrainerCombobox.SelectedIndex = entry.trainerClass;
            pbEffectsCombosListbox.SelectedIndex = pbEffectsTrainerChooseMainCombobox.SelectedIndex = entry.comboID;

            tbEditorTrClassFramePreviewUpDown.Value = 0;
        }

        private void pbEffectsVsPokemonListbox_SelectedIndexChanged(object sender, EventArgs e) {
            int pokemonSelection = pbEffectsVsPokemonListbox.SelectedIndex;

            if (Helpers.disableHandlers || pokemonSelection < 0) {
                return;
            }

            (int pokemonID, int comboID) entry = vsPokemonEffectsList[pokemonSelection];

            try {
                pbEffectsPokemonCombobox.SelectedIndex = entry.pokemonID;
            } catch (ArgumentOutOfRangeException) {
                pbEffectsPokemonCombobox.SelectedIndex = 0;
            }
            pbEffectsCombosListbox.SelectedIndex = pbEffectsPokemonChooseMainCombobox.SelectedIndex = entry.comboID;
        }

        private void pbEffectsCombosListbox_SelectedIndexChanged(object sender, EventArgs e) {
            int comboSelection = pbEffectsCombosListbox.SelectedIndex;

            if (Helpers.disableHandlers || comboSelection < 0) {
                return;
            }

            (ushort vsGraph, ushort battleSSEQ) entry = effectsComboTable[comboSelection];
            pbEffectsBattleSSEQUpDown.Value = entry.battleSSEQ;
            pbEffectsVSAnimationUpDown.Value = entry.vsGraph;
        }

        private void pbEffectsTrainerCombobox_SelectedIndexChanged(object sender, EventArgs e) {
            int maxFrames = LoadTrainerClassPic((sender as ComboBox).SelectedIndex);
            UpdateTrainerClassPic(tbEditorTrClassPictureBox);

            tbEditorTrClassFramePreviewUpDown.Maximum = maxFrames;
            tbEditortrainerClassFrameMaxLabel.Text = "/" + maxFrames;
        }
        private void pbEffectsPokemonCombobox_SelectedIndexChanged(object sender, EventArgs e) {
            ComboBox cb = sender as ComboBox;
            tbEditorPokeminiPictureBox.Image = GetPokePic(cb.SelectedIndex, tbEditorPokeminiPictureBox.Width, tbEditorPokeminiPictureBox.Height, tableEditorMonIconPal, tableEditorMonIconTile, tableEditorMonIconSprite);
            tbEditorPokeminiPictureBox.Update();
        }

        #endregion
        private void ExclusiveCBInvert(CheckBox cb) {
            if (Helpers.disableHandlers) {
                return;
            }

            Helpers.disableHandlers = true;

            if (cb.Checked) {
                cb.Checked = !cb.Checked;
            }

            Helpers.disableHandlers = false;
        }

        private void unpackToFolderToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog of = new OpenFileDialog {
                Filter = "NARC File (*.narc)|*.narc"
            };
            if (of.ShowDialog(this) != DialogResult.OK) {
                MessageBox.Show("Operation cancelled.", "User discarded operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Narc userfile = Narc.Open(of.FileName);
            if (userfile is null) {
                MessageBox.Show("The file you selected is not a valid NARC.", "Cannot proceed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            MessageBox.Show("Choose where to save the NARC content.\nDSPRE will automatically make a subdirectory.", "Choose destination path", MessageBoxButtons.OK, MessageBoxIcon.Information);

            CommonOpenFileDialog narcDir = new CommonOpenFileDialog {
                IsFolderPicker = true,
                Multiselect = false
            };

            if (narcDir.ShowDialog() != CommonFileDialogResult.Ok) {
                MessageBox.Show("Operation cancelled.", "User discarded operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string finalExtractedPath = narcDir.FileName + "\\" + Path.GetFileNameWithoutExtension(of.FileName);
            userfile.ExtractToFolder(finalExtractedPath);
            MessageBox.Show("The contents of " + of.FileName + " have been extracted and saved.", "NARC Extracted", MessageBoxButtons.OK, MessageBoxIcon.Information);

            DialogResult d = MessageBox.Show("Do you want to rename the files according to their contents?", "Waiting for user", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (d.Equals(DialogResult.Yes)) {
                ContentBasedBatchRename(new DirectoryInfo(finalExtractedPath));
            }
        }

        private void buildFromFolderToolStripMenuItem_Click(object sender, EventArgs e) {
            CommonOpenFileDialog narcDir = new CommonOpenFileDialog {
                IsFolderPicker = true,
                Multiselect = false
            };

            if (narcDir.ShowDialog() != CommonFileDialogResult.Ok) {
                MessageBox.Show("Operation cancelled.", "User discarded operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            MessageBox.Show("Choose where to save the output NARC file.", "Name your NARC file", MessageBoxButtons.OK, MessageBoxIcon.Information);

            SaveFileDialog sf = new SaveFileDialog {
                Filter = "NARC File (*.narc)|*.narc",
                FileName = Path.GetFileName(narcDir.FileName)
            };
            if (sf.ShowDialog(this) != DialogResult.OK) {
                MessageBox.Show("Operation cancelled.", "User discarded operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Narc.FromFolder(narcDir.FileName).Save(sf.FileName);
            MessageBox.Show("The contents of folder \"" + narcDir.FileName + "\" have been packed.", "NARC Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void listBasedToolStripMenuItem_Click(object sender, EventArgs e) {
            (DirectoryInfo d, FileInfo[] files) dirData = OpenNonEmptyDir(title: "List-Based Batch Rename Tool");
            DirectoryInfo d = dirData.d;
            FileInfo[] files = dirData.files;

            if (d == null || files == null) {
                return;
            }

            /*==================================================================*/

            MessageBox.Show("Choose your enumeration text file.", "Input list file", MessageBoxButtons.OK, MessageBoxIcon.Information);
            OpenFileDialog of = new OpenFileDialog {
                Filter = "List File (*.txt; *.list)|*.txt;*.list"
            };

            if (of.ShowDialog(this) != DialogResult.OK) {
                MessageBox.Show("Operation cancelled.", "User discarded operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            /*==================================================================*/

            const string COMMENT_CHAR = "#";
            const string ISOLATED_FOLDERNAME = "DSPRE_IsolatedFiles";

            string[] listLines = File.ReadAllLines(of.FileName);
            listLines = listLines.Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith(COMMENT_CHAR)).ToArray();

            if (listLines.Length <= 0) {
                MessageBox.Show("The enumeration text file you selected is empty or only contains comment lines.\nCan't proceed.", "Invalid list file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string msg = "About to process ";
            int tot;
            string extra = "";

            int diff = files.Length - listLines.Length;
            if ( diff < 0 ) { //listLines.Length > files.Length 
                tot = files.Length;
                extra = "(Please note that the length of the chosen list [" + listLines.Length + " entries] " +
                    "exceeds the number of files in the folder.)" + "\n\n";
            } else if ( diff == 0 ) { //listLines.Length == files.Length
                tot = files.Length;
            } else { // diff > 0 --> listLines.Length < files.Length
                tot = listLines.Length;
                extra = "(Please note that there aren't enough entries in the list to rename all files in the chosen folder.\n" +
                    diff + " file" + (diff > 1 ? "s" : "") + " won't be renamed.)" + "\n\n";
            }

            msg += tot + " file" + (tot > 1 ? "s" : "");

            DialogResult dr = MessageBox.Show(msg + " from the input folder (taken in ascending order), " +
                "according to the list file you provided.\n" +
                "If a destination file already exists, DSPRE will append a number to its name.\n\n" + extra +
                "Do you want to proceed?", "Confirm operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr.Equals(DialogResult.Yes)) {
                int i;
                for (i = 0; i < tot; i++) {
                    FileInfo f = files[i];
                    Console.WriteLine(f.Name);
                    string destName = Path.GetDirectoryName(f.FullName) + "\\" + listLines[i];

                    if (string.IsNullOrWhiteSpace(destName)) {
                        continue;
                    }

                    File.Move(f.FullName, MakeUniqueName(destName));
                }

                MessageBox.Show("The contents of folder \"" + d.FullName + "\" have been renamed according to " + "\"" + of.FileName + "\".", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (listLines.Length < files.Length) {
                    dr = MessageBox.Show("Do you want to isolate the unnamed files by moving them to a dedicated folder?", "Waiting for user", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dr.Equals(DialogResult.Yes)) {
                        string isolatedDir = d.FullName + "\\" + ISOLATED_FOLDERNAME;
                        if (Directory.Exists(isolatedDir)) {
                            Directory.Delete(isolatedDir);
                        }
                        Directory.CreateDirectory(d.FullName + "\\" + ISOLATED_FOLDERNAME);

                        while ( i < files.Length ) {
                            FileInfo f = files[i];
                            Console.WriteLine(f.Name);
                            string destName = d.FullName + "\\" + ISOLATED_FOLDERNAME + "\\" + f.Name;
                            File.Move(f.FullName, destName);
                            i++;
                        }
                        MessageBox.Show("Isolated files have been moved to " + "\"" + ISOLATED_FOLDERNAME + "\"", "Files moved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            } else {
                MessageBox.Show("Operation cancelled.", "User discarded operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void contentBasedToolStripMenuItem_Click(object sender, EventArgs e) {
            ContentBasedBatchRename();
        }

        private void ContentBasedBatchRename(DirectoryInfo d = null) {
            (DirectoryInfo d, FileInfo[] files) dirData = OpenNonEmptyDir(d, title: "Content-Based Batch Rename Tool");
            d = dirData.d;
            FileInfo[] files = dirData.files;
            
            if (d == null || files == null) {
                return;
            }

            DialogResult dr = MessageBox.Show("About to rename " + files.Length + " file" + (files.Length > 1 ? "s" : "") +
                " from the input folder (taken in ascending order), according to their content.\n" +
                "If a destination file already exists, DSPRE will append a number to its name.\n\n" +
                "Do you want to proceed?", "Confirm operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr.Equals(DialogResult.Yes)) {
                List<string> enumerationFile = new List<string> {
                    "#============================================================================",
                    "# File enumeration definition for folder " + "\"" + d.Name + "\"",
                    "#============================================================================"
                };
                int initialLength = enumerationFile.Count;

                const byte toRead = 16;
                foreach (FileInfo f in files) {
                    Console.WriteLine(f.Name);

                    string fileNameOnly = Path.GetFileNameWithoutExtension(f.FullName);
                    string dirNameOnly = Path.GetDirectoryName(f.FullName);

                    string destName = "";
                    byte[] b = DSUtils.ReadFromFile(f.FullName, 0, toRead);

                    if (b == null || b.Length < toRead) {
                        continue;
                    }

                    string magic = "";

                    if (b[0] == 'B' && b[3] == '0') { //B**0
                        ushort nameOffset;

                        destName = dirNameOnly + "\\"; //Full filename can be changed
                        nameOffset = (ushort)(52 + (4 * (BitConverter.ToUInt16(b, 0xE) - 1)));

                        if (b[1] == 'T' && b[2] == 'X') { //BTX0
#if false
                            nameOffset += 0xEC;
#else
                            destName = fileNameOnly;
#endif
                        }

                        string nameRead = Encoding.UTF8.GetString(DSUtils.ReadFromFile(f.FullName, nameOffset, 16)).TrimEnd(new char[] { (char)0 });
                        
                        if (nameRead.Length <= 0 || nameRead.IndexOfAny(Path.GetInvalidPathChars()) >= 0 ) {
                            destName = fileNameOnly; //Filename can't be changed, only extension
                        } else {
                            destName += nameRead;
                        }

                        destName += ".ns";
                        for (int i = 0; i < 3; i++) {
                            magic += Char.ToLower((char)b[i]);
                        }
                    } else {
                        destName = fileNameOnly + ".";
                        byte offset = 0;

                        if (b[5] == 'R' && b[8] == 'N') { 
                            offset = 5;
                        }

                        for (int i = 0; i < 4; i++) {
                            magic += Char.ToLower((char)b[offset + i]);
                        }
                    }

                    if (string.IsNullOrWhiteSpace(magic) || !magic.All(char.IsLetterOrDigit)) {
                        continue;
                    }

                    destName += magic;

                    if (string.IsNullOrWhiteSpace(destName)) {
                        continue;
                    }

                    destName = MakeUniqueName(destName, fileNameOnly = null, dirNameOnly);
                    File.Move(f.FullName, Path.Combine(Path.GetDirectoryName(f.FullName), Path.GetFileName(destName)));

                    enumerationFile.Add(Path.GetFileName(destName));
                }

                if (enumerationFile.Count > initialLength) {
                    MessageBox.Show("Files inside folder \"" + d.FullName + "\" have been renamed according to their contents.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    DialogResult response = MessageBox.Show("Do you want to save a file enumeration list?", "Waiting for user", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (response.Equals(DialogResult.Yes)) {
                        MessageBox.Show("Choose where to save the output list file.", "Name your list file", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        SaveFileDialog sf = new SaveFileDialog {
                            Filter = "List File (*.txt; *.list)|*.txt;*.list",
                            FileName = d.Name + ".list"
                        };
                        if (sf.ShowDialog(this) != DialogResult.OK) {
                            MessageBox.Show("Operation cancelled.", "User discarded operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        File.WriteAllLines(sf.FileName, enumerationFile);
                        MessageBox.Show("List file saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                } else {
                    MessageBox.Show("No file content could be recognized.", "Operation terminated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            } else {
                MessageBox.Show("Operation cancelled.", "User discarded operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void fromFolderContentsToolStripMenuItem_Click(object sender, EventArgs e) {
            (DirectoryInfo d, FileInfo[] files) dirData = OpenNonEmptyDir(title: "Folder-Based List Builder");
            DirectoryInfo d = dirData.d;
            FileInfo[] filePaths = dirData.files;

            if (d == null || filePaths == null) {
                return;
            }

            MessageBox.Show("Choose where to save the output list file.", "Name your list file", MessageBoxButtons.OK, MessageBoxIcon.Information);

            SaveFileDialog sf = new SaveFileDialog {
                Filter = "List File (*.txt; *.list)|*.txt;*.list",
                FileName = d.Name + ".list"
            };
            if (sf.ShowDialog(this) != DialogResult.OK) {
                MessageBox.Show("Operation cancelled.", "User discarded operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            File.WriteAllLines(sf.FileName, new string[] {
                "#============================================================================",
                "# File enumeration definition for folder " + "\"" + d.Name + "\"",
                "#============================================================================"
            });
            File.AppendAllLines(sf.FileName, filePaths.Select(f => f.Name).ToArray());

            MessageBox.Show("List file saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void enumBasedListBuilderToolStripButton_Click(object sender, EventArgs e) {
            MessageBox.Show("Pick a C Enum File [with entries on different lines].", "Enum-Based List Builder", MessageBoxButtons.OK, MessageBoxIcon.Information);

            OpenFileDialog of = new OpenFileDialog {
                Filter = "Any Text File(*.*)|*.*"
            };
            if (of.ShowDialog(this) != DialogResult.OK) {
                MessageBox.Show("Operation cancelled.", "User discarded operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try {
                Dictionary<int, string> entries = new Dictionary<int, string>();

                string[] cFileLines = File.ReadAllLines(of.FileName);
                cFileLines = cFileLines.Select(x => x.Trim()).ToArray();

                int enumStartLine;
                for (enumStartLine = 0; enumStartLine < cFileLines.Length; enumStartLine++) {
                    if (cFileLines[enumStartLine].Replace(" ", "").Contains("enum{")) {
                        break;
                    }
                }

                if (cFileLines.Length - 1 == enumStartLine) {
                    MessageBox.Show("Abrupt termination of enum file.\nAborting.", "Parser error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int terminationLine;
                for (terminationLine = enumStartLine + 1; terminationLine < cFileLines.Length; terminationLine++) {
                    if (cFileLines[terminationLine].Replace(" ", "").Contains("};")) {
                        break;
                    }
                }

                if (terminationLine >= cFileLines.Length - 1) {
                    MessageBox.Show("Enum file is malformed.\nAborting", "Parser error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                };

                if (terminationLine - enumStartLine <= 2) {
                    MessageBox.Show("This utility needs at least 2 enum entries.\nAborting.", "Parser error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                int indexFirstDifferentChar = cFileLines[enumStartLine + 1].Zip(cFileLines[enumStartLine + 2], (char1, char2) => char1 == char2).TakeWhile(b => b).Count();
                int lastCommonUnderscore = cFileLines[enumStartLine + 1].Substring(0, indexFirstDifferentChar).LastIndexOf('_');

                int lastNumber = 0;

                MessageBox.Show("Choose where to save the output list file.", "Name your list file", MessageBoxButtons.OK, MessageBoxIcon.Information);
                string shortFileName = Path.GetFileNameWithoutExtension(of.FileName);

                SaveFileDialog sf = new SaveFileDialog {
                    Filter = "List File (*.txt; *.list)|*.txt;*.list",
                    FileName = shortFileName + ".list"
                };
                if (sf.ShowDialog(this) != DialogResult.OK) {
                    MessageBox.Show("Operation cancelled.", "User discarded operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                for (int s = enumStartLine + 1; s < terminationLine; s++) {
                    string withoutComment;

                    int indexOfComment = cFileLines[s].IndexOf("//");
                    if (indexOfComment > 0) {
                        withoutComment = cFileLines[s].Substring(0, indexOfComment);
                    } else {
                        withoutComment = cFileLines[s];
                    }

                    string differentSubstring = withoutComment.Substring(lastCommonUnderscore + 1).Trim().Replace(",", "");
                    int indexOfEquals = differentSubstring.LastIndexOf('=');

                    string entry = differentSubstring.Substring(0, indexOfEquals).Trim();
                    if (indexOfEquals > 0) {
                        string numstr = differentSubstring.Substring(indexOfEquals + 1);
                        string[] split = numstr.Split(new char[] { ' ' }, options: StringSplitOptions.RemoveEmptyEntries);

                        if (split.Length > 1) {
                            throw new Exception();
                        }

                        lastNumber = int.Parse(split[0]);
                    }

                    int posOfUnderscore = entry.LastIndexOf('_');
                    if (posOfUnderscore >= 0) {
                        entry = entry.Remove(posOfUnderscore, 1).Insert(posOfUnderscore, ".");
                    }

                    entries.Add(lastNumber, entry);
                    lastNumber++;
                }

                IEnumerable<KeyValuePair<int, string>> sortedEntries = entries.OrderBy(kvp => kvp.Key);

                File.WriteAllLines(sf.FileName, new string[] {
                    "#============================================================================",
                    "# File enumeration definition based on " + "\"" + shortFileName + "\"",
                    "#============================================================================"
                });
                File.AppendAllLines(sf.FileName, sortedEntries.Select(kvp => kvp.Value));

                MessageBox.Show("List file saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch (Exception ex) {
                MessageBox.Show("The input enum file couldn't be read correctly.\nNo output file has been written." +
                    "\n\nAborting.", "Parser error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("Details: " + ex.Message, "Failure details", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string MakeUniqueName(string fileName, string fileNameOnly = null, string dirNameOnly = null, string extension = null) {
            if (fileNameOnly == null) {
                fileNameOnly = Path.GetFileNameWithoutExtension(fileName);
            }
            if (dirNameOnly == null) {
                dirNameOnly = Path.GetDirectoryName(fileName);
            }
            if (extension == null) {
                extension = Path.GetExtension(fileName);
            }

            int append = 1;

            while (File.Exists(Path.Combine(dirNameOnly, fileName)) ) {
                string tmp = fileNameOnly + "(" + (append++) + ")";
                fileName = Path.Combine(dirNameOnly, tmp + extension);
            }
            return fileName;
        }

        private (DirectoryInfo, FileInfo[]) OpenNonEmptyDir(DirectoryInfo d = null, string title = "Waiting for user") {
            /*==================================================================*/
            if (d == null) {
                MessageBox.Show("Choose a source folder.", title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                CommonOpenFileDialog sourceDirDialog = new CommonOpenFileDialog {
                    IsFolderPicker = true,
                    Multiselect = false
                };

                if (sourceDirDialog.ShowDialog() != CommonFileDialogResult.Ok) {
                    MessageBox.Show("Operation cancelled.", "User discarded operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return (null, null);
                }

                d = new DirectoryInfo(sourceDirDialog.FileName);
            }

            FileInfo[] tempfiles = d.GetFiles();
            FileInfo[] files = tempfiles.OrderBy(n => System.Text.RegularExpressions.Regex.Replace(n.Name, @"\d+", e => e.Value.PadLeft(tempfiles.Length.ToString().Length, '0'))).ToArray();

            if (files.Length <= 0) {
                MessageBox.Show("Folder " + "\"" + d.FullName + "\"" + " is empty.\nCan't proceed.", "Invalid folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (null, null);
            };

            return (d, files);
        }

        private void simpleToolStripMenuItem_MouseDown(object sender, MouseEventArgs e) {
            ToolStripMenuItem tsmi = (sender as ToolStripMenuItem);
            SetMenuLayout((byte)tsmi.GetCurrentParent().Items.IndexOf(tsmi));
        }

        private void SetMenuLayout(byte layoutStyle) {
            Console.WriteLine("Setting menuLayout to" + layoutStyle);

            IList list = menuViewToolStripMenuItem.DropDownItems;
            for (int i = 0; i < list.Count; i++) {
                (list[i] as ToolStripMenuItem).Checked = (i == layoutStyle);
            }

            Properties.Settings.Default.menuLayout = layoutStyle;

            switch (layoutStyle) {
                case 0:
                    buildNarcFromFolderToolStripButton.Visible = false;
                    unpackNARCtoFolderToolStripButton.Visible = false;
                    separator_afterNarcUtils.Visible = false;

                    listBasedBatchRenameToolStripButton.Visible = false;
                    contentBasedBatchRenameToolStripButton.Visible = false;
                    separator_afterRenameUtils.Visible = false;

                    enumBasedListBuilderToolStripButton.Visible = false;
                    folderBasedListBuilderToolStriButton.Visible = false;
                    separator_afterListUtils.Visible = false;

                    nsbmdAddTexButton.Visible = false;
                    nsbmdRemoveTexButton.Visible = false;
                    nsbmdExportTexButton.Visible = false;
                    separator_afterNsbmdUtils.Visible = false;

                    wildEditorButton.Visible = false;
                    romToolboxToolStripButton.Visible = false;
                    break;
                case 1:
                    buildNarcFromFolderToolStripButton.Visible = false;
                    unpackNARCtoFolderToolStripButton.Visible = false;
                    separator_afterNarcUtils.Visible = false;

                    listBasedBatchRenameToolStripButton.Visible = false;
                    contentBasedBatchRenameToolStripButton.Visible = false;
                    separator_afterRenameUtils.Visible = false;

                    enumBasedListBuilderToolStripButton.Visible = false;
                    folderBasedListBuilderToolStriButton.Visible = false;
                    separator_afterListUtils.Visible = false;

                    nsbmdAddTexButton.Visible = true;
                    nsbmdRemoveTexButton.Visible = true;
                    nsbmdExportTexButton.Visible = true;
                    separator_afterNsbmdUtils.Visible = true;

                    wildEditorButton.Visible = true;
                    romToolboxToolStripButton.Visible = true;
                    break;
                case 2:
                default:
                    foreach (ToolStripItem c in mainToolStrip.Items) {
                        c.Visible = true;
                    }
                    break;
            }
        }

        private void locateCurrentMatrixFile_Click(object sender, EventArgs e) {
            Helpers.ExplorerSelect(Path.Combine(gameDirs[DirNames.matrices].unpackedDir, selectMatrixComboBox.SelectedIndex.ToString("D4")));
        }

        private void locateCurrentMapBin_Click(object sender, EventArgs e) {
            Helpers.ExplorerSelect(Path.Combine(gameDirs[DirNames.maps].unpackedDir, selectMapComboBox.SelectedIndex.ToString("D4")));
        }

        private void locateCurrentNsbtx_Click(object sender, EventArgs e) {
            if (mapTilesetRadioButton.Checked) {
                Helpers.ExplorerSelect(Path.Combine(gameDirs[DirNames.mapTextures].unpackedDir, texturePacksListBox.SelectedIndex.ToString("D4")));
            } else {
                Helpers.ExplorerSelect(Path.Combine(gameDirs[DirNames.buildingTextures].unpackedDir, texturePacksListBox.SelectedIndex.ToString("D4")));
            }
        }

        private void locateCurrentAreaData_Click(object sender, EventArgs e) {
            Helpers.ExplorerSelect(Path.Combine(gameDirs[DirNames.areaData].unpackedDir, selectAreaDataListBox.SelectedIndex.ToString("D4")));
        }
        private void locateCurrentEvFile_Click(object sender, EventArgs e) {
 
        }
        private void locateCurrentScriptFile_Click(object sender, EventArgs e) {
            Helpers.ExplorerSelect(Path.Combine(gameDirs[DirNames.scripts].unpackedDir, selectScriptFileComboBox.SelectedIndex.ToString("D4")));
        }
        private void locateCurrentTextArchive_Click(object sender, EventArgs e) {

        }

        //////////////////////////////////////////
        ///NSBTX Visualizer

        private float nsbtxScaleFactor = 1.0f;

        public void PictureBoxDisable(object sender, PaintEventArgs e) {
            if (sender is PictureBox pict && pict.Image != null && (!pict.Enabled)) {
                using (var img = new Bitmap(pict.Image, pict.ClientSize)) {
                    ControlPaint.DrawImageDisabled(e.Graphics, img, 0, 0, pict.BackColor);
                }
            }
        }

        private int NSBTXRender(int tex, int pal, float scale = -1, NSBTX_File file = null) {
            NSBTX_File toload = file;
            if (toload is null) {
                if (currentNsbtx is null) {
                    return -1;
                }

                toload = currentNsbtx;
            }

            (Bitmap bmp, int ctrlCode) ret;
            if (tex == -1 && pal == -1) {
                return -1;
            } else {
                ret = toload.GetBitmap(tex, pal);
            }

            if (ret.bmp != null) {
                try {
                    texturePictureBox.Image = ret.bmp.Resize(scale);
                    texturePictureBox.Invalidate();
                } catch { }
            }
            return ret.ctrlCode;
        }
        private void scalingTrackBar_Scroll(object sender, EventArgs e) {
            int val = (sender as TrackBar).Value;
            nsbtxScaleFactor = (float)(val > 0 ? val + 1 : Math.Pow(2, (sender as TrackBar).Value));

            scalingLabel.Text = $"x{nsbtxScaleFactor}";
            NSBTXRender(texturesListBox.SelectedIndex, palettesListBox.SelectedIndex, scale: nsbtxScaleFactor);
        }

        private void invertDragCheckbox_CheckedChanged(object sender, EventArgs e) {
            texturePictureBox.invertDrag = invertDragCheckbox.Checked;
        }

        private void repositionImageButton_Click(object sender, EventArgs e) {
            texturePictureBox.RedrawCentered();
        }

        private void texturedMapRenderCheckBox_CheckedChanged(object sender, EventArgs e) {
            mapTexturesOn = (sender as CheckBox).Checked;
           Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }

        private void texturedBldRenderCheckBox_CheckedChanged(object sender, EventArgs e) {
            bldTexturesOn = (sender as CheckBox).Checked;
           Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }
    }
}