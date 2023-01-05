using System.IO;

namespace DSPRE.ROMFiles {
    /// <summary>
    /// Class to store area data in Pokémon NDS games
    /// </summary>
    public class AreaData : RomFile {
        internal static readonly byte TYPE_INDOOR = 0;
        internal static readonly byte TYPE_OUTDOOR = 1;

        public ushort buildingsTileset;
        public ushort mapTileset;
        public ushort dynamicTextureType;
        public ushort unknown1;
        public byte areaType = TYPE_OUTDOOR; //HGSS ONLY
        public ushort lightType; //using an overabundant size. HGSS only needs a byte

        public AreaData(byte ID) {
            string path = Filesystem.GetAreaDataPath(ID);
            Stream data = new FileStream(path, FileMode.Open);
            LoadFile(data);
        }

        public AreaData(Stream data) {
            LoadFile(data);
        }

        void LoadFile(Stream data) {
            using (BinaryReader reader = new BinaryReader(data)) {
                buildingsTileset = reader.ReadUInt16();
                mapTileset = reader.ReadUInt16();

                if (RomInfo.gameFamily == RomInfo.GameFamilies.HGSS) {
                    dynamicTextureType = reader.ReadUInt16();
                    areaType = reader.ReadByte();
                    lightType = reader.ReadByte();
                }
                else {
                    unknown1 = reader.ReadUInt16();
                    lightType = reader.ReadUInt16();
                }
            }
        }

        public override byte[] ToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                writer.Write(buildingsTileset);
                writer.Write(mapTileset);

                if (RomInfo.gameFamily == RomInfo.GameFamilies.HGSS) {
                    writer.Write(dynamicTextureType);
                    writer.Write(areaType);
                    writer.Write((byte)lightType);
                } else {
                    writer.Write(unknown1);
                    writer.Write((ushort)lightType);
                }
            }
            return newData.ToArray();
        }

        public void SaveToFileDefaultDir(int IDtoReplace, bool showSuccessMessage = true) {
            SaveToFileDefaultDir(RomInfo.DirNames.areaData, IDtoReplace, showSuccessMessage);
        }

        public void SaveToFileExplorePath(string suggestedFileName, bool showSuccessMessage = true) {
            SaveToFileExplorePath("Gen IV Area Data File", "bin", suggestedFileName, showSuccessMessage);
        }
    }
}