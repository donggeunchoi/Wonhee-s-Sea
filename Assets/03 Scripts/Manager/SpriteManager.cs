using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : Singleton<SpriteManager>
{
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
}
