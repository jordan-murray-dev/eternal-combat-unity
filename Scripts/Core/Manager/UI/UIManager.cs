using EC.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace EC
{
    public class UIManager : IManager
    {
        private Dictionary<UILayer, List<BaseUIPanel>> m_OpenedPanels;
        private Stack<BaseUIPanel> m_OpenedPanelsWithDarkMask;
        private Dictionary<UIType, Stack<BaseUIPanel>> m_CachedPanels;
        private Dictionary<UIType, UIPanelAttributeInfo> m_AttributeInfos;
        private Dictionary<UILayer, RectTransform> m_LayerTransforms;
        private UIDarkMask m_DarkMask;
        private RectTransform m_Root;
        private List<BaseUIPanel> _Buffer;

        #region Init

        public void OnInit()
        {
            int uiLayerLength = Enum.GetValues(typeof(UILayer)).Length;
            int uiTypeLength = Enum.GetValues(typeof(UIType)).Length;

            m_OpenedPanels = new Dictionary<UILayer, List<BaseUIPanel>>(uiLayerLength);
            m_OpenedPanelsWithDarkMask = new Stack<BaseUIPanel>();
            m_CachedPanels = new Dictionary<UIType, Stack<BaseUIPanel>>(uiTypeLength);
            m_AttributeInfos = new Dictionary<UIType, UIPanelAttributeInfo>(uiTypeLength);
            m_LayerTransforms = new Dictionary<UILayer, RectTransform>(uiLayerLength);
            m_DarkMask = null;
            m_Root = null;
            _Buffer = new List<BaseUIPanel>(uiTypeLength);

            //InitRoot();
            //InitDarkMask();
            //InitLayerTransforms();
            InitAttributeInfos();
            InitOpenedPanels();
            InitCachedPanels();
        }

        private void InitOpenedPanels()
        {
            Array values = Enum.GetValues(typeof(UILayer));
            foreach (UILayer layer in values)
            {
                m_OpenedPanels.Add(layer, new List<BaseUIPanel>());
            }
        }

        private void InitCachedPanels()
        {
            Array values = Enum.GetValues(typeof(UIType));
            foreach (UIType uIType in values)
            {
                UIPanelAttributeInfo info = m_AttributeInfos[uIType];
                if (info.uIInstanceType == UIInstanceType.SingleInstance)
                    m_CachedPanels.Add(uIType, new Stack<BaseUIPanel>(1));
                else
                    m_CachedPanels.Add(uIType, new Stack<BaseUIPanel>());
            }
        }

        /*private void InitRoot()
        {
            m_Root = GameObject.Find("UIRoot").GetComponent<RectTransform>();
            GameObject.DontDestroyOnLoad(m_Root.gameObject);
        }

        private void InitDarkMask()
        {
            m_DarkMask = m_Root.Find("DarkMask").GetComponent<UIDarkMask>();
        }

        private void InitLayerTransforms()
        {
            Array values = Enum.GetValues(typeof(UILayer));
            foreach (UILayer layer in values)
            {
                m_LayerTransforms.Add(layer, m_Root.Find(layer.ToString()).GetComponent<RectTransform>());
            }
        }*/

        private void InitAttributeInfos()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (Type type in assembly.GetTypes())
            {
                object[] attrs = type.GetCustomAttributes(typeof(UIPanelAttribute), false);

                if (attrs.Length <= 0)
                    continue;

                if (!type.IsSubclassOf(typeof(BaseUIPanel)))
                    continue;

                UIPanelAttribute uIPanelAttribute = (UIPanelAttribute)attrs[0];
                UIPanelAttributeInfo uIPanelAttributeInfo = new UIPanelAttributeInfo(
                    type, uIPanelAttribute.uIType, uIPanelAttribute.uILayer, uIPanelAttribute.uIInstanceType, uIPanelAttribute.openWithDarkMask);
                m_AttributeInfos.Add(uIPanelAttribute.uIType, uIPanelAttributeInfo);
            }
        }

        #endregion

        public void OnStart()
        {

        }

        public void Open(UIType uIType,object param=null)
        {
            bool canOpen = CanOpen(uIType);
            if (!canOpen)
                return;

            BaseUIPanel baseUIPanel;
            UIPanelAttributeInfo attributeInfo = m_AttributeInfos[uIType];
            RectTransform parentRectTrans = m_LayerTransforms[attributeInfo.uILayer];
            if (m_CachedPanels.ContainsKey(uIType) && m_CachedPanels[uIType].Count > 0)
            {
                baseUIPanel = m_CachedPanels[uIType].Pop();
            }
            else
            {
                baseUIPanel = Activator.CreateInstance(attributeInfo.type) as BaseUIPanel;
                baseUIPanel.Init(parentRectTrans);
            }

            m_OpenedPanels[attributeInfo.uILayer].Add(baseUIPanel);

            //Black mask
            if (attributeInfo.openWithDarkMask)
            {
                m_DarkMask.Open();
                m_DarkMask.SetParent(m_Root);
                m_DarkMask.SetSiblingIndex(parentRectTrans, baseUIPanel.RectTrans.GetSiblingIndex());
                m_OpenedPanelsWithDarkMask.Push(baseUIPanel);
            }

            baseUIPanel.RectTrans.SetAsLastSibling();
            baseUIPanel.Open(param);
        }

        public void Close(UIType uIType, object param = null)
        {
            if(!IsOpen(uIType))
                return;
            UIPanelAttributeInfo info = m_AttributeInfos[uIType];
            List<BaseUIPanel> uIPanels = m_OpenedPanels[info.uILayer];
            if (uIPanels.Count <= 0)
                return;
            BaseUIPanel baseUIPanel= uIPanels[uIPanels.Count - 1];
            Close(baseUIPanel, param);
        }

        public void Close(BaseUIPanel baseUIPanel, object param = null)
        {
            if(baseUIPanel==null)
                return;
            if (!baseUIPanel.RequestClose())
                return;

            UIType uIType = baseUIPanel.UIType;
            UIPanelAttributeInfo info = m_AttributeInfos[uIType];
            List<BaseUIPanel> uIPanels = m_OpenedPanels[info.uILayer];
            bool contains=false;
            for (int i = 0; i < uIPanels.Count; i++)
            {
                BaseUIPanel item = uIPanels[i];
                if (item == baseUIPanel)
                {
                    contains = true;
                    break;
                }
            }
            if (!contains)
                return;

            baseUIPanel.Close(param);

            if (info.openWithDarkMask)
            {
                m_DarkMask.Close();
                bool equalToPeek= m_OpenedPanelsWithDarkMask.Peek() == baseUIPanel;
                if (!equalToPeek)
                    throw new GameException("Invalid peek exceptopm");
                m_OpenedPanelsWithDarkMask.Pop();
                if(m_OpenedPanelsWithDarkMask.Count>0)
                {
                    BaseUIPanel peekPanel = m_OpenedPanelsWithDarkMask.Peek();
                    m_DarkMask.Open();
                    m_DarkMask.SetParent(m_Root);
                    m_DarkMask.SetSiblingIndex(peekPanel.ParentRectTrans, peekPanel.RectTrans.GetSiblingIndex());
                }
            }

            uIPanels.Remove(baseUIPanel);
            m_CachedPanels[uIType].Push(baseUIPanel);
        }

        public bool CanOpen(UIType uIType)
        {
            UIPanelAttributeInfo info = m_AttributeInfos[uIType];
            if (info.uIInstanceType == UIInstanceType.SingleInstance)
                return IsOpen(uIType) ? false : true;
            else
                return true;
        }

        public bool IsOpen(UIType uIType)
        {
            UIPanelAttributeInfo info = m_AttributeInfos[uIType];
            List<BaseUIPanel> uIPanels= m_OpenedPanels[info.uILayer];
            foreach (BaseUIPanel item in uIPanels)
            {
                if(item.UIType==uIType)
                    return true;
            }
            return false;
        }

        public void OnUpdate()
        {
            foreach (var pair in m_OpenedPanels)
            {
                _Buffer.Clear();
                _Buffer.AddRange(pair.Value);
                foreach (var panel in _Buffer)
                {
                    panel.Update();
                }
            }
        }
        public void OnLateUpdate()
        {
        }

        public void OnDestroy()
        {
        }

        public BaseUIPanel GetOpenedPanel(UIType uIType)
        {
            UIPanelAttributeInfo attributeInfo = m_AttributeInfos[uIType];
            bool result = m_OpenedPanels.TryGetValue(attributeInfo.uILayer, out List<BaseUIPanel> panels);
            if(!result)
                return null;
            if(panels.Count<=0) 
                return null;
            return panels[0];
        }
    }
}
