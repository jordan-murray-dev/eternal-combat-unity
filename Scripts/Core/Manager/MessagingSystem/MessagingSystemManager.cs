using System;
using System.Collections.Generic;

namespace EC
{
    public class MessagingSystemManager : IManager
    {
        private Dictionary<uint, Action<object>> m_Messages; 

        public void OnDestroy()
        {
        }

        public void OnInit()
        {
            m_Messages=new Dictionary<uint, Action<object>>();
        }

        public void OnLateUpdate()
        {
        }

        public void OnStart()
        {
        }

        public void OnUpdate()
        {
        }

        public void Subscribe(uint messageId, Action<object> action)
        {
            if (!m_Messages.ContainsKey(messageId))
            {
                m_Messages.Add(messageId, null);
            }
            m_Messages[messageId] += action;
        }

        public void Unsubscribe(uint messageId, Action<object> action)
        {
            if (!m_Messages.ContainsKey(messageId))
            {
                GameLog.Warning($"You are trying to unsubscribe an event which does not been subscribed.");
                return;
            }
            m_Messages[messageId] -= action;
        }

        public void Dispatch(uint messageId,object param)
        {
            if (!m_Messages.ContainsKey(messageId))
            {
                return;
            }
            m_Messages[messageId]?.Invoke(param);
        }
    }
}
