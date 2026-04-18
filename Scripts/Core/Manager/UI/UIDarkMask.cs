using UnityEngine;

namespace EC.UI
{
    public class UIDarkMask : MonoBehaviour
    {
        private RectTransform m_RectTrans;

        private void Awake()
        {
            m_RectTrans = GetComponent<RectTransform>();
        }

        public void Open()
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
        }

        public void SetParent(RectTransform parent)
        {
            m_RectTrans.SetParent(parent, false);
        }

        public void Close()
        {
            if (gameObject.activeSelf)
                gameObject.SetActive(false);
        }

        public void SetSiblingIndex(RectTransform parent, int index)
        {
            m_RectTrans.SetParent(parent, false);
            m_RectTrans.SetSiblingIndex(index);
        }

        public void SetAsFirstSibling(RectTransform parent)
        {
            m_RectTrans.SetParent(parent, false);
            m_RectTrans.SetAsFirstSibling();
        }

        public void SetAsLastSibling(RectTransform parent)
        {
            m_RectTrans.SetParent(parent, false);
            m_RectTrans.SetAsLastSibling();
        }
    }
}
