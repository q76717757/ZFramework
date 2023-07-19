using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Compilation;
using System.IO;
using System.Text;
using System.Globalization;

namespace ZFramework.Editor
{
    [CustomEditor(typeof(MonoScript))]
    internal class MonoScriptInspector : UnityEditor.Editor
    {
        private const int MAX_CHARS = 7000;
        private GUIStyle textStyle;
        private GUIContent textContent;

        Encoding encoding;

        public override void OnInspectorGUI()
        {
            var assetPath = AssetDatabase.GetAssetPath(target);
            var assemblyName = CompilationPipeline.GetAssemblyNameFromScriptPath(assetPath);
            // assemblyName is null for MonoScript's inside assemblies.
            if (assemblyName != null)
            {
                GUILayout.Label("Assembly Information", EditorStyles.boldLabel);

                EditorGUILayout.LabelField("Filename", assemblyName);

                var assemblyDefinitionFile = CompilationPipeline.GetAssemblyDefinitionFilePathFromScriptPath(assetPath);

                if (assemblyDefinitionFile != null)
                {
                    var assemblyDefintionFileAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assemblyDefinitionFile);

                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUILayout.ObjectField("Definition File", assemblyDefintionFileAsset, typeof(TextAsset), false);
                    }
                }
            }
            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                if (encoding == null)
                {
                    encoding = GetFileEncoding(assetPath);
                }
                EditorGUILayout.LabelField("文件编码:" + encoding.EncodingName);

                EditorGUI.BeginDisabledGroup(encoding.CodePage == Encoding.UTF8.CodePage);
                if (GUILayout.Button("转换为UTF8"))
                {
                    ConvertToUTF8(assetPath);
                }
                EditorGUI.EndDisabledGroup();
            }

            
            EditorGUILayout.Space();

            TextAsset textAsset = target as TextAsset;
            if (textAsset != null)
            {
                bool enabledTemp = GUI.enabled;
                GUI.enabled = true;

                string text = string.Empty;
                if (Path.GetExtension(AssetDatabase.GetAssetPath(textAsset)) != ".bytes")
                {
                    text = textAsset.text;
                    if (text.Length > MAX_CHARS)
                        text = text.Substring(0, MAX_CHARS) + "...\n\n<...etc...>";
                }

                if (textContent == null)
                {
                    textContent = new GUIContent();
                    textContent.image = null;
                    textContent.tooltip = null;
                }
                if (textStyle == null)
                {
                    textStyle = new GUIStyle("ScriptText");
                }

                textContent.text = text;
                Rect rect = GUILayoutUtility.GetRect(textContent, textStyle);
                rect.width += rect.x;
                rect.x = 0;
                GUI.Box(rect, text, textStyle);

                GUI.enabled = enabledTemp;
            }
        }

        private Encoding GetFileEncoding(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                Encoding output;
                byte[] buffer = new byte[4];
                fs.Read(buffer, 0, 4);

                //带bom
                //UTF8    --> EF BB BF
                //UTF16LE --> FF FE
                //UTF16BE --> FE FF
                //UTF32LE --> FF FE 00 00
                //UTF32BE --> 00 00 FE FF
                if (buffer[0] >= 0xEF)
                {
                    if (buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF)
                    {
                        output = Encoding.UTF8;
                    }
                    else if (buffer[0] == 0xFE && buffer[1] == 0xFF)
                    {
                        output = Encoding.BigEndianUnicode;
                    }
                    else if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                    {
                        output = Encoding.Unicode;
                    }
                    else
                    {
                        output = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.ANSICodePage);
                    }
                    //
                }
                else//不带bom
                {
                    fs.Position = 0;
                    if (IsUTF8Bytes(fs))//UTF8
                    {
                        output = Encoding.UTF8;
                    }
                    else//ANSI
                    {
                        output = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.ANSICodePage);
                    }
                }
                return output;
            }
        }

        private bool IsUTF8Bytes(FileStream stream)
        {
            //UTF8编码规则
            //1字节 0xxxxxxx
            //2字节 110xxxxx 10xxxxxx
            //3字节 1110xxxx 10xxxxxx 10xxxxxx
            //4字节 11110xxx 10xxxxxx 10xxxxxx 10xxxxxx
            //5字节 111110xx 10xxxxxx 10xxxxxx 10xxxxxx 10xxxxxx
            //6字节 1111110x 10xxxxxx 10xxxxxx 10xxxxxx 10xxxxxx 10xxxxxx

            byte[] bytes = new byte[1];
            while (stream.Read(bytes, 0, 1) > 0)
            {
                //单字节    ASCII
                if (bytes[0] < 0b1000_0000)
                    continue;

                //多字节
                int cnt = 0;
                byte b = bytes[0]; //判断首字节1的个数
                while ((b & 0b1000_0000) == 0b1000_0000)
                {
                    cnt++;
                    b <<= 1;
                }

                for (int i = 1; i < cnt; i++)//除首字节外的后续字节
                {
                    if (stream.Read(bytes, 0, 1) <= 0)//后续字节不够 不满足UTF8编码
                        return false;
                    if ((bytes[0] & 0b1100_0000) != 0b1000_0000)//后续字节的头两位不是10  不满足UTF8编码
                        return false;
                }
            }
            return true;
        }

        private void ConvertToUTF8(string assetPath)
        {
            if (encoding == null || encoding.CodePage == Encoding.UTF8.CodePage)
            {
                return;
            }
            var bytes = File.ReadAllBytes(assetPath);
            string s = encoding.GetString(bytes);
            var utf8Bytes= Encoding.UTF8.GetBytes(s);
            File.WriteAllBytes(assetPath, utf8Bytes);

            AssetDatabase.Refresh();
        }
    }
}
