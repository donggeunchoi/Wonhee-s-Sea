using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishSlotUI : MonoBehaviour
{
    [Header("아이콘")]
    [SerializeField]
    Image fishicon;

    [Header("개수")]
    [SerializeField]
    TextMeshProUGUI countText;
    Action<string> actID = null;
    string id = "";

    public void FirstInit(Sprite _sprite,string _id, int _count, Action<string> _actID)
    {
        if(fishicon != null)
            fishicon.sprite = _sprite;

        if(countText != null)
            countText.text = _count.ToString();

        actID = _actID;
        id = _id;
    }

    public void OnClickRecive()
    {
        actID?.Invoke(id);
    }
}
