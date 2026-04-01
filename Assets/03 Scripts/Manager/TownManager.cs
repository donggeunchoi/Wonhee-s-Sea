using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TownManager : MonoBehaviour
{
    //[Header(" 스텐드 ")]
    //[SerializeField]
    



    public void OnClickDungeonScene()
    {
        SceneManager.LoadScene("Gwang_DungeonScene");
    }
}
