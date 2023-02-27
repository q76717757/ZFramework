/// <summary>
/// UI信息
/// </summary>
public class UI_Info
{
    /// <summary>
    /// UI名字
    /// UI路径
    /// </summary>
    public string Name { get; private set; }
    public string Path { get; private set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="path">UI路径</param>
    public UI_Info(string path)
    {
        //UI名字为路径末尾的文件名
        Name = path.Substring(path.LastIndexOf('/') + 1);
        Path = path;
    }
}
