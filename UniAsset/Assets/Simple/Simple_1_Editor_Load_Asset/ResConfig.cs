using UniAsset;
using UnityEngine;

/// <summary>
/// 资源加载的配置，预先设置好，也可以采取其它方式来配置，只要将数据配好能传给UniAsset进行初始化就行
/// </summary>
public class ResConfig : ASingletonMonoBehaviour<ResConfig>
{
    public ResLoadMode resLoadMode;
    public string netPath;
}