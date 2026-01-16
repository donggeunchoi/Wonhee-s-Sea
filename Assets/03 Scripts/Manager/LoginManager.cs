using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public void OnClickLogin()
    {
        Backend.BMember.GuestLogin("게스트 로그인으로 로그인함", (callback) => {
            if (callback.IsSuccess())
            {
                SceneManager.LoadScene("Dong_TownScene");
                Debug.Log("게스트 로그인에 성공했습니다");
            }
        });
    }
}
