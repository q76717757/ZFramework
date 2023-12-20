namespace ZFramework
{
    public class UISettingAttribute : BaseAttribute
    {
        public string AssetPath { get; }
        public UIGroupType GroupType { get; }
        public UISettingAttribute(string assetPath, UIGroupType groupType)
        {
            AssetPath = assetPath;
            GroupType = groupType;
        }
    }
}
