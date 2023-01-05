using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DSPRE.ROMFiles {
    /* ---------------------- WILD POKÉMON DATA STRUCTURE (DPPt):----------------------------
        
       0x0  //  byte:       Walking encounter rate
       0x4  //  byte:       Level of
       0x2  //  ushort:     Matrix number
       0x4  //  ushort:     Script file number
       0x6  //  ushort:     Level script file number
       0x8  //  ushort:     Text Archive number
       0xA  //  ushort:     Day music track number
       0xC  //  ushort:     Night music track number
       0xE  //  ushort:     Wild Pokemon file number
       0x10 //  ushort:     Event file number

       * Diamond/Pearl:
       0x12 //  ushort:     Index of map name in Text Archive #382 (US version)   
       
       * Platinum:
       0x12 //  byte:       Index of map name in Text Archive #382 (US version)
       0x13 //  byte:       Map name textbox type value

       0x14 //  byte:       Weather value
       0x15 //  byte:       Camera value
       0x16 //  byte:       Boolean flag: show name when entering map
       0x17 //  byte:       Bitwise permission flags:

       -----------------    1: Allow Fly
       -----------------    2: ?
       -----------------    3: ?
       -----------------    4: Allow Bike usage
       -----------------    5: ?
       -----------------    6: ?
       -----------------    7: Esc. Rope
       -----------------    8: ?

    /* ---------------------- WILD POKÉMON DATA STRUCTURE (HGSS):----------------------------
        
       0x0  //  byte:       Wild Pokemon file number
       0x1  //  byte:       Area data value
       0x2  //  byte:       ?
       0x3  //  byte:       ?
       0x4  //  ushort:     Matrix number
       0x6  //  ushort:     Script file number
       0x8  //  ushort:     Level script file
       0xA  //  ushort:     Text Archive number
       0xC  //  ushort:     Day music track number
       0xE  //  ushort:     Night music track number
       0x10 //  ushort:     Event file number
       0x12 //  byte:       Index of map name in Text Archive #382 (US version)
       0x13 //  byte:       Map name textbox type value
       0x14 //  byte:       Weather value
       0x15 //  byte:       Camera value
       0x16 //  byte:       Follow mode (for the Pokemon following hero)
       0x17 //  byte:       Bitwise permission flags:

       -----------------    1: Allow Fly
       -----------------    2: ?
       -----------------    3: ?
       -----------------    4: Allow Bike usage
       -----------------    5: ?
       -----------------    6: ?
       -----------------    7: Esc. Rope
       -----------------    8: ?

    ----------------------------------------------------------------------------------*/

    /// <summary>
    /// General class to store common wild Pokemon data across all Gen IV Pokemon NDS games
    /// </summary>
    public abstract class EncounterFile : RomFile {
        public const string msgFixed = " (already fixed)";
        public const string extension = "wld";
        #region Fields (19)

        /* Encounter rates */
        public byte goodRodRate { get; set; }
        public byte oldRodRate { get; set; }
        public byte superRodRate { get; set; }
        public byte surfRate { get; set; }
        public byte walkingRate { get; set; }

        /* Levels */
        public byte[] goodRodMaxLevels = new byte[5];
        public byte[] goodRodMinLevels = new byte[5];
        public byte[] oldRodMaxLevels = new byte[5];
        public byte[] oldRodMinLevels = new byte[5];
        public byte[] walkingLevels = new byte[12];
        public byte[] superRodMaxLevels = new byte[5];
        public byte[] superRodMinLevels = new byte[5];
        public byte[] surfMaxLevels = new byte[5];
        public byte[] surfMinLevels = new byte[5];

        /* Encounters */
        public ushort[] goodRodPokemon = new ushort[5];
        public ushort[] oldRodPokemon = new ushort[5];
        public ushort[] superRodPokemon = new ushort[5];
        public ushort[] surfPokemon = new ushort[5];
        public ushort[] swarmPokemon { get; set; }  //2 for DPPt, 4 for HGSS
        #endregion

        #region Methods (1)
        public void SaveToFileDefaultDir(int IDtoReplace, bool showSuccessMessage = true) {
            SaveToFileDefaultDir(RomInfo.DirNames.encounters, IDtoReplace, showSuccessMessage);
        }

        public void ReportErrors(List<string> errorList) {
            string fullError = "The following sections of this encounter file couldn't be read correctly: " + Environment.NewLine;

            string errorSections = "";
            foreach (string elem in errorList) {
                errorSections += "- " + elem + Environment.NewLine;
            }
            fullError += errorSections;

            fullError += Environment.NewLine + "It is recommended that you check them before resaving.";
            
            if (errorSections.Contains(msgFixed)) {
                fullError += Environment.NewLine + "Fields marked as " + '\'' + msgFixed + '\'' + " have been repaired with a value of 0.";
            }

            MessageBox.Show(fullError, "Encounter File error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion
    }
}