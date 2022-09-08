using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public struct VersionInfo
    {
        public int X;
        public int Y;
        public int Z;
        public int W;
        public string suffix;

        public override string ToString()
        {
            if (string.IsNullOrEmpty(suffix))
            {
                return $"{X}.{Y}.{Z}.{W}";
            }
            else
            {
                return $"{X}.{Y}.{Z}.{W}-{suffix}";
            }
        }


        public static bool TryParse(string s,out VersionInfo version)
        {
            if (!string.IsNullOrEmpty(s))
            {
                string[] parts = s.Split('.');
                if (parts.Length == 4)
                {
                    if (int.TryParse(parts[0],out int x) && 
                        int.TryParse(parts[1], out int y) &&
                        int.TryParse(parts[2], out int z))
                    {
                        string[] ex = parts[3].Split('-');
                        if (ex.Length == 1 && int.TryParse(ex[0], out int w))
                        {
                            version = new VersionInfo()
                            {
                                X = x,
                                Y = y,
                                Z = z,
                                W = w,
                                suffix = string.Empty
                            };
                            return true;
                        }
                        if (ex.Length > 1 && int.TryParse(ex[0], out w))
                        {
                            version = new VersionInfo()
                            {
                                X = x,
                                Y = y,
                                Z = z,
                                W= w,
                                suffix = ex[1]
                            };
                            return true;
                        }
                    }
                }
            }
            version = default;
            return false;
        }

    }
}
