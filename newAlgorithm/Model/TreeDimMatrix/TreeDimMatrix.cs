using System.Collections.Generic;
using System.ComponentModel;

namespace newAlgorithm.Model
{
    public class TreeDimMatrix
    {
        public List<TreeDimMatrixNode> treeDimMatrix = new List<TreeDimMatrixNode>();
        public int deviceCount = 0;
        public Dictionary<int, int> lastPosition = new Dictionary<int, int>();

        public TreeDimMatrix(int deviceCount)
        {
            this.deviceCount = deviceCount;
            for (int i = 1; i <= deviceCount; i++)
            {
                lastPosition.Add(i, 0);
            }
        }

        public TreeDimMatrix AddNode(int deviceNumber, int type, int position, int count)
        {
            TreeDimMatrixNode node = new TreeDimMatrixNode(deviceNumber, type, position, count);
            treeDimMatrix.Add(node);

            ++lastPosition[deviceNumber];

            return this;
        }

        public TreeDimMatrixNode Find(int deviceNumber, int type, int position)
        {
            foreach (TreeDimMatrixNode node in treeDimMatrix)
            {
                if (node.DeviceNumber == deviceNumber && node.Type == type && node.Position == position)
                    return node;
            }

            return null;
        }

        public int GetValue(int deviceNumber, int type, int position)
        {
            foreach (TreeDimMatrixNode node in treeDimMatrix)
            {
                if (node.DeviceNumber == deviceNumber && node.Type == type && node.Position == position)
                    return node.Count;
            }

            return 0;
        }
    }
}
