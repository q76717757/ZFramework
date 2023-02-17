using UnityEngine;
public class StartPanel : BasePanel
{
    public static readonly string path = "UI/StartPanel/StartPanel";
    public StartPanel() : base(new UI_Info(path)) {  }

    public override void OnEnter()
    {
        base.OnEnter();
        UserInterface.Instance.Fade(this, new LoginPanel(), 3);

    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}
