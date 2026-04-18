using UnityEngine;

namespace EC.UI
{
    public abstract class BaseUIPanel
    {
        public RectTransform ParentRectTrans => m_ParentRectTrans;
        public RectTransform RectTrans => m_RectTrans;

        protected RectTransform m_ParentRectTrans;
        protected RectTransform m_RectTrans;
        protected bool m_IsOpen;
        protected GameObject gameObject;

        public abstract UIType UIType { get; }
        public abstract string UIAssetPath { get; }

        public void Init(RectTransform parentRectTrans)
        {
            m_ParentRectTrans = parentRectTrans;
            m_RectTrans = null;
            m_IsOpen=false;
            gameObject = null;

            LoadAsset();

            OnInit();
        }

        protected virtual void OnInit()
        {

        }

        private void LoadAsset()
        {
            UnityEngine.Object asset = App.ResourceManager.LoadAsset(PathHelper.GetUIAssetPath(UIAssetPath));
            GameObject gameObject = UnityEngine.Object.Instantiate(asset as GameObject, m_ParentRectTrans, false);
            this.gameObject = gameObject;
            m_RectTrans = gameObject.GetComponent<RectTransform>();
        }

        public void Open(object param) 
        {
            m_IsOpen=true;
            OnOpen(param);
        }

        protected virtual void OnOpen(object param)
        {
            m_RectTrans.gameObject.SetActive(true);
        }

        public void Update()
        {
            OnUpdate();
        }

        protected virtual void OnUpdate()
        {
        }

        protected T GetUnityComponent<T>(string path) where T : UnityEngine.Component
        {
            Transform trans= m_RectTrans.Find(path);
            if(trans == null) return null;
            return trans.GetComponent<T>();
        }

        public void Close(object param)
        {
            m_IsOpen = false;
            OnClose();
        }

        public virtual bool RequestClose() => true;

        protected virtual void OnClose()
        {
            this.gameObject.SetActive(false);
        }
    }
}
