using System.IO;

namespace DSPRE.ROMFiles {
    public class TrainerFile : RomFile {
        public const int POKE_IN_PARTY = 6;
        public static readonly string NAME_NOT_FOUND = "NAME READ ERROR";

        public string name;
        public TrainerProperties trp;
        public Party party;

        public TrainerFile(TrainerProperties trp, string name = ""){
            this.name = name;
            this.trp = trp;
            trp.partyCount = 1;
            this.party = new Party(1, init: true, trp);
        }
        public TrainerFile(TrainerProperties trp, Stream partyData, string name = "") {
            this.name = name;
            this.trp = trp;
            party = new Party(readFirstByte: false, POKE_IN_PARTY, partyData, this.trp);
        }

        public override byte[] ToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                writer.Write(name);

                byte[] trDat = trp.ToByteArray();
                writer.Write((byte)trDat.Length);
                writer.Write(trDat);

                byte[] pDat = party.ToByteArray();
                writer.Write((byte)pDat.Length);
                writer.Write(pDat);
            }
            return newData.ToArray();
        }

        public void SaveToFileExplorePath(string suggestedFileName, bool showSuccessMessage = true) {
            SaveToFileExplorePath("Gen IV Trainer File", "trf", suggestedFileName, showSuccessMessage);
        }
    }

}