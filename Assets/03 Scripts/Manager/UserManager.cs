using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class UserManager : Singleton<UserManager>
{
   public PuserInfo userInfo = new PuserInfo();
   public PDungeonInfo dungeonInfo = new PDungeonInfo();
   

    public Subject<int> OnFishID = new Subject<int>();

    //유저 닉네임
    public string userNickName = "";

    // 유저 indate
    public string userIndate = "";

    // 유저 게이머 아이디
    public string userGamerID = "";




}
