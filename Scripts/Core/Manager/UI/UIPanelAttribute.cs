using System;

namespace EC.UI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UIPanelAttribute : Attribute
    {
        public UIType uIType;
        public UILayer uILayer;
        public UIInstanceType uIInstanceType;
        public bool openWithDarkMask;

        public UIPanelAttribute(UIType uIType, UILayer uILayer,UIInstanceType uIInstanceType,bool openWithDarkMask = false)
        {
            this.uIType = uIType;
            this.uILayer = uILayer;
            this.uIInstanceType = uIInstanceType;
            this.openWithDarkMask = openWithDarkMask;
        }
    }

    public struct UIPanelAttributeInfo
    {
        public readonly Type type;
        public readonly UIType uIType;
        public readonly UILayer uILayer;
        public readonly UIInstanceType uIInstanceType;
        public readonly bool openWithDarkMask;

        public UIPanelAttributeInfo(Type type, UIType uIType, UILayer uILayer,UIInstanceType uIInstanceType, bool openWithDarkMask)
        {
            this.type = type;
            this.uIType = uIType;
            this.uILayer = uILayer;
            this.uIInstanceType = uIInstanceType;
            this.openWithDarkMask = openWithDarkMask;
        }
    }

}
