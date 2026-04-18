using System;
using UnityEngine;

namespace EC
{
    public class MonoApp:MonoBehaviour
    {
        private void Awake()
        {
            if(App.Instance != null )
            {
                this.gameObject.SetActive(false);
                return;
            }

#if UNITY_IOS
            // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Application-targetFrameRate.html
            Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
#endif

            DontDestroyOnLoad(this.gameObject);

            App app= App.CreateApp();
            app.OnAwake();
        }

        private void Start()
        {
            App.Instance.OnStart();
        }

        private void Update()
        {
            App.Instance.OnUpdate();
        }

        private void LateUpdate()
        {
            App.Instance.OnLateUpdate();
        }

#if UNITY_EDITOR
        private Action m_DrawGizmosAction;
        private void OnDrawGizmos()
        {
            m_DrawGizmosAction?.Invoke();
        } 
        public void AddDrawGizmos(Action action)
        {
            m_DrawGizmosAction += action;
        }
#endif

        private void OnDestroy()
        {
        }
    }
}
