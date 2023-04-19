## 构建资源包
* 通过点击顶部菜单<b>UniAsset/Build AssetBundle</b>来构建资源包,基于SBP来构建，支持仅构建发生了变化的资源。
* <b>打包粒度是根据UnityResource目录来进行划分</b>，处于同一个目录的资源是同一个AB包，目录划分合理打出来的AB粒度就是合理的。当然如果觉得不灵活这部分也可以改下，代码入口EditorMenu.cs
![image](https://user-images.githubusercontent.com/38308449/230401965-a0c55f2e-f764-42c5-8b98-a74449c4e122.png)

## 支持多种加载器
* AssetDataBase: 仅支持在编辑器下使用，优点是不用构建资源包就可以加载，用于开发期间的快速验证
* AssetBundle: 实际运行环境下使用，需要先构建出资源包

## 支持资源内存管理
大致的管理流程如下，采用了引用计数的方式来进行释放管理
![Reducer](https://user-images.githubusercontent.com/38308449/233108110-af3eb443-34f0-426c-ba0a-c60c0362c403.jpg)

## 其它功能实现
* 支持远程下载资源包（下载-校验-加载）
* 支持本地内嵌资源包（单机游戏）

## 使用案例
