using System.IO;
using System.Windows.Forms;
using LibNDSFormats.NSBMD;

namespace DSPRE.ROMFiles {
  /// <summary>
  /// Class to store building data from Pokémon NDS games
  /// </summary>
  public class Building {
    public NSBMD NSBMDFile;
    public uint modelID { get; set; }
    public short xPosition { get; set; }
    public short yPosition { get; set; }
    public short zPosition { get; set; }
    public ushort xFraction { get; set; }
    public ushort yFraction { get; set; }
    public ushort zFraction { get; set; }
    public ushort xRotation { get; set; }
    public ushort yRotation { get; set; }
    public ushort zRotation { get; set; }
    public uint width { get; set; }
    public uint height { get; set; }
    public uint length { get; set; }

    public Building(Stream data) {
      using (BinaryReader reader = new BinaryReader(data)) {
        modelID = reader.ReadUInt32();

        xFraction = reader.ReadUInt16();
        xPosition = reader.ReadInt16();
        yFraction = reader.ReadUInt16();
        yPosition = reader.ReadInt16();
        zFraction = reader.ReadUInt16();
        zPosition = reader.ReadInt16();
                
        xRotation = reader.ReadUInt16();
        reader.BaseStream.Position += 0x2;
        yRotation = reader.ReadUInt16();
        reader.BaseStream.Position += 0x2;
        zRotation = reader.ReadUInt16();
        reader.BaseStream.Position += 0x2;

        reader.BaseStream.Position += 0x1;

        width = reader.ReadUInt16();
        reader.BaseStream.Position += 0x2;
        height = reader.ReadUInt16();
        reader.BaseStream.Position += 0x2;
        length = reader.ReadUInt16();
                
        //reader.BaseStream.Position += 0x2;
      }
    }
    public Building() {
      modelID = 0;
      xFraction = 0;
      xPosition = 0;
      yFraction = 0;
      yPosition = 1;
      zFraction = 0;
      zPosition = 0;

      xRotation = yRotation = zRotation = 0;
      width = 16;
      height = 16;
      length = 16;
    }

    public Building(Building toCopy) {
      modelID = toCopy.modelID;
      xFraction = toCopy.xFraction;
      xPosition = toCopy.xPosition;
      yFraction = toCopy.yFraction;
      yPosition = (short)(toCopy.yPosition + 1);
      zFraction = toCopy.zFraction;
      zPosition = toCopy.zPosition;

      xRotation = toCopy.xRotation;
      yRotation = toCopy.yRotation;
      zRotation = toCopy.zRotation;

      width = toCopy.width;
      height = toCopy.height;
      length = toCopy.length;
    }

    public static ushort DegToU16(float deg) {
      return (ushort)(deg * 65536 / 360);
    }
    public static float U16ToDeg(ushort u16) {
      return (float)u16 * 360 / 65536;
    }
    public void LoadModelData(bool interior) {
      string modelPath = Filesystem.GetBuildingModelPath(interior, (int)modelID);

      if (string.IsNullOrWhiteSpace(modelPath) || !File.Exists(modelPath)) {
        MessageBox.Show("Building " + modelID + " could not be found in\n" + '"' + Path.GetDirectoryName(modelPath) + '"', "Building not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }
      try {
        using (Stream fs = new FileStream(modelPath, FileMode.Open)) {
          this.NSBMDFile = NSBMDLoader.LoadNSBMD(fs);
        }
      } catch (FileNotFoundException) {
        MessageBox.Show("Building " + modelID + " could not be found in\n" + '"' + Path.GetDirectoryName(modelPath) + '"', "Building not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
  }
}
