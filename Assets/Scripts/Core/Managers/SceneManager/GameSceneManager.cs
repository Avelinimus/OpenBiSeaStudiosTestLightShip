using Cysharp.Threading.Tasks;

using Game.Utils;

using System;

using UnityEngine.SceneManagement;

namespace Game.Core.Managers
{
    public enum SceneType
    {
        Game,
        Space
    }

    public sealed class GameSceneManager : Manager
    {
        public Action<SceneType, Scene> SCENE_LOADED;
        public Action<SceneType, Scene> SCENE_UNLOADED;

        public override void Dispose()
        {
        }

        public async UniTask LoadSceneAsync(SceneType sceneType, LoadSceneMode loadSceneMode)
        {
            var asyncLoad = SceneManager.LoadSceneAsync(sceneType.ToString(), loadSceneMode);
            await asyncLoad.ToUniTask();
            SCENE_LOADED.SafeInvoke(sceneType, SceneManager.GetSceneByName(sceneType.ToString()));
        }

        public async UniTask UnloadSceneAsync(SceneType sceneType)
        {
            try
            {
                var asyncLoad = SceneManager.UnloadSceneAsync(sceneType.ToString());
                await asyncLoad.ToUniTask();
                SCENE_UNLOADED.SafeInvoke(sceneType, SceneManager.GetSceneByName(sceneType.ToString()));
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}