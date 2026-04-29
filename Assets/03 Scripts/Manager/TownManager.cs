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
        SceneManager.LoadScene("Gwang_DungeonScene");
    }
}
