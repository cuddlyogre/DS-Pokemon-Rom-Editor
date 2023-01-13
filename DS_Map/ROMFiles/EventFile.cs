using System.Collections.Generic;
using System.IO;

namespace DSPRE.ROMFiles {
  /// <summary>
  /// Classes to store event data in Pokémon NDS games
  /// </summary>
  public class EventFile : RomFile {
    public enum SerializationOrders {
      Spawnables,
      Overworlds,
      Warps,
      Triggers
    }

    public static readonly string DefaultFilter = "Event File (*.evt, *.ev)|*.evt;*.ev";

    public List<Spawnable> spawnables = new List<Spawnable>();
    public List<Overworld> overworlds = new List<Overworld>();
    public List<Warp> warps = new List<Warp>();
    public List<Trigger> triggers = new List<Trigger>();

    public EventFile() { }

    public EventFile(int ID) {
      string path = Filesystem.GetEventPath(ID);
      Stream data = new FileStream(path, FileMode.Open);
      LoadFile(data);
    }

    public EventFile(Stream data) {
      LoadFile(data);
    }

    void LoadFile(Stream data) {
      using (BinaryReader reader = new BinaryReader(data)) {
        /* Read spawnables */
        uint spawnablesCount = reader.ReadUInt32();
        for (int i = 0; i < spawnablesCount; i++) {
          spawnables.Add(new Spawnable(new MemoryStream(reader.ReadBytes(0x14))));
        }

        /* Read overworlds */
        uint overworldsCount = reader.ReadUInt32();
        for (int i = 0; i < overworldsCount; i++) {
          overworlds.Add(new Overworld(new MemoryStream(reader.ReadBytes(0x20))));
        }

        /* Read warps */
        uint warpsCount = reader.ReadUInt32();
        for (int i = 0; i < warpsCount; i++) {
          warps.Add(new Warp(new MemoryStream(reader.ReadBytes(0xC))));
        }

        /* Read triggers */
        uint triggersCount = reader.ReadUInt32();
        for (int i = 0; i < triggersCount; i++) {
          triggers.Add(new Trigger(new MemoryStream(reader.ReadBytes(0x10))));
        }
      }
    }

    public override string ToString() {
      return base.ToString();
    }

    public override byte[] ToByteArray() {
      MemoryStream newData = new MemoryStream();
      using (BinaryWriter writer = new BinaryWriter(newData)) {
        /* Write spawnables */
        writer.Write((uint)spawnables.Count);
        for (int i = 0; i < spawnables.Count; i++) {
          writer.Write(spawnables[i].ToByteArray());
        }

        /* Write overworlds */
        writer.Write((uint)overworlds.Count);
        for (int i = 0; i < overworlds.Count; i++) {
          writer.Write(overworlds[i].ToByteArray());
        }

        /* Write warps */
        writer.Write((uint)warps.Count);
        for (int i = 0; i < warps.Count; i++) {
          writer.Write(warps[i].ToByteArray());
        }

        /* Write triggers */
        writer.Write((uint)triggers.Count);
        for (int i = 0; i < triggers.Count; i++) {
          writer.Write(triggers[i].ToByteArray());
        }
      }

      return newData.ToArray();
    }

    public void SaveToFileDefaultDir(int IDtoReplace, bool showSuccessMessage = true) {
      SaveToFileDefaultDir(RomInfo.DirNames.eventFiles, IDtoReplace, showSuccessMessage);
    }

    public void SaveToFileExplorePath(string suggestedFileName, bool showSuccessMessage = true) {
      SaveToFileExplorePath("Gen IV Event File", "ev", suggestedFileName, showSuccessMessage);
    }

    internal bool isEmpty() => (spawnables is null || spawnables.Count == 0) &&
                               (overworlds is null || overworlds.Count == 0) &&
                               (warps is null || warps.Count == 0) &&
                               (triggers is null || triggers.Count == 0);
  }
}
