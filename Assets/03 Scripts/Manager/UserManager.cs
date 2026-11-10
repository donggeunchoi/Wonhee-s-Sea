using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class UserManager : Singleton<UserManager>
{
   public PuserInfo userInfo = new PuserInfo();
   public PDungeonInfo dungeonInfo = new PDungeonInfo();
    public PEquipInfo equipInfo = new PEquipInfo();
   

    public Subject<int> OnFishID = new Subject<int>();
    public List<int> dungeonSpawnFishIds = new List<int>();

    //유저 닉네임
    public string userNickName = "";

    // 유저 indate
    public string userIndate = "";

    // 유저 게이머 아이디
    public string userGamerID = "";

    public bool AddItem( EItemType itemType, int itemID, int count )
    {
        if( itemID <= 0 || count <= 0 )
            return false;

        string key = "D" + itemID;

        switch( itemType )
        {
            case EItemType.Fish:
            {
                if( userInfo == null )
                    return false;

                if( userInfo.fishContainer == null )
                    userInfo.fishContainer = new Dictionary<string, int>();

                userInfo.fishContainer.TryGetValue( key, out int currentCount );
                if( currentCount < 0 || currentCount > int.MaxValue - count )
                    return false;

                userInfo.AddFish( itemID.ToString(), count );
                return true;
            }
            case EItemType.Ingredient:
            {
                if( userInfo == null )
                    return false;

                if( userInfo.ingredientContainer == null )
                    userInfo.ingredientContainer = new Dictionary<string, int>();

                userInfo.ingredientContainer.TryGetValue( key, out int currentCount );
                if( currentCount < 0 || currentCount > int.MaxValue - count )
                    return false;

                userInfo.ingredientContainer[ key ] = currentCount + count;
                return true;
            }
            case EItemType.Weapon:
            {
                // 장비는 수량 대신 보유 여부와 레벨을 관리한다.
                if( equipInfo == null || count != 1 )
                    return false;

                if( equipInfo.WeaponLevelData == null )
                    equipInfo.WeaponLevelData = new Dictionary<string, int>();

                if( equipInfo.WeaponLevelData.TryGetValue( key, out int level ) && level >= 0 )
                    return false;

                equipInfo.WeaponLevelData[ key ] = 0;
                return true;
            }
            case EItemType.Employee:
            {
                if( equipInfo == null )
                    return false;

                if( equipInfo.EmployeeLevelData == null )
                    equipInfo.EmployeeLevelData = new Dictionary<string, int>();

                if( equipInfo.haveEmploies == null )
                    equipInfo.haveEmploies = new Dictionary<string, int>();

                equipInfo.haveEmploies.TryGetValue( key, out int currentCount );
                if( currentCount < 0 || currentCount > int.MaxValue - count )
                    return false;

                if( equipInfo.EmployeeLevelData.TryGetValue( key, out int level ) == false || level < 0 )
                    equipInfo.EmployeeLevelData[ key ] = 0;

                equipInfo.haveEmploies[ key ] = currentCount + count;
                return true;
            }
            case EItemType.Skin:
            {
                // 스킨은 중복 수량 없이 보유 ID만 등록한다.
                if( equipInfo == null || count != 1 )
                    return false;

                if( equipInfo.SkinEquipData == null )
                    equipInfo.SkinEquipData = new List<int>();

                if( equipInfo.SkinEquipData.Contains( itemID ) )
                    return false;

                equipInfo.SkinEquipData.Add( itemID );
                return true;
            }
        }

        return false;
    }

    public bool ConsumeGood( EGoodType goodType, int cost )
    {
        if( userInfo == null || cost < 0 )
            return false;

        switch( goodType )
        {
            case EGoodType.Gold:
            {
                if( userInfo.gold < cost )
                    return false;

                userInfo.gold -= cost;
                return true;
            }
            case EGoodType.Dia:
            {
                if( userInfo.dia < cost )
                    return false;

                userInfo.dia -= cost;
                return true;
            }
            case EGoodType.Enchant:
            {
                if( userInfo.enchant < cost )
                    return false;

                userInfo.enchant -= cost;
                return true;
            }
        }

        return false;
    }

    public void SetDungeonSpawnFishIdsFromPlacedStands()
    {
        dungeonSpawnFishIds.Clear();

        HashSet<int> placedStandIds = GetPlacedContentStandIds();
        if (placedStandIds.Count == 0)
            return;

        List<StandRecipeData> recipeDatas = JsonManager.Ins.GetJson<StandRecipeData>();
        List<StandRecipeInputData> inputDatas = JsonManager.Ins.GetJson<StandRecipeInputData>();

        if (recipeDatas == null || inputDatas == null)
            return;

        HashSet<int> fishIds = new HashSet<int>();

        for (int i = 0; i < recipeDatas.Count; i++)
        {
            StandRecipeData recipe = recipeDatas[i];
            if (recipe == null || !placedStandIds.Contains(recipe.standId))
                continue;

            for (int j = 0; j < inputDatas.Count; j++)
            {
                StandRecipeInputData input = inputDatas[j];
                if (input == null || input.recipeId != recipe.recipeId)
                    continue;

                AddFishInputId(input.inputItemId1, fishIds);
                AddFishInputId(input.inputItemId2, fishIds);
            }
        }

        dungeonSpawnFishIds.AddRange(fishIds);
    }

    private HashSet<int> GetPlacedContentStandIds()
    {
        HashSet<int> standIds = new HashSet<int>();

        CTStandContent[] ctStands = FindObjectsOfType<CTStandContent>();
        for (int i = 0; i < ctStands.Length; i++)
        {
            if (ctStands[i] != null)
                standIds.Add(ctStands[i].StandID);
        }

        CBStandContent[] cbStands = FindObjectsOfType<CBStandContent>();
        for (int i = 0; i < cbStands.Length; i++)
        {
            if (cbStands[i] != null)
                standIds.Add(cbStands[i].StandID);
        }

        return standIds;
    }

    private void AddFishInputId(int itemId, HashSet<int> fishIds)
    {
        if (itemId <= 0)
            return;

        ItemDataTable itemData = JsonManager.Ins.GetDataByKey<ItemDataTable>(
            itemId,
            item => item.itemId
        );

        if (itemData == null || itemData.itemType1 != (int)EItemType.Fish)
            return;

        fishIds.Add(itemId);
    }



}
