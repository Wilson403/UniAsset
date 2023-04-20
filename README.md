## 构建资源包
* 通过点击顶部菜单<b>UniAsset/Build AssetBundle</b>来构建资源包,基于SBP来构建，支持仅构建发生了变化的资源。
* <b>打包粒度是根据UnityResource目录来进行划分</b>，处于同一个目录的资源是同一个AB包，**目录划分合理打出来的AB粒度就是合理的。**当然如果觉得不灵活这部分也可以改下，代码入口EditorMenu.cs
![image](https://user-images.githubusercontent.com/38308449/230401965-a0c55f2e-f764-42c5-8b98-a74449c4e122.png)


---


## 支持多种加载器

* AssetDataBase: 仅支持在编辑器下使用，优点是不用构建资源包就可以加载，用于开发期间的快速验证
* AssetBundle: 实际运行环境下使用，需要先构建出资源包


---


## 支持资源内存管理

由于UnityEngine.Object属于非托管资源，我们需要自己管理资源的释放，大致的管理流程如下，采用了引用计数的方式来进行释放管理
![Reducer](https://user-images.githubusercontent.com/38308449/233108110-af3eb443-34f0-426c-ba0a-c60c0362c403.jpg)

## 其它功能实现
* 支持远程下载资源包（下载-校验-加载）
* 支持本地内嵌资源包（单机游戏）


---


## 部分目录说明

UniAsset/UniAssetTool ： 包含本地文件服务器HFS，用于验证测试下载功能

UniAsset/LibraryUniAsset/EditorConfigs：UniAsset编辑器工具的配置

UniAsset/LibraryUniAsset/Release：UniAsset构建出资源包的正式目录 （不加入版本控制）

UniAsset/LibraryUniAsset/ReleaseCache：UniAsset构建出资源包的缓存目录（不加入版本控制）

UniAsset/LibraryUniAsset/RuntimeCaches：UniAsset运行时产生的缓存文件，如果需要删除缓存数据，删除这个目录即可（不加入版本控制）



UniAsset/UniAsset/Assets/Plugins：依赖的库（Json库，ZIP压缩等）

UniAsset/UniAsset/Assets/Simple：演示案例

UniAsset/UniAsset/Assets/UniAsset：框架核心代码

UniAsset/UniAsset/Assets/UnityResource：资源总目录，构建资源包时就是根据这里面的资源进行划分打包


---


## UniAsset工具栏说明
![image](https://user-images.githubusercontent.com/38308449/233287830-a51bc74a-536c-4ff1-8ce1-e9d68d2450c4.png)
* Build AssetBundle: 构建AseetBundle，点击后需要填写参数，分别是客户端版本和资源包版本。客户端版本就是APP版本，这个跟随Unity PlayerSetting的设置，是只读的。资源包版本自定义设置，含义是给你当前构建出来的资源包一个唯一标识，如1.0
* GenerateAssetBundleName:生成AssetBundle名称变量脚本，用于编码时方便加载路径的参数填入。该命令会在Build AssetBundle命令执行之后自动执行一次
* Setting：Setting.json文件发布设置。Setting.json文件有什么用呢？这个主要是用于远程下载AssetBundle时使用的，用于记录客户端版本当前对应的最新资源包是多少，根据这个参数从CDN拉取对应的资源
* Create ZIP to StreamingAssets：将AssetBundle资源压缩为Zip，用于单机不下载的游戏，一次性将所需资源都放进程序包里


---


## 如果使用

```c#
ResMgr.Ins.Load<GameObject> (AssetBundleName.PREFAB_OTHER_PREFAB_01 , "prefab_01").Instantiate (); //加载并实例化一个GameObject
```

```c#
var assetInfo2 = ResMgr.Ins.Load<Sprite> (AssetBundleName.PREFAB_OTHER_PREFAB_01 , "prefab_01");
Debug.Log (assetInfo2.Asset.texture);
assetInfo2.AddRefCount (); //增加引用
assetInfo2.SubRefCount (); //减少引用
```

```c#
//异步加载
ResMgr.Ins.LoadAsync<GameObject> (AssetBundleName.PREFAB_OTHER_PREFAB_01 , "prefab_01" , (assetInfo) =>
{
    assetInfo.Instantiate (transform);
});
```

* 需要注意的是，**GameObject类型的资源，你无需关心释放，框架会自动管理，只是第一次实例化需要调用框架本身Instantiate ()，之后拷贝可以随意**

* 对于像图片这种引用类型资源，需要手动Add和Sub引用，在那里使用就在那里释放。**当然你可以做多一层的封装来简化代码，拿UI框架举例子，所有界面都继承一个类，这个类分别在创建时Add，销毁时Sub，这样就不用每次都重复相同的细节**


---


## Simple演示说明

* Simple/AssetDataBase : AssetDataBase 演示，这个模式是编辑器下专用的，无法在真机上使用。优点是不用经过构建AB包的过程直接就可以加载，提高开发效率，而且AssetDataBase 的加载效率很低

* Simple/LocalAssetBundle：本地加载AssetBundle，编辑器和真机都可以使用。测试这个场景前，请先执行Build AssetBundle保证资源包生成完成，然后在执行Setting生成Setting.json文件，接下来启动游戏即可。（Setting.json文件不是远程下载才使用的吗，为什么本地加载也需要。本地加载在编辑器模式下需要模拟一部分环境，所以也生成一份，如果是真机本地加载也可以不生成）

* Simple/RemoteAssetBundle：远程加载AssetBundle，加载方式和LocalAssetBundle一样，但是要先把资源从CDN上下载到本地。可以用HFS来搭建本地文件服务器来模拟。步骤1：先执行Build AssetBundle保证资源包生成。步骤2：执行Setting生成Setting.json文件，请确保Setting.json的参数包含了你当前构建的资源包版本。步骤3：找到构建出资源包的目录（上面目录说明有），比如UniAsset\UniAsset\LibraryUniAsset\Release\res，把这个路径设置到HFS，然后填入url
![image](https://user-images.githubusercontent.com/38308449/233287978-ee1887a8-6445-497e-8366-de6f76dc0526.png)


---


## 其它的一些说明

* 虽然可以直接使用本框架进行开发，但是还是建议完全理解框架并根据自己的项目进行魔改
* 如果有不对的地方或者建议，可以提IS或者直接邮件联系yiqun2529092370@outlook.com
