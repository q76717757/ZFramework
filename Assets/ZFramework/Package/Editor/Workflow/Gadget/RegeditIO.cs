using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;

namespace ZFramework.Editor
{
    public class RegeditIO
    {
        //把设置写到windows注册表  //Registry.LocalMachine 有权限问题  用CurrentUser则没有
        private static string ReadRegedit(string name)
        {
            try
            {
                RegistryKey hkml = Registry.CurrentUser;
                RegistryKey software = hkml.OpenSubKey("Software", true);
                RegistryKey aimdir = software.OpenSubKey("ZFramework", true);
                var values = aimdir.GetValue(name);
                return values.ToString();
            }
            catch (System.Exception)
            {
                return null;
            }
        }
        private static bool WriteRegedit(string name, string tovalue)
        {
            try
            {
                RegistryKey hklm = Registry.CurrentUser;
                RegistryKey software = hklm.OpenSubKey("Software", true);
                RegistryKey aimdir = software.OpenSubKey("ZFramework", true);
                if (aimdir == null)
                    aimdir = software.CreateSubKey("ZFramework");
                aimdir.SetValue(name, tovalue);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
            
        }
        private static void DeleteRegedit(string name)
        {
            string[] aimnames;
            RegistryKey hkml = Registry.CurrentUser;
            RegistryKey software = hkml.OpenSubKey("Software", true);
            RegistryKey aimdir = software.OpenSubKey("ZFramework", true);
            if (aimdir == null) return;
            aimnames = aimdir.GetSubKeyNames();
            foreach (string aimKey in aimnames)
            {
                if (aimKey == name)
                    aimdir.DeleteSubKeyTree(name);
            }
        }
        private static bool ContainsRegedit(string name)
        {
            string[] subkeyNames;
            RegistryKey hkml = Registry.CurrentUser;
            RegistryKey software = hkml.OpenSubKey("Software", true);
            RegistryKey aimdir = software.OpenSubKey("ZFramework", true);

            if (aimdir == null) return false;
            subkeyNames = aimdir.GetSubKeyNames();
            foreach (string keyName in subkeyNames)
            {
                if (keyName == name)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
