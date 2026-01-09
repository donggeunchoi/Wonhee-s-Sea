using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invar : MonoBehaviour
{
}

    [Serializable]
    public class UserInfo
    {
        public int level;
        public int exp;
        public int gold;
        
    }

    [Serializable]
    public class StatInfo
    {
        //던전에서만 사용하는 변수
        public int HP;
        public int Speed;
        public int Attack;
        public int HPRegen;
        public int Oxygen;
        //타운에서만 사용하는 변수
        
    }

