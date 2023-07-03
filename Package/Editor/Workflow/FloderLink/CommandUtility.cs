using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace ZFramework.Editor
{
    public static class CommandUtility
    {
        /// <summary>
        /// 调用cmd命令,返回控制台的输出结果
        /// </summary>
        /// <param name="commands">一行或多行命令</param>
        /// <returns></returns>
        public static string Start(string[] commands)
        {
            if (commands == null || commands.Length == 0)
            {
                return string.Empty;
            }
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    FileName = "cmd.exe",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.ANSICodePage),
                };

                Process process = Process.Start(startInfo);
                for (int i = 0; i < commands.Length; i++)
                {
                    if (i != commands.Length - 1)
                    {
                        process.StandardInput.WriteLine(commands[i]);
                    }
                    else
                    {
                        //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令
                        //如果不执行exit命令，后面调用ReadToEnd()方法会假死
                        //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令
                        process.StandardInput.WriteLine($"{commands[i]}&exit");
                    }
                }
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();
                return output;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
                return string.Empty;
            }
        }
    }
}
