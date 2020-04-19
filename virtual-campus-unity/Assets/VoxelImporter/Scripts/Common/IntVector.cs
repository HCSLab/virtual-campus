using System;

namespace VoxelImporter
{
        [Serializable, System.Diagnostics.DebuggerDisplay("\"({x}, {y}\")")]
        public struct IntVector2
        {
            public IntVector2(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public static IntVector2 operator -(IntVector2 value)
            {
                return new IntVector2(-value.x, -value.y);
            }
            public static IntVector2 operator -(IntVector2 value1, IntVector2 value2)
            {
                return new IntVector2(value1.x - value2.x, value1.y - value2.y);
            }
            public static IntVector2 operator *(int scaleFactor, IntVector2 value)
            {
                return new IntVector2(scaleFactor * value.x, scaleFactor * value.y);
            }
            public static IntVector2 operator *(IntVector2 value, int scaleFactor)
            {
                return new IntVector2(value.x * scaleFactor, value.y * scaleFactor);
            }
            public static IntVector2 operator *(IntVector2 value1, IntVector2 value2)
            {
                return new IntVector2(value1.x * value2.x, value1.y * value2.y);
            }
            public static IntVector2 operator /(IntVector2 value, int divider)
            {
                return new IntVector2(value.x / divider, value.y / divider);
            }
            public static IntVector2 operator /(IntVector2 value1, IntVector2 value2)
            {
                return new IntVector2(value1.x / value2.x, value1.y / value2.y);
            }
            public static IntVector2 operator +(IntVector2 value1, IntVector2 value2)
            {
                return new IntVector2(value1.x + value2.x, value1.y + value2.y);
            }
            public static bool operator ==(IntVector2 value1, IntVector2 value2)
            {
                return value1.x == value2.x && value1.y == value2.y;
            }
            public static bool operator !=(IntVector2 value1, IntVector2 value2)
            {
                return value1.x != value2.x || value1.y != value2.y;
            }
            public static IntVector2 Max(IntVector2 value1, IntVector2 value2)
            {
                return new IntVector2(System.Math.Max(value1.x, value2.x), System.Math.Max(value1.y, value2.y));
            }
            public static IntVector2 Min(IntVector2 value1, IntVector2 value2)
            {
                return new IntVector2(System.Math.Min(value1.x, value2.x), System.Math.Min(value1.y, value2.y));
            }
            public static IntVector2 zero { get { return new IntVector2(0, 0); } }
            public static IntVector2 one { get { return new IntVector2(1, 1); } }
            public override int GetHashCode()
            {
                return x.GetHashCode() ^ y.GetHashCode();
            }
            public override bool Equals(System.Object obj)
            {
                if (!(obj is IntVector2)) return false;
                IntVector2 data = (IntVector2)obj;
                return this == data;
            }

            public int x, y;
        }
        [Serializable, System.Diagnostics.DebuggerDisplay("\"({x}, {y}, {z}\")")]
        public struct IntVector3
        {
            public IntVector3(int x, int y, int z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public static IntVector3 operator -(IntVector3 value)
            {
                return new IntVector3(-value.x, -value.y, -value.z);
            }
            public static IntVector3 operator -(IntVector3 value1, IntVector3 value2)
            {
                return new IntVector3(value1.x - value2.x, value1.y - value2.y, value1.z - value2.z);
            }
            public static IntVector3 operator *(int scaleFactor, IntVector3 value)
            {
                return new IntVector3(scaleFactor * value.x, scaleFactor * value.y, scaleFactor * value.z);
            }
            public static IntVector3 operator *(IntVector3 value, int scaleFactor)
            {
                return new IntVector3(value.x * scaleFactor, value.y * scaleFactor, value.z * scaleFactor);
            }
            public static IntVector3 operator *(IntVector3 value1, IntVector3 value2)
            {
                return new IntVector3(value1.x * value2.x, value1.y * value2.y, value1.z * value2.z);
            }
            public static IntVector3 operator /(IntVector3 value, int divider)
            {
                return new IntVector3(value.x / divider, value.y / divider, value.z / divider);
            }
            public static IntVector3 operator /(IntVector3 value1, IntVector3 value2)
            {
                return new IntVector3(value1.x / value2.x, value1.y / value2.y, value1.z / value2.z);
            }
            public static IntVector3 operator +(IntVector3 value1, IntVector3 value2)
            {
                return new IntVector3(value1.x + value2.x, value1.y + value2.y, value1.z + value2.z);
            }
            public static bool operator ==(IntVector3 value1, IntVector3 value2)
            {
                return value1.x == value2.x && value1.y == value2.y && value1.z == value2.z;
            }
            public static bool operator !=(IntVector3 value1, IntVector3 value2)
            {
                return value1.x != value2.x || value1.y != value2.y || value1.z != value2.z;
            }
            public static IntVector3 Max(IntVector3 value1, IntVector3 value2)
            {
                return new IntVector3(System.Math.Max(value1.x, value2.x), System.Math.Max(value1.y, value2.y), System.Math.Max(value1.z, value2.z));
            }
            public static IntVector3 Min(IntVector3 value1, IntVector3 value2)
            {
                return new IntVector3(System.Math.Min(value1.x, value2.x), System.Math.Min(value1.y, value2.y), System.Math.Min(value1.z, value2.z));
            }
            public static IntVector3 zero { get { return new IntVector3(0, 0, 0); } }
            public static IntVector3 one { get { return new IntVector3(1, 1, 1); } }
            public override int GetHashCode()
            {
                return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
            }
            public override bool Equals(System.Object obj)
            {
                if (!(obj is IntVector3)) return false;
                IntVector3 data = (IntVector3)obj;
                return this == data;
            }

            public int x, y, z;
        }
    [Serializable, System.Diagnostics.DebuggerDisplay("\"({x}, {y}, {z}, {w}\")")]
    public struct IntVector4
    {
        public IntVector4(int x, int y, int z, int w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static IntVector4 operator -(IntVector4 value)
        {
            return new IntVector4(-value.x, -value.y, -value.z, -value.w);
        }
        public static IntVector4 operator -(IntVector4 value1, IntVector4 value2)
        {
            return new IntVector4(value1.x - value2.x, value1.y - value2.y, value1.z - value2.z, value1.w - value2.w);
        }
        public static IntVector4 operator *(int scaleFactor, IntVector4 value)
        {
            return new IntVector4(scaleFactor * value.x, scaleFactor * value.y, scaleFactor * value.z, scaleFactor * value.w);
        }
        public static IntVector4 operator *(IntVector4 value, int scaleFactor)
        {
            return new IntVector4(value.x * scaleFactor, value.y * scaleFactor, value.z * scaleFactor, value.w * scaleFactor);
        }
        public static IntVector4 operator *(IntVector4 value1, IntVector4 value2)
        {
            return new IntVector4(value1.x * value2.x, value1.y * value2.y, value1.z * value2.z, value1.w * value2.w);
        }
        public static IntVector4 operator /(IntVector4 value, int divider)
        {
            return new IntVector4(value.x / divider, value.y / divider, value.z / divider, value.w / divider);
        }
        public static IntVector4 operator /(IntVector4 value1, IntVector4 value2)
        {
            return new IntVector4(value1.x / value2.x, value1.y / value2.y, value1.z / value2.z, value1.w / value2.w);
        }
        public static IntVector4 operator +(IntVector4 value1, IntVector4 value2)
        {
            return new IntVector4(value1.x + value2.x, value1.y + value2.y, value1.z + value2.z, value1.w + value2.w);
        }
        public static bool operator ==(IntVector4 value1, IntVector4 value2)
        {
            return value1.x == value2.x && value1.y == value2.y && value1.z == value2.z && value1.w == value2.w;
        }
        public static bool operator !=(IntVector4 value1, IntVector4 value2)
        {
            return value1.x != value2.x || value1.y != value2.y || value1.z != value2.z || value1.w != value2.w;
        }
        public static IntVector4 Max(IntVector4 value1, IntVector4 value2)
        {
            return new IntVector4(System.Math.Max(value1.x, value2.x), System.Math.Max(value1.y, value2.y), System.Math.Max(value1.z, value2.z), System.Math.Max(value1.w, value2.w));
        }
        public static IntVector4 Min(IntVector4 value1, IntVector4 value2)
        {
            return new IntVector4(System.Math.Min(value1.x, value2.x), System.Math.Min(value1.y, value2.y), System.Math.Min(value1.z, value2.z), System.Math.Min(value1.w, value2.w));
        }
        public static IntVector4 zero { get { return new IntVector4(0, 0, 0, 0); } }
        public static IntVector4 one { get { return new IntVector4(1, 1, 1, 1); } }
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode();
        }
        public override bool Equals(System.Object obj)
        {
            if (!(obj is IntVector4)) return false;
            IntVector4 data = (IntVector4)obj;
            return this == data;
        }

        public int x, y, z, w;
    }
}
