namespace EC.UI
{
    public enum UIType : byte
    {
    }

    public enum UILayer : byte
    {
        Bottom = 0,
        BottomMiddle,
        Middle,
        MiddleTop,
        Top,
        TopMost,
    }

    public enum UIInstanceType : byte
    {
        SingleInstance = 0,
        MultipleInstance
    }
}
