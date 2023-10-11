using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Test
{
	public class 查找高亮文件夹 : EditorWindow
	{
        [MenuItem("Custom Tools/Highlight Folder")]
        public static void ShowWindow()
        {
            GetWindow<查找高亮文件夹>("Folder Highlighter");
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Highlight Folder 1"))
            {
                // 选择文件夹1的路径
                string folderPath = AssetDatabase.GUIDToAssetPath("Assets/Editor");
                Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(folderPath);
                // 使项目窗口获得焦点
                EditorUtility.FocusProjectWindow();
            }

            if (GUILayout.Button("Highlight Folder 2"))
            {
                // 选择文件夹2的路径
                string folderPath = AssetDatabase.GUIDToAssetPath("YourFolder2GUID");
                Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(folderPath);
                // 使项目窗口获得焦点
                EditorUtility.FocusProjectWindow();
            }

            // 在这里添加更多的按钮以选择其他文件夹
        }
    }
}