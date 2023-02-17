/// <summary>
/// 消息弹窗面板
/// </summary>
public class MessagePanel : BasePanel
{
    public static readonly string path = "UI/MessagePanel/MessagePanel";
    public MessagePanel() : base(new UI_Info(path)) { }

    /// <summary>
    /// 弃用Base.OnEnter方法
    /// </summary>
    public override void OnEnter() { }
}
