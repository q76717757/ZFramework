using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
#if UNITY_EDITOR
public class FileEx
{
    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    public static extern int SHILCreateFromPath(string pszPath, out IntPtr ppIdl, ref uint rgflnOut);
    // 获取根文件夹的 PIDL
    static IntPtr GetRootFolderPIDL(string path)
    {
        IntPtr pidl;
        uint attributes = 0;
        if (SHILCreateFromPath(path, out pidl, ref attributes) == 0)
        {
            return pidl;
        }
        return IntPtr.Zero;
    }
    public static string OpenFolderBrowser(string path)
    {
        FolderBrowser.FolderBrowserDialog dlg = new FolderBrowser.FolderBrowserDialog();
        // 设置对话框的所有者窗口句柄（可选）
        dlg.hwndOwner = GetForegroundWindow();

        IntPtr defaultPath = GetRootFolderPIDL(path);
        // defaultPath = IntPtr.Zero;
        // 设置根文件夹（可选）
        dlg.pidlRoot = defaultPath/* IntPtr.Zero*/;
        // 设置显示名称（可选）
        dlg.displayName = null;
        // 设置对话框标题（可选）
        dlg.title = "Select Folder";
        // 设置标志位（可选）
        dlg.flags = 0;
        // 设置回调函数（可选）
        dlg.callback = IntPtr.Zero;
        // 设置图标索引（可选）
        dlg.image = 0;
        // 设置默认根文件夹（可选）
        dlg.root = IntPtr.Zero;
        // 调用文件夹浏览器对话框
        if (FolderBrowser.ShowFolderBrowserDialog(ref dlg))
        {
            // 获取选定的文件夹路径
            // Debug.Log("Selected folder: " + dlg.folderPath);
            // 释放根文件夹的 PIDL 资源
            if (defaultPath != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(defaultPath);
            }
            return dlg.folderPath;
        }
        else
        {
            // 用户取消了选择
            //   Debug.Log("Selection cancelled.");
            // 释放根文件夹的 PIDL 资源
            if (defaultPath != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(defaultPath);
            }
            return "";
        }
    }

    // 获取前台窗口句柄的函数
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    /// <summary>
    /// 递归复制文件夹
    /// </summary>
    /// <param name="sourceFolderPath"></param>
    /// <param name="destinationFolderPath"></param>
    /// <returns> 0成功  1目标路径已存在该文件夹</returns>
    public static int CopyFolder(string sourceFolderPath, string destinationFolderPath)
    {
        try
        {
            if (Directory.Exists(destinationFolderPath))
            {
                return 1;
            }
            // 创建目标文件夹
            Directory.CreateDirectory(destinationFolderPath);

            // 获取源文件夹中的所有文件和子文件夹
            string[] files = Directory.GetFiles(sourceFolderPath);
            string[] subDirectories = Directory.GetDirectories(sourceFolderPath);

            // 拷贝文件
            foreach (string filePath in files)
            {
                string fileName = Path.GetFileName(filePath);
                string destinationFilePath = Path.Combine(destinationFolderPath, fileName);
                File.Copy(filePath, destinationFilePath, true);
            }

            // 递归拷贝子文件夹
            foreach (string subDirectoryPath in subDirectories)
            {
                string directoryName = Path.GetFileName(subDirectoryPath);
                string destinationSubFolderPath = Path.Combine(destinationFolderPath, directoryName);
                CopyFolder(subDirectoryPath, destinationSubFolderPath);
            }
            Debug.Log("ProjectSettings_Copy:Succeed");
        }
        catch (IOException e)
        {
            Debug.LogError("ProjectSettings_Copy_Failed:" + e.Message);
        }
        return 0;
    }

    public static void OpenFolder(string path)
    {
        Application.OpenURL(path);
    }

}
public class FolderBrowser
{
    // 结构体定义
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct FolderBrowserDialog
    {
        public IntPtr hwndOwner;
        public IntPtr pidlRoot;
        public string displayName;
        public string title;
        public uint flags;
        public IntPtr callback;
        public int image;
        public IntPtr root;
        public string folderPath;
    }

    // API 函数定义
    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr SHBrowseForFolder(ref FolderBrowserDialog dlg);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    public static extern bool SHGetPathFromIDList(IntPtr pidl, IntPtr pszPath);

    // Helper 方法
    public static bool ShowFolderBrowserDialog(ref FolderBrowserDialog dlg)
    {
        IntPtr pidl = SHBrowseForFolder(ref dlg);
        if (pidl != IntPtr.Zero)
        {
            IntPtr pszPath = Marshal.AllocHGlobal(260);
            if (SHGetPathFromIDList(pidl, pszPath))
            {
                dlg.folderPath = Marshal.PtrToStringAuto(pszPath);
                Marshal.FreeHGlobal(pszPath);
                return true;
            }
        }
        return false;
    }
}
#endif