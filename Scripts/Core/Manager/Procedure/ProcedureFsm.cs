using System.Collections.Generic;
using System;

namespace EC
{
    public class ProcedureFsm
    {
        private readonly Dictionary<Type, ProcedureFsmState> m_States;
        private Dictionary<string, Object> m_Datas;
        private ProcedureFsmState m_CurrentState;
        private bool m_IsDestroyed;

        /// <summary>
        /// 初始化有限状态机的新实例。
        /// </summary>
        public ProcedureFsm()
        {
            m_States = new Dictionary<Type, ProcedureFsmState>();
            m_Datas = null;
            m_CurrentState = null;
            m_IsDestroyed = true;
        }

        /// <summary>
        /// 获取有限状态机中状态的数量。
        /// </summary>
        public int FsmStateCount
        {
            get
            {
                return m_States.Count;
            }
        }

        /// <summary>
        /// 获取有限状态机是否正在运行。
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return m_CurrentState != null;
            }
        }

        /// <summary>
        /// 获取有限状态机是否被销毁。
        /// </summary>
        public bool IsDestroyed
        {
            get
            {
                return m_IsDestroyed;
            }
        }

        /// <summary>
        /// 获取当前有限状态机状态。
        /// </summary>
        public ProcedureFsmState CurrentState
        {
            get
            {
                return m_CurrentState;
            }
        }

        /// <summary>
        /// 获取当前有限状态机状态名称。
        /// </summary>
        public string CurrentStateName
        {
            get
            {
                return m_CurrentState != null ? m_CurrentState.GetType().FullName : null;
            }
        }

        /// <summary>
        /// 创建有限状态机。
        /// </summary>
        /// <param name="name">有限状态机名称。</param>
        /// <param name="owner">有限状态机持有者。</param>
        /// <param name="states">有限状态机状态集合。</param>
        /// <returns>创建的有限状态机。</returns>
        public static ProcedureFsm Create( params ProcedureFsmState[] states)
        {
            if (states == null || states.Length < 1)
            {
                throw new GameException("ProcedureFsmState states is invalid.");
            }

            ProcedureFsm fsm = new ProcedureFsm();
            fsm.m_IsDestroyed = false;
            foreach (ProcedureFsmState state in states)
            {
                if (state == null)
                {
                    throw new GameException("FSM states is invalid.");
                }

                Type stateType = state.GetType();
                if (fsm.m_States.ContainsKey(stateType))
                {
                    throw new GameException(string.Format("FSM '{0}' state '{1}' is already exist.", fsm.GetType().FullName, state.GetType().FullName));
                }

                fsm.m_States.Add(stateType, state);
                state.OnInit(fsm);
            }

            return fsm;
        }

        /// <summary>
        /// 清理有限状态机。
        /// </summary>
        public void Clear()
        {
            if (m_CurrentState != null)
            {
                m_CurrentState.OnLeave(true);
            }

            foreach (KeyValuePair<Type, ProcedureFsmState> state in m_States)
            {
                state.Value.OnDestroy();
            }

            m_States.Clear();

            if (m_Datas != null)
            {
                m_Datas.Clear();
            }

            m_CurrentState = null;
            m_IsDestroyed = true;
        }

        /// <summary>
        /// 开始有限状态机。
        /// </summary>
        /// <typeparam name="TState">要开始的有限状态机状态类型。</typeparam>
        public void Start<TState>() where TState : ProcedureFsmState
        {
            if (IsRunning)
            {
                throw new GameException("FSM is running, can not start again.");
            }

            ProcedureFsmState state = GetState<TState>();
            if (state == null)
            {
                throw new GameException(string.Format("FSM '{0}' can not start state '{1}' which is not exist.", this.GetType().FullName, typeof(TState).FullName));
            }

            m_CurrentState = state;
            m_CurrentState.OnEnter();
        }

        /// <summary>
        /// 是否存在有限状态机状态。
        /// </summary>
        /// <typeparam name="TState">要检查的有限状态机状态类型。</typeparam>
        /// <returns>是否存在有限状态机状态。</returns>
        public bool HasState<TState>() where TState : ProcedureFsmState
        {
            return m_States.ContainsKey(typeof(TState));
        }

        /// <summary>
        /// 获取有限状态机状态。
        /// </summary>
        /// <typeparam name="TState">要获取的有限状态机状态类型。</typeparam>
        /// <returns>要获取的有限状态机状态。</returns>
        public TState GetState<TState>() where TState : ProcedureFsmState
        {
            if (m_States.TryGetValue(typeof(TState), out ProcedureFsmState state))
            {
                return (TState)state;
            }

            return null;
        }

        /// <summary>
        /// 获取有限状态机状态。
        /// </summary>
        /// <param name="stateType">要获取的有限状态机状态类型。</param>
        /// <returns>要获取的有限状态机状态。</returns>
        public ProcedureFsmState GetState(Type stateType)
        {
            if (stateType == null)
            {
                throw new GameException("State type is invalid.");
            }

            if (!typeof(ProcedureFsmState).IsAssignableFrom(stateType))
            {
                throw new GameException(string.Format("State type '{0}' is invalid.", stateType.FullName));
            }

            ProcedureFsmState state;
            if (m_States.TryGetValue(stateType, out state))
            {
                return state;
            }

            return null;
        }


        /// <summary>
        /// 获取有限状态机的所有状态。
        /// </summary>
        /// <returns>有限状态机的所有状态。</returns>
        public ProcedureFsmState[] GetAllStates()
        {
            int index = 0;
            ProcedureFsmState[] results = new ProcedureFsmState[m_States.Count];
            foreach (KeyValuePair<Type, ProcedureFsmState> state in m_States)
            {
                results[index++] = state.Value;
            }

            return results;
        }

        /// <summary>
        /// 是否存在有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>有限状态机数据是否存在。</returns>
        public bool HasData(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new GameException("Data name is invalid.");
            }

            if (m_Datas == null)
            {
                return false;
            }

            return m_Datas.ContainsKey(name);
        }

        /// <summary>
        /// 获取有限状态机数据。
        /// </summary>
        /// <typeparam name="TData">要获取的有限状态机数据的类型。</typeparam>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>要获取的有限状态机数据。</returns>
        public TData GetData<TData>(string name)
        {
            return (TData)GetData(name);
        }

        /// <summary>
        /// 获取有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>要获取的有限状态机数据。</returns>
        public Object GetData(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new GameException("Data name is invalid.");
            }

            if (m_Datas == null)
            {
                return null;
            }

            if (m_Datas.TryGetValue(name, out Object data))
            {
                return data;
            }

            return null;
        }

        /// <summary>
        /// 设置有限状态机数据。
        /// </summary>
        /// <typeparam name="TData">要设置的有限状态机数据的类型。</typeparam>
        /// <param name="name">有限状态机数据名称。</param>
        /// <param name="data">要设置的有限状态机数据。</param>
        public void SetData<TData>(string name, TData data)
        {
            SetData(name, (Object)data);
        }

        /// <summary>
        /// 设置有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <param name="data">要设置的有限状态机数据。</param>
        public void SetData(string name, Object data)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new GameException("Data name is invalid.");
            }

            if (m_Datas == null)
            {
                m_Datas = new Dictionary<string, Object>(StringComparer.Ordinal);
            }

            m_Datas[name] = data;
        }

        /// <summary>
        /// 移除有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>是否移除有限状态机数据成功。</returns>
        public bool RemoveData(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new GameException("Data name is invalid.");
            }

            if (m_Datas == null)
            {
                return false;
            }

            return m_Datas.Remove(name);
        }

        /// <summary>
        /// 有限状态机轮询。
        /// </summary>
        public void Update()
        {
            if (m_CurrentState == null)
            {
                return;
            }

            m_CurrentState.OnUpdate();
        }

        /// <summary>
        /// 关闭并清理有限状态机。
        /// </summary>
        public void Shutdown()
        {
            Clear();
        }

        /// <summary>
        /// 切换当前有限状态机状态。
        /// </summary>
        /// <typeparam name="TState">要切换到的有限状态机状态类型。</typeparam>
        public void ChangeState<TState>() where TState : ProcedureFsmState
        {
            ChangeState(typeof(TState));
        }

        /// <summary>
        /// 切换当前有限状态机状态。
        /// </summary>
        /// <param name="stateType">要切换到的有限状态机状态类型。</param>
        internal void ChangeState(Type stateType)
        {
            if (m_CurrentState == null)
            {
                throw new GameException("Current state is invalid.");
            }

            ProcedureFsmState state = GetState(stateType);
            if (state == null)
            {
                throw new GameException(string.Format("FSM '{0}' can not change state to '{1}' which is not exist.", this.GetType().FullName, stateType.FullName));
            }

            m_CurrentState.OnLeave(false);
            m_CurrentState = state;
            m_CurrentState.OnEnter();
        }
    }
}
