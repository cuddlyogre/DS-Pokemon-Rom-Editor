using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace DSPRE.ROMFiles {
  /* ---------------------- HEADER DATA STRUCTURE (DPPt):----------------------------
      
     0x0  //  byte:       Area data value
     0x1  //  byte:       Unknown value
     0x2  //  ushort:     Matrix number
     0x4  //  ushort:     Script file number
     0x6  //  ushort:     Level script file number
     0x8  //  ushort:     Text Archive number
     0xA  //  ushort:     Day music track number
     0xC  //  ushort:     Night music track number
     0xE  //  ushort:     Wild Pokémon file number
     0x10 //  ushort:     Event file number

     * D/P:
     0x12 //  ushort:     Index of map name in Text Archive #382 (US version)   
     
     * Plat:
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

  /* ---------------------- HEADER DATA STRUCTURE (HGSS):----------------------------
      
     0x0  //  byte:       Wild Pokémon file number
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
     0x16 //  byte:       Follow mode (for the Pokémon following hero)
     0x17 //  byte:       Bitwise permission flags:

      DPPT
     -----------------    1: Allow Fly
     -----------------    2: Allow Esc. Rope
     -----------------    3: Allow Running
     -----------------    4: Allow Bike 
     -----------------    5: Battle BG b4
     -----------------    6: Battle BG b3
     -----------------    7: Battle BG b2
     -----------------    8: Battle BG b1

      HGSS
     -----------------    1: ?
     -----------------    2: ?
     -----------------    3: ?
     -----------------    4: Allow Fly 
     -----------------    5: Allow Esc. Rope
     -----------------    6: ?
     -----------------    7: Allow Bicycle
     -----------------    8: ?

  ----------------------------------------------------------------------------------*/

  /// <summary>
  /// General class to store common map header data across all Gen IV Pokémon NDS games
  /// </summary>
  public abstract class MapHeader : RomFile {
    /*System*/
    public static readonly string DefaultFilter = "DSPRE Header File (*.dsh; *.bin)|*.dsh;*.bin";
    public ushort ID { get; set; }
    public static readonly byte length = 24;
    public static readonly string nameSeparator = " -   ";

    public enum SearchableFields : byte {
      AreaDataID,
      CameraAngleID,
      EventFileID,
      InternalName,
      LevelScriptID,
      MatrixID,
      MusicDayID,

      //MusicDayName,
      MusicNightID,

      //MusicNightName,
      ScriptFileID,
      TextArchiveID,
      WeatherID,
    };
    /**/

    public byte areaDataID { get; set; }
    public byte cameraAngleID { get; set; }
    public ushort eventFileID { get; set; }
    public ushort levelScriptID { get; set; }
    public ushort matrixID { get; set; }
    public ushort scriptFileID { get; set; }
    public ushort musicDayID { get; set; }
    public ushort musicNightID { get; set; }
    public byte locationSpecifier { get; set; }
    public byte battleBackground { get; set; }
    public ushort textArchiveID { get; set; }
    public byte weatherID { get; set; }
    public byte flags { get; set; }
    public ushort wildPokemon { get; set; }

    public static MapHeader LoadFromByteArray(byte[] headerData, ushort headerNumber, RomInfo.GameFamilies gameFamily = RomInfo.GameFamilies.NULL) {
      /* Encapsulate header data into the class appropriate for the gameVersion */
      if (headerData.Length < MapHeader.length) {
        MessageBox.Show("File of header " + headerNumber + " is too small and can't store header data.", "Header file too small", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return null;
      }

      if (gameFamily == RomInfo.GameFamilies.NULL) {
        gameFamily = RomInfo.gameFamily;
      }

      switch (gameFamily) {
        case RomInfo.GameFamilies.DP:
          return new MapHeaderDP(headerNumber, new MemoryStream(headerData));
        case RomInfo.GameFamilies.Plat:
          return new MapHeaderPt(headerNumber, new MemoryStream(headerData));
        default:
          return new MapHeaderHGSS(headerNumber, new MemoryStream(headerData));
      }
    }

    public static MapHeader LoadFromFile(string filename, ushort headerNumber, long offsetInFile, RomInfo.GameFamilies gameFamily = RomInfo.GameFamilies.NULL) {
      /* Calculate header offset and load data */
      byte[] headerData = DSUtils.ReadFromFile(filename, offsetInFile, MapHeader.length);
      return LoadFromByteArray(headerData, headerNumber, gameFamily);
    }

    public static MapHeader LoadFromARM9(ushort headerNumber, RomInfo.GameFamilies gameFamily = RomInfo.GameFamilies.NULL) {
      long headerOffset = RomInfo.headerTableOffset + MapHeader.length * headerNumber;
      return LoadFromFile(RomInfo.arm9Path, headerNumber, headerOffset, gameFamily);
    }

    public static MapHeader GetMapHeader(ushort headerNumber) {
      MapHeader currentHeader;
      /* Check if dynamic headers patch has been applied, and load header from arm9 or a/0/5/0 accordingly */
      if (ROMToolboxDialog.DynamicHeadersPatchApplied) {
        string path = Filesystem.GetDynamicHeaderPath(headerNumber);
        currentHeader = MapHeader.LoadFromFile(path, headerNumber, 0);
      }
      else {
        currentHeader = MapHeader.LoadFromARM9(headerNumber);
      }

      return currentHeader;
    }

    public static int GetHeaderCount() {
      int headerCount;
      if (ROMToolboxDialog.DynamicHeadersPatchApplied) {
        headerCount = Filesystem.GetDynamicHeadersCount();
      }
      else {
        headerCount = RomInfo.GetHeaderCount();
      }

      return headerCount;
    }

    public void SaveFile() {
      /* Check if dynamic headers patch has been applied, and save header to arm9 or a/0/5/0 accordingly */
      if (ROMToolboxDialog.DynamicHeadersPatchApplied) {
        string path = Filesystem.GetDynamicHeaderPath(ID);
        DSUtils.WriteToFile(path, this.ToByteArray(), 0, 0, fmode: FileMode.Create);
      }
      else {
        uint headerOffset = (uint)(RomInfo.headerTableOffset + MapHeader.length * this.ID);
        DSUtils.ARM9.WriteBytes(this.ToByteArray(), headerOffset);
      }
    }
  }
}
