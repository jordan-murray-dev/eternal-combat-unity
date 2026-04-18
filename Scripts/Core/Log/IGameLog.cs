namespace EC
{
    public interface IGameLog
    {
        GameLogLevel LogLevel { get; }

        void Init(GameLogLevel logLevel);
     
        void Debug_Internal(string format);
        void Info_Internal(string format);
        void Warning_Internal(string format);
        void Error_Internal(string format);

        //------------

        void DebugFormat_Internal(string format, params object[] args);
        void InfoFormat_Internal(string format, params object[] args);
        void WarningFormat_Internal(string format, params object[] args);
        void ErrorFormat_Internal(string format, params object[] args);
    }
}
