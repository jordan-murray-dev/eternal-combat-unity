namespace EC
{
    public class PlayerManager : IManager
    {
        private Player _Player;
        public Player Player
        {
            get { return _Player; }
            set { _Player = value; }
        }

        public void OnInit()
        {
        }

        public void OnStart()
        {
        }

        public void OnUpdate()
        {
        }

        public void OnLateUpdate()
        {
        }

        public void OnDestroy()
        {
        }
    }
}