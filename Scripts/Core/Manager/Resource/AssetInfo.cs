using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace EC
{
    public class AssetInfo
    {
        public enum AssetLoadState
        {
            Unload,
            Loading,
            AbortLoading,
            Loaded,
        }

        public struct CallbackInfo
        {
            public readonly ulong callbackId;
            public readonly Action<ulong,UnityEngine.Object> callback;
            public CallbackInfo(ulong callbackId, Action<ulong,UnityEngine.Object> callback)
            {
                this.callbackId = callbackId;
                this.callback = callback;
            }
        }

        public string assetPath { get; private set; }
        public AssetLoadState assetLoadState { get; private set; }
        public uint assetRefCount { get; private set; }
        public List<CallbackInfo> callbackInfos { get; private set; }
        public AsyncOperationHandle<UnityEngine.Object> handle { get; private set; }
        public UnityEngine.Object asset { get; private set; }

        public AssetInfo(string assetPath)
        {
            this.assetPath = assetPath;
            assetLoadState = AssetLoadState.Unload;
            assetRefCount = 0;
            callbackInfos = new List<CallbackInfo>();
            handle = default;
            asset = null;
        }

        public void AddAssetRefCount() => assetRefCount++;
        public void SubtractAssetRefCount() => assetRefCount--;

        public void AddCallback(ulong callbackId, Action<ulong,UnityEngine.Object> callback)
        {
            callbackInfos.Add(new CallbackInfo(callbackId, callback));
        }

        public void RemoveCallback(ulong callbackId)
        {
            for (int i = callbackInfos.Count - 1; i >= 0; i--)
            {
                CallbackInfo callbackInfo = callbackInfos[i];
                if (callbackInfo.callbackId == callbackId)
                {
                    callbackInfos.RemoveAt(i);
                    return;
                }
            }
        }

        public void LoadAssetAsync()
        {
            switch (assetLoadState)
            {
                case AssetLoadState.Unload:
                    {
                        assetLoadState = AssetLoadState.Loading;
                        break;
                    }
                case AssetLoadState.Loading:
                    throw new GameException("Invalid load state");
                case AssetLoadState.AbortLoading:
                    throw new GameException("Invalid load state");
                case AssetLoadState.Loaded:
                    throw new GameException("Invalid load state");
                default:
                    throw new GameException("Invalid load state");
            }

            handle = Addressables.LoadAssetAsync<UnityEngine.Object>(assetPath);
            handle.Completed -= OnLoadAssetCompleted;
            handle.Completed += OnLoadAssetCompleted;
        }

        public void LoadAssetSync()
        {
            switch (assetLoadState)
            {
                case AssetLoadState.Unload:
                    {
                        assetLoadState = AssetLoadState.Loading;
                        break;
                    }
                case AssetLoadState.Loading:
                    throw new GameException("Invalid load state");
                case AssetLoadState.AbortLoading:
                    throw new GameException("Invalid load state");
                case AssetLoadState.Loaded:
                    throw new GameException("Invalid load state");
                default:
                    throw new GameException("Invalid load state");
            }

            handle = Addressables.LoadAssetAsync<UnityEngine.Object>(assetPath);
            handle.Completed -= OnLoadAssetCompleted;
            handle.Completed += OnLoadAssetCompleted;
            handle.WaitForCompletion();
        }

        public void WaitForCompletion()
        {
            handle.WaitForCompletion();
        }

        public void AbortLoading()
        {
            if (assetRefCount > 0)
                return;
            switch (assetLoadState)
            {
                case AssetLoadState.Unload:
                    throw new GameException("Invalid load state");
                case AssetLoadState.Loading:
                    break;
                case AssetLoadState.AbortLoading:
                    throw new GameException("Invalid load state");
                case AssetLoadState.Loaded:
                    throw new GameException("Invalid load state");
                default:
                    throw new GameException("Invalid load state");
            }
            assetLoadState = AssetLoadState.AbortLoading;
        }

        public void CancelAbortLoading()
        {
            if (assetRefCount <= 0)
                return;
            switch (assetLoadState)
            {
                case AssetLoadState.Unload:
                    throw new GameException("Invalid load state");
                case AssetLoadState.Loading:
                    throw new GameException("Invalid load state");
                case AssetLoadState.AbortLoading:
                    break;
                case AssetLoadState.Loaded:
                    throw new GameException("Invalid load state");
                default:
                    throw new GameException("Invalid load state");
            }
            assetLoadState = AssetLoadState.Loading;
        }

        private void OnLoadAssetCompleted(AsyncOperationHandle<UnityEngine.Object> paramHandler)
        {
            switch (paramHandler.Status)
            {
                case AsyncOperationStatus.None:
                    throw new GameException("Invalid AsyncOperationStatus");
                case AsyncOperationStatus.Succeeded:
                    break;
                case AsyncOperationStatus.Failed:
                    throw new GameException("Load Asset Failed");
                default:
                    throw new GameException("Invalid AsyncOperationStatus");
            }

            switch (assetLoadState)
            {
                case AssetLoadState.Unload:
                    {
                        throw new GameException("Invalid load state");
                    }
                case AssetLoadState.Loading:
                    {
                        asset = paramHandler.Result;
                        assetLoadState = AssetLoadState.Loaded;

                        for (int i = 0; i < callbackInfos.Count; i++)
                        {
                            CallbackInfo callbackInfo = callbackInfos[i];
                            callbackInfo.callback.Invoke(callbackInfo.callbackId, asset);
                        }
                        callbackInfos.Clear();

                        break;
                    }
                case AssetLoadState.AbortLoading:
                    {
                        throw new GameException("Invalid load state");
                    }
                case AssetLoadState.Loaded:
                    {
                        throw new GameException("Invalid load state");
                    }
                default:
                    throw new GameException("Invalid load state");
            }
        }

        public void InvokeCallback(ulong callbackId,in Action<ulong,UnityEngine.Object> callback)
        {
            if (assetLoadState != AssetLoadState.Loaded)
                throw new GameException("Invalid load state");
            callback.Invoke(callbackId,asset);
        }

        public bool UnloadAssetCondition()
        {
            bool condition1 = assetLoadState == AssetLoadState.Loaded;
            bool condition2 = assetRefCount == 0;
            return condition1 && condition2;
        }

        public void UnloadAsset()
        {
            bool conditionMet = UnloadAssetCondition();
            if (!conditionMet)
                return;
            Addressables.Release(handle);
            Reset();
        }

        private void Reset()
        {
            assetLoadState = AssetLoadState.Unload;
            assetRefCount = 0;
            callbackInfos.Clear();
            handle = default;
            asset = null;

        }
    }
}
