using UnityEngine;

namespace EC
{
    public static class GameDebug
    {
        public static void Assert(bool condition)
        {
            Debug.Assert(condition);
        }
    }
}
