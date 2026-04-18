using System.Collections.Generic;

namespace EC
{
	public class App
	{
		#region Instance
		private static App m_Instance;
		public static App Instance => m_Instance;

		public static App CreateApp()
		{
			if (m_Instance != null)
				throw new GameException("App shouldn't exist, please check!");

            if(UnityEngine.Time.timeScale != 1f)
            {
                UnityEngine.Time.timeScale = 1f;
            }

#if !UNITY_EDITOR
            UnityEngine.Rendering.Universal.UniversalRenderPipeline.asset.renderScale = 0.45f; 
#endif

            m_Instance = new App();
			return m_Instance;
		}

		#endregion

		#region Log

		public static GameLog Log => App.Instance.m_Log;

        #endregion

        #region Managers

        public static ProcedureManager ProcedureManager=> App.Instance.m_ProcedureManager;
        public static ResourceManager ResourceManager=> App.Instance.m_ResourceManager;
        public static SceneManager SceneManager => App.Instance.m_SceneManager;
        public static UIManager UIManager=> App.Instance.m_UIManager;
        public static MonoManager MonoManager=> App.Instance.m_MonoManager;
        public static MessagingSystemManager MessagingSystemManager => App.Instance.m_MessagingSystemManager;
        public static PlayerManager PlayerManager => App.Instance.m_PlayerManager;

        #endregion

        private GameLog m_Log;

        private ProcedureManager m_ProcedureManager;
        private ResourceManager m_ResourceManager;
		private SceneManager m_SceneManager;
        private UIManager m_UIManager;
        private MonoManager m_MonoManager;
        private MessagingSystemManager m_MessagingSystemManager;
        private PlayerManager m_PlayerManager;

        private IManager[] m_Managers;

		public IManager[] Managers=> m_Managers;

		public App()
        {
            
        }

		public void OnAwake()
		{
			InitLog();
			RegisterManagers();
			ManagersOnInit();
        }

		private void InitLog()
		{
            m_Log=new GameLog();
            m_Log.Init(GameLogLevel.Debug);
        }

		private void RegisterManagers()
		{
            //Custom register managers
            List<IManager> catchManagers = new List<IManager>();

            m_ResourceManager = new ResourceManager();
            m_UIManager = new UIManager();
            m_ProcedureManager = new ProcedureManager();
            m_SceneManager = new SceneManager();
            m_MonoManager=new MonoManager();
            m_MessagingSystemManager = new MessagingSystemManager();
            m_PlayerManager = new PlayerManager();

            catchManagers.Add(m_MonoManager);
            catchManagers.Add(m_ResourceManager);
            catchManagers.Add(m_UIManager);
            catchManagers.Add(m_ProcedureManager);
            catchManagers.Add(m_SceneManager);
            catchManagers.Add(m_MessagingSystemManager);
            catchManagers.Add(m_PlayerManager);

            m_Managers = catchManagers.ToArray();
        }

		private void ManagersOnInit()
		{
			foreach (IManager manager in m_Managers)
			{
                manager.OnInit();
            }
		}

		public void OnStart()
		{
			ManagersOnStart();
        }

        private void ManagersOnStart()
        {
            foreach (IManager manager in m_Managers)
            {
                manager.OnStart();
            }
        }

        public void OnUpdate()
		{
			ManagersOnUpdate();
        }

        private void ManagersOnUpdate()
        {
            foreach (IManager manager in m_Managers)
            {
                manager.OnUpdate();
            }
        }

        public void OnLateUpdate()
		{
            ManagersOnLateUpdate();
        }

        private void ManagersOnLateUpdate()
        {
            foreach (IManager manager in m_Managers)
            {
                manager.OnLateUpdate();
            }
        }

        public void OnDestroy()
		{
            ManagersOnDestroy();
        }

        private void ManagersOnDestroy()
        {
            foreach (IManager manager in m_Managers)
            {
                manager.OnDestroy();
            }
        }
    } 
}
