using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

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

    public string ItemId => id;

    public void FirstInit(Sprite _sprite,string _id, int _count, Action<string> _actID)
    {
        gameObject.SetActive(true);
        if (fishicon != null)
            fishicon.sprite = _sprite;

        if(countText != null)
            countText.text = _count.ToString();

        actID = _actID;
        id = _id;

        fishicon.gameObject.SetActive(true);
        countText.gameObject.SetActive(true);
    }

    public void OnClickRecive()
    {
        if (string.IsNullOrEmpty(id) == false)
        {
            Debug.LogWarning("없음");
        }
        else
        {
            Debug.LogWarning("없음2");
        }
            actID?.Invoke(id);
    }
}
