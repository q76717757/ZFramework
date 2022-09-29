using System;

namespace ZFramework
{
    /// <summary>
    /// 版本号取四位整数 + 杠字符串后缀(可选)  [1.0.1.1-Alpha]  或者  [1.0.1.1] 都是合法的
    /// </summary>
    public struct VersionInfo
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
        /// 内部版本号
        /// </summary>
        public int Z;
        /// <summary>
        /// 修订版本号
        /// </summary>
        public int W;
        /// <summary>
        /// 版本后缀
        /// </summary>
        public string S;

        public VersionInfo(int x,int y,int z,int w,string suf = null)
        { 
            X = x;
            Y = y;
            Z = z;
            W = w;
            S = suf;
        }

        public static bool TryParse(string s, out VersionInfo version)
        {
            if (!string.IsNullOrEmpty(s))
            {
                string[] parts = s.Split('.');
                if (parts.Length == 4)
                {
                    if (int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y) && int.TryParse(parts[2], out int z))
                    {
                        string[] ex = parts[3].Split('-');
                        if (ex.Length == 1 && int.TryParse(ex[0], out int w))
                        {
                            version = new VersionInfo(x, y, z, w);
                            return true;
                        }
                        if (ex.Length > 1 && int.TryParse(ex[0], out w))
                        {
                            version = new VersionInfo(x, y, z, w, ex[1]);
                            return true;
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
                return $"{X}.{Y}.{Z}.{W}";
            }
            else
            {
                return $"{X}.{Y}.{Z}.{W}-{S}";
            }
        }

        #region 运算符重载
        public override bool Equals(object obj)
        {
            return (obj is VersionInfo v) && this == v;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public static bool operator ==(VersionInfo v1, VersionInfo v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z && v1.W == v2.W;
        }
        public static bool operator !=(VersionInfo v1, VersionInfo v2)
        {
            return !(v1 == v2);
        }
        public static bool operator >(VersionInfo v1, VersionInfo v2)
        {
            return v1.X >= v2.X && v1.Y >= v2.Y && v1.Z >= v2.Z && v1.W > v2.W;
        }
        public static bool operator <(VersionInfo v1, VersionInfo v2)
        {
            return v1.X <= v2.X && v1.Y <= v2.Y && v1.Z <= v2.Z && v1.W < v2.W;
        }
        public static bool operator >=(VersionInfo v1, VersionInfo v2)
        {
            return !(v1 < v2);
        }
        public static bool operator <=(VersionInfo v1, VersionInfo v2)
        {
            return !(v1 > v2);
        }
        #endregion

    }
}
