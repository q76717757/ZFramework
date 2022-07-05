using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public struct VersionInfo
    {
        public int a;
        public int b;
        public int c;
        public int d;

        public bool IsInvalid
        {
            get
            {
                return !(a >= 0 && b >= 0 && c >= 0 && d > 0);
            }
        }

        public static VersionInfo Zero
        {
            get {
                return new VersionInfo();
            }
        }

        public VersionInfo(string versionStr)
        {
            if (!string.IsNullOrEmpty(versionStr))
            {
                var aa = versionStr.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (aa.Length == 4)
                {
                    if (int.TryParse(aa[0], out a) && int.TryParse(aa[1],out b) && int.TryParse(aa[2],out c) && int.TryParse(aa[3],out d))
                    {
                        return;
                    }
                }
            }
            this.a = 0;
            this.b = 0;
            this.c = 0;
            this.d = 0;
        }
        public VersionInfo(int a, int b, int c, int d)
        { 
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }


        public static VersionInfo operator +(VersionInfo a, VersionInfo b)
        {
            return new VersionInfo()
            {
                 a = a.a + b.a,
                 b = a.b + b.b, 
                 c = a.c + b.c,
                 d = a.d + b.d,
            };
        }
        public static bool operator ==(VersionInfo a, VersionInfo b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(VersionInfo a, VersionInfo b)
        {
            return !a.Equals(b);
        }
        public static bool operator >(VersionInfo a, VersionInfo b)
        {
            return a.a > b.a || a.b > b.b || a.c > b.c || a.d > b.d;
        }
        public static bool operator <(VersionInfo a, VersionInfo b)
        {
            return a.a < b.a || a.b < b.b || a.c < b.c || a.d < b.d;
        }
        public static bool operator >=(VersionInfo a, VersionInfo b)
        {
            return a < b;
        }
        public static bool operator <=(VersionInfo a, VersionInfo b)
        {
            return a > b;
        }

        public override int GetHashCode()
        {
            return a.GetHashCode() ^ (b.GetHashCode() << 4) ^ (c.GetHashCode() << 12) ^ d.GetHashCode();
        }
        public override bool Equals(object other)
        {
            if (!(other is VersionInfo))
            {
                return false;
            }
            return Equals((VersionInfo)other);
        }
        public bool Equals(VersionInfo other)
        {
            return a == other.a && b == other.b && c == other.c && d == other.d;
        }
        public override string ToString()
        {
            return $"{a}.{b}.{c}.{d}";
        }
    }
}
