using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : Singleton<SpriteManager>
{
    string iconPath = "Art/Weapon/";

   public Sprite GetIcon(string _id,EStoreType _storeType )
    {
        switch (_storeType)
        {
            case EStoreType.Equipment:
            {
                    return Resources.Load<Sprite>($"{iconPath}WP{_id}");
                }

            default:
                return null;
        }
    }
}
