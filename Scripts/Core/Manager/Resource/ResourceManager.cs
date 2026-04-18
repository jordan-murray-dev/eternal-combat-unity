using System;
using System.Collections.Generic;

namespace EC
{
    public class ResourceManager : IManager
    {
        private ulong m_AssetCallbackId;
        private Dictionary<string, AssetInfo> m_AssetDictionary;

        public void OnInit()
        {
            m_AssetCallbackId = 1;
            m_AssetDictionary =new Dictionary<string, AssetInfo>();
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

        #region Function

        private ulong GetAssetCallbackId() => m_AssetCallbackId++;

        public ulong LoadAssetCallback(string path,Action<ulong,UnityEngine.Object> loadComplete)
        {
            ulong assetCallbackId = GetAssetCallbackId();
            if (!m_AssetDictionary.ContainsKey(path))
                m_AssetDictionary.Add(path, new AssetInfo(path));
            AssetInfo assetInfo = m_AssetDictionary[path];
            switch (assetInfo.assetLoadState)
            {
                case AssetInfo.AssetLoadState.Unload:
                    {
                        assetInfo.AddAssetRefCount();
                        assetInfo.AddCallback(assetCallbackId, loadComplete);
                        assetInfo.LoadAssetAsync();
                        break;
                    }
                case AssetInfo.AssetLoadState.Loading:
                    {
                        assetInfo.AddAssetRefCount();
                        assetInfo.AddCallback(assetCallbackId, loadComplete);
                        break;
                    }
                case AssetInfo.AssetLoadState.AbortLoading:
                    {
                        assetInfo.AddAssetRefCount();
                        assetInfo.AddCallback(assetCallbackId, loadComplete);
                        assetInfo.CancelAbortLoading();
                        break;
                    }
                case AssetInfo.AssetLoadState.Loaded:
                    {
                        assetInfo.AddAssetRefCount();
                        assetInfo.InvokeCallback(assetCallbackId,loadComplete);
                        break;
                    }
                default:
                    throw new GameException("Invalid load state");
            }
            return assetCallbackId;
        }

        public UnityEngine.Object LoadAsset(string path)
        {
            if (!m_AssetDictionary.ContainsKey(path))
                m_AssetDictionary.Add(path, new AssetInfo(path));
            AssetInfo assetInfo = m_AssetDictionary[path];
            switch (assetInfo.assetLoadState)
            {
                case AssetInfo.AssetLoadState.Unload:
                    {
                        assetInfo.AddAssetRefCount();
                        assetInfo.LoadAssetSync();
                        return assetInfo.asset;
                    }
                case AssetInfo.AssetLoadState.Loading:
                    {
                        assetInfo.AddAssetRefCount();
                        assetInfo.WaitForCompletion();
                        return assetInfo.asset;
                    }
                case AssetInfo.AssetLoadState.AbortLoading:
                    {
                        assetInfo.AddAssetRefCount();
                        assetInfo.CancelAbortLoading();
                        assetInfo.WaitForCompletion();
                        return assetInfo.asset;
                    }
                case AssetInfo.AssetLoadState.Loaded:
                    {
                        assetInfo.AddAssetRefCount();
                        return assetInfo.asset;
                    }
                default:
                    throw new GameException("Invalid load state");
            }
        }

        public bool HasAssetInfo(string path)
        {
            return m_AssetDictionary.ContainsKey(path);
        }

        public AssetInfo GetAssetInfo(string path)
        {
            if(!HasAssetInfo(path))
                return null;
            return m_AssetDictionary[path];
        }

        public void UnloadAsset(string path,ulong assetCallbackId=0)
        {
            if (!m_AssetDictionary.ContainsKey(path))
                throw new GameException("No asset for this path");
            AssetInfo assetInfo = m_AssetDictionary[path];
            switch (assetInfo.assetLoadState)
            {
                case AssetInfo.AssetLoadState.Unload:
                    {
                        GameLog.Warning("Asset has already been unload, please do not repeatedly unload it.");
                        break;
                    }
                case AssetInfo.AssetLoadState.Loading:
                    {
                        assetInfo.SubtractAssetRefCount();
                        assetInfo.RemoveCallback(assetCallbackId);
                        assetInfo.AbortLoading();
                        break;
                    }
                case AssetInfo.AssetLoadState.AbortLoading:
                    {
                        GameLog.Warning("Asset load state is AbortLoading, please do not repeatedly unload it.");
                        break;
                    }
                case AssetInfo.AssetLoadState.Loaded:
                    {
                        assetInfo.SubtractAssetRefCount();
                        assetInfo.UnloadAsset();
                        break;
                    }
                default:
                    throw new GameException("Invalid load state");
            }
        }

        #endregion

        public void OnDestroy()
        {
        }
    }
}
