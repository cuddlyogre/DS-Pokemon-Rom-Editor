using System.Collections.Generic;
using System.IO;

namespace DSPRE.ROMFiles {
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
}
