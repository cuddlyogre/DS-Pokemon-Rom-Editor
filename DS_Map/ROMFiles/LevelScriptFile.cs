using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;

namespace DSPRE.ROMFiles {
  public class LevelScriptFile {
    public int ID;
    public BindingList<LevelScriptTrigger> bufferSet = new BindingList<LevelScriptTrigger>();

    public LevelScriptFile() { }

    public LevelScriptFile(int id) {
      this.ID = id;
      string path1 = Filesystem.scripts;
      string path = Path.Combine(path1, this.ID.ToString("D4"));
      parse_file(path);
    }

    public void parse_file(string path) {
      FileStream fs = new FileStream(path, FileMode.Open);
      using (BinaryReader br = new BinaryReader(fs)) {
        bool hasConditionalStructure = false;

        //conditionalStructureOffset is used to ensure the structure of the file is correct
        int conditionalStructureOffset = -1;

        while (true) {
          //first byte is the script type
          //if not a valid script type, break loop
          byte triggerType = br.ReadByte();
          if (!LevelScriptTrigger.IsValidTriggerType(triggerType)) break;

          //subtract triggerType length from conditionalStructureOffset
          if (hasConditionalStructure) conditionalStructureOffset -= sizeof(byte);

          //if trigger type is a variable value, that doesn't immediately mean we're processing that trigger
          //the trigger data is processed last if it is there
          if (triggerType == LevelScriptTrigger.VARIABLEVALUE) {
            hasConditionalStructure = true;
            conditionalStructureOffset = (int)br.ReadUInt32();
            continue;
          }

          //map screen load trigger doesn't have a value or variable
          uint scriptToTrigger = br.ReadUInt32();
          bufferSet.Add(new MapScreenLoadTrigger(triggerType, (int)scriptToTrigger));

          //subtract scriptToTrigger length from conditionalStructureOffset
          if (hasConditionalStructure) conditionalStructureOffset -= sizeof(UInt32);
        }

        //the earliest position a trigger can be
        const int SMALLEST_TRIGGER_SIZE = 5;

        //if triggerType is invalid
        //and next uint16 == 0
        //and the file stream length is shorter than the earliest position a trigger can be
        if (br.BaseStream.Position == 1 && br.ReadUInt16() == 0 && fs.Length < SMALLEST_TRIGGER_SIZE) {
          return;
          throw new InvalidDataException("This level script does nothing."); // "Interesting..."
        }

        //br.BaseStream.Position == 3
        //triggerType is valid,
        //stream position is earlier than the first possible trigger, or
        //there is no start script condition specified
        if (br.BaseStream.Position < SMALLEST_TRIGGER_SIZE) {
          throw new InvalidDataException("Parser failure: The input file you attempted to load is either malformed or not a Level Script file.");
        }

        //there are no instances of a variable value trigger
        if (!hasConditionalStructure) {
          return;
        }

        //if there's a variable value trigger but the offset is incorrect, the file is corrupt
        if (conditionalStructureOffset != 1) {
          throw new InvalidDataException($"Field error: The Level Script file you attempted to load is broken. {conditionalStructureOffset}");
        }

        //get the variable value trigger parts
        while (true) {
          //there are no variables below 1
          int variableID = br.ReadUInt16();
          if (variableID <= 0) break;

          int varExpectedValue = br.ReadUInt16();
          int scriptToTrigger = br.ReadUInt16();
          bufferSet.Add(new VariableValueTrigger(scriptToTrigger, variableID, varExpectedValue));
        }
      }
    }

    public long write_file(string path, bool word_alignment_padding = false) {
      FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
      using (BinaryWriter bw = new BinaryWriter(fs)) {
        HashSet<MapScreenLoadTrigger> mapScreenLoadTriggers = new HashSet<MapScreenLoadTrigger>();
        HashSet<VariableValueTrigger> variableValueTriggers = new HashSet<VariableValueTrigger>();

        foreach (LevelScriptTrigger item in bufferSet) {
          if (item is VariableValueTrigger variableValueTrigger) {
            variableValueTriggers.Add(variableValueTrigger);
          }
          else if (item is MapScreenLoadTrigger mapScreenLoadTrigger) {
            mapScreenLoadTriggers.Add(mapScreenLoadTrigger);
          }
        }

        foreach (MapScreenLoadTrigger item in mapScreenLoadTriggers) {
          bw.Write((byte)item.triggerType);
          bw.Write((UInt32)item.scriptTriggered);
        }

        if (variableValueTriggers.Count > 0) {
          bw.Write((byte)LevelScriptTrigger.VARIABLEVALUE);
          bw.Write((UInt32)1);
          bw.Write((byte)0);
          foreach (VariableValueTrigger item in variableValueTriggers) {
            bw.Write((UInt16)item.variableToWatch);
            bw.Write((UInt16)item.expectedValue);
            bw.Write((UInt16)item.scriptTriggered);
          }
        }

        bw.Write((UInt16)0);

        if (word_alignment_padding) {
          long missing_bytes = bw.BaseStream.Position % 4;
          for (int i = 0; i < 4 - missing_bytes; i++) {
            bw.Write((byte)0);
          }
        }

        return bw.BaseStream.Position;
      }
    }
  }

  public class LevelScriptTrigger {
    public const int VARIABLEVALUE = 1;
    public const int MAPCHANGE = 2;
    public const int SCREENRESET = 3;
    public const int LOADGAME = 4;

    private static int[] _triggerTypes;
    public int triggerType { get; set; }
    public int scriptTriggered { get; set; }

    public LevelScriptTrigger(int triggerType, int scriptTriggered) {
      this.triggerType = triggerType;
      this.scriptTriggered = scriptTriggered;
    }

    public static bool IsValidTriggerType(byte triggerType) {
      if (_triggerTypes == null) {
        _triggerTypes = new[] { VARIABLEVALUE, MAPCHANGE, SCREENRESET, LOADGAME };
      }

      return Array.IndexOf(_triggerTypes, triggerType) != -1;
    }

    public override string ToString() {
      return "Starts Script " + scriptTriggered;
    }
  }

  public class VariableValueTrigger : LevelScriptTrigger {
    public int variableToWatch { get; set; }
    public int expectedValue { get; set; }

    public VariableValueTrigger(int scriptIDtoTrigger, int variableToWatch, int expectedValue) : base(VARIABLEVALUE, scriptIDtoTrigger) {
      this.variableToWatch = variableToWatch;
      this.expectedValue = expectedValue;
    }

    public override string ToString() {
      return base.ToString() + " when Var " + variableToWatch + " == " + expectedValue;
    }

    public override bool Equals(object obj) {
      // If the passed object is null
      if (obj == null) {
        return false;
      }

      if (!(obj is VariableValueTrigger)) {
        return false;
      }

      return this.ToString() == ((VariableValueTrigger)obj).ToString();
    }

    public override int GetHashCode() {
      return this.triggerType.GetHashCode()^variableToWatch.GetHashCode()^expectedValue.GetHashCode();
    }
  }

  public class MapScreenLoadTrigger : LevelScriptTrigger {
    public MapScreenLoadTrigger(int type, int scriptTriggered) : base(type, scriptTriggered) { }

    public override string ToString() {
      switch (triggerType) {
        case LevelScriptTrigger.MAPCHANGE:
          return base.ToString() + " upon entering the LS map.";
        case LevelScriptTrigger.SCREENRESET:
          return base.ToString() + " when a fadescreen happens in the LS map.";
        case LevelScriptTrigger.LOADGAME:
          return base.ToString() + " when the game resumes in the LS map.";
        default:
          return base.ToString() + " under unknown circumstances.";
      }
    }

    public override bool Equals(object obj) {
      // If the passed object is null
      if (obj == null) {
        return false;
      }

      if (obj is MapScreenLoadTrigger) {
        return this.ToString() == ((MapScreenLoadTrigger)obj).ToString();
      }

      return false;
    }

    public override int GetHashCode() {
      return this.triggerType.GetHashCode()^scriptTriggered.GetHashCode();
    }
  }
}
