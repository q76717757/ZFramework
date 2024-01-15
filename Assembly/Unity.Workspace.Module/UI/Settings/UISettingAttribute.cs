namespace ZFramework
{
    public class UISettingAttribute : BaseAttribute
    {
        public string AssetPath { get; }
        public UILayer Layer { get; }
        public UISettingAttribute(string assetPath, UILayer layer)
        {
            AssetPath = assetPath;
            Layer = layer;
        }

    }
}
