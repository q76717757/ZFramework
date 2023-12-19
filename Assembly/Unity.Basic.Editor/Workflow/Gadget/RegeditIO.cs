using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Security.Permissions;
using Microsoft.Win32;

namespace ZFramework.Editor
{
    public class RegeditIO
    {
        //权限检查
        static void Check()
        {
            try
            {
                RegistryPermission permission = new RegistryPermission(RegistryPermissionAccess.Read, "HKEY_LOCAL_MACHINE\\Software");
                permission.Demand();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        //把设置写到windows注册表  //Registry.LocalMachine 有权限问题  用CurrentUser则没有
        private static string ReadRegedit(string name)
        {
            // 读取注册表中的一个键值
            RegistryKey key = null;
            try
            {
                key = Registry.CurrentUser.OpenSubKey("Software\\AppName");
                if (key != null)
                {
                    string value = (string)key.GetValue("KeyName");
                    // TODO: 对读取到的值进行处理

                    return value;
                }
            }
            catch (SecurityException ex)
            {
                // 当前用户没有权限读取注册表，处理异常

                throw ex;
            }
            finally
            {
                if (key != null)
                    key.Close();
            }
            return default;
        }
        private static bool WriteRegedit(string name, string tovalue)
        {
            // 写入注册表中的一个键值
            RegistryKey key = null;
            try
            {
                key = Registry.CurrentUser.CreateSubKey("Software\\AppName");
                if (key != null)
                {
                    key.SetValue("KeyName", "value");
                    // TODO: 写入完成后进行处理

                    return true;
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                // 当前用户没有权限写入注册表，处理异常
                throw ex;
            }
            finally
            {
                if (key != null)
                    key.Close();
            }
            return false;
        }

        private static void DeleteRegedit(string name)
        {
            // 删除键
            try
            {
                Registry.CurrentUser.DeleteSubKeyTree("Software\\AppName");
            }
            catch (Exception ex)
            {
                // 处理删除失败的情况
            }
        }

        private static bool ContainsRegedit(string name)
        {
            //查询键
            try
            {
                var key = Registry.CurrentUser.OpenSubKey("Software\\AppName");
                var subKeys = key.GetSubKeyNames();
                foreach (var item in subKeys)
                {
                    if (item == name)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
