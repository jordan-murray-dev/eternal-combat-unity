namespace EC
{
    public abstract class ProcedureFsmState
    {
        private ProcedureFsm m_Fsm;

        /// <summary>
        /// 初始化有限状态机状态基类的新实例。
        /// </summary>
        public ProcedureFsmState()
        {
        }

        /// <summary>
        /// 有限状态机状态初始化时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        public virtual void OnInit(ProcedureFsm fsm)
        {
            m_Fsm = fsm;
        }

        /// <summary>
        /// 有限状态机状态进入时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        public virtual void OnEnter()
        {
        }

        /// <summary>
        /// 有限状态机状态轮询时调用。
        /// </summary>
        public virtual void OnUpdate()
        {
        }

        /// <summary>
        /// 有限状态机状态离开时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        /// <param name="isShutdown">是否是关闭有限状态机时触发。</param>
        public virtual void OnLeave(bool isShutdown)
        {
        }

        /// <summary>
        /// 有限状态机状态销毁时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        public virtual void OnDestroy()
        {
        }

        /// <summary>
        /// 切换当前有限状态机状态。
        /// </summary>
        /// <typeparam name="TState">要切换到的有限状态机状态类型。</typeparam>
        /// <param name="fsm">有限状态机引用。</param>
        protected void ChangeState<TState>() where TState : ProcedureFsmState
        {
            m_Fsm.ChangeState<TState>();
        }
    }
}
