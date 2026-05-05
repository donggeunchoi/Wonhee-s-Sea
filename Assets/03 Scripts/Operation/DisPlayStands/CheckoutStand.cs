using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckoutStand : MonoBehaviour
{
    public void Pay(string itemId, int amount)
    {
        UserManager.Ins.userInfo.AddGold(amount);
        //경험치와 골드의 구분이 있을테니 나중에 수정할 필요가 있음.

        //경험치가 오르는것은 퀘스트를 끝내야 오르는 경우니 이 코드는 필요없는 코드.
        //다만 퀘스트가 클리어 되고있는 걸 확인할 함수가 필요할 부분이 필요 컬렉션벨류 이런걸로?
        //UserManager.Ins.userInfo.AddExp(amount);

    }
}
