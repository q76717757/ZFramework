using System;

namespace ZFramework
{
    /// <summary>
    /// XYZ(整数)-S(可空字符串)    --> [1.0.1-Alpha]或者[1.0.1]之类的
    /// </summary>
    public struct VersionNum
    {
        /// <summary>
        /// 主版本号
        /// </summary>
        public int X;
        /// <summary>
        /// 子版本号
        /// </summary>
        public int Y;
        /// <summary>
        /// 修订版本号
        /// </summary>
        public int Z;
        /// <summary>
        /// 版本标识
        /// </summary>
        public string S;

        public VersionNum(int x,int y,int z,string suf = null)
        { 
            X = x;
            Y = y;
            Z = z;
            S = suf;
        }

        public static bool TryParse(string s, out VersionNum version)
        {
            if (!string.IsNullOrEmpty(s))
            {
                string[] parts = s.Split('.');
                if (parts.Length == 3)
                {
                    if (int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
                    {
                        string[] ex = parts[2].Split('-');
                        if (int.TryParse(ex[0], out int z))
                        {
                            if (ex.Length > 1)
                            {
                                version = new VersionNum(x, y, z, ex[1]);
                                return true;
                            }
                            else
                            {
                                version = new VersionNum(x, y, z);
                                return true;
                            }
                        }
                    }
                }
            }
            version = default;
            return false;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(S))
            {
                return ToNumString();
            }
            else
            {
                return $"{X}.{Y}.{Z}-{S}";
            }
        }
        public string ToNumString()
        { 
            return $"{X}.{Y}.{Z}";
        }

        #region 运算符重载
        public override bool Equals(object obj)
        {
            return (obj is VersionNum v) && this == v;
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        public static bool operator ==(VersionNum v1, VersionNum v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z;
        }
        public static bool operator !=(VersionNum v1, VersionNum v2)
        {
            return !(v1 == v2);
        }
        public static bool operator >(VersionNum v1, VersionNum v2)
        {
            return v1.X >= v2.X && v1.Y >= v2.Y && v1.Z > v2.Z;
        }
        public static bool operator <(VersionNum v1, VersionNum v2)
        {
            return v1.X <= v2.X && v1.Y <= v2.Y && v1.Z < v2.Z;
        }
        public static bool operator >=(VersionNum v1, VersionNum v2)
        {
            return !(v1 < v2);
        }
        public static bool operator <=(VersionNum v1, VersionNum v2)
        {
            return !(v1 > v2);
        }
        #endregion

    }
}
