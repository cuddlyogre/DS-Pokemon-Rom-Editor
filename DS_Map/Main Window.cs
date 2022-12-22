using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NarcAPI;
using DSPRE.Resources;
using DSPRE.ROMFiles;
using static DSPRE.RomInfo;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace DSPRE {
    public partial class MainProgram : Form {
        public MainProgram() {
            InitializeComponent();
            EditorPanels.Initialize(this);
            Helpers.Initialize(this);
            SetMenuLayout(Properties.Settings.Default.menuLayout); //Read user settings for menu layout
        }

        public bool iconON = false;

        /* ROM Information */
        public static string gameCode;
        public static byte europeByte;

        private void MainProgram_FormClosing(object sender, FormClosingEventArgs e) {
            if (MessageBox.Show("Are you sure you want to quit?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) {
                e.Cancel = true;
            }
            Properties.Settings.Default.Save();
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
            Helpers.statusLabelMessage("Unpacking ROM contents to " + RomInfo.workDir + " ...");
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

        private void romToolBoxToolStripMenuItem_Click(object sender, EventArgs e) {
            using (ROMToolboxDialog window = new ROMToolboxDialog()) {
                window.ShowDialog();
                if (ROMToolboxDialog.flag_standardizedItems && eventEditor.eventEditorIsReady) {
                    eventEditor.UpdateItemComboBox(RomInfo.GetItemNames());
                }
                if (ROMToolboxDialog.flag_DynamicHeadersPatchApplied) {
                    headerEditor.addHeaderBTN.Enabled = true;
                    headerEditor.removeLastHeaderBTN.Enabled = true;
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
            Helpers.statusLabelMessage("Setting up Commands Database. Please wait...");
            Update();
            CommandsDatabase form = new CommandsDatabase(namesDict, paramsDict, actionsDict, comparisonOPsDict);
            form.Show();
            Helpers.statusLabelMessage();
        }
        private void headerSearchToolStripButton_Click(object sender, EventArgs e) {
            mainTabControl.SelectedIndex = 0; //Select Header Editor
            using (HeaderSearch h = new HeaderSearch(ref headerEditor.internalNames, headerEditor.headerListBox, statusLabel)) {
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
            Helpers.toolStripProgressBar.Visible = true;

            Helpers.statusLabelMessage("Attempting to unpack Building Editor NARCs... Please wait. This might take a while");
            Helpers.toolStripProgressBar.Visible = true;
            Helpers.toolStripProgressBar.Maximum = 4;
            Helpers.toolStripProgressBar.Value = 0;
            Update();

            List<DirNames> toUnpack = new List<DirNames> {
                DirNames.exteriorBuildingModels,
                DirNames.buildingConfigFiles,
                DirNames.buildingTextures,
                DirNames.areaData
            };

            if (forceUnpack) {
                DSUtils.ForceUnpackNarcs(toUnpack);

                if (RomInfo.gameFamily == GameFamilies.HGSS) {
                    DSUtils.ForceUnpackNarcs(new List<DirNames> { DirNames.interiorBuildingModels });// Last = interior buildings dir
                }
            } else {
                DSUtils.TryUnpackNarcs(toUnpack);

                if (RomInfo.gameFamily == GameFamilies.HGSS) {
                    DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.interiorBuildingModels });
                }
            }

            Helpers.toolStripProgressBar.Value = 0;
            Helpers.toolStripProgressBar.Visible = false;
            Helpers.statusLabelMessage();
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
                    Helpers.statusLabelMessage("Loading aborted");
                    Update();
                    return;
                case 0:
                    break;
                case 1:
                case 2:
                    Application.DoEvents();
                    if (userchoice == 1) {
                        Helpers.statusLabelMessage("Deleting old data...");
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
                            Helpers.statusLabelError("ERROR");
                            languageLabel.Text = "";
                            versionLabel.Text = "Error";
                            return;
                        }
                        DSUtils.ARM9.EditSize(-12);
                    } catch (IOException) {
                        MessageBox.Show("Can't access temp directory: \n" + RomInfo.workDir + "\nThis might be a temporary issue.\nMake sure no other process is using it and try again.", "Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Helpers.statusLabelError("ERROR: Concurrent access to " + RomInfo.workDir);
                        Update();
                        return;
                    }
                    break;
            }

            iconON = true;
            gameIcon.Refresh();  // Paint game icon
            Helpers.statusLabelMessage("Attempting to unpack NARCs from folder...");
            Update();

            ReadROMInitData();
        }

        private void CheckROMLanguage() {
            versionLabel.Visible = true;
            languageLabel.Visible = true;

            versionLabel.Text = RomInfo.gameVersion.ToString() + " " + "[" + RomInfo.romID + "]";
            languageLabel.Text = "Lang: " + RomInfo.gameLanguage;

            if (RomInfo.gameLanguage == GameLanguages.English) {
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
                if ( !RomInfo.gameFamily.Equals(GameFamilies.HGSS) ) {
                    MessageBox.Show("Unexpected compressed ARM9. It is advised that you double check the ARM9.");
                }
                if (!DSUtils.ARM9.Decompress(RomInfo.arm9Path)) {
                    MessageBox.Show("ARM9 decompression failed. The program can't proceed.\nAborting.",
                                "Error with ARM9 decompression", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            /* Setup essential editors */
            headerEditor.SetupHeaderEditor(true);
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
            Helpers.statusLabelMessage();
            this.Text += "  -  " + RomInfo.fileName;
        }

        private void saveRom_Click(object sender, EventArgs e) {
            SaveFileDialog saveRom = new SaveFileDialog {
                Filter = DSUtils.NDSRomFilter
            };
            if (saveRom.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            Helpers.statusLabelMessage("Repacking NARCS...");
            Update();

            // Repack NARCs
            foreach (KeyValuePair<DirNames, (string packedDir, string unpackedDir)> kvp in RomInfo.gameDirs) {
                DirectoryInfo di = new DirectoryInfo(kvp.Value.unpackedDir);
                if (di.Exists) {
                    Narc.FromFolder(kvp.Value.unpackedDir).Save(kvp.Value.packedDir); // Make new NARC from folder
                }
            }


            if ( DSUtils.ARM9.CheckCompressionMark() ) {
                Helpers.statusLabelMessage("Awaiting user response...");
                DialogResult d = MessageBox.Show("The ARM9 file of this ROM is currently uncompressed, but marked as compressed.\n" +
                    "This will prevent your ROM from working on native hardware.\n\n" +
                "Do you want to mark the ARM9 as uncompressed?", "ARM9 compression mismatch detected",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (d == DialogResult.Yes) {
                    DSUtils.ARM9.WriteBytes( new byte[4] { 0, 0, 0, 0 }, (uint)(RomInfo.gameFamily == GameFamilies.DP ? 0xB7C : 0xBB4) );
                }
            }

            Helpers.statusLabelMessage("Repacking ROM...");

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

            if (RomInfo.gameFamily != GameFamilies.DP && RomInfo.gameFamily != GameFamilies.Plat) {
                if (eventEditor.eventEditorIsReady) {
                    if (DSUtils.OverlayIsCompressed(1)) {
                        DSUtils.DecompressOverlay(1);
                    }
                }
            }

            Properties.Settings.Default.Save();
            Helpers.statusLabelMessage();
        }
        private void unpackAllButton_Click(object sender, EventArgs e) {
            Helpers.statusLabelMessage("Awaiting user response...");
            DialogResult d = MessageBox.Show("Do you wish to unpack all extracted NARCS?\n" +
                "This operation might be long and can't be interrupted.\n\n" +
                "Any unsaved changes made to the ROM in this session will be lost." +
                "\nProceed?", "About to unpack all NARCS",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (d == DialogResult.Yes) {
                Helpers.toolStripProgressBar.Maximum = RomInfo.gameDirs.Count;
                Helpers.toolStripProgressBar.Visible = true;
                Helpers.toolStripProgressBar.Value = 0;
                Helpers.statusLabelMessage("Attempting to unpack all NARCs... Be patient. This might take a while...");
                Update();

                DSUtils.ForceUnpackNarcs(Enum.GetValues(typeof(DirNames)).Cast<DirNames>().ToList());
                MessageBox.Show("Operation completed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Helpers.toolStripProgressBar.Value = 0;
                Helpers.toolStripProgressBar.Visible = false;

                headerEditor.SetupHeaderEditor(true);
                matrixEditor.SetupMatrixEditor(true);
                mapEditor.SetupMapEditor(true);
                nsbtxEditor.SetupNSBTXEditor(true);
                eventEditor.SetupEventEditor(true);
                scriptEditor.SetupScriptEditor(true);
                levelScriptEditor.SetUpLevelScriptEditor(true);
                textEditor.SetupTextEditor(true);
                // cameraEditor.SetupCameraEditor(true);
                trainerEditor.SetupTrainerEditor(true);
                // tableEditor.SetupTableEditor(true);

                Helpers.statusLabelMessage();
                Update();
            }
        }
        private void updateMapNarcsButton_Click(object sender, EventArgs e) {
            Helpers.statusLabelMessage("Awaiting user response...");
            DialogResult d = MessageBox.Show("Do you wish to unpack all NARC files necessary for the Building Editor ?\n" +
               "This operation might be long and can't be interrupted.\n\n" +
               "Any unsaved changes made to building models and textures in this session will be lost." +
               "\nProceed?", "About to unpack Building NARCs",
               MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (d == DialogResult.Yes) {
                unpackBuildingEditorNARCs(forceUnpack: true);

                MessageBox.Show("Operation completed.", "Success",
                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                Helpers.statusLabelMessage();

                if (mapEditor.mapEditorIsReady) {
                    mapEditor.updateBuildingListComboBox(mapEditor.interiorbldRadioButton.Checked);
                }
                Update();
            }
        }
        private void diamondAndPearlToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenCommandsDatabase(RomInfo.BuildCommandNamesDatabase(GameFamilies.DP), RomInfo.BuildCommandParametersDatabase(GameFamilies.DP),
                RomInfo.BuildActionNamesDatabase(GameFamilies.DP), RomInfo.BuildComparisonOperatorsDatabase(GameFamilies.DP));
        }
        private void platinumToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenCommandsDatabase(RomInfo.BuildCommandNamesDatabase(GameFamilies.Plat), RomInfo.BuildCommandParametersDatabase(GameFamilies.Plat),
                RomInfo.BuildActionNamesDatabase(GameFamilies.Plat), RomInfo.BuildComparisonOperatorsDatabase(GameFamilies.Plat));
        }
        private void heartGoldAndSoulSilverToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenCommandsDatabase(RomInfo.BuildCommandNamesDatabase(GameFamilies.HGSS), RomInfo.BuildCommandParametersDatabase(GameFamilies.HGSS),
                RomInfo.BuildActionNamesDatabase(GameFamilies.HGSS), RomInfo.BuildComparisonOperatorsDatabase(GameFamilies.HGSS));
        }

        private void mainTabControl_SelectedIndexChanged(object sender, EventArgs e) {
            if (mainTabControl.SelectedTab == headerEditorTabPage) {
                headerEditor.SetupHeaderEditor();
                return;
            }

            if (mainTabControl.SelectedTab == matrixEditorTabPage) {
                matrixEditor.SetupMatrixEditor();
                return;
            }

            if (mainTabControl.SelectedTab == mapEditorTabPage) {
                mapEditor.SetupMapEditor();
                return;
            }

            if (mainTabControl.SelectedTab == nsbtxEditorTabPage) {
                nsbtxEditor.SetupNSBTXEditor();
                return;
            }

            if (mainTabControl.SelectedTab == eventEditorTabPage) {
                eventEditor.SetupEventEditor();
                return;
            }

            if (mainTabControl.SelectedTab == scriptEditorTabPage) {
                scriptEditor.SetupScriptEditor();
                return;
            }

            if (mainTabControl.SelectedTab == levelScriptEditorTabPage) {
                levelScriptEditor.SetUpLevelScriptEditor();
                return;
            }

            if (mainTabControl.SelectedTab == textEditorTabPage) {
                textEditor.SetupTextEditor();
                return;
            }

            if (mainTabControl.SelectedTab == cameraEditorTabPage) {
                cameraEditor.SetupCameraEditor();
                return;
            }

            if (mainTabControl.SelectedTab == trainerEditorTabPage) {
                trainerEditor.SetupTrainerEditor();
                return;
            }

            if (mainTabControl.SelectedTab == tableEditorTabPage) {
                headerEditor.resetHeaderSearch();
                tableEditor.SetupTableEditor();
                return;
            }
        }

        private void spawnEditorToolStripButton_Click(object sender, EventArgs e) {
            matrixEditor.SetupMatrixEditor();

            using (SpawnEditor ed = new SpawnEditor(headerEditor.headerListBoxNames)) {
                ed.ShowDialog();
            }
        }
        private void spawnEditorToolStripMenuItem_Click(object sender, EventArgs e) {
            spawnEditorToolStripButton_Click(null, null);
        }
        private void wildEditorButton_Click(object sender, EventArgs e) {
            openWildEditor(loadCurrent: false);
        }

        private void openWildEditor(bool loadCurrent) {
            Helpers.statusLabelMessage("Attempting to extract Wild Encounters NARC...");
            Update();

            DSUtils.TryUnpackNarcs(new List<DirNames>() { DirNames.encounters, DirNames.monIcons });

            Helpers.statusLabelMessage("Passing control to Wild Pokémon Editor...");
            Update();

            int encToOpen = loadCurrent ? (int)headerEditor.wildPokeUpDown.Value : 0;

            string wildPokeUnpackedPath = gameDirs[DirNames.encounters].unpackedDir;
            switch (RomInfo.gameFamily) {
                case GameFamilies.DP:
                case GameFamilies.Plat:
                    using (WildEditorDPPt editor = new WildEditorDPPt(wildPokeUnpackedPath, RomInfo.GetPokemonNames(), encToOpen))
                        editor.ShowDialog();
                    break;
                default:
                    using (WildEditorHGSS editor = new WildEditorHGSS(wildPokeUnpackedPath, RomInfo.GetPokemonNames(), encToOpen))
                        editor.ShowDialog();
                    break;
            }
            Helpers.statusLabelMessage();
        }

        private void mapEditorTabPage_Enter(object sender, EventArgs e) {
            mapEditor.makeCurrent();
        }

        private void eventEditorTabPage_Enter(object sender, EventArgs e) {
            eventEditor.eventOpenGlControl.MakeCurrent();
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
                }

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
            }

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
    }
}