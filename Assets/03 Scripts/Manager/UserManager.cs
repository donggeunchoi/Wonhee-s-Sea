using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserManager : Singleton<UserManager>
{
   public PUserInfo userInfo = new PUserInfo();
   public PDungeonInfo dungeonInfo = new PDungeonInfo();
   public PTownInfo townInfo = new PTownInfo();
   public PBuildingSaveData buildingSaveData = new PBuildingSaveData();

    //유저 닉네임
    public string userNickName = "";

    // 유저 indate
    public string userIndate = "";

    // 유저 게이머 아이디
    public string userGamerID = "";

}
