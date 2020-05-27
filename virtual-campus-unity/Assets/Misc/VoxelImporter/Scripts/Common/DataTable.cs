using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace VoxelImporter
{
    public class DataTable3<Type>
    {
        public DataTable3(int reserveX = 0, int reserveY = 0, int reserveZ = 0)
        {
            reserve = new IntVector3(reserveX, reserveY, reserveZ);
            enable = new FlagTable3(reserveX, reserveY, reserveZ);
        }

        public void Set(int x, int y, int z, Type param)
        {
            Assert.IsTrue(x >= 0 && y >= 0 && z >= 0);
            #region Alloc
            reserve = IntVector3.Max(reserve, new IntVector3(x + 1, y + 1, z + 1));
            if (table == null)
            {
                table = new Type[reserve.x][][];
            }
            if (x >= table.Length)
            {
                var newTmp = new Type[reserve.x][][];
                table.CopyTo(newTmp, 0);
                table = newTmp;
            }
            if(table[x] == null)
            {
                table[x] = new Type[reserve.y][];
            }
            if (y >= table[x].Length)
            {
                var newTmp = new Type[reserve.y][];
                table[x].CopyTo(newTmp, 0);
                table[x] = newTmp;
            }
            if (table[x][y] == null)
            {
                table[x][y] = new Type[reserve.z];
            }
            if (z >= table[x][y].Length)
            {
                var newTmp = new Type[reserve.z];
                table[x][y].CopyTo(newTmp, 0);
                table[x][y] = newTmp;
            }
            #endregion
            table[x][y][z] = param;
            enable.Set(x, y, z, true);
        }
        public void Set(IntVector3 pos, Type param)
        {
            Set(pos.x, pos.y, pos.z, param);
        }
        public Type Get(int x, int y, int z)
        {
            if (!enable.Get(x, y, z)) return default(Type);
            return table[x][y][z];
        }
        public Type Get(IntVector3 pos)
        {
            return Get(pos.x, pos.y, pos.z);
        }
        public void Remove(int x, int y, int z)
        {
            if (!enable.Get(x, y, z)) return;
            enable.Set(x, y, z, false);
        }
        public void Remove(IntVector3 pos)
        {
            Remove(pos.x, pos.y, pos.z);
        }
        public void Clear()
        {
            table = null;
            enable.Clear();
        }
        public bool Contains(int x, int y, int z)
        {
            return enable.Get(x, y, z);
        }
        public bool Contains(IntVector3 pos)
        {
            return enable.Get(pos);
        }

        public void AllAction(Action<int, int, int, Type> action)
        {
            if (table == null) return;
            enable.AllAction((x, y, z) =>
            {
                action(x, y, z, table[x][y][z]);
            });
        }

        private IntVector3 reserve;
        private Type[][][] table;
        private FlagTable3 enable;
    }
}
