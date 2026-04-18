namespace EC
{
    public interface IManager
    {
        void OnInit();
        void OnStart();
        void OnUpdate();
        void OnLateUpdate();
        void OnDestroy();
    }
}
