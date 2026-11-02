using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class EditorTool : EditorWindow
{
    [MenuItem("Tools/DongGeunTool")]
    static void InitWindow()
    {
        EditorTool w = (EditorTool)EditorWindow.GetWindow(typeof(EditorTool));
        w.Show();
    }

    string screenshotPath = "Assets/Screenshots"; // 기본 저장 경로
    int scale = 1; // 기본 스케일 (1 = 원본 해상도)
    private void OnGUI()
    {
        GUILayout.Space(3f);

        GUILayout.Label("\n씬 화면 전환", EditorStyles.boldLabel);
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Space(3f);
            if (GUILayout.Button("로그인씬"))
            {
                if (EditorSceneManager.GetActiveScene().isDirty)
                {
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                }
                EditorSceneManager.OpenScene("Assets/02 Scenes/LoginScene.unity");
            }
            GUILayout.Space(3f);
            if (GUILayout.Button("경영 스테이지1"))
            {
                if (EditorSceneManager.GetActiveScene().isDirty)
                {
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                }
                EditorSceneManager.OpenScene("Assets/02 Scenes/TownScene_Stage1.unity");
            }
            GUILayout.Space(3f);
            if (GUILayout.Button("바다씬"))
            {
                if (EditorSceneManager.GetActiveScene().isDirty)
                {
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                }
                EditorSceneManager.OpenScene("Assets/02 Scenes/Gwang_DungeonScene.unity");
            }
            GUILayout.Space(3f);
        }

        GUILayout.Space(1f);

    }
}
