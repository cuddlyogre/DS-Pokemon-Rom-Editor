using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPRE.ROMFiles {
  class SafariZoneEncounterFile {
    public int ID;

    public byte TallGrassObjectSlots;
    public byte SurfObjectSlots;
    public byte OldRodObjectSlots;
    public byte GoodRodObjectSlots;
    public byte SuperRodObjectSlots;

    private const int EncounterSlots = 10;

    public List<SafariZoneEncounter> TallGrassMorningEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneEncounter> TallGrassDayEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneEncounter> TallGrassNightEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneEncounter> TallGrassMorningObjectEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneEncounter> TallGrassDayObjectEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneEncounter> TallGrassNightObjectEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneObject> TallGrassObjects = new List<SafariZoneObject>();

    public List<SafariZoneEncounter> SurfMorningEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneEncounter> SurfDayEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneEncounter> SurfNightEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneEncounter> SurfMorningObjectEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneEncounter> SurfDayObjectEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneEncounter> SurfNightObjectEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneObject> SurfObjects = new List<SafariZoneObject>();

    public List<SafariZoneEncounter> OldRodMorningEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneEncounter> OldRodDayEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneEncounter> OldRodNightEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneEncounter> OldRodMorningObjectEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneEncounter> OldRodDayObjectEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneEncounter> OldRodNightObjectEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneObject> OldRodObjects = new List<SafariZoneObject>();

    public List<SafariZoneEncounter> GoodRodMorningEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneEncounter> GoodRodDayEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneEncounter> GoodRodNightEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneEncounter> GoodRodMorningObjectEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneEncounter> GoodRodDayObjectEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneEncounter> GoodRodNightObjectEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneObject> GoodRodObjects = new List<SafariZoneObject>();

    public List<SafariZoneEncounter> SuperRodMorningEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneEncounter> SuperRodDayEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneEncounter> SuperRodNightEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneEncounter> SuperRodMorningObjectEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneEncounter> SuperRodDayObjectEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneEncounter> SuperRodNightObjectEncounters = new List<SafariZoneEncounter>();
    public List<SafariZoneObject> SuperRodObjects = new List<SafariZoneObject>();

    public SafariZoneEncounterFile(int id) {
      this.ID = id;
      string path = Filesystem.GetSafariZonePath(id);
      parse_file(path);
    }

    public SafariZoneEncounterFile(string path) {
      parse_file(path);
    }

    public void parse_file(string path) {
      FileStream fs = new FileStream(path, FileMode.Open);
      using (BinaryReader br = new BinaryReader(fs)) {
        if (br.BaseStream.Length < 5) return;
        ReadObjectSlots(br);
        ReadTallGrass(br);
        ReadSurf(br);
        ReadOldRod(br);
        ReadGoodRod(br);
        ReadSuperRod(br);
      }
    }

    private void ReadObjectSlots(BinaryReader br) {
      //#1 Section - Object Arrangement Allocation
      TallGrassObjectSlots = br.ReadByte();
      SurfObjectSlots = br.ReadByte();
      OldRodObjectSlots = br.ReadByte();
      GoodRodObjectSlots = br.ReadByte();
      SuperRodObjectSlots = br.ReadByte();

      br.ReadByte();
      br.ReadByte();
      br.ReadByte();
    }

    private void ReadTallGrass(BinaryReader br) {
      //#2 Section - Tall Grass Encounters
      for (int i = 0; i < EncounterSlots; i++) {
        TallGrassMorningEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      for (int i = 0; i < EncounterSlots; i++) {
        TallGrassDayEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      for (int i = 0; i < EncounterSlots; i++) {
        TallGrassNightEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      //#3 Section - Tall Grass Encounters (Object Arrangement)
      for (int i = 0; i < TallGrassObjectSlots; i++) {
        TallGrassMorningObjectEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      for (int i = 0; i < TallGrassObjectSlots; i++) {
        TallGrassDayObjectEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      for (int i = 0; i < TallGrassObjectSlots; i++) {
        TallGrassNightObjectEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      //#4 Section - Object Arrangement Requirements (Tall Grass)
      for (int i = 0; i < TallGrassObjectSlots; i++) {
        TallGrassObjects.Add(new SafariZoneObject(br));
        TallGrassObjects.Add(new SafariZoneObject(br));
      }
    }

    private void ReadSurf(BinaryReader br) {
      //#5 Section - Surf Encounters
      for (int i = 0; i < EncounterSlots; i++) {
        SurfMorningEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      for (int i = 0; i < EncounterSlots; i++) {
        SurfDayEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      for (int i = 0; i < EncounterSlots; i++) {
        SurfNightEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      //#6 Section - Surf Encounters (Object Arrangement)
      for (int i = 0; i < SurfObjectSlots; i++) {
        SurfMorningObjectEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      for (int i = 0; i < SurfObjectSlots; i++) {
        SurfDayObjectEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      for (int i = 0; i < SurfObjectSlots; i++) {
        SurfNightObjectEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      //#7 Section - Object Arrangement Requirements (Surf)
      for (int i = 0; i < SurfObjectSlots; i++) {
        SurfObjects.Add(new SafariZoneObject(br));
        SurfObjects.Add(new SafariZoneObject(br));
      }
    }

    private void ReadOldRod(BinaryReader br) {
      //#8 Section - Old Rod Encounters
      for (int i = 0; i < EncounterSlots; i++) {
        OldRodMorningEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      for (int i = 0; i < EncounterSlots; i++) {
        OldRodDayEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      for (int i = 0; i < EncounterSlots; i++) {
        OldRodNightEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      //#9 Section - Old Rod Encounters (Object Arrangement)
      for (int i = 0; i < OldRodObjectSlots; i++) {
        OldRodMorningObjectEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      for (int i = 0; i < OldRodObjectSlots; i++) {
        OldRodDayObjectEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      for (int i = 0; i < OldRodObjectSlots; i++) {
        OldRodNightObjectEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      //#10 Section - Object Arrangement Requirements (Old Rod)
      for (int i = 0; i < OldRodObjectSlots; i++) {
        OldRodObjects.Add(new SafariZoneObject(br));
        OldRodObjects.Add(new SafariZoneObject(br));
      }
    }

    private void ReadGoodRod(BinaryReader br) {
      //#11 Section - Good Rod Encounters
      for (int i = 0; i < EncounterSlots; i++) {
        GoodRodMorningEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      for (int i = 0; i < EncounterSlots; i++) {
        GoodRodDayEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      for (int i = 0; i < EncounterSlots; i++) {
        GoodRodNightEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      //#12 Section - Good Rod Encounters (Object Arrangement)
      for (int i = 0; i < GoodRodObjectSlots; i++) {
        GoodRodMorningObjectEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      for (int i = 0; i < GoodRodObjectSlots; i++) {
        GoodRodDayObjectEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      for (int i = 0; i < GoodRodObjectSlots; i++) {
        GoodRodNightObjectEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      //#13 Section - Object Arrangement Requirements (Good Rod)
      for (int i = 0; i < GoodRodObjectSlots; i++) {
        GoodRodObjects.Add(new SafariZoneObject(br));
        GoodRodObjects.Add(new SafariZoneObject(br));
      }
    }

    private void ReadSuperRod(BinaryReader br) {
      //#14 Section - Super Rod Encounters
      for (int i = 0; i < EncounterSlots; i++) {
        SuperRodMorningEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      for (int i = 0; i < EncounterSlots; i++) {
        SuperRodDayEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      for (int i = 0; i < EncounterSlots; i++) {
        SuperRodNightEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      //#15 Section - Super Rod Encounters (Object Arrangement)
      for (int i = 0; i < SuperRodObjectSlots; i++) {
        SuperRodMorningObjectEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      for (int i = 0; i < SuperRodObjectSlots; i++) {
        SuperRodDayObjectEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      for (int i = 0; i < SuperRodObjectSlots; i++) {
        SuperRodNightObjectEncounters.Add(new SafariZoneEncounter(br));
        br.ReadByte();
      }

      //#16 Section - Object Arrangement Requirements (Super Rod)
      for (int i = 0; i < SuperRodObjectSlots; i++) {
        SuperRodObjects.Add(new SafariZoneObject(br));
        SuperRodObjects.Add(new SafariZoneObject(br));
      }
    }
  }
}
