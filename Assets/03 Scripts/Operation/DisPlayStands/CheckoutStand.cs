using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckoutStand : MonoBehaviour
{
    public int money;

    public void Pay(string itemId, int amount)
    {
        // 일단은 단순히 돈만 증가(가격은 DB에서 읽어도 됨)
        money += amount * 1;
        Debug.Log($"결제 완료: {itemId} x{amount}, money={money}");
    }
}
