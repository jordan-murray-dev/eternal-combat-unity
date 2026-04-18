namespace EC
{
    public class GameLog : IGameLog
    {
        public GameLogLevel LogLevel => m_LogLevel;
        private GameLogLevel m_LogLevel;

        public void Init(GameLogLevel logLevel)
        {
            m_LogLevel = logLevel;
        }

        #region Internal

        public void Debug_Internal(string format)
        {
            if (m_LogLevel > GameLogLevel.Debug)
                return;
            UnityEngine.Debug.Log(format);
        }

        public void Info_Internal(string format)
        {
            if (m_LogLevel > GameLogLevel.Info)
                return;
            UnityEngine.Debug.Log(format);
        }

        public void Warning_Internal(string format)
        {
            if (m_LogLevel > GameLogLevel.Warning)
                return;
            UnityEngine.Debug.LogWarning(format);
        }

        public void Error_Internal(string format)
        {
            if (m_LogLevel > GameLogLevel.Error)
                return;
            UnityEngine.Debug.LogError(format);
        }

        public void DebugFormat_Internal(string format, params object[] args)
        {
            if (m_LogLevel > GameLogLevel.Debug)
                return;
            UnityEngine.Debug.LogFormat(format, args);
        }

        public void InfoFormat_Internal(string format, params object[] args)
        {
            if (m_LogLevel > GameLogLevel.Info)
                return;
            UnityEngine.Debug.LogFormat(format, args);
        }

        public void WarningFormat_Internal(string format, params object[] args)
        {
            if (m_LogLevel > GameLogLevel.Warning)
                return;
            UnityEngine.Debug.LogWarningFormat(format, args);
        }

        public void ErrorFormat_Internal(string format, params object[] args)
        {
            if (m_LogLevel > GameLogLevel.Error)
                return;
            UnityEngine.Debug.LogErrorFormat(format, args);
        }

        #endregion

        #region Static

        public static void Debug(string format)
        {
            App.Log.Debug_Internal(format);
        }

        public static void Info(string format)
        {
            App.Log.Info_Internal(format);
        }

        public static void Warning(string format)
        {
            App.Log.Warning_Internal(format);
        }

        public static void Error(string format)
        {
            App.Log.Error_Internal(format);
        }

        //------------


        public static void DebugFormat(string format, params object[] args)
        {
            App.Log.DebugFormat_Internal(format, args);
        }

        public static void InfoFormat(string format, params object[] args)
        {
            App.Log.InfoFormat_Internal(format, args);
        }

        public static void WarningFormat(string format, params object[] args)
        {
            App.Log.WarningFormat_Internal(format, args);
        }

        public static void ErrorFormat(string format, params object[] args)
        {
            App.Log.ErrorFormat_Internal(format, args);
        }

        #endregion
    }
}
