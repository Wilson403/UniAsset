using System;
using System.IO;
using UnityEngine;

namespace UniAssetEditor
{
    public static class EditorUtil
    {
        /// <summary>
        /// 判断当前字符是否为中文
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsChinese (char c)
        {
            return c >= 0x4E00 && c <= 0x9FA5;
        }

        /// <summary>
        /// 判断当前字符串是否为中文
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsChinese (string str)
        {
            char [] ch = str.ToCharArray ();
            if ( str != null )
            {
                for ( int i = 0 ; i < ch.Length ; i++ )
                {
                    if ( IsChinese (ch [i]) )
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 打开目录
        /// </summary>
        /// <param name="path"></param>
        public static void OpenDirectory (string path)
        {
            if ( string.IsNullOrEmpty (path) ) return;

            if ( Application.platform == RuntimePlatform.WindowsEditor )
            {
                path = path.Replace ("/" , "\\");
                if ( !Directory.Exists (path) )
                {
                    Debug.LogError ("No Directory: " + path);
                    return;
                }

                System.Diagnostics.Process.Start ("explorer.exe" , path);
            }
        }

        /// <summary>
        /// 打开目录
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        public static void OpenDirectory (string path , string fileName)
        {
            if ( string.IsNullOrEmpty (path) ) return;

            if ( Application.platform == RuntimePlatform.WindowsEditor )
            {
                path = path.Replace ("/" , "\\");
                if ( !Directory.Exists (path) )
                {
                    Debug.LogError ("No Directory: " + path);
                    return;
                }

                System.Diagnostics.Process.Start ("Explorer" , "/select," + path + "\\" + fileName);
            }
        }

        /// <summary>
        /// 执行外部进程
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="workingDir"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool Exec (string filename , string workingDir = null , string args = null)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process ();
            process.StartInfo.FileName = filename;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = true;

            if ( !string.IsNullOrEmpty (workingDir) )
            {
                process.StartInfo.WorkingDirectory = workingDir;
            }

            if ( !string.IsNullOrEmpty (args) )
            {
                process.StartInfo.Arguments = args;
            }

            try
            {
                process.Start ();
                if ( process.StartInfo.RedirectStandardOutput && process.StartInfo.RedirectStandardError )
                {
                    process.BeginOutputReadLine ();
                    Debug.LogError (process.StandardError.ReadToEnd ());
                }
                else if ( process.StartInfo.RedirectStandardOutput )
                {
                    string data = process.StandardOutput.ReadToEnd ();
                    Debug.Log (data);
                }
                else if ( process.StartInfo.RedirectStandardError )
                {
                    string data = process.StandardError.ReadToEnd ();
                    Debug.LogError (data);
                }
            }
            catch ( Exception e )
            {
                Debug.LogException (e);
                return false;
            }
            process.WaitForExit ();
            int exit_code = process.ExitCode;
            process.Close ();
            return exit_code == 0;
        }
    }
}