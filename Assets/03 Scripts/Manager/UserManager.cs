using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class UserManager : Singleton<UserManager>
{
   public PuserInfo userInfo = new PuserInfo();
   public PDungeonInfo dungeonInfo = new PDungeonInfo();
   

    public Subject<int> OnFishID = new Subject<int>();
    public List<int> dungeonSpawnFishIds = new List<int>();

    //유저 닉네임
    public string userNickName = "";

    // 유저 indate
    public string userIndate = "";

    // 유저 게이머 아이디
    public string userGamerID = "";

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
