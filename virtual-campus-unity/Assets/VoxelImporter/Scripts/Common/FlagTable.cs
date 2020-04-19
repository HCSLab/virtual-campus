using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace VoxelImporter
{
    public class FlagTable3
    {
        public FlagTable3(int reserveX = 0, int reserveY = 0, int reserveZ = 0)
        {
            reserve = new IntVector3(reserveX, reserveY, reserveZ);
        }

        public void Set(int x, int y, int z, bool flag)
        {
            Assert.IsTrue(x >= 0 && y >= 0 && z >= 0);
            int bIndex = Mathf.FloorToInt(z / 64f);
            var cIndex = z % 64;
            if(!flag)
            {
                if (table == null || x >= table.Length) return;
                if (table[x] == null || y >= table[x].Length) return;
                if (table[x][y] == null || bIndex >= table[x][y].Length) return;
            }
            #region Alloc
            reserve = IntVector3.Max(reserve, new IntVector3(x + 1, y + 1, z + 1));
            if (table == null)
            {
                table = new UInt64[reserve.x][][];
            }
            if (x >= table.Length)
            {
                var newTmp = new UInt64[x + 1][][];
                table.CopyTo(newTmp, 0);
                table = newTmp;
            }
            if(table[x] == null)
            {
                table[x] = new UInt64[reserve.y][];
            }
            if (y >= table[x].Length)
            {
                var newTmp = new UInt64[y + 1][];
                table[x].CopyTo(newTmp, 0);
                table[x] = newTmp;
            }
            if (table[x][y] == null)
            {
                table[x][y] = new UInt64[reserve.z];
            }
            if (bIndex >= table[x][y].Length)
            {
                var newTmp = new UInt64[bufferSize];
                table[x][y].CopyTo(newTmp, 0);
                table[x][y] = newTmp;
            }
            #endregion
            if (flag)
                table[x][y][bIndex] |= ((UInt64)1 << cIndex);
            else
                table[x][y][bIndex] &= ~((UInt64)1 << cIndex);
        }
        public void Set(IntVector3 pos, bool flag)
        {
            Set(pos.x, pos.y, pos.z, flag);
        }
        public bool Get(int x, int y, int z)
        {
            var bIndex = Mathf.FloorToInt(z / 64f);
            var cIndex = z % 64;
            if (table == null || x < 0 || x >= table.Length) return false;
            if (table[x] == null || y < 0 || y >= table[x].Length) return false;
            if (table[x][y] == null || bIndex < 0 || bIndex >= table[x][y].Length) return false;
            return (table[x][y][bIndex] & ((UInt64)1 << cIndex)) != 0;
        }
        public bool Get(IntVector3 pos)
        {
            return Get(pos.x, pos.y, pos.z);
        }
        public void Clear()
        {
            table = null;
        }

        public void AllAction(Action<int, int, int> action)
        {
            if (table == null) return;
            for (int x = 0; x < table.Length; x++)
            {
                if (table[x] == null) continue;
                for (int y = 0; y < table[x].Length; y++)
                {
                    if (table[x][y] == null) continue;
                    for (int bz = 0; bz < table[x][y].Length; bz++)
                    {
                        for (int i = 0; i < 64; i++)
                        {
                            var z = bz * 64 + i;
                            if (Get(x, y, z))
                            {
                                action(x, y, z);
                            }
                        }
                    }
                }
            }
        }

        private int bufferSize { get { return Mathf.CeilToInt(reserve.z / 64f); } }

        private IntVector3 reserve;
        private UInt64[][][] table;
    }
}
