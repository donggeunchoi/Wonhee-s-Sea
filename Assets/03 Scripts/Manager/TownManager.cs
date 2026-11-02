using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TownManager : Singleton<TownManager>
{

    public void OnClickStore()
    {
        Debug.LogWarning("상점 버튼 눌림");
    }

    public void OnClickTown()
    {
        Debug.LogWarning("상점 버튼 눌림");
    }

    public void OnClickDungeonScene()
    {
        UserManager.Ins.SetDungeonSpawnFishIdsFromPlacedStands();
        SceneManager.LoadScene("Gwang_DungeonScene");
    }
}
