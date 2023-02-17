/// <summary>
/// UI��Ϣ
/// </summary>
public class UI_Info
{
    /// <summary>
    /// UI����
    /// UI·��
    /// </summary>
    public string Name { get; private set; }
    public string Path { get; private set; }

    /// <summary>
    /// ���캯��
    /// </summary>
    /// <param name="path">UI·��</param>
    public UI_Info(string path)
    {
        //UI����Ϊ·��ĩβ���ļ���
        Name = path.Substring(path.LastIndexOf('/') + 1);
        Path = path;
    }
}
