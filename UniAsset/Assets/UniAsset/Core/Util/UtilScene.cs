using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

namespace UniAsset
{
    public class UtilScene : SafeSingleton<UtilScene>
    {
        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="loadSceneName"></param>
        /// <param name="loadSceneMode"></param>
        /// <param name="complete"></param>
        /// <param name="process"></param>
        /// <param name="error"></param>
        public void LoadScene (string loadSceneName , LoadSceneMode loadSceneMode = LoadSceneMode.Single , Action complete = null , Action<float> process = null , Action<string> error = null)
        {
            Scene targetScene = SceneManager.GetSceneByName (loadSceneName);
            if ( !targetScene.isLoaded )
            {
                ResMgr.Ins.StartCoroutine (LoadSceneAsync (loadSceneName , loadSceneMode , complete , process));
            }
            else
            {
                error?.Invoke ("场景已经在加载中");
            }
        }

        IEnumerator LoadSceneAsync (string loadSceneName , LoadSceneMode loadSceneMode , Action complete , Action<float> process)
        {
            AsyncOperation async = SceneManager.LoadSceneAsync (loadSceneName , loadSceneMode);
            do
            {
                process?.Invoke (async.progress);
                yield return new WaitForEndOfFrame ();
            }
            while ( !async.isDone );
            yield return async;
            complete?.Invoke ();
        }

        /// <summary>
        /// 卸载场景（异步）
        /// </summary>
        /// <param name="unloadSceneName"></param>
        /// <param name="complete"></param>
        public void UnloadScene (string unloadSceneName , Action complete = null)
        {
            Scene targetScene = SceneManager.GetSceneByName (unloadSceneName);
            if ( targetScene.isLoaded )
            {
                if ( targetScene == SceneManager.GetActiveScene () )
                {
                    Debug.LogWarning ($"{targetScene.name}处于激活状态,暂时无法卸载");
                }
                else
                {
                    Debug.LogWarning ($"场景{unloadSceneName}卸载");
                    SceneManager.UnloadSceneAsync (unloadSceneName);
                }
            }
            else
            {
                Debug.LogWarning ($"无需卸载场景{unloadSceneName}，这个场景已经不存在了");
            }
        }

        IEnumerator UnloadSceneYield (string unloadSceneName , Action complete)
        {
            yield return null;
            AsyncOperation asny = SceneManager.UnloadSceneAsync (unloadSceneName);
            yield return asny;
            complete?.Invoke ();
        }

        /// <summary>
        /// 控制场景隐藏显示
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="isShow"></param>
        public void SetSceneObjsShowOrHide (string sceneName , bool isShow)
        {
            Scene scene = SceneManager.GetSceneByName (sceneName);
            if ( scene != null )
            {
                foreach ( GameObject obj in scene.GetRootGameObjects () )
                {
                    obj.SetActive (isShow);
                }
            }
        }

        /// <summary>
        /// 将一个场景标识为激活
        /// </summary>
        /// <param name="sceneName"></param>
        public void SetActiveScene (string sceneName)
        {
            Scene targetScene = SceneManager.GetSceneByName (sceneName);
            if ( targetScene.IsValid () )
            {
                SceneManager.SetActiveScene (targetScene);
            }
            else
            {
                Debug.LogError ("激活失败，场景无效");
            }
        }

        /// <summary>
        /// 场景是否加载好了
        /// </summary>
        /// <param name="unloadSceneName"></param>
        /// <returns></returns>
        public bool SceneIsLoaded (string unloadSceneName)
        {
            Scene targetScene = SceneManager.GetSceneByName (unloadSceneName);
            return targetScene.isLoaded;
        }

        /// <summary>
        /// 通过名称获取场景
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Scene GetSceneByName (string name)
        {
            return SceneManager.GetSceneByName (name);
        }
    }
}