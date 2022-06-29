using System;
using System.Runtime.InteropServices;

/*
 * 基本属性
InitialDirectory 对话框的初始目录
Filter 要在对话框中显示的文件筛选器，例如，"文本文件(*.txt)|*.txt|所有文件(*.*)||*.*"
FilterIndex 在对话框中选择的文件筛选器的索引，如果选第一项就设为1
RestoreDirectory 控制对话框在关闭之前是否恢复当前目录
FileName 获取或设置一个包含在文件对话框中选定的文件名的字符串。
Title 将显示在对话框标题栏中的字符
AddExtension 是否自动添加默认扩展名
CheckPathExists 在对话框返回之前，检查指定路径是否存在
DefaultExt 默认扩展名
DereferenceLinks 在从对话框返回前是否取消引用快捷方式
ShowHelp 启用"帮助"按钮
ValiDateNames 控制对话框检查文件名中是否不含有无效的字符或序列

    OpenFileDialog
    SaveFileDialog
Filter 要在对话框中显示的文件筛选器，例如，"文本文件(*.txt)|*.txt|所有文件(*.*)|*.*"
FilterIndex 在对话框中选择的文件筛选器的索引，如果选第一项就设为1
RestoreDirectory 控制对话框在关闭之前是否恢复当前目录
AddExtension 是否自动添加默认扩展名
CheckFileExists 获取或设置一个值，该值指示如果用户指定不存在的文件名，对话框是否显示警告。
CheckPathExists 在对话框返回之前，检查指定路径是否存在
Container 控制在将要创建文件时，是否提示用户。只有在ValidateNames为真值时，才适用。
DefaultExt 缺省扩展名
DereferenceLinks 在从对话框返回前是否取消引用快捷方式
FileName 获取或设置一个包含在文件对话框中选定的文件名的字符串。
InitialDirector 对话框的初始目录
OverwritePrompt 控制在将要在改写现在文件时是否提示用户，只有在ValidateNames为真值时，才适用
ShowHelp 启用"帮助"按钮
Title 将显示在对话框标题栏中的字符
ValidateNames 控制对话框检查文件名中是否不含有无效的字符或序列

    */

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
public static class WindowsEx
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class WindowsDialog
    {
        public int structSize = 0;
        public IntPtr dlgOwner = IntPtr.Zero;
        public IntPtr instance = IntPtr.Zero;
        public String filter = null;
        public String customFilter = null;
        public int maxCustFilter = 0;
        public int filterIndex = 0;
        public String file = null;
        public int maxFile = 0;
        public String fileTitle = null;
        public int maxFileTitle = 0;
        public String initialDir = null;
        public String title = null;
        public int flags = 0;
        public short fileOffset = 0;
        public short fileExtension = 0;
        public String defExt = null;
        public IntPtr custData = IntPtr.Zero;
        public IntPtr hook = IntPtr.Zero;
        public String templateName = null;
        public IntPtr reservedPtr = IntPtr.Zero;
        public int reservedInt = 0;
        public int flagsEx = 0;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    private static extern IntPtr GetForegroundWindow();//获取当前焦点窗口句柄

    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    private static extern bool GetOpenFileName([In, Out] WindowsDialog ofn);

    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    private static extern bool GetSaveFileName([In, Out] WindowsDialog ofn);

    [DllImport("shell32.dll", ExactSpelling = true)]
    private static extern void ILFree(IntPtr pidlList);

    [DllImport("shell32.dll",CharSet = CharSet.Unicode, ExactSpelling = true)]
    private static extern IntPtr ILCreateFromPathW(string pszPath);

    [DllImport("shell32.dll",ExactSpelling = true)]
    private static extern int SHOpenFolderAndSelectItems(IntPtr pidlList, uint cild, IntPtr children, uint dwFlags);


    public static string LoadFilePath(string filter,bool 允许多选 = false) {
        WindowsDialog openFile = new WindowsDialog();
        openFile.dlgOwner = GetForegroundWindow();//*设置父窗口句柄  这样才可以模态
        openFile.structSize = Marshal.SizeOf(openFile);
        openFile.filter = filter;  //"XLS文件(*.xls)\0*.xls\0\0            XLSX文件(*.xlsx)\0*.xlsx\0\0";
        openFile.file = new string(new char[256]);
        openFile.maxFile = openFile.file.Length;
        openFile.fileTitle = new string(new char[64]);
        openFile.maxFileTitle = openFile.fileTitle.Length;
        openFile.initialDir = System.IO.Path.Combine(UnityEngine.Application.streamingAssetsPath, "TempSyncRoomData").Replace('/', '\\');
        openFile.title = "读取文件";

        openFile.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
        if (允许多选)
        {
            openFile.flags |= 0x00000200;
        }

        //https://docs.microsoft.com/zh-cn/windows/win32/api/commdlg/ns-commdlg-openfilenamea flags内容

        if (GetOpenFileName(openFile))//读取文件
        {
            return openFile.file;
        }
        return null;
    }
    public static string SaveFilePath(string ext,string filter) {//默认后缀 xls
        WindowsDialog ofn = new WindowsDialog();
        ofn.dlgOwner = GetForegroundWindow();
        ofn.structSize = Marshal.SizeOf(ofn);
        ofn.filter = filter;
        ofn.file = new string(new char[256]);
        ofn.maxFile = ofn.file.Length;
        ofn.fileTitle = new string(new char[64]);
        ofn.maxFileTitle = ofn.fileTitle.Length;
        ofn.initialDir = UnityEngine.Application.dataPath.Replace('/', '\\');//默认路径
        ofn.title = "保存文件";
        ofn.defExt = ext;//默认拓展名  不用点  比如xls
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 /*| 0x00000200*/ | 0x00000008;//x200多选标记 多选会变成文件夹
        if (GetSaveFileName(ofn))
        {
            return ofn.file;
        }
        return null;
    }
    public static void OpenFolderAndSelectItem(string filePath)
    {
        IntPtr pidlList = ILCreateFromPathW(filePath.Replace("/", "\\"));
        if (pidlList != IntPtr.Zero)
        {
            try
            {
                Marshal.ThrowExceptionForHR(SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0));
            }
            catch
            {
                ILFree(pidlList);//释放句柄
            }
        }
    }
    public static void OpenFolder(string path)
    {
        UnityEngine.Application.OpenURL($"file:///{path}");
    }
}
#endif
