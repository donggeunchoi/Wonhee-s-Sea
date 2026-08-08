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
        _resultUI.Open();
    }
}
