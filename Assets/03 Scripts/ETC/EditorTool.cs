using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class EditorTool : EditorWindow
{
    [MenuItem("Tools/GrakungTool")]
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
            if (GUILayout.Button("경영씬"))
            {
                if (EditorSceneManager.GetActiveScene().isDirty)
                {
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                }
                EditorSceneManager.OpenScene("Assets/02 Scenes/TownScene.unity");
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

        GUILayout.Label("\n팝업 프리팹 제작하는 장소", EditorStyles.boldLabel);
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Space(3f);
            if (GUILayout.Button("프리팹제작 (클라)"))
            {
                if (EditorSceneManager.GetActiveScene().isDirty)
                {
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                }
                EditorSceneManager.OpenScene("Assets/Scenes/PopupBoxCreate1.unity");
            }
            GUILayout.Space(3f);

            if (GUILayout.Button("프리팹제작 (원화)"))
            {
                if (EditorSceneManager.GetActiveScene().isDirty)
                {
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                }
                EditorSceneManager.OpenScene("Assets/Scenes/PopupBoxCreate2.unity");
            }
            GUILayout.Space(3f);
        }

        //    GUILayout.Label("\n데이터 삭제", EditorStyles.boldLabel);
        //    using (new EditorGUILayout.HorizontalScope())
        //    {
        //        GUILayout.Space(3f);
        //        if (GUILayout.Button("메타데이터(차트) 삭제하기"))
        //        {
        //            bool deletePop = EditorUtility.DisplayDialog("알림", "메타데이터를 삭제하시겠습니까? 삭제하는 순간 서버에서 무조건 다시 다운받습니다.", "데이터 삭제하기", "취소");
        //            if (deletePop == true)
        //            {
        //                PlayerPrefs.DeleteKey("MetaVersion");
        //                EditorUtility.DisplayDialog("알림", "메타데이터 삭제 완료", "확인");
        //            }
        //        }
        //        GUILayout.Space(3f);
        //    }
        //    using (new EditorGUILayout.HorizontalScope())
        //    {
        //        GUILayout.Space(3f);
        //        if (GUILayout.Button("모든 게임 데이터 삭제하기(로그인, 환경설정 등)"))
        //        {
        //            bool deletePop = EditorUtility.DisplayDialog("알림", "정말로 삭제하시겠습니까?\n게스트 로그인일 경우 아이디를 찾지 못할 수도 있습니다.", "데이터 삭제하기", "취소");
        //            if (deletePop == true)
        //            {
        //                PlayerPrefs.DeleteAll();
        //                EditorUtility.DisplayDialog("알림", "모든 데이터 삭제 완료", "확인");
        //            }
        //        }
        //        GUILayout.Space(3f);
        //    }

        //    GUILayout.Label("\n게임화면 캡처", EditorStyles.boldLabel);
        //    using (new EditorGUILayout.HorizontalScope())
        //    {
        //        GUILayout.Space(3f);
        //        // 저장 경로 설정
        //        if (GUILayout.Button("저장 경로 설정"))
        //        {
        //            // 폴더 브라우저 열기
        //            string path = EditorUtility.OpenFolderPanel("저장 경로를 선택하세요", screenshotPath, "");
        //            if (!string.IsNullOrEmpty(path))
        //            {
        //                screenshotPath = path;
        //            }
        //        }

        //        GUILayout.Space(3f);

        //    }
        //    GUILayout.Space(3f);
        //    using (new EditorGUILayout.HorizontalScope())
        //    {
        //        // 해상도 배율 설정
        //        GUILayout.Label("Save Path: " + screenshotPath);
        //    }

        //    GUILayout.Space(3f);
        //    using (new EditorGUILayout.HorizontalScope())
        //    {
        //        // 해상도 배율 설정
        //        scale = EditorGUILayout.IntSlider("Screenshot Scale", scale, 1, 4);
        //    }

        //    GUILayout.Space(3f);
        //    using (new EditorGUILayout.HorizontalScope())
        //    {
        //        // 캡처 버튼
        //        if (GUILayout.Button("게임화면 캡처"))
        //        {
        //            TakeScreenshot();
        //        }
        //    }

        //    GUILayout.Space(3f);
        //}

        //private void TakeScreenshot()
        //{
        //    // 파일명에 타임스탬프 추가
        //    string fileName = "Screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        //    string fullPath = System.IO.Path.Combine(screenshotPath, fileName);

        //    // 스크린샷 찍기
        //    ScreenCapture.CaptureScreenshot(fullPath, scale);
        //    Debug.Log("Screenshot saved to: " + fullPath);
        //}
    }
}
