using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnBtn : MonoBehaviour
{
    // 임시
    [SerializeField] private PlayerInventory _playerInventory;

    public void OnClickExit()
    {
        if (_playerInventory != null)
        {
            string data = string.Join(",", _playerInventory.FishList);

            BackendManager.Ins.OnTossFish.OnNext(data);
        }

        SceneManager.LoadScene("TownScene_Stage1");
    }
}
