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

        public void statusLabelMessage(string msg = "Ready") {
            statusLabel.Text = msg;
            statusLabel.Font = new Font(statusLabel.Font, FontStyle.Regular);
            statusLabel.ForeColor = Color.Black;
            statusLabel.Invalidate();
        }

        public void statusLabelError(string errorMsg, bool severe = true) {
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
            mapEditor.mapOpenGlControl.InitializeContexts();

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
                mapEditor.SetupMapEditor();
                nsbtxEditor.SetupNSBTXEditor();
                eventEditor.SetupEventEditor();
                scriptEditor.SetupScriptEditorTextAreas();
                scriptEditor.SetupScriptEditor();
                textEditor.SetupTextEditor();
                trainerEditor.SetupTrainerEditor();

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

                if (mapEditor.mapEditorIsReady) {
                    mapEditor.updateBuildingListComboBox(mapEditor.interiorbldRadioButton.Checked);
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
                if (!mapEditor.mapEditorIsReady) {
                    mapEditor.SetupMapEditor();
                    mapEditor.mapEditorIsReady = true;
                }
            } else if (mainTabControl.SelectedTab == nsbtxEditorTabPage) {
                if (!nsbtxEditor.nsbtxEditorIsReady) {
                    nsbtxEditor.SetupNSBTXEditor();
                    nsbtxEditor.nsbtxEditorIsReady = true;
                }
            } else if (mainTabControl.SelectedTab == eventEditorTabPage) {
                if (!eventEditor.eventEditorIsReady) {
                    eventEditor.SetupEventEditor();
                    eventEditor.eventEditorIsReady = true;
                }
            } else if (mainTabControl.SelectedTab == scriptEditorTabPage) {
                if (!scriptEditor.scriptEditorIsReady) {
                    scriptEditor.SetupScriptEditorTextAreas();
                    scriptEditor.SetupScriptEditor();
                    scriptEditor.scriptEditorIsReady = true;
                }
            } else if (mainTabControl.SelectedTab == textEditorTabPage) {
                if (!textEditor.textEditorIsReady) {
                    textEditor.SetupTextEditor();
                    textEditor.textEditorIsReady = true;
                }
            } else if (mainTabControl.SelectedTab == cameraEditorTabPage) {
                if (!cameraEditor.cameraEditorIsReady) {
                    cameraEditor.SetupCameraEditor();
                    cameraEditor.cameraEditorIsReady = true;
                }
            } else if (mainTabControl.SelectedTab == trainerEditorTabPage) {
                if (!trainerEditor.trainerEditorIsReady) {
                    trainerEditor.SetupTrainerEditor();
                    trainerEditor.trainerEditorIsReady = true;
                }
            } else if (mainTabControl.SelectedTab == tableEditorTabPage) {
                if (!tableEditor.tableEditorIsReady) {
                    resetHeaderSearch();
                    tableEditor.SetupTableEditor();
                    tableEditor.tableEditorIsReady = true;
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
            if (!nsbtxEditor.nsbtxEditorIsReady) {
                nsbtxEditor.SetupNSBTXEditor();
                nsbtxEditor.nsbtxEditorIsReady = true;
            }

            nsbtxEditor.selectAreaDataListBox.SelectedIndex = (int)areaDataUpDown.Value;
            nsbtxEditor.texturePacksListBox.SelectedIndex = (nsbtxEditor.mapTilesetRadioButton.Checked ? (int)nsbtxEditor.areaDataMapTilesetUpDown.Value : (int)nsbtxEditor.areaDataBuildingTilesetUpDown.Value);
            mainTabControl.SelectedTab = nsbtxEditorTabPage;

            if (nsbtxEditor.texturesListBox.Items.Count > 0)
                nsbtxEditor.texturesListBox.SelectedIndex = 0;
            if (nsbtxEditor.palettesListBox.Items.Count > 0)
                nsbtxEditor.palettesListBox.SelectedIndex = 0;
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

                if (!mapEditor.mapEditorIsReady) {
                    mapEditor.SetupMapEditor();
                    mapEditor.mapEditorIsReady = true;
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
                mapEditor.selectMapComboBox.SelectedIndex = currentMatrix.maps[e.RowIndex, e.ColumnIndex];
                mapEditor.mapTextureComboBox.SelectedIndex = areaData.mapTileset + 1;
                mapEditor.buildTextureComboBox.SelectedIndex = areaData.buildingsTileset + 1;
                mainTabControl.SelectedTab = mapEditorTabPage;

                if (areaData.areaType == AreaData.TYPE_INDOOR) {
                    mapEditor.interiorbldRadioButton.Checked = true;
                } else {
                    mapEditor.exteriorbldRadioButton.Checked = true;
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

        /* Map Rotation vars */

        /* Screenshot Interpolation mode */
        public InterpolationMode intMode;

        /*  Camera settings */


        /* Renderers */

        /* Map file */

        /* Permission painters */
        #endregion

        #region Subroutines




        #endregion

        private void addMapFileButton_Click(object sender, EventArgs e) {

        }
        private void replaceMapBinButton_Click(object sender, EventArgs e) {

        }


        private void buildTextureComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }
        private void mapTextureComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }
        private void mapEditorTabPage_Enter(object sender, EventArgs e) {
            mapEditor.makeCurrent();
        }

        private void mapOpenGlControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {

        }
        private void mapOpenGlControl_KeyUp(object sender, KeyEventArgs e) {

        }
        private void mapOpenGlControl_Click(object sender, EventArgs e) {

        }
        private void bldRoundWhole_CheckedChanged(object sender, EventArgs e) {

        }
        private void bldRoundDec_CheckedChanged(object sender, EventArgs e) {

        }
        private void bldRoundCent_CheckedChanged(object sender, EventArgs e) {

        }
        private void bldRoundMil_CheckedChanged(object sender, EventArgs e) {

        }
        private void bldRoundDecmil_CheckedChanged(object sender, EventArgs e) {

        }
        private void bldRoundCentMil_CheckedChanged(object sender, EventArgs e) {

        }
        private void bldPlaceWithMouseCheckbox_CheckedChanged(object sender, EventArgs e) {

        }
        private void bldPlaceLockXcheckbox_CheckedChanged(object sender, EventArgs e) {

        }

        private void bldPlaceLockZcheckbox_CheckedChanged(object sender, EventArgs e) {

        }
        private void mapPartsTabControl_SelectedIndexChanged(object sender, EventArgs e) {

        }
        private void radio2D_CheckedChanged(object sender, EventArgs e) {

        }


        private void mapScreenshotButton_Click(object sender, EventArgs e) {

        }
        private void removeLastMapFileButton_Click(object sender, EventArgs e) {

        }
        private void saveMapButton_Click(object sender, EventArgs e) {

        }
        private void exportCurrentMapBinButton_Click(object sender, EventArgs e) {

        }
        private void selectMapComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }
        private void wireframeCheckBox_CheckedChanged(object sender, EventArgs e) {

        }

        #region Building Editor
        private void addBuildingButton_Click(object sender, EventArgs e) {

        }
        private void duplicateBuildingButton_Click(object sender, EventArgs e) {

        }

        private void buildIndexComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }
        private void buildingsListBox_SelectedIndexChanged(object sender, EventArgs e) {

        }
        private void xRotBuildUpDown_ValueChanged(object sender, EventArgs e) {

        }

        private void yRotBuildUpDown_ValueChanged(object sender, EventArgs e) {

        }

        private void zRotBuildUpDown_ValueChanged(object sender, EventArgs e) {

        }

        private void xRotDegBldUpDown_ValueChanged(object sender, EventArgs e) {

        }

        private void yRotDegBldUpDown_ValueChanged(object sender, EventArgs e) {

        }

        private void zRotDegBldUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void buildingHeightUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void buildingLengthUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void buildingWidthUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void exportBuildingsButton_Click(object sender, EventArgs e) {

        }
        private void importBuildingsButton_Click(object sender, EventArgs e) {

        }
        private void interiorRadioButton_CheckedChanged(object sender, EventArgs e) {

        }
        private void removeBuildingButton_Click(object sender, EventArgs e) {

        }
        private void xBuildUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void zBuildUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void yBuildUpDown_ValueChanged(object sender, EventArgs e) {

        }
        #endregion

        #region Movement Permissions Editor

        #region Subroutines




        #endregion

        private void clearCurrentButton_Click(object sender, EventArgs e) {

        }

        private void collisionPictureBox_Click(object sender, EventArgs e) {

        }
        private void exportMovButton_Click(object sender, EventArgs e) {

        }
        private void importMovButton_Click(object sender, EventArgs e) {

        }
        private void movPictureBox_Click(object sender, EventArgs e) {

        }
        private void movPictureBox_MouseMove(object sender, MouseEventArgs e) {

        }
        private void collisionPainterComboBox_SelectedIndexChange(object sender, EventArgs e) {

        }
        private void typePainterComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void typePainterUpDown_ValueChanged(object sender, EventArgs e) {

        }


        private void typePictureBox_Click(object sender, EventArgs e) {

        }
        private void typesRadioButton_CheckedChanged(object sender, EventArgs e) {

        }
        private void valueTypeRadioButton_CheckedChanged(object sender, EventArgs e) {

        }
        #endregion

        #region 3D Model Editor
        private void importMapButton_Click(object sender, EventArgs e) {

        }
        private void exportMapButton_Click(object sender, EventArgs e) {

        }

        private void daeExportButton_Click(object sender, EventArgs e) {

        }
        #endregion

        #region BDHC Editor
        private void bdhcImportButton_Click(object sender, EventArgs e) {

        }
        private void bdhcExportButton_Click(object sender, EventArgs e) {

        }
        private void soundPlatesImportButton_Click(object sender, EventArgs e) {

        }
        private void soundPlatesExportButton_Click(object sender, EventArgs e) {

        }
        private void soundPlatesBlankButton_Click(object sender, EventArgs e) {

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


        #endregion
        #region Helper Methods

        private void scriptEditorTabControl_TabIndexChanged(object sender, EventArgs e) {

        }


        private void openSearchScriptEditorButton_Click(object sender, EventArgs e) {

        }





        #region Numbers, Bookmarks, Code Folding




        #endregion

        #region Main Menu Commands

        private void scriptEditorWordWrapCheckbox_CheckedChanged(object sender, EventArgs e) {

        }

        private void viewWhiteSpacesButton_Click(object sender, EventArgs e) {

        }

        private void ScriptEditorCollapseButton_Click(object sender, EventArgs e) {

        }

        private void ScriptEditorExpandButton_Click(object sender, EventArgs e) {

        }


        #endregion

        #region Uppercase / Lowercase

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

        #endregion

        #region Quick Search Bar
        private void BtnPrevSearchScript_Click(object sender, EventArgs e) {

        }

        private void BtnNextSearchScript_Click(object sender, EventArgs e) {

        }

        private void BtnPrevSearchFunc_Click(object sender, EventArgs e) {

        }

        private void BtnNextSearchFunc_Click(object sender, EventArgs e) {

        }

        private void BtnPrevSearchActions_Click(object sender, EventArgs e) {

        }

        private void BtnNextSearchActions_Click(object sender, EventArgs e) {

        }

        private void BtnCloseSearchScript_Click(object sender, EventArgs e) {

        }

        private void BtnCloseSearchFunc_Click(object sender, EventArgs e) {

        }

        private void BtnCloseSearchActions_Click(object sender, EventArgs e) {

        }

        private void scriptTxtSearch_KeyDown(object sender, KeyEventArgs e) {

        }
        private void functionTxtSearch_KeyDown(object sender, KeyEventArgs e) {

        }
        private void actiontTxtSearch_KeyDown(object sender, KeyEventArgs e) {

        }


        private void panelSearchScriptTextBox_TextChanged(object sender, EventArgs e) {

        }
        private void panelSearchFunctionTextBox_TextChanged(object sender, EventArgs e) {

        }
        private void panelSearchActionTextBox_TextChanged(object sender, EventArgs e) {

        }

        #endregion
        private void addScriptFileButton_Click(object sender, EventArgs e) {

        }
        private void exportScriptFileButton_Click(object sender, EventArgs e) {

        }
        private void saveScriptFileButton_Click(object sender, EventArgs e) {

        }
        private void clearCurrentLevelScriptButton_Click(object sender, EventArgs e) {

        }
        private void importScriptFileButton_Click(object sender, EventArgs e) {

        }
        private void openScriptButton_Click(object sender, EventArgs e) {
            if (!scriptEditor.scriptEditorIsReady) {
                scriptEditor.SetupScriptEditorTextAreas();
                scriptEditor.SetupScriptEditor();
                scriptEditor.scriptEditorIsReady = true;
            }

            scriptEditor.scriptEditorTabControl.SelectedIndex = 0;
            scriptEditor.selectScriptFileComboBox.SelectedIndex = (int)scriptFileUpDown.Value;
            mainTabControl.SelectedTab = scriptEditorTabPage;
        }
        private void openLevelScriptButton_Click(object sender, EventArgs e) {
            if (!scriptEditor.scriptEditorIsReady) {
                scriptEditor.SetupScriptEditorTextAreas();
                scriptEditor.SetupScriptEditor();
                scriptEditor.scriptEditorIsReady = true;
            }

            scriptEditor.selectScriptFileComboBox.SelectedIndex = (int)levelScriptUpDown.Value;
            mainTabControl.SelectedTab = scriptEditorTabPage;
        }
        private void removeScriptFileButton_Click(object sender, EventArgs e) {

        }
        private void searchInScriptsButton_Click(object sender, EventArgs e) {

        }

        private void searchInScripts_GoToEntryResult(object sender, MouseEventArgs e) {

        }
        private void searchInScriptsResultListBox_KeyDown(object sender, KeyEventArgs e) {

        }
        private void searchInScriptsTextBox_KeyDown(object sender, KeyEventArgs e) {

        }
        private void selectScriptFileComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void UpdateScriptNumberFormatDec(object sender, EventArgs e) {

        }
        private void UpdateScriptNumberFormatHex(object sender, EventArgs e) {

        }
        private void UpdateScriptNumberFormatNoPref(object sender, EventArgs e) {

        }

        private void scriptsNavListbox_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void functionsNavListbox_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void actionsNavListbox_SelectedIndexChanged(object sender, EventArgs e) {

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

        private void buildingsTilesetRadioButton_CheckedChanged(object sender, EventArgs e) {

        }
        private void exportNSBTXButton_Click(object sender, EventArgs e) {

        }
        private void importNSBTXButton_Click(object sender, EventArgs e) {

        }
        private void mapTilesetRadioButton_CheckedChanged(object sender, EventArgs e) {

        }
        private void palettesListBox_SelectedIndexChanged(object sender, EventArgs e) {

        }
        private void texturePacksListBox_SelectedIndexChanged(object sender, EventArgs e) {

        }
        private void texturesListBox_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void areaDataBuildingTilesetUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void areaDataDynamicTexturesUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void areaDataLightTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }
        private void areaDataMapTilesetUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void saveAreaDataButton_Click(object sender, EventArgs e) {

        }
        private void selectAreaDataListBox_SelectedIndexChanged(object sender, EventArgs e) {

        }
        private void indoorAreaRadioButton_CheckedChanged(object sender, EventArgs e) {

        }
        private void addNSBTXButton_Click(object sender, EventArgs e) {

        }
        private void removeNSBTXButton_Click(object sender, EventArgs e) {

        }
        private void addAreaDataButton_Click(object sender, EventArgs e) {

        }
        private void removeAreaDataButton_Click(object sender, EventArgs e) {

        }
        private void exportAreaDataButton_Click(object sender, EventArgs e) {

        }
        private void importAreaDataButton_Click(object sender, EventArgs e) {

        }
        #endregion

        #region Camera Editor


        private void saveCameraTableButton_Click(object sender, EventArgs e) {

        }
        private void cameraEditorDataGridView_CellValidated(object sender, DataGridViewCellEventArgs e) {

        }
        private void exportCameraTableButton_Click(object sender, EventArgs e) {

        }

        private void cameraEditorDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e) {

        }
        private void importCameraTableButton_Click(object sender, EventArgs e) {

        }
        #endregion

        #region Trainer Editor



        private void trainerComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }



        private void partyPokemon1ComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }
        private void partyPokemon2ComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void partyPokemon3ComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void partyPokemon4ComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void partyPokemon5ComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void partyPokemon6ComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void partyItem1ComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void partyItem2ComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void partyItem3ComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void partyItem4ComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void partyItem5ComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void partyItem6ComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void DVExplainButton_Click(object sender, EventArgs e) {

        }

        private void partyCountUpDown_ValueChanged(object sender, EventArgs e) {

        }

        private void trainerMovesCheckBox_CheckedChanged(object sender, EventArgs e) {

        }
        private void trainerItemsCheckBox_CheckedChanged(object sender, EventArgs e) {

        }
        private void partyMoveComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void trainerSaveCurrentButton_Click(object sender, EventArgs e) {

        }

        private void trainerClassListBox_SelectedIndexChanged(object sender, EventArgs e) {

        }


        private void addTrainerButton_Click(object sender, EventArgs e) {

        }

        private void exportTrainerButton_Click(object sender, EventArgs e) {

        }

        private void importTrainerButton_Click(object sender, EventArgs e) {

        }

        private void exportPropertiesButton_Click(object sender, EventArgs e) {

        }

        private void importReplacePropertiesButton_Click(object sender, EventArgs e) {

        }

        private void exportPartyButton_Click(object sender, EventArgs e) {

        }

        private void importReplacePartyButton_Click(object sender, EventArgs e) {

        }

        private void saveTrainerClassButton_Click(object sender, EventArgs e) {

        }

        private void trClassFramePreviewUpDown_ValueChanged(object sender, EventArgs e) {

        }
        #endregion

        #region Table Editor
        #region Variables


        #endregion

        private void conditionalMusicTableListBox_SelectedIndexChanged(object sender, EventArgs e) {

        }
        private void headerConditionalMusicComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }
        private void flagConditionalMusicUpDown_ValueChanged(object sender, EventArgs e) {

        }

        private void musicIDconditionalMusicUpDown_ValueChanged(object sender, EventArgs e) {

        }
        private void HOWconditionalMusicTableButton_Click(object sender, EventArgs e) {

        }

        private void saveConditionalMusicTableBTN_Click(object sender, EventArgs e) {

        }

        private void TBLEditortrainerClassPreviewPic_ValueChanged(object sender, EventArgs e) {

        }

        private void saveEffectComboBTN_Click(object sender, EventArgs e) {

        }

        private void saveVSPokemonEntryBTN_Click(object sender, EventArgs e) {

        }

        private void saveVSTrainerEntryBTN_Click(object sender, EventArgs e) {

        }

        private void HOWpbEffectsTableButton_Click(object sender, EventArgs e) {

        }

        private void HOWvsPokemonButton_Click(object sender, EventArgs e) {

        }
        
        private void HOWVsTrainerButton_Click(object sender, EventArgs e) {

        }

        private void pbEffectsVsTrainerListbox_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void pbEffectsVsPokemonListbox_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void pbEffectsCombosListbox_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void pbEffectsTrainerCombobox_SelectedIndexChanged(object sender, EventArgs e) {

        }
        private void pbEffectsPokemonCombobox_SelectedIndexChanged(object sender, EventArgs e) {

        }

        #endregion

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

        }

        private void locateCurrentNsbtx_Click(object sender, EventArgs e) {

        }

        private void locateCurrentAreaData_Click(object sender, EventArgs e) {

        }
        private void locateCurrentEvFile_Click(object sender, EventArgs e) {
 
        }
        private void locateCurrentScriptFile_Click(object sender, EventArgs e) {

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


        private void scalingTrackBar_Scroll(object sender, EventArgs e) {

        }

        private void invertDragCheckbox_CheckedChanged(object sender, EventArgs e) {

        }

        private void repositionImageButton_Click(object sender, EventArgs e) {

        }
    }
}