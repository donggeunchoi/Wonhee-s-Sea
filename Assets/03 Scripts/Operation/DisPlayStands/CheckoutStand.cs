using UnityEngine;

public class CheckoutStand : MonoBehaviour
{
    public void Pay(string itemId, int amount)
    {
        if (amount <= 0)
            return;

        if (!TryGetItemPrice(itemId, out int price))
            return;

        int gold = price * amount;
        UserManager.Ins.userInfo.AddGold(gold);
    }

    private bool TryGetItemPrice(string itemId, out int price)
    {
        price = 0;

        if (!int.TryParse(itemId, out int ingredientId))
        {
            Debug.LogWarning($"[CheckoutStand] Invalid itemId: {itemId}", this);
            return false;
        }

        IngredientData data = JsonManager.Ins.GetDataByKey<IngredientData>(
            ingredientId,
            ingredient => ingredient.ingredientId
        );

        if (data == null)
        {
            Debug.LogWarning($"[CheckoutStand] Ingredient price not found. itemId={itemId}", this);
            return false;
        }

        if (data.cost <= 0)
        {
            Debug.LogWarning($"[CheckoutStand] Invalid item cost. itemId={itemId}, cost={data.cost}", this);
            return false;
        }

        price = data.cost;
        return true;
    }
}
