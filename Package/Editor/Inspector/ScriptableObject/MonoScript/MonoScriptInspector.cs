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
                EditorGUILayout.LabelField("�ļ�����:" + encoding.EncodingName);
                if (GUILayout.Button("ת��ΪUTF8"))
                {
                    ConvertToUTF8(assetPath);
                }
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
            Encoding output;

            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4];
            fs.Read(buffer, 0, 4);

            //��bom
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
            }
            else//����bom
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
            fs.Close();
            return output;
        }

        private bool IsUTF8Bytes(FileStream stream)
        {
            //UTF8�������
            //1�ֽ� 0xxxxxxx   
            //2�ֽ� 110xxxxx 10xxxxxx
            //3�ֽ� 1110xxxx 10xxxxxx 10xxxxxx
            //4�ֽ� 11110xxx 10xxxxxx 10xxxxxx 10xxxxxx
            //5�ֽ� 111110xx 10xxxxxx 10xxxxxx 10xxxxxx 10xxxxxx
            //6�ֽ� 1111110x 10xxxxxx 10xxxxxx 10xxxxxx 10xxxxxx 10xxxxxx

            byte[] bytes = new byte[1];
            while (stream.Read(bytes, 0, 1) > 0)
            {
                //���ֽ�    ASCII
                if (bytes[0] < 0b1000_0000)
                    continue;

                //���ֽ�
                int cnt = 0;
                byte b = bytes[0]; //�ж����ֽ�1�ĸ���
                while ((b & 0b1000_0000) == 0b1000_0000)
                {
                    cnt++;
                    b <<= 1;
                }
                cnt -= 1;//��ȥ���ֽ�  �õ������ֽڵĸ���

                for (int i = 0; i < cnt; i++)
                {
                    if (stream.Read(bytes, 0, 1) <= 0)//�����ֽڲ��� ������UTF8����
                        return false;
                    if ((bytes[0] & 0b1100_0000) != 0b1000_0000)//�����ֽڵ�ͷ��λ����10  ������UTF8����
                        return false;
                }
            }
            return true;
        }

        private void ConvertToUTF8(string assetPath)
        {
            if (encoding == null || encoding == Encoding.UTF8)
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
