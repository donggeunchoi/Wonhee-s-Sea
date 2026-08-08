using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnBtn : MonoBehaviour
{
    // 임시
    [SerializeField] private PlayerInventory _playerInventory;
    [SerializeField] private DungeonResultUI _resultUI;

    public void OnClickExit()
    {
        if (_playerInventory != null)
        {
            for(int i = 0; i < _playerInventory.FishList.Count; i++)
            {
                int id = _playerInventory.FishList[i];

                UserManager.Ins.userInfo.AddFish(id.ToString(), 1);
                BackendManager.Ins.OnTossFish.OnNext(id.ToString());
            }

            BackendManager.Ins.SaveData();
        }
        _resultUI.Open();
    }
}
