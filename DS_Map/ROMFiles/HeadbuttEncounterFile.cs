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
      string path = RomInfo.headbutt + "\\" + id.ToString("D4");
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

  public class HeadbuttEncounter {
    public ushort pokemonID;
    public byte minLevel;
    public byte maxLevel;

    public HeadbuttEncounter(BinaryReader br) {
      this.pokemonID = br.ReadUInt16();
      this.minLevel = br.ReadByte();
      this.maxLevel = br.ReadByte();
    }

    public override string ToString() {
      string[] pokemonNames = RomInfo.GetPokemonNames();
      string pokemon = pokemonNames[pokemonID];
      return $"{pokemonID,3} {pokemon,10}: {minLevel} - {maxLevel}";
    }
  }

  public class HeadbuttTreeGroup {
    const int treeCount = 6; //number of trees in each tree group

    public List<HeadbuttTree> trees = new List<HeadbuttTree>();

    public HeadbuttTreeGroup(BinaryReader br, HeadbuttTree.Types headbuttTreeType) {
      for (int j = 0; j < treeCount; j++) {
        trees.Add(new HeadbuttTree(br, headbuttTreeType));
      }
    }

    public HeadbuttTreeGroup(HeadbuttTree.Types headbuttTreeType) {
      for (int j = 0; j < treeCount; j++) {
        trees.Add(new HeadbuttTree(headbuttTreeType));
      }
    }

    public HeadbuttTreeGroup(HeadbuttTreeGroup original) {
      foreach (HeadbuttTree headbuttTree in original.trees) {
        trees.Add(new HeadbuttTree(headbuttTree));
      }
    }

    public override string ToString() {
      return $"{nameof(HeadbuttTreeGroup)}";
    }
  }

  public class HeadbuttTree {
    public HeadbuttTree.Types headbuttTreeType;

    private ushort _globalX;
    private ushort _globalY;
    private ushort _matrixX;
    private ushort _matrixY;
    private ushort _mapX;
    private ushort _mapY;

    public bool unused { get { return globalX == ushort.MaxValue && globalY == ushort.MaxValue; } }

    public enum Types {
      Normal,
      Special,
    }

    public HeadbuttTree(BinaryReader br, HeadbuttTree.Types headbuttTreeType) {
      this.globalX = br.ReadUInt16();
      this.globalY = br.ReadUInt16();
      this.headbuttTreeType = headbuttTreeType;
    }

    public HeadbuttTree(HeadbuttTree.Types headbuttTreeType, ushort globalX = ushort.MaxValue, ushort globalY = ushort.MaxValue) {
      this.globalX = globalX;
      this.globalY = globalY;
      this.headbuttTreeType = headbuttTreeType;
    }

    public HeadbuttTree(HeadbuttTree original) {
      this.globalX = original.globalX;
      this.globalY = original.globalY;
      this.headbuttTreeType = original.headbuttTreeType;
    }

    public ushort globalX {
      get { return _globalX; }
      set {
        _globalX = value;
        _matrixX = (ushort)(_globalX / MapFile.mapSize);
        _mapX = (ushort)(_globalX % MapFile.mapSize);
      }
    }

    public ushort globalY {
      get { return _globalY; }
      set {
        _globalY = value;
        _matrixY = (ushort)(_globalY / MapFile.mapSize);
        _mapY = (ushort)(_globalY % MapFile.mapSize);
      }
    }

    public ushort matrixX {
      get {
        return _matrixX;
      }
      set {
        _matrixX = value;
        _globalX = (ushort)(_matrixX * MapFile.mapSize + _mapX);
        _mapX = (ushort)(_globalX % MapFile.mapSize);
      }
    }

    public ushort matrixY {
      get {
        return _matrixY;
      }
      set {
        _matrixY = value;
        _globalY = (ushort)(_matrixY * MapFile.mapSize + _mapY);
        _mapY = (ushort)(_globalY % MapFile.mapSize);
      }
    }

    public ushort mapX {
      get {
        return _mapX;
      }
      set {
        _mapX = value;
        _globalX = (ushort)(_matrixX * MapFile.mapSize + _mapX);
        _matrixX = (ushort)(_globalX / MapFile.mapSize);
      }
    }

    public ushort mapY {
      get {
        return _mapY;
      }
      set {
        _mapY = value;
        _globalY = (ushort)(_matrixY * MapFile.mapSize + _mapY);
        _matrixY = (ushort)(_globalY / MapFile.mapSize);
      }
    }

    public override string ToString() {
      string suffix = unused ? "unused" : $"globalX: {globalX}, globalY: {globalY}";
      return $"{nameof(HeadbuttTree)} - {suffix}";
    }
  }
}
