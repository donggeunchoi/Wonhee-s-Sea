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

    public void FirstInit(Sprite _sprite, int _count)
    {
        if(fishicon != null)
            fishicon.sprite = _sprite;

        if(countText != null)
            countText.text = _count.ToString();
    }
}
