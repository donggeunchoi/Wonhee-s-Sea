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
public class PuserInfo
{
    //경영 플레이어 정보
    public int level = 1;
    public int exp = 0;
    public int gold = 0;
    public int dia = 0;
    public int enchant = 0;
    public int maxCarryCount = 5;
    public int currentStage = 1;

    //던전과 경영에서 공통으로 사용하는 정보
    public Dictionary<string,int> fishContainer = new Dictionary<string, int>();
    public List<PCollectionValue> collectionVelues = new List<PCollectionValue>();

    public void AddGold(int amount)
    {
        if (amount <= 0)
            return;

        gold += amount;
    }

    public void AddExp(int amount)
    {
        if (amount <= 0)
            return;

        exp += amount;

        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        //이 코드도 나중에 변경이 필요한 부분일듯.
        int needExp = level * 100;

        while (exp >= needExp)
        {
            exp -= needExp;
            level++;

            needExp = level * 100;
        }
    }

    public void AddFish(string fishId, int count)
    {
        string madeKey = "D" + fishId;
        if (string.IsNullOrEmpty(madeKey)) return;
        if (count <= 0) return;

        if (fishContainer.ContainsKey(madeKey))
            fishContainer[madeKey] = (int)fishContainer[madeKey] + count;
        else
            fishContainer[madeKey] = count;
    }

    public int GetFishCount(string fishId)
    {
        string madeKey = "D" + fishId;
        if (string.IsNullOrEmpty(madeKey)) return 0;

        if (fishContainer.TryGetValue(madeKey, out int value))
            return System.Convert.ToInt32(value);

        return 0;
    }

    public bool RemoveFish(string fishId, int count)
    {
        string madeKey = "D" + fishId;
        if (string.IsNullOrEmpty(madeKey))
            return false;

        if (count <= 0)
            return false;

        if (fishContainer == null)
            return false;

        if (!fishContainer.TryGetValue(madeKey, out int currentCount))
            return false;

        if (currentCount < count)
            return false;

        currentCount -= count;

        if (currentCount <= 0)
            fishContainer.Remove(madeKey);
        else
            fishContainer[madeKey] = currentCount;

        return true;
    }

    public void SetCollectionValue(ETargetType targetType, EQuestType questType, int targetId, int value)
    {
        if (targetType == ETargetType.None)
            return;

        if (questType == EQuestType.None)
            return;

        if (targetId <= 0)
            return;

        if (value <= 0)
            return;

        if (collectionVelues == null)
            collectionVelues = new List<PCollectionValue>();

        PCollectionValue data = collectionVelues.Find(x =>
        x.targetType == targetType &&
        x.questType == questType &&
        x.targetId == targetId
    );

        if (data != null)
        {
            data.value += value;
        }
        else
        {
            collectionVelues.Add(new PCollectionValue
            {
                targetType = targetType,
                questType = questType,
                targetId = targetId,
                value = value
            });
        }
    }

    public int GetCollectionValue(ETargetType targetType, EQuestType questType, int id)
    {
        if(collectionVelues == null)
            return 0;

        PCollectionValue data = collectionVelues.Find(x =>
        x.targetType == targetType &&
        x.questType == questType &&
        x.targetId == id);

        if (data == null)
            return 0;

        return data.value; 
    }

    public bool IsDifferent(PuserInfo other)
    {
        if (other == null) return true;

        if (level != other.level) return true;
        if (gold != other.gold) return true;
        if (dia != other.dia) return true;
        if (exp != other.exp) return true;
        if (enchant != other.enchant) return true;
        if (maxCarryCount != other.maxCarryCount) return true;
        if(currentStage != other.currentStage) return true;

        if (fishContainer == null && other.fishContainer != null) return true;
        if (fishContainer != null && other.fishContainer == null) return true;

        if (fishContainer != null && other.fishContainer != null)
        {
            if (fishContainer.Count != other.fishContainer.Count)
                return true;

            foreach (var pair in fishContainer)
            {
                if (!other.fishContainer.TryGetValue(pair.Key, out int otherValue))
                    return true;

                if (pair.Value != otherValue)
                    return true;
            }
        }

        if (collectionVelues == null && other.collectionVelues != null) return true;
        if (collectionVelues != null && other.collectionVelues == null) return true;

        if (collectionVelues != null && other.collectionVelues != null)
        {
            if (collectionVelues.Count != other.collectionVelues.Count)
                return true;

            for (int i = 0; i < collectionVelues.Count; i++)
            {
                var a = collectionVelues[i];
                var b = other.collectionVelues[i];

                if (a.targetType != b.targetType) return true;
                if (a.questType != b.questType) return true;
                if (a.targetId != b.targetId) return true;
                if (a.value != b.value) return true;
            }
        }

        return false;
    }

    public Param FormatJsonData()
    {
        Param p = new Param();
        p.Add("level", level);
        p.Add("exp", exp);
        p.Add("gold", gold);
        p.Add("enchant", enchant);
        p.Add("dia", dia);
        p.Add("maxCarryCount", maxCarryCount);
        p.Add("currentStage", currentStage);
        p.Add("fishContainer", fishContainer);//딕셔너리
        p.Add("collectionVelues", collectionVelues);//리스트

        return p;
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

    public void Damaged(int amount)
    {
        if (amount <= 0)
            return;

        Oxygen -= amount;
    }

    public void Heal(int amount)
    {
        if (amount <= 0)
            return;

        HP += amount;
    }
    
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
public class PEquipInfo
{
    public Dictionary<string, int > WeaponLevelData = new Dictionary<string, int>(); 
    //무기레벨 정보 키값 D+id, 벨류 레벨(0이면 구매만 한경우, -1이든 키값도 없으면 구매도 안한 경우로 처리)

    public List<int> SkinEquipData = new List<int>();  
    //스킨을 보윻면 아이디를 숫자로 넣어줌 이렇게 하는 이유는 데이터를 저장할때 딕셔너리가 품이 더들어가서 단순히 숫자만 저장하기위함.

    public Dictionary<string, int> EmployeeLevelData =  new Dictionary<string, int>();
    //직원 레벨 정보, 키값 D+id, 벨류 레벨(0이면 구매만 한경우, -1 혹은 키값 없으면 구매도 안한경우)


    public bool IsDifferent(PDungeonInfo other)
    {
        if (other == null)
            return true;


        return false;
        //return HP != other.HP
        //    || Attack != other.Attack;
    }

    public Param FormatJsonData()
    {
        Param p = new Param();
        p.Add("WeaponLevelData", WeaponLevelData);
        p.Add("SkinEquipData", SkinEquipData);
        p.Add("EmployeeLevelData", EmployeeLevelData);
        return p;
    }

}

[Serializable]
public class PCollectionValue
{
    public ETargetType targetType;
    public EQuestType questType;
    public int targetId;
    public int value;
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