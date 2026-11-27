using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : Singleton<SpriteManager>
{
    [Header("골드")]
    [SerializeField]
    Sprite goldIcon;

    [Header("다이아")]
    [SerializeField]
    Sprite diaIcon;

    [Header("강화석")]
    [SerializeField]
    Sprite enchantIcon;

    string iconPath = "Art/";

   public Sprite GetIcon(string _id,EStoreType _storeType )
    {
        switch (_storeType)
        {
            case EStoreType.Equipment:
            {
                    return Resources.Load<Sprite>($"{iconPath}Weapon/WP{_id}");
            }

            case EStoreType.Skin:
                {
                    return Resources.Load<Sprite>($"{iconPath}Skin/SK_{_id}");
                }

            case EStoreType.Employee:
                {
                    return Resources.Load<Sprite>($"{iconPath}Employee/EM_{_id}");
                }

            default:
                return null;
        }
    }

    public Sprite OnGetSprite(EItemType _itemType)
    {
        switch (_itemType)
        {
            case EItemType.Gold:
                {
                    return goldIcon;
                }
                
            case EItemType.Dia:
                {
                    return diaIcon;
                }
                
            case EItemType.Enchant:
                {
                    return enchantIcon;
                }

                default: return null;
        }
    }
}
