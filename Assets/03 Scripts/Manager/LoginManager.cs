using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    public void OnClickLogin()
    {
        Backend.BMember.GuestLogin("게스트 로그인으로 로그인함", (callback) => {
            if (callback.IsSuccess())
            {
                Debug.Log("게스트 로그인에 성공했습니다");
            }
        });
    }
}
