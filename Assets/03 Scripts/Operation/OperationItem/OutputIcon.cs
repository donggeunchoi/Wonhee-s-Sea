using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutputIcon : MonoBehaviour
{
    [Header("결과물 아이콘")]
    [SerializeField]
    Image iconImage;

    public void SetSprite(Sprite sprite)
    {
        if (iconImage != null)
        {
            iconImage.sprite = sprite;
        }
    }
}
