namespace ZFramework
{
    public class UISettingAttribute : BaseAttribute
    {
        public string AssetPath { get; }
        public UISortLayer Layer { get; }
        public UISettingAttribute(string assetPath, UISortLayer layer)
        {
            AssetPath = assetPath;
            Layer = layer;
        }

    }
}
