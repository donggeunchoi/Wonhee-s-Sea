using BackEnd;
using BackEnd.BackndNewtonsoft.Json;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//IsDifferent는 해당 값이 다른지 같은지 판단하기위해서 만든겁니다.
//변경된 값만 서버에 전송하거나 할때 사용합니다.

//FormatJsonData는 뒤끝 서버에 저장할때 JSON형태로 변환하기 위해 만든겁니다.

[Serializable]
public class PUserInfo
{
    public int level = 1;
    public int exp;
    public int gold;
    public int maxCarryCount = 5;

    //키값 : 물고기 ID, 값 : 보유 수량 
    public Dictionary<string, int> fishStorages = new Dictionary<string, int>();

    //물고기 딕셔너리 추가
    public void AddFish(string fishID, int count)
    {
        if (string.IsNullOrEmpty(fishID))
            return;

        if (count <= 0)
            return;

        if (fishStorages.ContainsKey(fishID))
            fishStorages[fishID] += count;
        else
            fishStorages[fishID] = count;
    }

    //물고기 수량 조회
    public int GetFishCount(string fishID)
    {
        if (string.IsNullOrEmpty(fishID))
            return 0;

        if (fishStorages == null)
            return 0;

        if (fishStorages.TryGetValue(fishID, out int count))
            return count;

        return 0;
    }

    // 물고기 제거
    public bool RemoveFish(string fishID, int count)
    {
        if (string.IsNullOrEmpty(fishID))
            return false;

        if (count <= 0)
            return false;

        if (fishStorages == null)
            return false;

        if (!fishStorages.TryGetValue(fishID, out int currentCount))
            return false;

        if (currentCount < count)
            return false;

        currentCount -= count;

        if (currentCount <= 0)
            fishStorages.Remove(fishID);
        else
            fishStorages[fishID] = currentCount;

        return true;
    }


    public bool IsDifferent(PUserInfo other)
    {
        if (other == null) return true;

        if (level != other.level) return true;
        if (gold != other.gold) return true;
        if (exp != other.exp) return true;
        if (maxCarryCount != other.maxCarryCount) return true;

        // fishStorage 비교
        if (fishStorages == null && other.fishStorages == null)
            return false;

        if (fishStorages == null || other.fishStorages == null)
            return true;

        if (fishStorages.Count != other.fishStorages.Count)
            return true;

        foreach (var pair in fishStorages)
        {
            if (!other.fishStorages.TryGetValue(pair.Key, out int otherValue))
                return true;

            if (pair.Value != otherValue)
                return true;
        }

        return false;
    }

    public Param FormatJsonData()
    {
        Param p = new Param();
        p.Add("level", level);
        p.Add("exp", exp);
        p.Add("gold", gold);
        p.Add("maxCarryCount", maxCarryCount);

        // 딕셔너리 저장
        p.Add("fishStorage", JsonConvert.SerializeObject(fishStorages));

        return p;
    }
    public void SyncFromJson()
    {
        if (fishStorages == null)
            fishStorages = new Dictionary<string, int>();
    }
}


[Serializable]
public class PDungeonInfo
{
    //던전에서만 사용하는 변수
    public int HP = 100;
    public int Speed = 0;
    public int Attack = 10;
    public int HPRegen = 0;
    public int Oxygen = 0;

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
public class PBuildingState
{
    public int buildingId;
    public int level;
    public bool isUnlocked;
    public bool isOperating;
    public int staffId;        // 배치 직원
}

[Serializable]
public class  PBuildingSaveData
{
    public List<PBuildingState> buildings = new List<PBuildingState>();

    public string buildingsJson = "[]";

    public void SyncToJson()
    {
        buildingsJson = JsonConvert.SerializeObject(buildings);
    }

    public void SyncFromJson()
    {
        if (string.IsNullOrEmpty(buildingsJson))
            buildingsJson = "[]";

        try
        {
            buildings = JsonConvert.DeserializeObject<List<PBuildingState>>(buildingsJson);
            if (buildings == null) buildings = new List<PBuildingState>();
        }
        catch
        {
            buildings = new List<PBuildingState>();
            buildingsJson = "[]";
        }
    }

    public bool IsDifferent(PBuildingSaveData other)
    {
        if (other == null) return true;
        if (buildingsJson != other.buildingsJson) return true; //문자열 비교가 제일 깔끔/빠름
        return false;
    }

    public Param FormatJsonData()
    {
        // 저장 직전에 JSON 동기화
        SyncToJson();

        Param p = new Param();
        p.Add("buildingsJson", buildingsJson);
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