using System.Collections.Generic;
using System.IO;

namespace DSPRE.ROMFiles {
    public class HeadbuttTreeGroup {
        const int treeCount = 6; //number of trees in each tree group
        public readonly List<HeadbuttTree> trees = new List<HeadbuttTree>();

        public HeadbuttTreeGroup(BinaryReader br) {
            for (int i = 0; i < treeCount; i++) {
                ushort x = br.ReadUInt16();
                ushort y = br.ReadUInt16();
                trees.Add(new HeadbuttTree(x, y));
            }
        }

        public HeadbuttTreeGroup() {
            for (int i = 0; i < treeCount; i++) {
                trees.Add(new HeadbuttTree());
            }
        }

        public HeadbuttTreeGroup(HeadbuttTreeGroup original) {
            foreach (HeadbuttTree headbuttTree in original.trees) {
                trees.Add(new HeadbuttTree(headbuttTree));
            }
        }

        public override string ToString() {
            if (trees.Count == 0) {
                return "Empty Tree Group";
            }
            return $"Tree Group at {trees[0].globalX}, {trees[0].globalY}";
        }
    }
}
