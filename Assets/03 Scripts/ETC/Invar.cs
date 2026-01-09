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
        public int HP;
        public int Speed;
        public int Attack;
        public int HPRegen;
        public int Oxygen;
    }

