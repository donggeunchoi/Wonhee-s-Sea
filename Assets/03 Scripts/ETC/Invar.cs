using BackEnd;
using BackEnd.BackndNewtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PUserInfo
{
    public int level;
    public int exp;
    public int gold;

    public bool IsDifferent(PUserInfo other)
    {
        if (other == null) return true;

        return level != other.level
            || gold != other.gold;
    }

    public Param FormatJsonData()
    {
        Param p = new Param();
        p.Add("level", level);
        p.Add("exp", exp);
        p.Add("gold", gold);
        return p;
    }

}

[Serializable]
public class PDungeonInfo
{
    //던전에서만 사용하는 변수
    public int HP;
    public int Speed;
    public int Attack;
    public int HPRegen;
    public int Oxygen;

    public bool IsDifferent(PDungeonInfo other)
    {
        if (other == null) return true;

        return HP != other.HP
            || Attack != other.Attack;
    }

    public Param FormatJsonData()
    {
        Param p = new Param();
        p.Add("HP", HP);
        p.Add("Speed", Speed);
        p.Add("Attack", Attack);
        p.Add("HPRegen", HPRegen);
        p.Add("Oxygen", Oxygen);
        return p;
    }

}

[Serializable]
public class PTownInfo
{
    public int townLevel; // 타운 레벨 (해금 기준)
    public int rating; // 평점
    public int reputation;
    public int totalEarnedGold; // 지금까지 벌어들인 골드 총합
    public long todayEarnedGold; // 오늘 벌어들인 골드
    public int customerCount; // 지금까지 방문한 손님 수

    public bool IsDifferent(PTownInfo other)
    {
        if (other == null) return true;

        return townLevel != other.townLevel
            || rating != other.rating;
    }

    public Param FormatJsonData()
    {
        Param p = new Param();
        p.Add("townLevel", townLevel);
        p.Add("rating", rating);
        p.Add("reputation", reputation);
        return p;
    }
}


[Serializable]
public class  PBuildingSaveData
{
    public int buildingId;
    public int level;
    public bool isUnlocked;
    public bool isOperating;
    public int staffId;        // 배치 직원

    public List<int> buildingLevels = new List<int>();
    public bool IsDifferent(PBuildingSaveData other)
    {
        if (other == null) return true;
        if (buildingLevels.Count != other.buildingLevels.Count) return true;

        for (int i = 0; i < buildingLevels.Count; i++)
        {
            if (buildingLevels[i] != other.buildingLevels[i])
                return true;
        }

        return false;
    }

    public Param FormatJsonData()
    {
        Param p = new Param();

        // 안전하게 JSON 문자열로 저장
        string json = JsonConvert.SerializeObject(buildingLevels);
        p.Add("buildingLevels", json);

        return p;
    }
}

public static class CloneUtil
{
    public static T DeepClone<T>(T source)
    {
        if (source == null) return default;
        string json = JsonConvert.SerializeObject(source);
        return JsonConvert.DeserializeObject<T>(json);
    }
}