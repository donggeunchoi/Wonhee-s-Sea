using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class UserManager : Singleton<UserManager>
{
   public PUserInfo userInfo = new PUserInfo();
   public PDungeonInfo dungeonInfo = new PDungeonInfo();
   public PTownInfo townInfo = new PTownInfo();
   public PBuildingSaveData buildingSaveData = new PBuildingSaveData();


    public Subject<int> OnFishID = new Subject<int>();

    //유저 닉네임
    public string userNickName = "";

    // 유저 indate
    public string userIndate = "";

    // 유저 게이머 아이디
    public string userGamerID = "";




}
