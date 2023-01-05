using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace DSPRE.ROMFiles {
  //https://hirotdk.neocities.org/FileSpecs.html#Headbutt
  public class HeadbuttEncounterFile {
    public int ID;

    //get encounter tables, 12 normal pokemon definitions, 6 special pokemon definitions, 4 bytes per definition
    const int normalEncountersCount = 12;
    const int specialEncountersCount = 6;

    public byte normalTreeGroupsCount;
    public byte specialTreeGroupsCount;
    public bool hasTrees = false;
    public List<HeadbuttEncounter> normalEncounters;
    public List<HeadbuttEncounter> specialEncounters;
    public BindingList<HeadbuttTreeGroup> normalTreeGroups;
    public BindingList<HeadbuttTreeGroup> specialTreeGroups;

    public HeadbuttEncounterFile(int id) {
      this.ID = id;
      string path = Filesystem.GetHeadbuttPath(id);
      parse_file(path);
    }

    public HeadbuttEncounterFile(string path) {
      parse_file(path);
    }

    public void parse_file(string path) {
      FileStream fs = new FileStream(path, FileMode.Open);
      using (BinaryReader br = new BinaryReader(fs)) {
        //get the number of tree group definitions
        normalTreeGroupsCount = br.ReadByte();
        br.ReadByte(); //padding
        specialTreeGroupsCount = br.ReadByte();
        br.ReadByte(); //padding

        //if there are no trees defined in either section, there are no encounters or trees defined
        //instead of early return, use those methods to build an empty list
        hasTrees = normalTreeGroupsCount > 0 || specialTreeGroupsCount > 0;
        // if (!hasTrees) return;

        normalEncounters = getEncounters(br, normalEncountersCount, normalTreeGroupsCount); //48 bytes
        specialEncounters = getEncounters(br, specialEncountersCount, specialTreeGroupsCount); //24 bytes

        //tree groups have 6 sets of xy global coordinates x treeGroupsCount
        //coordinates need to be converted to matrix and local coordinates to be useful
        normalTreeGroups = getTreeGroups(br, normalTreeGroupsCount, HeadbuttTree.Types.Normal); //2 x normalTreeGroupsCount bytes
        specialTreeGroups = getTreeGroups(br, specialTreeGroupsCount, HeadbuttTree.Types.Special); //2 x specialTreeGroupsCount bytes
      }
    }

    public List<HeadbuttEncounter> getEncounters(BinaryReader br, int slots, int treeGroupsCount) {
      List<HeadbuttEncounter> encounters = new List<HeadbuttEncounter>();
      if (treeGroupsCount < 1) return encounters;

      encounters = new List<HeadbuttEncounter>();
      for (int i = 0; i < slots; i++) {
        encounters.Add(new HeadbuttEncounter(br));
      }

      return encounters;
    }

    //use logic from public Spawnable(Stream data)
    BindingList<HeadbuttTreeGroup> getTreeGroups(BinaryReader br, int treeGroupsCount, HeadbuttTree.Types headbuttTreeType) {
      BindingList<HeadbuttTreeGroup> treeGroups = new BindingList<HeadbuttTreeGroup>();
      if (treeGroupsCount < 1) return treeGroups;

      for (int i = 0; i < treeGroupsCount; i++) {
        treeGroups.Add(new HeadbuttTreeGroup(br, headbuttTreeType));
      }

      return treeGroups;
    }

    public byte[] ToByteArray() {
      MemoryStream newData = new MemoryStream();
      using (BinaryWriter writer = new BinaryWriter(newData)) {
        writer.Write((byte)normalTreeGroups.Count);
        writer.Write((byte)0);
        writer.Write((byte)specialTreeGroups.Count);
        writer.Write((byte)0);

        foreach (HeadbuttEncounter encounter in normalEncounters) {
          writer.Write((UInt16)encounter.pokemonID);
          writer.Write((byte)encounter.minLevel);
          writer.Write((byte)encounter.maxLevel);
        }

        foreach (HeadbuttEncounter encounter in specialEncounters) {
          writer.Write((UInt16)encounter.pokemonID);
          writer.Write((byte)encounter.minLevel);
          writer.Write((byte)encounter.maxLevel);
        }

        foreach (var treeGroup in normalTreeGroups) {
          foreach (var tree in treeGroup.trees) {
            writer.Write((UInt16)tree.globalX);
            writer.Write((UInt16)tree.globalY);
          }
        }

        foreach (var treeGroup in specialTreeGroups) {
          foreach (var tree in treeGroup.trees) {
            writer.Write((UInt16)tree.globalX);
            writer.Write((UInt16)tree.globalY);
          }
        }
      }

      return newData.ToArray();
    }

    public bool SaveToFile(string path, bool showSuccessMessage = true) {
      byte[] romFileToByteArray = ToByteArray();
      File.WriteAllBytes(path, romFileToByteArray);
      return true;
    }
  }
}
