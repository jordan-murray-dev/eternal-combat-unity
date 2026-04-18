namespace EC
{
    public static class PathHelper
    {
        public static string GetUIAssetPath(string uIAssetPath)
        {
            return string.Format("Assets/Res/UI/{0}.prefab", uIAssetPath);
        }

        public static string GetConfigAssetPath(string configName)
        {
            return string.Format("Assets/Res/GameConfig/{0}.txt", configName);
        }
    }
}
