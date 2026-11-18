using UnityEngine;

public class ReturnBtn : MonoBehaviour
{
    [SerializeField] private DungeonManager _dungeonManager;

    public void OnClickExit()
    {
        if (_dungeonManager != null)
            _dungeonManager.RequestReturn();
    }
}
