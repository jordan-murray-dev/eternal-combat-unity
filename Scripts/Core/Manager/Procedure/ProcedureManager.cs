using EC.Launch;
using EC.Gameplay;

namespace EC
{
    public class ProcedureManager : IManager
    {
        private ProcedureFsm m_Fsm;

        public void OnInit()
        {
            ProcedureLaunch procedureLaunch = new ProcedureLaunch();
            ProcedureGameplay procedureGameplay = new ProcedureGameplay();

            m_Fsm = ProcedureFsm.Create(procedureLaunch, procedureGameplay);
        }

        public void OnStart()
        {
            m_Fsm.Start<ProcedureLaunch>();
        }

        public void OnUpdate()
        {
            m_Fsm.Update();
        }

        public void OnLateUpdate()
        {
        }

        public ProcedureFsmState GetCurrentProcedure() => m_Fsm.CurrentState;

        public void ChangeProcedure(System.Type type)
        {
            m_Fsm.ChangeState(type);
        }

        public void OnDestroy()
        {
            m_Fsm.Shutdown();
        }
    }
}
