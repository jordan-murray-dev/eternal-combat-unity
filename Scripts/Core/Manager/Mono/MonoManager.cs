using UnityEngine;
using System.Collections;
using System;

namespace EC
{
    public class MonoManager : IManager
    {
        private MonoApp m_MonoApp;
        
        public Action UpdateAction;

        public void OnDestroy()
        {
        }

        public void OnInit()
        {
            m_MonoApp=GameObject.FindFirstObjectByType<MonoApp>();
        }

        public void OnLateUpdate()
        {
        }

        public void OnStart()
        {
        }

        public void OnUpdate()
        {
            UpdateAction?.Invoke();
        }

        public Coroutine StartCoroutine(IEnumerator coroutine)
        {
            return m_MonoApp.StartCoroutine(coroutine);
        }

        public void StopCoroutine(IEnumerator coroutine)
        {
            m_MonoApp.StopCoroutine(coroutine);
        }

        public T AddComponent<T>() where T : UnityEngine.Component
        {
            return m_MonoApp.gameObject.AddComponent<T>();
        }

        #region UNITY_EDITOR
#if UNITY_EDITOR
        public void OnDrawGizmos(Action action)
        {
            m_MonoApp.AddDrawGizmos(action);
        }  
#endif
        #endregion
    }
}
