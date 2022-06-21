namespace ZFramework
{
    public enum UILayer : byte
    {
        Background,//最底层
        Normal,//主要展示层
        Top,//弹窗遮罩层

        //遮罩使用模态开关  在任何层都可用  不用遮罩层  比如弹窗层模态挡所有层   normal层模态挡bg层  bg层模态按栈遮挡
    }

}
