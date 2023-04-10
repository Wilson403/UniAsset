using System;
using Primise4CSharp;
using UniAsset;
using UnityEngine;

public class UniAssetRuntime : ASingletonMonoBehaviour<UniAssetRuntime>
{
    public event Action onUpdate;
    public SettingVo Setting { get; private set; }
    public LocalDataModel LocalData { get; private set; }
    public LocalResVerModel LocalResVer { get; private set; }
    public ResInitializeParameters ResInitializeParameters { get; private set; }
    public ResLoadMode ResLoadMode { get; private set; }

    //首个资源包的文件版本号数据模型
    public ResVerModel firstNetResVer;

    //最新资源包的文件版本号数据模型
    public ResVerModel currentNetResVer;

    /// <summary>
    /// 获取资源版本信息
    /// </summary>
    /// <param name="isFirstVer"></param>
    /// <returns></returns>
    public ResVerModel GetResVerModel (bool isFirstVer)
    {
        return isFirstVer ? firstNetResVer : currentNetResVer;
    }

    private void Update ()
    {
        onUpdate?.Invoke ();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="initializeParameters"></param>
    /// <exception cref="System.Exception"></exception>
    public void Init (ResInitializeParameters initializeParameters)
    {
        if ( initializeParameters == null )
        {
            throw new System.Exception ("加载参数异常");
        }
        ResInitializeParameters = initializeParameters;

        if ( initializeParameters is EditorInitializeParameters )
        {
            ResLoadMode = ResLoadMode.ASSET_DATA_BASE;
        }
        else if ( initializeParameters is OnlineInitializeParameters )
        {
            ResLoadMode = ResLoadMode.REMOTE_ASSET_BUNDLE;
        }
        else if ( initializeParameters is OfflineInitializeParameters )
        {
            ResLoadMode = ResLoadMode.LOCAL_ASSET_BUNDLE;
        }

        LocalData = new LocalDataModel ();
        LocalResVer = new LocalResVerModel ();
    }

    readonly Promise _preloadPromise = new Promise ();
    /// <summary>
    /// 开启预载
    /// </summary>
    public Promise StartPreload ()
    {
        if ( ResInitializeParameters == null )
        {
            Debug.LogWarning ($"ResInitializeParameters数据为空，请先确保Init函数被执行");
            return _preloadPromise;
        }

        //先检查是否存在内嵌资源包(ZIP)，如果存在的话会等解压后再执行，不存在的话就直接执行
        new PackageUpdate ().Start (default).Then (() =>
        {
            //资源部署在远程服务器上，先走资源更新流程，把资源下载到本地目录
            if ( ResLoadMode == ResLoadMode.REMOTE_ASSET_BUNDLE )
            {
                UpdateSettingFile ();
                return;
            }

            PreloadDone ();
        });

        return _preloadPromise;
    }

    /// <summary>
    /// 获取根URL
    /// </summary>
    /// <param name="isFirstVer"></param>
    /// <returns></returns>
    public string GetRootUrl (bool isFirstVer)
    {
        if ( ResLoadMode == ResLoadMode.REMOTE_ASSET_BUNDLE )
        {
            string netPath = ( ResInitializeParameters as OnlineInitializeParameters ).NetResDir;
            string ver = isFirstVer ? Setting.GetFirstNetResPackageVer () : Setting.GetCurrentNetResPackageVer ();
            return FileSystem.CombinePaths (netPath , UtilResVersionCompare.Ins.GetAppMainVersion () , ver);
        }
        return "null";
    }

    /// <summary>
    /// 获取根URL
    /// </summary>
    /// <param name="isFirstVer"></param>
    /// <returns></returns>
    public string GetAssetBundleUrl (bool isFirstVer)
    {
        if ( ResLoadMode == ResLoadMode.REMOTE_ASSET_BUNDLE )
        {
            string netPath = ( ResInitializeParameters as OnlineInitializeParameters ).NetResDir;
            string ver = isFirstVer ? Setting.GetFirstNetResPackageVer () : Setting.GetCurrentNetResPackageVer ();
            return FileSystem.CombinePaths (netPath , UtilResVersionCompare.Ins.GetAppMainVersion () , ver , UniAssetConst.AB_DIR_NAME);
        }
        return "null";
    }

    #region 资源更新流程

    #region 第一步，更新Setting文件

    /// <summary>
    /// 更新Setting配置文件
    /// </summary>
    private void UpdateSettingFile ()
    {
        var promise = new SettingUpdate ().Start ();
        promise.Then (SettingUpdateDone);
        promise.Catch (OnUpdateError);
    }

    /// <summary>
    /// Setting配置文件更新完成
    /// </summary>
    /// <param name="settingVo"></param>
    private void SettingUpdateDone (SettingVo settingVo)
    {
        Setting = settingVo;
        CheckResPackageVer ();
    }

    #endregion

    #region 第二步，进行版本号对比，判断是否需要更新

    /// <summary>
    /// 检查资源包版本,判断是否需要更新资源
    /// </summary>
    private void CheckResPackageVer ()
    {
        new ResPackageVerChecker ().Start ().Then ((isNeedUpdateRes) =>
        {
            if ( isNeedUpdateRes )
            {
                ResVerFileUpdate ();
                return;
            }
            PreloadDone ();
        });
    }

    /// <summary>
    /// 资源版本文件更新
    /// </summary>
    private void ResVerFileUpdate ()
    {
        Promise promise = new ResVerFileUpdate ().Start ();
        promise.Then (StartupResUpdate);
        promise.Catch (OnUpdateError);
    }

    #endregion

    #region 第三步，实际的下载逻辑

    bool _isUpdate = false;

    /// <summary>
    /// 更新初始化所需资源
    /// </summary>
    private void StartupResUpdate ()
    {
        var promise = new ResUpdate (true).Start (Setting.startupResGroups , OnProgressChange);
        promise.Then (OnFirstResPackageUpdateComplete);
        promise.Catch (OnUpdateError);
    }

    /// <summary>
    /// 首个资源包更新完成
    /// </summary>
    /// <param name="isUpdate"></param>
    private void OnFirstResPackageUpdateComplete (bool isUpdate)
    {
        _isUpdate = !_isUpdate ? isUpdate : _isUpdate;
        //如果存在有效的最新资源包，进行下载
        if ( Setting.IsUsefulResVer () )
        {
            var promise = new ResUpdate (false).Start (Setting.startupResGroups , OnProgressChange);
            promise.Then (OnSecondResPackageUpdateComplete);
            promise.Catch (OnUpdateError);
            return;
        }
        CheckResHash ();
    }

    /// <summary>
    /// 次包（最新包）更新完成
    /// </summary>
    /// <param name="isUpdate"></param>
    private void OnSecondResPackageUpdateComplete (bool isUpdate)
    {
        _isUpdate = !_isUpdate ? isUpdate : _isUpdate;
        CheckResHash ();
    }

    /// <summary>
    /// 检查资源是否合法
    /// </summary>
    private void CheckResHash ()
    {
        if ( _isUpdate )
        {
            if ( !LocalResVer.CheckHash () )
            {
                OnReUpdate ();
                return;
            }
            PreloadDone ();
        }
        else
        {
            PreloadDone ();
        }
    }

    /// <summary>
    /// 预载完成
    /// </summary>
    public void PreloadDone ()
    {
        ResMgr.Ins.Init ();
        _preloadPromise.Resolve ();
    }

    /// <summary>
    /// 进度刷新
    /// </summary>
    /// <param name="process"></param>
    /// <param name="total"></param>
    public void OnProgressChange (float process , long total)
    {

    }

    #endregion

    #region 更新辅助方法

    int _loadFailReCount;
    const int LOAD_FAIL_RE_MAX_COUNT = 5;

    /// <summary>
    /// 更新失败
    /// </summary>
    /// <param name="e"></param>
    private void OnUpdateError (Exception e)
    {
        //如果允许重试，则重新开始下载
        if ( _loadFailReCount <= LOAD_FAIL_RE_MAX_COUNT )
        {
            _loadFailReCount++;
            OnReUpdate ();
            return;
        }
        throw e;
    }

    /// <summary>
    /// 重新更新
    /// </summary>
    private void OnReUpdate ()
    {
        //重新加载一次本地版本描述文件
        LocalResVer.Load ();

        //重新拉取Setting文件
        UpdateSettingFile ();
    }

    #endregion

    #endregion
}