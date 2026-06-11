using System.Collections;
using System.Collections.Generic;
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
            string data = string.Join(",", _playerInventory.FishList);

            //이거 넣어주셔야 사용이 가능합니다.
            //USerManager.Ins.userinfo.AddFish(id - 생선 id,count - 해당id별 잡은 개수);

            BackendManager.Ins.OnTossFish.OnNext(data);
            BackendManager.Ins.SaveData();
        }
        _resultUI.Open();
    }
}
