using System.Collections.Generic;
using System.IO;
using System.Text;
using UniAsset;
using UnityEditor;
using UnityEngine;

namespace UniAssetEditor
{
    public struct AssetBundleItemVo
    {
        public string explain;
        public string assetbundle;
        public string GetFieldName ()
        {
            return assetbundle.Replace ("/" , "_").Replace (".ab" , "").ToUpper ();
        }
    }

#pragma warning disable IDE0051
    public class GenerateAssetBundleNameModule
    {
        const string TEMPLATE_FILE = "Assets/UniAsset/Editor/Config/AssetBundleNameClassTemplate.txt";
        const string OUTPUT_CLASS_FILE = "Assets/UniAsset/Generated/AssetBundleName.cs";
        const string FIELD_EXPLAIN_FORMAT = "\t\t" + @"/// <summary>
        /// {0}
        /// </summary>";
        const string FIELD_FORMAT = "\t\tpublic const string {0} = \"{1}\";";

        public void Start ()
        {
            FindAssetBundles ();
            GeneratedAssetBundleNameClass ();
        }

        void GeneratedAssetBundleNameClass ()
        {
            var dir = Directory.GetParent (OUTPUT_CLASS_FILE);
            if ( false == dir.Exists )
            {
                dir.Create ();
            }

            var template = File.ReadAllText (TEMPLATE_FILE);
            StringBuilder sb = new StringBuilder ();

            foreach ( var vo in abList )
            {
                sb.AppendLine ();
                sb.AppendLine ();
                if ( false == string.IsNullOrEmpty (vo.explain) )
                {
                    sb.AppendLine (string.Format (FIELD_EXPLAIN_FORMAT , vo.explain));
                }
                sb.Append (string.Format (FIELD_FORMAT , vo.GetFieldName () , vo.assetbundle));
            }

            var classContent = template.Replace ("{0}" , sb.ToString ());
            File.WriteAllText (OUTPUT_CLASS_FILE , classContent);
            AssetDatabase.Refresh ();
            Debug.Log ($"生成完毕![{OUTPUT_CLASS_FILE}]");
        }

        public string generatedPath = OUTPUT_CLASS_FILE;
        public List<AssetBundleItemVo> abList;

        /// <summary>
        /// 找出所有要打包的资源
        /// </summary>
        void FindAssetBundles ()
        {
            List<AssetBundleItemVo> list = new List<AssetBundleItemVo> ();
            list.AddRange (GetAssetBundleNameList (UniAssetConst.ASSET_ROOT_DIR));
            abList = list;
        }

        /// <summary>
        /// 获取AB名称列表
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        List<AssetBundleItemVo> GetAssetBundleNameList (string path)
        {
            List<AssetBundleItemVo> list = new List<AssetBundleItemVo> ();
            string [] dirs = Directory.GetDirectories (path , "*" , SearchOption.AllDirectories);
            foreach ( var dir in dirs )
            {
                var sDir = FileSystem.StandardizeBackslashSeparator (dir);
                var di = new DirectoryInfo (sDir);
                if ( di.GetFiles ().Length == 0 )
                {
                    continue;
                }

                string abName = sDir.Substring (path.Length + 1) + UniAssetConst.AB_EXTENSION;

                AssetBundleItemVo vo;
                vo.explain = "";
                vo.assetbundle = abName;
                list.Add (vo);
            }
            return list;
        }
    }
}